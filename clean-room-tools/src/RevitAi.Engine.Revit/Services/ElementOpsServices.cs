using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using RevitAi.Engine.Abstractions.Services;

namespace RevitAi.Engine.Revit.Services;

/// <summary>
/// 元素操作服务：复制/旋转/镜像/阵列/拆分/删除/连接/剪切。
/// API 事实（反射核对 27.3）：
///   ElementTransformUtils.CopyElements/MoveElements/RotateElements/MirrorElements；
///   JoinGeometryUtils.JoinGeometry/UnjoinGeometry/SwitchJoinOrder；
///   SolidSolidCutUtils.AddCutBetweenSolids + InstanceVoidCutUtils.AddInstanceVoidCut；
///   PlumbingUtils.BreakCurve（管道）、MechanicalUtils.BreakCurve（风管）。
/// 约定：
///   * mirrorType=line 用平面（法向垂直于连线）镜像；mirrorType=point 等价于绕该点的竖直轴旋转 180°；
///   * array_type=linear 按 move 向量平移复制；array_type=radial 绕 origin 的竖直轴均分 360°。
/// </summary>
internal sealed class ElementOpsService : IElementOpsService
{
    public TransformResult CopyElements(object document, IReadOnlyList<int> elementIds, double dxMm, double dyMm, double dzMm)
    {
        var doc = ModelingService.RequireDocument(document);
        if (elementIds.Count == 0)
        {
            return new TransformResult(0, 0, Array.Empty<int>(), new[] { "没有要复制的元素" });
        }

        try
        {
            var translation = new XYZ(ModelingService.Mm(dxMm), ModelingService.Mm(dyMm), ModelingService.Mm(dzMm));
            var ids = elementIds.Select(id => new ElementId(id)).ToList();
            var created = ElementTransformUtils.CopyElements(doc, ids, translation);
            var createdIds = created.Select(id => (int)id.Value).ToList();
            return new TransformResult(elementIds.Count, createdIds.Count, createdIds, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return new TransformResult(elementIds.Count, 0, Array.Empty<int>(), new[] { ex.GetBaseException().Message });
        }
    }

    public TransformResult RotateElements(
        object document,
        IReadOnlyList<int> elementIds,
        double axisX,
        double axisY,
        double axisZ,
        double angleDegrees,
        double originX,
        double originY,
        double originZ)
    {
        var doc = ModelingService.RequireDocument(document);
        var ids = elementIds.Select(id => new ElementId(id)).ToList();
        if (ids.Count == 0)
        {
            return new TransformResult(0, 0, Array.Empty<int>(), new[] { "没有要旋转的元素" });
        }

        var axis = new XYZ(axisX, axisY, axisZ);
        if (axis.GetLength() < 1e-9)
        {
            return new TransformResult(ids.Count, 0, Array.Empty<int>(), new[] { "旋转轴向量不能为零向量" });
        }

        try
        {
            var origin = new XYZ(ModelingService.Mm(originX), ModelingService.Mm(originY), ModelingService.Mm(originZ));
            var line = Line.CreateUnbound(origin, axis.Normalize());
            var radians = angleDegrees * Math.PI / 180.0;
            ElementTransformUtils.RotateElements(doc, ids, line, radians);
            return new TransformResult(ids.Count, ids.Count, ids.Select(id => (int)id.Value).ToList(), Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return new TransformResult(ids.Count, 0, Array.Empty<int>(), new[] { ex.GetBaseException().Message });
        }
    }

    public TransformResult MirrorElements(
        object document,
        IReadOnlyList<int> elementIds,
        string mirrorType,
        double lineStartX,
        double lineStartY,
        double lineEndX,
        double lineEndY,
        double pointX,
        double pointY)
    {
        var doc = ModelingService.RequireDocument(document);
        var ids = elementIds.Select(id => new ElementId(id)).ToList();
        if (ids.Count == 0)
        {
            return new TransformResult(0, 0, Array.Empty<int>(), new[] { "没有要镜像的元素" });
        }

        try
        {
            var kind = (mirrorType ?? "line").Trim().ToLowerInvariant();
            List<int> results;

            if (kind is "point" or "pointmirror" or "点")
            {
                // 点镜像：等价于绕该点竖直轴旋转 180°。
                var origin = new XYZ(ModelingService.Mm(pointX), ModelingService.Mm(pointY), 0);
                var line = Line.CreateUnbound(origin, XYZ.BasisZ);
                ElementTransformUtils.RotateElements(doc, ids, line, Math.PI);
                results = ids.Select(id => (int)id.Value).ToList();
            }
            else
            {
                var start = new XYZ(ModelingService.Mm(lineStartX), ModelingService.Mm(lineStartY), 0);
                var end = new XYZ(ModelingService.Mm(lineEndX), ModelingService.Mm(lineEndY), 0);
                var direction = end - start;
                if (direction.GetLength() < 1e-9)
                {
                    return new TransformResult(ids.Count, 0, Array.Empty<int>(), new[] { "镜像线的起终点不能重合" });
                }

                var normal = new XYZ(-direction.Y, direction.X, 0).Normalize();
                var midpoint = (start + end) / 2;
                var plane = Plane.CreateByNormalAndOrigin(normal, midpoint);
                var created = ElementTransformUtils.MirrorElements(doc, ids, plane, true);
                results = created.Select(id => (int)id.Value).ToList();
            }

            return new TransformResult(ids.Count, results.Count, results, Array.Empty<string>());
        }
        catch (Exception ex)
        {
            return new TransformResult(ids.Count, 0, Array.Empty<int>(), new[] { ex.GetBaseException().Message });
        }
    }

    public TransformResult ArrayElements(
        object document,
        int elementId,
        string arrayType,
        int count,
        double moveX,
        double moveY,
        double moveZ,
        double originX,
        double originY,
        double originZ)
    {
        var doc = ModelingService.RequireDocument(document);
        if (count < 2)
        {
            return new TransformResult(count, 0, Array.Empty<int>(), new[] { "count 至少为 2（阵列至少生成 2 份）。" });
        }

        var source = doc.GetElement(new ElementId(elementId));
        if (source is null)
        {
            return new TransformResult(count, 0, Array.Empty<int>(), new[] { $"未找到元素 {elementId}。" });
        }

        var kind = (arrayType ?? "linear").Trim().ToLowerInvariant();
        var isRadial = kind is "radial" or "circular" or "环形";
        var createdIds = new List<int>();
        var failures = new List<string>();

        try
        {
            var rotationCenter = isRadial
                ? new XYZ(ModelingService.Mm(originX), ModelingService.Mm(originY), ModelingService.Mm(originZ))
                : XYZ.Zero;

            for (var index = 1; index < count; index++)
            {
                var translation = isRadial
                    ? XYZ.Zero
                    : new XYZ(ModelingService.Mm(moveX), ModelingService.Mm(moveY), ModelingService.Mm(moveZ));

                var copies = ElementTransformUtils.CopyElements(
                    doc,
                    new List<ElementId> { new ElementId(elementId) },
                    translation);

                foreach (var copy in copies)
                {
                    if (isRadial)
                    {
                        // 环形阵列：绕 origin 竖直轴均分 360°。
                        ElementTransformUtils.RotateElement(
                            doc,
                            copy,
                            Line.CreateUnbound(rotationCenter, XYZ.BasisZ),
                            2 * Math.PI * index / count);
                    }

                    createdIds.Add((int)copy.Value);
                }
            }

            return new TransformResult(count, createdIds.Count, createdIds, failures);
        }
        catch (Exception ex)
        {
            return new TransformResult(count, createdIds.Count, createdIds, new[] { ex.GetBaseException().Message });
        }
    }

    private static XYZ RadialStep(int index, int count, double originX, double originY, double originZ, out double radians)
    {
        // 环形阵列：绕 origin 竖直轴均分 360°，先按"序号×步进角"旋转，因此平移为零。
        radians = 2 * Math.PI * index / count;
        return XYZ.Zero;
    }

    public int SplitElement(
        object document,
        int elementId,
        string splitMode,
        double? lengthMm,
        IReadOnlyList<Point2> points,
        IReadOnlyList<double> parameters)
    {
        var doc = ModelingService.RequireDocument(document);
        var element = doc.GetElement(new ElementId(elementId))
                      ?? throw new InvalidOperationException($"未找到元素 {elementId}。");

        var curve = (element as MEPCurve)?.Location is LocationCurve location
            ? location.Curve
            : throw new InvalidOperationException(
                $"元素 {elementId}（{element?.GetType().Name}）不是曲线类 MEP 构件；"
                + "本实现支持拆分的构件为管道（PlumbingUtils.BreakCurve）与风管（MechanicalUtils.BreakCurve）。");

        var splitPoints = ResolveSplitPoints(curve, splitMode, lengthMm, points, parameters);
        var splitCount = 0;

        foreach (var splitPoint in splitPoints)
        {
            MEPCurve? target = doc.GetElement(new ElementId(elementId)) as MEPCurve;
            if (target is null)
            {
                break;
            }

            _ = target switch
            {
                Pipe => PlumbingUtils.BreakCurve(doc, new ElementId(elementId), splitPoint),
                Duct => MechanicalUtils.BreakCurve(doc, new ElementId(elementId), splitPoint),
                _ => throw new InvalidOperationException(
                    $"元素 {elementId} 的类别不支持 BreakCurve（仅支持管道与风管）。"),
            };

            splitCount++;
        }

        return splitCount;
    }

    private static List<XYZ> ResolveSplitPoints(
        Curve curve,
        string splitMode,
        double? lengthMm,
        IReadOnlyList<Point2> points,
        IReadOnlyList<double> parameters)
    {
        var mode = (splitMode ?? "point").Trim().ToLowerInvariant();
        var result = new List<XYZ>();

        switch (mode)
        {
            case "length":
            {
                if (lengthMm is null or <= 0)
                {
                    throw new InvalidOperationException("splitMode=length 需要提供 length（毫米）。");
                }

                var target = ModelingService.Mm(lengthMm!.Value);
                if (target >= curve.Length)
                {
                    throw new InvalidOperationException($"length {lengthMm} mm 超过构件长度 {Math.Round(curve.Length * 304.8, 1)} mm。");
                }

                result.Add(curve.Evaluate(target / curve.Length, true));
                break;
            }

            case "parameter":
            {
                if (parameters.Count == 0)
                {
                    throw new InvalidOperationException("splitMode=parameter 需要提供 parameters（0–1 的归一化参数）。");
                }

                foreach (var parameter in parameters)
                {
                    var normalized = parameter > 1 ? parameter / 100.0 : parameter;
                    if (normalized is <= 0 or >= 1)
                    {
                        throw new InvalidOperationException($"参数 {parameter} 超出 (0,1) 范围。");
                    }

                    result.Add(curve.Evaluate(normalized, true));
                }

                break;
            }

            default:
            {
                if (points.Count > 0)
                {
                    result.AddRange(points.Select(point => new XYZ(ModelingService.Mm(point.X), ModelingService.Mm(point.Y), 0)));
                }
                else
                {
                    // 未给点：按曲线中点拆分。
                    result.Add(curve.Evaluate(0.5, true));
                }

                break;
            }
        }

        if (result.Count == 0)
        {
            throw new InvalidOperationException("没有解析出任何拆分点。");
        }

        return result;
    }

    public int DeleteElements(object document, IReadOnlyList<int> elementIds)
    {
        var doc = ModelingService.RequireDocument(document);
        if (elementIds.Count == 0)
        {
            return 0;
        }

        var ids = elementIds.Select(id => new ElementId(id)).Where(id => doc.GetElement(id) is not null).ToList();
        if (ids.Count == 0)
        {
            return 0;
        }

        var deleted = doc.Delete(ids);
        return deleted.Count;
    }

    public int JoinGeometry(object document, IReadOnlyList<int> firstIds, IReadOnlyList<int> secondIds, string operation)
    {
        var doc = ModelingService.RequireDocument(document);
        var pairs = PairUp(firstIds, secondIds);
        if (pairs.Count == 0)
        {
            throw new InvalidOperationException("需要提供两组元素（element1Ids 与 element2Ids 至少各一个）。");
        }

        var normalized = (operation ?? "join").Trim().ToLowerInvariant();
        var affected = 0;

        foreach (var (firstId, secondId) in pairs)
        {
            var first = doc.GetElement(new ElementId(firstId));
            var second = doc.GetElement(new ElementId(secondId));
            if (first is null || second is null)
            {
                continue;
            }

            try
            {
                switch (normalized)
                {
                    case "unjoin":
                    case "remove":
                        JoinGeometryUtils.UnjoinGeometry(doc, first, second);
                        break;
                    case "switch":
                    case "switch_order":
                        JoinGeometryUtils.SwitchJoinOrder(doc, first, second);
                        break;
                    default:
                        JoinGeometryUtils.JoinGeometry(doc, first, second);
                        break;
                }

                affected++;
            }
            catch (Exception)
            {
                // 不可连接的组合跳过。
            }
        }

        return affected;
    }

    public int CutGeometry(object document, IReadOnlyList<int> toCutIds, IReadOnlyList<int> cuttingIds, string operation)
    {
        var doc = ModelingService.RequireDocument(document);
        var pairs = PairUp(toCutIds, cuttingIds);
        if (pairs.Count == 0)
        {
            throw new InvalidOperationException("需要提供被剪切元素与剪切元素（两组至少各一个）。");
        }

        var normalized = (operation ?? "cut").Trim().ToLowerInvariant();
        var affected = 0;

        foreach (var (targetId, cuttingId) in pairs)
        {
            var target = doc.GetElement(new ElementId(targetId));
            var cutting = doc.GetElement(new ElementId(cuttingId));
            if (target is null || cutting is null)
            {
                continue;
            }

            try
            {
                if (normalized is "uncut" or "remove")
                {
                    SolidSolidCutUtils.RemoveCutBetweenSolids(doc, target, cutting);
                }
                else if (cutting is FamilyInstance && InstanceVoidCutUtils.CanBeCutWithVoid(target))
                {
                    // 空心族实例优先用"实例空心剪切"。
                    InstanceVoidCutUtils.AddInstanceVoidCut(doc, target, cutting);
                }
                else
                {
                    SolidSolidCutUtils.AddCutBetweenSolids(doc, target, cutting);
                }

                affected++;
            }
            catch (Exception)
            {
                // 不满足剪切条件的组合跳过。
            }
        }

        return affected;
    }

    private static List<(int First, int Second)> PairUp(IReadOnlyList<int> firstIds, IReadOnlyList<int> secondIds)
    {
        var pairs = new List<(int, int)>();
        if (firstIds.Count == 0 || secondIds.Count == 0)
        {
            return pairs;
        }

        var count = Math.Max(firstIds.Count, secondIds.Count);
        for (var index = 0; index < count; index++)
        {
            pairs.Add((firstIds[Math.Min(index, firstIds.Count - 1)], secondIds[Math.Min(index, secondIds.Count - 1)]));
        }

        return pairs;
    }
}
