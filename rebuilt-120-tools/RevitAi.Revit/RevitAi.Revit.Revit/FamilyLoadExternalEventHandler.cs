using System;
using System.IO;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Revit;

public sealed class FamilyLoadExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Family Load";
	}

	public void Execute(UIApplication app)
	{
		//IL_0249: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Expected O, but got Unknown
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016d: Unknown result type (might be due to invalid IL or missing references)
		FamilyLoadRequest andClearRequest = FamilyLoadRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			return;
		}
		string text = null;
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed("没有活动文档"));
				return;
			}
			if (!File.Exists(andClearRequest.FilePath))
			{
				andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed("族文件不存在: " + andClearRequest.FilePath));
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
				Logger.Info("[FamilyLoadExternalEventHandler] 复制文件到临时位置: " + text);
			}
			catch (Exception ex)
			{
				andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed("复制文件失败: " + ex.Message));
				return;
			}
			Transaction val2 = new Transaction(val, "加载在线族库族文件");
			try
			{
				val2.Start("LoadFamily");
				try
				{
					FamilyLoadOptions familyLoadOptions = new FamilyLoadOptions();
					Family val3 = null;
					bool flag = val.LoadFamily(text, (IFamilyLoadOptions)(object)familyLoadOptions, out val3);
					val2.Commit();
					if (flag && val3 != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(47, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[FamilyLoadExternalEventHandler] 族加载成功: ");
						defaultInterpolatedStringHandler.AppendFormatted(andClearRequest.FamilyName);
						defaultInterpolatedStringHandler.AppendLiteral(" (ID: ");
						defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val3).Id);
						defaultInterpolatedStringHandler.AppendLiteral(")");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						andClearRequest.TaskSource.SetResult(FamilyLoadResult.Succeeded(((Element)val3).Id.Value.ToString()));
					}
					else
					{
						Logger.Warning("[FamilyLoadExternalEventHandler] 族加载失败: " + andClearRequest.FamilyName);
						andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed("族加载失败"));
					}
				}
				catch (Exception ex2)
				{
					val2.RollBack();
					Logger.Error("[FamilyLoadExternalEventHandler] 族加载异常: " + andClearRequest.FamilyName, ex2);
					andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed(ex2.Message));
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		catch (Exception ex3)
		{
			Logger.Error("[FamilyLoadExternalEventHandler] 处理请求异常", ex3);
			andClearRequest.TaskSource.SetResult(FamilyLoadResult.Failed(ex3.Message));
		}
		finally
		{
			if (text != null && File.Exists(text))
			{
				try
				{
					File.Delete(text);
					Logger.Debug("[FamilyLoadExternalEventHandler] 已删除临时文件: " + text);
				}
				catch (Exception ex4)
				{
					Logger.Warning("[FamilyLoadExternalEventHandler] 删除临时文件失败: " + ex4.Message);
				}
			}
		}
	}
}
