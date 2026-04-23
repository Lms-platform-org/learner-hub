using LearningPlatform.StudentService.DTOs;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Json;
namespace LearningPlatform.StudentService.Services
{
    public class BookDiscoveryService(HttpClient httpClient, IMemoryCache cache) : IBookDiscoveryService
    {

        public async Task<PagedResponseDto<BookDto>> SearchBooksByTopicAsync(string topic, int page, int pageSize)
        {
            string cacheKey = $"BookSearch_{topic.ToLower().Replace(" ", "_")}";

            if (!cache.TryGetValue(cacheKey, out List<BookDto> cachedBooks))
            {
                var response = await httpClient.GetFromJsonAsync<OpenLibraryResponse>(
                    $"https://openlibrary.org/search.json?q={topic}&limit=50");

                cachedBooks = response?.Docs ?? new List<BookDto>();

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(30))
                    .SetAbsoluteExpiration(TimeSpan.FromHours(1));

                cache.Set(cacheKey, cachedBooks, cacheOptions);
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
