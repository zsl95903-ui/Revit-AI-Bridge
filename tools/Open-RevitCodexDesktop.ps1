[CmdletBinding()]
param([string]$Workspace = (Get-Location).Path)

$ErrorActionPreference = "Stop"
$register = Join-Path $PSScriptRoot "Register-RevitCodex.ps1"
& powershell -NoProfile -ExecutionPolicy Bypass -File $register -Quiet

$local = Join-Path $env:LOCALAPPDATA "OpenAI\Codex\bin"
$codex = Get-ChildItem -LiteralPath $local -Recurse -File -Filter "codex.exe" -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTimeUtc -Descending |
    Select-Object -First 1
if (-not $codex) {
    $command = Get-Command codex.exe -ErrorAction SilentlyContinue
    if (-not $command) { throw "Codex executable was not found." }
    $codexPath = $command.Source
} else {
    $codexPath = $codex.FullName
}

Start-Process -FilePath $codexPath -ArgumentList @("app", $Workspace)