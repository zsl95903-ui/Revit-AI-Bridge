using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Revit.Models;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using ns0;
using ns6;

namespace RevitAi.Revit.AI.Tools;

[AITool("create_stair_type", Category = "建模工具", Description = "创建自定义楼梯类型，支持设置梯段结构深度和平台整体厚度。会自动复制现有楼梯类型并创建对应的整体梯段和整体平台类型。", RequiresTransaction = true, RequiresModification = true)]
public sealed class CreateStairTypeTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class343 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public CreateStairTypeTool createStairTypeTool_0;

		private string string_0;

		private string string_1;

		private double double_0;

		private double double_1;

		private object object_0;

		private IStairTypeManagerService istairTypeManagerService_0;

		private string string_2;

		private StairsType stairsType_0;

		private StairsType stairsType_1;

		private Exception exception_0;

		void IAsyncStateMachine.MoveNext()
		{
			AIToolResult result;
			try
			{
				string_0 = aitoolContext_0.GetParameter<string>("newStairTypeName", (string)null);
				string_1 = aitoolContext_0.GetParameter<string>("baseStairTypeName", string.Empty);
				double_0 = aitoolContext_0.GetParameter<double>("runThicknessMm", 150.0);
				double_1 = aitoolContext_0.GetParameter<double>("landingThicknessMm", 300.0);
				if (string.IsNullOrWhiteSpace(string_0))
				{
					result = AIToolResult.Fail("新楼梯类型名称不能为空");
				}
				else
				{
					if (double_0 < 50.0 || double_0 > 500.0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
						defaultInterpolatedStringHandler.AppendLiteral("[AI工具] 梯段结构深度 ");
						defaultInterpolatedStringHandler.AppendFormatted(double_0);
						defaultInterpolatedStringHandler.AppendLiteral("mm 超出推荐范围（50-500mm）");
						Logger.Warning(defaultInterpolatedStringHandler.ToStringAndClear());
					}
					if (double_1 < 100.0 || double_1 > 800.0)
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(34, 1);
						defaultInterpolatedStringHandler2.AppendLiteral("[AI工具] 平台整体厚度 ");
						defaultInterpolatedStringHandler2.AppendFormatted(double_1);
						defaultInterpolatedStringHandler2.AppendLiteral("mm 超出推荐范围（100-800mm）");
						Logger.Warning(defaultInterpolatedStringHandler2.ToStringAndClear());
					}
					Logger.Info("[AI工具] 开始创建自定义楼梯类型: " + string_0);
					Logger.Info("[AI工具]   基础类型: " + (string_1 ?? "(自动查找现浇)"));
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[AI工具]   梯段结构深度: ");
					defaultInterpolatedStringHandler3.AppendFormatted(double_0);
					defaultInterpolatedStringHandler3.AppendLiteral("mm");
					Logger.Info(defaultInterpolatedStringHandler3.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(19, 1);
					defaultInterpolatedStringHandler4.AppendLiteral("[AI工具]   平台整体厚度: ");
					defaultInterpolatedStringHandler4.AppendFormatted(double_1);
					defaultInterpolatedStringHandler4.AppendLiteral("mm");
					Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
					object_0 = createStairTypeTool_0.irevitAdapter_0.GetStairTypeManagerService();
					istairTypeManagerService_0 = object_0 as IStairTypeManagerService;
					if (istairTypeManagerService_0 == null)
					{
						result = AIToolResult.Fail("无法获取楼梯类型管理服务");
					}
					else
					{
						string_2 = string_1;
						if (!string.IsNullOrWhiteSpace(string_2))
						{
							goto IL_030e;
						}
						stairsType_1 = istairTypeManagerService_0.FindCastInPlaceStairType();
						if (stairsType_1 != null)
						{
							string_2 = ((Element)stairsType_1).Name;
							Logger.Info("[AI工具] 自动选择现浇楼梯类型: " + string_2);
							stairsType_1 = null;
							goto IL_030e;
						}
						result = AIToolResult.Fail("未找到现浇楼梯类型，请指定基础楼梯类型名称");
					}
				}
				goto end_IL_0002;
				IL_030e:
				stairsType_0 = istairTypeManagerService_0.CreateCustomStairType(string_2 ?? "", string_0, double_0, double_1);
				if (stairsType_0 == null)
				{
					result = AIToolResult.Fail("创建楼梯类型失败");
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(24, 2);
					defaultInterpolatedStringHandler5.AppendLiteral("[AI工具] 成功创建楼梯类型: ");
					defaultInterpolatedStringHandler5.AppendFormatted(((Element)stairsType_0).Name);
					defaultInterpolatedStringHandler5.AppendLiteral(" (ID: ");
					defaultInterpolatedStringHandler5.AppendFormatted(((Element)stairsType_0).Id.Value);
					defaultInterpolatedStringHandler5.AppendLiteral(")");
					Logger.Info(defaultInterpolatedStringHandler5.ToStringAndClear());
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(33, 3);
					defaultInterpolatedStringHandler6.AppendLiteral("成功创建楼梯类型「");
					defaultInterpolatedStringHandler6.AppendFormatted(((Element)stairsType_0).Name);
					defaultInterpolatedStringHandler6.AppendLiteral("」，参数：梯段结构深度 ");
					defaultInterpolatedStringHandler6.AppendFormatted(double_0);
					defaultInterpolatedStringHandler6.AppendLiteral("mm，平台整体厚度 ");
					defaultInterpolatedStringHandler6.AppendFormatted(double_1);
					defaultInterpolatedStringHandler6.AppendLiteral("mm");
					result = AIToolResult.Ok(defaultInterpolatedStringHandler6.ToStringAndClear(), (object)new Class84<long, string, string, double, double>(((Element)stairsType_0).Id.Value, ((Element)stairsType_0).Name, string_2, double_0, double_1));
				}
				end_IL_0002:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[AI工具] 创建楼梯类型失败", exception_0);
				result = AIToolResult.Fail("创建失败：" + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly IRevitAdapter irevitAdapter_0;

	public string Name => "create_stair_type";

	public string Category => "建模工具";

	public string Description => "创建自定义楼梯类型";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"newStairTypeName\": {\n                \"type\": \"string\",\n                \"description\": \"新楼梯类型的名称，必须唯一且不能与现有类型重复\"\n            },\n            \"baseStairTypeName\": {\n                \"type\": \"string\",\n                \"description\": \"基础楼梯类型名称（用于复制），如果为空则自动查找现浇楼梯类型\"\n            },\n            \"runThicknessMm\": {\n                \"type\": \"number\",\n                \"description\": \"梯段结构深度（毫米），即整体梯段的厚度，默认 150mm，推荐范围 100-300mm\",\n                \"default\": 150\n            },\n            \"landingThicknessMm\": {\n                \"type\": \"number\",\n                \"description\": \"平台整体厚度（毫米），即整体平台的厚度，默认 300mm，推荐范围 200-500mm\",\n                \"default\": 300\n            }\n        },\n        \"required\": [\"newStairTypeName\"]\n    }";

	public CreateStairTypeTool(IRevitAdapter revitAdapter)
	{
		irevitAdapter_0 = revitAdapter ?? throw new ArgumentNullException("revitAdapter");
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class343))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class343 stateMachine = new Class343();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.createStairTypeTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}
}
