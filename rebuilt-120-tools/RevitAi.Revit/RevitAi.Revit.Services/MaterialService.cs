using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class MaterialService : IMaterialService
{
	private readonly UIApplication _application;

	public MaterialService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> GetAllMaterials(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(Material));
			return val2.ToElements();
		}
		catch (Exception ex)
		{
			LogError("GetAllMaterials 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetMaterialByName(object document, string materialName)
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			FilteredElementCollector source = new FilteredElementCollector(val).OfClass(typeof(Material));
			Material val2 = ((IEnumerable)source).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Equals(materialName, StringComparison.OrdinalIgnoreCase));
			if (val2 != null)
			{
				return val2;
			}
			Material val3 = ((IEnumerable)source).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Contains(materialName) || materialName.Contains(((Element)m).Name));
			if (val3 != null)
			{
				return val3;
			}
			string cleanName = materialName.Replace(",", "").Replace("，", "").Replace(" ", "")
				.Trim();
			if (cleanName != materialName)
			{
				Material val4 = ((IEnumerable)source).Cast<Material>().FirstOrDefault((Material m) => ((Element)m).Name.Equals(cleanName, StringComparison.OrdinalIgnoreCase) || ((Element)m).Name.Contains(cleanName) || cleanName.Contains(((Element)m).Name));
				if (val4 != null)
				{
					return val4;
				}
			}
			LogError("GetMaterialByName: 找不到材质 '" + materialName + "'");
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetMaterialByName 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetElementMaterials(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			ICollection<ElementId> materialIds = val.GetMaterialIds(false);
			Document doc = val.Document;
			return from id in materialIds
				select doc.GetElement(id) into m
				where m != null
				select m;
		}
		catch (Exception ex)
		{
			LogError("GetElementMaterials 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public bool SetElementMaterial(object element, int materialId, object document)
	{
		return SetElementMaterial(element, materialId, document, (MaterialSetStrategy)0);
	}

	public bool SetElementMaterial(object element, int materialId, object document, MaterialSetStrategy strategy = (MaterialSetStrategy)0, string? parameterName = null, int layerIndex = 0)
	{
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Invalid comparison between Unknown and I4
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Expected I4, but got Unknown
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val != null)
			{
				Document val2 = (Document)((document is Document) ? document : null);
				if (val2 != null)
				{
					Element element2 = val2.GetElement(new ElementId((long)materialId));
					Material val3 = (Material)(object)((element2 is Material) ? element2 : null);
					if (val3 == null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
						defaultInterpolatedStringHandler.AppendLiteral("SetElementMaterial: 找不到材质 ID ");
						defaultInterpolatedStringHandler.AppendFormatted(materialId);
						LogError(defaultInterpolatedStringHandler.ToStringAndClear());
						return false;
					}
					if ((int)strategy == 0)
					{
						strategy = DetectMaterialSetStrategy(val);
					}
					return ((int)(strategy) - 1) switch
					{
						0 => SetCompoundStructureLayerMaterial(val, val3, val2, layerIndex), 
						1 => SetTypeParameterMaterial(val, val3, parameterName ?? "材质"), 
						2 => SetInstanceParameterMaterial(val, val3, parameterName ?? "材质"), 
						_ => false, 
					};
				}
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(73, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("SetElementMaterial: 类型检查失败 - element is Element: ");
			defaultInterpolatedStringHandler2.AppendFormatted(element is Element);
			defaultInterpolatedStringHandler2.AppendLiteral(", document is Document: ");
			defaultInterpolatedStringHandler2.AppendFormatted(document is Document);
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetElementMaterial 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetElementMaterialByName(object element, string materialName, object document, MaterialSetStrategy strategy = (MaterialSetStrategy)0, string? parameterName = null, int layerIndex = 0)
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetElementMaterialByName: 无效的文档类型");
				return false;
			}
			object? materialByName = GetMaterialByName(document, materialName);
			Material val2 = (Material)((materialByName is Material) ? materialByName : null);
			if (val2 == null)
			{
				LogError("SetElementMaterialByName: 找不到材质 " + materialName);
				return false;
			}
			return SetElementMaterial(element, (int)((Element)val2).Id.Value, document, strategy, parameterName, layerIndex);
		}
		catch (Exception ex)
		{
			LogError("SetElementMaterialByName 失败: " + ex.Message);
			return false;
		}
	}

	public (int successCount, int failedCount) SetElementMaterialsBatch(IEnumerable<object> elements, int materialId, object document, MaterialSetStrategy strategy = (MaterialSetStrategy)0, string? parameterName = null, int layerIndex = 0)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		int num2 = 0;
		foreach (object element in elements)
		{
			if (SetElementMaterial(element, materialId, document, strategy, parameterName, layerIndex))
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		return (successCount: num, failedCount: num2);
	}

	public (int successCount, int failedCount) SetMaterialsByCategoryMap(object document, IDictionary<string, string> categoryMaterialMap, bool elementTypeFilter = true)
	{
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		int num = 0;
		int num2 = 0;
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SetMaterialsByCategoryMap: 无效的文档类型");
				return (successCount: 0, failedCount: 0);
			}
			foreach (KeyValuePair<string, string> item in categoryMaterialMap)
			{
				try
				{
					string key = item.Key;
					string value = item.Value;
					Category val2 = ((CategoryNameMap)val.Settings.Categories).get_Item(key);
					if (val2 == null)
					{
						LogWarning("未找到类别: " + key);
						continue;
					}
					FilteredElementCollector val3 = new FilteredElementCollector(val);
					if (elementTypeFilter)
					{
						val3.WhereElementIsElementType();
					}
					val3.OfCategoryId(val2.Id);
					IList<Element> list = val3.ToElements();
					foreach (Element item2 in list)
					{
						if (SetElementMaterialByName(item2, value, document, (MaterialSetStrategy)0))
						{
							num++;
						}
						else
						{
							num2++;
						}
					}
				}
				catch (Exception ex)
				{
					LogError("处理类别 " + item.Key + " 失败: " + ex.Message);
					num2++;
				}
			}
		}
		catch (Exception ex2)
		{
			LogError("SetMaterialsByCategoryMap 失败: " + ex2.Message);
		}
		return (successCount: num, failedCount: num2);
	}

	public object? CreateMaterial(object document, string materialName)
	{
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateMaterial: 无效的文档类型");
				return null;
			}
			if (string.IsNullOrWhiteSpace(materialName))
			{
				LogError("CreateMaterial: 材质名称不能为空");
				return null;
			}
			object materialByName = GetMaterialByName(document, materialName);
			if (materialByName != null)
			{
				LogWarning("CreateMaterial: 材质 '" + materialName + "' 已存在");
				return materialByName;
			}
			Material val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Material))).Cast<Material>().FirstOrDefault();
			if (val2 == null)
			{
				LogError("CreateMaterial: 找不到可用的材质模板");
				return null;
			}
			Material val3 = val2.Duplicate(materialName);
			if (val3 == null)
			{
				LogError("CreateMaterial: 创建材质 '" + materialName + "' 失败");
				return null;
			}
			LogInfo("✅ 成功创建材质: " + materialName);
			return val3;
		}
		catch (Exception ex)
		{
			LogError("CreateMaterial 失败: " + ex.Message);
			return null;
		}
	}

	public object? DuplicateMaterial(object document, int sourceMaterialId, string newMaterialName)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			Element element = val.GetElement(new ElementId((long)sourceMaterialId));
			Material val2 = (Material)(object)((element is Material) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 1);
				defaultInterpolatedStringHandler.AppendLiteral("DuplicateMaterial: 找不到源材质 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(sourceMaterialId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			LogError("DuplicateMaterial: 方法尚未完全实现");
			return null;
		}
		catch (Exception ex)
		{
			LogError("DuplicateMaterial 失败: " + ex.Message);
			return null;
		}
	}

	public (string? Name, float Red, float Green, float Blue, float Alpha)? GetMaterialProperties(object material)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				return null;
			}
			Color color = val.Color;
			return (((Element)val).Name, (int)color.Red, (int)color.Green, (int)color.Blue, 255f);
		}
		catch (Exception ex)
		{
			LogError("GetMaterialProperties 失败: " + ex.Message);
			return null;
		}
	}

	private static void LogError(string message)
	{
		Logger.Error("[MaterialService] " + message);
	}

	public IEnumerable<object> GetMaterials(object document)
	{
		return GetAllMaterials(document);
	}

	public string? GetMaterialClass(object material)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				return null;
			}
			try
			{
				PropertyInfo property = ((object)val).GetType().GetProperty("Class");
				if (property != null)
				{
					object value = property.GetValue(val);
					if (value is string result)
					{
						return result;
					}
				}
			}
			catch
			{
			}
			return ((Element)val).Name;
		}
		catch (Exception ex)
		{
			LogError("GetMaterialClass 失败: " + ex.Message);
			return null;
		}
	}

	public (byte R, byte G, byte B)? GetMaterialColor(object material)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				return null;
			}
			Color color = val.Color;
			return (color.Red, color.Green, color.Blue);
		}
		catch (Exception ex)
		{
			LogError("GetMaterialColor 失败: " + ex.Message);
			return null;
		}
	}

	public bool SetMaterialColor(object material, byte red, byte green, byte blue)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected O, but got Unknown
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				LogError("SetMaterialColor: 无效的材质类型");
				return false;
			}
			val.Color = new Color(red, green, blue);
			Color color = val.Color;
			if (color.Red != red || color.Green != green || color.Blue != blue)
			{
				LogWarning("SetMaterialColor: 设置后颜色验证不完全匹配，可能是系统限制");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 4);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 成功将材质 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' 的颜色设置为 RGB(");
			defaultInterpolatedStringHandler.AppendFormatted(red);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(green);
			defaultInterpolatedStringHandler.AppendLiteral(", ");
			defaultInterpolatedStringHandler.AppendFormatted(blue);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetMaterialColor 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetMaterialTransparency(object material, int transparency)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				LogError("SetMaterialTransparency: 无效的材质类型");
				return false;
			}
			if (transparency < 0 || transparency > 100)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
				defaultInterpolatedStringHandler.AppendLiteral("SetMaterialTransparency: 透明度值必须在 0-100 范围内，当前值: ");
				defaultInterpolatedStringHandler.AppendFormatted(transparency);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			int transparency2 = val.Transparency;
			bool useRenderAppearanceForShading = val.UseRenderAppearanceForShading;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(75, 3);
			defaultInterpolatedStringHandler2.AppendLiteral("[MaterialService] 材质 '");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler2.AppendLiteral("' 当前状态: Transparency=");
			defaultInterpolatedStringHandler2.AppendFormatted(transparency2);
			defaultInterpolatedStringHandler2.AppendLiteral(", UseRenderAppearanceForShading=");
			defaultInterpolatedStringHandler2.AppendFormatted(useRenderAppearanceForShading);
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			if (val.UseRenderAppearanceForShading)
			{
				val.UseRenderAppearanceForShading = false;
				LogInfo("[MaterialService] 已将材质 '" + ((Element)val).Name + "' 的 UseRenderAppearanceForShading 设置为 false");
			}
			val.Transparency = transparency;
			int transparency3 = val.Transparency;
			if (transparency3 != transparency)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(40, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("SetMaterialTransparency: 设置后验证失败，期望 ");
				defaultInterpolatedStringHandler3.AppendFormatted(transparency);
				defaultInterpolatedStringHandler3.AppendLiteral("，实际 ");
				defaultInterpolatedStringHandler3.AppendFormatted(transparency3);
				LogError(defaultInterpolatedStringHandler3.ToStringAndClear());
				return false;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(24, 3);
			defaultInterpolatedStringHandler4.AppendLiteral("✅ 成功将材质 '");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler4.AppendLiteral("' 的透明度从 ");
			defaultInterpolatedStringHandler4.AppendFormatted(transparency2);
			defaultInterpolatedStringHandler4.AppendLiteral("% 设置为 ");
			defaultInterpolatedStringHandler4.AppendFormatted(transparency);
			defaultInterpolatedStringHandler4.AppendLiteral("%");
			LogInfo(defaultInterpolatedStringHandler4.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("SetMaterialTransparency 失败: " + ex.Message);
			return false;
		}
	}

	public int? GetMaterialTransparency(object material)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				LogError("GetMaterialTransparency: 无效的材质类型");
				return null;
			}
			return val.Transparency;
		}
		catch (Exception ex)
		{
			LogError("GetMaterialTransparency 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetAppearanceName(object material)
	{
		try
		{
			Material val = (Material)((material is Material) ? material : null);
			if (val == null)
			{
				return null;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetAppearanceName 失败: " + ex.Message);
			return null;
		}
	}

	private static MaterialSetStrategy DetectMaterialSetStrategy(Element element)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (!(element is FloorType))
		{
			if (!(element is WallType))
			{
				return (MaterialSetStrategy)2;
			}
			return (MaterialSetStrategy)1;
		}
		return (MaterialSetStrategy)1;
	}

	private bool SetCompoundStructureLayerMaterial(Element element, Material material, Document doc, int layerIndex)
	{
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		try
		{
			CompoundStructure val = null;
			WallType val2 = (WallType)(object)((element is WallType) ? element : null);
			if (val2 != null)
			{
				val = ((HostObjAttributes)val2).GetCompoundStructure();
			}
			else
			{
				FloorType val3 = (FloorType)(object)((element is FloorType) ? element : null);
				if (val3 == null)
				{
					return false;
				}
				val = ((HostObjAttributes)val3).GetCompoundStructure();
			}
			if (val == null)
			{
				return false;
			}
			IList<CompoundStructureLayer> layers = val.GetLayers();
			if (layers.Count <= layerIndex)
			{
				return false;
			}
			List<CompoundStructureLayer> list = layers.ToList();
			list[layerIndex] = new CompoundStructureLayer(layers[layerIndex].Width, layers[layerIndex].Function, ((Element)material).Id);
			val.SetLayers((IList<CompoundStructureLayer>)list);
			WallType val4 = (WallType)(object)((element is WallType) ? element : null);
			if (val4 != null)
			{
				((HostObjAttributes)val4).SetCompoundStructure(val);
			}
			else
			{
				FloorType val5 = (FloorType)(object)((element is FloorType) ? element : null);
				if (val5 != null)
				{
					((HostObjAttributes)val5).SetCompoundStructure(val);
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			LogError("设置复合结构层材质失败: " + ex.Message);
			return false;
		}
	}

	private bool SetTypeParameterMaterial(Element element, Material material, string parameterName)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Invalid comparison between Unknown and I4
		try
		{
			foreach (Parameter parameter in element.Parameters)
			{
				Parameter val = parameter;
				if (val.Definition.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase) && (int)val.StorageType == 4)
				{
					val.Set(((Element)material).Id);
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			LogError("设置类型参数材质失败: " + ex.Message);
			return false;
		}
	}

	private bool SetInstanceParameterMaterial(Element element, Material material, string parameterName)
	{
		return SetTypeParameterMaterial(element, material, parameterName);
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[MaterialService] " + message);
	}

	private static void LogDebug(string message)
	{
		Logger.Debug("[MaterialService] " + message);
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[MaterialService] " + message);
	}

	public bool DeleteMaterial(object document, string materialName, bool checkUsage = true)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DeleteMaterial: 无效的文档类型");
				return false;
			}
			if (string.IsNullOrWhiteSpace(materialName))
			{
				LogError("DeleteMaterial: 材质名称不能为空");
				return false;
			}
			object? materialByName = GetMaterialByName(document, materialName);
			Material val2 = (Material)((materialByName is Material) ? materialByName : null);
			if (val2 == null)
			{
				LogError("DeleteMaterial: 找不到材质 '" + materialName + "'");
				return false;
			}
			if (checkUsage && IsMaterialInUse(document, (int)((Element)val2).Id.Value))
			{
				LogWarning("DeleteMaterial: 材质 '" + materialName + "' 正被使用，无法删除");
				return false;
			}
			val.Delete(((Element)val2).Id);
			LogInfo("✅ 成功删除材质: " + materialName);
			return true;
		}
		catch (Exception ex)
		{
			LogError("DeleteMaterial 失败: " + ex.Message);
			return false;
		}
	}

	public bool RenameMaterial(object document, string oldMaterialName, string newMaterialName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("RenameMaterial: 无效的文档类型");
				return false;
			}
			if (string.IsNullOrWhiteSpace(oldMaterialName) || string.IsNullOrWhiteSpace(newMaterialName))
			{
				LogError("RenameMaterial: 材质名称不能为空");
				return false;
			}
			object? materialByName = GetMaterialByName(document, oldMaterialName);
			Material val2 = (Material)((materialByName is Material) ? materialByName : null);
			if (val2 == null)
			{
				LogError("RenameMaterial: 找不到材质 '" + oldMaterialName + "'");
				return false;
			}
			object materialByName2 = GetMaterialByName(document, newMaterialName);
			if (materialByName2 != null)
			{
				LogWarning("RenameMaterial: 材质 '" + newMaterialName + "' 已存在");
				return false;
			}
			((Element)val2).Name = newMaterialName;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 2);
			defaultInterpolatedStringHandler.AppendLiteral("✅ 成功将材质 '");
			defaultInterpolatedStringHandler.AppendFormatted(oldMaterialName);
			defaultInterpolatedStringHandler.AppendLiteral("' 重命名为 '");
			defaultInterpolatedStringHandler.AppendFormatted(newMaterialName);
			defaultInterpolatedStringHandler.AppendLiteral("'");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return true;
		}
		catch (Exception ex)
		{
			LogError("RenameMaterial 失败: " + ex.Message);
			return false;
		}
	}

	public bool IsMaterialInUse(object document, int materialId)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("IsMaterialInUse: 无效的文档类型");
				return false;
			}
			Element element = val.GetElement(new ElementId((long)materialId));
			Material val2 = (Material)(object)((element is Material) ? element : null);
			if (val2 == null)
			{
				return false;
			}
			IEnumerable<object> elementsUsingMaterial = GetElementsUsingMaterial(document, materialId);
			return elementsUsingMaterial.Any();
		}
		catch (Exception ex)
		{
			LogError("IsMaterialInUse 失败: " + ex.Message);
			return false;
		}
	}

	public IEnumerable<object> GetElementsUsingMaterial(object document, int materialId)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetElementsUsingMaterial: 无效的文档类型");
				return Enumerable.Empty<object>();
			}
			Element element = val.GetElement(new ElementId((long)materialId));
			Material val2 = (Material)(object)((element is Material) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetElementsUsingMaterial: 找不到材质 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(materialId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val3 = new FilteredElementCollector(val);
			IList<Element> list = val3.WhereElementIsNotElementType().ToElements();
			List<Element> list2 = new List<Element>();
			foreach (Element item in list)
			{
				try
				{
					ICollection<ElementId> materialIds = item.GetMaterialIds(false);
					if (materialIds.Contains(((Element)val2).Id))
					{
						list2.Add(item);
					}
				}
				catch
				{
				}
			}
			return list2.Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetElementsUsingMaterial 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}
}
