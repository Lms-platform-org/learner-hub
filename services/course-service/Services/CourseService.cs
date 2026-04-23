using Courses.Api.Repositories;
using Courses.Models;

namespace Courses.Api.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _repository.GetAllPublishedAsync();
        }

        public async Task<IEnumerable<Course>> GetTeacherCoursesAsync(string teacherId)
        {
            return await _repository.GetByTeacherIdAsync(teacherId);
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            course.TeacherId = "Teacher1"; // Strategy: Hardcoded based on current project state
            course.Status = CourseStatus.Draft;
            course.CreatedAt = DateTime.UtcNow;
            
            await _repository.AddAsync(course);
            await _repository.SaveChangesAsync();
            return course;
        }

        public async Task<bool> UpdateCourseAsync(int id, Course courseInput)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null || course.TeacherId != "Teacher1") return false;

            course.Title = courseInput.Title;
            course.Description = courseInput.Description;
            course.Price = courseInput.Price;

            await _repository.UpdateAsync(course);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<Course?> PublishCourseAsync(int id, string teacherId)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null || course.TeacherId != teacherId) return null;

            if (course.Status == CourseStatus.Draft)
            {
                course.Status = CourseStatus.Published;
                course.PublishedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(course);
                await _repository.SaveChangesAsync();
            }
            return course;
        }

        public async Task<CourseVideo?> AddVideoToCourseAsync(int courseId, string teacherId, string videoUrl)
        {
            var course = await _repository.GetByIdAsync(courseId);
            if (course == null || course.TeacherId != teacherId) return null;

            var video = new CourseVideo
            {
                CourseId = courseId,
                VideoUrl = videoUrl
            };

            await _repository.AddVideoAsync(video);
            await _repository.SaveChangesAsync();
            return video;
        }
    }
}
