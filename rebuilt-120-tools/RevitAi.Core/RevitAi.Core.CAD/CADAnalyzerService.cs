using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Models.CADAnalysis;
using RevitAi.Abstractions.Services;
using RevitAi.Core.CAD.Analyzers;
using ns3;
using ns7;

namespace RevitAi.Core.CAD;

internal sealed class CADAnalyzerService : ICADAnalyzerService
{
	[CompilerGenerated]
	public sealed class Class85
	{
		public CADAnalysisOptions cadanalysisOptions_0;

		public List<ManholeData> list_0;

		public CADFileData cadfileData_0;

		public IEnumerable<string> ienumerable_0;

		internal List<AnnotationLineGroup> method_0()
		{
			return new ManholeAnnotationAnalyzer(cadanalysisOptions_0.LineEndToManholeMaxDistanceMM, cadanalysisOptions_0.TextToManholeMaxDistanceMM, cadanalysisOptions_0.LineConnectionToleranceMM, cadanalysisOptions_0.TextToLineMaxDistanceMM, cadanalysisOptions_0.DirectionAngleToleranceDegrees).method_0(list_0, cadfileData_0, ienumerable_0, cadanalysisOptions_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct74 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<PipeNetworkAnalysisResult> asyncTaskMethodBuilder_0;

		public string string_0;

		public CADAnalysisOptions cadanalysisOptions_0;

		public CADAnalyzerService cadanalyzerService_0;

		public Action<AnalysisProgress> action_0;

		private Stopwatch stopwatch_0;

		private PipeNetworkAnalysisResult pipeNetworkAnalysisResult_0;

		private CADFileData cadfileData_0;

		private List<ManholeData> list_0;

		private List<PipeData> list_1;

		private TaskAwaiter<List<AnnotationLineGroup>> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Expected O, but got Unknown
			//IL_0061: Expected O, but got Unknown
			int num = int_0;
			CADAnalyzerService cADAnalyzerService = cadanalyzerService_0;
			if ((uint)num > 1u)
			{
				stopwatch_0 = Stopwatch.StartNew();
				pipeNetworkAnalysisResult_0 = new PipeNetworkAnalysisResult
				{
					CadFilePath = string_0,
					Options = cadanalysisOptions_0,
					AnalysisTime = DateTime.Now,
					Statistics = new AnalysisStatistics
					{
						AnalysisStartTime = DateTime.Now
					}
				};
			}
			PipeNetworkAnalysisResult result;
			try
			{
				TaskAwaiter<List<AnnotationLineGroup>> awaiter;
				if (num == 0)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<AnnotationLineGroup>>);
					num = -1;
					int_0 = -1;
					goto IL_03b3;
				}
				if (num == 1)
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<List<AnnotationLineGroup>>);
					num = -1;
					int_0 = -1;
					goto IL_0492;
				}
				cADAnalyzerService.method_1(action_0, "开始分析", 0, "开始解析 CAD 文件");
				Logger.Info("[CADAnalyzerService] 使用默认单位转换系数: 1000.0（米）");
				cadfileData_0 = cADAnalyzerService.icadfileService_0.ParseCADFile(string_0, 1000.0);
				if (cadfileData_0 != null)
				{
					Action<AnalysisProgress> action = action_0;
					string text = "解析 CAD";
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(16, 1);
					defaultInterpolatedStringHandler.AppendLiteral("CAD 文件解析成功: ");
					defaultInterpolatedStringHandler.AppendFormatted(cadfileData_0.Layers.Count);
					defaultInterpolatedStringHandler.AppendLiteral(" 个图层");
					cADAnalyzerService.method_1(action, text, 10, defaultInterpolatedStringHandler.ToStringAndClear());
					cADAnalyzerService.method_1(action_0, "提取管井", 20, "开始提取管井数据");
					list_0 = ((ICADAnalyzerService)cADAnalyzerService).ExtractManholes(cadfileData_0, (IEnumerable<string>)cadanalysisOptions_0.ManholeLayerNames);
					pipeNetworkAnalysisResult_0.Manholes = list_0;
					pipeNetworkAnalysisResult_0.Statistics.TotalManholes = list_0.Count;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler2.AppendLiteral("[CADAnalyzerService] 提取管井: ");
					defaultInterpolatedStringHandler2.AppendFormatted(list_0.Count);
					defaultInterpolatedStringHandler2.AppendLiteral(" 个");
					Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
					cADAnalyzerService.method_1(action_0, "提取管道", 30, "开始提取管道数据");
					list_1 = ((ICADAnalyzerService)cADAnalyzerService).ExtractPipes(cadfileData_0, (IEnumerable<string>)cadanalysisOptions_0.PipeLayerNames);
					pipeNetworkAnalysisResult_0.Pipes = list_1;
					pipeNetworkAnalysisResult_0.Statistics.TotalPipes = list_1.Count;
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(29, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[CADAnalyzerService] 提取管道: ");
					defaultInterpolatedStringHandler3.AppendFormatted(list_1.Count);
					defaultInterpolatedStringHandler3.AppendLiteral(" 个");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					if (cadanalysisOptions_0.ManholeAnnotationLayerNames.Count > 0)
					{
						cADAnalyzerService.method_1(action_0, "分析管井标注", 40, "开始分析管井标注");
						awaiter = ((ICADAnalyzerService)cADAnalyzerService).AnalyzeManholeAnnotationsAsync(list_0, cadfileData_0, (IEnumerable<string>)cadanalysisOptions_0.ManholeAnnotationLayerNames, cadanalysisOptions_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
						goto IL_03b3;
					}
					goto IL_0403;
				}
				pipeNetworkAnalysisResult_0.Statistics.IsSuccess = false;
				pipeNetworkAnalysisResult_0.Statistics.ErrorMessage = "无法解析 CAD 文件";
				cADAnalyzerService.method_2(action_0, pipeNetworkAnalysisResult_0.Statistics.ErrorMessage);
				result = pipeNetworkAnalysisResult_0;
				goto end_IL_0062;
				IL_04e2:
				cADAnalyzerService.method_1(action_0, "建立拓扑", 80, "开始建立管道与管井的连接关系");
				((ICADAnalyzerService)cADAnalyzerService).ConnectPipesToManholes(list_1, list_0, cadanalysisOptions_0.PipeToManholeConnectionToleranceMM);
				pipeNetworkAnalysisResult_0.Statistics.ConnectedPipes = list_1.Count((PipeData pipeData_0) => pipeData_0.IsConnectedToManholes);
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(35, 2);
				defaultInterpolatedStringHandler4.AppendLiteral("[CADAnalyzerService] 建立拓扑: ");
				defaultInterpolatedStringHandler4.AppendFormatted(pipeNetworkAnalysisResult_0.Statistics.ConnectedPipes);
				defaultInterpolatedStringHandler4.AppendLiteral("/");
				defaultInterpolatedStringHandler4.AppendFormatted(list_1.Count);
				defaultInterpolatedStringHandler4.AppendLiteral(" 条管道已连接");
				Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
				cADAnalyzerService.method_1(action_0, "计算统计", 90, "计算统计信息");
				pipeNetworkAnalysisResult_0.Statistics.TotalPipeLength = list_1.Sum((PipeData pipeData_0) => pipeData_0.Length);
				stopwatch_0.Stop();
				pipeNetworkAnalysisResult_0.Statistics.AnalysisEndTime = DateTime.Now;
				pipeNetworkAnalysisResult_0.Statistics.AnalysisDurationMs = stopwatch_0.ElapsedMilliseconds;
				pipeNetworkAnalysisResult_0.Statistics.IsSuccess = true;
				Action<AnalysisProgress> action2 = action_0;
				string text2 = "完成";
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(12, 1);
				defaultInterpolatedStringHandler5.AppendLiteral("分析完成，耗时: ");
				defaultInterpolatedStringHandler5.AppendFormatted(pipeNetworkAnalysisResult_0.Statistics.AnalysisDurationMs);
				defaultInterpolatedStringHandler5.AppendLiteral(" ms");
				cADAnalyzerService.method_1(action2, text2, 100, defaultInterpolatedStringHandler5.ToStringAndClear());
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(43, 3);
				defaultInterpolatedStringHandler6.AppendLiteral("[CADAnalyzerService] 分析完成: ");
				defaultInterpolatedStringHandler6.AppendLiteral("管井=");
				defaultInterpolatedStringHandler6.AppendFormatted(pipeNetworkAnalysisResult_0.Statistics.TotalManholes);
				defaultInterpolatedStringHandler6.AppendLiteral(", ");
				defaultInterpolatedStringHandler6.AppendLiteral("管道=");
				defaultInterpolatedStringHandler6.AppendFormatted(pipeNetworkAnalysisResult_0.Statistics.TotalPipes);
				defaultInterpolatedStringHandler6.AppendLiteral(", ");
				defaultInterpolatedStringHandler6.AppendLiteral("耗时=");
				defaultInterpolatedStringHandler6.AppendFormatted(pipeNetworkAnalysisResult_0.Statistics.AnalysisDurationMs);
				defaultInterpolatedStringHandler6.AppendLiteral(" ms");
				Logger.Info(defaultInterpolatedStringHandler6.ToStringAndClear());
				result = pipeNetworkAnalysisResult_0;
				goto end_IL_0062;
				IL_0403:
				if (cadanalysisOptions_0.PipeAnnotationLayerNames.Count > 0)
				{
					cADAnalyzerService.method_1(action_0, "分析管道标注", 60, "开始分析管道标注");
					awaiter = ((ICADAnalyzerService)cADAnalyzerService).AnalyzePipeAnnotationsAsync(list_1, cadfileData_0, (IEnumerable<string>)cadanalysisOptions_0.PipeAnnotationLayerNames, cadanalysisOptions_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 1;
						int_0 = 1;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
					goto IL_0492;
				}
				goto IL_04e2;
				IL_0492:
				List<AnnotationLineGroup> result2 = awaiter.GetResult();
				pipeNetworkAnalysisResult_0.PipeAnnotationLineGroups = result2;
				pipeNetworkAnalysisResult_0.Statistics.AnnotatedPipes = list_1.Count((PipeData pipeData_0) => pipeData_0.HasAnnotation);
				goto IL_04e2;
				IL_03b3:
				List<AnnotationLineGroup> result3 = awaiter.GetResult();
				pipeNetworkAnalysisResult_0.ManholeAnnotationLineGroups = result3;
				pipeNetworkAnalysisResult_0.Statistics.AnnotatedManholes = list_0.Count((ManholeData manholeData_0) => manholeData_0.HasAnnotation);
				goto IL_0403;
				end_IL_0062:;
			}
			catch (Exception ex)
			{
				stopwatch_0.Stop();
				pipeNetworkAnalysisResult_0.Statistics.AnalysisEndTime = DateTime.Now;
				pipeNetworkAnalysisResult_0.Statistics.AnalysisDurationMs = stopwatch_0.ElapsedMilliseconds;
				pipeNetworkAnalysisResult_0.Statistics.IsSuccess = false;
				pipeNetworkAnalysisResult_0.Statistics.ErrorMessage = ex.Message;
				Logger.Error("[CADAnalyzerService] 分析失败: " + ex.Message);
				cADAnalyzerService.method_2(action_0, ex.Message);
				result = pipeNetworkAnalysisResult_0;
			}
			int_0 = -2;
			stopwatch_0 = null;
			pipeNetworkAnalysisResult_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private readonly ICADFileService icadfileService_0;

	public CADAnalyzerService(ICADFileService cadFileService)
	{
		icadfileService_0 = cadFileService ?? throw new ArgumentNullException("cadFileService");
	}

	[AsyncStateMachine(typeof(Struct74))]
	Task<PipeNetworkAnalysisResult?> ICADAnalyzerService.AnalyzeCADFileAsync(string cadFilePath, CADAnalysisOptions options, Action<AnalysisProgress>? progressCallback = null)
	{
		Struct74 stateMachine = default(Struct74);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<PipeNetworkAnalysisResult>.Create();
		stateMachine.cadanalyzerService_0 = this;
		stateMachine.string_0 = cadFilePath;
		stateMachine.cadanalysisOptions_0 = options;
		stateMachine.action_0 = progressCallback;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	List<ManholeData> ICADAnalyzerService.ExtractManholes(CADFileData cadData, IEnumerable<string> manholeLayerNames)
	{
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Expected O, but got Unknown
		List<ManholeData> list = new List<ManholeData>();
		List<string> list2 = manholeLayerNames.ToList();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[CADAnalyzerService] 开始提取管井，图层数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		foreach (string item2 in list2)
		{
			List<CADBlockInfo> blocksByLayer = cadData.GetBlocksByLayer(item2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[CADAnalyzerService] 图层 '");
			defaultInterpolatedStringHandler2.AppendFormatted(item2);
			defaultInterpolatedStringHandler2.AppendLiteral("': ");
			defaultInterpolatedStringHandler2.AppendFormatted(blocksByLayer.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 个图块");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			foreach (CADBlockInfo item3 in blocksByLayer)
			{
				ManholeData item = new ManholeData
				{
					BlockName = item3.Name,
					X = item3.X,
					Y = item3.Y,
					Z = 0.0,
					LayerName = item3.LayerName
				};
				list.Add(item);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(31, 1);
		defaultInterpolatedStringHandler3.AppendLiteral("[CADAnalyzerService] 管井提取完成: ");
		defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler3.AppendLiteral(" 个");
		Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		return list;
	}

	List<PipeData> ICADAnalyzerService.ExtractPipes(CADFileData cadData, IEnumerable<string> pipeLayerNames)
	{
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_011a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0179: Unknown result type (might be due to invalid IL or missing references)
		//IL_0183: Expected O, but got Unknown
		List<PipeData> list = new List<PipeData>();
		List<string> list2 = pipeLayerNames.ToList();
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[CADAnalyzerService] 开始提取管道，图层数量: ");
		defaultInterpolatedStringHandler.AppendFormatted(list2.Count);
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		foreach (string item2 in list2)
		{
			List<CADLineInfo> linesByLayer = cadData.GetLinesByLayer(item2);
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler2.AppendLiteral("[CADAnalyzerService] 图层 '");
			defaultInterpolatedStringHandler2.AppendFormatted(item2);
			defaultInterpolatedStringHandler2.AppendLiteral("': ");
			defaultInterpolatedStringHandler2.AppendFormatted(linesByLayer.Count);
			defaultInterpolatedStringHandler2.AppendLiteral(" 条线条");
			Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			foreach (CADLineInfo item3 in linesByLayer)
			{
				double num = item3.EndX - item3.StartX;
				double num2 = item3.EndY - item3.StartY;
				double length = Math.Sqrt(num * num + num2 * num2);
				PipeData item = new PipeData
				{
					StartX = item3.StartX,
					StartY = item3.StartY,
					StartZ = 0.0,
					EndX = item3.EndX,
					EndY = item3.EndY,
					EndZ = 0.0,
					LayerName = item3.LayerName,
					LineType = item3.LineType,
					Length = length
				};
				list.Add(item);
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(39, 2);
		defaultInterpolatedStringHandler3.AppendLiteral("[CADAnalyzerService] 管道提取完成: ");
		defaultInterpolatedStringHandler3.AppendFormatted(list.Count);
		defaultInterpolatedStringHandler3.AppendLiteral(" 个，总长度: ");
		defaultInterpolatedStringHandler3.AppendFormatted(list.Sum((PipeData pipeData_0) => pipeData_0.Length) / 1000.0, "F2");
		defaultInterpolatedStringHandler3.AppendLiteral(" 米");
		Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
		return list;
	}

	Task<List<AnnotationLineGroup>> ICADAnalyzerService.AnalyzeManholeAnnotationsAsync(List<ManholeData> manholes, CADFileData cadData, IEnumerable<string> annotationLayerNames, CADAnalysisOptions options)
	{
		return Task.Run(() => new ManholeAnnotationAnalyzer(options.LineEndToManholeMaxDistanceMM, options.TextToManholeMaxDistanceMM, options.LineConnectionToleranceMM, options.TextToLineMaxDistanceMM, options.DirectionAngleToleranceDegrees).method_0(manholes, cadData, annotationLayerNames, options));
	}

	Task<List<AnnotationLineGroup>> ICADAnalyzerService.AnalyzePipeAnnotationsAsync(List<PipeData> pipes, CADFileData cadData, IEnumerable<string> annotationLayerNames, CADAnalysisOptions options)
	{
		return Task.Run(delegate
		{
			Logger.Warning("[CADAnalyzerService] 管道标注分析功能尚未实现");
			return new List<AnnotationLineGroup>();
		});
	}

	void ICADAnalyzerService.ConnectPipesToManholes(List<PipeData> pipes, List<ManholeData> manholes, double connectionToleranceMM = 100.0)
	{
		if (pipes != null && pipes.Count != 0)
		{
			if (manholes != null && manholes.Count != 0)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(43, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[CADAnalyzerService] 开始建立管道与管井的连接关系，容差: ");
				defaultInterpolatedStringHandler.AppendFormatted(connectionToleranceMM);
				defaultInterpolatedStringHandler.AppendLiteral(" mm");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				double num = connectionToleranceMM * connectionToleranceMM;
				int num2 = 0;
				foreach (PipeData pipe in pipes)
				{
					foreach (ManholeData manhole in manholes)
					{
						if (!(method_0(pipe.StartX, pipe.StartY, pipe.StartZ, manhole.X, manhole.Y, manhole.Z) > num))
						{
							pipe.StartManholeId = manhole.Id;
							manhole.ConnectedPipeIds.Add(pipe.Id);
							num2++;
							break;
						}
					}
					foreach (ManholeData manhole2 in manholes)
					{
						if (!(method_0(pipe.EndX, pipe.EndY, pipe.EndZ, manhole2.X, manhole2.Y, manhole2.Z) > num))
						{
							pipe.EndManholeId = manhole2.Id;
							if (pipe.StartManholeId != manhole2.Id)
							{
								manhole2.ConnectedPipeIds.Add(pipe.Id);
								num2++;
							}
							break;
						}
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("[CADAnalyzerService] 连接建立完成: ");
				defaultInterpolatedStringHandler2.AppendFormatted(num2);
				defaultInterpolatedStringHandler2.AppendLiteral(" 个连接点");
				Logger.Info(defaultInterpolatedStringHandler2.ToStringAndClear());
			}
			else
			{
				Logger.Warning("[CADAnalyzerService] 管井列表为空，无法建立连接");
			}
		}
		else
		{
			Logger.Warning("[CADAnalyzerService] 管道列表为空，无法建立连接");
		}
	}

	List<AnnotationLineGroup> ICADAnalyzerService.ConnectAnnotationLines(List<CADLineInfo> lines, double connectionToleranceMM = 10.0)
	{
		return new Class88(connectionToleranceMM).method_0(lines);
	}

	void ICADAnalyzerService.AssociateTextsWithLineGroups(List<AnnotationLineGroup> lineGroups, List<CADTextInfo> texts, double maxDistanceMM = 500.0, double angleToleranceDegrees = 15.0)
	{
		new TextLineAssociationAnalyzer(maxDistanceMM, angleToleranceDegrees).method_0(lineGroups, texts);
	}

	void ICADAnalyzerService.AssociateLineGroupsToManholes(List<ManholeData> manholes, List<AnnotationLineGroup> lineGroups, double maxDistanceMM = 500.0)
	{
		throw new NotImplementedException("请使用 AnalyzeManholeAnnotationsAsync 方法");
	}

	void ICADAnalyzerService.AssociateRemainingTextsToManholes(List<ManholeData> manholes, List<CADTextInfo> texts, double maxDistanceMM = 1000.0)
	{
		throw new NotImplementedException("请使用 AnalyzeManholeAnnotationsAsync 方法");
	}

	void ICADAnalyzerService.AssociateLineGroupsToPipes(List<PipeData> pipes, List<AnnotationLineGroup> lineGroups, double maxDistanceMM = 500.0)
	{
		throw new NotImplementedException("管道标注匹配功能尚未实现");
	}

	void ICADAnalyzerService.AssociateRemainingTextsToPipes(List<PipeData> pipes, List<CADTextInfo> texts, double maxDistanceMM = 1000.0)
	{
		throw new NotImplementedException("管道文字匹配功能尚未实现");
	}

	double ICADAnalyzerService.CalculateDistance(double x1, double y1, double z1, double x2, double y2, double z2)
	{
		double num = x2 - x1;
		double num2 = y2 - y1;
		double num3 = z2 - z1;
		return Math.Sqrt(num * num + num2 * num2 + num3 * num3);
	}

	private double method_0(double double_0, double double_1, double double_2, double double_3, double double_4, double double_5)
	{
		double num = double_3 - double_0;
		double num2 = double_4 - double_1;
		double num3 = double_5 - double_2;
		return num * num + num2 * num2 + num3 * num3;
	}

	double ICADAnalyzerService.CalculateLineDirection(double startX, double startY, double endX, double endY)
	{
		double x = endX - startX;
		double num = Math.Atan2(endY - startY, x);
		if (num < 0.0)
		{
			num += Math.PI * 2.0;
		}
		return num;
	}

	bool ICADAnalyzerService.AreAnglesParallel(double angle1, double angle2, double toleranceDegrees = 15.0)
	{
		double num = toleranceDegrees * Math.PI / 180.0;
		double num2;
		for (num2 = Math.Abs(angle1 - angle2); num2 > Math.PI; num2 -= Math.PI)
		{
		}
		if (!(num2 <= num))
		{
			return num2 >= Math.PI - num;
		}
		return true;
	}

	private void method_1(Action<AnalysisProgress>? action_0, string string_0, int int_0, string string_1)
	{
		action_0?.Invoke(AnalysisProgress.Create(string_0, int_0, string_1));
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(27, 3);
		defaultInterpolatedStringHandler.AppendLiteral("[CADAnalyzerService] [");
		defaultInterpolatedStringHandler.AppendFormatted(string_0);
		defaultInterpolatedStringHandler.AppendLiteral("] ");
		defaultInterpolatedStringHandler.AppendFormatted(int_0);
		defaultInterpolatedStringHandler.AppendLiteral("%: ");
		defaultInterpolatedStringHandler.AppendFormatted(string_1);
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private void method_2(Action<AnalysisProgress>? action_0, string string_0)
	{
		action_0?.Invoke(AnalysisProgress.CreateError(string_0));
	}
}
