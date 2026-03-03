using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface ICommentRepository
{
    Task<List<Comment>> GetByNoteIdAsync(int noteId);
    Task<int> SaveAsync(Comment comment);
    Task<int> DeleteAsync(int id);
    Task<int> DeleteByNoteIdAsync(int noteId);
}
