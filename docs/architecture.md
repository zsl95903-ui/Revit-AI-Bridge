# Architecture

## Goals

The bridge exists to make Revit automation explicit, typed, observable, and recoverable. The Agent should not receive unrestricted access to the Revit API object model or the document process.

## Components

- `ReVitAI.Bridge`: Revit add-in host and `ToolDispatcher`.
- `BridgeServer`: current-user Named Pipe server and discovery file.
- `ExternalEventInvoker`: moves requests onto the Revit API thread.
- `CodexBatchHost`: exposes the descriptor catalog.
- `RevitAi.McpServer`: stdio MCP transport for Codex and other MCP clients.
- `tools`: registration and invocation helpers.
- `agent-workflow.md`: behavioral contract for tool selection and verification.

## Transport Layers

```text
Agent / MCP client
        |
        v
RevitAi.McpServer or direct JSON client
        |
        v
%LOCALAPPDATA%\ReVitAI\revitai-bridge.json
        |
        v
Named Pipe (current user)
        |
        v
ReVitAI.Bridge BridgeServer
        |
        v
ExternalEventInvoker
        |
        v
ToolDispatcher
```

## Execution Model

1. The client sends one JSON request.
2. The bridge resolves the active document and optional expected GUID.
3. Read-only calls execute on the Revit thread.
4. Mutating calls start a Revit transaction.
5. `dryRun` rolls the transaction back.
6. Successful mutations commit and return structured results.
7. The Agent reads back changed IDs before reporting success.
8. Errors cause rollback and are returned with structured error codes.

## Plan Execution

`apply_drawing_plan` accepts ordered operations and dependency phases. It is intended for drawing generation and other multi-step workflows. `batch` is the lower-level primitive. Both preserve one undo group where supported and stop on failure.

## Host Isolation

The wider Revit AI deployment uses two isolated entrypoints:

- **Legacy Native**: original built-in Agent workflow and native registry.
- **Codex Contract**: headless Batch bridge and MCP server, no ribbon UI.

Only one manifest should be active at a time. This prevents shared static registries and vendor state from competing in the Revit process.

## Unit Contract

- Public geometry inputs and outputs use millimetres unless the schema says otherwise.
- `create_level.elevation` uses metres.
- Internal Revit conversions remain behind the dispatcher.
- The Agent must read the schema before converting values.

## Safety Contract

- No save, synchronization, close, or overwrite without explicit user intent.
- No arbitrary code execution in the default Batch catalog.
- No success report without readback.
- No fabricated element IDs or family types.