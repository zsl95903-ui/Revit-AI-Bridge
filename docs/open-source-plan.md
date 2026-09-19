# Revit AI Bridge 开源发布方案

状态：首发源码核验完成，可以按“自研核心源码公开发布”的方式执行。
发布标签：`1.2.5`
核心程序集：`RevitAiBatch.dll 1.2.5.0`
工具数量：`47`

## 1. 项目定位

Revit AI Bridge 是面向 Autodesk Revit 2027 的本地类型化自动化桥接层。它把读取、规划、建模、验证和保存拆成结构化工具，通过当前用户 Named Pipe 与 Revit 主线程的 `IExternalEventHandler` 通信。

面向 GitHub 的首发定位：

> Open-source, local-first bridge toolkit that exposes typed Revit 2027 automation tools to AI agents and scripts.

首发内容包括：

- 自研 Revit API 工具层
- Named Pipe 桥接层
- 47 个工具 schema 和批量计划执行器
- Revit 加载项入口
- 构建、打包和安装脚本
- 架构、安全、兼容性和第三方审计文档

首发不包含：

- Autodesk Revit API 二进制
- 用户模型、日志、截图和导出文件
- 未明确许可的第三方二进制
- 内部兼容程序集和历史发布包

## 2. 已核验能力

### 文档与上下文

- 活动文档、单位和文档状态读取
- 标高、轴网、视图和视图设置读取
- 墙类型、楼板类型和族类型读取
- 元素按类别、名称和类型查询
- 元素包围盒、位置、Solid 和 Curve 读取
- 房间边界读取

### 基准与定位

- 创建和更新标高
- 创建轴网
- 调整标高标头
- 激活或创建 3D 视图

### 建筑与结构建模

- 创建直墙和带洞口的分段墙
- 创建闭合轮廓楼板
- 创建原生矩形柱实例
- 创建原生结构梁实例或结构框架类别受控回退
- 创建原生门、窗宿主实例
- 创建房间
- 创建楼梯、基础、柱、梁和体量块几何
- 创建详图线、曲线、圆弧和多段线
- 创建尺寸和文字注释

### 计划与事务

- `apply_drawing_plan` 按依赖阶段执行多操作计划
- `batch` 在单事务内执行多个工具
- 支持 `dryRun`
- 文档 GUID 一致性检查
- 每项写入返回 ElementId
- Warning 级 Failure 自动处理
- 支持视图激活、自动缩放和裁剪范围扩展

### 视图、导出和文档

- 隐藏和显示元素
- 项目线颜色覆盖
- 创建 3D 视图
- 导出视图 PNG
- 保存和另存为

## 3. 公开源码逻辑树

公开源码包含 10 个 C# 文件：

```text
src/RevitAiBatch/
  BridgeLog.cs
  BridgeServer.cs
  CodexBatchHost.cs
  ExternalEventInvoker.cs
  Models.cs
  RevitAiBatch.csproj
  RevitAiBatchApplication.cs
  ToolDispatcher.Additional.cs
  ToolDispatcher.cs
  Units.cs
```

运行调用链：

```text
AI / Script
    |
    v
Named Pipe client
    |
    v
BridgeServer
    |
    v
ExternalEventInvoker
    |
    v
Revit API external event
    |
    v
ToolDispatcher
    |
    +--> read-only query
    +--> transaction
    +--> dry-run rollback or commit
    +--> readback DTO
```

主要类：

- `BridgeServer`：Named Pipe 生命周期、发现文件和请求分发。
- `ExternalEventInvoker`：把工作封送到 Revit API 线程。
- `ToolDispatcher`：工具注册、schema、事务、提交、回滚和回读。
- `CodexBatchHost`：工具描述、工具目录和宿主调用入口。
- `RevitAiBatchApplication`：Revit 加载项入口和状态命令。
- `Models` 和 `Units`：消息、JSON 和单位转换。

## 4. 首发包内容

建议首发 Release 只提供：

- `RevitAiBatch.dll 1.2.5.0`
- `RevitAiBatch.deps.json`
- `RevitAiBatch.addin`
- `install.ps1`
- `README.md`
- `LICENSE`
- `THIRD_PARTY_NOTICES.md`
- `SHA256SUMS.txt`
- `sbom.cdx.json`

当前加载项未签名。正式公开安装包应明确提示该状态，或者由维护者使用代码签名证书完成签名。

## 5. 开源边界

可以开源：

- 自研 C# 源码
- JSON 协议和 schema
- PowerShell 构建及安装脚本
- 工具文档
- 构建脚本

不提交：

- Autodesk Revit API 二进制
- 未明确许可的第三方 DLL
- 第三方源码副本
- 用户模型、日志、图片和凭据
- 内部命名、私有字段或未公开兼容二进制

## 6. 许可证方案

自有核心使用 Apache-2.0。发布时必须保留：

- `LICENSE`
- `NOTICE`
- `THIRD_PARTY_NOTICES.md`
- `docs/third-party-audit.md`
- Release 包的 `SHA256SUMS.txt`
- 发布包依赖的 SBOM

第三方依赖应按以下原则处理：

- 源码仓库不 vendoring 第三方源码。
- 只为实际分发的第三方二进制保留许可证文本。
- 许可证状态不明确的组件不能进入默认 Release。
- Autodesk API 始终由用户本地安装提供。

## 7. 版本计划

统一版本号：

```text
Git tag        v1.2.5
Assembly       1.2.5.0
File version   1.2.5.0
Catalog        1.2.5
```

后续版本建议使用语义化版本：

- `1.2.x`：兼容修复、新工具和稳定性改进。
- `1.3.0`：协议或 schema 有向后兼容扩展。
- `2.0.0`：协议或调用契约发生不兼容变化。

## 8. 开发历程

1. 原型阶段：建立 Named Pipe、ExternalEvent 和基础 Revit 读取工具。
2. 计划执行阶段：增加 `batch`、`apply_drawing_plan`、dry-run 和分组事务。
3. 宿主集成阶段：把工具注册到 Revit AI 入口，形成 47 工具目录。
4. 建筑结构工具阶段：增加原生柱、梁、房间、门、窗、族加载、删除和几何查询。
5. 稳定性阶段：修复类别比较、Warning 模态框、宿主门窗 Z 标高和批量事务回滚问题。
6. 开源准备阶段：拆分公开核心，清理历史发布内容，建立许可证、构建、扫描和文档流程。

## 9. 后续路线

### Phase 1：协议稳定

- 固定 JSON 请求和响应格式。
- 为每个工具生成 JSON Schema。
- 明确读取、写入、dry-run 和回读契约。
- 增加协议版本兼容字段。

### Phase 2：自动化测试

- JSON 解析测试。
- 单位转换测试。
- 路径安全测试。
- schema 快照测试。
- dry-run 回滚测试。
- Revit 2027 自托管 Runner 冒烟测试。

### Phase 3：持续发布

- 生成可复现的清洁 ZIP。
- 输出 SHA256 和 SBOM。
- 建立 release checklist。
- 在完成证书配置后提供签名版本。

## 10. 发布检查表

- [x] 公开源码通过内部字段和历史命名扫描。
- [x] 公开源码通过 Release 编译。
- [x] 版本号统一为 `1.2.5.0`。
- [x] Autodesk DLL 未提交。
- [x] 用户模型和本机绝对路径未提交。
- [x] 第三方引用审计已生成。
- [ ] 配置代码签名证书。
- [ ] 生成首发 Release 的 SHA256 和 SBOM。
