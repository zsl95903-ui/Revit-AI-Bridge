using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Revit;

[Transaction(TransactionMode.Manual)]
[Regeneration(RegenerationOption.Manual)]
public class RoadSurfaceCutExternalEventHandler : IExternalEventHandler
{
	public void Execute(UIApplication app)
	{
		//IL_085b: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0803: Unknown result type (might be due to invalid IL or missing references)
		//IL_07f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0684: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Expected O, but got Unknown
		//IL_068d: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0797: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0374: Unknown result type (might be due to invalid IL or missing references)
		//IL_037b: Expected O, but got Unknown
		//IL_0652: Unknown result type (might be due to invalid IL or missing references)
		//IL_0659: Expected O, but got Unknown
		//IL_065b: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Expected O, but got Unknown
		try
		{
			RoadSurfaceCutRequest andClearCutRequest = RoadSurfaceRefinementRequestManager.GetAndClearCutRequest();
			if (andClearCutRequest == null)
			{
				TaskDialog.Show("提示", "无效的剪切请求");
				return;
			}
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "请打开一个文档" }));
				return;
			}
			if (andClearCutRequest.VoidInstances == null || andClearCutRequest.VoidInstances.Count == 0)
			{
				Logger.Warning("[精修路基路面] 未选择空心族实例");
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "未选择空心族实例" }));
				return;
			}
			object object_ = andClearCutRequest.VoidInstances[0];
			ElementId val2 = method_0(object_);
			if (val2 == ElementId.InvalidElementId)
			{
				Logger.Warning("[精修路基路面] 无法获取空心族实例 ID");
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "无法获取空心族实例 ID" }));
				return;
			}
			Element element = val.GetElement(val2);
			if (element == null)
			{
				Logger.Warning("[精修路基路面] 无法获取空心族实例元素");
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "无法获取空心族实例元素" }));
				return;
			}
			if (andClearCutRequest.CategoryNames == null || andClearCutRequest.CategoryNames.Count == 0)
			{
				Logger.Warning("[精修路基路面] 未选择要剪切的族类型");
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "未选择要剪切的族类型" }));
				return;
			}
			int value = andClearCutRequest.SelectedCategoryInstances?.Count ?? 0;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 3);
			defaultInterpolatedStringHandler.AppendLiteral("[精修路基路面] 开始剪切: ");
			defaultInterpolatedStringHandler.AppendFormatted(value);
			defaultInterpolatedStringHandler.AppendLiteral(" 个选中实例, ");
			defaultInterpolatedStringHandler.AppendFormatted(andClearCutRequest.CutAllInstances ? "扩展到同类型所有实例" : "仅剪切选中实例");
			defaultInterpolatedStringHandler.AppendLiteral(", 类别: ");
			defaultInterpolatedStringHandler.AppendFormatted(string.Join(", ", andClearCutRequest.CategoryNames));
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			List<FamilyInstance> list = new List<FamilyInstance>();
			HashSet<long> hashSet = new HashSet<long>();
			if (andClearCutRequest.SelectedCategoryInstances != null)
			{
				foreach (object selectedCategoryInstance in andClearCutRequest.SelectedCategoryInstances)
				{
					ElementId val3 = method_0(selectedCategoryInstance);
					if (val3 != ElementId.InvalidElementId)
					{
						hashSet.Add(val3.Value);
					}
				}
			}
			Logger.Info("[精修路基路面] 用户选择的实例 ID: [" + string.Join(", ", hashSet) + "]");
			foreach (string categoryName in andClearCutRequest.CategoryNames)
			{
				Category val4 = method_1(val, categoryName);
				if (val4 == null)
				{
					Logger.Warning("[精修路基路面] 找不到类别: " + categoryName);
					continue;
				}
				FilteredElementCollector val5 = new FilteredElementCollector(val);
				val5.OfCategory((BuiltInCategory)val4.Id.Value);
				val5.OfClass(typeof(FamilyInstance));
				foreach (Element item in val5)
				{
					FamilyInstance val6 = (FamilyInstance)(object)((item is FamilyInstance) ? item : null);
					if (val6 != null)
					{
						long value2 = ((Element)val6).Id.Value;
						if (hashSet.Contains(value2))
						{
							list.Add(val6);
						}
					}
				}
			}
			if (list.Count == 0)
			{
				Logger.Warning("[精修路基路面] 未找到要剪切的元素");
				andClearCutRequest.OnCompleted?.Invoke((0, new List<string> { "未找到要剪切的元素" }));
				return;
			}
			if (andClearCutRequest.CutAllInstances)
			{
				HashSet<ElementId> hashSet2 = new HashSet<ElementId>();
				foreach (FamilyInstance item2 in list)
				{
					ElementId typeId = ((Element)item2).GetTypeId();
					if (typeId != (ElementId)null && typeId != ElementId.InvalidElementId)
					{
						hashSet2.Add(typeId);
					}
				}
				FilteredElementCollector val7 = new FilteredElementCollector(val);
				val7.OfClass(typeof(FamilyInstance));
				List<FamilyInstance> list2 = new List<FamilyInstance>();
				foreach (Element item3 in val7)
				{
					FamilyInstance val8 = (FamilyInstance)(object)((item3 is FamilyInstance) ? item3 : null);
					if (val8 != null)
					{
						ElementId typeId2 = ((Element)val8).GetTypeId();
						if (typeId2 != (ElementId)null && typeId2 != ElementId.InvalidElementId && hashSet2.Contains(typeId2))
						{
							list2.Add(val8);
						}
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(31, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("[精修路基路面] 扩展: ");
				defaultInterpolatedStringHandler2.AppendFormatted(hashSet2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个类型, ");
				defaultInterpolatedStringHandler2.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个选中 → ");
				defaultInterpolatedStringHandler2.AppendFormatted(list2.Count);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个总实例");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
				list = list2;
			}
			else
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(20, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("[精修路基路面] 仅剪切选中的 ");
				defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler3.AppendLiteral(" 个实例");
				Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
			int num = 0;
			int num2 = 0;
			List<string> list3 = new List<string>();
			Transaction val9 = new Transaction(val, "批量剪切");
			try
			{
				val9.Start();
				CutFailureReason value3 = default(CutFailureReason);
				bool flag2 = default(bool);
				foreach (FamilyInstance item4 in list)
				{
					bool flag = false;
					string text = string.Empty;
					SubTransaction val10 = new SubTransaction(val);
					try
					{
						val10.Start();
						try
						{
							FamilyInstance val11 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
							if (val11 != null)
							{
								try
								{
									if (InstanceVoidCutUtils.IsVoidInstanceCuttingElement((Element)(object)val11))
									{
										if (InstanceVoidCutUtils.CanBeCutWithVoid((Element)(object)item4))
										{
											InstanceVoidCutUtils.AddInstanceVoidCut(val, (Element)(object)item4, (Element)(object)val11);
											flag = true;
										}
										else
										{
											text = "无法用空心剪切";
										}
									}
									else
									{
										text = "剪切元素不包含空心";
									}
								}
								catch (Exception ex)
								{
									text = "空心剪切失败: " + ex.Message;
								}
							}
							if (!flag)
							{
								try
								{
									if (SolidSolidCutUtils.CanElementCutElement(element, (Element)(object)item4, out value3))
									{
										if (!SolidSolidCutUtils.CutExistsBetweenElements(element, (Element)(object)item4, out flag2))
										{
											SolidSolidCutUtils.AddCutBetweenSolids(val, element, (Element)(object)item4);
											flag = true;
										}
										else
										{
											flag = true;
										}
									}
									else if (!string.IsNullOrEmpty(text))
									{
										string text2 = text;
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
										defaultInterpolatedStringHandler4.AppendLiteral(", 实心剪切: ");
										defaultInterpolatedStringHandler4.AppendFormatted<CutFailureReason>(value3);
										text = text2 + defaultInterpolatedStringHandler4.ToStringAndClear();
									}
									else
									{
										DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(10, 1);
										defaultInterpolatedStringHandler5.AppendLiteral("无法进行实心剪切: ");
										defaultInterpolatedStringHandler5.AppendFormatted<CutFailureReason>(value3);
										text = defaultInterpolatedStringHandler5.ToStringAndClear();
									}
								}
								catch (Exception ex2)
								{
									text = (string.IsNullOrEmpty(text) ? ("实心剪切失败: " + ex2.Message) : (text + ", 实心剪切异常: " + ex2.Message));
								}
							}
							if (flag)
							{
								val10.Commit();
								num++;
								continue;
							}
							val10.RollBack();
							num2++;
							list3.Add(((Element)item4).Name + ": " + text);
							Logger.Warning("[精修路基路面] 剪切失败: " + ((Element)item4).Name + ", 原因: " + text);
						}
						catch (Exception ex3)
						{
							val10.RollBack();
							num2++;
							list3.Add(((Element)item4).Name + ": " + ex3.Message);
							Logger.Warning("[精修路基路面] 剪切异常: " + ((Element)item4).Name + ", 原因: " + ex3.Message);
						}
					}
					finally
					{
						((IDisposable)val10)?.Dispose();
					}
				}
				val9.Commit();
			}
			finally
			{
				((IDisposable)val9)?.Dispose();
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(22, 3);
			defaultInterpolatedStringHandler6.AppendLiteral("[精修路基路面] 完成: 成功 ");
			defaultInterpolatedStringHandler6.AppendFormatted(num);
			defaultInterpolatedStringHandler6.AppendLiteral("/");
			defaultInterpolatedStringHandler6.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler6.AppendLiteral(", 失败 ");
			defaultInterpolatedStringHandler6.AppendFormatted(num2);
			Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
			andClearCutRequest.OnCompleted?.Invoke((num, list3));
		}
		catch (Exception ex4)
		{
			Logger.Error("[精修路基路面] 剪切操作失败: " + ex4.Message);
			TaskDialog.Show("错误", "剪切操作失败: " + ex4.Message);
		}
	}

	private ElementId method_0(object object_0)
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
		return ElementId.InvalidElementId;
	}

	private Category? method_1(Document document_0, string string_0)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Expected O, but got Unknown
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Expected O, but got Unknown
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(document_0);
			val.WhereElementIsNotElementType();
			foreach (Element item in val)
			{
				Category category = item.Category;
				if (category != null && category.Name == string_0)
				{
					return category;
				}
			}
			Categories categories = document_0.Settings.Categories;
			foreach (Category item2 in (CategoryNameMap)categories)
			{
				Category val2 = item2;
				if (val2.Name == string_0)
				{
					return val2;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[精修路基路面] 获取类别失败: " + string_0 + ", " + ex.Message);
			return null;
		}
	}

	public string GetName()
	{
		return "路面精修剪切操作";
	}
}
