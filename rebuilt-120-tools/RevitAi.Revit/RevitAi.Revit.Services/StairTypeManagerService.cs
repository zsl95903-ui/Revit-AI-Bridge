using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ns6;

namespace RevitAi.Revit.Services;

public class StairTypeManagerService : IStairTypeManagerService
{
	private readonly Document _document;

	public StairTypeManagerService(Document document)
	{
		_document = document ?? throw new ArgumentNullException("document");
		Logger.Info("[楼梯类型管理] 初始化楼梯类型管理服务");
	}

	public StairsType? FindCastInPlaceStairType()
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		Logger.Info("[楼梯类型管理] 开始查找现浇楼梯类型");
		List<StairsType> list = ((IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(StairsType))).Cast<StairsType>().ToList();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[楼梯类型管理] 找到 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个楼梯类型");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		StairsType val = list.FirstOrDefault((StairsType st) => IsCastInPlaceStairType(st));
		StairsType val2 = val ?? list.FirstOrDefault();
		if (val2 != null)
		{
			Logger.Info("[楼梯类型管理] 选择楼梯类型: " + ((Element)val2).Name);
		}
		else
		{
			Logger.Warning("[楼梯类型管理] 未找到任何楼梯类型");
		}
		return val2;
	}

	public StairsType CreateCustomStairType(string baseStairTypeName, string newStairTypeName, double runThicknessMm, double landingThicknessMm)
	{
		Logger.Info("[楼梯类型管理] ========== 开始创建自定义楼梯类型 ==========");
		Logger.Info("[楼梯类型管理] 源类型: " + baseStairTypeName);
		Logger.Info("[楼梯类型管理] 新类型名: " + newStairTypeName);
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(19, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[楼梯类型管理] 梯段结构深度: ");
		defaultInterpolatedStringHandler.AppendFormatted(runThicknessMm);
		defaultInterpolatedStringHandler.AppendLiteral("mm");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(19, 1);
		defaultInterpolatedStringHandler2.AppendLiteral("[楼梯类型管理] 平台整体厚度: ");
		defaultInterpolatedStringHandler2.AppendFormatted(landingThicknessMm);
		defaultInterpolatedStringHandler2.AppendLiteral("mm");
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		try
		{
			Logger.Info("[楼梯类型管理] 步骤1: 查找基础楼梯类型");
			StairsType val = FindStairTypeByName(baseStairTypeName);
			if (val == null)
			{
				Logger.Error("[楼梯类型管理] 找不到楼梯类型: " + baseStairTypeName);
				throw new InvalidOperationException("找不到楼梯类型: " + baseStairTypeName);
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler3.AppendLiteral("[楼梯类型管理] 找到基础楼梯类型: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val).Name);
			defaultInterpolatedStringHandler3.AppendLiteral(" (ID: ");
			defaultInterpolatedStringHandler3.AppendFormatted(((Element)val).Id.Value);
			defaultInterpolatedStringHandler3.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			Logger.Info("[楼梯类型管理] 步骤2: 复制楼梯类型");
			ElementType obj = ((ElementType)val).Duplicate(newStairTypeName);
			StairsType val2 = (StairsType)(object)((obj is StairsType) ? obj : null);
			if (val2 == null)
			{
				Logger.Error("[楼梯类型管理] 复制楼梯类型失败，返回null");
				throw new InvalidOperationException("复制楼梯类型失败");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("[楼梯类型管理] 成功复制楼梯类型: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler4.AppendLiteral(" (ID: ");
			defaultInterpolatedStringHandler4.AppendFormatted(((Element)val2).Id.Value);
			defaultInterpolatedStringHandler4.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			Logger.Info("[楼梯类型管理] 步骤3: 查找整体梯段和整体平台类型");
			List<ElementType> list = FindMonolithicRunTypes();
			List<ElementType> list2 = FindMonolithicLandingTypes();
			string text = string.Empty;
			string text2 = string.Empty;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler5.AppendLiteral("[楼梯类型管理] 找到 ");
			defaultInterpolatedStringHandler5.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler5.AppendLiteral(" 个整体梯段类型");
			Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(20, 1);
			defaultInterpolatedStringHandler6.AppendLiteral("[楼梯类型管理] 找到 ");
			defaultInterpolatedStringHandler6.AppendFormatted(list2.Count);
			defaultInterpolatedStringHandler6.AppendLiteral(" 个整体平台类型");
			Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
			if (list.Count > 0)
			{
				Logger.Info("[楼梯类型管理] 步骤4: 创建自定义整体梯段类型");
				text = newStairTypeName + "_整体梯段";
				ElementType val3 = list.FirstOrDefault((ElementType rt) => ((Element)rt).Name.Contains("150") && ((Element)rt).Name.Contains("结构深度")) ?? list[0];
				Logger.Info("[楼梯类型管理] 源梯段: " + ((Element)val3).Name);
				Logger.Info("[楼梯类型管理] 新梯段名: " + text);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(19, 1);
				defaultInterpolatedStringHandler7.AppendLiteral("[楼梯类型管理] 设置结构深度: ");
				defaultInterpolatedStringHandler7.AppendFormatted(runThicknessMm);
				defaultInterpolatedStringHandler7.AppendLiteral("mm");
				Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
				ElementType val4 = FindElementTypeByName(text);
				if (val4 != null)
				{
					Logger.Info("[楼梯类型管理] 梯段类型已存在，直接使用: " + ((Element)val4).Name);
				}
				else
				{
					try
					{
						ElementType val5 = CreateCustomRunType(val3, text, runThicknessMm);
						if (val5 != null)
						{
							Logger.Info("[楼梯类型管理] 成功创建梯段类型: " + ((Element)val5).Name);
						}
					}
					catch (Exception ex)
					{
						Logger.Error("[楼梯类型管理] 创建梯段类型失败", ex);
					}
				}
			}
			else
			{
				Logger.Warning("[楼梯类型管理] 跳过梯段类型创建：未找到整体梯段类型");
			}
			if (list2.Count > 0)
			{
				Logger.Info("[楼梯类型管理] 步骤5: 创建自定义整体平台类型");
				text2 = newStairTypeName + "_整体平台";
				ElementType val6 = list2.FirstOrDefault((ElementType lt) => ((Element)lt).Name.Contains("300") && ((Element)lt).Name.Contains("厚度")) ?? list2[0];
				Logger.Info("[楼梯类型管理] 源平台: " + ((Element)val6).Name);
				Logger.Info("[楼梯类型管理] 新平台名: " + text2);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(19, 1);
				defaultInterpolatedStringHandler8.AppendLiteral("[楼梯类型管理] 设置整体厚度: ");
				defaultInterpolatedStringHandler8.AppendFormatted(landingThicknessMm);
				defaultInterpolatedStringHandler8.AppendLiteral("mm");
				Logger.Info(defaultInterpolatedStringHandler8.ToStringAndClear());
				ElementType val7 = FindElementTypeByName(text2);
				if (val7 != null)
				{
					Logger.Info("[楼梯类型管理] 平台类型已存在，直接使用: " + ((Element)val7).Name);
				}
				else
				{
					try
					{
						ElementType val8 = CreateCustomLandingType(val6, text2, landingThicknessMm);
						if (val8 != null)
						{
							Logger.Info("[楼梯类型管理] 成功创建平台类型: " + ((Element)val8).Name);
						}
					}
					catch (Exception ex2)
					{
						Logger.Error("[楼梯类型管理] 创建平台类型失败", ex2);
					}
				}
			}
			else
			{
				Logger.Warning("[楼梯类型管理] 跳过平台类型创建：未找到整体平台类型");
			}
			Logger.Info("[楼梯类型管理] 步骤6: 关联梯段和平台类型到楼梯类型");
			ElementType val9 = FindElementTypeByName(text);
			ElementType val10 = FindElementTypeByName(text2);
			if (val9 != null)
			{
				Logger.Info("[楼梯类型管理] 尝试设置梯段类型: " + ((Element)val9).Name);
				bool flag = SetStairRunType(val2, ((Element)val9).Id);
				Logger.Info("[楼梯类型管理] 梯段类型设置" + (flag ? "成功" : "失败"));
			}
			if (val10 != null)
			{
				Logger.Info("[楼梯类型管理] 尝试设置平台类型: " + ((Element)val10).Name);
				bool flag2 = SetStairLandingType(val2, ((Element)val10).Id);
				Logger.Info("[楼梯类型管理] 平台类型设置" + (flag2 ? "成功" : "失败"));
			}
			Logger.Info("[楼梯类型管理] ========== 楼梯类型创建完成 ==========");
			return val2;
		}
		catch (Exception ex3)
		{
			Logger.Error("[楼梯类型管理] 创建自定义楼梯类型失败", ex3);
			throw;
		}
	}

	public IList<string> GetStairTypeNames(bool filterCastInPlace = true)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		IEnumerable<StairsType> source = ((IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(StairsType))).Cast<StairsType>();
		if (filterCastInPlace)
		{
			source = source.Where((StairsType st) => IsCastInPlaceStairType(st));
		}
		return source.Select((StairsType st) => ((Element)st).Name).ToList();
	}

	public StairsType? FindStairTypeByName(string typeName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(StairsType))).Cast<StairsType>().FirstOrDefault((StairsType st) => ((Element)st).Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
	}

	private List<ElementType> FindMonolithicRunTypes()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<ElementType> source = (from ElementType et in (IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(ElementType))
			where ((Element)et).Category != null
			select et).ToList();
		return source.Where((ElementType et) => ((Element)et).Category != null && ((Element)et).Category.Id.Value == -2000919L && IsMonolithicFamily(et)).ToList();
	}

	private List<ElementType> FindMonolithicLandingTypes()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		List<ElementType> source = (from ElementType et in (IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(ElementType))
			where ((Element)et).Category != null
			select et).ToList();
		return source.Where((ElementType et) => ((Element)et).Category != null && ((Element)et).Category.Id.Value == -2000920L && IsMonolithicFamily(et)).ToList();
	}

	private bool IsMonolithicFamily(ElementType elementType)
	{
		if (elementType == null)
		{
			return false;
		}
		string familyName = elementType.FamilyName;
		if (string.IsNullOrEmpty(familyName))
		{
			return false;
		}
		if (familyName.Contains("非整体"))
		{
			return false;
		}
		return familyName.Contains("整体") || familyName.Contains("Monolithic", StringComparison.OrdinalIgnoreCase);
	}

	private bool IsCastInPlaceStairType(StairsType stairsType)
	{
		if (stairsType == null)
		{
			return false;
		}
		string name = ((Element)stairsType).Name;
		string[] array = new string[5]
		{
			"现浇",
			"现场浇筑",
			"浇筑",
			"现场",
			"混凝土"
		};
		string[] array2 = array;
		int num = 0;
		while (true)
		{
			if (num < array2.Length)
			{
				string value = array2[num];
				if (name.Contains(value))
				{
					break;
				}
				num++;
				continue;
			}
			string[] array3 = new string[5]
			{
				"Cast",
				"In-Place",
				"In Place",
				"CIP",
				"CastInPlace"
			};
			string[] array4 = array3;
			int num2 = 0;
			while (true)
			{
				if (num2 < array4.Length)
				{
					string value2 = array4[num2];
					if (name.IndexOf(value2, StringComparison.OrdinalIgnoreCase) >= 0)
					{
						break;
					}
					num2++;
					continue;
				}
				return false;
			}
			return true;
		}
		return true;
	}

	private ElementType CreateCustomRunType(ElementType sourceRunType, string newTypeName, double thicknessMm)
	{
		ElementType val = sourceRunType.Duplicate(newTypeName);
		if (val == null)
		{
			throw new InvalidOperationException("复制梯段类型失败");
		}
		SetParameterInMm((Element)(object)val, thicknessMm, "结构深度", "Structural Depth");
		return val;
	}

	private ElementType CreateCustomLandingType(ElementType sourceLandingType, string newTypeName, double thicknessMm)
	{
		ElementType val = sourceLandingType.Duplicate(newTypeName);
		if (val == null)
		{
			throw new InvalidOperationException("复制平台类型失败");
		}
		SetParameterInMm((Element)(object)val, thicknessMm, "整体厚度", "Monolithic Thickness");
		return val;
	}

	private bool SetParameterInMm(Element element, double valueMm, params string[] paramNames)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Invalid comparison between Unknown and I4
		double num = valueMm / 304.8;
		foreach (string text in paramNames)
		{
			Parameter val = element.LookupParameter(text);
			if (val != null && (int)val.StorageType == 2)
			{
				try
				{
					val.Set(num);
					return true;
				}
				catch (Exception ex)
				{
					Logger.Warning("设置参数 " + text + " 失败: " + ex.Message);
				}
			}
		}
		return false;
	}

	private ElementType? FindElementTypeByName(string typeName)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		return ((IEnumerable)new FilteredElementCollector(_document).OfClass(typeof(ElementType))).Cast<ElementType>().FirstOrDefault((ElementType et) => ((Element)et).Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
	}

	private bool SetStairRunType(StairsType stairsType, ElementId runTypeId)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		try
		{
			string[] array = new string[5]
			{
				"梯段类型",
				"Run Type",
				"RunTypeId",
				"楼梯梯段类型",
				"Stair Run Type"
			};
			string[] array2 = array;
			int num = 0;
			Parameter val;
			while (true)
			{
				if (num < array2.Length)
				{
					string text = array2[num];
					val = ((Element)stairsType).LookupParameter(text);
					if (val != null && (int)val.StorageType == 4)
					{
						break;
					}
					num++;
					continue;
				}
				Logger.Warning("[楼梯类型管理] 未找到梯段类型参数");
				return false;
			}
			val.Set(runTypeId);
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("[楼梯类型管理] 设置梯段类型失败", ex);
			return false;
		}
	}

	private bool SetStairLandingType(StairsType stairsType, ElementId landingTypeId)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Invalid comparison between Unknown and I4
		try
		{
			string[] array = new string[5]
			{
				"平台类型",
				"Landing Type",
				"LandingTypeId",
				"楼梯平台类型",
				"Stair Landing Type"
			};
			string[] array2 = array;
			int num = 0;
			Parameter val;
			while (true)
			{
				if (num < array2.Length)
				{
					string text = array2[num];
					val = ((Element)stairsType).LookupParameter(text);
					if (val != null && (int)val.StorageType == 4)
					{
						break;
					}
					num++;
					continue;
				}
				Logger.Warning("[楼梯类型管理] 未找到平台类型参数");
				return false;
			}
			val.Set(landingTypeId);
			return true;
		}
		catch (Exception ex)
		{
			Logger.Error("[楼梯类型管理] 设置平台类型失败", ex);
			return false;
		}
	}
}
