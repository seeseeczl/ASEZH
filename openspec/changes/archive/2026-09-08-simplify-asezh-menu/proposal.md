## Why

`Window/ASEZH` 当前同时暴露安装、撤回和两项维护诊断命令，普通用户难以判断哪些是日常操作。撤回能力又已存在于接入窗口，形成重复入口。

## What Changes

- `Window/ASEZH` 只保留「接入 Amplify Shader Editor」作为普通用户入口。
- 在接入窗口内保留「移除汉化补丁」，并新增折叠的「高级/诊断」区域，承载「重新加载词典」和「运行本地化测试」。
- 保持撤回确认、词典重载和自测行为不变，不改变补丁、Shader 或词典语义。
- 非目标：不重新设计安装器、不增加菜单层级、不删除维护能力。

## Capabilities

### New Capabilities

无。

### Modified Capabilities

- `display-locale`：将四个顶层 Window 菜单入口收敛为一个入口，并规定维护操作在接入窗口内可用。

## Impact

影响 `ASEZHInstallerWindow`、`ASELocale`、用户文档和菜单入口测试。公开运行时 API、依赖和 ASE 源码补丁不变。本变更并入尚未发布的 `0.0.8` 交付，不额外增加版本号。
