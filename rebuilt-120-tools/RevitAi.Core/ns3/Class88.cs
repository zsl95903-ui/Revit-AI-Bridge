using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using ns7;

namespace ns3;

internal sealed class Class88
{
	[CompilerGenerated]
	public sealed class Class89
	{
		public List<CADLineInfo> list_0;

		internal CADLineInfo method_0(int int_0)
		{
			return list_0[int_0];
		}
	}

	private readonly double double_0;

	public Class88(double double_1 = 10.0)
	{
		double_0 = double_1;
	}

	public List<AnnotationLineGroup> method_0(List<CADLineInfo> list_0)
	{
		if (list_0 != null && list_0.Count != 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[AnnotationLineGroupAnalyzer] 开始连接 ");
			defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 条标注线条");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			List<AnnotationLineGroup> list = new List<AnnotationLineGroup>();
			HashSet<int> hashSet = new HashSet<int>();
			Dictionary<int, List<int>> dictionary_ = method_1(list_0);
			for (int i = 0; i < list_0.Count; i++)
			{
				if (!hashSet.Contains(i))
				{
					AnnotationLineGroup val = method_4(list_0, i, dictionary_, hashSet);
					if (val.Lines.Count > 0)
					{
						list.Add(val);
					}
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[AnnotationLineGroupAnalyzer] 连接完成，生成 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个线条组");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		Logger.Warning("[AnnotationLineGroupAnalyzer] 线条列表为空");
		return new List<AnnotationLineGroup>();
	}

	private Dictionary<int, List<int>> method_1(List<CADLineInfo> list_0)
	{
		Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
		for (int i = 0; i < list_0.Count; i++)
		{
			dictionary[i] = new List<int>();
			for (int j = 0; j < list_0.Count; j++)
			{
				if (i != j && method_2(list_0[i], list_0[j]))
				{
					dictionary[i].Add(j);
				}
			}
		}
		return dictionary;
	}

	private bool method_2(CADLineInfo cadlineInfo_0, CADLineInfo cadlineInfo_1)
	{
		double startX = cadlineInfo_0.StartX;
		double startY = cadlineInfo_0.StartY;
		double startZ = cadlineInfo_0.StartZ;
		double double_ = startY;
		double double_2 = startX;
		double endX = cadlineInfo_0.EndX;
		double endY = cadlineInfo_0.EndY;
		double endZ = cadlineInfo_0.EndZ;
		double double_3 = endY;
		double double_4 = endX;
		double startX2 = cadlineInfo_1.StartX;
		double startY2 = cadlineInfo_1.StartY;
		double startZ2 = cadlineInfo_1.StartZ;
		double double_5 = startY2;
		double double_6 = startX2;
		double endX2 = cadlineInfo_1.EndX;
		double endY2 = cadlineInfo_1.EndY;
		double endZ2 = cadlineInfo_1.EndZ;
		double double_7 = endY2;
		double double_8 = endX2;
		double num = double_0 * double_0;
		if (!(method_3(double_2, double_, startZ, double_6, double_5, startZ2) <= num) && !(method_3(double_2, double_, startZ, double_8, double_7, endZ2) <= num) && !(method_3(double_4, double_3, endZ, double_6, double_5, startZ2) <= num))
		{
			return method_3(double_4, double_3, endZ, double_8, double_7, endZ2) <= num;
		}
		return true;
	}

	private double method_3(double double_1, double double_2, double double_3, double double_4, double double_5, double double_6)
	{
		double num = double_4 - double_1;
		double num2 = double_5 - double_2;
		double num3 = double_6 - double_3;
		return num * num + num2 * num2 + num3 * num3;
	}

	private AnnotationLineGroup method_4(List<CADLineInfo> list_0, int int_0, Dictionary<int, List<int>> dictionary_0, HashSet<int> hashSet_0)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		List<int> list = new List<int>();
		Queue<int> queue = new Queue<int>();
		queue.Enqueue(int_0);
		hashSet_0.Add(int_0);
		while (queue.Count > 0)
		{
			int num = queue.Dequeue();
			list.Add(num);
			foreach (int item in dictionary_0[num])
			{
				if (!hashSet_0.Contains(item))
				{
					hashSet_0.Add(item);
					queue.Enqueue(item);
				}
			}
		}
		List<int> list2 = method_5(list_0, list);
		AnnotationLineGroup val = new AnnotationLineGroup
		{
			Lines = list2.ConvertAll((int index) => list_0[index])
		};
		if (list2.Count > 0)
		{
			CADLineInfo val2 = val.Lines[0];
			val.StartPoint = (val2.StartX, val2.StartY, val2.StartZ);
			CADLineInfo val3 = val.Lines[val.Lines.Count - 1];
			val.EndPoint = (val3.EndX, val3.EndY, val3.EndZ);
			val.TotalLength = method_6(val.Lines);
			val.PrimaryDirectionAngle = method_7(val.Lines);
		}
		return val;
	}

	private List<int> method_5(List<CADLineInfo> list_0, List<int> list_1)
	{
		if (list_1.Count == 0)
		{
			return list_1;
		}
		if (list_1.Count == 1)
		{
			return list_1;
		}
		List<int> list = new List<int>();
		HashSet<int> hashSet = new HashSet<int>(list_1);
		int num = list_1[0];
		list.Add(num);
		hashSet.Remove(num);
		CADLineInfo obj = list_0[num];
		double double_ = obj.EndX;
		double double_2 = obj.EndY;
		double double_3 = obj.EndZ;
		while (hashSet.Count > 0)
		{
			int? num2 = null;
			bool flag = true;
			foreach (int item in hashSet)
			{
				CADLineInfo val = list_0[item];
				if (method_3(double_, double_2, double_3, val.StartX, val.StartY, val.StartZ) > double_0 * double_0)
				{
					if (!(method_3(double_, double_2, double_3, val.EndX, val.EndY, val.EndZ) > double_0 * double_0))
					{
						num2 = item;
						flag = false;
						break;
					}
					continue;
				}
				num2 = item;
				flag = true;
				break;
			}
			if (num2.HasValue)
			{
				list.Add(num2.Value);
				hashSet.Remove(num2.Value);
				CADLineInfo val2 = list_0[num2.Value];
				if (flag)
				{
					double_ = val2.EndX;
					double_2 = val2.EndY;
					double_3 = val2.EndZ;
					continue;
				}
				double_ = val2.StartX;
				double_2 = val2.StartY;
				double_3 = val2.StartZ;
				double startX = val2.StartX;
				double startY = val2.StartY;
				double startZ = val2.StartZ;
				val2.StartX = val2.EndX;
				val2.StartY = val2.EndY;
				val2.StartZ = val2.EndZ;
				val2.EndX = startX;
				val2.EndY = startY;
				val2.EndZ = startZ;
			}
			else
			{
				int num3 = hashSet.First();
				list.Add(num3);
				hashSet.Remove(num3);
				CADLineInfo obj2 = list_0[num3];
				double_ = obj2.EndX;
				double_2 = obj2.EndY;
				double_3 = obj2.EndZ;
			}
		}
		return list;
	}

	private double method_6(List<CADLineInfo> list_0)
	{
		double num = 0.0;
		foreach (CADLineInfo item in list_0)
		{
			double num2 = item.EndX - item.StartX;
			double num3 = item.EndY - item.StartY;
			double num4 = item.EndZ - item.StartZ;
			num += Math.Sqrt(num2 * num2 + num3 * num3 + num4 * num4);
		}
		return num;
	}

	private double method_7(List<CADLineInfo> list_0)
	{
		if (list_0.Count == 0)
		{
			return 0.0;
		}
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		foreach (CADLineInfo item in list_0)
		{
			double num4 = item.EndX - item.StartX;
			double num5 = item.EndY - item.StartY;
			double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
			if (num6 > 0.0)
			{
				double num7 = Math.Atan2(num5, num4);
				double num8 = num6;
				num += num8 * Math.Sin(num7);
				num2 += num8 * Math.Cos(num7);
				num3 += num8;
			}
		}
		if (num3 == 0.0)
		{
			return 0.0;
		}
		double num9 = Math.Atan2(num / num3, num2 / num3);
		if (num9 < 0.0)
		{
			num9 += Math.PI * 2.0;
		}
		return num9;
	}
}
