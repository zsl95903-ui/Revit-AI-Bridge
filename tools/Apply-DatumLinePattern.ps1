[CmdletBinding()]
param(
    [string]$PatternName = "",
    [long]$PatternId = 0
)

$ErrorActionPreference = "Stop"

# Keep this script ASCII-only so Authenticode signing cannot corrupt its
# encoding. U+957F U+865A U+7EBF is the default Chinese pattern name.
if ([string]::IsNullOrWhiteSpace($PatternName)) {
    $PatternName = [string]::Concat(
        [char]0x957F,
        [char]0x865A,
        [char]0x7EBF)
}

$directory = Join-Path $env:LOCALAPPDATA "ReVitAI"
$discoveryPaths = @(
    (Join-Path $directory "revitai-bridge.json"),
    (Join-Path $directory "codex-bridge.json"),
    (Join-Path $directory "bridge.json")
)

$discovery = $null
foreach ($path in $discoveryPaths) {
    if (-not (Test-Path -LiteralPath $path)) {
        continue
    }
    $candidate = Get-Content -Raw -LiteralPath $path | ConvertFrom-Json
    if (-not [string]::IsNullOrWhiteSpace([string]$candidate.pipeName)) {
        $discovery = $candidate
        break
    }
}
if ($null -eq $discovery) {
    throw "No live Revit AI named-pipe discovery file was found."
}

$arguments = @{}
if ($PatternId -gt 0) {
    $arguments.patternId = $PatternId
} else {
    $arguments.patternName = $PatternName
}

$request = @{
    type = "invoke"
    requestId = [Guid]::NewGuid().ToString("N")
    tool = "set_level_line_pattern"
    arguments = $arguments
} | ConvertTo-Json -Depth 10 -Compress

$pipe = [System.IO.Pipes.NamedPipeClientStream]::new(
    ".",
    [string]$discovery.pipeName,
    [System.IO.Pipes.PipeDirection]::InOut)
try {
    $pipe.Connect(15000)
    $utf8 = [System.Text.UTF8Encoding]::new($false)
    $writer = [System.IO.StreamWriter]::new($pipe, $utf8, 4096, $true)
    $reader = [System.IO.StreamReader]::new($pipe, $utf8, $false, 4096, $true)
    try {
        $writer.AutoFlush = $true
        $writer.WriteLine($request)
        $responseText = $reader.ReadLine()
    } finally {
        $writer.Dispose()
        $reader.Dispose()
    }
} finally {
    $pipe.Dispose()
}

if ([string]::IsNullOrWhiteSpace($responseText)) {
    throw "The Revit AI bridge returned no response."
}

$response = $responseText | ConvertFrom-Json
if ($response.success -ne $true) {
    $message = if (-not [string]::IsNullOrWhiteSpace([string]$response.errorMessage)) {
        [string]$response.errorMessage
    } elseif (-not [string]::IsNullOrWhiteSpace([string]$response.errorCode)) {
        [string]$response.errorCode
    } else {
        $responseText
    }
    throw $message
}

Write-Host "Datum line pattern applied."
$response.payload | ConvertTo-Json -Depth 10