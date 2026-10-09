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

internal sealed class DwgExportService : IDwgExportService
{
	private readonly UIApplication _application;

	public DwgExportService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public DwgExportResult ExportView(object document, object view, DwgExportConfig config)
	{
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a2: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Expected O, but got Unknown
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return new DwgExportResult
			{
				IsSuccess = false,
				Error = "无效的文档对象"
			};
		}
		View val2 = (View)((view is View) ? view : null);
		if (val2 == null)
		{
			return new DwgExportResult
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
			DWGExportOptions val3 = new DWGExportOptions();
			string directoryName2 = Path.GetDirectoryName(config.FilePath);
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(config.FilePath);
			if (config.ExportAsDXF)
			{
				DXFExportOptions val4 = new DXFExportOptions();
				val.Export(directoryName2 ?? "", fileNameWithoutExtension, (ICollection<ElementId>)new List<ElementId> { ((Element)val2).Id }, val4);
			}
			else
			{
				val.Export(directoryName2 ?? "", fileNameWithoutExtension, (ICollection<ElementId>)new List<ElementId> { ((Element)val2).Id }, val3);
			}
			string name = ((Element)val2).Name;
			Logger.Info("[DwgExportService] 成功导出视图 '" + name + "' 到 " + config.FilePath);
			return new DwgExportResult
			{
				IsSuccess = true,
				FilePath = config.FilePath,
				ViewName = name
			};
		}
		catch (Exception ex)
		{
			Logger.Error("[DwgExportService] 导出视图失败: " + ex.Message, ex);
			return new DwgExportResult
			{
				IsSuccess = false,
				Error = ex.Message
			};
		}
	}

	public DwgBatchExportResult ExportViews(object document, IEnumerable<object> views, string directory, DwgExportConfig? config = null)
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
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		DwgBatchExportResult val = new DwgBatchExportResult
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
				Logger.Error("[DwgExportService] 创建导出目录失败: " + ex.Message, ex);
				val.FailCount = val.TotalViews;
				return val;
			}
		}
		DwgExportConfig val2 = (DwgExportConfig)(((object)config) ?? ((object)new DwgExportConfig()));
		foreach (object view in views)
		{
			View val3 = (View)((view is View) ? view : null);
			if (val3 == null)
			{
				val.Results.Add(new DwgExportResult
				{
					IsSuccess = false,
					Error = "无效的视图对象"
				});
				int failCount = val.FailCount;
				val.FailCount = failCount + 1;
				continue;
			}
			string text = GetDefaultFileName(val3, val2.ExportAsDXF ? "dxf" : "dwg");
			if (string.IsNullOrEmpty(text))
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler.AppendLiteral("View_");
				defaultInterpolatedStringHandler.AppendFormatted(((Element)val3).Id.Value);
				defaultInterpolatedStringHandler.AppendLiteral(".dwg");
				text = defaultInterpolatedStringHandler.ToStringAndClear();
			}
			text = SanitizeFileName(text);
			string filePath = Path.Combine(directory, text);
			DwgExportConfig config2 = new DwgExportConfig
			{
				FilePath = filePath,
				ExportSettingName = val2.ExportSettingName,
				ExportAs3D = val2.ExportAs3D,
				SharedLevels = val2.SharedLevels,
				VersionDescription = val2.VersionDescription,
				ExportAsDXF = val2.ExportAsDXF
			};
			DwgExportResult val4 = ExportView(document, val3, config2);
			val.Results.Add(val4);
			if (val4.IsSuccess)
			{
				int failCount = val.SuccessCount;
				val.SuccessCount = failCount + 1;
			}
			else
			{
				int failCount = val.FailCount;
				val.FailCount = failCount + 1;
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(37, 4);
		defaultInterpolatedStringHandler2.AppendLiteral("[DwgExportService] 批量导出完成: 成功 ");
		defaultInterpolatedStringHandler2.AppendFormatted(val.SuccessCount);
		defaultInterpolatedStringHandler2.AppendLiteral("/");
		defaultInterpolatedStringHandler2.AppendFormatted(val.TotalViews);
		defaultInterpolatedStringHandler2.AppendLiteral(", 失败 ");
		defaultInterpolatedStringHandler2.AppendFormatted(val.FailCount);
		defaultInterpolatedStringHandler2.AppendLiteral("/");
		defaultInterpolatedStringHandler2.AppendFormatted(val.TotalViews);
		Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
		return val;
	}

	public DwgBatchExportResult ExportViewsByIds(object document, IEnumerable<int> viewIds, string directory, DwgExportConfig? config = null)
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
			return new DwgBatchExportResult
			{
				TotalViews = viewIds.Count(),
				FailCount = viewIds.Count(),
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

	public string? GetDefaultFileName(object view, string format = "dwg")
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
				Title = "选择 DWG 导出目录"
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
			Logger.Error("[DwgExportService] 文件夹选择对话框失败: " + ex.Message, ex);
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
