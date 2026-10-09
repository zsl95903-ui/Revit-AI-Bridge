using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class CADGeometryService : ICADGeometryService
{
	private readonly UIApplication _application;

	public CADGeometryService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> GetCADImportInstances(object document)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetCADImportInstances: document 参数无效");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(ImportInstance));
			list.AddRange(val2.ToElements());
			FilteredElementCollector val3 = new FilteredElementCollector(val).OfClass(typeof(RevitLinkInstance));
			IList<Element> list2 = val3.ToElements();
			foreach (RevitLinkInstance item in list2)
			{
				RevitLinkInstance val4 = item;
				try
				{
					Document linkDocument = val4.GetLinkDocument();
					if (linkDocument != null)
					{
						FilteredElementCollector val5 = new FilteredElementCollector(linkDocument).OfClass(typeof(ImportInstance));
						list.AddRange(val5.ToElements());
					}
				}
				catch (Exception ex)
				{
					LogError("获取链接文档失败: " + ex.Message);
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(14, 1);
			defaultInterpolatedStringHandler.AppendLiteral("找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个 CAD 导入实例");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex2)
		{
			LogError("GetCADImportInstances 失败: " + ex2.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetGeometryObjects(object importInstance)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			Options val2 = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = true,
				IncludeNonVisibleObjects = true
			};
			GeometryElement val3 = ((Element)val).get_Geometry(val2);
			if ((GeometryObject)(object)val3 == (GeometryObject)null)
			{
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			foreach (GeometryObject item in val3)
			{
				list.Add(item);
				GeometryInstance val4 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val4 == null)
				{
					continue;
				}
				try
				{
					GeometryElement instanceGeometry = val4.GetInstanceGeometry();
					if (!((GeometryObject)(object)instanceGeometry != (GeometryObject)null))
					{
						continue;
					}
					foreach (GeometryObject item2 in instanceGeometry)
					{
						list.Add(item2);
					}
				}
				catch
				{
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
			defaultInterpolatedStringHandler.AppendLiteral("ImportInstance 包含 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个几何对象");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetGeometryObjects 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public string? GetGeometryObjectType(object geometryObject)
	{
		try
		{
			if (geometryObject == null)
			{
				return null;
			}
			Curve val = (Curve)((geometryObject is Curve) ? geometryObject : null);
			if (val != null)
			{
				if (val is Line)
				{
					return "Line";
				}
				if (val is Arc)
				{
					return "Arc";
				}
				if (val is Ellipse)
				{
					return "Ellipse";
				}
				if (val is HermiteSpline)
				{
					return "Spline";
				}
				if (val is NurbSpline)
				{
					return "NurbSpline";
				}
				return "Curve";
			}
			if (geometryObject is GeometryInstance)
			{
				return "Block";
			}
			if (geometryObject is Solid)
			{
				return "Solid";
			}
			if (geometryObject is Face)
			{
				return "Face";
			}
			if (geometryObject is Edge)
			{
				return "Edge";
			}
			return geometryObject.GetType().Name;
		}
		catch (Exception ex)
		{
			LogError("GetGeometryObjectType 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetGeometryObjectLayer(object geometryObject, object document)
	{
		try
		{
			GeometryObject val = (GeometryObject)((geometryObject is GeometryObject) ? geometryObject : null);
			if (val == null)
			{
				return null;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				LogError("GetGeometryObjectLayer: document 参数无效");
				return null;
			}
			if (val.GraphicsStyleId == (ElementId)null || val.GraphicsStyleId == ElementId.InvalidElementId)
			{
				return null;
			}
			Element element = val2.GetElement(val.GraphicsStyleId);
			GraphicsStyle val3 = (GraphicsStyle)(object)((element is GraphicsStyle) ? element : null);
			if (val3 == null)
			{
				return null;
			}
			Category graphicsStyleCategory = val3.GraphicsStyleCategory;
			return (graphicsStyleCategory != null) ? graphicsStyleCategory.Name : null;
		}
		catch (Exception ex)
		{
			LogError("GetGeometryObjectLayer 失败: " + ex.Message);
			return null;
		}
	}

	public (string? content, object? position) GetTextInfo(object textObject)
	{
		try
		{
			return (content: null, position: null);
		}
		catch (Exception ex)
		{
			LogError("GetTextInfo 失败: " + ex.Message);
			return (content: null, position: null);
		}
	}

	public (object? start, object? end) GetCurveEndpoints(object curveObject)
	{
		try
		{
			Curve val = (Curve)((curveObject is Curve) ? curveObject : null);
			if (val == null)
			{
				return (start: null, end: null);
			}
			Line val2 = (Line)(object)((val is Line) ? val : null);
			if (val2 != null)
			{
				return (start: ((Curve)val2).GetEndPoint(0), end: ((Curve)val2).GetEndPoint(1));
			}
			Arc val3 = (Arc)(object)((val is Arc) ? val : null);
			if (val3 != null)
			{
				return (start: ((Curve)val3).Evaluate(0.0, false), end: ((Curve)val3).Evaluate(1.0, false));
			}
			return (start: null, end: null);
		}
		catch (Exception ex)
		{
			LogError("GetCurveEndpoints 失败: " + ex.Message);
			return (start: null, end: null);
		}
	}

	public (string? name, object? position) GetBlockInfo(object instanceObject)
	{
		try
		{
			GeometryInstance val = (GeometryInstance)((instanceObject is GeometryInstance) ? instanceObject : null);
			if (val == null)
			{
				return (name: null, position: null);
			}
			Transform transform = val.Transform;
			XYZ origin = transform.Origin;
			return (name: null, position: origin);
		}
		catch (Exception ex)
		{
			LogError("GetBlockInfo 失败: " + ex.Message);
			return (name: null, position: null);
		}
	}

	public IEnumerable<object> FilterByLayer(IEnumerable<object> geometryObjects, string layerName, object document)
	{
		try
		{
			return geometryObjects.Where((object geom) => GetGeometryObjectLayer(geom, document) == layerName).ToList();
		}
		catch (Exception ex)
		{
			LogError("FilterByLayer 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public Dictionary<string, List<object>> GroupByType(IEnumerable<object> geometryObjects)
	{
		try
		{
			Dictionary<string, List<object>> dictionary = new Dictionary<string, List<object>>();
			foreach (object geometryObject in geometryObjects)
			{
				string key = GetGeometryObjectType(geometryObject) ?? "Unknown";
				if (!dictionary.ContainsKey(key))
				{
					dictionary[key] = new List<object>();
				}
				dictionary[key].Add(geometryObject);
			}
			return dictionary;
		}
		catch (Exception ex)
		{
			LogError("GroupByType 失败: " + ex.Message);
			return new Dictionary<string, List<object>>();
		}
	}

	public List<CADTextInfo> GetTextsByExploding(object importInstance, object document)
	{
		List<CADTextInfo> result = new List<CADTextInfo>();
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return result;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				return result;
			}
			LogWarning("[炸开法] ImportInstance.Explode() 在当前 Revit 版本中不可用，此方法暂未实现");
			return result;
		}
		catch (Exception ex)
		{
			LogError("GetTextsByExploding 失败: " + ex.Message);
			return result;
		}
	}

	public (double X, double Y, double Z)? GetCADImportPosition(object importInstance)
	{
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Expected O, but got Unknown
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			Location location = ((Element)val).Location;
			LocationPoint val2 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val2 != null)
			{
				XYZ point = val2.Point;
				return (point.X * 0.3048, point.Y * 0.3048, point.Z * 0.3048);
			}
			Options val3 = new Options
			{
				DetailLevel = (ViewDetailLevel)1,
				ComputeReferences = false
			};
			GeometryElement val4 = ((Element)val).get_Geometry(val3);
			if ((GeometryObject)(object)val4 != (GeometryObject)null)
			{
				foreach (GeometryObject item in val4)
				{
					GeometryInstance val5 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
					if (val5 != null)
					{
						Transform transform = val5.Transform;
						XYZ origin = transform.Origin;
						return (origin.X * 0.3048, origin.Y * 0.3048, origin.Z * 0.3048);
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetCADImportPosition 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetCADImportRotation(object importInstance)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			Location location = ((Element)val).Location;
			LocationPoint val2 = (LocationPoint)(object)((location is LocationPoint) ? location : null);
			if (val2 != null)
			{
				return val2.Rotation * 180.0 / Math.PI;
			}
			Options val3 = new Options
			{
				DetailLevel = (ViewDetailLevel)1,
				ComputeReferences = false
			};
			GeometryElement val4 = ((Element)val).get_Geometry(val3);
			if ((GeometryObject)(object)val4 != (GeometryObject)null)
			{
				foreach (GeometryObject item in val4)
				{
					GeometryInstance val5 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
					if (val5 != null)
					{
						Transform transform = val5.Transform;
						XYZ basisX = transform.BasisX;
						double num = Math.Atan2(basisX.Y, basisX.X);
						return num * 180.0 / Math.PI;
					}
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetCADImportRotation 失败: " + ex.Message);
			return null;
		}
	}

	public int? GetCADImportOwnerViewId(object importInstance)
	{
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			ElementId ownerViewId = ((Element)val).OwnerViewId;
			if (ownerViewId == (ElementId)null || ownerViewId == ElementId.InvalidElementId)
			{
				return null;
			}
			return (int)ownerViewId.Value;
		}
		catch (Exception ex)
		{
			LogError("GetCADImportOwnerViewId 失败: " + ex.Message);
			return null;
		}
	}

	public int? GetCADImportLevelId(object importInstance)
	{
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			ElementId levelId = ((Element)val).LevelId;
			if (levelId == (ElementId)null || levelId == ElementId.InvalidElementId)
			{
				return null;
			}
			return (int)levelId.Value;
		}
		catch (Exception ex)
		{
			LogError("GetCADImportLevelId 失败: " + ex.Message);
			return null;
		}
	}

	private void LogError(string message)
	{
		try
		{
			Logger.Error("[CADGeometryService] " + message);
		}
		catch
		{
		}
	}

	private void LogWarning(string message)
	{
		try
		{
			Logger.Warning("[CADGeometryService] " + message);
		}
		catch
		{
		}
	}

	private void LogInfo(string message)
	{
		try
		{
			Logger.Info("[CADGeometryService] " + message);
		}
		catch
		{
		}
	}
}
