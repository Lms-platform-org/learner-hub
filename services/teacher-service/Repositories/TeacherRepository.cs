using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Courses.Api.Data;
using Courses.Models;

namespace TeacherDashboardApi.Repositories
{
    public class TeacherRepository : ITeacherRepository
    {
        private readonly ApplicationDbContext _context;

        public TeacherRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetCoursesByTeacherAsync(string teacherId)
        {
            return await _context.Courses
                .Where(c => c.InstructorId == teacherId)
                .Include(c => c.Videos)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int courseId, string teacherId)
        {
            return await _context.Courses
                .Include(c => c.Videos)
                .FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == teacherId);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course> UpdateCourseAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<bool> DeleteCourseAsync(int courseId, string teacherId)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Id == courseId && c.InstructorId == teacherId);
            if (course == null) return false;

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(int courseId, string teacherId)
        {
            // Verify course belongs to teacher
            var courseExists = await _context.Courses.AnyAsync(c => c.Id == courseId && c.InstructorId == teacherId);
            if (!courseExists) return Enumerable.Empty<Enrollment>();

            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .Include(e => e.Student)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByTeacherAsync(string teacherId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Where(e => e.Course!.InstructorId == teacherId)
                .ToListAsync();
        }
    }
}
