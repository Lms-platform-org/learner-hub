using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Repositories;
using LearningPlatform.StudentService.Exceptions;

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

        public async Task<List<Bookmark>> GetAllAsync(string studentId) =>
            await _repo.GetByStudentIdAsync(studentId);

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
            return new PagedResponseDto<Bookmark> { Data = data, Page = page, PageSize = pageSize, TotalCount = total };
        }

        public async Task<PagedResponseDto<Bookmark>> GetByCategoryPagedAsync(string studentId, string category, int page, int pageSize)
        {
            var (data, total) = await _repo.GetByCategoryPagedAsync(studentId, category, page, pageSize);
            return new PagedResponseDto<Bookmark> { Data = data, Page = page, PageSize = pageSize, TotalCount = total };
        }

        public async Task<bool> ToggleAsync(string studentId, BookmarkDto dto)
        {
            if (dto.Type == "course")
            {
                if (dto.CourseId == null || dto.CourseId <= 0)
                {
                    _logger.LogWarning("Invalid CourseId provided for bookmark toggle");
                    throw new BusinessException("Invalid CourseId");
                }

                var existing = await _repo.GetByCourseIdAsync(studentId, dto.CourseId.Value);
                if (existing != null)
                {
                    _logger.LogInformation("Removing bookmark for course {CourseId}", dto.CourseId.Value);
                    await _repo.DeleteAsync(existing);
                    return false; // removed
                }
            }
            else if (dto.Type == "book")
            {
                if (string.IsNullOrWhiteSpace(dto.BookKey))
                {
                    _logger.LogWarning("BookKey is required for book bookmark toggle");
                    throw new BusinessException("BookKey is required");
                }

                var existing = await _repo.GetByBookKeyAsync(studentId, dto.BookKey);
                if (existing != null)
                {
                    _logger.LogInformation("Removing bookmark for book {BookKey}", dto.BookKey);
                    await _repo.DeleteAsync(existing);
                    return false; // removed
                }
            }

            _logger.LogInformation("Adding bookmark type {Type}", dto.Type);

            await _repo.AddAsync(new Bookmark
            {
                StudentId = studentId,
                CourseId = dto.CourseId,
                BookKey = dto.BookKey?.Trim(),
                BookTitle = dto.BookTitle?.Trim(),
                BookAuthor = dto.BookAuthor?.Trim(),
                Type = dto.Type,
                Category = AutoDetectCategory(dto),
                PersonalNote = null // student adds note later from bookmarks page
            });

            return true; // added
        }

        public async Task AddAsync(string studentId, BookmarkDto dto)
        {
            await ToggleAsync(studentId, dto);
        }

        public async Task UpdateNoteAsync(string studentId, int id, string? note)
        {
            var bookmark = await _repo.GetByIdAsync(id);
            if (bookmark == null || bookmark.StudentId != studentId)
            {
                _logger.LogWarning("Bookmark {BookmarkId} not found for user {UserId}", id, studentId);
                throw new ResourceNotFoundException("Bookmark", id);
            }

            _logger.LogInformation("Updating note for bookmark {BookmarkId}", id);
            bookmark.PersonalNote = note?.Trim();
            await _repo.UpdateAsync(bookmark);
        }

        public async Task<bool> UpdateAsync(string studentId, int id, BookmarkDto dto)
        {
            var bookmark = await _repo.GetByIdAsync(id);
            if (bookmark == null || bookmark.StudentId != studentId)
            {
                _logger.LogWarning("Bookmark {BookmarkId} not found for user {UserId}", id, studentId);
                throw new ResourceNotFoundException("Bookmark", id);
            }

            _logger.LogInformation("Updating bookmark {BookmarkId}", id);
            bookmark.PersonalNote = dto.PersonalNote?.Trim();
            await _repo.UpdateAsync(bookmark);
            return true;
        }

        public async Task<bool> IsBookmarkedAsync(string studentId, BookmarkDto dto)
        {
            if (dto.Type == "course" && dto.CourseId.HasValue)
                return await _repo.GetByCourseIdAsync(studentId, dto.CourseId.Value) != null;
            
            if (dto.Type == "book" && !string.IsNullOrWhiteSpace(dto.BookKey))
                return await _repo.GetByBookKeyAsync(studentId, dto.BookKey) != null;
            
            return false;
        }

        public async Task<bool> DeleteAsync(string studentId, int id)
        {
            var bookmark = await _repo.GetByIdAsync(id);
            if (bookmark == null || bookmark.StudentId != studentId)
            {
                _logger.LogWarning("Bookmark {BookmarkId} not found for user {UserId}", id, studentId);
                throw new ResourceNotFoundException("Bookmark", id);
            }

            _logger.LogInformation("Deleting bookmark {BookmarkId}", id);
            await _repo.DeleteAsync(bookmark);
            return true;
        }

        // Auto detect category from book title or course
        private string AutoDetectCategory(BookmarkDto dto)
        {
            if (dto.Type == "course")
                return "course";

            var text = $"{dto.BookTitle} {dto.BookAuthor}".ToLower();

            if (ContainsAny(text, "python", "django", "flask"))         return "python";
            if (ContainsAny(text, "machine learning", "deep learning",
                "neural", "ai ", "artificial intelligence", "data science",
                "tensorflow", "pytorch"))                                return "ml";
            if (ContainsAny(text, "javascript", "react", "angular",
                "vue", "node", "html", "css", "web"))                   return "web";
            if (ContainsAny(text, "java ", "spring", "kotlin"))         return "java";
            if (ContainsAny(text, "sql", "database", "mongodb",
                "postgres", "mysql"))                                    return "database";
            if (ContainsAny(text, "c#", ".net", "asp.net", "csharp"))   return "dotnet";
            if (ContainsAny(text, "algorithm", "data structure",
                "competitive", "leetcode"))                              return "algorithms";
            if (ContainsAny(text, "cloud", "aws", "azure", "devops",
                "docker", "kubernetes"))                                 return "cloud";
            if (ContainsAny(text, "math", "calculus", "statistics",
                "linear algebra"))                                       return "math";

            return "general";
        }

        private bool ContainsAny(string text, params string[] keywords) =>
            keywords.Any(k => text.Contains(k));
    }
}
