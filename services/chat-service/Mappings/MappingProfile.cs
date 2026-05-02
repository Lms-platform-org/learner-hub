using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Models;

namespace WebApplication1.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, MessageResponse>();
            CreateMap<ChatSession, ChatSessionResponse>();

            CreateMap<SendMessageRequest, Message>();
            CreateMap<CreateSessionRequest, ChatSession>();
        }
    }
}