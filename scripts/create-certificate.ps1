#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Creates a self-signed certificate for development signing of the sparse package.
    Must match the Publisher in AppxManifest.xml: CN=7ZipExplorerExtension
#>

$ErrorActionPreference = "Stop"
$Subject = "CN=7ZipExplorerExtension"

# Check if cert already exists
$existing = Get-ChildItem Cert:\CurrentUser\My | Where-Object { $_.Subject -eq $Subject }
if ($existing) {
    Write-Host "Certificate already exists: $($existing.Thumbprint)"
    Write-Host "Removing old certificate..."
    $existing | Remove-Item -Force
}

Write-Host "Creating self-signed certificate with subject: $Subject"
$cert = New-SelfSignedCertificate `
    -Type Custom `
    -Subject $Subject `
    -KeyUsage DigitalSignature `
    -FriendlyName "7-Zip Explorer Extension Dev Certificate" `
    -CertStoreLocation "Cert:\CurrentUser\My" `
    -TextExtension @("2.5.29.37={text}1.3.6.1.5.5.7.3.3", "2.5.29.19={text}")

Write-Host "Certificate created: $($cert.Thumbprint)"

# Trust the certificate for sideloading. Only the PUBLIC certificate is placed in
# the TrustedPeople store; the private key stays in CurrentUser\My and is never
# written to disk. install.ps1 signs directly from the store by thumbprint, so
# there is no PFX (and no shared password) that could be recovered to forge a
# package this machine would trust.
Write-Host "Adding certificate to Trusted People store (requires admin)..."
$publicCert = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($cert.RawData)
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($publicCert)
$store.Close()

# Remove any PFX left on disk by older versions of this script.
$oldPfx = Join-Path $PSScriptRoot "dev-cert.pfx"
if (Test-Path $oldPfx) { Remove-Item $oldPfx -Force; Write-Host "Removed stale dev-cert.pfx." }

Write-Host ""
Write-Host "Done! Certificate is trusted for sideloading (private key kept in the store; no PFX on disk)."
