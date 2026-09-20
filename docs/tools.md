# Tools

The canonical machine-readable catalog is [tool-catalog.json](tool-catalog.json).

Release: `2.1.0`

Tool count: **48**

## Groups

### Annotation (3)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `create_detail_curves` | yes | yes | Create detail lines, arcs, and polylines in one view and transaction. |
| `create_dimension_by_elements` | yes | yes | Create a dimension chain from elements. |
| `create_text_notes` | yes | yes | Create one or more text notes in a view. |

### Architecture (7)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `create_door` | yes | yes | Create a native door family instance in a host wall. |
| `create_floor_by_profile` | yes | yes | Create a native floor from a closed profile. |
| `create_room` | yes | yes | Create and name a room at a point on a level. |
| `create_straight_wall` | yes | yes | Create a straight native wall. |
| `create_wall_with_openings` | yes | yes | Create a straight native wall with door or window gaps and header segments. |
| `create_window` | yes | yes | Create a native window family instance in a host wall. |
| `get_room_boundaries` | no | yes | Read the boundary loops and segments of a room. |

### Document & Context (7)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `activate_view` | no | yes | Activate a Revit view in the UI. |
| `get_active_view` | no | yes | Read the active view. |
| `get_all_views` | no | yes | Read all non-template views. |
| `get_document_info` | no | yes | Read active document information. |
| `get_document_snapshot` | no | yes | Read document identity, active view, levels, and context for precise preflight. |
| `get_project_units` | no | yes | Read effective project length unit. |
| `ping` | no | yes | Bridge health check. |

### Element Operations (5)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `delete_elements` | yes | yes | Delete one or more elements by ID. |
| `element_query` | no | yes | Query elements by category, name, type, and limit. |
| `get_element_geometry` | no | yes | Read bounding box, location, solids, curves, category, and type of an element. |
| `move_elements` | yes | yes | Move elements by millimetres. |
| `rotate_element` | yes | yes | Rotate elements by degrees. |

### Levels & Grids (7)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `create_grid` | yes | yes | Create one or more grids. |
| `create_level` | yes | yes | Create a level at a metre elevation. |
| `get_all_grids` | no | yes | Read all grids. |
| `get_all_levels` | no | yes | Read all levels. |
| `set_level_head_text_size` | yes | yes | Set datum level head text size in millimetres. |
| `set_level_line_pattern` | yes | yes | Set the projection line pattern for the Level category. |
| `upsert_level` | yes | yes | Create or update a level by name and elevation. |

### Plans & I/O (5)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `apply_drawing_plan` | yes | yes | Execute a multi-operation drawing plan with dependency phases and one undo group. |
| `batch` | yes | yes | Execute multiple tools in one transaction. |
| `import_pdf_underlay` | yes | yes | Import a PDF page as a Revit image underlay. |
| `save_document` | yes | yes | Save the active document, or save it to a path when provided. |
| `save_document_as` | yes | yes | Save the active document as a .rvt file. |

### Structure (3)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `create_boxes` | yes | yes | Create extruded 3D boxes for columns, stairs, and massing. |
| `create_structural_beam` | yes | yes | Create a native structural beam or a structural-framing fallback. |
| `create_structural_column` | yes | yes | Create a native structural column or a structural-column fallback. |

### Types & Families (4)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `get_family_types` | no | yes | List loaded family types by family, type, and category. |
| `get_floor_types` | no | yes | Read available floor types. |
| `get_wall_types` | no | yes | Read available wall types. |
| `load_family` | yes | yes | Load a Revit family file into the active document. |

### Views (7)

| Tool | Write | Dry-run | Description |
|---|---:|---:|---|
| `create_3d_view` | yes | yes | Create or reuse a native 3D view and activate it on the canvas. |
| `export_view_image` | no | no | Export a view to PNG. |
| `fit_view_crop_to_elements` | yes | yes | Expand a view crop to include selected or visible elements. |
| `get_view_settings` | no | yes | Read view crop, outline, scale, and detail level. |
| `hide_elements_in_view` | yes | yes | Hide or unhide elements in a specific view. |
| `set_element_color` | yes | yes | Apply a projection-line color override in a view. |
| `zoom_to_elements` | no | no | Zoom the active view to elements. |

## Write Safety

Mutating tools accept `dryRun` when supported. Plan and batch execution use transactions and can stop on the first error. Readback values are returned after every write.
