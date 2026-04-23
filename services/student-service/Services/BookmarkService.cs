using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Repositories;
using Microsoft.Extensions.Logging;

namespace LearningPlatform.StudentService.Services
{
    public class BookmarkService : IBookmarkService
    {
        private readonly IBookmarkRepository _repo;
        private readonly ILogger<BookmarkService> _logger;

        public BookmarkService(IBookmarkRepository repo, ILogger<BookmarkService> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        public async Task<List<Bookmark>> GetAllAsync(string studentId)
        {
            _logger.LogInformation("Fetching all bookmarks for user {UserId}", studentId);
            return await _repo.GetByStudentIdAsync(studentId);
        }
        public async Task<Bookmark?> GetByIdAsync(string studentId, int id)
        {
            var bookmark = await _repo.GetByIdAsync(id);
            if (bookmark == null || bookmark.StudentId != studentId)
                return null;
            return bookmark;
        }

        public async Task<PagedResponseDto<Bookmark>> GetPagedAsync(string studentId, int page, int pageSize)
        {
            var (data, total) = await _repo.GetPagedAsync(studentId, page, pageSize);

            return new PagedResponseDto<Bookmark>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<PagedResponseDto<Bookmark>> GetByCategoryPagedAsync(string studentId, string category, int page, int pageSize)
        {
            var (data, total) = await _repo.GetByCategoryPagedAsync(studentId, category, page, pageSize);

            return new PagedResponseDto<Bookmark>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task AddAsync(string studentId, BookmarkDto dto)
        {
            if (dto.Type == "course" && (dto.CourseId == null || dto.CourseId <= 0))
                throw new Exception("Invalid CourseId for course bookmark");

            if (dto.Type == "book" && string.IsNullOrWhiteSpace(dto.BookKey))
                throw new Exception("BookKey is required for book bookmark");

            if (string.IsNullOrWhiteSpace(dto.Category))
                throw new Exception("Category is required");

            _logger.LogInformation("Adding bookmark type {Type}", dto.Type);

            var bookmark = new Bookmark
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                BookKey = dto.BookKey?.Trim(),
                BookTitle = dto.BookTitle?.Trim(),
                BookAuthor = dto.BookAuthor?.Trim(),
                Type = dto.Type,
                Category = dto.Category.Trim().ToLower(),
                PersonalNote = dto.PersonalNote?.Trim()
            };

            await _repo.AddAsync(bookmark);
        }

        public async Task<bool> UpdateAsync(string studentId, int id, BookmarkDto dto)
        {
            if (id <= 0)
                throw new Exception("Invalid bookmark id");

            var bookmark = await _repo.GetByIdAsync(id);

            if (bookmark == null)
                throw new Exception("Bookmark not found");

            if (bookmark.StudentId != studentId)
                throw new Exception("Unauthorized access");

            _logger.LogInformation("Updating bookmark {Id}", id);

            bookmark.Category = dto.Category?.Trim().ToLower();
            bookmark.PersonalNote = dto.PersonalNote?.Trim();

            await _repo.UpdateAsync(bookmark);

            return true;
        }

        public async Task<bool> DeleteAsync(string studentId, int id)
        {
            if (id <= 0)
                throw new Exception("Invalid bookmark id");

            var bookmark = await _repo.GetByIdAsync(id);

            if (bookmark == null)
                throw new Exception("Bookmark not found");

            if (bookmark.StudentId != studentId)
                throw new Exception("Unauthorized access");

            _logger.LogInformation("Deleting bookmark {Id}", id);

            await _repo.DeleteAsync(bookmark);

            return true;
        }
    }
}