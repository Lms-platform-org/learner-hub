using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public interface IStudentExperienceService
    {
        Task SetResumePointAsync(string studentId, int courseId, int lessonId, int seconds);
        Task<StudentProgress> MarkLessonCompleteAsync(string studentId, int courseId, int lessonId);
        Task<StudentProgress?> GetProgressAsync(string studentId, int courseId);
    }
}
