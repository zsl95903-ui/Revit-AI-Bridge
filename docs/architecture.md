# Architecture

## Data Flow

```text
Revit-AI-Bridge Agent / local client
        |
        v
Named Pipe request
        |
        v
BridgeServer
        |
        v
ExternalEventInvoker
        |
        v
Revit main thread
        |
        v
ToolDispatcher
        |
        +--> read-only query
        +--> transaction
        +--> dry-run rollback
        +--> readback DTO
```

## Main Components

- `BridgeServer`: owns the named pipe, discovery file and request loop.
- `ExternalEventInvoker`: queues work onto the Revit API thread.
- `ReVitAIBatchHost`: exposes the tool descriptor catalog.
- `ToolDispatcher`: validates and executes all tools.
- `ReVitAIBridgeApplication`: Revit add-in entry point and ribbon commands.
- `Models`: request, response and tool descriptor types.
- `Units`: millimetres and Revit internal unit conversion.

## Execution Model

1. The client sends one JSON line.
2. The bridge resolves the expected document.
3. Read-only calls execute directly.
4. Mutating calls start a Revit transaction.
5. `dryRun` rolls the transaction back.
6. Successful mutations commit and return element IDs.
7. Warnings can be filtered; errors cause rollback.
