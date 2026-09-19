[CmdletBinding()]
param([string]$RevitVersion = "2027")
$ErrorActionPreference = "Stop"
if (Get-Process Revit -ErrorAction SilentlyContinue) { throw "Close Revit before installing." }
$root = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
$payload = Join-Path $PSScriptRoot "RevitAiBatchV1"
New-Item -ItemType Directory -Path $payload -Force | Out-Null
Copy-Item -Path (Join-Path $PSScriptRoot "*.dll") -Destination $payload -Force
Copy-Item -Path (Join-Path $PSScriptRoot "*.json") -Destination $payload -Force
Copy-Item -Path (Join-Path $PSScriptRoot "RevitAiBatch.addin") -Destination (Join-Path $root "RevitAiBatch.addin") -Force
Write-Host "Installed Revit AI Bridge."
