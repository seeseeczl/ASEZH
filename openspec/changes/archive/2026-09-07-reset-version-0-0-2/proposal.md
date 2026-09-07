## Why

首次 Git 发布误用了 `1.0.0`。维护者要求把对外版本重置为 `0.0.2`，之后仍按 `0.0.1` 递增，避免把未稳定的显示层标成 1.x。

## What Changes

- `package.json` `version` 从 `1.0.1`（未对外推送）重置为 `0.0.2`。
- README、CHANGELOG 与 OpenSpec 版本规格同步为 `0.0.2`。
- 此后每次可更新交付：`0.0.3`、`0.0.4`…，未经明确要求不升 minor/major。
- **BREAKING**（对已安装 `1.0.0` 的 Git 包）：Package Manager 显示版本会从 `1.0.0` 变为 `0.0.2`。Git 仍按提交更新，功能不回退。

## Capabilities

### New Capabilities

- （无）

### Modified Capabilities

- `package-versioning`: 对外起点改为 `0.0.2`，后续只加 `0.0.1`。废弃「本交付变为 1.0.1 / 下一交付 1.0.2」的场景。

## Impact

- `package.json`、`README.md`、`CHANGELOG.md`、`openspec/specs/package-versioning/spec.md`
- 不改显示层查找语义或 ASE 钩子
