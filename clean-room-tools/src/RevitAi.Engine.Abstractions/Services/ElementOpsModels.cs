namespace RevitAi.Engine.Abstractions.Services;

// ---------------------------------------------------------------------------------------------
// 批次 4 第三刀：元素操作（复制/旋转/镜像/阵列/拆分/删除/连接/剪切）。
// ---------------------------------------------------------------------------------------------

public sealed record TransformResult(
    int Requested,
    int Succeeded,
    IReadOnlyList<int> ResultIds,
    IReadOnlyList<string> Failures);

public interface IElementOpsService
{
    TransformResult CopyElements(object document, IReadOnlyList<int> elementIds, double dxMm, double dyMm, double dzMm);

    TransformResult RotateElements(
        object document,
        IReadOnlyList<int> elementIds,
        double axisX,
        double axisY,
        double axisZ,
        double angleDegrees,
        double originX,
        double originY,
        double originZ);

    TransformResult MirrorElements(
        object document,
        IReadOnlyList<int> elementIds,
        string mirrorType,
        double lineStartX,
        double lineStartY,
        double lineEndX,
        double lineEndY,
        double pointX,
        double pointY);

    TransformResult ArrayElements(
        object document,
        int elementId,
        string arrayType,
        int count,
        double moveX,
        double moveY,
        double moveZ,
        double originX,
        double originY,
        double originZ);

    /// <summary>拆分曲线类构件（管道/风管）；返回拆分成的新段数量。</summary>
    int SplitElement(
        object document,
        int elementId,
        string splitMode,
        double? lengthMm,
        IReadOnlyList<Point2> points,
        IReadOnlyList<double> parameters);

    int DeleteElements(object document, IReadOnlyList<int> elementIds);

    int JoinGeometry(object document, IReadOnlyList<int> firstIds, IReadOnlyList<int> secondIds, string operation);

    int CutGeometry(object document, IReadOnlyList<int> toCutIds, IReadOnlyList<int> cuttingIds, string operation);
}
