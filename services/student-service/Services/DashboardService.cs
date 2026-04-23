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
                throw new Exception("Invalid student id");

            _logger.LogInformation("Building dashboard for user {UserId}", studentId);

            var bookmarks = await _bookmarkRepo.GetByStudentIdAsync(studentId);

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

            var enrollments = await _enrollmentRepo.GetByStudentIdAsync(studentId);
            var enrolledCourseIds = enrollments.Select(e => e.CourseId).ToList();

            var totalXP = recentProgress.Sum(p => p.XPScore);

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