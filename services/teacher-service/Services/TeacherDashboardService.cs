using LearningPlatform.TeacherService.DTOs;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Assembles the teacher dashboard by querying the Course API through
    /// <see cref="ICourseApiClient"/>.
    ///
    /// Aggregation rules:
    ///   - TotalStudents  = sum of enrollment counts across all teacher courses
    ///   - TotalEarnings  = sum of (Price × StudentCount) per course
    ///   - BestSelling    = course with the highest student count
    ///   - Counts for each course are fetched concurrently to minimise latency
    /// </summary>
    public class TeacherDashboardService : ITeacherDashboardService
    {
        private readonly ICourseApiClient _courseApiClient;
        private readonly ILogger<TeacherDashboardService> _logger;

        public TeacherDashboardService(
            ICourseApiClient courseApiClient,
            ILogger<TeacherDashboardService> logger)
        {
            _courseApiClient = courseApiClient;
            _logger = logger;
        }

        public async Task<TeacherDashboardDto> GetDashboardAsync(string teacherId)
        {
            _logger.LogInformation("Building dashboard for teacher {TeacherId}", teacherId);

            var courses = await _courseApiClient.GetTeacherCoursesAsync(teacherId);

            if (courses.Count == 0)
            {
                return new TeacherDashboardDto
                {
                    TeacherId = teacherId
                };
            }

            // ── Fetch enrollment counts concurrently ──────────────────────────
            // One task per course; if a course-count call fails the task returns 0
            // so the dashboard still renders with partial data.
            var countTasks = courses.Select(async c =>
            {
                try
                {
                    var count = await _courseApiClient.GetEnrollmentCountAsync(c.Id);
                    return (Course: c, Count: count);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex,
                        "Could not fetch enrollment count for course {CourseId}; defaulting to 0", c.Id);
                    return (Course: c, Count: 0);
                }
            });

            var results = await Task.WhenAll(countTasks);

            // ── Aggregate ─────────────────────────────────────────────────────
            var metrics = results.Select(r => new CourseMetricsDto
            {
                CourseId = r.Course.Id,
                Title = r.Course.Title,
                Status = r.Course.Status,
                Price = r.Course.Price,
                StudentCount = r.Count,
                Earnings = r.Course.Price * r.Count,
                CreatedAt = r.Course.CreatedAt,
                PublishedAt = r.Course.PublishedAt,
                // UnreadMessages left as 0 until Chat API integration is complete
                UnreadMessages = 0
            }).ToList();

            var bestResult = results
                .OrderByDescending(r => r.Count)
                .First();

            var dashboard = new TeacherDashboardDto
            {
                TeacherId = teacherId,
                TotalCourses = courses.Count,
                PublishedCourses = courses.Count(c => c.Status == "Published"),
                DraftCourses = courses.Count(c => c.Status == "Draft"),
                TotalStudents = metrics.Sum(m => m.StudentCount),
                TotalEarnings = metrics.Sum(m => m.Earnings),
                BestSellingCourse = bestResult.Course,
                BestSellingCourseStudentCount = bestResult.Count,
                CourseMetrics = metrics.OrderByDescending(m => m.StudentCount).ToList()
            };

            _logger.LogInformation(
                "Dashboard built for {TeacherId}: {Total} courses, {Students} students, {Earnings:C} earnings",
                teacherId, dashboard.TotalCourses, dashboard.TotalStudents, dashboard.TotalEarnings);

            return dashboard;
        }
    }
}