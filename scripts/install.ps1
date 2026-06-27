#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Builds the MSIX package (7ZipMenu.dll + stub bundled in), signs it from the
    cert store, and registers it with Windows. Run build.ps1 first.
#>

$ErrorActionPreference = "Stop"
$ProjectRoot = Split-Path $PSScriptRoot -Parent
$BuildDir = Join-Path $ProjectRoot "build\Release"
$SparseDir = Join-Path $ProjectRoot "sparse-package"
$ManifestDir = Join-Path $ProjectRoot "manifest"
$ScriptsDir = $PSScriptRoot

$DllPath = Join-Path $BuildDir "7ZipMenu.dll"
$StubPath = Join-Path $BuildDir "7ZipMenuStub.exe"
$MsixPath = Join-Path $BuildDir "7ZipExplorerExtension.msix"
$SignSubject = "CN=7ZipExplorerExtension"

# --- Validate prerequisites ---

if (-not (Test-Path $DllPath)) {
    Write-Error "7ZipMenu.dll not found at $DllPath. Run build.ps1 first."
    exit 1
}

if (-not (Test-Path $StubPath)) {
    Write-Error "7ZipMenuStub.exe not found at $StubPath. Run build.ps1 first."
    exit 1
}

$cert = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq $SignSubject } | Select-Object -First 1
if (-not $cert) {
    Write-Error "Signing certificate '$SignSubject' not found in CurrentUser\My. Run create-certificate.ps1 first."
    exit 1
}

# --- Prepare sparse package staging directory ---

Write-Host "Preparing package..."

# Copy manifest
Copy-Item (Join-Path $ManifestDir "AppxManifest.xml") $SparseDir -Force

# Ensure Assets folder exists with placeholder
$assetsDir = Join-Path $SparseDir "Assets"
if (-not (Test-Path $assetsDir)) {
    New-Item -ItemType Directory -Path $assetsDir -Force | Out-Null
}

# Bundle the actual binaries INSIDE the package so the signed MSIX and its block
# map cover the executable code. The DLL then loads from the locked WindowsApps
# location instead of an external, user-writable folder.
Copy-Item $DllPath $SparseDir -Force
Copy-Item $StubPath $SparseDir -Force

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

Write-Host "Building MSIX package..."
if (Test-Path $MsixPath) { Remove-Item $MsixPath -Force }

& $makeappxExe pack /o /d $SparseDir /nv /p $MsixPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "makeappx failed with exit code $LASTEXITCODE"
    exit 1
}

# --- Sign MSIX ---

Write-Host "Signing package with certificate $($cert.Thumbprint)..."
& $signtoolExe sign /fd SHA256 /sha1 $($cert.Thumbprint) $MsixPath
if ($LASTEXITCODE -ne 0) {
    Write-Error "signtool failed with exit code $LASTEXITCODE"
    exit 1
}

# --- Register sparse package ---

Write-Host "Registering package (binaries bundled in the signed MSIX)..."
Add-AppxPackage -Path $MsixPath

Write-Host ""
Write-Host "Installation complete!"
Write-Host "Restarting Explorer to load the extension..."

Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Start-Process explorer

Write-Host "Done! Right-click any file in Explorer to see the 7-Zip context menu."
