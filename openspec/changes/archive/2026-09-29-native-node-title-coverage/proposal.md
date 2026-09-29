## Why

用户反馈"还有好多节点名没有汉化"。对着本地 ASE 1.9.9.5 源码核对后确认是三个叠加的原因：8 个内置节点类型根本不在原生白名单里（白名单只按类型全名放行，不在名单里的标题与 Search 列表永远保持英文）；5 个白名单节点的名字在 `node_title` 词典里没有词条；`Toggle Switch` 节点自己画标题、绕过了 `ParentNode` 的标题钩子。

## What Changes

- 把 8 个内置节点类型补入原生白名单，并补齐它们的 `node_title` 词条。
- 补齐 5 个白名单节点的缺失词条与 `Debug` 分类词条。
- 按既有 ScreenColor/StaticSwitch 的方式接入 `ToggleSwitchNode` 的标题绘制点。
- 审计命令增加节点标题三类检查（类型白名单、节点词条、分类词条）。
- 非目标：不翻译节点重命名文本、自定义 Shader Function、Flyme 分类与用户数据。

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `native-node-display`: 原生节点标题与分类的覆盖完整性，以及可审计性。

## Impact

原生类型白名单、词典 node_title/category 表、原生标题目录、审计脚本与合成 fixture；包版本 0.0.15。
