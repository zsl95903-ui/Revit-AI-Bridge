using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using ns7;

namespace RevitAi.Core.CAD;

public sealed class PipeNetworkModelingService : IPipeNetworkModelingService
{
	[CompilerGenerated]
	public sealed class Class87
	{
		public PipeData pipeData_0;

		internal bool method_0(EnrichedManholeData enrichedManholeData_0)
		{
			return enrichedManholeData_0.CADData.Id == pipeData_0.StartManholeId;
		}

		internal bool method_1(EnrichedManholeData enrichedManholeData_0)
		{
			return enrichedManholeData_0.CADData.Id == pipeData_0.EndManholeId;
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct75 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<PipeNetworkModelingResult> asyncTaskMethodBuilder_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_0061: Unknown result type (might be due to invalid IL or missing references)
			//IL_0066: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_007e: Expected O, but got Unknown
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				int num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			PipeNetworkModelingResult result = new PipeNetworkModelingResult
			{
				Success = false,
				ErrorMessage = "此方法需要在适配层实现，请使用适配层的 PipeNetworkModelingServiceAdapter"
			};
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly IExcelDataService iexcelDataService_0;

	public PipeNetworkModelingService(IExcelDataService excelDataService)
	{
		iexcelDataService_0 = excelDataService ?? throw new ArgumentNullException("excelDataService");
	}

	public Result<Dictionary<string, ManholeParameterData>> ReadManholeParameterTable(string filePath, string sheetName, ManholeParameterReadConfig? config = null)
	{
		return iexcelDataService_0.ReadManholeParameterTable(filePath, sheetName, config);
	}

	public PipeNetworkModelingData MergeData(PipeNetworkAnalysisResult analysisResult, Dictionary<string, ManholeParameterData> parameterData, PipeNetworkModelingOptions? options = null)
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		//IL_0037: Expected O, but got Unknown
		if (options == null)
		{
			options = new PipeNetworkModelingOptions();
		}
		PipeNetworkModelingData val = new PipeNetworkModelingData
		{
			AnalysisResult = analysisResult
		};
		ManholeParameterData value = parameterData.FirstOrDefault().Value;
		val.ParameterDataSource = ((value != null) ? value.Source : null);
		PipeNetworkModelingData val2 = val;
		List<EnrichedManholeData> list = new List<EnrichedManholeData>();
		foreach (ManholeData manhole in analysisResult.Manholes)
		{
			EnrichedManholeData item = smethod_0(manhole, parameterData, options);
			list.Add(item);
		}
		if (options.UseNeighborDataForMissing)
		{
			smethod_1(list, options);
		}
		smethod_2(list, options);
		smethod_3(list);
		val2.Manholes = list;
		List<EnrichedPipeData> list2 = new List<EnrichedPipeData>();
		foreach (PipeData pipe in analysisResult.Pipes)
		{
			EnrichedPipeData item2 = smethod_4(pipe, list, options);
			list2.Add(item2);
		}
		val2.Pipes = list2;
		return val2;
	}

