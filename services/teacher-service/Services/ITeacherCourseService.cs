using LearningPlatform.TeacherService.DTOs;

namespace LearningPlatform.TeacherService.Services
{
    public interface ITeacherCourseService
    {
        Task<List<CourseReadDto>> GetMyCoursesAsync(string teacherId);
        Task<CourseReadDto?>      GetCourseAsync(int courseId);
        Task<CourseReadDto>       CreateCourseAsync(CourseWriteDto dto, string teacherId);
        Task                      UpdateCourseAsync(int courseId, CourseWriteDto dto, string teacherId);
        Task<CourseReadDto>       PublishCourseAsync(int courseId, string teacherId);
        Task                      AddVideoAsync(int courseId, string videoUrl, string teacherId);
    }
}
