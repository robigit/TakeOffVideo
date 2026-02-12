# TakeOffVideo Copilot Instructions

## Project Overview

**TakeOffVideo** is a Blazor WebAssembly application for recording and analyzing athlete takeoff videos in track and field long jump events. The app enables simultaneous video recording from a camera and frame-by-frame analysis of recorded videos.

**Target Framework**: .NET 10.0  
**Deployment**: Azure Static Web Apps

## Build Commands

```bash
# Build the entire solution
dotnet build TakeOffVideo.sln

# Build in Release mode
dotnet build TakeOffVideo.sln -c Release

# Publish the main app (for deployment)
dotnet publish TakeOffVideo/TakeOffVideo.csproj -c Release

# Run the application locally
dotnet run --project TakeOffVideo/TakeOffVideo.csproj
```

**Note**: There are no automated tests in this project.

## Solution Architecture

### Project Structure

```
TakeOffVideo.sln
├── TakeOffVideo/                    # Main Blazor WASM app (UI layer)
├── TakeOffVideo.Library/            # Reusable Razor component library
├── TakeOffVideo.Localization/       # i18n resources (IT/EN)
└── WebStorageManagement/            # Browser storage abstraction
```

### Project Responsibilities

| Project | Purpose |
|---------|---------|
| **TakeOffVideo** | Main application hosting pages (Index, Documentazione, Opzioni, etc.) and top-level components (Recorder, Analyzer) |
| **TakeOffVideo.Library** | Reusable components (`VideoMarker`, `CameraSelector`, `VideoFileManagerTable`, `ModalDialog`), services (`TOVFileManager`), models (`VideoFile`), and utilities (`JSModule`) |
| **TakeOffVideo.Localization** | Localized string resources via `IStringLocalizer<LangResources>` |
| **WebStorageManagement** | Abstracts browser LocalStorage/SessionStorage APIs via `IWebStorageService` |

### Application Flow

The application has a **dual-component architecture** displayed side-by-side on the Index page:

#### RECORDER (Left Side)
1. User selects camera via `CameraSelector`
2. Choose shift/round (Turno: 1-6) and athlete number (Pettorale)
3. Click "Registra" → starts video capture using browser MediaRecorder API
4. Video saved with metadata → `TOVFileManager.AggiungiNuovo()` persists to file system
5. `OnNuovo` event fired → notifies Analyzer to auto-load the new video

**Flow**: Camera → VideoMarker (recording mode) → Blob URL → TOVFileManager → File System

#### ANALYZER (Right Side)
1. `VideoFileManagerTable` displays recorded videos from file system
2. User selects video → `VideoMarker.SetVideo()` loads for playback
3. Features: frame-by-frame navigation, overlay line positioning, image capture
4. Image extraction → `TOVFileManager.SalvaFileSuCartellaImg()` saves to Images folder

**Flow**: Video selection → VideoMarker (playback mode) → Frame extraction → Image save

### Key Data Models

#### VideoFile
Core model representing a recorded video:
```csharp
public class VideoFile
{
    DateTime OraRegistrazione    // Recording timestamp
    string? Url                  // Blob URL or file reference
    string? Tipo                 // Video format (webm, mp4, etc.)
    int Turno                    // Shift/round (1-6, or 0 for test)
    string? Pettorale            // Athlete number
    TimeSpan Durata              // Duration
    bool Pinned                  // Pin to top of list
    string NomeFile              // Computed: TOV_yyyyMMdd_HH-mm-ss_Turno_Pettorale.{Tipo}
}
```

#### AtletaWise
Athlete metadata loaded from DBF files:
```csharp
NOMINATIVO    // Athlete name
PETTO         // Bib number (Pettorale)
CATEG         // Category
CODGARA       // Competition code
CONFERMA      // Confirmed status (S/N)
```

