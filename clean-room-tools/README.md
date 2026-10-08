# RevitAI Clean-Room Tools

Self-developed Revit 2027 tool layer for the ReVitAI Bridge / Revit-AI-Bridge workflow.

## Contents

- `src/RevitAi.Engine.Abstractions` / `Core` / `Revit` / `ToolContracts` / `Addin`
- `src/RevitToolSet` + `src/RevitToolSet.Implementations`
- `tools/ToolSetGenerator`
- `tests/RevitAi.Engine.SmokeTests`
- `samples/revitai-ui-wrapper`

## Boundary

This module contains only project-authored source code. It does **not** contain
vendor binaries, decompiled vendor source, or Autodesk Revit API binaries.

`contracts/tool-contracts.json` is interoperability metadata used to generate
the typed tool classes. See `NOTICE.md`.

## Build

```powershell
powershell -ExecutionPolicy Bypass -File .\build.ps1
```

The build expects a local Revit 2027 installation for `RevitAPI.dll` and
`RevitAPIUI.dll`. Those assemblies are referenced from disk and are not copied
into this repository.
