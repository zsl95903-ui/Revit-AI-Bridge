using System.Text.Json;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Structure;

namespace ReVitAI.Bridge;

internal sealed partial class ToolDispatcher
{
    private static object GetFamilyTypes(InvocationContext context)
    {
        var source = context.Arguments;
        var familyFilter = Json.StringAny(source, "familyName", "familyContains");
        var typeFilter = Json.StringAny(source, "nameContains", "typeName");
        var categoryFilter = Json.StringAny(source, "category");
        var builtInCategoryFilter = Json.StringAny(source, "builtInCategory", "built_in_category");
        var limit = Math.Clamp(Json.Int(source, "limit", 500), 1, 2000);
        var categoryId = ResolveOptionalCategoryId(context.Document, source);

        var symbols = new FilteredElementCollector(context.Document)
            .OfClass(typeof(FamilySymbol))
            .Cast<FamilySymbol>()
            .Where(symbol =>
            {
                var familyName = symbol.Family?.Name ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(familyFilter)
                    && !familyName.Contains(familyFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(typeFilter)
                    && !symbol.Name.Contains(typeFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(builtInCategoryFilter)
                    && !string.Equals(TryGetBuiltInCategory(symbol.Category), builtInCategoryFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                if (categoryId is not null && symbol.Category?.Id.Value != categoryId.Value)
                {
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(categoryFilter)
                    && !(symbol.Category?.Name ?? string.Empty)
                        .Contains(categoryFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            })
            .OrderBy(symbol => symbol.Category?.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(symbol => symbol.Family?.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(symbol => symbol.Name, StringComparer.OrdinalIgnoreCase)
            .Take(limit)
            .Select(symbol => new
            {
                TypeId = symbol.Id.Value,
                FamilyName = symbol.Family?.Name,
                TypeName = symbol.Name,
                CategoryId = symbol.Category?.Id.Value,
                Category = symbol.Category?.Name,
                BuiltInCategory = TryGetBuiltInCategory(symbol.Category),
                IsActive = symbol.IsActive,
            })
            .ToArray();

        return new
        {
            Items = symbols,
            Count = symbols.Length,
        };
    }

    private static object CreateStructuralColumn(InvocationContext context)
    {
        var source = context.Arguments;
        var position = Json.Point(source, "position");
        var levelId = ResolveLevelId(context.Document, source);
        var level = (Level)context.Document.GetElement(new ElementId(levelId));
        var widthMm = Json.Double(source, "widthMm", Json.Double(source, "width", 500));
        var depthMm = Json.Double(source, "depthMm", Json.Double(source, "depth", 600));
        var heightMm = Json.Double(source, "heightMm", Json.Double(source, "height", 3600));
        var rotation = Json.Double(source, "rotationDegrees", Json.Double(source, "rotation"));
        var name = Json.StringAny(source, "name", "mark") ?? "ReVitAI Structural Column";
        var familyTypeId = Json.Long(source, "familyTypeId", Json.Long(source, "family_type_id"));

        if (familyTypeId > 0
            && context.Document.GetElement(new ElementId(familyTypeId)) is FamilySymbol symbol
            && IsColumnCategory(symbol.Category))
        {
            if (!symbol.IsActive)
            {
                symbol.Activate();
                context.Document.Regenerate();
            }

            var instance = context.Document.Create.NewFamilyInstance(
                position,
                symbol,
                level,
                StructuralType.Column);
            TrySetMark(instance, name);
            return new
            {
                ElementId = instance.Id.Value,
                NativeFamilyInstance = true,
                FallbackDirectShape = false,
                LevelId = levelId,
                FamilyTypeId = symbol.Id.Value,
                Position = Units.PointDto(position),
            };
        }

        var solid = CreateBoxSolid(position, widthMm, depthMm, heightMm, rotation);
        var shape = DirectShape.CreateElement(
            context.Document,
            new ElementId(BuiltInCategory.OST_StructuralColumns));
        shape.SetShape([solid]);
        shape.SetName(name);
        return new
        {
            ElementId = shape.Id.Value,
            NativeFamilyInstance = false,
            FallbackDirectShape = true,
            LevelId = levelId,
            WidthMm = widthMm,
            DepthMm = depthMm,
            HeightMm = heightMm,
            Position = Units.PointDto(position),
            Warning = "No valid structural column familyTypeId was supplied; created a structural-column DirectShape fallback.",
        };
    }

    private static object CreateStructuralBeam(InvocationContext context)
    {
        var source = context.Arguments;
        var start = Json.Point(source, "start");
        var end = Json.Point(source, "end");
        if (start.DistanceTo(end) < Units.MmToFeet(0.1))
        {
            throw new InvalidOperationException("Beam start and end points are too close.");
        }

        var levelId = ResolveLevelId(context.Document, source);
        var level = (Level)context.Document.GetElement(new ElementId(levelId));
        var beamWidthMm = Json.Double(source, "beamWidthMm", Json.Double(source, "widthMm", Json.Double(source, "width", 250)));
        var beamHeightMm = Json.Double(source, "beamHeightMm", Json.Double(source, "heightMm", Json.Double(source, "height", 600)));
        var name = Json.StringAny(source, "name", "mark") ?? "ReVitAI Structural Beam";
        var familyTypeId = Json.Long(source, "familyTypeId", Json.Long(source, "family_type_id"));

        if (familyTypeId > 0
            && context.Document.GetElement(new ElementId(familyTypeId)) is FamilySymbol symbol
            && IsBeamCategory(symbol.Category))
        {
            if (!symbol.IsActive)
            {
                symbol.Activate();
                context.Document.Regenerate();
            }

            var line = Line.CreateBound(start, end);
            var instance = context.Document.Create.NewFamilyInstance(
                line,
                symbol,
                level,
                StructuralType.Beam);
            TrySetMark(instance, name);
            return new
            {
                ElementId = instance.Id.Value,
                NativeFamilyInstance = true,
                FallbackDirectShape = false,
                LevelId = levelId,
                FamilyTypeId = symbol.Id.Value,
                Start = Units.PointDto(start),
                End = Units.PointDto(end),
            };
        }

        var dx = end.X - start.X;
        var dy = end.Y - start.Y;
        var lengthMm = Units.FeetToMm(Math.Sqrt(dx * dx + dy * dy));
        var rotation = Math.Atan2(dy, dx) * 180.0 / Math.PI;
        var midpoint = new XYZ(
            (start.X + end.X) / 2.0,
            (start.Y + end.Y) / 2.0,
            Math.Min(start.Z, end.Z));
        var solid = CreateBoxSolid(midpoint, lengthMm, beamWidthMm, beamHeightMm, rotation);
        var shape = DirectShape.CreateElement(
            context.Document,
            new ElementId(BuiltInCategory.OST_StructuralFraming));
        shape.SetShape([solid]);
        shape.SetName(name);
        return new
        {
            ElementId = shape.Id.Value,
            NativeFamilyInstance = false,
            FallbackDirectShape = true,
            LevelId = levelId,
            LengthMm = lengthMm,
            BeamWidthMm = beamWidthMm,
            BeamHeightMm = beamHeightMm,
            Start = Units.PointDto(start),
            End = Units.PointDto(end),
            Warning = "No valid structural framing familyTypeId was supplied; created a structural-framing DirectShape fallback.",
        };
    }

    private static object CreateRoom(InvocationContext context)
    {
        var source = context.Arguments;
        var levelId = ResolveLevelId(context.Document, source);
        var level = (Level)context.Document.GetElement(new ElementId(levelId));
        var point = Json.Point(source, "position");
        var room = context.Document.Create.NewRoom(level, new UV(point.X, point.Y));
        var name = Json.StringAny(source, "roomName", "name");
        var number = Json.StringAny(source, "roomNumber", "number");
        TrySetRoomParameter(room, BuiltInParameter.ROOM_NAME, name);
        TrySetRoomParameter(room, BuiltInParameter.ROOM_NUMBER, number);
        return new
        {
            RoomId = room.Id.Value,
            LevelId = levelId,
            Name = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString(),
            Number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER)?.AsString(),
            Position = Units.PointDto(point),
        };
    }

    private static object DeleteElements(InvocationContext context)
    {
        var source = context.Arguments;
        var ids = ElementIds(source, "elementIds")
            .Concat(ElementIds(source, "ids"))
            .Concat(SingleElementId(source, "elementId"))
            .Distinct()
            .ToArray();
        if (ids.Length == 0)
        {
            throw new InvalidOperationException("No element IDs were supplied for deletion.");
        }

        var elementIds = ids.Select(id => new ElementId(id)).ToArray();
        var deleted = context.Document.Delete(elementIds);
        return new
        {
            RequestedCount = elementIds.Length,
            DeletedCount = deleted.Count,
            RequestedIds = ids,
            DeletedIds = deleted.Select(id => id.Value).ToArray(),
        };
    }

    private static object ElementQuery(InvocationContext context)
    {
        var source = context.Arguments;
        var limit = Math.Clamp(Json.Int(source, "limit", 200), 1, 2000);
        var nameFilter = Json.StringAny(source, "nameContains", "name");
        var typeFilter = Json.StringAny(source, "typeContains", "typeName");
        var categoryId = ResolveOptionalCategoryId(context.Document, source);
        var includeTypes = Json.Bool(source, "includeTypes", false);

        var collector = new FilteredElementCollector(context.Document);
        if (!includeTypes)
        {
            collector = collector.WhereElementIsNotElementType();
        }

        var items = collector
            .Where(element =>
            {
                if (categoryId is not null && element.Category?.Id.Value != categoryId.Value)
                {
                    return false;
                }

                var name = SafeElementName(element);
                if (!string.IsNullOrWhiteSpace(nameFilter)
                    && !name.Contains(nameFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                var typeName = SafeTypeName(context.Document, element);
                if (!string.IsNullOrWhiteSpace(typeFilter)
                    && !typeName.Contains(typeFilter, StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }

                return true;
            })
            .OrderBy(element => element.Category?.Name, StringComparer.OrdinalIgnoreCase)
            .ThenBy(element => element.Id.Value)
            .Take(limit)
            .Select(element => ElementSummary(context.Document, element))
            .ToArray();

        return new
        {
            Items = items,
            Count = items.Length,
        };
    }

    private static object GetElementGeometry(InvocationContext context)
    {
        var source = context.Arguments;
        var elementId = Json.Long(source, "elementId", Json.Long(source, "element_id"));
        if (elementId <= 0)
        {
            throw new InvalidOperationException("An elementId is required.");
        }

        var element = context.Document.GetElement(new ElementId(elementId))
            ?? throw new InvalidOperationException($"Element {elementId} was not found.");
        var options = new Options
        {
            DetailLevel = ViewDetailLevel.Fine,
            ComputeReferences = true,
            IncludeNonVisibleObjects = Json.Bool(source, "includeNonVisibleObjects", false),
        };
        var geometry = element.get_Geometry(options);
        var solids = new List<object>();
        var curves = new List<object>();
        if (geometry is not null)
        {
            foreach (var geometryObject in geometry)
            {
                CollectGeometry(geometryObject, solids, curves);
            }
        }

        return new
        {
            ElementId = element.Id.Value,
            Name = SafeElementName(element),
            TypeName = SafeTypeName(context.Document, element),
            Category = element.Category?.Name,
            BoundingBox = BoundingBoxDto(element.get_BoundingBox(null)),
            Location = LocationDto(element.Location),
            SolidCount = solids.Count,
            CurveCount = curves.Count,
            Solids = solids.Take(100).ToArray(),
            Curves = curves.Take(200).ToArray(),
        };
    }

    private static object LoadFamilyTool(InvocationContext context)
    {
        var source = context.Arguments;
        var path = Json.StringAny(source, "path", "filePath")
            ?? throw new InvalidOperationException("A family file path is required.");
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Family file was not found.", path);
        }

        var loaded = context.Document.LoadFamily(path, new AlwaysLoadFamilyOptions(), out var family);
        var typeIds = family.GetFamilySymbolIds().Select(id => id.Value).ToArray();
        return new
        {
            Loaded = loaded,
            FamilyName = family.Name,
            FamilyId = family.Id.Value,
            TypeIds = typeIds,
            TypeCount = typeIds.Length,
        };
    }

    private static object CreateHostedInstance(InvocationContext context, bool isDoor)
    {
        var source = context.Arguments;
        var familyTypeId = Json.Long(source, "familyTypeId", Json.Long(source, "family_type_id"));
        var hostId = Json.Long(source, "hostId", Json.Long(source, "wallId", Json.Long(source, "wall_id")));
        if (familyTypeId <= 0 || hostId <= 0)
        {
            throw new InvalidOperationException("familyTypeId and hostId are required.");
        }

        var symbol = context.Document.GetElement(new ElementId(familyTypeId)) as FamilySymbol
            ?? throw new InvalidOperationException($"Family type {familyTypeId} was not found.");
        var host = context.Document.GetElement(new ElementId(hostId))
            ?? throw new InvalidOperationException($"Host element {hostId} was not found.");
        if (!symbol.IsActive)
        {
            symbol.Activate();
            context.Document.Regenerate();
        }

        var levelId = ResolveLevelId(context.Document, source);
        var level = (Level)context.Document.GetElement(new ElementId(levelId));
        var position = Json.Point(source, "position");
        var instance = context.Document.Create.NewFamilyInstance(
            position,
            symbol,
            host,
            level,
            StructuralType.NonStructural);
        var name = Json.StringAny(source, "name", "mark");
        TrySetMark(instance, name);
        return new
        {
            ElementId = instance.Id.Value,
            Kind = isDoor ? "Door" : "Window",
            HostId = hostId,
            FamilyTypeId = familyTypeId,
            LevelId = levelId,
            Position = Units.PointDto(position),
            NativeFamilyInstance = true,
        };
    }

    private static object GetRoomBoundaries(InvocationContext context)
    {
        var roomId = Json.Long(context.Arguments, "roomId", Json.Long(context.Arguments, "elementId"));
        var room = context.Document.GetElement(new ElementId(roomId)) as Room
            ?? throw new InvalidOperationException($"Room {roomId} was not found.");
        var boundaries = room.GetBoundarySegments(new SpatialElementBoundaryOptions()) ?? [];
        var segments = new List<object>();
        foreach (var loop in boundaries)
        {
            foreach (var segment in loop)
            {
                segments.Add(new
                {
                    Start = Units.PointDto(segment.GetCurve().GetEndPoint(0)),
                    End = Units.PointDto(segment.GetCurve().GetEndPoint(1)),
                    ElementId = segment.ElementId.Value,
                });
            }
        }

        return new
        {
            RoomId = room.Id.Value,
            Name = room.get_Parameter(BuiltInParameter.ROOM_NAME)?.AsString(),
            Number = room.get_Parameter(BuiltInParameter.ROOM_NUMBER)?.AsString(),
            SegmentCount = segments.Count,
            Segments = segments,
        };
    }

    private static void CollectGeometry(GeometryObject geometryObject, List<object> solids, List<object> curves)
    {
        switch (geometryObject)
        {
            case Solid solid when solid.Faces.Size > 0:
                solids.Add(new
                {
                    FaceCount = solid.Faces.Size,
                    EdgeCount = solid.Edges.Size,
                    VolumeM3 = Math.Abs(Units.CubicFeetToCubicMeters(solid.Volume)),
                    BoundingBox = BoundingBoxDto(solid.GetBoundingBox()),
                });
                break;
            case Curve curve:
                curves.Add(new
                {
                    CurveType = curve.GetType().Name,
                    LengthMm = Units.FeetToMm(curve.Length),
                    Start = Units.PointDto(curve.GetEndPoint(0)),
                    End = Units.PointDto(curve.GetEndPoint(1)),
                });
                break;
            case GeometryInstance instance:
                foreach (var child in instance.GetInstanceGeometry())
                {
                    CollectGeometry(child, solids, curves);
                }
                break;
            case GeometryElement element:
                foreach (var child in element)
                {
                    CollectGeometry(child, solids, curves);
                }
                break;
        }
    }

    private static object ElementSummary(Document document, Element element)
    {
        return new
        {
            ElementId = element.Id.Value,
            Name = SafeElementName(element),
            TypeName = SafeTypeName(document, element),
            CategoryId = element.Category?.Id.Value,
            Category = element.Category?.Name,
            LevelId = (element.LevelId ?? ElementId.InvalidElementId).Value,
            BoundingBox = BoundingBoxDto(element.get_BoundingBox(null)),
            Location = LocationDto(element.Location),
        };
    }

    private static object? BoundingBoxDto(BoundingBoxXYZ? box)
    {
        if (box is null)
        {
            return null;
        }

        return new
        {
            Min = Units.PointDto(box.Min),
            Max = Units.PointDto(box.Max),
        };
    }

    private static object? LocationDto(Location? location)
    {
        return location switch
        {
            LocationPoint point => new { Type = "Point", Point = Units.PointDto(point.Point) },
            LocationCurve curve => new
            {
                Type = "Curve",
                Start = Units.PointDto(curve.Curve.GetEndPoint(0)),
                End = Units.PointDto(curve.Curve.GetEndPoint(1)),
            },
            _ => null,
        };
    }

    private static ElementId? ResolveOptionalCategoryId(Document document, JsonElement source)
    {
        var categoryId = Json.Long(source, "categoryId", Json.Long(source, "category_id"));
        if (categoryId > 0)
        {
            return new ElementId(categoryId);
        }

        var builtInCategory = Json.StringAny(source, "builtInCategory", "built_in_category");
        if (!string.IsNullOrWhiteSpace(builtInCategory)
            && Enum.TryParse<BuiltInCategory>(builtInCategory, true, out var builtIn))
        {
            return new ElementId(builtIn);
        }

        var categoryName = Json.StringAny(source, "category", "categoryName");
        if (string.IsNullOrWhiteSpace(categoryName))
        {
            return null;
        }

        if (long.TryParse(categoryName, out var parsed))
        {
            return new ElementId(parsed);
        }

        var category = document.Settings.Categories
            .Cast<Category>()
            .FirstOrDefault(candidate =>
                candidate.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)
                || candidate.Name.Contains(categoryName, StringComparison.OrdinalIgnoreCase));
        return category?.Id;
    }

    private static bool IsColumnCategory(Category? category)
    {
        var builtIn = TryGetBuiltInCategory(category);
        return builtIn == BuiltInCategory.OST_Columns.ToString()
            || builtIn == BuiltInCategory.OST_StructuralColumns.ToString();
    }

    private static bool IsBeamCategory(Category? category)
    {
        var builtIn = TryGetBuiltInCategory(category);
        return builtIn == BuiltInCategory.OST_StructuralFraming.ToString()
            || builtIn == BuiltInCategory.OST_StructuralFramingSystem.ToString();
    }

    private static string? TryGetBuiltInCategory(Category? category)
    {
        try
        {
            return category?.BuiltInCategory.ToString();
        }
        catch
        {
            return null;
        }
    }

    private static string SafeElementName(Element element)
    {
        try
        {
            return element.Name ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string SafeTypeName(Document document, Element element)
    {
        try
        {
            return document.GetElement(element.GetTypeId())?.Name ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    private static void TrySetColumnLevels(
        Document document,
        FamilyInstance instance,
        JsonElement source,
        long baseLevelId)
    {
        var topLevelId = Json.Long(source, "topLevelId", Json.Long(source, "top_level_id"));
        var baseParameter = instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_PARAM);
        if (baseParameter is not null && !baseParameter.IsReadOnly && baseLevelId > 0)
        {
            baseParameter.Set(new ElementId(baseLevelId));
        }

        if (topLevelId > 0)
        {
            var topParameter = instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_PARAM);
            if (topParameter is not null && !topParameter.IsReadOnly)
            {
                topParameter.Set(new ElementId(topLevelId));
            }
        }

        var baseOffset = instance.get_Parameter(BuiltInParameter.FAMILY_BASE_LEVEL_OFFSET_PARAM);
        if (baseOffset is not null && !baseOffset.IsReadOnly)
        {
            baseOffset.Set(Units.MmToFeet(Json.Double(source, "baseOffsetMm", Json.Double(source, "base_offset_mm"))));
        }

        var topOffset = instance.get_Parameter(BuiltInParameter.FAMILY_TOP_LEVEL_OFFSET_PARAM);
        if (topOffset is not null && !topOffset.IsReadOnly)
        {
            topOffset.Set(Units.MmToFeet(Json.Double(source, "topOffsetMm", Json.Double(source, "top_offset_mm"))));
        }
        var lengthParameter = instance.get_Parameter(BuiltInParameter.INSTANCE_LENGTH_PARAM);
        if (lengthParameter is not null && !lengthParameter.IsReadOnly)
        {
            lengthParameter.Set(Units.MmToFeet(Json.Double(source, "heightMm", Json.Double(source, "height"))));
        }
    }
    private static object SetLevelLinePattern(InvocationContext context)
    {
        var source = context.Arguments;
        var patternName = Json.StringAny(
            source,
            "patternName",
            "linePatternName",
            "name");
        var patternId = Json.Long(
            source,
            "patternId",
            Json.Long(source, "linePatternId", 0));
        LinePatternElement? pattern = null;

        if (patternId > 0)
        {
            pattern = context.Document.GetElement(new ElementId(patternId))
                as LinePatternElement;
        }

        if (pattern is null && !string.IsNullOrWhiteSpace(patternName))
        {
            pattern = new FilteredElementCollector(context.Document)
                .OfClass(typeof(LinePatternElement))
                .Cast<LinePatternElement>()
                .FirstOrDefault(candidate => candidate.Name.Equals(
                    patternName,
                    StringComparison.OrdinalIgnoreCase));
        }

        if (pattern is null)
        {
            throw new InvalidOperationException(
                "A valid line patternId or patternName is required.");
        }

        var category = context.Document.Settings.Categories.get_Item(
            BuiltInCategory.OST_Levels);
        var oldPatternId = category.GetLinePatternId(
            GraphicsStyleType.Projection);
        if (!context.DryRun)
        {
            category.SetLinePatternId(
                pattern.Id,
                GraphicsStyleType.Projection);
        }

        return new
        {
            Scope = "AllLevels",
            PatternId = pattern.Id.Value,
            PatternName = pattern.Name,
            PreviousPatternId = oldPatternId.Value,
            Applied = !context.DryRun,
        };
    }
    private static void TrySetMark(Element element, string? mark)
    {
        if (string.IsNullOrWhiteSpace(mark))
        {
            return;
        }

        var parameter = element.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
        if (parameter is not null && !parameter.IsReadOnly)
        {
            parameter.Set(mark);
        }
    }

    private static void TrySetRoomParameter(Room room, BuiltInParameter parameterId, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        var parameter = room.get_Parameter(parameterId);
        if (parameter is not null && !parameter.IsReadOnly)
        {
            parameter.Set(value);
        }
    }

    private sealed class WarningSuppressor : IFailuresPreprocessor
    {
        public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            foreach (var failure in failuresAccessor.GetFailureMessages())
            {
                if (failure.GetSeverity() == FailureSeverity.Warning)
                {
                    failuresAccessor.DeleteWarning(failure);
                }
            }

            return FailureProcessingResult.Continue;
        }
    }
    private sealed class AlwaysLoadFamilyOptions : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }

        public bool OnSharedFamilyFound(
            Family sharedFamily,
            bool familyInUse,
            out FamilySource source,
            out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
}
