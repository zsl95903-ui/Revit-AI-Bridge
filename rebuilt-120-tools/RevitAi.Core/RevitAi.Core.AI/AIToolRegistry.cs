using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.Adapters;
using RevitAi.Abstractions.Logging;
using RevitAi.Abstractions.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ns7;

namespace RevitAi.Core.AI;

public sealed class AIToolRegistry : IAIToolRegistry
{
	[CompilerGenerated]
	public sealed class Class112
	{
		public string string_0;

		internal bool method_0(IAITool iaitool_0)
		{
			if (!string.IsNullOrEmpty(iaitool_0.Category))
			{
				return iaitool_0.Category.Equals(string_0, StringComparison.OrdinalIgnoreCase);
			}
			return false;
		}
	}

	private static readonly Lazy<AIToolRegistry> lazy_0 = new Lazy<AIToolRegistry>(() => new AIToolRegistry());

	private readonly Dictionary<string, IAITool> dictionary_0 = new Dictionary<string, IAITool>();

	private readonly object object_0 = new object();

	private IRevitAdapter? irevitAdapter_0;

	private Assembly? assembly_0;

	private bool bool_0;

	public static AIToolRegistry Instance => lazy_0.Value;

	private AIToolRegistry()
	{
	}

	public void SetRevitAdapter(IRevitAdapter revitAdapter)
	{
		irevitAdapter_0 = revitAdapter;
	}

	public void SetAdapterAssembly(Assembly? adapterAssembly)
	{
		assembly_0 = adapterAssembly;
	}

	public void EnsureToolsDiscovered()
	{
		if (bool_0)
		{
			return;
		}
		if (assembly_0 == null)
		{
			Logger.Warning("[AIToolRegistry] 适配器程序集未设置，无法发现工具");
			return;
		}
		try
		{
			DiscoverAndRegisterToolsFromAssembly(assembly_0);
			bool_0 = true;
			int count = dictionary_0.Count;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("[AIToolRegistry] ✅ 工具注册完成，共注册 ");
			defaultInterpolatedStringHandler.AppendFormatted(count);
			defaultInterpolatedStringHandler.AppendLiteral(" 个工具");
			Logger.Info(defaultInterpolatedStringHandler.ToStringAndClear());
		}
		catch (Exception ex)
		{
			Logger.Error("[AIToolRegistry] 工具注册失败", ex);
		}
	}

	public void RegisterTool(IAITool tool)
	{
		if (tool == null)
		{
			throw new ArgumentNullException("tool");
		}
		lock (object_0)
		{
			if (dictionary_0.ContainsKey(tool.Name))
			{
				Logger.Warning("[AIToolRegistry] 工具 '" + tool.Name + "' 已存在，将被覆盖");
			}
			dictionary_0[tool.Name] = tool;
		}
	}

	public void UnregisterTool(string toolName)
	{
		lock (object_0)
		{
			if (dictionary_0.Remove(toolName))
			{
				Logger.Info("[AIToolRegistry] 已取消注册工具: " + toolName);
			}
		}
	}

	public IAITool? GetTool(string toolName)
	{
		lock (object_0)
		{
			IAITool value;
			return dictionary_0.TryGetValue(toolName, out value) ? value : null;
		}
	}

	public IEnumerable<IAITool> GetAllTools()
	{
		lock (object_0)
		{
			return dictionary_0.Values.ToList();
		}
	}

	public bool ContainsTool(string toolName)
	{
		lock (object_0)
		{
			return dictionary_0.ContainsKey(toolName);
		}
	}

