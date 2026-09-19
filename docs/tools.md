# Tools

The canonical machine-readable catalog is [tool-catalog.json](tool-catalog.json).

## Groups

- Document and context
- Levels and grids
- Architecture
- Structure
- Rooms and boundaries
- Annotation and detail
- Views and export
- Plans and batches
- Families, query and object management

## Write Safety

Mutating tools accept `dryRun` when supported. Plan and batch execution use transactions and can stop on the first error. Readback values are returned after every write.
