# Revit AI Bridge

Local-first, typed automation bridge for Autodesk Revit 2027.

Revit AI Bridge connects an AI agent, script, or local client to Revit through a user-scoped Named Pipe. Revit API work is marshalled onto the Revit main thread, executed in a transaction, checked by readback, and returned as structured JSON.

## Highlights

- 47 typed tools for document inspection, modeling, annotation, views, and export.
- Levels, grids, walls, floors, rooms, doors, windows, columns, beams, and detail creation.
- Read-only queries for levels, grids, family types, rooms, elements, and geometry.
- `dryRun`, `batch`, and dependency-aware `apply_drawing_plan`.
- User-scoped Named Pipe transport with document GUID checks.
- Millimetre-based public geometry API.
- Transaction rollback on failure and explicit readback values.
- Built and verified against Revit 2027.

## Tool Groups

1. Document and context
2. Levels and grids
3. Architectural components
4. Structural components
5. Rooms and boundaries
6. Annotation and detail
7. Views and export
8. Plans and batching
9. Families, query, and object management

The canonical machine-readable catalog is [docs/tool-catalog.json](docs/tool-catalog.json).

## Architecture

```text
AI agent / script
        |
        v
Named Pipe client
        |
        v
Bridge server
        |
        v
Revit ExternalEvent
        |
        v
Tool dispatcher
        |
        +--> read-only query
        +--> transaction
        +--> dry-run rollback
        +--> readback JSON
```

## Requirements

- Autodesk Revit 2027 x64.
- .NET 10 x64 SDK for building from source.
- Windows 10 or Windows 11.

Autodesk Revit API assemblies are not redistributed by this repository. Builds expect `RevitAPI.dll` and `RevitAPIUI.dll` to be available from a local Revit installation.

## Build and Package

```powershell
powershell -ExecutionPolicy Bypass -File .\build\build.ps1
powershell -ExecutionPolicy Bypass -File .\build\package.ps1
```

Use `-RevitApiDir` when Revit is installed somewhere other than `C:\Program Files\Autodesk\Revit 2027`.

## Install

1. Close Revit.
2. Extract the package produced under `artifacts`.
3. Run `install.ps1` from the extracted package.
4. Start Revit and open a project.
5. Confirm that the **Revit AI Bridge** add-in loaded.

The installation script copies the add-in and payload into the current user's `%APPDATA%\Autodesk\Revit\Addins\2027` directory.

## First Calls

The bridge writes a discovery file to:

```text
%LOCALAPPDATA%\RevitAi\codex-bridge.json
```

Use the pipe name from that file as the transport for JSON-line requests. Start with `tools.list`, then call `get_document_snapshot`. Review a plan with `dryRun: true` before applying it.

## Safety Model

- The Named Pipe is current-user only.
- Requests can include an expected document GUID.
- Read and write operations are separated.
- Mutating operations run in Revit transactions.
- Failed operations roll back.
- Plan execution can stop on the first error.
- Warning-level failures can be filtered without hiding errors.
- Arbitrary code execution is not exposed by the default bridge.

## Repository Layout

```text
src/RevitAiBatch
build
installer
docs
```

## Documentation

- [Architecture](docs/architecture.md)
- [Tools](docs/tools.md)
- [Security](docs/security.md)
- [Compatibility](docs/compatibility.md)
- [Development history](docs/development-history.md)
- [Third-party audit](docs/third-party-audit.md)
- [Open-source release plan](docs/open-source-plan.md)
- [Project overview](docs/project-overview.md)

## Compatibility

The current target is Revit 2027. Older Revit versions require a separate API compatibility pass and are not part of the initial release promise.

## License

The project source is licensed under Apache-2.0 unless otherwise noted. Third-party components remain under their respective licenses. See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md) and [docs/third-party-audit.md](docs/third-party-audit.md).
