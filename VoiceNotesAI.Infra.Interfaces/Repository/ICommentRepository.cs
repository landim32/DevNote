using VoiceNotesAI.DTOs;

namespace VoiceNotesAI.Repository;

public interface ICommentRepository
{
    Task<List<CommentInfo>> GetByNoteIdAsync(int noteId);
    Task<int> SaveAsync(CommentInfo comment);
    Task<int> DeleteAsync(int id);
    Task<int> DeleteByNoteIdAsync(int noteId);
}
