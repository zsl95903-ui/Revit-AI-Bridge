using System;
using System.Collections.Generic;
using RevitAi.Abstractions.Revit;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using ns6;

using OperationCanceledException = System.OperationCanceledException;
namespace RevitAi.Revit.Revit;

[Regeneration(RegenerationOption.Manual)]
[Transaction(TransactionMode.Manual)]
public class RoadSurfaceSelectionExternalEventHandler : IExternalEventHandler
{
	public void Execute(UIApplication app)
	{
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0098: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Expected I4, but got Unknown
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			RoadSurfaceSelectionRequest andClearSelectionRequest = RoadSurfaceRefinementRequestManager.GetAndClearSelectionRequest();
			if (andClearSelectionRequest == null)
			{
				TaskDialog.Show("提示", "无效的选择请求");
				return;
			}
			UIDocument activeUIDocument = app.ActiveUIDocument;
			Document val = ((activeUIDocument != null) ? activeUIDocument.Document : null);
			if (val == null || activeUIDocument == null)
			{
				TaskDialog.Show("提示", "请打开一个文档");
				andClearSelectionRequest.OnCompleted?.Invoke(new List<object>());
				return;
			}
			_ = activeUIDocument.Selection;
			List<object> obj = new List<object>();
			RoadSurfaceSelectionMode mode = andClearSelectionRequest.Mode;
			RoadSurfaceSelectionMode val2 = mode;
			switch ((int)val2)
			{
			case 0:
				obj = method_0(activeUIDocument, val, andClearSelectionRequest.Prompt, andClearSelectionRequest.FilterCategoryId, andClearSelectionRequest.IsSingleSelection);
				break;
			case 1:
				obj = method_1(activeUIDocument, val, andClearSelectionRequest.Prompt, andClearSelectionRequest.IsSingleSelection);
				break;
			case 2:
				obj = method_2(activeUIDocument, val, andClearSelectionRequest.Prompt, andClearSelectionRequest.IsSingleSelection);
				break;
			}
			andClearSelectionRequest.OnCompleted?.Invoke(obj);
		}
		catch (Exception ex)
		{
			TaskDialog.Show("错误", "选择操作失败: " + ex.Message);
		}
	}

	private List<object> method_0(UIDocument uidocument_0, Document document_0, string string_0, int? nullable_0, bool bool_0)
	{
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		Selection selection = uidocument_0.Selection;
		List<object> list = new List<object>();
		try
		{
			CategorySelectionFilter categorySelectionFilter = new CategorySelectionFilter((BuiltInCategory)(-2000032L));
			if (bool_0)
			{
				Reference val = selection.PickObject((ObjectType)1, (ISelectionFilter)(object)categorySelectionFilter, string_0);
				if (val != null)
				{
					Element element = document_0.GetElement(val);
					if (element != null)
					{
						list.Add(element);
					}
				}
			}
			else
			{
				IList<Reference> list2 = selection.PickObjects((ObjectType)1, (ISelectionFilter)(object)categorySelectionFilter, string_0);
				foreach (Reference item in list2)
				{
					Element element2 = document_0.GetElement(item);
					if (element2 != null)
					{
						list.Add(element2);
					}
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex3)
		{
			TaskDialog.Show("错误", "选择楼板失败: " + ex3.Message);
		}
		return list;
	}

	private List<object> method_1(UIDocument uidocument_0, Document document_0, string string_0, bool bool_0)
	{
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		Selection selection = uidocument_0.Selection;
		List<object> list = new List<object>();
		try
		{
			FamilyInstanceSelectionFilter familyInstanceSelectionFilter = new FamilyInstanceSelectionFilter();
			IList<Reference> list2;
			if (bool_0)
			{
				Reference val = selection.PickObject((ObjectType)1, (ISelectionFilter)(object)familyInstanceSelectionFilter, string_0);
				list2 = ((val == null) ? new List<Reference>() : new List<Reference> { val });
			}
			else
			{
				list2 = selection.PickObjects((ObjectType)1, (ISelectionFilter)(object)familyInstanceSelectionFilter, string_0);
			}
			foreach (Reference item in list2)
			{
				Element element = document_0.GetElement(item);
				if (element != null)
				{
					FamilyInstance val2 = (FamilyInstance)(object)((element is FamilyInstance) ? element : null);
					if (val2 != null && method_3(val2))
					{
						list.Add(element);
					}
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex3)
		{
			TaskDialog.Show("错误", "选择空心族失败: " + ex3.Message);
		}
		return list;
	}

	private List<object> method_2(UIDocument uidocument_0, Document document_0, string string_0, bool bool_0)
	{
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		Selection selection = uidocument_0.Selection;
		List<object> list = new List<object>();
		try
		{
			IList<Reference> list2;
			if (bool_0)
			{
				Reference val = selection.PickObject((ObjectType)1, string_0);
				list2 = ((val == null) ? new List<Reference>() : new List<Reference> { val });
			}
			else
			{
				list2 = selection.PickObjects((ObjectType)1, string_0);
			}
			foreach (Reference item in list2)
			{
				Element element = document_0.GetElement(item);
				if (element != null)
				{
					list.Add(element);
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		catch (Exception ex3)
		{
			TaskDialog.Show("错误", "选择元素失败: " + ex3.Message);
		}
		return list;
	}

	private bool method_3(FamilyInstance familyInstance_0)
	{
		try
		{
			Family family = familyInstance_0.Symbol.Family;
			string name = ((Element)family).Name;
			return name.Contains("空心") || name.Contains("Void", StringComparison.OrdinalIgnoreCase);
		}
		catch
		{
			return false;
		}
	}

	public string GetName()
	{
		return "路面精修选择操作";
	}
}
