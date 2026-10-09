using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using ns3;
using ns7;

namespace RevitAi.Core.CAD.Analyzers;

internal sealed class ManholeAnnotationAnalyzer
{
	[CompilerGenerated]
	public sealed class Class90
	{
		public HashSet<string> hashSet_0;

		internal bool method_0(CADTextInfo cadtextInfo_0)
		{
			return hashSet_0.Contains(cadtextInfo_0.Content);
		}
	}

	[CompilerGenerated]
	public sealed class Class91
	{
		public KeyValuePair<string, CADTextInfo> keyValuePair_0;

		internal bool method_0(ManholeData manholeData_0)
		{
			return manholeData_0.Id == keyValuePair_0.Key;
		}
	}

	private readonly double double_0;

	private readonly double double_1;

	private readonly double double_2;

	private readonly double double_3;

	private readonly double double_4;

	public ManholeAnnotationAnalyzer(double lineEndToManholeMaxDistanceMM = 500.0, double textToManholeMaxDistanceMM = 1000.0, double lineConnectionToleranceMM = 10.0, double textToLineMaxDistanceMM = 500.0, double directionAngleToleranceDegrees = 15.0)
	{
		double_0 = lineEndToManholeMaxDistanceMM;
		double_1 = textToManholeMaxDistanceMM;
		double_2 = lineConnectionToleranceMM;
		double_3 = textToLineMaxDistanceMM;
		double_4 = directionAngleToleranceDegrees;
	}

