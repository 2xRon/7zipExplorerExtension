#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Builds the sparse package, signs it, and registers it with Windows.
    The DLL must already be built (run build.ps1 first).
#>

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
$BuildDir = Join-Path $ProjectRoot "build\Release"
$SparseDir = Join-Path $ProjectRoot "sparse-package"
$ManifestDir = Join-Path $ProjectRoot "manifest"
$ScriptsDir = $PSScriptRoot

$DllPath = Join-Path $BuildDir "7ZipMenu.dll"
$MsixPath = Join-Path $BuildDir "7ZipExplorerExtension.msix"
$PfxPath = Join-Path $ScriptsDir "dev-cert.pfx"
$PfxPassword = "7ZipExtDev"

# --- Validate prerequisites ---

if (-not (Test-Path $DllPath)) {
    Write-Error "7ZipMenu.dll not found at $DllPath. Run build.ps1 first."
    exit 1
}

if (-not (Test-Path $PfxPath)) {
    Write-Error "dev-cert.pfx not found. Run create-certificate.ps1 first."
    exit 1
}

# --- Prepare sparse package staging directory ---

Write-Host "Preparing sparse package..."

# Copy manifest
Copy-Item (Join-Path $ManifestDir "AppxManifest.xml") $SparseDir -Force

# Ensure Assets folder exists with placeholder
$assetsDir = Join-Path $SparseDir "Assets"
if (-not (Test-Path $assetsDir)) {
    New-Item -ItemType Directory -Path $assetsDir -Force | Out-Null
}

# --- Find Windows SDK tools ---

$sdkPath = Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\Windows Kits\Installed Roots" -Name KitsRoot10 -ErrorAction SilentlyContinue
if (-not $sdkPath) {
    Write-Error "Windows SDK not found. Install Windows 10/11 SDK."
    exit 1
}

$sdkRoot = $sdkPath.KitsRoot10
$sdkBinVersions = Get-ChildItem (Join-Path $sdkRoot "bin") -Directory | Sort-Object Name -Descending
$sdkBin = $null
foreach ($ver in $sdkBinVersions) {
    $makeappx = Join-Path $ver.FullName "x64\makeappx.exe"
    if (Test-Path $makeappx) {
        $sdkBin = Join-Path $ver.FullName "x64"
        break
    }
}

if (-not $sdkBin) {
    Write-Error "Could not find makeappx.exe in Windows SDK."
    exit 1
}

$makeappxExe = Join-Path $sdkBin "makeappx.exe"
$signtoolExe = Join-Path $sdkBin "signtool.exe"

Write-Host "Using SDK tools from: $sdkBin"

# --- Remove existing package if installed ---

$existingPkg = Get-AppxPackage -Name "7ZipExplorerExtension" -ErrorAction SilentlyContinue
if ($existingPkg) {
    Write-Host "Removing existing package..."
    Remove-AppxPackage -Package $existingPkg.PackageFullName
    Start-Sleep -Seconds 2
}

# --- Build MSIX ---

Write-Host "Building sparse package..."
if (Test-Path $MsixPath) { Remove-Item $MsixPath -Force }

& $makeappxExe pack /o /d $SparseDir /nv /p $MsixPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "makeappx failed with exit code $LASTEXITCODE"
    exit 1
}

# --- Sign MSIX ---

Write-Host "Signing package..."
& $signtoolExe sign /fd SHA256 /a /f $PfxPath /p $PfxPassword $MsixPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "signtool failed with exit code $LASTEXITCODE"
    exit 1
}

# --- Register sparse package ---

Write-Host "Registering sparse package with external location: $BuildDir"
Add-AppxPackage -Path $MsixPath -ExternalLocation $BuildDir

Write-Host ""
Write-Host "Installation complete!"
Write-Host "Restarting Explorer to load the extension..."

Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Start-Process explorer

Write-Host "Done! Right-click any file in Explorer to see the 7-Zip context menu."
