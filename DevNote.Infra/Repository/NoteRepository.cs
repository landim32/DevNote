using AutoMapper;
using DevNote.Context;
using DevNote.DTOs;
using DevNote.Models;

namespace DevNote.Repository;

public class NoteRepository : INoteRepository
{
    private readonly AppDatabase _database;
    private readonly IMapper _mapper;

    public NoteRepository(AppDatabase database, IMapper mapper)
    {
        _database = database;
        _mapper = mapper;
    }

    public async Task<List<NoteInfo>> GetAllAsync()
    {
        var notes = await _database.Connection
            .Table<Note>()
            .Where(n => !n.IsArchived)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<NoteInfo>>(notes);
    }

    public async Task<NoteInfo?> GetByIdAsync(int id)
    {
        var note = await _database.Connection
            .Table<Note>()
            .Where(n => n.Id == id)
            .FirstOrDefaultAsync();
        return note is null ? null : _mapper.Map<NoteInfo>(note);
    }

    public async Task<int> SaveAsync(NoteInfo noteInfo)
    {
        var note = _mapper.Map<Note>(noteInfo);

        if (note.Id != 0)
        {
            await _database.Connection.UpdateAsync(note);
            return note.Id;
        }

        await _database.Connection.InsertAsync(note);
        return note.Id;
    }

    public async Task<int> DeleteAsync(int id)
    {
        return await _database.Connection.DeleteAsync<Note>(id);
    }

    public async Task<List<NoteInfo>> GetByCategoryAsync(string category)
    {
        var notes = await _database.Connection
            .Table<Note>()
            .Where(n => n.Category == category && !n.IsArchived)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<NoteInfo>>(notes);
    }

    public async Task<List<NoteInfo>> GetArchivedAsync()
    {
        var notes = await _database.Connection
            .Table<Note>()
            .Where(n => n.IsArchived)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
        return _mapper.Map<List<NoteInfo>>(notes);
    }

    public async Task ArchiveAsync(int id)
    {
        var note = await _database.Connection.Table<Note>().Where(n => n.Id == id).FirstOrDefaultAsync();
        if (note != null)
        {
            note.IsArchived = true;
            note.UpdatedAt = DateTime.UtcNow;
            await _database.Connection.UpdateAsync(note);
        }
    }

    public async Task RestoreAsync(int id)
    {
        var note = await _database.Connection.Table<Note>().Where(n => n.Id == id).FirstOrDefaultAsync();
        if (note != null)
        {
            note.IsArchived = false;
            note.UpdatedAt = DateTime.UtcNow;
            await _database.Connection.UpdateAsync(note);
        }
    }
}
