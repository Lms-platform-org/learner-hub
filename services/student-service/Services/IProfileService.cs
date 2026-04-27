using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;

namespace LearningPlatform.StudentService.Services
{
    public interface IProfileService
    {
        Task<StudentProfile?> GetAsync(string studentId);
        Task SaveAsync(string studentId, ProfileDto dto);
    }
}
