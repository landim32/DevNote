using VoiceNotesAI.Models;

namespace VoiceNotesAI.Services;

public interface IAIService
{
    Task<NoteResult> InterpretNoteAsync(string transcribedText);
    Task<string> ConsolidateNoteAsync(string noteContent, List<string> comments);
}
