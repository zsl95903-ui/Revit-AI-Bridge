[CmdletBinding()]
param(
    [string]$Name = "revit",
    [string]$McpServerPath,
    [switch]$Quiet
)

$ErrorActionPreference = "Stop"

function Find-CodexExecutable {
    $local = Join-Path $env:LOCALAPPDATA "OpenAI\Codex\bin"
    if (Test-Path -LiteralPath $local) {
        $candidate = Get-ChildItem -LiteralPath $local -Recurse -File -Filter "codex.exe" |
            Sort-Object LastWriteTimeUtc -Descending |
            Select-Object -First 1
        if ($candidate) { return $candidate.FullName }
    }

    foreach ($name in @("codex.exe", "codex.cmd")) {
        $command = Get-Command $name -ErrorAction SilentlyContinue
        if ($command) { return $command.Source }
    }
    return $null
}

if ([string]::IsNullOrWhiteSpace($McpServerPath)) {
    $root = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot ".."))
    $candidates = @(
        (Join-Path $root "McpServer\RevitAi.McpServer.exe"),
        (Join-Path $root "src\RevitAi.McpServer\bin\Release\net10.0\win-x64\RevitAi.McpServer.exe"),
        (Join-Path $root "src\RevitAi.McpServer\bin\Release\net10.0\RevitAi.McpServer.exe")
    )
    $McpServerPath = $candidates |
        Where-Object { Test-Path -LiteralPath $_ } |
        Select-Object -First 1
}
$McpServerPath = [IO.Path]::GetFullPath($McpServerPath)
if (-not (Test-Path -LiteralPath $McpServerPath)) {
    throw "Revit MCP server was not found: $McpServerPath"
}

$codex = Find-CodexExecutable
if (-not $codex) {
    throw "Codex executable was not found."
}

$existing = & $codex mcp get $Name 2>$null
if ($LASTEXITCODE -eq 0) {
    if (-not $Quiet) {
        Write-Host "Codex MCP '$Name' is already registered."
    }
    return
}

& $codex mcp add $Name -- $McpServerPath
if ($LASTEXITCODE -ne 0) {
    throw "Failed to register Codex MCP '$Name'."
}
if (-not $Quiet) {
    Write-Host "Registered Codex MCP '$Name' -> $McpServerPath"
}