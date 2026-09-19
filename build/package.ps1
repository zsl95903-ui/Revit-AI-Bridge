[CmdletBinding()]
param([string]$RevitApiDir = "C:\Program Files\Autodesk\Revit 2027")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot "build.ps1") -RevitApiDir $RevitApiDir
$out = Join-Path $root "artifacts\RevitAI-Bridge-v1.2.5"
$bin = Join-Path $root "src\RevitAiBatch\bin\x64\Release"
if (Test-Path $out) { Remove-Item -LiteralPath $out -Recurse -Force }
New-Item -ItemType Directory -Path $out -Force | Out-Null
Copy-Item -Path (Join-Path $bin "RevitAiBatch.dll") -Destination $out -Force
Copy-Item -Path (Join-Path $bin "RevitAiBatch.deps.json") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\RevitAiBatch.addin") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\install.ps1") -Destination $out -Force
Copy-Item -Path (Join-Path $root "LICENSE") -Destination $out -Force
Copy-Item -Path (Join-Path $root "THIRD_PARTY_NOTICES.md") -Destination $out -Force
Compress-Archive -Path (Join-Path $out "*") -DestinationPath ($out + ".zip") -Force
Write-Host "Package: $out.zip"
