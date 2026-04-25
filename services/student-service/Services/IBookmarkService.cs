using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Services
{
    public interface IBookmarkService
    {
        Task<List<Bookmark>> GetAllAsync(string studentId);
        Task AddAsync(string studentId, BookmarkDto dto);
        Task<bool> ToggleAsync(string studentId, BookmarkDto dto);
        Task<bool> DeleteAsync(string studentId, int id);
        Task<Bookmark?> GetByIdAsync(string studentId, int id);
        Task<bool> UpdateAsync(string studentId, int id, BookmarkDto dto);
        Task UpdateNoteAsync(string studentId, int id, string? note);
        Task<bool> IsBookmarkedAsync(string studentId, BookmarkDto dto);
        Task<PagedResponseDto<Bookmark>> GetPagedAsync(string studentId, int page, int pageSize);
        Task<PagedResponseDto<Bookmark>> GetByCategoryPagedAsync(string studentId, string category, int page, int pageSize);
    }
}
