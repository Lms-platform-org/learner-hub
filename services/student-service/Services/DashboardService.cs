using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Repositories;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.StudentService.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly IProgressRepository _progressRepo;
        private readonly IRecommendationService _recommendationService;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IBookmarkRepository bookmarkRepo,
            IProgressRepository progressRepo,
            IRecommendationService recommendationService,
            IEnrollmentRepository enrollmentRepo,
            ILogger<DashboardService> logger)
        {
            _bookmarkRepo = bookmarkRepo;
            _progressRepo = progressRepo;
            _recommendationService = recommendationService;
            _enrollmentRepo = enrollmentRepo;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardAsync(string studentId)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                _logger.LogWarning("Invalid student id provided for dashboard");
                throw new Exception("Invalid student id");
            }

            _logger.LogInformation("Building dashboard for user {UserId}", studentId);

            var bookmarks = await _bookmarkRepo.GetByStudentIdAsync(studentId);
            _logger.LogInformation("Retrieved {BookmarkCount} bookmarks for user {UserId}", bookmarks.Count, studentId);

            var recentBookmarks = bookmarks
                .OrderByDescending(b => b.Id)
                .Take(5)
                .Select(b => new BookmarkDto
                {
                    CourseId = b.CourseId,
                    Category = b.Category,
                    PersonalNote = b.PersonalNote
                })
                .ToList();

            var courseIds = bookmarks
                .OrderByDescending(b => b.Id)
                .Take(5)
                .Where(b => b.CourseId != null)
                .Select(b => b.CourseId!.Value)
                .ToList();

            var progressData = await _progressRepo
                .GetProgressForCoursesAsync(studentId, courseIds);
            _logger.LogInformation("Retrieved progress for {CourseCount} courses for user {UserId}", progressData.Count, studentId);

            var progressList = progressData
                .Select(p => new ProgressResponseDto
                {
                    Percentage = p.Percentage,
                    XP = p.XPScore,
                    EarnedMilestones = p.EarnedMilestones ?? new(),
                    LastLessonId = p.LastLessonId,
                    LastVideoTimestampSeconds = p.LastVideoTimestampSeconds,
                    LastAccessed = p.LastAccessed
                })
                .ToList();

            // Continue watching — courses started but not finished, most recently accessed first
            var recentProgress = await _progressRepo.GetRecentProgressAsync(studentId, 5);
            _logger.LogInformation("Retrieved {RecentProgressCount} recent progress records for user {UserId}", recentProgress.Count, studentId);

            var continueWatching = recentProgress
                .Where(p => p.Percentage > 0 && p.Percentage < 100)
                .Select(p => new ProgressResponseDto
                {
                    Percentage = p.Percentage,
                    XP = p.XPScore,
                    EarnedMilestones = p.EarnedMilestones ?? new(),
                    LastLessonId = p.LastLessonId,
                    LastVideoTimestampSeconds = p.LastVideoTimestampSeconds,
                    LastAccessed = p.LastAccessed
                })
                .ToList();

            var recommendations = await _recommendationService
                .GetPersonalizedCourseIdsAsync(studentId, 5);
            _logger.LogInformation("Retrieved {RecommendationCount} recommendations for user {UserId}", recommendations.Count, studentId);

            var enrollments = await _enrollmentRepo.GetByStudentIdAsync(studentId);
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();
            _logger.LogInformation("Retrieved {EnrollmentCount} enrollments for user {UserId}", enrolledCourseIds.Count, studentId);

            var totalXP = recentProgress.Sum(p => p.XPScore);

            _logger.LogInformation("Dashboard built successfully for user {UserId} with {TotalXP} XP", studentId, totalXP);

            return new DashboardDto
            {
                RecentBookmarks = recentBookmarks,
                RecommendedCourses = recommendations,
                Progress = progressList,
                ContinueWatching = continueWatching,
                EnrolledCourseIds = enrolledCourseIds,
                TotalBookmarks = bookmarks.Count,
                TotalXP = totalXP
            };
        }
    }
}
