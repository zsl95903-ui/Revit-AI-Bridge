using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ns6;

using Document = Autodesk.Revit.DB.Document;

using InvalidOperationException = System.InvalidOperationException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.Services;

internal sealed class ModificationService : IModificationService
{
	private class GeometryWarningSuppressor : IFailuresPreprocessor
	{
		public FailureProcessingResult PreprocessFailures(FailuresAccessor failuresAccessor)
		{
			IList<FailureMessageAccessor> failureMessages = failuresAccessor.GetFailureMessages();
			foreach (FailureMessageAccessor item in failureMessages)
			{
				string descriptionText = item.GetDescriptionText();
				if (descriptionText.Contains("高亮显示的图元已被连接但未相交") || descriptionText.Contains("Highlighted elements are joined but do not intersect") || descriptionText.Contains("Elements are joined but do not intersect"))
				{
					failuresAccessor.DeleteWarning(item);
					LogInfo("GeometryWarningSuppressor: 已抑制警告 - " + descriptionText);
				}
				else if (descriptionText.Contains("无法连接元素") || descriptionText.Contains("Elements cannot be joined") || descriptionText.Contains("连接但不相交"))
				{
					failuresAccessor.DeleteWarning(item);
					LogInfo("GeometryWarningSuppressor: 已抑制警告 - " + descriptionText);
				}
			}
			return (FailureProcessingResult)0;
		}
	}

	private readonly UIApplication _application;

	public ModificationService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> ArrayElement(object document, object element, int numberOfMembers, double moveX = 0.0, double moveY = 0.0, double moveZ = 0.0)
	{
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		List<object> list = new List<object>();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ArrayElement: document 不是 Document 类型");
				return list;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("ArrayElement: element 不是 Element 类型");
				return list;
			}
			if (numberOfMembers < 2)
			{
				LogError("ArrayElement: 阵列数量必须 >= 2");
				return list;
			}
			XYZ val3 = new XYZ(moveX, moveY, moveZ);
			List<ElementId> list2 = new List<ElementId>();
			for (int i = 1; i < numberOfMembers; i++)
			{
				ICollection<ElementId> collection = ElementTransformUtils.CopyElement(val, val2.Id, val3);
				if (collection != null && collection.Count > 0)
				{
					list2.AddRange(collection);
				}
			}
			foreach (ElementId item in list2)
			{
				Element element2 = val.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
			list.Add(val2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ArrayElement: 成功创建 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个阵列元素");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			LogError("ArrayElement 失败: " + ex.Message);
		}
		return list;
	}

