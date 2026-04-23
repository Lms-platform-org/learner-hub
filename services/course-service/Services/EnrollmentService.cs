using Courses.Api.Repositories;
using Courses.Models;

namespace Courses.Api.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repository;

        public EnrollmentService(IEnrollmentRepository repository)
        {
            _repository = repository;
        }

        public async Task<Enrollment?> EnrollStudentAsync(int courseId, string studentId)
        {
            // Check if already enrolled
            var existing = await _repository.GetByStudentAndCourseAsync(studentId, courseId);
            if (existing != null) return existing;

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId,
                EnrolledAt = DateTime.UtcNow
            };

            await _repository.AddAsync(enrollment);
            await _repository.SaveChangesAsync();

            return enrollment;
        }

        public async Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId)
        {
            return await _repository.GetByCourseIdAsync(courseId);
        }

        public async Task<int> GetEnrollmentCountByCourseIdAsync(int courseId)
        {
            return await _repository.GetCountByCourseIdAsync(courseId);
        }
    }
}
