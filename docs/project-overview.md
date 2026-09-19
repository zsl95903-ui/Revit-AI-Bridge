# Revit AI Bridge 开源项目简介

版本：`1.2.5`
状态：内部开发版已核验，准备进行开源拆分
发布日期：2026-09-19

## 1. 一句话简介

Revit AI Bridge 是一个运行在本机、面向 Autodesk Revit 2027 的类型化 AI 自动化桥接工具。它把 Revit API 的读取、建模、查询、计划执行和保存能力封装为 47 个结构化工具，并通过用户级 Named Pipe 提供给 AI Agent、脚本或本地客户端。

## 2. 项目目的

Revit 本身虽然提供完整 API，但 AI Agent 直接操作 Revit 存在几个问题：

- Revit API 必须在 Revit 主线程执行。
- 任意 C# 脚本难以审计、复现和回滚。
- 多模型、多文档并行时容易操作错误文件。
- 大语言模型直接生成 Revit API 代码不稳定。
- 传统视觉方式需要加载大量截图，上下文成本高。

本项目的目的是提供一个可审计的中间层：

1. AI 只提交结构化 JSON。
2. 桥接验证工具名、参数、单位和目标文档。
3. Revit 主线程执行事务。
4. 支持 dry-run、回滚和回读。
5. 返回模型可继续处理的 JSON，不依赖反复截图。

## 3. 项目价值

### 对 AI Agent

- 将不可控的代码生成替换为明确的工具调用。
- 每个工具都有输入 schema 和可回读结果。
- 支持批量计划和依赖阶段。
- 大幅减少截图和视觉 token 消耗。

### 对 Revit 开发者

- 提供现成的 Named Pipe 与 ExternalEvent 架构。
- 工具事务、单位转换和错误回滚已经封装。
- 可直接扩展新的文档、建筑、结构、注释和视图工具。

### 对工程团队

- 可以把重复建模任务脚本化。
- 可以用 JSON 计划记录操作过程。
- 可以在执行前 dry-run，执行后读取 ElementId 验证。
- 不把模型数据发送到云服务，默认本地运行。

## 4. 典型场景

- 从轴网和标高开始创建标准楼层。
- 批量创建柱、梁、楼板、墙和房间。
- 按表格数据生成构件和族实例。
- 调取或检查元素几何、类别和位置。
- 对图纸 PDF 的结构化提取结果执行 Revit 建模计划。
- 让 AI Agent 在模型变更前执行 dry-run。
- 自动导出 3D 校核图或视图图片。

## 5. 核心能力

- 47 个高精度工具。
- Dry-run 和事务回滚。
- `batch` 多工具批量执行。
- `apply_drawing_plan` 分阶段工程计划。
- 当前用户限定 Named Pipe。
- 文档 GUID 检查。
- 毫米与 Revit 内部英尺单位转换。
- 原生墙、楼板、房间、门窗、柱。
- 结构框架原生或受控回退。
- 元素查询、几何查询和删除。
- 3D 视图创建和 PNG 导出。

## 6. 系统结构

`	ext
AI Agent / Script
        |
        v
Named Pipe Client
        |
        v
Bridge Server
        |
        v
Revit ExternalEvent
        |
        v
Tool Dispatcher
        |
        +--> Transaction
        +--> Dry-run
        +--> Readback JSON
`

## 7. 测试环境

本次最终核验环境：

- Windows 版本：Microsoft Windows 11 家庭版 中文版 10.0.26200
- Autodesk Revit：2027.3，教育版
- .NET SDK：10.0.401
- 目标框架：`net10.0-windows`
- 平台：`x64`
- Git：2.53.0.2
- PowerShell：Windows PowerShell 5.1
- 测试模型：`<local-test-model>.rvt`

已完成的实际验证：

- `RevitAiBatch` 编译通过，0 个错误。
- 内置 Revit AI 注册工具成功，工具数 47。
- Revit 2027 实际创建和保存模型成功。
- 原生柱、门窗、房间和几何查询实际回读成功。
- 3D 视图创建和 PNG 导出成功。
- 文档保存后 `IsModified=false`。

尚未验证：

- Revit 2025、2026 或其他版本。
- 多 Revit 实例并发写入。
- 长时间无人值守运行。
- GitHub Actions 中的 Revit 实机测试。
- 正式代码签名后的加载行为。
- 商业使用场景下的全部第三方许可。

## 8. 开源价值定位

该项目适合作为：

- Revit + AI Agent 的本地工具桥。
- CAD/PDF 提取结果到 Revit 的结构化执行层。
- Revit 自动化插件开发的最小框架。
- MCP、HTTP 或本地 CLI 的上游工具核心。
- 建筑、结构自动化脚本的公共协议层。

## 9. 当前发布边界

开源源码仓库建议只包含自研工具层、协议层、桥接层和安装脚本。

不建议直接提交：

- Revit 安装目录中的 API DLL。
- 旧控制台兼容二进制。
- 需要单独授权的表格、PDF、浏览器或 CAD 依赖。
- 用户模型、日志、截图和导出文件。
- 历史品牌字段和旧桥接命名。

## 10. 建议许可证

自有核心建议使用 Apache-2.0。第三方依赖单独维护 `THIRD_PARTY_NOTICES.md`，并在发布包中注明版本、来源、许可证和分发方式。

## 11. 推荐首发内容

- 干净核心源码
- 47 工具 JSON 清单
- 架构文档
- 安全模型
- 安装脚本
- 示例 drawing plan
- Revit 2027 实测报告
- 构建与 Release 流程
