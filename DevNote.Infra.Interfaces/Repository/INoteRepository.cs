using DevNote.DTOs;

namespace DevNote.Repository;

public interface INoteRepository
{
    Task<List<NoteInfo>> GetAllAsync();
    Task<NoteInfo?> GetByIdAsync(int id);
    Task<int> SaveAsync(NoteInfo note);
    Task<int> DeleteAsync(int id);
    Task<List<NoteInfo>> GetByCategoryAsync(string category);
    Task<List<NoteInfo>> GetArchivedAsync();
    Task ArchiveAsync(int id);
    Task RestoreAsync(int id);
}
