using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RevitAi.Abstractions.Models.CADAnalysis;

namespace RevitAi.Abstractions.Services;

public interface ICADAnalyzerService
{
	Task<PipeNetworkAnalysisResult?> AnalyzeCADFileAsync(string cadFilePath, CADAnalysisOptions options, Action<AnalysisProgress>? progressCallback = null);

	List<ManholeData> ExtractManholes(CADFileData cadData, IEnumerable<string> manholeLayerNames);

	List<PipeData> ExtractPipes(CADFileData cadData, IEnumerable<string> pipeLayerNames);

	Task<List<AnnotationLineGroup>> AnalyzeManholeAnnotationsAsync(List<ManholeData> manholes, CADFileData cadData, IEnumerable<string> annotationLayerNames, CADAnalysisOptions options);

	Task<List<AnnotationLineGroup>> AnalyzePipeAnnotationsAsync(List<PipeData> pipes, CADFileData cadData, IEnumerable<string> annotationLayerNames, CADAnalysisOptions options);

	void ConnectPipesToManholes(List<PipeData> pipes, List<ManholeData> manholes, double connectionToleranceMM = 100.0);

	List<AnnotationLineGroup> ConnectAnnotationLines(List<CADLineInfo> lines, double connectionToleranceMM = 10.0);

	void AssociateTextsWithLineGroups(List<AnnotationLineGroup> lineGroups, List<CADTextInfo> texts, double maxDistanceMM = 500.0, double angleToleranceDegrees = 15.0);

	void AssociateLineGroupsToManholes(List<ManholeData> manholes, List<AnnotationLineGroup> lineGroups, double maxDistanceMM = 500.0);

	void AssociateRemainingTextsToManholes(List<ManholeData> manholes, List<CADTextInfo> texts, double maxDistanceMM = 1000.0);

	void AssociateLineGroupsToPipes(List<PipeData> pipes, List<AnnotationLineGroup> lineGroups, double maxDistanceMM = 500.0);

	void AssociateRemainingTextsToPipes(List<PipeData> pipes, List<CADTextInfo> texts, double maxDistanceMM = 1000.0);

	double CalculateDistance(double x1, double y1, double z1, double x2, double y2, double z2);

	double CalculateLineDirection(double startX, double startY, double endX, double endY);

	bool AreAnglesParallel(double angle1, double angle2, double toleranceDegrees = 15.0);
}
