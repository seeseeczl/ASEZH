# 钩子清单

ASEZH 本体不修改 ASE 生成逻辑。要让某个 ASE 版本显示中文，必须在下列**显示路径**调用 `ASELocale`。自动安装器只覆盖锚点仍与官方 ASE 相近的文件；对不上就按本表手工接。

## P0 — 没有这些，设置面板几乎仍是英文

| 文件 | 钩子 | 做法 |
| --- | --- | --- |
| `UndoParentNode.cs` | `EditorGUILayoutPopup` / `IntPopup` / `EditorGUIPopup` | label 走 `T()`；`displayedOptions` 走 `TranslateArray` / `TranslateContents`（clone） |
| `UndoParentNode.cs` | `EditorGUILayoutEnumPopup` / `EditorGUIEnumPopup` | `ASELocale.LayoutEnumPopup` / `AreaEnumPopup`，按索引还原 enum |
| `UndoParentNode.cs` | `EditorGUILayoutToggle` / `ToggleLeft` / `Foldout` | label 走 `T()` / `GUI()` |
| `ToolsWindow.cs` | 左侧工具栏 | 源码图标右侧 `DrawLanguageToggle`，高度 21 与图标顶对齐 |

## P1 — 标题与检索

| 文件 | 钩子 | 做法 |
| --- | --- | --- |
| `NodeParametersWindow.cs` | 选中节点标题 | `T(Attributes.Name, TableNodeTitle)` |
| `ParentNode.cs` | `DrawTitle` | 只改 **显示** GUIContent，不要改 `m_content.text` |
| `ParentNode.cs` | `CheckFindText` | `MatchesSearch`，中英都能搜到 |
| `PaletteParent.cs` | 分类折叠 / 列表项 | `T(category, TableCategory)`；列表 `TNodeListLabel(Name, ItemUIContent.text)`；创建节点仍传英文 `Name`。安装器已覆盖。 |
| `PaletteParent.cs` | 搜索 | `MatchesSearch`；空搜索必须列出全部节点。禁止在未过滤分支插入 `if( !MatchesSearch ) continue`。过滤值等于「搜索」标签时视为空搜索。 |
| `NodeUtils.cs` | 分组标题 | `T(sectionName)` |

## P2 — Shader 值与 UI 标签拆开

| 文件 | 钩子 | 做法 |
| --- | --- | --- |
| `ZBufferOpHelper.cs` | ZWrite Popup | 增加 `ZWriteModeLabels`（英文条目与 `ZWriteModeValues` 相同），Popup 用 Labels，生成仍用 `ZWriteModeValues`。安装器按括号配对写入数组，禁止用会吃到 `ZTestModeDict` 的 `{.*?}` 正则。 |
| `TemplateDepthModule.cs` / `OutlineNode.cs` | 同上 | Popup 用 Labels |
| `InlineProperty.EnumTypePopup` | 参数名 | 传入的必须是 Labels，禁止把 keyword 数组直接拿去翻译后写回 |

## 禁止

- 对 TextField **值**（属性名、Pass 名、Inspector 名）调用 `T()`
- 把译后字符串写入 Shader / 序列化
- 把 `Direct3D` / `Vulkan` / `PlayStation` 写进词典
- 在节点面板标题旁或 Preferences 再放语言开关
