# 7-Zip Explorer Extension

A Windows 11 context menu shell extension that integrates [7-Zip](https://www.7-zip.org/) into the **modern right-click menu** — no more digging through "Show more options."

![Windows 11](https://img.shields.io/badge/Windows%2011-0078D4?style=flat&logo=windows11&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET%2010-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=csharp&logoColor=white)
![License](https://img.shields.io/badge/license-MIT-green)

## Features

- **Modern context menu** — appears in the top-level Windows 11 right-click menu
- **Dynamic labels** — shows actual filenames (e.g., `Extract to "project\"`, `Add to "photos.7z"`)
- **Smart visibility** — extract/test commands only appear for archive files
- **Full 7-Zip integration** — delegates to `7zG.exe`, `7zFM.exe`, and `7z.exe`
### Context Menu Items

| Command | Description |
|---------|-------------|
| Open archive | Opens in 7-Zip File Manager |
| Extract files... | Extract with options dialog |
| Extract Here | Extract to current directory |
| Extract to "name\\" | Extract to a named subfolder |
| Test archive | Verify archive integrity |
| Add to archive... | Compress with options dialog |
| Compress and email... | Compress and attach to email |
| Add to "name.7z" | Quick compress to .7z |
| Add to "name.zip" | Quick compress to .zip |
| Compress to "name.7z" and email | Compress to .7z and email |
| Compress to "name.zip" and email | Compress to .zip and email |

## Prerequisites

- **Windows 11** (build 19041+)
- **7-Zip** installed ([download](https://www.7-zip.org/download.html))
- **.NET 10 SDK**
- **Visual Studio 2022/2026** (or Build Tools) with the **Desktop development with C++** workload — Native AOT links the native image with the MSVC toolchain
- **Windows 10/11 SDK** — provides `makeappx.exe` / `signtool.exe` used by `install.ps1`

## Building

### Using the build script (recommended)

```powershell
.\scripts\build.ps1
```

This publishes the Native AOT COM DLL to `build\Release\7ZipMenu.dll`. It works from any shell — Native AOT locates the MSVC linker itself. The MSIX sparse package is assembled later by `install.ps1`.

### Using dotnet directly

```powershell
dotnet publish src\7ZipMenu\7ZipMenu.csproj /p:Configuration=Release /p:Platform=x64
```

Native AOT compiles the DLL to native code, so the **Desktop development with C++** workload (MSVC toolchain) must be installed.

### Using Visual Studio

Open `7ZipMenu.slnx` in Visual Studio 2022/2026 and build. The solution contains three projects:

| Project | Output | Purpose |
|---------|--------|---------|
| 7ZipMenu | `7ZipMenu.dll` | COM DLL shell extension (Native AOT) |
| 7ZipMenuStub | `7ZipMenuStub.exe` | Stub required by the MSIX schema |
| Package | `.msix` | Windows Application Packaging Project |

## Installation

### 1. Create a development certificate (first time only)

Run as Administrator:

```powershell
.\scripts\create-certificate.ps1
```

This creates a self-signed certificate and trusts it for sideloading.

### 2. Install the extension

Run as Administrator:

```powershell
.\scripts\install.ps1
```

This will:
1. Package the sparse MSIX
2. Sign it with the dev certificate
3. Register it with Windows
4. Restart Explorer

### 3. Verify

Right-click any file in Explorer — you should see **7-Zip** in the modern context menu.

## Uninstallation

```powershell
.\scripts\uninstall.ps1
```

## How It Works

The extension is a **COM DLL** (`7ZipMenu.dll`) written in C# and compiled ahead-of-time with **.NET Native AOT**. It implements the `IExplorerCommand` interface via source-generated COM interop (`[GeneratedComInterface]` / `[GeneratedComClass]`), so the target machine needs no .NET runtime installed. It registers via a **sparse MSIX package** to gain the app identity required by Windows 11's modern context menu.

```
Explorer (right-click)
  └─ Loads 7ZipMenu.dll (in-process COM)
       └─ IExplorerCommand::EnumSubCommands → returns all menu items
       └─ IExplorerCommand::GetTitle → dynamic labels from IShellItemArray
       └─ IExplorerCommand::GetState → hides extract items for non-archives
       └─ IExplorerCommand::Invoke → launches 7zG.exe / 7zFM.exe / 7z.exe
```

### Architecture

| Component | Purpose |
|-----------|---------|
| `ExplorerCommandBase` | Base class with shared IExplorerCommand implementation |
| `SevenZipRootCommand` | Top-level "7-Zip" cascading menu (COM-registered CLSID) |
| `SubCommands` | 12 leaf commands with dynamic titles and smart visibility |
| `CommandEnumerator` | `IEnumExplorerCommand` for Explorer to discover subcommands |
| `SevenZipUtils` | Finds 7-Zip installation via registry, detects archive file types |
| Sparse MSIX | Provides app identity for Windows 11 modern menu registration |

### 7-Zip Detection

The extension searches for 7-Zip in this order:
1. Registry: `HKLM\SOFTWARE\7-Zip\Path64` / `Path`
2. Registry: `HKCU\SOFTWARE\7-Zip\Path64` / `Path`
3. `C:\Program Files\7-Zip\`
4. `C:\Program Files (x86)\7-Zip\`

## Project Structure

```
├── 7ZipMenu.slnx               # Visual Studio solution (XML format)
├── src/
│   ├── 7ZipMenu/               # COM DLL shell extension (C# / Native AOT)
│   │   ├── 7ZipMenu.csproj
│   │   ├── DllExports.cs           # DllGetClassObject / DllCanUnloadNow exports
│   │   ├── ClassFactory.cs         # IClassFactory implementation
│   │   ├── ExplorerCommandBase.cs  # Base IExplorerCommand implementation
│   │   ├── SevenZipRootCommand.cs  # Top-level "7-Zip" cascading menu
│   │   ├── CommandEnumerator.cs    # IEnumExplorerCommand
│   │   ├── SevenZipUtils.cs         # 7-Zip path discovery + archive detection
│   │   ├── NativeMethods/           # COM interface & enum definitions
│   │   └── SubCommands/             # Leaf commands (extract / compress / etc.)
│   ├── 7ZipMenuStub/           # Stub executable required by the MSIX schema
│   └── Package/                # Windows Application Packaging Project (.wapproj)
├── manifest/
│   ├── AppxManifest.xml        # Sparse package manifest
│   └── dllmanifest.manifest    # Embedded activation manifest
├── scripts/
│   ├── build.ps1               # Build the Native AOT DLL via `dotnet publish`
│   ├── create-certificate.ps1  # Dev certificate setup
│   ├── install.ps1             # Package, sign, and register
│   └── uninstall.ps1           # Remove extension
├── sparse-package/             # MSIX staging directory
└── old/                        # Legacy C++/WRL implementation (archived)
```

## License

MIT
