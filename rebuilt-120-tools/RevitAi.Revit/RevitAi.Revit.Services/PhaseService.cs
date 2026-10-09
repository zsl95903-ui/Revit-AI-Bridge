using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class PhaseService : IPhaseService
{
	private readonly UIApplication _application;

	public PhaseService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> GetAllPhases(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			return ((IEnumerable)val.Phases).Cast<Phase>();
		}
		catch (Exception ex)
		{
			LogError("GetAllPhases 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? GetElementCreatedPhase(object element)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012100L));
			if (val2 != null && (int)val2.StorageType == 4)
			{
				ElementId val3 = val2.AsElementId();
				return val.Document.GetElement(val3);
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementCreatedPhase 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetElementDemolishedPhase(object element)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012101L));
			if (val2 != null && (int)val2.StorageType == 4)
			{
				ElementId val3 = val2.AsElementId();
				return val.Document.GetElement(val3);
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetElementDemolishedPhase 失败: " + ex.Message);
			return null;
		}
	}

	public bool SetElementCreatedPhase(object element, int phaseId, object document)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012100L));
			if (val2 != null && !((APIObject)val2).IsReadOnly)
			{
				val2.Set(new ElementId((long)phaseId));
				return true;
			}
			LogError("SetElementCreatedPhase: 无法设置创建阶段");
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetElementCreatedPhase 失败: " + ex.Message);
			return false;
		}
	}

	public bool SetElementDemolishedPhase(object element, int phaseId, object document)
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return false;
			}
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012101L));
			if (val2 != null && !((APIObject)val2).IsReadOnly)
			{
				val2.Set(new ElementId((long)phaseId));
				return true;
			}
			LogError("SetElementDemolishedPhase: 无法设置拆除阶段");
			return false;
		}
		catch (Exception ex)
		{
			LogError("SetElementDemolishedPhase 失败: " + ex.Message);
			return false;
		}
	}

	public object? GetPhaseByName(object document, string phaseName)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			return ((IEnumerable)val.Phases).Cast<Phase>().FirstOrDefault((Phase p) => ((Element)p).Name.Equals(phaseName, StringComparison.OrdinalIgnoreCase));
		}
		catch (Exception ex)
		{
			LogError("GetPhaseByName 失败: " + ex.Message);
			return null;
		}
	}

	private static void LogError(string message)
	{
	}

	public (object? CreatedPhase, object? DemolishedPhase)? GetElementPhases(object element)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Invalid comparison between Unknown and I4
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Invalid comparison between Unknown and I4
		try
		{
			Element val = (Element)((element is Element) ? element : null);
			if (val == null)
			{
				return null;
			}
			object item = null;
			object item2 = null;
			Parameter val2 = val.get_Parameter((BuiltInParameter)(-1012100L));
			if (val2 != null && (int)val2.StorageType == 4)
			{
				ElementId val3 = val2.AsElementId();
				if (val3 != (ElementId)null && val3 != ElementId.InvalidElementId)
				{
					item = val.Document.GetElement(val3);
				}
			}
			Parameter val4 = val.get_Parameter((BuiltInParameter)(-1012101L));
			if (val4 != null && (int)val4.StorageType == 4)
			{
				ElementId val5 = val4.AsElementId();
				if (val5 != (ElementId)null && val5 != ElementId.InvalidElementId)
				{
					item2 = val.Document.GetElement(val5);
				}
			}
			return (item, item2);
		}
		catch (Exception ex)
		{
			LogError("GetElementPhases 失败: " + ex.Message);
			return null;
		}
	}
}
