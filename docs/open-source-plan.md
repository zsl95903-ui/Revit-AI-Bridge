# ReVitAI Bridge 开源发布方案

状态：`2.1.0` 核心版与 MCP 源码已构建并完成发布物核验。
定位：为之前发布的 `Revit-AI-Bridge` Agent 工作流提供本地、类型化的 Revit 工具和 API 层。
核心程序集：`ReVitAI.Bridge.dll 2.1.0.0`
工具数量：`48`

## 1. 项目关系

`Revit-AI-Bridge` 负责 Agent 工作流和任务编排。
`ReVitAI.Bridge` 负责在 Revit 2027 内执行结构化工具请求，并把结果回传给 Agent。

调用关系：

```text
Revit-AI-Bridge Agent
        |
        v
ReVitAI.Bridge Named Pipe
        |
        v
Revit ExternalEvent
        |
        v
Tool Dispatcher
        |
        v
Revit 2027 API
```

## 2. 发布范围

开源内容包括：

- `ReVitAI.Bridge` 自研 C# 源码。
- 48 个 Revit 工具定义和 JSON Schema。
- Named Pipe 通信层。
- Revit ExternalEvent 和主线程调度层。
- 事务、回读、dry-run、batch 和 drawing plan 执行层。
- Revit 加载项入口。
- 构建、打包和安装脚本。
- 架构、安全、第三方审计和项目说明文档。

不分发：

- Autodesk Revit API 二进制。
- 用户模型、日志、截图和导出文件。
- 未明确许可的第三方二进制。
- 本机路径、凭据或测试数据。

## 3. 核心能力

- 活动文档、单位、标高、轴网、视图和族类型读取。
- 墙、楼板、房间、门窗、柱和梁创建。
- 详图线、注释、尺寸和文字创建。
- 元素查询、几何查询、移动、旋转和删除。
- 视图激活、裁剪、隐藏、颜色覆盖和 PNG 导出。
- 文档保存和另存为。
- `batch` 批量执行。
- `apply_drawing_plan` 分阶段计划执行。
- `dryRun` 回滚和写入后回读。

## 4. 源码结构

```text
src/ReVitAI.Bridge/
  BridgeLog.cs
  BridgeServer.cs
  ExternalEventInvoker.cs
  Models.cs
  ReVitAI.Bridge.csproj
  ReVitAIBatchHost.cs
  ReVitAIBridgeApplication.cs
  ToolDispatcher.Additional.cs
  ToolDispatcher.cs
  Units.cs
```

主要组件：

- `BridgeServer`：Named Pipe、发现文件和请求处理。
- `ExternalEventInvoker`：把请求调度到 Revit 主线程。
- `ToolDispatcher`：工具注册、Schema、事务、回读和错误处理。
- `ReVitAIBatchHost`：工具目录和宿主调用入口。
- `ReVitAIBridgeApplication`：Revit 加载项入口和状态命令。
- `Models`、`Units`：协议类型和单位转换。

## 5. 发布包内容

- `ReVitAI.Bridge.dll`
- `ReVitAI.Bridge.deps.json`
- `ReVitAI.Bridge.addin`
- `install.ps1`
- `README.md`
- `LICENSE`
- `NOTICE`
- `THIRD_PARTY_NOTICES.md`
- `CHANGELOG.md`
- `docs`
- `sbom.cdx.json`

当前加载项未签名。正式公开安装包可由维护者使用代码签名证书完成签名。

## 6. 构建

```powershell
powershell -ExecutionPolicy Bypass -File .\build\build.ps1
powershell -ExecutionPolicy Bypass -File .\build\package.ps1
```

Revit API 路径不是默认目录时：

```powershell
powershell -ExecutionPolicy Bypass -File .\build\build.ps1 `
  -RevitApiDir "D:\Autodesk\Revit 2027"
```

## 7. 安装与联调

1. 关闭 Revit。
2. 解压 `ReVitAI-Bridge-Core-v2.1.0.zip`。
3. 运行 `install.ps1`。
4. 启动 Revit 2027 并打开项目。
5. 从 `%LOCALAPPDATA%\ReVitAI\revitai-bridge.json` 读取 Named Pipe 名称。
6. Agent 先调用 `tools.list` 和 `get_document_snapshot`。
7. 写入前使用 `dryRun: true` 审查计划。

## 8. 验证

- Release 构建：`0 errors / 0 warnings`。
- 工具目录声明数量和实际数量：`48 / 48`。
- 核心程序集文件版本：`2.1.0.0`。
- 发布包仅包含一个程序集：`ReVitAI.Bridge.dll`。
- 源码包不包含 `artifacts`、`.git`、`bin`、`obj` 和本机运行数据。

## 9. 许可

自有核心使用 Apache-2.0。Autodesk Revit API 由用户本地 Revit 安装提供，不随项目分发。第三方审计见 `docs/third-party-audit.md`。

## 10. 发布检查表

- [x] 命名空间统一为 `ReVitAI.Bridge`。
- [x] 工具集合字段统一为 `ReVitAI`。
- [x] 48 个工具目录通过构建核验。
- [x] Release 构建通过。
- [x] 发布包通过程序集和文件边界检查。
- [x] 源码包通过敏感凭据和本机路径扫描。
- [ ] 配置代码签名证书。
