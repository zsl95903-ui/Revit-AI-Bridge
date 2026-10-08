using System.Text.Json;
using System.Text.Json.Nodes;
using RevitAi.Engine.Abstractions;
using RevitAi.Engine.Abstractions.Adapters;
using RevitAi.Engine.Core;
using RevitAi.Engine.ToolContracts;
using RevitToolSet.Implementations;

namespace RevitAi.Engine.SmokeTests;

/// <summary>
/// 不依赖 Revit 的冒烟测试：验证引擎原语（注册表/路由/缓存/单位/参数解析）
/// 以及与厂商 TOOL-API.json 的契约一致性对拍。退出码非 0 表示有失败项。
/// </summary>
internal static class Program
{
    private static int _passed;
    private static readonly List<string> Failures = new();

    private static async Task<int> Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var vendorPath = ResolveVendorPath(args);
        var toolIndexPath = ResolveToolIndexPath(args);
        var toolCatalogPath = ResolveToolCatalogPath(args);

        Console.WriteLine("=== Revit AI Engine PoC 冒烟测试 ===");
        Console.WriteLine($"厂商契约样本：{(vendorPath is null ? "(未提供，跳过对拍)" : vendorPath)}");
        Console.WriteLine($"厂商全量目录：{(toolCatalogPath is null ? "(未提供，跳过 120 工具全量对拍)" : toolCatalogPath)}");
        Console.WriteLine($"工具名索引：{(toolIndexPath is null ? "(未提供，跳过 120 工具全量对拍)" : toolIndexPath)}");
        Console.WriteLine();

        TestUnits();
        TestParameterParsing();
        TestArgumentReader();
        TestCache();
        await TestRegistryAndRoutingAsync();
        await TestReadOnlyImplementationsAsync();
        await TestWriteImplementationsAsync();
        await TestParameterImplementationsAsync();
        await TestMaterialImplementationsAsync();
        await TestFamilyElementImplementationsAsync();
        await TestProjectParameterImplementationsAsync();
        await TestModelingImplementationsAsync();
        await TestModelingImplementations2Async();
        await TestElementOpsImplementationsAsync();
        await TestQueryInteractionImplementationsAsync();
        await TestDomainImplementationsAsync();
        await TestDomainImplementations2Async();
        await TestClosingImplementationsAsync();
        TestFullImplementationCoverage(toolIndexPath);
        TestContractCatalog();
        TestVendorParity(vendorPath);
        TestToolSetParity(toolCatalogPath, toolIndexPath);

        Console.WriteLine();
        Console.WriteLine($"通过 {_passed} 项，失败 {Failures.Count} 项。");
        foreach (var failure in Failures)
        {
            Console.WriteLine("  ✗ " + failure);
        }

