using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Repositories;

namespace LearningPlatform.StudentService.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        private readonly ICourseIntegrationService _courseService;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(
            IEnrollmentRepository repo,
            ICourseIntegrationService courseService,
            ILogger<EnrollmentService> logger)
        {
            _repo = repo;
            _courseService = courseService;
            _logger = logger;
        }

        public async Task EnrollAsync(string studentId, int courseId)
        {
            if (courseId <= 0)
            {
                _logger.LogWarning("Invalid course id {CourseId} provided for enrollment", courseId);
                throw new Exception("Invalid course id");
            }

            var course = await _courseService.GetCourseByIdAsync(courseId);
            if (course == null)
            {
                _logger.LogWarning("Course {CourseId} not found for enrollment", courseId);
                throw new Exception("Course not found");
            }

            var alreadyEnrolled = await _repo.IsEnrolledAsync(studentId, courseId);
            if (alreadyEnrolled)
            {
                _logger.LogWarning("Student {StudentId} already enrolled in course {CourseId}", studentId, courseId);
                throw new Exception("Already enrolled in this course");
            }

            _logger.LogInformation("Enrolling student {StudentId} in course {CourseId}", studentId, courseId);

            await _repo.AddAsync(new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId
            });

            await _courseService.NotifyEnrollmentAsync(studentId, courseId);
        }

        public async Task<List<Enrollment>> GetEnrolledCoursesAsync(string studentId) =>
            await _repo.GetByStudentIdAsync(studentId);

        public async Task<bool> IsEnrolledAsync(string studentId, int courseId) =>
            await _repo.IsEnrolledAsync(studentId, courseId);
    }
}
