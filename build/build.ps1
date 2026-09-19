[CmdletBinding()]
param([string]$RevitApiDir = "C:\Program Files\Autodesk\Revit 2027")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
dotnet build (Join-Path $root "src\RevitAiBatch\RevitAiBatch.csproj") -c Release -p:Platform=x64 -p:RevitApiDir="$RevitApiDir"
