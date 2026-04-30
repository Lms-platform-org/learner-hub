using Courses.Models;

namespace Courses.Api.Services
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<bool> CheckDuplicateCourseAsync(string title, string instructorId);
        Task<IEnumerable<Course>> GetTeacherCoursesAsync(string instructorId);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<Course> CreateCourseAsync(Course course);
        Task<bool> UpdateCourseAsync(int id, Course course);
        Task<Course?> PublishCourseAsync(int id, string instructorId);
        Task<CourseVideo?> AddVideoToCourseAsync(int courseId, string instructorId, string videoUrl);
    }
}
