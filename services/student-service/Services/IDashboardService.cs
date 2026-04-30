using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(string studentId);
    }
}
