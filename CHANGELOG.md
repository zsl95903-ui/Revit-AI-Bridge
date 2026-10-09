# Changelog

## 2026-10-09 - Revit 2027 Hybrid Test Package

- Added `releases/2027-test/RevitAI-Hybrid-Setup-5.0.0.exe`.
- Added rebuilt Revit 2027 120-tool source set under `rebuilt-120-tools/`.
- Added hybrid worktree under `worktree/`, including the Revit AI WebView2 UI, PDF/CAD extensions, packaging script, and embedded Codex bridge.
- Codex bridge now starts automatically with the Revit AI UI and writes `%LOCALAPPDATA%\RevitAi\codex-bridge.json`.
- This release is a test build, not a formal production release.

## 2.1.0 - 2026-09-20

- Expanded the typed bridge catalog from 47 to 48 tools.
- Added `set_level_line_pattern` for Level-category datum presentation.
- Added headless add-in startup for isolated Codex contract hosting.
- Added the stdio MCP server and Codex registration/invocation helpers.
- Documented the Agent workflow for discovery, preflight, dry-run, mutation, readback, and user boundaries.
- Added explicit open-source and repository secret boundaries.
- Clarified that local signing material, vendor binaries, and user data are not distributed.


## 1.4.0 - 2026-09-19

- Released the ReVitAI Bridge core tool and API layer for the Revit-AI-Bridge Agent workflow.
- Kept 47 native Revit 2027 tools with typed JSON contracts.
- Added current-user Named Pipe discovery and structured request handling.
- Added transaction, dry-run, batch, plan, and readback support.
- Unified the source, assembly, add-in, and tool catalog under the `ReVitAI.Bridge` namespace.
- Added clean source and binary release packaging for Revit 2027.
