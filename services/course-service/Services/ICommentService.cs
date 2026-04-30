using Courses.Models.DTOs;

namespace Courses.Api.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentResponseDto>> GetCommentsByCourseIdAsync(int courseId);
        Task<CommentResponseDto?> AddCommentAsync(CreateCommentDto dto);
        Task<IEnumerable<CommentResponseDto>> GetAllCommentsAsync();
    }
}
