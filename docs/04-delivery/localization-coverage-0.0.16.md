# 0.0.16 FAGUI 属性名保持原文证据

目标包版本：`0.0.16`（`REL-ASEZH-0016`）
变更：`keep-fagui-attribute-names`
主机：macOS 27.0 arm64

## 问题

FAGUI 属性勾选列表在 `PropertyNode.cs` 通过 `EditorGUILayoutToggleLeft` 绘制。该包装方法已被安装器改写成先调用 `ASELocale.T`，因此 FAGUI 自有标识会被汉化：
`Foldout → 折叠`、`Ramp → 渐变条`、`Vector → 矢量`、`HelpBox → 帮助框`、`Tooltip → 提示`、`KeywordDesc → Keyword 说明`。

FAGUI 是外部框架，其属性名是框架标识而不是界面词汇，应当按原文显示。

## 修复

- 该调用点改为直接使用 Unity 原生 `EditorGUILayout.ToggleLeft`，不再经过显示层；FAGUI 名称始终按 ASE 源码里的写法显示。
- 不改 ASE 源码中 FAGUI 属性名文本本身，也不改 FAGUI 面板的其它字段。
- 词典条目保留：`Foldout` / `Ramp` / `Vector` 等英文词仍被其它界面位置使用，删除会误伤。
- 普通属性列表（`m_selectedAttribsArr`，非 FAGUI）继续汉化，未受影响。

## 验证

| 目标环境 | 静态检查 | 合成生命周期 | 许可 ASE 生命周期 | preimage 恢复 | 总结 |
| --- | --- | --- | --- | --- | --- |
| Tuanjie 2022.3.61t9 / macOS + ASE 1.9.9.5 | pass | pass | pass | pass | pass |

`v0.0.16` 的必选团结门禁为 `pass`；合成 fixture 已补上 FAGUI 调用片段，公开回归同样覆盖该补丁。

## 说明：截图里的中文不是本包写的

用户截图中出现的 `Foldout（当前属性）` 等文案来自 ASE 源码里新增的 `GetFaguiDisplayName` 硬编码表，
不是 ASEZH 产物：仓库全部历史中没有该代码，ASEZH 安装器只写入 `ASELocale.` / `ASENativeDisplay.` 包装，
从不写硬编码中文；ASEZH 的安装回执 preimage 中也没有这些字符串。若要恢复成 FAGUI 原文，
需要由 ASE 源码侧撤回那次本地修改。

## 未验证（非阻塞观察项）

真实画布交互截图、其他 Unity 版本与 Windows 未执行。
