using System.Collections.Generic;
using System.Threading.Tasks;
using Courses.Models;

namespace TeacherDashboardApi.Repositories
{
    public interface ITeacherRepository
    {
        Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId);
        Task<Course?> GetCourseByIdAsync(int courseId, string teacherId);
        Task<Course> CreateCourseAsync(Course course);
        Task<Course> UpdateCourseAsync(Course course);
        Task<bool> DeleteCourseAsync(int courseId, string teacherId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId, string teacherId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByTeacherAsync(string teacherId);
    }
}
