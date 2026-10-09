using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Core;
using Autodesk.Revit.DB;
using ns0;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class RevitCADFileService : ICADFileService
{
	public CADFileInfo? GetCADFilePath(object importInstance, object document)
	{
		//IL_0582: Unknown result type (might be due to invalid IL or missing references)
		//IL_0587: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_05aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_05c3: Expected O, but got Unknown
		//IL_049d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Expected O, but got Unknown
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_0140: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Invalid comparison between Unknown and I4
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Invalid comparison between Unknown and I4
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0301: Invalid comparison between Unknown and I4
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				return null;
			}
			try
			{
				ElementId typeId = ((Element)val).GetTypeId();
				if (typeId != (ElementId)null && typeId != ElementId.InvalidElementId)
				{
					Element element = val2.GetElement(typeId);
					CADLinkType val3 = (CADLinkType)(object)((element is CADLinkType) ? element : null);
					if (val3 != null)
					{
						ExternalFileReference externalFileReference = ((Element)val3).GetExternalFileReference();
						if (externalFileReference != null)
						{
							ModelPath absolutePath = externalFileReference.GetAbsolutePath();
							string text = ModelPathUtils.ConvertModelPathToUserVisiblePath(absolutePath);
							if (!string.IsNullOrEmpty(text))
							{
								Logger.Info("[RevitCADFileService] 从 CADLinkType 获取路径: " + text);
								if (File.Exists(text))
								{
									string text2 = "未知";
									double num = 1.0;
									try
									{
										Parameter val4 = null;
										string[] array = new string[3]
										{
											"导入单位",
											"Import Units",
											"Unit"
										};
										string[] array2 = array;
										foreach (string text3 in array2)
										{
											foreach (Parameter parameter in ((Element)val3).Parameters)
											{
												Parameter val5 = parameter;
												if (val5.Definition.Name == text3)
												{
													val4 = val5;
													Logger.Info("[RevitCADFileService] 找到导入单位参数: " + text3);
													break;
												}
											}
											if (val4 != null)
											{
												break;
											}
										}
										if (val4 == null)
										{
											string text4 = string.Join(", ", from Parameter p in (IEnumerable)((Element)val3).Parameters
												select p.Definition.Name);
											Logger.Warning("[RevitCADFileService] 未找到导入单位参数，可用参数: " + text4);
										}
										if (val4 != null && val4.HasValue)
										{
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(32, 1);
											defaultInterpolatedStringHandler.AppendLiteral("[RevitCADFileService] 导入单位参数类型: ");
											defaultInterpolatedStringHandler.AppendFormatted<StorageType>(val4.StorageType);
											Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
											if ((int)val4.StorageType == 1)
											{
												int num2 = val4.AsInteger();
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(33, 1);
												defaultInterpolatedStringHandler2.AppendLiteral("[RevitCADFileService] 导入单位参数整数值: ");
												defaultInterpolatedStringHandler2.AppendFormatted(num2);
												Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
												text2 = GetImportUnitName(num2);
												num = GetConversionFactorToMM(num2);
											}
											else if ((int)val4.StorageType == 3)
											{
												string text5 = val4.AsString();
												Logger.Info("[RevitCADFileService] 导入单位参数字符串值: " + text5);
												text2 = text5 ?? "";
												num = ParseChineseUnitToFactor(text5);
											}
											else if ((int)val4.StorageType == 2)
											{
												double value = val4.AsDouble();
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(34, 1);
												defaultInterpolatedStringHandler3.AppendLiteral("[RevitCADFileService] 导入单位参数双精度值: ");
												defaultInterpolatedStringHandler3.AppendFormatted(value);
												Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(4, 1);
												defaultInterpolatedStringHandler4.AppendLiteral("数值(");
												defaultInterpolatedStringHandler4.AppendFormatted(value);
												defaultInterpolatedStringHandler4.AppendLiteral(")");
												text2 = defaultInterpolatedStringHandler4.ToStringAndClear();
											}
											else
											{
												DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(35, 1);
												defaultInterpolatedStringHandler5.AppendLiteral("[RevitCADFileService] 导入单位参数类型不支持: ");
												defaultInterpolatedStringHandler5.AppendFormatted<StorageType>(val4.StorageType);
												Logger.Warning(defaultInterpolatedStringHandler5.ToStringAndClear());
											}
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(40, 2);
											defaultInterpolatedStringHandler6.AppendLiteral("[RevitCADFileService] CAD 导入单位: ");
											defaultInterpolatedStringHandler6.AppendFormatted(text2);
											defaultInterpolatedStringHandler6.AppendLiteral(" (转换系数=");
											defaultInterpolatedStringHandler6.AppendFormatted(num);
											defaultInterpolatedStringHandler6.AppendLiteral(")");
											Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
										}
										else
										{
											Logger.Warning("[RevitCADFileService] 无法读取导入单位参数，使用默认值（毫米）");
											text2 = "毫米";
											num = 1.0;
										}
									}
									catch (Exception ex)
									{
										Logger.Warning("[RevitCADFileService] 读取导入单位失败: " + ex.Message + "，使用默认值（毫米）");
										text2 = "毫米";
										num = 1.0;
									}
									double num3 = 1.0;
									try
									{
										Parameter val6 = null;
										foreach (Parameter parameter2 in ((Element)val3).Parameters)
										{
											Parameter val7 = parameter2;
											if (val7.Definition.Name == "比例系数")
											{
												val6 = val7;
												break;
											}
										}
										if (val6 != null && val6.HasValue)
										{
											num3 = val6.AsDouble();
											DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(32, 1);
											defaultInterpolatedStringHandler7.AppendLiteral("[RevitCADFileService] CAD 比例系数: ");
											defaultInterpolatedStringHandler7.AppendFormatted(num3);
											Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
										}
										else
										{
											Logger.Warning("[RevitCADFileService] 无法读取比例系数参数，使用默认值（1.0）");
											num3 = 1.0;
										}
									}
									catch (Exception ex2)
									{
										Logger.Warning("[RevitCADFileService] 读取比例系数失败: " + ex2.Message + "，使用默认值（1.0）");
										num3 = 1.0;
									}
									return new CADFileInfo
									{
										FilePath = text,
										ImportInstanceId = ((int)((Element)val).Id.Value).ToString(),
										ImportUnit = text2,
										ConversionFactorToMM = num,
										ScaleFactor = num3
									};
								}
								Logger.Warning("[RevitCADFileService] 文件不存在: " + text);
							}
						}
					}
					else
					{
						Logger.Debug("[RevitCADFileService] 元素类型 " + ((object)element)?.GetType().Name + " 不是 CADLinkType");
					}
				}
			}
			catch (Exception ex3)
			{
				Logger.Debug("[RevitCADFileService] 从 CADLinkType 获取失败: " + ex3.Message);
			}
			Logger.Warning("[RevitCADFileService] 无法从 ImportInstance 获取 CAD 文件路径");
			return null;
		}
		catch (Exception ex4)
		{
			Logger.Error("[RevitCADFileService] GetCADFilePath 失败: " + ex4.Message);
			return null;
		}
	}

	private string GetImportUnitName(int unitValue)
	{
		switch (unitValue)
		{
		case 0:
			return "米";
		case 1:
			return "厘米";
		case 2:
			return "毫米";
		case 3:
			return "英尺";
		default:
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 1);
			defaultInterpolatedStringHandler.AppendLiteral("未知(");
			defaultInterpolatedStringHandler.AppendFormatted(unitValue);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
		case 6:
			return "英寸";
		}
	}

	private double GetConversionFactorToMM(int unitValue)
	{
		return unitValue switch
		{
			0 => 1000.0, 
			1 => 10.0, 
			2 => 1.0, 
			3 => 304.8, 
			6 => 25.4, 
			_ => 1.0, 
		};
	}

	private double ParseChineseUnitToFactor(string? unitStr)
	{
		if (string.IsNullOrEmpty(unitStr))
		{
			return 1.0;
		}
		string text = unitStr.Trim().ToLower();
		uint num = Class653.smethod_0(text);
		if (num <= 1613635087)
		{
			if (num <= 842557787)
			{
				if (num <= 425467148)
				{
					if (num <= 337141308)
					{
						if (num != 30197303)
						{
							if (num == 337141308 && text == "millimeters")
							{
								goto IL_0370;
							}
						}
						else if (text == "decimeter")
						{
							goto IL_04b3;
						}
					}
					else if (num != 381738493)
					{
						if (num == 425467148 && text == "decimeters")
						{
							goto IL_04b3;
						}
					}
					else if (text == "inch")
					{
						goto IL_03b1;
					}
				}
				else if (num <= 542584942)
				{
					if (num != 445638213)
					{
						if (num == 542584942 && text == "custom")
						{
							goto IL_0256;
						}
					}
					else if (text == "公里")
					{
						goto IL_050e;
					}
				}
				else if (num != 760240793)
				{
					if (num == 842557787 && text == "毫米")
					{
						goto IL_0370;
					}
				}
				else if (text == "foot")
				{
					goto IL_0404;
				}
			}
			else if (num <= 1222947139)
			{
				if (num <= 1094220446)
				{
					if (num != 869228051)
					{
						if (num == 1094220446 && text == "in")
						{
							goto IL_03b1;
						}
					}
					else if (text == "centimeter")
					{
						goto IL_05a5;
					}
				}
				else if (num != 1144847493)
				{
					if (num == 1222947139 && text == "meters")
					{
						goto IL_057b;
					}
				}
				else if (text == "km")
				{
					goto IL_050e;
				}
			}
			else if (num <= 1502369796)
			{
				if (num != 1495456279)
				{
					if (num == 1502369796 && text == "自定义")
					{
						goto IL_0256;
					}
				}
				else if (text == "ft")
				{
					goto IL_0404;
				}
			}
			else if (num != 1544048658)
			{
				if (num == 1613635087 && text == "mm")
				{
					goto IL_0370;
				}
			}
			else if (text == "英尺")
			{
				goto IL_0404;
			}
		}
		else if (num <= 3140845397u)
		{
			if (num <= 2095398625)
			{
				if (num <= 1680451373)
				{
					if (num != 1679612730)
					{
						if (num == 1680451373 && text == "cm")
						{
							goto IL_05a5;
						}
					}
					else if (text == "dm")
					{
						goto IL_04b3;
					}
				}
				else if (num != 2086207015)
				{
					if (num == 2095398625 && text == "inches")
					{
						goto IL_03b1;
					}
				}
				else if (text == "millimeter")
				{
					goto IL_0370;
				}
			}
			else if (num <= 2584467372u)
			{
				if (num != 2470140894u)
				{
					if (num == 2584467372u && text == "英寸")
					{
						goto IL_03b1;
					}
				}
				else if (text == "default")
				{
					goto IL_055d;
				}
			}
			else if (num != 2689594086u)
			{
				if (num == 3140845397u && text == "feet")
				{
					goto IL_0404;
				}
			}
			else if (text == "kilometers")
			{
				goto IL_050e;
			}
		}
		else if (num <= 3539395134u)
		{
			if (num <= 3311476706u)
			{
				if (num != 3216173843u)
				{
					if (num == 3311476706u && text == "meter")
					{
						goto IL_057b;
					}
				}
				else if (text == "千米")
				{
					goto IL_050e;
				}
			}
			else if (num != 3330539388u)
			{
				if (num == 3539395134u && text == "分米")
				{
					goto IL_04b3;
				}
			}
			else if (text == "厘米")
			{
				goto IL_05a5;
			}
		}
		else if (num <= 4011141425u)
		{
			if (num != 3893112696u)
			{
				if (num == 4011141425u && text == "kilometer")
				{
					goto IL_050e;
				}
			}
			else if (text == "m")
			{
				goto IL_057b;
			}
		}
		else if (num != 4017197344u)
		{
			if (num != 4135014786u)
			{
				if (num == 4252212153u && text == "默认")
				{
					goto IL_055d;
				}
			}
			else if (text == "米")
			{
				goto IL_057b;
			}
		}
		else if (text == "centimeters")
		{
			goto IL_05a5;
		}
		double result = 1.0;
		goto IL_05af;
		IL_04b3:
		result = 100.0;
		goto IL_05af;
		IL_0404:
		result = 304.8;
		goto IL_05af;
		IL_03b1:
		result = 25.4;
		goto IL_05af;
		IL_050e:
		result = 1000000.0;
		goto IL_05af;
		IL_055d:
		result = 1.0;
		goto IL_05af;
		IL_0370:
		result = 1.0;
		goto IL_05af;
		IL_05a5:
		result = 10.0;
		goto IL_05af;
		IL_05af:
		return result;
		IL_0256:
		result = 1.0;
		goto IL_05af;
		IL_057b:
		result = 1000.0;
		goto IL_05af;
	}

	public CADFileData? ParseCADFile(string filePath, double conversionFactorToMM)
	{
		try
		{
			ICADFileService val = CoreServicesFactory.CreateCADFileService();
			return val.ParseCADFile(filePath, conversionFactorToMM);
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitCADFileService] ParseCADFile 失败: " + ex.Message);
			return null;
		}
	}

	public CADFileData? ParseImportInstance(object importInstance, object document)
	{
		try
		{
			ImportInstance val = (ImportInstance)((importInstance is ImportInstance) ? importInstance : null);
			if (val == null)
			{
				return null;
			}
			Document val2 = (Document)((document is Document) ? document : null);
			if (val2 == null)
			{
				return null;
			}
			CADFileInfo cADFilePath = GetCADFilePath(val, val2);
			if (cADFilePath == null || string.IsNullOrEmpty(cADFilePath.FilePath))
			{
				Logger.Error("[RevitCADFileService] ❌ 无法获取 CAD 文件路径，无法解析文字和图块信息");
				Logger.Error("[RevitCADFileService] 💡 解决方案：请确保 CAD 文件是通过链接方式导入，或者原始 CAD 文件在 Revit 文档目录中");
				return null;
			}
			Logger.Info("[RevitCADFileService] 找到 CAD 文件: " + cADFilePath.FilePath);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(50, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[RevitCADFileService] 使用 CAD 导入单位: ");
			defaultInterpolatedStringHandler.AppendFormatted(cADFilePath.ImportUnit);
			defaultInterpolatedStringHandler.AppendLiteral(" (转换系数=");
			defaultInterpolatedStringHandler.AppendFormatted(cADFilePath.ConversionFactorToMM);
			defaultInterpolatedStringHandler.AppendLiteral(", 比例系数=");
			defaultInterpolatedStringHandler.AppendFormatted(cADFilePath.ScaleFactor);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			ICADFileService val3 = CoreServicesFactory.CreateCADFileService();
			CADFileData val4 = val3.ParseCADFile(cADFilePath.FilePath, cADFilePath.ConversionFactorToMM);
			if (val4 == null)
			{
				Logger.Error("[RevitCADFileService] ❌ ACadSharp 解析 CAD 文件失败: " + cADFilePath.FilePath);
				return null;
			}
			Transform importInstanceTransform = GetImportInstanceTransform(val, val2);
			if (importInstanceTransform != null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(60, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[RevitCADFileService] 应用 ImportInstance 变换: Origin=(");
				defaultInterpolatedStringHandler2.AppendFormatted(importInstanceTransform.Origin.X * 304.8, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(importInstanceTransform.Origin.Y * 304.8, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(importInstanceTransform.Origin.Z * 304.8, "F2");
				defaultInterpolatedStringHandler2.AppendLiteral(") mm");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				ApplyTransformToCADData(val4, importInstanceTransform);
			}
			else
			{
				Logger.Warning("[RevitCADFileService] 无法获取 ImportInstance 变换，使用原始坐标");
			}
			return val4;
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitCADFileService] ParseImportInstance 失败: " + ex.Message, ex);
			return null;
		}
	}

	private Transform? GetImportInstanceTransform(ImportInstance import, Document doc)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		try
		{
			Options val = new Options
			{
				DetailLevel = (ViewDetailLevel)3,
				ComputeReferences = true,
				IncludeNonVisibleObjects = true
			};
			GeometryElement val2 = ((Element)import).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				return null;
			}
			foreach (GeometryObject item in val2)
			{
				GeometryInstance val3 = (GeometryInstance)(object)((item is GeometryInstance) ? item : null);
				if (val3 != null)
				{
					return val3.Transform;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Debug("[RevitCADFileService] 获取 ImportInstance 变换失败: " + ex.Message);
			return null;
		}
	}

	private void ApplyTransformToCADData(CADFileData cadData, Transform transform)
	{
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0129: Expected O, but got Unknown
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Expected O, but got Unknown
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0235: Expected O, but got Unknown
		try
		{
			if (cadData.Texts != null)
			{
				foreach (CADTextInfo text in cadData.Texts)
				{
					XYZ val = new XYZ(text.X / 304.8, text.Y / 304.8, text.Z / 304.8);
					XYZ val2 = transform.OfPoint(val);
					text.X = val2.X * 304.8;
					text.Y = val2.Y * 304.8;
					text.Z = val2.Z * 304.8;
				}
			}
			if (cadData.Blocks != null)
			{
				foreach (CADBlockInfo block in cadData.Blocks)
				{
					XYZ val3 = new XYZ(block.X / 304.8, block.Y / 304.8, block.Z / 304.8);
					XYZ val4 = transform.OfPoint(val3);
					block.X = val4.X * 304.8;
					block.Y = val4.Y * 304.8;
					block.Z = val4.Z * 304.8;
				}
			}
			if (cadData.Lines != null)
			{
				foreach (CADLineInfo line in cadData.Lines)
				{
					XYZ val5 = new XYZ(line.StartX / 304.8, line.StartY / 304.8, line.StartZ / 304.8);
					XYZ val6 = new XYZ(line.EndX / 304.8, line.EndY / 304.8, line.EndZ / 304.8);
					XYZ val7 = transform.OfPoint(val5);
					XYZ val8 = transform.OfPoint(val6);
					line.StartX = val7.X * 304.8;
					line.StartY = val7.Y * 304.8;
					line.StartZ = val7.Z * 304.8;
					line.EndX = val8.X * 304.8;
					line.EndY = val8.Y * 304.8;
					line.EndZ = val8.Z * 304.8;
				}
			}
			Logger.Info("[RevitCADFileService] 成功应用变换到 CAD 数据");
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitCADFileService] 应用变换失败: " + ex.Message);
		}
	}

	public CADFileData? FilterByLayer(object importInstance, object document, string layerName)
	{
		Logger.Warning("[RevitCADFileService] FilterByLayer 暂未实现");
		return null;
	}

	public List<CADFileData> ParseMultipleImportInstances(IEnumerable<object> importInstances, object document)
	{
		Logger.Warning("[RevitCADFileService] ParseMultipleImportInstances 暂未实现");
		return new List<CADFileData>();
	}

	public bool IsDwgFile(string filePath)
	{
		try
		{
			if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
			{
				return false;
			}
			string text = Path.GetExtension(filePath).ToLower();
			return text == ".dwg";
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitCADFileService] IsDwgFile 检查失败: " + ex.Message);
			return false;
		}
	}

	public List<string> GetLayersFromCADFile(string cadFilePath)
	{
		try
		{
			ICADFileService val = CoreServicesFactory.CreateCADFileService();
			return val.GetLayersFromCADFile(cadFilePath);
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitCADFileService] GetLayersFromCADFile 失败: " + ex.Message);
			return new List<string>();
		}
	}

	[Obsolete("ACadSharp 直接支持 DWG 文件，无需转换")]
	public string? ConvertDwgToDxf(string dwgFilePath, string? outputFolder = null)
	{
		Logger.Warning("[RevitCADFileService] ACadSharp 直接支持 DWG 文件，无需转换");
		return null;
	}

	[Obsolete("ACadSharp 直接支持 DWG 文件，无需转换工具")]
	public DwgConverterStatus GetConverterStatus()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Expected O, but got Unknown
		return new DwgConverterStatus
		{
			HasOdaConverter = false,
			HasAutoCAD = false,
			SupportedMethods = new List<string> { "直接读取 DWG 文件（ACadSharp）" },
			RecommendedMethod = "ACadSharp",
			Message = "ACadSharp 直接支持 DWG 和 DXF 格式，无需转换工具，开箱即用。"
		};
	}
}
