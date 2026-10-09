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
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

internal sealed class AnnotationService : IAnnotationService
{
	private readonly UIApplication _application;

	private readonly IInfrastructureService _infrastructureService;

	public AnnotationService(UIApplication application, IInfrastructureService infrastructureService)
	{
		_application = application ?? throw new ArgumentNullException("application");
		_infrastructureService = infrastructureService ?? throw new ArgumentNullException("infrastructureService");
	}

	public object? CreateDimension(object document, object view, IList<object> references, object linePosition, int? dimensionTypeId = null)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateDimension: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateDimension: view 不是 View 类型");
				return null;
			}
			if (!ParsePosition(linePosition, out XYZ xyz))
			{
				LogError("CreateDimension: 无法解析位置参数");
				return null;
			}
			if (dimensionTypeId.HasValue && dimensionTypeId.Value > 0)
			{
				ElementId val3 = new ElementId((long)dimensionTypeId.Value);
			}
			else
			{
				ElementId val3 = GetDefaultDimensionTypeId(val);
				if (val3 == ElementId.InvalidElementId)
				{
					LogError("CreateDimension: 无法获取默认标注类型");
					return null;
				}
			}
			ReferenceArray val4 = CreateReferenceArray(val, references);
			if (val4 == null || val4.IsEmpty)
			{
				LogError("CreateDimension: 无法创建参考数组");
				return null;
			}
			Line val5 = Line.CreateBound(xyz, xyz.Add(XYZ.BasisX));
			Dimension val6 = ((ItemFactoryBase)val.Create).NewDimension(val2, val5, val4);
			if (val6 == null)
			{
				LogError("CreateDimension: Dimension.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CreateDimension: 成功创建尺寸标注，ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val6).Id.Value);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val6;
		}
		catch (Exception ex)
		{
			LogError("CreateDimension 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateTextNote(object document, object view, string text, object position, int? textTypeId = null)
	{
		return CreateTextNote(document, view, text, position, textTypeId, false);
	}

	public object? CreateTextNote(object document, object view, string text, object position, int? textTypeId = null, bool addLeader = false)
	{
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateTextNote: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateTextNote: view 不是 View 类型");
				return null;
			}
			if (!ParsePosition(position, out XYZ xyz))
			{
				LogError("CreateTextNote: 无法解析位置参数");
				return null;
			}
			ElementId val3;
			if (textTypeId.HasValue && textTypeId.Value > 0)
			{
				val3 = new ElementId((long)textTypeId.Value);
			}
			else
			{
				val3 = GetDefaultTextNoteTypeId(val);
				if (val3 == ElementId.InvalidElementId)
				{
					LogError("CreateTextNote: 无法获取默认文字注释类型");
					return null;
				}
			}
			TextNoteOptions val4 = new TextNoteOptions
			{
				TypeId = val3
			};
			TextNote val5 = TextNote.Create(val, ((Element)val2).Id, xyz, text, val4);
			if (val5 == null)
			{
				LogError("CreateTextNote: TextNote.Create 返回 null");
				return null;
			}
			if (addLeader)
			{
				try
				{
					val5.AddLeader((TextNoteLeaderTypes)0);
					LogInfo("CreateTextNote: 成功为文字注释添加引线");
				}
				catch (Exception ex)
				{
					LogWarning("CreateTextNote: 添加引线失败 - " + ex.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CreateTextNote: 成功创建文字注释，ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val5).Id.Value);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val5;
		}
		catch (Exception ex2)
		{
			LogError("CreateTextNote 失败: " + ex2.Message);
			return null;
		}
	}

	public IEnumerable<object> GetAllDimensions(object document)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllDimensions: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(Dimension));
			return val2.ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetAllDimensions 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetAllTextNotes(object document)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllTextNotes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(TextNote));
			return val2.ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetAllTextNotes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? CreateTag(object document, object view, object element, int tagTypeId, object position)
	{
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateTag: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateTag: view 不是 View 类型");
				return null;
			}
			Element val3 = (Element)((element is Element) ? element : null);
			if (val3 == null)
			{
				LogError("CreateTag: element 不是 Element 类型");
				return null;
			}
			if (!ParsePosition(position, out XYZ xyz))
			{
				LogError("CreateTag: 无法解析位置参数");
				return null;
			}
			Element element2 = val.GetElement(new ElementId((long)tagTypeId));
			if (element2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateTag: 无法找到标签类型 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(tagTypeId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			ElementId id = ((Element)val2).Id;
			IndependentTag val4 = IndependentTag.Create(val, id, new Reference(val3), false, (TagMode)1, (TagOrientation)0, xyz);
			if (val4 == null)
			{
				LogError("CreateTag: IndependentTag.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateTag: 成功创建标签，ID: ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)val4).Id.Value);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateTag 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateRevisionCloud(object document, object view, IList<object> points, int revisionId)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateRevisionCloud: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateRevisionCloud: view 不是 View 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)revisionId));
			Revision val3 = (Revision)(object)((element is Revision) ? element : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateRevisionCloud: 无法找到修订 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(revisionId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			List<XYZ> list = new List<XYZ>();
			foreach (object point in points)
			{
				if (ParsePosition(point, out XYZ xyz))
				{
					list.Add(xyz);
				}
			}
			if (list.Count < 2)
			{
				LogError("CreateRevisionCloud: 至少需要 2 个点来创建修订云");
				return null;
			}
			LogError("CreateRevisionCloud: 该方法暂未完整实现（Revit API 复杂）");
			return null;
		}
		catch (Exception ex)
		{
			LogError("CreateRevisionCloud 失败: " + ex.Message);
			return null;
		}
	}

	private bool ParsePosition(object positionObj, out XYZ xyz)
	{
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Expected O, but got Unknown
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		try
		{
			if (positionObj == null)
			{
				xyz = XYZ.Zero;
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (positionObj is IDictionary<string, object> dictionary)
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
			Type type = positionObj.GetType();
			PropertyInfo property = type.GetProperty("x", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			PropertyInfo property2 = type.GetProperty("y", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			PropertyInfo property3 = type.GetProperty("z", BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
			if (property != null && property2 != null && property3 != null)
			{
				try
				{
					num = Convert.ToDouble(property.GetValue(positionObj, null));
					num2 = Convert.ToDouble(property2.GetValue(positionObj, null));
					num3 = Convert.ToDouble(property3.GetValue(positionObj, null));
					xyz = new XYZ(num, num2, num3);
					return true;
				}
				catch
				{
				}
			}
			LogError("ParsePosition: 无法解析位置对象类型 " + type.Name + "，支持 IDictionary 或 x/y/z 属性");
			xyz = XYZ.Zero;
			return false;
		}
		catch
		{
			xyz = XYZ.Zero;
			return false;
		}
	}

	private ElementId GetDefaultDimensionTypeId(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element obj = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).FirstElement();
			DimensionType val = (DimensionType)(object)((obj is DimensionType) ? obj : null);
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = ((Element)val).Id;
				if (obj2 != null)
				{
					goto IL_0036;
				}
			}
			obj2 = ElementId.InvalidElementId;
			goto IL_0036;
			IL_0036:
			return (ElementId)obj2;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private ElementId GetDefaultTextNoteTypeId(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element obj = new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)).FirstElement();
			TextNoteType val = (TextNoteType)(object)((obj is TextNoteType) ? obj : null);
			object obj2;
			if (val == null)
			{
				obj2 = null;
			}
			else
			{
				obj2 = ((Element)val).Id;
				if (obj2 != null)
				{
					goto IL_0036;
				}
			}
			obj2 = ElementId.InvalidElementId;
			goto IL_0036;
			IL_0036:
			return (ElementId)obj2;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	public object? CreateDimensionByPoints(object document, object view, IList<object> points, object linePosition, int? dimensionTypeId = null)
	{
		//IL_0220: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Expected O, but got Unknown
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0182: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateDimensionByPoints: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateDimensionByPoints: view 不是 View 类型");
				return null;
			}
			if (!ParsePosition(linePosition, out XYZ xyz))
			{
				LogError("CreateDimensionByPoints: 无法解析标注线位置参数");
				return null;
			}
			ElementId val3;
			if (dimensionTypeId.HasValue && dimensionTypeId.Value > 0)
			{
				val3 = new ElementId((long)dimensionTypeId.Value);
			}
			else
			{
				val3 = GetDefaultDimensionTypeId(val);
				if (val3 == ElementId.InvalidElementId)
				{
					LogError("CreateDimensionByPoints: 无法获取默认标注类型");
					return null;
				}
			}
			List<XYZ> list = new List<XYZ>();
			foreach (object point in points)
			{
				if (ParsePosition(point, out XYZ xyz2))
				{
					list.Add(xyz2);
				}
			}
			if (list.Count < 2)
			{
				LogError("CreateDimensionByPoints: 至少需要 2 个点来创建尺寸标注");
				return null;
			}
			XYZ val4 = list[0];
			XYZ val5 = list[1];
			XYZ val6 = (val5 - val4).Normalize();
			XYZ val7 = xyz;
			XYZ val8 = xyz.Add(val6.Multiply(10.0));
			Line val9 = Line.CreateBound(val7, val8);
			ReferenceArray val10 = new ReferenceArray();
			List<ElementId> list2 = new List<ElementId>();
			foreach (XYZ item in list)
			{
				try
				{
					XYZ val11 = val6.CrossProduct(XYZ.BasisZ).Normalize();
					Line val12 = Line.CreateBound(item.Subtract(val11.Multiply(0.5)), item.Add(val11.Multiply(0.5)));
					Plane val13 = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, XYZ.Zero);
					SketchPlane val14 = SketchPlane.Create(val, val13);
					ModelCurve val15 = ((ItemFactoryBase)val.Create).NewModelCurve((Curve)(object)val12, val14);
					if (val15 != null)
					{
						val10.Append(new Reference((Element)(object)val15));
						list2.Add(((Element)val15).Id);
					}
				}
				catch (Exception ex)
				{
					LogError("CreateDimensionByPoints: 创建参考曲线失败 - " + ex.Message);
				}
			}
			if (val10.IsEmpty)
			{
				LogError("CreateDimensionByPoints: 无法创建任何参考点");
				return null;
			}
			Dimension val16;
			try
			{
				val16 = ((ItemFactoryBase)val.Create).NewDimension(val2, val9, val10);
			}
			catch (Exception ex2)
			{
				LogError("CreateDimensionByPoints: NewDimension 失败 - " + ex2.Message);
				foreach (ElementId item2 in list2)
				{
					try
					{
						val.Delete(item2);
					}
					catch
					{
					}
				}
				return null;
			}
			if (val16 == null)
			{
				LogError("CreateDimensionByPoints: NewDimension 返回 null");
				foreach (ElementId item3 in list2)
				{
					try
					{
						val.Delete(item3);
					}
					catch
					{
					}
				}
				return null;
			}
			try
			{
				((Element)val16).ChangeTypeId(val3);
			}
			catch
			{
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
			defaultInterpolatedStringHandler.AppendLiteral("CreateDimensionByPoints: 成功创建尺寸标注，ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val16).Id.Value);
			defaultInterpolatedStringHandler.AppendLiteral("（创建了 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个参考曲线）");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val16;
		}
		catch (Exception ex3)
		{
			LogError("CreateDimensionByPoints 失败: " + ex3.Message);
			return null;
		}
	}

	public object? CreateDimensionByElements(object document, object view, IList<int> elementIds, object dimensionLineOrigin, object dimensionDirection, int? dimensionTypeId = null, double? offset = null, string position = "auto", double? autoOffsetDistance = null)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_03f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateDimensionByElements: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateDimensionByElements: view 不是 View 类型");
				return null;
			}
			if (!ParsePosition(dimensionLineOrigin, out XYZ xyz))
			{
				LogError("CreateDimensionByElements: 无法解析标注线原点参数");
				return null;
			}
			if (!ParsePosition(dimensionDirection, out XYZ xyz2))
			{
				LogError("CreateDimensionByElements: 无法解析标注方向参数");
				return null;
			}
			double offset2 = offset.GetValueOrDefault() / 304.8;
			List<Element> list = new List<Element>();
			foreach (int elementId in elementIds)
			{
				Element element = val.GetElement(new ElementId((long)elementId));
				if (element != null)
				{
					list.Add(element);
				}
			}
			if (list.Count < 2)
			{
				LogError("CreateDimensionByElements: 至少需要2个元素才能创建标注");
				return null;
			}
			bool flag = xyz2.X == 0.0 && xyz2.Y == 0.0 && xyz2.Z == 0.0;
			List<XYZ> list2 = new List<XYZ>();
			if (flag)
			{
				list2 = DimensionCreator.DetectDimensionDirections(list);
				if (list2.Count == 0)
				{
					list2.Add(XYZ.BasisX);
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateDimensionByElements: 自动检测到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个标注方向");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				XYZ item = new XYZ(xyz2.X / 304.8, xyz2.Y / 304.8, xyz2.Z / 304.8);
				list2.Add(item);
			}
			DimensionCreator dimensionCreator = new DimensionCreator(val, val2);
			ElementId val3 = (ElementId)((!dimensionTypeId.HasValue || dimensionTypeId.Value <= 0) ? ((object)ElementId.InvalidElementId) : ((object)new ElementId((long)dimensionTypeId.Value)));
			Dimension val4 = null;
			int num = 0;
			foreach (XYZ item2 in list2)
			{
				XYZ val5;
				if (xyz.X == 0.0 && xyz.Y == 0.0 && xyz.Z == 0.0)
				{
					double num2 = autoOffsetDistance ?? 2000.0;
					val5 = DimensionCreator.DetectDimensionLineOrigin(list, item2, position, val2, num2);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(67, 6);
					defaultInterpolatedStringHandler2.AppendLiteral("CreateDimensionByElements: 方向 ");
					defaultInterpolatedStringHandler2.AppendFormatted(num + 1);
					defaultInterpolatedStringHandler2.AppendLiteral(" 自动检测标注线原点 = (");
					defaultInterpolatedStringHandler2.AppendFormatted(val5.X, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(val5.Y, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(val5.Z, "F2");
					defaultInterpolatedStringHandler2.AppendLiteral(") 英尺 (位置: ");
					defaultInterpolatedStringHandler2.AppendFormatted(position);
					defaultInterpolatedStringHandler2.AppendLiteral(", 偏移: ");
					defaultInterpolatedStringHandler2.AppendFormatted(num2, "F0");
					defaultInterpolatedStringHandler2.AppendLiteral("mm)");
					LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					val5 = new XYZ(xyz.X / 304.8, xyz.Y / 304.8, xyz.Z / 304.8);
				}
				Dimension val6 = dimensionCreator.CreateDimension(list, val5, item2.Normalize(), (val3 != ElementId.InvalidElementId) ? val3 : null, offset2);
				if (val6 != null)
				{
					num++;
					if (val4 == null)
					{
						val4 = val6;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(55, 5);
					defaultInterpolatedStringHandler3.AppendLiteral("CreateDimensionByElements: 成功创建第 ");
					defaultInterpolatedStringHandler3.AppendFormatted(num);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个尺寸标注，ID: ");
					defaultInterpolatedStringHandler3.AppendFormatted(((Element)val6).Id.Value);
					defaultInterpolatedStringHandler3.AppendLiteral("，方向: (");
					defaultInterpolatedStringHandler3.AppendFormatted(item2.X, "F2");
					defaultInterpolatedStringHandler3.AppendLiteral(", ");
					defaultInterpolatedStringHandler3.AppendFormatted(item2.Y, "F2");
					defaultInterpolatedStringHandler3.AppendLiteral(", ");
					defaultInterpolatedStringHandler3.AppendFormatted(item2.Z, "F2");
					defaultInterpolatedStringHandler3.AppendLiteral(")");
					LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(39, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("CreateDimensionByElements: 方向 ");
					defaultInterpolatedStringHandler4.AppendFormatted(num + 1);
					defaultInterpolatedStringHandler4.AppendLiteral(" 创建尺寸标注失败");
					LogWarning(defaultInterpolatedStringHandler4.ToStringAndClear());
				}
			}
			if (val4 == null)
			{
				LogError("CreateDimensionByElements: 所有方向的尺寸标注都创建失败");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(40, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("CreateDimensionByElements: 总共成功创建 ");
			defaultInterpolatedStringHandler5.AppendFormatted(num);
			defaultInterpolatedStringHandler5.AppendLiteral(" 个尺寸标注");
			LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateDimensionByElements 失败: " + ex.Message);
			return null;
		}
	}

	private ReferenceArray? CreateReferenceArray(Document doc, IList<object> references)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Expected O, but got Unknown
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		try
		{
			ReferenceArray val = new ReferenceArray();
			foreach (object reference in references)
			{
				Element val2 = (Element)((reference is Element) ? reference : null);
				if (val2 != null)
				{
					Reference val3 = new Reference(val2);
					val.Append(val3);
					continue;
				}
				Reference val4 = (Reference)((reference is Reference) ? reference : null);
				if (val4 != null)
				{
					val.Append(val4);
				}
			}
			return val;
		}
		catch (Exception ex)
		{
			LogError("CreateReferenceArray 失败: " + ex.Message);
			return null;
		}
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[AnnotationService] " + message);
	}

	private static void LogError(string message)
	{
		Logger.Error("[AnnotationService] " + message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[AnnotationService] " + message);
	}

	public IEnumerable<object> GetAllTags(object document)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllTags: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			try
			{
				IList<Element> list2 = new FilteredElementCollector(val).OfClass(typeof(IndependentTag)).ToElements();
				list.AddRange(list2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetAllTags: 找到 ");
				defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个 IndependentTag");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex)
			{
				LogWarning("获取 IndependentTag 失败: " + ex.Message);
			}
			string[] array = new string[3]
			{
				"RoomTag",
				"SpaceTag",
				"AreaTag"
			};
			string[] array2 = array;
			foreach (string text in array2)
			{
				try
				{
					Type type = Type.GetType("Autodesk.Revit.DB." + text + ", RevitAPI");
					if (type != null)
					{
						IList<Element> list3 = new FilteredElementCollector(val).OfClass(type).ToElements();
						list.AddRange(list3);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 2);
						defaultInterpolatedStringHandler2.AppendLiteral("GetAllTags: 找到 ");
						defaultInterpolatedStringHandler2.AppendFormatted(list3.Count);
						defaultInterpolatedStringHandler2.AppendLiteral(" 个 ");
						defaultInterpolatedStringHandler2.AppendFormatted(text);
						LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
				}
				catch (Exception ex2)
				{
					Logger.Warning("[AnnotationService] GetAllTags: 类型 " + text + " 不存在或获取失败 - " + ex2.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(21, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("GetAllTags: 总共找到 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个标记");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return list;
		}
		catch (Exception ex3)
		{
			LogError("GetAllTags 失败: " + ex3.Message);
			return Enumerable.Empty<object>();
		}
	}

	public int? GetTaggedElementId(object tag)
	{
		try
		{
			IndependentTag val = (IndependentTag)((tag is IndependentTag) ? tag : null);
			if (val != null)
			{
				ICollection<LinkElementId> taggedElementIds = val.GetTaggedElementIds();
				if (taggedElementIds != null && taggedElementIds.Count > 0)
				{
					LinkElementId val2 = taggedElementIds.First();
					try
					{
						ElementId hostElementId = val2.HostElementId;
						if (hostElementId != (ElementId)null && hostElementId != ElementId.InvalidElementId)
						{
							return (int)hostElementId.Value;
						}
					}
					catch
					{
					}
				}
				return null;
			}
			try
			{
				Type type = tag.GetType();
				_ = type.Name;
				string[] array = new string[3]
				{
					"Room",
					"Space",
					"Area"
				};
				string[] array2 = array;
				foreach (string name in array2)
				{
					try
					{
						PropertyInfo property = type.GetProperty(name);
						if (!(property != null))
						{
							continue;
						}
						object value = property.GetValue(tag);
						if (value == null)
						{
							continue;
						}
						PropertyInfo property2 = value.GetType().GetProperty("Id");
						if (property2 != null)
						{
							object value2 = property2.GetValue(value);
							ElementId val3 = (ElementId)((value2 is ElementId) ? value2 : null);
							if (val3 != null && val3 != (ElementId)null && val3 != ElementId.InvalidElementId)
							{
								return (int)val3.Value;
							}
						}
					}
					catch
					{
					}
				}
			}
			catch (Exception ex)
			{
				LogWarning("GetTaggedElementId: 反射处理失败 - " + ex.Message);
			}
			LogError("GetTaggedElementId: 不支持的标记类型: " + tag.GetType().Name);
			return null;
		}
		catch (Exception ex2)
		{
			LogError("GetTaggedElementId 失败: " + ex2.Message);
			return null;
		}
	}

	public IEnumerable<object> GetTagsInView(object document, object view)
	{
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetTagsInView: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("GetTagsInView: view 不是 View 类型");
				return Enumerable.Empty<object>();
			}
			IList<Element> list = new FilteredElementCollector(val, ((Element)val2).Id).OfClass(typeof(IndependentTag)).ToElements();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 2);
			defaultInterpolatedStringHandler.AppendLiteral("GetTagsInView: 视图 ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val2).Id);
			defaultInterpolatedStringHandler.AppendLiteral(" 中找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个标记");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list.Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetTagsInView 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IList<int> GetTaggedElementIds(object tag)
	{
		List<int> list = new List<int>();
		try
		{
			IndependentTag val = (IndependentTag)((tag is IndependentTag) ? tag : null);
			if (val != null)
			{
				ICollection<LinkElementId> taggedElementIds = val.GetTaggedElementIds();
				if (taggedElementIds != null)
				{
					foreach (LinkElementId item in taggedElementIds)
					{
						try
						{
							ElementId hostElementId = item.HostElementId;
							if (hostElementId != (ElementId)null && hostElementId != ElementId.InvalidElementId)
							{
								list.Add((int)hostElementId.Value);
							}
						}
						catch
						{
						}
					}
				}
			}
			else
			{
				int? taggedElementId = GetTaggedElementId(tag);
				if (taggedElementId.HasValue)
				{
					list.Add(taggedElementId.Value);
				}
			}
		}
		catch (Exception ex)
		{
			LogWarning("GetTaggedElementIds 失败: " + ex.Message);
		}
		return list;
	}

	public object? CreateTag(object document, object view, object element, object position, int tagTypeId = 0, bool addLeader = false)
	{
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected O, but got Unknown
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Expected O, but got Unknown
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Expected O, but got Unknown
		//IL_0254: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateTag: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateTag: view 不是 View 类型");
				return null;
			}
			Element val3 = (Element)((element is Element) ? element : null);
			if (val3 == null)
			{
				LogError("CreateTag: element 不是 Element 类型");
				return null;
			}
			if (!ParsePosition(position, out XYZ xyz))
			{
				LogError("CreateTag: 无法解析位置参数");
				return null;
			}
			Element val4 = null;
			Room val5 = (Room)(object)((val3 is Room) ? val3 : null);
			if (val5 != null)
			{
				LogInfo("CreateTag: 使用 RoomTag API 创建房间标记");
				Location location = ((Element)val5).Location;
				LocationPoint val6 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
				if (val6 == null)
				{
					LogError("CreateTag: 房间没有有效的位置点");
					return null;
				}
				XYZ point = val6.Point;
				UV val7 = new UV(point.X, point.Y);
				RoomTag val8 = val.Create.NewRoomTag(new LinkElementId(((Element)val5).Id), val7, (ElementId)null);
				if (val8 == null)
				{
					LogError("CreateTag: NewRoomTag 返回 null");
					return null;
				}
				if (tagTypeId > 0)
				{
					try
					{
						Element element2 = val.GetElement(new ElementId((long)tagTypeId));
						RoomTagType val9 = (RoomTagType)(object)((element2 is RoomTagType) ? element2 : null);
						if (val9 != null)
						{
							val8.RoomTagType = val9;
							LogInfo("CreateTag: 已应用标记类型: " + ((Element)val9).Name);
						}
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
						defaultInterpolatedStringHandler.AppendLiteral("CreateTag: 应用房间标记类型 ");
						defaultInterpolatedStringHandler.AppendFormatted(tagTypeId);
						defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val4 = (Element)(object)val8;
			}
			else
			{
				LogInfo("CreateTag: 使用 IndependentTag.Create API 创建标记");
				IndependentTag val10 = IndependentTag.Create(val, ((Element)val2).Id, new Reference(val3), addLeader, (TagMode)0, (TagOrientation)0, xyz);
				if (val10 == null)
				{
					LogError("CreateTag: IndependentTag.Create 返回 null");
					return null;
				}
				if (tagTypeId > 0)
				{
					try
					{
						Element element3 = val.GetElement(new ElementId((long)tagTypeId));
						if (element3 != null)
						{
							((Element)val10).ChangeTypeId(new ElementId((long)tagTypeId));
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
							defaultInterpolatedStringHandler2.AppendLiteral("CreateTag: 已应用标记类型 ID: ");
							defaultInterpolatedStringHandler2.AppendFormatted(tagTypeId);
							LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
						}
					}
					catch (Exception ex2)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(24, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("CreateTag: 切换标记类型 ");
						defaultInterpolatedStringHandler3.AppendFormatted(tagTypeId);
						defaultInterpolatedStringHandler3.AppendLiteral(" 失败 - ");
						defaultInterpolatedStringHandler3.AppendFormatted(ex2.Message);
						LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
					}
				}
				val4 = (Element)(object)val10;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(29, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateTag: 成功创建标记，ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(val4.Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral("，目标元素: ");
			defaultInterpolatedStringHandler4.AppendFormatted(val3.Id.Value);
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val4;
		}
		catch (Exception ex3)
		{
			LogError("CreateTag 失败: " + ex3.Message);
			return null;
		}
	}

	public object? CreateAngularDimension(object document, object view, int element1Id, int element2Id, object arcPosition, int? dimensionTypeId = null)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Expected O, but got Unknown
		//IL_0309: Unknown result type (might be due to invalid IL or missing references)
		//IL_0310: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateAngularDimension: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateAngularDimension: view 不是 View 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)element1Id));
			Element element2 = val.GetElement(new ElementId((long)element2Id));
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateAngularDimension: 找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(element1Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 的第一个元素");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (element2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateAngularDimension: 找不到 ID 为 ");
				defaultInterpolatedStringHandler2.AppendFormatted(element2Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" 的第二个元素");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			XYZ val3 = ExtractElementCenterDirection(element);
			XYZ val4 = ExtractElementCenterDirection(element2);
			if (val3 == null || val4 == null)
			{
				LogError("CreateAngularDimension: 无法提取元素方向");
				return null;
			}
			XYZ val5 = CalculateIntersectionPoint(element, element2, val3, val4);
			if (val5 == null)
			{
				LogError("CreateAngularDimension: 无法计算角度顶点");
				return null;
			}
			if (!ParsePosition(arcPosition, out XYZ xyz))
			{
				LogError("CreateAngularDimension: 无法解析圆弧位置参数");
				return null;
			}
			XYZ val6;
			if (IsZeroPosition(xyz))
			{
				val6 = CalculateOptimalArcPosition(val5, val3, val4, val2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(42, 3);
				defaultInterpolatedStringHandler3.AppendLiteral("CreateAngularDimension: 自动计算圆弧中心位置: (");
				defaultInterpolatedStringHandler3.AppendFormatted(val6.X, "F2");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(val6.Y, "F2");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(val6.Z, "F2");
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			else
			{
				val6 = new XYZ(xyz.X / 304.8, xyz.Y / 304.8, xyz.Z / 304.8);
			}
			double num = val5.DistanceTo(val6);
			if (num < 0.1)
			{
				LogWarning("CreateAngularDimension: 圆弧半径过小，使用默认值 5 英尺");
				num = 5.0;
				XYZ val7 = val3.Normalize().Add(val4.Normalize()).Normalize();
				val6 = val5.Add(val7.Multiply(num));
			}
			ElementId val8;
			if (dimensionTypeId.HasValue && dimensionTypeId.Value > 0)
			{
				val8 = new ElementId((long)dimensionTypeId.Value);
			}
			else
			{
				val8 = GetDefaultDimensionTypeId(val);
				if (val8 == ElementId.InvalidElementId)
				{
					LogError("CreateAngularDimension: 无法获取默认标注类型");
					return null;
				}
			}
			Dimension val11;
			try
			{
				IList<Reference> list = ExtractAngularDimensionReferences(val, element, element2, val3, val4, val5);
				if (list == null || list.Count < 2)
				{
					LogError("CreateAngularDimension: 无法从元素提取有效的几何参考");
					return null;
				}
				Arc val9 = CreateArcForAngularDimension(val5, val3, val4, val6, num);
				if ((GeometryObject)(object)val9 == (GeometryObject)null)
				{
					LogError("CreateAngularDimension: 无法创建圆弧");
					return null;
				}
				Element element3 = val.GetElement(val8);
				DimensionType val10 = (DimensionType)(object)((element3 is DimensionType) ? element3 : null);
				val11 = (Dimension)(object)AngularDimension.Create(val, val2, val9, list, val10);
				if (val11 == null)
				{
					LogError("CreateAngularDimension: AngularDimension.Create 返回 null");
					return null;
				}
				LogInfo("CreateAngularDimension: 成功创建角度标注");
			}
			catch (Exception ex)
			{
				LogError("CreateAngularDimension: 创建角度标注失败 - " + ex.Message);
				return null;
			}
			if (val11 == null)
			{
				LogError("CreateAngularDimension: AngularDimension.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(37, 1);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateAngularDimension: 成功创建角度标注，ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val11).Id.Value);
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val11;
		}
		catch (Exception ex2)
		{
			LogError("CreateAngularDimension 失败: " + ex2.Message);
			return null;
		}
	}

	private XYZ? ExtractElementCenterDirection(Element element)
	{
		try
		{
			Wall val = (Wall)(object)((element is Wall) ? element : null);
			if (val != null)
			{
				Location location = ((Element)val).Location;
				LocationCurve val2 = (LocationCurve)(object)((location is LocationCurve) ? location : null);
				if (val2 != null)
				{
					Curve curve = val2.Curve;
					Line val3 = (Line)(object)((curve is Line) ? curve : null);
					if (val3 != null)
					{
						return val3.Direction.Normalize();
					}
				}
			}
			FamilyInstance val4 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val4 != null)
			{
				Location location2 = ((Element)val4).Location;
				LocationCurve val5 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
				if (val5 != null)
				{
					Curve curve2 = val5.Curve;
					Line val6 = (Line)(object)((curve2 is Line) ? curve2 : null);
					if (val6 != null)
					{
						return val6.Direction.Normalize();
					}
				}
				XYZ elementCenter = GetElementCenter(element);
				if (elementCenter != null)
				{
					Transform transform = ((Instance)val4).GetTransform();
					return transform.BasisX;
				}
			}
			CurveElement val7 = (CurveElement)(object)((element is CurveElement) ? element : null);
			if (val7 != null)
			{
				Curve geometryCurve = val7.GeometryCurve;
				Line val8 = (Line)(object)((geometryCurve is Line) ? geometryCurve : null);
				if (val8 != null)
				{
					return val8.Direction.Normalize();
				}
			}
			Grid val9 = (Grid)(object)((element is Grid) ? element : null);
			if (val9 != null)
			{
				Curve curve3 = val9.Curve;
				Line val10 = (Line)(object)((curve3 is Line) ? curve3 : null);
				if (val10 != null)
				{
					return val10.Direction.Normalize();
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogWarning("ExtractElementCenterDirection: 提取元素方向失败 - " + ex.Message);
			return null;
		}
	}

	private XYZ? CalculateIntersectionPoint(Element element1, Element element2, XYZ direction1, XYZ direction2)
	{
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		try
		{
			XYZ elementCenter = GetElementCenter(element1);
			XYZ elementCenter2 = GetElementCenter(element2);
			if (elementCenter == null || elementCenter2 == null)
			{
				return null;
			}
			double value = direction1.Normalize().DotProduct(direction2.Normalize());
			if (Math.Abs(value) > 0.996)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(52, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CalculateIntersectionPoint: 两个元素方向平行（夹角约 ");
				defaultInterpolatedStringHandler.AppendFormatted(Math.Acos(Math.Abs(value)) * 180.0 / Math.PI, "F1");
				defaultInterpolatedStringHandler.AppendLiteral("°），无法创建角度标注");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			XYZ val = direction1.CrossProduct(direction2);
			if (val.GetLength() < 0.001)
			{
				LogError("CalculateIntersectionPoint: 两个元素方向平行或重合，无法创建角度标注");
				return null;
			}
			double num = ((elementCenter2.X - elementCenter.X) * direction2.Y - (elementCenter2.Y - elementCenter.Y) * direction2.X) / (direction1.X * direction2.Y - direction1.Y * direction2.X);
			XYZ val2 = new XYZ(elementCenter.X + num * direction1.X, elementCenter.Y + num * direction1.Y, (elementCenter.Z + elementCenter2.Z) / 2.0);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("CalculateIntersectionPoint: 计算交点: (");
			defaultInterpolatedStringHandler2.AppendFormatted(val2.X, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(", ");
			defaultInterpolatedStringHandler2.AppendFormatted(val2.Y, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(", ");
			defaultInterpolatedStringHandler2.AppendFormatted(val2.Z, "F2");
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogWarning("CalculateIntersectionPoint: 计算交点失败 - " + ex.Message);
			return null;
		}
	}

	private XYZ? GetElementCenter(Element element)
	{
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		try
		{
			BoundingBoxXYZ val = element.get_BoundingBox((View)null);
			if (val != null)
			{
				return new XYZ((val.Min.X + val.Max.X) / 2.0, (val.Min.Y + val.Max.Y) / 2.0, (val.Min.Z + val.Max.Z) / 2.0);
			}
			Wall val2 = (Wall)(object)((element is Wall) ? element : null);
			if (val2 != null)
			{
				Location location = ((Element)val2).Location;
				LocationCurve val3 = (LocationCurve)(object)((location is LocationCurve) ? location : null);
				if (val3 != null)
				{
					Curve curve = val3.Curve;
					return curve.Evaluate(0.5, true);
				}
			}
			FamilyInstance val4 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			if (val4 != null)
			{
				Location location2 = ((Element)val4).Location;
				LocationPoint val5 = (LocationPoint)(object)((location2 is LocationPoint) ? location2 : null);
				if (val5 != null)
				{
					return val5.Point;
				}
				Location location3 = ((Element)val4).Location;
				LocationCurve val6 = (LocationCurve)(object)((location3 is LocationCurve) ? location3 : null);
				if (val6 != null)
				{
					return val6.Curve.Evaluate(0.5, true);
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogWarning("GetElementCenter: 获取元素中心失败 - " + ex.Message);
			return null;
		}
	}

	private XYZ CalculateOptimalArcPosition(XYZ vertex, XYZ direction1, XYZ direction2, View view)
	{
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		try
		{
			XYZ val = direction1.Normalize().Add(direction2.Normalize()).Normalize();
			XYZ val2 = vertex.Add(val.Multiply(5.0));
			ViewPlan val3 = (ViewPlan)(object)((view is ViewPlan) ? view : null);
			if (val3 != null)
			{
				Level genLevel = ((View)val3).GenLevel;
				double num = ((genLevel != null) ? genLevel.Elevation : 0.0);
				val2 = new XYZ(val2.X, val2.Y, num);
			}
			return val2;
		}
		catch
		{
			return vertex.Add(new XYZ(5.0, 5.0, 0.0));
		}
	}

	private bool IsZeroPosition(XYZ position)
	{
		return Math.Abs(position.X) < 0.001 && Math.Abs(position.Y) < 0.001 && Math.Abs(position.Z) < 0.001;
	}

	private Arc? CreateArcForAngularDimension(XYZ vertex, XYZ direction1, XYZ direction2, XYZ arcCenter, double radius)
	{
		try
		{
			XYZ val = direction1.Normalize();
			XYZ val2 = direction2.Normalize();
			double num = Math.Atan2(val.Y, val.X);
			double num2 = Math.Atan2(val2.Y, val2.X);
			double num3;
			for (num3 = num2 - num; num3 <= -Math.PI; num3 += Math.PI * 2.0)
			{
			}
			while (num3 > Math.PI)
			{
				num3 -= Math.PI * 2.0;
			}
			double num4;
			double num5;
			if (num3 >= 0.0)
			{
				num4 = num;
				num5 = num + num3;
			}
			else
			{
				num4 = num + num3;
				num5 = num;
			}
			return Arc.Create(arcCenter, radius, num4, num5, XYZ.BasisZ, XYZ.BasisX);
		}
		catch (Exception ex)
		{
			LogError("CreateArcForAngularDimension: 创建圆弧失败 - " + ex.Message);
			return null;
		}
	}

	private IList<Reference>? ExtractAngularDimensionReferences(Document doc, Element element1, Element element2, XYZ direction1, XYZ direction2, XYZ vertex)
	{
		try
		{
			List<Reference> list = new List<Reference>();
			Reference val = ExtractElementEdgeReference(element1, direction1, vertex);
			if (val == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ExtractAngularDimensionReferences: 无法从元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element1.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 提取边缘参考");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			list.Add(val);
			Reference val2 = ExtractElementEdgeReference(element2, direction2, vertex);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("ExtractAngularDimensionReferences: 无法从元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(element2.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" 提取边缘参考");
				LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			list.Add(val2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(46, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("ExtractAngularDimensionReferences: 成功提取 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 个几何参考");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("ExtractAngularDimensionReferences: 提取参考失败 - " + ex.Message);
			return null;
		}
	}

	private Reference? ExtractElementEdgeReference(Element element, XYZ targetDirection, XYZ vertex)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		try
		{
			Wall val = (Wall)(object)((element is Wall) ? element : null);
			if (val != null)
			{
				return ExtractWallEdgeReference(val, targetDirection, vertex);
			}
			Options val2 = new Options
			{
				ComputeReferences = true,
				DetailLevel = (ViewDetailLevel)2
			};
			GeometryElement val3 = element.get_Geometry(val2);
			if ((GeometryObject)(object)val3 == (GeometryObject)null)
			{
				return null;
			}
			Reference val4 = null;
			double num = -1.0;
			foreach (GeometryObject item in val3)
			{
				Solid val5 = (Solid)(object)((item is Solid) ? item : null);
				if (val5 != null)
				{
					foreach (Edge edge in val5.Edges)
					{
						Edge val6 = edge;
						Curve val7 = val6.AsCurve();
						Line val8 = (Line)(object)((val7 is Line) ? val7 : null);
						if (val8 != null)
						{
							XYZ val9 = val8.Direction.Normalize();
							double num2 = Math.Abs(val9.DotProduct(targetDirection.Normalize()));
							if (num2 > num)
							{
								num = num2;
								val4 = val6.Reference;
							}
						}
					}
					continue;
				}
				GeometryInstance val10 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val10 == null)
				{
					continue;
				}
				GeometryElement instanceGeometry = val10.GetInstanceGeometry();
				if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
				{
					continue;
				}
				foreach (GeometryObject item2 in instanceGeometry)
				{
					Solid val11 = (Solid)(object)((item2 is Solid) ? item2 : null);
					if (val11 == null)
					{
						continue;
					}
					foreach (Edge edge2 in val11.Edges)
					{
						Edge val12 = edge2;
						Curve val13 = val12.AsCurve();
						Line val14 = (Line)(object)((val13 is Line) ? val13 : null);
						if (val14 != null)
						{
							XYZ val15 = val14.Direction.Normalize();
							double num3 = Math.Abs(val15.DotProduct(targetDirection.Normalize()));
							if (num3 > num)
							{
								num = num3;
								val4 = val12.Reference;
							}
						}
					}
				}
			}
			if (val4 != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 2);
				defaultInterpolatedStringHandler.AppendLiteral("ExtractElementEdgeReference: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(element.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 找到最佳边缘参考 (方向匹配度: ");
				defaultInterpolatedStringHandler.AppendFormatted(num, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return val4;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("ExtractElementEdgeReference: 元素 ");
			defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(element.Id);
			defaultInterpolatedStringHandler2.AppendLiteral(" 未找到合适的边缘参考");
			LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
			return null;
		}
		catch (Exception ex)
		{
			LogWarning("ExtractElementEdgeReference: 提取边缘参考失败 - " + ex.Message);
			return null;
		}
	}

	private Reference? ExtractWallEdgeReference(Wall wall, XYZ targetDirection, XYZ vertex)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0091: Expected O, but got Unknown
		try
		{
			Options val = new Options
			{
				ComputeReferences = true,
				DetailLevel = (ViewDetailLevel)2
			};
			GeometryElement val2 = ((Element)wall).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				return null;
			}
			Reference val3 = null;
			double num = -1.0;
			double value = double.MaxValue;
			foreach (GeometryObject item in val2)
			{
				Solid val4 = (Solid)(object)((item is Solid) ? item : null);
				if (val4 == null)
				{
					continue;
				}
				foreach (Edge edge in val4.Edges)
				{
					Edge val5 = edge;
					Curve val6 = val5.AsCurve();
					Line val7 = (Line)(object)((val6 is Line) ? val6 : null);
					if (val7 != null)
					{
						XYZ val8 = val7.Direction.Normalize();
						XYZ val9 = (((Curve)val7).GetEndPoint(0) + ((Curve)val7).GetEndPoint(1)) / 2.0;
						double num2 = val9.DistanceTo(vertex);
						double num3 = Math.Abs(val8.DotProduct(targetDirection.Normalize()));
						double num4 = num3 - num2 / 100.0;
						if (num4 > num)
						{
							num = num4;
							val3 = val5.Reference;
							value = num2;
						}
					}
				}
			}
			if (val3 != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(51, 3);
				defaultInterpolatedStringHandler.AppendLiteral("ExtractWallEdgeReference: 墙 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)wall).Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 找到边缘参考 (匹配度: ");
				defaultInterpolatedStringHandler.AppendFormatted(num, "F3");
				defaultInterpolatedStringHandler.AppendLiteral(", 距离顶点: ");
				defaultInterpolatedStringHandler.AppendFormatted(value, "F2");
				defaultInterpolatedStringHandler.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return val3;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogWarning("ExtractWallEdgeReference: 提取墙边缘参考失败 - " + ex.Message);
			return null;
		}
	}

	public object? CreateSpotDimension(object document, object view, int elementId, object point, string @operator, object? bend = null, object? end = null, bool hasLeader = true, int? spotDimensionTypeId = null)
	{
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Expected O, but got Unknown
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Expected O, but got Unknown
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f5: Expected O, but got Unknown
		//IL_01c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSpotDimension: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("CreateSpotDimension: view 不是 View 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)elementId));
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateSpotDimension: 找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(elementId);
				defaultInterpolatedStringHandler.AppendLiteral(" 的元素");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (!ParsePosition(point, out XYZ xyz))
			{
				LogError("CreateSpotDimension: 无法解析点坐标参数");
				return null;
			}
			XYZ val3 = new XYZ(xyz.X / 304.8, xyz.Y / 304.8, xyz.Z / 304.8);
			XYZ val4 = (XYZ)((bend == null || !ParsePosition(bend, out XYZ xyz2)) ? ((object)val3.Add(new XYZ(5.0, 5.0, 0.0))) : ((object)new XYZ(xyz2.X / 304.8, xyz2.Y / 304.8, xyz2.Z / 304.8)));
			XYZ val5 = (XYZ)((end == null || !ParsePosition(end, out XYZ xyz3)) ? ((object)val4.Add(new XYZ(3.3, 0.0, 0.0))) : ((object)new XYZ(xyz3.X / 304.8, xyz3.Y / 304.8, xyz3.Z / 304.8)));
			ElementId val6;
			if (spotDimensionTypeId.HasValue && spotDimensionTypeId.Value > 0)
			{
				val6 = new ElementId((long)spotDimensionTypeId.Value);
			}
			else
			{
				val6 = GetDefaultSpotDimensionTypeId(val, @operator);
				if (val6 == ElementId.InvalidElementId)
				{
					LogError("CreateSpotDimension: 无法获取默认高程标注类型（" + @operator + "）");
					return null;
				}
			}
			Reference elementGeometryReference = GetElementGeometryReference(element, val3, @operator);
			if (elementGeometryReference == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateSpotDimension: 无法从元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(elementId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 提取几何参考");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			SpotDimension val7 = null;
			string text = @operator.ToUpper();
			string text2 = text;
			if (!(text2 == "SPOT_ELEVATION"))
			{
				if (!(text2 == "SPOT_COORDINATE"))
				{
					if (!(text2 == "SPOT_SLOPE"))
					{
						LogError("CreateSpotDimension: 未知的操作符 '" + @operator + "'");
						return null;
					}
					LogWarning("CreateSpotDimension: NewSpotSlope API 可能不可用，使用 NewSpotElevation 作为替代");
					val7 = val.Create.NewSpotElevation(val2, elementGeometryReference, val3, val4, val5, val3, hasLeader);
				}
				else
				{
					val7 = val.Create.NewSpotCoordinate(val2, elementGeometryReference, val3, val4, val5, val3, hasLeader);
					if (val7 == null)
					{
						XYZ elementCenterLocation = GetElementCenterLocation(element);
						if (elementCenterLocation != null)
						{
							LogInfo("CreateSpotDimension: 用户指定位置失败，尝试使用元素中心位置");
							Reference elementGeometryReference2 = GetElementGeometryReference(element, elementCenterLocation, @operator);
							if (elementGeometryReference2 == null)
							{
								LogWarning("CreateSpotDimension: 无法获取中心位置的几何参考");
							}
							else
							{
								XYZ val8 = RecalculateLeaderPosition(elementCenterLocation, val3, val4);
								XYZ val9 = RecalculateLeaderPosition(elementCenterLocation, val3, val5);
								val7 = val.Create.NewSpotCoordinate(val2, elementGeometryReference2, elementCenterLocation, val8, val9, elementCenterLocation, hasLeader);
							}
						}
					}
				}
			}
			else
			{
				val7 = val.Create.NewSpotElevation(val2, elementGeometryReference, val3, val4, val5, val3, hasLeader);
				if (val7 == null)
				{
					XYZ elementCenterLocation2 = GetElementCenterLocation(element);
					if (elementCenterLocation2 != null)
					{
						LogInfo("CreateSpotDimension: 用户指定位置失败，尝试使用元素中心位置");
						Reference elementGeometryReference3 = GetElementGeometryReference(element, elementCenterLocation2, @operator);
						if (elementGeometryReference3 == null)
						{
							LogWarning("CreateSpotDimension: 无法获取中心位置的几何参考");
						}
						else
						{
							XYZ val10 = RecalculateLeaderPosition(elementCenterLocation2, val3, val4);
							XYZ val11 = RecalculateLeaderPosition(elementCenterLocation2, val3, val5);
							val7 = val.Create.NewSpotElevation(val2, elementGeometryReference3, elementCenterLocation2, val10, val11, elementCenterLocation2, hasLeader);
							if (val7 == null)
							{
								LogWarning("CreateSpotDimension: 策略2失败，无法为管线元素创建 " + @operator + " 高程标注");
							}
						}
					}
				}
			}
			if (val7 == null)
			{
				LogError("CreateSpotDimension: 创建高程标注失败（API 返回 null）");
				return null;
			}
			try
			{
				if (val6 != (ElementId)null && val6 != ElementId.InvalidElementId)
				{
					((Element)val7).ChangeTypeId(val6);
				}
			}
			catch (Exception ex)
			{
				LogWarning("CreateSpotDimension: 应用高程标注类型失败 - " + ex.Message);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("CreateSpotDimension: 成功创建 ");
			defaultInterpolatedStringHandler3.AppendFormatted(@operator);
			defaultInterpolatedStringHandler3.AppendLiteral(" 高程标注，ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val7).Id.Value);
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return val7;
		}
		catch (Exception ex2)
		{
			LogError("CreateSpotDimension 失败: " + ex2.Message);
			return null;
		}
	}

	private Reference? GetElementGeometryReference(Element element, XYZ point, string @operator)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f0: Expected O, but got Unknown
		try
		{
			Level val = (Level)(object)((element is Level) ? element : null);
			if (val != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetElementGeometryReference: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted(element.Id.Value);
				defaultInterpolatedStringHandler.AppendLiteral(" 是标高，使用元素引用");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return new Reference((Element)(object)val);
			}
			Grid val2 = (Grid)(object)((element is Grid) ? element : null);
			if (val2 != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("GetElementGeometryReference: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(element.Id.Value);
				defaultInterpolatedStringHandler2.AppendLiteral(" 是网格，使用元素引用");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				return new Reference((Element)(object)val2);
			}
			FamilyInstance val3 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
			object obj;
			if (val3 != null)
			{
				Category category = element.Category;
				if (category == null)
				{
					obj = null;
				}
				else
				{
					obj = category.Name;
					if (obj != null)
					{
						goto IL_00f8;
					}
				}
				obj = "";
				goto IL_00f8;
			}
			goto IL_0233;
			IL_0233:
			if (element is Wall || element is Floor || element is RoofBase)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("GetElementGeometryReference: 元素 ");
				defaultInterpolatedStringHandler3.AppendFormatted(element.Id.Value);
				defaultInterpolatedStringHandler3.AppendLiteral(" 是墙/楼板/屋顶，提取几何面引用");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
				return ExtractFaceReferenceFromElement(element, point, @operator);
			}
			return ExtractFaceReferenceFromElement(element, point, @operator);
			IL_00f8:
			string text = (string)obj;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(52, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("GetElementGeometryReference: 元素 ");
			defaultInterpolatedStringHandler4.AppendFormatted(element.Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(" 是族实例（类别：");
			defaultInterpolatedStringHandler4.AppendFormatted(text);
			defaultInterpolatedStringHandler4.AppendLiteral("），尝试提取几何面引用");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			if (text.Contains("管道") || text.Contains("管段") || text.Contains("Pipe") || text.Contains("Piping") || text.Contains("风管") || text.Contains("Duct") || text.Contains("管道系统"))
			{
				try
				{
					Reference result = new Reference((Element)(object)val3);
					LogInfo("GetElementGeometryReference: 使用管线元素引用");
					return result;
				}
				catch
				{
					LogInfo("GetElementGeometryReference: 管线元素引用失败，尝试几何面提取");
				}
			}
			Reference val4 = ExtractFaceReferenceFromElement(element, point, @operator);
			if (val4 != null)
			{
				return val4;
			}
			goto IL_0233;
		}
		catch (Exception ex)
		{
			LogError("GetElementGeometryReference 失败: " + ex.Message);
			return null;
		}
	}

	private Reference? ExtractFaceReferenceFromElement(Element element, XYZ point, string @operator)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		try
		{
			Options val = new Options
			{
				ComputeReferences = true,
				DetailLevel = (ViewDetailLevel)2
			};
			GeometryElement val2 = element.get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ExtractFaceReferenceFromElement: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted(element.Id.Value);
				defaultInterpolatedStringHandler.AppendLiteral(" 无法获取几何");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			List<(Face, double)> list = new List<(Face, double)>();
			foreach (GeometryObject item in val2)
			{
				Solid val3 = (Solid)(object)((item is Solid) ? item : null);
				if (val3 != null)
				{
					ProcessSolidForFaces(val3, point, @operator, list);
					continue;
				}
				GeometryInstance val4 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val4 == null)
				{
					continue;
				}
				GeometryElement instanceGeometry = val4.GetInstanceGeometry();
				if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
				{
					continue;
				}
				foreach (GeometryObject item2 in instanceGeometry)
				{
					Solid val5 = (Solid)(object)((item2 is Solid) ? item2 : null);
					if (val5 != null)
					{
						Transform transform = val4.Transform;
						ProcessSolidForFaces(val5, transform.Inverse.OfPoint(point), @operator, list);
					}
				}
			}
			if (list.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("ExtractFaceReferenceFromElement: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(element.Id.Value);
				defaultInterpolatedStringHandler2.AppendLiteral(" 未找到任何平面");
				LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			return list.OrderByDescending<(Face, double), double>(((Face face, double score) f) => f.score).First().Item1.Reference;
		}
		catch (Exception ex)
		{
			LogError("ExtractFaceReferenceFromElement 失败: " + ex.Message);
			return null;
		}
	}

	private void ProcessSolidForFaces(Solid solid, XYZ point, string @operator, List<(Face face, double score)> candidates)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		foreach (Face face in solid.Faces)
		{
			Face val = face;
			PlanarFace val2 = (PlanarFace)(object)((val is PlanarFace) ? val : null);
			if (val2 != null)
			{
				double num = ScoreFaceForSpotDimension(val2, point, @operator);
				if (num > 0.0)
				{
					candidates.Add((val, num));
				}
			}
		}
	}

	private double ScoreFaceForSpotDimension(PlanarFace face, XYZ point, string @operator)
	{
		try
		{
			XYZ val = face.FaceNormal.Normalize();
			double num = 0.0;
			IntersectionResult val2 = ((Face)face).Project(point);
			if (val2 == null)
			{
				num += 5.0;
			}
			else
			{
				XYZ xYZPoint = val2.XYZPoint;
				double num2 = point.DistanceTo(xYZPoint);
				num = ((num2 < 0.01) ? (num + 100.0) : ((num2 < 1.0) ? (num + 80.0) : ((!(num2 < 5.0)) ? (num + 30.0) : (num + 60.0))));
			}
			string text = @operator.ToUpper();
			string text2 = text;
			if (text2 == "SPOT_ELEVATION" || text2 == "SPOT_COORDINATE")
			{
				num = ((Math.Abs(val.Z) > 0.9) ? (num + 40.0) : ((!(Math.Abs(val.X) > 0.9) && !(Math.Abs(val.Y) > 0.9)) ? (num + 20.0) : (num + 30.0)));
			}
			else if (text2 == "SPOT_SLOPE")
			{
				double num3 = Math.Acos(Math.Abs(val.Z));
				if (num3 > 0.05 && num3 < 3.0915926535897933)
				{
					num += 50.0;
					num += num3 * 20.0;
				}
				else
				{
					num += 5.0;
				}
			}
			return num;
		}
		catch
		{
			return 0.0;
		}
	}

	private ElementId GetDefaultSpotDimensionTypeId(Document doc, string @operator)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			List<SpotDimensionType> list = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(SpotDimensionType))).OfType<SpotDimensionType>().ToList();
			if (list.Count == 0)
			{
				return ElementId.InvalidElementId;
			}
			SpotDimensionType val = null;
			foreach (SpotDimensionType item in list)
			{
				string text = ((Element)item).Name ?? "";
				string text2 = @operator.ToUpper();
				string text3 = text2;
				if (!(text3 == "SPOT_ELEVATION"))
				{
					if (!(text3 == "SPOT_COORDINATE"))
					{
						if (text3 == "SPOT_SLOPE" && (text.Contains("坡度") || text.Contains("斜率")) && val == null)
						{
							val = item;
						}
					}
					else if ((text.Contains("坐标") || text.Contains("XYZ")) && val == null)
					{
						val = item;
					}
				}
				else if ((text.Contains("高程点") || text.Contains("标高")) && val == null)
				{
					val = item;
				}
			}
			return ((Element)(val ?? list.First())).Id;
		}
		catch
		{
			return ElementId.InvalidElementId;
		}
	}

	private XYZ? GetElementCenterLocation(Element element)
	{
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		try
		{
			Location location = element.Location;
			LocationPoint val = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val != null)
			{
				return val.Point;
			}
			Location location2 = element.Location;
			LocationCurve val2 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
			if (val2 != null && (GeometryObject)(object)val2.Curve != (GeometryObject)null)
			{
				return val2.Curve.Evaluate(0.5, true);
			}
			BoundingBoxXYZ val3 = element.get_BoundingBox((View)null);
			if (val3 != null)
			{
				return new XYZ((val3.Min.X + val3.Max.X) / 2.0, (val3.Min.Y + val3.Max.Y) / 2.0, (val3.Min.Z + val3.Max.Z) / 2.0);
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementCenterLocation 失败: " + ex.Message);
			return null;
		}
	}

	private XYZ RecalculateLeaderPosition(XYZ newOrigin, XYZ oldOrigin, XYZ originalLeaderPos)
	{
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Expected O, but got Unknown
		try
		{
			XYZ val = new XYZ(originalLeaderPos.X - oldOrigin.X, originalLeaderPos.Y - oldOrigin.Y, originalLeaderPos.Z - oldOrigin.Z);
			return new XYZ(newOrigin.X + val.X, newOrigin.Y + val.Y, newOrigin.Z + val.Z);
		}
		catch
		{
			return originalLeaderPos;
		}
	}
}
