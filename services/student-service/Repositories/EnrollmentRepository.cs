using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService.Repositories
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly StudentDbContext _context;

        public EnrollmentRepository(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetAsync(string studentId, int courseId) =>
            await _context.Enrollments
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        public async Task<List<Enrollment>> GetByStudentIdAsync(string studentId) =>
            await _context.Enrollments
                .AsNoTracking()
                .Where(e => e.StudentId == studentId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId) =>
            await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

        public async Task AddAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }
    }
}
