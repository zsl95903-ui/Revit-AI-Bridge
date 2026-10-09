using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

internal sealed class CreationService : ICreationService
{
	private readonly UIApplication _application;

	public CreationService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public object? CreateLevel(object document, double elevation, string name)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateLevel: document 不是 Document 类型");
				return null;
			}
			Level val2 = Level.Create(val, elevation);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
				defaultInterpolatedStringHandler.AppendLiteral("CreateLevel: Level.Create 返回 null (elevation: ");
				defaultInterpolatedStringHandler.AppendFormatted(elevation);
				defaultInterpolatedStringHandler.AppendLiteral(", name: ");
				defaultInterpolatedStringHandler.AppendFormatted(name);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (!string.IsNullOrWhiteSpace(name))
			{
				try
				{
					((Element)val2).Name = name;
				}
				catch (Exception ex)
				{
					LogWarning("CreateLevel: 设置名称失败 - " + ex.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(42, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateLevel: 成功创建标高 '");
			defaultInterpolatedStringHandler2.AppendFormatted(name);
			defaultInterpolatedStringHandler2.AppendLiteral("' (ID: ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)val2).Id.Value);
			defaultInterpolatedStringHandler2.AppendLiteral(", Elevation: ");
			defaultInterpolatedStringHandler2.AppendFormatted(elevation);
			defaultInterpolatedStringHandler2.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val2;
		}
		catch (Exception ex2)
		{
			LogError("CreateLevel 失败: " + ex2.GetType().Name + " - " + ex2.Message);
			return null;
		}
	}

	public object? CreateGrid(object document, object start, object end, string? name = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateGrid: document 不是 Document 类型");
				return null;
			}
			if (!ParsePoint(start, out XYZ xyz) || xyz == null)
			{
				LogError("CreateGrid: 无法解析起点");
				return null;
			}
			if (!ParsePoint(end, out XYZ xyz2) || xyz2 == null)
			{
				LogError("CreateGrid: 无法解析终点");
				return null;
			}
			double num = xyz.DistanceTo(xyz2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 8);
			defaultInterpolatedStringHandler.AppendLiteral("CreateGrid: 起点=(");
			defaultInterpolatedStringHandler.AppendFormatted(xyz.X, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(xyz.Y, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(xyz.Z, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("), 终点=(");
			defaultInterpolatedStringHandler.AppendFormatted(xyz2.X, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(xyz2.Y, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(xyz2.Z, "F4");
			defaultInterpolatedStringHandler.AppendLiteral("), 线段长度=");
			defaultInterpolatedStringHandler.AppendFormatted(num, "F4");
			defaultInterpolatedStringHandler.AppendLiteral(" 英尺, 名称='");
			defaultInterpolatedStringHandler.AppendFormatted(name ?? "自动");
			defaultInterpolatedStringHandler.AppendLiteral("'");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			if (num < 0.0625)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("CreateGrid: 线段长度过短 (");
				defaultInterpolatedStringHandler2.AppendFormatted(num, "F4");
				defaultInterpolatedStringHandler2.AppendLiteral(" 英尺)，最小要求 0.0625 英尺（约 1.6 毫米）");
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			Line val2 = Line.CreateBound(xyz, xyz2);
			Grid val3 = Grid.Create(val, val2);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(55, 5);
				defaultInterpolatedStringHandler3.AppendLiteral("CreateGrid: Grid.Create 返回 null (起点=(");
				defaultInterpolatedStringHandler3.AppendFormatted(xyz.X, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(xyz.Y, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("), 终点=(");
				defaultInterpolatedStringHandler3.AppendFormatted(xyz2.X, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(", ");
				defaultInterpolatedStringHandler3.AppendFormatted(xyz2.Y, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral("), 长度=");
				defaultInterpolatedStringHandler3.AppendFormatted(num, "F4");
				defaultInterpolatedStringHandler3.AppendLiteral(")");
				LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
				return null;
			}
			if (!string.IsNullOrWhiteSpace(name))
			{
				try
				{
					((Element)val3).Name = name;
				}
				catch (Exception ex)
				{
					LogWarning("CreateGrid: 设置名称失败 - " + ex.Message);
				}
			}
			string name2 = ((Element)val3).Name;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(38, 3);
			defaultInterpolatedStringHandler4.AppendLiteral("CreateGrid: 成功创建网格 '");
			defaultInterpolatedStringHandler4.AppendFormatted(name2);
			defaultInterpolatedStringHandler4.AppendLiteral("' (ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val3).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(", 原始名称: '");
			defaultInterpolatedStringHandler4.AppendFormatted(name ?? "自动");
			defaultInterpolatedStringHandler4.AppendLiteral("')");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return val3;
		}
		catch (Exception ex2)
		{
			LogError("CreateGrid 失败: " + ex2.GetType().Name + " - " + ex2.Message);
			return null;
		}
	}

	public object? CreateDimension(object document, object view, IList<object> references, object position)
	{
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Expected O, but got Unknown
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
			if (!ParsePoint(position, out XYZ xyz) || xyz == null)
			{
				LogError("CreateDimension: 无法解析位置");
				return null;
			}
			Element obj = new FilteredElementCollector(val).OfClass(typeof(DimensionType)).FirstElement();
			DimensionType val3 = (DimensionType)(object)((obj is DimensionType) ? obj : null);
			if (val3 == null)
			{
				LogError("CreateDimension: 无法获取默认尺寸标注类型");
				return null;
			}
			ReferenceArray val4 = new ReferenceArray();
			foreach (object reference in references)
			{
				Element val5 = (Element)((reference is Element) ? reference : null);
				if (val5 != null)
				{
					Reference val6 = new Reference(val5);
					val4.Append(val6);
					continue;
				}
				Reference val7 = (Reference)((reference is Reference) ? reference : null);
				if (val7 != null)
				{
					val4.Append(val7);
				}
			}
			if (val4.Size == 0)
			{
				LogError("CreateDimension: 没有有效的参考对象");
				return null;
			}
			Line val8 = Line.CreateBound(xyz, xyz.Add(XYZ.BasisX));
			Dimension val9 = ((ItemFactoryBase)val.Create).NewDimension(val2, val8, val4);
			if (val9 == null)
			{
				LogError("CreateDimension: NewDimension 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
			defaultInterpolatedStringHandler.AppendLiteral("CreateDimension: 成功创建尺寸标注 (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val9).Id.Value);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val9;
		}
		catch (Exception ex)
		{
			LogError("CreateDimension 失败: " + ex.GetType().Name + " - " + ex.Message);
			return null;
		}
	}

	private bool ParsePoint(object pointObj, [NotNullWhen(true)] out XYZ? xyz)
	{
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Expected O, but got Unknown
		try
		{
			if (pointObj == null)
			{
				xyz = null;
				return false;
			}
			Type type = pointObj.GetType();
			PropertyInfo property = type.GetProperty("x", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo property2 = type.GetProperty("y", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			PropertyInfo property3 = type.GetProperty("z", BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			if (property != null && property2 != null && property3 != null)
			{
				object value = property.GetValue(pointObj);
				object value2 = property2.GetValue(pointObj);
				object value3 = property3.GetValue(pointObj);
				double num = Convert.ToDouble(value);
				double num2 = Convert.ToDouble(value2);
				double num3 = Convert.ToDouble(value3);
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

	private static void LogInfo(string message)
	{
		Logger.Info("[CreationService] " + message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[CreationService] " + message);
	}

	private static void LogError(string message)
	{
		Logger.Error("[CreationService] " + message);
	}

	public object? CreateRoomAtPoint(object document, object level, object point, object? transaction = null)
	{
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateRoomAtPoint: document 不是 Document 类型");
				return null;
			}
			Level val2 = (Level)((level is Level) ? level : null);
			if (val2 == null)
			{
				LogError("CreateRoomAtPoint: level 不是 Level 类型");
				return null;
			}
			if (!ParsePoint(point, out XYZ xyz) || xyz == null)
			{
				LogError("CreateRoomAtPoint: 无法解析位置点");
				return null;
			}
			UV val3 = new UV(xyz.X, xyz.Y);
			Room val4 = val.Create.NewRoom(val2, val3);
			if (val4 == null)
			{
				LogError("CreateRoomAtPoint: NewRoom 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(57, 4);
			defaultInterpolatedStringHandler.AppendLiteral("CreateRoomAtPoint: 成功创建房间 (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val4).Id.Value);
			defaultInterpolatedStringHandler.AppendLiteral(", Level: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral(", Position: (");
			defaultInterpolatedStringHandler.AppendFormatted(xyz.X);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(xyz.Y);
			defaultInterpolatedStringHandler.AppendLiteral("))");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateRoomAtPoint 失败: " + ex.GetType().Name + " - " + ex.Message);
			return null;
		}
	}

	public object? CreateRoom(object document, double x, double y, int levelId)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateRoom: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)levelId));
			Level val2 = (Level)(object)((element is Level) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateRoom: 找不到 ID 为 ");
				defaultInterpolatedStringHandler.AppendFormatted(levelId);
				defaultInterpolatedStringHandler.AppendLiteral(" 的标高");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			new XYZ(x, y, 0.0);
			UV val3 = new UV(x, y);
			Room val4 = val.Create.NewRoom(val2, val3);
			if (val4 == null)
			{
				LogError("CreateRoom: Room.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(23, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateRoom: 成功创建房间，ID: ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)val4).Id.Value);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val4;
		}
		catch (Exception ex)
		{
			LogError("CreateRoom 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> CreateAllRoomsInLevel(object document, object level, object? transaction = null)
	{
		return CreateAllRoomsInLevel(document, level, null, transaction);
	}

	public IEnumerable<object> CreateAllRoomsInLevel(object document, object level, object? phase, object? transaction = null)
	{
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b5: Invalid comparison between Unknown and I4
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateAllRoomsInLevel: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			Level val2 = (Level)((level is Level) ? level : null);
			if (val2 == null)
			{
				LogError("CreateAllRoomsInLevel: level 不是 Level 类型");
				return Enumerable.Empty<object>();
			}
			Phase val3 = null;
			if (phase != null)
			{
				val3 = (Phase)((phase is Phase) ? phase : null);
				if (val3 == null)
				{
					LogError("CreateAllRoomsInLevel: phase 不是 Phase 类型");
					return Enumerable.Empty<object>();
				}
			}
			else
			{
				View activeView = val.ActiveView;
				if (activeView != null)
				{
					try
					{
						Parameter val4 = ((Element)activeView).get_Parameter((BuiltInParameter)(-1012102L));
						if (val4 != null && (int)val4.StorageType == 4)
						{
							ElementId val5 = val4.AsElementId();
							Element element = val.GetElement(val5);
							val3 = (Phase)(object)((element is Phase) ? element : null);
						}
					}
					catch
					{
					}
				}
				if (val3 == null)
				{
					IList<Element> source = new FilteredElementCollector(val).OfClass(typeof(Phase)).ToElements();
					val3 = (from p in source.OfType<Phase>()
						orderby ((Element)p).Id.Value
						select p).LastOrDefault();
				}
			}
			if (val3 == null)
			{
				LogError("CreateAllRoomsInLevel: 无法确定阶段");
				return Enumerable.Empty<object>();
			}
			ICollection<ElementId> collection = val.Create.NewRooms2(val2, val3);
			if (collection == null || !collection.Any())
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(60, 2);
				defaultInterpolatedStringHandler.AppendLiteral("CreateAllRoomsInLevel: 未在标高 ");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
				defaultInterpolatedStringHandler.AppendLiteral("（阶段 ");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)val3).Name);
				defaultInterpolatedStringHandler.AppendLiteral("）上创建任何房间（可能没有闭合区域或该阶段没有房间类型）");
				LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			foreach (ElementId item in collection)
			{
				Element element2 = val.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateAllRoomsInLevel: 成功在标高 ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler2.AppendLiteral(" 上创建 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个房间");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("CreateAllRoomsInLevel 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}
}
