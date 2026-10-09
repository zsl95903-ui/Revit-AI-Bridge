using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using ns0;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Revit;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class RoadSurfaceVoidGenerationExternalEventHandler : IExternalEventHandler
{
	public void Execute(UIApplication app)
	{
		//IL_0f6b: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_1022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0e23: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_096e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0975: Expected O, but got Unknown
		//IL_097e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0988: Expected O, but got Unknown
		//IL_06ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0706: Expected O, but got Unknown
		//IL_07a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_07a8: Expected O, but got Unknown
		//IL_0a0d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a14: Expected O, but got Unknown
		//IL_07aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a16: Unknown result type (might be due to invalid IL or missing references)
		//IL_080e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0815: Expected O, but got Unknown
		//IL_034c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0353: Expected O, but got Unknown
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Expected O, but got Unknown
		//IL_03ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0400: Expected O, but got Unknown
		//IL_0cee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0cf5: Expected O, but got Unknown
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Expected O, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a3: Expected O, but got Unknown
		//IL_04af: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b6: Expected O, but got Unknown
		//IL_052b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Expected O, but got Unknown
		//IL_04ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_04f6: Expected O, but got Unknown
		//IL_0569: Unknown result type (might be due to invalid IL or missing references)
		//IL_0573: Expected O, but got Unknown
		try
		{
			RoadSurfaceVoidGenerationRequest andClearVoidGenerationRequest = RoadSurfaceRefinementRequestManager.GetAndClearVoidGenerationRequest();
			if (andClearVoidGenerationRequest == null)
			{
				TaskDialog.Show("提示", "无效的空心生成请求");
				return;
			}
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "请打开一个文档", null));
				return;
			}
			Logger.Info("[精修路基路面] 开始执行步骤2：生成空心");
			object floorElement = andClearVoidGenerationRequest.FloorElement;
			if (floorElement == null)
			{
				Logger.Warning("[精修路基路面] 未选择楼板");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "未选择楼板", null));
				return;
			}
			ElementId val2 = method_4(floorElement);
			Element element = val.GetElement(val2);
			if (element == null)
			{
				Logger.Warning("[精修路基路面] 选择的元素不是楼板");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "选择的元素不是楼板", null));
				return;
			}
			Floor val3 = (Floor)(object)((element is Floor) ? element : null);
			if (val3 == null)
			{
				Logger.Warning("[精修路基路面] 选择的元素不是楼板");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "选择的元素不是楼板", null));
				return;
			}
			double num = method_0(val, "AST_R_路基路面_3");
			if (num <= 0.0)
			{
				Logger.Warning("[精修路基路面] 无法从 AST_R_路基路面_3 族获取高度范围，将使用默认高度 5 米");
				num = 16.404199475065617;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[精修路基路面] 从族实例获取的拉伸高度: ");
				defaultInterpolatedStringHandler.AppendFormatted(num * 304.8, "F2");
				defaultInterpolatedStringHandler.AppendLiteral(" 毫米");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			string text = method_2();
			if (string.IsNullOrEmpty(text))
			{
				Logger.Error("[精修路基路面] 找不到族模板文件：公制常规模型.rft");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "找不到族模板文件：公制常规模型.rft", null));
				return;
			}
			Logger.Info("[精修路基路面] 使用模板: " + text);
			List<CurveLoop> list = method_3(val3);
			if (list == null || list.Count == 0)
			{
				Logger.Error("[精修路基路面] 无法提取楼板轮廓");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "无法提取楼板轮廓", null));
				return;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("[精修路基路面] 提取到 ");
			defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个轮廓环（外轮廓+内部开洞）");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			BoundingBoxXYZ val4 = ((Element)val3).get_BoundingBox((View)null);
			if (val4 == null)
			{
				Logger.Error("[精修路基路面] 无法获取楼板边界框");
				andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "无法获取楼板边界框", null));
				return;
			}
			XYZ val5 = new XYZ(val4.Min.X, val4.Min.Y, val4.Min.Z);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(17, 1);
			defaultInterpolatedStringHandler3.AppendLiteral("[精修路基路面] 楼板原点位置: ");
			defaultInterpolatedStringHandler3.AppendFormatted<XYZ>(val5);
			Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			List<object> list2 = new List<object>();
			Document val6 = null;
			try
			{
				val6 = app.Application.NewFamilyDocument(text);
				if (val6 == null)
				{
					Logger.Error("[精修路基路面] 无法创建族文档");
					andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "无法创建族文档", null));
					return;
				}
				Transaction val7 = new Transaction(val6, "创建拉伸");
				try
				{
					val7.Start();
					try
					{
						Level val8 = null;
						FilteredElementCollector val9 = new FilteredElementCollector(val6);
						val9.OfClass(typeof(Level));
						foreach (Element item2 in val9)
						{
							val8 = (Level)(object)((item2 is Level) ? item2 : null);
							if (val8 != null)
							{
								break;
							}
						}
						if (val8 == null)
						{
							val8 = Level.Create(val6, 0.0);
						}
						Plane val10 = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ(0.0, 0.0, val5.Z));
						SketchPlane val11 = SketchPlane.Create(val6, val10);
						CurveArrArray val12 = new CurveArrArray();
						if (list.Count > 0)
						{
							CurveArray val13 = new CurveArray();
							foreach (Curve item3 in list[0])
							{
								Curve val14 = item3.CreateTransformed(Transform.CreateTranslation(new XYZ(0.0 - val5.X, 0.0 - val5.Y, 0.0)));
								val13.Append(val14);
							}
							val12.Append(val13);
						}
						for (int i = 1; i < list.Count; i++)
						{
							CurveArray val15 = new CurveArray();
							foreach (Curve item4 in list[i])
							{
								Curve val16 = item4.CreateTransformed(Transform.CreateTranslation(new XYZ(0.0 - val5.X, 0.0 - val5.Y, 0.0)));
								val15.Append(val16);
							}
							val12.Append(val15);
						}
						val6.FamilyCreate.NewExtrusion(false, val12, val11, num);
						Parameter val17 = ((Element)val6.OwnerFamily).get_Parameter((BuiltInParameter)(-1012811L));
						if (val17 != null && !((APIObject)val17).IsReadOnly)
						{
							try
							{
								val17.Set(1);
								Logger.Info("[精修路基路面] 已设置 FAMILY_ALLOW_CUT_WITH_VOIDS = 1");
							}
							catch (Exception ex)
							{
								Logger.Error("[精修路基路面] 设置切割参数失败: " + ex.Message);
							}
						}
						else
						{
							Logger.Warning("[精修路基路面] 未找到 FAMILY_ALLOW_CUT_WITH_VOIDS 参数");
						}
						val7.Commit();
						Logger.Info("[精修路基路面] 族内空心拉伸创建成功");
					}
					catch (Exception ex2)
					{
						val7.RollBack();
						Logger.Error("[精修路基路面] 创建拉伸失败: " + ex2.Message);
						andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "创建拉伸失败: " + ex2.Message, null));
						return;
					}
				}
				finally
				{
					((IDisposable)val7)?.Dispose();
				}
				string text2 = method_5(val);
				Logger.Info("[精修路基路面] 生成族名称: " + text2);
				string tempPath = Path.GetTempPath();
				string text3 = Path.Combine(tempPath, text2 + ".rfa");
				try
				{
					SaveAsOptions val18 = new SaveAsOptions();
					val18.OverwriteExistingFile = true;
					val6.SaveAs(text3, val18);
					Logger.Info("[精修路基路面] 族文件已保存到: " + text3);
				}
				catch (Exception ex3)
				{
					Logger.Error("[精修路基路面] 保存族文件失败: " + ex3.Message);
					andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "保存族文件失败: " + ex3.Message, null));
					return;
				}
				Family val19 = null;
				try
				{
					val6.Close(false);
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text3);
					Transaction val20 = new Transaction(val, "载入族");
					try
					{
						val20.Start();
						bool flag = val.LoadFamily(text3);
						val20.Commit();
						if (!flag)
						{
							Logger.Error("[精修路基路面] 载入族失败");
							andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "载入族失败", null));
							return;
						}
					}
					finally
					{
						((IDisposable)val20)?.Dispose();
					}
					FilteredElementCollector val21 = new FilteredElementCollector(val);
					val21.OfClass(typeof(Family));
					foreach (Element item5 in val21)
					{
						Family val22 = (Family)(object)((item5 is Family) ? item5 : null);
						if (val22 != null && ((Element)val22).Name == fileNameWithoutExtension)
						{
							val19 = val22;
							break;
						}
					}
					if (val19 == null)
					{
						Logger.Error("[精修路基路面] 找不到载入的族");
						andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "找不到载入的族", null));
						return;
					}
					Logger.Info("[精修路基路面] 族已载入项目: " + ((Element)val19).Name);
					try
					{
						if (File.Exists(text3))
						{
							File.Delete(text3);
							Logger.Info("[精修路基路面] 已删除临时族文件: " + text3);
						}
					}
					catch (Exception ex4)
					{
						Logger.Warning("[精修路基路面] 删除临时族文件失败: " + ex4.Message);
					}
				}
				catch (Exception ex5)
				{
					Logger.Error("[精修路基路面] 载入族失败: " + ex5.Message);
					andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "载入族失败: " + ex5.Message, null));
					return;
				}
				FamilySymbol val23 = null;
				FilteredElementCollector val24 = new FilteredElementCollector(val);
				val24.WherePasses((ElementFilter)new FamilySymbolFilter(((Element)val19).Id));
				foreach (Element item6 in val24)
				{
					val23 = (FamilySymbol)(object)((item6 is FamilySymbol) ? item6 : null);
					if (val23 != null)
					{
						break;
					}
				}
				if (val23 == null)
				{
					Logger.Error("[精修路基路面] 找不到族符号");
					andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "找不到族符号", null));
					return;
				}
				Transaction val25 = new Transaction(val, "放置族实例");
				try
				{
					val25.Start();
					try
					{
						ElementId levelId = ((Element)val3).LevelId;
						Element element2 = val.GetElement(levelId);
						Level val26 = (Level)(object)((element2 is Level) ? element2 : null);
						if (val26 == null)
						{
							Logger.Error("[精修路基路面] 无法获取楼板标高");
							andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "无法获取楼板标高", null));
							return;
						}
						XYZ val27 = val5;
						if (!val23.IsActive)
						{
							val23.Activate();
						}
						FamilyInstance val28 = ((ItemFactoryBase)val.Create).NewFamilyInstance(val27, val23, val26, (StructuralType)0);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler4.AppendLiteral("[精修路基路面] 族实例已创建，ID: ");
						defaultInterpolatedStringHandler4.AppendFormatted(((Element)val28).Id.Value);
						Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
						Element element3 = val.GetElement(((Element)val28).LevelId);
						Level val29 = (Level)(object)((element3 is Level) ? element3 : null);
						double num2 = ((val29 != null) ? val29.Elevation : 0.0);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(25, 2);
						defaultInterpolatedStringHandler5.AppendLiteral("[精修路基路面] 族实例标高: ");
						object obj;
						if (val29 == null)
						{
							obj = null;
						}
						else
						{
							obj = ((Element)val29).Name;
							if (obj != null)
							{
								goto IL_0b3f;
							}
						}
						obj = "未知";
						goto IL_0b3f;
						IL_0b3f:
						defaultInterpolatedStringHandler5.AppendFormatted((string?)obj);
						defaultInterpolatedStringHandler5.AppendLiteral(", 高程: ");
						defaultInterpolatedStringHandler5.AppendFormatted(num2 * 304.8, "F2");
						defaultInterpolatedStringHandler5.AppendLiteral(" 毫米");
						Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
						Tuple<double, double> tuple = method_1(val, "AST_R_路基路面_3");
						double num3 = val5.Z - num2;
						if (tuple != null)
						{
							double item = tuple.Item1;
							num3 = item - num2;
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(31, 2);
							defaultInterpolatedStringHandler6.AppendLiteral("[精修路基路面] 族实例minZ: ");
							defaultInterpolatedStringHandler6.AppendFormatted(item * 304.8, "F2");
							defaultInterpolatedStringHandler6.AppendLiteral(" 毫米, 标高程: ");
							defaultInterpolatedStringHandler6.AppendFormatted(num2 * 304.8, "F2");
							defaultInterpolatedStringHandler6.AppendLiteral(" 毫米");
							Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(30, 2);
						defaultInterpolatedStringHandler7.AppendLiteral("[精修路基路面] 楼板底部Z: ");
						defaultInterpolatedStringHandler7.AppendFormatted(val5.Z * 304.8, "F2");
						defaultInterpolatedStringHandler7.AppendLiteral(" 毫米, 计算偏移: ");
						defaultInterpolatedStringHandler7.AppendFormatted(num3 * 304.8, "F2");
						defaultInterpolatedStringHandler7.AppendLiteral(" 毫米");
						Logger.Info(defaultInterpolatedStringHandler7.ToStringAndClear());
						Parameter val30 = null;
						foreach (Parameter parameter in ((Element)val28).Parameters)
						{
							Parameter val31 = parameter;
							string name = val31.Definition.Name;
							if (name.Contains("偏移") || name.Contains("Offset") || name.Equals("偏移"))
							{
								val30 = val31;
								Logger.Info("[精修路基路面] 找到偏移参数: " + name);
								break;
							}
						}
						if (val30 != null && !((APIObject)val30).IsReadOnly)
						{
							try
							{
								val30.Set(num3);
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(21, 1);
								defaultInterpolatedStringHandler8.AppendLiteral("[精修路基路面] 已设置偏移参数: ");
								defaultInterpolatedStringHandler8.AppendFormatted(num3 * 304.8, "F2");
								defaultInterpolatedStringHandler8.AppendLiteral(" 毫米");
								Logger.Info(defaultInterpolatedStringHandler8.ToStringAndClear());
							}
							catch (Exception ex6)
							{
								Logger.Warning("[精修路基路面] 设置偏移参数失败: " + ex6.Message);
							}
						}
						else
						{
							Logger.Warning("[精修路基路面] 未找到偏移参数或参数为只读");
						}
						val25.Commit();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(20, 1);
						defaultInterpolatedStringHandler9.AppendLiteral("[精修路基路面] 族实例已放置，位置: ");
						defaultInterpolatedStringHandler9.AppendFormatted<XYZ>(val27);
						Logger.Info(defaultInterpolatedStringHandler9.ToStringAndClear());
						double num4 = num * 304.8;
						list2.Add(val28);
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(22, 1);
						defaultInterpolatedStringHandler10.AppendLiteral("[精修路基路面] 步骤2完成，生成 ");
						defaultInterpolatedStringHandler10.AppendFormatted(list2.Count);
						defaultInterpolatedStringHandler10.AppendLiteral(" 个空心");
						Logger.Info(defaultInterpolatedStringHandler10.ToStringAndClear());
						Action<ValueTuple<bool, string, List<object>>> onCompleted = andClearVoidGenerationRequest.OnCompleted;
						if (onCompleted != null)
						{
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(37, 3);
							defaultInterpolatedStringHandler11.AppendLiteral("楼板轮廓空心族已创建并载入！\n\n轮廓环数: ");
							defaultInterpolatedStringHandler11.AppendFormatted(list.Count);
							defaultInterpolatedStringHandler11.AppendLiteral("\n拉伸高度: ");
							defaultInterpolatedStringHandler11.AppendFormatted(num4, "F2");
							defaultInterpolatedStringHandler11.AppendLiteral(" 毫米 (");
							defaultInterpolatedStringHandler11.AppendFormatted(num4 / 1000.0, "F2");
							defaultInterpolatedStringHandler11.AppendLiteral(" 米)");
							onCompleted((true, defaultInterpolatedStringHandler11.ToStringAndClear(), list2));
						}
					}
					catch (Exception ex7)
					{
						val25.RollBack();
						Logger.Error("[精修路基路面] 放置族实例失败: " + ex7.Message);
						andClearVoidGenerationRequest.OnCompleted?.Invoke((false, "放置族实例失败: " + ex7.Message, null));
					}
				}
				finally
				{
					((IDisposable)val25)?.Dispose();
				}
			}
			finally
			{
				if (val6 != null)
				{
					try
					{
						val6.Close(false);
					}
					catch
					{
					}
				}
			}
		}
		catch (Exception ex8)
		{
			Logger.Error("[精修路基路面] 执行失败: " + ex8.Message);
			TaskDialog.Show("错误", "空心生成失败: " + ex8.Message);
		}
	}

	private double method_0(Document document_0, string string_0)
	{
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		try
		{
			Logger.Info("[精修路基路面] 开始查找族: " + string_0);
			FilteredElementCollector val = new FilteredElementCollector(document_0);
			val.OfClass(typeof(FamilyInstance));
			double num = double.MaxValue;
			double num2 = double.MinValue;
			int num3 = 0;
			foreach (Element item in val)
			{
				FamilyInstance val2 = (FamilyInstance)(object)((item is FamilyInstance) ? item : null);
				if (val2 == null)
				{
					continue;
				}
				Family family = val2.Symbol.Family;
				if (family != null && ((Element)family).Name == string_0)
				{
					num3++;
					BoundingBoxXYZ val3 = ((Element)val2).get_BoundingBox((View)null);
					if (val3 != null)
					{
						num = Math.Min(num, val3.Min.Z);
						num2 = Math.Max(num2, val3.Max.Z);
					}
				}
			}
			if (num3 == 0)
			{
				Logger.Warning("[精修路基路面] 未找到族 " + string_0 + " 的实例");
				return -1.0;
			}
			double num4 = num2 - num;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(39, 5);
			defaultInterpolatedStringHandler.AppendLiteral("[精修路基路面] 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(num3);
			defaultInterpolatedStringHandler.AppendLiteral(" 个 ");
			defaultInterpolatedStringHandler.AppendFormatted(string_0);
			defaultInterpolatedStringHandler.AppendLiteral(" 实例，Z 范围: ");
			defaultInterpolatedStringHandler.AppendFormatted(num * 304.8, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(" - ");
			defaultInterpolatedStringHandler.AppendFormatted(num2 * 304.8, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(" 毫米，高度: ");
			defaultInterpolatedStringHandler.AppendFormatted(num4 * 304.8, "F2");
			defaultInterpolatedStringHandler.AppendLiteral(" 毫米");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return num4;
		}
		catch (Exception ex)
		{
			Logger.Error("[精修路基路面] 获取族高度范围失败: " + ex.Message);
			return -1.0;
		}
	}

	private Tuple<double, double>? method_1(Document document_0, string string_0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(document_0);
			val.OfClass(typeof(FamilyInstance));
			double num = double.MaxValue;
			double num2 = double.MinValue;
			int num3 = 0;
			foreach (Element item in val)
			{
				FamilyInstance val2 = (FamilyInstance)(object)((item is FamilyInstance) ? item : null);
				if (val2 == null)
				{
					continue;
				}
				Family family = val2.Symbol.Family;
				if (family != null && ((Element)family).Name == string_0)
				{
					num3++;
					BoundingBoxXYZ val3 = ((Element)val2).get_BoundingBox((View)null);
					if (val3 != null)
					{
						num = Math.Min(num, val3.Min.Z);
						num2 = Math.Max(num2, val3.Max.Z);
					}
				}
			}
			if (num3 > 0)
			{
				return new Tuple<double, double>(num, num2);
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[精修路基路面] 获取族 Z 范围失败: " + ex.Message);
			return null;
		}
	}

	private string? method_2()
	{
		string location = Assembly.GetExecutingAssembly().Location;
		string directoryName = Path.GetDirectoryName(location);
		if (directoryName != null)
		{
			string[] obj = new string[4]
			{
				"Resources\\FamilyTemplates\\公制常规模型.rft",
				null,
				null,
				null
			};
			InlineArray6<string> gparam_ = default(InlineArray6<string>);
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 0) = directoryName;
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 1) = "..";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 2) = "..";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 3) = "Resources";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 4) = "FamilyTemplates";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_, 5) = "公制常规模型.rft";
			obj[1] = Path.Combine(Class653.smethod_1<InlineArray6<string>, string>(in gparam_, 6));
			InlineArray7<string> gparam_2 = default(InlineArray7<string>);
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 0) = directoryName;
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 1) = "..";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 2) = "..";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 3) = "..";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 4) = "06_Resources";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 5) = "FamilyTemplates";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_2, 6) = "公制常规模型.rft";
			obj[2] = Path.Combine(Class653.smethod_1<InlineArray7<string>, string>(in gparam_2, 7));
			InlineArray5<string> gparam_3 = default(InlineArray5<string>);
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_3, 0) = directoryName;
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_3, 1) = "..";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_3, 2) = "Resources";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_3, 3) = "FamilyTemplates";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_3, 4) = "公制常规模型.rft";
			obj[3] = Path.Combine(Class653.smethod_1<InlineArray5<string>, string>(in gparam_3, 5));
			string[] array = obj;
			string[] array2 = array;
			foreach (string path in array2)
			{
				try
				{
					string fullPath = Path.GetFullPath(path);
					if (File.Exists(fullPath))
					{
						Logger.Info("[精修路基路面] 找到模板文件: " + fullPath);
						return fullPath;
					}
				}
				catch
				{
				}
			}
		}
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
		InlineArray6<string> gparam_4 = default(InlineArray6<string>);
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 0) = folderPath;
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 1) = "Autodesk";
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 2) = "Revit";
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 3) = "Autodesk Revit 2025";
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 4) = "Family Templates";
		Class653.smethod_2<InlineArray6<string>, string>(ref gparam_4, 5) = "Chinese";
		folderPath = Path.Combine(Class653.smethod_1<InlineArray6<string>, string>(in gparam_4, 6));
		if (Directory.Exists(folderPath))
		{
			string[] files = Directory.GetFiles(folderPath, "*.rft", SearchOption.AllDirectories);
			string text = files.FirstOrDefault((string string_0) => string_0.Contains("公制常规模型") || string_0.Contains("Metric Generic Model", StringComparison.OrdinalIgnoreCase));
			if (!string.IsNullOrEmpty(text) && File.Exists(text))
			{
				Logger.Info("[精修路基路面] 使用 Revit 安装目录模板: " + text);
				return text;
			}
		}
		Logger.Error("[精修路基路面] 未找到族模板文件");
		return null;
	}

	private List<CurveLoop>? method_3(Floor floor_0)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Expected O, but got Unknown
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_029f: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a6: Expected O, but got Unknown
		//IL_02a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Expected O, but got Unknown
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fe: Expected O, but got Unknown
		//IL_02c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c7: Expected O, but got Unknown
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Expected O, but got Unknown
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0185: Expected O, but got Unknown
		List<CurveLoop> list = new List<CurveLoop>();
		try
		{
			Options val = new Options();
			val.DetailLevel = (ViewDetailLevel)2;
			val.ComputeReferences = true;
			GeometryElement val2 = ((Element)floor_0).get_Geometry(val);
			if ((GeometryObject)(object)val2 == (GeometryObject)null)
			{
				Logger.Error("[精修路基路面] 无法获取楼板几何元素");
				return null;
			}
			foreach (GeometryObject item in val2)
			{
				Solid val3 = (Solid)(object)((item is Solid) ? item : null);
				if (val3 == null)
				{
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
						if (val5 == null || !(val5.Volume > 0.0))
						{
							continue;
						}
						FaceArray faces = val5.Faces;
						foreach (Face item3 in faces)
						{
							Face val6 = item3;
							XYZ val7 = val6.ComputeNormal(UV.Zero);
							if (!val7.IsAlmostEqualTo(XYZ.BasisZ) && !val7.IsAlmostEqualTo(XYZ.BasisZ.Negate()))
							{
								continue;
							}
							EdgeArrayArray edgeLoops = val6.EdgeLoops;
							foreach (EdgeArray item4 in edgeLoops)
							{
								EdgeArray val8 = item4;
								CurveLoop val9 = new CurveLoop();
								foreach (Edge item5 in val8)
								{
									Edge val10 = item5;
									val9.Append(val10.AsCurve());
								}
								if (!val9.IsOpen())
								{
									list.Add(val9);
								}
							}
							break;
						}
						break;
					}
					continue;
				}
				FaceArray faces2 = val3.Faces;
				foreach (Face item6 in faces2)
				{
					Face val11 = item6;
					XYZ val12 = val11.ComputeNormal(UV.Zero);
					if (!val12.IsAlmostEqualTo(XYZ.BasisZ) && !val12.IsAlmostEqualTo(XYZ.BasisZ.Negate()))
					{
						continue;
					}
					EdgeArrayArray edgeLoops2 = val11.EdgeLoops;
					foreach (EdgeArray item7 in edgeLoops2)
					{
						EdgeArray val13 = item7;
						CurveLoop val14 = new CurveLoop();
						foreach (Edge item8 in val13)
						{
							Edge val15 = item8;
							Curve val16 = val15.AsCurve();
							val14.Append(val16);
						}
						if (!val14.IsOpen())
						{
							list.Add(val14);
						}
					}
					break;
				}
				break;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[精修路基路面] 提取了 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个轮廓环");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return list;
		}
		catch (Exception ex)
		{
			Logger.Error("[精修路基路面] 提取楼板轮廓失败: " + ex.Message);
			return null;
		}
	}

	private ElementId method_4(object object_0)
	{
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		try
		{
			PropertyInfo property = object_0.GetType().GetProperty("Id");
			if (property != null)
			{
				object value = property.GetValue(object_0);
				ElementId val = (ElementId)((value is ElementId) ? value : null);
				if (val != null)
				{
					return val;
				}
				if (value is long num)
				{
					return new ElementId(num);
				}
				if (value is int num2)
				{
					return new ElementId((long)num2);
				}
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[精修路基路面] 获取 ElementId 失败: " + ex.Message);
		}
		throw new InvalidOperationException("无法从元素对象获取 ElementId");
	}

	private string method_5(Document document_0)
	{
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		try
		{
			int num = 0;
			FilteredElementCollector val = new FilteredElementCollector(document_0);
			val.OfClass(typeof(Family));
			foreach (Element item in val)
			{
				Family val2 = (Family)(object)((item is Family) ? item : null);
				if (val2 != null && ((Element)val2).Name.StartsWith("AST_R_楼板空心_"))
				{
					string s = ((Element)val2).Name.Substring("AST_R_楼板空心_".Length);
					if (int.TryParse(s, out var result))
					{
						num = Math.Max(num, result);
					}
				}
			}
			int value = num + 1;
			string result2 = $"{"AST_R_楼板空心_"}{value}";
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[精修路基路面] 找到现有族最大编号: ");
			defaultInterpolatedStringHandler.AppendFormatted(num);
			defaultInterpolatedStringHandler.AppendLiteral("，新编号: ");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return result2;
		}
		catch (Exception ex)
		{
			Logger.Warning("[精修路基路面] 生成族名称时出错，使用默认名称: " + ex.Message);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
			defaultInterpolatedStringHandler2.AppendLiteral("AST_R_楼板空心_");
			defaultInterpolatedStringHandler2.AppendFormatted(Guid.NewGuid(), "N");
			return defaultInterpolatedStringHandler2.ToStringAndClear();
		}
	}

	public string GetName()
	{
		return "路面精修空心生成";
	}
}
