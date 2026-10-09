using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Core.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using ns6;

namespace RevitAi.Revit.Services;

public class RevitSatelliteMapImporter : IDisposable
{
	private Document _document = null;

	private bool _disposed = false;

	private bool _textureApplied = false;

	private Level? _currentLevel = null;

	private static ConcurrentDictionary<string, GeoReferencePoint> _referencePoints = new ConcurrentDictionary<string, GeoReferencePoint>();

	public bool PaintDeferred { get; private set; } = false;

	public ElementId DeferredFloorId { get; private set; } = ElementId.InvalidElementId;

	public ElementId DeferredMaterialId { get; private set; } = ElementId.InvalidElementId;

	public List<ElementId> ImportSatelliteMap(Document doc, MergedImageResult mergedResult)
	{
		return ImportSatelliteMapInternal(doc, mergedResult, null);
	}

	public List<ElementId> ImportSatelliteMapWithTransaction(Document doc, MergedImageResult mergedResult, Transaction externalTransaction)
	{
		return ImportSatelliteMapInternal(doc, mergedResult, externalTransaction);
	}

	private List<ElementId> ImportSatelliteMapInternal(Document doc, MergedImageResult mergedResult, Transaction? externalTransaction)
	{
		//IL_0704: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0641: Unknown result type (might be due to invalid IL or missing references)
		//IL_0849: Unknown result type (might be due to invalid IL or missing references)
		//IL_084f: Invalid comparison between Unknown and I4
		//IL_0866: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ac: Expected O, but got Unknown
		//IL_0595: Unknown result type (might be due to invalid IL or missing references)
		//IL_059c: Expected O, but got Unknown
		//IL_059e: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0502: Expected O, but got Unknown
		//IL_0503: Unknown result type (might be due to invalid IL or missing references)
		//IL_0527: Unknown result type (might be due to invalid IL or missing references)
		//IL_053e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0544: Invalid comparison between Unknown and I4
		//IL_0547: Unknown result type (might be due to invalid IL or missing references)
		//IL_054d: Invalid comparison between Unknown and I4
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_0603: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_05cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0623: Unknown result type (might be due to invalid IL or missing references)
		//IL_062a: Expected O, but got Unknown
		//IL_06d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06dd: Expected O, but got Unknown
		//IL_06df: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f8: Unknown result type (might be due to invalid IL or missing references)
		Transaction val = null;
		try
		{
			_document = doc;
			double widthFeet = mergedResult.RevitSize.widthFeet;
			double heightFeet = mergedResult.RevitSize.heightFeet;
			string text = doc.PathName;
			if (string.IsNullOrEmpty(text))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
				defaultInterpolatedStringHandler.AppendLiteral("Unsaved_");
				defaultInterpolatedStringHandler.AppendFormatted(doc.Title);
				defaultInterpolatedStringHandler.AppendLiteral("_");
				defaultInterpolatedStringHandler.AppendFormatted(((object)doc).GetHashCode());
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			double num = (mergedResult.GeoBounds.minLat + mergedResult.GeoBounds.maxLat) / 2.0;
			double num2 = (mergedResult.GeoBounds.minLon + mergedResult.GeoBounds.maxLon) / 2.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			if (_referencePoints.TryGetValue(text, out GeoReferencePoint value))
			{
				Logger.Info("[RevitImporter] 参考点: " + value.GetDisplayText());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(26, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[RevitImporter] 当前中心: (");
				defaultInterpolatedStringHandler2.AppendFormatted(num, "F6");
				defaultInterpolatedStringHandler2.AppendLiteral(", ");
				defaultInterpolatedStringHandler2.AppendFormatted(num2, "F6");
				defaultInterpolatedStringHandler2.AppendLiteral(")");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				double num6 = num - value.CenterLat;
				double num7 = num2 - value.CenterLon;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler3.AppendLiteral("[RevitImporter] 地理差值: ΔLat=");
				defaultInterpolatedStringHandler3.AppendFormatted(num6, "F6");
				defaultInterpolatedStringHandler3.AppendLiteral("°, ΔLon=");
				defaultInterpolatedStringHandler3.AppendFormatted(num7, "F6");
				defaultInterpolatedStringHandler3.AppendLiteral("°");
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
				double num8 = GeoCoordinateConverter.LatDeltaToMeters(num6);
				double num9 = GeoCoordinateConverter.LonDeltaToMeters((num + value.CenterLat) / 2.0, num7);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(36, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("[RevitImporter] 地理偏移: ΔLat=");
				defaultInterpolatedStringHandler4.AppendFormatted(num8, "F0");
				defaultInterpolatedStringHandler4.AppendLiteral("m, ΔLon=");
				defaultInterpolatedStringHandler4.AppendFormatted(num9, "F0");
				defaultInterpolatedStringHandler4.AppendLiteral("m");
				Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
				double num10 = num9 / 0.3048;
				double num11 = num8 / 0.3048;
				num3 = value.RevitOriginX + num10;
				num4 = value.RevitOriginY + num11;
				num5 = value.RevitOriginZ;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(34, 3);
				defaultInterpolatedStringHandler5.AppendLiteral("[RevitImporter] 新楼板中心: Revit(");
				defaultInterpolatedStringHandler5.AppendFormatted(num3, "F2");
				defaultInterpolatedStringHandler5.AppendLiteral(", ");
				defaultInterpolatedStringHandler5.AppendFormatted(num4, "F2");
				defaultInterpolatedStringHandler5.AppendLiteral(", ");
				defaultInterpolatedStringHandler5.AppendFormatted(num5, "F2");
				defaultInterpolatedStringHandler5.AppendLiteral(")");
				Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
			}
			else
			{
				Logger.Info("[RevitImporter] 首次导入，设置参考点");
				num3 = 0.0;
				num4 = 0.0;
				num5 = 0.0;
			}
			XYZ floorCenter = new XYZ(num3, num4, num5);
			List<ElementId> list = new List<ElementId>();
			bool flag = externalTransaction == null;
			ElementId invalidElementId = ElementId.InvalidElementId;
			if (!File.Exists(mergedResult.ImagePath))
			{
				Logger.Error("[RevitImporter] 图片文件不存在: " + mergedResult.ImagePath);
				return list;
			}
			try
			{
				invalidElementId = CreateGeographicMaterial(doc, mergedResult.ImagePath, widthFeet, heightFeet, externalTransaction);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(41, 2);
				defaultInterpolatedStringHandler6.AppendLiteral("[RevitImporter] 材质创建完成，MaterialId: ");
				defaultInterpolatedStringHandler6.AppendFormatted(invalidElementId.Value);
				defaultInterpolatedStringHandler6.AppendLiteral(", 有效: ");
				defaultInterpolatedStringHandler6.AppendFormatted(invalidElementId != ElementId.InvalidElementId);
				Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
			}
			catch (Exception ex)
			{
				Logger.Error("[RevitImporter] 创建材质失败: " + ex.Message);
				if (ex.Message.Contains("AppearanceAssetEditScope") || ex.Message.Contains("不可访问") || ex.Message.Contains("保护级别"))
				{
					Logger.Error("[RevitImporter] 需要 Revit 2018.1 或更高版本");
				}
				return list;
			}
			Floor val2 = null;
			SubTransaction val3 = null;
			if (flag)
			{
				val = new Transaction(doc, "创建卫星图楼板");
				val.Start();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(28, 1);
				defaultInterpolatedStringHandler7.AppendLiteral("[RevitImporter] 楼板事务已启动，状态: ");
				defaultInterpolatedStringHandler7.AppendFormatted<TransactionStatus>(val.GetStatus());
				Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
				if ((int)val.GetStatus() != 1 && (int)val.GetStatus() != 3)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(27, 1);
					defaultInterpolatedStringHandler8.AppendLiteral("[RevitImporter] 事务启动失败，状态: ");
					defaultInterpolatedStringHandler8.AppendFormatted<TransactionStatus>(val.GetStatus());
					Logger.Error(defaultInterpolatedStringHandler8.ToStringAndClear());
					return list;
				}
			}
			else
			{
				val3 = new SubTransaction(doc);
				val3.Start();
			}
			try
			{
				val2 = CreateMapCanvas(doc, widthFeet, heightFeet, floorCenter);
				if (val2 == null)
				{
					Logger.Error("[RevitImporter] 楼板创建失败");
					if (flag)
					{
						val.RollBack();
					}
					else
					{
						val3.RollBack();
					}
					return list;
				}
				list.Add(((Element)val2).Id);
				if (flag)
				{
					val.Commit();
				}
				else
				{
					val3.Commit();
				}
				if (!_referencePoints.ContainsKey(text))
				{
					GeoReferencePoint value2 = new GeoReferencePoint(num, num2, num3, num4, num5);
					_referencePoints.TryAdd(text, value2);
				}
			}
			catch (Exception ex2)
			{
				if (flag)
				{
					val.RollBack();
				}
				else
				{
					val3.RollBack();
				}
				Logger.Error("[RevitImporter] 创建楼板失败: " + ex2.Message);
				return list;
			}
			if (_textureApplied && invalidElementId != ElementId.InvalidElementId)
			{
				if (flag)
				{
					Element element = doc.GetElement(((Element)val2).Id);
					Floor val4 = (Floor)(object)((element is Floor) ? element : null);
					if (val4 == null)
					{
						Logger.Error("[RevitImporter] 无法获取楼板引用");
						return list;
					}
					Transaction val5 = new Transaction(doc, "应用卫星图材质");
					try
					{
						val5.Start();
						try
						{
							doc.Regenerate();
							ApplyMaterialToTopFace(doc, val4, invalidElementId);
							val5.Commit();
						}
						catch (Exception ex3)
						{
							val5.RollBack();
							Logger.Warning("[RevitImporter] Paint 工具应用失败: " + ex3.Message);
						}
					}
					finally
					{
						((IDisposable)val5)?.Dispose();
					}
				}
				else
				{
					PaintDeferred = true;
					DeferredFloorId = ((Element)val2).Id;
					DeferredMaterialId = invalidElementId;
					Logger.Info("[RevitImporter] Paint 推迟到父事务提交后执行");
				}
			}
			else if (invalidElementId == ElementId.InvalidElementId)
			{
				Logger.Error("[RevitImporter] 材质创建失败，需要 Revit 2018.1 或更高版本");
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(29, 2);
			defaultInterpolatedStringHandler9.AppendLiteral("[RevitImporter] 导入完成: ");
			defaultInterpolatedStringHandler9.AppendFormatted(widthFeet, "F0");
			defaultInterpolatedStringHandler9.AppendLiteral("ft x ");
			defaultInterpolatedStringHandler9.AppendFormatted(heightFeet, "F0");
			defaultInterpolatedStringHandler9.AppendLiteral("ft");
			Logger.Info(defaultInterpolatedStringHandler9.ToStringAndClear());
			if (_textureApplied)
			{
				Logger.Info("[RevitImporter] ✅ 卫星图已应用（1:1 真实世界比例）");
				Logger.Info("[RevitImporter] 提示：在「光线追踪」模式下可查看");
			}
			else if (invalidElementId != ElementId.InvalidElementId)
			{
				Logger.Info("[RevitImporter] ⚠️ 材质创建成功，但需要手动应用到楼板");
			}
			return list;
		}
		catch (Exception ex4)
		{
			bool flag2 = externalTransaction == null;
			if (((val != null) & flag2) && (int)val.GetStatus() == 1)
			{
				Logger.Warning("[RevitImporter] 出现错误，回滚事务");
				val.RollBack();
			}
			Logger.Error("[RevitImporter] 导入失败: " + ex4.Message);
			return new List<ElementId>();
		}
		finally
		{
			if (externalTransaction == null && val != null)
			{
				val.Dispose();
			}
		}
	}

	private ElementId CreateGeographicMaterial(Document doc, string imgPath, double widthFt, double heightFt, Transaction? parentTransaction = null)
	{
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_06b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Expected O, but got Unknown
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Expected O, but got Unknown
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		ElementId val = ElementId.InvalidElementId;
		AppearanceAssetElement val2 = null;
		bool flag = parentTransaction != null;
		Transaction val3 = null;
		SubTransaction val4 = null;
		try
		{
			if (flag)
			{
				val4 = new SubTransaction(doc);
				val4.Start();
			}
			else
			{
				val3 = new Transaction(doc, "创建卫星图材质基础");
				val3.Start();
			}
			try
			{
				string text = "RevitAi_卫星地图_" + Guid.NewGuid().ToString("N").Substring(0, 8);
				val = Material.Create(doc, text);
				Element element = doc.GetElement(val);
				Material val5 = (Material)(object)((element is Material) ? element : null);
				if (val5 == null)
				{
					Logger.Error("[RevitImporter] 创建材质对象失败");
					if (flag)
					{
						val4.RollBack();
					}
					else
					{
						val3.RollBack();
					}
					return ElementId.InvalidElementId;
				}
				val5.Color = new Color((byte)200, (byte)200, (byte)200);
				try
				{
					val2 = FindCompatibleAppearanceAsset(doc, text);
					if (val2 == null)
					{
						Logger.Error("[RevitImporter] 文档中没有找到包含 generic_diffuse 属性的外观资产");
						if (flag)
						{
							val4.RollBack();
						}
						else
						{
							val3.RollBack();
						}
						return ElementId.InvalidElementId;
					}
				}
				catch (Exception ex)
				{
					Logger.Error("[RevitImporter] 创建外观资产失败: " + ex.Message);
					if (flag)
					{
						val4.RollBack();
					}
					else
					{
						val3.RollBack();
					}
					return ElementId.InvalidElementId;
				}
				val5.AppearanceAssetId = ((Element)val2).Id;
				if (flag)
				{
					val4.Commit();
				}
				else
				{
					val3.Commit();
				}
			}
			catch (Exception ex2)
			{
				if (flag)
				{
					val4.RollBack();
				}
				else
				{
					val3.RollBack();
				}
				Logger.Error("[RevitImporter] 创建材质基础失败: " + ex2.Message);
				return ElementId.InvalidElementId;
			}
		}
		finally
		{
			if (val3 != null)
			{
				val3.Dispose();
			}
			if (val4 != null)
			{
				val4.Dispose();
			}
		}
		_textureApplied = false;
		Transaction val6 = null;
		SubTransaction val7 = null;
		try
		{
			Type typeFromHandle = typeof(AppearanceAssetEditScope);
			if (typeFromHandle != null)
			{
				if (flag)
				{
					val7 = new SubTransaction(doc);
					val7.Start();
				}
				else
				{
					val6 = new Transaction(doc, "设置卫星图贴图");
					val6.Start();
				}
				try
				{
					object obj = Activator.CreateInstance(typeFromHandle, doc);
					MethodInfo method = typeFromHandle.GetMethod("Start", new Type[1] { typeof(ElementId) });
					object obj2 = method.Invoke(obj, new object[1] { ((Element)val2).Id });
					MethodInfo method2 = obj2.GetType().GetMethod("FindByName", new Type[1] { typeof(string) });
					object obj3 = method2.Invoke(obj2, new object[1] { "generic_diffuse" });
					if (obj3 != null)
					{
						MethodInfo method3 = obj3.GetType().GetMethod("GetSingleConnectedAsset", new Type[0]);
						object obj4 = method3.Invoke(obj3, new object[0]);
						if (obj4 == null)
						{
							try
							{
								MethodInfo method4 = obj3.GetType().GetMethod("AddConnectedAsset", new Type[1] { typeof(string) });
								if (method4 != null)
								{
									method4.Invoke(obj3, new object[1] { "UnifiedBitmap" });
									obj4 = method3.Invoke(obj3, new object[0]);
								}
							}
							catch (Exception)
							{
								Logger.Error("[RevitImporter] 创建位图资产失败");
							}
						}
						if (obj4 != null)
						{
							string[] possibleNames = new string[4]
							{
								"unifiedbitmap_Bitmap",
								"UnifiedbitmapBitmap",
								"Source",
								"Bitmap"
							};
							object obj5 = TryFindProperty(obj4, method2, possibleNames);
							if (obj5 != null)
							{
								PropertyInfo property = obj5.GetType().GetProperty("Value");
								if (property != null)
								{
									property.SetValue(obj5, imgPath, null);
								}
							}
							object obj6 = method2.Invoke(obj4, new object[1] { "texture_RealWorldScaleX" });
							if (obj6 != null)
							{
								PropertyInfo property2 = obj6.GetType().GetProperty("Value");
								if (property2 != null)
								{
									property2.SetValue(obj6, widthFt * 12.0, null);
								}
							}
							object obj7 = method2.Invoke(obj4, new object[1] { "texture_RealWorldScaleY" });
							if (obj7 != null)
							{
								PropertyInfo property3 = obj7.GetType().GetProperty("Value");
								if (property3 != null)
								{
									property3.SetValue(obj7, heightFt * 12.0, null);
								}
							}
							object obj8 = method2.Invoke(obj4, new object[1] { "texture_RealWorldOffsetX" });
							if (obj8 != null)
							{
								PropertyInfo property4 = obj8.GetType().GetProperty("Value");
								if (property4 != null)
								{
									property4.SetValue(obj8, (0.0 - widthFt * 12.0) / 2.0, null);
								}
							}
							object obj9 = method2.Invoke(obj4, new object[1] { "texture_RealWorldOffsetY" });
							if (obj9 != null)
							{
								PropertyInfo property5 = obj9.GetType().GetProperty("Value");
								if (property5 != null)
								{
									property5.SetValue(obj9, (0.0 - heightFt * 12.0) / 2.0, null);
								}
							}
							object obj10 = method2.Invoke(obj4, new object[1] { "texture_URepeat" });
							if (obj10 != null)
							{
								PropertyInfo property6 = obj10.GetType().GetProperty("Value");
								if (property6 != null)
								{
									property6.SetValue(obj10, false, null);
								}
							}
							object obj11 = method2.Invoke(obj4, new object[1] { "texture_VRepeat" });
							if (obj11 != null)
							{
								PropertyInfo property7 = obj11.GetType().GetProperty("Value");
								if (property7 != null)
								{
									property7.SetValue(obj11, false, null);
								}
							}
						}
						else
						{
							Logger.Warning("[RevitImporter] 无法获取位图资产");
						}
					}
					else
					{
						Logger.Warning("[RevitImporter] generic_diffuse 属性不存在");
					}
					MethodInfo method5 = typeFromHandle.GetMethod("Commit", new Type[1] { typeof(bool) });
					method5.Invoke(obj, new object[1] { true });
					if (flag)
					{
						val7.Commit();
					}
					else
					{
						val6.Commit();
					}
				}
				catch (Exception)
				{
					if (flag && val7 != null)
					{
						val7.RollBack();
					}
					else if (val6 != null)
					{
						val6.RollBack();
					}
					throw;
				}
				Logger.Info("[RevitImporter] 材质设置成功");
				_textureApplied = true;
			}
			else
			{
				Logger.Warning("[RevitImporter] AppearanceAssetEditScope 不可用，需要 Revit 2018.1+");
			}
		}
		catch (Exception ex5)
		{
			string text2 = ((ex5.InnerException != null) ? (", 内部异常: " + ex5.InnerException.Message) : "");
			Logger.Warning("[RevitImporter] 设置贴图失败: " + ex5.Message + text2);
		}
		finally
		{
			if (val6 != null)
			{
				val6.Dispose();
			}
			if (val7 != null)
			{
				val7.Dispose();
			}
		}
		return val;
	}

	private Floor? CreateMapCanvas(Document doc, double widthFt, double heightFt, XYZ floorCenter)
	{
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Expected O, but got Unknown
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_012f: Expected O, but got Unknown
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Expected O, but got Unknown
		//IL_0162: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Expected O, but got Unknown
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Expected O, but got Unknown
		try
		{
			ElementId val = ElementId.InvalidElementId;
			try
			{
				val = doc.GetDefaultElementTypeId((ElementTypeGroup)4);
			}
			catch
			{
				Element obj2 = new FilteredElementCollector(doc).OfClass(typeof(FloorType)).FirstElement();
				FloorType val2 = (FloorType)(object)((obj2 is FloorType) ? obj2 : null);
				if (val2 != null)
				{
					val = ((Element)val2).Id;
				}
			}
			Level val3 = null;
			double num = double.MaxValue;
			foreach (Level item in new FilteredElementCollector(doc).OfClass(typeof(Level)).ToElements())
			{
				Level val4 = item;
				double num2 = Math.Abs(val4.Elevation);
				if (num2 < num)
				{
					num = num2;
					val3 = val4;
				}
			}
			if (val3 == null)
			{
				Logger.Error("[RevitImporter] 未找到标高");
				return null;
			}
			_currentLevel = val3;
			double num3 = widthFt / 2.0;
			double num4 = heightFt / 2.0;
			double num5 = ((floorCenter.Z != 0.0) ? floorCenter.Z : val3.Elevation);
			XYZ val5 = new XYZ(floorCenter.X - num3, floorCenter.Y - num4, num5);
			XYZ val6 = new XYZ(floorCenter.X + num3, floorCenter.Y - num4, num5);
			XYZ val7 = new XYZ(floorCenter.X + num3, floorCenter.Y + num4, num5);
			XYZ val8 = new XYZ(floorCenter.X - num3, floorCenter.Y + num4, num5);
			Floor val9 = null;
			CurveLoop val10 = new CurveLoop();
			val10.Append((Curve)(object)Line.CreateBound(val5, val6));
			val10.Append((Curve)(object)Line.CreateBound(val6, val7));
			val10.Append((Curve)(object)Line.CreateBound(val7, val8));
			val10.Append((Curve)(object)Line.CreateBound(val8, val5));
			val9 = Floor.Create(doc, (IList<CurveLoop>)new List<CurveLoop> { val10 }, val, ((Element)val3).Id);
			if (val9 == null)
			{
				Logger.Error("[RevitImporter] 楼板创建失败");
				return null;
			}
			return val9;
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitImporter] 创建楼板失败: " + ex.Message);
			return null;
		}
	}

	public void ApplyDeferredPaint(Document doc, Floor floor, ElementId materialId)
	{
		ApplyMaterialToTopFace(doc, floor, materialId);
	}

	private void ApplyMaterialToTopFace(Document doc, Floor floor, ElementId materialId)
	{
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Expected O, but got Unknown
		//IL_016a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		try
		{
			MethodInfo method = typeof(Floor).GetMethod("GetTopFaces");
			if (method != null)
			{
				IList list = method.Invoke(floor, null) as IList;
				MethodInfo method2 = typeof(Floor).GetMethod("GetGeometryObjectFromReference");
				if (list != null && list.Count > 0 && method2 != null)
				{
					foreach (object item in list)
					{
						object? obj = method2.Invoke(floor, new object[1] { item });
						Face val = (Face)((obj is Face) ? obj : null);
						if ((GeometryObject)(object)val != (GeometryObject)null)
						{
							doc.Paint(((Element)floor).Id, val, materialId);
							Logger.Info("[RevitImporter] 材质已通过 GetTopFaces 应用到楼板顶面");
							return;
						}
					}
				}
			}
			Options val2 = new Options();
			GeometryElement val3 = ((Element)floor).get_Geometry(val2);
			if ((GeometryObject)(object)val3 == (GeometryObject)null)
			{
				Logger.Warning("[RevitImporter] 无法获取楼板几何信息");
				return;
			}
			foreach (GeometryObject item2 in val3)
			{
				Solid val4 = (Solid)(object)((item2 is Solid) ? item2 : null);
				if (val4 == null)
				{
					continue;
				}
				foreach (Face face in val4.Faces)
				{
					Face val5 = face;
					XYZ val6 = val5.ComputeNormal(new UV(0.5, 0.5));
					if (val6 != null && val6.Z > 0.9)
					{
						doc.Paint(((Element)floor).Id, val5, materialId);
						return;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[RevitImporter] 应用材质失败: " + ex.Message);
			throw;
		}
	}

	private object? TryFindProperty(object asset, MethodInfo findByNameMethod, string[] possibleNames)
	{
		foreach (string text in possibleNames)
		{
			try
			{
				object obj = findByNameMethod.Invoke(asset, new object[1] { text });
				if (obj != null)
				{
					return obj;
				}
			}
			catch
			{
			}
		}
		return null;
	}

	private AppearanceAssetElement? FindCompatibleAppearanceAsset(Document doc, string matName)
	{
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		string[] array = new string[13]
		{
			"Generic",
			"Default",
			"Glazing",
			"Ceramic",
			"Concrete",
			"Masonry",
			"Metal",
			"Paint",
			"Plastic",
			"Wood",
			"Flooring",
			"Wall",
			"Roofing"
		};
		List<AppearanceAssetElement> list = new FilteredElementCollector(doc).OfClass(typeof(AppearanceAssetElement)).ToElements().Cast<AppearanceAssetElement>()
			.ToList();
		if (!list.Any())
		{
			Logger.Error("[RevitImporter] 文档中没有外观资产");
			return null;
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[RevitImporter] 找到 ");
		defaultInterpolatedStringHandler.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个外观资产");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		string[] array2 = array;
		foreach (string keyword in array2)
		{
			List<AppearanceAssetElement> list2 = list.Where((AppearanceAssetElement a) => ((Element)a).Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
			foreach (AppearanceAssetElement item in list2)
			{
				if (HasGenericDiffuseProperty(item))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[RevitImporter] 找到兼容材质: ");
					defaultInterpolatedStringHandler2.AppendFormatted(((Element)item).Name);
					defaultInterpolatedStringHandler2.AppendLiteral(" (关键词: ");
					defaultInterpolatedStringHandler2.AppendFormatted(keyword);
					defaultInterpolatedStringHandler2.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					return item.Duplicate("Asset_" + matName);
				}
			}
		}
		Logger.Info("[RevitImporter] 关键词材质未找到，检查所有材质...");
		foreach (AppearanceAssetElement item2 in list)
		{
			if (HasGenericDiffuseProperty(item2))
			{
				Logger.Info("[RevitImporter] 找到兼容材质: " + ((Element)item2).Name + " (全面搜索)");
				return item2.Duplicate("Asset_" + matName);
			}
		}
		Logger.Warning("[RevitImporter] 未找到包含 generic_diffuse 的材质，使用第一个可用材质");
		AppearanceAssetElement val = list.First();
		return val.Duplicate("Asset_" + matName);
	}

	private bool HasGenericDiffuseProperty(AppearanceAssetElement assetElem)
	{
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		try
		{
			Type typeFromHandle = typeof(AppearanceAssetEditScope);
			if (typeFromHandle == null)
			{
				return false;
			}
			object obj = Activator.CreateInstance(typeFromHandle, _document);
			if (obj == null)
			{
				return false;
			}
			AppearanceAssetEditScope val = (AppearanceAssetEditScope)obj;
			try
			{
				object obj2 = typeFromHandle.GetMethod("Start", new Type[1] { typeof(ElementId) })?.Invoke(val, new object[1] { ((Element)assetElem).Id });
				if (obj2 != null)
				{
					MethodInfo method = obj2.GetType().GetMethod("FindByName", new Type[1] { typeof(string) });
					if (method != null)
					{
						object obj3 = method.Invoke(obj2, new object[1] { "generic_diffuse" });
						if (obj3 != null)
						{
							Type type = ((object)val)?.GetType();
							if (type != null)
							{
								type.GetMethod("Dispose", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)?.Invoke(val, null);
							}
							return true;
						}
					}
				}
				typeFromHandle.GetMethod("Cancel")?.Invoke(val, null);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logger.Debug("[RevitImporter] 检查材质属性时出错: " + ex.Message);
		}
		return false;
	}

	public static void ClearReferencePoint(string docPath)
	{
		if (!string.IsNullOrEmpty(docPath) && _referencePoints.TryRemove(docPath, out GeoReferencePoint value))
		{
			Logger.Info("[RevitImporter] ✅ 已清除参考点: " + value.GetDisplayText());
		}
	}

	public static void ClearReferencePoint(Document doc)
	{
		if (doc != null)
		{
			ClearReferencePoint(doc.PathName);
		}
	}

	public static GeoReferencePoint? GetReferencePoint(Document doc)
	{
		if (doc == null)
		{
			return null;
		}
		string text = doc.PathName;
		if (string.IsNullOrEmpty(text))
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 2);
			defaultInterpolatedStringHandler.AppendLiteral("Unsaved_");
			defaultInterpolatedStringHandler.AppendFormatted(doc.Title);
			defaultInterpolatedStringHandler.AppendLiteral("_");
			defaultInterpolatedStringHandler.AppendFormatted(((object)doc).GetHashCode());
			text = defaultInterpolatedStringHandler.ToStringAndClear();
		}
		if (_referencePoints.TryGetValue(text, out GeoReferencePoint value))
		{
			return value;
		}
		return null;
	}

	public static Dictionary<string, GeoReferencePoint> GetAllReferencePoints()
	{
		return _referencePoints.ToDictionary<KeyValuePair<string, GeoReferencePoint>, string, GeoReferencePoint>((KeyValuePair<string, GeoReferencePoint> kvp) => kvp.Key, (KeyValuePair<string, GeoReferencePoint> kvp) => kvp.Value);
	}

	public void Dispose()
	{
		if (!_disposed)
		{
			_document = null;
			_disposed = true;
		}
	}
}
