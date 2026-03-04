using VoiceNotesAI.DTOs;

namespace VoiceNotesAI.Repository;

public interface ICategoryRepository
{
    Task<List<CategoryInfo>> GetAllAsync();
    Task<CategoryInfo?> GetByIdAsync(int id);
    Task<int> SaveAsync(CategoryInfo category);
    Task<int> DeleteAsync(int id);
}
