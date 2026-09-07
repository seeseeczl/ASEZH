## Why

`Window/ASEZH` 子菜单仍是英文，和工程里其它 Window 项不一致。接入后没有官方入口把 ASE 源码里的显示钩子撤掉，同事只能手工还原文件。

## What Changes

- `Window/ASEZH` 下菜单改为中文。产品名 `ASEZH` 保持英文。
- 增加「移除汉化补丁」：确认后把安装器写入 ASE 的显示钩子撤回到接入前片段。不卸载 UPM 包。
- 不把 ASEZH 自己的 Window 菜单接到画布语言开关上（Unity `MenuItem` 必须是编译期常量）。

## Capabilities

### New Capabilities

- （无）

### Modified Capabilities

- `ase-hooks-installer`: 必须能撤回已应用的显示钩子，且不改 Shader 生成值数组。
- `display-locale`: ASEZH 自己的 Window 子菜单显示中文。

## Impact

- `Editor/Installer/ASEZHInstallerWindow.cs`、`ASEZHPatcher.cs`、`Editor/ASELocale.cs`
- 未发布的 Search 列表筛空修复一并进入本次 `0.0.7`
