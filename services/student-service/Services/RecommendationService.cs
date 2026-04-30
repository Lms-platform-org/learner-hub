using LearningPlatform.StudentService.Repositories;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.StudentService.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IBookmarkRepository _bookmarkRepo;
        private readonly IProgressRepository _progressRepo;
        private readonly ILogger<RecommendationService> _logger;

        public RecommendationService(
            IBookmarkRepository bookmarkRepo,
            IProgressRepository progressRepo,
            ILogger<RecommendationService> logger)
        {
            _bookmarkRepo = bookmarkRepo;
            _progressRepo = progressRepo;
            _logger = logger;
        }

        public async Task<List<int>> GetTrendingCourseIdsAsync(int count)
        {
            count = NormalizeCount(count);

            _logger.LogInformation("Fetching trending courses, count={Count}", count);

            var result = await _progressRepo.GetTrendingCourseIdsAsync(count);
            _logger.LogInformation("Retrieved {TrendingCount} trending courses", result.Count);

            return result.Distinct().ToList();
        }

        public async Task<List<int>> GetPersonalizedCourseIdsAsync(string studentId, int count)
        {
            if (string.IsNullOrWhiteSpace(studentId))
            {
                _logger.LogWarning("Invalid student id provided for personalized recommendations");
                throw new Exception("Invalid student id");
            }

            count = NormalizeCount(count);

            _logger.LogInformation("Fetching personalized recommendations for {UserId}", studentId);

            var studentCourseIds = await _bookmarkRepo.GetAllBookmarkedCourseIdsAsync(studentId);
            _logger.LogInformation("Student {UserId} has {BookmarkCount} bookmarked courses", studentId, studentCourseIds.Count);

            if (!studentCourseIds.Any())
            {
                _logger.LogInformation("No bookmarks found for {UserId}. Falling back to trending.", studentId);
                return await GetTrendingCourseIdsAsync(count);
            }

            var recommendations = await _bookmarkRepo
                .GetSimilarUsersBookmarksAsync(studentId, studentCourseIds, count);
            _logger.LogInformation("Retrieved {RecommendationCount} similar user bookmarks for {UserId}", recommendations.Count, studentId);

            var cleaned = recommendations
                .Where(id => !studentCourseIds.Contains(id))
                .Distinct()
                .Take(count)
                .ToList();

            if (!cleaned.Any())
            {
                _logger.LogInformation("No strong recommendations found for {UserId}. Falling back to trending.", studentId);
                return await GetTrendingCourseIdsAsync(count);
            }

            _logger.LogInformation("Returning {FinalCount} personalized recommendations for {UserId}", cleaned.Count, studentId);
            return cleaned;
        }

        private int NormalizeCount(int count)
        {
            if (count <= 0) return 5;
            if (count > 50) return 50;
            return count;
        }
    }
}