	private static EnrichedManholeData smethod_0(ManholeData manholeData_0, Dictionary<string, ManholeParameterData> dictionary_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Expected O, but got Unknown
		EnrichedManholeData val = new EnrichedManholeData
		{
			CADData = manholeData_0,
			PositionX = manholeData_0.X,
			PositionY = manholeData_0.Y,
			PositionZ = 0.0,
			WellDepthMM = 0.0,
			PipeBottomElevationMM = 0.0
		};
		ManholeParameterData value = null;
		string text = null;
		if (manholeData_0.Annotations.Count > 0)
		{
			foreach (ManholeAnnotation annotation in manholeData_0.Annotations)
			{
				if (!dictionary_0.TryGetValue(annotation.Content, out value))
				{
					List<string> list = (from string_0 in annotation.Content.Split(new char[2] { ';', '；' })
						select string_0.Trim() into string_0
						where !string.IsNullOrEmpty(string_0)
						select string_0).ToList();
					string text2 = null;
					int num = 0;
					foreach (string item in list)
					{
						if (!dictionary_0.TryGetValue(item, out ManholeParameterData value2))
						{
							foreach (KeyValuePair<string, ManholeParameterData> item2 in dictionary_0)
							{
								if (item.Contains(item2.Key) && item2.Key.Length > num)
								{
									value = item2.Value;
									text2 = item2.Key;
									num = item2.Key.Length;
								}
							}
							continue;
						}
						value = value2;
						text = item;
						break;
					}
					if (text != null || (text2 != null && value != null))
					{
						if (text2 != null)
						{
							text = text2;
						}
						break;
					}
					continue;
				}
				text = annotation.Content;
				break;
			}
		}
		else if (manholeData_0.Annotation != null)
		{
			if (dictionary_0.TryGetValue(manholeData_0.Annotation, out value))
			{
				text = manholeData_0.Annotation;
			}
			else
			{
				List<string> list2 = (from string_0 in manholeData_0.Annotation.Split(new char[2] { ';', '；' })
					select string_0.Trim() into string_0
					where !string.IsNullOrEmpty(string_0)
					select string_0).ToList();
				string text3 = null;
				int num2 = 0;
				foreach (string item3 in list2)
				{
					if (!dictionary_0.TryGetValue(item3, out ManholeParameterData value3))
					{
						foreach (KeyValuePair<string, ManholeParameterData> item4 in dictionary_0)
						{
							if (item3.Contains(item4.Key) && item4.Key.Length > num2)
							{
								value = item4.Value;
								text3 = item4.Key;
								num2 = item4.Key.Length;
							}
						}
						continue;
					}
					value = value3;
					text = item3;
					break;
				}
				if (text3 != null && value != null)
				{
					text = text3;
				}
			}
		}
		if (value != null)
		{
			val.ParameterData = value;
			val.MatchedAnnotation = text;
		}
		val.CheckMissingFields();
		return val;
	}

	private static void smethod_1(List<EnrichedManholeData> list_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		foreach (EnrichedManholeData item in list_0)
		{
			if (item.ParameterData == null)
			{
				EnrichedManholeData val = smethod_6(item, list_0, pipeNetworkModelingOptions_0);
				if (val != null && val.ParameterData != null)
				{
					item.ParameterData = val.ParameterData;
					item.MissingFields.Add("使用相邻管井 " + val.CADData.Annotation + " 的参数");
				}
				continue;
			}
			if (item.ParameterData.GroundElevationM == 0.0)
			{
				EnrichedManholeData val2 = smethod_7(item, list_0, delegate(EnrichedManholeData enrichedManholeData_0)
				{
					ManholeParameterData parameterData = enrichedManholeData_0.ParameterData;
					return parameterData != null && parameterData.GroundElevationM > 0.0;
				}, pipeNetworkModelingOptions_0);
				if (val2 != null && val2.ParameterData != null)
				{
					item.ParameterData.GroundElevationM = val2.ParameterData.GroundElevationM;
					item.MissingFields.Add("地面标高来自相邻管井 " + val2.CADData.Annotation);
				}
			}
			if (item.ParameterData.PipeBottomElevationM == 0.0)
			{
				EnrichedManholeData val3 = smethod_7(item, list_0, delegate(EnrichedManholeData enrichedManholeData_0)
				{
					ManholeParameterData parameterData = enrichedManholeData_0.ParameterData;
					return parameterData == null || parameterData.PipeBottomElevationM != 0.0;
				}, pipeNetworkModelingOptions_0);
				if (val3 != null && val3.ParameterData != null)
				{
					item.ParameterData.PipeBottomElevationM = val3.ParameterData.PipeBottomElevationM;
					item.MissingFields.Add("管底标高来自相邻管井 " + val3.CADData.Annotation);
				}
			}
			if (item.ParameterData.WellDepthM == 0.0)
			{
				EnrichedManholeData val4 = smethod_7(item, list_0, delegate(EnrichedManholeData enrichedManholeData_0)
				{
					ManholeParameterData parameterData = enrichedManholeData_0.ParameterData;
					return parameterData != null && parameterData.WellDepthM > 0.0;
				}, pipeNetworkModelingOptions_0);
				if (val4 != null && val4.ParameterData != null)
				{
					item.ParameterData.WellDepthM = val4.ParameterData.WellDepthM;
					item.MissingFields.Add("井深来自相邻管井 " + val4.CADData.Annotation);
				}
			}
		}
	}

