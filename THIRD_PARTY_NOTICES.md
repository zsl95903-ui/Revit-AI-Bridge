# Third-Party Notices

The source repository does not vendor third-party source code.

## External Build Dependency

- Autodesk Revit API (`RevitAPI.dll`, `RevitAPIUI.dll`)
  - Proprietary Autodesk dependency.
  - Not distributed by this repository.
  - Users must provide it from a local Autodesk Revit installation.

## Published Source

The public source project references only:

- `System.*` from the .NET runtime.
- `Autodesk.Revit.*` from the locally installed Revit API.

There are no NuGet `PackageReference` entries and no vendored third-party source files in the initial source release.

## Release Packages

Any future release package that includes a third-party binary must add the component name, version, source URL, license, and redistribution status here before release. The initial clean core package contains only the project-authored `ReVitAI.Bridge` assembly and installer scripts.

See [docs/third-party-audit.md](docs/third-party-audit.md) for the file-level audit of the ReVitAI Bridge core release.