        return Failures.Count == 0 ? 0 : 1;
    }

    // ---------------------------------------------------------------- 单位

    private static void TestUnits()
    {
        var units = new UnitService();
        Check("304.8 mm == 1 ft", Math.Abs(units.MmToFeet(304.8) - 1.0) < 1e-12);
        Check("1 ft == 304.8 mm", Math.Abs(units.FeetToMm(1.0) - 304.8) < 1e-9);
        Check("3.048 m == 10 ft", Math.Abs(units.MetersToFeet(3.048) - 10.0) < 1e-9);
        Check("10 ft == 3.048 m", Math.Abs(units.FeetToMeters(10.0) - 3.048) < 1e-12);
        Check("Round(1.23456, 3) == 1.235", Math.Abs(units.Round(1.23456, 3) - 1.235) < 1e-12);
    }

    // ------------------------------------------------------- 参数解析 / 兼容层

    private static void TestParameterParsing()
    {
        var parameters = ToolParameterParser.Parse(
            """{"a":1,"b":2.5,"c":"x","d":true,"e":[1,2,3],"f":["p","q"],"g":{"h":1},"n":null}""");
        var context = new AIToolContext { Parameters = parameters };

        Check("JSON 整数归一为 long 后按 int 读取", context.GetParameter("a", 0) == 1);
        Check("double 读取", Math.Abs(context.GetParameter("b", 0.0) - 2.5) < 1e-12);
        Check("string 读取", context.GetParameter("c", string.Empty) == "x");
        Check("bool 读取", context.GetParameter("d", false));
        Check("int[] 读取", context.GetParameter("e", Array.Empty<int>()).SequenceEqual(new[] { 1, 2, 3 }));
        Check("List<string> 读取", context.GetParameter("f", new List<string>()).SequenceEqual(new[] { "p", "q" }));
        Check("嵌套对象读取", context.GetParameter("g", new Dictionary<string, object?>()).ContainsKey("h"));
        Check("缺失键返回默认值", context.GetParameter("zz", 42) == 42);
        Check("null 值返回默认值", context.GetParameter("n", "fallback") == "fallback");
        Check("HasParameter", context.HasParameter("a") && !context.HasParameter("zz"));
    }

    private static void TestArgumentReader()
    {
        var parameters = ToolParameterParser.Parse("""{"elevation":3.5,"levelName":"F1","elementIds":[1,2],"elementId":9}""");

        Check("单值 elevation → 长度 1 的列表", ToolArgumentReader.ReadNumbers(parameters, "elevation").SequenceEqual(new[] { 3.5 }));
        Check("单值 levelName → 长度 1 的列表", ToolArgumentReader.ReadStrings(parameters, "levelName").SequenceEqual(new[] { "F1" }));
        Check("elementIds + elementId 合并", ToolArgumentReader.ReadIntegers(parameters, "elementIds", "elementId").SequenceEqual(new[] { 1, 2, 9 }));

        var arrayParameters = ToolParameterParser.Parse("""{"elevation":[3.0,4.5,6.0]}""");
        Check("数组 elevation → 3 项", ToolArgumentReader.ReadNumbers(arrayParameters, "elevation").Count == 3);

        var cached = new List<Dictionary<string, object?>> { new() { ["id"] = 11L }, new() { ["id"] = 12 }, new() { ["other"] = 1 } };
        Check("从缓存条目抽取 id", ToolArgumentReader.ExtractIds(cached).SequenceEqual(new[] { 11, 12 }));
    }

    // ---------------------------------------------------------------- 缓存

    private static void TestCache()
    {
        var cache = new SessionAIToolDataCache();
        var id = cache.Store("s1", "levels", new List<Dictionary<string, object?>> { new() { ["id"] = 7 } });

        Check("cache_id 形如 cache_N", id.StartsWith("cache_", StringComparison.Ordinal));
        Check("Exists", cache.Exists(id));
        var value = cache.Retrieve<List<Dictionary<string, object?>>>(id);
        Check("Retrieve 类型正确", value is not null && value.Count == 1 && Convert.ToInt32(value[0]["id"]) == 7);
        Check("ValidateCache 有效", cache.ValidateCache(id).IsValid);
        Check("ValidateCache 失效", !cache.ValidateCache("cache_9999").IsValid);
        Check("会话不匹配被识别", !cache.ValidateCacheWithSession(id, "s2").IsValid);
        Check("会话匹配通过", cache.ValidateCacheWithSession(id, "s1").IsValid);

        cache.ClearSession("s1");
        Check("ClearSession 生效", !cache.Exists(id));

        var evictCache = new SessionAIToolDataCache();
        for (var i = 0; i < 101; i++)
        {
            evictCache.Store("s2", "k" + i, new { index = i });
        }

        Check($"超 100 条触发批量淘汰（当前 {evictCache.Count} 条，应为 76）", evictCache.Count == 76);

        var bigCache = new SessionAIToolDataCache();
        var threw = false;
        try
        {
            bigCache.Store("s3", "huge", new string('x', 51 * 1024 * 1024));
        }
        catch (InvalidOperationException)
        {
            threw = true;
        }

        Check("单条超 50MB 抛错", threw);
    }

    // ------------------------------------------------------- 注册表与路由

    private static async Task TestRegistryAndRoutingAsync()
    {
        var cache = new SessionAIToolDataCache();
        var registry = new AIToolRegistry();
        registry.SetRevitAdapter(new FakeAdapter());
        registry.SetUnitService(new UnitService());
        registry.SetDataCache(cache);
        registry.DiscoverAndRegisterToolsFromAssembly(typeof(FakeDirectTool).Assembly);

        Check("反射发现 3 个 fake 工具", registry.GetAllTools().Count == 3);
        Check("ContainsTool", registry.ContainsTool(FakeWriteTool.ToolName));
        Check("GetAttribute 读到事务标志", registry.GetAttribute(FakeWriteTool.ToolName)?.RequiresTransaction == true);

        var toolsJson = registry.GetToolsDefinitionForAI();
        var toolsNode = JsonNode.Parse(toolsJson)!.AsArray();
        Check("tools 数组条目数正确", toolsNode.Count == 3);
        Check("tools 条目为 function 形式",
            toolsNode.All(node => node!["type"]?.GetValue<string>() == "function" && node["function"]?["name"] is not null));

        var host = new FakeToolHost();
        var handler = new AIToolInvocationHandler(registry, host, cache);
        var context = new AIToolContext { RevitAdapter = new FakeAdapter(), UnitService = new UnitService(), DataCache = cache };

        var direct = await handler.HandleToolCallAsync(new ToolCallInfo(FakeDirectTool.ToolName), context);
        Check("只读·不需文档 → 当前线程直接执行", direct.Success && !host.WasUsed);

        var document = await handler.HandleToolCallAsync(new ToolCallInfo(FakeDocumentTool.ToolName), context);
        Check("只读·需文档 → 走宿主且不开事务", document.Success && host.WasUsed && host.LastRequiresTransaction == false);

        var write = await handler.HandleToolCallAsync(new ToolCallInfo(FakeWriteTool.ToolName), context);
        Check("写入工具 → 走宿主且开事务", write.Success && host.LastRequiresTransaction == true);

        var unknownRaised = false;
        try
        {
            await handler.HandleToolCallAsync(new ToolCallInfo("not_registered"), new AIToolContext());
        }
        catch (InvalidOperationException)
        {
            unknownRaised = true;
        }

        Check("未注册工具报错", unknownRaised);

        var unavailableHandler = new AIToolInvocationHandler(registry, new FakeToolHost { IsAvailable = false }, cache);
        var unavailable = await unavailableHandler.HandleToolCallAsync(new ToolCallInfo(FakeWriteTool.ToolName), new AIToolContext());
        Check("宿主不可用 → 返回失败而不是抛错", !unavailable.Success && unavailable.Message.Contains("宿主不可用", StringComparison.Ordinal));

        Check("sessionId 会写入上下文",
            await SessionIdFlowsThroughAsync(registry, cache));
    }

    private static async Task<bool> SessionIdFlowsThroughAsync(AIToolRegistry registry, SessionAIToolDataCache cache)
    {
        var host = new FakeToolHost();
        var handler = new AIToolInvocationHandler(registry, host, cache);
        var context = new AIToolContext();
        var parameters = ToolParameterParser.Parse("""{"sessionId":"s-42"}""");
        await handler.HandleToolCallAsync(new ToolCallInfo(FakeDirectTool.ToolName, parameters), context);
        return context.SessionId == "s-42";
    }

    // ------------------------------------------------ 只读工具实现（批次 1）

    private static async Task TestReadOnlyImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        ReadOnlyImplementations.RegisterAll();
        ReadOnlyImplementations2.RegisterAll();
        ReadOnlyImplementations3.RegisterAll();

        Check($"批次 1 已登记实现 {ToolImplementationRegistry.Count} 个（应为 27）", ToolImplementationRegistry.Count == 27);

        var adapter = new FakeAdapter();
        var cache = new SessionAIToolDataCache();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = cache };

        var grids = await ExecuteImplementationAsync("get_all_grids", adapter, context, """{}""");
        Check("get_all_grids 返回 2 个轴网", grids.Success && grids.Message.Contains("2 个轴网", StringComparison.Ordinal));

        var views = await ExecuteImplementationAsync("get_all_views", adapter, context, """{}""");
        Check("get_all_views 返回 3 个视图", views.Success && views.Message.Contains("3 个视图", StringComparison.Ordinal));

        var active = await ExecuteImplementationAsync("get_active_view", adapter, context, """{}""");
        Check("get_active_view 返回活动视图", active.Success && active.Message.Contains("标高 1", StringComparison.Ordinal));

        var categories = await ExecuteImplementationAsync("get_all_categories", adapter, context, """{}""");
        Check("get_all_categories 返回 2 个类别", categories.Success && categories.Message.Contains("2 个类别", StringComparison.Ordinal));

        var worksets = await ExecuteImplementationAsync("get_all_worksets", adapter, context, """{}""");
        Check("get_all_worksets 返回 1 个工作集", worksets.Success && worksets.Message.Contains("1 个工作集", StringComparison.Ordinal));

        var payload = new List<object?>
        {
            new Dictionary<string, object?> { ["id"] = 7 },
            new Dictionary<string, object?> { ["id"] = 8 },
        };
        var cacheId = cache.Store("s-batch1", "elements", payload);

        var cached = await ExecuteImplementationAsync("get_cache_data", adapter, context, $$"""{"cacheId":"{{cacheId}}"}""");
        Check("get_cache_data 读回 2 条", cached.Success && cached.Message.Contains("读取 2 条", StringComparison.Ordinal));

        var missing = await ExecuteImplementationAsync("get_cache_data", adapter, context, """{"cacheId":"cache_9999"}""");
        Check("get_cache_data 对无效 cacheId 返回失败", !missing.Success);

        var noCacheId = await ExecuteImplementationAsync("get_cache_data", adapter, context, """{}""");
        Check("get_cache_data 缺少 cacheId 返回失败", !noCacheId.Success);

        // ---- 批次 1 第二批 ----
        var families = await ExecuteImplementationAsync("get_all_families", adapter, context, """{}""");
        Check("get_all_families 返回 2 个族", families.Success && families.Message.Contains("2 个族", StringComparison.Ordinal));

        var familyTypes = await ExecuteImplementationAsync("get_family_types", adapter, context, """{"familyName":"M_单扇门"}""");
        Check("get_family_types 按族名过滤到 1 个", familyTypes.Success && familyTypes.Message.Contains("1 个族类型", StringComparison.Ordinal));

        var rooms = await ExecuteImplementationAsync("get_all_rooms", adapter, context, """{}""");
        Check("get_all_rooms 返回 1 个房间", rooms.Success && rooms.Message.Contains("1 个房间", StringComparison.Ordinal));

        var tags = await ExecuteImplementationAsync("get_all_tags", adapter, context, """{}""");
        Check("get_all_tags 返回 1 个标记", tags.Success && tags.Message.Contains("1 个标记", StringComparison.Ordinal));

        var location = await ExecuteImplementationAsync("get_element_location", adapter, context, """{"elementId":8001}""");
        Check("get_element_location 返回坐标", location.Success && location.Message.Contains("100.0", StringComparison.Ordinal));

        var missingLocation = await ExecuteImplementationAsync("get_element_location", adapter, context, """{"elementId":9999}""");
        Check("get_element_location 对不存在元素返回失败", !missingLocation.Success);

        var geometry = await ExecuteImplementationAsync("get_element_geometry", adapter, context, """{"elementId":8001}""");
        Check("get_element_geometry 返回实体/曲线计数", geometry.Success && geometry.Message.Contains("实体 2 个", StringComparison.Ordinal));

        var elements = await ExecuteImplementationAsync("get_all_elements", adapter, context, """{"limit":50}""");
        Check("get_all_elements 返回 2 个元素", elements.Success && elements.Message.Contains("2 个元素", StringComparison.Ordinal));

        var units = await ExecuteImplementationAsync("get_project_units", adapter, context, """{}""");
        Check("get_project_units 返回 mm", units.Success && units.Message.Contains("mm", StringComparison.Ordinal));

        var viewSettings = await ExecuteImplementationAsync("get_view_settings", adapter, context, """{"viewId":2001}""");
        Check("get_view_settings 返回比例", viewSettings.Success && viewSettings.Message.Contains("1:100", StringComparison.Ordinal));

        var viewFilters = await ExecuteImplementationAsync("get_view_filters", adapter, context, """{"viewId":2001}""");
        Check("get_view_filters 返回 1 个过滤器", viewFilters.Success && viewFilters.Message.Contains("1 个过滤器", StringComparison.Ordinal));

        var visibility = await ExecuteImplementationAsync("get_category_visibility", adapter, context, """{"categoryName":"墙"}""");
        Check("get_category_visibility 返回可见", visibility.Success && visibility.Message.Contains("可见", StringComparison.Ordinal));

        var missingCategory = await ExecuteImplementationAsync("get_category_visibility", adapter, context, """{}""");
        Check("get_category_visibility 缺 categoryName 返回失败", !missingCategory.Success);

        var pipeTypes = await ExecuteImplementationAsync("get_pipe_types", adapter, context, """{}""");
        Check("get_pipe_types 返回 1 个类型", pipeTypes.Success && pipeTypes.Message.Contains("1 个管道类型", StringComparison.Ordinal));

        var roofTypes = await ExecuteImplementationAsync("get_roof_types", adapter, context, """{}""");
        Check("get_roof_types 返回 1 个类型", roofTypes.Success && roofTypes.Message.Contains("1 个屋顶类型", StringComparison.Ordinal));

        // ---- 批次 1 第三批 ----
        var phases = await ExecuteImplementationAsync("get_element_phases", adapter, context, """{"elementId":8001}""");
        Check("get_element_phases 返回创建阶段", phases.Success && phases.Message.Contains("现有", StringComparison.Ordinal));

        var workset = await ExecuteImplementationAsync("get_element_workset", adapter, context, """{"elementId":8001}""");
        Check("get_element_workset 返回工作集", workset.Success && workset.Message.Contains("工作集 1", StringComparison.Ordinal));

        var links = await ExecuteImplementationAsync("get_links", adapter, context, """{}""");
        Check("get_links 返回 1 个链接", links.Success && links.Message.Contains("1 个链接", StringComparison.Ordinal));

        var linkElements = await ExecuteImplementationAsync("get_link_elements", adapter, context, """{"linkInstanceId":11001}""");
        Check("get_link_elements 返回 1 个元素", linkElements.Success && linkElements.Message.Contains("1 个元素", StringComparison.Ordinal));

        var missingLink = await ExecuteImplementationAsync("get_link_elements", adapter, context, """{}""");
        Check("get_link_elements 缺 linkInstanceId 返回失败", !missingLink.Success);

        var roof = await ExecuteImplementationAsync("get_roof_info", adapter, context, """{"elementId":12001}""");
        Check("get_roof_info 返回屋顶面积", roof.Success && roof.Message.Contains("128.75", StringComparison.Ordinal));

        var notRoof = await ExecuteImplementationAsync("get_roof_info", adapter, context, """{"elementId":8001}""");
        Check("get_roof_info 对非屋顶返回失败", !notRoof.Success);

        var filterParameters = await ExecuteImplementationAsync("get_available_filter_parameters", adapter, context, """{}""");
        Check("get_available_filter_parameters 返回 2 个参数", filterParameters.Success && filterParameters.Message.Contains("2 个", StringComparison.Ordinal));
    }

    // ------------------------------------------------ 写入工具实现（批次 2）

    private static async Task TestWriteImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        WriteImplementations.RegisterAll();
        WriteImplementations2.RegisterAll();
        WriteImplementations3.RegisterAll();

        Check($"批次 2 已登记实现 {ToolImplementationRegistry.Count} 个（应为 20）", ToolImplementationRegistry.Count == 20);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var detail = await ExecuteImplementationAsync("set_view_detail_level", adapter, context, """{"view_id":2001,"detail_level":"Fine"}""");
        Check("set_view_detail_level 生效", detail.Success && detail.Message.Contains("Fine", StringComparison.Ordinal));

        var style = await ExecuteImplementationAsync("set_view_display_style", adapter, context, """{"view_id":2001,"display_style":"Wireframe"}""");
        Check("set_view_display_style 生效", style.Success && style.Message.Contains("Wireframe", StringComparison.Ordinal));

        var background = await ExecuteImplementationAsync("set_background_color", adapter, context, """{"hexColor":"#102030"}""");
        Check("set_background_color 解析 hex", background.Success && background.Message.Contains("16, 32, 48", StringComparison.Ordinal));

        var temporary = await ExecuteImplementationAsync("temporary_view_control", adapter, context, """{"action":"ISOLATE_ELEMENTS","element_ids":[8001,8002]}""");
        Check("temporary_view_control 隔离 2 个元素", temporary.Success && temporary.Message.Contains("影响 2 个元素", StringComparison.Ordinal));

        var temporaryCategory = await ExecuteImplementationAsync("temporary_view_control", adapter, context, """{"action":"HIDE_ELEMENTS","target_scope":"CATEGORY_NAMES","category_names":["墙"]}""");
        Check("temporary_view_control 对 CATEGORY_NAMES 明确失败", !temporaryCategory.Success);

        var duplicated = await ExecuteImplementationAsync("duplicate_view", adapter, context, """{"viewId":2001,"newViewName":"标高 1 - 副本","duplicateDetail":"withDetailing"}""");
        Check("duplicate_view 返回新视图 id", duplicated.Success && duplicated.Message.Contains("2100", StringComparison.Ordinal));

        var floorPlan = await ExecuteImplementationAsync("create_floor_plan", adapter, context, """{"level_id":101,"view_name":"新平面"}""");
        Check("create_floor_plan 用 level_id 反查标高名", floorPlan.Success && floorPlan.Message.Contains("标高 1", StringComparison.Ordinal));

        var section = await ExecuteImplementationAsync("create_section_view", adapter, context, """{"view_name":"A-A","direction":"north","min_point":{"x":0,"y":0,"z":0},"max_point":{"x":10000,"y":5000,"z":4000}}""");
        Check("create_section_view 返回新视图", section.Success && section.Message.Contains("2202", StringComparison.Ordinal));

        var sheet = await ExecuteImplementationAsync("create_sheet", adapter, context, """{"title":"一层平面","number":"A101"}""");
        Check("create_sheet 返回图纸 id", sheet.Success && sheet.Message.Contains("2203", StringComparison.Ordinal));

        var note = await ExecuteImplementationAsync("create_text_note", adapter, context, """{"viewId":2001,"text":"注意：这里是注释","position":{"x":1000,"y":2000}}""");
        Check("create_text_note 返回注释 id", note.Success && note.Message.Contains("2204", StringComparison.Ordinal));

        var filter = await ExecuteImplementationAsync("set_view_filter", adapter, context, """{"filterName":"可见性过滤器","action":"apply","visible":true,"color":{"red":255,"green":0,"blue":0}}""");
        Check("set_view_filter 应用过滤器", filter.Success && filter.Message.Contains("已执行 apply", StringComparison.Ordinal));

        var deletedFilter = await ExecuteImplementationAsync("delete_view_filter", adapter, context, """{"filterName":"可见性过滤器"}""");
        Check("delete_view_filter 删除成功", deletedFilter.Success);

        var missingFilter = await ExecuteImplementationAsync("delete_view_filter", adapter, context, """{"filterName":"不存在的过滤器"}""");
        Check("delete_view_filter 对不存在的过滤器返回失败", !missingFilter.Success);

        // ---- 批次 2 剩余 ----
        var createdFilter = await ExecuteImplementationAsync("create_view_filter", adapter, context, """{"filterName":"墙类型过滤","categoryNames":["墙"],"rules":[{"parameterName":"类型名称","ruleType":"equals","value":"常规 - 200mm"}]}""");
        Check("create_view_filter 创建过滤器", createdFilter.Success && createdFilter.Message.Contains("9500", StringComparison.Ordinal));

        var tag = await ExecuteImplementationAsync("create_tag", adapter, context, """{"elementId":8001,"position":{"x":1000,"y":2000},"addLeader":false}""");
        Check("create_tag 返回标记 id", tag.Success && tag.Message.Contains("9600", StringComparison.Ordinal));

        var tagMissing = await ExecuteImplementationAsync("create_tag", adapter, context, """{}""");
        Check("create_tag 缺 elementId 返回失败", !tagMissing.Success);

        var schedule = await ExecuteImplementationAsync("create_schedule", adapter, context, """{"category":"墙","scheduleName":"墙明细表","fields":["类型","体积"]}""");
        Check("create_schedule 返回明细表 id", schedule.Success && schedule.Message.Contains("9700", StringComparison.Ordinal));

        var scheduleFields = await ExecuteImplementationAsync("create_schedule", adapter, context, """{"category":"墙","fieldUniqueIds":["abc"]}""");
        Check("create_schedule 对 fieldUniqueIds 明确失败", !scheduleFields.Success);

        var images = await ExecuteImplementationAsync("export_view_images", adapter, context, """{"dataSource":"ids","viewIds":[2001,2002],"width":1600}""");
        Check("export_view_images 导出 2 张", images.Success && images.Message.Contains("2 张视图图片", StringComparison.Ordinal));

        var dwg = await ExecuteImplementationAsync("export_views_as_dwg", adapter, context, """{"dataSource":"ids","viewIds":[2001],"exportSettingName":"ACAD2018"}""");
        Check("export_views_as_dwg 导出 1 个视图", dwg.Success && dwg.Message.Contains("1 个视图", StringComparison.Ordinal));

        var noDataSource = await ExecuteImplementationAsync("export_views_as_dwg", adapter, context, """{}""");
        Check("export_views_as_dwg 缺 dataSource 返回失败", !noDataSource.Success);

        var csv = await ExecuteImplementationAsync("export_schedule_to_csv", adapter, context, """{"scheduleName":"墙明细表"}""");
        Check("export_schedule_to_csv 返回行数", csv.Success && csv.Message.Contains("12 行", StringComparison.Ordinal));

        // ---- 批次 2 收尾 ----
        var spot = await ExecuteImplementationAsync("create_spot_dimension", adapter, context, """{"operator":"elevation","elementId":8001,"point":{"x":1000,"y":2000}}""");
        Check("create_spot_dimension 返回标注 id", spot.Success && spot.Message.Contains("9800", StringComparison.Ordinal));

        var spotMissing = await ExecuteImplementationAsync("create_spot_dimension", adapter, context, """{"operator":"elevation"}""");
        Check("create_spot_dimension 缺 elementId 返回失败", !spotMissing.Success);

        var edited = await ExecuteImplementationAsync("edit_schedule", adapter, context, """{"scheduleId":9700,"addFieldParameterIds":[1001,1002],"removeFieldParameterIds":[1003],"replaceFields":false}""");
        Check("edit_schedule 返回明细表 id", edited.Success && edited.Message.Contains("新增 2 字段", StringComparison.Ordinal));

        var editedMissing = await ExecuteImplementationAsync("edit_schedule", adapter, context, """{}""");
        Check("edit_schedule 缺 scheduleId/scheduleName 返回失败", !editedMissing.Success);

        var xlsxPath = Path.Combine(Path.GetTempPath(), "revitai-engine-poc", "smoke-export.xlsx");
        var excel = await ExecuteImplementationAsync(
            "export_excel",
            adapter,
            context,
            $$"""{"dataSource":"data","sheetName":"冒烟表","filePath":{{System.Text.Json.JsonSerializer.Serialize(xlsxPath)}},"data":[{"id":7,"name":"墙","volume":"1.5"},{"id":8,"name":"楼板","volume":"3.25"}]}""");
        Check("export_excel 返回导出行数", excel.Success && excel.Message.Contains("3 行", StringComparison.Ordinal));
        Check("export_excel 生成的文件是合法 xlsx（zip 内含 sheet1.xml）", File.Exists(xlsxPath) && IsValidXlsx(xlsxPath));

        var excelNoSource = await ExecuteImplementationAsync("export_excel", adapter, context, """{}""");
        Check("export_excel 缺 dataSource 返回失败", !excelNoSource.Success);
    }

    private static bool IsValidXlsx(string path)
    {
        try
        {
            using var archive = System.IO.Compression.ZipFile.OpenRead(path);
            return archive.Entries.Any(entry => entry.FullName == "xl/worksheets/sheet1.xml")
                   && archive.Entries.Any(entry => entry.FullName == "xl/workbook.xml")
                   && archive.Entries.Any(entry => entry.FullName == "[Content_Types].xml");
        }
        catch (Exception)
        {
            return false;
        }
    }

    // ------------------------------------------------ 参数工具实现（批次 3）

    private static async Task TestParameterImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        ParameterImplementations.RegisterAll();

        Check($"批次 3 第一刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 3）", ToolImplementationRegistry.Count == 3);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var query = await ExecuteImplementationAsync("query_parameter_info", adapter, context, """{"operation":"list","elementId":8001}""");
        Check("query_parameter_info 返回 3 个参数", query.Success && query.Message.Contains("已查询 1 个元素", StringComparison.Ordinal));

        var filtered = await ExecuteImplementationAsync("query_parameter_info", adapter, context, """{"operation":"list","elementId":8001,"parameterName":"体积"}""");
        Check("query_parameter_info 按名称过滤", filtered.Success && filtered.Message.Contains("体积", StringComparison.Ordinal));

        var queryMissing = await ExecuteImplementationAsync("query_parameter_info", adapter, context, """{"operation":"list"}""");
        Check("query_parameter_info 缺元素标识返回失败", !queryMissing.Success);

        var setSingle = await ExecuteImplementationAsync("set_parameter_values", adapter, context, """{"elementId":8001,"parameterName":"标高","value":"102"}""");
        Check("set_parameter_values 单元素赋值成功", setSingle.Success && setSingle.Message.Contains("已设置 1", StringComparison.Ordinal));

        var setBulk = await ExecuteImplementationAsync("set_parameter_values", adapter, context, """{"elementParameterValues":[{"elementId":8001,"parameterName":"标高","value":"102"},{"elementId":8001,"parameterName":"只读参数","value":"x"}]}""");
        Check("set_parameter_values 批量部分成功（1/2）", setBulk.Success && setBulk.Message.Contains("1/2", StringComparison.Ordinal));

        var setNothing = await ExecuteImplementationAsync("set_parameter_values", adapter, context, """{"elementParameterValues":[{"elementId":8001,"parameterName":"只读参数","value":"x"}]}""");
        Check("set_parameter_values 全部失败时返回失败", !setNothing.Success);

        var globals = await ExecuteImplementationAsync("manage_global_parameters", adapter, context, """{"operation":"list"}""");
        Check("manage_global_parameters list 返回 1 个", globals.Success && globals.Message.Contains("共 1 个", StringComparison.Ordinal));

        var created = await ExecuteImplementationAsync("manage_global_parameters", adapter, context, """{"operation":"create","parameterName":"层高2","parameterType":"Length","value":"3000"}""");
        Check("manage_global_parameters create 成功", created.Success && created.Message.Contains("层高2", StringComparison.Ordinal));

        var updated = await ExecuteImplementationAsync("manage_global_parameters", adapter, context, """{"operation":"set_value","parameterName":"层高","value":"3500"}""");
        Check("manage_global_parameters set_value 成功", updated.Success && updated.Message.Contains("3500", StringComparison.Ordinal));

        var deleted = await ExecuteImplementationAsync("manage_global_parameters", adapter, context, """{"operation":"delete","parameterName":"层高2"}""");
        Check("manage_global_parameters delete 成功", deleted.Success);

        var unknownOperation = await ExecuteImplementationAsync("manage_global_parameters", adapter, context, """{"operation":"not_an_operation"}""");
        Check("manage_global_parameters 未知操作返回失败", !unknownOperation.Success);
    }

    // ------------------------------------------------ 材料与复合层（批次 3 第二刀）

    private static async Task TestMaterialImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        MaterialImplementations.RegisterAll();

        Check($"批次 3 第二刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 6）", ToolImplementationRegistry.Count == 6);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var list = await ExecuteImplementationAsync("material_query", adapter, context, """{"operation":"list"}""");
        Check("material_query 返回 2 个材料", list.Success && list.Message.Contains("2 个材料", StringComparison.Ordinal));

        var filtered = await ExecuteImplementationAsync("material_query", adapter, context, """{"operation":"list","filter":"玻璃"}""");
        Check("material_query 按名称过滤", filtered.Success && filtered.Message.Contains("1 个材料", StringComparison.Ordinal));

        var single = await ExecuteImplementationAsync("material_query", adapter, context, """{"operation":"get","materialName":"玻璃"}""");
        Check("material_query get 返回颜色与透明度", single.Success && single.Message.Contains("玻璃", StringComparison.Ordinal));

        var created = await ExecuteImplementationAsync("material_manager", adapter, context, """{"operation":"create","materialName":"涂料 - 白色","red":255,"green":255,"blue":255}""");
        Check("material_manager create 成功", created.Success && created.Message.Contains("涂料 - 白色", StringComparison.Ordinal));

        var duplicate = await ExecuteImplementationCatchingAsync("material_manager", adapter, context, """{"operation":"create","materialName":"玻璃"}""");
        Check("material_manager 重名且 force=false 时失败", !duplicate.Success);

        var forced = await ExecuteImplementationAsync("material_manager", adapter, context, """{"operation":"create","materialName":"玻璃","force":true,"transparency":40}""");
        Check("material_manager force=true 复用并更新", forced.Success && forced.Message.Contains("40", StringComparison.Ordinal));

        var updated = await ExecuteImplementationAsync("material_manager", adapter, context, """{"operation":"update","materialName":"玻璃","hexColor":"#112233"}""");
        Check("material_manager update 改颜色", updated.Success && updated.Message.Contains("#112233", StringComparison.Ordinal));

        var deleted = await ExecuteImplementationAsync("material_manager", adapter, context, """{"operation":"delete","materialName":"涂料 - 白色"}""");
        Check("material_manager delete 成功", deleted.Success);

        var unknown = await ExecuteImplementationAsync("material_manager", adapter, context, """{"operation":"nope"}""");
        Check("material_manager 未知操作返回失败", !unknown.Success);

        var layers = await ExecuteImplementationAsync("get_compound_structure_layers", adapter, context, """{"elementTypeId":300}""");
        Check("get_compound_structure_layers 返回 2 层 / 220 mm", layers.Success && layers.Message.Contains("2 层，总厚度 220", StringComparison.Ordinal));

        var layersMissing = await ExecuteImplementationAsync("get_compound_structure_layers", adapter, context, """{}""");
        Check("get_compound_structure_layers 缺类型标识返回失败", !layersMissing.Success);

        var added = await ExecuteImplementationAsync("add_compound_structure_layer", adapter, context, """{"elementTypeId":300,"thicknessMM":50,"function":"insulation","materialName":"玻璃"}""");
        Check("add_compound_structure_layer 后 3 层", added.Success && added.Message.Contains("3 层", StringComparison.Ordinal));

        var modified = await ExecuteImplementationAsync("modify_compound_structure_layer", adapter, context, """{"elementTypeId":300,"layerIndex":0,"thicknessMM":35,"function":"Finish2"}""");
        Check("modify_compound_structure_layer 改厚度/功能", modified.Success && modified.Message.Contains("35 mm", StringComparison.Ordinal));

        var removed = await ExecuteImplementationAsync("delete_compound_structure_layer", adapter, context, """{"elementTypeId":300,"layerIndex":1}""");
        Check("delete_compound_structure_layer 成功", removed.Success);

        var outOfRange = await ExecuteImplementationCatchingAsync("delete_compound_structure_layer", adapter, context, """{"elementTypeId":300,"layerIndex":99}""");
        Check("delete_compound_structure_layer 越界被拦下", !outOfRange.Success);

        var after = await ExecuteImplementationAsync("get_compound_structure_layers", adapter, context, """{"elementTypeId":300}""");
        Check("删除后剩 2 层", after.Success && after.Message.Contains("2 层", StringComparison.Ordinal));
    }

    private static async Task<AIToolResult> ExecuteImplementationCatchingAsync(
        string toolName,
        IRevitAdapter adapter,
        AIToolContext context,
        string parametersJson)
    {
        try
        {
            return await ExecuteImplementationAsync(toolName, adapter, context, parametersJson);
        }
        catch (Exception ex)
        {
            return AIToolResult.Fail(ex.GetBaseException().Message);
        }
    }

    // ------------------------------------------------ 族与元素管理（批次 3 第三刀）

    private static async Task TestFamilyElementImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        FamilyElementImplementations.RegisterAll();

        Check($"批次 3 第三刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 6）", ToolImplementationRegistry.Count == 6);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var duplicated = await ExecuteImplementationAsync("duplicate_family_type", adapter, context, """{"sourceTypeId":300,"newTypeName":"常规 - 300mm"}""");
        Check("duplicate_family_type 返回新类型 id", duplicated.Success && duplicated.Message.Contains("3100", StringComparison.Ordinal));

        var duplicateBad = await ExecuteImplementationCatchingAsync("duplicate_family_type", adapter, context, """{"sourceTypeId":300}""");
        Check("duplicate_family_type 缺 newTypeName 返回失败", !duplicateBad.Success);

        var changed = await ExecuteImplementationAsync("change_element_types", adapter, context, """{"elementIds":[8001,8002],"newTypeId":300}""");
        Check("change_element_types 换 2 个元素", changed.Success && changed.Message.Contains("2 个元素", StringComparison.Ordinal));

        var changedPartial = await ExecuteImplementationAsync("change_element_types", adapter, context, """{"elementIds":[8001,9999],"newTypeId":300}""");
        Check("change_element_types 部分成功给出 1/2", changedPartial.Success && changedPartial.Message.Contains("1/2", StringComparison.Ordinal));

        var changedMissing = await ExecuteImplementationAsync("change_element_types", adapter, context, """{"newTypeId":300}""");
        Check("change_element_types 缺元素标识返回失败", !changedMissing.Success);

        var locked = await ExecuteImplementationAsync("element_lock_manager", adapter, context, """{"operation":"lock","elementIds":[8001,8002]}""");
        Check("element_lock_manager 锁定 2 个", locked.Success && locked.Message.Contains("已锁定 2", StringComparison.Ordinal));

        var lockState = await ExecuteImplementationAsync("element_lock_manager", adapter, context, """{"operation":"get","elementIds":[8001,8002,8003]}""");
        Check("element_lock_manager 查询状态（2 个已锁）", lockState.Success && lockState.Message.Contains("已锁定 2 个", StringComparison.Ordinal));

        var unlocked = await ExecuteImplementationAsync("element_lock_manager", adapter, context, """{"operation":"unlock","elementIds":[8001]}""");
        Check("element_lock_manager 解锁 1 个", unlocked.Success && unlocked.Message.Contains("已解锁 1", StringComparison.Ordinal));

        var lockUnknown = await ExecuteImplementationAsync("element_lock_manager", adapter, context, """{"operation":"nope","elementIds":[8001]}""");
        Check("element_lock_manager 未知操作返回失败", !lockUnknown.Success);

        var group = await ExecuteImplementationAsync("element_group_manager", adapter, context, """{"operation":"create","elementIds":[8001,8002],"groupName":"测试组"}""");
        Check("element_group_manager 建组", group.Success && group.Message.Contains("测试组", StringComparison.Ordinal));

        var groups = await ExecuteImplementationAsync("element_group_manager", adapter, context, """{"operation":"list"}""");
        Check("element_group_manager 列出 1 个组", groups.Success && groups.Message.Contains("共 1 个组", StringComparison.Ordinal));

        var ungrouped = await ExecuteImplementationAsync("element_group_manager", adapter, context, """{"operation":"ungroup","groupId":4000}""");
        Check("element_group_manager 解组", ungrouped.Success && ungrouped.Message.Contains("已解组 1", StringComparison.Ordinal));

        var purge = await ExecuteImplementationAsync("purge_unused", adapter, context, """{"types":["types","materials"]}""");
        Check("purge_unused 返回清理统计", purge.Success && purge.Message.Contains("已清理 3/7", StringComparison.Ordinal));

        var colored = await ExecuteImplementationAsync("set_element_color", adapter, context, """{"action":"set","color":{"red":255,"green":0,"blue":0},"elements":[8001,8002]}""");
        Check("set_element_color 设置 2 个元素", colored.Success && colored.Message.Contains("2 个元素", StringComparison.Ordinal));

        var colorPerElement = await ExecuteImplementationAsync("set_element_color", adapter, context, """{"action":"set","elements":[{"elementId":8001,"color":{"red":0,"green":255,"blue":0}},{"elementId":8002,"color":{"red":0,"green":0,"blue":255}}]}""");
        Check("set_element_color 逐元素颜色", colorPerElement.Success && colorPerElement.Message.Contains("2 个元素", StringComparison.Ordinal));

        var colorBad = await ExecuteImplementationCatchingAsync("set_element_color", adapter, context, """{"action":"set","elements":[8001]}""");
        Check("set_element_color 缺颜色被拦下", !colorBad.Success);

        var colorMissing = await ExecuteImplementationAsync("set_element_color", adapter, context, """{"action":"set","color":{"red":1,"green":2,"blue":3}}""");
        Check("set_element_color 缺 elements 返回失败", !colorMissing.Success);
    }

    // ------------------------------------------------ 共享参数与项目参数（批次 3 第四刀）

    private static async Task TestProjectParameterImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        ProjectParameterImplementations.RegisterAll();

        Check($"批次 3 第四刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 2）", ToolImplementationRegistry.Count == 2);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var status = await ExecuteImplementationAsync("manage_shared_parameters", adapter, context, """{"operation":"status"}""");
        Check("manage_shared_parameters status 可用", status.Success && status.Message.Contains("已加载", StringComparison.Ordinal));

        var sharedList = await ExecuteImplementationAsync("manage_shared_parameters", adapter, context, """{"operation":"list"}""");
        Check("manage_shared_parameters list 返回 1 组 1 定义", sharedList.Success && sharedList.Message.Contains("1 组 / 1 个定义", StringComparison.Ordinal));

        var sharedCreate = await ExecuteImplementationAsync("manage_shared_parameters", adapter, context, """{"operation":"create","parameterName":"房间湿度","parameterType":"Number","groupName":"自定义参数"}""");
        Check("manage_shared_parameters create 成功", sharedCreate.Success && sharedCreate.Message.Contains("房间湿度", StringComparison.Ordinal));

        var sharedDelete = await ExecuteImplementationAsync("manage_shared_parameters", adapter, context, """{"operation":"delete","parameterName":"房间湿度"}""");
        Check("manage_shared_parameters delete 明确说明 API 不支持", !sharedDelete.Success && sharedDelete.Message.Contains("不支持", StringComparison.Ordinal));

        var sharedUnknown = await ExecuteImplementationAsync("manage_shared_parameters", adapter, context, """{"operation":"nope"}""");
        Check("manage_shared_parameters 未知操作返回失败", !sharedUnknown.Success);

        var projectList = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"list"}""");
        Check("manage_project_parameters list 返回 1 个", projectList.Success && projectList.Message.Contains("共 1 个", StringComparison.Ordinal));

        var projectCreateDuplicate = await ExecuteImplementationCatchingAsync("manage_project_parameters", adapter, context, """{"operation":"create","parameterName":"自定义参数","categoryNames":["墙"]}""");
        Check("manage_project_parameters 重复绑定被拦下", !projectCreateDuplicate.Success);

        var projectCreate = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"create","parameterName":"房间湿度","sharedParameterName":"自定义参数","bindingType":"type","categoryNames":["墙","楼板"]}""");
        Check("manage_project_parameters create 绑定 2 个类别", projectCreate.Success && projectCreate.Message.Contains("2 个类别", StringComparison.Ordinal));

        var projectCreateMissingShared = await ExecuteImplementationCatchingAsync("manage_project_parameters", adapter, context, """{"operation":"create","parameterName":"不存在的参数","categoryNames":["墙"]}""");
        Check("manage_project_parameters 缺共享定义时给出明确失败", !projectCreateMissingShared.Success && projectCreateMissingShared.Message.Contains("共享参数文件里找不到定义", StringComparison.Ordinal));

        var addCategories = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"add_categories","parameterName":"自定义参数","categoriesToAdd":["楼板","屋顶"]}""");
        Check("manage_project_parameters add_categories 后 3 个类别", addCategories.Success && addCategories.Message.Contains("3 个类别", StringComparison.Ordinal));

        var removeCategories = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"remove_categories","parameterName":"自定义参数","categoriesToRemove":["墙"]}""");
        Check("manage_project_parameters remove_categories 后 2 个类别", removeCategories.Success && removeCategories.Message.Contains("2 个类别", StringComparison.Ordinal));

        var projectDelete = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"delete","parameterName":"房间湿度"}""");
        Check("manage_project_parameters delete 成功", projectDelete.Success);

        var projectDeleteMissing = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"delete","parameterName":"不存在的参数"}""");
        Check("manage_project_parameters 删除不存在项返回失败", !projectDeleteMissing.Success);

        var projectUnknown = await ExecuteImplementationAsync("manage_project_parameters", adapter, context, """{"operation":"nope"}""");
        Check("manage_project_parameters 未知操作返回失败", !projectUnknown.Success);
    }

    // ------------------------------------------------ 建模原语（批次 4 第一刀）

    private static async Task TestModelingImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        ModelingImplementations.RegisterAll();

        Check($"批次 4 第一刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 8）", ToolImplementationRegistry.Count == 8);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var grids = await ExecuteImplementationAsync("create_grid", adapter, context, """{"grids":[{"name":"1","start_x":0,"start_y":0,"end_x":0,"end_y":8000},{"name":"2","start_x":0,"start_y":0,"end_x":10000,"end_y":0}]}""");
        Check("create_grid 创建 2 条轴网", grids.Success && grids.Message.Contains("2 个轴网", StringComparison.Ordinal));

        var gridBad = await ExecuteImplementationAsync("create_grid", adapter, context, """{"grids":[{"start_x":0,"start_y":0,"end_x":0,"end_y":0}]}""");
        Check("create_grid 起终点重合时失败", !gridBad.Success);

        var room = await ExecuteImplementationAsync("create_room", adapter, context, """{"levelId":101,"x":2000,"y":3000,"roomName":"办公室","roomNumber":"101"}""");
        Check("create_room 成功", room.Success && room.Message.Contains("1 个房间", StringComparison.Ordinal));

        var roomMissing = await ExecuteImplementationAsync("create_room", adapter, context, """{"x":1000,"y":1000}""");
        Check("create_room 缺 levelId 返回失败", !roomMissing.Success);

        var wallSingle = await ExecuteImplementationAsync("create_straight_wall", adapter, context, """{"start_x":0,"start_y":0,"end_x":6000,"end_y":0,"level_id":101,"height":3000}""");
        Check("create_straight_wall 单条创建 1 面墙", wallSingle.Success && wallSingle.Message.Contains("1 个墙", StringComparison.Ordinal));

        var wallBatch = await ExecuteImplementationAsync("create_straight_wall", adapter, context, """{"walls":[{"start_x":0,"start_y":0,"end_x":6000,"end_y":0},{"start_x":6000,"start_y":0,"end_x":6000,"end_y":4000}]}""");
        Check("create_straight_wall 数组创建 2 面墙", wallBatch.Success && wallBatch.Message.Contains("2 个墙", StringComparison.Ordinal));

        var wallZero = await ExecuteImplementationAsync("create_straight_wall", adapter, context, """{"walls":[{"start_x":0,"start_y":0,"end_x":0,"end_y":0}]}""");
        Check("create_straight_wall 零长度墙被拦下", !wallZero.Success);

        var column = await ExecuteImplementationAsync("create_column", adapter, context, """{"position_x":3000,"position_y":3000,"level_id":101}""");
        Check("create_column 创建 1 根柱", column.Success && column.Message.Contains("1 个结构柱", StringComparison.Ordinal));

        var beams = await ExecuteImplementationAsync("create_beam", adapter, context, """{"beams":[{"start_x":0,"start_y":0,"end_x":6000,"end_y":0},{"start_x":0,"start_y":4000,"end_x":6000,"end_y":4000}]}""");
        Check("create_beam 创建 2 根梁", beams.Success && beams.Message.Contains("2 个梁", StringComparison.Ordinal));

        var instanceMissingType = await ExecuteImplementationAsync("create_family_instance", adapter, context, """{"x":1000,"y":1000,"z":0}""");
        Check("create_family_instance 缺 type_id 返回失败", !instanceMissingType.Success);

        var instance = await ExecuteImplementationAsync("create_family_instance", adapter, context, """{"type_id":777,"x":1000,"y":1000,"z":0,"level_id":101}""");
        Check("create_family_instance 创建 1 个实例", instance.Success && instance.Message.Contains("1 个族实例", StringComparison.Ordinal));

        var doorMissing = await ExecuteImplementationAsync("create_door_in_wall", adapter, context, """{"position_x":1000,"position_y":0}""");
        Check("create_door_in_wall 缺 wall_id 返回失败", !doorMissing.Success);

        var door = await ExecuteImplementationAsync("create_door_in_wall", adapter, context, """{"wall_id":8001,"position_x":1500,"position_y":0,"door_type_id":888,"flip":true}""");
        Check("create_door_in_wall 创建 1 个门", door.Success && door.Message.Contains("1 个门", StringComparison.Ordinal));

        var windows = await ExecuteImplementationAsync("create_window_in_wall", adapter, context, """{"windows":[{"wall_id":8001,"position_x":500,"position_y":0,"window_type_id":999},{"wall_id":8001,"position_x":2500,"position_y":0,"window_type_id":999,"height_offset":1200}]}""");
        Check("create_window_in_wall 创建 2 个窗", windows.Success && windows.Message.Contains("2 个窗", StringComparison.Ordinal));
    }

    // ------------------------------------------------ MEP 与形体（批次 4 第二刀）

    private static async Task TestModelingImplementations2Async()
    {
        ToolImplementationRegistry.Clear();
        ModelingImplementations2.RegisterAll();

        Check($"批次 4 第二刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 8）", ToolImplementationRegistry.Count == 8);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var pipe = await ExecuteImplementationAsync("create_pipe", adapter, context, """{"start_x":0,"start_y":0,"start_z":3000,"end_x":6000,"end_y":0,"end_z":3000,"level_id":101,"diameter":100}""");
        Check("create_pipe 创建 1 段管道", pipe.Success && pipe.Message.Contains("1 个管道", StringComparison.Ordinal));

        var pipeZero = await ExecuteImplementationAsync("create_pipe", adapter, context, """{"pipes":[{"start_x":0,"start_y":0,"start_z":3000,"end_x":0,"end_y":0,"end_z":3000}]}""");
        Check("create_pipe 零长度被打回", !pipeZero.Success);

        var ducts = await ExecuteImplementationAsync("create_duct", adapter, context, """{"ducts":[{"start_x":0,"start_y":0,"start_z":3000,"end_x":6000,"end_y":0,"end_z":3000,"width":400,"height":200},{"start_x":6000,"start_y":0,"start_z":3000,"end_x":6000,"end_y":4000,"end_z":3000,"width":400,"height":200}]}""");
        Check("create_duct 创建 2 段风管", ducts.Success && ducts.Message.Contains("2 个风管", StringComparison.Ordinal));

        var tray = await ExecuteImplementationAsync("create_cable_tray", adapter, context, """{"start_x":0,"start_y":0,"start_z":2800,"end_x":5000,"end_y":0,"end_z":2800,"width":300}""");
        Check("create_cable_tray 创建 1 段桥架", tray.Success && tray.Message.Contains("1 个桥架", StringComparison.Ordinal));

        var floor = await ExecuteImplementationAsync("create_floor_by_profile", adapter, context, """{"points":[{"x":0,"y":0},{"x":6000,"y":0},{"x":6000,"y":4000},{"x":0,"y":4000}],"level_id":101}""");
        Check("create_floor_by_profile 创建 1 块楼板", floor.Success && floor.Message.Contains("1 个楼板", StringComparison.Ordinal));

        var floorBad = await ExecuteImplementationAsync("create_floor_by_profile", adapter, context, """{"points":[{"x":0,"y":0},{"x":6000,"y":0}],"level_id":101}""");
        Check("create_floor_by_profile 点数不足被拦下", !floorBad.Success);

        var roofMissingLevel = await ExecuteImplementationAsync("create_footprint_roof", adapter, context, """{"points":[{"x":0,"y":0},{"x":6000,"y":0},{"x":6000,"y":4000}]}""");
        Check("create_footprint_roof 缺 level_id 返回失败", !roofMissingLevel.Success);

        var roof = await ExecuteImplementationAsync("create_footprint_roof", adapter, context, """{"points":[{"x":0,"y":0},{"x":6000,"y":0},{"x":6000,"y":4000},{"x":0,"y":4000}],"level_id":101}""");
        Check("create_footprint_roof 创建 1 个屋顶", roof.Success && roof.Message.Contains("1 个足迹屋顶", StringComparison.Ordinal));

        var extrusionRoof = await ExecuteImplementationAsync("create_extrusion_roof", adapter, context, """{"points":[[0,0],[6000,0],[3000,2000]],"level_id":101,"extrusion_start":0,"extrusion_end":6000}""");
        Check("create_extrusion_roof 创建拉伸屋顶", extrusionRoof.Success && extrusionRoof.Message.Contains("拉伸屋顶", StringComparison.Ordinal));

        var dimensionMissingView = await ExecuteImplementationAsync("create_dimension_by_elements", adapter, context, """{"elementIds":[8001,8002]}""");
        Check("create_dimension_by_elements 缺 viewId 返回失败", !dimensionMissingView.Success);

        var dimension = await ExecuteImplementationAsync("create_dimension_by_elements", adapter, context, """{"viewId":2001,"elementIds":[8001,8002],"position":{"x":0,"y":-2000}}""");
        Check("create_dimension_by_elements 创建尺寸", dimension.Success && dimension.Message.Contains("尺寸标注", StringComparison.Ordinal));

        var dimensionTooFew = await ExecuteImplementationAsync("create_dimension_by_elements", adapter, context, """{"viewId":2001,"elementIds":[8001]}""");
        Check("create_dimension_by_elements 少于 2 个元素返回失败", !dimensionTooFew.Success);

        var view3d = await ExecuteImplementationAsync("create_3d_view", adapter, context, """{"viewName":"模型三维","elementIds":[8001,8002]}""");
        Check("create_3d_view 创建三维视图", view3d.Success && view3d.Message.Contains("三维视图", StringComparison.Ordinal));
    }

    // ------------------------------------------------ 元素操作（批次 4 第三刀）

    private static async Task TestElementOpsImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        ElementOpsImplementations.RegisterAll();

        Check($"批次 4 第三刀已登记实现 {ToolImplementationRegistry.Count} 个（应为 8）", ToolImplementationRegistry.Count == 8);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var copy = await ExecuteImplementationAsync("copy_element", adapter, context, """{"elementIds":[8001,8002],"x":3000,"y":0,"z":0}""");
        Check("copy_element 复制 2 个元素", copy.Success && copy.Message.Contains("完成 2/2", StringComparison.Ordinal));

        var copyMissing = await ExecuteImplementationAsync("copy_element", adapter, context, """{"x":1000}""");
        Check("copy_element 缺元素标识返回失败", !copyMissing.Success);

        var rotateNoAngle = await ExecuteImplementationAsync("rotate_element", adapter, context, """{"elementIds":[8001]}""");
        Check("rotate_element 缺 angleDegrees 返回失败", !rotateNoAngle.Success);

        var rotate = await ExecuteImplementationAsync("rotate_element", adapter, context, """{"elementIds":[8001],"axisZ":1,"angleDegrees":90,"originX":0,"originY":0,"originZ":0}""");
        Check("rotate_element 旋转 90°", rotate.Success && rotate.Message.Contains("90", StringComparison.Ordinal));

        var rotateZeroAxis = await ExecuteImplementationAsync("rotate_element", adapter, context, """{"elementIds":[8001],"axisX":0,"axisY":0,"axisZ":0,"angleDegrees":90}""");
        Check("rotate_element 零轴向失败", !rotateZeroAxis.Success);

        var mirrorBad = await ExecuteImplementationAsync("mirror_element", adapter, context, """{"elementIds":[8001],"mirrorType":"line","lineStartX":0,"lineStartY":0,"lineEndX":0,"lineEndY":0}""");
        Check("mirror_element 零长度镜像线失败", !mirrorBad.Success);

        var mirror = await ExecuteImplementationAsync("mirror_element", adapter, context, """{"elementIds":[8001],"mirrorType":"line","lineStartX":0,"lineStartY":5000,"lineEndX":6000,"lineEndY":5000}""");
        Check("mirror_element 沿镜像线镜像", mirror.Success && mirror.Message.Contains("完成 1/1", StringComparison.Ordinal));

        var arrayBad = await ExecuteImplementationAsync("array_element", adapter, context, """{"element_id":8001,"count":1}""");
        Check("array_element count<2 返回失败", !arrayBad.Success);

        var array = await ExecuteImplementationAsync("array_element", adapter, context, """{"element_id":8001,"array_type":"linear","count":4,"move_x":3000,"move_y":0,"move_z":0}""");
        Check("array_element 线性阵列 4 份", array.Success && array.Message.Contains("3/3", StringComparison.Ordinal));

        var splitParamMissing = await ExecuteImplementationCatchingAsync("split_element", adapter, context, """{"elementId":8001,"splitMode":"parameter"}""");
        Check("split_element parameter 模式缺参数被拦下", !splitParamMissing.Success);

        var splitPoints = await ExecuteImplementationAsync("split_element", adapter, context, """{"elementId":8001,"splitMode":"points","points":[{"x":3000,"y":0}]}""");
        Check("split_element 按点拆分", splitPoints.Success && splitPoints.Message.Contains("已拆分 1 处", StringComparison.Ordinal));

        var splitBadType = await ExecuteImplementationAsync("split_element", adapter, context, """{"elementId":9999,"splitMode":"point"}""");
        Check("split_element 非曲线构件给出明确失败", !splitBadType.Success);

        var deleted = await ExecuteImplementationAsync("delete_elements", adapter, context, """{"elementIds":[8001,9999]}""");
        Check("delete_elements 删除 1 个（跳过不存在的）", deleted.Success && deleted.Message.Contains("已删除 1 个元素", StringComparison.Ordinal));

        var join = await ExecuteImplementationAsync("join_geometry", adapter, context, """{"element1Ids":[8001],"element2Ids":[8002],"operation":"join"}""");
        Check("join_geometry 连接 1 对", join.Success && join.Message.Contains("1 对", StringComparison.Ordinal));

        var joinMissing = await ExecuteImplementationCatchingAsync("join_geometry", adapter, context, """{"element1Ids":[8001]}""");
        Check("join_geometry 缺第二组被拦下", !joinMissing.Success);

        var cut = await ExecuteImplementationAsync("cut_geometry", adapter, context, """{"elementToCutIds":[8001],"cuttingElementIds":[8002],"operation":"cut"}""");
        Check("cut_geometry 剪切 1 对", cut.Success && cut.Message.Contains("1 对", StringComparison.Ordinal));

        var uncut = await ExecuteImplementationAsync("cut_geometry", adapter, context, """{"elementToCutIds":[8001],"cuttingElementIds":[8002],"operation":"uncut"}""");
        Check("cut_geometry uncut 也走通", uncut.Success);
    }

    // ------------------------------------------------ 查询与交互

    private static async Task TestQueryInteractionImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        QueryInteractionImplementations.RegisterAll();

        Check($"查询与交互已登记实现 {ToolImplementationRegistry.Count} 个（应为 11）", ToolImplementationRegistry.Count == 11);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var query = await ExecuteImplementationAsync("element_query", adapter, context, """{"operation":"byCategory","categoryName":"墙"}""");
        Check("element_query 返回 2 个元素", query.Success && query.Message.Contains("查询到 2 个元素", StringComparison.Ordinal));

        var queryByType = await ExecuteImplementationCatchingAsync("element_query", adapter, context, """{"operation":"byType"}""");
        Check("element_query byType 缺 typeId/typeName 被拦下", !queryByType.Success);

        var filtered = await ExecuteImplementationAsync("filter_elements", adapter, context, """{"categoryName":"墙"}""");
        Check("filter_elements 过滤后 1 个元素", filtered.Success && filtered.Message.Contains("剩 1 个元素", StringComparison.Ordinal));

        var filteredEmpty = await ExecuteImplementationAsync("filter_elements", adapter, context, """{"categoryName":"不存在的类别"}""");
        Check("filter_elements 无匹配返回 0", filteredEmpty.Success && filteredEmpty.Message.Contains("剩 0 个元素", StringComparison.Ordinal));

        var boundaries = await ExecuteImplementationAsync("get_room_boundaries", adapter, context, """{"roomId":8001}""");
        Check("get_room_boundaries 返回 1 条边界环 / 20000mm", boundaries.Success && boundaries.Message.Contains("总周长 20000", StringComparison.Ordinal));

        var boundariesBad = await ExecuteImplementationCatchingAsync("get_room_boundaries", adapter, context, """{"roomId":9999}""");
        Check("get_room_boundaries 非房间被拦下", !boundariesBad.Success);

        var intersection = await ExecuteImplementationAsync("get_grid_intersection", adapter, context, """{"grid1_id":201,"grid2_id":202}""");
        Check("get_grid_intersection 有交点", intersection.Success && intersection.Message.Contains("交点", StringComparison.Ordinal));

        var parallel = await ExecuteImplementationAsync("get_grid_intersection", adapter, context, """{"grid1_id":201,"grid2_id":201}""");
        Check("get_grid_intersection 重合轴网返回失败", !parallel.Success);

        var offsetEast = await ExecuteImplementationAsync("get_offset_point", adapter, context, """{"base_point":{"x":1000,"y":2000,"z":0},"direction":"east","distance":5000}""");
        Check("get_offset_point 向东偏移 5000", offsetEast.Success && offsetEast.Message.Contains("(6000, 2000, 0)", StringComparison.Ordinal));

        var offsetAngle = await ExecuteImplementationAsync("get_offset_point", adapter, context, """{"base_point":{"x":0,"y":0,"z":0},"direction":"north","distance":1000,"angle":90}""");
        // 角度以 +X 轴逆时针为正（与 Revit/数学惯例一致），90° 即向北。
        Check("get_offset_point 角度优先于方向词（90°=正北）", offsetAngle.Success && offsetAngle.Message.Contains("(0, 1000, 0)", StringComparison.Ordinal));

        var offsetBad = await ExecuteImplementationAsync("get_offset_point", adapter, context, """{"base_point":{"x":0,"y":0,"z":0},"direction":"up","distance":1000}""");
        Check("get_offset_point 非法方向返回失败", !offsetBad.Success);

        var fields = await ExecuteImplementationAsync("get_schedule_fields", adapter, context, """{"scheduleName":"墙明细表"}""");
        Check("get_schedule_fields 返回 2 个字段", fields.Success && fields.Message.Contains("2 个字段", StringComparison.Ordinal));

        var fieldsMissing = await ExecuteImplementationAsync("get_schedule_fields", adapter, context, """{}""");
        Check("get_schedule_fields 缺标识返回失败", !fieldsMissing.Success);

        var scheduleData = await ExecuteImplementationAsync("read_schedule_data", adapter, context, """{"scheduleName":"墙明细表","includeHeader":true}""");
        Check("read_schedule_data 返回 2 行", scheduleData.Success && scheduleData.Message.Contains("本次返回 2 行", StringComparison.Ordinal));

        var collision = await ExecuteImplementationAsync("check_collision", adapter, context, """{"source":[8001],"target":[8002]}""");
        Check("check_collision 发现 1 处碰撞", collision.Success && collision.Message.Contains("发现 1 处碰撞", StringComparison.Ordinal));

        var collisionMissing = await ExecuteImplementationAsync("check_collision", adapter, context, """{"target":[8002]}""");
        Check("check_collision 缺 source 返回失败", !collisionMissing.Success);

        var linkDeleted = await ExecuteImplementationAsync("delete_link", adapter, context, """{"linkId":11001}""");
        Check("delete_link 删除链接", linkDeleted.Success && linkDeleted.Message.Contains("已删除链接", StringComparison.Ordinal));

        var linkBadType = await ExecuteImplementationCatchingAsync("delete_link", adapter, context, """{"linkId":8001}""");
        Check("delete_link 非链接元素被拦下", !linkBadType.Success);

        var units = await ExecuteImplementationAsync("set_project_units", adapter, context, """{"length":"mm","area":"m2"}""");
        Check("set_project_units 更新 2 项", units.Success && units.Message.Contains("2 项", StringComparison.Ordinal));

        var unitsMissing = await ExecuteImplementationAsync("set_project_units", adapter, context, """{}""");
        Check("set_project_units 缺参数返回失败", !unitsMissing.Success);

        var visibility = await ExecuteImplementationAsync("set_category_visibility", adapter, context, """{"view_id":2001,"category_name":"墙","visible":false}""");
        Check("set_category_visibility 隐藏类别", visibility.Success && visibility.Message.Contains("已隐藏", StringComparison.Ordinal));

        var visibilityBad = await ExecuteImplementationCatchingAsync("set_category_visibility", adapter, context, """{"view_id":2001,"category_name":"不存在的类别","visible":true}""");
        Check("set_category_visibility 未知类别被拦下", !visibilityBad.Success);
    }

    // ------------------------------------------------ 领域重工程（批次 5 第一批）

    private static async Task TestDomainImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        DomainImplementations.RegisterAll();

        Check($"批次 5 第一批已登记实现 {ToolImplementationRegistry.Count} 个（应为 6）", ToolImplementationRegistry.Count == 6);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var topography = await ExecuteImplementationAsync("create_topography_surface", adapter, context, """{"points":[{"x":0,"y":0,"z":0},{"x":10000,"y":0,"z":500},{"x":10000,"y":8000,"z":1200}]}""");
        Check("create_topography_surface 创建地形（3 点）", topography.Success && topography.Message.Contains("3 个点", StringComparison.Ordinal));

        var topographyTooFew = await ExecuteImplementationAsync("create_topography_surface", adapter, context, """{"points":[{"x":0,"y":0,"z":0},{"x":1000,"y":0,"z":0}]}""");
        Check("create_topography_surface 少于 3 点返回失败", !topographyTooFew.Success);

        var topographyDuplicated = await ExecuteImplementationCatchingAsync("create_topography_surface", adapter, context, """{"points":[[0,0,0],[0,0,0],[0,0,0]]}""");
        Check("create_topography_surface 重合点被拦下", !topographyDuplicated.Success);

        var fromFloorsNoConfirm = await ExecuteImplementationAsync("create_topography_from_floors", adapter, context, """{"topography_element_id":9001,"floor_element_ids":[8001]}""");
        Check("create_topography_from_floors 未确认时拒绝执行", !fromFloorsNoConfirm.Success && fromFloorsNoConfirm.Message.Contains("confirmed=true", StringComparison.Ordinal));

        var fromFloors = await ExecuteImplementationAsync("create_topography_from_floors", adapter, context, """{"topography_element_id":9001,"floor_element_ids":[8001],"confirmed":true}""");
        Check("create_topography_from_floors 追加轮廓点", fromFloors.Success && fromFloors.Message.Contains("本次新增 4 个", StringComparison.Ordinal));

        var fromFloorsMissingTopo = await ExecuteImplementationAsync("create_topography_from_floors", adapter, context, """{"floor_element_ids":[8001],"confirmed":true}""");
        Check("create_topography_from_floors 缺地形 id 返回失败", !fromFloorsMissingTopo.Success);

        var insulationAdd = await ExecuteImplementationAsync("insulation_manager", adapter, context, """{"operation":"add","element_ids":[300],"thickness":50,"material":"岩棉"}""");
        Check("insulation_manager add 成功", insulationAdd.Success && insulationAdd.Message.Contains("1 个元素", StringComparison.Ordinal));

        var insulationAddFail = await ExecuteImplementationAsync("insulation_manager", adapter, context, """{"operation":"add","element_ids":[9999]}""");
        Check("insulation_manager 无变化时返回失败并说明原因", !insulationAddFail.Success && insulationAddFail.Message.Contains("复合层", StringComparison.Ordinal));

        var insulationBadOp = await ExecuteImplementationAsync("insulation_manager", adapter, context, """{"operation":"nope","element_ids":[300]}""");
        Check("insulation_manager 未知操作返回失败", !insulationBadOp.Success);

        var insulationList = await ExecuteImplementationAsync("insulation_query", adapter, context, """{"operation":"list"}""");
        Check("insulation_query 统计保温层 1 条", insulationList.Success && insulationList.Message.Contains("保温层 1 条", StringComparison.Ordinal));

        var insulationUninsulated = await ExecuteImplementationAsync("insulation_query", adapter, context, """{"operation":"uninsulated"}""");
        Check("insulation_query only_uninsulated 时保温层 0 条", insulationUninsulated.Success && insulationUninsulated.Message.Contains("保温层 0 条", StringComparison.Ordinal));

        var systemQuery = await ExecuteImplementationAsync("mep_system_query", adapter, context, """{"operation":"list"}""");
        Check("mep_system_query 返回 1 个系统 / 未保温 1 个", systemQuery.Success && systemQuery.Message.Contains("共 1 个 MEP 系统", StringComparison.Ordinal) && systemQuery.Message.Contains("未保温 1 个", StringComparison.Ordinal));

        var systemUninsulated = await ExecuteImplementationAsync("mep_system_query", adapter, context, """{"operation":"list","only_uninsulated":true}""");
        Check("mep_system_query only_uninsulated 过滤生效", systemUninsulated.Success && systemUninsulated.Message.Contains("共 1 个 MEP 系统", StringComparison.Ordinal));

        var systemCreate = await ExecuteImplementationAsync("mep_system_manager", adapter, context, """{"operation":"create","system_type_id":700,"new_system_name":"新风 1"}""");
        Check("mep_system_manager create 成功", systemCreate.Success && systemCreate.Message.Contains("新风 1", StringComparison.Ordinal));

        var systemCreateBadType = await ExecuteImplementationCatchingAsync("mep_system_manager", adapter, context, """{"operation":"create","system_type_id":9999}""");
        Check("mep_system_manager 非系统类型被拦下", !systemCreateBadType.Success);

        var systemRename = await ExecuteImplementationAsync("mep_system_manager", adapter, context, """{"operation":"rename","system_type_id":9500,"new_name":"家用冷水 A"}""");
        Check("mep_system_manager rename 成功", systemRename.Success && systemRename.Message.Contains("家用冷水 A", StringComparison.Ordinal));

        var systemDeleteBlocked = await ExecuteImplementationCatchingAsync("mep_system_manager", adapter, context, """{"operation":"delete","system_type_id":9500}""");
        Check("mep_system_manager 有成员时删除被拦下", !systemDeleteBlocked.Success && systemDeleteBlocked.Message.Contains("force=true", StringComparison.Ordinal));

        var systemDeleteForced = await ExecuteImplementationAsync("mep_system_manager", adapter, context, """{"operation":"delete","system_type_id":9500,"force":true}""");
        Check("mep_system_manager force=true 删除成功", systemDeleteForced.Success);

        var systemBadOp = await ExecuteImplementationAsync("mep_system_manager", adapter, context, """{"operation":"nope"}""");
        Check("mep_system_manager 未知操作返回失败", !systemBadOp.Success);
    }

    // ------------------------------------------------ 收尾批：道路 / CAD / 剪切顺序 / UI

    private static async Task TestDomainImplementations2Async()
    {
        ToolImplementationRegistry.Clear();
        DomainImplementations2.RegisterAll();

        Check($"收尾批已登记实现 {ToolImplementationRegistry.Count} 个（应为 7）", ToolImplementationRegistry.Count == 7);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var road = await ExecuteImplementationAsync("create_road_from_walls", adapter, context, """{"wall_element_ids":[8001,8002],"default_road_width_meters":6}""");
        Check("create_road_from_walls 生成路面板", road.Success && road.Message.Contains("路面板 9800", StringComparison.Ordinal));

        var roadMissing = await ExecuteImplementationAsync("create_road_from_walls", adapter, context, """{}""");
        Check("create_road_from_walls 缺墙 id 返回失败", !roadMissing.Success);

        var cadList = await ExecuteImplementationAsync("cad_layer_manager", adapter, context, """{"operation":"list","linkId":11001}""");
        Check("cad_layer_manager list 返回 2 个图层", cadList.Success && cadList.Message.Contains("2 个图层", StringComparison.Ordinal));

        var cadHide = await ExecuteImplementationAsync("cad_layer_manager", adapter, context, """{"operation":"hide","linkId":11001,"layerName":"A-WALL"}""");
        Check("cad_layer_manager hide 单个图层", cadHide.Success && cadHide.Message.Contains("涉及 1 个图层", StringComparison.Ordinal));

        var cadBadOp = await ExecuteImplementationCatchingAsync("cad_layer_manager", adapter, context, """{"operation":"nope","linkId":11001}""");
        Check("cad_layer_manager 未知操作被拦下", !cadBadOp.Success);

        var cadBadLink = await ExecuteImplementationCatchingAsync("cad_layer_manager", adapter, context, """{"operation":"list","linkId":9999}""");
        Check("cad_layer_manager 链接不存在被拦下", !cadBadLink.Success);

        var cutDryRun = await ExecuteImplementationAsync("fix_cutting_order", adapter, context, """{"cuttingElementIds":[8001],"elementToCutIds":[8002],"dryRun":true}""");
        Check("fix_cutting_order dryRun 只报告", cutDryRun.Success && cutDryRun.Message.Contains("dryRun", StringComparison.Ordinal));

        var cutFix = await ExecuteImplementationAsync("fix_cutting_order", adapter, context, """{"cuttingElementIds":[8001],"elementToCutIds":[8002]}""");
        Check("fix_cutting_order 修正 1 对", cutFix.Success && cutFix.Message.Contains("修正 1 对", StringComparison.Ordinal));

        var cutMissing = await ExecuteImplementationCatchingAsync("fix_cutting_order", adapter, context, """{"cuttingElementIds":[8001]}""");
        Check("fix_cutting_order 缺第二组被拦下", !cutMissing.Success);

        var select = await ExecuteImplementationAsync("select_elements", adapter, context, """{"elementIds":[8001,8002],"append":false,"zoomToFit":true}""");
        Check("select_elements 选中 2 个并缩放", select.Success && select.Message.Contains("已选择 2 个元素", StringComparison.Ordinal));

        var selectMissing = await ExecuteImplementationAsync("select_elements", adapter, context, """{}""");
        Check("select_elements 缺元素标识返回失败", !selectMissing.Success);

        var clear = await ExecuteImplementationAsync("clear_selection", adapter, context, """{}""");
        Check("clear_selection 清空选择", clear.Success && clear.Message.Contains("已清空选择", StringComparison.Ordinal));

        var zoom = await ExecuteImplementationAsync("zoom_to_elements", adapter, context, """{"elementIds":[8001],"fitFactor":1.2}""");
        Check("zoom_to_elements 缩放到 1 个元素", zoom.Success && zoom.Message.Contains("已缩放到 1 个元素", StringComparison.Ordinal));

        var activate = await ExecuteImplementationAsync("activate_view", adapter, context, """{"viewId":2001}""");
        Check("activate_view 激活视图", activate.Success && activate.Message.Contains("已激活视图", StringComparison.Ordinal));

        var activateMissing = await ExecuteImplementationAsync("activate_view", adapter, context, """{}""");
        Check("activate_view 缺 viewId 返回失败", !activateMissing.Success);

        // 无 UI 会话时（例如后台线程）应给出明确失败而不是崩溃
        adapter.UiDocument = null;
        var noUi = await ExecuteImplementationCatchingAsync("clear_selection", adapter, context, """{}""");
        Check("无 UIDocument 时 UI 工具明确失败", !noUi.Success && noUi.Message.Contains("UIDocument", StringComparison.Ordinal));
        adapter.UiDocument = new object();
    }

    // ------------------------------------------------ 收尾批：楼梯 / 屋顶 / 卫星图 / 代码类

    private static async Task TestClosingImplementationsAsync()
    {
        ToolImplementationRegistry.Clear();
        DomainImplementations3.RegisterAll();
        CodeToolImplementations.RegisterAll();

        Check($"收尾批已登记实现 {ToolImplementationRegistry.Count} 个（应为 6）", ToolImplementationRegistry.Count == 6);

        var adapter = new FakeAdapter();
        var context = new AIToolContext { RevitAdapter = adapter, UnitService = new UnitService(), DataCache = new SessionAIToolDataCache() };

        var stair = await ExecuteImplementationAsync("create_stair", adapter, context, """{"totalHeightMm":3000,"runCount":16,"treadDepthMm":280,"runWidthMm":1200}""");
        Check("create_stair 创建楼梯（16 跑）", stair.Success && stair.Message.Contains("楼梯 9900", StringComparison.Ordinal));

        var stairBad = await ExecuteImplementationAsync("create_stair", adapter, context, """{"totalHeightMm":0,"runCount":0}""");
        Check("create_stair 缺高度/跑数返回失败", !stairBad.Success);

        var stairType = await ExecuteImplementationAsync("create_stair_type", adapter, context, """{"newStairTypeName":"楼梯 - 200mm","runThicknessMm":200}""");
        Check("create_stair_type 创建类型", stairType.Success && stairType.Message.Contains("9902", StringComparison.Ordinal));

        var stairTypeBad = await ExecuteImplementationAsync("create_stair_type", adapter, context, """{}""");
        Check("create_stair_type 缺名称返回失败", !stairTypeBad.Success);

        var roofMissing = await ExecuteImplementationAsync("modify_roof", adapter, context, """{"defines_slope":true}""");
        Check("modify_roof 缺 roof_id 返回失败", !roofMissing.Success);

        var roof = await ExecuteImplementationAsync("modify_roof", adapter, context, """{"roof_id":8001,"defines_slope":true,"slope_angle":30,"apply_to_all_edges":true}""");
        Check("modify_roof 更新 4 条边", roof.Success && roof.Message.Contains("4 条边", StringComparison.Ordinal));

        var roofBad = await ExecuteImplementationCatchingAsync("modify_roof", adapter, context, """{"roof_id":9999,"defines_slope":true}""");
        Check("modify_roof 非足迹屋顶返回失败", !roofBad.Success);

        var satellite = await ExecuteImplementationAsync("import_satellite_map", adapter, context, """{"location_name":"上海","latitude":31.2,"longitude":121.5}""");
        Check("import_satellite_map 无图片时如实说明需要本地栅格", !satellite.Success && satellite.Message.Contains("不提供网络能力", StringComparison.Ordinal));

        var satelliteWithImage = await ExecuteImplementationAsync("import_satellite_map", adapter, context, """{"location_name":"上海","latitude":31.2,"longitude":121.5,"image_path":"C:\\Temp\\map.png"}""");
        Check("import_satellite_map 有本地图片时导入成功", satelliteWithImage.Success && satelliteWithImage.Message.Contains("9950", StringComparison.Ordinal));

        // 代码片段库：改用临时文件，避免污染用户目录
        var snippetPath = Path.Combine(Path.GetTempPath(), "revitai-engine-poc", $"snippets-{Guid.NewGuid():N}.json");
        Environment.SetEnvironmentVariable("REVITAI_SNIPPETS_PATH", snippetPath);

        var snippetList = await ExecuteImplementationAsync("code_snippet", adapter, context, """{"action":"list"}""");
        Check("code_snippet list 初始为空", snippetList.Success && snippetList.Message.Contains("共 0 条", StringComparison.Ordinal));

        var snippetSave = await ExecuteImplementationAsync("code_snippet", adapter, context, """{"action":"save","name":"批量建轴网","description":"一次建多根轴网","code":"{\"steps\":[{\"tool\":\"create_grid\"}]}","tags":["轴网","批量"]}""");
        Check("code_snippet save 成功", snippetSave.Success && snippetSave.Message.Contains("批量建轴网", StringComparison.Ordinal));

        var snippetGet = await ExecuteImplementationAsync("code_snippet", adapter, context, """{"action":"get","name":"批量建轴网"}""");
        Check("code_snippet get 取回内容", snippetGet.Success && snippetGet.Message.Contains("批量建轴网", StringComparison.Ordinal));

        var snippetDelete = await ExecuteImplementationAsync("code_snippet", adapter, context, """{"action":"delete","name":"批量建轴网"}""");
        Check("code_snippet delete 成功", snippetDelete.Success);

        var codeRejected = await ExecuteImplementationAsync("execute_code", adapter, context, """{"code":"using System; class X { static void Main() { System.IO.File.Delete(\"a\"); } }"}""");
        Check("execute_code 拒绝任意代码", !codeRejected.Success && codeRejected.Message.Contains("不执行任意代码", StringComparison.Ordinal));

        var codePlan = await ExecuteImplementationAsync("execute_code", adapter, context, """{"code":"{\"steps\":[{\"tool\":\"create_grid\",\"parameters\":{\"grids\":[{\"start_x\":0,\"start_y\":0,\"end_x\":0,\"end_y\":8000}]}},{\"tool\":\"zoom_to_elements\",\"parameters\":{\"elementIds\":[8001]}}]}"}""");
        Check("execute_code 解析 JSON 计划（2 步）", codePlan.Success && codePlan.Message.Contains("2 步", StringComparison.Ordinal));

        var codeLinePlan = await ExecuteImplementationAsync("execute_code", adapter, context, """{"code":"tool(create_grid, {\"grids\":[]})"}""");
        Check("execute_code 解析行式计划", codeLinePlan.Success && codeLinePlan.Message.Contains("1 步", StringComparison.Ordinal));

        var codeEmpty = await ExecuteImplementationAsync("execute_code", adapter, context, """{"code":"   "}""");
        Check("execute_code 空计划返回失败", !codeEmpty.Success);

        Environment.SetEnvironmentVariable("REVITAI_SNIPPETS_PATH", null);
    }

    // ------------------------------------------------ 全量覆盖验收

    /// <summary>
    /// 断言 tool-index.json 里的每个工具名都有自研实现。
    /// 注：Engine.Revit 侧的 4 个实现不在本测试工程引用范围内（由 Revit 内自检覆盖）。
    /// </summary>
    private static void TestFullImplementationCoverage(string? toolIndexPath)
    {
        // 与 SelfTestCommand 保持一致的注册顺序（Engine.Revit 侧 4 个除外）。
        ToolImplementationRegistry.Clear();
        RevitToolSet.Implementations.ReadOnlyImplementations.RegisterAll();
        RevitToolSet.Implementations.ReadOnlyImplementations2.RegisterAll();
        RevitToolSet.Implementations.ReadOnlyImplementations3.RegisterAll();
        RevitToolSet.Implementations.WriteImplementations.RegisterAll();
        RevitToolSet.Implementations.WriteImplementations2.RegisterAll();
        RevitToolSet.Implementations.WriteImplementations3.RegisterAll();
        RevitToolSet.Implementations.ParameterImplementations.RegisterAll();
        RevitToolSet.Implementations.MaterialImplementations.RegisterAll();
        RevitToolSet.Implementations.FamilyElementImplementations.RegisterAll();
        RevitToolSet.Implementations.ProjectParameterImplementations.RegisterAll();
        RevitToolSet.Implementations.ModelingImplementations.RegisterAll();
        RevitToolSet.Implementations.ModelingImplementations2.RegisterAll();
        RevitToolSet.Implementations.ElementOpsImplementations.RegisterAll();
        RevitToolSet.Implementations.QueryInteractionImplementations.RegisterAll();
        RevitToolSet.Implementations.DomainImplementations.RegisterAll();
        RevitToolSet.Implementations.DomainImplementations2.RegisterAll();
        RevitToolSet.Implementations.DomainImplementations3.RegisterAll();
        RevitToolSet.Implementations.CodeToolImplementations.RegisterAll();

        var registered = ToolImplementationRegistry.ImplementedNames;
        var engineSide = new[] { "get_all_levels", "create_level", "move_elements", "upsert_level" };

        Check($"已登记自研实现 {registered.Count} 个（不小于 117）", registered.Count >= 117);

        if (toolIndexPath is null || !File.Exists(toolIndexPath))
        {
            Check("工具名索引存在（可做全量覆盖断言）", false);
            return;
        }

        using var document = System.Text.Json.JsonDocument.Parse(File.ReadAllText(toolIndexPath));
        var indexNames = document.RootElement
            .EnumerateArray()
            .Select(item => item.GetProperty("name").GetString() ?? string.Empty)
            .Where(name => name.Length > 0)
            .ToList();

        Check($"工具名索引共 {indexNames.Count} 个（应为 121）", indexNames.Count == 121);

        var missing = indexNames
            .Where(name => !registered.Contains(name, StringComparer.OrdinalIgnoreCase))
            .ToList();

        var missingRequired = missing.Where(name => !engineSide.Contains(name, StringComparer.OrdinalIgnoreCase)).ToList();
        var missingEngineSide = missing.Where(name => engineSide.Contains(name, StringComparer.OrdinalIgnoreCase)).ToList();

        Check($"厂商/自研清单全部有自研实现（缺口 {missingRequired.Count} 个）", missingRequired.Count == 0);
        if (missingRequired.Count > 0)
        {
            Console.WriteLine($"     缺口：{string.Join(", ", missingRequired)}");
        }

        Check($"仅 4 个 Engine.Revit 侧实现由 Revit 内自检覆盖（实际 {missingEngineSide.Count} 个）", missingEngineSide.Count <= 4);

        var extras = registered
            .Where(name => !indexNames.Contains(name, StringComparer.OrdinalIgnoreCase))
            .ToList();
        Console.WriteLine($"     附加自研工具（不在厂商清单内）：{(extras.Count == 0 ? "无" : string.Join(", ", extras))}");
    }

    private static Task<AIToolResult> ExecuteImplementationAsync(
        string toolName,
        IRevitAdapter adapter,
        AIToolContext context,
        string parametersJson)
    {
        if (!ToolImplementationRegistry.TryGet(toolName, out var implementation))
        {
            return Task.FromResult(AIToolResult.Fail($"未登记实现：{toolName}"));
        }

        context.Parameters = ToolParameterParser.Parse(parametersJson);
        return implementation.ExecuteAsync(adapter, context);
    }

    // ------------------------------------------------------------ 契约目录

    private static void TestContractCatalog()
    {
        var catalog = ToolContracts.ToolContracts.All;
        Check("契约目录 4 项", catalog.Count == 4);
        Check("工具名唯一", catalog.Select(contract => contract.Name).Distinct(StringComparer.Ordinal).Count() == catalog.Count);

        foreach (var contract in catalog)
        {
            var schemaParses = true;
            try
            {
                using var _ = JsonDocument.Parse(contract.ParametersSchema);
            }
            catch (JsonException)
            {
                schemaParses = false;
            }

            Check($"{contract.Name} 的 schema 是合法 JSON", schemaParses);
        }

        Check("写工具都声明了事务", catalog.Where(c => c.Name is "create_level" or "move_elements" or "upsert_level").All(c => c.RequiresTransaction));
    }

    // ------------------------------------------------- 与厂商契约对拍

    private static void TestVendorParity(string? vendorPath)
    {
        if (vendorPath is null || !File.Exists(vendorPath))
        {
            Console.WriteLine("[跳过] 未找到厂商契约文件，未做对拍。");
            return;
        }

        using var document = JsonDocument.Parse(File.ReadAllText(vendorPath));
        var tools = document.RootElement.GetProperty("tools").EnumerateArray().ToArray();
        Check("厂商契约样本 3 项", tools.Length == 3);

        foreach (var vendorTool in tools)
        {
            var name = vendorTool.GetProperty("name").GetString()!;
            var contract = ToolContracts.ToolContracts.All.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.Ordinal));
            if (contract is null)
            {
                Check($"{name} 存在同名契约", false);
                continue;
            }

            Check($"{name} 分类一致", contract.Category == vendorTool.GetProperty("category").GetString());
            Check($"{name} 描述一致", contract.Description == vendorTool.GetProperty("description").GetString());
            Check($"{name} RequiresTransaction 一致", contract.RequiresTransaction == vendorTool.GetProperty("requiresTransaction").GetBoolean());
            Check($"{name} RequiresModification 一致", contract.RequiresModification == vendorTool.GetProperty("requiresModification").GetBoolean());

            var ours = JsonNode.Parse(contract.ParametersSchema);
            var theirs = JsonNode.Parse(vendorTool.GetProperty("parameters").GetRawText());
            Check($"{name} 参数 schema 语义一致", JsonNode.DeepEquals(ours, theirs));
        }
    }

    // ---------------------------------------------------------------- 工具

    private static string? ResolveToolIndexPath(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] is "--tool-index")
            {
                return Path.GetFullPath(args[i + 1]);
            }
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("REVITTOOLSET_INDEX");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        var candidate = Path.Combine(Path.GetTempPath(), "revitai-engine-poc", "generated", "tool-index.json");
        return File.Exists(candidate) ? candidate : null;
    }

    /// <summary>
    /// RevitToolSet 全量对拍：生成器产出的 121 个工具（厂商 120 + 自研 upsert_level）
    /// 逐个与厂商 TOOL-API.json 比对名称/分类/描述/事务标志/参数 schema。
    /// </summary>
    private static void TestToolSetParity(string? catalogPath, string? toolIndexPath)
    {
        if (catalogPath is null || !File.Exists(catalogPath))
        {
            Console.WriteLine("[跳过] 未提供厂商全量目录（TOOL-API.json），未做 RevitToolSet 全量对拍。");
            return;
        }

        if (toolIndexPath is null || !File.Exists(toolIndexPath))
        {
            Console.WriteLine("[跳过] 未找到工具名索引（先运行 build.ps1 生成）。");
            return;
        }

        using var indexDocument = JsonDocument.Parse(File.ReadAllText(toolIndexPath));
        var indexEntries = indexDocument.RootElement.EnumerateArray().ToArray();
        Check($"RevitToolSet 工具总数 = 121（厂商 120 + 自研 1），实际 {indexEntries.Length}", indexEntries.Length == 121);

        var byName = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
        foreach (var entry in indexEntries)
        {
            byName[entry.GetProperty("name").GetString()!] = entry;
        }

        Check("自研扩展 upsert_level 已包含", byName.ContainsKey("upsert_level"));

        using var vendorDocument = JsonDocument.Parse(File.ReadAllText(catalogPath));
        var mismatches = 0;
        var compared = 0;

        // 厂商全量目录是数组结构；工具名索引同样来自生成器
        foreach (var vendorTool in vendorDocument.RootElement.EnumerateArray())
        {
            var name = vendorTool.GetProperty("name").GetString()!;
            if (!byName.TryGetValue(name, out var entry))
            {
                Failures.Add($"RevitToolSet 缺少厂商工具 {name}");
                mismatches++;
                continue;
            }

            compared++;
            var ok = entry.GetProperty("category").GetString() == vendorTool.GetProperty("category").GetString()
                     && entry.GetProperty("description").GetString() == vendorTool.GetProperty("description").GetString()
                     && entry.GetProperty("requiresTransaction").GetBoolean() == vendorTool.GetProperty("requiresTransaction").GetBoolean()
                     && entry.GetProperty("requiresModification").GetBoolean() == vendorTool.GetProperty("requiresModification").GetBoolean()
                     && JsonNode.DeepEquals(JsonNode.Parse(entry.GetProperty("parameters").GetRawText()), JsonNode.Parse(vendorTool.GetProperty("parameters").GetRawText()));

            if (!ok)
            {
                Failures.Add($"RevitToolSet 工具 {name} 契约与厂商不一致");
                mismatches++;
            }
        }

        Check($"厂商 120 个工具在 RevitToolSet 中逐个匹配（比对 {compared} 个，不一致 {mismatches} 个）", mismatches == 0 && compared == 120);
    }

    /// <summary>模块自带的工具契约目录（120 项；生成器会额外加入 upsert_level）。</summary>
    private static string? ResolveToolCatalogPath(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] is "--tool-catalog")
            {
                return Path.GetFullPath(args[i + 1]);
            }
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("REVITTOOLSET_CATALOG");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        // 从当前目录向上找模块自带的契约目录，避免依赖开发机绝对路径。
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            var candidate = Path.Combine(
                directory.FullName,
                "contracts",
                "tool-contracts.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static string? ResolveVendorPath(string[] args)
    {
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] is "--vendor" or "-v")
            {
                return Path.GetFullPath(args[i + 1]);
            }
        }

        var fromEnvironment = Environment.GetEnvironmentVariable("REVITAI_POC_VENDOR_CONTRACTS");
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        // 构建输出在 %TEMP%，所以默认只能靠参数/环境变量；这里再试着从工作目录向上找一次。
        var directory = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, "contracts", "vendor-tools.json");
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        return null;
    }

    private static void Check(string name, bool condition)
    {
        if (condition)
        {
            _passed++;
            Console.WriteLine("  ✓ " + name);
        }
        else
        {
            Failures.Add(name);
            Console.WriteLine("  ✗ " + name);
        }
    }
}
