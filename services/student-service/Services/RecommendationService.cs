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

            return result.Distinct().ToList();
        }

        public async Task<List<int>> GetPersonalizedCourseIdsAsync(string studentId, int count)
        {
            if (string.IsNullOrWhiteSpace(studentId))
                throw new Exception("Invalid student id");

            count = NormalizeCount(count);

            _logger.LogInformation("Fetching personalized recommendations for {UserId}", studentId);

            var studentCourseIds = await _bookmarkRepo.GetAllBookmarkedCourseIdsAsync(studentId);

            if (!studentCourseIds.Any())
            {
                _logger.LogInformation("No bookmarks found. Falling back to trending.");
                return await GetTrendingCourseIdsAsync(count);
            }

            var recommendations = await _bookmarkRepo
                .GetSimilarUsersBookmarksAsync(studentId, studentCourseIds, count);

            var cleaned = recommendations
                .Where(id => !studentCourseIds.Contains(id))
                .Distinct()
                .Take(count)
                .ToList();

            if (!cleaned.Any())
            {
                _logger.LogInformation("No strong recommendations. Falling back to trending.");
                return await GetTrendingCourseIdsAsync(count);
            }

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