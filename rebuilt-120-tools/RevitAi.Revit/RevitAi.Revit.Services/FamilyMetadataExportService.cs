using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.Loader;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json;
using ns0;
using ns6;

namespace RevitAi.Revit.Services;

internal sealed class FamilyMetadataExportService : IFamilyMetadataService
{
	private readonly UIApplication _application;

	private readonly ILogger _logger = ServiceProvider.GetLogger();

	public FamilyMetadataExportService(UIApplication application)
	{
		_application = application ?? throw new ArgumentNullException("application");
	}

	public IEnumerable<FamilyInfo> GetAllFamilies(object document)
	{
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Expected O, but got Unknown
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			_logger.Error("[FamilyMetadataExportService] 无效的文档对象", (Exception)null);
			return Enumerable.Empty<FamilyInfo>();
		}
		try
		{
			FilteredElementCollector val2 = new FilteredElementCollector(val).OfClass(typeof(Family));
			List<FamilyInfo> list = new List<FamilyInfo>();
			foreach (Family item in val2)
			{
				Family val3 = item;
				if (val3.FamilyCategory != null && val3.IsEditable)
				{
					list.Add(new FamilyInfo
					{
						Family = val3,
						DisplayName = "[" + val3.FamilyCategory.Name + "] " + ((Element)val3).Name,
						FamilyName = ((Element)val3).Name,
						CategoryName = val3.FamilyCategory.Name
					});
				}
			}
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(40, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[FamilyMetadataExportService] 找到 ");
			defaultInterpolatedStringHandler.AppendFormatted(list.Count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个可编辑的族");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return (from f in list
				orderby f.CategoryName, f.FamilyName
				select f).ToList();
		}
		catch (Exception ex)
		{
			_logger.Error("[FamilyMetadataExportService] 获取族列表失败", ex);
			return Enumerable.Empty<FamilyInfo>();
		}
	}

