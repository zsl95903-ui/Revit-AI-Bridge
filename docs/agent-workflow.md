# Agent Workflow

This document describes the intended contract between an AI agent and ReVitAI Bridge.

## Roles

- **Agent**: interprets user intent, chooses tools, and reports observed results.
- **MCP server**: exposes the live Revit catalog to the agent.
- **Bridge**: validates requests, marshals work to the Revit thread, and returns structured results.
- **Tool dispatcher**: owns schemas, units, transactions, rollback, and readback.
- **User**: owns document save, synchronization, close, and destructive data decisions.

## Core Loop

```text
User intent
   |
   v
Read context -> select tool -> preflight -> mutate or query
   |                                      |
   |                                      v
   +------------------------------- read back result
   |
   v
Report observed values, uncertainty, and next safe step
```

## Required Steps

### 1. Discover context

Start with:

- `ping`
- `get_document_info` or `get_document_snapshot`
- `get_active_view`
- `get_project_units`

Never assume a document, view, level, grid, or family type from an earlier session.

### 2. Use the live catalog

Call `tools.list` or MCP `tools/list`. Only invoke names returned by that catalog. Do not invent tool names and do not silently fall back to arbitrary code execution.

### 3. Preflight identifiers

Resolve IDs from the active document before writing:

- levels with `get_all_levels`;
- grids with `get_all_grids`;
- family types with `get_family_types`;
- wall and floor types with `get_wall_types` and `get_floor_types`;
- rooms with `get_room_boundaries`;
- elements with `element_query` or `get_element_geometry`.

If a required element is missing, report what is missing instead of fabricating an ID.

### 4. Plan the write

Use `dryRun: true` when supported. For multi-step work, build an `apply_drawing_plan` or `batch` request with explicit dependencies.

Recommended phases:

1. datum and levels;
2. grids;
3. walls and floors;
4. hosted elements;
5. annotation and dimensions;
6. view crop and export.

### 5. Mutate once per logical operation

Each mutating operation should have a clear purpose and one transaction. A failed operation must roll back. Do not hide unrelated document-wide changes in one ambiguous request.

### 6. Read back

After every write, query the affected elements again. Report:

- requested values;
- observed values;
- returned element IDs;
- tools that returned warnings;
- any value that could not be verified.

### 7. Stop at the user boundary

Do not save, synchronize, close, or overwrite a document unless the user explicitly asks. Do not elevate a dry-run into a committed write without authorization.

## Drawing Workflow

For architectural drawing generation, the preferred order is:

1. Import the PDF underlay if needed.
2. Read the active view and document context.
3. Create or update levels and grids.
4. Create floors, walls, rooms, columns, and beams.
5. Add doors, windows, detail curves, text, and dimensions.
6. Fit the view crop and export an image.
7. Read back created IDs and return a concise drawing summary.

## Failure Handling

- `revit_context_not_ready`: open the Revit host and retry.
- `tool_not_found`: refresh the live catalog; never invent a replacement.
- `astools_tool_error`: inspect the structured error and correct arguments.
- Transaction failure: do not claim success; report rollback.
- Ambiguous units: use the tool schema and project units before converting.
- Missing type or family: resolve the real loaded type or stop.

## Anti-Patterns

- Calling tools from a stale catalog.
- Hard-coding element IDs from another document.
- Treating a dry-run as a completed write.
- Creating proxy geometry such as thin walls or pipes to imitate grids.
- Using arbitrary code execution when a typed tool exists.
- Reporting success without an independent readback.