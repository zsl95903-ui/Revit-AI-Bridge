# Open-Source Scope

## Included

- `src/ReVitAI.Bridge`: the typed Revit 2027 bridge, dispatcher, schemas, transactions, and 48 tools.
- `src/RevitAi.McpServer`: the stdio MCP forwarder that exposes the live bridge catalog.
- `tools`: client registration, invocation examples, and a typed datum-line-pattern example.
- `docs`: architecture, tool catalog, Agent workflow, security, compatibility, and third-party boundaries.
- `installer` and `build`: source-build and packaging scripts for the Batch bridge.

## Excluded

- Private code-signing keys, PFX/P12 files, DPAPI backups, and certificate-store exports.
- API keys, model-provider tokens, chat logs, session stores, and user settings.
- Machine-specific absolute paths and local discovery files.
- Autodesk Revit API binaries.
- Proprietary vendor DLLs and decompiled vendor source.
- Local build outputs, model files, screenshots, and release archives containing restricted material.

## Rationale

The public repository is intended to document and share the reusable integration layer: typed tool contracts, bridge architecture, MCP transport, transaction rules, readback behavior, and Agent workflow. It is not a redistribution of Autodesk Revit or third-party vendor binaries.

## Contribution Rule

Pull requests must not include signing material, secrets, proprietary binaries, decompiled vendor source, local user paths, or model data. Run `git status` and inspect `git diff --cached` before committing.