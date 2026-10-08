[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$VendorContracts,
    [string]$VendorToolCatalog,
    [switch]$SkipTests
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

if ([string]::IsNullOrWhiteSpace($VendorContracts)) {
    $VendorContracts = Join-Path $root 'contracts\vendor-tools.json'
}

if ([string]::IsNullOrWhiteSpace($VendorToolCatalog)) {
    $VendorToolCatalog = Join-Path $root 'contracts\tool-contracts.json'
}

$generated = Join-Path $env:TEMP 'revitai-engine-poc\generated'
$toolIndex = Join-Path $generated 'tool-index.json'

Write-Host '== 生成 RevitToolSet：厂商 120 工具契约 -> 工具类 + 工具名索引 ==' -ForegroundColor Cyan
if (-not (Test-Path -LiteralPath $VendorToolCatalog)) {
    throw "找不到厂商工具契约: $VendorToolCatalog (可用 -VendorToolCatalog 指定)"
}
dotnet run --project (Join-Path $root 'tools\ToolSetGenerator\ToolSetGenerator.csproj') `
    -c $Configuration --nologo -- --catalog $VendorToolCatalog --out-tools $generated --namespace RevitToolSet
if ($LASTEXITCODE -ne 0) { throw "工具集生成失败 (exit=$LASTEXITCODE)" }

Write-Host ''
Write-Host '== 构建 (net10.0 / net10.0-windows, Revit 2027 API) ==' -ForegroundColor Cyan
dotnet build (Join-Path $root 'src\RevitAi.Engine.Addin\RevitAi.Engine.Addin.csproj') -c $Configuration --nologo
if ($LASTEXITCODE -ne 0) { throw "构建失败 (exit=$LASTEXITCODE)" }

if (-not $SkipTests) {
    Write-Host ''
    Write-Host '== 冒烟测试 (无 Revit 依赖, 含 120 工具契约对拍) ==' -ForegroundColor Cyan
    dotnet run --project (Join-Path $root 'tests\RevitAi.Engine.SmokeTests\RevitAi.Engine.SmokeTests.csproj') `
        -c $Configuration --nologo -- --vendor $VendorContracts --tool-index $toolIndex
    if ($LASTEXITCODE -ne 0) { throw "冒烟测试失败 (exit=$LASTEXITCODE)" }
}

$output = Join-Path $env:TEMP "revitai-engine-poc\bin\RevitAi.Engine.Addin\$Configuration"
Write-Host ''
Write-Host "构建输出: $output" -ForegroundColor Green
Get-ChildItem $output -File -ErrorAction SilentlyContinue |
    Where-Object { $_.Extension -in '.dll', '.json', '.addin', '.pdb' } |
    Select-Object Name, Length |
    Format-Table -AutoSize |
    Out-String |
    Write-Host
