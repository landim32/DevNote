using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoiceNotesAI.Models;
using VoiceNotesAI.Services;

namespace VoiceNotesAI.ViewModels;

public partial class NoteDetailViewModel : ObservableObject, IQueryAttributable
{
    private readonly INoteRepository _noteRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly ISettingsRepository _settingsRepository;
    private readonly IAIService _aiService;
    private readonly ISpeechToTextService _speechToTextService;
    private readonly IAudioService _audioService;

    public NoteDetailViewModel(
        INoteRepository noteRepository,
        ICategoryRepository categoryRepository,
        ICommentRepository commentRepository,
        ISettingsRepository settingsRepository,
        IAIService aiService,
        ISpeechToTextService speechToTextService,
        IAudioService audioService)
    {
        _noteRepository = noteRepository;
        _categoryRepository = categoryRepository;
        _commentRepository = commentRepository;
        _settingsRepository = settingsRepository;
        _aiService = aiService;
        _speechToTextService = speechToTextService;
        _audioService = audioService;
    }

    [ObservableProperty]
    private int _noteId;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _description = string.Empty;

    [ObservableProperty]
    private string _category = string.Empty;

    [ObservableProperty]
    private string _audioFilePath = string.Empty;

    [ObservableProperty]
    private DateTime _createdAt;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private bool _isNewNote = true;

    [ObservableProperty]
    private string _pageTitle = "Nova Nota";

    [ObservableProperty]
    private ObservableCollection<string> _availableCategories = [];

    [ObservableProperty]
    private ObservableCollection<Comment> _comments = [];

    [ObservableProperty]
    private bool _isFabMenuOpen;

    [ObservableProperty]
    private bool _isRecordingComment;

    [ObservableProperty]
    private bool _isTranscribing;

    [ObservableProperty]
    private bool _isConsolidating;

