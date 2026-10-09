using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using RevitAi.Core.CAD;
using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using ns6;

using Document = Autodesk.Revit.DB.Document;

namespace RevitAi.Revit.Services;

public sealed class PipeNetworkModelingServiceAdapter : IPipeNetworkModelingService
{
	private readonly PipeNetworkModelingService _coreService;

	private readonly IFamilyService _familyService;

	private readonly IParameterService _parameterService;

	private readonly IPipeCreationService _pipeCreationService;

	public PipeNetworkModelingServiceAdapter(PipeNetworkModelingService coreService, IFamilyService familyService, IParameterService parameterService, IPipeCreationService? pipeCreationService = null)
	{
		_coreService = coreService ?? throw new ArgumentNullException("coreService");
		_familyService = familyService ?? throw new ArgumentNullException("familyService");
		_parameterService = parameterService ?? throw new ArgumentNullException("parameterService");
		_pipeCreationService = (IPipeCreationService)(((object)pipeCreationService) ?? ((object)new PipeCreationService()));
	}

	public Result<Dictionary<string, ManholeParameterData>> ReadManholeParameterTable(string filePath, string sheetName, ManholeParameterReadConfig? config = null)
	{
		return _coreService.ReadManholeParameterTable(filePath, sheetName, config);
	}

	public PipeNetworkModelingData MergeData(PipeNetworkAnalysisResult analysisResult, Dictionary<string, ManholeParameterData> parameterData, PipeNetworkModelingOptions? options = null)
	{
		return _coreService.MergeData(analysisResult, parameterData, options);
	}

