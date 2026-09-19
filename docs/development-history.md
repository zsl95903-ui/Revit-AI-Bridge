# Development History

## Prototype

- Named Pipe transport.
- Revit ExternalEvent dispatch.
- Document, level, grid and view reads.

## Planning Layer

- Multi-step `batch`.
- `apply_drawing_plan`.
- Transaction grouping and dry-run.

## Native Tool Layer

- Native columns, beams, rooms, doors and windows.
- Geometry, category and family queries.
- Delete and family-load tools.
- Detail, annotation, view and export tools.

## Stability Work

- Fixed category identity handling.
- Added warning failure filtering.
- Corrected level-relative insertion points.
- Added a typed tool catalog and smoke verification.

## Revit-AI-Bridge Integration

- Aligned the core tool API with the previously published Revit-AI-Bridge Agent workflow.
- Added Named Pipe discovery for local Agent clients.
- Standardized JSON-line request and response handling.
- Added release packaging and installation for Revit 2027.