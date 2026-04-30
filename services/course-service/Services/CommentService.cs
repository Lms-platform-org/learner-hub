using Courses.Api.Repositories;
using Courses.Models;
using Courses.Models.DTOs;

namespace Courses.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICourseRepository _courseRepository;

        public CommentService(
            ICommentRepository commentRepository, 
            ICourseRepository courseRepository)
        {
            _commentRepository = commentRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByCourseIdAsync(int courseId)
        {
            var comments = await _commentRepository.GetByCourseIdAsync(courseId);
            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                CourseId = c.CourseId,
                UserId = c.UserId,
                UserName = "User", // Placeholder without user api
                UserRole = "User", // Placeholder without user api
                Content = c.Content,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<CommentResponseDto?> AddCommentAsync(CreateCommentDto dto)
        {
            if (!await _courseRepository.ExistsAsync(dto.CourseId)) return null;
            
            if (string.IsNullOrEmpty(dto.UserId)) return null;

            var comment = new Comment
            {
                CourseId = dto.CourseId,
                UserId = dto.UserId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            await _commentRepository.AddAsync(comment);
            await _commentRepository.SaveChangesAsync();

            return new CommentResponseDto
            {
                Id = comment.Id,
                CourseId = comment.CourseId,
                UserId = comment.UserId,
                UserName = "User",
                UserRole = "User",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync()
        {
            var comments = await _commentRepository.GetAllAsync();
            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                CourseId = c.CourseId,
                UserId = c.UserId,
                UserName = "User",
                UserRole = "User",
                Content = c.Content,
                CreatedAt = c.CreatedAt
            });
        }
    }
}
