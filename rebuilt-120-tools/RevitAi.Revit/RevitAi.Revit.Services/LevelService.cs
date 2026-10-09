using System;
using System.Collections.Generic;
using System.Linq;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class LevelService : ILevelService
{
	private readonly UIApplication _uiApplication;

	public LevelService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public IEnumerable<object> GetAllLevels(object document)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return Enumerable.Empty<object>();
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val);
			return val2.OfCategory((BuiltInCategory)(-2000240L)).ToElements().Cast<object>();
		}
		catch (Exception ex)
		{
			LogError("GetAllLevels 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public string? GetLevelName(object level)
	{
		try
		{
			Level val = (Level)((level is Level) ? level : null);
			if (val == null)
			{
				return null;
			}
			return ((Element)val).Name;
		}
		catch (Exception ex)
		{
			LogError("GetLevelName 失败: " + ex.Message);
			return null;
		}
	}

	public double? GetLevelElevation(object level)
	{
		try
		{
			Level val = (Level)((level is Level) ? level : null);
			if (val == null)
			{
				return null;
			}
			return val.Elevation;
		}
		catch (Exception ex)
		{
			LogError("GetLevelElevation 失败: " + ex.Message);
			return null;
		}
	}

	public int? GetLevelId(object level)
	{
		try
		{
			Level val = (Level)((level is Level) ? level : null);
			if (val == null)
			{
				return null;
			}
			return (int)((Element)val).Id.Value;
		}
		catch (Exception ex)
		{
			LogError("GetLevelId 失败: " + ex.Message);
			return null;
		}
	}

	public object? GetLevelByName(object document, string name)
	{
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			FilteredElementCollector val2 = new FilteredElementCollector(val);
			return val2.OfCategory((BuiltInCategory)(-2000240L)).ToElements().OfType<Level>()
				.FirstOrDefault((Level l) => ((Element)l).Name == name);
		}
		catch (Exception ex)
		{
			LogError("GetLevelByName 失败: " + ex.Message);
			return null;
		}
	}

	private void LogError(string message)
	{
	}
}
