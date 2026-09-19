[CmdletBinding()]
param([string]$RevitVersion = "2027")
$ErrorActionPreference = "Stop"
if (Get-Process Revit -ErrorAction SilentlyContinue) { throw "Close Revit before installing." }
$root = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
$payload = Join-Path $PSScriptRoot "ReVitAIBridgeV1"
New-Item -ItemType Directory -Path $payload -Force | Out-Null
Copy-Item -Path (Join-Path $PSScriptRoot "*.dll") -Destination $payload -Force
Copy-Item -Path (Join-Path $PSScriptRoot "*.json") -Destination $payload -Force
Copy-Item -Path (Join-Path $PSScriptRoot "ReVitAI.Bridge.addin") -Destination (Join-Path $root "ReVitAI.Bridge.addin") -Force
Write-Host "Installed ReVitAI Bridge."
