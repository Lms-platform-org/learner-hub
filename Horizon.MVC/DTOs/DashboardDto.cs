namespace Horizon.MVC.DTOs
{
    public class DashboardDto
    {
        public List<BookmarkDto> RecentBookmarks { get; set; } = new();
        public List<int> RecommendedCourses { get; set; } = new();
        public List<ProgressResponseDto> Progress { get; set; } = new();
        public List<ProgressResponseDto> ContinueWatching { get; set; } = new();
        public List<int> EnrolledCourseIds { get; set; } = new();
        public int TotalBookmarks { get; set; }
        public int TotalXP { get; set; }
    }
}


