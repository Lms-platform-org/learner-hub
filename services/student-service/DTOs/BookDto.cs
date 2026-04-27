using System.Text.Json.Serialization;

namespace LearningPlatform.StudentService.DTOs
{
    public class BookDto
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("author_name")]
        public List<string>? Author_Name { get; set; }

        [JsonPropertyName("key")]
        public string? Key { get; set; } // e.g. /works/OL45804W → https://openlibrary.org/works/OL45804W

        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }
    }
}
