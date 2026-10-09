using System;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class AnalysisStatistics
{
	public int TotalManholes { get; set; }

	public int AnnotatedManholes { get; set; }

	public int UnannotatedManholes => TotalManholes - AnnotatedManholes;

	public double ManholeAnnotationCoverage
	{
		get
		{
			if (TotalManholes == 0)
			{
				return 0.0;
			}
			return (double)AnnotatedManholes / (double)TotalManholes * 100.0;
		}
	}

	public int TotalPipes { get; set; }

	public int AnnotatedPipes { get; set; }

	public int UnannotatedPipes => TotalPipes - AnnotatedPipes;

	public double PipeAnnotationCoverage
	{
		get
		{
			if (TotalPipes == 0)
			{
				return 0.0;
			}
			return (double)AnnotatedPipes / (double)TotalPipes * 100.0;
		}
	}

	public int ConnectedPipes { get; set; }

	public int UnconnectedPipes => TotalPipes - ConnectedPipes;

	public double PipeConnectionRate
	{
		get
		{
			if (TotalPipes == 0)
			{
				return 0.0;
			}
			return (double)ConnectedPipes / (double)TotalPipes * 100.0;
		}
	}

	public double TotalPipeLength { get; set; }

	public double AveragePipeLength
	{
		get
		{
			if (TotalPipes == 0)
			{
				return 0.0;
			}
			return TotalPipeLength / (double)TotalPipes;
		}
	}

	public long AnalysisDurationMs { get; set; }

	public DateTime AnalysisStartTime { get; set; }

	public DateTime AnalysisEndTime { get; set; }

	public bool IsSuccess { get; set; }

	public string? ErrorMessage { get; set; }

	public string GetSummary()
	{
		if (!IsSuccess)
		{
			return "分析失败: " + ErrorMessage;
		}
		return $"管井: {TotalManholes} (标注: {AnnotatedManholes}, 覆盖率: {ManholeAnnotationCoverage:F1}%)\n管道: {TotalPipes} (标注: {AnnotatedPipes}, 覆盖率: {PipeAnnotationCoverage:F1}%)\n连接: {ConnectedPipes}/{TotalPipes} ({PipeConnectionRate:F1}%)\n总长度: {TotalPipeLength / 1000.0:F2} 米\n耗时: {AnalysisDurationMs} ms";
	}
}
