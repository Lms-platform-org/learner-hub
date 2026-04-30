using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;

namespace LearningPlatform.StudentService.Repositories
{
    public interface IBookmarkRepository
    {
        Task AddAsync(Bookmark bookmark);
        Task<Bookmark?> GetByIdAsync(int id);
        Task<Bookmark?> GetByCourseIdAsync(string studentId, int courseId);
        Task<Bookmark?> GetByBookKeyAsync(string studentId, string bookKey);
        Task DeleteAsync(Bookmark bookmark);
        Task UpdateAsync(Bookmark bookmark);
        Task<List<Bookmark>> GetByStudentIdAsync(string studentId);
        Task<List<Bookmark>> GetByCategoryAsync(string studentId, string category);
        Task<(List<Bookmark>, int)> GetPagedAsync(string studentId, int page, int pageSize);
        Task<(List<Bookmark>, int)> GetByCategoryPagedAsync(string studentId, string category, int page, int pageSize);
        Task<List<int>> GetAllBookmarkedCourseIdsAsync(string studentId);
        Task<List<int>> GetSimilarUsersBookmarksAsync(string studentId, List<int> studentCourseIds, int count);
    }
}