	public List<AnnotationLineGroup> method_0(List<ManholeData> list_0, CADFileData cadfileData_0, IEnumerable<string> ienumerable_0, CADAnalysisOptions cadanalysisOptions_0)
	{
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[ManholeAnnotationAnalyzer] 开始分析管井标注，管井数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		List<CADLineInfo> list = new List<CADLineInfo>();
		List<CADTextInfo> list2 = new List<CADTextInfo>();
		foreach (string item in ienumerable_0)
		{
			List<CADLineInfo> linesByLayer = cadfileData_0.GetLinesByLayer(item);
			List<CADTextInfo> textsByLayer = cadfileData_0.GetTextsByLayer(item);
			list.AddRange(linesByLayer);
			list2.AddRange(textsByLayer);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[ManholeAnnotationAnalyzer] 图层 '");
			defaultInterpolatedStringHandler2.AppendFormatted(item);
			defaultInterpolatedStringHandler2.AppendLiteral("': 线条=");
			defaultInterpolatedStringHandler2.AppendFormatted(linesByLayer.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(", 文字=");
			defaultInterpolatedStringHandler2.AppendFormatted(textsByLayer.Count);
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(40, 2);
		defaultInterpolatedStringHandler3.AppendLiteral("[ManholeAnnotationAnalyzer] 总计: 线条=");
		defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler3.AppendLiteral(", 文字=");
		defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
		Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		List<AnnotationLineGroup> list3 = new Class88(double_2).method_0(list);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(38, 1);
		defaultInterpolatedStringHandler4.AppendLiteral("[ManholeAnnotationAnalyzer] 生成 ");
		defaultInterpolatedStringHandler4.AppendFormatted(list3.Count);
		defaultInterpolatedStringHandler4.AppendLiteral(" 个标注线条组");
		Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
		new TextLineAssociationAnalyzer(double_3, double_4).method_0(list3, list2);
		int value = list3.Count((AnnotationLineGroup annotationLineGroup_0) => annotationLineGroup_0.HasAssociatedTexts);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(39, 2);
		defaultInterpolatedStringHandler5.AppendLiteral("[ManholeAnnotationAnalyzer] ");
		defaultInterpolatedStringHandler5.AppendFormatted(value);
		defaultInterpolatedStringHandler5.AppendLiteral("/");
		defaultInterpolatedStringHandler5.AppendFormatted(list3.Count);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个线条组关联了文字");
		Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
		method_1(list_0, list3, cadanalysisOptions_0.LineEndToManholeMaxDistanceMM);
		int value2 = list_0.Count((ManholeData manholeData_0) => manholeData_0.HasAnnotation && manholeData_0.AnnotationType == "LineGroup");
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(40, 2);
		defaultInterpolatedStringHandler6.AppendLiteral("[ManholeAnnotationAnalyzer] ");
		defaultInterpolatedStringHandler6.AppendFormatted(value2);
		defaultInterpolatedStringHandler6.AppendLiteral("/");
		defaultInterpolatedStringHandler6.AppendFormatted(list_0.Count);
		defaultInterpolatedStringHandler6.AppendLiteral(" 个管井通过线条组标注");
		Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
		List<ManholeData> list4 = list_0.Where((ManholeData manholeData_0) => !manholeData_0.HasAnnotation).ToList();
		if (list4.Count > 0)
		{
			method_2(list4, list2, cadanalysisOptions_0.TextToManholeMaxDistanceMM);
			int value3 = list4.Count((ManholeData manholeData_0) => manholeData_0.HasAnnotation);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(39, 2);
			defaultInterpolatedStringHandler7.AppendLiteral("[ManholeAnnotationAnalyzer] ");
			defaultInterpolatedStringHandler7.AppendFormatted(value3);
			defaultInterpolatedStringHandler7.AppendLiteral("/");
			defaultInterpolatedStringHandler7.AppendFormatted(list4.Count);
			defaultInterpolatedStringHandler7.AppendLiteral(" 个管井通过距离标注");
			Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
			HashSet<string> hashSet_0 = new HashSet<string>(from manholeData_0 in list4
				where manholeData_0.HasAnnotation && manholeData_0.AnnotationType == "Distance"
				select manholeData_0.Annotation ?? string.Empty);
			list2.RemoveAll((CADTextInfo cadtextInfo_0) => hashSet_0.Contains(cadtextInfo_0.Content));
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(43, 2);
		defaultInterpolatedStringHandler8.AppendLiteral("[ManholeAnnotationAnalyzer] 管井标注分析完成: ");
		defaultInterpolatedStringHandler8.AppendFormatted(list_0.Count((ManholeData manholeData_0) => manholeData_0.HasAnnotation));
		defaultInterpolatedStringHandler8.AppendLiteral("/");
		defaultInterpolatedStringHandler8.AppendFormatted(list_0.Count);
		defaultInterpolatedStringHandler8.AppendLiteral(" 已标注");
		Logger.Info(defaultInterpolatedStringHandler8.ToStringAndClear());
		return list3;
	}

	private void method_1(List<ManholeData> list_0, List<AnnotationLineGroup> list_1, double double_5)
	{
		if (list_0 != null && list_0.Count != 0)
		{
			if (list_1 != null && list_1.Count != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[ManholeAnnotationAnalyzer] 开始将 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个线条组匹配到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个管井");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				HashSet<ManholeData> hashSet = new HashSet<ManholeData>();
				HashSet<AnnotationLineGroup> hashSet2 = new HashSet<AnnotationLineGroup>();
				foreach (AnnotationLineGroup item in list_1)
				{
					if (!item.HasAssociatedTexts)
					{
						continue;
					}
					(double, double, double) firstEndpoint = item.GetFirstEndpoint();
					(double, double, double) lastEndpoint = item.GetLastEndpoint();
					ManholeData val = null;
					double num = double_5;
					foreach (ManholeData item2 in list_0)
					{
						double val2 = method_3(firstEndpoint.Item1, firstEndpoint.Item2, firstEndpoint.Item3, item2.X, item2.Y, item2.Z);
						double val3 = method_3(lastEndpoint.Item1, lastEndpoint.Item2, lastEndpoint.Item3, item2.X, item2.Y, item2.Z);
						double num2 = Math.Min(val2, val3);
						if (num2 < num)
						{
							num = num2;
							val = item2;
						}
					}
					if (val != null && !val.HasAnnotation)
					{
						val.Annotation = item.GetMergedTextContent();
						val.AnnotationType = "LineGroup";
						hashSet.Add(val);
						hashSet2.Add(item);
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[ManholeAnnotationAnalyzer] 线条组匹配完成: ");
				defaultInterpolatedStringHandler2.AppendFormatted(hashSet.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个管井已标注");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				Logger.Warning("[ManholeAnnotationAnalyzer] 线条组列表为空，无法匹配标注");
			}
		}
		else
		{
			Logger.Warning("[ManholeAnnotationAnalyzer] 管井列表为空，无法匹配标注");
		}
	}

	private void method_2(List<ManholeData> list_0, List<CADTextInfo> list_1, double double_5)
	{
		if (list_0 != null && list_0.Count != 0)
		{
			if (list_1 != null && list_1.Count != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[ManholeAnnotationAnalyzer] 开始将文字匹配到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个未标注管井");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				TextLineAssociationAnalyzer textLineAssociationAnalyzer = new TextLineAssociationAnalyzer(double_5, double_4);
				List<(string, double, double, double)> list_2 = list_0.Select((ManholeData manholeData_0) => (Id: manholeData_0.Id, X: manholeData_0.X, Y: manholeData_0.Y, Z: manholeData_0.Z)).ToList();
				Dictionary<string, CADTextInfo> dictionary = textLineAssociationAnalyzer.method_6(list_2, list_1, double_5);
				foreach (KeyValuePair<string, CADTextInfo> keyValuePair_0 in dictionary)
				{
					ManholeData val = list_0.First((ManholeData manholeData_0) => manholeData_0.Id == keyValuePair_0.Key);
					CADTextInfo value = keyValuePair_0.Value;
					if (!val.HasAnnotation)
					{
						val.Annotation = value.Content;
						val.AnnotationType = "Distance";
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[ManholeAnnotationAnalyzer] 文字匹配完成: ");
				defaultInterpolatedStringHandler2.AppendFormatted(dictionary.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个管井已标注");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				Logger.Warning("[ManholeAnnotationAnalyzer] 文字列表为空，无法匹配");
			}
		}
		else
		{
			Logger.Warning("[ManholeAnnotationAnalyzer] 管井列表为空，无法匹配文字");
		}
	}

	private double method_3(double double_5, double double_6, double double_7, double double_8, double double_9, double double_10)
	{
		double num = double_8 - double_5;
		double num2 = double_9 - double_6;
		double num3 = double_10 - double_7;
		return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}
}
