using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace RevitAiBatch;

internal sealed partial class ToolDispatcher
{
    private const string BatchName = "Codex Revit Batch";
    private readonly Dictionary<string, ToolDefinition> _tools;

    public ToolDispatcher()
    {
        var tools = new List<ToolDefinition>
        {
            Read("ping", "Bridge health check.", ctx => new
            {
                Pong = true,
                Document = ctx.Document.Title,
                DocumentPath = ctx.Document.PathName,
            }),
            Read("get_document_info", "Read active document information.", GetDocumentInfo),
            Read("get_active_view", "Read the active view.", GetActiveView),
            Read("get_all_levels", "Read all levels.", GetAllLevels),
            Read("get_all_grids", "Read all grids.", GetAllGrids),
            Read("get_wall_types", "Read available wall types.", GetWallTypes),
            Read("get_floor_types", "Read available floor types.", GetFloorTypes),
            Read("get_project_units", "Read effective project length unit.", _ => new
            {
                ProjectUnit = "mm",
                LengthUnit = "millimeters",
                InternalUnit = "decimal feet",
            }),
            Read("get_all_views", "Read all non-template views.", GetAllViews),
            Read(
                "get_document_snapshot",
                "Read document identity, active view, levels, and context for precise preflight.",
                GetDocumentSnapshot),
            Read("activate_view", "Activate a Revit view in the UI.", ActivateView),
            Read("get_view_settings", "Read view crop, outline, scale, and detail level.", GetViewSettings),
            Write("hide_elements_in_view", "Hide or unhide elements in a specific view.", HideElementsInView),
            Write("create_3d_view", "Create or reuse a native 3D view and activate it on the canvas.", Create3DView),
            Read("zoom_to_elements", "Zoom the active view to elements.", ZoomToElements, mutating: false),
            Read("export_view_image", "Export a view to PNG.", ExportViewImage, mutating: false),
            Write("save_document", "Save the active document, or save it to a path when provided.", SaveDocument),
            Write("save_document_as", "Save the active document as a .rvt file.", SaveDocument),
            Write("create_grid", "Create one or more grids.", CreateGrids),
            Write("create_level", "Create a level at a metre elevation.", CreateLevel),
            Write("upsert_level", "Create or update a level by name and elevation.", UpsertLevel),
            Write("set_level_head_text_size", "Set datum level head text size in millimetres.", SetLevelHeadTextSize),
            Write("create_straight_wall", "Create a straight native wall.", CreateStraightWall),
            Write(
                "create_wall_with_openings",
                "Create a straight native wall with door or window gaps and header segments.",
                CreateWallWithOpenings),
            Write("create_floor_by_profile", "Create a native floor from a closed profile.", CreateFloorByProfile),
            Write("create_boxes", "Create extruded 3D boxes for columns, stairs, and massing.", CreateBoxes),
            Read("get_family_types", "List loaded family types by family, type, and category.", GetFamilyTypes),
            Write("create_structural_column", "Create a native structural column or a structural-column fallback.", CreateStructuralColumn),
            Write("create_structural_beam", "Create a native structural beam or a structural-framing fallback.", CreateStructuralBeam),
            Write("create_room", "Create and name a room at a point on a level.", CreateRoom),
            Read("get_room_boundaries", "Read the boundary loops and segments of a room.", GetRoomBoundaries),
            Write("delete_elements", "Delete one or more elements by ID.", DeleteElements),
            Read("get_element_geometry", "Read bounding box, location, solids, curves, category, and type of an element.", GetElementGeometry),
            Read("element_query", "Query elements by category, name, type, and limit.", ElementQuery),
            Write("load_family", "Load a Revit family file into the active document.", LoadFamilyTool),
            Write("create_door", "Create a native door family instance in a host wall.", ctx => CreateHostedInstance(ctx, true)),
            Write("create_window", "Create a native window family instance in a host wall.", ctx => CreateHostedInstance(ctx, false)),
            Write("create_dimension_by_elements", "Create a dimension chain from elements.", CreateDimensionByElements),
            Write("import_pdf_underlay", "Import a PDF page as a Revit image underlay.", ImportPdfUnderlay),
            Write(
                "create_detail_curves",
                "Create detail lines, arcs, and polylines in one view and transaction.",
                CreateDetailCurves),
            Write("create_text_notes", "Create one or more text notes in a view.", CreateTextNotes),
            Write("set_element_color", "Apply a projection-line color override in a view.", SetElementColor),
            Write("move_elements", "Move elements by millimetres.", MoveElements),
            Write("rotate_element", "Rotate elements by degrees.", RotateElements),
            Write(
                "fit_view_crop_to_elements",
                "Expand a view crop to include selected or visible elements.",
                FitViewCropToElements),
            Write(
                "apply_drawing_plan",
                "Execute a multi-operation drawing plan with dependency phases and one undo group.",
                _ => throw new InvalidOperationException(
                    "apply_drawing_plan must be executed through the phased dispatcher.")),
            Write("batch", "Execute multiple tools in one transaction.", ExecuteBatch),
        };

        _tools = tools.ToDictionary(tool => tool.Name, StringComparer.OrdinalIgnoreCase);
    }

    public IReadOnlyCollection<ToolDefinition> Tools => _tools.Values;

    public object Catalog(bool includeSchema)
    {
        var tools = _tools.Values
            .OrderBy(tool => tool.Name)
            .Select(tool => new
            {
                tool.Name,
                tool.Description,
                ToolSet = "Codex Batch",
                tool.Mutating,
                tool.RequiresActiveDocument,
                SupportsDryRun = !tool.Name.Equals("zoom_to_elements", StringComparison.OrdinalIgnoreCase)
                    && !tool.Name.Equals("export_view_image", StringComparison.OrdinalIgnoreCase),
                InputSchema = includeSchema ? tool.InputSchema ?? GenericSchema() : null,
            })
            .ToArray();

        return new
        {
            Tools = tools,
            Count = tools.Length,
        };
    }

    public async Task<object?> ExecuteAsync(
        ExternalEventInvoker invoker,
        string tool,
        JsonElement arguments)
    {
        if (!_tools.ContainsKey(tool))
        {
            throw new InvalidOperationException($"Tool '{tool}' was not found.");
        }

        return await invoker.InvokeAsync(tool, arguments).ConfigureAwait(false);
    }

    public object? ExecuteOnRevitThread(UIApplication app, PendingCommand command)
    {
        if (!_tools.TryGetValue(command.Tool, out var definition))
        {
            throw new InvalidOperationException($"Tool '{command.Tool}' was not found.");
        }

        var dryRun = Json.Bool(
            command.Arguments,
            "dryRun",
            Json.Bool(command.Arguments, "dry_run"));
        if (command.Tool.Equals("apply_drawing_plan", StringComparison.OrdinalIgnoreCase))
        {
            return ExecuteDrawingPlanPhased(app, command.Arguments, dryRun);
        }

        if (command.Tool.Equals("save_document", StringComparison.OrdinalIgnoreCase)
            || command.Tool.Equals("save_document_as", StringComparison.OrdinalIgnoreCase))
        {
            return definition.Handler(new InvocationContext
            {
                UiApplication = app,
                Arguments = command.Arguments,
                DryRun = dryRun,
            });
        }

        var readOnly = IsReadOnly(command.Tool, command.Arguments);
        if (readOnly)
        {
            return definition.Handler(new InvocationContext
            {
                UiApplication = app,
                Arguments = command.Arguments,
                DryRun = dryRun,
            });
        }

        if (definition.RequiresActiveDocument && app.ActiveUIDocument is null)
        {
            throw new InvalidOperationException("No active Revit document.");
        }

        var document = app.ActiveUIDocument!.Document;
        using var transaction = new Transaction(document, $"{BatchName}: {command.Tool}");
        transaction.Start();
            var failureOptions = transaction.GetFailureHandlingOptions();
            failureOptions.SetFailuresPreprocessor(new WarningSuppressor());
            transaction.SetFailureHandlingOptions(failureOptions);

        try
        {
            var result = definition.Handler(new InvocationContext
            {
                UiApplication = app,
                Arguments = command.Arguments,
                DryRun = dryRun,
            });

            if (dryRun)
            {
                transaction.RollBack();
                return new
                {
                    Result = result,
                    DryRun = true,
                    Committed = false,
                };
            }

            transaction.Commit();
            return result;
        }
        catch
        {
            if (transaction.GetStatus() == TransactionStatus.Started)
            {
                transaction.RollBack();
            }

            throw;
        }
    }

    private bool IsReadOnly(string tool, JsonElement arguments)
    {
        if (!_tools.TryGetValue(tool, out var definition))
        {
            return false;
        }

        if (tool.Equals("batch", StringComparison.OrdinalIgnoreCase))
        {
            var steps = Json.Array(arguments, "steps").ToArray();
            return steps.Length > 0
                && steps.All(step =>
                {
                    var commandName = Json.String(step, "tool")
                        ?? Json.String(step, "command")
                        ?? string.Empty;
                    return _tools.TryGetValue(commandName, out var subTool) && !subTool.Mutating;
                });
        }

        return !definition.Mutating;
    }

    internal bool IsMutating(string tool)
    {
        return _tools.TryGetValue(tool, out var definition) && definition.Mutating;
    }

    internal IReadOnlyList<CodexBatchToolDescriptor> DescribeTools()
    {
        return _tools.Values
            .OrderBy(tool => tool.Name, StringComparer.OrdinalIgnoreCase)
            .Select(tool => new CodexBatchToolDescriptor(
                tool.Name,
                tool.Description,
                tool.Mutating,
                JsonSerializer.Serialize(tool.InputSchema ?? GenericSchema(), Json.Options)))
            .ToArray();
    }
    private static ToolDefinition Read(
        string name,
        string description,
        Func<InvocationContext, object?> handler,
        bool mutating = false)
        => new()
        {
            Name = name,
            Description = description,
            Mutating = mutating,
            RequiresActiveDocument = true,
            Handler = handler,
        };

    private static ToolDefinition Write(
        string name,
        string description,
        Func<InvocationContext, object?> handler)
        => new()
        {
            Name = name,
            Description = description,
            Mutating = true,
            RequiresActiveDocument = true,
            Handler = handler,
        };

    private static object GenericSchema() => new
    {
        Type = "object",
        AdditionalProperties = true,
    };

