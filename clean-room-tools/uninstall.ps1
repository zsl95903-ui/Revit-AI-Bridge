[CmdletBinding()]
param(
    [string]$RevitVersion = '2027'
)

$ErrorActionPreference = 'Stop'

if (Get-Process Revit -ErrorAction SilentlyContinue) {
    throw '请先关闭 Revit 再卸载。'
}

$addinsRoot = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
$destination = Join-Path $addinsRoot 'RevitAiEngine.PoC'
$manifest = Join-Path $addinsRoot 'RevitAi.Engine.addin'

foreach ($path in @($destination, $manifest)) {
    if (Test-Path -LiteralPath $path) {
        Remove-Item -LiteralPath $path -Recurse -Force
        Write-Host "已删除: $path"
    }
}

Write-Host '卸载完成（%LOCALAPPDATA%\RevitAiEngine 的日志与自检结果保留，可自行删除）。'
