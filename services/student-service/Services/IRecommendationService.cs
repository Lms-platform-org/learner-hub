namespace LearningPlatform.StudentService.Services
{
    public interface IRecommendationService
    {
        Task<List<int>> GetTrendingCourseIdsAsync(int count);
        Task<List<int>> GetPersonalizedCourseIdsAsync(string studentId, int count);
    }
}
