using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService.Repositories
{
    public class ProgressRepository : IProgressRepository
    {
        private readonly StudentDbContext _context;

        public ProgressRepository(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<StudentProgress?> GetProgressAsync(string sid, int cid)
        {
            return await _context.ProgressRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.StudentId == sid && p.CourseId == cid);
        }

        public async Task AddAsync(StudentProgress p)
        {
            _context.ProgressRecords.Add(p);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(StudentProgress p)
        {
            var tracked = _context.ChangeTracker.Entries<StudentProgress>()
                .FirstOrDefault(e => e.Entity.StudentId == p.StudentId && e.Entity.CourseId == p.CourseId);

            if (tracked != null)
                tracked.CurrentValues.SetValues(p);
            else
                _context.ProgressRecords.Update(p);

            await _context.SaveChangesAsync();
        }

        public async Task<List<int>> GetTrendingCourseIdsAsync(int count)
        {
            if (count <= 0) count = 5;
            if (count > 50) count = 50;

            return await _context.ProgressRecords
                .AsNoTracking()
                .GroupBy(p => p.CourseId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();
        }


        public async Task<List<StudentProgress>> GetProgressForCoursesAsync(string sid, List<int> courseIds)
        {
            return await _context.ProgressRecords
                .AsNoTracking()
                .Where(p => p.StudentId == sid && courseIds.Contains(p.CourseId))
                .ToListAsync();
        }

        public async Task<List<StudentProgress>> GetRecentProgressAsync(string sid, int count)
        {
            if (count <= 0) count = 5;
            if (count > 50) count = 50;

            return await _context.ProgressRecords
                .AsNoTracking()
                .Where(p => p.StudentId == sid)
                .OrderByDescending(p => p.LastAccessed)
                .Take(count)
                .ToListAsync();
        }
    }
}