    private static object GetDocumentInfo(InvocationContext context)
    {
        var document = context.Document;
        return new
        {
            Title = document.Title,
            Path = document.PathName,
            IsFamilyDocument = document.IsFamilyDocument,
            IsModified = document.IsModified,
            ProjectUnit = "mm",
        };
    }

    private static object GetActiveView(InvocationContext context)
    {
        var view = context.UiDocument.ActiveView;
        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ViewType = view.ViewType.ToString(),
        };
    }

    private static object GetDocumentSnapshot(InvocationContext context)
    {
        var document = context.Document;
        var activeView = context.UiDocument.ActiveView;
        var levels = new FilteredElementCollector(document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(level => level.Elevation)
            .Select(level => new
            {
                LevelId = level.Id.Value,
                Name = level.Name,
                ElevationM = Math.Round(Units.FeetToMm(level.Elevation) / 1000.0, 6),
            })
            .ToArray();
        var viewCount = new FilteredElementCollector(document)
            .OfClass(typeof(View))
            .Cast<View>()
            .Count(view => !view.IsTemplate);

        return new
        {
            DocumentGuid = document.CreationGUID.ToString("D"),
            Title = document.Title,
            Path = document.PathName,
            IsModified = document.IsModified,
            IsFamilyDocument = document.IsFamilyDocument,
            ActiveView = new
            {
                ViewId = activeView.Id.Value,
                ViewName = activeView.Name,
                ViewType = activeView.ViewType.ToString(),
            },
            Levels = levels,
            LevelCount = levels.Length,
            ViewCount = viewCount,
            Unit = "mm",
        };
    }

    private static object GetAllLevels(InvocationContext context)
    {
        var levels = new FilteredElementCollector(context.Document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .OrderBy(level => level.Elevation)
            .Select(level => new
            {
                LevelId = level.Id.Value,
                Name = level.Name,
                Elevation = Math.Round(level.Elevation * Units.MillimetersPerFoot / 1000.0, 6),
            })
            .ToArray();

        return new
        {
            Levels = levels,
            Unit = "meters",
            Count = levels.Length,
        };
    }

    private static object GetAllGrids(InvocationContext context)
    {
        var grids = new FilteredElementCollector(context.Document)
            .OfClass(typeof(Grid))
            .Cast<Grid>()
            .OrderBy(grid => grid.Name, StringComparer.OrdinalIgnoreCase)
            .Select(grid =>
            {
                var curve = grid.Curve;
                var start = curve.GetEndPoint(0);
                var end = curve.GetEndPoint(1);
                var direction = (end - start).Normalize();
                return new
                {
                    GridId = grid.Id.Value,
                    Name = grid.Name,
                    Type = grid.IsCurved ? "ArcGrid" : "LineGrid",
                    LocationUnit = "millimeters",
                    Curve = new
                    {
                        CurveType = curve is Line ? "Line" : curve.GetType().Name,
                        StartPoint = Units.PointDto(start),
                        EndPoint = Units.PointDto(end),
                        Direction = new
                        {
                            X = Math.Round(direction.X, 8),
                            Y = Math.Round(direction.Y, 8),
                            Z = Math.Round(direction.Z, 8),
                        },
                        LengthMm = Math.Round(Units.FeetToMm(curve.Length), 4),
                    },
                };
            })
            .ToArray();

        return new
        {
            Items = grids,
            Count = grids.Length,
            TotalCount = grids.Length,
        };
    }

    private static object GetAllViews(InvocationContext context)
    {
        var views = new FilteredElementCollector(context.Document)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(view => !view.IsTemplate)
            .OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase)
            .Select(view => new
            {
                Id = view.Id.Value,
                Name = view.Name,
                Type = view.ViewType.ToString(),
            })
            .ToArray();

        return new
        {
            Items = views,
            Count = views.Length,
        };
    }

    private static object GetWallTypes(InvocationContext context)
    {
        var types = new FilteredElementCollector(context.Document)
            .OfClass(typeof(WallType))
            .Cast<WallType>()
            .Select(type => new
            {
                TypeId = type.Id.Value,
                Name = type.Name,
                Kind = type.Kind.ToString(),
                WidthMm = Math.Round(Units.FeetToMm(type.Width), 3),
            })
            .ToArray();
        return new
        {
            Items = types,
            Count = types.Length,
        };
    }

    private static object GetFloorTypes(InvocationContext context)
    {
        var types = new FilteredElementCollector(context.Document)
            .OfClass(typeof(FloorType))
            .Cast<FloorType>()
            .Select(type => new
            {
                TypeId = type.Id.Value,
                Name = type.Name,
                ThicknessMm = Math.Round(
                    Units.FeetToMm(type.get_Parameter(BuiltInParameter.FLOOR_ATTR_THICKNESS_PARAM)?.AsDouble() ?? 0),
                    3),
            })
            .ToArray();
        return new
        {
            Items = types,
            Count = types.Length,
        };
    }

    private static object ActivateView(InvocationContext context)
    {
        var view = ResolveView(context.Document, context.Arguments, null);
        context.UiDocument.RequestViewChange(view);
        context.UiDocument.RefreshActiveView();
        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ViewType = view.ViewType.ToString(),
        };
    }

    private static object CreateGrids(InvocationContext context)
    {
        var source = context.Arguments;
        var grids = Json.Array(source, "grids").ToArray();
        if (grids.Length == 0)
        {
            grids = [source];
        }

        var created = new List<object>();
        foreach (var item in grids)
        {
            var start = StartPoint(item);
            var end = EndPoint(item);
            if (start.DistanceTo(end) < Units.MmToFeet(0.1))
            {
                throw new InvalidOperationException("Grid endpoints are too close.");
            }

            var line = Line.CreateBound(start, end);
            var grid = Grid.Create(context.Document, line);
            var requestedName = Json.StringAny(item, "grid_name", "gridName", "name");
            string? warning = null;
            if (!string.IsNullOrWhiteSpace(requestedName))
            {
                var original = grid.Name;
                try
                {
                    grid.Name = requestedName;
                    warning = original == grid.Name && original != requestedName
                        ? $"Revit kept grid name '{grid.Name}'."
                        : null;
                }
                catch (Exception ex)
                {
                    warning = ex.Message;
                }
            }

            created.Add(new
            {
                GridId = grid.Id.Value,
                GridName = grid.Name,
                Start = Units.PointDto(start),
                End = Units.PointDto(end),
                Unit = "mm",
                RenameWarning = warning,
            });
        }

        return new
        {
            SuccessCount = created.Count,
            FailedCount = 0,
            ProjectUnit = "mm",
            CreatedGrids = created,
        };
    }

    private static object CreateLevel(InvocationContext context)
    {
        var elevationMeters = Json.Double(context.Arguments, "elevation", Json.Double(context.Arguments, "elevationMeters"));
        var name = Json.String(context.Arguments, "name") ?? $"Level {elevationMeters:0.###}";
        var existing = new FilteredElementCollector(context.Document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .FirstOrDefault(level => Math.Abs(level.Elevation - Units.MmToFeet(elevationMeters * 1000.0)) < 1e-6);
        if (existing is not null)
        {
            return new
            {
                LevelId = existing.Id.Value,
                Name = existing.Name,
                ElevationM = Math.Round(Units.FeetToMm(existing.Elevation) / 1000.0, 6),
                Existing = true,
            };
        }

        var level = Level.Create(context.Document, Units.MmToFeet(elevationMeters * 1000.0));
        try
        {
            level.Name = name;
        }
        catch
        {
            // Keep the created level if Revit rejects the requested name.
        }

        return new
        {
            LevelId = level.Id.Value,
            Name = level.Name,
            ElevationM = Math.Round(Units.FeetToMm(level.Elevation) / 1000.0, 6),
            Existing = false,
        };
    }

    private static object UpsertLevel(InvocationContext context)
    {
        var document = context.Document;
        var name = Json.StringAny(context.Arguments, "name", "levelName")
            ?? throw new InvalidOperationException("A level name is required.");
        var elevationM = Json.Double(context.Arguments, "elevation", Json.Double(context.Arguments, "elevationMeters"));
        var elevationFeet = Units.MmToFeet(elevationM * 1000.0);
        var toleranceFeet = Units.MmToFeet(Json.Double(context.Arguments, "toleranceMm", 5));
        var levels = new FilteredElementCollector(document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToArray();
        var existing = levels.FirstOrDefault(level =>
            level.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            ?? levels.FirstOrDefault(level => Math.Abs(level.Elevation - elevationFeet) <= toleranceFeet);
        if (existing is null)
        {
            var created = Level.Create(document, elevationFeet);
            try { created.Name = name; } catch { }
            TrySetBuildingStory(created, Json.Bool(context.Arguments, "buildingStory", true));
            return new
            {
                LevelId = created.Id.Value,
                Name = created.Name,
                ElevationM = Math.Round(Units.FeetToMm(created.Elevation) / 1000.0, 6),
                Existing = false,
                ElevationUpdated = false,
            };
        }

        var oldElevationM = Units.FeetToMm(existing.Elevation) / 1000.0;
        var updateExisting = Json.Bool(context.Arguments, "updateExisting", true);
        var elevationUpdated = false;
        if (updateExisting && Math.Abs(existing.Elevation - elevationFeet) > toleranceFeet)
        {
            existing.Elevation = elevationFeet;
            elevationUpdated = true;
        }

        if (!existing.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            try { existing.Name = name; } catch { }
        }

        TrySetBuildingStory(existing, Json.Bool(context.Arguments, "buildingStory", true));
        return new
        {
            LevelId = existing.Id.Value,
            Name = existing.Name,
            OldElevationM = Math.Round(oldElevationM, 6),
            ElevationM = Math.Round(Units.FeetToMm(existing.Elevation) / 1000.0, 6),
            Existing = true,
            ElevationUpdated = elevationUpdated,
        };
    }

    private static void TrySetBuildingStory(Level level, bool isBuildingStory)
    {
        var parameter = level.get_Parameter(BuiltInParameter.LEVEL_IS_BUILDING_STORY);
        if (parameter is not null && !parameter.IsReadOnly)
        {
            parameter.Set(isBuildingStory ? 1 : 0);
        }
    }

    private static object SetLevelHeadTextSize(InvocationContext context)
    {
        var sizeMm = Math.Max(1.0, Json.Double(context.Arguments, "sizeMm", 4.0));
        var sizeFeet = Units.MmToFeet(sizeMm);
        var changed = new List<object>();
        var levels = new FilteredElementCollector(context.Document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .ToArray();
        var visitedTagTypes = new HashSet<long>();
        foreach (var level in levels)
        {
            var levelChanged = TrySetDatumTextSize(level, sizeFeet);
            var levelType = context.Document.GetElement(level.GetTypeId());
            var typeChanged = levelType is not null && TrySetDatumTextSize(levelType, sizeFeet);
            var tagChanged = false;
            long? tagTypeId = null;
            var tagIdParameter = levelType?.get_Parameter(BuiltInParameter.LEVEL_HEAD_TAG);
            if (tagIdParameter is not null)
            {
                var tagId = tagIdParameter.AsElementId();
                if (tagId is not null && !tagId.Equals(ElementId.InvalidElementId))
                {
                    tagTypeId = tagId.Value;
                    if (visitedTagTypes.Add(tagId.Value))
                    {
                        var tagType = context.Document.GetElement(tagId);
                        tagChanged = tagType is not null && TrySetDatumTextSize(tagType, sizeFeet);
                    }
                }
            }

            if (levelChanged || typeChanged || tagChanged)
            {
                changed.Add(new
                {
                    LevelId = level.Id.Value,
                    Name = level.Name,
                    InstanceChanged = levelChanged,
                    TypeChanged = typeChanged,
                    TagTypeId = tagTypeId,
                    TagTypeChanged = tagChanged,
                });
            }
        }

        return new
        {
            SizeMm = sizeMm,
            ChangedCount = changed.Count,
            Levels = changed,
        };
    }

    private static bool TrySetDatumTextSize(Element element, double sizeFeet)
    {
        var parameter = element.get_Parameter(BuiltInParameter.DATUM_TEXT);
        if (parameter is null || parameter.IsReadOnly || parameter.StorageType != StorageType.Double)
        {
            return false;
        }

        parameter.Set(sizeFeet);
        return true;
    }
    private static object CreateStraightWall(InvocationContext context)
    {
        var source = context.Arguments;
        var start = StartPoint(source);
        var end = EndPoint(source);
        var levelId = ResolveLevelId(context.Document, source);
        var wallTypeId = ResolveWallTypeId(context.Document, source);

        var heightMm = Json.Double(source, "heightMm", Json.Double(source, "height", 3600));
        var offsetMm = Json.Double(source, "offsetMm", Json.Double(source, "offset", 0));
        var wall = Wall.Create(
            context.Document,
            Line.CreateBound(start, end),
            new ElementId(wallTypeId),
            new ElementId(levelId),
            Units.MmToFeet(heightMm),
            Units.MmToFeet(offsetMm),
            Json.Bool(source, "flip", false),
            Json.Bool(source, "structural", false));

        return new
        {
            WallId = wall.Id.Value,
            LevelId = levelId,
            WallTypeId = wallTypeId,
            HeightMm = heightMm,
            Start = Units.PointDto(start),
            End = Units.PointDto(end),
        };
    }

    private static object CreateWallWithOpenings(InvocationContext context)
    {
        var source = context.Arguments;
        var start = StartPoint(source);
        var end = EndPoint(source);
        var lengthMm = Units.FeetToMm(start.DistanceTo(end));
        if (lengthMm < 1)
        {
            throw new InvalidOperationException("A wall with openings requires positive length.");
        }

        var levelId = ResolveLevelId(context.Document, source);
        var wallTypeId = ResolveWallTypeId(context.Document, source);

        var heightMm = Math.Max(1, Json.Double(source, "heightMm", Json.Double(source, "height", 3600)));
        var flip = Json.Bool(source, "flip", false);
        var structural = Json.Bool(source, "structural", false);
        var openings = Json.Array(source, "openings")
            .Select(opening => new
            {
                OffsetMm = Json.Double(opening, "offsetMm", Json.Double(opening, "offset")),
                WidthMm = Json.Double(opening, "widthMm", Json.Double(opening, "width", 900)),
                BottomMm = Json.Double(opening, "bottomMm", Json.Double(opening, "bottom", 0)),
                TopMm = Json.Double(opening, "topMm", Json.Double(opening, "top", 2100)),
            })
            .OrderBy(opening => opening.OffsetMm)
            .ToArray();
        var wallIds = new List<long>();
        var cursorMm = 0.0;

        foreach (var opening in openings)
        {
            if (opening.WidthMm <= 0
                || opening.OffsetMm < 0
                || opening.OffsetMm + opening.WidthMm > lengthMm + 1e-6
                || opening.TopMm <= opening.BottomMm
                || opening.TopMm > heightMm + 1e-6)
            {
                throw new InvalidOperationException("An opening is outside the wall bounds.");
            }

            AddWallSegment(
                context,
                start,
                end,
                cursorMm,
                opening.OffsetMm,
                0,
                heightMm,
                wallTypeId,
                levelId,
                flip,
                structural,
                wallIds);
            AddWallSegment(
                context,
                start,
                end,
                opening.OffsetMm,
                opening.OffsetMm + opening.WidthMm,
                0,
                opening.BottomMm,
                wallTypeId,
                levelId,
                flip,
                structural,
                wallIds);
            AddWallSegment(
                context,
                start,
                end,
                opening.OffsetMm,
                opening.OffsetMm + opening.WidthMm,
                opening.TopMm,
                heightMm,
                wallTypeId,
                levelId,
                flip,
                structural,
                wallIds);
            cursorMm = opening.OffsetMm + opening.WidthMm;
        }

        AddWallSegment(
            context,
            start,
            end,
            cursorMm,
            lengthMm,
            0,
            heightMm,
            wallTypeId,
            levelId,
            flip,
            structural,
            wallIds);

        return new
        {
            WallIds = wallIds,
            SegmentCount = wallIds.Count,
            OpeningCount = openings.Length,
            Start = Units.PointDto(start),
            End = Units.PointDto(end),
            HeightMm = heightMm,
        };
    }

    private static void AddWallSegment(
        InvocationContext context,
        XYZ start,
        XYZ end,
        double offsetStartMm,
        double offsetEndMm,
        double bottomMm,
        double topMm,
        long wallTypeId,
        long levelId,
        bool flip,
        bool structural,
        List<long> wallIds)
    {
        var segmentLengthMm = offsetEndMm - offsetStartMm;
        var segmentHeightMm = topMm - bottomMm;
        if (segmentLengthMm <= 1e-6 || segmentHeightMm <= 1e-6)
        {
            return;
        }

        var direction = (end - start).Normalize();
        var segmentStart = start + direction * Units.MmToFeet(offsetStartMm);
        var segmentEnd = start + direction * Units.MmToFeet(offsetEndMm);
        var wall = Wall.Create(
            context.Document,
            Line.CreateBound(segmentStart, segmentEnd),
            new ElementId(wallTypeId),
            new ElementId(levelId),
            Units.MmToFeet(segmentHeightMm),
            Units.MmToFeet(bottomMm),
            flip,
            structural);
        wallIds.Add(wall.Id.Value);
    }

    private static object CreateBoxes(InvocationContext context)
    {
        var boxes = Json.Array(context.Arguments, "boxes").ToArray();
        if (boxes.Length == 0)
        {
            throw new InvalidOperationException("At least one box is required.");
        }

        var created = new List<object>();
        foreach (var item in boxes)
        {
            var position = Json.Point(item, "position");
            var width = Math.Max(0.1, Json.Double(item, "widthMm", Json.Double(item, "width", 400)));
            var depth = Math.Max(0.1, Json.Double(item, "depthMm", Json.Double(item, "depth", 400)));
            var height = Math.Max(0.1, Json.Double(item, "heightMm", Json.Double(item, "height", 3600)));
            var rotation = Json.Double(item, "rotationDegrees", Json.Double(item, "rotation"));
            var solid = CreateBoxSolid(position, width, depth, height, rotation);
            var shape = DirectShape.CreateElement(
                context.Document,
                new ElementId(BuiltInCategory.OST_GenericModel));
            shape.SetShape([solid]);
            shape.SetName(Json.StringAny(item, "name") ?? "Codex 3D Box");
            created.Add(new
            {
                ElementId = shape.Id.Value,
                Position = Units.PointDto(position),
                WidthMm = width,
                DepthMm = depth,
                HeightMm = height,
                RotationDegrees = rotation,
            });
        }

        return new
        {
            Count = created.Count,
            Boxes = created,
        };
    }

    private static Solid CreateBoxSolid(
        XYZ baseCenter,
        double widthMm,
        double depthMm,
        double heightMm,
        double rotationDegrees)
    {
        var halfWidth = Units.MmToFeet(widthMm / 2.0);
        var halfDepth = Units.MmToFeet(depthMm / 2.0);
        var radians = rotationDegrees * Math.PI / 180.0;
        var cos = Math.Cos(radians);
        var sin = Math.Sin(radians);
        var corners = new[]
        {
            (X: -halfWidth, Y: -halfDepth),
            (X: halfWidth, Y: -halfDepth),
            (X: halfWidth, Y: halfDepth),
            (X: -halfWidth, Y: halfDepth),
        };
        var points = corners
            .Select(corner => new XYZ(
                baseCenter.X + corner.X * cos - corner.Y * sin,
                baseCenter.Y + corner.X * sin + corner.Y * cos,
                baseCenter.Z))
            .ToArray();
        var loop = new CurveLoop();
        for (var index = 0; index < points.Length; index++)
        {
            loop.Append(Line.CreateBound(points[index], points[(index + 1) % points.Length]));
        }

        return GeometryCreationUtilities.CreateExtrusionGeometry(
            [loop],
            XYZ.BasisZ,
            Units.MmToFeet(heightMm));
    }
    private static object CreateFloorByProfile(InvocationContext context)
    {
        var source = context.Arguments;
        var points = Json.Array(source, "points")
            .Select(point => Units.Point(Json.Double(point, "x"), Json.Double(point, "y")))
            .ToArray();
        if (points.Length < 3)
        {
            throw new InvalidOperationException("A floor profile requires at least three points.");
        }

        var levelId = ResolveLevelId(context.Document, source);
        var floorTypeId = Json.Long(source, "floorTypeId", Json.Long(source, "floor_type_id"));
        if (floorTypeId <= 0)
        {
            floorTypeId = Floor.GetDefaultFloorType(context.Document, false).Value;
        }

        var curveLoop = new CurveLoop();
        for (var index = 0; index < points.Length; index++)
        {
            curveLoop.Append(Line.CreateBound(points[index], points[(index + 1) % points.Length]));
        }

        var floor = Floor.Create(
            context.Document,
            new[] { curveLoop },
            new ElementId(floorTypeId),
            new ElementId(levelId));
        return new
        {
            FloorId = floor.Id.Value,
            LevelId = levelId,
            FloorTypeId = floorTypeId,
            PointCount = points.Length,
        };
    }

    private static object CreateDimensionByElements(InvocationContext context)
    {
        var source = context.Arguments;
        var view = ResolveView(context.Document, source, context.UiDocument.ActiveView);

        var refs = ResolveReferences(context.Document, source).ToArray();
        if (refs.Length < 2)
        {
            throw new InvalidOperationException("At least two elements are required for a dimension.");
        }

        Line line;
        if (source.ValueKind == JsonValueKind.Object
            && source.TryGetProperty("lineStart", out _)
            && source.TryGetProperty("lineEnd", out _))
        {
            line = Line.CreateBound(Json.Point(source, "lineStart"), Json.Point(source, "lineEnd"));
        }
        else
        {
            line = BuildAutomaticDimensionLine(context, source, refs);
        }

        var dimension = CreateDimension(context.Document, view, line, refs, source);
        return new
        {
            DimensionId = dimension.Id.Value,
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ElementCount = refs.Length,
        };
    }

    private object ExecuteDrawingPlanPhased(
        UIApplication app,
        JsonElement source,
        bool dryRun)
    {
        if (app.ActiveUIDocument is null)
        {
            throw new InvalidOperationException("No active Revit document.");
        }

        var context = new InvocationContext
        {
            UiApplication = app,
            Arguments = source,
            DryRun = dryRun,
        };
        var document = context.Document;
        View? planView = null;
        if (HasAnyProperty(source, "viewId", "view_id", "viewName", "view_name"))
        {
            if (Json.Bool(source, "activateView", true))
            {
                planView = ResolveView(document, source, null);
                context.UiDocument.RequestViewChange(planView);
                context.UiDocument.RefreshActiveView();
            }
        }

        var operations = Json.Array(source, "ops").ToArray();
        var results = new List<object>();
        var stopOnError = Json.Bool(source, "stopOnError", true);
        var pendingZoomIds = new List<long>();
        var group = new TransactionGroup(document, $"{BatchName}: Drawing plan");
        group.Start();

        try
        {
            using (var transaction = new Transaction(document, $"{BatchName}: Draft geometry"))
            {
                transaction.Start();
            var failureOptions = transaction.GetFailureHandlingOptions();
            failureOptions.SetFailuresPreprocessor(new WarningSuppressor());
            transaction.SetFailureHandlingOptions(failureOptions);
                ExecutePlanPhase(
                    context,
                    operations,
                    results,
                    stopOnError,
                    includeDimensions: false,
                    pendingZoomIds);
                document.Regenerate();
                transaction.Commit();
            }

            using (var transaction = new Transaction(document, $"{BatchName}: Dimensions"))
            {
                transaction.Start();
            var failureOptions = transaction.GetFailureHandlingOptions();
            failureOptions.SetFailuresPreprocessor(new WarningSuppressor());
            transaction.SetFailureHandlingOptions(failureOptions);
                ExecutePlanPhase(
                    context,
                    operations,
                    results,
                    stopOnError,
                    includeDimensions: true,
                    pendingZoomIds);
                transaction.Commit();
            }

            if (dryRun)
            {
                group.RollBack();
            }
            else
            {
                group.Assimilate();
            }
        }
        catch
        {
            if (group.GetStatus() == TransactionStatus.Started)
            {
                group.RollBack();
            }

            throw;
        }

        string? cropFitWarning = null;
        if (!dryRun
            && Json.Bool(source, "fitCropToCreated", false)
            && pendingZoomIds.Count > 0)
        {
            try
            {
                var targetView = planView ?? context.UiDocument.ActiveView;
                using var cropTransaction = new Transaction(document, $"{BatchName}: Fit view crop");
                cropTransaction.Start();
                FitViewCropToElements(
                    targetView,
                    pendingZoomIds
                        .Distinct()
                        .Select(id => document.GetElement(new ElementId(id)))
                        .Where(element => element is not null)
                        .Cast<Element>(),
                    Json.Double(source, "cropMarginMm", 500));
                cropTransaction.Commit();
            }
            catch (Exception ex)
            {
                cropFitWarning = ex.GetBaseException().Message;
            }
        }

        return new
        {
            OperationCount = operations.Length,
            Results = results.OrderBy(result =>
                (int)result.GetType().GetProperty("Index")!.GetValue(result)!).ToArray(),
            HadFailures = results.Any(result =>
                !(bool)result.GetType().GetProperty("Success")!.GetValue(result)!),
            DryRun = dryRun,
            Committed = !dryRun,
            CropFitWarning = cropFitWarning,
        };
    }

    private static void ExecutePlanPhase(
        InvocationContext context,
        JsonElement[] operations,
        List<object> results,
        bool stopOnError,
        bool includeDimensions,
        List<long> pendingZoomIds)
    {
        for (var index = 0; index < operations.Length; index++)
        {
            var operation = operations[index];
            var type = Json.String(operation, "type")?.Trim().ToLowerInvariant() ?? string.Empty;
            var isDimension = type == "create_dimensions";
            if (isDimension != includeDimensions)
            {
                continue;
            }

            if (type == "zoom_to_elements")
            {
                pendingZoomIds.AddRange(
                    ElementIds(operation, "elementIds")
                        .Concat(SingleElementId(operation, "elementId")));
                results.Add(new
                {
                    Index = index,
                    Type = type,
                    Success = true,
                    Data = new { Deferred = true },
                });
                continue;
            }

            try
            {
                object? data = type switch
                {
                    "create_grids" => CreateGrids(context with { Arguments = operation }),
                    "create_level" => CreateLevel(context with { Arguments = operation }),
                    "create_straight_wall" => CreateStraightWall(context with { Arguments = operation }),
                    "create_wall_with_openings" => CreateWallWithOpenings(context with { Arguments = operation }),
                    "create_floor_by_profile" => CreateFloorByProfile(context with { Arguments = operation }),
                    "create_boxes" => CreateBoxes(context with { Arguments = operation }),
                    "create_structural_column" => CreateStructuralColumn(context with { Arguments = operation }),
                    "create_structural_beam" => CreateStructuralBeam(context with { Arguments = operation }),
                    "create_room" => CreateRoom(context with { Arguments = operation }),
                    "delete_elements" => DeleteElements(context with { Arguments = operation }),
                    "load_family" => LoadFamilyTool(context with { Arguments = operation }),
                    "create_door" => CreateHostedInstance(context with { Arguments = operation }, true),
                    "create_window" => CreateHostedInstance(context with { Arguments = operation }, false),
                    "create_dimensions" => CreateDimensionGroups(context, operation),
                    "import_pdf_underlay" => ImportPdfUnderlay(context with { Arguments = operation }),
                    "create_detail_lines" => CreateDetailLines(context with { Arguments = operation }),
                    "create_detail_curves" => CreateDetailCurves(context with { Arguments = operation }),
                    "create_text_notes" => CreateTextNotes(context with { Arguments = operation }),
                    "set_color" or "set_element_color" => SetElementColor(context with { Arguments = operation }),
                    "move_elements" => MoveElements(context with { Arguments = operation }),
                    "rotate_element" or "rotate_elements" => RotateElements(context with { Arguments = operation }),
                    _ => throw new InvalidOperationException($"Unsupported drawing-plan operation '{type}'."),
                };

                results.Add(new
                {
                    Index = index,
                    Type = type,
                    Success = true,
                    Data = data,
                });
                CollectZoomIds(data, pendingZoomIds);
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    Index = index,
                    Type = type,
                    Success = false,
                    Error = ex.Message,
                });

                if (stopOnError)
                {
                    throw new InvalidOperationException(
                        $"Drawing plan failed at operation {index} ({type}): {ex.Message}",
                        ex);
                }
            }
        }
    }

    private static object ExecuteBatch(InvocationContext context)
    {
        var operations = Json.Array(context.Arguments, "steps").ToArray();
        var results = new List<object>();
        var stopOnError = Json.Bool(context.Arguments, "stopOnError", true);

        for (var index = 0; index < operations.Length; index++)
        {
            var step = operations[index];
            var tool = Json.StringAny(step, "tool", "command")
                ?? throw new InvalidOperationException($"Batch step {index} has no tool name.");
            var args = step.ValueKind == JsonValueKind.Object
                && step.TryGetProperty("arguments", out var arguments)
                ? arguments
                : step.ValueKind == JsonValueKind.Object
                    && step.TryGetProperty("params", out var parameters)
                    ? parameters
                    : Json.EmptyObject();

            try
            {
                var dispatcher = new ToolDispatcher();
                var toolDefinition = dispatcher._tools[tool];
                var data = toolDefinition.Handler(context with { Arguments = args });
                results.Add(new
                {
                    Index = index,
                    Tool = tool,
                    Success = true,
                    Data = data,
                });
            }
            catch (Exception ex)
            {
                results.Add(new
                {
                    Index = index,
                    Tool = tool,
                    Success = false,
                    Error = ex.Message,
                });

                if (stopOnError)
                {
                    throw new InvalidOperationException(
                        $"Batch failed at step {index} ({tool}): {ex.Message}",
                        ex);
                }
            }
        }

        return new
        {
            Committed = true,
            Count = operations.Length,
            Results = results,
        };
    }

    private static object CreateDimensionGroups(InvocationContext context, JsonElement operation)
    {
        var groups = Json.Array(operation, "groups").ToArray();
        var results = new List<object>();

        foreach (var group in groups)
        {
            var view = ResolveView(context.Document, group, context.UiDocument.ActiveView);
            var refs = ResolveReferences(context.Document, group).ToArray();
            if (refs.Length < 2)
            {
                throw new InvalidOperationException("A dimension group requires at least two references.");
            }

            var line = Line.CreateBound(Json.Point(group, "lineStart"), Json.Point(group, "lineEnd"));
            var dimension = CreateDimension(context.Document, view, line, refs, group);
            results.Add(new
            {
                Name = Json.StringAny(group, "name", "groupName"),
                DimensionId = dimension.Id.Value,
                ViewId = view.Id.Value,
                ReferenceCount = refs.Length,
                LineStart = Units.PointDto(line.GetEndPoint(0)),
                LineEnd = Units.PointDto(line.GetEndPoint(1)),
            });
        }

        return new
        {
            Count = results.Count,
            Dimensions = results,
        };
    }

    private static object ImportPdfUnderlay(InvocationContext context)
    {
        var source = context.Arguments;
        var path = Json.StringAny(source, "path", "filePath")
            ?? throw new InvalidOperationException("PDF path is required.");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("PDF file was not found.", path);
        }

        var pageNumber = Math.Max(1, Json.Int(source, "pageNumber", Json.Int(source, "page", 1)));
        var resolution = Math.Max(72, Json.Int(source, "resolution", 300));
        var useRelativePath = Json.Bool(source, "useRelativePath");
        var sourceTypeName = Json.String(source, "sourceType");
        var sourceType = string.Equals(sourceTypeName, "link", StringComparison.OrdinalIgnoreCase)
            ? ImageTypeSource.Link
            : ImageTypeSource.Import;

        var imageTypeOptions = new ImageTypeOptions(path, useRelativePath, sourceType)
        {
            PageNumber = pageNumber,
            Resolution = resolution,
        };

        if (!imageTypeOptions.IsValid(context.Document))
        {
            throw new InvalidOperationException("Revit rejected the PDF image options.");
        }

        var imageType = ImageType.Create(context.Document, imageTypeOptions);
        var view = ResolveView(context.Document, source, context.UiDocument.ActiveView);

        var location = source.TryGetProperty("location", out _)
            ? Json.Point(source, "location")
            : XYZ.Zero;
        var placement = new ImagePlacementOptions(location, BoxPlacement.Center);
        var instance = ImageInstance.Create(
            context.Document,
            view,
            imageType.Id,
            placement);

        var widthMm = Json.Double(source, "widthMm", Json.Double(source, "width", 0));
        var heightMm = Json.Double(source, "heightMm", Json.Double(source, "height", 0));
        if (widthMm > 0)
        {
            instance.Width = Units.MmToFeet(widthMm);
        }

        if (heightMm > 0)
        {
            instance.Height = Units.MmToFeet(heightMm);
        }

        var rotationDegrees = Json.Double(source, "rotationDegrees", 0);
        if (Math.Abs(rotationDegrees) > 1e-9)
        {
            var axis = Line.CreateUnbound(
                instance.GetLocation(BoxPlacement.Center),
                XYZ.BasisZ);
            ElementTransformUtils.RotateElement(
                context.Document,
                instance.Id,
                axis,
                rotationDegrees * Math.PI / 180.0);
        }

        if (instance.CanHaveSnaps)
        {
            instance.EnableSnaps = true;
        }

        return new
        {
            ImageTypeId = imageType.Id.Value,
            ImageInstanceId = instance.Id.Value,
            ViewId = view.Id.Value,
            ViewName = view.Name,
            PageNumber = pageNumber,
            Resolution = resolution,
            SourceType = sourceType.ToString(),
            CanHaveSnaps = instance.CanHaveSnaps,
            SnapsEnabled = instance.CanHaveSnaps && instance.EnableSnaps,
            WidthMm = Math.Round(Units.FeetToMm(instance.Width), 3),
            HeightMm = Math.Round(Units.FeetToMm(instance.Height), 3),
        };
    }

    private static object CreateDetailLines(InvocationContext context)
    {
        var view = ResolveView(context.Document, context.Arguments, context.UiDocument.ActiveView);
        var lines = Json.Array(context.Arguments, "lines").ToArray();
        var created = new List<object>();
        var hasOperationColor = TryReadColor(context.Arguments, out var operationColor);

        foreach (var item in lines)
        {
            var start = Json.Point(item, "start");
            var end = Json.Point(item, "end");
            var curve = Line.CreateBound(start, end);
            var detailLine = context.Document.Create.NewDetailCurve(view, curve);
            var lineStyleId = Json.Long(item, "lineStyleId", 0);
            if (lineStyleId > 0)
            {
                detailLine.LineStyle = context.Document.GetElement(new ElementId(lineStyleId)) as GraphicsStyle;
            }

            if (TryReadColor(item, out var itemColor) || hasOperationColor)
            {
                var color = TryReadColor(item, out itemColor) ? itemColor : operationColor;
                var overrides = new OverrideGraphicSettings();
                overrides.SetProjectionLineColor(color);
                view.SetElementOverrides(detailLine.Id, overrides);
            }

            created.Add(new
            {
                ElementId = detailLine.Id.Value,
                Start = Units.PointDto(start),
                End = Units.PointDto(end),
            });
        }

        return new
        {
            Count = created.Count,
            Lines = created,
        };
    }

    private static object CreateDetailCurves(InvocationContext context)
    {
        var source = context.Arguments;
        var view = ResolveView(context.Document, source, context.UiDocument.ActiveView);
        var items = Json.Array(source, "curves").ToArray();
        if (items.Length == 0)
        {
            items = Json.Array(source, "lines").ToArray();
        }

        if (items.Length == 0)
        {
            throw new InvalidOperationException("At least one curve is required.");
        }

        var hasOperationColor = TryReadColor(source, out var operationColor);
        var created = new List<object>();
        foreach (var item in items)
        {
            var lineStyleId = Json.Long(item, "lineStyleId", 0);
            var hasItemColor = TryReadColor(item, out var itemColor);
            foreach (var curve in BuildDetailCurves(item))
            {
                if (curve.Length < Units.MmToFeet(0.001))
                {
                    throw new InvalidOperationException("A detail curve has near-zero length.");
                }

                var detailCurve = context.Document.Create.NewDetailCurve(view, curve);
                if (lineStyleId > 0)
                {
                    detailCurve.LineStyle = context.Document.GetElement(new ElementId(lineStyleId)) as GraphicsStyle;
                }

                if (hasItemColor || hasOperationColor)
                {
                    var color = hasItemColor ? itemColor : operationColor;
                    var overrides = new OverrideGraphicSettings();
                    overrides.SetProjectionLineColor(color);
                    view.SetElementOverrides(detailCurve.Id, overrides);
                }

                created.Add(new
                {
                    ElementId = detailCurve.Id.Value,
                    CurveType = curve.GetType().Name,
                    Start = Units.PointDto(curve.GetEndPoint(0)),
                    End = Units.PointDto(curve.GetEndPoint(1)),
                    Mid = Units.PointDto(curve.Evaluate(0.5, true)),
                    LengthMm = Math.Round(Units.FeetToMm(curve.Length), 4),
                });
            }
        }

        return new
        {
            Count = created.Count,
            Curves = created,
            ViewId = view.Id.Value,
            ViewName = view.Name,
        };
    }

    private static object CreateTextNotes(InvocationContext context)
    {
        var source = context.Arguments;
        var view = ResolveView(context.Document, source, context.UiDocument.ActiveView);
        var notes = Json.Array(source, "notes").ToArray();
        if (notes.Length == 0 && !string.IsNullOrWhiteSpace(Json.String(source, "text")))
        {
            notes = [source];
        }

        if (notes.Length == 0)
        {
            throw new InvalidOperationException("At least one text note is required.");
        }

        var defaultTypeId = ResolveTextNoteTypeId(context.Document, source);
        var hasOperationColor = TryReadColor(source, out var operationColor);
        var created = new List<object>();
        foreach (var item in notes)
        {
            var text = Json.StringAny(item, "text", "content")
                ?? throw new InvalidOperationException("A text note requires text.");
            var position = Json.Point(item, "position");
            var typeId = Json.Long(item, "textTypeId", defaultTypeId);
            if (context.Document.GetElement(new ElementId(typeId)) is not TextNoteType)
            {
                typeId = defaultTypeId;
            }

            var note = TextNote.Create(
                context.Document,
                view.Id,
                position,
                text,
                new ElementId(typeId));
            var widthMm = Json.Double(item, "widthMm", Json.Double(item, "width", 0));
            if (widthMm > 0)
            {
                note.Width = Units.MmToFeet(widthMm);
            }

            if (TryReadColor(item, out var itemColor) || hasOperationColor)
            {
                var color = TryReadColor(item, out itemColor) ? itemColor : operationColor;
                var overrides = new OverrideGraphicSettings();
                overrides.SetProjectionLineColor(color);
                view.SetElementOverrides(note.Id, overrides);
            }

            created.Add(new
            {
                TextNoteId = note.Id.Value,
                Text = text,
                Position = Units.PointDto(position),
                TypeId = typeId,
            });
        }

        return new
        {
            Count = created.Count,
            Notes = created,
            ViewId = view.Id.Value,
            ViewName = view.Name,
        };
    }
    private static object SetElementColor(InvocationContext context)
    {
        var source = context.Arguments;
        var view = ResolveView(context.Document, source, context.UiDocument.ActiveView);
        var elementIds = ElementIds(source, "elements")
            .Concat(ElementIds(source, "elementIds"))
            .Distinct()
            .ToArray();
        if (elementIds.Length == 0)
        {
            throw new InvalidOperationException("No elements were supplied.");
        }

        var red = 0;
        var green = 0;
        var blue = 0;
        if (source.TryGetProperty("color", out var color) && color.ValueKind == JsonValueKind.Object)
        {
            red = Json.Int(color, "red", Json.Int(color, "r"));
            green = Json.Int(color, "green", Json.Int(color, "g"));
            blue = Json.Int(color, "blue", Json.Int(color, "b"));
        }
        else
        {
            red = Json.Int(source, "red");
            green = Json.Int(source, "green");
            blue = Json.Int(source, "blue");
        }

        var lineColor = new Color(
            (byte)Math.Clamp(red, 0, 255),
            (byte)Math.Clamp(green, 0, 255),
            (byte)Math.Clamp(blue, 0, 255));
        var overrides = new OverrideGraphicSettings();
        overrides.SetProjectionLineColor(lineColor);
        overrides.SetCutLineColor(lineColor);
        overrides.SetSurfaceForegroundPatternColor(lineColor);
        overrides.SetCutForegroundPatternColor(lineColor);

        foreach (var id in elementIds)
        {
            view.SetElementOverrides(new ElementId(id), overrides);
        }

        return new
        {
            Count = elementIds.Length,
            ElementIds = elementIds,
            ViewId = view.Id.Value,
            Color = new { Red = lineColor.Red, Green = lineColor.Green, Blue = lineColor.Blue },
        };
    }

    private static object MoveElements(InvocationContext context)
    {
        var ids = ElementIds(context.Arguments, "elementIds")
            .Concat(ElementIds(context.Arguments, "elements"))
            .Concat(SingleElementId(context.Arguments, "elementId"))
            .Distinct()
            .ToArray();
        if (ids.Length == 0)
        {
            throw new InvalidOperationException("No elements were supplied.");
        }

        var displacement = Units.Vector(
            Json.Double(context.Arguments, "x"),
            Json.Double(context.Arguments, "y"),
            Json.Double(context.Arguments, "z"));
        ElementTransformUtils.MoveElements(
            context.Document,
            ids.Select(id => new ElementId(id)).ToArray(),
            displacement);

        return new
        {
            MovedCount = ids.Length,
            ElementIds = ids,
            Displacement = Units.PointDto(displacement),
        };
    }

    private static object RotateElements(InvocationContext context)
    {
        var ids = ElementIds(context.Arguments, "elementIds")
            .Concat(SingleElementId(context.Arguments, "elementId"))
            .Distinct()
            .ToArray();
        if (ids.Length == 0)
        {
            throw new InvalidOperationException("No elements were supplied.");
        }

        var angle = Json.Double(context.Arguments, "angleDegrees", Json.Double(context.Arguments, "angle"));
        var axis = context.Arguments.TryGetProperty("axis", out var axisElement)
            ? Json.Point(axisElement, "direction")
            : new XYZ(
                Json.Double(context.Arguments, "axisX", 0),
                Json.Double(context.Arguments, "axisY", 0),
                Json.Double(context.Arguments, "axisZ", 1));
        if (axis.IsZeroLength())
        {
            axis = XYZ.BasisZ;
        }

        var origin = context.Arguments.TryGetProperty("origin", out _)
            ? Json.Point(context.Arguments, "origin")
            : new XYZ(
                Units.MmToFeet(Json.Double(context.Arguments, "originX")),
                Units.MmToFeet(Json.Double(context.Arguments, "originY")),
                Units.MmToFeet(Json.Double(context.Arguments, "originZ")));
        var rotationAxis = Line.CreateUnbound(origin, axis.Normalize());

        foreach (var id in ids)
        {
            ElementTransformUtils.RotateElement(
                context.Document,
                new ElementId(id),
                rotationAxis,
                angle * Math.PI / 180.0);
        }

        return new
        {
            RotatedCount = ids.Length,
            ElementIds = ids,
            AngleDegrees = angle,
            Origin = Units.PointDto(origin),
        };
    }

    private static object ZoomToElements(InvocationContext context)
    {
        var ids = ElementIds(context.Arguments, "elementIds")
            .Concat(SingleElementId(context.Arguments, "elementId"))
            .Distinct()
            .ToArray();
        if (ids.Length == 0)
        {
            throw new InvalidOperationException("No elements were supplied.");
        }

        context.UiDocument.ShowElements(ids.Select(id => new ElementId(id)).ToArray());
        context.UiDocument.RefreshActiveView();
        return new
        {
            ElementCount = ids.Length,
            ViewId = context.UiDocument.ActiveView.Id.Value,
        };
    }

    private static object SaveDocument(InvocationContext context)
    {
        var document = context.Document;
        var requestedPath = Json.StringAny(context.Arguments, "path", "filePath", "savePath");
        if (string.IsNullOrWhiteSpace(requestedPath))
        {
            if (!string.IsNullOrWhiteSpace(document.PathName))
            {
                document.Save();
                return new
                {
                    Title = document.Title,
                    Path = document.PathName,
                    IsModified = document.IsModified,
                    SavedAs = false,
                    DefaultLocationUsed = false,
                };
            }

            var safeTitle = string.Concat(document.Title.Select(character =>
                Path.GetInvalidFileNameChars().Contains(character) ? '_' : character));
            var defaultDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "RevitAI Models");
            var defaultPath = Path.Combine(defaultDirectory, safeTitle + ".rvt");
            Directory.CreateDirectory(defaultDirectory);
            SaveDocumentPath(document, defaultPath, overwrite: true);
            return new
            {
                Title = document.Title,
                Path = document.PathName,
                IsModified = document.IsModified,
                SavedAs = true,
                DefaultLocationUsed = true,
            };
        }

        var fullPath = Path.IsPathRooted(requestedPath)
            ? Path.GetFullPath(requestedPath)
            : Path.GetFullPath(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "RevitAI Models",
                requestedPath));
        if (!Path.GetExtension(fullPath).Equals(".rvt", StringComparison.OrdinalIgnoreCase))
        {
            fullPath = Path.ChangeExtension(fullPath, ".rvt");
        }

        Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        SaveDocumentPath(document, fullPath, Json.Bool(context.Arguments, "overwrite", true), Json.Int(context.Arguments, "maximumBackups", 1));
        return new
        {
            Title = document.Title,
            Path = document.PathName,
            IsModified = document.IsModified,
            SavedAs = true,
            DefaultLocationUsed = false,
        };
    }

    private static void SaveDocumentPath(
        Document document,
        string path,
        bool overwrite,
        int maximumBackups = 1)
    {
        var options = new SaveAsOptions
        {
            OverwriteExistingFile = overwrite,
            Compact = true,
            MaximumBackups = Math.Max(1, maximumBackups),
        };
        document.SaveAs(path, options);
    }
    private static object Create3DView(InvocationContext context)
    {
        var name = Json.StringAny(context.Arguments, "name", "viewName")
            ?? "Codex 3D View";
        var view = CreateOrGet3DView(context.Document, name);
        var autoFloorIds = Json.Bool(context.Arguments, "hideFloors", false)
            ? new FilteredElementCollector(context.Document)
                .OfClass(typeof(Floor))
                .Select(element => element.Id)
                .ToArray()
            : [];
        var hideIds = ElementIds(context.Arguments, "hideElementIds")
            .Concat(ElementIds(context.Arguments, "elementIds"))
            .Select(id => new ElementId(id))
            .Concat(autoFloorIds)
            .Distinct()
            .ToArray();
        if (hideIds.Length > 0)
        {
            view.HideElements(hideIds);
        }

        var activate = Json.Bool(context.Arguments, "activate", false);
        if (activate)
        {
            context.UiDocument.RequestViewChange(view);
            context.UiDocument.RefreshActiveView();
        }

        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ViewType = view.ViewType.ToString(),
            HiddenElementCount = hideIds.Length,
            Activated = activate,
        };
    }

    private static View3D CreateOrGet3DView(Document document, string name)
    {
        var existing = new FilteredElementCollector(document)
            .OfClass(typeof(View3D))
            .Cast<View3D>()
            .FirstOrDefault(view => !view.IsTemplate
                && view.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (existing is not null)
        {
            return existing;
        }

        var viewFamilyType = new FilteredElementCollector(document)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .First(type => type.ViewFamily == ViewFamily.ThreeDimensional);
        var view = View3D.CreateIsometric(document, viewFamilyType.Id);
        view.Name = name;
        view.DisplayStyle = DisplayStyle.ShadingWithEdges;
        return view;
    }
    private static object HideElementsInView(InvocationContext context)
    {
        var view = ResolveView(context.Document, context.Arguments, context.UiDocument.ActiveView);
        var ids = ElementIds(context.Arguments, "elementIds")
            .Concat(ElementIds(context.Arguments, "elements"))
            .Concat(SingleElementId(context.Arguments, "elementId"))
            .Distinct()
            .Select(id => new ElementId(id))
            .ToArray();
        if (ids.Length == 0)
        {
            throw new InvalidOperationException("No elements were supplied.");
        }

        if (Json.Bool(context.Arguments, "hide", true))
        {
            view.HideElements(ids);
        }
        else
        {
            view.UnhideElements(ids);
        }

        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ElementCount = ids.Length,
            Hidden = Json.Bool(context.Arguments, "hide", true),
        };
    }
    private static object GetViewSettings(InvocationContext context)
    {
        var view = ResolveView(context.Document, context.Arguments, context.UiDocument.ActiveView);
        var crop = view.CropBox;
        var outline = view.Outline;
        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ViewType = view.ViewType.ToString(),
            view.Scale,
            DetailLevel = view.DetailLevel.ToString(),
            CropBoxActive = view.CropBoxActive,
            CropBoxVisible = view.CropBoxVisible,
            CropBox = new
            {
                Min = Units.PointDto(crop.Min),
                Max = Units.PointDto(crop.Max),
            },
            Outline = new
            {
                Min = Units.PointDto(new XYZ(outline.Min.U, outline.Min.V, 0)),
                Max = Units.PointDto(new XYZ(outline.Max.U, outline.Max.V, 0)),
            },
        };
    }

    private static object FitViewCropToElements(InvocationContext context)
    {
        var view = ResolveView(context.Document, context.Arguments, context.UiDocument.ActiveView);
        var ids = ElementIds(context.Arguments, "elementIds")
            .Concat(ElementIds(context.Arguments, "elements"))
            .Concat(SingleElementId(context.Arguments, "elementId"))
            .Distinct()
            .ToArray();
        var elements = ids.Length > 0
            ? ids.Select(id => context.Document.GetElement(new ElementId(id))).Where(element => element is not null).Cast<Element>()
            : new FilteredElementCollector(context.Document, view.Id)
                .WhereElementIsNotElementType()
                .ToElements();
        var result = FitViewCropToElements(
            view,
            elements,
            Json.Double(context.Arguments, "marginMm", Json.Double(context.Arguments, "margin", 500)));
        return result;
    }

    private static object FitViewCropToElements(
        View view,
        IEnumerable<Element> elements,
        double marginMm)
    {
        var minX = double.PositiveInfinity;
        var minY = double.PositiveInfinity;
        var maxX = double.NegativeInfinity;
        var maxY = double.NegativeInfinity;
        var count = 0;
        foreach (var element in elements)
        {
            BoundingBoxXYZ? box;
            try
            {
                box = element.get_BoundingBox(view);
            }
            catch
            {
                box = null;
            }

            if (box is null)
            {
                continue;
            }

            minX = Math.Min(minX, box.Min.X);
            minY = Math.Min(minY, box.Min.Y);
            maxX = Math.Max(maxX, box.Max.X);
            maxY = Math.Max(maxY, box.Max.Y);
            count++;
        }

        if (count == 0 || double.IsInfinity(minX) || double.IsInfinity(minY))
        {
            throw new InvalidOperationException("No elements with a visible bounding box were found.");
        }

        var margin = Units.MmToFeet(Math.Max(0, marginMm));
        var crop = view.CropBox;
        crop.Min = new XYZ(minX - margin, minY - margin, crop.Min.Z);
        crop.Max = new XYZ(maxX + margin, maxY + margin, crop.Max.Z);
        view.CropBox = crop;
        view.CropBoxActive = true;

        return new
        {
            ViewId = view.Id.Value,
            ViewName = view.Name,
            ElementCount = count,
            MarginMm = marginMm,
            CropMin = Units.PointDto(crop.Min),
            CropMax = Units.PointDto(crop.Max),
        };
    }
    private static object ExportViewImage(InvocationContext context)
    {
        var document = context.Document;
        var view = ResolveView(document, context.Arguments, context.UiDocument.ActiveView);
        var outputDirectory = Json.StringAny(context.Arguments, "outputDirectory", "exportDirectory");
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            outputDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                "RevitAI Batch Exports");
        }

        Directory.CreateDirectory(outputDirectory);
        var fileBase = Json.StringAny(context.Arguments, "fileName", "filePath")
            ?? $"View_{view.Id.Value}_{DateTime.Now:yyyyMMdd_HHmmss}";
        fileBase = Path.GetFileNameWithoutExtension(fileBase);
        var fullBase = Path.Combine(outputDirectory, fileBase);
        var width = Math.Clamp(Json.Int(context.Arguments, "width", 2400), 256, 8192);
        var attempts = new List<object>();
        var variants = new[]
        {
            new { Name = "base-fit", FilePath = fullBase, UseZoom = true },
            new { Name = "base-default", FilePath = fullBase, UseZoom = false },
            new { Name = "png-fit", FilePath = fullBase + ".png", UseZoom = true },
            new { Name = "png-default", FilePath = fullBase + ".png", UseZoom = false },
        };

        foreach (var variant in variants)
        {
            try
            {
                var options = new ImageExportOptions
                {
                    FilePath = variant.FilePath,
                    ExportRange = ExportRange.SetOfViews,
                    HLRandWFViewsFileType = ImageFileType.PNG,
                    ShadowViewsFileType = ImageFileType.PNG,
                    FitDirection = FitDirectionType.Horizontal,
                    ShouldCreateWebSite = false,
                    ZoomType = variant.UseZoom ? ZoomFitType.FitToPage : ZoomFitType.Zoom,
                };
                if (variant.UseZoom)
                {
                    options.PixelSize = width;
                }
                else
                {
                    options.ImageResolution = ImageResolution.DPI_300;
                    options.Zoom = 100;
                }
                options.SetViewsAndSheets([view.Id]);
                var validFileName = ImageExportOptions.IsValidFileName(variant.FilePath);
                var expectedPath = ImageExportOptions.GetFileName(document, view.Id);
                document.ExportImage(options);
                var matchingFiles = Directory
                    .GetFiles(outputDirectory, fileBase + "*" + ".png")
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .ToArray();
                return new
                {
                    ViewId = view.Id.Value,
                    ViewName = view.Name,
                    OutputPath = expectedPath,
                    Exists = File.Exists(expectedPath) || matchingFiles.Length > 0,
                    MatchingFiles = matchingFiles,
                    Width = width,
                    Attempt = variant.Name,
                    ValidFileName = validFileName,
                };
            }
            catch (Exception ex)
            {
                attempts.Add(new
                {
                    Variant = variant.Name,
                    FilePath = variant.FilePath,
                    Error = ex.GetBaseException().Message,
                });
            }
        }

        throw new InvalidOperationException(
            $"Failed to export image after {attempts.Count} attempts: {JsonSerializer.Serialize(attempts)}");
    }

    private static Dimension CreateDimension(
        Document document,
        View view,
        Line line,
        IReadOnlyCollection<Reference> references,
        JsonElement source)
    {
        var referenceArray = new ReferenceArray();
        foreach (var reference in references)
        {
            referenceArray.Append(reference);
        }

        var dimensionTypeId = Json.Long(source, "dimensionTypeId", 0);
        var dimension = dimensionTypeId > 0
            ? document.Create.NewDimension(
                view,
                line,
                referenceArray,
                document.GetElement(new ElementId(dimensionTypeId)) as DimensionType)
            : document.Create.NewDimension(view, line, referenceArray);

        if (dimension is null)
        {
            throw new InvalidOperationException("Revit could not create the dimension.");
        }

        if (TryReadColor(source, out var color))
        {
            var overrides = new OverrideGraphicSettings();
            overrides.SetProjectionLineColor(color);
            view.SetElementOverrides(dimension.Id, overrides);
        }

        return dimension;
    }

    private static IEnumerable<Reference> ResolveReferences(Document document, JsonElement source)
    {
        foreach (var id in ElementIds(source, "elementIds"))
        {
            var element = document.GetElement(new ElementId(id))
                ?? throw new InvalidOperationException($"Element {id} was not found.");
            yield return GetGeometricReference(element);
        }

        var gridNames = Json.Array(source, "gridNames")
            .Concat(Json.Array(source, "referenceNames"))
            .Where(item => item.ValueKind == JsonValueKind.String)
            .Select(item => item.GetString())
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .ToArray();
        if (gridNames.Length == 0)
        {
            yield break;
        }

        var grids = new FilteredElementCollector(document)
            .OfClass(typeof(Grid))
            .Cast<Grid>()
            .ToArray();
        foreach (var name in gridNames)
        {
            var grid = grids.FirstOrDefault(candidate =>
                candidate.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                ?? throw new InvalidOperationException($"Grid '{name}' was not found.");
            yield return GetGeometricReference(grid);
        }
    }

    private static Reference GetGeometricReference(Element element)
    {
        if (element is Grid grid)
        {
            return new Reference(grid);
        }

        var options = new Options { ComputeReferences = true };
        var geometry = element.get_Geometry(options);
        if (geometry is not null)
        {
            foreach (var geometryObject in geometry)
            {
                if (geometryObject is Curve curve && curve.Reference is not null)
                {
                    return curve.Reference;
                }

                if (geometryObject is Solid solid)
                {
                    foreach (Face face in solid.Faces)
                    {
                        if (face.Reference is not null)
                        {
                            return face.Reference;
                        }
                    }
                }
            }
        }

        throw new InvalidOperationException(
            $"Element {element.Id.Value} has no geometric reference usable for a dimension.");
    }

    private static Line BuildAutomaticDimensionLine(
        InvocationContext context,
        JsonElement source,
        IReadOnlyCollection<Reference> references)
    {
        var elements = references
            .Select(reference => context.Document.GetElement(reference))
            .Where(element => element is not null)
            .Cast<Element>()
            .ToArray();
        if (elements.Length == 0)
        {
            throw new InvalidOperationException("Dimension references could not be resolved.");
        }

        var boxes = elements.Select(element => element.get_BoundingBox(context.UiDocument.ActiveView))
            .Where(box => box is not null)
            .Cast<BoundingBoxXYZ>()
            .ToArray();
        if (boxes.Length == 0)
        {
            throw new InvalidOperationException("Dimension references have no visible bounding box.");
        }

        var minX = boxes.Min(box => box.Min.X);
        var maxX = boxes.Max(box => box.Max.X);
        var minY = boxes.Min(box => box.Min.Y);
        var maxY = boxes.Max(box => box.Max.Y);
        var position = Json.String(source, "position")?.ToLowerInvariant() ?? "auto";
        var offset = Units.MmToFeet(Json.Double(source, "autoOffsetDistance", 2000));

        return position switch
        {
            "左" or "left" => Line.CreateBound(new XYZ(minX - offset, minY, 0), new XYZ(minX - offset, maxY, 0)),
            "右" or "right" => Line.CreateBound(new XYZ(maxX + offset, minY, 0), new XYZ(maxX + offset, maxY, 0)),
            "下" or "bottom" => Line.CreateBound(new XYZ(minX, minY - offset, 0), new XYZ(maxX, minY - offset, 0)),
            _ => Line.CreateBound(new XYZ(minX, maxY + offset, 0), new XYZ(maxX, maxY + offset, 0)),
        };
    }

    private static IEnumerable<long> ElementIds(JsonElement element, string propertyName)
    {
        foreach (var item in Json.Array(element, propertyName))
        {
            if (item.ValueKind == JsonValueKind.Number && item.TryGetInt64(out var number))
            {
                yield return number;
            }
            else if (item.ValueKind == JsonValueKind.String
                && long.TryParse(item.GetString(), out number))
            {
                yield return number;
            }
            else if (item.ValueKind == JsonValueKind.Object
                && item.TryGetProperty("id", out var id)
                && id.TryGetInt64(out number))
            {
                yield return number;
            }
        }
    }

    private static IEnumerable<long> SingleElementId(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Object
            && element.TryGetProperty(propertyName, out var value)
            && value.TryGetInt64(out var id))
        {
            yield return id;
        }
    }

    private static XYZ StartPoint(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("start", out var start))
        {
            return Json.Point(element, "start");
        }

        return new XYZ(
            Units.MmToFeet(Json.Double(element, "start_x", Json.Double(element, "startX"))),
            Units.MmToFeet(Json.Double(element, "start_y", Json.Double(element, "startY"))),
            Units.MmToFeet(Json.Double(element, "start_z", Json.Double(element, "startZ"))));
    }

    private static XYZ EndPoint(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object && element.TryGetProperty("end", out var end))
        {
            return Json.Point(element, "end");
        }

        return new XYZ(
            Units.MmToFeet(Json.Double(element, "end_x", Json.Double(element, "endX"))),
            Units.MmToFeet(Json.Double(element, "end_y", Json.Double(element, "endY"))),
            Units.MmToFeet(Json.Double(element, "end_z", Json.Double(element, "endZ"))));
    }

    private static bool TryReadColor(JsonElement source, out Color color)
    {
        color = new Color(0, 255, 0);
        if (source.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        if (source.TryGetProperty("color", out var colorElement))
        {
            if (colorElement.ValueKind == JsonValueKind.String)
            {
                var text = colorElement.GetString()?.TrimStart('#') ?? string.Empty;
                if (text.Length == 6 && int.TryParse(text, System.Globalization.NumberStyles.HexNumber, null, out var rgb))
                {
                    color = new Color((byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
                    return true;
                }
            }
            else if (colorElement.ValueKind == JsonValueKind.Object)
            {
                color = new Color(
                    (byte)Math.Clamp(Json.Int(colorElement, "red", Json.Int(colorElement, "r", 0)), 0, 255),
                    (byte)Math.Clamp(Json.Int(colorElement, "green", Json.Int(colorElement, "g", 255)), 0, 255),
                    (byte)Math.Clamp(Json.Int(colorElement, "blue", Json.Int(colorElement, "b", 0)), 0, 255));
                return true;
            }
        }

        if (source.TryGetProperty("green", out _))
        {
            color = new Color(
                (byte)Math.Clamp(Json.Int(source, "red", 0), 0, 255),
                (byte)Math.Clamp(Json.Int(source, "green", 255), 0, 255),
                (byte)Math.Clamp(Json.Int(source, "blue", 0), 0, 255));
            return true;
        }

        return false;
    }

    private static IEnumerable<Curve> BuildDetailCurves(JsonElement item)
    {
        var curveType = (Json.StringAny(item, "type", "curveType") ?? "line")
            .Trim()
            .ToLowerInvariant();
        switch (curveType)
        {
            case "":
            case "line":
            case "detail_line":
                yield return Line.CreateBound(Json.Point(item, "start"), Json.Point(item, "end"));
                yield break;

            case "polyline":
            {
                var points = Json.Array(item, "points")
                    .Select(point => Units.Point(
                        Json.Double(point, "x", Json.Double(point, "X")),
                        Json.Double(point, "y", Json.Double(point, "Y")),
                        Json.Double(point, "z", Json.Double(point, "Z"))))
                    .ToArray();
                if (points.Length < 2)
                {
                    throw new InvalidOperationException("A polyline requires at least two points.");
                }

                for (var index = 0; index < points.Length - 1; index++)
                {
                    yield return Line.CreateBound(points[index], points[index + 1]);
                }

                if (Json.Bool(item, "closed"))
                {
                    yield return Line.CreateBound(points[^1], points[0]);
                }

                yield break;
            }

            case "arc":
            case "arc_center":
            {
                var center = Json.Point(item, "center");
                var radiusMm = Json.Double(item, "radiusMm", Json.Double(item, "radius"));
                if (radiusMm <= 0)
                {
                    throw new InvalidOperationException("An arc requires a positive radius.");
                }

                var startDegrees = Json.Double(
                    item,
                    "startAngleDegrees",
                    Json.Double(item, "startAngle"));
                var endDegrees = Json.Double(
                    item,
                    "endAngleDegrees",
                    Json.Double(item, "endAngle"));
                while (endDegrees <= startDegrees)
                {
                    endDegrees += 360;
                }

                yield return Arc.Create(
                    center,
                    Units.MmToFeet(radiusMm),
                    startDegrees * Math.PI / 180.0,
                    endDegrees * Math.PI / 180.0,
                    XYZ.BasisX,
                    XYZ.BasisY);
                yield break;
            }

            case "arc_three_point":
            case "arcthreepoint":
            {
                XYZ pointOnArc;
                if (item.TryGetProperty("pointOnArc", out _))
                {
                    pointOnArc = Json.Point(item, "pointOnArc");
                }
                else if (item.TryGetProperty("mid", out _))
                {
                    pointOnArc = Json.Point(item, "mid");
                }
                else if (item.TryGetProperty("through", out _))
                {
                    pointOnArc = Json.Point(item, "through");
                }
                else
                {
                    throw new InvalidOperationException("A three-point arc requires pointOnArc.");
                }

                yield return Arc.Create(
                    Json.Point(item, "start"),
                    Json.Point(item, "end"),
                    pointOnArc);
                yield break;
            }

            default:
                throw new InvalidOperationException($"Unsupported detail curve type '{curveType}'.");
        }
    }

    private static long ResolveTextNoteTypeId(Document document, JsonElement source)
    {
        var typeId = Json.Long(source, "textTypeId", Json.Long(source, "text_type_id"));
        if (typeId > 0 && document.GetElement(new ElementId(typeId)) is TextNoteType)
        {
            return typeId;
        }

        var textType = new FilteredElementCollector(document)
            .OfClass(typeof(TextNoteType))
            .Cast<TextNoteType>()
            .FirstOrDefault();
        return textType?.Id.Value
            ?? throw new InvalidOperationException("No text note type is available in the document.");
    }

    private static View ResolveView(Document document, JsonElement source, View? fallback)
    {
        var viewId = Json.Long(source, "viewId", Json.Long(source, "view_id"));
        var viewName = Json.StringAny(source, "viewName", "view_name");
        if (viewId > 0)
        {
            if (document.GetElement(new ElementId(viewId)) is View idView && !idView.IsTemplate)
            {
                return idView;
            }

            if (string.IsNullOrWhiteSpace(viewName))
            {
                throw new InvalidOperationException(
                    $"View {viewId} was not found in the current document. Resolve viewId again or use viewName.");
            }
        }

        if (string.IsNullOrWhiteSpace(viewName))
        {
            return fallback
                ?? throw new InvalidOperationException("A valid viewId or viewName is required.");
        }

        var candidates = new FilteredElementCollector(document)
            .OfClass(typeof(View))
            .Cast<View>()
            .Where(view => !view.IsTemplate
                && view.Name.Equals(viewName, StringComparison.OrdinalIgnoreCase))
            .ToArray();
        var requestedViewType = Json.StringAny(source, "viewType", "view_type");
        if (!string.IsNullOrWhiteSpace(requestedViewType))
        {
            candidates = candidates
                .Where(view => view.ViewType.ToString().Equals(
                    requestedViewType,
                    StringComparison.OrdinalIgnoreCase))
                .ToArray();
        }
        else
        {
            var floorPlans = candidates
                .Where(view => view.ViewType == ViewType.FloorPlan)
                .ToArray();
            if (floorPlans.Length > 0)
            {
                candidates = floorPlans;
            }
        }

        if (candidates.Length == 0)
        {
            throw new InvalidOperationException($"View '{viewName}' was not found.");
        }

        if (candidates.Length == 1)
        {
            return candidates[0];
        }

        if (fallback is not null)
        {
            var activeMatch = candidates.FirstOrDefault(view => view.Id.Value == fallback.Id.Value);
            if (activeMatch is not null)
            {
                return activeMatch;
            }
        }

        var primaryViews = candidates.Where(IsPrimaryView).ToArray();
        if (primaryViews.Length == 1)
        {
            return primaryViews[0];
        }

        var options = string.Join(
            ", ",
            candidates.Select(view => $"{view.Id.Value}:{view.ViewType}"));
        throw new InvalidOperationException(
            $"View name '{viewName}' is ambiguous in the current document. Use viewId. Candidates: {options}");
    }

    private static bool IsPrimaryView(View view)
    {
        try
        {
            return view.GetPrimaryViewId().Value == view.Id.Value;
        }
        catch
        {
            return true;
        }
    }

    private static bool HasAnyProperty(JsonElement element, params string[] names)
    {
        if (element.ValueKind != JsonValueKind.Object)
        {
            return false;
        }

        return names.Any(name => element.TryGetProperty(name, out _));
    }

    private static void CollectZoomIds(object? value, List<long> ids)
    {
        if (value is null)
        {
            return;
        }

        if (value is ElementId elementId)
        {
            ids.Add(elementId.Value);
            return;
        }

        if (value is long id)
        {
            ids.Add(id);
            return;
        }

        if (value is int intId)
        {
            ids.Add(intId);
            return;
        }

        if (value is string text && long.TryParse(text, out var parsedId))
        {
            ids.Add(parsedId);
            return;
        }

        if (value is System.Collections.IEnumerable enumerable)
        {
            foreach (var item in enumerable)
            {
                CollectZoomIds(item, ids);
            }

            return;
        }

        var zoomProperties = new HashSet<string>(StringComparer.Ordinal)
        {
            "GridId",
            "WallId",
            "FloorId",
            "DimensionId",
            "ElementId",
            "ImageInstanceId",
            "TextNoteId",
            "ElementIds",
            "CreatedGrids",
            "Dimensions",
            "Lines",
            "Curves",
            "Notes",
        };
        foreach (var property in value.GetType().GetProperties())
        {
            if (!zoomProperties.Contains(property.Name))
            {
                continue;
            }

            CollectZoomIds(property.GetValue(value), ids);
        }
    }
    private static long ResolveWallTypeId(Document document, JsonElement source)
    {
        var wallTypeId = Json.Long(source, "wallTypeId", Json.Long(source, "wall_type_id"));
        if (wallTypeId > 0 && document.GetElement(new ElementId(wallTypeId)) is WallType)
        {
            return wallTypeId;
        }

        var wallTypeName = Json.StringAny(source, "wallTypeName", "wall_type_name");
        var wallTypes = new FilteredElementCollector(document)
            .OfClass(typeof(WallType))
            .Cast<WallType>()
            .Where(type => type.Kind == WallKind.Basic)
            .ToArray();
        if (!string.IsNullOrWhiteSpace(wallTypeName))
        {
            var named = wallTypes.FirstOrDefault(type =>
                type.Name.Equals(wallTypeName, StringComparison.OrdinalIgnoreCase))
                ?? wallTypes.FirstOrDefault(type =>
                    type.Name.Contains(wallTypeName, StringComparison.OrdinalIgnoreCase));
            if (named is not null)
            {
                return named.Id.Value;
            }

            throw new InvalidOperationException($"Wall type '{wallTypeName}' was not found.");
        }

        var twoHundred = wallTypes.FirstOrDefault(type =>
            type.Name.Contains("200", StringComparison.OrdinalIgnoreCase));
        return (twoHundred ?? wallTypes.First()).Id.Value;
    }
    private static long ResolveLevelId(Document document, JsonElement source)
    {
        var levelId = Json.Long(source, "levelId", Json.Long(source, "level_id"));
        if (levelId > 0)
        {
            if (document.GetElement(new ElementId(levelId)) is Level existingLevel)
            {
                return existingLevel.Id.Value;
            }
        }

        var levelName = Json.StringAny(source, "levelName", "level_name")
            ?? throw new InvalidOperationException("A levelId or levelName is required.");
        var level = new FilteredElementCollector(document)
            .OfClass(typeof(Level))
            .Cast<Level>()
            .FirstOrDefault(candidate =>
                candidate.Name.Equals(levelName, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Level '{levelName}' was not found.");
        return level.Id.Value;
    }
}