	public object? MirrorElement(object document, object element, double planeNormalX, double planeNormalY, double planeNormalZ, double planeOriginX, double planeOriginY, double planeOriginZ)
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0058: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MirrorElement: document 不是 Document 类型");
				return null;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("MirrorElement: element 不是 Element 类型");
				return null;
			}
			XYZ val3 = new XYZ(planeNormalX, planeNormalY, planeNormalZ);
			XYZ val4 = new XYZ(planeOriginX, planeOriginY, planeOriginZ);
			if (val3.IsZeroLength())
			{
				LogError("MirrorElement: 镜像平面法线为零向量");
				return null;
			}
			Plane val5 = Plane.CreateByNormalAndOrigin(val3, val4);
			ElementTransformUtils.MirrorElement(val, val2.Id, val5);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler.AppendLiteral("MirrorElement: 成功镜像元素 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogError("MirrorElement 失败: " + ex.Message);
			return null;
		}
	}

	public bool TrimExtendElement(object document, object element, string operation, object? targetElement = null)
	{
		try
		{
			LogError("TrimExtendElement: 方法尚未实现");
			return false;
		}
		catch (Exception ex)
		{
			LogError("TrimExtendElement 失败: " + ex.Message);
			return false;
		}
	}

	public SplitResult SplitElementAdvanced(object document, object element, string splitMode, IEnumerable<(double X, double Y, double Z)>? splitPoints = null, double splitLength = 0.0, IEnumerable<double>? splitParameters = null, bool autoCreateFittings = true)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		//IL_0223: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Expected O, but got Unknown
		SplitResult val = new SplitResult();
		try
		{
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				val.Error = "document 不是 Document 类型";
				return val;
			}
			Element val3 = (Element)((element is Element) ? element : null);
			if (val3 == null)
			{
				val.Error = "element 不是 Element 类型";
				return val;
			}
			if (string.IsNullOrEmpty(splitMode))
			{
				val.Error = "splitMode 不能为空";
				return val;
			}
			Curve elementCurve = GetElementCurve(val3);
			if (((GeometryObject)(object)elementCurve == (GeometryObject)null || !elementCurve.IsBound) && !(val3 is FamilyInstance))
			{
				val.Error = "无法获取元素曲线，此元素可能不支持拆分";
				return val;
			}
			List<XYZ> list = null;
			string text = splitMode.Trim().ToLowerInvariant();
			string text2 = text;
			string text3 = text2;
			if (!(text3 == "bylength"))
			{
				if (!(text3 == "bypoints"))
				{
					if (!(text3 == "byparameters"))
					{
						val.Error = "不支持的拆分模式: " + splitMode;
						return val;
					}
					if (splitParameters == null)
					{
						val.Error = "byParameters 模式需要 splitParameters 参数";
						return val;
					}
					if ((GeometryObject)(object)elementCurve == (GeometryObject)null)
					{
						val.Error = "byParameters 模式要求元素具有曲线";
						return val;
					}
					list = new List<XYZ>();
					foreach (double splitParameter in splitParameters)
					{
						if (splitParameter > 0.0 && splitParameter < 1.0)
						{
							list.Add(elementCurve.Evaluate(splitParameter, true));
						}
					}
				}
				else
				{
					if (splitPoints == null)
					{
						val.Error = "byPoints 模式需要 splitPoints 参数";
						return val;
					}
					list = new List<XYZ>();
					foreach (var splitPoint in splitPoints)
					{
						list.Add(new XYZ(splitPoint.X, splitPoint.Y, splitPoint.Z));
					}
				}
			}
			else
			{
				if (splitLength <= 0.0)
				{
					val.Error = "byLength 模式需要 splitLength > 0（英尺）";
					return val;
				}
				if ((GeometryObject)(object)elementCurve == (GeometryObject)null)
				{
					val.Error = "byLength 模式要求元素具有曲线";
					return val;
				}
				list = CalculateSplitPointsByLength(elementCurve, splitLength);
			}
			if (list == null || list.Count == 0)
			{
				val.Error = "没有计算出有效的拆分点";
				return val;
			}
			Pipe val4 = (Pipe)(object)((val3 is Pipe) ? val3 : null);
			if (val4 != null)
			{
				return SplitPipe(val2, val4, list, autoCreateFittings);
			}
			Duct val5 = (Duct)(object)((val3 is Duct) ? val3 : null);
			if (val5 != null)
			{
				return SplitDuct(val2, val5, list, autoCreateFittings);
			}
			FamilyInstance val6 = (FamilyInstance)(object)((val3 is FamilyInstance) ? val3 : null);
			object obj;
			if (val6 != null)
			{
				if (!(((Element)val6).Location is LocationCurve))
				{
					Category category = val3.Category;
					if (category == null)
					{
						obj = null;
					}
					else
					{
						obj = category.Name;
						if (obj != null)
						{
							goto IL_0350;
						}
					}
					obj = "结构构件";
					goto IL_0350;
				}
				return SplitFamilyInstance(val2, val6, list);
			}
			string text4 = "类别「";
			Category category2 = val3.Category;
			object obj2;
			if (category2 == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = category2.Name;
				if (obj2 != null)
				{
					goto IL_03f4;
				}
			}
			obj2 = ((object)val3).GetType().Name;
			goto IL_03f4;
			IL_0350:
			string text5 = (string)obj;
			if (text5.Contains("结构柱") || text5.Contains("柱"))
			{
				val.SkippedReason = "垂直结构柱（基于标高放置）不支持拆分。仅支持「斜柱」（两点放置）和「结构框架/梁」的拆分。";
				return val;
			}
			val.SkippedReason = "此" + text5 + "没有线性几何，不支持拆分（可能需要使用两点放置的族）";
			return val;
			IL_03f4:
			val.SkippedReason = text4 + (string?)obj2 + "」暂不支持拆分（墙等暂未实现）";
			return val;
		}
		catch (Exception ex)
		{
			LogError("SplitElementAdvanced 失败: " + ex.Message);
			val.Error = ex.Message;
			return val;
		}
	}

	private static Curve? GetElementCurve(Element element)
	{
		try
		{
			Location location = element.Location;
			LocationCurve val = (LocationCurve)(object)((location is LocationCurve) ? location : null);
			if (val != null && (GeometryObject)(object)val.Curve != (GeometryObject)null)
			{
				return val.Curve;
			}
			return null;
		}
		catch
		{
			return null;
		}
	}

	private static List<XYZ> CalculateSplitPointsByLength(Curve curve, double segmentLength)
	{
		List<XYZ> list = new List<XYZ>();
		double length = curve.Length;
		if (length <= segmentLength || segmentLength <= 0.0)
		{
			return list;
		}
		int num = (int)Math.Floor(length / segmentLength);
		for (int i = 1; i <= num; i++)
		{
			double num2 = (double)i * segmentLength;
			if (num2 < length)
			{
				list.Add(curve.Evaluate(num2, false));
			}
		}
		return list;
	}

	private static List<XYZ> OrderPointsAlongCurve(Curve curve, IEnumerable<XYZ> points)
	{
		List<(XYZ, double)> list = new List<(XYZ, double)>();
		foreach (XYZ point in points)
		{
			try
			{
				IntersectionResult val = curve.Project(point);
				if (val != null && val.XYZPoint != null)
				{
					XYZ xYZPoint = val.XYZPoint;
					XYZ endPoint = curve.GetEndPoint(0);
					double item = endPoint.DistanceTo(xYZPoint);
					list.Add((xYZPoint, item));
				}
			}
			catch
			{
			}
		}
		return (from x in list
			orderby x.Item2
			select x.Item1).ToList();
	}

	private static Connector? FindConnectorAtPoint(Element element, XYZ point, double tolerance = 0.0032808398950131233)
	{
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		try
		{
			ConnectorSet val = null;
			Pipe val2 = (Pipe)(object)((element is Pipe) ? element : null);
			if (val2 != null)
			{
				ConnectorManager connectorManager = ((MEPCurve)val2).ConnectorManager;
				val = ((connectorManager != null) ? connectorManager.Connectors : null);
			}
			else
			{
				Duct val3 = (Duct)(object)((element is Duct) ? element : null);
				if (val3 != null)
				{
					ConnectorManager connectorManager2 = ((MEPCurve)val3).ConnectorManager;
					val = ((connectorManager2 != null) ? connectorManager2.Connectors : null);
				}
				else
				{
					FamilyInstance val4 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
					if (val4 != null)
					{
						MEPModel mEPModel = val4.MEPModel;
						ConnectorManager val5 = ((mEPModel != null) ? mEPModel.ConnectorManager : null);
						val = ((val5 != null) ? val5.Connectors : null);
					}
				}
			}
			if (val == null)
			{
				return null;
			}
			Connector val6 = null;
			double num = double.MaxValue;
			foreach (Connector item in val)
			{
				Connector val7 = item;
				if (((val7 != null) ? val7.Origin : null) != null)
				{
					double num2 = val7.Origin.DistanceTo(point);
					if (num2 < num)
					{
						num = num2;
						val6 = val7;
					}
				}
			}
			return (val6 == null || num > tolerance) ? null : val6;
		}
		catch
		{
			return null;
		}
	}

	private SplitResult SplitPipe(Document doc, Pipe pipe, List<XYZ> splitPoints, bool autoCreateFittings)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		SplitResult val = new SplitResult();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		try
		{
			Curve elementCurve = GetElementCurve((Element)(object)pipe);
			if ((GeometryObject)(object)elementCurve == (GeometryObject)null)
			{
				val.Error = "无法获取管道曲线";
				return val;
			}
			List<(XYZ, double)> list3 = new List<(XYZ, double)>();
			foreach (XYZ splitPoint in splitPoints)
			{
				try
				{
					IntersectionResult val2 = elementCurve.Project(splitPoint);
					if (val2 != null && val2.XYZPoint != null)
					{
						XYZ endPoint = elementCurve.GetEndPoint(0);
						double item = endPoint.DistanceTo(val2.XYZPoint);
						list3.Add((val2.XYZPoint, item));
					}
				}
				catch
				{
				}
			}
			list3 = list3.OrderBy<(XYZ, double), double>(((XYZ point, double distance) x) => x.distance).ToList();
			Pipe val3 = pipe;
			double num = elementCurve.Length;
			for (int num2 = list3.Count - 1; num2 >= 0; num2--)
			{
				try
				{
					(XYZ, double) tuple = list3[num2];
					_ = tuple.Item1;
					double item2 = tuple.Item2;
					double num3 = num - item2;
					if (!(num3 <= 1E-06))
					{
						Curve elementCurve2 = GetElementCurve((Element)(object)val3);
						if ((GeometryObject)(object)elementCurve2 == (GeometryObject)null)
						{
							LogWarning("SplitPipe: 无法获取当前管道段曲线");
							break;
						}
						double length = elementCurve2.Length;
						if (num3 > length + 1E-06)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
							defaultInterpolatedStringHandler.AppendLiteral("SplitPipe: 相对距离 ");
							defaultInterpolatedStringHandler.AppendFormatted(num3, "F4");
							defaultInterpolatedStringHandler.AppendLiteral(" 超出当前段长度 ");
							defaultInterpolatedStringHandler.AppendFormatted(length, "F4");
							LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							double num4 = length - num3;
							XYZ val4 = elementCurve2.Evaluate(num4, false);
							ElementId val5 = PlumbingUtils.BreakCurve(doc, ((Element)val3).Id, val4);
							if (val5 == (ElementId)null || val5 == ElementId.InvalidElementId)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("SplitPipe: 在距离起点 ");
								defaultInterpolatedStringHandler2.AppendFormatted(num4, "F4");
								defaultInterpolatedStringHandler2.AppendLiteral(" 处 BreakCurve 返回无效ID");
								LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
							else
							{
								Element element = doc.GetElement(val5);
								Pipe val6 = (Pipe)(object)((element is Pipe) ? element : null);
								if (val6 == null)
								{
									LogWarning("SplitPipe: 无法获取新创建的管道");
								}
								else
								{
									list.Add((int)val5.Value);
									if (autoCreateFittings)
									{
										Connector val7 = FindConnectorAtPoint((Element)(object)val3, val4);
										Connector val8 = FindConnectorAtPoint((Element)(object)val6, val4);
										if (val7 != null && val8 != null)
										{
											try
											{
												FamilyInstance val9 = doc.Create.NewUnionFitting(val7, val8);
												if (val9 != null)
												{
													list2.Add((int)((Element)val9).Id.Value);
												}
											}
											catch (Exception ex)
											{
												LogWarning("SplitPipe: 创建管件失败 - " + ex.Message);
											}
										}
									}
									int segmentCount = val.SegmentCount;
									val.SegmentCount = segmentCount + 1;
									val3 = val6;
									num = item2;
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					LogError("SplitPipe: 拆分点处理失败 - " + ex2.Message);
				}
			}
			val.NewElementIds = list;
			val.CreatedFittingIds = list2;
			val.Success = val.SegmentCount > 0;
			if (!val.Success && val.Error == null)
			{
				val.Error = "管道未在任何位置成功拆分";
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 3);
			defaultInterpolatedStringHandler3.AppendLiteral("SplitPipe: 管道 ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)pipe).Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" 拆分完成，段数 ");
			defaultInterpolatedStringHandler3.AppendFormatted(val.SegmentCount);
			defaultInterpolatedStringHandler3.AppendLiteral("，管件 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		catch (Exception ex3)
		{
			LogError("SplitPipe 失败: " + ex3.Message);
			val.Error = ex3.Message;
		}
		return val;
	}

	private SplitResult SplitDuct(Document doc, Duct duct, List<XYZ> splitPoints, bool autoCreateFittings)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		SplitResult val = new SplitResult();
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		try
		{
			Curve elementCurve = GetElementCurve((Element)(object)duct);
			if ((GeometryObject)(object)elementCurve == (GeometryObject)null)
			{
				val.Error = "无法获取风管曲线";
				return val;
			}
			List<(XYZ, double)> list3 = new List<(XYZ, double)>();
			foreach (XYZ splitPoint in splitPoints)
			{
				try
				{
					IntersectionResult val2 = elementCurve.Project(splitPoint);
					if (val2 != null && val2.XYZPoint != null)
					{
						XYZ endPoint = elementCurve.GetEndPoint(0);
						double item = endPoint.DistanceTo(val2.XYZPoint);
						list3.Add((val2.XYZPoint, item));
					}
				}
				catch
				{
				}
			}
			list3 = list3.OrderBy<(XYZ, double), double>(((XYZ point, double distance) x) => x.distance).ToList();
			Duct val3 = duct;
			double num = elementCurve.Length;
			for (int num2 = list3.Count - 1; num2 >= 0; num2--)
			{
				try
				{
					(XYZ, double) tuple = list3[num2];
					_ = tuple.Item1;
					double item2 = tuple.Item2;
					double num3 = num - item2;
					if (!(num3 <= 1E-06))
					{
						Curve elementCurve2 = GetElementCurve((Element)(object)val3);
						if ((GeometryObject)(object)elementCurve2 == (GeometryObject)null)
						{
							LogWarning("SplitDuct: 无法获取当前风管段曲线");
							break;
						}
						double length = elementCurve2.Length;
						if (num3 > length + 1E-06)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
							defaultInterpolatedStringHandler.AppendLiteral("SplitDuct: 相对距离 ");
							defaultInterpolatedStringHandler.AppendFormatted(num3, "F4");
							defaultInterpolatedStringHandler.AppendLiteral(" 超出当前段长度 ");
							defaultInterpolatedStringHandler.AppendFormatted(length, "F4");
							LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
						}
						else
						{
							double num4 = length - num3;
							XYZ val4 = elementCurve2.Evaluate(num4, false);
							ElementId val5 = MechanicalUtils.BreakCurve(doc, ((Element)val3).Id, val4);
							if (val5 == (ElementId)null || val5 == ElementId.InvalidElementId)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 1);
								defaultInterpolatedStringHandler2.AppendLiteral("SplitDuct: 在距离起点 ");
								defaultInterpolatedStringHandler2.AppendFormatted(num4, "F4");
								defaultInterpolatedStringHandler2.AppendLiteral(" 处 BreakCurve 返回无效ID");
								LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
							else
							{
								Element element = doc.GetElement(val5);
								Duct val6 = (Duct)(object)((element is Duct) ? element : null);
								if (val6 == null)
								{
									LogWarning("SplitDuct: 无法获取新创建的风管");
								}
								else
								{
									list.Add((int)val5.Value);
									if (autoCreateFittings)
									{
										Connector val7 = FindConnectorAtPoint((Element)(object)val3, val4);
										Connector val8 = FindConnectorAtPoint((Element)(object)val6, val4);
										if (val7 != null && val8 != null)
										{
											try
											{
												FamilyInstance val9 = doc.Create.NewUnionFitting(val7, val8);
												if (val9 != null)
												{
													list2.Add((int)((Element)val9).Id.Value);
												}
											}
											catch (Exception ex)
											{
												LogWarning("SplitDuct: 创建管件失败 - " + ex.Message);
											}
										}
									}
									int segmentCount = val.SegmentCount;
									val.SegmentCount = segmentCount + 1;
									val3 = val6;
									num = item2;
								}
							}
						}
					}
				}
				catch (Exception ex2)
				{
					LogError("SplitDuct: 拆分点处理失败 - " + ex2.Message);
				}
			}
			val.NewElementIds = list;
			val.CreatedFittingIds = list2;
			val.Success = val.SegmentCount > 0;
			if (!val.Success && val.Error == null)
			{
				val.Error = "风管未在任何位置成功拆分";
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 3);
			defaultInterpolatedStringHandler3.AppendLiteral("SplitDuct: 风管 ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)duct).Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" 拆分完成，段数 ");
			defaultInterpolatedStringHandler3.AppendFormatted(val.SegmentCount);
			defaultInterpolatedStringHandler3.AppendLiteral("，管件 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		catch (Exception ex3)
		{
			LogError("SplitDuct 失败: " + ex3.Message);
			val.Error = ex3.Message;
		}
		return val;
	}

	private SplitResult SplitFamilyInstance(Document doc, FamilyInstance fi, List<XYZ> splitPoints)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Expected O, but got Unknown
		SplitResult val = new SplitResult();
		List<int> list = new List<int>();
		try
		{
			Curve elementCurve = GetElementCurve((Element)(object)fi);
			if ((GeometryObject)(object)elementCurve == (GeometryObject)null)
			{
				val.Error = "无法获取族实例曲线，此元素可能不支持拆分";
				return val;
			}
			List<double> list2 = new List<double>();
			XYZ endPoint = elementCurve.GetEndPoint(0);
			XYZ endPoint2 = elementCurve.GetEndPoint(1);
			double num = endPoint.DistanceTo(endPoint2);
			if (num <= 0.0)
			{
				val.Error = "曲线长度为 0，无法拆分";
				return val;
			}
			foreach (XYZ splitPoint in splitPoints)
			{
				try
				{
					IntersectionResult val2 = elementCurve.Project(splitPoint);
					if (((val2 != null) ? val2.XYZPoint : null) != null)
					{
						XYZ xYZPoint = val2.XYZPoint;
						double num2 = endPoint.DistanceTo(xYZPoint);
						double num3 = num2 / num;
						if (num3 > 0.001 && num3 < 0.999)
						{
							list2.Add(num3);
						}
					}
				}
				catch
				{
				}
			}
			if (list2.Count == 0)
			{
				val.Error = "拆分点投影后没有有效的归一化参数";
				return val;
			}
			list2.Sort();
			double num4 = 1.0;
			for (int num5 = list2.Count - 1; num5 >= 0; num5--)
			{
				try
				{
					double num6 = list2[num5];
					double num7 = num6 / num4;
					ElementId val3 = fi.Split(num7);
					if (val3 == (ElementId)null || val3 == ElementId.InvalidElementId)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
						defaultInterpolatedStringHandler.AppendLiteral("SplitFamilyInstance: 在参数 ");
						defaultInterpolatedStringHandler.AppendFormatted(num6, "F4");
						defaultInterpolatedStringHandler.AppendLiteral(" (相对 ");
						defaultInterpolatedStringHandler.AppendFormatted(num7, "F4");
						defaultInterpolatedStringHandler.AppendLiteral(") 处拆分返回无效ID");
						LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					else
					{
						list.Add((int)val3.Value);
						int segmentCount = val.SegmentCount;
						val.SegmentCount = segmentCount + 1;
						num4 = num6;
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("SplitFamilyInstance: 在参数 ");
					defaultInterpolatedStringHandler2.AppendFormatted(list2[num5], "F4");
					defaultInterpolatedStringHandler2.AppendLiteral(" 处拆分失败 - ");
					defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			val.NewElementIds = list;
			val.Success = val.SegmentCount > 0;
			if (!val.Success && val.Error == null)
			{
				val.Error = "族实例未在任何位置成功拆分";
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("SplitFamilyInstance: 族实例 ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(((Element)fi).Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" 拆分完成，段数 ");
			defaultInterpolatedStringHandler3.AppendFormatted(val.SegmentCount);
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		catch (Exception ex2)
		{
			LogError("SplitFamilyInstance 失败: " + ex2.Message);
			val.Error = ex2.Message;
		}
		return val;
	}

	public bool JoinElements(object document, object element1, object element2, string joinType = "Join")
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("JoinElements: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((element1 is Element) ? element1 : null);
			if (val2 == null)
			{
				LogError("JoinElements: element1 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((element2 is Element) ? element2 : null);
			if (val3 == null)
			{
				LogError("JoinElements: element2 不是 Element 类型");
				return false;
			}
			if (joinType.Equals("Join", StringComparison.OrdinalIgnoreCase))
			{
				JoinGeometryUtils.JoinGeometry(val, val2, val3);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler.AppendLiteral("JoinElements: 成功连接几何（元素1: ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
				defaultInterpolatedStringHandler.AppendLiteral(", 元素2: ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val3.Id);
				defaultInterpolatedStringHandler.AppendLiteral("）");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return true;
			}
			if (joinType.Equals("Unjoin", StringComparison.OrdinalIgnoreCase))
			{
				JoinGeometryUtils.UnjoinGeometry(val, val2, val3);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("JoinElements: 成功取消连接几何（元素1: ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val2.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(", 元素2: ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val3.Id);
				defaultInterpolatedStringHandler2.AppendLiteral("）");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				return true;
			}
			LogError("JoinElements: 不支持的连接类型 '" + joinType + "'，请使用 'Join' 或 'Unjoin'");
			return false;
		}
		catch (Exception ex)
		{
			LogError("JoinElements 失败: " + ex.Message);
			return false;
		}
	}

	public object? GroupElements(object document, IEnumerable<int> elementIds)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GroupElements: document 不是 Document 类型");
				return null;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogError("GroupElements: 元素 ID 列表为空");
				return null;
			}
			Group val2 = ((ItemFactoryBase)val.Create).NewGroup((ICollection<ElementId>)list);
			if (val2 == null)
			{
				LogError("GroupElements: 创建组失败");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GroupElements: 成功创建组，包含 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogError("GroupElements 失败: " + ex.Message);
			return null;
		}
	}

	public object? GroupElements(object document, IEnumerable<int> elementIds, string? groupName = null)
	{
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GroupElements: document 不是 Document 类型");
				return null;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogError("GroupElements: 元素 ID 列表为空");
				return null;
			}
			Group val2 = ((ItemFactoryBase)val.Create).NewGroup((ICollection<ElementId>)list);
			if (val2 == null)
			{
				LogError("GroupElements: 创建组失败");
				return null;
			}
			if (!string.IsNullOrWhiteSpace(groupName))
			{
				try
				{
					GroupType val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(GroupType))).Cast<GroupType>().FirstOrDefault((GroupType gt) => ((Element)gt).Name == groupName);
					if (val3 != null && ((Element)val3).Id != ((Element)val2.GroupType).Id)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
						defaultInterpolatedStringHandler.AppendFormatted(groupName);
						defaultInterpolatedStringHandler.AppendLiteral("_");
						defaultInterpolatedStringHandler.AppendFormatted(DateTime.Now, "yyyyMMdd_HHmmss");
						string text = defaultInterpolatedStringHandler.ToStringAndClear();
						((Element)val2.GroupType).Name = text;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("GroupElements: 组名称 '");
						defaultInterpolatedStringHandler2.AppendFormatted(groupName);
						defaultInterpolatedStringHandler2.AppendLiteral("' 已存在，使用 '");
						defaultInterpolatedStringHandler2.AppendFormatted(text);
						defaultInterpolatedStringHandler2.AppendLiteral("'");
						LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
					else
					{
						((Element)val2.GroupType).Name = groupName;
					}
				}
				catch (Exception ex)
				{
					LogWarning("GroupElements: 无法设置组名称 '" + groupName + "': " + ex.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("GroupElements: 成功创建组 '");
			defaultInterpolatedStringHandler3.AppendFormatted(groupName ?? "未命名");
			defaultInterpolatedStringHandler3.AppendLiteral("'，包含 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return val2;
		}
		catch (Exception ex2)
		{
			LogError("GroupElements 失败: " + ex2.Message);
			return null;
		}
	}

	public IEnumerable<int> UngroupElements(object document, int groupId)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("UngroupElements: document 不是 Document 类型");
				return Enumerable.Empty<int>();
			}
			Element element = val.GetElement(new ElementId((long)groupId));
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("UngroupElements: 找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(groupId);
				defaultInterpolatedStringHandler.AppendLiteral(" 的组元素");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return Enumerable.Empty<int>();
			}
			Group val2 = (Group)(object)((element is Group) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("UngroupElements: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(groupId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 不是组类型");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return Enumerable.Empty<int>();
			}
			(from id in val2.GetMemberIds()
				select (int)id.Value).ToList();
			ICollection<ElementId> collection = val2.UngroupMembers();
			if (collection == null || collection.Count == 0)
			{
				LogWarning("UngroupElements: 解组操作未返回任何元素");
				return Enumerable.Empty<int>();
			}
			List<int> list = collection.Select((ElementId id) => (int)id.Value).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("UngroupElements: 成功解组，释放 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("UngroupElements 失败: " + ex.Message);
			return Enumerable.Empty<int>();
		}
	}

	public int MoveElements(object document, IEnumerable<int> elementIds, double moveX, double moveY, double moveZ)
	{
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MoveElements: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogWarning("MoveElements: 元素 ID 列表为空");
				return 0;
			}
			XYZ val2 = new XYZ(moveX, moveY, moveZ);
			ElementTransformUtils.MoveElements(val, (ICollection<ElementId>)list, val2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 4);
			defaultInterpolatedStringHandler.AppendLiteral("MoveElements: 成功移动 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素 (位移: ");
			defaultInterpolatedStringHandler.AppendFormatted(moveX, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(moveY, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(moveZ, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(" 英尺)");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list.Count;
		}
		catch (Exception ex)
		{
			LogError("MoveElements 失败: " + ex.Message);
			return 0;
		}
	}

	public int RotateElements(object document, IEnumerable<int> elementIds, double axisX, double axisY, double axisZ, double angleDegrees, double originX = 0.0, double originY = 0.0, double originZ = 0.0)
	{
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RotateElements: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogWarning("RotateElements: 元素 ID 列表为空");
				return 0;
			}
			XYZ val2 = XYZ.BasisX * axisX + XYZ.BasisY * axisY + XYZ.BasisZ * axisZ;
			XYZ val3 = new XYZ(originX, originY, originZ);
			Line val4 = Line.CreateUnbound(val3, val2);
			double num = angleDegrees * Math.PI / 180.0;
			ElementTransformUtils.RotateElements(val, (ICollection<ElementId>)list, val4, num);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 8);
			defaultInterpolatedStringHandler.AppendLiteral("RotateElements: 成功旋转 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素 (中心: (");
			defaultInterpolatedStringHandler.AppendFormatted(originX, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(originY, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(originZ, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("), 轴: (");
			defaultInterpolatedStringHandler.AppendFormatted(axisX);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(axisY);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(axisZ);
			defaultInterpolatedStringHandler.AppendLiteral("), 角度: ");
			defaultInterpolatedStringHandler.AppendFormatted(angleDegrees);
			defaultInterpolatedStringHandler.AppendLiteral("°)");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list.Count;
		}
		catch (Exception ex)
		{
			LogError("RotateElements 失败: " + ex.Message);
			return 0;
		}
	}

	public MirrorResult MirrorElements(object document, IEnumerable<int> elementIds, double planeOriginX, double planeOriginY, double planeOriginZ, double planeNormalX, double planeNormalY, double planeNormalZ, bool mirrorCopies = false)
	{
		//IL_0197: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MirrorElements: document 不是 Document 类型");
				return new MirrorResult
				{
					Success = false,
					MirrorElementIds = Enumerable.Empty<int>()
				};
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogWarning("MirrorElements: 元素 ID 列表为空");
				return new MirrorResult
				{
					Success = false,
					MirrorElementIds = Enumerable.Empty<int>()
				};
			}
			XYZ val2 = new XYZ(planeOriginX, planeOriginY, planeOriginZ);
			XYZ val3 = new XYZ(planeNormalX, planeNormalY, planeNormalZ);
			Plane val4 = Plane.CreateByNormalAndOrigin(val3, val2);
			IList<ElementId> source = ElementTransformUtils.MirrorElements(val, (ICollection<ElementId>)list, val4, mirrorCopies);
			List<int> list2 = source.Select((ElementId id) => (int)id.Value).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(42, 2);
			defaultInterpolatedStringHandler.AppendLiteral("MirrorElements: 成功镜像 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素 (mirrorCopies: ");
			defaultInterpolatedStringHandler.AppendFormatted(mirrorCopies);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return new MirrorResult
			{
				Success = true,
				MirrorElementIds = list2,
				IsMirrorCopy = mirrorCopies
			};
		}
		catch (Exception ex)
		{
			LogError("MirrorElements 失败: " + ex.Message);
			return new MirrorResult
			{
				Success = false,
				MirrorElementIds = Enumerable.Empty<int>()
			};
		}
	}

	public IEnumerable<object> CopyElements(object document, IEnumerable<int> elementIds, double copyX, double copyY, double copyZ)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (list.Count == 0)
			{
				LogWarning("CopyElements: 元素 ID 列表为空");
				return Enumerable.Empty<object>();
			}
			XYZ val2 = XYZ.BasisX * copyX + XYZ.BasisY * copyY + XYZ.BasisZ * copyZ;
			ICollection<ElementId> collection = ElementTransformUtils.CopyElements(val, (ICollection<ElementId>)list, val2);
			List<object> list2 = new List<object>();
			foreach (ElementId item in collection)
			{
				Element element = val.GetElement(item);
				if (element != null)
				{
					list2.Add(element);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 4);
			defaultInterpolatedStringHandler.AppendLiteral("CopyElements: 成功复制 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素 (位移: (");
			defaultInterpolatedStringHandler.AppendFormatted(copyX, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(copyY, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(copyZ, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(") 英尺)");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list2;
		}
		catch (Exception ex)
		{
			LogError("CopyElements 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public int DeleteElements(object document, IEnumerable<int> elementIds)
	{
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteElements: document 不是 Document 类型");
				return 0;
			}
			List<int> list = elementIds.ToList();
			if (list.Count == 0)
			{
				LogError("DeleteElements: 元素 ID 列表为空");
				return 0;
			}
			int num = 0;
			List<int> list2 = new List<int>();
			foreach (int item in list)
			{
				try
				{
					ElementId val2 = new ElementId((long)item);
					val.Delete(val2);
					num++;
				}
				catch (Exception ex)
				{
					list2.Add(item);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除元素 ID ");
					defaultInterpolatedStringHandler.AppendFormatted(item);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			if (list2.Count > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("DeleteElements: 成功删除 ");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个元素，");
				defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个元素删除失败 (ID: ");
				defaultInterpolatedStringHandler2.AppendFormatted(string.Join(", ", list2));
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("DeleteElements: 成功删除 ");
				defaultInterpolatedStringHandler3.AppendFormatted(num);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个元素");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			return num;
		}
		catch (Exception ex2)
		{
			LogError("DeleteElements 失败: " + ex2.Message);
			return 0;
		}
	}

	public int ChangeElementTypes(object document, IEnumerable<int> elementIds, int newTypeId)
	{
		//IL_0260: Expected O, but got Unknown
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ChangeElementTypes: document 不是 Document 类型");
				return 0;
			}
			Element element = val.GetElement(new ElementId((long)newTypeId));
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ChangeElementTypes: 找不到新类型 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(newTypeId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return 0;
			}
			if (!(element is ElementType))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("ChangeElementTypes: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(newTypeId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 不是有效的类型");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return 0;
			}
			int num = 0;
			int num2 = 0;
			List<int> list = elementIds.ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(34, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("ChangeElementTypes: 开始更改 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个元素的类型为 ");
			defaultInterpolatedStringHandler3.AppendFormatted(newTypeId);
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			foreach (int item in list)
			{
				try
				{
					Element element2 = val.GetElement(new ElementId((long)item));
					if (element2 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("ChangeElementTypes: 找不到元素 ID ");
						defaultInterpolatedStringHandler4.AppendFormatted(item);
						LogWarning(defaultInterpolatedStringHandler4.ToStringAndClear());
						num2++;
						continue;
					}
					if (element2 == null)
					{
						goto IL_0199;
					}
					Element val2 = element2;
					if (false)
					{
						goto IL_0199;
					}
					ElementId typeId = val2.GetTypeId();
					if (typeId == (ElementId)null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler5.AppendLiteral("ChangeElementTypes: 元素 ");
						defaultInterpolatedStringHandler5.AppendFormatted(item);
						defaultInterpolatedStringHandler5.AppendLiteral(" 没有类型 ID，可能无法更改类型");
						LogWarning(defaultInterpolatedStringHandler5.ToStringAndClear());
						num2++;
					}
					else
					{
						val2.ChangeTypeId(new ElementId((long)newTypeId));
						num++;
					}
					goto end_IL_0139;
					IL_0199:
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(37, 1);
					defaultInterpolatedStringHandler6.AppendLiteral("ChangeElementTypes: 元素 ");
					defaultInterpolatedStringHandler6.AppendFormatted(item);
					defaultInterpolatedStringHandler6.AppendLiteral(" 不是 Element 类型");
					LogWarning(defaultInterpolatedStringHandler6.ToStringAndClear());
					num2++;
					end_IL_0139:;
				}
				catch (InvalidOperationException ex)
				{
					InvalidOperationException ex2 = ex;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler7.AppendLiteral("ChangeElementTypes: 元素 ");
					defaultInterpolatedStringHandler7.AppendFormatted(item);
					defaultInterpolatedStringHandler7.AppendLiteral(" 无法更改类型 - ");
					defaultInterpolatedStringHandler7.AppendFormatted(((Exception)(object)ex2).Message);
					LogWarning(defaultInterpolatedStringHandler7.ToStringAndClear());
					num2++;
				}
				catch (Exception ex3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler8.AppendLiteral("ChangeElementTypes: 更改元素 ");
					defaultInterpolatedStringHandler8.AppendFormatted(item);
					defaultInterpolatedStringHandler8.AppendLiteral(" 类型失败 - ");
					defaultInterpolatedStringHandler8.AppendFormatted(ex3.Message);
					LogError(defaultInterpolatedStringHandler8.ToStringAndClear());
					num2++;
				}
			}
			if (num2 > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(38, 2);
				defaultInterpolatedStringHandler9.AppendLiteral("ChangeElementTypes: 成功更改 ");
				defaultInterpolatedStringHandler9.AppendFormatted(num);
				defaultInterpolatedStringHandler9.AppendLiteral(" 个元素类型，");
				defaultInterpolatedStringHandler9.AppendFormatted(num2);
				defaultInterpolatedStringHandler9.AppendLiteral(" 个元素失败");
				LogWarning(defaultInterpolatedStringHandler9.ToStringAndClear());
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler10.AppendLiteral("ChangeElementTypes: 成功更改 ");
				defaultInterpolatedStringHandler10.AppendFormatted(num);
				defaultInterpolatedStringHandler10.AppendLiteral(" 个元素类型");
				LogInfo(defaultInterpolatedStringHandler10.ToStringAndClear());
			}
			return num;
		}
		catch (Exception ex4)
		{
			LogError("ChangeElementTypes 失败: " + ex4.Message);
			return 0;
		}
	}

	public IEnumerable<object> ArrayElementLinear(object document, object element, int numberOfMembers, double moveX, double moveY, double moveZ)
	{
		//IL_016b: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		List<object> list = new List<object>();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ArrayElementLinear: document 不是 Document 类型");
				return list;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("ArrayElementLinear: element 不是 Element 类型");
				return list;
			}
			if (numberOfMembers < 2)
			{
				LogError("ArrayElementLinear: 阵列成员数量必须大于 1");
				return list;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				LogError("ArrayElementLinear: 无法获取当前视图");
				return list;
			}
			XYZ val3 = new XYZ(moveX, moveY, moveZ);
			ICollection<ElementId> collection = LinearArray.ArrayElementWithoutAssociation(val, activeView, val2.Id, numberOfMembers, val3, (ArrayAnchorMember)0);
			if (collection == null || collection.Count == 0)
			{
				LogError("ArrayElementLinear: 创建阵列失败，返回空列表");
				return list;
			}
			foreach (ElementId item in collection)
			{
				Element element2 = val.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ArrayElementLinear: 成功创建线性阵列，包含 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个成员");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (InvalidOperationException ex)
		{
			InvalidOperationException ex2 = ex;
			LogError("ArrayElementLinear: 此元素类型不支持阵列 - " + ((Exception)(object)ex2).Message);
		}
		catch (Exception ex3)
		{
			LogError("ArrayElementLinear 失败: " + ex3.Message);
		}
		return list;
	}

	public IEnumerable<object> ArrayElementRadial(object document, object element, int numberOfMembers, double axisX, double axisY, double axisZ, double originX, double originY, double originZ)
	{
		//IL_0190: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected O, but got Unknown
		List<object> list = new List<object>();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ArrayElementRadial: document 不是 Document 类型");
				return list;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("ArrayElementRadial: element 不是 Element 类型");
				return list;
			}
			if (numberOfMembers < 2)
			{
				LogError("ArrayElementRadial: 阵列成员数量必须大于 1");
				return list;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				LogError("ArrayElementRadial: 无法获取当前视图");
				return list;
			}
			XYZ val3 = new XYZ(originX, originY, originZ);
			XYZ val4 = new XYZ(axisX, axisY, axisZ);
			Line val5 = Line.CreateUnbound(val3, val4);
			double num = Math.PI * 2.0;
			ICollection<ElementId> collection = RadialArray.ArrayElementWithoutAssociation(val, activeView, val2.Id, numberOfMembers, val5, num, (ArrayAnchorMember)0);
			if (collection == null || collection.Count == 0)
			{
				LogError("ArrayElementRadial: 创建阵列失败，返回空列表");
				return list;
			}
			foreach (ElementId item in collection)
			{
				Element element2 = val.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ArrayElementRadial: 成功创建径向阵列，包含 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个成员");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (InvalidOperationException ex)
		{
			InvalidOperationException ex2 = ex;
			LogError("ArrayElementRadial: 此元素类型不支持阵列 - " + ((Exception)(object)ex2).Message);
		}
		catch (Exception ex3)
		{
			LogError("ArrayElementRadial 失败: " + ex3.Message);
		}
		return list;
	}

	private static void LogError(string message)
	{
		Logger.Error("[ModificationService] " + message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[ModificationService] " + message);
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[ModificationService] " + message);
	}

	public object? MirrorElementByLine(object document, object element, object line)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MirrorElementByLine: document 不是 Document 类型");
				return null;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("MirrorElementByLine: element 不是 Element 类型");
				return null;
			}
			Line val3 = (Line)((line is Line) ? line : null);
			if (val3 == null)
			{
				LogError("MirrorElementByLine: line 不是 Line 类型");
				return null;
			}
			Plane val4 = Plane.CreateByNormalAndOrigin(val3.Direction, val3.Origin);
			ElementTransformUtils.MirrorElement(val, val2.Id, val4);
			return val2;
		}
		catch (Exception ex)
		{
			LogError("MirrorElementByLine 失败: " + ex.Message);
			return null;
		}
	}

	public object? MirrorElementByPoint(object document, object element, object origin, object normal)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MirrorElementByPoint: document 不是 Document 类型");
				return null;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("MirrorElementByPoint: element 不是 Element 类型");
				return null;
			}
			if (!ParsePoint(origin, out XYZ xyz))
			{
				LogError("MirrorElementByPoint: 无法解析原点");
				return null;
			}
			if (!ParsePoint(normal, out XYZ xyz2))
			{
				LogError("MirrorElementByPoint: 无法解析法线");
				return null;
			}
			Plane val3 = Plane.CreateByNormalAndOrigin(xyz2, xyz);
			ElementTransformUtils.MirrorElement(val, val2.Id, val3);
			return val2;
		}
		catch (Exception ex)
		{
			LogError("MirrorElementByPoint 失败: " + ex.Message);
			return null;
		}
	}

	private bool ParsePoint(object pointObj, out XYZ? xyz)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		try
		{
			if (pointObj == null)
			{
				xyz = null;
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (pointObj is IDictionary<string, object> dictionary)
			{
				if (dictionary.TryGetValue("x", out var value) && value != null)
				{
					num = Convert.ToDouble(value);
					flag = true;
				}
				if (dictionary.TryGetValue("y", out var value2) && value2 != null)
				{
					num2 = Convert.ToDouble(value2);
					flag2 = true;
				}
				if (dictionary.TryGetValue("z", out var value3) && value3 != null)
				{
					num3 = Convert.ToDouble(value3);
					flag3 = true;
				}
				if (flag & flag2 & flag3)
				{
					xyz = new XYZ(num, num2, num3);
					return true;
				}
			}
			Type type = pointObj.GetType();
			PropertyInfo property = type.GetProperty("x");
			PropertyInfo property2 = type.GetProperty("y");
			PropertyInfo property3 = type.GetProperty("z");
			if (property != null && property2 != null && property3 != null)
			{
				num = Convert.ToDouble(property.GetValue(pointObj));
				num2 = Convert.ToDouble(property2.GetValue(pointObj));
				num3 = Convert.ToDouble(property3.GetValue(pointObj));
				xyz = new XYZ(num, num2, num3);
				return true;
			}
			xyz = null;
			return false;
		}
		catch
		{
			xyz = null;
			return false;
		}
	}

	public IEnumerable<object> DivideRoom(object document, object room, object startPoint, object endPoint, object? transaction = null)
	{
		try
		{
			SpatialElement val = (SpatialElement)((room is SpatialElement) ? room : null);
			if (val == null)
			{
				LogError("DivideRoom: room 参数不是 SpatialElement 类型");
				return new List<object>();
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				LogError("DivideRoom: document 不是 Document 类型");
				return new List<object>();
			}
			if (!ParsePoint(startPoint, out XYZ _))
			{
				LogError("DivideRoom: 无法解析起点");
				return new List<object>();
			}
			if (!ParsePoint(endPoint, out XYZ _))
			{
				LogError("DivideRoom: 无法解析终点");
				return new List<object>();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("DivideRoom: 房间分割功能尚未完全实现（房间 ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val).Id);
			defaultInterpolatedStringHandler.AppendLiteral("）");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return new List<object>();
		}
		catch (Exception ex)
		{
			LogError("DivideRoom 失败: " + ex.Message);
			return new List<object>();
		}
	}

	public object? MergeRooms(object document, IEnumerable<object> rooms, object? transaction = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("MergeRooms: document 不是 Document 类型");
				return null;
			}
			List<object> list = rooms.ToList();
			if (list.Count < 2)
			{
				LogError("MergeRooms: 至少需要两个房间才能合并");
				return null;
			}
			foreach (object item in list)
			{
				if (!(item is SpatialElement))
				{
					LogError("MergeRooms: rooms 列表中包含非 SpatialElement 类型的对象");
					return null;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
			defaultInterpolatedStringHandler.AppendLiteral("MergeRooms: 房间合并功能尚未完全实现（房间数量: ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral("）");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return null;
		}
		catch (Exception ex)
		{
			LogError("MergeRooms 失败: " + ex.Message);
			return null;
		}
	}

	public int PurgeUnusedFamilies(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedFamilies: document 不是 Document 类型");
				return 0;
			}
			if (!val.IsModifiable)
			{
				LogError("PurgeUnusedFamilies: 文档不在可修改状态或没有活动事务");
				return 0;
			}
			PerformanceAdviser performanceAdviser = PerformanceAdviser.GetPerformanceAdviser();
			List<PerformanceAdviserRuleId> list = performanceAdviser.GetAllRuleIds().ToList();
			Guid purgeGuid = Guid.Parse("e8c63650-70b7-435a-9010-ec97660c1bda");
			PerformanceAdviserRuleId val2 = list.Find((PerformanceAdviserRuleId r) => ((GuidEnum)r).Guid.Equals(purgeGuid));
			int num = 0;
			if (val2 != (PerformanceAdviserRuleId)null)
			{
				for (int num2 = 0; num2 < 5; num2++)
				{
					IList<FailureMessage> list2 = performanceAdviser.ExecuteRules(val, (IList<PerformanceAdviserRuleId>)new List<PerformanceAdviserRuleId> { val2 });
					List<ElementId> list3 = new List<ElementId>();
					foreach (FailureMessage item in list2)
					{
						try
						{
							ICollection<ElementId> failingElements = item.GetFailingElements();
							list3.AddRange(failingElements);
						}
						catch (Exception ex)
						{
							LogError("获取失败元素时出错: " + ex.Message);
						}
					}
					if (!list3.Any())
					{
						break;
					}
					int num3 = 0;
					try
					{
						val.Delete((ICollection<ElementId>)list3);
						num3 = list3.Count;
					}
					catch
					{
						foreach (ElementId item2 in list3)
						{
							try
							{
								val.Delete(item2);
								num3++;
							}
							catch (Exception ex2)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
								defaultInterpolatedStringHandler.AppendLiteral("删除元素 ");
								defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
								defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
								defaultInterpolatedStringHandler.AppendFormatted(ex2.Message);
								LogError(defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
					}
					num += num3;
					if (num3 == 0)
					{
						break;
					}
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedFamilies: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的族和族类型");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return num;
		}
		catch (Exception ex3)
		{
			LogError("PurgeUnusedFamilies 失败: " + ex3.Message);
			return 0;
		}
	}

	public int PurgeUnusedMaterials(object document)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedMaterials: document 不是 Document 类型");
				return 0;
			}
			HashSet<ElementId> first = new FilteredElementCollector(val).OfClass(typeof(Material)).ToElementIds().ToHashSet();
			HashSet<ElementId> hashSet = new HashSet<ElementId>();
			ICollection<ElementId> collection = new FilteredElementCollector(val).WhereElementIsNotElementType().ToElementIds();
			foreach (ElementId item in collection)
			{
				try
				{
					Element element = val.GetElement(item);
					if (element == null)
					{
						continue;
					}
					foreach (ElementId materialId in element.GetMaterialIds(false))
					{
						hashSet.Add(materialId);
					}
				}
				catch
				{
				}
			}
			List<ElementId> list = first.Except(hashSet).ToList();
			foreach (ElementId item2 in list)
			{
				try
				{
					val.Delete(item2);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除材质 ");
					defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedMaterials: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的材质");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedMaterials 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedMaterialAssets(object document)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedMaterialAssets: document 不是 Document 类型");
				return 0;
			}
			int num = 0;
			try
			{
				List<ElementId> list = new FilteredElementCollector(val).OfClass(typeof(AppearanceAssetElement)).ToElementIds().ToList();
				HashSet<ElementId> hashSet = new HashSet<ElementId>();
				IEnumerable<Material> enumerable = new FilteredElementCollector(val).OfClass(typeof(Material)).ToElements().Cast<Material>();
				foreach (Material item in enumerable)
				{
					try
					{
						if (item.AppearanceAssetId != (ElementId)null && item.AppearanceAssetId != ElementId.InvalidElementId)
						{
							hashSet.Add(item.AppearanceAssetId);
						}
					}
					catch
					{
					}
				}
				foreach (ElementId item2 in list)
				{
					if (!hashSet.Contains(item2))
					{
						try
						{
							val.Delete(item2);
							num++;
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler.AppendLiteral("删除外观资源 ");
							defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
							defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
							defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
							LogError(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(41, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedMaterialAssets: 清理了 ");
				defaultInterpolatedStringHandler2.AppendFormatted(num);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的外观资源");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			catch (Exception ex2)
			{
				LogError("清理外观资源失败: " + ex2.Message);
			}
			return num;
		}
		catch (Exception ex3)
		{
			LogError("PurgeUnusedMaterialAssets 失败: " + ex3.Message);
			return 0;
		}
	}

	public int PurgeUnusedViews(object document)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Invalid comparison between Unknown and I4
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Invalid comparison between Unknown and I4
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedViews: document 不是 Document 类型");
				return 0;
			}
			List<View> list = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(View))).OfType<View>().ToList();
			List<ElementId> list2 = new List<ElementId>();
			foreach (View item in list)
			{
				if (item.IsTemplate || (int)item.ViewType == 5 || (int)item.ViewType == 122)
				{
					continue;
				}
				bool flag = false;
				bool flag2 = false;
				((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(View))).OfType<View>().ToList();
				List<ViewSheet> list3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ViewSheet))).OfType<ViewSheet>().ToList();
				foreach (ViewSheet item2 in list3)
				{
					try
					{
						ISet<ElementId> allPlacedViews = item2.GetAllPlacedViews();
						if (allPlacedViews.Contains(((Element)item).Id))
						{
							flag = true;
							break;
						}
					}
					catch
					{
					}
				}
				if (!flag && !flag2 && ((Element)item).Id != ((Element)val.ActiveView).Id && !((Element)item).Name.Contains("{") && !((Element)item).Name.StartsWith("§"))
				{
					list2.Add(((Element)item).Id);
				}
			}
			foreach (ElementId item3 in list2)
			{
				try
				{
					val.Delete(item3);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除视图 ");
					defaultInterpolatedStringHandler.AppendFormatted(item3.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedViews: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的视图");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list2.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedViews 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedViewTemplates(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedViewTemplates: document 不是 Document 类型");
				return 0;
			}
			List<View> source = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(View))).OfType<View>().ToList();
			List<View> source2 = source.Where((View v) => v.IsTemplate).ToList();
			List<View> list = source.Where((View v) => !v.IsTemplate).ToList();
			HashSet<ElementId> usedTemplateIds = new HashSet<ElementId>();
			foreach (View item in list)
			{
				try
				{
					if (item.ViewTemplateId != (ElementId)null && item.ViewTemplateId != ElementId.InvalidElementId)
					{
						usedTemplateIds.Add(item.ViewTemplateId);
					}
				}
				catch
				{
				}
			}
			List<ElementId> list2 = (from t in source2
				where !usedTemplateIds.Contains(((Element)t).Id)
				select ((Element)t).Id).ToList();
			foreach (ElementId item2 in list2)
			{
				try
				{
					val.Delete(item2);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除视图样板 ");
					defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedViewTemplates: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的视图样板");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list2.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedViewTemplates 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedLinePatterns(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Invalid comparison between Unknown and I4
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Invalid comparison between Unknown and I4
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Invalid comparison between Unknown and I4
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedLinePatterns: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> source = new FilteredElementCollector(val).OfClass(typeof(LinePatternElement)).ToElementIds().ToList();
			HashSet<ElementId> usedLinePatternIds = new HashSet<ElementId>();
			Categories categories = val.Settings.Categories;
			foreach (Category item in (CategoryNameMap)categories)
			{
				Category val2 = item;
				try
				{
					if ((int)val2.CategoryType != 1 && (int)val2.CategoryType == 2)
					{
					}
				}
				catch
				{
				}
			}
			ICollection<ElementId> collection = new FilteredElementCollector(val).WhereElementIsNotElementType().ToElementIds();
			foreach (ElementId item2 in collection)
			{
				try
				{
					Element element = val.GetElement(item2);
					if (element == null)
					{
						continue;
					}
					ParameterSet parameters = element.Parameters;
					foreach (Parameter item3 in parameters)
					{
						Parameter val3 = item3;
						if ((int)val3.StorageType != 4)
						{
							continue;
						}
						ElementId val4 = val3.AsElementId();
						if (val4 != (ElementId)null && val4 != ElementId.InvalidElementId)
						{
							Element element2 = val.GetElement(val4);
							if (element2 is LinePatternElement)
							{
								usedLinePatternIds.Add(val4);
							}
						}
					}
				}
				catch
				{
				}
			}
			List<ElementId> list = source.Where((ElementId id) => !usedLinePatternIds.Contains(id)).ToList();
			if (list.Count > 0)
			{
				Transaction val5 = new Transaction(val, "清理未使用的线型");
				try
				{
					val5.Start();
					foreach (ElementId item4 in list)
					{
						try
						{
							val.Delete(item4);
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
							defaultInterpolatedStringHandler.AppendLiteral("删除线型 ");
							defaultInterpolatedStringHandler.AppendFormatted(item4.Value);
							defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
							defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
							LogError(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
					val5.Commit();
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedLinePatterns: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的线型");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedLinePatterns 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedFillPatterns(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Invalid comparison between Unknown and I4
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedFillPatterns: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> source = new FilteredElementCollector(val).OfClass(typeof(FillPatternElement)).ToElementIds().ToList();
			HashSet<ElementId> usedFillPatternIds = new HashSet<ElementId>();
			ICollection<ElementId> collection = new FilteredElementCollector(val).OfClass(typeof(Material)).ToElementIds();
			foreach (ElementId item in collection)
			{
				try
				{
					Element element = val.GetElement(item);
					Material val2 = (Material)(object)((element is Material) ? element : null);
					if (val2 != null)
					{
					}
				}
				catch
				{
				}
			}
			Categories categories = val.Settings.Categories;
			foreach (Category item2 in (CategoryNameMap)categories)
			{
				Category val3 = item2;
				try
				{
					if ((int)val3.CategoryType == 1)
					{
						ElementId val4 = ((val3.Material != null) ? ((Element)val3.Material).Id : null);
						if (val4 != (ElementId)null && val4 != ElementId.InvalidElementId)
						{
						}
					}
				}
				catch
				{
				}
			}
			List<ElementId> list = source.Where((ElementId id) => !usedFillPatternIds.Contains(id)).ToList();
			if (list.Count > 0)
			{
				Transaction val5 = new Transaction(val, "清理未使用的填充图案");
				try
				{
					val5.Start();
					foreach (ElementId item3 in list)
					{
						try
						{
							val.Delete(item3);
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler.AppendLiteral("删除填充图案 ");
							defaultInterpolatedStringHandler.AppendFormatted(item3.Value);
							defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
							defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
							LogError(defaultInterpolatedStringHandler.ToStringAndClear());
						}
					}
					val5.Commit();
				}
				finally
				{
					((IDisposable)val5)?.Dispose();
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(39, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedFillPatterns: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的填充图案");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedFillPatterns 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedAnnotationStyles(object document)
	{
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedAnnotationStyles: document 不是 Document 类型");
				return 0;
			}
			int num = 0;
			try
			{
				List<ElementId> source = new FilteredElementCollector(val).OfClass(typeof(TextNoteType)).ToElementIds().ToList();
				HashSet<ElementId> usedTextTypeIds = new HashSet<ElementId>();
				ICollection<ElementId> collection = new FilteredElementCollector(val).OfClass(typeof(TextNote)).ToElementIds();
				foreach (ElementId item in collection)
				{
					try
					{
						Element element = val.GetElement(item);
						TextNote val2 = (TextNote)(object)((element is TextNote) ? element : null);
						if (val2 != null)
						{
							usedTextTypeIds.Add(((Element)val2).GetTypeId());
						}
					}
					catch
					{
					}
				}
				List<ElementId> list = source.Where((ElementId id) => !usedTextTypeIds.Contains(id)).ToList();
				foreach (ElementId item2 in list)
				{
					try
					{
						val.Delete(item2);
						num++;
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 2);
						defaultInterpolatedStringHandler.AppendLiteral("删除文字类型 ");
						defaultInterpolatedStringHandler.AppendFormatted(item2.Value);
						defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						LogError(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
			}
			catch (Exception ex2)
			{
				LogError("清理文字类型失败: " + ex2.Message);
			}
			try
			{
				List<ElementId> source2 = new FilteredElementCollector(val).OfClass(typeof(DimensionType)).ToElementIds().ToList();
				HashSet<ElementId> usedDimensionTypeIds = new HashSet<ElementId>();
				ICollection<ElementId> collection2 = new FilteredElementCollector(val).OfClass(typeof(Dimension)).ToElementIds();
				foreach (ElementId item3 in collection2)
				{
					try
					{
						Element element2 = val.GetElement(item3);
						Dimension val3 = (Dimension)(object)((element2 is Dimension) ? element2 : null);
						if (val3 != null)
						{
							usedDimensionTypeIds.Add(((Element)val3).GetTypeId());
						}
					}
					catch
					{
					}
				}
				List<ElementId> list2 = source2.Where((ElementId id) => !usedDimensionTypeIds.Contains(id)).ToList();
				foreach (ElementId item4 in list2)
				{
					try
					{
						val.Delete(item4);
						num++;
					}
					catch (Exception ex3)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(12, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("删除标注类型 ");
						defaultInterpolatedStringHandler2.AppendFormatted(item4.Value);
						defaultInterpolatedStringHandler2.AppendLiteral(" 失败: ");
						defaultInterpolatedStringHandler2.AppendFormatted(ex3.Message);
						LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
				}
			}
			catch (Exception ex4)
			{
				LogError("清理标注类型失败: " + ex4.Message);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("PurgeUnusedAnnotationStyles: 清理了 ");
			defaultInterpolatedStringHandler3.AppendFormatted(num);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个未使用的标注样式");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return num;
		}
		catch (Exception ex5)
		{
			LogError("PurgeUnusedAnnotationStyles 失败: " + ex5.Message);
			return 0;
		}
	}

	public int PurgeUnusedModelGroups(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedModelGroups: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> list = new FilteredElementCollector(val).OfClass(typeof(GroupType)).ToElementIds().ToList();
			List<ElementId> list2 = new List<ElementId>();
			foreach (ElementId item in list)
			{
				try
				{
					Element element = val.GetElement(item);
					GroupType val2 = (GroupType)(object)((element is GroupType) ? element : null);
					if (val2 == null)
					{
						continue;
					}
					GroupSet groups = val2.Groups;
					if (groups.Size <= 0)
					{
						continue;
					}
					IEnumerator enumerator2 = groups.GetEnumerator();
					if (!enumerator2.MoveNext())
					{
						continue;
					}
					object current2 = enumerator2.Current;
					ElementId val3 = (ElementId)((current2 is ElementId) ? current2 : null);
					if (!(val3 != (ElementId)null))
					{
						continue;
					}
					Element element2 = val.GetElement(val3);
					Group val4 = (Group)(object)((element2 is Group) ? element2 : null);
					if (val4 != null)
					{
						Category category = ((Element)val4).Category;
						if (category != null && category.Name.Contains("Model Group"))
						{
							list2.Add(item);
						}
					}
				}
				catch
				{
				}
			}
			HashSet<ElementId> usedGroupTypeIds = new HashSet<ElementId>();
			ICollection<ElementId> collection = new FilteredElementCollector(val).OfClass(typeof(Group)).ToElementIds();
			foreach (ElementId item2 in collection)
			{
				try
				{
					Element element3 = val.GetElement(item2);
					Group val5 = (Group)(object)((element3 is Group) ? element3 : null);
					if (val5 != null)
					{
						Category category2 = ((Element)val5).Category;
						if (category2 != null && category2.Name.Contains("Model Group"))
						{
							usedGroupTypeIds.Add(((Element)val5).GetTypeId());
						}
					}
				}
				catch
				{
				}
			}
			List<ElementId> list3 = list2.Where((ElementId id) => !usedGroupTypeIds.Contains(id)).ToList();
			foreach (ElementId item3 in list3)
			{
				try
				{
					val.Delete(item3);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除模型组类型 ");
					defaultInterpolatedStringHandler.AppendFormatted(item3.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedModelGroups: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list3.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的模型组");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list3.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedModelGroups 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedDetailGroups(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedDetailGroups: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> list = new FilteredElementCollector(val).OfClass(typeof(GroupType)).ToElementIds().ToList();
			List<ElementId> list2 = new List<ElementId>();
			foreach (ElementId item in list)
			{
				try
				{
					Element element = val.GetElement(item);
					GroupType val2 = (GroupType)(object)((element is GroupType) ? element : null);
					if (val2 == null)
					{
						continue;
					}
					GroupSet groups = val2.Groups;
					if (groups.Size <= 0)
					{
						continue;
					}
					IEnumerator enumerator2 = groups.GetEnumerator();
					if (!enumerator2.MoveNext())
					{
						continue;
					}
					object current2 = enumerator2.Current;
					ElementId val3 = (ElementId)((current2 is ElementId) ? current2 : null);
					if (!(val3 != (ElementId)null))
					{
						continue;
					}
					Element element2 = val.GetElement(val3);
					Group val4 = (Group)(object)((element2 is Group) ? element2 : null);
					if (val4 != null)
					{
						Category category = ((Element)val4).Category;
						if (category != null && category.Name.Contains("Detail Group"))
						{
							list2.Add(item);
						}
					}
				}
				catch
				{
				}
			}
			HashSet<ElementId> usedGroupTypeIds = new HashSet<ElementId>();
			ICollection<ElementId> collection = new FilteredElementCollector(val).OfClass(typeof(Group)).ToElementIds();
			foreach (ElementId item2 in collection)
			{
				try
				{
					Element element3 = val.GetElement(item2);
					Group val5 = (Group)(object)((element3 is Group) ? element3 : null);
					if (val5 != null)
					{
						Category category2 = ((Element)val5).Category;
						if (category2 != null && category2.Name.Contains("Detail Group"))
						{
							usedGroupTypeIds.Add(((Element)val5).GetTypeId());
						}
					}
				}
				catch
				{
				}
			}
			List<ElementId> list3 = list2.Where((ElementId id) => !usedGroupTypeIds.Contains(id)).ToList();
			foreach (ElementId item3 in list3)
			{
				try
				{
					val.Delete(item3);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(13, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除详图组类型 ");
					defaultInterpolatedStringHandler.AppendFormatted(item3.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedDetailGroups: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list3.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的详图组");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list3.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedDetailGroups 失败: " + ex2.Message);
			return 0;
		}
	}

	public int PurgeUnusedViewFilters(object document)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PurgeUnusedViewFilters: document 不是 Document 类型");
				return 0;
			}
			List<ElementId> source = new FilteredElementCollector(val).OfClass(typeof(FilterElement)).ToElementIds().ToList();
			HashSet<ElementId> usedFilterIds = new HashSet<ElementId>();
			List<View> list = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(View))).OfType<View>().ToList();
			foreach (View item in list)
			{
				try
				{
					ICollection<ElementId> filters = item.GetFilters();
					foreach (ElementId item2 in filters)
					{
						usedFilterIds.Add(item2);
					}
				}
				catch
				{
				}
			}
			List<ElementId> list2 = source.Where((ElementId id) => !usedFilterIds.Contains(id)).ToList();
			foreach (ElementId item3 in list2)
			{
				try
				{
					val.Delete(item3);
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 2);
					defaultInterpolatedStringHandler.AppendLiteral("删除过滤器 ");
					defaultInterpolatedStringHandler.AppendFormatted(item3.Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(39, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("PurgeUnusedViewFilters: 清理了 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的视图过滤器");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list2.Count;
		}
		catch (Exception ex2)
		{
			LogError("PurgeUnusedViewFilters 失败: " + ex2.Message);
			return 0;
		}
	}

	public bool CutGeometry(object document, object elementToCut, object cuttingElement)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CutGeometry: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((elementToCut is Element) ? elementToCut : null);
			if (val2 == null)
			{
				LogError("CutGeometry: elementToCut 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((cuttingElement is Element) ? cuttingElement : null);
			if (val3 == null)
			{
				LogError("CutGeometry: cuttingElement 不是 Element 类型");
				return false;
			}
			FamilyInstance val4 = (FamilyInstance)(object)((val3 is FamilyInstance) ? val3 : null);
			if (val4 != null)
			{
				try
				{
					if (InstanceVoidCutUtils.IsVoidInstanceCuttingElement((Element)(object)val4) && InstanceVoidCutUtils.CanBeCutWithVoid(val2))
					{
						InstanceVoidCutUtils.AddInstanceVoidCut(val, val2, (Element)(object)val4);
						return true;
					}
				}
				catch (Exception ex)
				{
					LogWarning("CutGeometry: [空心剪切] 失败 - " + ex.Message);
				}
			}
			try
			{
				CutFailureReason val5 = default(CutFailureReason);
				if (SolidSolidCutUtils.CanElementCutElement(val3, val2, out val5))
				{
					bool flag = default(bool);
					if (!SolidSolidCutUtils.CutExistsBetweenElements(val3, val2, out flag))
					{
						SolidSolidCutUtils.AddCutBetweenSolids(val, val3, val2);
						return true;
					}
					return true;
				}
			}
			catch (Exception ex2)
			{
				LogWarning("CutGeometry: [实心剪切] 失败 - " + ex2.Message);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 2);
			defaultInterpolatedStringHandler.AppendLiteral("CutGeometry: 所有剪切方法都失败 - ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val3.Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 无法剪切 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
			LogError(defaultInterpolatedStringHandler.ToStringAndClear());
			return false;
		}
		catch (Exception ex3)
		{
			LogError("CutGeometry 失败: " + ex3.Message);
			return false;
		}
	}

	public bool UncutGeometry(object document, object elementToUncut, object cuttingElement)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("UncutGeometry: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((elementToUncut is Element) ? elementToUncut : null);
			if (val2 == null)
			{
				LogError("UncutGeometry: elementToUncut 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((cuttingElement is Element) ? cuttingElement : null);
			if (val3 != null)
			{
				FamilyInstance val4 = (FamilyInstance)(object)((val3 is FamilyInstance) ? val3 : null);
				if (val4 != null)
				{
					try
					{
						if (InstanceVoidCutUtils.InstanceVoidCutExists(val2, (Element)(object)val4))
						{
							InstanceVoidCutUtils.RemoveInstanceVoidCut(val, val2, (Element)(object)val4);
							return true;
						}
					}
					catch (Exception ex)
					{
						LogWarning("UncutGeometry: [取消空心剪切] 失败 - " + ex.Message);
					}
				}
				try
				{
					bool flag = default(bool);
					if (SolidSolidCutUtils.CutExistsBetweenElements(val3, val2, out flag))
					{
						SolidSolidCutUtils.RemoveCutBetweenSolids(val, val3, val2);
						return true;
					}
				}
				catch (Exception ex2)
				{
					LogWarning("UncutGeometry: [取消实心剪切] 失败 - " + ex2.Message);
				}
				try
				{
					JoinGeometryUtils.UnjoinGeometry(val, val2, val3);
					return true;
				}
				catch (Exception ex3)
				{
					LogError("UncutGeometry: [取消连接剪切] 失败 - " + ex3.Message);
					return false;
				}
			}
			LogError("UncutGeometry: cuttingElement 不是 Element 类型");
			return false;
		}
		catch (Exception ex4)
		{
			LogError("UncutGeometry 失败: " + ex4.Message);
			return false;
		}
	}

	public bool JoinGeometry(object document, object element1, object element2)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("JoinGeometry: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((element1 is Element) ? element1 : null);
			if (val2 == null)
			{
				LogError("JoinGeometry: element1 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((element2 is Element) ? element2 : null);
			if (val3 == null)
			{
				LogError("JoinGeometry: element2 不是 Element 类型");
				return false;
			}
			JoinGeometryUtils.JoinGeometry(val, val2, val3);
			return true;
		}
		catch (Exception ex)
		{
			LogError("JoinGeometry 失败: " + ex.Message);
			return false;
		}
	}

	public bool UnjoinGeometry(object document, object element1, object element2)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("UnjoinGeometry: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((element1 is Element) ? element1 : null);
			if (val2 == null)
			{
				LogError("UnjoinGeometry: element1 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((element2 is Element) ? element2 : null);
			if (val3 == null)
			{
				LogError("UnjoinGeometry: element2 不是 Element 类型");
				return false;
			}
			JoinGeometryUtils.UnjoinGeometry(val, val2, val3);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler.AppendLiteral("UnjoinGeometry: 成功取消连接几何（元素1: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
			defaultInterpolatedStringHandler.AppendLiteral(", 元素2: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val3.Id);
			defaultInterpolatedStringHandler.AppendLiteral("）");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("UnjoinGeometry 失败: " + ex.Message);
			return false;
		}
	}

	public bool SwitchJoinOrder(object document, object element1, object element2)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SwitchJoinOrder: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((element1 is Element) ? element1 : null);
			if (val2 == null)
			{
				LogError("SwitchJoinOrder: element1 不是 Element 类型");
				return false;
			}
			Element val3 = (Element)((element2 is Element) ? element2 : null);
			if (val3 == null)
			{
				LogError("SwitchJoinOrder: element2 不是 Element 类型");
				return false;
			}
			bool flag = JoinGeometryUtils.AreElementsJoined(val, val2, val3);
			bool flag2 = JoinGeometryUtils.AreElementsJoined(val, val3, val2);
			if (flag || flag2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 2);
				defaultInterpolatedStringHandler.AppendLiteral("SwitchJoinOrder: 当前连接状态 - elem1->elem2: ");
				defaultInterpolatedStringHandler.AppendFormatted(flag);
				defaultInterpolatedStringHandler.AppendLiteral(", elem2->elem1: ");
				defaultInterpolatedStringHandler.AppendFormatted(flag2);
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				if (flag & flag2)
				{
					try
					{
						JoinGeometryUtils.UnjoinGeometry(val, val2, val3);
						LogInfo("SwitchJoinOrder: 取消 elem1->elem2 连接");
					}
					catch (Exception ex)
					{
						LogWarning("SwitchJoinOrder: 取消 elem1->elem2 连接失败 - " + ex.Message);
					}
					try
					{
						JoinGeometryUtils.UnjoinGeometry(val, val3, val2);
						LogInfo("SwitchJoinOrder: 取消 elem2->elem1 连接");
					}
					catch (Exception ex2)
					{
						LogWarning("SwitchJoinOrder: 取消 elem2->elem1 连接失败 - " + ex2.Message);
					}
				}
				else if (flag)
				{
					try
					{
						JoinGeometryUtils.UnjoinGeometry(val, val2, val3);
						LogInfo("SwitchJoinOrder: 取消 elem1->elem2 连接");
					}
					catch (Exception ex3)
					{
						LogWarning("SwitchJoinOrder: 取消连接失败 - " + ex3.Message);
						return false;
					}
				}
				else if (flag2)
				{
					try
					{
						JoinGeometryUtils.UnjoinGeometry(val, val3, val2);
						LogInfo("SwitchJoinOrder: 取消 elem2->elem1 连接");
					}
					catch (Exception ex4)
					{
						LogWarning("SwitchJoinOrder: 取消连接失败 - " + ex4.Message);
						return false;
					}
				}
				try
				{
					JoinGeometryUtils.JoinGeometry(val, val3, val2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(36, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("SwitchJoinOrder: 成功切换连接顺序（新顺序: ");
					defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val3.Id);
					defaultInterpolatedStringHandler2.AppendLiteral(" -> ");
					defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val2.Id);
					defaultInterpolatedStringHandler2.AppendLiteral("）");
					LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
					return true;
				}
				catch (Exception ex5)
				{
					LogError("SwitchJoinOrder: 重新连接失败 - " + ex5.Message);
					try
					{
						if (flag)
						{
							JoinGeometryUtils.JoinGeometry(val, val2, val3);
							LogInfo("SwitchJoinOrder: 已恢复原连接 elem1->elem2");
						}
					}
					catch (Exception ex6)
					{
						LogError("SwitchJoinOrder: 恢复原连接也失败 - " + ex6.Message);
					}
					return false;
				}
			}
			LogError("SwitchJoinOrder: 元素尚未连接，无法切换顺序");
			return false;
		}
		catch (Exception ex7)
		{
			LogError("SwitchJoinOrder 失败: " + ex7.Message);
			return false;
		}
	}

	public bool AreElementsJoined(object element1, object element2)
	{
		try
		{
			Element val = (Element)((element1 is Element) ? element1 : null);
			if (val == null)
			{
				LogError("AreElementsJoined: element1 不是 Element 类型");
				return false;
			}
			Element val2 = (Element)((element2 is Element) ? element2 : null);
			if (val2 == null)
			{
				LogError("AreElementsJoined: element2 不是 Element 类型");
				return false;
			}
			return JoinGeometryUtils.AreElementsJoined(val.Document, val, val2);
		}
		catch (Exception ex)
		{
			LogError("AreElementsJoined 失败: " + ex.Message);
			return false;
		}
	}

	public string? GetJoinOrder(object element1, object element2)
	{
		try
		{
			Element val = (Element)((element1 is Element) ? element1 : null);
			if (val == null)
			{
				LogError("GetJoinOrder: element1 不是 Element 类型");
				return null;
			}
			Element val2 = (Element)((element2 is Element) ? element2 : null);
			if (val2 == null)
			{
				LogError("GetJoinOrder: element2 不是 Element 类型");
				return null;
			}
			Document document = val.Document;
			bool flag = JoinGeometryUtils.AreElementsJoined(document, val, val2);
			bool flag2 = JoinGeometryUtils.AreElementsJoined(document, val2, val);
			if (!flag && !flag2)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 2);
				defaultInterpolatedStringHandler.AppendLiteral("元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 和 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 未连接");
				return defaultInterpolatedStringHandler.ToStringAndClear();
			}
			if (flag)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("连接顺序: ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" (主) -> ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val2.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" (从)");
				return defaultInterpolatedStringHandler2.ToStringAndClear();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(18, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("连接顺序: ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(val2.Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" (主) -> ");
			defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(val.Id);
			defaultInterpolatedStringHandler3.AppendLiteral(" (从)");
			return defaultInterpolatedStringHandler3.ToStringAndClear();
		}
		catch (Exception ex)
		{
			LogError("GetJoinOrder 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsCuttingElementInJoin(object element1, object element2)
	{
		try
		{
			Element val = (Element)((element1 is Element) ? element1 : null);
			if (val == null)
			{
				LogError("IsCuttingElementInJoin: element1 不是 Element 类型");
				return false;
			}
			Element val2 = (Element)((element2 is Element) ? element2 : null);
			if (val2 == null)
			{
				LogError("IsCuttingElementInJoin: element2 不是 Element 类型");
				return false;
			}
			Document document = val.Document;
			if (!JoinGeometryUtils.AreElementsJoined(document, val, val2))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 2);
				defaultInterpolatedStringHandler.AppendLiteral("IsCuttingElementInJoin: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 和 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val2.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 未连接");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			return JoinGeometryUtils.IsCuttingElementInJoin(document, val, val2);
		}
		catch (Exception ex)
		{
			LogError("IsCuttingElementInJoin 失败: " + ex.Message);
			return false;
		}
	}
}
