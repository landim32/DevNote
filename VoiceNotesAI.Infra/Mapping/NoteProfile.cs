using AutoMapper;
using VoiceNotesAI.DTOs;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Mapping;

public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteInfo>();
        CreateMap<NoteInfo, Note>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
    }
}
