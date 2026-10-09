using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("get_links", Category = "链接管理", Description = "获取文档中所有链接实例（包含 Revit 链接模型和 CAD 导入/链接）", RequiresTransaction = false, RequiresModification = false)]
public sealed class GetLinksTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class502 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public GetLinksTool getLinksTool_0;

		private ILinkService ilinkService_0;

		private IElementService ielementService_0;

		private ICADGeometryService icadgeometryService_0;

		private ICADFileService icadfileService_0;

		private List<object> list_0;

		private IEnumerable<object> ienumerable_0;

		private IEnumerator<object> ienumerator_0;

		private object object_0;

		private int? nullable_0;

		private (double X, double Y, double Z)? nullable_1;

		private double? nullable_2;

		private object object_1;

		private string string_0;

		private string string_1;

		private bool bool_0;

		private int? nullable_3;

		private (string? FilePath, string? LinkType, bool IsLoaded, int? LinkTypeId)? nullable_4;

		private string string_2;

		private string string_3;

		private bool bool_1;

		private int? nullable_5;

		private IEnumerable<object> ienumerable_1;

		private IEnumerator<object> ienumerator_1;

		private object object_2;

		private int? nullable_6;

		private CADFileInfo cadfileInfo_0;

		private string string_4;

		private string string_5;

		private string string_6;

		private double double_0;

		private (double X, double Y, double Z)? nullable_7;

		private double? nullable_8;

		private int? nullable_9;

		private int? nullable_10;

		private string string_7;

		private string string_8;

		private object object_3;

		private object object_4;

		private Exception exception_0;

		private Exception exception_1;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class502 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter = taskAwaiter_0;
				taskAwaiter_0 = default(TaskAwaiter);
				num = -1;
				int_0 = -1;
			}
			awaiter.GetResult();
			AIToolResult result;
			try
			{
				IRevitAdapter revitAdapter = aitoolContext_0.RevitAdapter;
				ilinkService_0 = ((revitAdapter != null) ? revitAdapter.LinkService : null);
				IRevitAdapter revitAdapter2 = aitoolContext_0.RevitAdapter;
				ielementService_0 = ((revitAdapter2 != null) ? revitAdapter2.ElementService : null);
				IRevitAdapter revitAdapter3 = aitoolContext_0.RevitAdapter;
				icadgeometryService_0 = ((revitAdapter3 != null) ? revitAdapter3.CADGeometryService : null);
				IRevitAdapter revitAdapter4 = aitoolContext_0.RevitAdapter;
				icadfileService_0 = ((revitAdapter4 != null) ? revitAdapter4.CADFileService : null);
				if (ilinkService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 LinkService");
				}
				else if (ielementService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ElementService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					list_0 = new List<object>();
					ienumerable_0 = ilinkService_0.GetAllLinkInstances(aitoolContext_0.Document);
					ienumerator_0 = ienumerable_0.GetEnumerator();
					try
					{
						while (ienumerator_0.MoveNext())
						{
							object_0 = ienumerator_0.Current;
							nullable_0 = ielementService_0.GetElementId(object_0);
							nullable_1 = ilinkService_0.GetLinkPosition(object_0);
							nullable_2 = ilinkService_0.GetLinkRotation(object_0);
							object_1 = ilinkService_0.GetLinkTypeFromInstance(object_0);
							string_0 = "未知";
							string_1 = "";
							bool_0 = false;
							nullable_3 = null;
							if (object_1 != null)
							{
								nullable_4 = ilinkService_0.GetLinkInfo(object_1);
								if (nullable_4.HasValue)
								{
									(string, string, bool, int?) value = nullable_4.Value;
									string_2 = value.Item1;
									string_3 = value.Item2;
									bool_1 = value.Item3;
									nullable_5 = value.Item4;
									string_0 = string_3 ?? "未知";
									string_1 = string_2 ?? "";
									bool_0 = bool_1;
									nullable_3 = nullable_5;
									string_2 = null;
									string_3 = null;
								}
								nullable_4 = null;
							}
							list_0.Add(new Class185<int?, int?, string, string, string, Class35<double, double, double, string>, Class186<double, string>, string>(nullable_0, nullable_3, string_0, string_1, bool_0 ? "已加载" : "已卸载", nullable_1.HasValue ? new Class35<double, double, double, string>(Math.Round(nullable_1.Value.X, 3), Math.Round(nullable_1.Value.Y, 3), Math.Round(nullable_1.Value.Z, 3), "米") : null, nullable_2.HasValue ? new Class186<double, string>(Math.Round(nullable_2.Value, 2), "度") : null, "Revit"));
							object_1 = null;
							string_0 = null;
							string_1 = null;
							object_0 = null;
						}
					}
					finally
					{
						if (num < 0 && ienumerator_0 != null)
						{
							ienumerator_0.Dispose();
						}
					}
					ienumerator_0 = null;
					if (icadgeometryService_0 != null && icadfileService_0 != null)
					{
						ienumerable_1 = icadgeometryService_0.GetCADImportInstances(aitoolContext_0.Document);
						ienumerator_1 = ienumerable_1.GetEnumerator();
						try
						{
							while (ienumerator_1.MoveNext())
							{
								object_2 = ienumerator_1.Current;
								try
								{
									nullable_6 = ielementService_0.GetElementId(object_2);
									cadfileInfo_0 = icadfileService_0.GetCADFilePath(object_2, aitoolContext_0.Document);
									string_4 = "CAD 链接";
									string_5 = "";
									string_6 = "未知";
									double_0 = 1.0;
									if (cadfileInfo_0 != null)
									{
										string_4 = Path.GetFileNameWithoutExtension(cadfileInfo_0.FilePath);
										string_5 = cadfileInfo_0.FilePath;
										string_6 = cadfileInfo_0.ImportUnit;
										double_0 = cadfileInfo_0.ScaleFactor;
									}
									nullable_7 = icadgeometryService_0.GetCADImportPosition(object_2);
									nullable_8 = icadgeometryService_0.GetCADImportRotation(object_2);
									nullable_9 = icadgeometryService_0.GetCADImportOwnerViewId(object_2);
									nullable_10 = icadgeometryService_0.GetCADImportLevelId(object_2);
									string_7 = null;
									string_8 = null;
									if (nullable_9.HasValue)
									{
										object_3 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_9.Value);
										if (object_3 != null)
										{
											string_7 = ielementService_0.GetElementName(object_3);
										}
										object_3 = null;
									}
									if (nullable_10.HasValue)
									{
										object_4 = ielementService_0.GetElementById(aitoolContext_0.Document, nullable_10.Value);
										if (object_4 != null)
										{
											string_8 = ielementService_0.GetElementName(object_4);
										}
										object_4 = null;
									}
									list_0.Add(new Class187<int?, string, string, string, double, Class35<double, double, double, string>, Class186<double, string>, string, string, string>(nullable_6, string_4, string_5, string_6, double_0, nullable_7.HasValue ? new Class35<double, double, double, string>(Math.Round(nullable_7.Value.X, 3), Math.Round(nullable_7.Value.Y, 3), Math.Round(nullable_7.Value.Z, 3), "米") : null, nullable_8.HasValue ? new Class186<double, string>(Math.Round(nullable_8.Value, 2), "度") : null, string_7 ?? "模型视图", string_8 ?? "无", "CAD"));
									cadfileInfo_0 = null;
									string_4 = null;
									string_5 = null;
									string_6 = null;
									string_7 = null;
									string_8 = null;
								}
								catch (Exception ex)
								{
									exception_0 = ex;
									Logger.Warning("[GetLinksTool] 处理 CAD 导入实例时出错: " + exception_0.Message);
								}
								object_2 = null;
							}
						}
						finally
						{
							if (num < 0 && ienumerator_1 != null)
							{
								ienumerator_1.Dispose();
							}
						}
						ienumerator_1 = null;
						ienumerable_1 = null;
					}
					if (list_0.Count == 0)
					{
						result = AIToolResult.Ok("当前文档中没有链接实例", (object)new Class188<int, object[]>(0, Array.Empty<object>()));
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(9, 1);
						defaultInterpolatedStringHandler.AppendLiteral("找到 ");
						defaultInterpolatedStringHandler.AppendFormatted(list_0.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个链接实例");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler.ToStringAndClear(), (object)new Class188<int, List<object>>(list_0.Count, list_0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_1 = ex;
				Logger.Error("[GetLinksTool] 获取链接失败: " + exception_1.Message);
				result = AIToolResult.Fail("获取链接失败: " + exception_1.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "get_links";

	public string Category => "链接管理";

	public string Description => "获取文档中所有链接实例（Revit 链接和 CAD 链接）";

	public string ParametersSchema => "\r\n    {\r\n        \"type\": \"object\",\r\n        \"properties\": {},\r\n        \"required\": []\r\n    }";

	[AsyncStateMachine(typeof(Class502))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class502 stateMachine = new Class502();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.getLinksTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
