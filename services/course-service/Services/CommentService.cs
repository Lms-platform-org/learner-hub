using Courses.Api.Repositories;
using Courses.Models;
using Courses.Models.DTOs;

namespace Courses.Api.Services
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IUserRepository _userRepository;

        public CommentService(
            ICommentRepository commentRepository, 
            ICourseRepository courseRepository, 
            IUserRepository userRepository)
        {
            _commentRepository = commentRepository;
            _courseRepository = courseRepository;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<CommentResponseDto>> GetCommentsByCourseIdAsync(int courseId)
        {
            var comments = await _commentRepository.GetByCourseIdAsync(courseId);
            return comments.Select(c => new CommentResponseDto
            {
                Id = c.Id,
                CourseId = c.CourseId,
                UserId = c.UserId,
                UserName = c.User?.Name ?? "Unknown",
                UserRole = c.User?.Role ?? "Unknown",
                Content = c.Content,
                CreatedAt = c.CreatedAt
            });
        }

        public async Task<CommentResponseDto?> AddCommentAsync(CreateCommentDto dto)
        {
            if (!await _courseRepository.ExistsAsync(dto.CourseId)) return null;
            
            var user = await _userRepository.GetByIdAsync(dto.UserId);
            if (user == null) return null;

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
                UserName = user.Name,
                UserRole = user.Role,
                Content = comment.Content,
                CreatedAt = comment.CreatedAt
            };
        }
    }
}
