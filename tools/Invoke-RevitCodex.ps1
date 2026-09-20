[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$Instruction,
    [switch]$AutoApprove,
    [string]$Workspace = (Get-Location).Path
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

$register = Join-Path $PSScriptRoot "Register-RevitCodex.ps1"
& powershell -NoProfile -ExecutionPolicy Bypass -File $register -Quiet
if ($LASTEXITCODE -ne 0) {
    throw "Revit MCP registration failed."
}

$codex = Find-CodexExecutable
if (-not $codex) {
    throw "Codex executable was not found."
}

$prompt = @"
Use the revit MCP server in this task.
Call revit_list_tools first, then call only tool names returned by that list.
Do not call execute_code or invent tool names.
For multi-step drawings, call apply_drawing_plan with an ops array and a fixed viewId.
After writes, read back the created or changed elements and report the observed IDs.

Task:
$Instruction
"@

$arguments = @("exec", "--json", "--skip-git-repo-check")
if ($AutoApprove) {
    $arguments += "--dangerously-bypass-approvals-and-sandbox"
}
$arguments += "-"

$arguments | ForEach-Object { Write-Verbose $_ }
$prompt | & $codex @arguments
exit $LASTEXITCODE