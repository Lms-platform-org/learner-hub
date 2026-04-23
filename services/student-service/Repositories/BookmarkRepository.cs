using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService.Repositories
{
    public class BookmarkRepository : IBookmarkRepository
    {
        private readonly StudentDbContext _context;

        public BookmarkRepository(StudentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Bookmark b)
        {
            _context.Bookmarks.Add(b);
            await _context.SaveChangesAsync();
        }

        public async Task<Bookmark?> GetByIdAsync(int id)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task DeleteAsync(Bookmark bookmark)
        {
            _context.Bookmarks.Remove(bookmark);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Bookmark bookmark)
        {
            var tracked = _context.ChangeTracker.Entries<Bookmark>()
                .FirstOrDefault(e => e.Entity.Id == bookmark.Id);

            if (tracked != null)
                tracked.CurrentValues.SetValues(bookmark);
            else
                _context.Bookmarks.Update(bookmark);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Bookmark>> GetByStudentIdAsync(string sid)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.StudentId == sid)
                .OrderByDescending(b => b.Id)
                .ToListAsync();
        }

        public async Task<List<Bookmark>> GetByCategoryAsync(string sid, string cat)
        {
            var normalized = cat.Trim().ToLower();

            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.StudentId == sid && b.Category == normalized)
                .OrderByDescending(b => b.Id)
                .ToListAsync();
        }

        public async Task<List<int>> GetAllBookmarkedCourseIdsAsync(string sid)
        {
            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.StudentId == sid && b.CourseId != null)
                .Select(b => b.CourseId!.Value)
                .Distinct()
                .ToListAsync();
        }

        public async Task<(List<Bookmark>, int)> GetPagedAsync(string sid, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var query = _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.StudentId == sid);

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }

        public async Task<(List<Bookmark>, int)> GetByCategoryPagedAsync(string sid, string category, int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 50) pageSize = 10;

            var normalized = category.Trim().ToLower();

            var query = _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.StudentId == sid && b.Category.ToLower() == normalized);

            var total = await query.CountAsync();

            var data = await query
                .OrderByDescending(b => b.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (data, total);
        }

        public async Task<List<int>> GetSimilarUsersBookmarksAsync(string sid, List<int> studentCourseIds, int count)
        {
            if (count <= 0) count = 5;
            if (count > 50) count = 50;

            return await _context.Bookmarks
                .AsNoTracking()
                .Where(b => b.CourseId != null && studentCourseIds.Contains(b.CourseId.Value) && b.StudentId != sid)
                .Select(b => b.StudentId)
                .Distinct()
                .Join(
                    _context.Bookmarks.AsNoTracking().Where(b => b.CourseId != null),
                    userId => userId,
                    b => b.StudentId,
                    (userId, b) => b.CourseId!.Value
                )
                .Where(cid => !studentCourseIds.Contains(cid))
                .GroupBy(cid => cid)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(count)
                .ToListAsync();
        }
    }
}