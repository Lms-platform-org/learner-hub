using AutoMapper;
using Courses.Models;
using Courses.Models.DTOs;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Course Mappings
            CreateMap<Course, CourseReadDTO>();
            CreateMap<CourseCreateDTO, Course>()
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CourseStatus.Draft))
                .ForMember(dest => dest.IsPublished, opt => opt.MapFrom(src => false));

            CreateMap<CourseUpdateDTO, Course>()
                .ForMember(dest => dest.InstructorId, opt => opt.Ignore()) // Don't update instructor
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());

            // Enrollment Mapping
            CreateMap<Enrollment, StudentEnrollmentDTO>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student != null ? src.Student.Name : "Unknown"))
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : "Unknown"));
        }
    }
}
