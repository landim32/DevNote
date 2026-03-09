using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevNote.DTOs;
using DevNote.Services.Interfaces;

namespace DevNote.ViewModels;

public partial class NoteListViewModel : ObservableObject
{
    private readonly INoteService _noteService;

    public NoteListViewModel(INoteService noteService)
    {
        _noteService = noteService;
    }

    [ObservableProperty]
    private ObservableCollection<NoteInfo> _notes = [];

    [ObservableProperty]
    private ObservableCollection<string> _filterCategories = [];

    [ObservableProperty]
    private string _selectedCategory = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private bool _isEmpty;

    [ObservableProperty]
    private bool _isFabMenuOpen;

    [ObservableProperty]
    private bool _isShowingArchived;

    [RelayCommand]
    private void ToggleFabMenu()
    {
        IsFabMenuOpen = !IsFabMenuOpen;
    }

    [RelayCommand]
    private async Task GoToNewTextNoteAsync()
    {
        IsFabMenuOpen = false;
        await Shell.Current.GoToAsync("NoteDetailPage");
    }

    [RelayCommand]
    private async Task LoadNotesAsync()
    {
        IsLoading = true;

        try
        {
            await LoadCategoriesAsync();

            List<NoteInfo> noteList;
            if (SelectedCategory == "__archived__")
            {
                noteList = await _noteService.GetArchivedAsync();
                IsShowingArchived = true;
            }
            else
            {
                noteList = string.IsNullOrEmpty(SelectedCategory)
                    ? await _noteService.GetAllAsync()
                    : await _noteService.GetByCategoryAsync(SelectedCategory);
                IsShowingArchived = false;
            }

            Notes = new ObservableCollection<NoteInfo>(noteList);
            IsEmpty = Notes.Count == 0;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadCategoriesAsync()
    {
        var categoryNames = await _noteService.GetAllCategoryNamesAsync();
        FilterCategories = new ObservableCollection<string>(
            new[] { "", "__archived__" }.Concat(categoryNames)
        );
    }

    [RelayCommand]
    private async Task GoToRecordingAsync()
    {
        IsFabMenuOpen = false;
        await Shell.Current.GoToAsync("RecordingPage");
    }

    [RelayCommand]
    private async Task GoToDetailAsync(NoteInfo note)
    {
        var parameters = new Dictionary<string, object> { { "NoteInfo", note } };
        await Shell.Current.GoToAsync("NoteDetailPage", parameters);
    }

    [RelayCommand]
    private async Task DeleteNoteAsync(NoteInfo note)
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir nota",
            $"Deseja excluir \"{note.Title}\" permanentemente?",
            "Sim", "Não");

        if (!confirm) return;

        await _noteService.DeleteAsync(note.Id);
        Notes.Remove(note);
        IsEmpty = Notes.Count == 0;
    }

    [RelayCommand]
    private async Task ArchiveNoteAsync(NoteInfo note)
    {
        await _noteService.ArchiveAsync(note.Id);
        Notes.Remove(note);
        IsEmpty = Notes.Count == 0;
    }

    [RelayCommand]
    private async Task FilterByCategoryAsync(string category)
    {
        SelectedCategory = category;
        await LoadNotesAsync();
    }
}
