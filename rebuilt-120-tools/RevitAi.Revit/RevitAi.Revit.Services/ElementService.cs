using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using RevitAi.Abstractions.Collision;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using ns0;
using ns6;

using Document = Autodesk.Revit.DB.Document;

using Application = Autodesk.Revit.ApplicationServices.Application;
namespace RevitAi.Revit.Services;

internal sealed class ElementService : IElementService
{
	private readonly UIApplication _uiApplication;

	public ElementService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public object? GetElementById(object document, int elementId)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			ElementId val2 = new ElementId((long)elementId);
			return val.GetElement(val2);
		}
		catch (Exception ex)
		{
			LogError("GetElementById 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetElementByUniqueId(object document, string uniqueId)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			return val.GetElement(uniqueId);
		}
		catch (Exception ex)
		{
			LogError("GetElementByUniqueId 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetAllElements(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).WhereElementIsNotElementType();
			return val2.ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetAllElements 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetElementsByCategory(object document, string categoryName)
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			Category val2 = null;
			foreach (Category item in (CategoryNameMap)val.Settings.Categories)
			{
				Category val3 = item;
				if (val3.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
				{
					val2 = val3;
					break;
				}
			}
			if (val2 == null)
			{
				LogError("类别不存在: " + categoryName);
				LogInfo("提示：请使用 Revit 界面中显示的类别名称（中文），例如：墙、楼板、门、窗等");
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val4 = new FilteredElementCollector(val);
			return val4.OfCategory((BuiltInCategory)val2.Id.Value).WhereElementIsNotElementType().ToElements()
				.Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetElementsByCategory 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetElementsByType(object document, string typeName)
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val);
			IList<Element> source = val2.ToElements();
			return source.Where((Element e) => ((object)e).GetType().Name == typeName || (e.Name?.Contains(typeName) ?? false)).Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetElementsByType 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public bool DeleteElement(object document, object element)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteElement: document 不是 Document 类型");
				return false;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				LogError("DeleteElement: element 不是 Element 类型");
				return false;
			}
			val.Delete(val2.Id);
			return true;
		}
		catch (Exception ex)
		{
			LogError("DeleteElement 失败: " + ex.Message);
			return false;
		}
	}

	public object? CopyElement(object document, object element)
	{
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				return null;
			}
			List<ElementId> list = new List<ElementId> { val2.Id };
			ICollection<ElementId> collection = ElementTransformUtils.CopyElements(val, (ICollection<ElementId>)list, val, Transform.Identity, new CopyPasteOptions());
			if (collection.Count > 0)
			{
				return val.GetElement(collection.First());
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("CopyElement 失败: " + ex.Message);
			return null;
		}
	}

	public bool MoveElement(object document, object element, double x, double y, double z)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return false;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				return false;
			}
			XYZ val3 = XYZ.BasisX * x + XYZ.BasisY * y + XYZ.BasisZ * z;
			ElementTransformUtils.MoveElement(val, val2.Id, val3);
			return true;
		}
		catch (Exception ex)
		{
			LogError("MoveElement 失败: " + ex.Message);
			return false;
		}
	}

	public bool RotateElement(object document, object element, double axisX, double axisY, double axisZ, double angleDegrees)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return false;
			}
			Element val2 = (Element)((element is Element) ? element : null);
			if (val2 == null)
			{
				return false;
			}
			Location location = val2.Location;
			LocationPoint val3 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val3 == null)
			{
				LogError("RotateElement: 元素没有点位置");
				return false;
			}
			XYZ val4 = XYZ.BasisX * axisX + XYZ.BasisY * axisY + XYZ.BasisZ * axisZ;
			double num = angleDegrees * Math.PI / 180.0;
			ElementTransformUtils.RotateElement(val, val2.Id, Line.CreateUnbound(val3.Point, val4), num);
			return true;
		}
		catch (Exception ex)
		{
			LogError("RotateElement 失败: " + ex.Message);
			return false;
		}
	}

	public (double X, double Y, double Z)? GetElementLocation(object element)
	{
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Expected O, but got Unknown
		//IL_01f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Location location = val.Location;
			LocationPoint val2 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val2 != null && val2.Point != null)
			{
				XYZ point = val2.Point;
				return (point.X, point.Y, point.Z);
			}
			Location location2 = val.Location;
			LocationCurve val3 = (LocationCurve)(object)((location2 is LocationCurve) ? location2 : null);
			if (val3 != null && (GeometryObject)(object)val3.Curve != (GeometryObject)null)
			{
				Curve curve = val3.Curve;
				XYZ endPoint = curve.GetEndPoint(0);
				return (endPoint.X, endPoint.Y, endPoint.Z);
			}
			Grid val4 = (Grid)(object)((val is Grid) ? val : null);
			if (val4 != null && (GeometryObject)(object)val4.Curve != (GeometryObject)null)
			{
				Curve curve2 = val4.Curve;
				XYZ endPoint2 = curve2.GetEndPoint(0);
				XYZ endPoint3 = curve2.GetEndPoint(1);
				XYZ val5 = new XYZ((endPoint2.X + endPoint3.X) / 2.0, (endPoint2.Y + endPoint3.Y) / 2.0, (endPoint2.Z + endPoint3.Z) / 2.0);
				return (val5.X, val5.Y, val5.Z);
			}
			try
			{
				BoundingBoxXYZ val6 = val.get_BoundingBox((View)null);
				if (val6 != null)
				{
					XYZ val7 = new XYZ((val6.Min.X + val6.Max.X) / 2.0, (val6.Min.Y + val6.Max.Y) / 2.0, (val6.Min.Z + val6.Max.Z) / 2.0);
					return (val7.X, val7.Y, val7.Z);
				}
			}
			catch
			{
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementLocation 失败: " + ex.Message);
			return null;
		}
	}

	public GridCurveInfo? GetGridCurveInfo(object element)
	{
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Grid val2 = (Grid)(object)((val is Grid) ? val : null);
			if (val2 == null || (GeometryObject)(object)val2.Curve == (GeometryObject)null)
			{
				return null;
			}
			Curve curve = val2.Curve;
			string text = "Line";
			if (curve is Arc)
			{
				text = "Arc";
			}
			XYZ endPoint = curve.GetEndPoint(0);
			XYZ endPoint2 = curve.GetEndPoint(1);
			XYZ val3 = new XYZ(endPoint2.X - endPoint.X, endPoint2.Y - endPoint.Y, endPoint2.Z - endPoint.Z);
			double length = val3.GetLength();
			if (length > 0.0)
			{
				val3 = val3.Normalize();
			}
			GridCurveInfo result = new GridCurveInfo
			{
				CurveType = text,
				StartX = endPoint.X,
				StartY = endPoint.Y,
				StartZ = endPoint.Z,
				EndX = endPoint2.X,
				EndY = endPoint2.Y,
				EndZ = endPoint2.Z,
				DirectionX = val3.X,
				DirectionY = val3.Y,
				DirectionZ = val3.Z,
				Length = length
			};
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[ElementService] 获取轴网曲线信息: 类型=");
			defaultInterpolatedStringHandler.AppendFormatted(text);
			defaultInterpolatedStringHandler.AppendLiteral(", 长度=");
			defaultInterpolatedStringHandler.AppendFormatted(length, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("英尺");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return result;
		}
		catch (Exception ex)
		{
			LogError("GetGridCurveInfo 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetElementCategory(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Category category = val.Category;
			return (category != null) ? category.Name : null;
		}
		catch (Exception ex)
		{
			LogError("GetElementCategory 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetElementName(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			return val.Name;
		}
		catch (Exception ex)
		{
			LogError("GetElementName 失败: " + ex.Message);
			return null;
		}
	}

	public int? GetElementId(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			return (int)val.Id.Value;
		}
		catch (Exception ex)
		{
			LogError("GetElementId 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateStraightWall(object document, double startX, double startY, double endX, double endY, int levelId, double height, int? wallTypeId = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateStraightWall: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateStraightWall: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			WallType val3 = null;
			if (wallTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)wallTypeId.Value));
				val3 = (WallType)(object)((element2 is WallType) ? element2 : null);
			}
			if (val3 == null)
			{
				Element obj = new FilteredElementCollector(val).OfClass(typeof(WallType)).FirstElement();
				val3 = (WallType)(object)((obj is WallType) ? obj : null);
			}
			if (val3 == null)
			{
				LogError("CreateStraightWall: 找不到墙类型");
				return null;
			}
			XYZ val4 = new XYZ(startX, startY, val2.Elevation);
			XYZ val5 = new XYZ(endX, endY, val2.Elevation);
			Line val6 = Line.CreateBound(val4, val5);
			Wall val7 = Wall.Create(val, (Curve)(object)val6, ((Element)val3).Id, ((Element)val2).Id, height, 0.0, false, false);
			if (val7 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(58, 4);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateStraightWall: Wall.Create 返回 null (起点=(");
				defaultInterpolatedStringHandler2.AppendFormatted(startX, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(startY, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler2.AppendFormatted(endX, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(endY, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("))");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(56, 6);
			defaultInterpolatedStringHandler3.AppendLiteral("CreateStraightWall: 成功创建墙体 (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val7).Id.Value);
			defaultInterpolatedStringHandler3.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler3.AppendLiteral(", 起点=(");
			defaultInterpolatedStringHandler3.AppendFormatted(startX, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(startY, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral("), 终点=(");
			defaultInterpolatedStringHandler3.AppendFormatted(endX, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(endY, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return val7;
		}
		catch (Exception ex)
		{
			LogError("CreateStraightWall 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateDoorInWall(object document, int wallId, double positionX, double positionY, int? doorTypeId = null, bool flip = false)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateDoorInWall: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)wallId));
			Wall val2 = (Wall)(object)((element is Wall) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateDoorInWall: 找不到墙体 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(wallId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			FamilySymbol val3 = null;
			if (doorTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)doorTypeId.Value));
				val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
				if (val3 == null && element2 != null)
				{
					string elemTypeName = element2.Name;
					val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(FamilySymbol)).OfCategory((BuiltInCategory)(-2000023L))).Cast<FamilySymbol>().FirstOrDefault((FamilySymbol fs) => ((Element)fs).Name.Equals(elemTypeName));
				}
			}
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateDoorInWall: 找不到门类型 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(doorTypeId ?? (-1));
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			if (!val3.IsActive)
			{
				val3.Activate();
			}
			XYZ val4 = new XYZ(positionX, positionY, 0.0);
			FamilyInstance val5 = ((ItemFactoryBase)val.Create).NewFamilyInstance(val4, val3, (Element)(object)val2, (StructuralType)0);
			if (val5 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(59, 3);
				defaultInterpolatedStringHandler3.AppendLiteral("CreateDoorInWall: NewFamilyInstance 返回 null (位置=(");
				defaultInterpolatedStringHandler3.AppendFormatted(positionX, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(positionY, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("), 墙ID=");
				defaultInterpolatedStringHandler3.AppendFormatted(wallId);
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(44, 4);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateDoorInWall: 成功创建门 (ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val5).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler4.AppendLiteral(", 位置=(");
			defaultInterpolatedStringHandler4.AppendFormatted(positionX, "F4");
			defaultInterpolatedStringHandler4.AppendLiteral(", ");
			defaultInterpolatedStringHandler4.AppendFormatted(positionY, "F4");
			defaultInterpolatedStringHandler4.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val5;
		}
		catch (Exception ex)
		{
			LogError("CreateDoorInWall 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateWindowInWall(object document, int wallId, double positionX, double positionY, int? windowTypeId = null, double heightOffset = 4.0)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateWindowInWall: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)wallId));
			Wall val2 = (Wall)(object)((element is Wall) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateWindowInWall: 找不到墙体 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(wallId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			FamilySymbol val3 = null;
			if (windowTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)windowTypeId.Value));
				val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
				if (val3 == null && element2 != null)
				{
					string elemTypeName = element2.Name;
					val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(FamilySymbol)).OfCategory((BuiltInCategory)(-2000014L))).Cast<FamilySymbol>().FirstOrDefault((FamilySymbol fs) => ((Element)fs).Name.Equals(elemTypeName));
				}
			}
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateWindowInWall: 找不到窗类型 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(windowTypeId ?? (-1));
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			if (!val3.IsActive)
			{
				val3.Activate();
			}
			XYZ val4 = new XYZ(positionX, positionY, heightOffset);
			FamilyInstance val5 = ((ItemFactoryBase)val.Create).NewFamilyInstance(val4, val3, (Element)(object)val2, (StructuralType)0);
			if (val5 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(61, 3);
				defaultInterpolatedStringHandler3.AppendLiteral("CreateWindowInWall: NewFamilyInstance 返回 null (位置=(");
				defaultInterpolatedStringHandler3.AppendFormatted(positionX, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(positionY, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("), 墙ID=");
				defaultInterpolatedStringHandler3.AppendFormatted(wallId);
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(46, 4);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateWindowInWall: 成功创建窗 (ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val5).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler4.AppendLiteral(", 位置=(");
			defaultInterpolatedStringHandler4.AppendFormatted(positionX, "F4");
			defaultInterpolatedStringHandler4.AppendLiteral(", ");
			defaultInterpolatedStringHandler4.AppendFormatted(positionY, "F4");
			defaultInterpolatedStringHandler4.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val5;
		}
		catch (Exception ex)
		{
			LogError("CreateWindowInWall 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateColumn(object document, double positionX, double positionY, int levelId, double height = 0.0, int? columnTypeId = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateColumn: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateColumn: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			FamilySymbol val3 = null;
			if (columnTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)columnTypeId.Value));
				val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
			}
			if (val3 == null)
			{
				Element obj = new FilteredElementCollector(val).OfClass(typeof(FamilySymbol)).OfCategory((BuiltInCategory)(-2001330L)).FirstElement();
				val3 = (FamilySymbol)(object)((obj is FamilySymbol) ? obj : null);
			}
			if (val3 == null)
			{
				LogError("CreateColumn: 找不到柱类型");
				return null;
			}
			if (!val3.IsActive)
			{
				val3.Activate();
			}
			XYZ val4 = new XYZ(positionX, positionY, val2.Elevation);
			FamilyInstance val5 = ((ItemFactoryBase)val.Create).NewFamilyInstance(val4, val3, val2, (StructuralType)3);
			if (val5 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(54, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateColumn: NewFamilyInstance 返回 null (位置=(");
				defaultInterpolatedStringHandler2.AppendFormatted(positionX, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(positionY, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("), 标高=");
				defaultInterpolatedStringHandler2.AppendFormatted(levelId);
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(40, 4);
			defaultInterpolatedStringHandler3.AppendLiteral("CreateColumn: 成功创建柱 (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val5).Id.Value);
			defaultInterpolatedStringHandler3.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler3.AppendLiteral(", 位置=(");
			defaultInterpolatedStringHandler3.AppendFormatted(positionX, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(positionY, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return val5;
		}
		catch (Exception ex)
		{
			LogError("CreateColumn 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateBeam(object document, double startX, double startY, double endX, double endY, int levelId, int? beamTypeId = null)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_008c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Expected O, but got Unknown
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateBeam: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateBeam: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			FamilySymbol val3 = null;
			if (beamTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)beamTypeId.Value));
				val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
			}
			if (val3 == null)
			{
				Element obj = new FilteredElementCollector(val).OfClass(typeof(FamilySymbol)).OfCategory((BuiltInCategory)(-2001320L)).FirstElement();
				val3 = (FamilySymbol)(object)((obj is FamilySymbol) ? obj : null);
			}
			if (val3 == null)
			{
				LogError("CreateBeam: 找不到梁类型");
				return null;
			}
			if (!val3.IsActive)
			{
				val3.Activate();
			}
			XYZ val4 = new XYZ(startX, startY, val2.Elevation);
			XYZ val5 = new XYZ(endX, endY, val2.Elevation);
			Line val6 = Line.CreateBound(val4, val5);
			FamilyInstance val7 = val.Create.NewFamilyInstance((Curve)(object)val6, val3, val2, (StructuralType)1);
			if (val7 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(61, 5);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateBeam: NewFamilyInstance 返回 null (起点=(");
				defaultInterpolatedStringHandler2.AppendFormatted(startX, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(startY, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler2.AppendFormatted(endX, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(endY, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral("), 标高=");
				defaultInterpolatedStringHandler2.AppendFormatted(levelId);
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(47, 6);
			defaultInterpolatedStringHandler3.AppendLiteral("CreateBeam: 成功创建梁 (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val7).Id.Value);
			defaultInterpolatedStringHandler3.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler3.AppendLiteral(", 起点=(");
			defaultInterpolatedStringHandler3.AppendFormatted(startX, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(startY, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral("), 终点=(");
			defaultInterpolatedStringHandler3.AppendFormatted(endX, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral(", ");
			defaultInterpolatedStringHandler3.AppendFormatted(endY, "F4");
			defaultInterpolatedStringHandler3.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return val7;
		}
		catch (Exception ex)
		{
			LogError("CreateBeam 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreatePipe(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int pipeTypeId, int? systemTypeId = null, double diameterMM = 100.0)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Expected O, but got Unknown
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b0: Expected O, but got Unknown
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Expected O, but got Unknown
		//IL_00fe: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreatePipe: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreatePipe: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			PipeType val3 = null;
			Element element2 = val.GetElement(new ElementId((long)pipeTypeId));
			val3 = (PipeType)(object)((element2 is PipeType) ? element2 : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreatePipe: 找不到管道类型 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(pipeTypeId);
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			PipingSystemType val4 = null;
			if (systemTypeId.HasValue)
			{
				Element element3 = val.GetElement(new ElementId((long)systemTypeId.Value));
				val4 = (PipingSystemType)(object)((element3 is PipingSystemType) ? element3 : null);
			}
			if (val4 == null)
			{
				Element obj = new FilteredElementCollector(val).OfClass(typeof(PipingSystemType)).FirstElement();
				val4 = (PipingSystemType)(object)((obj is PipingSystemType) ? obj : null);
				if (val4 == null)
				{
					LogError("CreatePipe: 找不到管道系统类型");
					return null;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("CreatePipe: 使用默认系统类型 '");
				defaultInterpolatedStringHandler3.AppendFormatted(((Element)val4).Name);
				defaultInterpolatedStringHandler3.AppendLiteral("' (ID: ");
				defaultInterpolatedStringHandler3.AppendFormatted(((Element)val4).Id.Value);
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			XYZ val5 = new XYZ(startX, startY, startZ);
			XYZ val6 = new XYZ(endX, endY, endZ);
			Pipe val7 = Pipe.Create(val, ((Element)val4).Id, ((Element)val3).Id, ((Element)val2).Id, val5, val6);
			if (val7 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(54, 6);
				defaultInterpolatedStringHandler4.AppendLiteral("CreatePipe: Pipe.Create 返回 null (起点=(");
				defaultInterpolatedStringHandler4.AppendFormatted(startX, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(startY, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(startZ, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler4.AppendFormatted(endX, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(endY, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(endZ, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral("))");
				LogError(defaultInterpolatedStringHandler4.ToStringAndClear());
				return null;
			}
			SetPipeDiameter(val7, diameterMM);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(44, 4);
			defaultInterpolatedStringHandler5.AppendLiteral("CreatePipe: 成功创建管道 (ID: ");
			defaultInterpolatedStringHandler5.AppendFormatted(((Element)val7).Id.Value);
			defaultInterpolatedStringHandler5.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler5.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler5.AppendLiteral(", 系统类型=");
			defaultInterpolatedStringHandler5.AppendFormatted(((Element)val4).Name);
			defaultInterpolatedStringHandler5.AppendLiteral(", 直径=");
			defaultInterpolatedStringHandler5.AppendFormatted(diameterMM);
			defaultInterpolatedStringHandler5.AppendLiteral("mm)");
			LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
			return val7;
		}
		catch (Exception ex)
		{
			LogError("CreatePipe 失败: " + ex.Message);
			return null;
		}
	}

	private void SetPipeDiameter(Pipe pipe, double diameterMM)
	{
		try
		{
			Parameter val = ((Element)pipe).get_Parameter((BuiltInParameter)(-1140225L));
			if (val != null && !((APIObject)val).IsReadOnly)
			{
				val.Set(diameterMM / 304.8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetPipeDiameter: 成功设置管道直径为 ");
				defaultInterpolatedStringHandler.AppendFormatted(diameterMM);
				defaultInterpolatedStringHandler.AppendLiteral("mm");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				LogWarning("SetPipeDiameter: 无法设置管道直径（参数为只读或不存在）");
			}
		}
		catch (Exception ex)
		{
			LogError("SetPipeDiameter 失败: " + ex.Message);
		}
	}

	public IEnumerable<object> GetDuctTypes(object document)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetDuctTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<Class311<long, string, string>> list = (from DuctType ductType in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(DuctType))
				select new Class311<long, string, string>(((Element)ductType).Id.Value, ((Element)ductType).Name, GetDuctShape(ductType))).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetDuctTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个风管类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetDuctTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	private string GetDuctShape(DuctType ductType)
	{
		try
		{
			Parameter val = ((Element)ductType).get_Parameter((BuiltInParameter)(-1002002L));
			if (val == null)
			{
				return "未知";
			}
			string text = val.AsString();
			if (string.IsNullOrEmpty(text))
			{
				return "未知";
			}
			if (text.Contains("椭圆") || text.Contains("Oval", StringComparison.OrdinalIgnoreCase))
			{
				return "椭圆形";
			}
			if (text.Contains("圆形") || text.Contains("Round", StringComparison.OrdinalIgnoreCase))
			{
				return "圆形";
			}
			if (text.Contains("矩形") || text.Contains("Rectangular", StringComparison.OrdinalIgnoreCase))
			{
				return "矩形";
			}
			return text;
		}
		catch (Exception ex)
		{
			LogWarning("GetDuctShape 失败: " + ex.Message);
			return "未知";
		}
	}

	public IEnumerable<object> GetDuctSystemTypes(object document)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetDuctSystemTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<MEPSystemType> list = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(MEPSystemType))).Cast<MEPSystemType>().Where(delegate(MEPSystemType st)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				//IL_0007: Invalid comparison between Unknown and I4
				//IL_000a: Unknown result type (might be due to invalid IL or missing references)
				//IL_0010: Invalid comparison between Unknown and I4
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0019: Invalid comparison between Unknown and I4
				//IL_001c: Unknown result type (might be due to invalid IL or missing references)
				//IL_0022: Invalid comparison between Unknown and I4
				return (int)st.SystemClassification == 1 || (int)st.SystemClassification == 2 || (int)st.SystemClassification == 3 || (int)st.SystemClassification == 4;
			}).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetDuctSystemTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个风管系统类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetDuctSystemTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? CreateDuct(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int ductTypeId, int? systemTypeId = null, double widthMM = 300.0, double heightMM = 300.0, double diameterMM = 0.0)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e9: Expected O, but got Unknown
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateDuct: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateDuct: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			Element element2 = val.GetElement(new ElementId((long)ductTypeId));
			DuctType val3 = (DuctType)(object)((element2 is DuctType) ? element2 : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateDuct: 找不到风管类型 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(ductTypeId);
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			MEPSystemType val4 = null;
			if (systemTypeId.HasValue)
			{
				Element element3 = val.GetElement(new ElementId((long)systemTypeId.Value));
				val4 = (MEPSystemType)(object)((element3 is MEPSystemType) ? element3 : null);
				if (val4 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("CreateDuct: 找不到系统类型 ID ");
					defaultInterpolatedStringHandler3.AppendFormatted(systemTypeId.Value);
					LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
					return null;
				}
			}
			else
			{
				MEPSystemType val5 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(MEPSystemType))).Cast<MEPSystemType>().Where(delegate(MEPSystemType st)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Invalid comparison between Unknown and I4
					return (int)st.SystemClassification == 1;
				}).FirstOrDefault();
				val4 = val5;
				if (val4 == null)
				{
					LogError("CreateDuct: 找不到默认的送风系统类型");
					return null;
				}
			}
			XYZ val6 = new XYZ(startX, startY, startZ);
			XYZ val7 = new XYZ(endX, endY, endZ);
			Duct val8 = Duct.Create(val, ((Element)val4).Id, ((Element)val3).Id, ((Element)val2).Id, val6, val7);
			if (val8 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(54, 6);
				defaultInterpolatedStringHandler4.AppendLiteral("CreateDuct: Duct.Create 返回 null (起点=(");
				defaultInterpolatedStringHandler4.AppendFormatted(startX, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(startY, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(startZ, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler4.AppendFormatted(endX, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(endY, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral(", ");
				defaultInterpolatedStringHandler4.AppendFormatted(endZ, "F4");
				defaultInterpolatedStringHandler4.AppendLiteral("))");
				LogError(defaultInterpolatedStringHandler4.ToStringAndClear());
				return null;
			}
			if (diameterMM > 0.0)
			{
				SetDuctDiameter(val8, diameterMM);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(39, 3);
				defaultInterpolatedStringHandler5.AppendLiteral("CreateDuct: 成功创建圆形风管 (ID: ");
				defaultInterpolatedStringHandler5.AppendFormatted(((Element)val8).Id.Value);
				defaultInterpolatedStringHandler5.AppendLiteral(", 类型=");
				defaultInterpolatedStringHandler5.AppendFormatted(((Element)val3).Name);
				defaultInterpolatedStringHandler5.AppendLiteral(", 直径=");
				defaultInterpolatedStringHandler5.AppendFormatted(diameterMM);
				defaultInterpolatedStringHandler5.AppendLiteral("mm)");
				LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
			}
			else
			{
				SetDuctWidthAndHeight(val8, widthMM, heightMM);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(40, 4);
				defaultInterpolatedStringHandler6.AppendLiteral("CreateDuct: 成功创建矩形风管 (ID: ");
				defaultInterpolatedStringHandler6.AppendFormatted(((Element)val8).Id.Value);
				defaultInterpolatedStringHandler6.AppendLiteral(", 类型=");
				defaultInterpolatedStringHandler6.AppendFormatted(((Element)val3).Name);
				defaultInterpolatedStringHandler6.AppendLiteral(", 尺寸=");
				defaultInterpolatedStringHandler6.AppendFormatted(widthMM);
				defaultInterpolatedStringHandler6.AppendLiteral("x");
				defaultInterpolatedStringHandler6.AppendFormatted(heightMM);
				defaultInterpolatedStringHandler6.AppendLiteral("mm)");
				LogInfo(defaultInterpolatedStringHandler6.ToStringAndClear());
			}
			return val8;
		}
		catch (Exception ex)
		{
			LogError("CreateDuct 失败: " + ex.Message);
			return null;
		}
	}

	private void SetDuctWidthAndHeight(Duct duct, double widthMM, double heightMM)
	{
		try
		{
			Parameter val = ((Element)duct).get_Parameter((BuiltInParameter)(-1114101L));
			if (val != null && !((APIObject)val).IsReadOnly)
			{
				val.Set(widthMM / 304.8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetDuctWidthAndHeight: 成功设置风管宽度为 ");
				defaultInterpolatedStringHandler.AppendFormatted(widthMM);
				defaultInterpolatedStringHandler.AppendLiteral("mm");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				LogWarning("SetDuctWidthAndHeight: 无法设置风管宽度（参数为只读或不存在）");
			}
			Parameter val2 = ((Element)duct).get_Parameter((BuiltInParameter)(-1114102L));
			if (val2 != null && !((APIObject)val2).IsReadOnly)
			{
				val2.Set(heightMM / 304.8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(35, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("SetDuctWidthAndHeight: 成功设置风管高度为 ");
				defaultInterpolatedStringHandler2.AppendFormatted(heightMM);
				defaultInterpolatedStringHandler2.AppendLiteral("mm");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				LogWarning("SetDuctWidthAndHeight: 无法设置风管高度（参数为只读或不存在）");
			}
		}
		catch (Exception ex)
		{
			LogError("SetDuctWidthAndHeight 失败: " + ex.Message);
		}
	}

	private void SetDuctDiameter(Duct duct, double diameterMM)
	{
		try
		{
			Parameter val = ((Element)duct).get_Parameter((BuiltInParameter)(-1140225L));
			if (val != null && !((APIObject)val).IsReadOnly)
			{
				val.Set(diameterMM / 304.8);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetDuctDiameter: 成功设置风管直径为 ");
				defaultInterpolatedStringHandler.AppendFormatted(diameterMM);
				defaultInterpolatedStringHandler.AppendLiteral("mm");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				LogWarning("SetDuctDiameter: 无法设置风管直径（参数为只读或不存在）");
			}
		}
		catch (Exception ex)
		{
			LogError("SetDuctDiameter 失败: " + ex.Message);
		}
	}

	public object? CreateFloorByProfile(object document, IEnumerable<(double X, double Y)> points, int levelId, int? floorTypeId = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Expected O, but got Unknown
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Expected O, but got Unknown
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateFloorByProfile: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateFloorByProfile: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			FloorType val3 = null;
			if (floorTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)floorTypeId.Value));
				val3 = (FloorType)(object)((element2 is FloorType) ? element2 : null);
			}
			if (val3 == null)
			{
				Element obj = new FilteredElementCollector(val).OfClass(typeof(FloorType)).FirstElement();
				val3 = (FloorType)(object)((obj is FloorType) ? obj : null);
			}
			if (val3 == null)
			{
				LogError("CreateFloorByProfile: 找不到楼板类型");
				return null;
			}
			List<Curve> list = new List<Curve>();
			List<(double, double)> list2 = points.ToList();
			if (list2.Count < 3)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(42, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateFloorByProfile: 轮廓点数量不足（至少需要3个点，当前");
				defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral("个）");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			int num = 0;
			(double, double) tuple;
			(double, double) tuple2;
			while (true)
			{
				if (num < list2.Count)
				{
					tuple = list2[num];
					tuple2 = list2[(num + 1) % list2.Count];
					double num2 = tuple.Item1 - tuple2.Item1;
					double num3 = tuple.Item2 - tuple2.Item2;
					double num4 = num2 * num2 + num3 * num3;
					if (num4 < 1E-10)
					{
						break;
					}
					num++;
					continue;
				}
				for (int i = 0; i < list2.Count; i++)
				{
					(double, double) tuple3 = list2[i];
					(double, double) tuple4 = list2[(i + 1) % list2.Count];
					XYZ val4 = new XYZ(tuple3.Item1, tuple3.Item2, val2.Elevation);
					XYZ val5 = new XYZ(tuple4.Item1, tuple4.Item2, val2.Elevation);
					Line item = Line.CreateBound(val4, val5);
					list.Add((Curve)(object)item);
				}
				CurveLoop val6 = null;
				try
				{
					val6 = CurveLoop.Create((IList<Curve>)list);
				}
				catch (ArgumentException ex)
				{
					LogError("CreateFloorByProfile: CurveLoop.Create 失败 - 轮廓不是有效的闭合曲线: " + ex.Message);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 4);
					defaultInterpolatedStringHandler3.AppendLiteral("  点数: ");
					defaultInterpolatedStringHandler3.AppendFormatted(list2.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(", 标高: ");
					defaultInterpolatedStringHandler3.AppendFormatted(((Element)val2).Name);
					defaultInterpolatedStringHandler3.AppendLiteral(" (ID: ");
					defaultInterpolatedStringHandler3.AppendFormatted(levelId);
					defaultInterpolatedStringHandler3.AppendLiteral("), 楼板类型: ");
					defaultInterpolatedStringHandler3.AppendFormatted(((Element)val3).Name);
					LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
					for (int j = 0; j < list2.Count; j++)
					{
						(double, double) tuple5 = list2[j];
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(11, 3);
						defaultInterpolatedStringHandler4.AppendLiteral("  点");
						defaultInterpolatedStringHandler4.AppendFormatted(j);
						defaultInterpolatedStringHandler4.AppendLiteral(": X=");
						defaultInterpolatedStringHandler4.AppendFormatted(tuple5.Item1, "F4");
						defaultInterpolatedStringHandler4.AppendLiteral(", Y=");
						defaultInterpolatedStringHandler4.AppendFormatted(tuple5.Item2, "F4");
						LogError(defaultInterpolatedStringHandler4.ToStringAndClear());
					}
					return null;
				}
				Floor val7 = Floor.Create(val, (IList<CurveLoop>)new List<CurveLoop> { val6 }, ((Element)val3).Id, ((Element)val2).Id, false, (Line)null, 0.0);
				if (val7 == null)
				{
					LogError("CreateFloorByProfile: Floor.Create 返回 null (可能原因: 轮廓自相交、面积过小、或楼板类型不支持)");
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(30, 4);
					defaultInterpolatedStringHandler5.AppendLiteral("  详细信息: 标高=");
					defaultInterpolatedStringHandler5.AppendFormatted(((Element)val2).Name);
					defaultInterpolatedStringHandler5.AppendLiteral(" (ID: ");
					defaultInterpolatedStringHandler5.AppendFormatted(levelId);
					defaultInterpolatedStringHandler5.AppendLiteral("), 楼板类型=");
					defaultInterpolatedStringHandler5.AppendFormatted(((Element)val3).Name);
					defaultInterpolatedStringHandler5.AppendLiteral(", 点数=");
					defaultInterpolatedStringHandler5.AppendFormatted(list2.Count);
					LogError(defaultInterpolatedStringHandler5.ToStringAndClear());
					for (int k = 0; k < list2.Count; k++)
					{
						(double, double) tuple6 = list2[k];
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(11, 3);
						defaultInterpolatedStringHandler6.AppendLiteral("  点");
						defaultInterpolatedStringHandler6.AppendFormatted(k);
						defaultInterpolatedStringHandler6.AppendLiteral(": X=");
						defaultInterpolatedStringHandler6.AppendFormatted(tuple6.Item1, "F4");
						defaultInterpolatedStringHandler6.AppendLiteral(", Y=");
						defaultInterpolatedStringHandler6.AppendFormatted(tuple6.Item2, "F4");
						LogError(defaultInterpolatedStringHandler6.ToStringAndClear());
					}
					return null;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(45, 3);
				defaultInterpolatedStringHandler7.AppendLiteral("CreateFloorByProfile: 成功创建楼板 (ID: ");
				defaultInterpolatedStringHandler7.AppendFormatted(((Element)val7).Id.Value);
				defaultInterpolatedStringHandler7.AppendLiteral(", 类型=");
				defaultInterpolatedStringHandler7.AppendFormatted(((Element)val3).Name);
				defaultInterpolatedStringHandler7.AppendLiteral(", 标高=");
				defaultInterpolatedStringHandler7.AppendFormatted(levelId);
				defaultInterpolatedStringHandler7.AppendLiteral(")");
				LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
				return val7;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler8.AppendLiteral("CreateFloorByProfile: 相邻点重合（点");
			defaultInterpolatedStringHandler8.AppendFormatted(num);
			defaultInterpolatedStringHandler8.AppendLiteral("和点");
			defaultInterpolatedStringHandler8.AppendFormatted((num + 1) % list2.Count);
			defaultInterpolatedStringHandler8.AppendLiteral("）");
			LogError(defaultInterpolatedStringHandler8.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(9, 3);
			defaultInterpolatedStringHandler9.AppendLiteral("  点");
			defaultInterpolatedStringHandler9.AppendFormatted(num);
			defaultInterpolatedStringHandler9.AppendLiteral(": (");
			defaultInterpolatedStringHandler9.AppendFormatted(tuple.Item1, "F4");
			defaultInterpolatedStringHandler9.AppendLiteral(", ");
			defaultInterpolatedStringHandler9.AppendFormatted(tuple.Item2, "F4");
			defaultInterpolatedStringHandler9.AppendLiteral(")");
			LogError(defaultInterpolatedStringHandler9.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(9, 3);
			defaultInterpolatedStringHandler10.AppendLiteral("  点");
			defaultInterpolatedStringHandler10.AppendFormatted((num + 1) % list2.Count);
			defaultInterpolatedStringHandler10.AppendLiteral(": (");
			defaultInterpolatedStringHandler10.AppendFormatted(tuple2.Item1, "F4");
			defaultInterpolatedStringHandler10.AppendLiteral(", ");
			defaultInterpolatedStringHandler10.AppendFormatted(tuple2.Item2, "F4");
			defaultInterpolatedStringHandler10.AppendLiteral(")");
			LogError(defaultInterpolatedStringHandler10.ToStringAndClear());
			return null;
		}
		catch (Exception ex2)
		{
			LogError("CreateFloorByProfile 失败: " + ex2.Message);
			return null;
		}
	}

	public IEnumerable<object> GetPipeTypes(object document)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetPipeTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			IEnumerable<object> enumerable = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipeType))).Cast<PipeType>().Cast<object>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetPipeTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(enumerable.Count());
			defaultInterpolatedStringHandler.AppendLiteral(" 个管道类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return enumerable;
		}
		catch (Exception ex)
		{
			LogError("GetPipeTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetPipingSystemTypes(object document)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetPipingSystemTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			IEnumerable<object> enumerable = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(PipingSystemType))).Cast<PipingSystemType>().Cast<object>();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetPipingSystemTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(enumerable.Count());
			defaultInterpolatedStringHandler.AppendLiteral(" 个管道系统类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return enumerable;
		}
		catch (Exception ex)
		{
			LogError("GetPipingSystemTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetCableTrayTypes(object document)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetCableTrayTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<Class312<long, string, string>> list = (from CableTrayType ct in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(CableTrayType))
				select new Class312<long, string, string>(((Element)ct).Id.Value, ((Element)ct).Name, GetCableTrayPartType(ct))).ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetCableTrayTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个桥架类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetCableTrayTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	private string GetCableTrayPartType(CableTrayType cableTrayType)
	{
		try
		{
			if (cableTrayType.IsWithFitting)
			{
				return "带配件";
			}
			return "无配件";
		}
		catch (Exception ex)
		{
			LogWarning("GetCableTrayPartType 失败: " + ex.Message);
			return "未知";
		}
	}

	public object? CreateCableTray(object document, double startX, double startY, double startZ, double endX, double endY, double endZ, int levelId, int cableTrayTypeId, double widthMM = 300.0, double heightMM = 150.0)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateCableTray: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateCableTray: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			Element element2 = val.GetElement(new ElementId((long)cableTrayTypeId));
			CableTrayType val3 = (CableTrayType)(object)((element2 is CableTrayType) ? element2 : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateCableTray: 找不到桥架类型 ID ");
				defaultInterpolatedStringHandler2.AppendFormatted(cableTrayTypeId);
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			double num = widthMM / 304.8;
			double num2 = heightMM / 304.8;
			XYZ val4 = new XYZ(startX, startY, startZ);
			XYZ val5 = new XYZ(endX, endY, endZ);
			CableTray val6 = CableTray.Create(val, ((Element)val3).Id, val4, val5, ((Element)val2).Id);
			if (val6 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(64, 6);
				defaultInterpolatedStringHandler3.AppendLiteral("CreateCableTray: CableTray.Create 返回 null (起点=(");
				defaultInterpolatedStringHandler3.AppendFormatted(startX, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(startY, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(startZ, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler3.AppendFormatted(endX, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(endY, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(endZ, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("))");
				LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
				return null;
			}
			Parameter val7 = ((Element)val6).get_Parameter((BuiltInParameter)(-1114101L));
			if (val7 != null && !((APIObject)val7).IsReadOnly)
			{
				val7.Set(num);
			}
			Parameter val8 = ((Element)val6).get_Parameter((BuiltInParameter)(-1114102L));
			if (val8 != null && !((APIObject)val8).IsReadOnly)
			{
				val8.Set(num2);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(49, 4);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateCableTray: 成功创建桥架 (ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val6).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(", 类型=");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler4.AppendLiteral(", 宽度=");
			defaultInterpolatedStringHandler4.AppendFormatted(widthMM);
			defaultInterpolatedStringHandler4.AppendLiteral("mm, 高度=");
			defaultInterpolatedStringHandler4.AppendFormatted(heightMM);
			defaultInterpolatedStringHandler4.AppendLiteral("mm)");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val6;
		}
		catch (Exception ex)
		{
			LogError("CreateCableTray 失败: " + ex.Message);
			return null;
		}
	}

	public int DeleteElements(object document, IEnumerable<int> elementIds)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteElements: document 不是 Document 类型");
				return 0;
			}
			int num = 0;
			foreach (int elementId in elementIds)
			{
				try
				{
					ElementId val2 = new ElementId((long)elementId);
					val.Delete(val2);
					num++;
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 2);
					defaultInterpolatedStringHandler.AppendLiteral("DeleteElements: 删除元素 ");
					defaultInterpolatedStringHandler.AppendFormatted(elementId);
					defaultInterpolatedStringHandler.AppendLiteral(" 失败 - ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			return num;
		}
		catch (Exception ex2)
		{
			LogError("DeleteElements 失败: " + ex2.Message);
			return 0;
		}
	}

	public int CountElementsByCategory(object document, string categoryName)
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return 0;
			}
			if (string.IsNullOrEmpty(categoryName))
			{
				LogError("CountElementsByCategory: 类别名称为空");
				return 0;
			}
			LogInfo("CountElementsByCategory: 正在查找类别 '" + categoryName + "'");
			Category val2 = null;
			foreach (Category item in (CategoryNameMap)val.Settings.Categories)
			{
				Category val3 = item;
				if (val3.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
				{
					val2 = val3;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(15, 2);
					defaultInterpolatedStringHandler.AppendLiteral("✅ 找到类别: ");
					defaultInterpolatedStringHandler.AppendFormatted(val3.Name);
					defaultInterpolatedStringHandler.AppendLiteral(" (ID: ");
					defaultInterpolatedStringHandler.AppendFormatted(val3.Id.Value);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					break;
				}
			}
			if (val2 == null)
			{
				LogError("CountElementsByCategory: 类别不存在 - " + categoryName);
				LogInfo("提示：请使用类别的中文名称，例如：墙、楼板、门、窗、柱、梁等");
				return 0;
			}
			FilteredElementCollector val4 = new FilteredElementCollector(val);
			int count = val4.OfCategory((BuiltInCategory)val2.Id.Value).ToElements().Count;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(15, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("✅ 类别 '");
			defaultInterpolatedStringHandler2.AppendFormatted(val2.Name);
			defaultInterpolatedStringHandler2.AppendLiteral("' 找到 ");
			defaultInterpolatedStringHandler2.AppendFormatted(count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个元素");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return count;
		}
		catch (Exception ex)
		{
			LogError("CountElementsByCategory 失败: " + ex.Message);
			return 0;
		}
	}

	public IEnumerable<(string Name, object? Value, string Type)> GetAllParameters(object element)
	{
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Expected I4, but got Unknown
		List<(string, object, string)> list = new List<(string, object, string)>();
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetAllParameters: element 不是 Element 类型");
				return list;
			}
			ParameterSet parameters = val.Parameters;
			foreach (Parameter item3 in parameters)
			{
				Parameter val2 = item3;
				string name = val2.Definition.Name;
				object item = null;
				string item2 = ((object)val2.StorageType/*cast due to constrained. prefix*/).ToString();
				StorageType storageType = val2.StorageType;
				StorageType val3 = storageType;
				switch ((int)val3)
				{
				case 0:
					item = null;
					break;
				case 1:
					item = val2.AsInteger();
					break;
				case 2:
					item = val2.AsDouble();
					break;
				case 3:
					item = val2.AsString();
					break;
				case 4:
					item = val2.AsElementId().Value;
					break;
				}
				list.Add((name, item, item2));
			}
		}
		catch (Exception ex)
		{
			LogError("GetAllParameters 失败: " + ex.Message);
		}
		return list;
	}

	public object? GetParameterValue(object element, string parameterName)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetParameterValue: element 不是 Element 类型");
				return null;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 != null)
			{
				return GetParameterValue(val2);
			}
			ElementId typeId = val.GetTypeId();
			if (typeId != (ElementId)null && !((object)typeId).Equals((object?)ElementId.InvalidElementId))
			{
				Element element2 = val.Document.GetElement(typeId);
				if (element2 != null)
				{
					Parameter val3 = element2.LookupParameter(parameterName);
					if (val3 != null)
					{
						return GetParameterValue(val3);
					}
				}
			}
			LogError("GetParameterValue: 参数不存在（实例和类型级别均未找到）- " + parameterName);
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetParameterValue 失败: " + ex.Message);
			return null;
		}
	}

	public bool SetParameterValue(object element, string parameterName, object value, object document)
	{
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected I4, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("SetParameterValue: element 不是 Element 类型");
				return false;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				LogError("SetParameterValue: 参数不存在 - " + parameterName);
				return false;
			}
			if (!((APIObject)val2).IsReadOnly)
			{
				StorageType storageType = val2.StorageType;
				StorageType val3 = storageType;
				switch ((int)(val3) - 1)
				{
				case 0:
					val2.Set(Convert.ToInt32(value));
					break;
				case 1:
					val2.Set(Convert.ToDouble(value));
					break;
				case 2:
					val2.Set(Convert.ToString(value));
					break;
				case 3:
					if (value is int num)
					{
						val2.Set(new ElementId((long)num));
					}
					break;
				}
				return true;
			}
			LogError("SetParameterValue: 参数只读 - " + parameterName);
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetParameterValue 失败: " + ex.Message);
			return false;
		}
	}

	public string? GetParameterFormula(object element, string parameterName)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetParameterFormula: element 不是 Element 类型");
				return null;
			}
			Parameter val2 = val.LookupParameter(parameterName);
			if (val2 == null)
			{
				LogError("GetParameterFormula: 参数不存在 - " + parameterName);
				return null;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetParameterFormula 失败: " + ex.Message);
			return null;
		}
	}

	private static object? GetParameterValue(Parameter param)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected I4, but got Unknown
		StorageType storageType = param.StorageType;
		StorageType val = storageType;
		return (int)val switch
		{
			0 => null, 
			1 => param.AsInteger(), 
			2 => param.AsDouble(), 
			3 => param.AsString(), 
			4 => param.AsElementId().Value, 
			_ => null, 
		};
	}

	public string? GetElementTypeName(object element)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			ElementType val2 = (ElementType)(object)((val is ElementType) ? val : null);
			if (val2 != null)
			{
				return ((Element)val2).Name;
			}
			Parameter val3 = val.get_Parameter((BuiltInParameter)(-1002001L));
			if (val3 != null && (int)val3.StorageType == 3)
			{
				string text = val3.AsString();
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			ElementId typeId = val.GetTypeId();
			if (typeId != (ElementId)null && !((object)typeId).Equals((object?)ElementId.InvalidElementId))
			{
				Document document = val.Document;
				Element element2 = document.GetElement(typeId);
				if (element2 != null)
				{
					ElementType val4 = (ElementType)(object)((element2 is ElementType) ? element2 : null);
					if (val4 != null)
					{
						return ((Element)val4).Name;
					}
					if (!string.IsNullOrEmpty(element2.Name))
					{
						return element2.Name;
					}
				}
			}
			if (!string.IsNullOrEmpty(val.Name))
			{
				return val.Name;
			}
			return ((object)val).GetType().Name;
		}
		catch (Exception ex)
		{
			LogError("GetElementTypeName 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsElementType(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			return val is ElementType;
		}
		catch (Exception ex)
		{
			LogError("IsElementType 失败: " + ex.Message);
			return false;
		}
	}

	public int? GetElementTypeId(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementTypeId: element 不是 Element 类型");
				return null;
			}
			ElementId typeId = val.GetTypeId();
			if (typeId == (ElementId)null || ((object)typeId).Equals((object?)ElementId.InvalidElementId))
			{
				return null;
			}
			return (int)typeId.Value;
		}
		catch (Exception ex)
		{
			LogError("GetElementTypeId 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetElementArea(object element)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementArea: element 不是 Element 类型");
				return null;
			}
			SpatialElement val2 = (SpatialElement)(object)((val is SpatialElement) ? val : null);
			if (val2 != null)
			{
				Parameter val3 = ((Element)val2).get_Parameter((BuiltInParameter)(-1006902L));
				if (val3 != null && (int)val3.StorageType == 2)
				{
					return val3.AsDouble() * 0.092903;
				}
			}
			string[] array = new string[3]
			{
				"Area",
				"面积",
				"Gross Area"
			};
			string[] array2 = array;
			int num = 0;
			Parameter val4;
			while (true)
			{
				if (num < array2.Length)
				{
					string text = array2[num];
					val4 = val.LookupParameter(text);
					if (val4 != null && (int)val4.StorageType == 2)
					{
						break;
					}
					num++;
					continue;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetElementArea: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有面积参数");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			return val4.AsDouble() * 0.092903;
		}
		catch (Exception ex)
		{
			LogError("GetElementArea 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetElementVolume(object element)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Invalid comparison between Unknown and I4
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementVolume: element 不是 Element 类型");
				return null;
			}
			SpatialElement val2 = (SpatialElement)(object)((val is SpatialElement) ? val : null);
			if (val2 != null)
			{
				Parameter val3 = ((Element)val2).get_Parameter((BuiltInParameter)(-1006921L));
				if (val3 != null && (int)val3.StorageType == 2)
				{
					return val3.AsDouble() * 0.0283168;
				}
			}
			string[] array = new string[2]
			{
				"Volume",
				"体积"
			};
			string[] array2 = array;
			int num = 0;
			Parameter val4;
			while (true)
			{
				if (num < array2.Length)
				{
					string text = array2[num];
					val4 = val.LookupParameter(text);
					if (val4 != null && (int)val4.StorageType == 2)
					{
						break;
					}
					num++;
					continue;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetElementVolume: 元素 ");
				defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler.AppendLiteral(" 没有体积参数");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			return val4.AsDouble() * 0.0283168;
		}
		catch (Exception ex)
		{
			LogError("GetElementVolume 失败: " + ex.Message);
			return null;
		}
	}

	public (string? CreatedPhase, string? DemolishedPhase)? GetElementPhases(object element)
	{
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Invalid comparison between Unknown and I4
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementPhases: element 不是 Element 类型");
				return null;
			}
			string item = null;
			string item2 = null;
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012100L));
			if (val2 != null && (int)val2.StorageType == 4)
			{
				ElementId val3 = val2.AsElementId();
				Element element2 = val.Document.GetElement(val3);
				Phase val4 = (Phase)(object)((element2 is Phase) ? element2 : null);
				item = ((val4 != null) ? ((Element)val4).Name : null);
			}
			Parameter val5 = val.get_Parameter((BuiltInParameter)(-1012101L));
			if (val5 != null && (int)val5.StorageType == 4)
			{
				ElementId val6 = val5.AsElementId();
				Element element3 = val.Document.GetElement(val6);
				Phase val7 = (Phase)(object)((element3 is Phase) ? element3 : null);
				item2 = ((val7 != null) ? ((Element)val7).Name : null);
			}
			return (item, item2);
		}
		catch (Exception ex)
		{
			LogError("GetElementPhases 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetWorksets(object document, bool userCreatedOnly = false)
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetWorksets: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			FilteredWorksetCollector val2 = new FilteredWorksetCollector(val);
			foreach (Workset item in val2)
			{
				if (!userCreatedOnly || !((WorksetPreview)item).IsDefaultWorkset)
				{
					list.Add(item);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetWorksets 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetWorksetByName(object document, string worksetName)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetWorksetByName: document 不是 Document 类型");
				return null;
			}
			FilteredWorksetCollector val2 = new FilteredWorksetCollector(val);
			foreach (Workset item in val2)
			{
				if (((WorksetPreview)item).Name == worksetName)
				{
					return item;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetWorksetByName 失败: " + ex.Message);
			return null;
		}
	}

	public int GetWorksetId(object workset)
	{
		try
		{
			Workset val = (Workset)((workset is Workset) ? workset : null);
			if (val != null)
			{
				FieldInfo field = ((object)((WorksetPreview)val).Id).GetType().GetField("m_value", BindingFlags.Instance | BindingFlags.NonPublic);
				if (field != null && field.GetValue(((WorksetPreview)val).Id) is int result)
				{
					return result;
				}
				return -1;
			}
			return -1;
		}
		catch (Exception ex)
		{
			LogError("GetWorksetId 失败: " + ex.Message);
			return -1;
		}
	}

	public string? GetWorksetName(object workset)
	{
		try
		{
			Workset val = (Workset)((workset is Workset) ? workset : null);
			if (val != null)
			{
				return ((WorksetPreview)val).Name;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetWorksetName 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsWorksetVisible(object workset)
	{
		try
		{
			Workset val = (Workset)((workset is Workset) ? workset : null);
			if (val != null)
			{
				return val.IsOpen;
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("IsWorksetVisible 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsWorksetOpen(object workset)
	{
		try
		{
			Workset val = (Workset)((workset is Workset) ? workset : null);
			if (val != null)
			{
				return val.IsOpen;
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("IsWorksetOpen 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsDefaultWorkset(object workset)
	{
		try
		{
			Workset val = (Workset)((workset is Workset) ? workset : null);
			if (val != null)
			{
				return ((WorksetPreview)val).IsDefaultWorkset;
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("IsDefaultWorkset 失败: " + ex.Message);
			return false;
		}
	}

	public object? GetElementWorkset(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementWorkset: element 不是 Element 类型");
				return null;
			}
			WorksetId worksetId = val.WorksetId;
			WorksetTable worksetTable = val.Document.GetWorksetTable();
			return worksetTable.GetWorkset(worksetId);
		}
		catch (Exception ex)
		{
			LogError("GetElementWorkset 失败: " + ex.Message);
			return null;
		}
	}

	public bool SetElementWorkset(object element, object workset, object document)
	{
		try
		{
			LogError("SetElementWorkset: 此功能暂未实现");
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetElementWorkset 失败: " + ex.Message);
			return false;
		}
	}

	public object? CreateWorkset(object document, string worksetName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateWorkset: document 不是 Document 类型");
				return null;
			}
			Workset val2 = Workset.Create(val, worksetName);
			if (val2 == null)
			{
				LogError("CreateWorkset: Workset.Create 返回 null (名称='" + worksetName + "')");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
			defaultInterpolatedStringHandler.AppendLiteral("CreateWorkset: 成功创建工作集 (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((WorksetPreview)val2).Id.IntegerValue);
			defaultInterpolatedStringHandler.AppendLiteral(", 名称='");
			defaultInterpolatedStringHandler.AppendFormatted(worksetName);
			defaultInterpolatedStringHandler.AppendLiteral("')");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex)
		{
			LogError("CreateWorkset 失败: " + ex.Message);
			return null;
		}
	}

	private static void LogError(string message)
	{
		Logger.Error("[ElementService] " + message);
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[ElementService] " + message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[ElementService] " + message);
	}

	public int? GetMaterialIdByName(object document, string materialName)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetMaterialIdByName: document 不是 Document 类型");
				return null;
			}
			Material val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Material))).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Equals(materialName, StringComparison.OrdinalIgnoreCase));
			if (val2 != null)
			{
				return (int)((Element)val2).Id.Value;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetMaterialIdByName 失败: " + ex.Message);
			return null;
		}
	}

	public int? DuplicateView(object document, object view, string duplicateOption)
	{
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DuplicateView: document 不是 Document 类型");
				return null;
			}
			View val2 = (View)((view is View) ? view : null);
			if (val2 == null)
			{
				LogError("DuplicateView: view 不是 View 类型");
				return null;
			}
			ViewDuplicateOption val3 = ((duplicateOption == "WithDetailing") ? ((ViewDuplicateOption)2) : ((duplicateOption == "Duplicate") ? ((ViewDuplicateOption)0) : ((ViewDuplicateOption)2)));
			ViewDuplicateOption val4 = val3;
			ElementId val5 = val2.Duplicate(val4);
			return (int)val5.Value;
		}
		catch (Exception ex)
		{
			LogError("DuplicateView 失败: " + ex.Message);
			return null;
		}
	}

	public void SetViewLevel(object view, int levelId)
	{
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				LogError("SetViewLevel: view 不是 View 类型");
				return;
			}
			ViewPlan val2 = (ViewPlan)(object)((val is ViewPlan) ? val : null);
			if (val2 != null)
			{
				Parameter val3 = ((Element)val2).get_Parameter((BuiltInParameter)(-1005166L));
				if (val3 != null && !((APIObject)val3).IsReadOnly)
				{
					val3.Set(new ElementId((long)levelId));
				}
			}
		}
		catch (Exception ex)
		{
			LogError("SetViewLevel 失败: " + ex.Message);
		}
	}

	public void SetViewName(object view, string name)
	{
		try
		{
			View val = (View)((view is View) ? view : null);
			if (val == null)
			{
				LogError("SetViewName: view 不是 View 类型");
			}
			else
			{
				((Element)val).Name = name;
			}
		}
		catch (Exception ex)
		{
			LogError("SetViewName 失败: " + ex.Message);
		}
	}

	public object? GetElementGeometry(object document, object element)
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementGeometry: element 不是 Element 类型");
				return null;
			}
			Options val2 = new Options();
			return val.get_Geometry(val2);
		}
		catch (Exception ex)
		{
			LogError("GetElementGeometry 失败: " + ex.Message);
			return null;
		}
	}

	public double GetElementArea(object document, object element)
	{
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Invalid comparison between Unknown and I4
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Expected O, but got Unknown
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				LogError("GetElementArea: element 不是 Element 类型");
				return 0.0;
			}
			double num = 0.0;
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012805L));
			if (val2 != null && (int)val2.StorageType == 2)
			{
				double num2 = val2.AsDouble();
				if (num2 > 0.0)
				{
					return num2;
				}
			}
			Options val3 = new Options
			{
				ComputeReferences = false,
				IncludeNonVisibleObjects = false
			};
			GeometryElement val4 = val.get_Geometry(val3);
			if ((GeometryObject)(object)val4 == (GeometryObject)null)
			{
				return 0.0;
			}
			foreach (GeometryObject item in val4)
			{
				Face val5 = (Face)(object)((item is Face) ? item : null);
				if (val5 != null)
				{
					num += val5.Area;
					continue;
				}
				Solid val6 = (Solid)(object)((item is Solid) ? item : null);
				if (val6 != null)
				{
					foreach (Face face in val6.Faces)
					{
						Face val7 = face;
						num += val7.Area;
					}
					continue;
				}
				GeometryInstance val8 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val8 == null)
				{
					continue;
				}
				GeometryElement instanceGeometry = val8.GetInstanceGeometry();
				if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
				{
					continue;
				}
				foreach (GeometryObject item2 in instanceGeometry)
				{
					Face val9 = (Face)(object)((item2 is Face) ? item2 : null);
					if (val9 != null)
					{
						num += val9.Area;
						continue;
					}
					Solid val10 = (Solid)(object)((item2 is Solid) ? item2 : null);
					if (val10 == null)
					{
						continue;
					}
					foreach (Face face2 in val10.Faces)
					{
						Face val11 = face2;
						num += val11.Area;
					}
				}
			}
			return num;
		}
		catch (Exception ex)
		{
			LogError("GetElementArea 失败: " + ex.Message);
			return 0.0;
		}
	}

	public string? GetCadElementLayerName(object element)
	{
		//IL_0137: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Expected O, but got Unknown
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Expected O, but got Unknown
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			ImportInstance val2 = (ImportInstance)(object)((val is ImportInstance) ? val : null);
			if (val2 == null)
			{
				return null;
			}
			Category category = val.Category;
			if (category != null)
			{
				string name = category.Name;
				if (!string.IsNullOrEmpty(name))
				{
					return name;
				}
			}
			foreach (Parameter parameter in val.Parameters)
			{
				Parameter val3 = parameter;
				try
				{
					string name2 = val3.Definition.Name;
					if ((name2.Contains("Layer", StringComparison.OrdinalIgnoreCase) || name2.Contains("图层", StringComparison.OrdinalIgnoreCase)) && (int)val3.StorageType == 3)
					{
						string text = val3.AsString();
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
				catch
				{
				}
			}
			Element element2 = val.Document.GetElement(val.GetTypeId());
			ElementType val4 = (ElementType)(object)((element2 is ElementType) ? element2 : null);
			if (val4 != null)
			{
				foreach (Parameter parameter2 in ((Element)val4).Parameters)
				{
					Parameter val5 = parameter2;
					try
					{
						string name3 = val5.Definition.Name;
						if ((name3.Contains("Layer", StringComparison.OrdinalIgnoreCase) || name3.Contains("图层", StringComparison.OrdinalIgnoreCase)) && (int)val5.StorageType == 3)
						{
							string text2 = val5.AsString();
							if (!string.IsNullOrEmpty(text2))
							{
								return text2;
							}
						}
					}
					catch
					{
					}
				}
			}
			return (category != null) ? category.Name : null;
		}
		catch (Exception ex)
		{
			LogError("GetCadElementLayerName 失败: " + ex.Message);
			return null;
		}
	}

	public int ColorElements(object document, IEnumerable<int> elementIds, (byte Red, byte Green, byte Blue)? lineColorRGB = null, (byte Red, byte Green, byte Blue)? fillPatternRGB = null, (byte Red, byte Green, byte Blue)? cutFillColorRGB = null)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016a: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ColorElements: document 不是 Document 类型");
				return 0;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				LogError("ColorElements: 无法获取当前活动视图");
				return 0;
			}
			int num = 0;
			foreach (int elementId in elementIds)
			{
				try
				{
					ElementId val2 = new ElementId((long)elementId);
					Element element = val.GetElement(val2);
					if (element == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
						defaultInterpolatedStringHandler.AppendLiteral("ColorElements: 找不到元素 ID ");
						defaultInterpolatedStringHandler.AppendFormatted(elementId);
						LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
						continue;
					}
					OverrideGraphicSettings val3 = new OverrideGraphicSettings();
					if (lineColorRGB.HasValue)
					{
						Color projectionLineColor = new Color(lineColorRGB.Value.Red, lineColorRGB.Value.Green, lineColorRGB.Value.Blue);
						val3.SetProjectionLineColor(projectionLineColor);
					}
					if (fillPatternRGB.HasValue)
					{
						Color surfaceForegroundPatternColor = new Color(fillPatternRGB.Value.Red, fillPatternRGB.Value.Green, fillPatternRGB.Value.Blue);
						val3.SetSurfaceForegroundPatternColor(surfaceForegroundPatternColor);
					}
					if (cutFillColorRGB.HasValue)
					{
						Color cutForegroundPatternColor = new Color(cutFillColorRGB.Value.Red, cutFillColorRGB.Value.Green, cutFillColorRGB.Value.Blue);
						val3.SetCutForegroundPatternColor(cutForegroundPatternColor);
					}
					activeView.SetElementOverrides(val2, val3);
					num++;
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("ColorElements: 设置元素 ");
					defaultInterpolatedStringHandler2.AppendFormatted(elementId);
					defaultInterpolatedStringHandler2.AppendLiteral(" 着色失败 - ");
					defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
					LogWarning(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("ColorElements: 成功给 ");
			defaultInterpolatedStringHandler3.AppendFormatted(num);
			defaultInterpolatedStringHandler3.AppendLiteral("/");
			defaultInterpolatedStringHandler3.AppendFormatted(elementIds.Count());
			defaultInterpolatedStringHandler3.AppendLiteral(" 个元素着色");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			return num;
		}
		catch (Exception ex2)
		{
			LogError("ColorElements 失败: " + ex2.Message);
			return 0;
		}
	}

	public int ResetElementColors(object document, IEnumerable<int> elementIds)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ResetElementColors: document 不是 Document 类型");
				return 0;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				LogError("ResetElementColors: 无法获取当前活动视图");
				return 0;
			}
			int num = 0;
			foreach (int elementId in elementIds)
			{
				try
				{
					ElementId val2 = new ElementId((long)elementId);
					Element element = val.GetElement(val2);
					if (element != null)
					{
						activeView.SetElementOverrides(val2, new OverrideGraphicSettings());
						num++;
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
					defaultInterpolatedStringHandler.AppendLiteral("ResetElementColors: 重置元素 ");
					defaultInterpolatedStringHandler.AppendFormatted(elementId);
					defaultInterpolatedStringHandler.AppendLiteral(" 着色失败 - ");
					defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
					LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("ResetElementColors: 成功重置 ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral("/");
			defaultInterpolatedStringHandler2.AppendFormatted(elementIds.Count());
			defaultInterpolatedStringHandler2.AppendLiteral(" 个元素的着色");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return num;
		}
		catch (Exception ex2)
		{
			LogError("ResetElementColors 失败: " + ex2.Message);
			return 0;
		}
	}

	public int ResetAllElementColors(object document)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ResetAllElementColors: document 不是 Document 类型");
				return 0;
			}
			View activeView = val.ActiveView;
			if (activeView == null)
			{
				LogError("ResetAllElementColors: 无法获取当前活动视图");
				return 0;
			}
			List<ElementId> list = new FilteredElementCollector(val, ((Element)activeView).Id).WhereElementIsNotElementType().ToElementIds().ToList();
			int num = 0;
			foreach (ElementId item in list)
			{
				try
				{
					activeView.SetElementOverrides(item, new OverrideGraphicSettings());
					num++;
				}
				catch
				{
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ResetAllElementColors: 成功重置 ");
			defaultInterpolatedStringHandler.AppendFormatted(num);
			defaultInterpolatedStringHandler.AppendLiteral(" 个元素的着色");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return num;
		}
		catch (Exception ex)
		{
			LogError("ResetAllElementColors 失败: " + ex.Message);
			return 0;
		}
	}

	public IEnumerable<object> GetAllSharedParameters(object document)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013f: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllSharedParameters: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			IList<Element> list2 = new FilteredElementCollector(val).OfClass(typeof(SharedParameterElement)).ToElements();
			BindingMap parameterBindings = val.ParameterBindings;
			foreach (SharedParameterElement item in list2)
			{
				SharedParameterElement val2 = item;
				List<string> list3 = new List<string>();
				InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
				string gparam_ = null;
				string gparam_2 = null;
				string text = null;
				if (definition != null)
				{
					try
					{
						ForgeTypeId dataType = ((Definition)definition).GetDataType();
						text = GetDataTypeName(dataType);
						ForgeTypeId groupTypeId = ((Definition)definition).GetGroupTypeId();
						gparam_2 = GetGroupTypeName(groupTypeId);
					}
					catch
					{
						text = "Text";
						gparam_2 = "PG_DATA";
					}
					try
					{
						Binding val3 = ((DefinitionBindingMap)parameterBindings).get_Item((Definition)(object)definition);
						ElementBinding val4 = (ElementBinding)(object)((val3 is ElementBinding) ? val3 : null);
						if (val4 != null)
						{
							gparam_ = ((val3 is InstanceBinding) ? "instance" : "type");
							foreach (Category category in val4.Categories)
							{
								Category val5 = category;
								list3.Add(val5.Name);
							}
						}
					}
					catch
					{
					}
				}
				list.Add(new Class313<string, string, string, string[], int, string, string>(((Element)val2).Name, text ?? "Text", val2.GuidValue.ToString(), list3.ToArray(), list3.Count, gparam_, gparam_2));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetAllSharedParameters: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个共享参数");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllSharedParameters 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetSharedParameter(object document, string parameterName)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetSharedParameter: document 不是 Document 类型");
				return null;
			}
			SharedParameterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				return null;
			}
			List<string> list = new List<string>();
			InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
			BindingMap parameterBindings = val.ParameterBindings;
			if (definition != null)
			{
				try
				{
					Binding val3 = ((DefinitionBindingMap)parameterBindings).get_Item((Definition)(object)definition);
					ElementBinding val4 = (ElementBinding)(object)((val3 is ElementBinding) ? val3 : null);
					if (val4 != null)
					{
						foreach (Category category in val4.Categories)
						{
							Category val5 = category;
							list.Add(val5.Name);
						}
					}
				}
				catch
				{
				}
			}
			return new Class314<string, string, string, string[], int>(((Element)val2).Name, "Text", val2.GuidValue.ToString(), list.ToArray(), list.Count);
		}
		catch (Exception ex)
		{
			LogError("GetSharedParameter 失败: " + ex.Message);
			return null;
		}
	}

	public bool EnsureSharedParameterFile(object application)
	{
		try
		{
			Application val = (Application)((application is Application) ? application : null);
			if (val == null)
			{
				LogError("EnsureSharedParameterFile: application 不是 Application 类型");
				return false;
			}
			string sharedParametersFilename = val.SharedParametersFilename;
			if (!string.IsNullOrEmpty(sharedParametersFilename))
			{
				if (File.Exists(sharedParametersFilename))
				{
					LogInfo("EnsureSharedParameterFile: 共享参数文件已设置: " + sharedParametersFilename);
					return true;
				}
				LogWarning("EnsureSharedParameterFile: 设置的共享参数文件不存在: " + sharedParametersFilename);
			}
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string text = Path.Combine(folderPath, "RevitAi");
			string text2 = Path.Combine(text, "RevitAi.SharedParameters.txt");
			try
			{
				if (!Directory.Exists(text))
				{
					Directory.CreateDirectory(text);
					LogInfo("EnsureSharedParameterFile: 创建目录: " + text);
				}
				if (!File.Exists(text2))
				{
					string contents = "# This is a Revit shared parameter file.\r\n# Do not edit manually.\r\n*GROUP\tID\tNAME\r\n*PARAM\tGUID\tNAME\tDATATYPE\tDATACATEGORY\tGROUP\tVISIBLE\r\n";
					UTF8Encoding encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
					File.WriteAllText(text2, contents, encoding);
					LogInfo("EnsureSharedParameterFile: 创建默认共享参数文件: " + text2);
				}
				val.SharedParametersFilename = text2;
				LogInfo("EnsureSharedParameterFile: 设置共享参数文件: " + text2);
				DefinitionFile val2 = val.OpenSharedParameterFile();
				if (val2 == null)
				{
					LogError("EnsureSharedParameterFile: 无法打开创建的共享参数文件");
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				LogError("EnsureSharedParameterFile: 创建或设置共享参数文件失败 - " + ex.Message);
				return false;
			}
		}
		catch (Exception ex2)
		{
			LogError("EnsureSharedParameterFile 失败: " + ex2.Message);
			return false;
		}
	}

	public object? CreateSharedParameter(object document, string parameterName, string parameterType, string groupName, IEnumerable<string> categoryNames, bool instanceParameter = true)
	{
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Expected O, but got Unknown
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dd: Expected O, but got Unknown
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSharedParameter: document 不是 Document 类型");
				return new Class315<bool, string>(gparam_2: false, "document 不是 Document 类型");
			}
			object sharedParameter = GetSharedParameter(document, parameterName);
			if (sharedParameter != null)
			{
				return new Class315<bool, string>(gparam_2: false, "参数 '" + parameterName + "' 已存在");
			}
			Application application = val.Application;
			if (!EnsureSharedParameterFile(application))
			{
				return new Class315<bool, string>(gparam_2: false, "无法创建或设置共享参数文件");
			}
			DefinitionFile val2 = application.OpenSharedParameterFile();
			if (val2 == null)
			{
				return new Class315<bool, string>(gparam_2: false, "无法打开共享参数文件");
			}
			DefinitionGroups groups = val2.Groups;
			DefinitionGroup val3 = ((IEnumerable)groups).Cast<DefinitionGroup>().FirstOrDefault((DefinitionGroup g) => g.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase));
			if (val3 == null)
			{
				val3 = groups.Create(groupName);
			}
			ForgeTypeId val4 = MapParameterType(parameterType);
			if (val4 == (ForgeTypeId)null)
			{
				return new Class315<bool, string>(gparam_2: false, "不支持的参数类型: " + parameterType);
			}
			ExternalDefinition val5 = ((IEnumerable)val3.Definitions).Cast<ExternalDefinition>().FirstOrDefault((ExternalDefinition d) => ((Definition)d).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			ExternalDefinition val6;
			if (val5 != null)
			{
				val6 = val5;
				LogInfo("CreateSharedParameter: 使用现有共享参数定义 '" + parameterName + "'");
			}
			else
			{
				ExternalDefinitionCreationOptions val7 = new ExternalDefinitionCreationOptions(parameterName, val4);
				Definition obj = val3.Definitions.Create(val7);
				val6 = (ExternalDefinition)(object)((obj is ExternalDefinition) ? obj : null);
				if (val6 == null)
				{
					return new Class315<bool, string>(gparam_2: false, "创建共享参数定义失败");
				}
			}
			CategorySet val8 = application.Create.NewCategorySet();
			foreach (string categoryName in categoryNames)
			{
				Category val9 = null;
				foreach (Category item in (CategoryNameMap)val.Settings.Categories)
				{
					Category val10 = item;
					if (val10.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase))
					{
						val9 = val10;
						break;
					}
				}
				if (val9 != null)
				{
					val8.Insert(val9);
				}
				else
				{
					LogWarning("CreateSharedParameter: 找不到类别 '" + categoryName + "'");
				}
			}
			if (val8.Size != 0)
			{
				BindingMap parameterBindings = val.ParameterBindings;
				try
				{
					if (instanceParameter)
					{
						InstanceBinding val11 = new InstanceBinding(val8);
						((DefinitionBindingMap)parameterBindings).Insert((Definition)(object)val6, (Binding)(object)val11);
					}
					else
					{
						TypeBinding val12 = new TypeBinding(val8);
						((DefinitionBindingMap)parameterBindings).Insert((Definition)(object)val6, (Binding)(object)val12);
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
					defaultInterpolatedStringHandler.AppendLiteral("CreateSharedParameter: 成功创建共享参数 '");
					defaultInterpolatedStringHandler.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler.AppendLiteral("'，绑定到 ");
					defaultInterpolatedStringHandler.AppendFormatted(val8.Size);
					defaultInterpolatedStringHandler.AppendLiteral(" 个类别");
					LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
					return new Class316<bool, string, string, string, string[], int>(gparam_6: true, ((Definition)val6).Name, parameterType, val6.GUID.ToString(), categoryNames.ToArray(), val8.Size);
				}
				catch (Exception ex)
				{
					LogError("CreateSharedParameter: 创建绑定失败 - " + ex.Message);
					return new Class315<bool, string>(gparam_2: false, "创建绑定失败: " + ex.Message);
				}
			}
			return new Class315<bool, string>(gparam_2: false, "没有找到任何有效的类别");
		}
		catch (Exception ex2)
		{
			LogError("CreateSharedParameter 失败: " + ex2.Message);
			return new Class315<bool, string>(gparam_2: false, ex2.Message);
		}
	}

	public bool DeleteSharedParameter(object document, string parameterName)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteSharedParameter: document 不是 Document 类型");
				return false;
			}
			SharedParameterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				LogWarning("DeleteSharedParameter: 找不到参数 '" + parameterName + "'");
				return false;
			}
			int sharedParameterUsageCount = GetSharedParameterUsageCount(document, parameterName);
			if (sharedParameterUsageCount > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 2);
				defaultInterpolatedStringHandler.AppendLiteral("DeleteSharedParameter: 参数 '");
				defaultInterpolatedStringHandler.AppendFormatted(parameterName);
				defaultInterpolatedStringHandler.AppendLiteral("' 正被 ");
				defaultInterpolatedStringHandler.AppendFormatted(sharedParameterUsageCount);
				defaultInterpolatedStringHandler.AppendLiteral(" 个元素使用，无法删除");
				LogWarning(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
			BindingMap parameterBindings = val.ParameterBindings;
			if (definition != null)
			{
				try
				{
					((DefinitionBindingMap)parameterBindings).Erase((Definition)(object)definition);
					LogInfo("DeleteSharedParameter: 成功移除参数 '" + parameterName + "' 的绑定");
				}
				catch (Exception ex)
				{
					LogWarning("DeleteSharedParameter: 移除绑定失败 - " + ex.Message);
				}
			}
			ICollection<ElementId> collection = val.Delete(((Element)val2).Id);
			bool flag = collection.Count > 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("DeleteSharedParameter: ");
			defaultInterpolatedStringHandler2.AppendFormatted(flag ? "成功" : "失败");
			defaultInterpolatedStringHandler2.AppendLiteral("删除参数 '");
			defaultInterpolatedStringHandler2.AppendFormatted(parameterName);
			defaultInterpolatedStringHandler2.AppendLiteral("'");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return flag;
		}
		catch (Exception ex2)
		{
			LogError("DeleteSharedParameter 失败: " + ex2.Message);
			return false;
		}
	}

	public int RemoveSharedParameterBindings(object document, string parameterName, IEnumerable<string> categoryNames)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RemoveSharedParameterBindings: document 不是 Document 类型");
				return 0;
			}
			SharedParameterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				LogWarning("RemoveSharedParameterBindings: 找不到参数 '" + parameterName + "'");
				return 0;
			}
			InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
			BindingMap parameterBindings = val.ParameterBindings;
			if (definition != null && ((DefinitionBindingMap)parameterBindings).Contains((Definition)(object)definition))
			{
				try
				{
					parameterBindings.Remove((Definition)(object)definition);
					LogInfo("RemoveSharedParameterBindings: 已移除参数 '" + parameterName + "' 的所有绑定");
					return categoryNames.Count();
				}
				catch (Exception ex)
				{
					LogError("RemoveSharedParameterBindings 失败: " + ex.Message);
					return 0;
				}
			}
			LogWarning("RemoveSharedParameterBindings: 参数 '" + parameterName + "' 没有绑定");
			return 0;
		}
		catch (Exception ex2)
		{
			LogError("RemoveSharedParameterBindings 失败: " + ex2.Message);
			return 0;
		}
	}

	public int GetSharedParameterUsageCount(object document, string parameterName)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetSharedParameterUsageCount: document 不是 Document 类型");
				return 0;
			}
			SharedParameterElement val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				return 0;
			}
			ElementId id = ((Element)val2).Id;
			int num = 0;
			IList<Element> list = new FilteredElementCollector(val).WhereElementIsNotElementType().ToElements();
			foreach (Element item in list)
			{
				try
				{
					ParameterSet parameters = item.Parameters;
					foreach (Parameter item2 in parameters)
					{
						Parameter val3 = item2;
						if (val3.Id == id)
						{
							num++;
							break;
						}
					}
				}
				catch
				{
				}
			}
			return num;
		}
		catch (Exception ex)
		{
			LogError("GetSharedParameterUsageCount 失败: " + ex.Message);
			return 0;
		}
	}

	private static ForgeTypeId? MapParameterType(string parameterType)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Expected O, but got Unknown
		string text = parameterType.ToUpperInvariant();
		if (text == "TEXT")
		{
			return SpecTypeId.String.Text;
		}
		if (text == "INTEGER")
		{
			return SpecTypeId.Int.Integer;
		}
		if (text == "NUMBER")
		{
			return new ForgeTypeId("autodesk.spec:double-0.0");
		}
		return SpecTypeId.String.Text;
	}

	private static object? MapParameterGroup(string groupName)
	{
		return null;
	}

	public IEnumerable<CompoundLayerInfo>? GetCompoundStructureLayers(object elementType)
	{
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Unknown result type (might be due to invalid IL or missing references)
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		//IL_022a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Expected O, but got Unknown
		try
		{
			Element val = (Element)((elementType is Element) ? elementType : null);
			if (val == null)
			{
				LogError("GetCompoundStructureLayers: 无效的元素类型");
				return null;
			}
			CompoundStructure val2 = null;
			WallType val3 = (WallType)(object)((val is WallType) ? val : null);
			if (val3 != null)
			{
				val2 = ((HostObjAttributes)val3).GetCompoundStructure();
			}
			else
			{
				FloorType val4 = (FloorType)(object)((val is FloorType) ? val : null);
				if (val4 != null)
				{
					val2 = ((HostObjAttributes)val4).GetCompoundStructure();
				}
				else
				{
					RoofType val5 = (RoofType)(object)((val is RoofType) ? val : null);
					if (val5 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 2);
						defaultInterpolatedStringHandler.AppendLiteral("GetCompoundStructureLayers: 元素 ");
						defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
						defaultInterpolatedStringHandler.AppendLiteral(" (");
						defaultInterpolatedStringHandler.AppendFormatted(val.Name);
						defaultInterpolatedStringHandler.AppendLiteral(") 不是墙类型、楼板类型或屋顶类型");
						LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
						return null;
					}
					val2 = ((HostObjAttributes)val5).GetCompoundStructure();
				}
			}
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(43, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("GetCompoundStructureLayers: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler2.AppendLiteral(" (");
				defaultInterpolatedStringHandler2.AppendFormatted(val.Name);
				defaultInterpolatedStringHandler2.AppendLiteral(") 没有复合结构对象");
				LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			IList<CompoundStructureLayer> layers = val2.GetLayers();
			if (layers.Count == 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("GetCompoundStructureLayers: 元素 ");
				defaultInterpolatedStringHandler3.AppendFormatted<ElementId>(val.Id);
				defaultInterpolatedStringHandler3.AppendLiteral(" (");
				defaultInterpolatedStringHandler3.AppendFormatted(val.Name);
				defaultInterpolatedStringHandler3.AppendLiteral(") 的复合结构没有层（可能是简单墙类型）");
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
				return new List<CompoundLayerInfo>();
			}
			List<CompoundLayerInfo> list = new List<CompoundLayerInfo>();
			foreach (CompoundStructureLayer item in layers)
			{
				string materialName = null;
				if (item.MaterialId != (ElementId)null && item.MaterialId != ElementId.InvalidElementId)
				{
					Element element = val.Document.GetElement(item.MaterialId);
					Material val6 = (Material)(object)((element is Material) ? element : null);
					materialName = ((val6 != null) ? ((Element)val6).Name : null);
				}
				string materialFunctionName = GetMaterialFunctionName(item.Function);
				int num = layers.IndexOf(item);
				bool isCore = val2.IsCoreLayer(num);
				bool participatesInWrapping = val2.ParticipatesInWrapping(num);
				list.Add(new CompoundLayerInfo
				{
					Index = num,
					WidthMM = item.Width * 304.8,
					Function = ((object)item.Function/*cast due to constrained. prefix*/).ToString(),
					FunctionName = materialFunctionName,
					MaterialId = ((!(item.MaterialId != (ElementId)null) || !(item.MaterialId != ElementId.InvalidElementId)) ? ((int?)null) : new int?((int)item.MaterialId.Value)),
					MaterialName = materialName,
					IsCore = isCore,
					ParticipatesInWrapping = participatesInWrapping
				});
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(23, 3);
			defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功获取元素 ");
			defaultInterpolatedStringHandler4.AppendFormatted<ElementId>(val.Id);
			defaultInterpolatedStringHandler4.AppendLiteral(" (");
			defaultInterpolatedStringHandler4.AppendFormatted(val.Name);
			defaultInterpolatedStringHandler4.AppendLiteral(") 的 ");
			defaultInterpolatedStringHandler4.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 层复合结构信息");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetCompoundStructureLayers 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetCompoundStructureTotalThickness(object elementType)
	{
		try
		{
			Element val = (Element)((elementType is Element) ? elementType : null);
			if (val == null)
			{
				LogError("GetCompoundStructureTotalThickness: 无效的元素类型");
				return null;
			}
			CompoundStructure val2 = null;
			WallType val3 = (WallType)(object)((val is WallType) ? val : null);
			if (val3 != null)
			{
				val2 = ((HostObjAttributes)val3).GetCompoundStructure();
			}
			else
			{
				FloorType val4 = (FloorType)(object)((val is FloorType) ? val : null);
				if (val4 != null)
				{
					val2 = ((HostObjAttributes)val4).GetCompoundStructure();
				}
				else
				{
					RoofType val5 = (RoofType)(object)((val is RoofType) ? val : null);
					if (val5 == null)
					{
						return null;
					}
					val2 = ((HostObjAttributes)val5).GetCompoundStructure();
				}
			}
			if (val2 == null)
			{
				return null;
			}
			double width = val2.GetWidth();
			return width * 304.8;
		}
		catch (Exception ex)
		{
			LogError("GetCompoundStructureTotalThickness 失败: " + ex.Message);
			return null;
		}
	}

	public bool ModifyCompoundStructureLayer(object elementType, int layerIndex, object document, double? thicknessMM = null, int? materialId = null, string? function = null)
	{
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0335: Expected O, but got Unknown
		//IL_0403: Unknown result type (might be due to invalid IL or missing references)
		//IL_0440: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element val = (Element)((elementType is Element) ? elementType : null);
			if (val != null)
			{
				Document val2 = (Document)((document is Document) ? document : null);
				if (val2 != null)
				{
					CompoundStructure val3 = null;
					WallType val4 = (WallType)(object)((val is WallType) ? val : null);
					if (val4 != null)
					{
						val3 = ((HostObjAttributes)val4).GetCompoundStructure();
					}
					else
					{
						FloorType val5 = (FloorType)(object)((val is FloorType) ? val : null);
						if (val5 != null)
						{
							val3 = ((HostObjAttributes)val5).GetCompoundStructure();
						}
						else
						{
							RoofType val6 = (RoofType)(object)((val is RoofType) ? val : null);
							if (val6 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
								defaultInterpolatedStringHandler.AppendLiteral("ModifyCompoundStructureLayer: 元素 ");
								defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
								defaultInterpolatedStringHandler.AppendLiteral(" 不是墙类型、楼板类型或屋顶类型");
								LogError(defaultInterpolatedStringHandler.ToStringAndClear());
								return false;
							}
							val3 = ((HostObjAttributes)val6).GetCompoundStructure();
						}
					}
					if (val3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("ModifyCompoundStructureLayer: 元素 ");
						defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val.Id);
						defaultInterpolatedStringHandler2.AppendLiteral(" 没有复合结构");
						LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
						return false;
					}
					IList<CompoundStructureLayer> layers = val3.GetLayers();
					if (layerIndex < 0 || layerIndex >= layers.Count)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("ModifyCompoundStructureLayer: 层索引 ");
						defaultInterpolatedStringHandler3.AppendFormatted(layerIndex);
						defaultInterpolatedStringHandler3.AppendLiteral(" 超出范围（0-");
						defaultInterpolatedStringHandler3.AppendFormatted(layers.Count - 1);
						defaultInterpolatedStringHandler3.AppendLiteral("）");
						LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
						return false;
					}
					CompoundStructureLayer val7 = layers[layerIndex];
					bool flag = false;
					if (thicknessMM.HasValue)
					{
						double num = thicknessMM.Value / 304.8;
						if (num <= 0.0)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(43, 2);
							defaultInterpolatedStringHandler4.AppendLiteral("ModifyCompoundStructureLayer: 厚度值无效 ");
							defaultInterpolatedStringHandler4.AppendFormatted(thicknessMM.Value);
							defaultInterpolatedStringHandler4.AppendLiteral("mm（");
							defaultInterpolatedStringHandler4.AppendFormatted(num);
							defaultInterpolatedStringHandler4.AppendLiteral(" 英尺）");
							LogError(defaultInterpolatedStringHandler4.ToStringAndClear());
							return false;
						}
						try
						{
							double width = val7.Width;
							val3.SetLayerWidth(layerIndex, num);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(19, 3);
							defaultInterpolatedStringHandler5.AppendLiteral("✅ 层 ");
							defaultInterpolatedStringHandler5.AppendFormatted(layerIndex);
							defaultInterpolatedStringHandler5.AppendLiteral(" 厚度已从 ");
							defaultInterpolatedStringHandler5.AppendFormatted(width * 304.8, "F2");
							defaultInterpolatedStringHandler5.AppendLiteral("mm 修改为 ");
							defaultInterpolatedStringHandler5.AppendFormatted(thicknessMM.Value);
							defaultInterpolatedStringHandler5.AppendLiteral("mm");
							LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
							flag = true;
						}
						catch (Exception ex)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(46, 3);
							defaultInterpolatedStringHandler6.AppendLiteral("ModifyCompoundStructureLayer: 设置层 ");
							defaultInterpolatedStringHandler6.AppendFormatted(layerIndex);
							defaultInterpolatedStringHandler6.AppendLiteral(" 厚度失败 (");
							defaultInterpolatedStringHandler6.AppendFormatted(thicknessMM.Value);
							defaultInterpolatedStringHandler6.AppendLiteral("mm): ");
							defaultInterpolatedStringHandler6.AppendFormatted(ex.Message);
							LogError(defaultInterpolatedStringHandler6.ToStringAndClear());
							return false;
						}
					}
					if (materialId.HasValue)
					{
						ElementId val8 = new ElementId((long)materialId.Value);
						Element element = val2.GetElement(val8);
						Material val9 = (Material)(object)((element is Material) ? element : null);
						if (val9 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(39, 1);
							defaultInterpolatedStringHandler7.AppendLiteral("ModifyCompoundStructureLayer: 找不到材质 ID ");
							defaultInterpolatedStringHandler7.AppendFormatted(materialId.Value);
							LogWarning(defaultInterpolatedStringHandler7.ToStringAndClear());
							return false;
						}
						val3.SetMaterialId(layerIndex, ((Element)val9).Id);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(12, 2);
						defaultInterpolatedStringHandler8.AppendLiteral("✅ 层 ");
						defaultInterpolatedStringHandler8.AppendFormatted(layerIndex);
						defaultInterpolatedStringHandler8.AppendLiteral(" 材质已修改为 ");
						defaultInterpolatedStringHandler8.AppendFormatted(((Element)val9).Name);
						LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
						flag = true;
					}
					if (!string.IsNullOrEmpty(function))
					{
						if (Enum.TryParse<MaterialFunctionAssignment>(function, true, out MaterialFunctionAssignment result))
						{
							val3.SetLayerFunction(layerIndex, result);
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(12, 2);
							defaultInterpolatedStringHandler9.AppendLiteral("✅ 层 ");
							defaultInterpolatedStringHandler9.AppendFormatted(layerIndex);
							defaultInterpolatedStringHandler9.AppendLiteral(" 功能已修改为 ");
							defaultInterpolatedStringHandler9.AppendFormatted(GetMaterialFunctionName(result));
							LogInfo(defaultInterpolatedStringHandler9.ToStringAndClear());
							flag = true;
						}
						else
						{
							LogWarning("ModifyCompoundStructureLayer: 无效的层功能 '" + function + "'");
						}
					}
					if (!flag)
					{
						LogWarning("ModifyCompoundStructureLayer: 没有提供任何修改参数");
						return false;
					}
					try
					{
						WallType val10 = (WallType)(object)((val is WallType) ? val : null);
						if (val10 != null)
						{
							((HostObjAttributes)val10).SetCompoundStructure(val3);
						}
						else
						{
							FloorType val11 = (FloorType)(object)((val is FloorType) ? val : null);
							if (val11 != null)
							{
								((HostObjAttributes)val11).SetCompoundStructure(val3);
							}
							else
							{
								RoofType val12 = (RoofType)(object)((val is RoofType) ? val : null);
								if (val12 != null)
								{
									((HostObjAttributes)val12).SetCompoundStructure(val3);
								}
							}
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(18, 3);
						defaultInterpolatedStringHandler10.AppendLiteral("✅ 成功修改元素 ");
						defaultInterpolatedStringHandler10.AppendFormatted<ElementId>(val.Id);
						defaultInterpolatedStringHandler10.AppendLiteral(" (");
						defaultInterpolatedStringHandler10.AppendFormatted(val.Name);
						defaultInterpolatedStringHandler10.AppendLiteral(") 的复合层 ");
						defaultInterpolatedStringHandler10.AppendFormatted(layerIndex);
						LogInfo(defaultInterpolatedStringHandler10.ToStringAndClear());
						return true;
					}
					catch (Exception ex2)
					{
						LogError("ModifyCompoundStructureLayer: 应用复合结构更改失败: " + ex2.Message);
						return false;
					}
				}
			}
			LogError("ModifyCompoundStructureLayer: 无效的元素类型或文档");
			return false;
		}
		catch (Exception ex3)
		{
			LogError("ModifyCompoundStructureLayer 失败: " + ex3.Message);
			return false;
		}
	}

	public int AddCompoundStructureLayer(object elementType, object document, double thicknessMM, string function, int? materialId = null, int? insertIndex = null)
	{
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a5: Expected O, but got Unknown
		//IL_012f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		try
		{
			Element val = (Element)((elementType is Element) ? elementType : null);
			if (val != null)
			{
				Document val2 = (Document)((document is Document) ? document : null);
				if (val2 != null)
				{
					CompoundStructure val3 = null;
					WallType val4 = (WallType)(object)((val is WallType) ? val : null);
					if (val4 != null)
					{
						val3 = ((HostObjAttributes)val4).GetCompoundStructure();
					}
					else
					{
						FloorType val5 = (FloorType)(object)((val is FloorType) ? val : null);
						if (val5 != null)
						{
							val3 = ((HostObjAttributes)val5).GetCompoundStructure();
						}
						else
						{
							RoofType val6 = (RoofType)(object)((val is RoofType) ? val : null);
							if (val6 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(46, 1);
								defaultInterpolatedStringHandler.AppendLiteral("AddCompoundStructureLayer: 元素 ");
								defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
								defaultInterpolatedStringHandler.AppendLiteral(" 不是墙类型、楼板类型或屋顶类型");
								LogError(defaultInterpolatedStringHandler.ToStringAndClear());
								return -1;
							}
							val3 = ((HostObjAttributes)val6).GetCompoundStructure();
						}
					}
					if (val3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("AddCompoundStructureLayer: 元素 ");
						defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val.Id);
						defaultInterpolatedStringHandler2.AppendLiteral(" 没有复合结构");
						LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
						return -1;
					}
					if (!Enum.TryParse<MaterialFunctionAssignment>(function, true, out MaterialFunctionAssignment result))
					{
						LogError("AddCompoundStructureLayer: 无效的层功能 '" + function + "'");
						return -1;
					}
					double num = thicknessMM / 304.8;
					ElementId val7 = ElementId.InvalidElementId;
					if (materialId.HasValue)
					{
						ElementId val8 = new ElementId((long)materialId.Value);
						Element element = val2.GetElement(val8);
						Material val9 = (Material)(object)((element is Material) ? element : null);
						if (val9 == null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 1);
							defaultInterpolatedStringHandler3.AppendLiteral("AddCompoundStructureLayer: 找不到材质 ID ");
							defaultInterpolatedStringHandler3.AppendFormatted(materialId.Value);
							defaultInterpolatedStringHandler3.AppendLiteral("，将不设置材质");
							LogWarning(defaultInterpolatedStringHandler3.ToStringAndClear());
						}
						else
						{
							val7 = val8;
						}
					}
					CompoundStructureLayer item = new CompoundStructureLayer(num, result, val7);
					List<CompoundStructureLayer> list = val3.GetLayers().ToList();
					int num2;
					if (insertIndex.HasValue && insertIndex.Value >= 0 && insertIndex.Value <= list.Count)
					{
						list.Insert(insertIndex.Value, item);
						num2 = insertIndex.Value;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("在位置 ");
						defaultInterpolatedStringHandler4.AppendFormatted(insertIndex.Value);
						defaultInterpolatedStringHandler4.AppendLiteral(" 插入新层");
						LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
					}
					else
					{
						list.Add(item);
						num2 = list.Count - 1;
						LogInfo("添加新层到末尾");
					}
					val3.SetLayers((IList<CompoundStructureLayer>)list);
					WallType val10 = (WallType)(object)((val is WallType) ? val : null);
					if (val10 != null)
					{
						((HostObjAttributes)val10).SetCompoundStructure(val3);
					}
					else
					{
						FloorType val11 = (FloorType)(object)((val is FloorType) ? val : null);
						if (val11 != null)
						{
							((HostObjAttributes)val11).SetCompoundStructure(val3);
						}
						else
						{
							RoofType val12 = (RoofType)(object)((val is RoofType) ? val : null);
							if (val12 != null)
							{
								((HostObjAttributes)val12).SetCompoundStructure(val3);
							}
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(27, 3);
					defaultInterpolatedStringHandler5.AppendLiteral("✅ 成功在元素 ");
					defaultInterpolatedStringHandler5.AppendFormatted<ElementId>(val.Id);
					defaultInterpolatedStringHandler5.AppendLiteral(" (");
					defaultInterpolatedStringHandler5.AppendFormatted(val.Name);
					defaultInterpolatedStringHandler5.AppendLiteral(") 的复合结构中添加新层，索引: ");
					defaultInterpolatedStringHandler5.AppendFormatted(num2);
					LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
					return num2;
				}
			}
			LogError("AddCompoundStructureLayer: 无效的元素类型或文档");
			return -1;
		}
		catch (Exception ex)
		{
			LogError("AddCompoundStructureLayer 失败: " + ex.Message);
			return -1;
		}
	}

	public bool DeleteCompoundStructureLayer(object elementType, int layerIndex, object document)
	{
		try
		{
			Element val = (Element)((elementType is Element) ? elementType : null);
			if (val != null)
			{
				Document val2 = (Document)((document is Document) ? document : null);
				if (val2 != null)
				{
					CompoundStructure val3 = null;
					WallType val4 = (WallType)(object)((val is WallType) ? val : null);
					if (val4 != null)
					{
						val3 = ((HostObjAttributes)val4).GetCompoundStructure();
					}
					else
					{
						FloorType val5 = (FloorType)(object)((val is FloorType) ? val : null);
						if (val5 != null)
						{
							val3 = ((HostObjAttributes)val5).GetCompoundStructure();
						}
						else
						{
							RoofType val6 = (RoofType)(object)((val is RoofType) ? val : null);
							if (val6 == null)
							{
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
								defaultInterpolatedStringHandler.AppendLiteral("DeleteCompoundStructureLayer: 元素 ");
								defaultInterpolatedStringHandler.AppendFormatted<ElementId>(val.Id);
								defaultInterpolatedStringHandler.AppendLiteral(" 不是墙类型、楼板类型或屋顶类型");
								LogError(defaultInterpolatedStringHandler.ToStringAndClear());
								return false;
							}
							val3 = ((HostObjAttributes)val6).GetCompoundStructure();
						}
					}
					if (val3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(40, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("DeleteCompoundStructureLayer: 元素 ");
						defaultInterpolatedStringHandler2.AppendFormatted<ElementId>(val.Id);
						defaultInterpolatedStringHandler2.AppendLiteral(" 没有复合结构");
						LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
						return false;
					}
					IList<CompoundStructureLayer> layers = val3.GetLayers();
					if (layerIndex < 0 || layerIndex >= layers.Count)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 2);
						defaultInterpolatedStringHandler3.AppendLiteral("DeleteCompoundStructureLayer: 层索引 ");
						defaultInterpolatedStringHandler3.AppendFormatted(layerIndex);
						defaultInterpolatedStringHandler3.AppendLiteral(" 超出范围（0-");
						defaultInterpolatedStringHandler3.AppendFormatted(layers.Count - 1);
						defaultInterpolatedStringHandler3.AppendLiteral("）");
						LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
						return false;
					}
					if (layers.Count <= 1)
					{
						LogError("DeleteCompoundStructureLayer: 不能删除最后一层，复合结构至少需要一层");
						return false;
					}
					List<CompoundStructureLayer> list = layers.ToList();
					list.RemoveAt(layerIndex);
					val3.SetLayers((IList<CompoundStructureLayer>)list);
					WallType val7 = (WallType)(object)((val is WallType) ? val : null);
					if (val7 != null)
					{
						((HostObjAttributes)val7).SetCompoundStructure(val3);
					}
					else
					{
						FloorType val8 = (FloorType)(object)((val is FloorType) ? val : null);
						if (val8 != null)
						{
							((HostObjAttributes)val8).SetCompoundStructure(val3);
						}
						else
						{
							RoofType val9 = (RoofType)(object)((val is RoofType) ? val : null);
							if (val9 != null)
							{
								((HostObjAttributes)val9).SetCompoundStructure(val3);
							}
						}
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(18, 3);
					defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功删除元素 ");
					defaultInterpolatedStringHandler4.AppendFormatted<ElementId>(val.Id);
					defaultInterpolatedStringHandler4.AppendLiteral(" (");
					defaultInterpolatedStringHandler4.AppendFormatted(val.Name);
					defaultInterpolatedStringHandler4.AppendLiteral(") 的复合层 ");
					defaultInterpolatedStringHandler4.AppendFormatted(layerIndex);
					LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
					return true;
				}
			}
			LogError("DeleteCompoundStructureLayer: 无效的元素类型或文档");
			return false;
		}
		catch (Exception ex)
		{
			LogError("DeleteCompoundStructureLayer 失败: " + ex.Message);
			return false;
		}
	}

	private unsafe static string GetMaterialFunctionName(MaterialFunctionAssignment function)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected I4, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Invalid comparison between Unknown and I4
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Invalid comparison between Unknown and I4
		switch ((int)(function) - 1)
		{
		default:
			if ((int)function != 100)
			{
				if ((int)function != 200)
				{
					return ((object)(*(MaterialFunctionAssignment*)(&function))/*cast due to constrained. prefix*/).ToString();
				}
				return "结构板";
			}
			return "防潮层";
		case 0:
			return "结构层";
		case 1:
			return "基层";
		case 2:
			return "保温层";
		case 3:
			return "面层1";
		case 4:
			return "面层2";
		}
	}

	public bool LockElement(object document, int elementId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("LockElement: document 不是 Document 类型");
				return false;
			}
			ElementId val2 = new ElementId((long)elementId);
			Element element = val.GetElement(val2);
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(22, 1);
				defaultInterpolatedStringHandler.AppendLiteral("LockElement: 找不到元素 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(elementId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (element != null)
			{
				Element val3 = element;
				if (0 == 0)
				{
					if (val3.Pinned)
					{
						return true;
					}
					val3.Pinned = true;
					return true;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(30, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("LockElement: 元素 ");
			defaultInterpolatedStringHandler2.AppendFormatted(elementId);
			defaultInterpolatedStringHandler2.AppendLiteral(" 不是 Element 类型");
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			return false;
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("LockElement: 锁定元素 ");
			defaultInterpolatedStringHandler3.AppendFormatted(elementId);
			defaultInterpolatedStringHandler3.AppendLiteral(" 失败: ");
			defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
			LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
			return false;
		}
	}

	public bool UnlockElement(object document, int elementId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("UnlockElement: document 不是 Document 类型");
				return false;
			}
			ElementId val2 = new ElementId((long)elementId);
			Element element = val.GetElement(val2);
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
				defaultInterpolatedStringHandler.AppendLiteral("UnlockElement: 找不到元素 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(elementId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (element != null)
			{
				Element val3 = element;
				if (0 == 0)
				{
					if (!val3.Pinned)
					{
						return true;
					}
					val3.Pinned = false;
					return true;
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("UnlockElement: 元素 ");
			defaultInterpolatedStringHandler2.AppendFormatted(elementId);
			defaultInterpolatedStringHandler2.AppendLiteral(" 不是 Element 类型");
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			return false;
		}
		catch (Exception ex)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(25, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("UnlockElement: 解锁元素 ");
			defaultInterpolatedStringHandler3.AppendFormatted(elementId);
			defaultInterpolatedStringHandler3.AppendLiteral(" 失败: ");
			defaultInterpolatedStringHandler3.AppendFormatted(ex.Message);
			LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
			return false;
		}
	}

	public IEnumerable<object> GetRoofTypes(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(RoofType));
			return val2.ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("[Roof] GetRoofTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? CreateFootPrintRoof(object document, IEnumerable<(double X, double Y)> points, int levelId, int? roofTypeId = null)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Expected O, but got Unknown
		//IL_017a: Expected O, but got Unknown
		//IL_0191: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("[Roof] CreateFootPrintRoof: document 不是 Document 类型");
				return null;
			}
			List<(double, double)> list = points?.ToList();
			if (list == null || list.Count < 3)
			{
				LogError("[Roof] CreateFootPrintRoof: 轮廓点数量不足，至少需要 3 个点");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Roof] CreateFootPrintRoof: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			RoofType val3 = null;
			if (roofTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)roofTypeId.Value));
				val3 = (RoofType)(object)((element2 is RoofType) ? element2 : null);
			}
			if (val3 == null)
			{
				val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(RoofType))).Cast<RoofType>().FirstOrDefault();
			}
			if (val3 == null)
			{
				LogError("[Roof] CreateFootPrintRoof: 找不到可用的屋面类型");
				return null;
			}
			CurveArray val4 = new CurveArray();
			for (int i = 0; i < list.Count; i++)
			{
				(double, double) tuple = list[i];
				(double, double) tuple2 = list[(i + 1) % list.Count];
				val4.Append((Curve)(object)Line.CreateBound(new XYZ(tuple.Item1, tuple.Item2, 0.0), new XYZ(tuple2.Item1, tuple2.Item2, 0.0)));
			}
			ModelCurveArray val5 = new ModelCurveArray();
			FootPrintRoof val6 = val.Create.NewFootPrintRoof(val4, val2, val3, out val5);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[Roof] 成功创建迹线屋面 (ID: ");
			long? value;
			if (val6 == null)
			{
				value = null;
			}
			else
			{
				ElementId id = ((Element)val6).Id;
				value = ((id != null) ? new long?(id.Value) : ((long?)null));
			}
			defaultInterpolatedStringHandler2.AppendFormatted(value);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val6;
		}
		catch (Exception ex)
		{
			LogError("[Roof] CreateFootPrintRoof 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateExtrusionRoof(object document, IEnumerable<(double X, double Y)> points, int levelId, int? roofTypeId = null, double extrusionStart = 0.0, double extrusionEnd = 30.0)
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Expected O, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0113: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Expected O, but got Unknown
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_0263: Unknown result type (might be due to invalid IL or missing references)
		//IL_026d: Expected O, but got Unknown
		//IL_026d: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("[Roof] CreateExtrusionRoof: document 不是 Document 类型");
				return null;
			}
			List<(double, double)> list = points?.ToList();
			if (list == null || list.Count < 2)
			{
				LogError("[Roof] CreateExtrusionRoof: 轮廓点数量不足，至少需要 2 个点");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(37, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Roof] CreateExtrusionRoof: 找不到标高 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			RoofType val3 = null;
			if (roofTypeId.HasValue)
			{
				Element element2 = val.GetElement(new ElementId((long)roofTypeId.Value));
				val3 = (RoofType)(object)((element2 is RoofType) ? element2 : null);
			}
			if (val3 == null)
			{
				val3 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(RoofType))).Cast<RoofType>().FirstOrDefault();
			}
			if (val3 == null)
			{
				LogError("[Roof] CreateExtrusionRoof: 找不到可用的屋面类型");
				return null;
			}
			List<ReferencePlane> source = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(ReferencePlane))).Cast<ReferencePlane>().ToList();
			ReferencePlane val4 = source.FirstOrDefault((ReferencePlane p) => Math.Abs(p.Normal.DotProduct(XYZ.BasisZ)) < 1E-09) ?? source.FirstOrDefault();
			if (val4 == null)
			{
				XYZ val5 = new XYZ(0.0, 0.0, 0.0);
				XYZ val6 = new XYZ(100.0, 0.0, 0.0);
				XYZ val7 = new XYZ(0.0, 0.0, 1.0);
				val4 = ((ItemFactoryBase)val.Create).NewReferencePlane(val5, val6, val7, val.ActiveView);
				if (val4 == null)
				{
					LogError("[Roof] CreateExtrusionRoof: 无法创建参照平面");
					return null;
				}
			}
			CurveArray val8 = new CurveArray();
			for (int num = 0; num < list.Count - 1; num++)
			{
				(double, double) tuple = list[num];
				(double, double) tuple2 = list[num + 1];
				val8.Append((Curve)(object)Line.CreateBound(new XYZ(tuple.Item1, 0.0, tuple.Item2), new XYZ(tuple2.Item1, 0.0, tuple2.Item2)));
			}
			ExtrusionRoof val9 = val.Create.NewExtrusionRoof(val8, val4, val2, val3, extrusionStart, extrusionEnd);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(22, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[Roof] 成功创建挤出屋面 (ID: ");
			long? value;
			if (val9 == null)
			{
				value = null;
			}
			else
			{
				ElementId id = ((Element)val9).Id;
				value = ((id != null) ? new long?(id.Value) : ((long?)null));
			}
			defaultInterpolatedStringHandler2.AppendFormatted(value);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val9;
		}
		catch (Exception ex)
		{
			LogError("[Roof] CreateExtrusionRoof 失败: " + ex.Message);
			return null;
		}
	}

	public bool SetRoofEdgeSlope(object document, object roofElement, int edgeIndex, bool definesSlope, double slopeAngle)
	{
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Expected O, but got Unknown
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Expected O, but got Unknown
		try
		{
			FootPrintRoof val = (FootPrintRoof)((roofElement is FootPrintRoof) ? roofElement : null);
			if (val == null)
			{
				LogError("[Roof] SetRoofEdgeSlope: 元素不是迹线屋面");
				return false;
			}
			ModelCurveArrArray profiles = val.GetProfiles();
			int num = 0;
			ModelCurve val2 = null;
			foreach (ModelCurveArray item in profiles)
			{
				ModelCurveArray val3 = item;
				foreach (ModelCurve item2 in val3)
				{
					ModelCurve val4 = item2;
					if (num != edgeIndex)
					{
						num++;
						continue;
					}
					val2 = val4;
					break;
				}
				if (val2 != null)
				{
					break;
				}
			}
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Roof] SetRoofEdgeSlope: 找不到边索引 ");
				defaultInterpolatedStringHandler.AppendFormatted(edgeIndex);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			val.set_DefinesSlope(val2, definesSlope);
			if (definesSlope && slopeAngle > 0.0)
			{
				val.set_SlopeAngle(val2, slopeAngle * Math.PI / 180.0);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[Roof] 成功设置屋面边 ");
			defaultInterpolatedStringHandler2.AppendFormatted(edgeIndex);
			defaultInterpolatedStringHandler2.AppendLiteral(" 的坡度: ");
			defaultInterpolatedStringHandler2.AppendFormatted(definesSlope);
			defaultInterpolatedStringHandler2.AppendLiteral(", 角度: ");
			defaultInterpolatedStringHandler2.AppendFormatted(slopeAngle);
			defaultInterpolatedStringHandler2.AppendLiteral("°");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("[Roof] SetRoofEdgeSlope 失败: " + ex.Message);
			return false;
		}
	}

	public IEnumerable<(int EdgeIndex, bool DefinesSlope, double SlopeAngle)>? GetFootPrintRoofEdges(object roofElement)
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		try
		{
			FootPrintRoof val = (FootPrintRoof)((roofElement is FootPrintRoof) ? roofElement : null);
			if (val == null)
			{
				LogError("[Roof] GetFootPrintRoofEdges: 元素不是迹线屋面");
				return null;
			}
			List<(int, bool, double)> list = new List<(int, bool, double)>();
			ModelCurveArrArray profiles = val.GetProfiles();
			int num = 0;
			foreach (ModelCurveArray item3 in profiles)
			{
				ModelCurveArray val2 = item3;
				foreach (ModelCurve item4 in val2)
				{
					ModelCurve val3 = item4;
					bool item = val.get_DefinesSlope(val3);
					double num2 = val.get_SlopeAngle(val3);
					double item2 = num2 * 180.0 / Math.PI;
					list.Add((num, item, item2));
					num++;
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("[Roof] GetFootPrintRoofEdges 失败: " + ex.Message);
			return null;
		}
	}

	public bool CheckCollision(object document, int sourceElementId, int targetElementId)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("[Collision] 文档对象无效");
				return false;
			}
			ElementId val2 = new ElementId((long)sourceElementId);
			ElementId val3 = new ElementId((long)targetElementId);
			Element element = val.GetElement(val2);
			Element element2 = val.GetElement(val3);
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[Collision] 源元素 ");
				defaultInterpolatedStringHandler.AppendFormatted(sourceElementId);
				defaultInterpolatedStringHandler.AppendLiteral(" 不存在");
				Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			if (element2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(21, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[Collision] 目标元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(targetElementId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 不存在");
				Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
				return false;
			}
			ElementIntersectsElementFilter val4 = new ElementIntersectsElementFilter(element);
			bool flag = ((ElementFilter)val4).PassesFilter(val, val3);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(27, 3);
			defaultInterpolatedStringHandler3.AppendLiteral("[Collision] 碰撞检查: 元素 ");
			defaultInterpolatedStringHandler3.AppendFormatted(sourceElementId);
			defaultInterpolatedStringHandler3.AppendLiteral(" 与 ");
			defaultInterpolatedStringHandler3.AppendFormatted(targetElementId);
			defaultInterpolatedStringHandler3.AppendLiteral(" → ");
			defaultInterpolatedStringHandler3.AppendFormatted(flag ? "存在碰撞" : "无碰撞");
			Logger.Debug(defaultInterpolatedStringHandler3.ToStringAndClear());
			return flag;
		}
		catch (Exception ex)
		{
			LogError("[Collision] CheckCollision 失败: " + ex.Message);
			return false;
		}
	}

	public IEnumerable<CollisionResultItem> CheckCollisions(object document, IEnumerable<(int SourceId, int TargetId)> elementPairs)
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Expected O, but got Unknown
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Expected O, but got Unknown
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		List<CollisionResultItem> list = new List<CollisionResultItem>();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("[Collision] 文档对象无效");
				return list;
			}
			List<(int, int)> list2 = elementPairs.ToList();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[Collision] 开始批量碰撞检查，共 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 对元素");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (var (num, num2) in list2)
			{
				try
				{
					ElementId val2 = new ElementId((long)num);
					ElementId val3 = new ElementId((long)num2);
					Element element = val.GetElement(val2);
					Element element2 = val.GetElement(val3);
					if (element != null && element2 != null)
					{
						ElementIntersectsElementFilter val4 = new ElementIntersectsElementFilter(element);
						bool hasCollision = ((ElementFilter)val4).PassesFilter(val, val3);
						string elementCategory = GetElementCategory(element);
						string elementTypeName = GetElementTypeName(element);
						string elementFamilyName = GetElementFamilyName(element);
						string elementCategory2 = GetElementCategory(element2);
						string elementTypeName2 = GetElementTypeName(element2);
						string elementFamilyName2 = GetElementFamilyName(element2);
						list.Add(new CollisionResultItem
						{
							SourceElementId = num,
							SourceCategoryName = elementCategory,
							SourceTypeName = elementTypeName,
							SourceFamilyName = elementFamilyName,
							TargetElementId = num2,
							TargetCategoryName = elementCategory2,
							TargetTypeName = elementTypeName2,
							TargetFamilyName = elementFamilyName2,
							HasCollision = hasCollision,
							CheckTime = DateTime.UtcNow
						});
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("[Collision] 检查元素对 (");
					defaultInterpolatedStringHandler2.AppendFormatted(num);
					defaultInterpolatedStringHandler2.AppendLiteral(", ");
					defaultInterpolatedStringHandler2.AppendFormatted(num2);
					defaultInterpolatedStringHandler2.AppendLiteral(") 失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			int value = list.Count((CollisionResultItem r) => r.HasCollision);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[Collision] 批量检查完成: ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 对，其中 ");
			defaultInterpolatedStringHandler3.AppendFormatted(value);
			defaultInterpolatedStringHandler3.AppendLiteral(" 对存在碰撞");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		catch (Exception ex2)
		{
			LogError("[Collision] CheckCollisions 失败: " + ex2.Message);
		}
		return list;
	}

	public IEnumerable<CollisionResultItem> CheckCollisionsBatch(object document, IEnumerable<int> sourceIds, IEnumerable<int> targetIds)
	{
		//IL_0129: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Expected O, but got Unknown
		//IL_031d: Unknown result type (might be due to invalid IL or missing references)
		//IL_036f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_0381: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03de: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		List<CollisionResultItem> list = new List<CollisionResultItem>();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("[Collision] 文档对象无效");
				return list;
			}
			List<int> list2 = sourceIds.Distinct().ToList();
			List<int> list3 = targetIds.Distinct().ToList();
			if (list2.Count == 0 || list3.Count == 0)
			{
				return list;
			}
			bool flag = list2.Count == list3.Count && !list2.Except(list3).Any();
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[Collision] 开始批量碰撞检查（优化模式）：源 ");
			defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个，目标 ");
			defaultInterpolatedStringHandler.AppendFormatted(list3.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个，自碰撞: ");
			defaultInterpolatedStringHandler.AppendFormatted(flag);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			Dictionary<int, (Element, BoundingBoxXYZ, string, string, string)> dictionary = new Dictionary<int, (Element, BoundingBoxXYZ, string, string, string)>();
			IEnumerable<int> enumerable;
			if (!flag)
			{
				enumerable = list2.Union(list3);
			}
			else
			{
				IEnumerable<int> enumerable2 = list2;
				enumerable = enumerable2;
			}
			IEnumerable<int> enumerable3 = enumerable;
			foreach (int item3 in enumerable3)
			{
				try
				{
					Element element = val.GetElement(new ElementId((long)item3));
					if (element != null)
					{
						BoundingBoxXYZ item = null;
						try
						{
							item = element.get_BoundingBox((View)null);
						}
						catch
						{
						}
						dictionary[item3] = (element, item, GetElementCategory(element) ?? string.Empty, GetElementTypeName(element) ?? string.Empty, GetElementFamilyName(element) ?? string.Empty);
					}
				}
				catch
				{
				}
			}
			int num = 0;
			for (int i = 0; i < list2.Count; i++)
			{
				int sourceId = list2[i];
				if (!dictionary.TryGetValue(sourceId, out var value))
				{
					continue;
				}
				List<int> list4 = ((!flag) ? list3.Where((int id) => id != sourceId).ToList() : list2.Skip(i + 1).ToList());
				if (list4.Count == 0)
				{
					continue;
				}
				BoundingBoxXYZ item2 = value.Item2;
				List<int> list5 = new List<int>();
				foreach (int item4 in list4)
				{
					if (dictionary.TryGetValue(item4, out var value2))
					{
						if (item2 == null || value2.Item2 == null)
						{
							list5.Add(item4);
						}
						else if (BoundingBoxesIntersect(item2, value2.Item2))
						{
							list5.Add(item4);
						}
					}
				}
				num += list4.Count - list5.Count;
				if (list5.Count == 0)
				{
					continue;
				}
				try
				{
					List<ElementId> list6 = ((IEnumerable<int>)list5).Select((Func<int, ElementId>)delegate(int id)
					{
						//IL_0002: Unknown result type (might be due to invalid IL or missing references)
						//IL_0008: Expected O, but got Unknown
						return new ElementId((long)id);
					}).ToList();
					ElementIntersectsElementFilter val2 = new ElementIntersectsElementFilter(value.Item1);
					List<ElementId> list7 = new FilteredElementCollector(val, (ICollection<ElementId>)list6).WherePasses((ElementFilter)(object)val2).ToElementIds().ToList();
					DateTime utcNow = DateTime.UtcNow;
					foreach (ElementId item5 in list7)
					{
						int num2 = (int)item5.Value;
						if (dictionary.TryGetValue(num2, out var value3))
						{
							list.Add(new CollisionResultItem
							{
								SourceElementId = sourceId,
								SourceCategoryName = value.Item3,
								SourceTypeName = value.Item4,
								SourceFamilyName = value.Item5,
								TargetElementId = num2,
								TargetCategoryName = value3.Item3,
								TargetTypeName = value3.Item4,
								TargetFamilyName = value3.Item5,
								HasCollision = true,
								CheckTime = utcNow
							});
						}
					}
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(25, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[Collision] 批量检查源元素 ");
					defaultInterpolatedStringHandler2.AppendFormatted(sourceId);
					defaultInterpolatedStringHandler2.AppendLiteral(" 失败: ");
					defaultInterpolatedStringHandler2.AppendFormatted(ex.Message);
					LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(47, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[Collision] 批量检查完成（优化模式）: 发现 ");
			defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler3.AppendLiteral(" 对碰撞，BBox 预过滤剔除 ");
			defaultInterpolatedStringHandler3.AppendFormatted(num);
			defaultInterpolatedStringHandler3.AppendLiteral(" 对");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		}
		catch (Exception ex2)
		{
			LogError("[Collision] CheckCollisionsBatch 失败: " + ex2.Message);
		}
		return list;
	}

	private static bool BoundingBoxesIntersect(BoundingBoxXYZ bbox1, BoundingBoxXYZ bbox2)
	{
		if (bbox1 == null || bbox2 == null)
		{
			return false;
		}
		return !(bbox1.Max.X < bbox2.Min.X) && !(bbox2.Max.X < bbox1.Min.X) && !(bbox1.Max.Y < bbox2.Min.Y) && !(bbox2.Max.Y < bbox1.Min.Y) && !(bbox1.Max.Z < bbox2.Min.Z) && !(bbox2.Max.Z < bbox1.Min.Z);
	}

	private string? GetElementFamilyName(object element)
	{
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Expected I4, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val != null && val.Category != null)
			{
				Instance val2 = (Instance)(object)((val is Instance) ? val : null);
				if (val2 != null)
				{
					ElementId typeId = ((Element)val2).GetTypeId();
					if (typeId != (ElementId)null && typeId != ElementId.InvalidElementId)
					{
						Document document = val.Document;
						Element element2 = document.GetElement(typeId);
						if (element2 != null)
						{
							Parameter val3 = ((IEnumerable)element2.Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Equals("Family Name", StringComparison.OrdinalIgnoreCase) || p.Definition.Name.Equals("族名称", StringComparison.OrdinalIgnoreCase));
							if (val3 != null && val3.HasValue)
							{
								StorageType storageType = val3.StorageType;
								StorageType val4 = storageType;
								switch ((int)(val4) - 1)
								{
								default:
									return string.Empty;
								case 0:
									return val3.AsInteger().ToString();
								case 1:
									return val3.AsDouble().ToString();
								case 2:
									return val3.AsString();
								case 3:
								{
									ElementId val5 = val3.AsElementId();
									Element element3 = document.GetElement(val5);
									return (element3 != null) ? element3.Name : null;
								}
								}
							}
							string name = element2.Name;
							string[] array = name.Split(new char[1] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
							if (array.Length >= 2)
							{
								return array[0];
							}
						}
					}
				}
				return val.Name;
			}
			return null;
		}
		catch (Exception)
		{
			return null;
		}
	}

	public IEnumerable<object> GetAllGlobalParameters(object document)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllGlobalParameters: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			IList<Element> list2 = new FilteredElementCollector(val).OfClass(typeof(GlobalParameter)).ToElements();
			foreach (GlobalParameter item in list2)
			{
				GlobalParameter val2 = item;
				InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
				if (definition == null)
				{
					continue;
				}
				string text = null;
				string text2 = null;
				object gparam_ = null;
				try
				{
					ForgeTypeId dataType = ((Definition)definition).GetDataType();
					text = GetDataTypeName(dataType);
					text2 = val2.GetFormula();
				}
				catch
				{
					text = "Text";
					text2 = null;
				}
				try
				{
					ParameterValue value = val2.GetValue();
					DoubleParameterValue val3 = (DoubleParameterValue)(object)((value is DoubleParameterValue) ? value : null);
					if (val3 != null)
					{
						ForgeTypeId dataType2 = ((Definition)definition).GetDataType();
						gparam_ = ((dataType2 == SpecTypeId.Length) ? ((object)Math.Round(val3.Value * 304.8, 2)) : ((dataType2 == SpecTypeId.Area) ? ((object)Math.Round(val3.Value / 10.7639, 2)) : ((dataType2 == SpecTypeId.Volume) ? ((object)Math.Round(val3.Value / 35.3147, 2)) : ((!(dataType2 == SpecTypeId.Angle)) ? ((object)val3.Value) : ((object)Math.Round(val3.Value * 180.0 / Math.PI, 2))))));
					}
					else
					{
						IntegerParameterValue val4 = (IntegerParameterValue)(object)((value is IntegerParameterValue) ? value : null);
						if (val4 != null)
						{
							ForgeTypeId dataType3 = ((Definition)definition).GetDataType();
							gparam_ = ((!(dataType3 == SpecTypeId.Boolean.YesNo)) ? ((object)val4.Value) : ((object)(val4.Value == 1)));
						}
						else
						{
							StringParameterValue val5 = (StringParameterValue)(object)((value is StringParameterValue) ? value : null);
							if (val5 != null)
							{
								gparam_ = val5.Value;
							}
						}
					}
				}
				catch
				{
					gparam_ = null;
				}
				list.Add(new Class317<string, string, string, object>(((Definition)definition).Name, text ?? "Text", text2, gparam_));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetAllGlobalParameters: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个全局参数");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllGlobalParameters 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetGlobalParameter(object document, string parameterName)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetGlobalParameter: document 不是 Document 类型");
				return null;
			}
			GlobalParameter val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(GlobalParameter))).Cast<GlobalParameter>().FirstOrDefault((GlobalParameter p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				return null;
			}
			InternalDefinition definition = ((ParameterElement)val2).GetDefinition();
			if (definition == null)
			{
				return null;
			}
			string text = null;
			string text2 = null;
			object gparam_ = null;
			try
			{
				ForgeTypeId dataType = ((Definition)definition).GetDataType();
				text = GetDataTypeName(dataType);
				text2 = val2.GetFormula();
			}
			catch
			{
				text = "Text";
				text2 = null;
			}
			try
			{
				ParameterValue value = val2.GetValue();
				DoubleParameterValue val3 = (DoubleParameterValue)(object)((value is DoubleParameterValue) ? value : null);
				if (val3 != null)
				{
					gparam_ = val3.Value;
				}
				else
				{
					IntegerParameterValue val4 = (IntegerParameterValue)(object)((value is IntegerParameterValue) ? value : null);
					if (val4 != null)
					{
						gparam_ = val4.Value;
					}
					else
					{
						StringParameterValue val5 = (StringParameterValue)(object)((value is StringParameterValue) ? value : null);
						if (val5 != null)
						{
							gparam_ = val5.Value;
						}
					}
				}
			}
			catch
			{
				gparam_ = null;
			}
			return new Class317<string, string, string, object>(((Definition)definition).Name, text ?? "Text", text2, gparam_);
		}
		catch (Exception ex)
		{
			LogError("GetGlobalParameter 失败: " + ex.Message);
			return null;
		}
	}

	public object? CreateGlobalParameter(object document, string parameterName, string parameterType, string? formula = null, object? value = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateGlobalParameter: document 不是 Document 类型");
				return new Class315<bool, string>(gparam_2: false, "document 不是 Document 类型");
			}
			object globalParameter = GetGlobalParameter(document, parameterName);
			if (globalParameter != null)
			{
				return new Class315<bool, string>(gparam_2: false, "全局参数 '" + parameterName + "' 已存在");
			}
			string text = parameterType.ToLowerInvariant();
			uint num = Class653.smethod_0(text);
			ForgeTypeId val2;
			if (num <= 2211460629u)
			{
				if (num <= 467038368)
				{
					if (num != 436467439)
					{
						if (num != 467038368 || !(text == "number"))
						{
							goto IL_0202;
						}
						val2 = SpecTypeId.Number;
					}
					else
					{
						if (!(text == "yesno"))
						{
							goto IL_0202;
						}
						val2 = SpecTypeId.Boolean.YesNo;
					}
				}
				else if (num != 786459023)
				{
					if (num != 2211460629u || !(text == "length"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Length;
				}
				else
				{
					if (!(text == "volume"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Volume;
				}
			}
			else if (num <= 2907980824u)
			{
				if (num != 2601460036u)
				{
					if (num != 2907980824u || !(text == "angle"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Angle;
				}
				else
				{
					if (!(text == "area"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Area;
				}
			}
			else if (num != 3185987134u)
			{
				if (num != 3218261061u)
				{
					if (num != 3538210912u || !(text == "material"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Reference.Material;
				}
				else
				{
					if (!(text == "integer"))
					{
						goto IL_0202;
					}
					val2 = SpecTypeId.Int.Integer;
				}
			}
			else
			{
				if (!(text == "text"))
				{
					goto IL_0202;
				}
				val2 = SpecTypeId.String.Text;
			}
			goto IL_0212;
			IL_0212:
			ForgeTypeId val3 = val2;
			GlobalParameter val4 = GlobalParameter.Create(val, parameterName, val3);
			if (val4 == null)
			{
				return new Class315<bool, string>(gparam_2: false, "创建全局参数失败");
			}
			if (!string.IsNullOrEmpty(formula))
			{
				try
				{
					if (val4.IsValidFormula(formula))
					{
						val4.SetFormula(formula);
						LogInfo("CreateGlobalParameter: 成功设置公式 '" + formula + "'");
					}
					else
					{
						LogWarning("CreateGlobalParameter: 公式 '" + formula + "' 无效，未设置");
					}
				}
				catch (Exception ex)
				{
					LogError("CreateGlobalParameter: 设置公式失败 - " + ex.Message);
				}
			}
			if (value != null)
			{
				try
				{
					SetGlobalParameterValue(document, parameterName, value);
				}
				catch (Exception ex2)
				{
					LogError("CreateGlobalParameter: 设置值失败 - " + ex2.Message);
				}
			}
			LogInfo("CreateGlobalParameter: 成功创建全局参数 '" + parameterName + "'");
			return new Class318<bool, string, string, string, object>(gparam_5: true, parameterName, parameterType, formula, value);
			IL_0202:
			val2 = SpecTypeId.String.Text;
			goto IL_0212;
		}
		catch (Exception ex3)
		{
			LogError("CreateGlobalParameter 失败: " + ex3.Message);
			return new Class315<bool, string>(gparam_2: false, ex3.Message);
		}
	}

	public bool SetGlobalParameterValue(object document, string parameterName, object value)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_03e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03eb: Expected O, but got Unknown
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Expected O, but got Unknown
		//IL_0559: Unknown result type (might be due to invalid IL or missing references)
		//IL_0563: Expected O, but got Unknown
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Expected O, but got Unknown
		//IL_06b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c3: Expected O, but got Unknown
		//IL_061f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0629: Expected O, but got Unknown
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0364: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetGlobalParameterValue: document 不是 Document 类型");
				return false;
			}
			GlobalParameter val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(GlobalParameter))).Cast<GlobalParameter>().FirstOrDefault((GlobalParameter p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				LogWarning("SetGlobalParameterValue: 找不到参数 '" + parameterName + "'");
				return false;
			}
			ForgeTypeId dataType = ((Definition)((ParameterElement)val2).GetDefinition()).GetDataType();
			object obj;
			if (dataType == null)
			{
				obj = null;
			}
			else
			{
				obj = ((object)dataType).ToString();
				if (obj != null)
				{
					goto IL_00b9;
				}
			}
			obj = "null";
			goto IL_00b9;
			IL_0711:
			LogInfo("SetGlobalParameterValue: 成功设置全局参数 '" + parameterName + "' 的值");
			return true;
			IL_00b9:
			string value2 = (string)obj;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(56, 4);
			defaultInterpolatedStringHandler.AppendLiteral("SetGlobalParameterValue: 参数 '");
			defaultInterpolatedStringHandler.AppendFormatted(parameterName);
			defaultInterpolatedStringHandler.AppendLiteral("' 类型规范 = ");
			defaultInterpolatedStringHandler.AppendFormatted(value2);
			defaultInterpolatedStringHandler.AppendLiteral(", 输入值类型 = ");
			defaultInterpolatedStringHandler.AppendFormatted(value?.GetType().Name);
			defaultInterpolatedStringHandler.AppendLiteral(", 输入值 = ");
			defaultInterpolatedStringHandler.AppendFormatted<object>(value);
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			object obj2;
			if (dataType == SpecTypeId.String.Text)
			{
				if (value == null)
				{
					obj2 = null;
				}
				else
				{
					obj2 = value.ToString();
					if (obj2 != null)
					{
						goto IL_0174;
					}
				}
				obj2 = "";
				goto IL_0174;
			}
			int num4;
			object obj3;
			if (dataType == SpecTypeId.Int.Integer)
			{
				int num2;
				if (value is int num)
				{
					num2 = num;
				}
				else if (value is double num3)
				{
					num2 = (int)Math.Round(num3);
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(50, 3);
					defaultInterpolatedStringHandler2.AppendLiteral("SetGlobalParameterValue: Integer 参数 '");
					defaultInterpolatedStringHandler2.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler2.AppendLiteral("' - 将 ");
					defaultInterpolatedStringHandler2.AppendFormatted(num3);
					defaultInterpolatedStringHandler2.AppendLiteral(" 四舍五入为 ");
					defaultInterpolatedStringHandler2.AppendFormatted(num2);
					LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
				}
				else
				{
					num2 = Convert.ToInt32(value);
				}
				val2.SetValue((ParameterValue)new IntegerParameterValue(num2));
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(43, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("SetGlobalParameterValue: Integer 参数 '");
				defaultInterpolatedStringHandler3.AppendFormatted(parameterName);
				defaultInterpolatedStringHandler3.AppendLiteral("' 设置为 ");
				defaultInterpolatedStringHandler3.AppendFormatted(num2);
				LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			else
			{
				if (dataType == SpecTypeId.Boolean.YesNo)
				{
					if (value is bool flag)
					{
						num4 = (flag ? 1 : 0);
					}
					else if (value is int num5)
					{
						num4 = ((num5 != 0) ? 1 : 0);
					}
					else
					{
						if (!(value is double num6) || 1 == 0)
						{
							if (value == null)
							{
								obj3 = null;
							}
							else
							{
								obj3 = value.ToString();
								if (obj3 != null)
								{
									goto IL_033f;
								}
							}
							obj3 = "";
							goto IL_033f;
						}
						num4 = ((num6 != 0.0) ? 1 : 0);
					}
					goto IL_0357;
				}
				if (dataType == SpecTypeId.Length)
				{
					double num7 = Convert.ToDouble(value);
					double num8 = num7 / 304.8;
					val2.SetValue((ParameterValue)new DoubleParameterValue(num8));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(47, 3);
					defaultInterpolatedStringHandler4.AppendLiteral("SetGlobalParameterValue: Length 参数 '");
					defaultInterpolatedStringHandler4.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler4.AppendLiteral("' - ");
					defaultInterpolatedStringHandler4.AppendFormatted(num7);
					defaultInterpolatedStringHandler4.AppendLiteral("mm → ");
					defaultInterpolatedStringHandler4.AppendFormatted(num8, "F4");
					defaultInterpolatedStringHandler4.AppendLiteral("ft");
					LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
				}
				else if (dataType == SpecTypeId.Area)
				{
					double num9 = Convert.ToDouble(value);
					double num10 = num9 * 10.7639;
					val2.SetValue((ParameterValue)new DoubleParameterValue(num10));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(46, 3);
					defaultInterpolatedStringHandler5.AppendLiteral("SetGlobalParameterValue: Area 参数 '");
					defaultInterpolatedStringHandler5.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler5.AppendLiteral("' - ");
					defaultInterpolatedStringHandler5.AppendFormatted(num9);
					defaultInterpolatedStringHandler5.AppendLiteral("m² → ");
					defaultInterpolatedStringHandler5.AppendFormatted(num10, "F4");
					defaultInterpolatedStringHandler5.AppendLiteral("ft²");
					LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
				}
				else if (dataType == SpecTypeId.Volume)
				{
					double num11 = Convert.ToDouble(value);
					double num12 = num11 * 35.3147;
					val2.SetValue((ParameterValue)new DoubleParameterValue(num12));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(48, 3);
					defaultInterpolatedStringHandler6.AppendLiteral("SetGlobalParameterValue: Volume 参数 '");
					defaultInterpolatedStringHandler6.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler6.AppendLiteral("' - ");
					defaultInterpolatedStringHandler6.AppendFormatted(num11);
					defaultInterpolatedStringHandler6.AppendLiteral("m³ → ");
					defaultInterpolatedStringHandler6.AppendFormatted(num12, "F4");
					defaultInterpolatedStringHandler6.AppendLiteral("ft³");
					LogInfo(defaultInterpolatedStringHandler6.ToStringAndClear());
				}
				else if (dataType == SpecTypeId.Angle)
				{
					double num13 = Convert.ToDouble(value);
					double num14 = num13 * Math.PI / 180.0;
					val2.SetValue((ParameterValue)new DoubleParameterValue(num14));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(46, 3);
					defaultInterpolatedStringHandler7.AppendLiteral("SetGlobalParameterValue: Angle 参数 '");
					defaultInterpolatedStringHandler7.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler7.AppendLiteral("' - ");
					defaultInterpolatedStringHandler7.AppendFormatted(num13);
					defaultInterpolatedStringHandler7.AppendLiteral("° → ");
					defaultInterpolatedStringHandler7.AppendFormatted(num14, "F4");
					defaultInterpolatedStringHandler7.AppendLiteral("rad");
					LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
				}
				else
				{
					double num15 = Convert.ToDouble(value);
					val2.SetValue((ParameterValue)new DoubleParameterValue(num15));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(42, 2);
					defaultInterpolatedStringHandler8.AppendLiteral("SetGlobalParameterValue: Number 参数 '");
					defaultInterpolatedStringHandler8.AppendFormatted(parameterName);
					defaultInterpolatedStringHandler8.AppendLiteral("' 设置为 ");
					defaultInterpolatedStringHandler8.AppendFormatted(num15);
					LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
				}
			}
			goto IL_0711;
			IL_033f:
			num4 = (((string)obj3).Equals("true", StringComparison.OrdinalIgnoreCase) ? 1 : 0);
			goto IL_0357;
			IL_0357:
			val2.SetValue((ParameterValue)new IntegerParameterValue(num4));
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(41, 2);
			defaultInterpolatedStringHandler9.AppendLiteral("SetGlobalParameterValue: YesNo 参数 '");
			defaultInterpolatedStringHandler9.AppendFormatted(parameterName);
			defaultInterpolatedStringHandler9.AppendLiteral("' 设置为 ");
			defaultInterpolatedStringHandler9.AppendFormatted(num4);
			LogInfo(defaultInterpolatedStringHandler9.ToStringAndClear());
			goto IL_0711;
			IL_0174:
			val2.SetValue((ParameterValue)new StringParameterValue((string)obj2));
			goto IL_0711;
		}
		catch (Exception ex)
		{
			LogError("SetGlobalParameterValue 失败: " + ex.Message + "\n堆栈跟踪: " + ex.StackTrace);
			return false;
		}
	}

	public bool DeleteGlobalParameter(object document, string parameterName)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteGlobalParameter: document 不是 Document 类型");
				return false;
			}
			GlobalParameter val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(GlobalParameter))).Cast<GlobalParameter>().FirstOrDefault((GlobalParameter p) => ((Element)p).Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			if (val2 == null)
			{
				LogWarning("DeleteGlobalParameter: 找不到参数 '" + parameterName + "'");
				return false;
			}
			val.Delete(((Element)val2).Id);
			LogInfo("DeleteGlobalParameter: 成功删除全局参数 '" + parameterName + "'");
			return true;
		}
		catch (Exception ex)
		{
			LogError("DeleteGlobalParameter 失败: " + ex.Message);
			return false;
		}
	}

	public IEnumerable<object> GetAllProjectParameters(object document)
	{
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllProjectParameters: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			BindingMap parameterBindings = val.ParameterBindings;
			DefinitionBindingMapIterator val2 = ((DefinitionBindingMap)parameterBindings).ForwardIterator();
			while (val2.MoveNext())
			{
				Definition key = val2.Key;
				Binding val3 = ((DefinitionBindingMap)parameterBindings).get_Item(key);
				if (key == null || val3 == null || key is ExternalDefinition)
				{
					continue;
				}
				List<string> list2 = new List<string>();
				string gparam_ = null;
				string text = null;
				string text2 = null;
				try
				{
					ForgeTypeId dataType = key.GetDataType();
					text = GetDataTypeName(dataType);
					ForgeTypeId groupTypeId = key.GetGroupTypeId();
					text2 = GetGroupTypeName(groupTypeId);
				}
				catch
				{
					text = "Text";
					text2 = "PG_DATA";
				}
				try
				{
					gparam_ = ((val3 is InstanceBinding) ? "instance" : "type");
					ElementBinding val4 = (ElementBinding)(object)((val3 is ElementBinding) ? val3 : null);
					if (val4 != null)
					{
						foreach (Category category in val4.Categories)
						{
							Category val5 = category;
							list2.Add(val5.Name);
						}
					}
				}
				catch
				{
				}
				list.Add(new Class319<string, string, string[], int, string, string>(key.Name, text ?? "Text", list2.ToArray(), list2.Count, gparam_, text2));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetAllProjectParameters: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个项目参数");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllProjectParameters 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetProjectParameter(object document, string parameterName)
	{
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetProjectParameter: document 不是 Document 类型");
				return null;
			}
			BindingMap parameterBindings = val.ParameterBindings;
			DefinitionBindingMapIterator val2 = ((DefinitionBindingMap)parameterBindings).ForwardIterator();
			Definition key;
			Binding val3;
			do
			{
				if (val2.MoveNext())
				{
					key = val2.Key;
					val3 = ((DefinitionBindingMap)parameterBindings).get_Item(key);
					continue;
				}
				return null;
			}
			while (key == null || val3 == null || key is ExternalDefinition || !key.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
			List<string> list = new List<string>();
			string gparam_ = null;
			string text = null;
			string text2 = null;
			try
			{
				ForgeTypeId dataType = key.GetDataType();
				text = GetDataTypeName(dataType);
				ForgeTypeId groupTypeId = key.GetGroupTypeId();
				text2 = GetGroupTypeName(groupTypeId);
			}
			catch
			{
				text = "Text";
				text2 = "PG_DATA";
			}
			try
			{
				gparam_ = ((val3 is InstanceBinding) ? "instance" : "type");
				ElementBinding val4 = (ElementBinding)(object)((val3 is ElementBinding) ? val3 : null);
				if (val4 != null)
				{
					foreach (Category category in val4.Categories)
					{
						Category val5 = category;
						list.Add(val5.Name);
					}
				}
			}
			catch
			{
			}
			return new Class319<string, string, string[], int, string, string>(key.Name, text ?? "Text", list.ToArray(), list.Count, gparam_, text2);
		}
		catch (Exception ex)
		{
			LogError("GetProjectParameter 失败: " + ex.Message);
			return null;
		}
	}

	public bool DeleteProjectParameter(object document, string parameterName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteProjectParameter: document 不是 Document 类型");
				return false;
			}
			BindingMap parameterBindings = val.ParameterBindings;
			DefinitionBindingMapIterator val2 = ((DefinitionBindingMap)parameterBindings).ForwardIterator();
			Definition val3 = null;
			while (val2.MoveNext())
			{
				Definition key = val2.Key;
				if (key != null && !(key is ExternalDefinition) && key.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
				{
					val3 = key;
					break;
				}
			}
			if (val3 == null)
			{
				LogWarning("DeleteProjectParameter: 找不到参数 '" + parameterName + "'");
				return false;
			}
			parameterBindings.Remove(val3);
			LogInfo("DeleteProjectParameter: 成功删除项目参数 '" + parameterName + "'");
			return true;
		}
		catch (Exception ex)
		{
			LogError("DeleteProjectParameter 失败: " + ex.Message);
			return false;
		}
	}

	public object? CreateProjectParameterBinding(object document, string sharedParameterName, string[] categoryNames, string bindingType, string? parameterGroup = null)
	{
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document doc = (Document)((document is Document) ? document : null);
			if (doc == null)
			{
				LogError("CreateProjectParameterBinding: document 不是 Document 类型");
				return new Class315<bool, string>(gparam_2: false, "document 不是 Document 类型");
			}
			SharedParameterElement val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(SharedParameterElement))).Cast<SharedParameterElement>().FirstOrDefault((SharedParameterElement p) => ((Element)p).Name.Equals(sharedParameterName, StringComparison.OrdinalIgnoreCase));
			if (val == null)
			{
				return new Class315<bool, string>(gparam_2: false, "找不到共享参数 '" + sharedParameterName + "'。请先使用 manage_shared_parameters 创建共享参数。");
			}
			InternalDefinition definition = ((ParameterElement)val).GetDefinition();
			BindingMap parameterBindings = doc.ParameterBindings;
			if (((DefinitionBindingMap)parameterBindings).Contains((Definition)(object)definition))
			{
				return new Class315<bool, string>(gparam_2: false, "参数 '" + sharedParameterName + "' 已经存在绑定");
			}
			CategorySet val2 = doc.Application.Create.NewCategorySet();
			Application application = doc.Application;
			foreach (string categoryName in categoryNames)
			{
				Category val3 = ((IEnumerable)doc.Settings.Categories).Cast<Category>().FirstOrDefault((Category c) => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
				if (val3 != null)
				{
					val2.Insert(val3);
					LogInfo("CreateProjectParameterBinding: 添加类别 '" + categoryName + "'");
				}
				else
				{
					LogWarning("CreateProjectParameterBinding: 找不到类别 '" + categoryName + "'");
				}
			}
			if (val2.Size == 0)
			{
				return new Class315<bool, string>(gparam_2: false, "没有有效的类别可以绑定");
			}
			Binding val4 = (Binding)((!bindingType.Equals("type", StringComparison.OrdinalIgnoreCase)) ? ((object)application.Create.NewInstanceBinding(val2)) : ((object)application.Create.NewTypeBinding(val2)));
			ForgeTypeId val5 = GroupTypeId.Data;
			if (!string.IsNullOrEmpty(parameterGroup))
			{
				ForgeTypeId groupTypeIdByName = GetGroupTypeIdByName(parameterGroup);
				if (groupTypeIdByName != (ForgeTypeId)null)
				{
					val5 = groupTypeIdByName;
				}
			}
			if (parameterBindings.Insert((Definition)(object)definition, val4, val5))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 2);
				defaultInterpolatedStringHandler.AppendLiteral("CreateProjectParameterBinding: 成功创建项目参数绑定 '");
				defaultInterpolatedStringHandler.AppendFormatted(sharedParameterName);
				defaultInterpolatedStringHandler.AppendLiteral("'，绑定到 ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.Size);
				defaultInterpolatedStringHandler.AppendLiteral(" 个类别");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				string[] gparam_ = categoryNames.Where((string c) => ((IEnumerable)doc.Settings.Categories).Cast<Category>().Any((Category cat) => cat.Name.Equals(c, StringComparison.OrdinalIgnoreCase))).ToArray();
				return new Class320<bool, string, string, int, string[], string>(gparam_6: true, sharedParameterName, bindingType, val2.Size, gparam_, parameterGroup ?? "PG_DATA");
			}
			return new Class315<bool, string>(gparam_2: false, "插入绑定失败");
		}
		catch (Exception ex)
		{
			LogError("CreateProjectParameterBinding 失败: " + ex.Message);
			return new Class315<bool, string>(gparam_2: false, ex.Message);
		}
	}

	public object? UpdateProjectParameterBinding(object document, string parameterName, string[] categoriesToAdd, string[] categoriesToRemove)
	{
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Expected O, but got Unknown
		//IL_01cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d2: Expected O, but got Unknown
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Expected O, but got Unknown
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Expected O, but got Unknown
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("UpdateProjectParameterBinding: document 不是 Document 类型");
				return new Class315<bool, string>(gparam_2: false, "document 不是 Document 类型");
			}
			BindingMap parameterBindings = val.ParameterBindings;
			Definition val2 = null;
			Binding val3 = null;
			DefinitionBindingMapIterator val4 = ((DefinitionBindingMap)parameterBindings).ForwardIterator();
			while (val4.MoveNext())
			{
				Definition key = val4.Key;
				if (key != null && key.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
				{
					val2 = key;
					object current = val4.Current;
					val3 = (Binding)((current is Binding) ? current : null);
					break;
				}
			}
			if (val2 == null)
			{
				return new Class315<bool, string>(gparam_2: false, "找不到参数 '" + parameterName + "' 的绑定");
			}
			if (val3 == null)
			{
				return new Class315<bool, string>(gparam_2: false, "参数 '" + parameterName + "' 没有绑定");
			}
			CategorySet val5 = val.Application.Create.NewCategorySet();
			List<string> list = new List<string>();
			InstanceBinding val6 = (InstanceBinding)(object)((val3 is InstanceBinding) ? val3 : null);
			Binding val8;
			if (val6 != null)
			{
				foreach (Category category2 in ((ElementBinding)val6).Categories)
				{
					Category val7 = category2;
					if (!categoriesToRemove.Contains<string>(val7.Name, StringComparer.OrdinalIgnoreCase))
					{
						val5.Insert(val7);
					}
					else
					{
						list.Add("移除: " + val7.Name);
					}
				}
				val8 = (Binding)new InstanceBinding(val5);
			}
			else
			{
				TypeBinding val9 = (TypeBinding)(object)((val3 is TypeBinding) ? val3 : null);
				if (val9 == null)
				{
					return new Class315<bool, string>(gparam_2: false, "不支持的绑定类型");
				}
				foreach (Category category3 in ((ElementBinding)val9).Categories)
				{
					Category val10 = category3;
					if (!categoriesToRemove.Contains<string>(val10.Name, StringComparer.OrdinalIgnoreCase))
					{
						val5.Insert(val10);
					}
					else
					{
						list.Add("移除: " + val10.Name);
					}
				}
				val8 = (Binding)new TypeBinding(val5);
			}
			foreach (string categoryName in categoriesToAdd)
			{
				Category category = ((IEnumerable)val.Settings.Categories).Cast<Category>().FirstOrDefault((Category c) => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
				if (category != null)
				{
					bool flag = false;
					if (val3 is InstanceBinding)
					{
						flag = ((IEnumerable)((ElementBinding)(InstanceBinding)val3).Categories).Cast<Category>().Any((Category c) => c.Id == category.Id);
					}
					else if (val3 is TypeBinding)
					{
						flag = ((IEnumerable)((ElementBinding)(TypeBinding)val3).Categories).Cast<Category>().Any((Category c) => c.Id == category.Id);
					}
					if (!flag)
					{
						val5.Insert(category);
						list.Add("添加: " + categoryName);
						LogInfo("UpdateProjectParameterBinding: 添加类别 '" + categoryName + "'");
					}
				}
				else
				{
					LogWarning("UpdateProjectParameterBinding: 找不到类别 '" + categoryName + "'");
				}
			}
			val2.GetGroupTypeId();
			if (val5.Size == 0 && categoriesToRemove.Length != 0)
			{
				return new Class321<bool, string, List<string>>(gparam_3: false, "不能移除所有绑定的类别。如需删除参数，请使用 delete 操作。", list);
			}
			try
			{
				((DefinitionBindingMap)parameterBindings).Insert(val2, val8);
				LogInfo("UpdateProjectParameterBinding: 成功更新参数 '" + parameterName + "' 的绑定");
				int size = val5.Size;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("已更新 ");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个类别绑定");
				return new Class322<bool, string, int, List<string>, string>(gparam_5: true, parameterName, size, list, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex)
			{
				LogError("UpdateProjectParameterBinding: 更新绑定时出错 - " + ex.Message);
				return new Class315<bool, string>(gparam_2: false, "更新绑定失败: " + ex.Message);
			}
		}
		catch (Exception ex2)
		{
			LogError("UpdateProjectParameterBinding 失败: " + ex2.Message);
			return new Class315<bool, string>(gparam_2: false, ex2.Message);
		}
	}

	public IEnumerable<object> GetAllParameterGroups(object application)
	{
		List<object> list = new List<object>();
		try
		{
			Application val = (Application)((application is Application) ? application : null);
			if (val == null)
			{
				LogError("GetAllParameterGroups: application 不是 Application 类型");
				return list;
			}
			List<(string, string, string)> list2 = new List<(string, string, string)>
			{
				("group_key_data", "数据", "Data"),
				("group_key_text", "文字", "Text"),
				("group_key_length", "尺寸", "Length"),
				("group_key_structural", "结构", "Structural Analysis"),
				("group_key_electrical", "电气", "Electrical"),
				("group_key_mechanical", "机械", "Mechanical"),
				("group_key_piping", "管道", "Piping"),
				("group_key_energy", "能量", "Energy"),
				("group_key_plumbing", "卫浴", "Plumbing"),
				("group_key_area", "面积", "Area"),
				("group_key_cost", "价格", "Cost"),
				("group_key_project", "项目信息", "Project Information"),
				("group_key_materials", "材质", "Materials"),
				("group_key_graphics", "图形", "Graphics"),
				("group_key_general", "共享参数", "General")
			};
			foreach (var (gparam_, gparam_2, text) in list2)
			{
				list.Add(new Class323<string, string, string, string, string>(gparam_, text, gparam_2, text, text));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetAllParameterGroups: 成功获取 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个参数组");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			LogError("GetAllParameterGroups 失败: " + ex.Message);
		}
		return list;
	}

	private string GetDataTypeName(ForgeTypeId dataType)
	{
		if (dataType == (ForgeTypeId)null)
		{
			return "Text";
		}
		if (dataType == SpecTypeId.Length)
		{
			return "Length";
		}
		if (dataType == SpecTypeId.Area)
		{
			return "Area";
		}
		if (dataType == SpecTypeId.Volume)
		{
			return "Volume";
		}
		if (dataType == SpecTypeId.Angle)
		{
			return "Angle";
		}
		if (dataType == SpecTypeId.String.Text)
		{
			return "Text";
		}
		if (dataType == SpecTypeId.Int.Integer)
		{
			return "Integer";
		}
		if (dataType == SpecTypeId.Number)
		{
			return "Number";
		}
		if (dataType == SpecTypeId.Boolean.YesNo)
		{
			return "YesNo";
		}
		if (dataType == SpecTypeId.Reference.Material)
		{
			return "Material";
		}
		try
		{
			return LabelUtils.GetLabelForSpec(dataType) ?? dataType.TypeId;
		}
		catch
		{
			return dataType.TypeId;
		}
	}

	private string GetGroupTypeName(ForgeTypeId groupTypeId)
	{
		if (groupTypeId == (ForgeTypeId)null)
		{
			return "PG_DATA";
		}
		if (groupTypeId == GroupTypeId.Data)
		{
			return "PG_DATA";
		}
		if (groupTypeId == GroupTypeId.Text)
		{
			return "PG_TEXT";
		}
		if (groupTypeId == GroupTypeId.Graphics)
		{
			return "PG_GRAPHICS";
		}
		try
		{
			return LabelUtils.GetLabelForSpec(groupTypeId) ?? groupTypeId.TypeId;
		}
		catch
		{
			return groupTypeId.TypeId;
		}
	}

	private ForgeTypeId? GetGroupTypeIdByName(string groupName)
	{
		if (string.IsNullOrEmpty(groupName))
		{
			return null;
		}
		string text = groupName.ToUpperInvariant();
		if (text.Contains("数据") || text == "DATA" || text == "PG_DATA")
		{
			return GroupTypeId.Data;
		}
		if (text.Contains("文字") || text == "TEXT" || text == "PG_TEXT")
		{
			return GroupTypeId.Text;
		}
		if (text.Contains("图形") || text == "GRAPHICS" || text == "PG_GRAPHICS")
		{
			return GroupTypeId.Graphics;
		}
		if (text.Contains("尺寸") || text == "DIMENSIONS" || text == "DIMENSION" || text.Contains("标注"))
		{
			return GroupTypeId.Geometry;
		}
		if (text.Contains("结构") || text == "STRUCTURAL" || text == "STRUCTURAL_ANALYSIS")
		{
			return GroupTypeId.StructuralAnalysis;
		}
		if (text.Contains("电气") || text == "ELECTRICAL")
		{
			return GroupTypeId.Electrical;
		}
		if (text.Contains("机械") || text == "MECHANICAL")
		{
			return GroupTypeId.Mechanical;
		}
		if (text.Contains("管道") || text == "PIPING")
		{
			return GroupTypeId.Plumbing;
		}
		if (text.Contains("能量") || text == "ENERGY")
		{
			return GroupTypeId.EnergyAnalysis;
		}
		if (text.Contains("卫浴") || text == "PLUMBING")
		{
			return GroupTypeId.Plumbing;
		}
		if (text.Contains("面积") || text == "AREA")
		{
			return GroupTypeId.Area;
		}
		if (text.Contains("价格") || text == "COST")
		{
			return GroupTypeId.IdentityData;
		}
		if (text.Contains("项目") || text.Contains("PROJECT") || text == "PROJECT_INFO")
		{
			return GroupTypeId.IdentityData;
		}
		if (text.Contains("材质") || text.Contains("材料") || text == "MATERIALS")
		{
			return GroupTypeId.Materials;
		}
		if (text.Contains("约束") || text == "CONSTRAINTS")
		{
			return GroupTypeId.Constraints;
		}
		if (text.Contains("分析") || text == "ANALYSIS_RESULTS")
		{
			return GroupTypeId.AnalysisResults;
		}
		if (text.Contains("荷载") || text.Contains("载荷") || text == "LOADS")
		{
			return GroupTypeId.MechanicalLoads;
		}
		if (text.Contains("阶段") || text == "PHASING")
		{
			return GroupTypeId.Phasing;
		}
		if (text == "URL" || text.Contains("链接"))
		{
			return GroupTypeId.IdentityData;
		}
		LogWarning("GetGroupTypeIdByName: 参数组 '" + groupName + "' 未明确映射，使用默认数据组");
		return GroupTypeId.Data;
	}

	public IEnumerable<InsulationTypeInfo> GetPipeInsulationTypes(object document)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetPipeInsulationTypes: document 不是 Document 类型");
				return Enumerable.Empty<InsulationTypeInfo>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(PipeInsulationType)).WhereElementIsElementType();
			return from x in ((IEnumerable<Element>)val2.ToElements()).Select((Func<Element, InsulationTypeInfo>)delegate(Element e)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Expected O, but got Unknown
					return new InsulationTypeInfo
					{
						Id = GetElementIdValue(e.Id),
						Name = (e.Name ?? "未命名")
					};
				})
				orderby x.Name
				select x;
		}
		catch (Exception ex)
		{
			LogError("GetPipeInsulationTypes 失败: " + ex.Message);
			return Enumerable.Empty<InsulationTypeInfo>();
		}
	}

	public IEnumerable<InsulationTypeInfo> GetDuctInsulationTypes(object document)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetDuctInsulationTypes: document 不是 Document 类型");
				return Enumerable.Empty<InsulationTypeInfo>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(DuctInsulationType)).WhereElementIsElementType();
			return from x in ((IEnumerable<Element>)val2.ToElements()).Select((Func<Element, InsulationTypeInfo>)delegate(Element e)
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0017: Unknown result type (might be due to invalid IL or missing references)
					//IL_0032: Expected O, but got Unknown
					return new InsulationTypeInfo
					{
						Id = GetElementIdValue(e.Id),
						Name = (e.Name ?? "未命名")
					};
				})
				orderby x.Name
				select x;
		}
		catch (Exception ex)
		{
			LogError("GetDuctInsulationTypes 失败: " + ex.Message);
			return Enumerable.Empty<InsulationTypeInfo>();
		}
	}

	public IEnumerable<InsulationSystemInfo> GetPipeInsulationSystemInfos(object document, int? systemTypeId = null)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document doc = (Document)((document is Document) ? document : null);
			if (doc == null)
			{
				LogError("GetPipeInsulationSystemInfos: document 不是 Document 类型");
				return Enumerable.Empty<InsulationSystemInfo>();
			}
			Dictionary<int, string> systemTypeMap = GetPipingSystemTypes(document).ToDictionary(delegate(object t)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return GetElementIdValue(((Element)t).Id);
			}, delegate(object t)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((Element)t).Name ?? "未命名系统";
			});
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(Pipe)).WhereElementIsNotElementType();
			return from x in (from p in val.ToElements()
					select new
					{
						Pipe = p,
						SystemTypeId = GetSystemTypeId(doc, p.Id)
					} into x
					where !systemTypeId.HasValue || x.SystemTypeId == systemTypeId.Value
					group x by x.SystemTypeId).Select(g =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Expected O, but got Unknown
					string value;
					return new InsulationSystemInfo
					{
						SystemTypeId = g.Key,
						SystemTypeName = (systemTypeMap.TryGetValue(g.Key, out value) ? value : "未知系统"),
						ElementCount = g.Count(),
						InsulatedCount = g.Count(x => HasPipeInsulation(doc, x.Pipe.Id))
					};
				})
				orderby x.SystemTypeName
				select x;
		}
		catch (Exception ex)
		{
			LogError("GetPipeInsulationSystemInfos 失败: " + ex.Message);
			return Enumerable.Empty<InsulationSystemInfo>();
		}
	}

	public IEnumerable<InsulationSystemInfo> GetDuctInsulationSystemInfos(object document, int? systemTypeId = null)
	{
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document doc = (Document)((document is Document) ? document : null);
			if (doc == null)
			{
				LogError("GetDuctInsulationSystemInfos: document 不是 Document 类型");
				return Enumerable.Empty<InsulationSystemInfo>();
			}
			Dictionary<int, string> systemTypeMap = GetDuctSystemTypes(document).ToDictionary(delegate(object t)
			{
				//IL_0007: Unknown result type (might be due to invalid IL or missing references)
				return GetElementIdValue(((Element)t).Id);
			}, delegate(object t)
			{
				//IL_0001: Unknown result type (might be due to invalid IL or missing references)
				return ((Element)t).Name ?? "未命名系统";
			});
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(Duct)).WhereElementIsNotElementType();
			return from x in (from d in val.ToElements()
					select new
					{
						Duct = d,
						SystemTypeId = GetSystemTypeId(doc, d.Id)
					} into x
					where !systemTypeId.HasValue || x.SystemTypeId == systemTypeId.Value
					group x by x.SystemTypeId).Select(g =>
				{
					//IL_0000: Unknown result type (might be due to invalid IL or missing references)
					//IL_0005: Unknown result type (might be due to invalid IL or missing references)
					//IL_0011: Unknown result type (might be due to invalid IL or missing references)
					//IL_0039: Unknown result type (might be due to invalid IL or missing references)
					//IL_0045: Unknown result type (might be due to invalid IL or missing references)
					//IL_0071: Expected O, but got Unknown
					string value;
					return new InsulationSystemInfo
					{
						SystemTypeId = g.Key,
						SystemTypeName = (systemTypeMap.TryGetValue(g.Key, out value) ? value : "未知系统"),
						ElementCount = g.Count(),
						InsulatedCount = g.Count(x => HasDuctInsulation(doc, x.Duct.Id))
					};
				})
				orderby x.SystemTypeName
				select x;
		}
		catch (Exception ex)
		{
			LogError("GetDuctInsulationSystemInfos 失败: " + ex.Message);
			return Enumerable.Empty<InsulationSystemInfo>();
		}
	}

	public InsulationOperationResult? AddPipeInsulation(object document, int? systemTypeId, int insulationTypeId, double thicknessMM, IList<int>? elementIds = null, bool overrideExisting = true)
	{
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("AddPipeInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetPipeElements = GetTargetPipeElements(val, systemTypeId, elementIds);
			if (!targetPipeElements.Any())
			{
				LogWarning("[AddPipeInsulation] 未找到目标水管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)0,
					SystemTypeName = systemTypeName
				};
			}
			double num = MillimetersToFeet(thicknessMM);
			ElementId val2 = new ElementId((long)insulationTypeId);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			List<string> list = new List<string>();
			Transaction val3 = new Transaction(val, "RevitAi_添加水管保温");
			try
			{
				val3.Start();
				foreach (ElementId item in targetPipeElements)
				{
					try
					{
						IList<ElementId> pipeInsulationIds = GetPipeInsulationIds(val, item);
						if (pipeInsulationIds != null && pipeInsulationIds.Count > 0)
						{
							if (overrideExisting)
							{
								foreach (ElementId item2 in pipeInsulationIds)
								{
									Element element2 = val.GetElement(item2);
									PipeInsulation val4 = (PipeInsulation)(object)((element2 is PipeInsulation) ? element2 : null);
									if (val4 != null)
									{
										((Element)val4).ChangeTypeId(val2);
										((InsulationLiningBase)val4).Thickness = num;
									}
								}
								num3++;
							}
							else
							{
								num4++;
							}
						}
						else
						{
							PipeInsulation.Create(val, item, val2, num);
							num2++;
						}
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val3.Commit();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(47, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[AddPipeInsulation] 水管保温添加完成 - 新增: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num2);
			defaultInterpolatedStringHandler2.AppendLiteral(", 修改: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num3);
			defaultInterpolatedStringHandler2.AppendLiteral(", 跳过: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num4);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Added = num2,
				Modified = num3,
				Skipped = num4,
				Errors = list,
				TargetType = (InsulationTargetType)0,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("AddPipeInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	public InsulationOperationResult? AddDuctInsulation(object document, int? systemTypeId, int insulationTypeId, double thicknessMM, IList<int>? elementIds = null, bool overrideExisting = true)
	{
		//IL_0215: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("AddDuctInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetDuctElements = GetTargetDuctElements(val, systemTypeId, elementIds);
			if (!targetDuctElements.Any())
			{
				LogWarning("[AddDuctInsulation] 未找到目标风管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)1,
					SystemTypeName = systemTypeName
				};
			}
			double num = MillimetersToFeet(thicknessMM);
			ElementId val2 = new ElementId((long)insulationTypeId);
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			List<string> list = new List<string>();
			Transaction val3 = new Transaction(val, "RevitAi_添加风管保温");
			try
			{
				val3.Start();
				foreach (ElementId item in targetDuctElements)
				{
					try
					{
						IList<ElementId> ductInsulationIds = GetDuctInsulationIds(val, item);
						if (ductInsulationIds != null && ductInsulationIds.Count > 0)
						{
							if (overrideExisting)
							{
								foreach (ElementId item2 in ductInsulationIds)
								{
									Element element2 = val.GetElement(item2);
									DuctInsulation val4 = (DuctInsulation)(object)((element2 is DuctInsulation) ? element2 : null);
									if (val4 != null)
									{
										((Element)val4).ChangeTypeId(val2);
										((InsulationLiningBase)val4).Thickness = num;
									}
								}
								num3++;
							}
							else
							{
								num4++;
							}
						}
						else
						{
							DuctInsulation.Create(val, item, val2, num);
							num2++;
						}
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val3.Commit();
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(47, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[AddDuctInsulation] 风管保温添加完成 - 新增: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num2);
			defaultInterpolatedStringHandler2.AppendLiteral(", 修改: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num3);
			defaultInterpolatedStringHandler2.AppendLiteral(", 跳过: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num4);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Added = num2,
				Modified = num3,
				Skipped = num4,
				Errors = list,
				TargetType = (InsulationTargetType)1,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("AddDuctInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	public InsulationOperationResult? RemovePipeInsulation(object document, int? systemTypeId = null, IList<int>? elementIds = null)
	{
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RemovePipeInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetPipeElements = GetTargetPipeElements(val, systemTypeId, elementIds);
			if (!targetPipeElements.Any())
			{
				LogWarning("[RemovePipeInsulation] 未找到目标水管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)0,
					SystemTypeName = systemTypeName
				};
			}
			int num = 0;
			List<string> list = new List<string>();
			Transaction val2 = new Transaction(val, "RevitAi_删除水管保温");
			try
			{
				val2.Start();
				foreach (ElementId item in targetPipeElements)
				{
					try
					{
						IList<ElementId> pipeInsulationIds = GetPipeInsulationIds(val, item);
						if (pipeInsulationIds == null || pipeInsulationIds.Count <= 0)
						{
							continue;
						}
						foreach (ElementId item2 in pipeInsulationIds)
						{
							val.Delete(item2);
						}
						num++;
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[RemovePipeInsulation] 水管保温删除完成 - 删除: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Deleted = num,
				Errors = list,
				TargetType = (InsulationTargetType)0,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("RemovePipeInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	public InsulationOperationResult? RemoveDuctInsulation(object document, int? systemTypeId = null, IList<int>? elementIds = null)
	{
		//IL_01ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0212: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0229: Expected O, but got Unknown
		//IL_00c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RemoveDuctInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetDuctElements = GetTargetDuctElements(val, systemTypeId, elementIds);
			if (!targetDuctElements.Any())
			{
				LogWarning("[RemoveDuctInsulation] 未找到目标风管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)1,
					SystemTypeName = systemTypeName
				};
			}
			int num = 0;
			List<string> list = new List<string>();
			Transaction val2 = new Transaction(val, "RevitAi_删除风管保温");
			try
			{
				val2.Start();
				foreach (ElementId item in targetDuctElements)
				{
					try
					{
						IList<ElementId> ductInsulationIds = GetDuctInsulationIds(val, item);
						if (ductInsulationIds == null || ductInsulationIds.Count <= 0)
						{
							continue;
						}
						foreach (ElementId item2 in ductInsulationIds)
						{
							val.Delete(item2);
						}
						num++;
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[RemoveDuctInsulation] 风管保温删除完成 - 删除: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Deleted = num,
				Errors = list,
				TargetType = (InsulationTargetType)1,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("RemoveDuctInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	public InsulationOperationResult? ModifyPipeInsulation(object document, int? systemTypeId = null, int? newInsulationTypeId = null, double? newThicknessMM = null, IList<int>? elementIds = null)
	{
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ModifyPipeInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetPipeElements = GetTargetPipeElements(val, systemTypeId, elementIds);
			if (!targetPipeElements.Any())
			{
				LogWarning("[ModifyPipeInsulation] 未找到目标水管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)0,
					SystemTypeName = systemTypeName
				};
			}
			int num = 0;
			int num2 = 0;
			List<string> list = new List<string>();
			Transaction val2 = new Transaction(val, "RevitAi_修改水管保温");
			try
			{
				val2.Start();
				foreach (ElementId item in targetPipeElements)
				{
					try
					{
						IList<ElementId> pipeInsulationIds = GetPipeInsulationIds(val, item);
						if (pipeInsulationIds != null && pipeInsulationIds.Count > 0)
						{
							foreach (ElementId item2 in pipeInsulationIds)
							{
								Element element2 = val.GetElement(item2);
								PipeInsulation val3 = (PipeInsulation)(object)((element2 is PipeInsulation) ? element2 : null);
								if (val3 != null)
								{
									if (newInsulationTypeId.HasValue)
									{
										((Element)val3).ChangeTypeId(new ElementId((long)newInsulationTypeId.Value));
									}
									if (newThicknessMM.HasValue)
									{
										((InsulationLiningBase)val3).Thickness = MillimetersToFeet(newThicknessMM.Value);
									}
								}
							}
							num++;
						}
						else
						{
							num2++;
						}
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[ModifyPipeInsulation] 水管保温修改完成 - 修改: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(", 跳过: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num2);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Modified = num,
				Skipped = num2,
				Errors = list,
				TargetType = (InsulationTargetType)0,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("ModifyPipeInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	public InsulationOperationResult? ModifyDuctInsulation(object document, int? systemTypeId = null, int? newInsulationTypeId = null, double? newThicknessMM = null, IList<int>? elementIds = null)
	{
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_026f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0274: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Expected O, but got Unknown
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Expected O, but got Unknown
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ModifyDuctInsulation: document 不是 Document 类型");
				return null;
			}
			object obj;
			if (!systemTypeId.HasValue)
			{
				obj = "";
			}
			else
			{
				Element element = val.GetElement(NewElementId(systemTypeId.Value));
				if (element == null)
				{
					obj = null;
				}
				else
				{
					obj = element.Name;
					if (obj != null)
					{
						goto IL_0069;
					}
				}
				obj = "";
			}
			goto IL_0069;
			IL_0069:
			string systemTypeName = (string)obj;
			IList<ElementId> targetDuctElements = GetTargetDuctElements(val, systemTypeId, elementIds);
			if (!targetDuctElements.Any())
			{
				LogWarning("[ModifyDuctInsulation] 未找到目标风管");
				return new InsulationOperationResult
				{
					Success = true,
					TargetType = (InsulationTargetType)1,
					SystemTypeName = systemTypeName
				};
			}
			int num = 0;
			int num2 = 0;
			List<string> list = new List<string>();
			Transaction val2 = new Transaction(val, "RevitAi_修改风管保温");
			try
			{
				val2.Start();
				foreach (ElementId item in targetDuctElements)
				{
					try
					{
						IList<ElementId> ductInsulationIds = GetDuctInsulationIds(val, item);
						if (ductInsulationIds != null && ductInsulationIds.Count > 0)
						{
							foreach (ElementId item2 in ductInsulationIds)
							{
								Element element2 = val.GetElement(item2);
								DuctInsulation val3 = (DuctInsulation)(object)((element2 is DuctInsulation) ? element2 : null);
								if (val3 != null)
								{
									if (newInsulationTypeId.HasValue)
									{
										((Element)val3).ChangeTypeId(new ElementId((long)newInsulationTypeId.Value));
									}
									if (newThicknessMM.HasValue)
									{
										((InsulationLiningBase)val3).Thickness = MillimetersToFeet(newThicknessMM.Value);
									}
								}
							}
							num++;
						}
						else
						{
							num2++;
						}
					}
					catch (Exception ex)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler.AppendLiteral("元素 ");
						defaultInterpolatedStringHandler.AppendFormatted(GetElementIdValue(item));
						defaultInterpolatedStringHandler.AppendLiteral(": ");
						defaultInterpolatedStringHandler.AppendFormatted(ex.Message);
						list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
					}
				}
				val2.Commit();
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[ModifyDuctInsulation] 风管保温修改完成 - 修改: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num);
			defaultInterpolatedStringHandler2.AppendLiteral(", 跳过: ");
			defaultInterpolatedStringHandler2.AppendFormatted(num2);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return new InsulationOperationResult
			{
				Success = true,
				Modified = num,
				Skipped = num2,
				Errors = list,
				TargetType = (InsulationTargetType)1,
				SystemTypeName = systemTypeName
			};
		}
		catch (Exception ex2)
		{
			LogError("ModifyDuctInsulation 失败: " + ex2.Message);
			return null;
		}
	}

	private double MillimetersToFeet(double mm)
	{
		return mm / 304.8;
	}

	private IList<ElementId> GetPipeInsulationIds(Document doc, ElementId pipeId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		List<ElementId> list = new List<ElementId>();
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation)).WhereElementIsNotElementType();
			foreach (PipeInsulation item in val)
			{
				PipeInsulation val2 = item;
				Parameter val3 = ((IEnumerable)((Element)val2).Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("Host") || p.Definition.Name.Contains("主") || p.Definition.Name.Contains("关联"));
				if (val3 != null && val3.HasValue)
				{
					ElementId val4 = new ElementId(val3.AsElementId().Value);
					if (val4 == pipeId)
					{
						list.Add(((Element)val2).Id);
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private IList<ElementId> GetDuctInsulationIds(Document doc, ElementId ductId)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Expected O, but got Unknown
		List<ElementId> list = new List<ElementId>();
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation)).WhereElementIsNotElementType();
			foreach (DuctInsulation item in val)
			{
				DuctInsulation val2 = item;
				Parameter val3 = ((IEnumerable)((Element)val2).Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("Host") || p.Definition.Name.Contains("主") || p.Definition.Name.Contains("关联"));
				if (val3 != null && val3.HasValue)
				{
					ElementId val4 = new ElementId(val3.AsElementId().Value);
					if (val4 == ductId)
					{
						list.Add(((Element)val2).Id);
					}
				}
			}
		}
		catch
		{
		}
		return list;
	}

	private IList<ElementId> GetTargetPipeElements(Document doc, int? systemTypeId, IList<int>? elementIds)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (elementIds != null && elementIds.Count > 0)
		{
			return (from id in elementIds
				select NewElementId(id) into id
				where doc.GetElement(id) is Pipe
				select id).ToList();
		}
		if (systemTypeId.HasValue)
		{
			return (from e in (IEnumerable<Element>)new FilteredElementCollector(doc).OfClass(typeof(Pipe)).WhereElementIsNotElementType()
				where GetSystemTypeId(doc, e.Id) == systemTypeId.Value
				select e.Id).ToList();
		}
		return new FilteredElementCollector(doc).OfClass(typeof(Pipe)).WhereElementIsNotElementType().ToElementIds()
			.ToList();
	}

	private IList<ElementId> GetTargetDuctElements(Document doc, int? systemTypeId, IList<int>? elementIds)
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		if (elementIds != null && elementIds.Count > 0)
		{
			return (from id in elementIds
				select NewElementId(id) into id
				where doc.GetElement(id) is Duct
				select id).ToList();
		}
		if (systemTypeId.HasValue)
		{
			return (from e in (IEnumerable<Element>)new FilteredElementCollector(doc).OfClass(typeof(Duct)).WhereElementIsNotElementType()
				where GetSystemTypeId(doc, e.Id) == systemTypeId.Value
				select e.Id).ToList();
		}
		return new FilteredElementCollector(doc).OfClass(typeof(Duct)).WhereElementIsNotElementType().ToElementIds()
			.ToList();
	}

	private int GetSystemTypeId(Document doc, ElementId elementId)
	{
		try
		{
			Element element = doc.GetElement(elementId);
			if (element == null)
			{
				return -1;
			}
			Parameter val = ((IEnumerable)element.Parameters).OfType<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("System Type") || p.Definition.Name.Contains("系统类型"));
			if (val != null && val.HasValue)
			{
				return (int)val.AsElementId().Value;
			}
			return -1;
		}
		catch
		{
			return -1;
		}
	}

	private bool HasPipeInsulation(Document doc, ElementId pipeId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(PipeInsulation)).WhereElementIsNotElementType();
			foreach (PipeInsulation item in val)
			{
				PipeInsulation val2 = item;
				Parameter val3 = ((IEnumerable)((Element)val2).Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("Host") || p.Definition.Name.Contains("主") || p.Definition.Name.Contains("关联"));
				if (val3 != null && val3.HasValue)
				{
					ElementId val4 = new ElementId(val3.AsElementId().Value);
					if (val4 == pipeId)
					{
						return true;
					}
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private bool HasDuctInsulation(Document doc, ElementId ductId)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfClass(typeof(DuctInsulation)).WhereElementIsNotElementType();
			foreach (DuctInsulation item in val)
			{
				DuctInsulation val2 = item;
				Parameter val3 = ((IEnumerable)((Element)val2).Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Contains("Host") || p.Definition.Name.Contains("主") || p.Definition.Name.Contains("关联"));
				if (val3 != null && val3.HasValue)
				{
					ElementId val4 = new ElementId(val3.AsElementId().Value);
					if (val4 == ductId)
					{
						return true;
					}
				}
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	private int GetElementIdValue(ElementId id)
	{
		return (int)id.Value;
	}

	private ElementId NewElementId(int value)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		return new ElementId((long)value);
	}
}
