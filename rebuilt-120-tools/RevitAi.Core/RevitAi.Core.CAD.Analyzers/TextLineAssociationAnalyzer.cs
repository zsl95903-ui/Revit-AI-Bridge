using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using ns7;

namespace RevitAi.Core.CAD.Analyzers;

internal sealed class TextLineAssociationAnalyzer
{
	private readonly double double_0;

	private readonly double double_1;

	private readonly double double_2;

	public TextLineAssociationAnalyzer(double maxDistanceMM = 500.0, double angleToleranceDegrees = 15.0)
	{
		double_0 = maxDistanceMM;
		double_1 = angleToleranceDegrees;
		double_2 = angleToleranceDegrees * Math.PI / 180.0;
	}

	public void method_0(List<AnnotationLineGroup> list_0, List<CADTextInfo> list_1)
	{
		if (list_0 != null && list_0.Count != 0)
		{
			if (list_1 != null && list_1.Count != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[TextLineAssociationAnalyzer] 开始关联 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 条文字到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个线条组");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				HashSet<CADTextInfo> hashSet = new HashSet<CADTextInfo>();
				foreach (AnnotationLineGroup item in list_0)
				{
					foreach (var item2 in method_1(item, list_1))
					{
						if (!hashSet.Contains(item2.Text))
						{
							item.AssociatedTexts.Add(item2.Text);
							hashSet.Add(item2.Text);
						}
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[TextLineAssociationAnalyzer] 关联完成，");
				defaultInterpolatedStringHandler2.AppendFormatted(hashSet.Count);
				defaultInterpolatedStringHandler2.AppendLiteral("/");
				defaultInterpolatedStringHandler2.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 条文字已关联");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				Logger.Warning("[TextLineAssociationAnalyzer] 文字列表为空");
			}
		}
		else
		{
			Logger.Warning("[TextLineAssociationAnalyzer] 线条组列表为空");
		}
	}

	private List<(CADTextInfo Text, double Distance)> method_1(AnnotationLineGroup annotationLineGroup_0, List<CADTextInfo> list_0)
	{
		List<(CADTextInfo, double)> list = new List<(CADTextInfo, double)>();
		List<CADTextInfo> list2 = method_2(annotationLineGroup_0, list_0);
		if (list2.Count == 0)
		{
			Logger.Debug("[TextLineAssociationAnalyzer] 未找到与线条组 " + annotationLineGroup_0.Id + " 平行的文字");
			return list;
		}
		foreach (CADTextInfo item in list2)
		{
			double num = method_4(item, annotationLineGroup_0);
			if (num <= double_0)
			{
				list.Add((item, num));
			}
		}
		list.Sort(((CADTextInfo Text, double Distance) valueTuple_0, (CADTextInfo Text, double Distance) valueTuple_1) => valueTuple_0.Distance.CompareTo(valueTuple_1.Distance));
		return list;
	}

	private List<CADTextInfo> method_2(AnnotationLineGroup annotationLineGroup_0, List<CADTextInfo> list_0)
	{
		List<CADTextInfo> list = new List<CADTextInfo>();
		double primaryDirectionAngle = annotationLineGroup_0.PrimaryDirectionAngle;
		foreach (CADTextInfo item in list_0)
		{
			double valueOrDefault = item.Rotation.GetValueOrDefault();
			if (method_3(primaryDirectionAngle, valueOrDefault))
			{
				list.Add(item);
			}
		}
		return list;
	}

	private bool method_3(double double_3, double double_4)
	{
		double num;
		for (num = Math.Abs(double_3 - double_4); num > Math.PI; num -= Math.PI)
		{
		}
		if (!(num <= double_2))
		{
			return num >= Math.PI - double_2;
		}
		return true;
	}

	private double method_4(CADTextInfo cadtextInfo_0, AnnotationLineGroup annotationLineGroup_0)
	{
		(double, double, double) valueTuple_ = (cadtextInfo_0.X, cadtextInfo_0.Y, cadtextInfo_0.Z);
		double num = double.MaxValue;
		foreach (CADLineInfo line in annotationLineGroup_0.Lines)
		{
			double val = method_5(valueTuple_, line);
			num = Math.Min(num, val);
		}
		return num;
	}

	private double method_5((double X, double Y, double Z) valueTuple_0, CADLineInfo cadlineInfo_0)
	{
		(double X, double Y, double Z) tuple = valueTuple_0;
		double item = tuple.X;
		double item2 = tuple.Y;
		double item3 = tuple.Z;
		double startX = cadlineInfo_0.StartX;
		double startY = cadlineInfo_0.StartY;
		double startZ = cadlineInfo_0.StartZ;
		double num = startY;
		double num2 = startX;
		double endX = cadlineInfo_0.EndX;
		double endY = cadlineInfo_0.EndY;
		double endZ = cadlineInfo_0.EndZ;
		double num3 = endX - num2;
		double num4 = endY - num;
		double num5 = endZ - startZ;
		double num6 = num3 * num3 + num4 * num4 + num5 * num5;
		if (num6 == 0.0)
		{
			double num7 = item - num2;
			double num8 = item2 - num;
			double num9 = item3 - startZ;
			return Math.Sqrt(num7 * num7 + num8 * num8 + num9 * num9);
		}
		double val = ((item - num2) * num3 + (item2 - num) * num4 + (item3 - startZ) * num5) / num6;
		val = Math.Max(0.0, Math.Min(1.0, val));
		double num10 = num2 + val * num3;
		double num11 = num + val * num4;
		double num12 = startZ + val * num5;
		double num13 = item - num10;
		double num14 = item2 - num11;
		double num15 = item3 - num12;
		return Math.Sqrt(num13 * num13 + num14 * num14 + num15 * num15);
	}

	public Dictionary<string, CADTextInfo> method_6(List<(string Id, double X, double Y, double Z)> list_0, List<CADTextInfo> list_1, double double_3)
	{
		Dictionary<string, CADTextInfo> dictionary = new Dictionary<string, CADTextInfo>();
		if (list_0 != null && list_0.Count != 0)
		{
			if (list_1 != null && list_1.Count != 0)
			{
				HashSet<CADTextInfo> hashSet = new HashSet<CADTextInfo>();
				foreach (var item in list_0)
				{
					CADTextInfo val = null;
					double num = double_3;
					foreach (CADTextInfo item2 in list_1)
					{
						if (!hashSet.Contains(item2))
						{
							double num2 = Math.Sqrt(Math.Pow(item.X - item2.X, 2.0) + Math.Pow(item.Y - item2.Y, 2.0) + Math.Pow(item.Z - item2.Z, 2.0));
							if (num2 < num)
							{
								num = num2;
								val = item2;
							}
						}
					}
					if (val != null)
					{
						dictionary[item.Id] = val;
						hashSet.Add(val);
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[TextLineAssociationAnalyzer] 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(dictionary.Count);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个元素找到了最近的文字");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				return dictionary;
			}
			Logger.Warning("[TextLineAssociationAnalyzer] 文字列表为空");
			return dictionary;
		}
		Logger.Warning("[TextLineAssociationAnalyzer] 元素列表为空");
		return dictionary;
	}
}
