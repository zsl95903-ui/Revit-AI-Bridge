using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class DocumentService : IDocumentService
{
	private readonly UIApplication _uiApplication;

	public DocumentService(UIApplication uiApplication)
	{
		_uiApplication = uiApplication ?? throw new ArgumentNullException("uiApplication");
	}

	public object? GetActiveDocument()
	{
		try
		{
			UIDocument activeUIDocument = _uiApplication.ActiveUIDocument;
			return (activeUIDocument != null) ? activeUIDocument.Document : null;
		}
		catch (Exception ex)
		{
			LogError("GetActiveDocument 失败: " + ex.Message);
			return null;
		}
	}

	public IEnumerable<object> GetAllDocuments()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		try
		{
			Application application = _uiApplication.Application;
			DocumentSet documents = application.Documents;
			List<object> list = new List<object>();
			foreach (Document item2 in documents)
			{
				Document item = item2;
				list.Add(item);
			}
			return list;
		}
		catch (Exception ex)
		{
			LogError("GetAllDocuments 失败: " + ex.Message);
			return Enumerable.Empty<object>();
		}
	}

	public object? CreateNewDocument(string? templateFile = null)
	{
		try
		{
			Application application = _uiApplication.Application;
			Document result;
			if (string.IsNullOrWhiteSpace(templateFile))
			{
				result = application.NewProjectDocument((UnitSystem)0);
			}
			else
			{
				if (!File.Exists(templateFile))
				{
					LogError("模板文件不存在: " + templateFile);
					return null;
				}
				result = application.NewProjectDocument(templateFile);
			}
			return result;
		}
		catch (Exception ex)
		{
			LogError("CreateNewDocument 失败: " + ex.Message);
			return null;
		}
	}

	public object? OpenDocument(string filePath)
	{
		try
		{
			if (!File.Exists(filePath))
			{
				LogError("文档文件不存在: " + filePath);
				return null;
			}
			Application application = _uiApplication.Application;
			return application.OpenDocumentFile(filePath);
		}
		catch (Exception ex)
		{
			LogError("OpenDocument 失败: " + ex.Message);
			return null;
		}
	}

	public bool CloseDocument(object document, bool save = true)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CloseDocument: document 不是 Document 类型");
				return false;
			}
			if (save && val.IsModified)
			{
				val.Save();
			}
			val.Close(false);
			return true;
		}
		catch (Exception ex)
		{
			LogError("CloseDocument 失败: " + ex.Message);
			return false;
		}
	}

	public bool SaveDocument(object document, string? filePath = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("SaveDocument: document 不是 Document 类型");
				return false;
			}
			if (string.IsNullOrWhiteSpace(filePath))
			{
				val.Save();
			}
			else
			{
				val.SaveAs(filePath);
			}
			return true;
		}
		catch (Exception ex)
		{
			LogError("SaveDocument 失败: " + ex.Message);
			return false;
		}
	}

	public string? GetDocumentPath(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			return val.PathName;
		}
		catch (Exception ex)
		{
			LogError("GetDocumentPath 失败: " + ex.Message);
			return null;
		}
	}

	public string? GetDocumentName(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return null;
			}
			return val.Title;
		}
		catch (Exception ex)
		{
			LogError("GetDocumentName 失败: " + ex.Message);
			return null;
		}
	}

	public bool IsDocumentModified(object document)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				return false;
			}
			return val.IsModified;
		}
		catch (Exception ex)
		{
			LogError("IsDocumentModified 失败: " + ex.Message);
			return false;
		}
	}

	public object? CreateSheet(object document, int titleBlockTypeId, string sheetName, string sheetNumber)
	{
		return CreateSheet(document, (int?)titleBlockTypeId, sheetName, sheetNumber);
	}

	public object? CreateSheet(object document, int? titleBlockTypeId, string sheetName, string sheetNumber)
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("CreateSheet: document 不是 Document 类型");
				return null;
			}
			FamilySymbol val2 = null;
			if (titleBlockTypeId.HasValue && titleBlockTypeId.Value > 0)
			{
				Element element = val.GetElement(new ElementId((long)titleBlockTypeId.Value));
				val2 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
				if (val2 == null)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(26, 1);
					defaultInterpolatedStringHandler.AppendLiteral("CreateSheet: 无法找到标题栏类型 ID ");
					defaultInterpolatedStringHandler.AppendFormatted(titleBlockTypeId.Value);
					LogError(defaultInterpolatedStringHandler.ToStringAndClear());
					return null;
				}
			}
			else
			{
				val2 = GetFirstTitleBlockType(val);
				if (val2 == null)
				{
					LogError("CreateSheet: 无法找到任何可用的标题栏类型");
					return null;
				}
			}
			ViewSheet val3 = ViewSheet.Create(val, ((Element)val2).Id);
			if (val3 == null)
			{
				LogError("CreateSheet: ViewSheet.Create 返回 null");
				return null;
			}
			if (!string.IsNullOrEmpty(sheetName))
			{
				((Element)val3).Name = sheetName;
			}
			if (!string.IsNullOrEmpty(sheetNumber))
			{
				val3.SheetNumber = sheetNumber;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(28, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("CreateSheet: 成功创建图纸 '");
			defaultInterpolatedStringHandler2.AppendFormatted(sheetName);
			defaultInterpolatedStringHandler2.AppendLiteral("'（编号: ");
			defaultInterpolatedStringHandler2.AppendFormatted(sheetNumber);
			defaultInterpolatedStringHandler2.AppendLiteral("）");
			LogInfo(defaultInterpolatedStringHandler2.ToStringAndClear());
			return val3;
		}
		catch (Exception ex)
		{
			LogError("CreateSheet 失败: " + ex.Message);
			return null;
		}
	}

	public object? AddViewToSheet(object document, object sheet, object view, object? position)
	{
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Expected O, but got Unknown
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("AddViewToSheet: document 不是 Document 类型");
				return null;
			}
			ViewSheet val2 = (ViewSheet)((sheet is ViewSheet) ? sheet : null);
			if (val2 == null)
			{
				LogError("AddViewToSheet: sheet 不是 ViewSheet 类型");
				return null;
			}
			View val3 = (View)((view is View) ? view : null);
			if (val3 == null)
			{
				LogError("AddViewToSheet: view 不是 View 类型");
				return null;
			}
			XYZ val4;
			if (position != null && ParsePosition(position, out XYZ xyz) && xyz != null)
			{
				val4 = xyz;
			}
			else
			{
				BoundingBoxUV outline = ((View)val2).Outline;
				double num = (outline.Max.U - outline.Min.U) / 2.0 + outline.Min.U;
				double num2 = (outline.Max.V - outline.Min.V) / 2.0 + outline.Min.V;
				val4 = new XYZ(num, num2, 0.0);
			}
			Viewport val5 = Viewport.Create(val, ((Element)val2).Id, ((Element)val3).Id, val4);
			if (val5 == null)
			{
				LogError("AddViewToSheet: Viewport.Create 返回 null");
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
			defaultInterpolatedStringHandler.AppendLiteral("AddViewToSheet: 成功将视图 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val3).Name);
			defaultInterpolatedStringHandler.AppendLiteral("' 添加到图纸 '");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral("'");
			LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
			return val5;
		}
		catch (Exception ex)
		{
			LogError("AddViewToSheet 失败: " + ex.Message);
			return null;
		}
	}

	private FamilySymbol? GetFirstTitleBlockType(Document doc)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		try
		{
			FilteredElementCollector val = new FilteredElementCollector(doc).OfCategory((BuiltInCategory)(-2000280L)).OfClass(typeof(FamilySymbol));
			using (IEnumerator<Element> enumerator = val.GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					return (FamilySymbol)enumerator.Current;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			LogError("GetFirstTitleBlockType 失败: " + ex.Message);
			return null;
		}
	}

	private bool ParsePosition(object positionObj, out XYZ? xyz)
	{
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Expected O, but got Unknown
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Expected O, but got Unknown
		try
		{
			if (positionObj == null)
			{
				xyz = null;
				return false;
			}
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			if (positionObj is IDictionary<string, object> dictionary)
			{
				if (dictionary.TryGetValue("x", out var value) && value != null)
				{
					num = Convert.ToDouble(value);
					flag = true;
				}
				if (dictionary.TryGetValue("y", out var value2) && value2 != null)
				{
					num2 = Convert.ToDouble(value2);
					flag2 = true;
				}
				if (dictionary.TryGetValue("z", out var value3) && value3 != null)
				{
					num3 = Convert.ToDouble(value3);
					flag3 = true;
				}
				if (flag & flag2 & flag3)
				{
					xyz = new XYZ(num, num2, num3);
					return true;
				}
			}
			Type type = positionObj.GetType();
			PropertyInfo property = type.GetProperty("x");
			PropertyInfo property2 = type.GetProperty("y");
			PropertyInfo property3 = type.GetProperty("z");
			if (property != null && property2 != null && property3 != null)
			{
				num = Convert.ToDouble(property.GetValue(positionObj));
				num2 = Convert.ToDouble(property2.GetValue(positionObj));
				num3 = Convert.ToDouble(property3.GetValue(positionObj));
				xyz = new XYZ(num, num2, num3);
				return true;
			}
			xyz = null;
			return false;
		}
		catch
		{
			xyz = null;
			return false;
		}
	}

	public object? PlaceViewOnSheet(object document, object sheet, object view, object position)
	{
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("PlaceViewOnSheet: document 不是 Document 类型");
				return null;
			}
			ViewSheet val2 = (ViewSheet)((sheet is ViewSheet) ? sheet : null);
			if (val2 == null)
			{
				LogError("PlaceViewOnSheet: sheet 不是 ViewSheet 类型");
				return null;
			}
			View val3 = (View)((view is View) ? view : null);
			if (val3 == null)
			{
				LogError("PlaceViewOnSheet: view 不是 View 类型");
				return null;
			}
			if (ParsePosition(position, out XYZ xyz))
			{
				Transaction val4 = new Transaction(val, "放置视图到图纸");
				try
				{
					val4.Start();
					try
					{
						Viewport val5 = Viewport.Create(val, ((Element)val2).Id, ((Element)val3).Id, xyz);
						if (val5 == null)
						{
							LogError("PlaceViewOnSheet: Viewport.Create 返回 null");
							val4.RollBack();
							return null;
						}
						val4.Commit();
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
						defaultInterpolatedStringHandler.AppendLiteral("PlaceViewOnSheet: 成功创建视图端口，ID: ");
						defaultInterpolatedStringHandler.AppendFormatted(((Element)val5).Id.Value);
						LogInfo(defaultInterpolatedStringHandler.ToStringAndClear());
						return val5;
					}
					catch (Exception ex)
					{
						LogError("PlaceViewOnSheet: 事务执行失败 - " + ex.Message);
						val4.RollBack();
						return null;
					}
				}
				finally
				{
					((IDisposable)val4)?.Dispose();
				}
			}
			LogError("PlaceViewOnSheet: 无法解析位置参数");
			return null;
		}
		catch (Exception ex2)
		{
			LogError("PlaceViewOnSheet 失败: " + ex2.Message);
			return null;
		}
	}

	public bool ExportToPDF(object document, string filePath, IEnumerable<int>? viewIds = null)
	{
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val == null)
			{
				LogError("ExportToPDF: document 不是 Document 类型");
				return false;
			}
			LogError("ExportToPDF: 该方法暂未完整实现（需要使用 PDFExportOptions）");
			return false;
		}
		catch (Exception ex)
		{
			LogError("ExportToPDF 失败: " + ex.Message);
			return false;
		}
	}

	private static void LogInfo(string message)
	{
		Logger.Info("[DocumentService] " + message);
	}

	private static void LogError(string message)
	{
		Logger.Error("[DocumentService] " + message);
	}
}
