# 2027 测试版本说明

本仓库中与 Revit 2027 混合测试包相关的内容均为测试版本，非正式发行版，仅供参考、研究与本地验证使用。

## 包含内容

- `releases/2027-test/RevitAI-Hybrid-Setup-5.0.0.exe`
- `rebuilt-120-tools/`
- `worktree/`

## 风险提示

- 请在测试模型或隔离环境中验证。
- 不要直接用于生产项目、正式交付或未经备份的模型。
- 安装前请关闭 Revit。
- 本仓库不包含本机用户配置、日志、会话记录、缓存、API Key 或机器指纹数据。

## Codex 桥接

Revit AI UI 插件启动时会默认启动内置 Codex 桥，并写入：

`%LOCALAPPDATA%\RevitAi\codex-bridge.json`

外部 Codex 客户端可通过该 discovery 文件连接并调用插件注册的 Revit 工具。
