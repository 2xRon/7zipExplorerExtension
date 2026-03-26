<#
.SYNOPSIS
    Removes the 7-Zip Explorer Extension sparse package.
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

Write-Host "Restarting Explorer..."
Stop-Process -Name explorer -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Start-Process explorer

Write-Host "Done."
