using Courses.Api.Data;
using Courses.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses.Api.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllPublishedAsync()
        {
            return await _context.Courses
                .Include(c => c.Videos)
                .Where(c => c.Status == CourseStatus.Published)
                .OrderByDescending(c => c.PublishedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetByInstructorIdAsync(string instructorId)
        {
            return await _context.Courses
                .Include(c => c.Videos)
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Videos)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await Task.CompletedTask;
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Entry(course).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task AddVideoAsync(CourseVideo video)
        {
            _context.CourseVideos.Add(video);
            await Task.CompletedTask;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Courses.AnyAsync(e => e.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Videos)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsByTitleAndInstructorAsync(string title, string instructorId)
        {
            return await _context.Courses.AnyAsync(c => c.Title == title && c.InstructorId == instructorId);
        }
    }
}
