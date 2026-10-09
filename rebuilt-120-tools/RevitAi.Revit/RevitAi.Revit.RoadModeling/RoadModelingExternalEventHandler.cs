using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using RevitAi.Revit.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.RoadModeling;

public sealed class RoadModelingExternalEventHandler : IExternalEventHandler
{
	[CompilerGenerated]
	public sealed class Class337
	{
		public string string_0;

		internal bool method_0(Family family_0)
		{
			return ((Element)family_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[DebuggerBrowsable(DebuggerBrowsableState.Never)]
	[CompilerGenerated]
	private static RoadModelingExternalEventRequest? roadModelingExternalEventRequest_0;

	public static RoadModelingExternalEventRequest? CurrentRequest
	{
		[CompilerGenerated]
		get
		{
			return roadModelingExternalEventRequest_0;
		}
		[CompilerGenerated]
		set
		{
			roadModelingExternalEventRequest_0 = value;
		}
	}

	public string GetName()
	{
		return "Road Modeling";
	}

	public void Execute(UIApplication app)
	{
		ILogger logger = ServiceProvider.GetLogger();
		RoadModelingExternalEventRequest currentRequest = CurrentRequest;
		if (currentRequest == null)
		{
			logger.Error("[RoadModelingExternalEventHandler] 请求为空", (Exception)null);
			return;
		}
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null)
			{
				logger.Error("[RoadModelingExternalEventHandler] 无法获取文档", (Exception)null);
				currentRequest.CompletionSource?.TrySetException(new InvalidOperationException("无法获取 Revit 文档"));
				return;
			}
			if (currentRequest.IsAncillaryStructure)
			{
				foreach (var (text3, text4) in currentRequest.DownloadedFamilyPaths)
				{
					if (!string.IsNullOrEmpty(text4) && File.Exists(text4))
					{
						smethod_0(val, text3, text4, logger);
					}
					else
					{
						logger.Warning("[RoadModelingExternalEventHandler] 族文件不存在: " + text3);
					}
				}
			}
			else if (!string.IsNullOrEmpty(currentRequest.FamilyFilePath) && File.Exists(currentRequest.FamilyFilePath))
			{
				string string_ = currentRequest.FamilyName ?? "Unknown";
				string familyFilePath = currentRequest.FamilyFilePath;
				smethod_0(val, string_, familyFilePath, logger);
			}
			RoadModelingService roadModelingService = new RoadModelingService();
			Task<Result<RoadModelingResult>> task = ((!currentRequest.IsAncillaryStructure) ? roadModelingService.CreateRoadModelAsync(val, currentRequest.Project, currentRequest.SplitAtIntegerStations, currentRequest.IntegerStationInterval) : roadModelingService.CreateAncillaryStructureAsync(val, currentRequest.Project));
			task.Wait();
			Result<RoadModelingResult> result = task.Result;
			if (result.IsSuccess && result.Value != null)
			{
				currentRequest.CompletionSource?.TrySetResult(result.Value);
			}
			else
			{
				currentRequest.CompletionSource?.TrySetException(new Exception(result.Error ?? "建模失败"));
			}
			logger.Info("[RoadModelingExternalEventHandler] 建模操作完成");
		}
		catch (Exception ex)
		{
			logger.Error("[RoadModelingExternalEventHandler] 执行失败", ex);
			currentRequest.CompletionSource?.TrySetException(ex);
		}
		finally
		{
			CurrentRequest = null;
		}
	}

	private static void smethod_0(Document document_0, string string_0, string string_1, ILogger ilogger_0)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		Family val = ((IEnumerable)new FilteredElementCollector(document_0).OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family family_0) => ((Element)family_0).Name.Equals(string_0, StringComparison.OrdinalIgnoreCase));
		if (val != null)
		{
			return;
		}
		Transaction val2 = new Transaction(document_0, "载入族");
		try
		{
			val2.Start();
			string text = string.Join("_", string_0.Split(Path.GetInvalidFileNameChars()));
			string text2 = Path.Combine(Path.GetTempPath(), text + ".rfa");
			try
			{
				File.Copy(string_1, text2, overwrite: true);
				Family val3 = default(Family);
				if (document_0.LoadFamily(text2, out val3))
				{
					ilogger_0.Info("[RoadModelingExternalEventHandler] 族加载成功: " + string_0);
					val2.Commit();
				}
				else
				{
					ilogger_0.Warning("[RoadModelingExternalEventHandler] 族加载失败: " + string_0);
					val2.RollBack();
				}
			}
			catch
			{
				val2.RollBack();
				throw;
			}
			finally
			{
				if (File.Exists(text2))
				{
					try
					{
						File.Delete(text2);
					}
					catch
					{
					}
				}
			}
		}
		finally
		{
			((IDisposable)val2)?.Dispose();
		}
	}
}