	public FamilyMetadataExportResult ExportMetadata(object document, object family, string? exportDirectory)
	{
		string text = exportDirectory;
		if (string.IsNullOrEmpty(text))
		{
			text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "RevitAi", "FamilyMetadata");
			Directory.CreateDirectory(text);
		}
		return ExportFamilyComplete(document, family, text);
	}

	public (int SuccessCount, int FailCount) ExportMetadataBatch(object document, IEnumerable<object> families, string? exportDirectory, Action<int, int, string>? progressCallback)
	{
		List<object> list = families.ToList();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < list.Count; i++)
		{
			if (progressCallback != null)
			{
				int arg = i + 1;
				int count = list.Count;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 2);
				defaultInterpolatedStringHandler.AppendLiteral("正在导出 (");
				defaultInterpolatedStringHandler.AppendFormatted(i + 1);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				defaultInterpolatedStringHandler.AppendFormatted(list.Count);
				defaultInterpolatedStringHandler.AppendLiteral(")");
				progressCallback(arg, count, defaultInterpolatedStringHandler.ToStringAndClear());
			}
			FamilyMetadataExportResult val = ExportMetadata(document, list[i], exportDirectory);
			if (val.IsSuccess)
			{
				num++;
			}
			else
			{
				num2++;
			}
		}
		return (SuccessCount: num, FailCount: num2);
	}

	private string? ExportThumbnailView(Document familyDoc, string outputPath)
	{
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		View3D val = null;
		try
		{
			string directoryName = Path.GetDirectoryName(outputPath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			List<View3D> source = (from View3D v in (IEnumerable)new FilteredElementCollector(familyDoc).OfClass(typeof(View3D))
				where !((View)v).IsTemplate
				select v).ToList();
			val = source.FirstOrDefault((View3D v) => !v.IsPerspective);
			if (val == null)
			{
				val = source.FirstOrDefault();
			}
			if (val == null)
			{
				_logger.Warning("[FamilyMetadataExportService] 族文档中没有找到 3D 视图，跳过缩略图导出");
				return null;
			}
			Transaction val2 = new Transaction(familyDoc, "设置视图显示样式");
			try
			{
				val2.Start();
				try
				{
					XYZ val3 = new XYZ(-10.0, -10.0, 10.0);
					XYZ val4 = new XYZ(1.0, 1.0, -1.0).Normalize();
					XYZ val5 = new XYZ(1.0, 1.0, 2.0).Normalize();
					val.SetOrientation(new ViewOrientation3D(val3, val5, val4));
					Parameter val6 = ((Element)val).get_Parameter((BuiltInParameter)(-1005165L));
					if (val6 != null)
					{
						val6.Set(4);
					}
					val2.Commit();
				}
				catch (Exception ex)
				{
					_logger.Warning("[FamilyMetadataExportService] 设置视图样式失败: " + ex.Message + "，将使用默认设置");
					val2.RollBack();
				}
			}
			finally
			{
				((IDisposable)val2)?.Dispose();
			}
			ImageExportOptions val7 = new ImageExportOptions
			{
				FilePath = outputPath,
				ExportRange = (ExportRange)2,
				ZoomType = (ZoomFitType)0,
				ImageResolution = (ImageResolution)1,
				FitDirection = (FitDirectionType)0,
				PixelSize = 512
			};
			val7.SetViewsAndSheets((IList<ElementId>)new List<ElementId> { ((Element)val).Id });
			try
			{
				familyDoc.ExportImage(val7);
			}
			catch (Exception ex2)
			{
				_logger.Warning("[FamilyMetadataExportService] ExportImage 失败: " + ex2.GetType().Name + " - " + ex2.Message);
				if (ex2.InnerException != null)
				{
					_logger.Warning("[FamilyMetadataExportService] 内部异常: " + ex2.InnerException.Message);
				}
				throw;
			}
			_logger.Info("[FamilyMetadataExportService] 导出缩略图成功: " + outputPath);
			return outputPath;
		}
		catch (Exception ex3)
		{
			_logger.Warning("[FamilyMetadataExportService] 导出视图失败: " + ex3.GetType().Name + " - " + ex3.Message);
			return null;
		}
	}

	private string? GetFamilyFilePath(Family family)
	{
		try
		{
			string name = ((Element)family).Name;
			string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
			string folderPath2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			string folderPath3 = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			List<string> obj = new List<string> { Path.Combine(folderPath3, "Family", name + ".rfa") };
			InlineArray5<string> gparam_ = default(InlineArray5<string>);
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_, 0) = folderPath;
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_, 1) = "Autodesk";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_, 2) = "Revit";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_, 3) = "Family";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_, 4) = name + ".rfa";
			obj.Add(Path.Combine(Class653.smethod_1<InlineArray5<string>, string>(in gparam_, 5)));
			InlineArray5<string> gparam_2 = default(InlineArray5<string>);
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 0) = folderPath2;
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 1) = "Autodesk";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 2) = "Revit";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 3) = "Family";
			Class653.smethod_2<InlineArray5<string>, string>(ref gparam_2, 4) = name + ".rfa";
			obj.Add(Path.Combine(Class653.smethod_1<InlineArray5<string>, string>(in gparam_2, 5)));
			InlineArray6<string> gparam_3 = default(InlineArray6<string>);
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 0) = folderPath2;
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 1) = "Autodesk";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 2) = "RVT";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 3) = "Family";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 4) = "Chinese";
			Class653.smethod_2<InlineArray6<string>, string>(ref gparam_3, 5) = name + ".rfa";
			obj.Add(Path.Combine(Class653.smethod_1<InlineArray6<string>, string>(in gparam_3, 6)));
			InlineArray7<string> gparam_4 = default(InlineArray7<string>);
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 0) = folderPath2;
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 1) = "Autodesk";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 2) = "RVT";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 3) = "Family";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 4) = "Chinese";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 5) = "标题栏";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_4, 6) = name + ".rfa";
			obj.Add(Path.Combine(Class653.smethod_1<InlineArray7<string>, string>(in gparam_4, 7)));
			InlineArray7<string> gparam_5 = default(InlineArray7<string>);
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 0) = folderPath;
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 1) = "Autodesk";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 2) = "RVT";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 3) = "Family";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 4) = "Chinese";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 5) = "标题栏";
			Class653.smethod_2<InlineArray7<string>, string>(ref gparam_5, 6) = name + ".rfa";
			obj.Add(Path.Combine(Class653.smethod_1<InlineArray7<string>, string>(in gparam_5, 7)));
			List<string> list = obj;
			for (int i = 2018; i <= 2027; i++)
			{
				InlineArray6<string> gparam_6 = default(InlineArray6<string>);
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 0) = folderPath2;
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 1) = "Autodesk";
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 2) = "RVT " + i;
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 3) = "Family";
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 4) = "Chinese";
				Class653.smethod_2<InlineArray6<string>, string>(ref gparam_6, 5) = name + ".rfa";
				list.Add(Path.Combine(Class653.smethod_1<InlineArray6<string>, string>(in gparam_6, 6)));
				InlineArray7<string> gparam_7 = default(InlineArray7<string>);
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 0) = folderPath2;
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 1) = "Autodesk";
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 2) = "RVT " + i;
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 3) = "Family";
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 4) = "Chinese";
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 5) = "标题栏";
				Class653.smethod_2<InlineArray7<string>, string>(ref gparam_7, 6) = name + ".rfa";
				list.Add(Path.Combine(Class653.smethod_1<InlineArray7<string>, string>(in gparam_7, 7)));
			}
			foreach (string item in list.Distinct())
			{
				if (File.Exists(item))
				{
					return item;
				}
			}
			return null;
		}
		catch (Exception ex)
		{
			_logger.Warning("[FamilyMetadataExportService] 获取族文件路径失败: " + ((Element)family).Name + " - " + ex.Message);
			return null;
		}
	}

	private string ExportMetadataFromFamilyManager(Document familyDoc, string familyName, string version, string? thumbnailFileName, string targetDirectory)
	{
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		string text = Path.Combine(targetDirectory, familyName + ".json");
		List<object> list = new List<object>();
		string gparam_ = "";
		try
		{
			FamilyManager familyManager = familyDoc.FamilyManager;
			if (familyManager == null)
			{
				_logger.Warning("[FamilyMetadataExportService] 无法获取 FamilyManager: " + familyName);
				return text;
			}
			try
			{
				Family ownerFamily = familyDoc.OwnerFamily;
				if (ownerFamily != null && ownerFamily.FamilyCategory != null)
				{
					gparam_ = ownerFamily.FamilyCategory.Name;
				}
			}
			catch
			{
			}
			List<FamilyParameter> list2 = ((IEnumerable)familyManager.Parameters).Cast<FamilyParameter>().ToList();
			FamilyTypeSet types = familyManager.Types;
			ILogger logger = _logger;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(41, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[FamilyMetadataExportService] 族 '");
			defaultInterpolatedStringHandler.AppendFormatted(familyName);
			defaultInterpolatedStringHandler.AppendLiteral("' 有 ");
			defaultInterpolatedStringHandler.AppendFormatted(types.Size);
			defaultInterpolatedStringHandler.AppendLiteral(" 个类型");
			logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			foreach (FamilyType item3 in types)
			{
				FamilyType val = item3;
				try
				{
					List<object> list3 = new List<object>();
					foreach (FamilyParameter item4 in list2)
					{
						try
						{
							Definition definition = item4.Definition;
							Class324<string, string, string, bool, string> item = new Class324<string, string, string, bool, string>(definition.Name, GetParameterClassification(item4), GetFamilyParameterValue(val, item4, familyDoc), !item4.IsInstance, GetParameterGroupLabel(definition));
							list3.Add(item);
						}
						catch
						{
						}
					}
					Class325<string, List<object>> item2 = new Class325<string, List<object>>(val.Name, list3);
					list.Add(item2);
				}
				catch (Exception ex)
				{
					_logger.Warning("[FamilyMetadataExportService] 读取族类型失败: " + val.Name + " - " + ex.Message);
				}
			}
		}
		catch (Exception ex2)
		{
			_logger.Warning("[FamilyMetadataExportService] 读取族参数失败: " + familyName + " - " + ex2.Message);
		}
		Class326<string, string, string, string, string, List<object>> @class = new Class326<string, string, string, string, string, List<object>>(familyName, "导出自 Revit 项目中的族", version, thumbnailFileName ?? "", gparam_, list);
		string contents = JsonConvert.SerializeObject((object)@class, (Formatting)1);
		File.WriteAllText(text, contents);
		_logger.Info("[FamilyMetadataExportService] 导出元数据: " + text);
		return text;
	}

	private string FormatDouble(double? value)
	{
		if (!value.HasValue)
		{
			return "N/A";
		}
		return value.Value.ToString("0.00", CultureInfo.InvariantCulture);
	}

	private string? GetFamilyParameterValue(FamilyType familyType, FamilyParameter fp, Document? familyDoc)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Invalid comparison between Unknown and I4
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Invalid comparison between Unknown and I4
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Invalid comparison between Unknown and I4
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Invalid comparison between Unknown and I4
		try
		{
			string text = familyType.AsValueString(fp);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			StorageType storageType = fp.StorageType;
			if ((int)storageType == 4)
			{
				ElementId val = familyType.AsElementId(fp);
				if (val == ElementId.InvalidElementId)
				{
					return "按类别";
				}
				if (familyDoc != null)
				{
					try
					{
						Element element = familyDoc.GetElement(val);
						if (element != null)
						{
							return element.Name;
						}
					}
					catch
					{
					}
				}
				return val.Value.ToString();
			}
			if ((int)storageType == 1)
			{
				int? num = familyType.AsInteger(fp);
				try
				{
					ForgeTypeId dataType = fp.Definition.GetDataType();
					if (dataType != (ForgeTypeId)null)
					{
						string labelForSpec = LabelUtils.GetLabelForSpec(dataType);
						if (labelForSpec == "是/否")
						{
							return (num == 1) ? "是" : "否";
						}
					}
				}
				catch
				{
				}
				return num.ToString();
			}
			if ((int)storageType == 2)
			{
				double? value = familyType.AsDouble(fp);
				return FormatDouble(value);
			}
			if ((int)storageType == 3)
			{
				return familyType.AsString(fp) ?? "N/A";
			}
			return "N/A";
		}
		catch
		{
			return "N/A";
		}
	}

	private string? GetParameterValue(Parameter param)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Expected I4, but got Unknown
		try
		{
			StorageType storageType = param.StorageType;
			return ((int)(storageType) - 1) switch
			{
				0 => param.AsInteger().ToString(), 
				1 => param.AsDouble().ToString("0.00"), 
				2 => param.AsString(), 
				3 => param.AsElementId().Value.ToString(), 
				_ => "", 
			};
		}
		catch
		{
			return "";
		}
	}

	private string GetParameterValueTypeLabel(Definition def)
	{
		try
		{
			ForgeTypeId dataType = def.GetDataType();
			if (dataType != (ForgeTypeId)null)
			{
				return LabelUtils.GetLabelForSpec(dataType);
			}
			return "其他";
		}
		catch
		{
			return "其他";
		}
	}

	private string GetStorageTypeName(StorageType storageType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected I4, but got Unknown
		return ((int)(storageType) - 1) switch
		{
			0 => "整数", 
			1 => "数值", 
			2 => "文字", 
			3 => "元素ID", 
			_ => "未知", 
		};
	}

	private string GetParameterClassification(FamilyParameter fp)
	{
		if (fp.IsShared)
		{
			return "共享参数";
		}
		ElementId id = fp.Id;
		long num = 0L;
		num = id.Value;
		if (num < 0L)
		{
			return "内置参数";
		}
		return "族参数";
	}

	private string GetParameterGroupLabel(Definition def)
	{
		try
		{
			ForgeTypeId groupTypeId = def.GetGroupTypeId();
			if (groupTypeId != (ForgeTypeId)null)
			{
				return LabelUtils.GetLabelForGroup(groupTypeId);
			}
			return "其他";
		}
		catch
		{
			return "其他";
		}
	}

	private string GetDefinitionDescription(Definition def)
	{
		try
		{
			PropertyInfo property = ((object)def).GetType().GetProperty("Description");
			if (property != null)
			{
				string text = property.GetValue(def) as string;
				return text ?? "";
			}
			return "";
		}
		catch
		{
			return "";
		}
	}

	private string GetParameterSourceForElementParameter(Parameter param, bool isShared)
	{
		if (isShared)
		{
			return "共享参数";
		}
		Definition definition = param.Definition;
		if (definition is InternalDefinition)
		{
			return "内置参数";
		}
		return "项目参数";
	}

	public string? SaveFamilyToFile(object document, object family, string targetPath, bool overwriteExisting = true)
	{
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			_logger.Error("[FamilyMetadataExportService] 无效的文档对象", (Exception)null);
			return null;
		}
		Family val2 = (Family)((family is Family) ? family : null);
		if (val2 == null)
		{
			_logger.Error("[FamilyMetadataExportService] 无效的族对象", (Exception)null);
			return null;
		}
		Document val3 = null;
		try
		{
			string directoryName = Path.GetDirectoryName(targetPath);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			if (File.Exists(targetPath) && !overwriteExisting)
			{
				_logger.Warning("[FamilyMetadataExportService] 文件已存在且不覆盖: " + targetPath);
				return null;
			}
			string familyFilePath = GetFamilyFilePath(val2);
			if (string.IsNullOrEmpty(familyFilePath))
			{
				_logger.Warning("[FamilyMetadataExportService] 无法获取族文件路径: " + ((Element)val2).Name);
				return null;
			}
			val3 = _application.Application.OpenDocumentFile(familyFilePath);
			if (val3 == null)
			{
				_logger.Warning("[FamilyMetadataExportService] 无法打开族文档: " + familyFilePath);
				return null;
			}
			SaveAsOptions val4 = new SaveAsOptions
			{
				OverwriteExistingFile = overwriteExisting
			};
			val3.SaveAs(targetPath, val4);
			_logger.Info("[FamilyMetadataExportService] 保存族文件成功: " + targetPath);
			return targetPath;
		}
		catch (Exception ex)
		{
			_logger.Error("[FamilyMetadataExportService] 保存族文件失败: " + ((Element)val2).Name + " - " + ex.Message, (Exception)null);
			return null;
		}
		finally
		{
			if (val3 != null)
			{
				try
				{
					val3.Close(false);
				}
				catch
				{
				}
			}
		}
	}

	public FamilyMetadataExportResult ExportFamilyComplete(object document, object family, string exportDirectory, bool saveFamilyFile = false, bool exportThumbnail = true, bool exportMetadata = true, bool overwrite = false)
	{
		//IL_039c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Expected O, but got Unknown
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Expected O, but got Unknown
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Expected O, but got Unknown
		//IL_00ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Expected O, but got Unknown
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0115: Expected O, but got Unknown
		//IL_0135: Unknown result type (might be due to invalid IL or missing references)
		//IL_013a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0152: Expected O, but got Unknown
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0176: Expected O, but got Unknown
		//IL_01d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Expected O, but got Unknown
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_035a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0275: Unknown result type (might be due to invalid IL or missing references)
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Expected O, but got Unknown
		//IL_0373: Expected O, but got Unknown
		//IL_02dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0306: Expected O, but got Unknown
		Document val = (Document)((document is Document) ? document : null);
		if (val == null)
		{
			return new FamilyMetadataExportResult
			{
				IsSuccess = false,
				ErrorMessage = "无效的文档对象"
			};
		}
		Family val2 = (Family)((family is Family) ? family : null);
		if (val2 == null)
		{
			return new FamilyMetadataExportResult
			{
				IsSuccess = false,
				ErrorMessage = "无效的族对象"
			};
		}
		Document val3 = null;
		try
		{
			string versionNumber = _application.Application.VersionNumber;
			if (!val2.IsEditable)
			{
				_logger.Warning("[FamilyMetadataExportService] 族 '" + ((Element)val2).Name + "' 不可编辑（可能是系统族或内置族）");
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "族 '" + ((Element)val2).Name + "' 不允许编辑或导出"
				};
			}
			ISet<ElementId> familySymbolIds = val2.GetFamilySymbolIds();
			if (familySymbolIds.Count == 0)
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "族没有可用的类型"
				};
			}
			Element element = val.GetElement(familySymbolIds.First());
			FamilySymbol val4 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val4 == null)
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "无法获取族类型"
				};
			}
			if (!exportThumbnail && !exportMetadata && !saveFamilyFile)
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = true
				};
			}
			string text = string.Join("_", ((Element)val2).Name.Split(Path.GetInvalidFileNameChars()));
			val3 = val.EditFamily(val2);
			if (val3 == null)
			{
				_logger.Warning("[FamilyMetadataExportService] 无法进入族 '" + ((Element)val2).Name + "' 的编辑状态");
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "无法编辑族 '" + ((Element)val2).Name + "'"
				};
			}
			string text2 = null;
			string metadataPath = null;
			string text3 = null;
			if (saveFamilyFile)
			{
				string text4 = Path.Combine(exportDirectory, text + ".rfa");
				if (File.Exists(text4) && !overwrite)
				{
					return new FamilyMetadataExportResult
					{
						IsSuccess = false,
						ErrorMessage = "文件已存在: " + Path.GetFileName(text4)
					};
				}
				SaveAsOptions val5 = new SaveAsOptions
				{
					OverwriteExistingFile = true
				};
				val3.SaveAs(text4, val5);
				text3 = text4;
				_logger.Info("[FamilyMetadataExportService] 保存族文件: " + text3);
			}
			if (exportThumbnail)
			{
				text2 = Path.Combine(exportDirectory, text + ".png");
				if (File.Exists(text2) && !overwrite)
				{
					return new FamilyMetadataExportResult
					{
						IsSuccess = false,
						ErrorMessage = "文件已存在: " + Path.GetFileName(text2)
					};
				}
				ExportThumbnailView(val3, text2);
			}
			if (exportMetadata)
			{
				string text5 = ExportMetadataFromFamilyManager(val3, ((Element)val2).Name, versionNumber, (text2 == null || !File.Exists(text2)) ? null : Path.GetFileName(text2), exportDirectory);
				metadataPath = text5;
			}
			return new FamilyMetadataExportResult
			{
				IsSuccess = true,
				MetadataPath = metadataPath,
				ThumbnailPath = ((text2 == null || !File.Exists(text2)) ? null : text2)
			};
		}
		catch (Exception ex)
		{
			_logger.Error("[FamilyMetadataExportService] 完整导出失败: " + ((Element)val2).Name, ex);
			return new FamilyMetadataExportResult
			{
				IsSuccess = false,
				ErrorMessage = ex.Message
			};
		}
		finally
		{
			if (val3 != null)
			{
				try
				{
					val3.Close(false);
				}
				catch (Exception ex2)
				{
					_logger.Warning("[FamilyMetadataExportService] 关闭族文档失败: " + ((Element)val2).Name + " - " + ex2.Message);
				}
			}
		}
	}

	public FamilyMetadataExportResult ExportFamilyFile(string familyFilePath, string exportDirectory, bool saveFamilyFile = false, bool exportThumbnail = true, bool exportMetadata = true, bool overwrite = false)
	{
		//IL_01fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0200: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_0215: Expected O, but got Unknown
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_007a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_00ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Expected O, but got Unknown
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01da: Expected O, but got Unknown
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0155: Unknown result type (might be due to invalid IL or missing references)
		//IL_0172: Expected O, but got Unknown
		Document val = null;
		try
		{
			if (!File.Exists(familyFilePath))
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "族文件不存在"
				};
			}
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(familyFilePath);
			string versionNumber = _application.Application.VersionNumber;
			string text = familyFilePath;
			if (saveFamilyFile)
			{
				string text2 = Path.Combine(exportDirectory, Path.GetFileName(familyFilePath));
				if (File.Exists(text2) && !overwrite)
				{
					return new FamilyMetadataExportResult
					{
						IsSuccess = false,
						ErrorMessage = "文件已存在: " + Path.GetFileName(text2)
					};
				}
				if (File.Exists(familyFilePath))
				{
					File.Copy(familyFilePath, text2, overwrite: true);
				}
				text = text2;
			}
			if (!exportThumbnail && !exportMetadata)
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = true
				};
			}
			val = _application.Application.OpenDocumentFile(text);
			if (val == null)
			{
				return new FamilyMetadataExportResult
				{
					IsSuccess = false,
					ErrorMessage = "无法打开族文档"
				};
			}
			string fileNameWithoutExtension2 = Path.GetFileNameWithoutExtension(familyFilePath);
			string text3 = null;
			string metadataPath = null;
			if (exportThumbnail)
			{
				text3 = Path.Combine(exportDirectory, fileNameWithoutExtension + ".png");
				if (File.Exists(text3) && !overwrite)
				{
					return new FamilyMetadataExportResult
					{
						IsSuccess = false,
						ErrorMessage = "文件已存在: " + Path.GetFileName(text3)
					};
				}
				ExportThumbnailView(val, text3);
			}
			if (exportMetadata)
			{
				string text4 = ExportMetadataFromFamilyManager(val, fileNameWithoutExtension2, versionNumber, (text3 == null || !File.Exists(text3)) ? null : Path.GetFileName(text3), exportDirectory);
				metadataPath = text4;
			}
			return new FamilyMetadataExportResult
			{
				IsSuccess = true,
				MetadataPath = metadataPath,
				ThumbnailPath = ((text3 == null || !File.Exists(text3)) ? null : text3)
			};
		}
		catch (Exception ex)
		{
			_logger.Error("[FamilyMetadataExportService] 导出外部族失败: " + familyFilePath, ex);
			return new FamilyMetadataExportResult
			{
				IsSuccess = false,
				ErrorMessage = ex.Message
			};
		}
		finally
		{
			if (val != null)
			{
				try
				{
					val.Close(false);
				}
				catch
				{
				}
			}
		}
	}
}
