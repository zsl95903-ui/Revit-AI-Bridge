[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$RevitVersion = '2027'
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

if (Get-Process Revit -ErrorAction SilentlyContinue) {
    throw '请先关闭 Revit 再安装（插件清单只在启动时读取）。'
}

$buildScript = Join-Path $root 'build.ps1'
& $buildScript -Configuration $Configuration

$source = Join-Path $env:TEMP "revitai-engine-poc\bin\RevitAi.Engine.Addin\$Configuration"
if (-not (Test-Path -LiteralPath $source)) { throw "找不到构建输出: $source" }

$addinsRoot = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$RevitVersion"
$destination = Join-Path $addinsRoot 'RevitAiEngine.PoC'
New-Item -ItemType Directory -Force -Path $destination | Out-Null

# 只拷贝运行时需要的文件（RevitAPI/RevitAPIUI 由 Revit 自身提供，不复制）
Get-ChildItem $source -File |
    Where-Object { $_.Extension -in '.dll', '.json', '.pdb' } |
    Copy-Item -Destination $destination -Force

Copy-Item (Join-Path $source 'RevitAi.Engine.addin') (Join-Path $addinsRoot 'RevitAi.Engine.addin') -Force

Write-Host ''
Write-Host "已安装到: $destination" -ForegroundColor Green
Write-Host "清单: $(Join-Path $addinsRoot 'RevitAi.Engine.addin')" -ForegroundColor Green
Write-Host '启动 Revit 2027 后，在附加模块选项卡的 Revit AI Engine 面板点「引擎自检」。'
Write-Host '自检结果写入 %LOCALAPPDATA%\RevitAiEngine\selftest.json（工具清单为 tools.json）。'
