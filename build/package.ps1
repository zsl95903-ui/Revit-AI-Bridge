[CmdletBinding()]
param([string]$RevitApiDir = "C:\Program Files\Autodesk\Revit 2027")
$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
& (Join-Path $PSScriptRoot "build.ps1") -RevitApiDir $RevitApiDir
if ($LASTEXITCODE -ne 0) {
    throw "Packaging stopped because the build failed."
}

$version = "1.2.5"
$out = Join-Path $root "artifacts\RevitAI-Bridge-v$version"
$zip = "$out.zip"
$bin = Join-Path $root "src\RevitAiBatch\bin\x64\Release"
$resolvedRoot = [System.IO.Path]::GetFullPath($root).TrimEnd([System.IO.Path]::DirectorySeparatorChar)
$resolvedOut = [System.IO.Path]::GetFullPath($out)
if (-not $resolvedOut.StartsWith($resolvedRoot + [System.IO.Path]::DirectorySeparatorChar, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing to clean a package path outside the repository."
}
if (Test-Path -LiteralPath $out) { Remove-Item -LiteralPath $out -Recurse -Force }
New-Item -ItemType Directory -Path $out -Force | Out-Null
New-Item -ItemType Directory -Path (Join-Path $out "docs") -Force | Out-Null

Copy-Item -Path (Join-Path $bin "RevitAiBatch.dll") -Destination $out -Force
Copy-Item -Path (Join-Path $bin "RevitAiBatch.deps.json") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\RevitAiBatch.addin") -Destination $out -Force
Copy-Item -Path (Join-Path $root "installer\install.ps1") -Destination $out -Force
Copy-Item -Path (Join-Path $root "README.md") -Destination $out -Force
Copy-Item -Path (Join-Path $root "LICENSE") -Destination $out -Force
Copy-Item -Path (Join-Path $root "NOTICE") -Destination $out -Force
Copy-Item -Path (Join-Path $root "THIRD_PARTY_NOTICES.md") -Destination $out -Force
Copy-Item -Path (Join-Path $root "docs\third-party-audit.md") -Destination (Join-Path $out "docs") -Force

$sbom = [ordered]@{
    bomFormat = "CycloneDX"
    specVersion = "1.5"
    version = 1
    metadata = [ordered]@{
        timestamp = (Get-Date).ToUniversalTime().ToString("o")
        component = [ordered]@{
            type = "application"
            name = "Revit AI Bridge"
            version = $version
            licenses = @([ordered]@{ license = [ordered]@{ id = "Apache-2.0" } })
        }
    }
    components = @(
        [ordered]@{
            type = "library"
            name = "RevitAiBatch"
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

Compress-Archive -Path (Join-Path $out "*") -DestinationPath $zip -Force
$hash = (Get-FileHash -LiteralPath $zip -Algorithm SHA256).Hash.ToLowerInvariant()
"$hash  $([System.IO.Path]::GetFileName($zip))" | Set-Content -LiteralPath (Join-Path $root "artifacts\SHA256SUMS.txt") -Encoding ascii
Write-Host "Package: $zip"
Write-Host "SHA256: $hash"
