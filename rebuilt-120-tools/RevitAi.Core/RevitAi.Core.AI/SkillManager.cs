using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.AI.Models;
using RevitAi.Abstractions.Common;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Update;
using Newtonsoft.Json;
using ns0;
using ns7;

namespace RevitAi.Core.AI;

public sealed class SkillManager : ISkillManager
{
	[CompilerGenerated]
	public sealed class Class132
	{
		public SkillInfo skillInfo_0;

		internal bool method_0(SkillInfo skillInfo_1)
		{
			return skillInfo_1.SkillName == skillInfo_0.SkillName;
		}
	}

	[CompilerGenerated]
	public sealed class Class133
	{
		public string string_0;

		internal bool method_0(SkillInfo skillInfo_0)
		{
			return skillInfo_0.SkillName.Equals(string_0, StringComparison.OrdinalIgnoreCase);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct207 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<bool> asyncTaskMethodBuilder_0;

		public SkillManager skillManager_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<string?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SkillManager skillManager = skillManager_0;
			bool result2;
			try
			{
				TaskAwaiter<string> awaiter;
				if (num != 0)
				{
					Logger.Info("[SkillManager] 检查技能更新...");
					awaiter = skillManager.iupdateService_0.DownloadTextAsync("configs/skills.json", cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<string>);
					num = -1;
					int_0 = -1;
				}
				string result = awaiter.GetResult();
				if (string.IsNullOrEmpty(result))
				{
					Logger.Info("[SkillManager] 下载 skills.json 失败或文件不存在");
					result2 = false;
				}
				else
				{
					SkillsConfig val = JsonConvert.DeserializeObject<SkillsConfig>(result);
					if (val == null)
					{
						Logger.Warning("[SkillManager] 解析 skills.json 失败");
						result2 = false;
					}
					else if (skillManager.method_4(val.Version, skillManager.string_6) <= 0)
					{
						Logger.Info("[SkillManager] 已是最新技能版本: " + skillManager.string_6);
						result2 = false;
					}
					else
					{
						Logger.Info("[SkillManager] 发现新技能版本: " + skillManager.string_6 + " → " + val.Version);
						skillManager.method_2(result, val.Version);
						skillManager.method_3(val.Skills);
						skillManager.string_6 = val.Version;
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(36, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[SkillManager] ✅ 技能更新成功（版本: ");
						defaultInterpolatedStringHandler.AppendFormatted(val.Version);
						defaultInterpolatedStringHandler.AppendLiteral("，共 ");
						defaultInterpolatedStringHandler.AppendFormatted(val.Skills.Count);
						defaultInterpolatedStringHandler.AppendLiteral(" 个技能）");
						Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
						result2 = true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[SkillManager] COS 更新失败: " + ex.Message, ex);
				result2 = false;
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result2);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct208 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<Result<List<AIToolResult>>> asyncTaskMethodBuilder_0;

		public SkillManager skillManager_0;

		public string string_0;

		public CancellationToken cancellationToken_0;

		public IDictionary<string, object> idictionary_0;

		public AIToolContext aitoolContext_0;

		private List<AIToolResult> list_0;

		private IEnumerator<SkillStep> ienumerator_0;

		private SkillStep skillStep_0;

		private TaskAwaiter<AIToolResult?> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SkillManager skillManager = skillManager_0;
			Result<List<AIToolResult>> result;
			if (num != 0)
			{
				Result<SkillInfo> skillDetail = skillManager.GetSkillDetail(string_0);
				if (!skillDetail.IsSuccess || skillDetail.Value == null)
				{
					result = Result<List<AIToolResult>>.Failure("技能不存在: " + string_0);
					goto IL_035a;
				}
				SkillInfo value = skillDetail.Value;
				list_0 = new List<AIToolResult>();
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(29, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[SkillManager] 开始执行技能: ");
				defaultInterpolatedStringHandler.AppendFormatted(string_0);
				defaultInterpolatedStringHandler.AppendLiteral("（");
				defaultInterpolatedStringHandler.AppendFormatted(value.Steps.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个步骤）");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
				ienumerator_0 = value.Steps.OrderBy((SkillStep skillStep_0) => skillStep_0.Order).GetEnumerator();
			}
			try
			{
				if (num == 0)
				{
					goto IL_0113;
				}
				goto IL_027b;
				IL_0113:
				try
				{
					TaskAwaiter<AIToolResult> awaiter;
					if (num != 0)
					{
						awaiter = skillManager.method_5(skillStep_0, idictionary_0, aitoolContext_0, cancellationToken_0).GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
						num = -1;
						int_0 = -1;
					}
					AIToolResult result2 = awaiter.GetResult();
					if (result2 == null)
					{
						goto end_IL_0113;
					}
					list_0.Add(result2);
					if (result2.Success)
					{
						goto end_IL_0113;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(27, 2);
					defaultInterpolatedStringHandler2.AppendLiteral("[SkillManager] 技能执行失败，步骤 ");
					defaultInterpolatedStringHandler2.AppendFormatted(skillStep_0.Order);
					defaultInterpolatedStringHandler2.AppendLiteral(": ");
					defaultInterpolatedStringHandler2.AppendFormatted(result2.Error);
					Logger.Error(defaultInterpolatedStringHandler2.ToStringAndClear());
					result = Result<List<AIToolResult>>.Success(list_0);
					goto end_IL_010c;
					end_IL_0113:;
				}
				catch (Exception ex)
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(23, 1);
					defaultInterpolatedStringHandler3.AppendLiteral("[SkillManager] 执行步骤失败: ");
					defaultInterpolatedStringHandler3.AppendFormatted(skillStep_0.Order);
					Logger.Error(defaultInterpolatedStringHandler3.ToStringAndClear(), ex);
					list_0.Add(AIToolResult.Fail("步骤执行异常: " + ex.Message));
				}
				skillStep_0 = null;
				goto IL_027b;
				IL_027b:
				if (ienumerator_0.MoveNext())
				{
					skillStep_0 = ienumerator_0.Current;
					if (!cancellationToken_0.IsCancellationRequested)
					{
						goto IL_0113;
					}
					Logger.Info("[SkillManager] 技能执行已取消");
				}
				goto IL_02dd;
				end_IL_010c:;
			}
			finally
			{
				if (num < 0 && ienumerator_0 != null)
				{
					ienumerator_0.Dispose();
				}
			}
			goto IL_035a;
			IL_035a:
			int_0 = -2;
			list_0 = null;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_02dd:
			ienumerator_0 = null;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(32, 2);
			defaultInterpolatedStringHandler4.AppendLiteral("[SkillManager] 技能 '");
			defaultInterpolatedStringHandler4.AppendFormatted(string_0);
			defaultInterpolatedStringHandler4.AppendLiteral("' 执行完成，共 ");
			defaultInterpolatedStringHandler4.AppendFormatted(list_0.Count);
			defaultInterpolatedStringHandler4.AppendLiteral(" 个步骤");
			Logger.Info(defaultInterpolatedStringHandler4.ToStringAndClear());
			result = Result<List<AIToolResult>>.Success(list_0);
			goto IL_035a;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct209 : IAsyncStateMachine
	{
			private IAITool tool;
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public SkillStep skillStep_0;

		public SkillManager skillManager_0;

		public IDictionary<string, object> idictionary_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		private TaskAwaiter<AIToolResult> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SkillManager skillManager = skillManager_0;
			if (num == 0)
			{
				goto IL_00ef;
			}
			AIToolResult result;
			if (string.IsNullOrEmpty(skillStep_0.Tool))
			{
				result = AIToolResult.Fail("工具名称为空");
			}
			else
			{
				object obj = skillManager.method_6(skillStep_0.Parameters, idictionary_0);
				tool = skillManager.iaitoolRegistry_0.GetTool(skillStep_0.Tool);
				if (tool != null)
				{
					if (obj is IDictionary<string, object> dictionary)
					{
						IEnumerator<KeyValuePair<string, object>> enumerator = dictionary.GetEnumerator();
						try
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<string, object> current = enumerator.Current;
								aitoolContext_0.Parameters[current.Key] = current.Value;
							}
						}
						finally
						{
							if (num < 0)
							{
								enumerator?.Dispose();
							}
						}
					}
					goto IL_00ef;
				}
				result = AIToolResult.Fail("工具不存在: " + skillStep_0.Tool);
			}
			goto IL_019e;
			IL_019e:
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
			return;
			IL_00ef:
			try
			{
				TaskAwaiter<AIToolResult> awaiter;
				if (num != 0)
				{
					awaiter = tool.ExecuteAsync(aitoolContext_0, cancellationToken_0).GetAwaiter();
					if (!awaiter.IsCompleted)
					{
						num = 0;
						int_0 = 0;
						taskAwaiter_0 = awaiter;
						asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
						return;
					}
				}
				else
				{
					awaiter = taskAwaiter_0;
					taskAwaiter_0 = default(TaskAwaiter<AIToolResult>);
					num = -1;
					int_0 = -1;
				}
				result = awaiter.GetResult();
			}
			catch (Exception ex)
			{
				Logger.Error("[SkillManager] 工具执行失败: " + skillStep_0.Tool, ex);
				result = AIToolResult.Fail("工具执行异常: " + ex.Message);
			}
			goto IL_019e;
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	[StructLayout(LayoutKind.Auto)]
	[CompilerGenerated]
	public struct Struct210 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder asyncTaskMethodBuilder_0;

		public SkillManager skillManager_0;

		private TaskAwaiter<bool> taskAwaiter_0;

		void IAsyncStateMachine.MoveNext()
		{
			int num = int_0;
			SkillManager skillManager = skillManager_0;
			if (num != 0)
			{
				Logger.Info("[SkillManager] 初始化技能管理器...");
			}
			try
			{
				if (num != 0)
				{
					skillManager.method_0();
					skillManager.method_1();
				}
				try
				{
					TaskAwaiter<bool> awaiter;
					if (num != 0)
					{
						awaiter = skillManager.CheckAndUpdateFromCOSAsync().GetAwaiter();
						if (!awaiter.IsCompleted)
						{
							num = 0;
							int_0 = 0;
							taskAwaiter_0 = awaiter;
							asyncTaskMethodBuilder_0.AwaitUnsafeOnCompleted(ref awaiter, ref this);
							return;
						}
					}
					else
					{
						awaiter = taskAwaiter_0;
						taskAwaiter_0 = default(TaskAwaiter<bool>);
						num = -1;
						int_0 = -1;
					}
					awaiter.GetResult();
				}
				catch (Exception ex)
				{
					Logger.Warning("[SkillManager] COS 更新失败，使用本地/内置技能: " + ex.Message);
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(31, 1);
				defaultInterpolatedStringHandler.AppendLiteral("[SkillManager] ✅ 初始化完成，加载了 ");
				defaultInterpolatedStringHandler.AppendFormatted(skillManager.list_0.Count);
				defaultInterpolatedStringHandler.AppendLiteral(" 个技能");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			catch (Exception ex2)
			{
				Logger.Error("[SkillManager] 初始化失败: " + ex2.Message, ex2);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult();
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
			asyncTaskMethodBuilder_0.SetStateMachine(iasyncStateMachine_0);
		}
	}

	private const string string_0 = "RevitAi";

	private const string string_1 = "skills.json";

	private const string string_2 = "skills_version.txt";

	private const string string_3 = "configs/skills.json";

	private readonly IUpdateService iupdateService_0;

	private readonly IAIToolRegistry iaitoolRegistry_0;

	private readonly string string_4;

	private readonly string string_5;

	private List<SkillInfo> list_0 = new List<SkillInfo>();

	private string string_6 = "0.0.0";

	private readonly object object_0 = new object();

	public SkillManager(IUpdateService updateService, IAIToolRegistry toolRegistry)
	{
		iupdateService_0 = updateService ?? throw new ArgumentNullException("updateService");
		iaitoolRegistry_0 = toolRegistry ?? throw new ArgumentNullException("toolRegistry");
		string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
		string_4 = Path.Combine(folderPath, "RevitAi", "skills.json");
		string_5 = Path.Combine(folderPath, "RevitAi", "skills_version.txt");
	}

	[AsyncStateMachine(typeof(Struct210))]
	public Task InitializeAsync()
	{
		Struct210 stateMachine = default(Struct210);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder.Create();
		stateMachine.skillManager_0 = this;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_0()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008a: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Expected O, but got Unknown
		//IL_00e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Expected O, but got Unknown
		//IL_0140: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_016c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Expected O, but got Unknown
		//IL_0192: Expected O, but got Unknown
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Expected O, but got Unknown
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Expected O, but got Unknown
		//IL_0295: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_02aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Expected O, but got Unknown
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_030c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0318: Expected O, but got Unknown
		//IL_0322: Expected O, but got Unknown
		//IL_0328: Expected O, but got Unknown
		Logger.Info("[SkillManager] 加载内置技能...");
		SkillsConfig val = new SkillsConfig
		{
			Version = "1.0.0",
			UpdatedAt = DateTime.UtcNow,
			Skills = new List<SkillInfo>
			{
				new SkillInfo
				{
					SkillName = "create_wall_with_opening",
					DisplayName = "创建带门洞的墙",
					Description = "创建一面墙并在指定位置添加门洞",
					Category = "建模",
					Priority = 10,
					Steps = new List<SkillStep>
					{
						new SkillStep
						{
							Order = 1,
							Tool = "create_straight_wall",
							Parameters = new Class11<Class12<int, int, int>, Class12<int, int, int>, string>(new Class12<int, int, int>(0, 0, 0), new Class12<int, int, int>(5000, 0, 3000), "{{levelId}}"),
							Description = "创建墙体"
						},
						new SkillStep
						{
							Order = 2,
							Tool = "create_door",
							Parameters = new Class13<Class12<int, int, int>, string>(new Class12<int, int, int>(2500, 0, 0), "{{levelId}}"),
							Description = "在墙中间创建门"
						}
					},
					Variables = new List<SkillVariable>
					{
						new SkillVariable
						{
							Name = "levelId",
							Type = "string",
							Description = "标高 ID",
							Required = true
						}
					}
				},
				new SkillInfo
				{
					SkillName = "batch_create_columns",
					DisplayName = "批量创建结构柱",
					Description = "按网格批量创建结构柱",
					Category = "建模",
					Priority = 20,
					Steps = new List<SkillStep>
					{
						new SkillStep
						{
							Order = 1,
							Tool = "create_column",
							Parameters = new Class13<Class12<string, string, int>, string>(new Class12<string, string, int>("{{x}}", "{{y}}", 0), "{{levelId}}"),
							Description = "创建单根柱子"
						}
					},
					Variables = new List<SkillVariable>
					{
						new SkillVariable
						{
							Name = "levelId",
							Type = "string",
							Description = "标高 ID",
							Required = true
						},
						new SkillVariable
						{
							Name = "x",
							Type = "number",
							Description = "X 坐标（毫米）",
							Required = true
						},
						new SkillVariable
						{
							Name = "y",
							Type = "number",
							Description = "Y 坐标（毫米）",
							Required = true
						}
					}
				}
			}
		};
		method_3(val.Skills);
		string_6 = val.Version;
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 1);
		defaultInterpolatedStringHandler.AppendLiteral("[SkillManager] 加载了 ");
		defaultInterpolatedStringHandler.AppendFormatted(val.Skills.Count);
		defaultInterpolatedStringHandler.AppendLiteral(" 个内置技能");
		Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
	}

	private void method_1()
	{
		if (!File.Exists(string_4))
		{
			Logger.Info("[SkillManager] 本地缓存不存在");
			return;
		}
		try
		{
			SkillsConfig val = JsonConvert.DeserializeObject<SkillsConfig>(File.ReadAllText(string_4));
			if (val != null)
			{
				if (File.Exists(string_5))
				{
					File.ReadAllText(string_5);
				}
				if (method_4(val.Version, string_6) > 0)
				{
					method_3(val.Skills);
					string_6 = val.Version;
					Logger.Info("[SkillManager] 从本地缓存加载技能（版本: " + val.Version + "）");
					return;
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 2);
				defaultInterpolatedStringHandler.AppendLiteral("[SkillManager] 本地缓存版本（");
				defaultInterpolatedStringHandler.AppendFormatted(val.Version);
				defaultInterpolatedStringHandler.AppendLiteral("）不更新于当前版本（");
				defaultInterpolatedStringHandler.AppendFormatted(string_6);
				defaultInterpolatedStringHandler.AppendLiteral("）");
				Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
			}
		}
		catch (Exception ex)
		{
			Logger.Warning("[SkillManager] 加载本地缓存失败: " + ex.Message);
		}
	}

	[AsyncStateMachine(typeof(Struct207))]
	public Task<bool> CheckAndUpdateFromCOSAsync(CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct207 stateMachine = default(Struct207);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<bool>.Create();
		stateMachine.skillManager_0 = this;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_2(string string_7, string string_8)
	{
		try
		{
			string directoryName = Path.GetDirectoryName(string_4);
			if (!string.IsNullOrEmpty(directoryName) && !Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			File.WriteAllText(string_4, string_7);
			File.WriteAllText(string_5, string_8);
			Logger.Debug("[SkillManager] 技能已保存到本地缓存: " + string_4);
		}
		catch (Exception ex)
		{
			Logger.Warning("[SkillManager] 保存本地缓存失败: " + ex.Message);
		}
	}

	private void method_3(List<SkillInfo> list_1)
	{
		lock (object_0)
		{
			foreach (SkillInfo skillInfo_0 in list_1)
			{
				SkillInfo val = list_0.FirstOrDefault((SkillInfo val2) => val2.SkillName == skillInfo_0.SkillName);
				if (val != null)
				{
					list_0.Remove(val);
					Logger.Debug("[SkillManager] 更新技能: " + skillInfo_0.SkillName);
				}
				list_0.Add(skillInfo_0);
			}
			list_0 = list_0.OrderBy((SkillInfo val2) => val2.Priority).ToList();
		}
	}

	private int method_4(string string_7, string string_8)
	{
		string[] array = string_7.Split('.');
		string[] array2 = string_8.Split('.');
		int num = 0;
		int num2;
		int num3;
		while (true)
		{
			if (num < Math.Max(array.Length, array2.Length))
			{
				num2 = ((num < array.Length && int.TryParse(array[num], out var result)) ? result : 0);
				num3 = ((num < array2.Length && int.TryParse(array2[num], out var result2)) ? result2 : 0);
				if (num2 != num3)
				{
					break;
				}
				num++;
				continue;
			}
			return 0;
		}
		return num2.CompareTo(num3);
	}

	public Result<List<SkillInfo>> GetSkillList()
	{
		lock (object_0)
		{
			return Result<List<SkillInfo>>.Success(list_0.ToList());
		}
	}

	public Result<SkillInfo?> GetSkillDetail(string skillName)
	{
		if (string.IsNullOrEmpty(skillName))
		{
			return Result<SkillInfo>.Failure("技能名称不能为空");
		}
		lock (object_0)
		{
			SkillInfo val = list_0.FirstOrDefault((SkillInfo skillInfo_0) => skillInfo_0.SkillName.Equals(skillName, StringComparison.OrdinalIgnoreCase));
			if (val == null)
			{
				return Result<SkillInfo>.Failure("技能不存在: " + skillName);
			}
			return Result<SkillInfo>.Success(val);
		}
	}

	[AsyncStateMachine(typeof(Struct208))]
	public Task<Result<List<AIToolResult>>> ExecuteSkillAsync(string skillName, IDictionary<string, object> variables, AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Struct208 stateMachine = default(Struct208);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<Result<List<AIToolResult>>>.Create();
		stateMachine.skillManager_0 = this;
		stateMachine.string_0 = skillName;
		stateMachine.idictionary_0 = variables;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	[AsyncStateMachine(typeof(Struct209))]
	private Task<AIToolResult?> method_5(SkillStep skillStep_0, IDictionary<string, object> idictionary_0, AIToolContext aitoolContext_0, CancellationToken cancellationToken_0)
	{
		Struct209 stateMachine = default(Struct209);
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.skillManager_0 = this;
		stateMachine.skillStep_0 = skillStep_0;
		stateMachine.idictionary_0 = idictionary_0;
		stateMachine.aitoolContext_0 = aitoolContext_0;
		stateMachine.cancellationToken_0 = cancellationToken_0;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private object? method_6(object? object_1, IDictionary<string, object> idictionary_0)
	{
		if (object_1 == null)
		{
			return null;
		}
		try
		{
			string text = JsonConvert.SerializeObject(object_1);
			foreach (KeyValuePair<string, object> item in idictionary_0)
			{
				string oldValue = "{{" + item.Key + "}}";
				string text2 = JsonConvert.SerializeObject(item.Value);
				if (text2.StartsWith("\"") && text2.EndsWith("\""))
				{
					text2 = text2.Substring(1, text2.Length - 2);
				}
				text = text.Replace(oldValue, text2);
			}
			return JsonConvert.DeserializeObject<object>(text);
		}
		catch (Exception ex)
		{
			Logger.Warning("[SkillManager] 参数解析失败: " + ex.Message);
			return object_1;
		}
	}

	public string GenerateSkillToolDefinition()
	{
		lock (object_0)
		{
			string[] gparam_ = list_0.Select((SkillInfo skillInfo_0) => skillInfo_0.SkillName).ToArray();
			var anon = new
			{
				type = "function",
				function = new
				{
					name = "execute_skill",
					description = "执行预定义的工作流程（技能），包含多个工具调用步骤。AI 可完全自主执行，无需用户确认。可用技能：" + string.Join(", ", list_0.Select((SkillInfo skillInfo_0) => skillInfo_0.SkillName + "(" + skillInfo_0.DisplayName + ")")),
					parameters = new Class14<string, Class15<Class16<string, string, string[]>, Class17<string, string>>, string[]>("object", new Class15<Class16<string, string, string[]>, Class17<string, string>>(new Class16<string, string, string[]>("string", "技能名称（如：create_wall_with_opening）", gparam_), new Class17<string, string>("object", "技能参数（变量名和值的键值对，如：{\"levelId\": \"123\", \"x\": 1000}）")), new string[1] { "skillName" })
				}
			};
			return JsonConvert.SerializeObject((object)new[] { anon }, (Formatting)1);
		}
	}

	public string GetCurrentVersion()
	{
		return string_6;
	}

	public int GetSkillCount()
	{
		lock (object_0)
		{
			return list_0.Count;
		}
	}
}
