<#
.SYNOPSIS
    Builds the 7ZipMenu.dll (Native AOT) via `dotnet publish`.
    Works from any shell - no need to open a Developer PowerShell.
#>

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
$projPath = Join-Path $ProjectRoot "src\7ZipMenu\7ZipMenu.csproj"

# Native AOT linking shells out to vswhere.exe (via the ILCompiler's
# findvcvarsall.bat) to locate the MSVC linker. vswhere lives in the VS
# Installer directory, which is not on PATH by default - so a plain-shell
# build fails with "'vswhere.exe' is not recognized". Put it on PATH here.
if (-not (Get-Command vswhere.exe -ErrorAction SilentlyContinue)) {
    $programFilesX86 = ${env:ProgramFiles(x86)}
    $installerDir = if ($programFilesX86) { Join-Path $programFilesX86 "Microsoft Visual Studio\Installer" } else { $null }
    if ($installerDir -and (Test-Path (Join-Path $installerDir "vswhere.exe"))) {
        Write-Host "Adding VS Installer to PATH (for vswhere.exe): $installerDir"
        $env:PATH = "$installerDir;$env:PATH"
    } else {
        Write-Warning "vswhere.exe not found. Native AOT linking needs Visual Studio with the C++ (Desktop development with C++) workload. Build may fail."
    }
}

Write-Host "Building Release|x64..."

& dotnet publish $projPath /p:Configuration=Release /p:Platform=x64 /m /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
    exit 1
}

# Build the stub executable. It is bundled INSIDE the signed MSIX (the manifest's
# windows.fullTrustApplication entry point) next to 7ZipMenu.dll, so the package
# signature covers all executable code.
$stubProj = Join-Path $ProjectRoot "src\7ZipMenuStub\7ZipMenuStub.csproj"
$buildRelease = Join-Path $ProjectRoot "build\Release"
Write-Host "Building stub executable (7ZipMenuStub.exe)..."
& dotnet publish $stubProj /p:Configuration=Release /p:Platform=x64 -o $buildRelease /m /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Stub build failed with exit code $LASTEXITCODE"
    exit 1
}

$dllPath = Join-Path $ProjectRoot "build\Release\7ZipMenu.dll"
if (Test-Path $dllPath) {
    Write-Host ""
    Write-Host "Build successful: $dllPath"
    Write-Host "Run install.ps1 (as admin) to register the extension."
} else {
    Write-Error "DLL not found at expected location: $dllPath"
}
