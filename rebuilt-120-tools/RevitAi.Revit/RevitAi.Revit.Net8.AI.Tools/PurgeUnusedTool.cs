using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Services;
using ns0;
using ns6;

namespace RevitAi.Revit.Net8.AI.Tools;

[AITool("purge_unused", Category = "文档维护", Description = "清理文档中未使用的项目（族、材质、材质资源/外观资源、视图、视图样板、标注样式、组、视图过滤器等）。支持清理 9 种类型。此操作可以减少文件大小并清理未使用的资源。", RequiresTransaction = true, RequiresModification = true)]
public sealed class PurgeUnusedTool : IAITool
{
	private class Class615
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_0;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool bool_1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_2;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool bool_3;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_4;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool bool_5;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_6;

		[CompilerGenerated]
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool bool_7;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_8;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		[CompilerGenerated]
		private bool bool_9;

		public bool All
		{
			[CompilerGenerated]
			get
			{
				return bool_0;
			}
			[CompilerGenerated]
			set
			{
				bool_0 = value;
			}
		}

		public bool Families
		{
			[CompilerGenerated]
			get
			{
				return bool_1;
			}
			[CompilerGenerated]
			set
			{
				bool_1 = value;
			}
		}

		public bool Materials
		{
			[CompilerGenerated]
			get
			{
				return bool_2;
			}
			[CompilerGenerated]
			set
			{
				bool_2 = value;
			}
		}

		public bool MaterialAssets
		{
			[CompilerGenerated]
			get
			{
				return bool_3;
			}
			[CompilerGenerated]
			set
			{
				bool_3 = value;
			}
		}

		public bool Views
		{
			[CompilerGenerated]
			get
			{
				return bool_4;
			}
			[CompilerGenerated]
			set
			{
				bool_4 = value;
			}
		}

		public bool ViewTemplates
		{
			[CompilerGenerated]
			get
			{
				return bool_5;
			}
			[CompilerGenerated]
			set
			{
				bool_5 = value;
			}
		}

		public bool AnnotationStyles
		{
			[CompilerGenerated]
			get
			{
				return bool_6;
			}
			[CompilerGenerated]
			set
			{
				bool_6 = value;
			}
		}

		public bool ModelGroups
		{
			[CompilerGenerated]
			get
			{
				return bool_7;
			}
			[CompilerGenerated]
			set
			{
				bool_7 = value;
			}
		}

		public bool DetailGroups
		{
			[CompilerGenerated]
			get
			{
				return bool_8;
			}
			[CompilerGenerated]
			set
			{
				bool_8 = value;
			}
		}

		public bool ViewFilters
		{
			[CompilerGenerated]
			get
			{
				return bool_9;
			}
			[CompilerGenerated]
			set
			{
				bool_9 = value;
			}
		}

		public Class615()
		{
			All = false;
			Families = false;
			Materials = false;
			MaterialAssets = false;
			Views = false;
			ViewTemplates = false;
			AnnotationStyles = false;
			ModelGroups = false;
			DetailGroups = false;
			ViewFilters = false;
		}

