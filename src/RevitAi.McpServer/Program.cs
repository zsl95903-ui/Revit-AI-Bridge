using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

// RevitAi MCP server: exposes the running Revit add-in as MCP tools so a
// manually started Codex session can drive Revit. The add-in publishes its
// named pipe in %LOCALAPPDATA%\ReVitAI\revitai-bridge.json; this process only forwards
// JSON requests and never touches Revit directly.

var bridgePipe = Environment.GetEnvironmentVariable("REVIT_AI_BRIDGE_PIPE");
var serverVersion = "2.1.0-lean";

string? line;
while ((line = await Console.In.ReadLineAsync()) is not null)
{
    if (string.IsNullOrWhiteSpace(line))
    {
        continue;
    }

    JsonObject? request;
    try
    {
        request = JsonNode.Parse(line)?.AsObject();
    }
    catch (Exception ex)
    {
        Log($"bad request json: {ex.Message}");
        continue;
    }

    if (request is null)
    {
        continue;
    }

    var method = request["method"]?.GetValue<string>() ?? string.Empty;
    var id = request["id"];
    Log($"<- {method}");

    switch (method)
    {
        case "initialize":
            Respond(id, new JsonObject
            {
                ["protocolVersion"] = "2024-11-05",
                ["capabilities"] = new JsonObject
                {
                    ["tools"] = new JsonObject()
                },
                ["serverInfo"] = new JsonObject
                {
                    ["name"] = "revit-ai",
                    ["version"] = serverVersion
                }
            });
            break;

        case "notifications/initialized":
        case "notifications/cancelled":
            break;

        case "ping":
            Respond(id, new JsonObject());
            break;

        case "tools/list":
            var toolStatus = await SendAsync(bridgePipe, new JsonObject
            {
                ["type"] = "agent.status"
            });
            Respond(id, new JsonObject { ["tools"] = BuildTools(toolStatus) });
            break;

        case "tools/call":
            await HandleToolCallAsync(request, id, bridgePipe);
            break;

        default:
            if (id is not null)
            {
                RespondError(id, -32601, $"method not found: {method}");
            }

            break;
    }
}

return 0;

static JsonArray BuildTools(string? statusJson = null)
{
    var tools = new JsonArray
    {
        Tool(
            "revit_status",
            "返回当前 Revit 会话状态（连接状态、模型、工具数量、技能数量）。",
            new JsonObject()),
        Tool(
            "revit_list_tools",
            "列出当前 Revit 会话可用的全部工具名称与分类。",
            new JsonObject()),
        Tool(
            "revit_invoke_tool",
            "兼容入口：按名称调用一个 Revit 工具。新调用应优先使用直接工具名。",
            new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["tool"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "工具名称"
                    },
                    ["arguments"] = new JsonObject
                    {
                        ["type"] = "object",
                        ["description"] = "工具参数（键值对）"
                    }
                },
                ["required"] = new JsonArray("tool")
            }),
        Tool(
            "revit_agent_chat",
            "把一段简单自然语言任务交给隐藏 Revit 桥接执行。复杂工程绘图应直接调用具名工具。",
            new JsonObject
            {
                ["properties"] = new JsonObject
                {
                    ["message"] = new JsonObject
                    {
                        ["type"] = "string",
                        ["description"] = "交给 Revit 桥接的任务描述"
                    }
                },
                ["required"] = new JsonArray("message")
            })
    };

    if (string.IsNullOrWhiteSpace(statusJson))
    {
        return tools;
    }

    try
    {
        var status = JsonNode.Parse(statusJson)?["status"]?.AsObject();
        var catalog = status?["tools"]?.AsArray();
        if (catalog is null)
        {
            return tools;
        }

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "revit_status",
            "revit_list_tools",
            "revit_invoke_tool",
            "revit_agent_chat"
        };

        foreach (var item in catalog)
        {
            var name = item?["name"]?.GetValue<string>()?.Trim();
            if (string.IsNullOrWhiteSpace(name)
                || name.StartsWith("revit_", StringComparison.OrdinalIgnoreCase)
                || !seen.Add(name))
            {
                continue;
            }

            var description = item?["description"]?.GetValue<string>() ?? string.Empty;
            var toolSet = item?["toolSet"]?.GetValue<string>() ?? "未分类";
            var schema = item?["inputSchema"]?.DeepClone() as JsonObject
                ?? new JsonObject
                {
                    ["properties"] = new JsonObject(),
                    ["additionalProperties"] = true
                };
            tools.Add(Tool(
                name,
                $"{description} [{toolSet}]",
                schema));
        }
    }
    catch
    {
        // Keep the compatibility wrappers when the live catalog cannot be read.
    }

    return tools;
}

static JsonObject Tool(string name, string description, JsonObject schema)
{
    // JsonNode instances cannot be attached to two parents, so clone the
    // schema fragments before moving them into the tool definition.
    var properties = schema["properties"]?.DeepClone() as JsonObject
        ?? new JsonObject();
    var inputSchema = new JsonObject
    {
        ["type"] = "object",
        ["properties"] = properties
    };
    if (schema["additionalProperties"] is JsonValue additionalProperties)
    {
        inputSchema["additionalProperties"] = additionalProperties.DeepClone();
    }
    if (schema["required"] is JsonArray required && required.Count > 0)
    {
        inputSchema["required"] = required.DeepClone();
    }

    return new JsonObject
    {
        ["name"] = name,
        ["description"] = description,
        ["inputSchema"] = inputSchema
    };
}

