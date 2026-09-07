## Why

ASEZH 已作为独立 UPM 包发布，但需求只活在 README 和对话里，版本也停在 `1.0.0`。没有规格基线时，后续改动无法对照验收；不按 `0.0.1` 递增时，Package Manager 里的版本也无法对应每次可更新的交付。

## What Changes

- 用 OpenSpec 作为需求与变更的事实源：当前行为写入能力规格，之后每次语义变更走 `proposal → specs → design → tasks → apply → archive`。
- 包版本以 `package.json` 的 `version` 为准，每次对外交付只增加 `0.0.1`（patch），禁止跳号，除非用户明确要求升 minor/major。
- 本交付把版本从 `1.0.0` 增到 `1.0.1`，并增加 `CHANGELOG.md`。
- 为 Cursor 写入常驻规则，强制先对照 OpenSpec 再改代码。

## Capabilities

### New Capabilities

- `display-locale`: 显示层中文覆盖的硬规则（KEY 英文、生成物不汉化、平台名 fail-open、下拉只 clone）。
- `package-versioning`: 对外版本号来源、`0.0.1` 递增和 changelog 记录。
- `change-governance`: 用 OpenSpec 管理 FR 基线与 CR 增量，未入规格的语义不得当作已批准需求。

### Modified Capabilities

- （无。仓库此前没有 `openspec/specs/` 基线。）

## Impact

- 新增 `openspec/`、`.cursor/` 技能/命令、`.cursor/rules/` 治理规则。
- 修改 `package.json` `version`、`README.md`。
- 新增 `CHANGELOG.md`。
- 不改 `ASELocale` 查找语义、词典内容和 ASE 钩子行为。
