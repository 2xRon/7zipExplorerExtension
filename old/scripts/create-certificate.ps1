#Requires -RunAsAdministrator
<#
.SYNOPSIS
    Creates a self-signed certificate for development signing of the sparse package.
    Must match the Publisher in AppxManifest.xml: CN=7ZipExplorerExtension
#>

$ErrorActionPreference = "Stop"
$Subject = "CN=7ZipExplorerExtension"
$PfxPath = Join-Path $PSScriptRoot "dev-cert.pfx"
$PfxPassword = "7ZipExtDev"

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

# Export PFX
$password = ConvertTo-SecureString -String $PfxPassword -Force -AsPlainText
Export-PfxCertificate -Cert $cert -FilePath $PfxPath -Password $password | Out-Null
Write-Host "Exported PFX to: $PfxPath"

# Trust the certificate for sideloading
Write-Host "Adding certificate to Trusted People store (requires admin)..."
$certFile = [System.Security.Cryptography.X509Certificates.X509Certificate2]::new($PfxPath, $PfxPassword)
$store = New-Object System.Security.Cryptography.X509Certificates.X509Store("TrustedPeople", "LocalMachine")
$store.Open("ReadWrite")
$store.Add($certFile)
$store.Close()

Write-Host ""
Write-Host "Done! Certificate is trusted for sideloading."
Write-Host "PFX password: $PfxPassword"