static async Task HandleToolCallAsync(
    JsonObject request,
    JsonNode? id,
    string? configuredPipe)
{
    var parameters = request["params"]?.AsObject();
    var name = parameters?["name"]?.GetValue<string>() ?? string.Empty;
    var arguments = parameters?["arguments"]?.AsObject();

    JsonObject bridgeRequest;
    switch (name)
    {
        case "revit_status":
            bridgeRequest = new JsonObject { ["type"] = "agent.status" };
            break;

        case "revit_list_tools":
            var status = await SendAsync(configuredPipe, new JsonObject
            {
                ["type"] = "agent.status"
            });
            Respond(id, ToolResult(SummarizeTools(status), IsError(status)));
            return;

        case "revit_agent_chat":
            bridgeRequest = new JsonObject
            {
                ["type"] = "agent.chat",
                ["message"] = arguments?["message"]?.GetValue<string>() ?? string.Empty
            };
            break;

        default:
            bridgeRequest = new JsonObject
            {
                ["type"] = "invoke",
                ["tool"] = arguments?["tool"]?.GetValue<string>() ?? name,
                ["arguments"] = arguments?["arguments"]?.DeepClone() ?? new JsonObject()
            };
            break;
    }

    var response = await SendAsync(configuredPipe, bridgeRequest);
    Respond(id, ToolResult(response, IsError(response)));
}

static JsonObject ToolResult(string text, bool isError)
{
    return new JsonObject
    {
        ["content"] = new JsonArray
        {
            new JsonObject
            {
                ["type"] = "text",
                ["text"] = text
            }
        },
        ["isError"] = isError
    };
}

static string SummarizeTools(string statusJson)
{
    try
    {
        var status = JsonNode.Parse(statusJson)?["status"]?.AsObject();
        var tools = status?["tools"]?.AsArray();
        if (tools is null)
        {
            return statusJson;
        }

        var names = tools
            .Select(tool => tool?["name"]?.GetValue<string>())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToList();
        var builder = new StringBuilder();
        builder.Append("共 ").Append(names.Count).Append(" 个工具：").AppendLine();
        foreach (var tool in tools)
        {
            builder.Append("- ")
                .Append(tool?["name"]?.GetValue<string>())
                .Append(" [")
                .Append(tool?["toolSet"]?.GetValue<string>() ?? "未分类")
                .Append(']')
                .AppendLine();
        }

        return builder.ToString().TrimEnd();
    }
    catch
    {
        return statusJson;
    }
}

static bool IsError(string responseJson)
{
    try
    {
        var node = JsonNode.Parse(responseJson)?.AsObject();
        return node?["success"]?.GetValue<bool>() == false;
    }
    catch
    {
        return true;
    }
}

static async Task<string> SendAsync(string? configuredPipe, JsonObject request)
{
    var pipeName = ResolvePipeName(configuredPipe);
    if (string.IsNullOrWhiteSpace(pipeName))
    {
        return JsonSerializer.Serialize(new
        {
            success = false,
            error = "未找到 Revit 桥接管道，请确认 Revit 与 Revit AI 面板已打开。"
        });
    }

    try
    {
        await using var pipe = new NamedPipeClientStream(
            ".",
            pipeName,
            PipeDirection.InOut,
            PipeOptions.Asynchronous);
        using var timeout = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        await pipe.ConnectAsync(15000, timeout.Token);
        using var reader = new StreamReader(pipe, Encoding.UTF8, leaveOpen: true);
        await using var writer = new StreamWriter(
            pipe,
            new UTF8Encoding(false),
            leaveOpen: true)
        {
            AutoFlush = true
        };
        await writer.WriteLineAsync(request.ToJsonString());
        var response = await reader.ReadLineAsync(timeout.Token);
        return response ?? JsonSerializer.Serialize(new
        {
            success = false,
            error = "Revit 桥接未返回结果。"
        });
    }
    catch (Exception ex)
    {
        return JsonSerializer.Serialize(new
        {
            success = false,
            error = $"Revit 桥接调用失败: {ex.GetBaseException().Message}"
        });
    }
}

static string? ResolvePipeName(string? configuredPipe)
{
    if (!string.IsNullOrWhiteSpace(configuredPipe))
    {
        return configuredPipe;
    }

    try
    {
        var directory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RevitAi");
        foreach (var fileName in new[] { "revitai-bridge.json", "codex-bridge.json", "bridge.json" })
        {
            var path = Path.Combine(directory, fileName);
            if (!File.Exists(path))
            {
                continue;
            }

            var json = JsonNode.Parse(File.ReadAllText(path))?.AsObject();
            var pipeName = json?["pipeName"]?.GetValue<string>();
            if (!string.IsNullOrWhiteSpace(pipeName))
            {
                return pipeName;
            }
        }

        return null;
    }
    catch (Exception ex)
    {
        Log($"bridge discovery read failed: {ex.Message}");
        return null;
    }
}

static void Respond(JsonNode? id, JsonObject result)
{
    if (id is null)
    {
        return;
    }

    WriteLine(new JsonObject
    {
        ["jsonrpc"] = "2.0",
        ["id"] = id.DeepClone(),
        ["result"] = result
    });
}

static void RespondError(JsonNode id, int code, string message)
{
    WriteLine(new JsonObject
    {
        ["jsonrpc"] = "2.0",
        ["id"] = id.DeepClone(),
        ["error"] = new JsonObject
        {
            ["code"] = code,
            ["message"] = message
        }
    });
}

static void WriteLine(JsonObject payload)
{
    Console.Out.WriteLine(payload.ToJsonString());
    Console.Out.Flush();
}

static void Log(string message)
{
    // stdout is reserved for JSON-RPC; diagnostics go to stderr.
    Console.Error.WriteLine($"[revit-ai-mcp] {message}");
}
