using AutoMapper;
using Courses.Models;
using Courses.Models.DTOs;

namespace Courses.Api.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Course Mapping
            CreateMap<Course, CourseReadDTO>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CourseVideo, CourseVideoDTO>();

            CreateMap<CourseWriteDTO, Course>();

            // Comment Mapping
            CreateMap<Comment, CommentResponseDto>();

            CreateMap<CreateCommentDto, Comment>();

            // Enrollment Mapping
            CreateMap<Enrollment, EnrollmentReadDTO>()
                .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.Course != null ? src.Course.Title : null));

            CreateMap<EnrollmentCreateDTO, Enrollment>();
        }
    }
}
