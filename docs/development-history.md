# Development History

## Prototype

- Named pipe transport.
- Revit external event dispatch.
- Document, level, grid and view reads.

## Planning Layer

- Multi-step `batch`.
- `apply_drawing_plan`.
- Transaction grouping and dry-run.

## Integrated Host

- Tool registration into the Revit AI entry point.
- Expanded tool catalog to 47 tools.

## Building and Structure Tools

- Native columns, beams, rooms, doors and windows.
- Geometry, category and family queries.
- Delete and family-load tools.

## Stability Work

- Fixed category identity handling.
- Added warning failure filtering.
- Corrected level-relative insertion points.
- Added a typed tool catalog and smoke verification.

## Open Source Preparation

- Separated the clean bridge core from historical release-only components.
- Added license and third-party review documents.
- Added configurable Revit API build path.
- Excluded user models, logs and generated media.
