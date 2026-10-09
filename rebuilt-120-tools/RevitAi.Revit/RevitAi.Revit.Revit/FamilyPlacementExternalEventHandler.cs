using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using ns6;

using InvalidOperationException = System.InvalidOperationException;
namespace RevitAi.Revit.Revit;

public sealed class FamilyPlacementExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Family Placement";
	}

	public void Execute(UIApplication app)
	{
		//IL_0393: Expected O, but got Unknown
		//IL_0302: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01db: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0293: Unknown result type (might be due to invalid IL or missing references)
		FamilyPlacementRequest andClearRequest = FamilyPlacementRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			return;
		}
		string text = null;
		FamilySymbol val = null;
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val2 = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val2 == null || activeUIDocument == null)
			{
				andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("没有活动文档"));
				return;
			}
			if (!File.Exists(andClearRequest.FilePath))
			{
				andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("族文件不存在: " + andClearRequest.FilePath));
				return;
			}
			string text2 = string.Join("_", andClearRequest.FamilyName.Split(Path.GetInvalidFileNameChars()));
			if (string.IsNullOrWhiteSpace(text2))
			{
				text2 = Guid.NewGuid().ToString("N");
			}
			text = Path.Combine(Path.GetTempPath(), text2 + ".rfa");
			try
			{
				File.Copy(andClearRequest.FilePath, text, overwrite: true);
				Logger.Info("[FamilyPlacementExternalEventHandler] 复制文件到临时位置: " + text);
			}
			catch (Exception ex)
			{
				andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("复制文件失败: " + ex.Message));
				return;
			}
			Transaction val3 = new Transaction(val2, "RevitAi_载入并准备放置");
			try
			{
				val3.Start();
				try
				{
					FamilyPlacementLoadOptions familyPlacementLoadOptions = new FamilyPlacementLoadOptions();
					Family val4 = null;
					if (!val2.LoadFamily(text, (IFamilyLoadOptions)(object)familyPlacementLoadOptions, out val4) || val4 == null)
					{
						val3.RollBack();
						Logger.Warning("[FamilyPlacementExternalEventHandler] 族加载失败: " + andClearRequest.FamilyName);
						andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("族加载失败"));
						return;
					}
					ISet<ElementId> familySymbolIds = val4.GetFamilySymbolIds();
					if (familySymbolIds == null || familySymbolIds.Count == 0)
					{
						val3.RollBack();
						Logger.Warning("[FamilyPlacementExternalEventHandler] 族没有可用的类型: " + andClearRequest.FamilyName);
						andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("族没有可用的类型"));
						return;
					}
					ElementId val5 = familySymbolIds.First();
					Element element = val2.GetElement(val5);
					val = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
					if (val == null)
					{
						val3.RollBack();
						Logger.Warning("[FamilyPlacementExternalEventHandler] 无法获取族类型: " + andClearRequest.FamilyName);
						andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("无法获取族类型"));
						return;
					}
					if (!val.IsActive)
					{
						val.Activate();
						val2.Regenerate();
					}
					val3.Commit();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(55, 2);
					defaultInterpolatedStringHandler.AppendLiteral("[FamilyPlacementExternalEventHandler] 族加载并激活成功: ");
					defaultInterpolatedStringHandler.AppendFormatted(andClearRequest.FamilyName);
					defaultInterpolatedStringHandler.AppendLiteral(" (类型: ");
					defaultInterpolatedStringHandler.AppendFormatted(((Element)val).Name);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				catch (Exception ex2)
				{
					val3.RollBack();
					Logger.Error("[FamilyPlacementExternalEventHandler] 族加载异常: " + andClearRequest.FamilyName, ex2);
					andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed(ex2.Message));
					return;
				}
			}
			finally
			{
				((IDisposable)val3)?.Dispose();
			}
			if (val == null)
			{
				return;
			}
			try
			{
				activeUIDocument.PromptForFamilyInstancePlacement(val);
				Logger.Info("[FamilyPlacementExternalEventHandler] 已进入放置模式");
				andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Succeeded(((Element)val).Id.Value.ToString()));
			}
			catch (InvalidOperationException ex3)
			{
				InvalidOperationException ex4 = ex3;
				Logger.Warning("[FamilyPlacementExternalEventHandler] 当前视图无法放置该族构件: " + ((Exception)(object)ex4).Message);
				andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed("当前视图无法放置该族构件，请切换到平面或三维视图"));
			}
		}
		catch (Exception ex5)
		{
			Logger.Error("[FamilyPlacementExternalEventHandler] 处理请求异常", ex5);
			andClearRequest.TaskSource.SetResult(FamilyPlacementResult.Failed(ex5.Message));
		}
		finally
		{
			if (text != null && File.Exists(text))
			{
				try
				{
					File.Delete(text);
					Logger.Debug("[FamilyPlacementExternalEventHandler] 已删除临时文件: " + text);
				}
				catch (Exception ex6)
				{
					Logger.Warning("[FamilyPlacementExternalEventHandler] 删除临时文件失败: " + ex6.Message);
				}
			}
		}
	}
}
