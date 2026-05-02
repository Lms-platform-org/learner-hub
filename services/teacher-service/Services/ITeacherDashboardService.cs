using LearningPlatform.TeacherService.DTOs;

namespace LearningPlatform.TeacherService.Services
{
    public interface ITeacherDashboardService
    {
        /// <summary>
        /// Aggregates all dashboard metrics for the given teacher by calling
        /// the Course API for their courses and each course's enrollment count.
        /// </summary>
        Task<TeacherDashboardDto> GetDashboardAsync(string teacherId);
    }
}