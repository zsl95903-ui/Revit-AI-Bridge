using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.MEP;

public sealed class InsulationExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "保温工具操作";
	}

	public void Execute(UIApplication app)
	{
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Expected O, but got Unknown
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Expected O, but got Unknown
		//IL_00b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected I4, but got Unknown
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		InsulationRequest andClearRequest = InsulationRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			Logger.Warning("[InsulationExternalEventHandler] 没有待执行的请求");
			return;
		}
		try
		{
			UIDocument activeUIDocument = app.ActiveUIDocument;
			object document = andClearRequest.Document;
			Document val = (Document)(((document is Document) ? document : null) ?? ((activeUIDocument != null) ? activeUIDocument.Document : null));
			if (val == null)
			{
				smethod_0(andClearRequest, new InsulationRequestResult
				{
					Error = "无法获取文档"
				});
				return;
			}
			IModuleLoader moduleLoader = ServiceProvider.GetModuleLoader();
			IRevitAdapter val2 = ((moduleLoader != null) ? moduleLoader.RevitAdapter : null);
			IInsulationService val3 = ((val2 != null) ? val2.InsulationService : null);
			if (val3 == null)
			{
				smethod_0(andClearRequest, new InsulationRequestResult
				{
					Error = "无法获取保温服务"
				});
				return;
			}
			InsulationRequestResult val4 = new InsulationRequestResult();
			InsulationRequestType requestType = andClearRequest.RequestType;
			InsulationRequestType val5 = requestType;
			switch ((int)val5)
			{
			default:
				val4.Error = "未知请求类型";
				break;
			case 0:
				val4.InsulationTypes = val3.LoadAllInsulationTypes((object)val);
				val4.Systems = val3.LoadAllSystemData((object)val);
				break;
			case 1:
				val4.SizeRanges = val3.GetSizeRanges((object)val, andClearRequest.TargetSystem);
				break;
			case 2:
				val4.Summary = val3.ExecuteSystemAdd((object)val, andClearRequest.SelectedSystems);
				break;
			case 3:
				val4.Summary = val3.ExecuteSizeBasedAdd((object)val, andClearRequest.TargetSystem, andClearRequest.SizeRanges);
				break;
			case 4:
				val4.Summary = val3.ExecuteManualAdd((object)val, andClearRequest.ManualElements, andClearRequest.ManualThicknessMM, andClearRequest.ManualMaterial);
				break;
			case 5:
			{
				int count = default(int);
				val4.Error = val3.IsolateSystem((object)val, andClearRequest.TargetSystem, out count);
				val4.Count = count;
				break;
			}
			case 6:
				val4.Error = val3.IsolateElements(andClearRequest.ElementIds ?? new List<int>());
				break;
			case 7:
				val4.Count = val3.SelectUninsulated((object)val, andClearRequest.TargetSystem);
				break;
			case 8:
				val4.Message = val3.RestoreDisplay();
				break;
			case 9:
				val4.PickedElements = val3.PickInsulationElements(andClearRequest.Prompt);
				break;
			}
			smethod_0(andClearRequest, val4);
		}
		catch (Exception ex)
		{
			Logger.Error("[InsulationExternalEventHandler] 执行失败: " + ex.Message, ex);
			smethod_0(andClearRequest, new InsulationRequestResult
			{
				Error = "操作失败: " + ex.Message
			});
		}
	}

	private static void smethod_0(InsulationRequest insulationRequest_0, InsulationRequestResult insulationRequestResult_0)
	{
		insulationRequest_0.OnCompleted?.Invoke(insulationRequestResult_0);
	}
}
