# VoiceNotes AI — Notas de Voz Inteligentes

![.NET MAUI](https://img.shields.io/badge/.NET_MAUI-8.0-512BD4?logo=dotnet)
![Android](https://img.shields.io/badge/Android-21%2B-3DDC84?logo=android)
![OpenAI](https://img.shields.io/badge/OpenAI-Whisper%20%2B%20GPT--4-412991?logo=openai)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/landim32/VoiceNotesAI/actions/workflows/build-apk.yml/badge.svg)

## Overview

**VoiceNotes AI** is a .NET MAUI Android app that records voice notes and automatically structures them using OpenAI's **Whisper** (speech-to-text) and **GPT-4** (note interpretation) APIs. Simply tap the microphone, speak, and the app transcribes your audio, interprets the content, and generates a structured note with title, description, and category — all in Portuguese (Brazil).

Built with **MVVM architecture** using CommunityToolkit.Mvvm, SQLite for local storage, and dependency injection throughout.

---

## 🚀 Features

- 🎙️ **Voice Recording** — One-tap recording with automatic processing after stop
- 🧠 **AI Transcription** — Speech-to-text via OpenAI Whisper API
- 📝 **Smart Structuring** — GPT-4 interprets transcriptions and generates titled, categorized notes
- 🗂️ **Category Management** — Full CRUD for custom categories with 6 default seeds
- 🔍 **Category Filtering** — Filter notes by category on the main list
- ⚡ **Auto-Processing** — Records, transcribes, and structures in a single flow (no manual steps)
- ⚙️ **In-App Settings** — Configure OpenAI API key and model preferences at runtime
- 🌙 **Dark Mode** — Full light/dark theme support with brand colors
- 💾 **Offline Storage** — All notes persisted locally in SQLite
- ✏️ **Edit & Delete** — Swipe-to-delete and full note editing with category reassignment

---

## 🛠️ Technologies Used

### Core Framework
- **.NET MAUI 8** — Cross-platform UI framework (targeting Android)
- **CommunityToolkit.Mvvm 8.3** — Source-generated MVVM with `[ObservableProperty]` and `[RelayCommand]`

### AI & Speech
- **OpenAI Whisper API** — Audio transcription (speech-to-text)
- **OpenAI GPT-4o** — Note interpretation and structuring

### Database
- **SQLite** (sqlite-net-pcl 1.9) — Local storage for notes, categories, and settings
- **SQLitePCLRaw.bundle_green** — SQLite native bindings

### Audio
- **Plugin.Maui.Audio 3.0** — Cross-platform audio recording

### Configuration
- **Microsoft.Extensions.Configuration** — Embedded `appsettings.json` with runtime override via SQLite

### Testing
- **xUnit 2.6** — Test framework
- **Moq 4.20** — Mocking library
- **SQLite in-memory** — Temporary databases for repository tests

### CI/CD
- **GitHub Actions** — Automated Android APK builds
- **GitVersion 5.x** — Semantic versioning from git history

---

## 📁 Project Structure

```
VoiceNotesAI/
├── .github/
│   └── workflows/
│       └── build-apk.yml           # CI pipeline: build + release APK
├── VoiceNotesAI/                    # Main MAUI project
│   ├── Converters/                  # IValueConverter implementations
│   ├── Data/
│   │   └── AppDatabase.cs          # SQLite connection + table init + seed
│   ├── Helpers/
│   │   ├── AppSettings.cs          # OpenAISettings POCO
│   │   └── PromptTemplates.cs      # GPT-4 system prompt (Portuguese)
│   ├── Models/
│   │   ├── Note.cs                 # Voice note entity
│   │   ├── NoteResult.cs           # AI response DTO
│   │   ├── Category.cs             # Category entity
│   │   └── AppSetting.cs           # Key-value settings entity
│   ├── Pages/                       # XAML pages (UI layer)
│   │   ├── NoteListPage.xaml        # Main list with category filter + FAB
│   │   ├── RecordingPage.xaml       # Auto-start recording screen
│   │   ├── NoteResultPage.xaml      # AI result review + save
│   │   ├── NoteDetailPage.xaml      # Note edit/view
│   │   ├── CategoryListPage.xaml    # Category CRUD list
│   │   ├── CategoryDetailPage.xaml  # Category edit form
│   │   └── SettingsPage.xaml        # API key + model config
│   ├── Services/                    # Business logic (interface-based)
│   │   ├── IAudioService.cs         # Audio recording contract
│   │   ├── ISpeechToTextService.cs  # Whisper API contract
│   │   ├── IAIService.cs            # GPT-4 API contract
│   │   ├── INoteRepository.cs       # Note CRUD contract
│   │   ├── ICategoryRepository.cs   # Category CRUD contract
│   │   └── ISettingsRepository.cs   # Settings CRUD contract
│   ├── ViewModels/                  # MVVM ViewModels
│   │   ├── NoteListViewModel.cs     # Notes list + filter logic
│   │   ├── RecordingViewModel.cs    # Recording + auto-process
│   │   ├── NoteResultViewModel.cs   # AI result display
│   │   ├── NoteDetailViewModel.cs   # Note editing
│   │   ├── CategoryListViewModel.cs # Category management
│   │   ├── CategoryDetailViewModel.cs
│   │   └── SettingsViewModel.cs     # API settings management
│   ├── Resources/                   # Images, icons, styles, splash
│   ├── Platforms/Android/           # Android-specific config
│   ├── App.xaml                     # Global styles + converters
│   ├── AppShell.xaml                # Flyout navigation (hamburger menu)
│   ├── MauiProgram.cs              # DI registration
│   ├── appsettings.example.json     # Configuration template
│   └── VoiceNotesAI.csproj
├── VoiceNotesAI.Tests/              # Unit tests
│   ├── Services/                    # Repository + API service tests
│   ├── Models/                      # Model tests
│   └── Helpers/                     # Prompt template tests
├── GitVersion.yml                   # Semantic versioning config
├── global.json                      # .NET SDK version pinning
├── VoiceNotesAI.sln
└── README.md
```

---

## ⚙️ Environment Configuration

### 1. Copy the configuration template

```bash
cp VoiceNotesAI/appsettings.example.json VoiceNotesAI/appsettings.json
```

### 2. Edit `appsettings.json`

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key-here",
    "Model": "gpt-4o",
    "WhisperModel": "whisper-1"
  }
}
```

> **Note:** The API key can also be configured at runtime via the in-app **Settings** page (hamburger menu → Configurações). Runtime settings are persisted in SQLite and override `appsettings.json` defaults.

⚠️ **IMPORTANT**:
- Never commit `appsettings.json` with real credentials (it is gitignored)
- Only `appsettings.example.json` is version controlled
- You need a valid [OpenAI API key](https://platform.openai.com/api-keys) with access to Whisper and GPT-4

---

## 🔧 Setup

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- .NET MAUI Android workload
- Android SDK (API 21+)
- Android emulator or physical device

### Installation

#### 1. Clone the repository

```bash
git clone https://github.com/landim32/VoiceNotesAI.git
cd VoiceNotesAI
```

#### 2. Install the MAUI workload

```bash
dotnet workload install maui-android
```

#### 3. Configure the API key

```bash
cp VoiceNotesAI/appsettings.example.json VoiceNotesAI/appsettings.json
# Edit appsettings.json with your OpenAI API key
```

#### 4. Build

```bash
dotnet build VoiceNotesAI/VoiceNotesAI.csproj -f net8.0-android
```

#### 5. Deploy to emulator/device

```bash
dotnet build VoiceNotesAI/VoiceNotesAI.csproj -f net8.0-android -t:Install
adb shell am start -n br.com.emagine.voicenotesai/crc648da53556643be544.MainActivity
```

### Build Release APK

```bash
dotnet publish VoiceNotesAI/VoiceNotesAI.csproj -c Release -f net8.0-android -p:AndroidPackageFormat=apk
```

---

## 🧪 Testing

### Running Tests

**All tests:**
```bash
dotnet test VoiceNotesAI.Tests/VoiceNotesAI.Tests.csproj
```

**Single test by name:**
```bash
dotnet test VoiceNotesAI.Tests/VoiceNotesAI.Tests.csproj --filter "FullyQualifiedName~TestMethodName"
```

### Test Structure

```
VoiceNotesAI.Tests/
├── Services/
│   ├── AIServiceTests.cs              # GPT-4 API mocked HTTP tests
│   ├── SpeechToTextServiceTests.cs    # Whisper API mocked HTTP tests
│   ├── NoteRepositoryTests.cs         # SQLite CRUD tests
│   ├── CategoryRepositoryTests.cs     # Category CRUD tests
│   └── SettingsRepositoryTests.cs     # Key-value settings tests
├── Models/
│   ├── NoteTests.cs                   # Note entity tests
│   └── NoteResultTests.cs            # AI response DTO tests
└── Helpers/
    └── PromptTemplatesTests.cs        # Prompt generation tests
```

### Test Patterns

- **Service tests** mock `HttpMessageHandler` to simulate OpenAI API responses
- **Repository tests** use temporary SQLite databases with `IAsyncLifetime` for setup/teardown
- Source files are linked (not referenced) from the main project to avoid MAUI TFM incompatibility

---

## 🔄 CI/CD

### GitHub Actions

**Workflow:** `.github/workflows/build-apk.yml`

**Triggers:**
- `workflow_dispatch` (manual)
- After `Create Release` workflow completes

**Pipeline steps:**
1. Checkout code
2. Setup .NET 8 SDK (pinned via `global.json`)
3. Install `maui-android` workload
4. Determine version with GitVersion
5. Restore, build, and publish Android APK
6. Upload APK as artifact (90-day retention)
7. Attach APK to GitHub Release (if available)

**Versioning:** Semantic versioning via [GitVersion](https://gitversion.net/) with branch-based policies (main = patch, develop = minor, feature = minor + alpha tag).

---

## 🔍 Troubleshooting

### Common Issues

#### NETSDK1202: net8.0-android is out of support

**Cause:** .NET 10+ SDK treats net8.0 as EOL.

**Solution:** Ensure `global.json` pins the SDK and add `<CheckEolTargetFramework>false</CheckEolTargetFramework>` to the `.csproj`:
```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "latestFeature"
  }
}
```

#### appsettings.json not found during build

**Cause:** The file is gitignored and must be created manually.

**Solution:**
```bash
cp VoiceNotesAI/appsettings.example.json VoiceNotesAI/appsettings.json
```

#### Audio recording not working on emulator

**Cause:** Android emulator may not have microphone permissions granted.

**Solution:** Grant microphone permission in the emulator's app settings, or test on a physical device.

---

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

### Development Setup

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Make your changes
4. Run tests (`dotnet test VoiceNotesAI.Tests/VoiceNotesAI.Tests.csproj`)
5. Commit your changes (`git commit -m 'feat: add some AmazingFeature'`)
6. Push to the branch (`git push origin feature/AmazingFeature`)
7. Open a Pull Request

### Commit Convention

This project uses [GitVersion](https://gitversion.net/) for automatic semantic versioning:
- `feat:` / `feature:` — minor version bump
- `fix:` / `patch:` — patch version bump
- `major:` / `breaking:` — major version bump

---

## 👨‍💻 Author

Developed by **[Rodrigo Landim Carneiro](https://github.com/landim32)**

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

## 🙏 Acknowledgments

- Built with [.NET MAUI](https://dotnet.microsoft.com/apps/maui)
- AI powered by [OpenAI](https://openai.com/) (Whisper + GPT-4)
- MVVM toolkit by [CommunityToolkit.Mvvm](https://github.com/CommunityToolkit/dotnet)
- Audio recording by [Plugin.Maui.Audio](https://github.com/jfversluis/Plugin.Maui.Audio)
- Local storage by [sqlite-net](https://github.com/praeclarum/sqlite-net)

---

## 📞 Support

- **Issues**: [GitHub Issues](https://github.com/landim32/VoiceNotesAI/issues)

---

**⭐ If you find this project useful, please consider giving it a star!**