    [ObservableProperty]
    private string _recordingStatus = string.Empty;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Note", out var noteObj) && noteObj is Note note)
        {
            NoteId = note.Id;
            Title = note.Title;
            Description = note.Description;
            Category = note.Category;
            AudioFilePath = note.AudioFilePath;
            CreatedAt = note.CreatedAt;
            IsNewNote = false;
            PageTitle = "Detalhes da Nota";
        }
    }

    [RelayCommand]
    private async Task LoadCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        AvailableCategories = new ObservableCollection<string>(categories.Select(c => c.Name));

        if (IsNewNote && string.IsNullOrEmpty(Category))
        {
            var lastCategory = await _settingsRepository.GetAsync("LastSelectedCategory");
            if (!string.IsNullOrEmpty(lastCategory) && AvailableCategories.Contains(lastCategory))
            {
                Category = lastCategory;
            }
        }
    }

    [RelayCommand]
    private async Task LoadCommentsAsync()
    {
        if (NoteId > 0)
        {
            var comments = await _commentRepository.GetByNoteIdAsync(NoteId);
            Comments = new ObservableCollection<Comment>(comments);
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(Description))
        {
            await Shell.Current.DisplayAlert("Erro", "O conteúdo da nota é obrigatório.", "OK");
            return;
        }

        IsSaving = true;

        try
        {
            var title = Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = Description.Length > 80
                    ? Description[..80] + "..."
                    : Description;
            }

            var note = new Note
            {
                Id = NoteId,
                Title = title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath,
                CreatedAt = IsNewNote ? DateTime.Now : CreatedAt
            };

            await _noteRepository.SaveAsync(note);

            if (!string.IsNullOrEmpty(Category))
            {
                await _settingsRepository.SetAsync("LastSelectedCategory", Category);
            }

            await Shell.Current.GoToAsync("..");
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task EnsureNoteSavedAsync()
    {
        if (NoteId == 0)
        {
            var title = Title;
            if (string.IsNullOrWhiteSpace(title))
            {
                if (string.IsNullOrWhiteSpace(Description))
                    title = "Nova Nota";
                else
                    title = Description.Length > 80 ? Description[..80] + "..." : Description;
            }

            var note = new Note
            {
                Title = title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath
            };

            await _noteRepository.SaveAsync(note);
            NoteId = note.Id;
            IsNewNote = false;
            PageTitle = "Detalhes da Nota";
        }
    }

    [RelayCommand]
    private void ToggleFabMenu()
    {
        IsFabMenuOpen = !IsFabMenuOpen;
    }

    [RelayCommand]
    private async Task AddTextCommentAsync()
    {
        IsFabMenuOpen = false;

        var text = await Shell.Current.DisplayPromptAsync(
            "Novo Comentário",
            "Digite o comentário:",
            "Salvar",
            "Cancelar",
            placeholder: "Seu comentário...");

        if (string.IsNullOrWhiteSpace(text))
            return;

        await EnsureNoteSavedAsync();

        var comment = new Comment
        {
            NoteId = NoteId,
            Content = text
        };

        await _commentRepository.SaveAsync(comment);
        await LoadCommentsAsync();
    }

    [RelayCommand]
    private async Task AddVoiceCommentAsync()
    {
        IsFabMenuOpen = false;

        await EnsureNoteSavedAsync();

        var status = await Permissions.CheckStatusAsync<Permissions.Microphone>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.Microphone>();
            if (status != PermissionStatus.Granted)
            {
                await Shell.Current.DisplayAlert("Permissão negada",
                    "É necessário permissão do microfone para gravar comentários de voz.", "OK");
                return;
            }
        }

        RecordingStatus = "Gravando...";
        IsRecordingComment = true;
        await _audioService.StartRecordingAsync();
    }

    [RelayCommand]
    private async Task StopVoiceCommentAsync()
    {
        IsRecordingComment = false;
        RecordingStatus = "Transcrevendo...";
        IsTranscribing = true;

        try
        {
            var audioPath = await _audioService.StopRecordingAsync();

            if (string.IsNullOrEmpty(audioPath))
            {
                IsTranscribing = false;
                return;
            }

            var transcription = await _speechToTextService.TranscribeAsync(audioPath);

            if (string.IsNullOrWhiteSpace(transcription))
            {
                await Shell.Current.DisplayAlert("Aviso", "Não foi possível transcrever o áudio.", "OK");
                return;
            }

            var comment = new Comment
            {
                NoteId = NoteId,
                Content = transcription
            };

            await _commentRepository.SaveAsync(comment);
            await LoadCommentsAsync();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao transcrever: {ex.Message}", "OK");
        }
        finally
        {
            IsTranscribing = false;
            RecordingStatus = string.Empty;
        }
    }

    [RelayCommand]
    private async Task ConsolidateWithAIAsync()
    {
        IsFabMenuOpen = false;

        if (Comments.Count == 0)
        {
            await Shell.Current.DisplayAlert("Aviso", "Não há comentários para consolidar.", "OK");
            return;
        }

        var confirm = await Shell.Current.DisplayAlert(
            "Consolidar com IA",
            "A IA irá integrar a nota com todos os comentários em um texto consolidado. Os comentários serão removidos após a consolidação. Deseja continuar?",
            "Sim", "Não");

        if (!confirm) return;

        IsConsolidating = true;

        try
        {
            var commentTexts = Comments.Select(c => c.Content).ToList();
            var consolidated = await _aiService.ConsolidateNoteAsync(Description, commentTexts);

            Description = consolidated;
            await _commentRepository.DeleteByNoteIdAsync(NoteId);
            Comments.Clear();

            var note = new Note
            {
                Id = NoteId,
                Title = Title,
                Description = Description,
                Category = Category,
                AudioFilePath = AudioFilePath,
                CreatedAt = CreatedAt
            };

            await _noteRepository.SaveAsync(note);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Erro", $"Erro ao consolidar: {ex.Message}", "OK");
        }
        finally
        {
            IsConsolidating = false;
        }
    }

    [RelayCommand]
    private async Task DeleteCommentAsync(Comment comment)
    {
        await _commentRepository.DeleteAsync(comment.Id);
        Comments.Remove(comment);
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        bool confirm = await Shell.Current.DisplayAlert(
            "Excluir nota",
            $"Deseja excluir \"{Title}\"?",
            "Sim", "Não");

        if (!confirm) return;

        await _commentRepository.DeleteByNoteIdAsync(NoteId);
        await _noteRepository.DeleteAsync(NoteId);
        await Shell.Current.GoToAsync("..");
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        await Shell.Current.GoToAsync("..");
    }
}
