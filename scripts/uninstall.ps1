<#
.SYNOPSIS
    Removes the 7-Zip Explorer Extension package and its dev signing certificate.
#>

$ErrorActionPreference = "Stop"
$PackageName = "7ZipExplorerExtension"

$pkg = Get-AppxPackage -Name $PackageName -ErrorAction SilentlyContinue
if ($pkg) {
    Write-Host "Removing package: $($pkg.PackageFullName)"
    Remove-AppxPackage -Package $pkg.PackageFullName
    Write-Host "Package removed."
} else {
    Write-Host "Package '$PackageName' is not installed."
}

# Remove the dev signing certificate and its machine-wide sideloading trust
# (added by create-certificate.ps1). Removing from LocalMachine\TrustedPeople
# needs admin; if not elevated, clean the user store and warn.
$CertSubject = "CN=7ZipExplorerExtension"
Write-Host "Removing development certificate and trust..."
try {
    Get-ChildItem Cert:\CurrentUser\My |
        Where-Object { $_.Subject -eq $CertSubject } |
        Remove-Item -Force -ErrorAction SilentlyContinue
    $tp = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople", "LocalMachine")
    $tp.Open("ReadWrite")
    foreach ($c in @($tp.Certificates)) {
        if ($c.Subject -eq $CertSubject) { $tp.Remove($c) }
    }
    $tp.Close()
    Write-Host "Certificate trust removed."
} catch {
    Write-Warning "Could not remove machine-wide trust (run as Administrator to finish): $($_.Exception.Message)"
}

$oldPfx = Join-Path $PSScriptRoot "dev-cert.pfx"
if (Test-Path $oldPfx) { Remove-Item $oldPfx -Force -ErrorAction SilentlyContinue; Write-Host "Deleted leftover dev-cert.pfx." }

Write-Host "Restarting Explorer..."
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Start-Process explorer

Write-Host "Done."
