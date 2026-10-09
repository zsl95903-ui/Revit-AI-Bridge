# ReVitAI Bridge

Local-first, typed automation bridge for Autodesk Revit 2027.

> **测试版本说明**
>
> 本仓库中的 `RevitAI-Hybrid-Setup-5.0.0.exe`、`rebuilt-120-tools` 和 `worktree` 均为测试版本，非正式发行版，仅供参考、研究与本地验证使用。请在隔离环境或测试模型中验证，不要直接用于生产项目。

ReVitAI Bridge connects an AI agent, MCP client, script, or local process to Revit through a user-scoped Named Pipe. Revit API work is marshalled onto the Revit main thread, executed with transaction and rollback rules, and returned as structured JSON.

## Highlights

- 48 typed tools for context, datum, modeling, annotation, views, files, and batch plans.
- Codex/MCP integration through a transparent stdio MCP server.
- Agent workflow guidance for discovery, preflight, dry-run, mutation, and readback.
- Isolation-friendly headless add-in startup for the Codex contract host.
- Plan execution with dependency phases, `dryRun`, and one undo group.
- Millimetre-based public geometry API with explicit unit conversion.
- Current-user Named Pipe transport and document GUID validation.
- Transaction rollback on failure and readback values for writes.
- Built and verified against Revit 2027.

## 2027 Hybrid Test Package

本次大版本更新包含：

- `releases/2027-test/RevitAI-Hybrid-Setup-5.0.0.exe`
  - 完整的 Revit 2027 测试安装包，非正式发行版。
- `rebuilt-120-tools/`
  - Revit 2027 重建的 120 个工具集源码。
- `worktree/`
  - 混合工作树，包含 Revit AI 上层 WebView2 UI、PDF/CAD 扩展、内置 Codex 桥接封装和打包脚本。

内置 Codex 桥接默认随 Revit AI UI 启动，并写入：

```text
%LOCALAPPDATA%\RevitAi\codex-bridge.json
```

Codex 可以直接通过该接口调用插件注册的 Revit 绘图工具。

## Architecture

```text
Codex desktop / MCP client / script
                  |
                  v
       RevitAi.McpServer (stdio)
                  |
                  v
       Named Pipe JSON protocol
                  |
                  v
       ReVitAI.Bridge add-in host
                  |
                  v
        Revit ExternalEvent
                  |
                  v
            ToolDispatcher
                  |
        +---------+----------+
        |                    |
   read-only query      transaction
        |                    |
        +------ readback ----+
                             |
                     commit or rollback
```

The bridge can also run as a headless host without creating a ribbon tab. This supports separation between the native Agent host and a Codex contract host.

## Agent Workflow

1. Read document context with `get_document_snapshot`.
2. Select only tool names returned by `tools.list`.
3. Resolve real element, level, grid, family, and type IDs before writing.
4. Use `dryRun` when the tool supports it.
5. Execute one logical mutation in a transaction.
6. Read back changed elements and report observed values.
7. Never save, synchronize, close, or overwrite a document unless the user explicitly asks.

Full workflow:

- [Agent workflow (English)](docs/agent-workflow.md)
- [Agent 工作流（中文）](docs/agent-workflow.zh-CN.md)

## MCP Server

`src/RevitAi.McpServer` exposes the live Revit tool catalog as MCP tools. It discovers the current named pipe, reads the live catalog, and forwards `tools/call` requests without touching the Revit API directly.

Build and register it with:

```powershell
dotnet build .\src\RevitAi.McpServer\RevitAi.McpServer.csproj -c Release
powershell -ExecutionPolicy Bypass -File .\tools\Register-RevitCodex.ps1
powershell -ExecutionPolicy Bypass -File .\tools\Invoke-RevitCodex.ps1 `
  -Instruction "Read the active Revit document and create the requested levels."
```

## Tool Catalog

The canonical catalog is [docs/tool-catalog.json](docs/tool-catalog.json). It is generated from `src/ReVitAI.Bridge/ToolDispatcher.cs` and contains 48 tools.

Examples:

- Context: `get_document_snapshot`, `get_active_view`, `get_project_units`
- Datum: `create_level`, `upsert_level`, `set_level_line_pattern`, `create_grid`
- Architecture: `create_straight_wall`, `create_floor_by_profile`, `create_room`
- Annotation: `create_detail_curves`, `create_text_notes`, `create_dimension_by_elements`
- Views: `create_3d_view`, `fit_view_crop_to_elements`, `export_view_image`
- Plans: `dryRun`, `batch`, `apply_drawing_plan`

## Requirements

- Autodesk Revit 2027 x64.
- .NET 10 x64 SDK for building from source.
- Windows 10 or Windows 11.

Autodesk Revit API assemblies are not redistributed by this repository. Builds expect `RevitAPI.dll` and `RevitAPIUI.dll` from a local Revit installation.

## Build and Package

```powershell
powershell -ExecutionPolicy Bypass -File .\build\build.ps1
powershell -ExecutionPolicy Bypass -File .\build\package.ps1
```

Use `-RevitApiDir` when Revit is installed elsewhere.

## Install

1. Close Revit.
2. Extract the core package produced under `artifacts`.
3. Run `install.ps1` from the extracted package.
4. Start Revit and open a project.
5. Confirm that the **ReVitAI Bridge** add-in loaded.

The installer copies the add-in into the current user's Revit 2027 add-ins directory.

## First Calls

The bridge writes a discovery file to:

```text
%LOCALAPPDATA%\ReVitAI\revitai-bridge.json
```

Start with `tools.list`, then call `get_document_snapshot`. Review a plan with `dryRun: true` before applying it.

## Open-Source Boundary

This repository contains our bridge, dispatcher, MCP forwarder, scripts, and documentation. It does not contain:

- private signing keys, PFX files, DPAPI backups, or local certificate stores;
- API keys, chat history, user settings, or machine-specific paths;
- proprietary vendor DLLs or decompiled vendor source;
- Autodesk Revit API binaries.

See [Open-source scope](docs/open-source-scope.md) and [Security](docs/security.md).

## Documentation

- [Architecture](docs/architecture.md)
- [Tools](docs/tools.md)
- [Agent workflow](docs/agent-workflow.md)
- [Agent 工作流](docs/agent-workflow.zh-CN.md)
- [Security](docs/security.md)
- [Compatibility](docs/compatibility.md)
- [Open-source scope](docs/open-source-scope.md)
- [Development history](docs/development-history.md)
- [Third-party audit](docs/third-party-audit.md)

## License

The project source is licensed under Apache-2.0 unless otherwise noted. Third-party components remain under their respective licenses. See [THIRD_PARTY_NOTICES.md](THIRD_PARTY_NOTICES.md).
