using System;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Units;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Infrastructure;

public sealed class SetProjectUnitExternalEventHandler : IExternalEventHandler
{
	public string GetName()
	{
		return "Set Project Unit";
	}

	public void Execute(UIApplication app)
	{
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected I4, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Expected O, but got Unknown
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Expected O, but got Unknown
		SetProjectUnitRequest andClearRequest = SetProjectUnitRequestManager.GetAndClearRequest();
		if (andClearRequest == null)
		{
			Logger.Warning("[SetProjectUnitExternalEventHandler] 没有待执行的请求");
			return;
		}
		try
		{
			object document = andClearRequest.Document;
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				Logger.Error("[SetProjectUnitExternalEventHandler] 无效的文档对象");
				andClearRequest.OnCompleted?.Invoke(obj: false);
				return;
			}
			Transaction val2 = new Transaction(val, "设置项目单位");
			try
			{
				val2.Start();
				try
				{
					Autodesk.Revit.DB.Units units = val.GetUnits();
					UnitType unitType = andClearRequest.UnitType;
					ForgeTypeId val3;
					switch ((int)(unitType) - 1)
					{
					default:
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
						defaultInterpolatedStringHandler.AppendLiteral("不支持的单位类型: ");
						defaultInterpolatedStringHandler.AppendFormatted<UnitType>(andClearRequest.UnitType);
						throw new ArgumentException(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					case 0:
						val3 = SpecTypeId.Length;
						break;
					case 1:
						val3 = SpecTypeId.Area;
						break;
					case 2:
						val3 = SpecTypeId.Volume;
						break;
					case 3:
						val3 = SpecTypeId.Angle;
						break;
					case 4:
						val3 = SpecTypeId.Slope;
						break;
					}
					ForgeTypeId val4 = val3;
					if (andClearRequest.DisplayUnitType != null)
					{
						units.GetFormatOptions(val4);
						FormatOptions val5 = new FormatOptions((ForgeTypeId)andClearRequest.DisplayUnitType);
						units.SetFormatOptions(val4, val5);
					}
					val2.Commit();
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(44, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[SetProjectUnitExternalEventHandler] 成功设置单位 ");
					defaultInterpolatedStringHandler2.AppendFormatted<UnitType>(andClearRequest.UnitType);
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					andClearRequest.OnCompleted?.Invoke(obj: true);
				}
				catch (Exception ex)
				{
					val2.RollBack();
					Logger.Error("[SetProjectUnitExternalEventHandler] 设置单位失败: " + ex.Message, ex);
					andClearRequest.OnCompleted?.Invoke(obj: false);
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
		}
		catch (Exception ex2)
		{
			Logger.Error("[SetProjectUnitExternalEventHandler] 执行失败", ex2);
			andClearRequest.OnCompleted?.Invoke(obj: false);
		}
	}
}
