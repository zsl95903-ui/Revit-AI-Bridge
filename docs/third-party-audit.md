# Third-Party Audit

Audit date: 2026-09-19
Release: `1.2.5`
Scope: public source repository and the proposed clean source release.

## 1. Public Repository Scan

| File | Third-party reference | License | Publication decision |
|---|---|---|---|
| `src/RevitAiBatch/RevitAiBatch.csproj` | `RevitAPI.dll` | Autodesk proprietary | Build from local installation; do not commit |
| `src/RevitAiBatch/RevitAiBatch.csproj` | `RevitAPIUI.dll` | Autodesk proprietary | Build from local installation; do not commit |
| Public `.cs` files | `System.*` | .NET runtime, MIT | Allowed |
| Public `.cs` files | `Autodesk.Revit.*` | Autodesk proprietary API | Allowed only as a compile-time reference |

The public project has no `PackageReference` entries and no copied third-party source files.

## 2. Files Referencing a Third Party

1. `src/RevitAiBatch/RevitAiBatch.csproj` references `RevitAPI.dll` and `RevitAPIUI.dll` through a configurable `RevitApiDir`.
2. Public C# files import `System.*` and `Autodesk.Revit.*`. They do not contain third-party implementation code.

No Autodesk binary is included in this repository or in the proposed clean package.

## 3. Components Excluded from the Public Core

The following components were observed in older internal packaging work. They are not part of the public source repository and are not approved for the initial clean release without a separate license review.

| Component | Observed version | Upstream license | Decision |
|---|---:|---|---|
| ACadSharp | 3.4.9 | MIT | Excluded from initial clean core |
| Clipper2Lib | 2.0.0 | BSL-1.0 | Excluded from initial clean core |
| DelaunatorSharp | 1.0.11 | Requires release-package review | Not approved |
| EPPlus | 7.0.5 | Commercial terms may apply | Not approved |
| Newtonsoft.Json | 13.0 | MIT | Excluded from initial clean core |
| UglyToad.PdfPig | 0.1.16 | Apache-2.0 | Excluded from initial clean core |
| SkiaSharp | Bundled in an older package | MIT | Not in public core |
| PDFtoImage | Bundled in an older package | Requires release-package review | Not approved |
| pdfium | Bundled in an older package | Requires redistribution review | Not approved |
| WebView2 | Bundled in an older package | Microsoft redistribution terms | Not in public core |
| Microsoft.Extensions.* | Bundled in an older package | MIT | Not in public core |
| System.Drawing.Common | Bundled in an older package | MIT | Not in public core |

## 4. Publication Decision

- The public source repository contains only project-authored source, documentation, and build scripts.
- The initial release package should contain only the project-authored `RevitAiBatch` assembly.
- Autodesk API binaries must not be committed or attached to GitHub Releases.
- Any future binary bundle must be regenerated from an explicit dependency lock and accompanied by an updated notice file, SBOM, and checksums.
- A component with an unclear redistribution term is excluded rather than assumed compatible.

## 5. Pre-Release Checklist

- [x] Scan public tracked files for internal names and private fields.
- [x] Confirm no Autodesk binary is tracked.
- [x] Confirm no NuGet package reference is present.
- [x] Confirm assembly version is `1.2.5.0`.
- [x] Confirm user models and personal absolute paths are absent.
- [x] Build the public project in Release.
- [ ] Add code-signing certificate before a signed release.
- [ ] Generate SBOM and SHA256 checksums for the release asset.
