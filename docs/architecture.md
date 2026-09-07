# 架构

```
ASEZH (本仓库)                    目标 Unity 工程
┌─────────────────────┐          ┌──────────────────────────┐
│ ASEZHDictionary.json│          │ AmplifyShaderEditor/     │
│ ASELocale 引擎      │  显示调用 │  UndoParentNode  ───────►│ ASELocale.T / TranslateArray
│ Installer 锚点补丁  │          │  ToolsWindow DrawToggle  │
└─────────────────────┘          │  生成 Shader（英文 KEY） │
                                 └──────────────────────────┘
```

- **词典**是长期维护面：只加 table+key+zh。
- **引擎**不知道 ASE 节点类型，只做查找、clone、开关。
- **钩子**是版本适配面：随 ASE 源码漂移，用 Installer 扫描 + 手工补。
- ASE 若有独立 `AmplifyShaderEditor.asmdef`，必须 `references` 包含 `ASEZH.Editor`。`autoReferenced` 只对预定义程序集生效，不会让 ASE 看到 ASEZH。安装器会自动写入这条引用。

查找顺序：

- 字段标题 `T(key)`：option_label → category → node_title → panel → option_value
- 下拉值 `TranslateArray`：option_value → option_label → …

因此 `Add` 作节点名是「加法」，作 Blend Op 是「相加」。
