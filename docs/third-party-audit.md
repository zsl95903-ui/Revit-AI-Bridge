# Third-Party Audit

Audit date: 2026-09-19
Release: `1.4.0`
Scope: ReVitAI Bridge source and release packages for the Revit-AI-Bridge Agent workflow.

## 1. Repository References

| File | Reference | License | Distribution decision |
|---|---|---|---|
| `src/ReVitAI.Bridge/ReVitAI.Bridge.csproj` | `RevitAPI.dll` | Autodesk proprietary | Build from local installation; do not redistribute |
| `src/ReVitAI.Bridge/ReVitAI.Bridge.csproj` | `RevitAPIUI.dll` | Autodesk proprietary | Build from local installation; do not redistribute |
| Project C# files | `System.*` | .NET runtime license | Used as platform libraries |
| Project C# files | `Autodesk.Revit.*` | Autodesk proprietary API | Compile-time reference only |

## 2. Package Boundary

- The source repository contains project-authored source, documentation, and build scripts.
- The release package contains one project-authored assembly: `ReVitAI.Bridge.dll`.
- Autodesk API binaries are not included.
- No NuGet `PackageReference` is required.
- No third-party source code is copied into the repository.

## 3. Release Checklist

- [x] Confirm the assembly name and namespace are `ReVitAI.Bridge`.
- [x] Confirm release assembly version is `1.4.0.0`.
- [x] Confirm the release package contains one distributed assembly.
- [x] Confirm Autodesk API binaries are not packaged.
- [x] Confirm no local model, log, screenshot, or export file is packaged.
- [x] Confirm no credential or test endpoint is packaged.
- [x] Generate SBOM and SHA256 checksums.
- [ ] Add code-signing certificate before a signed release.

## 4. Redistribution Rule

Any future third-party binary must be reviewed before packaging. A component with unclear redistribution terms must not be added without a separate audit and updated notice file.
