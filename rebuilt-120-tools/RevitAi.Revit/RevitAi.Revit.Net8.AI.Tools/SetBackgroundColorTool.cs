using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("set_background_color", Category = "视图管理", Description = "设置 Revit 视图的背景颜色。支持使用十六进制颜色代码（如 #FF0000 表示红色）或 RGB 值（0-255）来指定颜色。如果未指定颜色，则在黑色和白色之间切换。", RequiresTransaction = false, RequiresModification = false)]
public sealed class SetBackgroundColorTool : IAITool
{
	[CompilerGenerated]
	public sealed class Class626 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public SetBackgroundColorTool setBackgroundColorTool_0;

		private int int_1;

		private int int_2;

		private int int_3;

		private string string_0;

		private bool bool_0;

		private IApplicationService iapplicationService_0;

		private byte byte_0;

		private byte byte_1;

		private byte byte_2;

		private byte byte_3;

		private byte byte_4;

		private byte byte_5;

		private string string_1;

		private (byte red, byte green, byte blue) valueTuple_0;

		private byte byte_6;

		private byte byte_7;

		private byte byte_8;

		private Exception exception_0;

		private TaskAwaiter taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			TaskAwaiter awaiter;
			if (int_0 != 0)
			{
				awaiter = Task.CompletedTask.GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					int num = 0;
					int_0 = 0;
					taskAwaiter_0 = awaiter;
					Class626 stateMachine = this;
					asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
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
			AIToolResult result;
			try
			{
				int_1 = aitoolContext_0.GetParameter<int>("red", -1);
				int_2 = aitoolContext_0.GetParameter<int>("green", -1);
				int_3 = aitoolContext_0.GetParameter<int>("blue", -1);
				string_0 = aitoolContext_0.GetParameter<string>("hexColor", (string)null);
				bool_0 = aitoolContext_0.GetParameter<bool>("toggle", false);
				iapplicationService_0 = setBackgroundColorTool_0.iapplicationService_0;
				(byte_0, byte_1, byte_2) = iapplicationService_0.GetBackgroundColor();
				if (bool_0)
				{
					if (byte_0 == 0 && byte_1 == 0 && byte_2 == 0)
					{
						iapplicationService_0.SetBackgroundColor(byte.MaxValue, byte.MaxValue, byte.MaxValue);
						byte_3 = byte.MaxValue;
						byte_4 = byte.MaxValue;
						byte_5 = byte.MaxValue;
					}
					else
					{
						iapplicationService_0.SetBackgroundColor((byte)0, (byte)0, (byte)0);
						byte_3 = 0;
						byte_4 = 0;
						byte_5 = 0;
					}
					goto IL_0355;
				}
				if (!string.IsNullOrEmpty(string_0))
				{
					valueTuple_0 = setBackgroundColorTool_0.method_0(string_0);
					iapplicationService_0.SetBackgroundColor(valueTuple_0.red, valueTuple_0.green, valueTuple_0.blue);
					byte_3 = valueTuple_0.red;
					byte_4 = valueTuple_0.green;
					byte_5 = valueTuple_0.blue;
					goto IL_0355;
				}
				if (int_1 < 0 || int_2 < 0 || int_3 < 0)
				{
					(byte, byte, byte) tuple2 = iapplicationService_0.ToggleBackgroundColor();
					byte_6 = tuple2.Item1;
					byte_7 = tuple2.Item2;
					byte_8 = tuple2.Item3;
					byte_3 = byte_6;
					byte_4 = byte_7;
					byte_5 = byte_8;
					goto IL_0355;
				}
				if (int_1 <= 255 && int_2 <= 255 && int_3 <= 255)
				{
					iapplicationService_0.SetBackgroundColor((byte)int_1, (byte)int_2, (byte)int_3);
					byte_3 = (byte)int_1;
					byte_4 = (byte)int_2;
					byte_5 = (byte)int_3;
					goto IL_0355;
				}
				result = AIToolResult.Fail("RGB 值必须在 0-255 范围内");
				goto end_IL_0067;
				IL_0355:
				if (byte_3 == 0 && byte_4 == 0 && byte_5 == 0)
				{
					string_1 = "黑色 (RGB: 0, 0, 0)";
				}
				else if (byte_3 == byte.MaxValue && byte_4 == byte.MaxValue && byte_5 == byte.MaxValue)
				{
					string_1 = "白色 (RGB: 255, 255, 255)";
				}
				else
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 3);
					defaultInterpolatedStringHandler.AppendLiteral("自定义颜色 (RGB: ");
					defaultInterpolatedStringHandler.AppendFormatted(byte_3);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(byte_4);
					defaultInterpolatedStringHandler.AppendLiteral(", ");
					defaultInterpolatedStringHandler.AppendFormatted(byte_5);
					defaultInterpolatedStringHandler.AppendLiteral(")");
					string_1 = defaultInterpolatedStringHandler.ToStringAndClear();
				}
				Logger.Info("[AI工具] 设置视图背景色为: " + string_1);
				string text = "视图背景色已设置为：" + string_1;
				byte gparam_ = byte_3;
				byte gparam_2 = byte_4;
				byte gparam_3 = byte_5;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(1, 3);
				defaultInterpolatedStringHandler2.AppendLiteral("#");
				defaultInterpolatedStringHandler2.AppendFormatted(byte_3, "X2");
				defaultInterpolatedStringHandler2.AppendFormatted(byte_4, "X2");
				defaultInterpolatedStringHandler2.AppendFormatted(byte_5, "X2");
				result = AIToolResult.Ok(text, (object)new Class294<byte, byte, byte, string, string>(gparam_, gparam_2, gparam_3, defaultInterpolatedStringHandler2.ToStringAndClear(), string_1));
				end_IL_0067:;
			}
			catch (Exception ex)
			{
				exception_0 = ex;
				Logger.Error("[AI工具] 设置视图背景色失败", exception_0);
				result = AIToolResult.Fail("设置背景色失败: " + exception_0.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	private readonly IApplicationService iapplicationService_0;

	public string Name => "set_background_color";

	public string Category => "视图管理";

	public string Description => "设置 Revit 视图的背景颜色";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"red\": {\n                \"type\": \"integer\",\n                \"description\": \"红色的 RGB 值（0-255），可选。与 green 和 blue 配合使用。\"\n            },\n            \"green\": {\n                \"type\": \"integer\",\n                \"description\": \"绿色的 RGB 值（0-255），可选。与 red 和 blue 配合使用。\"\n            },\n            \"blue\": {\n                \"type\": \"integer\",\n                \"description\": \"蓝色的 RGB 值（0-255），可选。与 red 和 green 配合使用。\"\n            },\n            \"hexColor\": {\n                \"type\": \"string\",\n                \"description\": \"十六进制颜色代码（如 #FF0000 表示红色、#00FF00 表示绿色、#0000FF 表示蓝色），可选。格式必须为 #RRGGBB。\"\n            },\n            \"toggle\": {\n                \"type\": \"boolean\",\n                \"description\": \"是否在黑色和白色之间切换，可选。如果为 true，则忽略其他颜色参数，直接在当前背景色（黑色切换为白色，白色切换为黑色，其他颜色切换为白色）之间切换。\"\n            }\n        }\n    }";

	public SetBackgroundColorTool(IApplicationService applicationService)
	{
		iapplicationService_0 = applicationService ?? throw new ArgumentNullException("applicationService");
	}

	[DebuggerStepThrough]
	[AsyncStateMachine(typeof(Class626))]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class626 stateMachine = new Class626();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.setBackgroundColorTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private (byte red, byte green, byte blue) method_0(string string_0)
	{
		try
		{
			string text = string_0.TrimStart('#');
			if (text.Length != 6)
			{
				throw new ArgumentException("十六进制颜色代码必须为 6 位（如 #FF0000）");
			}
			int num = Convert.ToInt32(text.Substring(0, 2), 16);
			int num2 = Convert.ToInt32(text.Substring(2, 2), 16);
			int num3 = Convert.ToInt32(text.Substring(4, 2), 16);
			return (red: (byte)num, green: (byte)num2, blue: (byte)num3);
		}
		catch (Exception ex)
		{
			Logger.Error("[SetBackgroundColorTool] 解析十六进制颜色失败: " + string_0, ex);
			throw new ArgumentException("无效的十六进制颜色代码: " + string_0, ex);
		}
	}
}