	private static void smethod_2(List<EnrichedManholeData> list_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Expected O, but got Unknown
		foreach (EnrichedManholeData item in list_0)
		{
			if (item.ParameterData == null)
			{
				item.ParameterData = new ManholeParameterData
				{
					ManholeId = (item.CADData.Annotation ?? "未标注"),
					GroundElevationM = pipeNetworkModelingOptions_0.DefaultGroundElevationM,
					PipeBottomElevationM = pipeNetworkModelingOptions_0.DefaultPipeBottomElevationM,
					WellDepthM = pipeNetworkModelingOptions_0.DefaultWellDepthM
				};
				item.MissingFields.Add("使用默认值");
				continue;
			}
			if (item.ParameterData.GroundElevationM == 0.0)
			{
				item.ParameterData.GroundElevationM = pipeNetworkModelingOptions_0.DefaultGroundElevationM;
				List<string> missingFields = item.MissingFields;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler.AppendLiteral("地面标高使用默认值 ");
				defaultInterpolatedStringHandler.AppendFormatted(pipeNetworkModelingOptions_0.DefaultGroundElevationM);
				defaultInterpolatedStringHandler.AppendLiteral("m");
				missingFields.Add(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			if (item.ParameterData.PipeBottomElevationM == 0.0)
			{
				item.ParameterData.PipeBottomElevationM = pipeNetworkModelingOptions_0.DefaultPipeBottomElevationM;
				List<string> missingFields2 = item.MissingFields;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(11, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("管底标高使用默认值 ");
				defaultInterpolatedStringHandler2.AppendFormatted(pipeNetworkModelingOptions_0.DefaultPipeBottomElevationM);
				defaultInterpolatedStringHandler2.AppendLiteral("m");
				missingFields2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			if (item.ParameterData.WellDepthM == 0.0)
			{
				item.ParameterData.WellDepthM = pipeNetworkModelingOptions_0.DefaultWellDepthM;
				List<string> missingFields3 = item.MissingFields;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(9, 1);
				defaultInterpolatedStringHandler3.AppendLiteral("井深使用默认值 ");
				defaultInterpolatedStringHandler3.AppendFormatted(pipeNetworkModelingOptions_0.DefaultWellDepthM);
				defaultInterpolatedStringHandler3.AppendLiteral("m");
				missingFields3.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
			}
		}
	}

	private static void smethod_3(List<EnrichedManholeData> list_0)
	{
		foreach (EnrichedManholeData item in list_0)
		{
			if (item.ParameterData != null)
			{
				item.PositionZ = item.ParameterData.GroundElevationM * 1000.0;
				item.WellDepthMM = item.ParameterData.WellDepthM * 1000.0;
				item.PipeBottomElevationMM = item.ParameterData.PipeBottomElevationM * 1000.0;
			}
		}
	}

	private static EnrichedPipeData smethod_4(PipeData pipeData_0, List<EnrichedManholeData> list_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b6: Expected O, but got Unknown
		EnrichedPipeData val = new EnrichedPipeData
		{
			CADData = pipeData_0,
			StartX = pipeData_0.StartX,
			StartY = pipeData_0.StartY,
			StartZ = 0.0,
			EndX = pipeData_0.EndX,
			EndY = pipeData_0.EndY,
			EndZ = 0.0,
			DiameterMM = (pipeData_0.DiameterMM ?? smethod_5(pipeData_0.Annotation, pipeNetworkModelingOptions_0))
		};
		if (pipeData_0.StartManholeId != null)
		{
			val.StartManhole = list_0.FirstOrDefault((EnrichedManholeData enrichedManholeData_0) => enrichedManholeData_0.CADData.Id == pipeData_0.StartManholeId);
			EnrichedManholeData startManhole = val.StartManhole;
			val.StartManholeId = ((startManhole != null) ? startManhole.ManholeId : null);
		}
		if (pipeData_0.EndManholeId != null)
		{
			val.EndManhole = list_0.FirstOrDefault((EnrichedManholeData enrichedManholeData_0) => enrichedManholeData_0.CADData.Id == pipeData_0.EndManholeId);
			EnrichedManholeData endManhole = val.EndManhole;
			val.EndManholeId = ((endManhole != null) ? endManhole.ManholeId : null);
		}
		if (val.StartManhole != null)
		{
			val.StartZ = val.StartManhole.PipeBottomElevationMM + val.DiameterMM / 2.0;
		}
		else
		{
			val.StartZ = pipeNetworkModelingOptions_0.DefaultPipeBottomElevationM * 1000.0 + val.DiameterMM / 2.0;
		}
		if (val.EndManhole != null)
		{
			val.EndZ = val.EndManhole.PipeBottomElevationMM + val.DiameterMM / 2.0;
		}
		else
		{
			val.EndZ = pipeNetworkModelingOptions_0.DefaultPipeBottomElevationM * 1000.0 + val.DiameterMM / 2.0;
		}
		return val;
	}

	private static double smethod_5(string? string_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		if (string.IsNullOrEmpty(string_0))
		{
			return pipeNetworkModelingOptions_0.DefaultPipeDiameterMM;
		}
		double? num = PipeData.ExtractDiameterFromAnnotation(string_0);
		if (num.HasValue)
		{
			return num.Value;
		}
		return pipeNetworkModelingOptions_0.DefaultPipeDiameterMM;
	}

	private static EnrichedManholeData? smethod_6(EnrichedManholeData enrichedManholeData_0, List<EnrichedManholeData> list_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		EnrichedManholeData result = null;
		double num = double.MaxValue;
		foreach (EnrichedManholeData item in list_0)
		{
			if (item != enrichedManholeData_0 && item.ParameterData != null)
			{
				double num2 = smethod_8(enrichedManholeData_0, item);
				if (num2 < num && num2 <= pipeNetworkModelingOptions_0.MaxNeighborSearchDistanceMM)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	private static EnrichedManholeData? smethod_7(EnrichedManholeData enrichedManholeData_0, List<EnrichedManholeData> list_0, Func<EnrichedManholeData, bool> func_0, PipeNetworkModelingOptions pipeNetworkModelingOptions_0)
	{
		EnrichedManholeData result = null;
		double num = double.MaxValue;
		foreach (EnrichedManholeData item in list_0)
		{
			if (item != enrichedManholeData_0 && func_0(item))
			{
				double num2 = smethod_8(enrichedManholeData_0, item);
				if (num2 < num && num2 <= pipeNetworkModelingOptions_0.MaxNeighborSearchDistanceMM)
				{
					num = num2;
					result = item;
				}
			}
		}
		return result;
	}

	private static double smethod_8(EnrichedManholeData enrichedManholeData_0, EnrichedManholeData enrichedManholeData_1)
	{
		double num = enrichedManholeData_0.PositionX - enrichedManholeData_1.PositionX;
		double num2 = enrichedManholeData_0.PositionY - enrichedManholeData_1.PositionY;
		return Math.Sqrt(num * num + num2 * num2);
	}

	[AsyncStateMachine(typeof(Struct75))]
	public Task<PipeNetworkModelingResult> CreatePipeNetworkModelAsync(object document, PipeNetworkModelingData modelingData, PipeNetworkModelingOptions? options = null, Action<ModelingProgress>? progressCallback = null)
	{
		Struct75 stateMachine = default(Struct75);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<PipeNetworkModelingResult>.Create();
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
