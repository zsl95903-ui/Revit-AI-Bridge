using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class ViewExportService : IViewExportService
{
	private readonly UIApplication _application;

	public ViewExportService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public ViewExportResult ExportView(object document, object view, ViewExportConfig config)
	{
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected O, but got Unknown
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return new ViewExportResult
			{
				IsSuccess = false,
				Error = "无效的文档对象"
			};
		}
		View val2 = (View)((view is View) ? view : null);
		if (val2 == null)
		{
			return new ViewExportResult
			{
				IsSuccess = false,
				Error = "无效的视图对象"
			};
		}
		try
		{
			string directoryName = Path.GetDirectoryName(config.FilePath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			ImageExportOptions val3 = new ImageExportOptions
			{
				FilePath = config.FilePath,
				FitDirection = (FitDirectionType)0,
				ZoomType = (ZoomFitType)0,
				PixelSize = config.Width,
				ExportRange = (ExportRange)2
			};
			val3.SetViewsAndSheets((IList<ElementId>)new List<ElementId> { ((Element)val2).Id });
			val.ExportImage(val3);
			string name = ((Element)val2).Name;
			Logger.Info("[ViewExportService] 成功导出视图 '" + name + "' 到 " + config.FilePath);
			return new ViewExportResult
			{
				IsSuccess = true,
				FilePath = config.FilePath,
				ViewName = name
			};
		}
		catch (Exception ex)
		{
			Logger.Error("[ViewExportService] 导出视图失败: " + ex.Message, ex);
			return new ViewExportResult
			{
				IsSuccess = false,
				Error = ex.Message
			};
		}
	}

	public BatchExportResult ExportViews(object document, IEnumerable<object> views, string directory, ViewExportConfig? config = null)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Expected O, but got Unknown
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0187: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a1: Expected O, but got Unknown
		BatchExportResult val = new BatchExportResult
		{
			ExportDirectory = directory,
			TotalViews = views.Count()
		};
		if (!Directory.Exists(directory))
		{
			try
			{
				Directory.CreateDirectory(directory);
			}
			catch (Exception ex)
			{
				Logger.Error("[ViewExportService] 创建导出目录失败: " + ex.Message, ex);
				val.FailureCount = val.TotalViews;
				return val;
			}
		}
		ViewExportConfig val2 = (ViewExportConfig)(((object)config) ?? ((object)new ViewExportConfig()));
		foreach (object view in views)
		{
			View val3 = (View)((view is View) ? view : null);
			if (val3 == null)
			{
				val.Results.Add(new ViewExportResult
				{
					IsSuccess = false,
					Error = "无效的视图对象"
				});
				int failureCount = val.FailureCount;
				val.FailureCount = failureCount + 1;
				continue;
			}
			string text = GetDefaultFileName(val3, "png");
			if (string.IsNullOrEmpty(text))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("View_");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)val3).Id.Value);
				defaultInterpolatedStringHandler.AppendLiteral(".png");
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			text = SanitizeFileName(text);
			string filePath = Path.Combine(directory, text);
			ViewExportConfig config2 = new ViewExportConfig
			{
				FilePath = filePath,
				Width = val2.Width,
				Height = val2.Height,
				ExportRange = val2.ExportRange,
				Quality = val2.Quality,
				ExportOnlyVisible = val2.ExportOnlyVisible,
				IncludeBackground = val2.IncludeBackground
			};
			ViewExportResult val4 = ExportView(document, val3, config2);
			val.Results.Add(val4);
			if (val4.IsSuccess)
			{
				int failureCount = val.SuccessCount;
				val.SuccessCount = failureCount + 1;
			}
			else
			{
				int failureCount = val.FailureCount;
				val.FailureCount = failureCount + 1;
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(38, 4);
		defaultInterpolatedStringHandler2.AppendLiteral("[ViewExportService] 批量导出完成: 成功 ");
		defaultInterpolatedStringHandler2.AppendFormatted(val.SuccessCount);
		defaultInterpolatedStringHandler2.AppendLiteral("/");
		defaultInterpolatedStringHandler2.AppendFormatted(val.TotalViews);
		defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
		defaultInterpolatedStringHandler2.AppendFormatted(val.FailureCount);
		defaultInterpolatedStringHandler2.AppendLiteral("/");
		defaultInterpolatedStringHandler2.AppendFormatted(val.TotalViews);
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		return val;
	}

	public BatchExportResult ExportViewsByIds(object document, IEnumerable<int> viewIds, string directory, ViewExportConfig? config = null)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return new BatchExportResult
			{
				TotalViews = viewIds.Count(),
				FailureCount = viewIds.Count(),
				ExportDirectory = directory
			};
		}
		List<object> list = new List<object>();
		foreach (int viewId in viewIds)
		{
			ElementId val2 = new ElementId((long)viewId);
			Element element = val.GetElement(val2);
			View val3 = (View)(object)((element is View) ? element : null);
			if (val3 != null)
			{
				list.Add(val3);
			}
		}
		return ExportViews(document, list, directory, config);
	}

	public string? GetDefaultFileName(object view, string format = "png")
	{
		View val = (View)((view is View) ? view : null);
		if (val == null)
		{
			return null;
		}
		string name = ((Element)val).Name;
		string text = SanitizeFileName(name);
		return text + "." + format.ToLower();
	}

	public string? SelectFolder(string? initialDirectory = null)
	{
		try
		{
			Microsoft.Win32.OpenFolderDialog folderBrowserDialog = new Microsoft.Win32.OpenFolderDialog
			{
				Title = "选择导出目录"
			};
			if (!string.IsNullOrEmpty(initialDirectory) && Directory.Exists(initialDirectory))
			{
				folderBrowserDialog.InitialDirectory = initialDirectory;
			}
			bool? dialogResult = folderBrowserDialog.ShowDialog();
			if (dialogResult == true)
			{
				return folderBrowserDialog.FolderName;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[ViewExportService] 文件夹选择对话框失败: " + ex.Message, ex);
			return null;
		}
	}

	private string SanitizeFileName(string? fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return "Unnamed";
		}
		char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
		string text = fileName ?? "Unnamed";
		char[] array = invalidFileNameChars;
		foreach (char oldChar in array)
		{
			text = text.Replace(oldChar, '_');
		}
		text = (text ?? "Unnamed").Replace(':', '_').Replace('/', '_').Replace('\\', '_')
			.Replace('?', '_')
			.Replace('*', '_')
			.Replace('"', '_')
			.Replace('<', '_')
			.Replace('>', '_')
			.Replace('|', '_');
		if (text.Length > 250)
		{
			text = text.Substring(0, 250);
		}
		return text;
	}
}
