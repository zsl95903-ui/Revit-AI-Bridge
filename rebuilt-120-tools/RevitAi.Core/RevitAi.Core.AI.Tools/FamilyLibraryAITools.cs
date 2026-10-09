using System;
using RevitAi.Abstractions.AI;
using RevitAi.Abstractions.FamilyLibrary;
using RevitAi.Abstractions.Logging;
using ns7;

namespace RevitAi.Core.AI.Tools;

public static class FamilyLibraryAITools
{
	public static void RegisterTools(IAIToolRegistry toolRegistry, IFamilyLibraryService familyLibraryService, IFamilyLoadService? familyLoadService = null)
	{
		if (toolRegistry == null)
		{
			Logger.Warning("[FamilyLibraryAI] 工具注册表为 null，无法注册在线族库 AI 工具");
			return;
		}
		if (familyLibraryService == null)
		{
			Logger.Warning("[FamilyLibraryAI] 族库服务为 null，无法注册在线族库 AI 工具");
			return;
		}
		try
		{
			SearchFamilyLibraryTool searchFamilyLibraryTool = new SearchFamilyLibraryTool(familyLibraryService);
			toolRegistry.RegisterTool((IAITool)(object)searchFamilyLibraryTool);
			if (familyLoadService != null)
			{
				LoadFamilyFromLibraryTool loadFamilyFromLibraryTool = new LoadFamilyFromLibraryTool(familyLibraryService, familyLoadService);
				toolRegistry.RegisterTool((IAITool)(object)loadFamilyFromLibraryTool);
			}
			else
			{
				Logger.Warning("[FamilyLibraryAI] 族加载服务为 null，跳过注册 load_family_from_library 工具");
			}
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibraryAI] 注册在线族库 AI 工具失败", ex);
		}
	}

	public static void RegisterSearchTool(IAIToolRegistry toolRegistry, IFamilyLibraryService familyLibraryService)
	{
		if (toolRegistry == null)
		{
			Logger.Warning("[FamilyLibraryAI] 工具注册表为 null，无法注册搜索工具");
			return;
		}
		if (familyLibraryService == null)
		{
			Logger.Warning("[FamilyLibraryAI] 族库服务为 null，无法注册搜索工具");
			return;
		}
		try
		{
			SearchFamilyLibraryTool searchFamilyLibraryTool = new SearchFamilyLibraryTool(familyLibraryService);
			toolRegistry.RegisterTool((IAITool)(object)searchFamilyLibraryTool);
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibraryAI] 注册搜索工具失败", ex);
		}
	}

	public static void RegisterLoadTool(IAIToolRegistry toolRegistry, IFamilyLibraryService familyLibraryService, IFamilyLoadService familyLoadService)
	{
		if (toolRegistry == null)
		{
			Logger.Warning("[FamilyLibraryAI] 工具注册表为 null，无法注册下载载入工具");
			return;
		}
		if (familyLibraryService == null)
		{
			Logger.Warning("[FamilyLibraryAI] 族库服务为 null，无法注册下载载入工具");
			return;
		}
		if (familyLoadService == null)
		{
			Logger.Warning("[FamilyLibraryAI] 族加载服务为 null，无法注册下载载入工具");
			return;
		}
		try
		{
			LoadFamilyFromLibraryTool loadFamilyFromLibraryTool = new LoadFamilyFromLibraryTool(familyLibraryService, familyLoadService);
			toolRegistry.RegisterTool((IAITool)(object)loadFamilyFromLibraryTool);
		}
		catch (Exception ex)
		{
			Logger.Error("[FamilyLibraryAI] 注册下载载入工具失败", ex);
		}
	}
}
