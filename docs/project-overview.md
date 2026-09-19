# ReVitAI Bridge 开源项目简介

版本：`1.4.0`
状态：核心版已构建并通过发布物核验
发布日期：2026-09-19

## 1. 项目定位

ReVitAI Bridge 是运行在本机、面向 Autodesk Revit 2027 的类型化工具和 API 层。

它与你之前发布的 `Revit-AI-Bridge` Agent 工作流配合使用：

```text
Revit-AI-Bridge Agent
        |
        v
ReVitAI Bridge
        |
        v
Autodesk Revit 2027
```

Agent 生成结构化 JSON 请求，ReVitAI Bridge 在 Revit 主线程执行对应工具，并用回读数据返回结果。

## 2. 解决的问题

- Revit API 必须在 Revit 主线程执行。
- Agent 直接生成 C# 代码难以审计和复现。
- 多文档操作需要明确目标文档。
- 写入操作需要事务、回滚和回读。
- 视觉截图方式会消耗大量上下文。

ReVitAI Bridge 提供稳定、可审计的结构化工具层，避免 Agent 直接拼接 Revit API 代码。

## 3. 核心能力

- 47 个 Revit 工具。
- 当前用户 Named Pipe。
- JSON-line 请求和响应。
- 文档 GUID 检查。
- 读取与写入分离。
- Revit 事务和失败回滚。
- `dryRun` 计划审查。
- `batch` 批量执行。
- `apply_drawing_plan` 分阶段计划。
- 写入后 ElementId 和属性回读。
- 毫米与 Revit 内部英尺单位转换。
- 建筑、结构、房间、注释、视图和导出工具。

## 4. 工具分组

1. 文档与上下文。
2. 标高与轴网。
3. 建筑构件。
4. 结构构件。
5. 房间与边界。
6. 注释与详图。
7. 视图与导出。
8. 计划与批处理。
9. 族、查询与对象管理。

完整目录见 [tool-catalog.json](tool-catalog.json)。

## 5. 调用流程

1. Agent 从 `%LOCALAPPDATA%\ReVitAI\revitai-bridge.json` 读取 Named Pipe 信息。
2. Agent 调用 `tools.list` 获取工具和输入 Schema。
3. Agent 调用 `get_document_snapshot` 获取当前文档上下文。
4. 写入前调用 `dryRun: true`。
5. 确认后调用写入工具或 `apply_drawing_plan`。
6. Bridge 返回 ElementId、状态和回读值。

## 6. 源码结构

```text
src/ReVitAI.Bridge
build
installer
docs
```

主要模块：

- Named Pipe 服务器。
- Revit ExternalEvent 调度器。
- 工具注册与执行器。
- 事务和回滚。
- 单位转换。
- Revit 加载项入口。

## 7. 运行环境

- Windows 10 或 Windows 11。
- Autodesk Revit 2027 x64。
- 从源码构建时需要 .NET 10 x64 SDK。
- Autodesk Revit API 二进制不随项目分发。

## 8. 构建结果

- Release 构建：`0 errors / 0 warnings`。
- 工具数量：`47`。
- 程序集：`ReVitAI.Bridge.dll 1.4.0.0`。
- 命名空间：`ReVitAI.Bridge`。
- 加载项名称：`ReVitAI Bridge`。

## 9. 发布边界

开源仓库包含自研工具层、协议层、桥接层、加载项和安装脚本。

不分发：

- Revit 安装目录中的 API DLL。
- 用户模型、日志、截图和导出文件。
- 本机绝对路径和测试数据。
- 敏感凭据、测试数据或本机私有信息。

## 10. 许可证

自有核心使用 Apache-2.0。第三方依赖审计见 `THIRD_PARTY_NOTICES.md` 和 `docs/third-party-audit.md`。
