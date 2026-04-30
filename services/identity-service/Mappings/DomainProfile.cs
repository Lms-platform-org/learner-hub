using AutoMapper;
using LearningPlatformAuth.Data;
using LearningPlatformAuth.Models;

namespace LearningPlatformAuth.Mappings
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<ApplicationUser, AuthResponse>()
                .ForMember(dest => dest.Token, opt => opt.Ignore()) // token set later
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName ?? string.Empty))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // roles added after token generation
        }
    }
}
