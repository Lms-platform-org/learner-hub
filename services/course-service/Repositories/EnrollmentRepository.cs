using Courses.Api.Data;
using Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }

        public async Task AddAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }

        public async Task<int> GetCountByCourseIdAsync(int courseId)
        {
            return await _context.Enrollments.CountAsync(e => e.CourseId == courseId);
        }

        public async Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetAllAsync()
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .ToListAsync();
        }
    }
}