	public string GetToolsDefinitionForAI()
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Expected O, but got Unknown
		lock (object_0)
		{
			if (dictionary_0.Count == 0)
			{
				Logger.Warning("[AIToolRegistry] ⚠️ 没有已注册的工具，返回空工具定义");
				return "[]";
			}
			return JsonConvert.SerializeObject((object)dictionary_0.Values.Select(delegate(IAITool iaitool_0)
			{
				JObject parameters = JObject.Parse(iaitool_0.ParametersSchema);
				return new
				{
					type = "function",
					function = new
					{
						name = iaitool_0.Name,
						description = iaitool_0.Description,
						parameters = parameters
					}
				};
			}), new JsonSerializerSettings
			{
				Formatting = (Formatting)0,
				NullValueHandling = (NullValueHandling)1
			});
		}
	}

	public void DiscoverAndRegisterToolsFromAssembly(Assembly assembly)
	{
		if (assembly == null)
		{
			throw new ArgumentNullException("assembly");
		}
		List<Type> list = (from type_0 in assembly.GetTypes()
			where type_0.IsClass && !type_0.IsAbstract && typeof(IAITool).IsAssignableFrom(type_0)
			select type_0).ToList();
		int num = 0;
		int num2 = 0;
		foreach (Type item in list)
		{
			try
			{
				if (((MemberInfo)item).GetCustomAttribute<AIToolAttribute>() == null)
				{
					Logger.Warning("[AIToolRegistry] 类型 " + item.Name + " 实现了 IAITool 但未标记 AIToolAttribute");
					continue;
				}
				IAITool val = method_0(item);
				if (val == null)
				{
					Logger.Warning("[AIToolRegistry] 无法创建工具实例: " + item.Name);
					num2++;
				}
				else
				{
					RegisterTool(val);
					num++;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("[AIToolRegistry] 注册工具 " + item.Name + " 失败", ex);
				num2++;
			}
		}
	}

	private IAITool? method_0(Type type_0)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Expected O, but got Unknown
		try
		{
			ConstructorInfo constructor = type_0.GetConstructor(Type.EmptyTypes);
			if (constructor != null)
			{
				return (IAITool)constructor.Invoke(null);
			}
			if (irevitAdapter_0 == null)
			{
				Logger.Warning("[AIToolRegistry] 无法创建 " + type_0.Name + "：需要 RevitAdapter 但未设置");
				return null;
			}
			ConstructorInfo[] constructors = type_0.GetConstructors();
			if (constructors.Length == 0)
			{
				Logger.Warning("[AIToolRegistry] " + type_0.Name + " 没有公共构造函数");
				return null;
			}
			ConstructorInfo constructorInfo = constructors[0];
			ParameterInfo[] parameters = constructorInfo.GetParameters();
			object[] array = new object[parameters.Length];
			int num = 0;
			while (true)
			{
				if (num < parameters.Length)
				{
					object obj = method_1(parameters[num].ParameterType);
					if (obj == null)
					{
						break;
					}
					array[num] = obj;
					num++;
					continue;
				}
				return (IAITool)constructorInfo.Invoke(array);
			}
			Logger.Warning("[AIToolRegistry] 无法解析服务: " + parameters[num].ParameterType.Name);
			return null;
		}
		catch (Exception ex)
		{
			Logger.Error("[AIToolRegistry] 创建工具实例 " + type_0.Name + " 失败", ex);
			return null;
		}
	}

	private object? method_1(Type type_0)
	{
		if (irevitAdapter_0 == null)
		{
			Logger.Warning("[AIToolRegistry] RevitAdapter 未设置，无法解析服务: " + type_0.Name);
			return null;
		}
		object obj = null;
		if (type_0 == typeof(IElementService))
		{
			obj = irevitAdapter_0.ElementService;
		}
		else if (type_0 == typeof(IDocumentService))
		{
			obj = irevitAdapter_0.DocumentService;
		}
		else if (type_0 == typeof(IParameterService))
		{
			obj = irevitAdapter_0.ParameterService;
		}
		else if (type_0 == typeof(IModificationService))
		{
			obj = irevitAdapter_0.ModificationService;
		}
		else if (type_0 == typeof(ICreationService))
		{
			obj = irevitAdapter_0.CreationService;
		}
		else if (type_0 == typeof(IGeometryService))
		{
			obj = irevitAdapter_0.GeometryService;
		}
		else if (type_0 == typeof(IAnalysisService))
		{
			obj = irevitAdapter_0.AnalysisService;
		}
		else if (type_0 == typeof(IAnnotationService))
		{
			obj = irevitAdapter_0.AnnotationService;
		}
		else if (type_0 == typeof(ISelectionService))
		{
			obj = irevitAdapter_0.SelectionService;
		}
		else if (type_0 == typeof(IViewService))
		{
			obj = irevitAdapter_0.ViewService;
		}
		else if (type_0 == typeof(ILevelService))
		{
			obj = irevitAdapter_0.LevelService;
		}
		else if (type_0 == typeof(ILinkService))
		{
			obj = irevitAdapter_0.LinkService;
		}
		else if (type_0 == typeof(IFamilyService))
		{
			obj = irevitAdapter_0.FamilyService;
		}
		else if (type_0 == typeof(IPhaseService))
		{
			obj = irevitAdapter_0.PhaseService;
		}
		else if (type_0 == typeof(IMaterialService))
		{
			obj = irevitAdapter_0.MaterialService;
		}
		else if (type_0 == typeof(IExcelDataService))
		{
			obj = irevitAdapter_0.GetExcelDataService();
		}
		else if (type_0 == typeof(IFileAttachmentService))
		{
			obj = irevitAdapter_0.GetFileAttachmentService();
		}
		else if (type_0 == typeof(IWallToRoadService))
		{
			obj = irevitAdapter_0.GetWallToRoadService();
		}
		else if (type_0 == typeof(ITopographyService))
		{
			obj = irevitAdapter_0.GetTopographyService();
		}
		else if (type_0 == typeof(IApplicationService))
		{
			obj = irevitAdapter_0.ApplicationService;
		}
		else
		{
			if (!(type_0 == typeof(IRevitAdapter)))
			{
				Logger.Warning("[AIToolRegistry] 未知的服务类型: " + type_0.Name);
				return null;
			}
			obj = irevitAdapter_0;
		}
		if (obj == null)
		{
			Logger.Warning("[AIToolRegistry] 服务 " + type_0.Name + " 尚未初始化（需要在 ExternalCommand 上下文中）");
		}
		return obj;
	}

	public string GetToolsSummaryForAI()
	{
		lock (object_0)
		{
			if (dictionary_0.Count == 0)
			{
				return "[]";
			}
			return JsonConvert.SerializeObject((object)dictionary_0.Values.Select((IAITool iaitool_0) => new
			{
				name = iaitool_0.Name,
				description = iaitool_0.Description
			}), (Formatting)0);
		}
	}

	public string? GetToolDetailForAI(string toolName)
	{
		lock (object_0)
		{
			if (!dictionary_0.TryGetValue(toolName, out IAITool value))
			{
				return null;
			}
			return JsonConvert.SerializeObject((object)new
			{
				type = "function",
				function = new
				{
					name = value.Name,
					description = value.Description,
					parameters = JObject.Parse(value.ParametersSchema)
				}
			}, (Formatting)0);
		}
	}

	public IEnumerable<string> GetToolCategories()
	{
		lock (object_0)
		{
			return (from string_0 in (from iaitool_0 in dictionary_0.Values
					select iaitool_0.Category into string_0
					where !string.IsNullOrEmpty(string_0)
					select string_0).Distinct()
				orderby string_0
				select string_0).ToList();
		}
	}

	public IEnumerable<IAITool> GetToolsByCategory(string category)
	{
		lock (object_0)
		{
			return dictionary_0.Values.Where((IAITool iaitool_0) => !string.IsNullOrEmpty(iaitool_0.Category) && iaitool_0.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
		}
	}

	public string GetCategorizedToolsSummary()
	{
		lock (object_0)
		{
			return JsonConvert.SerializeObject((object)(from iaitool_0 in dictionary_0.Values
				group iaitool_0 by iaitool_0.Category ?? "未分类" into igrouping_0
				orderby igrouping_0.Key
				select igrouping_0).ToDictionary((IGrouping<string, IAITool> igrouping_0) => igrouping_0.Key, (IGrouping<string, IAITool> igrouping_0) => igrouping_0.Select((IAITool iaitool_0) => new
			{
				name = iaitool_0.Name,
				description = iaitool_0.Description
			}).ToArray()), (Formatting)0);
		}
	}
}
