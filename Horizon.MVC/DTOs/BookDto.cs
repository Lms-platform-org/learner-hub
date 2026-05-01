using System.Text.Json.Serialization;

namespace Horizon.MVC.DTOs
{
    public class BookDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("author_name")]
        public List<string>? AuthorName { get; set; }

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; } // e.g. /works/OL45804W

        public string Authors => AuthorName != null ? string.Join(", ", AuthorName) : "Unknown";

        public string OpenLibraryUrl => !string.IsNullOrEmpty(Key)
            ? $"https://openlibrary.org{Key}"
            : $"https://openlibrary.org/search?q={Uri.EscapeDataString(Title)}";
    }
}


