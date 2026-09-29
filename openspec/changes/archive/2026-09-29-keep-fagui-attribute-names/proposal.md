## Why

用户反馈 FAGUI 的属性名不该被汉化。核对后确认：FAGUI 属性勾选列表在 `PropertyNode.cs` 里通过 `EditorGUILayoutToggleLeft` 绘制，而这个包装方法已被安装器改写成先走 `ASELocale.T`，于是 `Foldout`、`Ramp`、`Vector`、`HelpBox`、`Tooltip`、`KeywordDesc` 会被译成「折叠 / 渐变条 / 矢量 / 帮助框 / 提示 / Keyword 说明」。FAGUI 是外部框架，其属性名属于框架标识，不是界面词汇。

## What Changes

- 让 FAGUI 属性勾选列表绕开显示层：该调用点改为直接使用 Unity 原生 `EditorGUILayout.ToggleLeft`。
- 普通属性列表（非 FAGUI）与其它界面文案的汉化保持不变。
- 非目标：不改 ASE 源码里 FAGUI 属性名本身的文本，也不改 FAGUI 面板的其它字段。

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `display-locale`: 外部框架自有标识保持原文，不进入显示层翻译。

## Impact

界面文案目录新增一条"保持原文"补丁、合成 fixture 与词典无需改动；包版本 0.0.16。
