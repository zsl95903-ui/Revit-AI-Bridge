[CmdletBinding()]
param([string]$RevitApiDir = "C:\Program Files\Autodesk\Revit 2027")

if ($PSVersionTable.PSEdition -eq "Desktop" -and (Get-Command pwsh -ErrorAction SilentlyContinue)) {
    & pwsh -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath -RevitApiDir $RevitApiDir
    exit $LASTEXITCODE
}

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "src\ReVitAI.Bridge\ReVitAI.Bridge.csproj"
$buildRoot = Join-Path $root "artifacts\build\ReVitAI.Bridge"
$obj = (Join-Path $buildRoot "obj") + [IO.Path]::DirectorySeparatorChar
$bin = (Join-Path $buildRoot "bin") + [IO.Path]::DirectorySeparatorChar
New-Item -ItemType Directory -Force -Path $obj, $bin | Out-Null

$restoreArgs = @("restore", $project, "-p:BaseIntermediateOutputPath=$obj", "-p:OutputPath=$bin", "--force", "--no-cache")
& dotnet @restoreArgs
if ($LASTEXITCODE -ne 0) { throw "ReVitAI Bridge restore failed." }

$buildArgs = @("build", $project, "-c", "Release", "--no-restore", "-p:BaseIntermediateOutputPath=$obj", "-p:OutputPath=$bin", "-p:RevitApiDir=$RevitApiDir")
& dotnet @buildArgs
if ($LASTEXITCODE -ne 0) { throw "ReVitAI Bridge build failed." }

$mcpProject = Join-Path $root "src\RevitAi.McpServer\RevitAi.McpServer.csproj"
& dotnet build $mcpProject -c Release
if ($LASTEXITCODE -ne 0) { throw "RevitAi MCP Server build failed." }
