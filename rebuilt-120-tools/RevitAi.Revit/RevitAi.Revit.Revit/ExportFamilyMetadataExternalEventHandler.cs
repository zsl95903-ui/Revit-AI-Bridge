using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Revit;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Revit;

public sealed class ExportFamilyMetadataExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Export Family Metadata";
	}

	public void Execute(UIApplication app)
	{
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020e: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_010e: Expected O, but got Unknown
		ExportFamilyMetadataRequest andClearRequest = ExportFamilyMetadataRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			method_3("[ExportFamilyMetadataExternalEventHandler] 没有待处理的请求");
			return;
		}
		try
		{
			method_2("[ExportFamilyMetadataExternalEventHandler] 开始处理族元数据导出请求");
			UIDocument activeUIDocument = app.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 没有活动文档");
				ExportFamilyMetadataResult obj = new ExportFamilyMetadataResult
				{
					SuccessCount = 0,
					FailCount = andClearRequest.Families.Count,
					SkippedCount = 0,
					Errors = new List<string> { "没有活动文档" }
				};
				andClearRequest.OnCompleted?.Invoke(obj);
				return;
			}
			Document document = activeUIDocument.Document;
			IFamilyMetadataService val = method_1(app);
			if (val == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 无法获取族元数据服务");
				ExportFamilyMetadataResult obj2 = new ExportFamilyMetadataResult
				{
					SuccessCount = 0,
					FailCount = andClearRequest.Families.Count,
					SkippedCount = 0,
					Errors = new List<string> { "无法获取族元数据服务" }
				};
				andClearRequest.OnCompleted?.Invoke(obj2);
			}
			else
			{
				ExportFamilyMetadataResult val2 = method_0(val, document, andClearRequest.Families, andClearRequest.ExportDirectory, andClearRequest.SaveFamilyFile, andClearRequest.ExportThumbnail, andClearRequest.ExportMetadata, andClearRequest.Overwrite, andClearRequest.OnProgress);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(64, 3);
				defaultInterpolatedStringHandler.AppendLiteral("[ExportFamilyMetadataExternalEventHandler] 导出完成，成功: ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.SuccessCount);
				defaultInterpolatedStringHandler.AppendLiteral(", 失败: ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.FailCount);
				defaultInterpolatedStringHandler.AppendLiteral(", 跳过: ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.SkippedCount);
				method_2(defaultInterpolatedStringHandler.ToStringAndClear());
				andClearRequest.OnCompleted?.Invoke(val2);
			}
		}
		catch (Exception ex)
		{
			method_4("[ExportFamilyMetadataExternalEventHandler] 执行失败: " + ex.Message);
			ExportFamilyMetadataResult obj3 = new ExportFamilyMetadataResult
			{
				SuccessCount = 0,
				FailCount = andClearRequest.Families.Count,
				SkippedCount = 0,
				Errors = new List<string> { ex.Message }
			};
			andClearRequest.OnCompleted?.Invoke(obj3);
		}
	}

	private ExportFamilyMetadataResult method_0(IFamilyMetadataService ifamilyMetadataService_0, Document document_0, List<FamilyExportItem> list_0, string string_0, bool bool_0, bool bool_1, bool bool_2, bool bool_3, Action<int, int, string>? action_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Expected O, but got Unknown
		ExportFamilyMetadataResult val = new ExportFamilyMetadataResult
		{
			SuccessCount = 0,
			FailCount = 0,
			SkippedCount = 0,
			Errors = new List<string>()
		};
		for (int i = 0; i < list_0.Count; i++)
		{
			FamilyExportItem val2 = list_0[i];
			if (action_0 != null)
			{
				int arg = i + 1;
				int count = list_0.Count;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 3);
				defaultInterpolatedStringHandler.AppendLiteral("正在导出 (");
				defaultInterpolatedStringHandler.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral("): ");
				defaultInterpolatedStringHandler.AppendFormatted(val2.FamilyName);
				action_0(arg, count, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			try
			{
				FamilyMetadataExportResult val3 = ((!val2.IsExternalFamily) ? ifamilyMetadataService_0.ExportFamilyComplete((object)document_0, val2.Family, string_0, bool_0, bool_1, bool_2, bool_3) : ifamilyMetadataService_0.ExportFamilyFile(val2.FilePath, string_0, bool_0, bool_1, bool_2, bool_3));
				int successCount;
				if (val3.IsSuccess)
				{
					successCount = val.SuccessCount;
					val.SuccessCount = successCount + 1;
					continue;
				}
				string errorMessage = val3.ErrorMessage;
				if (errorMessage != null && errorMessage.Contains("已存在"))
				{
					goto IL_0157;
				}
				string errorMessage2 = val3.ErrorMessage;
				if (errorMessage2 != null && errorMessage2.Contains("存在"))
				{
					goto IL_0157;
				}
				successCount = val.FailCount;
				val.FailCount = successCount + 1;
				val.Errors.Add(val2.FamilyName + ": " + val3.ErrorMessage);
				goto end_IL_00bb;
				IL_0157:
				successCount = val.SkippedCount;
				val.SkippedCount = successCount + 1;
				end_IL_00bb:;
			}
			catch (Exception ex)
			{
				method_5("[ExportFamilyMetadataExternalEventHandler] 导出异常: " + val2.FamilyName, ex);
				int successCount = val.FailCount;
				val.FailCount = successCount + 1;
				val.Errors.Add(val2.FamilyName + ": " + ex.Message);
			}
		}
		return val;
	}

	private IFamilyMetadataService? method_1(UIApplication uiapplication_0)
	{
		try
		{
			Type type = Type.GetType("RevitAi.Abstractions.Loader.ServiceProvider, RevitAi.Abstractions");
			if (type == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 无法获取 ServiceProvider 类型");
				return null;
			}
			MethodInfo method = type.GetMethod("GetModuleLoader", BindingFlags.Static | BindingFlags.Public);
			if (method == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 无法获取 GetModuleLoader 方法");
				return null;
			}
			object obj = method.Invoke(null, null);
			if (obj == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] ModuleLoader 为 null");
				return null;
			}
			PropertyInfo property = obj.GetType().GetProperty("RevitAdapter");
			if (property == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 无法获取 RevitAdapter 属性");
				return null;
			}
			object value = property.GetValue(obj);
			if (value == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] RevitAdapter 为 null");
				return null;
			}
			PropertyInfo property2 = value.GetType().GetProperty("FamilyMetadataService");
			if (property2 == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] 无法获取 FamilyMetadataService 属性");
				return null;
			}
			object? value2 = property2.GetValue(value);
			IFamilyMetadataService val = (IFamilyMetadataService)((value2 is IFamilyMetadataService) ? value2 : null);
			if (val == null)
			{
				method_4("[ExportFamilyMetadataExternalEventHandler] FamilyMetadataService 为 null");
				return null;
			}
			return val;
		}
		catch (Exception ex)
		{
			method_4("[ExportFamilyMetadataExternalEventHandler] 获取族元数据服务失败: " + ex.Message);
			return null;
		}
	}

	private void method_2(string string_0)
	{
		try
		{
			Logger.Info("[ExportFamilyMetadataExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}

	private void method_3(string string_0)
	{
		try
		{
			Logger.Warning("[ExportFamilyMetadataExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}

	private void method_4(string string_0)
	{
		try
		{
			Logger.Error("[ExportFamilyMetadataExternalEventHandler] " + string_0);
		}
		catch
		{
		}
	}

	private void method_5(string string_0, Exception exception_0)
	{
		try
		{
			Logger.Error("[ExportFamilyMetadataExternalEventHandler] " + string_0, exception_0);
		}
		catch
		{
		}
	}
}
