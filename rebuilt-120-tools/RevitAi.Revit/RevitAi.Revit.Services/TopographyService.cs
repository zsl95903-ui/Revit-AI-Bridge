using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit.FloorTopography;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using DelaunatorSharp;
using ns6;

using InvalidOperationException = System.InvalidOperationException;
using ArgumentNullException = System.ArgumentNullException;
using Edge = Autodesk.Revit.DB.Edge;
namespace RevitAi.Revit.Services;

internal sealed class TopographyService : ITopographyService
{
	private class ProfileClassification
	{
		public IList<CurveLoop> OuterProfiles { get; set; } = new List<CurveLoop>();

		public IList<CurveLoop> InnerProfiles { get; set; } = new List<CurveLoop>();
	}

	private class ProfileInfo
	{
		public CurveLoop Profile { get; set; } = null;

		public double Area { get; set; }

		public bool IsOuter { get; set; }

		public int OriginalIndex { get; set; }
	}

	internal class SiteSubRegionFailurePreprocessor : IFailuresPreprocessor
	{
		public bool NeedsRetry { get; private set; }

		public string LastErrorMessage { get; private set; } = string.Empty;

		public SiteSubRegionFailurePreprocessor()
		{
			NeedsRetry = false;
			LastErrorMessage = string.Empty;
		}

		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			//IL_0104: Unknown result type (might be due to invalid IL or missing references)
			//IL_0107: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Invalid comparison between Unknown and I4
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Invalid comparison between Unknown and I4
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
				if (failureMessages.Count == 0)
				{
					return (FailureProcessingResult)0;
				}
				foreach (FailureMessageAccessor item in failureMessages)
				{
					FailureSeverity severity = item.GetSeverity();
					string text = (LastErrorMessage = item.GetDescriptionText());
					if ((int)severity == 1)
					{
						failuresAccessor.DeleteWarning(item);
					}
					else if ((int)severity == 2)
					{
						string text2 = text.ToLower();
						if (text2.Contains("相交") || text2.Contains("intersect") || text2.Contains("子面域") || text2.Contains("overlap") || text2.Contains("cannot intersect") || text2.Contains("subregion"))
						{
							NeedsRetry = true;
							return (FailureProcessingResult)2;
						}
						return (FailureProcessingResult)2;
					}
				}
				return (FailureProcessingResult)1;
			}
			catch
			{
				return (FailureProcessingResult)0;
			}
		}

		public static void SetHandlerForTransaction(SiteSubRegionFailurePreprocessor handler, Transaction transaction)
		{
			FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();
			failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)handler);
			transaction.SetFailureHandlingOptions(failureHandlingOptions);
		}
	}

	internal struct Rect2D(double minX, double minY, double maxX, double maxY)
	{
		public double MinX = minX;

		public double MinY = minY;

		public double MaxX = maxX;

		public double MaxY = maxY;

		public bool Contains(double x, double y)
		{
			return x >= MinX && x <= MaxX && y >= MinY && y <= MaxY;
		}

		public bool Contains(XYZ point)
		{
			return point.X >= MinX && point.X <= MaxX && point.Y >= MinY && point.Y <= MaxY;
		}
	}

	internal struct TriangleData
	{
		public int Index;

		public Rect2D Bounds;

		public XYZ V1;

		public XYZ V2;

		public XYZ V3;
	}

	internal class QuadtreeNode
	{
		private const int MaxObjects = 10;

		private const int MaxLevels = 8;

		private int _level;

		private List<TriangleData> _objects = new List<TriangleData>();

		private QuadtreeNode[]? _nodes;

		public Rect2D Boundary { get; }

		public QuadtreeNode(Rect2D boundary, int level, int maxLevels)
		{
			Boundary = boundary;
			_level = level;
		}

		public void Insert(TriangleData tri)
		{
			if (_nodes != null)
			{
				int index = GetIndex(tri.Bounds);
				if (index != -1)
				{
					_nodes[index].Insert(tri);
					return;
				}
			}
			_objects.Add(tri);
			if (_objects.Count <= 10 || _level >= 8)
			{
				return;
			}
			if (_nodes == null)
			{
				Subdivide();
			}
			int num = 0;
			while (num < _objects.Count)
			{
				int index2 = GetIndex(_objects[num].Bounds);
				if (index2 != -1 && _nodes != null)
				{
					_nodes[index2].Insert(_objects[num]);
					_objects.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}

		public void Query(XYZ point, List<TriangleData> results)
		{
			if (!Boundary.Contains(point))
			{
				return;
			}
			results.AddRange(_objects);
			if (_nodes != null)
			{
				QuadtreeNode[] nodes = _nodes;
				foreach (QuadtreeNode quadtreeNode in nodes)
				{
					quadtreeNode.Query(point, results);
				}
			}
		}

		private void Subdivide()
		{
			double num = (Boundary.MaxX - Boundary.MinX) / 2.0;
			double num2 = (Boundary.MaxY - Boundary.MinY) / 2.0;
			double minX = Boundary.MinX;
			double minY = Boundary.MinY;
			_nodes = new QuadtreeNode[4];
			_nodes[0] = new QuadtreeNode(new Rect2D(minX + num, minY, Boundary.MaxX, minY + num2), _level + 1, 8);
			_nodes[1] = new QuadtreeNode(new Rect2D(minX, minY, minX + num, minY + num2), _level + 1, 8);
			_nodes[2] = new QuadtreeNode(new Rect2D(minX, minY + num2, minX + num, Boundary.MaxY), _level + 1, 8);
			_nodes[3] = new QuadtreeNode(new Rect2D(minX + num, minY + num2, Boundary.MaxX, Boundary.MaxY), _level + 1, 8);
		}

		private int GetIndex(Rect2D bounds)
		{
			int result = -1;
			double num = Boundary.MinY + (Boundary.MaxY - Boundary.MinY) / 2.0;
			double num2 = Boundary.MinX + (Boundary.MaxX - Boundary.MinX) / 2.0;
			bool flag = bounds.MinY > num;
			bool flag2 = bounds.MaxY < num;
			if (flag)
			{
				if (bounds.MinX > num2)
				{
					result = 0;
				}
				else if (bounds.MaxX < num2)
				{
					result = 3;
				}
			}
			else if (flag2)
			{
				if (bounds.MinX > num2)
				{
					result = 1;
				}
				else if (bounds.MaxX < num2)
				{
					result = 2;
				}
			}
			return result;
		}
	}

	private readonly UIApplication _application;

	private readonly IParameterService _parameterService;

	private const double Tolerance = 0.0032;

	public TopographyService(UIApplication application, IParameterService? parameterService = null)
	{
		_application = application ?? throw new ArgumentNullException("application");
		_parameterService = (IParameterService)(((object)parameterService) ?? ((object)new ParameterService(application)));
	}

	public object? CreateTopographyFromFloorProfile(object document, object floorElement, object topographyElement)
	{
		List<(object, ElementId)> materialAssignments = new List<(object, ElementId)>();
		return CreateTopographyFromFloorProfile(document, floorElement, topographyElement, null, materialAssignments);
	}

	private object? CreateTopographyFromFloorProfile(object document, object floorElement, object topographyElement, ElementId? floorMaterialId, List<(object subRegion, ElementId materialId)> materialAssignments, double offsetMm = 1.0)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateTopographyFromFloorProfile: document 不是 Document 类型");
				return null;
			}
			Floor val2 = (Floor)((floorElement is Floor) ? floorElement : null);
			if (val2 == null)
			{
				LogError("CreateTopographyFromFloorProfile: floorElement 不是 Floor 类型");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
			defaultInterpolatedStringHandler.AppendLiteral("开始创建地形子面域，楼板 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val2).Id);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			IList<CurveLoop> floorProfiles = GetFloorProfiles(val2);
			if (floorProfiles == null || floorProfiles.Count == 0)
			{
				LogError("无法获取楼板轮廓");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("检测到 ");
			defaultInterpolatedStringHandler2.AppendFormatted(floorProfiles.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个轮廓");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			ProfileClassification profileClassification = ClassifyProfiles(floorProfiles);
			_ = profileClassification.OuterProfiles;
			_ = profileClassification.InnerProfiles;
			IList<XYZ> topographyPoints = GetTopographyPoints(topographyElement);
			if (topographyPoints == null || topographyPoints.Count == 0)
			{
				LogError("地形点集为空");
				return null;
			}
			QuadtreeNode quadtreeNode = BuildTriangleIndex(topographyPoints);
			if (quadtreeNode == null)
			{
				LogError("构建三角网索引失败");
				return null;
			}
			new List<object>();
			LogError("[TopographyService] Revit 2024+ 多轮廓处理暂未实现");
			return null;
		}
		catch (Exception ex)
		{
			LogError("CreateTopographyFromFloorProfile 失败: " + ex.Message);
			return null;
		}
	}

	public (int SuccessCount, List<string> Errors, List<object> CreatedTopographies) CreateTopographiesFromFloorProfiles(object document, List<object> floorElements, object topographyElement)
	{
		//IL_0bdd: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fe: Expected O, but got Unknown
		//IL_0700: Unknown result type (might be due to invalid IL or missing references)
		//IL_08dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b82: Unknown result type (might be due to invalid IL or missing references)
		//IL_0aef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0af6: Expected O, but got Unknown
		//IL_0af8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0442: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Expected O, but got Unknown
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a8: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		List<object> list2 = new List<object>();
		int num = 0;
		List<(object, ElementId)> list3 = new List<(object, ElementId)>();
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			list.Add("document 不是 Document 类型");
			return (SuccessCount: 0, Errors: list, CreatedTopographies: list2);
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
		defaultInterpolatedStringHandler.AppendLiteral("批量创建地形子面域开始，楼板数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(floorElements.Count);
		LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		var list4 = (from x in floorElements.Select((object floor2, int index) => new
			{
				Floor = floor2,
				Index = index,
				Area = GetFloorArea(floor2)
			})
			orderby x.Area descending
			select x).ToList();
		int num2 = 0;
		List<(Floor, ElementId, IList<CurveLoop>)> list5 = new List<(Floor, ElementId, IList<CurveLoop>)>();
		for (int num3 = 0; num3 < list4.Count; num3++)
		{
			var anon = list4[num3];
			object floor = anon.Floor;
			Floor val2 = (Floor)((floor is Floor) ? floor : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler2.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler2.AppendLiteral(": 元素不是 Floor 类型");
				list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
				continue;
			}
			IList<CurveLoop> floorProfiles = GetFloorProfiles(val2);
			if (floorProfiles == null || floorProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler3.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler3.AppendLiteral(": 无法获取楼板轮廓");
				list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
				continue;
			}
			ProfileClassification profileClassification = ClassifyProfiles(floorProfiles);
			IList<CurveLoop> outerProfiles = profileClassification.OuterProfiles;
			_ = profileClassification.InnerProfiles;
			if (outerProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler4.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler4.AppendLiteral(": 没有外轮廓");
				list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
				continue;
			}
			ElementId floorMaterialId = GetFloorMaterialId(val2);
			List<ProfileInfo> list6 = new List<ProfileInfo>();
			for (int num4 = 0; num4 < outerProfiles.Count; num4++)
			{
				double curveLoopArea = GetCurveLoopArea(outerProfiles[num4]);
				list6.Add(new ProfileInfo
				{
					Profile = outerProfiles[num4],
					Area = curveLoopArea,
					IsOuter = true,
					OriginalIndex = num4
				});
			}
			list6.Sort((ProfileInfo a, ProfileInfo b) => b.Area.CompareTo(a.Area));
			List<CurveLoop> list7 = list6.Select((ProfileInfo x) => x.Profile).ToList();
			list5.Add((val2, floorMaterialId, list7));
			num2 += list7.Count;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(12, 1);
		defaultInterpolatedStringHandler5.AppendLiteral("共有 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个外轮廓需要创建");
		LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
		int num5 = 0;
		for (int num6 = 0; num6 < list5.Count; num6++)
		{
			(Floor, ElementId, IList<CurveLoop>) tuple = list5[num6];
			_ = tuple.Item1;
			ElementId item = tuple.Item2;
			IList<CurveLoop> item2 = tuple.Item3;
			for (int num7 = 0; num7 < item2.Count; num7++)
			{
				num5++;
				CurveLoop val3 = item2[num7];
				GetCurveLoopArea(val3);
				bool flag = false;
				object obj = null;
				double num8 = 0.0;
				int num9 = 0;
				while (num8 <= 50.0 && !flag && num9 < 10)
				{
					num9++;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(15, 3);
					defaultInterpolatedStringHandler6.AppendLiteral("创建地形子面域 ");
					defaultInterpolatedStringHandler6.AppendFormatted(num5);
					defaultInterpolatedStringHandler6.AppendLiteral("/");
					defaultInterpolatedStringHandler6.AppendFormatted(num2);
					defaultInterpolatedStringHandler6.AppendLiteral(" (尝试 ");
					defaultInterpolatedStringHandler6.AppendFormatted(num9);
					defaultInterpolatedStringHandler6.AppendLiteral(")");
					Transaction val4 = new Transaction(val, defaultInterpolatedStringHandler6.ToStringAndClear());
					SiteSubRegionFailurePreprocessor failuresPreprocessor = new SiteSubRegionFailurePreprocessor();
					FailureHandlingOptions failureHandlingOptions = val4.GetFailureHandlingOptions();
					failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)failuresPreprocessor);
					failureHandlingOptions.SetForcedModalHandling(true);
					failureHandlingOptions.SetClearAfterRollback(true);
					val4.SetFailureHandlingOptions(failureHandlingOptions);
					val4.Start();
					try
					{
						obj = CreateSingleSiteSubRegion(val, val3, topographyElement, num8);
						if (obj != null)
						{
							list2.Add(obj);
							if (item != (ElementId)null)
							{
								list3.Add((obj, item));
							}
							num++;
							flag = true;
							string text;
							if (num8 != 0.0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(2, 1);
								defaultInterpolatedStringHandler7.AppendFormatted(num8, "F1");
								defaultInterpolatedStringHandler7.AppendLiteral("mm");
								text = defaultInterpolatedStringHandler7.ToStringAndClear();
							}
							else
							{
								text = "原始";
							}
							string value = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(12, 4);
							defaultInterpolatedStringHandler8.AppendLiteral("[");
							defaultInterpolatedStringHandler8.AppendFormatted(num5);
							defaultInterpolatedStringHandler8.AppendLiteral("/");
							defaultInterpolatedStringHandler8.AppendFormatted(num2);
							defaultInterpolatedStringHandler8.AppendLiteral("] 成功（");
							defaultInterpolatedStringHandler8.AppendFormatted(value);
							defaultInterpolatedStringHandler8.AppendLiteral("，尝试 ");
							defaultInterpolatedStringHandler8.AppendFormatted(num9);
							defaultInterpolatedStringHandler8.AppendLiteral("）");
							LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
							val4.Commit();
							break;
						}
						val4.RollBack();
						if (num8 == 0.0)
						{
							num8 = 1.0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(20, 1);
							defaultInterpolatedStringHandler9.AppendLiteral("创建失败（使用原始轮廓），尝试偏移 ");
							defaultInterpolatedStringHandler9.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler9.AppendLiteral("mm");
							LogInfo(defaultInterpolatedStringHandler9.ToStringAndClear());
							continue;
						}
						if (num8 < 50.0)
						{
							num8 *= 1.5;
							if (num8 > 50.0)
							{
								num8 = 50.0;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(16, 1);
							defaultInterpolatedStringHandler10.AppendLiteral("创建失败，增加偏移至 ");
							defaultInterpolatedStringHandler10.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler10.AppendLiteral("mm 重试");
							LogInfo(defaultInterpolatedStringHandler10.ToStringAndClear());
							continue;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(15, 1);
						defaultInterpolatedStringHandler11.AppendLiteral("创建失败，已达到最大偏移 ");
						defaultInterpolatedStringHandler11.AppendFormatted(50.0);
						defaultInterpolatedStringHandler11.AppendLiteral("mm");
						LogError(defaultInterpolatedStringHandler11.ToStringAndClear());
					}
					catch (InvalidOperationException ex)
					{
						InvalidOperationException ex2 = ex;
						val4.RollBack();
						string text2 = ((Exception)(object)ex2).Message.ToLower();
						if ((text2.Contains("相交") || text2.Contains("intersect") || text2.Contains("子面域") || text2.Contains("curve loops intersect") || text2.Contains("subregion") || text2.Contains("overlap")) && num8 < 50.0)
						{
							num8 = ((num8 != 0.0) ? (num8 * 1.5) : 1.0);
							if (num8 > 50.0)
							{
								num8 = 50.0;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(24, 2);
							defaultInterpolatedStringHandler12.AppendLiteral("检测到相交错误，增加偏移至 ");
							defaultInterpolatedStringHandler12.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler12.AppendLiteral("mm 重试（尝试 ");
							defaultInterpolatedStringHandler12.AppendFormatted(num9);
							defaultInterpolatedStringHandler12.AppendLiteral("）");
							LogInfo(defaultInterpolatedStringHandler12.ToStringAndClear());
							continue;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(9, 3);
						defaultInterpolatedStringHandler13.AppendLiteral("楼板 ");
						defaultInterpolatedStringHandler13.AppendFormatted(num6);
						defaultInterpolatedStringHandler13.AppendLiteral(" 轮廓 ");
						defaultInterpolatedStringHandler13.AppendFormatted(num7);
						defaultInterpolatedStringHandler13.AppendLiteral(": ");
						defaultInterpolatedStringHandler13.AppendFormatted(((Exception)(object)ex2).Message);
						list.Add(defaultInterpolatedStringHandler13.ToStringAndClear());
						LogError("创建失败: " + ((Exception)(object)ex2).Message);
					}
					catch (Exception ex3)
					{
						val4.RollBack();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler14 = new DefaultInterpolatedStringHandler(9, 3);
						defaultInterpolatedStringHandler14.AppendLiteral("楼板 ");
						defaultInterpolatedStringHandler14.AppendFormatted(num6);
						defaultInterpolatedStringHandler14.AppendLiteral(" 轮廓 ");
						defaultInterpolatedStringHandler14.AppendFormatted(num7);
						defaultInterpolatedStringHandler14.AppendLiteral(": ");
						defaultInterpolatedStringHandler14.AppendFormatted(ex3.Message);
						list.Add(defaultInterpolatedStringHandler14.ToStringAndClear());
						LogError("创建失败: " + ex3.Message);
					}
					break;
				}
				if (!flag)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler15 = new DefaultInterpolatedStringHandler(21, 3);
					defaultInterpolatedStringHandler15.AppendLiteral("楼板 ");
					defaultInterpolatedStringHandler15.AppendFormatted(num6);
					defaultInterpolatedStringHandler15.AppendLiteral(" 轮廓 ");
					defaultInterpolatedStringHandler15.AppendFormatted(num7);
					defaultInterpolatedStringHandler15.AppendLiteral(": 创建失败（已尝试 ");
					defaultInterpolatedStringHandler15.AppendFormatted(num9);
					defaultInterpolatedStringHandler15.AppendLiteral(" 次）");
					list.Add(defaultInterpolatedStringHandler15.ToStringAndClear());
				}
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler16 = new DefaultInterpolatedStringHandler(19, 1);
		defaultInterpolatedStringHandler16.AppendLiteral("创建阶段完成，成功创建 ");
		defaultInterpolatedStringHandler16.AppendFormatted(num);
		defaultInterpolatedStringHandler16.AppendLiteral(" 个地形子面域");
		LogInfo(defaultInterpolatedStringHandler16.ToStringAndClear());
		if (num == 0)
		{
			LogWarning("没有成功创建任何地形子面域");
			return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
		}
		if (list3.Count > 0)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler17 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler17.AppendLiteral("开始为 ");
			defaultInterpolatedStringHandler17.AppendFormatted(list3.Count);
			defaultInterpolatedStringHandler17.AppendLiteral(" 个外轮廓设置材质");
			LogInfo(defaultInterpolatedStringHandler17.ToStringAndClear());
			Transaction val5 = new Transaction(val, "设置地形子面域材质");
			val5.Start();
			try
			{
				int num10 = 0;
				foreach (var (obj2, val6) in list3)
				{
					try
					{
						if (obj2 != null && val6 != (ElementId)null && SetMaterialForSubRegion(obj2, val6))
						{
							num10++;
						}
					}
					catch (Exception ex4)
					{
						LogError("设置材质失败: " + ex4.Message);
					}
				}
				val5.Commit();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler18 = new DefaultInterpolatedStringHandler(12, 2);
				defaultInterpolatedStringHandler18.AppendLiteral("材质设置完成，成功: ");
				defaultInterpolatedStringHandler18.AppendFormatted(num10);
				defaultInterpolatedStringHandler18.AppendLiteral("/");
				defaultInterpolatedStringHandler18.AppendFormatted(list3.Count);
				LogInfo(defaultInterpolatedStringHandler18.ToStringAndClear());
			}
			catch (Exception ex5)
			{
				val5.RollBack();
				LogError("材质设置失败: " + ex5.Message);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler19 = new DefaultInterpolatedStringHandler(12, 2);
		defaultInterpolatedStringHandler19.AppendLiteral("批量创建完成，成功: ");
		defaultInterpolatedStringHandler19.AppendFormatted(num);
		defaultInterpolatedStringHandler19.AppendLiteral("/");
		defaultInterpolatedStringHandler19.AppendFormatted(floorElements.Count);
		LogInfo(defaultInterpolatedStringHandler19.ToStringAndClear());
		return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
	}

	public (int SuccessCount, List<string> Errors, List<object> CreatedTopographies) CreateTopographiesFromFloorProfiles(object document, List<object> floorElements, object topographyElement, IProgressReporter? progressReporter)
	{
		//IL_0d60: Unknown result type (might be due to invalid IL or missing references)
		//IL_076d: Expected O, but got Unknown
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_094b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b8: Expected O, but got Unknown
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c5b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c62: Expected O, but got Unknown
		//IL_0c64: Unknown result type (might be due to invalid IL or missing references)
		//IL_0624: Unknown result type (might be due to invalid IL or missing references)
		//IL_0617: Unknown result type (might be due to invalid IL or missing references)
		List<string> list = new List<string>();
		List<object> list2 = new List<object>();
		int num = 0;
		List<(object, ElementId)> list3 = new List<(object, ElementId)>();
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			list.Add("document 不是 Document 类型");
			return (SuccessCount: 0, Errors: list, CreatedTopographies: list2);
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
		defaultInterpolatedStringHandler.AppendLiteral("批量创建地形子面域开始，楼板数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(floorElements.Count);
		LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		if (progressReporter != null)
		{
			progressReporter.ReportCategory("预处理楼板", 0, 1);
		}
		var list4 = (from x in floorElements.Select((object floor2, int index) => new
			{
				Floor = floor2,
				Index = index,
				Area = GetFloorArea(floor2)
			})
			orderby x.Area descending
			select x).ToList();
		int num2 = 0;
		List<(Floor, ElementId, IList<CurveLoop>)> list5 = new List<(Floor, ElementId, IList<CurveLoop>)>();
		for (int num3 = 0; num3 < list4.Count; num3++)
		{
			var anon = list4[num3];
			object floor = anon.Floor;
			Floor val2 = (Floor)((floor is Floor) ? floor : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler2.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler2.AppendLiteral(": 元素不是 Floor 类型");
				list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
				continue;
			}
			IList<CurveLoop> floorProfiles = GetFloorProfiles(val2);
			if (floorProfiles == null || floorProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler3.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler3.AppendLiteral(": 无法获取楼板轮廓");
				list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
				continue;
			}
			ProfileClassification profileClassification = ClassifyProfiles(floorProfiles);
			IList<CurveLoop> outerProfiles = profileClassification.OuterProfiles;
			_ = profileClassification.InnerProfiles;
			if (outerProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler4.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler4.AppendLiteral(": 没有外轮廓");
				list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
				continue;
			}
			ElementId floorMaterialId = GetFloorMaterialId(val2);
			List<ProfileInfo> list6 = new List<ProfileInfo>();
			for (int num4 = 0; num4 < outerProfiles.Count; num4++)
			{
				double curveLoopArea = GetCurveLoopArea(outerProfiles[num4]);
				list6.Add(new ProfileInfo
				{
					Profile = outerProfiles[num4],
					Area = curveLoopArea,
					IsOuter = true,
					OriginalIndex = num4
				});
			}
			list6.Sort((ProfileInfo a, ProfileInfo b) => b.Area.CompareTo(a.Area));
			List<CurveLoop> list7 = list6.Select((ProfileInfo x) => x.Profile).ToList();
			list5.Add((val2, floorMaterialId, list7));
			num2 += list7.Count;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(12, 1);
		defaultInterpolatedStringHandler5.AppendLiteral("共有 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个外轮廓需要创建");
		LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
		if (progressReporter != null)
		{
			progressReporter.ReportCategory("预处理楼板", 1, 1);
		}
		if (progressReporter != null)
		{
			progressReporter.ReportCategory("创建地形子面域", 0, num2);
		}
		int num5 = 0;
		int num6 = -1;
		int num7 = 0;
		while (true)
		{
			if (num7 < list5.Count)
			{
				if (progressReporter != null && progressReporter.IsCancellationRequested)
				{
					break;
				}
				(Floor, ElementId, IList<CurveLoop>) tuple = list5[num7];
				_ = tuple.Item1;
				ElementId item = tuple.Item2;
				IList<CurveLoop> item2 = tuple.Item3;
				for (int num8 = 0; num8 < item2.Count; num8++)
				{
					if (progressReporter == null || !progressReporter.IsCancellationRequested)
					{
						num5++;
						CurveLoop val3 = item2[num8];
						GetCurveLoopArea(val3);
						bool flag = false;
						object obj = null;
						double num9 = 0.0;
						int num10 = 0;
						while (num9 <= 50.0 && !flag && num10 < 10)
						{
							num10++;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(15, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("创建地形子面域 ");
							defaultInterpolatedStringHandler6.AppendFormatted(num5);
							defaultInterpolatedStringHandler6.AppendLiteral("/");
							defaultInterpolatedStringHandler6.AppendFormatted(num2);
							defaultInterpolatedStringHandler6.AppendLiteral(" (尝试 ");
							defaultInterpolatedStringHandler6.AppendFormatted(num10);
							defaultInterpolatedStringHandler6.AppendLiteral(")");
							Transaction val4 = new Transaction(val, defaultInterpolatedStringHandler6.ToStringAndClear());
							SiteSubRegionFailurePreprocessor failuresPreprocessor = new SiteSubRegionFailurePreprocessor();
							FailureHandlingOptions failureHandlingOptions = val4.GetFailureHandlingOptions();
							failureHandlingOptions.SetFailuresPreprocessor((IFailuresPreprocessor)(object)failuresPreprocessor);
							failureHandlingOptions.SetForcedModalHandling(true);
							failureHandlingOptions.SetClearAfterRollback(true);
							val4.SetFailureHandlingOptions(failureHandlingOptions);
							val4.Start();
							try
							{
								obj = CreateSingleSiteSubRegion(val, val3, topographyElement, num9);
								if (obj != null)
								{
									list2.Add(obj);
									if (item != (ElementId)null)
									{
										list3.Add((obj, item));
									}
									num++;
									flag = true;
									string text;
									if (num9 != 0.0)
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(2, 1);
										defaultInterpolatedStringHandler7.AppendFormatted(num9, "F1");
										defaultInterpolatedStringHandler7.AppendLiteral("mm");
										text = defaultInterpolatedStringHandler7.ToStringAndClear();
									}
									else
									{
										text = "原始";
									}
									string value = text;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(12, 4);
									defaultInterpolatedStringHandler8.AppendLiteral("[");
									defaultInterpolatedStringHandler8.AppendFormatted(num5);
									defaultInterpolatedStringHandler8.AppendLiteral("/");
									defaultInterpolatedStringHandler8.AppendFormatted(num2);
									defaultInterpolatedStringHandler8.AppendLiteral("] 成功（");
									defaultInterpolatedStringHandler8.AppendFormatted(value);
									defaultInterpolatedStringHandler8.AppendLiteral("，尝试 ");
									defaultInterpolatedStringHandler8.AppendFormatted(num10);
									defaultInterpolatedStringHandler8.AppendLiteral("）");
									LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
									val4.Commit();
									break;
								}
								val4.RollBack();
								if (num9 == 0.0)
								{
									num9 = 1.0;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(20, 1);
									defaultInterpolatedStringHandler9.AppendLiteral("创建失败（使用原始轮廓），尝试偏移 ");
									defaultInterpolatedStringHandler9.AppendFormatted(num9, "F1");
									defaultInterpolatedStringHandler9.AppendLiteral("mm");
									LogInfo(defaultInterpolatedStringHandler9.ToStringAndClear());
									continue;
								}
								if (num9 < 50.0)
								{
									num9 *= 1.5;
									if (num9 > 50.0)
									{
										num9 = 50.0;
									}
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(16, 1);
									defaultInterpolatedStringHandler10.AppendLiteral("创建失败，增加偏移至 ");
									defaultInterpolatedStringHandler10.AppendFormatted(num9, "F1");
									defaultInterpolatedStringHandler10.AppendLiteral("mm 重试");
									LogInfo(defaultInterpolatedStringHandler10.ToStringAndClear());
									continue;
								}
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(15, 1);
								defaultInterpolatedStringHandler11.AppendLiteral("创建失败，已达到最大偏移 ");
								defaultInterpolatedStringHandler11.AppendFormatted(50.0);
								defaultInterpolatedStringHandler11.AppendLiteral("mm");
								LogError(defaultInterpolatedStringHandler11.ToStringAndClear());
							}
							catch (InvalidOperationException ex)
							{
								InvalidOperationException ex2 = ex;
								val4.RollBack();
								string text2 = ((Exception)(object)ex2).Message.ToLower();
								if ((text2.Contains("相交") || text2.Contains("intersect") || text2.Contains("子面域") || text2.Contains("curve loops intersect") || text2.Contains("subregion") || text2.Contains("overlap")) && num9 < 50.0)
								{
									num9 = ((num9 != 0.0) ? (num9 * 1.5) : 1.0);
									if (num9 > 50.0)
									{
										num9 = 50.0;
									}
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(24, 2);
									defaultInterpolatedStringHandler12.AppendLiteral("检测到相交错误，增加偏移至 ");
									defaultInterpolatedStringHandler12.AppendFormatted(num9, "F1");
									defaultInterpolatedStringHandler12.AppendLiteral("mm 重试（尝试 ");
									defaultInterpolatedStringHandler12.AppendFormatted(num10);
									defaultInterpolatedStringHandler12.AppendLiteral("）");
									LogInfo(defaultInterpolatedStringHandler12.ToStringAndClear());
									continue;
								}
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(9, 3);
								defaultInterpolatedStringHandler13.AppendLiteral("楼板 ");
								defaultInterpolatedStringHandler13.AppendFormatted(num7);
								defaultInterpolatedStringHandler13.AppendLiteral(" 轮廓 ");
								defaultInterpolatedStringHandler13.AppendFormatted(num8);
								defaultInterpolatedStringHandler13.AppendLiteral(": ");
								defaultInterpolatedStringHandler13.AppendFormatted(((Exception)(object)ex2).Message);
								list.Add(defaultInterpolatedStringHandler13.ToStringAndClear());
								LogError("创建失败: " + ((Exception)(object)ex2).Message);
							}
							catch (Exception ex3)
							{
								val4.RollBack();
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler14 = new DefaultInterpolatedStringHandler(9, 3);
								defaultInterpolatedStringHandler14.AppendLiteral("楼板 ");
								defaultInterpolatedStringHandler14.AppendFormatted(num7);
								defaultInterpolatedStringHandler14.AppendLiteral(" 轮廓 ");
								defaultInterpolatedStringHandler14.AppendFormatted(num8);
								defaultInterpolatedStringHandler14.AppendLiteral(": ");
								defaultInterpolatedStringHandler14.AppendFormatted(ex3.Message);
								list.Add(defaultInterpolatedStringHandler14.ToStringAndClear());
								LogError("创建失败: " + ex3.Message);
							}
							break;
						}
						if (progressReporter != null && num2 > 0)
						{
							int num11 = num5 * 100 / num2;
							if (num11 != num6)
							{
								num6 = num11;
								string text3;
								if (!flag)
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler15 = new DefaultInterpolatedStringHandler(6, 2);
									defaultInterpolatedStringHandler15.AppendLiteral("创建失败 ");
									defaultInterpolatedStringHandler15.AppendFormatted(num5);
									defaultInterpolatedStringHandler15.AppendLiteral("/");
									defaultInterpolatedStringHandler15.AppendFormatted(num2);
									text3 = defaultInterpolatedStringHandler15.ToStringAndClear();
								}
								else
								{
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler16 = new DefaultInterpolatedStringHandler(6, 2);
									defaultInterpolatedStringHandler16.AppendLiteral("成功创建 ");
									defaultInterpolatedStringHandler16.AppendFormatted(num5);
									defaultInterpolatedStringHandler16.AppendLiteral("/");
									defaultInterpolatedStringHandler16.AppendFormatted(num2);
									text3 = defaultInterpolatedStringHandler16.ToStringAndClear();
								}
								string text4 = text3;
								progressReporter.Report(num5, num2, text4);
							}
						}
						if (!flag)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler17 = new DefaultInterpolatedStringHandler(21, 3);
							defaultInterpolatedStringHandler17.AppendLiteral("楼板 ");
							defaultInterpolatedStringHandler17.AppendFormatted(num7);
							defaultInterpolatedStringHandler17.AppendLiteral(" 轮廓 ");
							defaultInterpolatedStringHandler17.AppendFormatted(num8);
							defaultInterpolatedStringHandler17.AppendLiteral(": 创建失败（已尝试 ");
							defaultInterpolatedStringHandler17.AppendFormatted(num10);
							defaultInterpolatedStringHandler17.AppendLiteral(" 次）");
							list.Add(defaultInterpolatedStringHandler17.ToStringAndClear());
						}
						continue;
					}
					LogInfo("用户取消了操作");
					list.Add("用户取消了操作");
					return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
				}
				num7++;
				continue;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler18 = new DefaultInterpolatedStringHandler(19, 1);
			defaultInterpolatedStringHandler18.AppendLiteral("创建阶段完成，成功创建 ");
			defaultInterpolatedStringHandler18.AppendFormatted(num);
			defaultInterpolatedStringHandler18.AppendLiteral(" 个地形子面域");
			LogInfo(defaultInterpolatedStringHandler18.ToStringAndClear());
			if (progressReporter != null)
			{
				progressReporter.ReportCategory("创建地形子面域", num2, num2);
			}
			if (progressReporter != null)
			{
				progressReporter.ReportCategory("设置材质", 0, 1);
			}
			if (num == 0)
			{
				LogWarning("没有成功创建任何地形子面域");
				return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
			}
			if (list3.Count > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler19 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler19.AppendLiteral("开始为 ");
				defaultInterpolatedStringHandler19.AppendFormatted(list3.Count);
				defaultInterpolatedStringHandler19.AppendLiteral(" 个外轮廓设置材质");
				LogInfo(defaultInterpolatedStringHandler19.ToStringAndClear());
				Transaction val5 = new Transaction(val, "设置地形子面域材质");
				val5.Start();
				try
				{
					int num12 = 0;
					foreach (var (obj2, val6) in list3)
					{
						try
						{
							if (obj2 != null && val6 != (ElementId)null && SetMaterialForSubRegion(obj2, val6))
							{
								num12++;
							}
						}
						catch (Exception ex4)
						{
							LogError("设置材质失败: " + ex4.Message);
						}
					}
					val5.Commit();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler20 = new DefaultInterpolatedStringHandler(12, 2);
					defaultInterpolatedStringHandler20.AppendLiteral("材质设置完成，成功: ");
					defaultInterpolatedStringHandler20.AppendFormatted(num12);
					defaultInterpolatedStringHandler20.AppendLiteral("/");
					defaultInterpolatedStringHandler20.AppendFormatted(list3.Count);
					LogInfo(defaultInterpolatedStringHandler20.ToStringAndClear());
					if (progressReporter != null)
					{
						progressReporter.ReportCategory("设置材质", 1, 1);
					}
				}
				catch (Exception ex5)
				{
					val5.RollBack();
					LogError("材质设置失败: " + ex5.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler21 = new DefaultInterpolatedStringHandler(12, 2);
			defaultInterpolatedStringHandler21.AppendLiteral("批量创建完成，成功: ");
			defaultInterpolatedStringHandler21.AppendFormatted(num);
			defaultInterpolatedStringHandler21.AppendLiteral("/");
			defaultInterpolatedStringHandler21.AppendFormatted(floorElements.Count);
			LogInfo(defaultInterpolatedStringHandler21.ToStringAndClear());
			return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
		}
		LogInfo("用户取消了操作");
		list.Add("用户取消了操作");
		return (SuccessCount: num, Errors: list, CreatedTopographies: list2);
	}

	private double GetFloorArea(object floor)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Invalid comparison between Unknown and I4
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Invalid comparison between Unknown and I4
		try
		{
			Floor val = (Floor)((floor is Floor) ? floor : null);
			if (val != null)
			{
				Parameter val2 = ((Element)val).get_Parameter((BuiltInParameter)(-1012805L));
				if (val2 != null && (int)val2.StorageType == 2)
				{
					return val2.AsDouble();
				}
				FloorType floorType = val.FloorType;
				if (floorType != null)
				{
					Parameter val3 = ((Element)floorType).get_Parameter((BuiltInParameter)(-1012805L));
					if (val3 != null && (int)val3.StorageType == 2)
					{
						return val3.AsDouble();
					}
				}
				return ComputeFloorAreaGeometry(val);
			}
			return 0.0;
		}
		catch (Exception ex)
		{
			LogWarning("获取楼板面积失败: " + ex.Message);
			return 0.0;
		}
	}

	private double ComputeFloorAreaGeometry(Floor floor)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Expected O, but got Unknown
		try
		{
			Options val = new Options();
			GeometryElement val2 = ((Element)floor).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				return 0.0;
			}
			double num = 0.0;
			foreach (GeometryObject item in val2)
			{
				Solid val3 = (Solid)(object)((item is Solid) ? item : null);
				if (val3 == null)
				{
					continue;
				}
				foreach (Face face in val3.Faces)
				{
					Face val4 = face;
					XYZ val5 = val4.ComputeNormal(new UV(0.5, 0.5));
					if (Math.Abs(val5.Z) > 0.9)
					{
						num += val4.Area;
					}
				}
			}
			return num;
		}
		catch
		{
			return 0.0;
		}
	}

	private ElementId? GetFloorMaterialId(Floor floor)
	{
		try
		{
			FloorType floorType = floor.FloorType;
			if (floorType == null)
			{
				LogWarning("无法获取楼板类型");
				return null;
			}
			CompoundStructure compoundStructure = ((HostObjAttributes)floorType).GetCompoundStructure();
			if (compoundStructure == null)
			{
				LogWarning("楼板类型没有复合结构");
				return null;
			}
			IList<CompoundStructureLayer> layers = compoundStructure.GetLayers();
			if (layers == null || layers.Count == 0)
			{
				LogWarning("楼板复合结构没有层");
				return null;
			}
			if (layers.Count == 0)
			{
				LogWarning("楼板复合结构层为空");
				return null;
			}
			CompoundStructureLayer val = layers[0];
			if (val == null)
			{
				LogWarning("无法获取楼板第一层");
				return null;
			}
			ElementId materialId = val.MaterialId;
			if (materialId != (ElementId)null && materialId.Value != -1L)
			{
				return materialId;
			}
			LogWarning("楼板第一层材质无效");
			return null;
		}
		catch (Exception ex)
		{
			LogError("获取楼板材质失败: " + ex.Message);
			return null;
		}
	}

	private double InterpolateZAtPoint(XYZ point, QuadtreeNode quadtree, IList<XYZ> rawPoints)
	{
		List<TriangleData> list = new List<TriangleData>();
		quadtree.Query(point, list);
		foreach (TriangleData item in list)
		{
			if (IsPointInTriangle(point, item.V1, item.V2, item.V3))
			{
				return GetInterpolatedZ(point, item.V1, item.V2, item.V3);
			}
		}
		return FindNearestPointZ(point, rawPoints);
	}

	private bool IsPointInTriangle(XYZ p, XYZ p0, XYZ p1, XYZ p2)
	{
		double num = p0.Y * p2.X - p0.X * p2.Y + (p2.Y - p0.Y) * p.X + (p0.X - p2.X) * p.Y;
		double num2 = p0.X * p1.Y - p0.Y * p1.X + (p0.Y - p1.Y) * p.X + (p1.X - p0.X) * p.Y;
		if (num < 0.0 != num2 < 0.0)
		{
			return false;
		}
		double num3 = (0.0 - p1.Y) * p2.X + p0.Y * (p2.X - p1.X) + p0.X * (p1.Y - p2.Y) + p1.X * p2.Y;
		return (!(num3 < 0.0)) ? (num >= 0.0 && num + num2 <= num3) : (num <= 0.0 && num + num2 >= num3);
	}

	private double GetInterpolatedZ(XYZ point, XYZ v1, XYZ v2, XYZ v3)
	{
		double area2D = GetArea2D(v1, v2, v3);
		if (Math.Abs(area2D) < 1E-09)
		{
			return v1.Z;
		}
		double num = GetArea2D(point, v2, v3) / area2D;
		double num2 = GetArea2D(v1, point, v3) / area2D;
		double num3 = 1.0 - num - num2;
		return num * v1.Z + num2 * v2.Z + num3 * v3.Z;
	}

	private double GetArea2D(XYZ a, XYZ b, XYZ c)
	{
		return 0.5 * Math.Abs(a.X * (b.Y - c.Y) + b.X * (c.Y - a.Y) + c.X * (a.Y - b.Y));
	}

	private double FindNearestPointZ(XYZ point, IList<XYZ> points)
	{
		double num = double.MaxValue;
		double z = point.Z;
		foreach (XYZ point2 in points)
		{
			double num2 = point2.X - point.X;
			double num3 = point2.Y - point.Y;
			double num4 = num2 * num2 + num3 * num3;
			if (num4 < num)
			{
				num = num4;
				z = point2.Z;
			}
		}
		return z;
	}

	private object? CreateToposolidWithShape(Document doc, Floor floor, CurveLoop boundaryLoop, List<XYZ> boundaryPointsWithElevation, ElementId? floorMaterialId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ToposolidType val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault((ToposolidType t) => ((Element)t).Name.Contains("Topography"));
			if (val == null)
			{
				LogWarning("未找到地形类型，尝试使用默认类型");
				val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault();
			}
			if (val == null)
			{
				LogError("文档中没有 ToposolidType");
				return null;
			}
			ElementId levelId = ((Element)floor).LevelId;
			Toposolid val2 = Toposolid.Create(doc, (IList<CurveLoop>)new List<CurveLoop> { boundaryLoop }, ((Element)val).Id, levelId);
			if (val2 == null)
			{
				LogError("创建 Toposolid 失败");
				return null;
			}
			if (floorMaterialId != (ElementId)null && floorMaterialId.Value != -1L)
			{
				try
				{
					Parameter val3 = ((Element)val2).LookupParameter("材质");
					if (val3 != null && !((APIObject)val3).IsReadOnly)
					{
						val3.Set((double)floorMaterialId.Value);
						LogInfo("[TopographyService] 成功设置 Toposolid 材质");
					}
				}
				catch (Exception ex)
				{
					LogWarning("设置 Toposolid 材质失败: " + ex.Message);
				}
			}
			try
			{
				SlabShapeEditor slabShapeEditor = val2.GetSlabShapeEditor();
				if (slabShapeEditor != null)
				{
					SlabShapeVertexArray slabShapeVertices = slabShapeEditor.SlabShapeVertices;
					foreach (XYZ item in boundaryPointsWithElevation)
					{
						SlabShapeVertex val4 = null;
						double num = double.MaxValue;
						foreach (object item2 in slabShapeVertices)
						{
							SlabShapeVertex val5 = (SlabShapeVertex)((item2 is SlabShapeVertex) ? item2 : null);
							if (val5 != null)
							{
								XYZ position = val5.Position;
								double num2 = position.X - item.X;
								double num3 = position.Y - item.Y;
								double num4 = num2 * num2 + num3 * num3;
								if (num4 < num)
								{
									num = num4;
									val4 = val5;
								}
							}
						}
						if (val4 != null && num < 1.0)
						{
							slabShapeEditor.ModifySubElement(val4, item.Z);
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 成功修改 ");
					defaultInterpolatedStringHandler.AppendFormatted(boundaryPointsWithElevation.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个顶点的高程");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			catch (Exception ex2)
			{
				LogWarning("使用 SlabShapeEditor 修改高程失败: " + ex2.Message);
			}
			LogInfo("[TopographyService] 创建 Toposolid 成功");
			return val2;
		}
		catch (Exception ex3)
		{
			LogError("CreateToposolidWithShape 失败: " + ex3.Message + "\n" + ex3.StackTrace);
			return null;
		}
	}

	private object? CreateSiteSubRegion(Document doc, CurveLoop boundaryLoop, object topographyElement)
	{
		try
		{
			if (!SiteSubRegion.IsValidBoundary((IList<CurveLoop>)new List<CurveLoop> { boundaryLoop }))
			{
				LogError("楼板边界不是有效的地形子面域边界");
				return null;
			}
			LogError("[TopographyService] Revit 2024+ 不支持 SiteSubRegion");
			return null;
		}
		catch (Exception ex)
		{
			LogError("CreateSiteSubRegion 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	private bool SetMaterialForSubRegion(object subRegion, ElementId materialId)
	{
		try
		{
			SiteSubRegion val = (SiteSubRegion)((subRegion is SiteSubRegion) ? subRegion : null);
			if (val == null)
			{
				LogError("[TopographyService] 子面域不是 SiteSubRegion 类型");
				return false;
			}
			TopographySurface topographySurface = val.TopographySurface;
			if (topographySurface == null)
			{
				LogWarning("[TopographyService] 无法通过 SiteSubRegion.TopographySurface 获取 Element");
				return false;
			}
			Parameter val2 = ((Element)topographySurface).get_Parameter((BuiltInParameter)(-1002107L));
			if (val2 == null)
			{
				LogWarning("[TopographyService] 无法获取材质参数（MATERIAL_ID_PARAM）");
				return false;
			}
			if (((APIObject)val2).IsReadOnly)
			{
				LogWarning("[TopographyService] 材质参数是只读的");
				return false;
			}
			val2.Set(materialId);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 成功设置子面域材质，材质 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(materialId.Value);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("[TopographyService] 设置子面域材质时发生异常: " + ex.Message);
			return false;
		}
	}

	private IList<CurveLoop>? GetFloorProfiles(Floor floor)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Expected O, but got Unknown
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		try
		{
			_ = ((Element)floor).Document;
			Options val = new Options();
			GeometryElement val2 = ((Element)floor).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("楼板 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)floor).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有几何信息");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			List<CurveLoop> list = new List<CurveLoop>();
			foreach (GeometryObject item in val2)
			{
				Solid val3 = (Solid)(object)((item is Solid) ? item : null);
				if (val3 == null)
				{
					continue;
				}
				foreach (Face face in val3.Faces)
				{
					Face val4 = face;
					XYZ val5 = val4.ComputeNormal(new UV(0.5, 0.5));
					if (Math.Abs(val5.Z) > 0.9)
					{
						IList<CurveLoop> edgesAsCurveLoops = val4.GetEdgesAsCurveLoops();
						if (edgesAsCurveLoops != null && edgesAsCurveLoops.Count > 0)
						{
							list.AddRange(edgesAsCurveLoops);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[TopographyService] 获取到 ");
				defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个轮廓");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				return list;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("楼板 ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)floor).Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" 无法找到水平面轮廓");
			LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
			return null;
		}
		catch (Exception ex)
		{
			LogError("获取楼板轮廓失败: " + ex.Message);
			return null;
		}
	}

	private CurveLoop? GetFloorProfile(Floor floor)
	{
		IList<CurveLoop> floorProfiles = GetFloorProfiles(floor);
		if (floorProfiles != null && floorProfiles.Count > 0)
		{
			return floorProfiles[0];
		}
		return null;
	}

	private ProfileClassification ClassifyProfiles(IList<CurveLoop> profiles)
	{
		ProfileClassification profileClassification = new ProfileClassification();
		if (profiles.Count == 0)
		{
			return profileClassification;
		}
		foreach (CurveLoop profile in profiles)
		{
			try
			{
				if (profile.IsCounterclockwise(XYZ.BasisZ))
				{
					profileClassification.OuterProfiles.Add(profile);
				}
				else
				{
					profileClassification.InnerProfiles.Add(profile);
				}
			}
			catch (Exception ex)
			{
				LogWarning("判断轮廓方向失败: " + ex.Message + "，默认作为外轮廓处理");
				profileClassification.OuterProfiles.Add(profile);
			}
		}
		return profileClassification;
	}

	private IList<XYZ>? GetTopographyPoints(object topographyElement)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Expected O, but got Unknown
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Expected O, but got Unknown
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		try
		{
			Toposolid val = (Toposolid)((topographyElement is Toposolid) ? topographyElement : null);
			if (val != null)
			{
				List<XYZ> list = new List<XYZ>();
				GeometryElement val2 = ((Element)val).get_Geometry(new Options());
				if ((GeometryObject)(object)val2 != (GeometryObject)null)
				{
					foreach (GeometryObject item in val2)
					{
						Solid val3 = (Solid)(object)((item is Solid) ? item : null);
						if (val3 == null)
						{
							continue;
						}
						foreach (Face face in val3.Faces)
						{
							Face val4 = face;
							EdgeArrayArray edgeLoops = val4.EdgeLoops;
							foreach (EdgeArray item2 in edgeLoops)
							{
								EdgeArray val5 = item2;
								foreach (Edge item3 in val5)
								{
									Edge val6 = item3;
									list.Add(val6.Evaluate(0.0));
									list.Add(val6.Evaluate(1.0));
								}
							}
						}
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 从 Toposolid 几何获取 ");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个点");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return list;
			}
			LogError("元素不是 Toposolid 类型");
			return null;
		}
		catch (Exception ex)
		{
			LogError("获取地形点集失败: " + ex.Message);
			return null;
		}
	}

	private QuadtreeNode? BuildTriangleIndex(IList<XYZ> points)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		try
		{
			List<IPoint> list = new List<IPoint>();
			for (int i = 0; i < points.Count; i++)
			{
				DelaunatorSharp.Point val = default(DelaunatorSharp.Point);
				val.X = points[i].X;
				val.Y = points[i].Y;
				list.Add((IPoint)(object)val);
			}
			Delaunator val2 = new Delaunator(list.ToArray());
			List<TriangleData> list2 = new List<TriangleData>();
			for (int j = 0; j < val2.Triangles.Length / 3; j++)
			{
				int num = val2.Triangles[j * 3];
				int num2 = val2.Triangles[j * 3 + 1];
				int num3 = val2.Triangles[j * 3 + 2];
				if (num >= 0 && num2 >= 0 && num3 >= 0 && num < points.Count && num2 < points.Count && num3 < points.Count)
				{
					XYZ val3 = points[num];
					XYZ val4 = points[num2];
					XYZ val5 = points[num3];
					Rect2D triangleBounds = GetTriangleBounds(val3, val4, val5);
					list2.Add(new TriangleData
					{
						Index = j,
						V1 = val3,
						V2 = val4,
						V3 = val5,
						Bounds = triangleBounds
					});
				}
			}
			Rect2D overallBounds = GetOverallBounds(points);
			QuadtreeNode quadtreeNode = new QuadtreeNode(overallBounds, 0, 8);
			foreach (TriangleData item in list2)
			{
				quadtreeNode.Insert(item);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 构建三角网完成，三角形数: ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return quadtreeNode;
		}
		catch (Exception ex)
		{
			LogError("构建三角网失败: " + ex.Message);
			return null;
		}
	}

	private Rect2D GetTriangleBounds(XYZ v0, XYZ v1, XYZ v2)
	{
		double minX = Math.Min(Math.Min(v0.X, v1.X), v2.X);
		double minY = Math.Min(Math.Min(v0.Y, v1.Y), v2.Y);
		double maxX = Math.Max(Math.Max(v0.X, v1.X), v2.X);
		double maxY = Math.Max(Math.Max(v0.Y, v1.Y), v2.Y);
		return new Rect2D(minX, minY, maxX, maxY);
	}

	private Rect2D GetOverallBounds(IList<XYZ> points)
	{
		double num = double.MaxValue;
		double num2 = double.MaxValue;
		double num3 = double.MinValue;
		double num4 = double.MinValue;
		foreach (XYZ point in points)
		{
			num = Math.Min(num, point.X);
			num2 = Math.Min(num2, point.Y);
			num3 = Math.Max(num3, point.X);
			num4 = Math.Max(num4, point.Y);
		}
		return new Rect2D(num, num2, num3, num4);
	}

	private List<XYZ> FilterPointsInsideBoundary(IList<XYZ> points, CurveLoop boundary)
	{
		List<XYZ> list = new List<XYZ>();
		(double, double, double, double) boundaryBoundingBox = GetBoundaryBoundingBox(boundary);
		foreach (XYZ point in points)
		{
			if (!(point.X < boundaryBoundingBox.Item1) && !(point.X > boundaryBoundingBox.Item3) && !(point.Y < boundaryBoundingBox.Item2) && !(point.Y > boundaryBoundingBox.Item4) && IsPointInBoundary(point, boundary))
			{
				list.Add(point);
			}
		}
		return list;
	}

	private (double MinX, double MinY, double MaxX, double MaxY) GetBoundaryBoundingBox(CurveLoop boundary)
	{
		double num = double.MaxValue;
		double num2 = double.MaxValue;
		double num3 = double.MinValue;
		double num4 = double.MinValue;
		foreach (Curve item in boundary)
		{
			XYZ endPoint = item.GetEndPoint(0);
			XYZ endPoint2 = item.GetEndPoint(1);
			num = Math.Min(num, Math.Min(endPoint.X, endPoint2.X));
			num2 = Math.Min(num2, Math.Min(endPoint.Y, endPoint2.Y));
			num3 = Math.Max(num3, Math.Max(endPoint.X, endPoint2.X));
			num4 = Math.Max(num4, Math.Max(endPoint.Y, endPoint2.Y));
		}
		return (MinX: num, MinY: num2, MaxX: num3, MaxY: num4);
	}

	private bool IsPointInBoundary(XYZ point, CurveLoop boundary)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		try
		{
			XYZ point2 = new XYZ(point.X, point.Y, 0.0);
			int num = 0;
			foreach (Curve item in boundary)
			{
				Line val = (Line)(object)((item is Line) ? item : null);
				if (val != null)
				{
					XYZ endPoint = ((Curve)val).GetEndPoint(0);
					XYZ endPoint2 = ((Curve)val).GetEndPoint(1);
					if (RayIntersectsSegment(point2, endPoint, endPoint2))
					{
						num++;
					}
				}
			}
			return num % 2 == 1;
		}
		catch
		{
			return false;
		}
	}

	private bool RayIntersectsSegment(XYZ point, XYZ segStart, XYZ segEnd)
	{
		if (segStart.Y > point.Y && segEnd.Y > point.Y)
		{
			return false;
		}
		if (segStart.Y < point.Y && segEnd.Y < point.Y)
		{
			return false;
		}
		if (segStart.Y == point.Y && segEnd.Y == point.Y)
		{
			return false;
		}
		double num = segEnd.X - segStart.X;
		double num2 = segEnd.Y - segStart.Y;
		double num3 = segStart.X;
		if (num2 != 0.0)
		{
			double num4 = (point.Y - segStart.Y) / num2;
			num3 = segStart.X + num4 * num;
		}
		return num3 >= point.X;
	}

	private List<XYZ> MergeAndDeduplicatePoints(List<XYZ> insidePoints, List<XYZ> boundaryPoints)
	{
		List<XYZ> list = new List<XYZ>();
		list.AddRange(insidePoints);
		list.AddRange(boundaryPoints);
		double num = 0.0032;
		Dictionary<(int, int), XYZ> dictionary = new Dictionary<(int, int), XYZ>();
		foreach (XYZ item3 in list)
		{
			int item = (int)(item3.X / num);
			int item2 = (int)(item3.Y / num);
			(int, int) key = (item, item2);
			if (!dictionary.ContainsKey(key))
			{
				dictionary[key] = item3;
			}
		}
		return dictionary.Values.ToList();
	}

	private object? CreateNewTopography(Document doc, List<XYZ> points)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (points.Count < 3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler.AppendLiteral("点数不足，无法创建地形: ");
				defaultInterpolatedStringHandler.AppendFormatted(points.Count);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			ToposolidType val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault((ToposolidType t) => ((Element)t).Name.Contains("Topography"));
			if (val == null)
			{
				LogWarning("未找到地形类型，尝试使用默认类型");
				val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault();
			}
			if (val == null)
			{
				LogError("文档中没有 ToposolidType");
				return null;
			}
			Toposolid result = Toposolid.Create(doc, (IList<XYZ>)points, ((Element)val).Id, ElementId.InvalidElementId);
			LogInfo("[TopographyService] 创建 Toposolid 成功");
			return result;
		}
		catch (Exception ex)
		{
			LogError("创建地形失败: " + ex.Message);
			return null;
		}
	}

	private object? CreateNewTopographyWithBoundary(Document doc, List<XYZ> allPoints, List<XYZ> boundaryPoints)
	{
		try
		{
			if (allPoints.Count < 3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler.AppendLiteral("点数不足，无法创建地形: ");
				defaultInterpolatedStringHandler.AppendFormatted(allPoints.Count);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			LogWarning("[TopographyService] Toposolid 不支持指定边界点，使用普通创建方法");
			return CreateNewTopography(doc, allPoints);
		}
		catch (Exception ex)
		{
			LogError("CreateNewTopographyWithBoundary 失败: " + ex.Message + "\n" + ex.StackTrace);
			return null;
		}
	}

	private CurveLoop? OffsetCurveLoopInward(CurveLoop originalLoop, double offsetDistance)
	{
		//IL_0167: Expected O, but got Unknown
		try
		{
			XYZ basisZ = XYZ.BasisZ;
			double num = (originalLoop.IsCounterclockwise(basisZ) ? (0.0 - offsetDistance) : offsetDistance);
			double curveLoopArea = GetCurveLoopArea(originalLoop);
			CurveLoop val = CurveLoop.CreateViaOffset(originalLoop, num, basisZ);
			if (val != null)
			{
				double curveLoopArea2 = GetCurveLoopArea(val);
				if (curveLoopArea2 < curveLoopArea * 0.99)
				{
					double value = (1.0 - curveLoopArea2 / curveLoopArea) * 100.0;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 2);
					defaultInterpolatedStringHandler.AppendLiteral("API偏移成功（缩小 ");
					defaultInterpolatedStringHandler.AppendFormatted(value, "F1");
					defaultInterpolatedStringHandler.AppendLiteral("%，偏移 ");
					defaultInterpolatedStringHandler.AppendFormatted(offsetDistance * 304.8, "F1");
					defaultInterpolatedStringHandler.AppendLiteral("mm）");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					return val;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("API偏移后面积未缩小（原面积 ");
				defaultInterpolatedStringHandler2.AppendFormatted(curveLoopArea, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral("，偏移后 ");
				defaultInterpolatedStringHandler2.AppendFormatted(curveLoopArea2, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral("）");
				LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			return OffsetCurveLoopManually(originalLoop, num, basisZ, curveLoopArea);
		}
		catch (InvalidOperationException ex)
		{
			InvalidOperationException ex2 = ex;
			LogWarning("API偏移抛出异常: " + ((Exception)(object)ex2).Message + "，尝试手动偏移");
			XYZ basisZ2 = XYZ.BasisZ;
			double offsetDistance2 = (originalLoop.IsCounterclockwise(basisZ2) ? (0.0 - offsetDistance) : offsetDistance);
			return OffsetCurveLoopManually(originalLoop, offsetDistance2, basisZ2);
		}
		catch (Exception ex3)
		{
			LogError("偏移完全失败: " + ex3.Message);
			return null;
		}
	}

	private CurveLoop? OffsetCurveLoopManually(CurveLoop originalLoop, double offsetDistance, XYZ normal, double originalArea = -1.0)
	{
		try
		{
			if (originalArea < 0.0)
			{
				originalArea = GetCurveLoopArea(originalLoop);
			}
			if (originalArea < 1.0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
				defaultInterpolatedStringHandler.AppendLiteral("轮廓面积太小 (");
				defaultInterpolatedStringHandler.AppendFormatted(originalArea, "F4");
				defaultInterpolatedStringHandler.AppendLiteral(" sqft)，跳过偏移");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			XYZ loopCenter = GetLoopCenter(originalLoop);
			List<Curve> list = new List<Curve>();
			foreach (Curve item2 in originalLoop)
			{
				XYZ endPoint = item2.GetEndPoint(0);
				XYZ endPoint2 = item2.GetEndPoint(1);
				XYZ val = (endPoint - loopCenter).Normalize();
				XYZ val2 = (endPoint2 - loopCenter).Normalize();
				XYZ val3 = endPoint - val * Math.Abs(offsetDistance);
				XYZ val4 = endPoint2 - val2 * Math.Abs(offsetDistance);
				Line val5 = (Line)(object)((item2 is Line) ? item2 : null);
				Curve item;
				if (val5 != null)
				{
					item = (Curve)(object)Line.CreateBound(val3, val4);
				}
				else
				{
					Arc val6 = (Arc)(object)((item2 is Arc) ? item2 : null);
					if (val6 != null)
					{
						try
						{
							item = (Curve)(object)Arc.Create(val3, val4, ((Curve)val6).Evaluate(0.5, true));
						}
						catch
						{
							item = (Curve)(object)Line.CreateBound(val3, val4);
						}
					}
					else
					{
						item = (Curve)(object)Line.CreateBound(val3, val4);
					}
				}
				list.Add(item);
			}
			if (list.Count > 0)
			{
				try
				{
					CurveLoop val7 = CurveLoop.Create((IList<Curve>)list);
					if (val7 != null)
					{
						double curveLoopArea = GetCurveLoopArea(val7);
						if (curveLoopArea < originalArea * 0.99)
						{
							double value = (1.0 - curveLoopArea / originalArea) * 100.0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("手动偏移成功（缩小 ");
							defaultInterpolatedStringHandler2.AppendFormatted(value, "F1");
							defaultInterpolatedStringHandler2.AppendLiteral("%，偏移 ");
							defaultInterpolatedStringHandler2.AppendFormatted(Math.Abs(offsetDistance) * 304.8, "F1");
							defaultInterpolatedStringHandler2.AppendLiteral("mm）");
							LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
							return val7;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("手动偏移后面积未有效缩小（原面积 ");
						defaultInterpolatedStringHandler3.AppendFormatted(originalArea, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral("，偏移后 ");
						defaultInterpolatedStringHandler3.AppendFormatted(curveLoopArea, "F2");
						defaultInterpolatedStringHandler3.AppendLiteral("）");
						LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
				}
				catch (Exception ex)
				{
					LogWarning("CurveLoop.Create 失败: " + ex.Message);
				}
			}
			return null;
		}
		catch (Exception ex2)
		{
			LogWarning("手动偏移失败: " + ex2.Message);
			return null;
		}
	}

	private XYZ GetLoopCenter(CurveLoop loop)
	{
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Expected O, but got Unknown
		try
		{
			List<XYZ> list = new List<XYZ>();
			foreach (Curve item in loop)
			{
				list.Add(item.GetEndPoint(0));
				list.Add(item.GetEndPoint(1));
			}
			if (list.Count == 0)
			{
				return XYZ.Zero;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			foreach (XYZ item2 in list)
			{
				num += item2.X;
				num2 += item2.Y;
				num3 += item2.Z;
			}
			return new XYZ(num / (double)list.Count, num2 / (double)list.Count, num3 / (double)list.Count);
		}
		catch
		{
			return XYZ.Zero;
		}
	}

	private double GetCurveLoopArea(CurveLoop loop)
	{
		try
		{
			double num = 0.0;
			List<XYZ> list = new List<XYZ>();
			foreach (Curve item in loop)
			{
				list.Add(item.GetEndPoint(0));
			}
			for (int i = 0; i < list.Count; i++)
			{
				int index = (i + 1) % list.Count;
				num += list[i].X * list[index].Y;
				num -= list[index].X * list[i].Y;
			}
			return Math.Abs(num) / 2.0;
		}
		catch (Exception ex)
		{
			LogWarning("[TopographyService] 计算轮廓面积失败: " + ex.Message);
			return 0.0;
		}
	}

	private object? CreateSingleSiteSubRegion(Document doc, CurveLoop profile, object topographyElement, double offsetMm = 0.0)
	{
		TopographySurface val = (TopographySurface)((topographyElement is TopographySurface) ? topographyElement : null);
		if (val == null)
		{
			LogError("宿主地形不是 TopographySurface 类型");
			return null;
		}
		try
		{
			CurveLoop item = profile;
			if (offsetMm > 0.0)
			{
				CurveLoop val2 = OffsetCurveLoopInward(profile, offsetMm / 304.8);
				if (val2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 1);
					defaultInterpolatedStringHandler.AppendLiteral("偏移轮廓失败（偏移 ");
					defaultInterpolatedStringHandler.AppendFormatted(offsetMm);
					defaultInterpolatedStringHandler.AppendLiteral("mm）");
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
					return null;
				}
				item = val2;
			}
			if (!SiteSubRegion.IsValidBoundary((IList<CurveLoop>)new List<CurveLoop> { item }))
			{
				LogError("轮廓不是有效的地形子面域边界");
				return null;
			}
			SiteSubRegion val3 = SiteSubRegion.Create(doc, (IList<CurveLoop>)new List<CurveLoop> { item }, ((Element)val).Id);
			if (val3 == null)
			{
				LogError("创建 SiteSubRegion 失败");
				return null;
			}
			return val3;
		}
		catch (Exception ex)
		{
			LogError("CreateSingleSiteSubRegion 失败: " + ex.Message);
			throw;
		}
	}

	private object? CreateSiteSubRegionWithRetry(Document doc, CurveLoop originalLoop, object topographyElement, double initialOffsetMm = 1.0, int maxRetries = 5)
	{
		//IL_00f3: Expected O, but got Unknown
		TopographySurface val = (TopographySurface)((topographyElement is TopographySurface) ? topographyElement : null);
		if (val == null)
		{
			LogError("宿主地形不是 TopographySurface 类型");
			return null;
		}
		double num = initialOffsetMm;
		CurveLoop val2 = originalLoop;
		for (int i = 0; i < maxRetries; i++)
		{
			try
			{
				if (!SiteSubRegion.IsValidBoundary((IList<CurveLoop>)new List<CurveLoop> { val2 }))
				{
					throw new InvalidOperationException("轮廓不是有效的地形子面域边界");
				}
				SiteSubRegion val3 = SiteSubRegion.Create(doc, (IList<CurveLoop>)new List<CurveLoop> { val2 }, ((Element)val).Id);
				if (val3 == null)
				{
					throw new InvalidOperationException("SiteSubRegion.Create 返回 null");
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 成功创建子面域（尝试 ");
				defaultInterpolatedStringHandler.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler.AppendLiteral("，偏移 ");
				defaultInterpolatedStringHandler.AppendFormatted(num);
				defaultInterpolatedStringHandler.AppendLiteral("mm）");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return val3;
			}
			catch (InvalidOperationException ex)
			{
				InvalidOperationException ex2 = ex;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[TopographyService] 创建子面域失败（尝试 ");
				defaultInterpolatedStringHandler2.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler2.AppendLiteral("，偏移 ");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral("mm）: ");
				defaultInterpolatedStringHandler2.AppendFormatted(((Exception)(object)ex2).Message);
				LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				if (i >= maxRetries - 1)
				{
					LogError("[TopographyService] 创建子面域失败，已达到最大重试次数");
					return null;
				}
				num *= 1.5;
				if (num > 50.0)
				{
					num = 50.0;
				}
				val2 = OffsetCurveLoopInward(originalLoop, num / 304.8);
				if (val2 == null)
				{
					LogError("[TopographyService] 偏移轮廓失败");
					return null;
				}
			}
		}
		return null;
	}

	private string GetElementId(object element)
	{
		try
		{
			Toposolid val = (Toposolid)((element is Toposolid) ? element : null);
			if (val != null)
			{
				return ((Element)val).Id.Value.ToString();
			}
			return "Unknown";
		}
		catch
		{
			return "Unknown";
		}
	}

	private void LogInfo(string message)
	{
		try
		{
			Logger.Info("[TopographyService] " + message);
		}
		catch
		{
		}
	}

	private void LogWarning(string message)
	{
		try
		{
			Logger.Warning("[TopographyService] " + message);
		}
		catch
		{
		}
	}

	private void LogError(string message)
	{
		try
		{
			Logger.Error("[TopographyService] " + message);
		}
		catch
		{
		}
	}

	public Result<FloorTopographyResult> CreateTopographiesFromFloorProfiles(object document, FloorTopographyRequest request)
	{
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008c: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Result<FloorTopographyResult>.Failure("document 不是 Document 类型");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 开始批量创建地形子面域（自动管理事务），楼板数量: ");
			defaultInterpolatedStringHandler.AppendFormatted(request.FloorElementIds.Count);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			List<object> list = new List<object>();
			foreach (int floorElementId in request.FloorElementIds)
			{
				Element element = val.GetElement(new ElementId((long)floorElementId));
				if (element != null)
				{
					list.Add(element);
				}
			}
			if (list.Count == 0)
			{
				return Result<FloorTopographyResult>.Failure("未找到有效的楼板元素");
			}
			ElementId val2 = new ElementId((long)request.TopographyElementId);
			Element element2 = val.GetElement(val2);
			if (element2 == null)
			{
				return Result<FloorTopographyResult>.Failure("未找到地形元素");
			}
			(int SuccessCount, List<string> Errors, List<object> CreatedTopographies) tuple = CreateTopographiesFromFloorProfiles(val, list, element2);
			int item = tuple.SuccessCount;
			List<string> item2 = tuple.Errors;
			List<object> item3 = tuple.CreatedTopographies;
			FloorTopographyResult val3 = new FloorTopographyResult
			{
				SuccessCount = item,
				Errors = item2,
				CreatedTopographies = item3
			};
			foreach (object item4 in item3)
			{
				if (item4 == null)
				{
					continue;
				}
				SiteSubRegion val4 = (SiteSubRegion)((item4 is SiteSubRegion) ? item4 : null);
				if (val4 != null)
				{
					TopographySurface topographySurface = val4.TopographySurface;
					if (topographySurface != null)
					{
						val3.CreatedSubRegionIds.Add((int)((Element)topographySurface).Id.Value);
					}
				}
				else
				{
					Element val5 = (Element)((item4 is Element) ? item4 : null);
					if (val5 != null)
					{
						val3.CreatedSubRegionIds.Add((int)val5.Id.Value);
					}
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[TopographyService] 批量创建完成（自动管理事务），成功: ");
			defaultInterpolatedStringHandler2.AppendFormatted(item);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<FloorTopographyResult>.Success(val3);
		}
		catch (Exception ex)
		{
			LogError("[TopographyService] 批量创建失败（自动管理事务）: " + ex.Message);
			return Result<FloorTopographyResult>.Failure("创建失败: " + ex.Message);
		}
	}

	public Result<FloorTopographyResult> CreateTopographiesFromFloorProfilesWithTransaction(object document, FloorTopographyRequest request, object externalTransaction)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Result<FloorTopographyResult>.Failure("document 不是 Document 类型");
			}
			if (!(externalTransaction is Transaction))
			{
				return Result<FloorTopographyResult>.Failure("externalTransaction 不是 Transaction 类型");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 开始批量创建地形子面域（使用外部事务），楼板数量: ");
			defaultInterpolatedStringHandler.AppendFormatted(request.FloorElementIds.Count);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			List<object> list = new List<object>();
			foreach (int floorElementId in request.FloorElementIds)
			{
				Element element = val.GetElement(new ElementId((long)floorElementId));
				if (element != null)
				{
					list.Add(element);
				}
			}
			if (list.Count == 0)
			{
				return Result<FloorTopographyResult>.Failure("未找到有效的楼板元素");
			}
			ElementId val2 = new ElementId((long)request.TopographyElementId);
			Element element2 = val.GetElement(val2);
			if (element2 == null)
			{
				return Result<FloorTopographyResult>.Failure("未找到地形元素");
			}
			FloorTopographyResult val3 = CreateTopographiesFromFloorProfilesWithTransactionInternal(val, list, element2, (Transaction)externalTransaction);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[TopographyService] 批量创建完成（使用外部事务），成功: ");
			defaultInterpolatedStringHandler2.AppendFormatted(val3.SuccessCount);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return Result<FloorTopographyResult>.Success(val3);
		}
		catch (Exception ex)
		{
			LogError("[TopographyService] 批量创建失败（使用外部事务）: " + ex.Message);
			return Result<FloorTopographyResult>.Failure("创建失败: " + ex.Message);
		}
	}

	private FloorTopographyResult CreateTopographiesFromFloorProfilesWithTransactionInternal(Document doc, List<object> floorElements, object topographyElement, Transaction externalTransaction)
	{
		//IL_05f7: Expected O, but got Unknown
		//IL_0969: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Unknown result type (might be due to invalid IL or missing references)
		//IL_097c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0985: Expected O, but got Unknown
		List<string> list = new List<string>();
		List<object> list2 = new List<object>();
		int num = 0;
		List<(object, ElementId)> list3 = new List<(object, ElementId)>();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 批量创建地形子面域开始（外部事务），楼板数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(floorElements.Count);
		LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		var list4 = (from x in floorElements.Select((object floor2, int index) => new
			{
				Floor = floor2,
				Index = index,
				Area = GetFloorArea(floor2)
			})
			orderby x.Area descending
			select x).ToList();
		int num2 = 0;
		List<(Floor, ElementId, IList<CurveLoop>)> list5 = new List<(Floor, ElementId, IList<CurveLoop>)>();
		for (int num3 = 0; num3 < list4.Count; num3++)
		{
			var anon = list4[num3];
			object floor = anon.Floor;
			Floor val = (Floor)((floor is Floor) ? floor : null);
			if (val == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler2.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler2.AppendLiteral(": 元素不是 Floor 类型");
				list.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
				continue;
			}
			IList<CurveLoop> floorProfiles = GetFloorProfiles(val);
			if (floorProfiles == null || floorProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(13, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler3.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler3.AppendLiteral(": 无法获取楼板轮廓");
				list.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
				continue;
			}
			ProfileClassification profileClassification = ClassifyProfiles(floorProfiles);
			IList<CurveLoop> outerProfiles = profileClassification.OuterProfiles;
			if (outerProfiles.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler4.AppendLiteral("索引 ");
				defaultInterpolatedStringHandler4.AppendFormatted(anon.Index);
				defaultInterpolatedStringHandler4.AppendLiteral(": 没有外轮廓");
				list.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
				continue;
			}
			ElementId floorMaterialId = GetFloorMaterialId(val);
			List<ProfileInfo> list6 = new List<ProfileInfo>();
			for (int num4 = 0; num4 < outerProfiles.Count; num4++)
			{
				double curveLoopArea = GetCurveLoopArea(outerProfiles[num4]);
				list6.Add(new ProfileInfo
				{
					Profile = outerProfiles[num4],
					Area = curveLoopArea,
					IsOuter = true,
					OriginalIndex = num4
				});
			}
			list6.Sort((ProfileInfo a, ProfileInfo b) => b.Area.CompareTo(a.Area));
			List<CurveLoop> list7 = list6.Select((ProfileInfo x) => x.Profile).ToList();
			list5.Add((val, floorMaterialId, list7));
			num2 += list7.Count;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(38, 1);
		defaultInterpolatedStringHandler5.AppendLiteral("[TopographyService] 共有 ");
		defaultInterpolatedStringHandler5.AppendFormatted(num2);
		defaultInterpolatedStringHandler5.AppendLiteral(" 个外轮廓需要创建（外部事务）");
		LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
		int num5 = 0;
		for (int num6 = 0; num6 < list5.Count; num6++)
		{
			(Floor, ElementId, IList<CurveLoop>) tuple = list5[num6];
			_ = tuple.Item1;
			ElementId item = tuple.Item2;
			IList<CurveLoop> item2 = tuple.Item3;
			for (int num7 = 0; num7 < item2.Count; num7++)
			{
				num5++;
				CurveLoop profile = item2[num7];
				bool flag = false;
				object obj = null;
				double num8 = 0.0;
				int num9 = 0;
				while (num8 <= 50.0 && !flag && num9 < 10)
				{
					num9++;
					try
					{
						obj = CreateSingleSiteSubRegionInTransaction(doc, profile, topographyElement, num8, externalTransaction);
						if (obj != null)
						{
							list2.Add(obj);
							if (item != (ElementId)null)
							{
								list3.Add((obj, item));
							}
							num++;
							flag = true;
							string text;
							if (num8 != 0.0)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(2, 1);
								defaultInterpolatedStringHandler6.AppendFormatted(num8, "F1");
								defaultInterpolatedStringHandler6.AppendLiteral("mm");
								text = defaultInterpolatedStringHandler6.ToStringAndClear();
							}
							else
							{
								text = "原始";
							}
							string value = text;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(32, 4);
							defaultInterpolatedStringHandler7.AppendLiteral("[TopographyService] [");
							defaultInterpolatedStringHandler7.AppendFormatted(num5);
							defaultInterpolatedStringHandler7.AppendLiteral("/");
							defaultInterpolatedStringHandler7.AppendFormatted(num2);
							defaultInterpolatedStringHandler7.AppendLiteral("] 成功（");
							defaultInterpolatedStringHandler7.AppendFormatted(value);
							defaultInterpolatedStringHandler7.AppendLiteral("，尝试 ");
							defaultInterpolatedStringHandler7.AppendFormatted(num9);
							defaultInterpolatedStringHandler7.AppendLiteral("）");
							LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
							break;
						}
						if (num8 == 0.0)
						{
							num8 = 1.0;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(40, 1);
							defaultInterpolatedStringHandler8.AppendLiteral("[TopographyService] 创建失败（使用原始轮廓），尝试偏移 ");
							defaultInterpolatedStringHandler8.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler8.AppendLiteral("mm");
							LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
							continue;
						}
						if (num8 < 50.0)
						{
							num8 *= 1.5;
							if (num8 > 50.0)
							{
								num8 = 50.0;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(36, 1);
							defaultInterpolatedStringHandler9.AppendLiteral("[TopographyService] 创建失败，增加偏移至 ");
							defaultInterpolatedStringHandler9.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler9.AppendLiteral("mm 重试");
							LogInfo(defaultInterpolatedStringHandler9.ToStringAndClear());
							continue;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(35, 1);
						defaultInterpolatedStringHandler10.AppendLiteral("[TopographyService] 创建失败，已达到最大偏移 ");
						defaultInterpolatedStringHandler10.AppendFormatted(50.0);
						defaultInterpolatedStringHandler10.AppendLiteral("mm");
						LogError(defaultInterpolatedStringHandler10.ToStringAndClear());
					}
					catch (InvalidOperationException ex)
					{
						InvalidOperationException ex2 = ex;
						string text2 = ((Exception)(object)ex2).Message.ToLower();
						if ((text2.Contains("相交") || text2.Contains("intersect") || text2.Contains("子面域") || text2.Contains("curve loops intersect") || text2.Contains("subregion") || text2.Contains("overlap")) && num8 < 50.0)
						{
							num8 = ((num8 != 0.0) ? (num8 * 1.5) : 1.0);
							if (num8 > 50.0)
							{
								num8 = 50.0;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(44, 2);
							defaultInterpolatedStringHandler11.AppendLiteral("[TopographyService] 检测到相交错误，增加偏移至 ");
							defaultInterpolatedStringHandler11.AppendFormatted(num8, "F1");
							defaultInterpolatedStringHandler11.AppendLiteral("mm 重试（尝试 ");
							defaultInterpolatedStringHandler11.AppendFormatted(num9);
							defaultInterpolatedStringHandler11.AppendLiteral("）");
							LogInfo(defaultInterpolatedStringHandler11.ToStringAndClear());
							continue;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(9, 3);
						defaultInterpolatedStringHandler12.AppendLiteral("楼板 ");
						defaultInterpolatedStringHandler12.AppendFormatted(num6);
						defaultInterpolatedStringHandler12.AppendLiteral(" 轮廓 ");
						defaultInterpolatedStringHandler12.AppendFormatted(num7);
						defaultInterpolatedStringHandler12.AppendLiteral(": ");
						defaultInterpolatedStringHandler12.AppendFormatted(((Exception)(object)ex2).Message);
						list.Add(defaultInterpolatedStringHandler12.ToStringAndClear());
						LogError("[TopographyService] 创建失败: " + ((Exception)(object)ex2).Message);
					}
					catch (Exception ex3)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(9, 3);
						defaultInterpolatedStringHandler13.AppendLiteral("楼板 ");
						defaultInterpolatedStringHandler13.AppendFormatted(num6);
						defaultInterpolatedStringHandler13.AppendLiteral(" 轮廓 ");
						defaultInterpolatedStringHandler13.AppendFormatted(num7);
						defaultInterpolatedStringHandler13.AppendLiteral(": ");
						defaultInterpolatedStringHandler13.AppendFormatted(ex3.Message);
						list.Add(defaultInterpolatedStringHandler13.ToStringAndClear());
						LogError("[TopographyService] 创建失败: " + ex3.Message);
					}
					break;
				}
				if (!flag)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler14 = new DefaultInterpolatedStringHandler(21, 3);
					defaultInterpolatedStringHandler14.AppendLiteral("楼板 ");
					defaultInterpolatedStringHandler14.AppendFormatted(num6);
					defaultInterpolatedStringHandler14.AppendLiteral(" 轮廓 ");
					defaultInterpolatedStringHandler14.AppendFormatted(num7);
					defaultInterpolatedStringHandler14.AppendLiteral(": 创建失败（已尝试 ");
					defaultInterpolatedStringHandler14.AppendFormatted(num9);
					defaultInterpolatedStringHandler14.AppendLiteral(" 次）");
					list.Add(defaultInterpolatedStringHandler14.ToStringAndClear());
				}
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler15 = new DefaultInterpolatedStringHandler(45, 1);
		defaultInterpolatedStringHandler15.AppendLiteral("[TopographyService] 创建阶段完成（外部事务），成功创建 ");
		defaultInterpolatedStringHandler15.AppendFormatted(num);
		defaultInterpolatedStringHandler15.AppendLiteral(" 个地形子面域");
		LogInfo(defaultInterpolatedStringHandler15.ToStringAndClear());
		if (num == 0)
		{
			LogWarning("[TopographyService] 没有成功创建任何地形子面域");
		}
		FloorTopographyResult val2 = new FloorTopographyResult
		{
			SuccessCount = num,
			Errors = list,
			CreatedTopographies = list2
		};
		foreach (object item3 in list2)
		{
			if (item3 == null)
			{
				continue;
			}
			SiteSubRegion val3 = (SiteSubRegion)((item3 is SiteSubRegion) ? item3 : null);
			if (val3 != null)
			{
				TopographySurface topographySurface = val3.TopographySurface;
				if (topographySurface != null)
				{
					val2.CreatedSubRegionIds.Add((int)((Element)topographySurface).Id.Value);
				}
			}
			else
			{
				Element val4 = (Element)((item3 is Element) ? item3 : null);
				if (val4 != null)
				{
					val2.CreatedSubRegionIds.Add((int)val4.Id.Value);
				}
			}
		}
		return val2;
	}

	private object? CreateSingleSiteSubRegionInTransaction(Document doc, CurveLoop profile, object topographyElement, double offsetMm, Transaction externalTransaction)
	{
		TopographySurface val = (TopographySurface)((topographyElement is TopographySurface) ? topographyElement : null);
		if (val == null)
		{
			LogError("[TopographyService] 宿主地形不是 TopographySurface 类型");
			return null;
		}
		try
		{
			CurveLoop item = profile;
			if (offsetMm > 0.0)
			{
				CurveLoop val2 = OffsetCurveLoopInward(profile, offsetMm / 304.8);
				if (val2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
					defaultInterpolatedStringHandler.AppendLiteral("[TopographyService] 偏移轮廓失败（偏移 ");
					defaultInterpolatedStringHandler.AppendFormatted(offsetMm);
					defaultInterpolatedStringHandler.AppendLiteral("mm）");
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
					return null;
				}
				item = val2;
			}
			if (!SiteSubRegion.IsValidBoundary((IList<CurveLoop>)new List<CurveLoop> { item }))
			{
				LogError("[TopographyService] 轮廓不是有效的地形子面域边界");
				return null;
			}
			SiteSubRegion val3 = SiteSubRegion.Create(doc, (IList<CurveLoop>)new List<CurveLoop> { item }, ((Element)val).Id);
			if (val3 == null)
			{
				LogError("[TopographyService] 创建 SiteSubRegion 失败");
				return null;
			}
			return val3;
		}
		catch (Exception ex)
		{
			LogError("[TopographyService] CreateSingleSiteSubRegionInTransaction 失败: " + ex.Message);
			throw;
		}
	}

	public object? CreateTopographySurface(object document, IList<object> points)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("[CreateTopographySurface] document 不是 Document 类型");
				return null;
			}
			if (points.Count < 3)
			{
				LogError("[CreateTopographySurface] 点数不足，至少需要3个点");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CreateTopographySurface] 准备创建地形，点数: ");
			defaultInterpolatedStringHandler.AppendFormatted(points.Count);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			IList<XYZ> points2 = ConvertToXYZList(points);
			return CreateToposolid(val, points2);
		}
		catch (Exception ex)
		{
			LogError("[CreateTopographySurface] 创建地形失败: " + ex.Message);
			return null;
		}
	}

	public int AddPointsToTopography(object document, object topographyElement, IList<object> newPoints)
	{
		LogError("[AddPointsToTopography] Revit 2025+ 的 Toposolid 不支持通过 API 添加高程点");
		return 0;
	}

	public int ModifyTopographyPoints(object document, object topographyElement, IList<object> targetPoints, IList<object> newPoints)
	{
		LogError("[ModifyTopographyPoints] Revit 2025+ 的 Toposolid 不支持通过 API 修改高程点");
		return 0;
	}

	public int DeleteTopographyPoints(object document, object topographyElement, IList<object> pointsToDelete)
	{
		LogError("[DeleteTopographyPoints] Revit 2025+ 的 Toposolid 不支持通过 API 删除高程点");
		return 0;
	}

	private IList<XYZ> ConvertToXYZList(IList<object> points)
	{
		List<XYZ> list = new List<XYZ>();
		foreach (object point in points)
		{
			XYZ val = (XYZ)((point is XYZ) ? point : null);
			if (val != null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	private object? CreateToposolid(Document doc, IList<XYZ> points)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ToposolidType val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault((ToposolidType t) => ((Element)t).Name.Contains("Topography"));
			if (val == null)
			{
				LogWarning("[CreateToposolid] 未找到地形类型，使用默认类型");
				val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(ToposolidType))).Cast<ToposolidType>().FirstOrDefault();
			}
			if (val == null)
			{
				LogError("[CreateToposolid] 文档中没有 ToposolidType");
				return null;
			}
			Toposolid val2 = Toposolid.Create(doc, points, ((Element)val).Id, ElementId.InvalidElementId);
			if (val2 == null)
			{
				LogError("[CreateToposolid] Toposolid.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[CreateToposolid] 成功创建 Toposolid，ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Id.Value);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogError("[CreateToposolid] 创建失败: " + ex.Message);
			return null;
		}
	}
}
