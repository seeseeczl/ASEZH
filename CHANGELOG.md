# Changelog

本仓库版本以 `package.json` 的 `version` 为准。当前起点是 `0.0.2`，每次面向消费者的交付只增加 `0.0.1`。

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
