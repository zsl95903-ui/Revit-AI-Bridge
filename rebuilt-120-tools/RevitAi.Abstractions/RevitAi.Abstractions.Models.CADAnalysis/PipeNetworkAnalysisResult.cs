using System;
using System.Collections.Generic;
using System.Linq;

namespace RevitAi.Abstractions.Models.CADAnalysis;

public sealed class PipeNetworkAnalysisResult
{
	public string AnalysisId { get; set; } = Guid.NewGuid().ToString();

	public string CadFilePath { get; set; } = string.Empty;

	public DateTime AnalysisTime { get; set; } = DateTime.Now;

	public List<ManholeData> Manholes { get; set; } = new List<ManholeData>();

	public List<PipeData> Pipes { get; set; } = new List<PipeData>();

	public List<AnnotationLineGroup> ManholeAnnotationLineGroups { get; set; } = new List<AnnotationLineGroup>();

	public List<AnnotationLineGroup> PipeAnnotationLineGroups { get; set; } = new List<AnnotationLineGroup>();

	public AnalysisStatistics Statistics { get; set; } = new AnalysisStatistics();

	public CADAnalysisOptions? Options { get; set; }

	public bool IsSuccess => Statistics.IsSuccess;

	public ManholeData? GetManholeById(string id)
	{
		return Manholes.FirstOrDefault((ManholeData m) => m.Id == id);
	}

	public PipeData? GetPipeById(string id)
	{
		return Pipes.FirstOrDefault((PipeData p) => p.Id == id);
	}

	public List<PipeData> GetPipesConnectedToManhole(string manholeId)
	{
		return Pipes.Where((PipeData p) => p.StartManholeId == manholeId || p.EndManholeId == manholeId).ToList();
	}

	public List<ManholeData> GetUnannotatedManholes()
	{
		return Manholes.Where((ManholeData m) => !m.HasAnnotation).ToList();
	}

	public List<PipeData> GetUnannotatedPipes()
	{
		return Pipes.Where((PipeData p) => !p.HasAnnotation).ToList();
	}

	public List<PipeData> GetUnconnectedPipes()
	{
		return Pipes.Where((PipeData p) => !p.IsConnectedToManholes).ToList();
	}

	public List<ManholeData> GetIsolatedManholes()
	{
		return Manholes.Where((ManholeData m) => m.ConnectedPipeIds.Count == 0).ToList();
	}

	public string GetSummary()
	{
		return $"管网分析完成于 {AnalysisTime:yyyy-MM-dd HH:mm:ss}\n管井: {Manholes.Count} 个\n管道: {Pipes.Count} 个\n" + Statistics.GetSummary();
	}

	public string GetHtmlReport()
	{
		string text = "<html><head><title>管网分析报告</title></head><body>";
		text += "<h1>管网分析报告</h1>";
		text += $"<p><strong>分析时间:</strong> {AnalysisTime:yyyy-MM-dd HH:mm:ss}</p>";
		text = text + "<p><strong>CAD 文件:</strong> " + CadFilePath + "</p>";
		text += "<h2>统计信息</h2>";
		text += "<ul>";
		text += $"<li>管井总数: {Statistics.TotalManholes}</li>";
		text += $"<li>有标注管井: {Statistics.AnnotatedManholes} ({Statistics.ManholeAnnotationCoverage:F1}%)</li>";
		text += $"<li>管道总数: {Statistics.TotalPipes}</li>";
		text += $"<li>有标注管道: {Statistics.AnnotatedPipes} ({Statistics.PipeAnnotationCoverage:F1}%)</li>";
		text += $"<li>连接管道: {Statistics.ConnectedPipes} ({Statistics.PipeConnectionRate:F1}%)</li>";
		text += $"<li>管道总长度: {Statistics.TotalPipeLength / 1000.0:F2} 米</li>";
		text += $"<li>分析耗时: {Statistics.AnalysisDurationMs} ms</li>";
		text += "</ul>";
		if (GetIsolatedManholes().Count > 0)
		{
			text += $"<h3>孤立管井 ({GetIsolatedManholes().Count})</h3>";
			text += "<ul>";
			foreach (ManholeData isolatedManhole in GetIsolatedManholes())
			{
				text += $"<li>{isolatedManhole.BlockName} at ({isolatedManhole.X:F1}, {isolatedManhole.Y:F1})";
				if (!string.IsNullOrEmpty(isolatedManhole.Annotation))
				{
					text = text + " - " + isolatedManhole.Annotation;
				}
				text += "</li>";
			}
			text += "</ul>";
		}
		if (GetUnconnectedPipes().Count > 0)
		{
			text += $"<h3>未连接管道 ({GetUnconnectedPipes().Count})</h3>";
			text += "<ul>";
			foreach (PipeData unconnectedPipe in GetUnconnectedPipes())
			{
				text += $"<li>{unconnectedPipe.LineType} - 长度: {unconnectedPipe.Length:F1}mm";
				if (!string.IsNullOrEmpty(unconnectedPipe.Annotation))
				{
					text = text + " - " + unconnectedPipe.Annotation;
				}
				text += "</li>";
			}
			text += "</ul>";
		}
		return text + "</body></html>";
	}
}
