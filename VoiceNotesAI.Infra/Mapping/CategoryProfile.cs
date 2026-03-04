using AutoMapper;
using VoiceNotesAI.DTOs;
using VoiceNotesAI.Models;

namespace VoiceNotesAI.Mapping;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryInfo>();
        CreateMap<CategoryInfo, Category>();
    }
}
