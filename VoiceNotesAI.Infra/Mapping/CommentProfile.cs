using AutoMapper;
using VoiceNotesAI.DTOs;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Mapping;

public class CommentProfile : Profile
{
    public CommentProfile()
    {
        CreateMap<Comment, CommentInfo>();
        CreateMap<CommentInfo, Comment>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
    }
}
