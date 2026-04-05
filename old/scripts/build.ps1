<#
.SYNOPSIS
    Builds the 7ZipMenu.dll using MSBuild from Visual Studio 2026.
#>

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
$VcxprojPath = Join-Path $ProjectRoot "7ZipMenu.vcxproj"

# Find MSBuild via vswhere
$vswhere = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
if (-not (Test-Path $vswhere)) {
    Write-Error "vswhere.exe not found. Is Visual Studio installed?"
    exit 1
}

$vsPath = & $vswhere -latest -property installationPath
$msbuild = Join-Path $vsPath "MSBuild\Current\Bin\MSBuild.exe"

if (-not (Test-Path $msbuild)) {
    Write-Error "MSBuild.exe not found at $msbuild"
    exit 1
}

Write-Host "Using MSBuild: $msbuild"
Write-Host "Building Release|x64..."

& $msbuild $VcxprojPath /p:Configuration=Release /p:Platform=x64 /m /v:minimal
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed with exit code $LASTEXITCODE"
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
