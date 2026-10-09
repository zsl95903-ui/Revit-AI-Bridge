using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ns0;
using ns6;

using Document = Autodesk.Revit.DB.Document;

using ArgumentException = System.ArgumentException;
using InvalidOperationException = System.InvalidOperationException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.Services;

internal sealed class FamilyService : IFamilyService
{
	private class FamilyLoadOptions : IFamilyLoadOptions
	{
		public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
		{
			overwriteParameterValues = true;
			return true;
		}

		public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
		{
			source = (FamilySource)0;
			overwriteParameterValues = true;
			return true;
		}
	}

	private readonly UIApplication _application;

	private readonly IParameterService _parameterService;

	public FamilyService(UIApplication application, IParameterService parameterService)
	{
		_application = application ?? throw new ArgumentNullException("application");
		_parameterService = parameterService ?? throw new ArgumentNullException("parameterService");
	}

	public IEnumerable<object> GetAllFamilies(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(Family));
			return val2.ToElements();
		}
		catch (Exception ex)
		{
			LogError("GetAllFamilies 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetFamilyTypes(object document, string familyName)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			IEnumerable<Family> enumerable = from Family f in (IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Family))
				where ((Element)f).Name.Equals(familyName, StringComparison.OrdinalIgnoreCase)
				select f;
			List<object> list = new List<object>();
			foreach (Family item in enumerable)
			{
				ISet<ElementId> familySymbolIds = item.GetFamilySymbolIds();
				foreach (ElementId item2 in familySymbolIds)
				{
					Element element = val.GetElement(item2);
					if (element != null)
					{
						list.Add(element);
					}
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetFamilyTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? LoadFamily(object document, string familyFilePath)
	{
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("LoadFamily: document 不是 Document 类型");
				return null;
			}
			LogError("[LoadFamily] 开始加载族文件: " + familyFilePath);
			if (!File.Exists(familyFilePath))
			{
				LogError("[LoadFamily] 族文件不存在: " + familyFilePath);
				return null;
			}
			FamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();
			LogError("[LoadFamily] 调用 doc.LoadFamily...");
			Family family = null;
			bool value;
			if (!(value = val.LoadFamily(familyFilePath, (IFamilyLoadOptions)(object)familyLoadOptions, out family)) || family == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[LoadFamily] doc.LoadFamily 失败: loadResult=");
				defaultInterpolatedStringHandler.AppendFormatted(value);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				LogError("[LoadFamily] 这通常意味着族文件版本不兼容或加载失败");
				LogError("[LoadFamily] 族文件: " + familyFilePath);
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[LoadFamily] 族加载成功: ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)family).Name);
			defaultInterpolatedStringHandler2.AppendLiteral(", ID: ");
			defaultInterpolatedStringHandler2.AppendFormatted(((Element)family).Id.Value);
			LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
			Family val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family f) => ((Element)f).Id == ((Element)family).Id);
			if (val2 == null)
			{
				LogError("[LoadFamily] 验证失败：族未在文档中找到");
				return null;
			}
			LogError("[LoadFamily] 验证成功：族已在文档中");
			return family;
		}
		catch (InvalidOperationException ex)
		{
			LogError("[LoadFamily] InvalidOperationException: " + ex.Message);
			LogError("[LoadFamily] 族文件: " + familyFilePath);
			LogError("[LoadFamily] 这通常意味着族文件版本与当前 Revit 版本不兼容");
			return null;
		}
		catch (ArgumentException ex2)
		{
			LogError("[LoadFamily] ArgumentException: " + ex2.Message);
			LogError("[LoadFamily] 族文件: " + familyFilePath);
			return null;
		}
		catch (Exception ex3)
		{
			LogError("[LoadFamily] " + ex3.GetType().Name + ": " + ex3.Message);
			LogError("[LoadFamily] 族文件: " + familyFilePath);
			return null;
		}
	}

	public object? CreateFamilyInstance(object document, int familySymbolId, double positionX, double positionY, double positionZ, int? levelId = null)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			Element element = val.GetElement(new ElementId((long)familySymbolId));
			FamilySymbol val2 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler.AppendLiteral("CreateFamilyInstance: 找不到族类型符号 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(familySymbolId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			if (!val2.IsActive)
			{
				val2.Activate();
				val.Regenerate();
			}
			return TryPlaceFamilyInstance(val, val2, positionX, positionY, positionZ, levelId);
		}
		catch (Exception ex)
		{
			LogError("CreateFamilyInstance 失败: " + ex.Message);
			return null;
		}
	}

	private object? TryPlaceFamilyInstance(Document doc, FamilySymbol symbol, double x, double y, double z, int? levelId)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		Level val = null;
		if (levelId.HasValue)
		{
			Element element = doc.GetElement(new ElementId((long)levelId.Value));
			val = (Level)(object)((element is Level) ? element : null);
		}
		if (val != null)
		{
			try
			{
				return ((ItemFactoryBase)doc.Create).NewFamilyInstance(new XYZ(x, y, z), symbol, val, (StructuralType)0);
			}
			catch (Exception ex)
			{
				LogWarning("方式1（基于标高）放置失败: " + ex.Message + "，尝试下一种方式...");
			}
		}
		if (val != null)
		{
			try
			{
				double num = val.Elevation + z;
				return ((ItemFactoryBase)doc.Create).NewFamilyInstance(new XYZ(x, y, num), symbol, (StructuralType)0);
			}
			catch (Exception ex2)
			{
				LogWarning("方式2（工作平面+标高高程）放置失败: " + ex2.Message + "，尝试下一种方式...");
			}
		}
		if (val == null)
		{
			try
			{
				return ((ItemFactoryBase)doc.Create).NewFamilyInstance(new XYZ(x, y, z), symbol, (StructuralType)0);
			}
			catch (Exception ex3)
			{
				LogWarning("方式3（纯坐标）放置失败: " + ex3.Message);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 5);
		defaultInterpolatedStringHandler.AppendLiteral("所有放置方式均失败: symbol=");
		defaultInterpolatedStringHandler.AppendFormatted(((Element)symbol).Name);
		defaultInterpolatedStringHandler.AppendLiteral(", x=");
		defaultInterpolatedStringHandler.AppendFormatted(x);
		defaultInterpolatedStringHandler.AppendLiteral(", y=");
		defaultInterpolatedStringHandler.AppendFormatted(y);
		defaultInterpolatedStringHandler.AppendLiteral(", z=");
		defaultInterpolatedStringHandler.AppendFormatted(z);
		defaultInterpolatedStringHandler.AppendLiteral(", levelId=");
		defaultInterpolatedStringHandler.AppendFormatted(levelId);
		LogError(defaultInterpolatedStringHandler.ToStringAndClear());
		return null;
	}

	public object? GetElementFamily(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			FamilyInstance val2 = (FamilyInstance)(object)((val is FamilyInstance) ? val : null);
			if (val2 != null)
			{
				return val2.Symbol.Family;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementFamily 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetElementFamilySymbol(object element)
	{
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			FamilyInstance val2 = (FamilyInstance)(object)((val is FamilyInstance) ? val : null);
			if (val2 != null)
			{
				return val2.Symbol;
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementFamilySymbol 失败: " + ex.Message);
			return null;
		}
	}

	public bool ChangeFamilySymbol(object element, int newSymbolId, object document)
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Expected O, but got Unknown
		try
		{
			FamilyInstance val = (FamilyInstance)((element is FamilyInstance) ? element : null);
			if (val == null)
			{
				LogError("ChangeFamilySymbol: element 不是 FamilyInstance 类型");
				return false;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				return false;
			}
			Element element2 = val2.GetElement(new ElementId((long)newSymbolId));
			FamilySymbol val3 = (FamilySymbol)(object)((element2 is FamilySymbol) ? element2 : null);
			if (val3 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
				defaultInterpolatedStringHandler.AppendLiteral("ChangeFamilySymbol: 找不到族类型符号 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(newSymbolId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return false;
			}
			val.Symbol = val3;
			return true;
		}
		catch (Exception ex)
		{
			LogError("ChangeFamilySymbol 失败: " + ex.Message);
			return false;
		}
	}

	private static void LogError(string message)
	{
	}

	public IEnumerable<object> GetFamilyTypes(object document, int familyId)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetFamilyTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			Element element = val.GetElement(new ElementId((long)familyId));
			Family val2 = (Family)(object)((element is Family) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(24, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetFamilyTypes: 找不到族 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(familyId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			ISet<ElementId> familySymbolIds = val2.GetFamilySymbolIds();
			foreach (ElementId item in familySymbolIds)
			{
				Element element2 = val.GetElement(item);
				if (element2 != null)
				{
					list.Add(element2);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetFamilyTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? LoadFamily(object document, string familyFilePath, string? familyName)
	{
		return LoadFamily(document, familyFilePath);
	}

	public object? GetFamilyOfTypeId(object document, int typeId)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetFamilyOfTypeId: document 不是 Document 类型");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)typeId));
			FamilySymbol val2 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
				defaultInterpolatedStringHandler.AppendLiteral("GetFamilyOfTypeId: 找不到类型 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(typeId);
				defaultInterpolatedStringHandler.AppendLiteral(" 的族符号");
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			return val2.Family;
		}
		catch (Exception ex)
		{
			LogError("GetFamilyOfTypeId 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<int>? GetFamilySymbolIds(object family)
	{
		try
		{
			Family val = (Family)((family is Family) ? family : null);
			if (val == null)
			{
				LogError("GetFamilySymbolIds: family 不是 Family 类型");
				return null;
			}
			ISet<ElementId> familySymbolIds = val.GetFamilySymbolIds();
			return familySymbolIds.Select((ElementId id) => (int)id.Value);
		}
		catch (Exception ex)
		{
			LogError("GetFamilySymbolIds 失败: " + ex.Message);
			return null;
		}
	}

	public object? DuplicateFamilyType(object document, int sourceTypeId, string newTypeName)
	{
		return DuplicateFamilyType(document, sourceTypeId, newTypeName, null);
	}

	public object? DuplicateFamilyType(object document, int sourceTypeId, string newTypeName, IDictionary<string, object>? parameterValues)
	{
		//IL_03a1: Expected O, but got Unknown
		//IL_03ea: Expected O, but got Unknown
		//IL_0945: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0196: Invalid comparison between Unknown and I4
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("DuplicateFamilyType: document 不是 Document 类型");
				return null;
			}
			if (string.IsNullOrWhiteSpace(newTypeName))
			{
				LogError("DuplicateFamilyType: 新类型名称不能为空");
				return null;
			}
			Element element = val.GetElement(new ElementId((long)sourceTypeId));
			if (element == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("DuplicateFamilyType: 找不到源类型 ID ");
				defaultInterpolatedStringHandler.AppendFormatted(sourceTypeId);
				LogError(defaultInterpolatedStringHandler.ToStringAndClear());
				return null;
			}
			ElementType val2 = (ElementType)(object)((element is ElementType) ? element : null);
			if (val2 == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(49, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("DuplicateFamilyType: 元素 ");
				defaultInterpolatedStringHandler2.AppendFormatted(sourceTypeId);
				defaultInterpolatedStringHandler2.AppendLiteral(" 不是 ElementType 类型，实际类型: ");
				defaultInterpolatedStringHandler2.AppendFormatted(((object)element).GetType().Name);
				LogError(defaultInterpolatedStringHandler2.ToStringAndClear());
				return null;
			}
			string name = ((Element)val2).Name;
			string name2 = ((object)val2).GetType().Name;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(30, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("DuplicateFamilyType: 源类型 '");
			defaultInterpolatedStringHandler3.AppendFormatted(name);
			defaultInterpolatedStringHandler3.AppendLiteral("' (");
			defaultInterpolatedStringHandler3.AppendFormatted(name2);
			defaultInterpolatedStringHandler3.AppendLiteral(")");
			LogInfo(defaultInterpolatedStringHandler3.ToStringAndClear());
			ElementType val3 = val2;
			WallType val4 = (WallType)(object)((val2 is WallType) ? val2 : null);
			if (val4 != null && (int)val4.Kind > 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(55, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("DuplicateFamilyType: 源墙类型 '");
				defaultInterpolatedStringHandler4.AppendFormatted(name);
				defaultInterpolatedStringHandler4.AppendLiteral("' 不是基本墙（Kind: ");
				defaultInterpolatedStringHandler4.AppendFormatted<WallKind>(val4.Kind);
				defaultInterpolatedStringHandler4.AppendLiteral("），尝试查找可用的基本墙类型");
				LogWarning(defaultInterpolatedStringHandler4.ToStringAndClear());
				WallType val5 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(WallType))).Cast<WallType>().FirstOrDefault(delegate(WallType wt)
				{
					//IL_0001: Unknown result type (might be due to invalid IL or missing references)
					//IL_0007: Invalid comparison between Unknown and I4
					return (int)wt.Kind == 0;
				});
				if (val5 != null)
				{
					val3 = (ElementType)(object)val5;
					LogInfo("DuplicateFamilyType: 使用基本墙类型 '" + ((Element)val5).Name + "' 作为源类型");
				}
				else
				{
					LogWarning("DuplicateFamilyType: 未找到基本墙类型，使用原始源类型 '" + name + "'");
				}
			}
			ElementType val6 = ((IEnumerable)new FilteredElementCollector(val).OfClass(((object)val3).GetType())).Cast<ElementType>().FirstOrDefault((ElementType et) => ((Element)et).Name.Equals(newTypeName, StringComparison.OrdinalIgnoreCase));
			if (val6 != null)
			{
				LogInfo("DuplicateFamilyType: 类型名称 '" + newTypeName + "' 已存在，返回现有类型");
				return val6;
			}
			Element val7 = null;
			try
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler5.AppendLiteral("DuplicateFamilyType: 尝试复制类型 '");
				defaultInterpolatedStringHandler5.AppendFormatted(((Element)val3).Name);
				defaultInterpolatedStringHandler5.AppendLiteral("' → '");
				defaultInterpolatedStringHandler5.AppendFormatted(newTypeName);
				defaultInterpolatedStringHandler5.AppendLiteral("'");
				LogInfo(defaultInterpolatedStringHandler5.ToStringAndClear());
				val7 = (Element)(object)val3.Duplicate(newTypeName);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(46, 1);
				defaultInterpolatedStringHandler6.AppendLiteral("DuplicateFamilyType: Duplicate() 方法成功，新类型 ID: ");
				defaultInterpolatedStringHandler6.AppendFormatted<ElementId>(val7.Id);
				LogInfo(defaultInterpolatedStringHandler6.ToStringAndClear());
			}
			catch (InvalidOperationException ex)
			{
				InvalidOperationException ex2 = ex;
				LogError("DuplicateFamilyType: 操作无效 - " + ((Exception)(object)ex2).Message);
				LogError("DuplicateFamilyType: 源类型 '" + ((Element)val3).Name + "' 可能不允许复制，请尝试使用其他类型作为模板");
				return null;
			}
			catch (ArgumentException ex3)
			{
				ArgumentException ex4 = ex3;
				LogError("DuplicateFamilyType: 参数错误 - " + ((Exception)(object)ex4).Message);
				LogError("DuplicateFamilyType: 可能原因：新类型名称 '" + newTypeName + "' 包含非法字符");
				return null;
			}
			if (parameterValues != null && parameterValues.Count > 0)
			{
				int num = 0;
				WallType val8 = (WallType)(object)((val7 is WallType) ? val7 : null);
				if (val8 != null && parameterValues.TryGetValue("宽度", out object value))
				{
					try
					{
						if (value is double num2)
						{
							CompoundStructure compoundStructure = ((HostObjAttributes)val8).GetCompoundStructure();
							if (compoundStructure != null)
							{
								double num3 = num2 / 304.8;
								if (compoundStructure.LayerCount > 0)
								{
									compoundStructure.SetLayerWidth(0, num3);
									((HostObjAttributes)val8).SetCompoundStructure(compoundStructure);
									num++;
									DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(59, 1);
									defaultInterpolatedStringHandler7.AppendLiteral("DuplicateFamilyType: 通过 CompoundStructure 设置墙宽度 = ");
									defaultInterpolatedStringHandler7.AppendFormatted(num2);
									defaultInterpolatedStringHandler7.AppendLiteral(" mm (层 0)");
									LogInfo(defaultInterpolatedStringHandler7.ToStringAndClear());
								}
								else
								{
									LogWarning("DuplicateFamilyType: 墙类型没有可用的结构层");
								}
							}
							else
							{
								LogWarning("DuplicateFamilyType: 无法获取墙的 CompoundStructure");
							}
						}
					}
					catch (Exception ex5)
					{
						LogWarning("DuplicateFamilyType: 设置墙宽度失败 - " + ex5.Message + "，将尝试使用普通参数设置");
					}
				}
				foreach (KeyValuePair<string, object> param in parameterValues)
				{
					if (val7 is WallType && param.Key.Equals("宽度", StringComparison.OrdinalIgnoreCase))
					{
						continue;
					}
					try
					{
						Parameter val9 = ((IEnumerable)val7.Parameters).Cast<Parameter>().FirstOrDefault((Parameter p) => p.Definition.Name.Equals(param.Key, StringComparison.OrdinalIgnoreCase));
						if (val9 == null)
						{
							LogWarning("DuplicateFamilyType: 找不到参数 '" + param.Key + "'");
							continue;
						}
						if (((APIObject)val9).IsReadOnly)
						{
							LogWarning("DuplicateFamilyType: 参数 '" + param.Key + "' 是只读的");
							continue;
						}
						bool flag = false;
						if (param.Value is string text)
						{
							flag = _parameterService.SetParameterValue((object)val7, param.Key, text);
						}
						else if (param.Value is int num4)
						{
							flag = _parameterService.SetParameterValue((object)val7, param.Key, num4);
						}
						else if (param.Value is double num5)
						{
							flag = _parameterService.SetParameterValue((object)val7, param.Key, num5);
						}
						else
						{
							string text2 = param.Value?.ToString();
							if (!string.IsNullOrEmpty(text2))
							{
								flag = _parameterService.SetParameterValue((object)val7, param.Key, text2);
							}
						}
						if (flag)
						{
							num++;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(31, 2);
							defaultInterpolatedStringHandler8.AppendLiteral("DuplicateFamilyType: 设置参数 '");
							defaultInterpolatedStringHandler8.AppendFormatted(param.Key);
							defaultInterpolatedStringHandler8.AppendLiteral("' = ");
							defaultInterpolatedStringHandler8.AppendFormatted<object>(param.Value);
							LogInfo(defaultInterpolatedStringHandler8.ToStringAndClear());
						}
						else
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(34, 2);
							defaultInterpolatedStringHandler9.AppendLiteral("DuplicateFamilyType: 设置参数 '");
							defaultInterpolatedStringHandler9.AppendFormatted(param.Key);
							defaultInterpolatedStringHandler9.AppendLiteral("' = ");
							defaultInterpolatedStringHandler9.AppendFormatted<object>(param.Value);
							defaultInterpolatedStringHandler9.AppendLiteral(" 失败");
							LogWarning(defaultInterpolatedStringHandler9.ToStringAndClear());
						}
					}
					catch (Exception ex6)
					{
						LogWarning("DuplicateFamilyType: 设置参数 '" + param.Key + "' 失败 - " + ex6.Message);
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(31, 2);
				defaultInterpolatedStringHandler10.AppendLiteral("DuplicateFamilyType: 成功修改 ");
				defaultInterpolatedStringHandler10.AppendFormatted(num);
				defaultInterpolatedStringHandler10.AppendLiteral("/");
				defaultInterpolatedStringHandler10.AppendFormatted(parameterValues.Count);
				defaultInterpolatedStringHandler10.AppendLiteral(" 个参数");
				LogInfo(defaultInterpolatedStringHandler10.ToStringAndClear());
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(35, 2);
			defaultInterpolatedStringHandler11.AppendLiteral("DuplicateFamilyType: 成功复制类型 '");
			defaultInterpolatedStringHandler11.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler11.AppendLiteral("' → '");
			defaultInterpolatedStringHandler11.AppendFormatted(newTypeName);
			defaultInterpolatedStringHandler11.AppendLiteral("'");
			LogInfo(defaultInterpolatedStringHandler11.ToStringAndClear());
			return val7;
		}
		catch (InvalidOperationException ex7)
		{
			InvalidOperationException ex8 = ex7;
			LogError("DuplicateFamilyType: 操作无效 - " + ((Exception)(object)ex8).Message);
			return null;
		}
		catch (Exception ex9)
		{
			LogError("DuplicateFamilyType 失败: " + ex9.Message);
			return null;
		}
	}

	private static void LogWarning(string message)
	{
		Logger.Warning("[FamilyService] " + message);
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[FamilyService] " + message);
	}

	public IEnumerable<object> GetAllSystemElementTypes(object document)
	{
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_034d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0213: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Expected O, but got Unknown
		//IL_0418: Unknown result type (might be due to invalid IL or missing references)
		//IL_041f: Expected O, but got Unknown
		//IL_037b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Expected O, but got Unknown
		//IL_02de: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e5: Expected O, but got Unknown
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Expected O, but got Unknown
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ab: Expected O, but got Unknown
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_009d: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetAllSystemElementTypes: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
			try
			{
				IList<Element> list2 = new FilteredElementCollector(val).OfClass(typeof(WallType)).ToElements();
				using IEnumerator<Element> enumerator = list2.GetEnumerator();
				int gparam_;
				string name;
				object obj;
				WallType val2;
				for (; enumerator.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_, name, (string)obj, ((object)val2.Kind/*cast due to constrained. prefix*/).ToString())))
				{
					val2 = (WallType)enumerator.Current;
					gparam_ = (int)((Element)val2).Id.Value;
					name = ((Element)val2).Name;
					Category category = ((Element)val2).Category;
					if (category == null)
					{
						obj = null;
					}
					else
					{
						obj = category.Name;
						if (obj != null)
						{
							continue;
						}
					}
					obj = "墙";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list3 = new FilteredElementCollector(val).OfClass(typeof(FloorType)).ToElements();
				using IEnumerator<Element> enumerator2 = list3.GetEnumerator();
				int gparam_2;
				string name2;
				object obj3;
				for (; enumerator2.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_2, name2, (string)obj3, "Floor")))
				{
					FloorType val3 = (FloorType)enumerator2.Current;
					gparam_2 = (int)((Element)val3).Id.Value;
					name2 = ((Element)val3).Name;
					Category category2 = ((Element)val3).Category;
					if (category2 == null)
					{
						obj3 = null;
					}
					else
					{
						obj3 = category2.Name;
						if (obj3 != null)
						{
							continue;
						}
					}
					obj3 = "楼板";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list4 = new FilteredElementCollector(val).OfClass(typeof(RoofType)).ToElements();
				using IEnumerator<Element> enumerator3 = list4.GetEnumerator();
				int gparam_3;
				string name3;
				object obj5;
				for (; enumerator3.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_3, name3, (string)obj5, "Roof")))
				{
					RoofType val4 = (RoofType)enumerator3.Current;
					gparam_3 = (int)((Element)val4).Id.Value;
					name3 = ((Element)val4).Name;
					Category category3 = ((Element)val4).Category;
					if (category3 == null)
					{
						obj5 = null;
					}
					else
					{
						obj5 = category3.Name;
						if (obj5 != null)
						{
							continue;
						}
					}
					obj5 = "屋顶";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list5 = new FilteredElementCollector(val).OfClass(typeof(LevelType)).ToElements();
				using IEnumerator<Element> enumerator4 = list5.GetEnumerator();
				int gparam_4;
				string name4;
				object obj7;
				for (; enumerator4.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_4, name4, (string)obj7, "Level")))
				{
					LevelType val5 = (LevelType)enumerator4.Current;
					gparam_4 = (int)((Element)val5).Id.Value;
					name4 = ((Element)val5).Name;
					Category category4 = ((Element)val5).Category;
					if (category4 == null)
					{
						obj7 = null;
					}
					else
					{
						obj7 = category4.Name;
						if (obj7 != null)
						{
							continue;
						}
					}
					obj7 = "标高";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list6 = new FilteredElementCollector(val).OfClass(typeof(CeilingType)).ToElements();
				using IEnumerator<Element> enumerator5 = list6.GetEnumerator();
				int gparam_5;
				string name5;
				object obj9;
				for (; enumerator5.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_5, name5, (string)obj9, "Ceiling")))
				{
					CeilingType val6 = (CeilingType)enumerator5.Current;
					gparam_5 = (int)((Element)val6).Id.Value;
					name5 = ((Element)val6).Name;
					Category category5 = ((Element)val6).Category;
					if (category5 == null)
					{
						obj9 = null;
					}
					else
					{
						obj9 = category5.Name;
						if (obj9 != null)
						{
							continue;
						}
					}
					obj9 = "天花板";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list7 = new FilteredElementCollector(val).OfClass(typeof(GridType)).ToElements();
				using IEnumerator<Element> enumerator6 = list7.GetEnumerator();
				int gparam_6;
				string name6;
				object obj11;
				for (; enumerator6.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_6, name6, (string)obj11, "Grid")))
				{
					GridType val7 = (GridType)enumerator6.Current;
					gparam_6 = (int)((Element)val7).Id.Value;
					name6 = ((Element)val7).Name;
					Category category6 = ((Element)val7).Category;
					if (category6 == null)
					{
						obj11 = null;
					}
					else
					{
						obj11 = category6.Name;
						if (obj11 != null)
						{
							continue;
						}
					}
					obj11 = "网格";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list8 = new FilteredElementCollector(val).OfClass(typeof(DimensionType)).ToElements();
				using IEnumerator<Element> enumerator7 = list8.GetEnumerator();
				int gparam_7;
				string name7;
				object obj13;
				for (; enumerator7.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_7, name7, (string)obj13, "Dimension")))
				{
					DimensionType val8 = (DimensionType)enumerator7.Current;
					gparam_7 = (int)((Element)val8).Id.Value;
					name7 = ((Element)val8).Name;
					Category category7 = ((Element)val8).Category;
					if (category7 == null)
					{
						obj13 = null;
					}
					else
					{
						obj13 = category7.Name;
						if (obj13 != null)
						{
							continue;
						}
					}
					obj13 = "尺寸标注";
				}
			}
			catch
			{
			}
			try
			{
				IList<Element> list9 = new FilteredElementCollector(val).OfClass(typeof(TextNoteType)).ToElements();
				using IEnumerator<Element> enumerator8 = list9.GetEnumerator();
				int gparam_8;
				string name8;
				object obj15;
				for (; enumerator8.MoveNext(); list.Add(new Class327<int, string, string, string>(gparam_8, name8, (string)obj15, "TextNote")))
				{
					TextNoteType val9 = (TextNoteType)enumerator8.Current;
					gparam_8 = (int)((Element)val9).Id.Value;
					name8 = ((Element)val9).Name;
					Category category8 = ((Element)val9).Category;
					if (category8 == null)
					{
						obj15 = null;
					}
					else
					{
						obj15 = category8.Name;
						if (obj15 != null)
						{
							continue;
						}
					}
					obj15 = "文字注释";
				}
			}
			catch
			{
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 1);
			defaultInterpolatedStringHandler.AppendLiteral("GetAllSystemElementTypes: 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个系统族类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllSystemElementTypes 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetTypesByCategory(object document, string categoryName)
	{
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetTypesByCategory: document 不是 Document 类型");
				return Enumerable.Empty<object>();
			}
			List<object> list = new List<object>();
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
				LogError("GetTypesByCategory: 类别不存在: " + categoryName);
				return Enumerable.Empty<object>();
			}
			BuiltInCategory val4 = (BuiltInCategory)val2.Id.Value;
			FilteredElementCollector val5 = new FilteredElementCollector(val).OfCategory(val4).WhereElementIsElementType();
			foreach (Element item2 in val5)
			{
				ElementType val6 = (ElementType)(object)((item2 is ElementType) ? item2 : null);
				if (val6 == null)
				{
					continue;
				}
				int gparam_ = (int)item2.Id.Value;
				string name = item2.Name;
				Category category = item2.Category;
				object obj;
				if (category == null)
				{
					obj = null;
				}
				else
				{
					obj = category.Name;
					if (obj != null)
					{
						goto IL_0124;
					}
				}
				obj = categoryName;
				goto IL_0124;
				IL_0124:
				list.Add(new Class328<int, string, string>(gparam_, name, (string)obj));
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
			defaultInterpolatedStringHandler.AppendLiteral("GetTypesByCategory: 类别 '");
			defaultInterpolatedStringHandler.AppendFormatted(categoryName);
			defaultInterpolatedStringHandler.AppendLiteral("' 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类型");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetTypesByCategory 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public bool FamilyExists(object document, string familyName)
	{
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("FamilyExists: document 不是 Document 类型");
				return false;
			}
			if (string.IsNullOrWhiteSpace(familyName))
			{
				LogError("FamilyExists: 族名称不能为空");
				return false;
			}
			Family val2 = ((IEnumerable)new FilteredElementCollector(val).OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family f) => ((Element)f).Name.Equals(familyName, StringComparison.OrdinalIgnoreCase));
			bool flag = val2 != null;
			LogInfo("FamilyExists: 族 '" + familyName + "' " + (flag ? "存在" : "不存在"));
			return flag;
		}
		catch (Exception ex)
		{
			LogError("FamilyExists 失败: " + ex.Message);
			return false;
		}
	}
}