	public Task<PipeNetworkModelingResult> CreatePipeNetworkModelAsync(object document, PipeNetworkModelingData modelingData, PipeNetworkModelingOptions? options = null, Action<ModelingProgress>? progressCallback = null)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0398: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_039f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a2: Invalid comparison between Unknown and I4
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Invalid comparison between Unknown and I4
		//IL_0435: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0441: Unknown result type (might be due to invalid IL or missing references)
		//IL_0449: Unknown result type (might be due to invalid IL or missing references)
		//IL_0451: Unknown result type (might be due to invalid IL or missing references)
		//IL_0459: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Expected O, but got Unknown
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		Stopwatch stopwatch = Stopwatch.StartNew();
		try
		{
			Document val = (Document)((document is Document) ? document : null);
			if (val != null)
			{
				if (options == null)
				{
					options = new PipeNetworkModelingOptions();
				}
				progressCallback?.Invoke(ModelingProgress.Create("准备建模", 0, "开始建模操作"));
				Transaction val2 = new Transaction(val, "创建市政管网");
				try
				{
					if ((int)val2.Start() != 1)
					{
						return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("无法启动事务"));
					}
					List<string> list = new List<string>();
					List<string> list2 = new List<string>();
					List<string> list3 = new List<string>();
					progressCallback?.Invoke(ModelingProgress.Create("加载族", 5, "加载管井族"));
					Family val3 = LoadFamilyIfNeeded(val, options, list);
					if (val3 == null)
					{
						val2.RollBack();
						return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("无法加载管井族: " + options.ManholeFamilyFilePath));
					}
					FamilySymbol familySymbol = GetFamilySymbol(val, val3, options.ManholeSymbolName, list);
					if (familySymbol == null)
					{
						val2.RollBack();
						return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("无法找到族符号: " + options.ManholeSymbolName));
					}
					int num = 0;
					Dictionary<string, ElementId> dictionary = new Dictionary<string, ElementId>();
					for (int i = 0; i < modelingData.Manholes.Count; i++)
					{
						EnrichedManholeData val4 = modelingData.Manholes[i];
						progressCallback?.Invoke(ModelingProgress.CreateItemProgress("创建管井", i + 1, modelingData.Manholes.Count, "创建管井 " + val4.ManholeId));
						try
						{
							FamilyInstance val5 = CreateManholeInstance(val, familySymbol, val4);
							if (val5 != null)
							{
								if (val4.WellDepthMM > 0.0)
								{
									_parameterService.SetParameterValue((object)val5, "井_深度", val4.WellDepthMM);
								}
								if (!string.IsNullOrEmpty(val4.ManholeId))
								{
									_parameterService.SetParameterValue((object)val5, "标记", val4.ManholeId);
								}
								dictionary[val4.CADData.Id] = ((Element)val5).Id;
								num++;
								if (val4.MissingFields.Count > 0)
								{
									list3.Add("管井 " + val4.ManholeId + ": " + string.Join(", ", val4.MissingFields));
								}
							}
							else
							{
								list2.Add("管井 " + val4.ManholeId + " 创建失败");
							}
						}
						catch (Exception ex)
						{
							list2.Add("管井 " + val4.ManholeId + ": " + ex.Message);
						}
					}
					progressCallback?.Invoke(ModelingProgress.Create("创建管道", 80, "开始创建管道"));
					PipeCreationResult val6 = _pipeCreationService.CreatePipes((object)val, modelingData.Pipes, options, (Action<int, int>)delegate(int current, int total)
					{
						Action<ModelingProgress>? action2 = progressCallback;
						if (action2 != null)
						{
							string text = "创建管道";
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 2);
							defaultInterpolatedStringHandler2.AppendLiteral("创建管道 (");
							defaultInterpolatedStringHandler2.AppendFormatted(current);
							defaultInterpolatedStringHandler2.AppendLiteral("/");
							defaultInterpolatedStringHandler2.AppendFormatted(total);
							defaultInterpolatedStringHandler2.AppendLiteral(")");
							action2(ModelingProgress.CreateItemProgress(text, current, total, defaultInterpolatedStringHandler2.ToStringAndClear()));
						}
					});
					int successCount = val6.SuccessCount;
					list2.AddRange(val6.Errors);
					list.AddRange(val6.Warnings);
					TransactionStatus val7 = val2.Commit();
					if ((int)val7 != 3)
					{
						return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("事务提交失败"));
					}
					stopwatch.Stop();
					Action<ModelingProgress>? action = progressCallback;
					if (action != null)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 2);
						defaultInterpolatedStringHandler.AppendLiteral("建模完成: ");
						defaultInterpolatedStringHandler.AppendFormatted(num);
						defaultInterpolatedStringHandler.AppendLiteral(" 个管井, ");
						defaultInterpolatedStringHandler.AppendFormatted(successCount);
						defaultInterpolatedStringHandler.AppendLiteral(" 个管道");
						action(ModelingProgress.CreateCompleted(defaultInterpolatedStringHandler.ToStringAndClear()));
					}
					return Task.FromResult<PipeNetworkModelingResult>(new PipeNetworkModelingResult
					{
						Success = true,
						CreatedManholesCount = num,
						CreatedPipesCount = successCount,
						Warnings = list,
						Errors = list2,
						MissingDataReport = list3,
						ElapsedMilliseconds = stopwatch.ElapsedMilliseconds
					});
				}
				finally
				{
					((IDisposable)val2)?.Dispose();
				}
			}
			return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("文档类型无效"));
		}
		catch (Exception ex2)
		{
			stopwatch.Stop();
			progressCallback?.Invoke(ModelingProgress.CreateError(ex2.Message));
			return Task.FromResult<PipeNetworkModelingResult>(PipeNetworkModelingResult.CreateFailure("建模失败: " + ex2.Message));
		}
	}

	private Family? LoadFamilyIfNeeded(Document doc, PipeNetworkModelingOptions options, List<string> warnings)
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		string text = null;
		try
		{
			Family val = ((IEnumerable)new FilteredElementCollector(doc).OfClass(typeof(Family))).Cast<Family>().FirstOrDefault((Family f) => ((Element)f).Name.Equals(options.ManholeFamilyName, StringComparison.OrdinalIgnoreCase));
			if (val != null)
			{
				Logger.Info("[PipeNetworkModeling] 族 " + options.ManholeFamilyName + " 已在项目中存在，跳过加载");
				return val;
			}
			string manholeFamilyPath = GetManholeFamilyPath(options.ManholeFamilyFilePath);
			if (string.IsNullOrEmpty(manholeFamilyPath))
			{
				Logger.Error("[PipeNetworkModeling] 族路径为空，但项目中找不到族 " + options.ManholeFamilyName);
				warnings.Add("项目中找不到管井族: " + options.ManholeFamilyName);
				return null;
			}
			if (manholeFamilyPath == null)
			{
				return null;
			}
			string text2 = string.Join("_", options.ManholeFamilyName.Split(Path.GetInvalidFileNameChars()));
			if (string.IsNullOrWhiteSpace(text2))
			{
				text2 = Guid.NewGuid().ToString("N");
			}
			text = Path.Combine(Path.GetTempPath(), text2 + ".rfa");
			try
			{
				File.Copy(manholeFamilyPath, text, overwrite: true);
				Logger.Info("[PipeNetworkModeling] 复制族文件到临时位置: " + text);
			}
			catch (Exception ex)
			{
				Logger.Error("[PipeNetworkModeling] 复制族文件失败: " + ex.Message);
				warnings.Add("复制族文件失败: " + ex.Message);
				return null;
			}
			Logger.Info("[PipeNetworkModeling] 加载管井族: " + text);
			object obj = _familyService.LoadFamily((object)doc, text);
			Family val2 = (Family)((obj is Family) ? obj : null);
			if (val2 == null)
			{
				warnings.Add("无法加载族文件: " + text);
				return null;
			}
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
			defaultInterpolatedStringHandler.AppendLiteral("[PipeNetworkModeling] 族加载成功: ");
			defaultInterpolatedStringHandler.AppendFormatted(((Element)val2).Name);
			defaultInterpolatedStringHandler.AppendLiteral(" (ID: ");
			defaultInterpolatedStringHandler.AppendFormatted<ElementId>(((Element)val2).Id);
			defaultInterpolatedStringHandler.AppendLiteral(")");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			return val2;
		}
		catch (Exception ex2)
		{
			warnings.Add("加载族时出错: " + ex2.Message);
			return null;
		}
		finally
		{
			if (text != null && File.Exists(text))
			{
				try
				{
					File.Delete(text);
					Logger.Debug("[PipeNetworkModeling] 已删除临时族文件: " + text);
				}
				catch (Exception ex3)
				{
					Logger.Warning("[PipeNetworkModeling] 删除临时族文件失败: " + ex3.Message);
				}
			}
		}
	}

	private string? GetManholeFamilyPath(string providedPath)
	{
		if (string.IsNullOrEmpty(providedPath))
		{
			Logger.Info("[PipeNetworkModeling] 使用项目中已存在的族，跳过族文件加载");
			return string.Empty;
		}
		if (File.Exists(providedPath))
		{
			Logger.Info("[PipeNetworkModeling] 使用在线族库下载的族文件: " + providedPath);
			return providedPath;
		}
		Logger.Error("[PipeNetworkModeling] 族文件路径不存在: " + providedPath);
		return null;
	}

	private FamilySymbol? GetFamilySymbol(Document doc, Family family, string symbolName, List<string> warnings)
	{
		try
		{
			ElementId val = family.GetFamilySymbolIds().FirstOrDefault();
			if (val == (ElementId)null)
			{
				warnings.Add("族 " + ((Element)family).Name + " 没有可用的类型符号");
				return null;
			}
			Element element = doc.GetElement(val);
			FamilySymbol val2 = (FamilySymbol)(object)((element is FamilySymbol) ? element : null);
			if (val2 == null)
			{
				warnings.Add("无法获取族符号");
				return null;
			}
			if (!val2.IsActive)
			{
				val2.Activate();
				doc.Regenerate();
			}
			return val2;
		}
		catch (Exception ex)
		{
			warnings.Add("获取族符号时出错: " + ex.Message);
			return null;
		}
	}

	private FamilyInstance? CreateManholeInstance(Document doc, FamilySymbol symbol, EnrichedManholeData manholeData)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		try
		{
			XYZ val = new XYZ(manholeData.PositionX / 304.8, manholeData.PositionY / 304.8, manholeData.PositionZ / 304.8);
			return ((ItemFactoryBase)doc.Create).NewFamilyInstance(val, symbol, (StructuralType)0);
		}
		catch (Exception)
		{
			return null;
		}
	}
}
