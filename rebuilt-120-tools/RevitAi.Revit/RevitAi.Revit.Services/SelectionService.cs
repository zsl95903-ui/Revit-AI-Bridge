using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ns6;

using OperationCanceledException = System.OperationCanceledException;
using ArgumentNullException = System.ArgumentNullException;
namespace RevitAi.Revit.Services;

internal sealed class SelectionService : ISelectionService
{
	private class WallSelectionFilter : ISelectionFilter
	{
		public bool AllowElement(Element elem)
		{
			return elem is Wall;
		}

		public bool AllowReference(Reference reference, XYZ position)
		{
			return false;
		}
	}

	private readonly UIApplication _application;

	public SelectionService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<object> PickElements(object document, string prompt = "请选择元素")
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PickElements: document 参数无效");
				return Enumerable.Empty<object>();
			}
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("PickElements: 无法获取 ActiveUIDocument");
				return Enumerable.Empty<object>();
			}
			IList<Reference> list = activeUIDocument.Selection.PickObjects((ObjectType)1, prompt);
			List<object> list2 = new List<object>();
			foreach (Reference item in list)
			{
				Element element = val.GetElement(item);
				if (element != null)
				{
					list2.Add(element);
				}
			}
			return list2;
		}
		catch (OperationCanceledException)
		{
			return Enumerable.Empty<object>();
		}
		catch (Exception ex2)
		{
			LogError("PickElements 失败: " + ex2.Message);
			return Enumerable.Empty<object>();
		}
	}

	public IEnumerable<object> GetSelectedElements(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("GetSelectedElements: document 参数无效");
				return Enumerable.Empty<object>();
			}
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("GetSelectedElements: 无法获取 ActiveUIDocument");
				return Enumerable.Empty<object>();
			}
			ICollection<ElementId> elementIds = activeUIDocument.Selection.GetElementIds();
			List<object> list = new List<object>();
			foreach (ElementId item in elementIds)
			{
				Element element = val.GetElement(item);
				if (element != null)
				{
					list.Add(element);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetSelectedElements 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public void ClearSelection(object document)
	{
		try
		{
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("ClearSelection: 无法获取 ActiveUIDocument");
			}
			else
			{
				activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)new List<ElementId>());
			}
		}
		catch (Exception ex)
		{
			LogError("ClearSelection 失败: " + ex.Message);
		}
	}

	public bool SelectElements(object document, IEnumerable<int> elementIds, bool append = false)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SelectElements: document 参数无效");
				return false;
			}
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("SelectElements: 无法获取 ActiveUIDocument");
				return false;
			}
			List<ElementId> list = elementIds.Select((Func<int, ElementId>)delegate(int id)
			{
				//IL_0002: Unknown result type (might be due to invalid IL or missing references)
				//IL_0008: Expected O, but got Unknown
				return new ElementId((long)id);
			}).ToList();
			if (append)
			{
				ICollection<ElementId> elementIds2 = activeUIDocument.Selection.GetElementIds();
				foreach (ElementId item in list)
				{
					elementIds2.Add(item);
				}
				activeUIDocument.Selection.SetElementIds(elementIds2);
			}
			else
			{
				activeUIDocument.Selection.SetElementIds((ICollection<ElementId>)list);
			}
			return true;
		}
		catch (Exception ex)
		{
			LogError("SelectElements 失败: " + ex.Message);
			return false;
		}
	}

	public Task<List<object>> SelectWallsAsync(string prompt = "请选择墙")
	{
		try
		{
			UIDocument activeUIDocument = _application.ActiveUIDocument;
			if (activeUIDocument == null)
			{
				LogError("SelectWallsAsync: 无法获取 ActiveUIDocument");
				return Task.FromResult(new List<object>());
			}
			Document document = activeUIDocument.Document;
			WallSelectionFilter wallSelectionFilter = new WallSelectionFilter();
			IList<Reference> list = activeUIDocument.Selection.PickObjects((ObjectType)1, (ISelectionFilter)(object)wallSelectionFilter, prompt);
			List<object> list2 = new List<object>();
			foreach (Reference item in list)
			{
				Element element = document.GetElement(item);
				Wall val = (Wall)(object)((element is Wall) ? element : null);
				if (val != null)
				{
					list2.Add(val);
				}
			}
			return Task.FromResult(list2);
		}
		catch (OperationCanceledException)
		{
			LogError("SelectWallsAsync: 用户取消选择");
			return Task.FromResult(new List<object>());
		}
		catch (Exception ex2)
		{
			LogError("SelectWallsAsync 失败: " + ex2.Message);
			return Task.FromResult(new List<object>());
		}
	}

	private void LogError(string message)
	{
		try
		{
			Logger.Error("[SelectionService] " + message);
		}
		catch
		{
		}
	}
}