#### GlobalObjects
Singleton service holding shared state:
```csharp
List<AtletaWise> Atleti      // Athletes list from DBF
string? GaraSel              // Selected competition ID
string? NomeFileDbf          // DBF file path
```

## Key Architectural Patterns

### JS Interop Abstraction

The `JSModule` base class provides a reusable pattern for dynamically loading ES6 modules:

```csharp
public abstract class JSModule : IAsyncDisposable
{
    protected JSModule(IJSRuntime js, string moduleUrl)
        => moduleTask = js.InvokeAsync<IJSObjectReference>("import", moduleUrl).AsTask();
    
    protected async ValueTask<T> InvokeAsync<T>(string identifier, params object[]? args)
        => await (await moduleTask).InvokeAsync<T>(identifier, args);
}
```

**Usage**: `TOVFileManager` and other classes extend `JSModule` to call JavaScript functions from corresponding `.js` files in `wwwroot/_content/`.

### Component Polymorphism

**VideoMarker** operates in two modes based on context:
- **Recording mode** (in Recorder): Captures from camera, auto-saves with timeout
- **Playback mode** (in Analyzer): Plays stored videos, enables frame-by-frame controls

The same component provides different UI controls and behaviors depending on which page uses it.

### Event-Driven Communication

`TOVFileManager.OnNuovo` event enables decoupled communication:
```csharp
public event Func<VideoFile, Task>? OnNuovo;
```

When Recorder saves a new video, Analyzer automatically loads it without tight coupling.

### Dual Storage Strategy

- **LocalStorage**: Persistent settings (recording type: `TipoRec`)
- **SessionStorage**: UI state (video marker line position per instance)

Both accessed via `IWebStorageService` dependency injection.

## Localization

Strings are localized using resource files:
- `LangResources.resx` (English, default)
- `LangResources.it.resx` (Italian)

**Usage in components**:
```csharp
@inject IStringLocalizer<LangResources> Loc

<button>@Loc["RecordButton"]</button>
```

## File System Access

Uses browser File System Access API via JavaScript interop:
- `TOVFileManager.ShowDirectoryPicker()` prompts user for directory access
- Videos saved as `TOV_yyyyMMdd_HH-mm-ss_Turno_Pettorale.{Tipo}`
- Images saved to separate Images subfolder

**Compatibility**: Falls back gracefully on unsupported browsers (`TipoScelta.NotSupported`).

## Naming Conventions

### Italian Terms (Domain-Specific)
- **Turno**: Shift/round number (1-6, or 0 for test recordings)
- **Pettorale**: Athlete bib/chest number
- **Atleta**: Athlete
- **Gara**: Competition/race
- **Stacco**: Takeoff (in long jump context)

### File Naming
- Components: PascalCase (`VideoMarker.razor`, `CameraSelector.razor`)
- JavaScript modules: Match C# class names (`TOVFileManager.js`)
- Video files: `TOV_yyyyMMdd_HH-mm-ss_Turno_Pettorale.webm`

## Common Development Patterns

### Dependency Injection in Program.cs

```csharp
builder.Services.AddScoped<TOVFileManager>();
builder.Services.AddSingleton<GlobalObjects>();
builder.Services.AddWebStorageManagement();
builder.Services.AddLocalization();
```

### Component Parameter Binding

VideoMarker and other library components use two-way binding and event callbacks:
```csharp
[Parameter] public EventCallback<VideoFile> OnPlayVideo { get; set; }
```

### Async Component Lifecycle

Components implementing `IAsyncDisposable` must properly clean up JS module references:
```csharp
public async ValueTask DisposeAsync() => await jsModule.DisposeAsync();
```

## Deployment

Automated via GitHub Actions workflow (`.github/workflows/azure-static-web-apps-gentle-sky-02dce4e03.yml`):
- Triggers on push/PR to `master` branch
- Builds and deploys to Azure Static Web Apps
- App location: `TakeOffVideo/`
- Output location: `wwwroot/`
