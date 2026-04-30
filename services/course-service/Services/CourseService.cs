using Courses.Api.Repositories;
using Courses.Models;
using Courses.Api.Exceptions;

namespace Courses.Api.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repository;

        public CourseService(ICourseRepository repository)
        {
            _repository = repository;
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _repository.GetAllAsync();
        }
        
        public async Task<bool> CheckDuplicateCourseAsync(string title, string instructorId)
        {
            return await _repository.ExistsByTitleAndInstructorAsync(title, instructorId);
        }

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _repository.GetAllPublishedAsync();
        }

        public async Task<IEnumerable<Course>> GetTeacherCoursesAsync(string instructorId)
        {
            return await _repository.GetByInstructorIdAsync(instructorId);
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            course.CreatedAt = DateTime.UtcNow;
            course.Status = CourseStatus.Draft;
            
            await _repository.AddAsync(course);
            await _repository.SaveChangesAsync();
            return course;
        }

        public async Task<bool> UpdateCourseAsync(int id, Course courseInput)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null) return false;

            course.Title = courseInput.Title;
            course.Description = courseInput.Description;
            course.Price = courseInput.Price;
            course.Category = courseInput.Category;
            course.Duration = courseInput.Duration;
            course.ThumbnailUrl = courseInput.ThumbnailUrl;
            course.StartDate = courseInput.StartDate;
            course.IsPublished = courseInput.IsPublished;

            await _repository.UpdateAsync(course);
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<Course?> PublishCourseAsync(int id, string instructorId)
        {
            var course = await _repository.GetByIdAsync(id);
            if (course == null || course.InstructorId != instructorId) return null;

            if (course.Status == CourseStatus.Draft)
            {
                course.Status = CourseStatus.Published;
                course.PublishedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(course);
                await _repository.SaveChangesAsync();
            }
            return course;
        }

        public async Task<CourseVideo?> AddVideoToCourseAsync(int courseId, string instructorId, string videoUrl)
        {
            var course = await _repository.GetByIdAsync(courseId);
            if (course == null || course.InstructorId != instructorId) return null;

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
