using Courses.Models;

namespace Courses.Api.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<IEnumerable<Course>> GetTeacherCoursesAsync(string teacherId);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(int id, Course course);
        Task<Course?> PublishCourseAsync(int id, string teacherId);
        Task<CourseVideo?> AddVideoToCourseAsync(int courseId, string teacherId, string videoUrl);
    }
}
