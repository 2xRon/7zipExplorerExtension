# 7-Zip Explorer Extension

A Windows 11 context menu shell extension that integrates [7-Zip](https://www.7-zip.org/) into the **modern right-click menu** — no more digging through "Show more options."

![Windows 11](https://img.shields.io/badge/Windows%2011-0078D4?style=flat&logo=windows11&logoColor=white)
![C++17](https://img.shields.io/badge/C%2B%2B17-00599C?style=flat&logo=c%2B%2B&logoColor=white)
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
- **Visual Studio 2022/2026** with "Desktop development with C++" workload
- **Windows 10/11 SDK**

## Building

### Using the build script

```powershell
.\scripts\build.ps1
```

### Using MSBuild directly

```powershell
msbuild 7ZipExplorerExtension.sln -p:Configuration=Release -p:Platform=x64
```

This builds the COM DLL (`build\Release\7ZipMenu.dll`), the stub executable, and the MSIX package.

### Using Visual Studio

Open `7ZipExplorerExtension.sln` in Visual Studio 2022/2026. Set the **Package** project as the startup project and build. The solution contains three projects:

| Project | Output | Purpose |
|---------|--------|---------|
| 7ZipMenu | `7ZipMenu.dll` | COM DLL shell extension |
| 7ZipMenuStub | `7ZipMenuStub.exe` | Stub required by MSIX schema |
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

The extension is a **COM DLL** (`7ZipMenu.dll`) that implements the `IExplorerCommand` interface using WRL (Windows Runtime C++ Template Library). It registers via a **sparse MSIX package** to gain the app identity required by Windows 11's modern context menu.

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
├── 7ZipExplorerExtension.sln   # Visual Studio solution
├── 7ZipMenu.vcxproj            # COM DLL project
├── 7ZipMenuStub.vcxproj        # Stub executable project
├── Package/
│   ├── Package.wapproj         # Windows Application Packaging Project
│   ├── Package.appxmanifest    # Full MSIX manifest
│   └── Images/                 # Package logo assets
├── src/
│   ├── dllmain.cpp             # DLL entry points
│   ├── framework.h             # Common includes
│   ├── ExplorerCommand.h/cpp   # Base IExplorerCommand class
│   ├── RootCommand.h/cpp       # Top-level cascading menu
│   ├── SubCommands.h/cpp       # All leaf commands
│   ├── CommandEnumerator.h/cpp # IEnumExplorerCommand
│   ├── SevenZipUtils.h/cpp     # 7-Zip path discovery
│   ├── Source.def              # DLL exports
│   └── 7ZipMenu.rc            # Resources + embedded manifest
├── manifest/
│   ├── AppxManifest.xml        # Sparse package manifest
│   └── dllmanifest.manifest    # Embedded MSIX identity
├── scripts/
│   ├── build.ps1               # Build script
│   ├── create-certificate.ps1  # Dev certificate setup
│   ├── install.ps1             # Package, sign, and register
│   └── uninstall.ps1           # Remove extension
└── sparse-package/             # MSIX staging directory
```

## License

MIT