		public void method_0(bool bool_10)
		{
			All = bool_10;
			Families = bool_10;
			Materials = bool_10;
			MaterialAssets = bool_10;
			Views = bool_10;
			ViewTemplates = bool_10;
			AnnotationStyles = bool_10;
			ModelGroups = bool_10;
			DetailGroups = bool_10;
			ViewFilters = bool_10;
		}
	}

	[CompilerGenerated]
	public sealed class Class616 : IAsyncStateMachine
	{
		public int int_0;

		public AsyncTaskMethodBuilder<AIToolResult> asyncTaskMethodBuilder_0;

		public AIToolContext aitoolContext_0;

		public CancellationToken cancellationToken_0;

		public PurgeUnusedTool purgeUnusedTool_0;

		private IModificationService imodificationService_0;

		private Class615 class615_0;

		private List<string> list_0;

		private int int_1;

		private object object_0;

		private string[] string_0;

		private string[] string_1;

		private int int_2;

		private string string_2;

		private string string_3;

		private List<string> list_1;

		private List<string>.Enumerator enumerator_0;

		private string string_4;

		private List<object> list_2;

		private List<object>.Enumerator enumerator_1;

		private object object_1;

		private string string_5;

		private int int_3;

		private Exception exception_0;

		private int int_4;

		private Exception exception_1;

		private int int_5;

		private Exception exception_2;

		private int int_6;

		private Exception exception_3;

		private int int_7;

		private Exception exception_4;

		private int int_8;

		private Exception exception_5;

		private int int_9;

		private Exception exception_6;

		private int int_10;

		private Exception exception_7;

		private int int_11;

		private Exception exception_8;

		private Exception exception_9;

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
					Class616 stateMachine = this;
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
				imodificationService_0 = ((revitAdapter != null) ? revitAdapter.ModificationService : null);
				if (imodificationService_0 == null)
				{
					result = AIToolResult.Fail("无法获取 ModificationService");
				}
				else if (aitoolContext_0.Document == null)
				{
					result = AIToolResult.Fail("文档对象为空");
				}
				else
				{
					class615_0 = new Class615();
					if (aitoolContext_0.HasParameter("types"))
					{
						object_0 = aitoolContext_0.GetParameter<object>("types", (object)null);
						string_0 = object_0 as string[];
						if (string_0 != null)
						{
							string_1 = string_0;
							for (int_2 = 0; int_2 < string_1.Length; int_2++)
							{
								string_2 = string_1[int_2];
								purgeUnusedTool_0.method_0(string_2, class615_0);
								string_2 = null;
							}
							string_1 = null;
						}
						else
						{
							string_3 = object_0 as string;
							if (string_3 != null)
							{
								purgeUnusedTool_0.method_0(string_3, class615_0);
							}
							else
							{
								list_1 = object_0 as List<string>;
								if (list_1 != null)
								{
									enumerator_0 = list_1.GetEnumerator();
									try
									{
										while (enumerator_0.MoveNext())
										{
											string_4 = enumerator_0.Current;
											purgeUnusedTool_0.method_0(string_4, class615_0);
											string_4 = null;
										}
									}
									finally
									{
										if (num < 0)
										{
											((IDisposable)enumerator_0/*cast due to constrained. prefix*/).Dispose();
										}
									}
									enumerator_0 = default(List<string>.Enumerator);
								}
								else
								{
									list_2 = object_0 as List<object>;
									if (list_2 != null)
									{
										enumerator_1 = list_2.GetEnumerator();
										try
										{
											while (enumerator_1.MoveNext())
											{
												object_1 = enumerator_1.Current;
												string_5 = object_1 as string;
												if (string_5 != null)
												{
													purgeUnusedTool_0.method_0(string_5, class615_0);
												}
												string_5 = null;
												object_1 = null;
											}
										}
										finally
										{
											if (num < 0)
											{
												((IDisposable)enumerator_1/*cast due to constrained. prefix*/).Dispose();
											}
										}
										enumerator_1 = default(List<object>.Enumerator);
									}
									list_2 = null;
								}
								list_1 = null;
							}
							string_3 = null;
						}
						object_0 = null;
						string_0 = null;
					}
					else
					{
						class615_0.method_0(bool_10: true);
					}
					list_0 = new List<string>();
					int_1 = 0;
					if (class615_0.Families)
					{
						try
						{
							int_3 = imodificationService_0.PurgeUnusedFamilies(aitoolContext_0.Document);
							int_1 += int_3;
							if (int_3 > 0)
							{
								List<string> list = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(11, 1);
								defaultInterpolatedStringHandler.AppendFormatted(int_3);
								defaultInterpolatedStringHandler.AppendLiteral(" 个未使用的族和族类型");
								list.Add(defaultInterpolatedStringHandler.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_0 = ex;
							list_0.Add("清理族失败: " + exception_0.Message);
						}
					}
					if (class615_0.Materials)
					{
						try
						{
							int_4 = imodificationService_0.PurgeUnusedMaterials(aitoolContext_0.Document);
							int_1 += int_4;
							if (int_4 > 0)
							{
								List<string> list2 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(8, 1);
								defaultInterpolatedStringHandler2.AppendFormatted(int_4);
								defaultInterpolatedStringHandler2.AppendLiteral(" 个未使用的材质");
								list2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_1 = ex;
							list_0.Add("清理材质失败: " + exception_1.Message);
						}
					}
					if (class615_0.MaterialAssets)
					{
						try
						{
							int_5 = imodificationService_0.PurgeUnusedMaterialAssets(aitoolContext_0.Document);
							int_1 += int_5;
							if (int_5 > 0)
							{
								List<string> list3 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(10, 1);
								defaultInterpolatedStringHandler3.AppendFormatted(int_5);
								defaultInterpolatedStringHandler3.AppendLiteral(" 个未使用的材质资源");
								list3.Add(defaultInterpolatedStringHandler3.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_2 = ex;
							list_0.Add("清理材质资源失败: " + exception_2.Message);
						}
					}
					if (class615_0.Views)
					{
						try
						{
							int_6 = imodificationService_0.PurgeUnusedViews(aitoolContext_0.Document);
							int_1 += int_6;
							if (int_6 > 0)
							{
								List<string> list4 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(8, 1);
								defaultInterpolatedStringHandler4.AppendFormatted(int_6);
								defaultInterpolatedStringHandler4.AppendLiteral(" 个未使用的视图");
								list4.Add(defaultInterpolatedStringHandler4.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_3 = ex;
							list_0.Add("清理视图失败: " + exception_3.Message);
						}
					}
					if (class615_0.ViewTemplates)
					{
						try
						{
							int_7 = imodificationService_0.PurgeUnusedViewTemplates(aitoolContext_0.Document);
							int_1 += int_7;
							if (int_7 > 0)
							{
								List<string> list5 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(10, 1);
								defaultInterpolatedStringHandler5.AppendFormatted(int_7);
								defaultInterpolatedStringHandler5.AppendLiteral(" 个未使用的视图样板");
								list5.Add(defaultInterpolatedStringHandler5.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_4 = ex;
							list_0.Add("清理视图样板失败: " + exception_4.Message);
						}
					}
					if (class615_0.AnnotationStyles)
					{
						try
						{
							int_8 = imodificationService_0.PurgeUnusedAnnotationStyles(aitoolContext_0.Document);
							int_1 += int_8;
							if (int_8 > 0)
							{
								List<string> list6 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(10, 1);
								defaultInterpolatedStringHandler6.AppendFormatted(int_8);
								defaultInterpolatedStringHandler6.AppendLiteral(" 个未使用的标注样式");
								list6.Add(defaultInterpolatedStringHandler6.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_5 = ex;
							list_0.Add("清理标注样式失败: " + exception_5.Message);
						}
					}
					if (class615_0.ModelGroups)
					{
						try
						{
							int_9 = imodificationService_0.PurgeUnusedModelGroups(aitoolContext_0.Document);
							int_1 += int_9;
							if (int_9 > 0)
							{
								List<string> list7 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(9, 1);
								defaultInterpolatedStringHandler7.AppendFormatted(int_9);
								defaultInterpolatedStringHandler7.AppendLiteral(" 个未使用的模型组");
								list7.Add(defaultInterpolatedStringHandler7.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_6 = ex;
							list_0.Add("清理模型组失败: " + exception_6.Message);
						}
					}
					if (class615_0.DetailGroups)
					{
						try
						{
							int_10 = imodificationService_0.PurgeUnusedDetailGroups(aitoolContext_0.Document);
							int_1 += int_10;
							if (int_10 > 0)
							{
								List<string> list8 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(9, 1);
								defaultInterpolatedStringHandler8.AppendFormatted(int_10);
								defaultInterpolatedStringHandler8.AppendLiteral(" 个未使用的详图组");
								list8.Add(defaultInterpolatedStringHandler8.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_7 = ex;
							list_0.Add("清理详图组失败: " + exception_7.Message);
						}
					}
					if (class615_0.ViewFilters)
					{
						try
						{
							int_11 = imodificationService_0.PurgeUnusedViewFilters(aitoolContext_0.Document);
							int_1 += int_11;
							if (int_11 > 0)
							{
								List<string> list9 = list_0;
								DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(11, 1);
								defaultInterpolatedStringHandler9.AppendFormatted(int_11);
								defaultInterpolatedStringHandler9.AppendLiteral(" 个未使用的视图过滤器");
								list9.Add(defaultInterpolatedStringHandler9.ToStringAndClear());
							}
						}
						catch (Exception ex)
						{
							exception_8 = ex;
							list_0.Add("清理视图过滤器失败: " + exception_8.Message);
						}
					}
					if (int_1 == 0 && list_0.Count == 0)
					{
						result = AIToolResult.Fail("未选择任何要清理的类型");
					}
					else
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(19, 1);
						defaultInterpolatedStringHandler10.AppendLiteral("✅ 清理完成，共清理 ");
						defaultInterpolatedStringHandler10.AppendFormatted(int_1);
						defaultInterpolatedStringHandler10.AppendLiteral(" 个未使用的项目");
						result = AIToolResult.Ok(defaultInterpolatedStringHandler10.ToStringAndClear(), (object)new Class282<int, List<string>>(int_1, list_0));
					}
				}
			}
			catch (Exception ex)
			{
				exception_9 = ex;
				result = AIToolResult.Fail("清理未使用项失败: " + exception_9.Message);
			}
			int_0 = -2;
			asyncTaskMethodBuilder_0.SetResult(result);
		}

		[DebuggerHidden]
		void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine iasyncStateMachine_0)
		{
		}
	}

	public string Name => "purge_unused";

	public string Category => "文档维护";

	public string Description => "清理文档中未使用的项目（族、材质、视图、视图样板、标注样式、组、视图过滤器等）";

	public string ParametersSchema => "\n    {\n        \"type\": \"object\",\n        \"properties\": {\n            \"types\": {\n                \"type\": \"array\",\n                \"items\": { \"type\": \"string\" },\n                \"description\": \"要清理的类型数组。支持的类型：'families'（族和族类型）、'materials'（材质）、'materialAssets'（材质资源/外观资源）、'views'（视图）、'viewTemplates'（视图样板）、'annotationStyles'（标注样式）、'modelGroups'（模型组）、'detailGroups'（详图组）、'viewFilters'（视图过滤器）。如果不提供，默认清理所有类型。\",\n                \"enum\": [\"families\", \"materials\", \"materialAssets\", \"views\", \"viewTemplates\", \"annotationStyles\", \"modelGroups\", \"detailGroups\", \"viewFilters\"]\n            }\n        },\n        \"required\": []\n    }";

	[AsyncStateMachine(typeof(Class616))]
	[DebuggerStepThrough]
	public Task<AIToolResult> ExecuteAsync(AIToolContext context, CancellationToken cancellationToken = default(CancellationToken))
	{
		Class616 stateMachine = new Class616();
		stateMachine.asyncTaskMethodBuilder_0 = AsyncTaskMethodBuilder<AIToolResult>.Create();
		stateMachine.purgeUnusedTool_0 = this;
		stateMachine.aitoolContext_0 = context;
		stateMachine.cancellationToken_0 = cancellationToken;
		stateMachine.int_0 = -1;
		stateMachine.asyncTaskMethodBuilder_0.Start(ref stateMachine);
		return stateMachine.asyncTaskMethodBuilder_0.Task;
	}

	private void method_0(string string_0, Class615 class615_0)
	{
		string text = string_0.ToLowerInvariant();
		string text2 = text;
		uint num = Class653.smethod_0(text2);
		if (num <= 2723564856u)
		{
			if (num <= 1336735441)
			{
				if (num <= 831392845)
				{
					if (num == 288591337)
					{
						if (!(text2 == "materials"))
						{
							return;
						}
						goto IL_0354;
					}
					if (num == 339569008)
					{
						if (!(text2 == "materialasset"))
						{
							return;
						}
						goto IL_03b8;
					}
					if (num != 831392845 || !(text2 == "modelgroup"))
					{
						return;
					}
				}
				else
				{
					if (num == 904074405)
					{
						if (!(text2 == "families"))
						{
							return;
						}
						goto IL_016f;
					}
					if (num != 1084048794)
					{
						if (num != 1336735441 || !(text2 == "views"))
						{
							return;
						}
						goto IL_0318;
					}
					if (!(text2 == "modelgroups"))
					{
						return;
					}
				}
			}
			else
			{
				if (num > 1605967500)
				{
					if (num != 2034090236)
					{
						if (num != 2503395559u)
						{
							if (num != 2723564856u || !(text2 == "assets"))
							{
								return;
							}
							goto IL_03b8;
						}
						if (!(text2 == "detailgroup"))
						{
							return;
						}
					}
					else if (!(text2 == "detailgroups"))
					{
						return;
					}
					class615_0.DetailGroups = true;
					return;
				}
				if (num == 1377633321)
				{
					if (!(text2 == "family"))
					{
						return;
					}
					goto IL_016f;
				}
				if (num == 1594842107)
				{
					if (!(text2 == "viewfilters"))
					{
						return;
					}
					goto IL_02b1;
				}
				if (num != 1605967500 || !(text2 == "group"))
				{
					return;
				}
			}
			goto IL_02d2;
		}
		if (num <= 3451552970u)
		{
			if (num <= 2882977356u)
			{
				if (num != 2796510346u)
				{
					if (num == 2846059906u)
					{
						if (!(text2 == "viewtemplate"))
						{
							return;
						}
						goto IL_0389;
					}
					if (num != 2882977356u || !(text2 == "filters"))
					{
						return;
					}
				}
				else if (!(text2 == "viewfilter"))
				{
					return;
				}
			}
			else
			{
				if (num == 2943077229u)
				{
					if (!(text2 == "groups"))
					{
						return;
					}
					goto IL_02d2;
				}
				if (num != 3353438327u)
				{
					if (num != 3451552970u || !(text2 == "annotationstyles"))
					{
						return;
					}
					goto IL_0339;
				}
				if (!(text2 == "filter"))
				{
					return;
				}
			}
			goto IL_02b1;
		}
		if (num <= 3685020920u)
		{
			if (num != 3538210912u)
			{
				if (num != 3628863037u)
				{
					if (num != 3685020920u || !(text2 == "view"))
					{
						return;
					}
					goto IL_0318;
				}
				if (!(text2 == "annotationstyle"))
				{
					return;
				}
				goto IL_0339;
			}
			if (!(text2 == "material"))
			{
				return;
			}
			goto IL_0354;
		}
		if (num != 3705905307u)
		{
			if (num != 3752611769u)
			{
				if (num != 4249227875u || !(text2 == "viewtemplates"))
				{
					return;
				}
				goto IL_0389;
			}
			if (!(text2 == "materialassets"))
			{
				return;
			}
		}
		else if (!(text2 == "asset"))
		{
			return;
		}
		goto IL_03b8;
		IL_0389:
		class615_0.ViewTemplates = true;
		return;
		IL_0339:
		class615_0.AnnotationStyles = true;
		return;
		IL_02b1:
		class615_0.ViewFilters = true;
		return;
		IL_03b8:
		class615_0.MaterialAssets = true;
		return;
		IL_0354:
		class615_0.Materials = true;
		return;
		IL_016f:
		class615_0.Families = true;
		return;
		IL_0318:
		class615_0.Views = true;
		return;
		IL_02d2:
		class615_0.ModelGroups = true;
	}
}
