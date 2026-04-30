using LearningPlatform.StudentService.DTOs;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
namespace LearningPlatform.StudentService.Services
{
    public class BookDiscoveryService : IBookDiscoveryService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<BookDiscoveryService> _logger;

        public BookDiscoveryService(HttpClient httpClient, IMemoryCache cache, ILogger<BookDiscoveryService> logger)
        {
            _httpClient = httpClient;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedResponseDto<BookDto>> SearchBooksByTopicAsync(string topic, int page, int pageSize)
        {
            string cacheKey = $"BookSearch_{topic.ToLower().Replace(" ", "_")}";

            if (!_cache.TryGetValue(cacheKey, out List<BookDto> cachedBooks))
            {
                try
                {
                    _logger.LogInformation("Fetching books from OpenLibrary for topic: {Topic}", topic);
                    var response = await _httpClient.GetFromJsonAsync<OpenLibraryResponse>(
                        $"https://openlibrary.org/search.json?q={topic}&limit=50");

                    cachedBooks = response?.Docs ?? new List<BookDto>();
                    _logger.LogInformation("Retrieved {BookCount} books for topic: {Topic}", cachedBooks.Count, topic);

                    var cacheOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                        .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                    _cache.Set(cacheKey, cachedBooks, cacheOptions);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to fetch books from OpenLibrary for topic: {Topic}", topic);
                    cachedBooks = new List<BookDto>();
                }
            }
            else
            {
                _logger.LogInformation("Retrieved books for topic {Topic} from cache", topic);
            }

            var total = cachedBooks.Count;

            var data = cachedBooks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResponseDto<BookDto>
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }
    }
}
