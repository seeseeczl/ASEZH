# Changelog

本仓库版本以 `package.json` 的 `version` 为准。当前起点是 `0.0.2`，每次面向消费者的交付只增加 `0.0.1`。

## 0.0.10 - 2026-09-08

- 安装器现在能识别 ASE 1.9.81 将 `ASELocale.T(m_searchFilterStr)` 缓存在局部变量后用于 Search 标签宽度计算的等价写法，不再将已接入代码误报为 `palette-search-width [mismatch]`。
- 等价识别要求赋值和宽度计算使用同一变量；Apply 保持源码不变，Remove 仅凭有效安装回执恢复 preimage，无回执时保留既有实现并由残留门禁拒绝假成功。

## 0.0.9 - 2026-09-08

- 修复 OpenSpec 文档元文件中的非法 GUID，避免团结在导入 Git 包时忽略文档资产并持续输出警告。

## 0.0.8 - 2026-09-08

- 补丁安装改为唯一 ASE 根预检、隔离计划、SHA-256 校验与失败回滚；撤回可跨 Editor 重启精确恢复接入前内容，并用结构化状态避免假报成功。
- 拆分安装器事务、目标解析、特殊变换与本地化存储/GUI 职责，保持现有公开 API；增加 6 个持久 EditMode 回归、公开合成生命周期门禁及许可外部真实 ASE 协议。
- 校准 OpenSpec 生命周期、动态版本与交付追溯规则，归档已完成的中文菜单/撤回变更，并统一用户文档中的 4 个中文菜单路径。
- `Window/ASEZH` 收敛为单一「接入 Amplify Shader Editor」入口；撤回保留在接入窗口，词典重载和本地化测试移入折叠的「高级/诊断」区域。
- 正式发布门禁收敛为团结 2022.3.61t9 + ASE 1.9.81；Shader 对比、真实画布/UI、其他 Unity 与 Windows 保留为非阻塞观察项，未验证时明确披露且不冒充通过。

## 0.0.7 - 2026-09-07

- Search 过滤不再把「搜索」标签当成关键词，避免节点列表被筛空。安装器会去掉 0.0.5 写进 `PaletteParent` 的错误 `continue`。词典加载日志改到 OnGUI 之后，避免第一次绘制把标签写进输入框。
- `Window/ASEZH` 子菜单改为中文，并增加「移除汉化补丁」。

## 0.0.6 - 2026-09-07

- 安装器不再用会跨过 `ZTestModeDict` 的正则补 `ZWriteModeLabels` 分号。`zwrite-labels` 按括号配对复制数组，并修复 `{ZTestMode.Less,1 };` 导致的 CS1513。

## 0.0.5 - 2026-09-07

- 安装器接入 `PaletteParent.cs`：Search 窗口内置分类（Camera And Screen 等）与节点行走显示层词典。

## 0.0.4 - 2026-09-07

- 安装器复制 `ZWriteModeLabels` 时带上数组末尾分号，并修复 0.0.3 已写入但缺 `;` 的文件。

## 0.0.3 - 2026-09-07

- 安装器按空白灵活匹配 ASE 锚点，EnumPopup / Toggle / 语言开关不再因 tab 与空格差异而 mismatch。
- `zwrite-labels` 会补上 `ZWriteModeLabels` 数组定义，避免只改 Popup 导致 CS0103。

## 0.0.2 - 2026-09-07

- 对外版本重置为 `0.0.2`，此后按 `0.0.1` 递增（`0.0.3`、`0.0.4`…）。
- 用 OpenSpec 管理需求基线与变更（`openspec/`）。
- 安装器可为 `AmplifyShaderEditor.asmdef` 写入对 `ASEZH.Editor` 的引用，避免 `ASELocale does not exist`。
- 为包内文档和根文件补齐 Unity `.meta`，消除 Git 包不可变目录下的 missing meta 警告。

## 1.0.0 - 2026-09-07（已退役）

- 首次 Git 发布。版本号已重置，请以 `0.0.2` 为当前起点。
