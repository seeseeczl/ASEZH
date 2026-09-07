## Why

安装器对 ASE 源码做精确字符串匹配。缩进（tab/空格）或换行稍有不同就会 mismatch，导致 EnumPopup / Toggle / 语言开关接不上。`zwrite-labels` 只把 Popup 改成 `ZWriteModeLabels`，却不补数组定义，干净 ASE 会 CS0103，扫描却显示 applied。

## What Changes

- Find / Marker 按空白灵活匹配，不再要求 tab 数量与官方片段逐字节相同。
- `zwrite-labels` 必须同时写入 `ZWriteModeLabels` 数组定义，并把 Popup 改为 Labels；生成仍用 `ZWriteModeValues`。
- 语言开关在 `openSourceCodeButton.Draw` 之后、`GUI.color = bufferedColor` 之后插入。
- 已接上的判定改为：ZWrite 要数组和 Popup 都在；不能只因出现 `ZWriteModeLabels` 标识就报 applied。

## Capabilities

### New Capabilities

- `ase-hooks-installer`: 安装器把显示钩子打进当前工程的 ASE 源码，且失败时不得把工程打到无法编译。

### Modified Capabilities

- （无）

## Impact

- `Editor/Installer/ASEZHPatcher.cs`
- `docs/adapt-ase-version.md`、`docs/hook-sites.md`
- 不改词典查找语义，不改生成 Shader 的 KEY
