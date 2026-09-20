[CmdletBinding()]
param([string]$RevitApiDir = "C:\Program Files\Autodesk\Revit 2027")

if ($PSVersionTable.PSEdition -eq "Desktop" -and (Get-Command pwsh -ErrorAction SilentlyContinue)) {
    & pwsh -NoProfile -ExecutionPolicy Bypass -File $PSCommandPath -RevitApiDir $RevitApiDir
    exit $LASTEXITCODE
}

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot "build.ps1") -RevitApiDir $RevitApiDir
if ($LASTEXITCODE -ne 0) { throw "Packaging stopped because the build failed." }

$version = "2.1.0"
$out = Join-Path $root "artifacts\ReVitAI-Bridge-Core-v$version"
$zip = "$out.zip"
$bin = Join-Path $root "artifacts\build\ReVitAI.Bridge\bin"
$resolvedRoot = [System.IO.Path]::GetFullPath($root).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
$resolvedOut = [System.IO.Path]::GetFullPath($out)
if (-not $resolvedOut.StartsWith($resolvedRoot + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to clean a package path outside the repository."
}
if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Recurse -Force }
New-Item -ItemType Directory -Path $out -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $out "docs") -Force | Out-Null

Copy-Item -Path (Join-Path $bin "ReVitAI.Bridge.dll") -Destination $out -Force
Copy-Item -Path (Join-Path $bin "ReVitAI.Bridge.deps.json") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\ReVitAI.Bridge.addin") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\install.ps1") -Destination $out -Force
Copy-Item -Path (Join-Path $root "README.md") -Destination $out -Force
Copy-Item -Path (Join-Path $root "LICENSE") -Destination $out -Force
Copy-Item -Path (Join-Path $root "NOTICE") -Destination $out -Force
Copy-Item -Path (Join-Path $root "THIRD_PARTY_NOTICES.md") -Destination $out -Force
Copy-Item -Path (Join-Path $root "CHANGELOG.md") -Destination $out -Force
Copy-Item -Path (Join-Path $root "docs\*") -Destination (Join-Path $out "docs") -Recurse -Force

$sbom = [ordered]@{
    bomFormat = "CycloneDX"
    specVersion = "1.5"
    version = 1
    metadata = [ordered]@{
        timestamp = (Get-Date).ToUniversalTime().ToString("o")
        component = [ordered]@{
            type = "application"
            name = "ReVitAI Bridge Core"
            version = $version
            licenses = @([ordered]@{ license = [ordered]@{ id = "Apache-2.0" } })
        }
    }
    components = @(
        [ordered]@{
            type = "library"
            name = "ReVitAI.Bridge"
            version = "$version.0"
            licenses = @([ordered]@{ license = [ordered]@{ id = "Apache-2.0" } })
        },
        [ordered]@{
            type = "library"
            name = "Autodesk Revit API"
            version = "2027"
            scope = "optional"
            properties = @([ordered]@{ name = "distribution"; value = "not redistributed" })
        }
    )
}
$sbom | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $out "sbom.cdx.json") -Encoding utf8

$distributedAssemblies = @(Get-ChildItem -LiteralPath $out -Recurse -File -Filter "*.dll")
if ($distributedAssemblies.Count -ne 1 -or $distributedAssemblies[0].Name -ne "ReVitAI.Bridge.dll") {
    throw "Core release must contain exactly one distributed assembly: ReVitAI.Bridge.dll."
}

Compress-Archive -Path (Join-Path $out "*") -DestinationPath $zip -Force
$hash = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $([System.IO.Path]::GetFileName($zip))" | Set-Content -LiteralPath (Join-Path $root "artifacts\SHA256SUMS.txt") -Encoding ascii
Write-Host "Package: $zip"
Write-Host "SHA256: $hash"
