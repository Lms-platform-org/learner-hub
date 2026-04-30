using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public interface IBookDiscoveryService
    {
        Task<PagedResponseDto<BookDto>> SearchBooksByTopicAsync(string topic, int page, int pageSize);
    }
}
