## Context

见 proposal.md。安装器把「接入」建模为一段 Find → Replace 文本改写，并按锚点是否命中决定
`ready` / `applied` / `mismatch`。该模型隐含假设被挂钩方法的签名在 ASE 各版本间稳定，而
ASE 1.9.9 恰好打破了这个假设：`OnNodeLayout` 增加了 `NodeUpdateCache cache = null`。

## Goals / Non-Goals

目标：签名漂移只影响能否命中锚点，不影响已声明锚点之外的安全性；老版本 ASE 继续可用。
非目标：把锚点改成正则或按语义解析 C#；未声明漂移仍拒绝写入，交由 `docs/adapt-ase-version.md` 手工接入。

## Decisions

- 不直接改写成新签名。仓库的正式门禁与历史证据仍是老签名 ASE，直接替换会把旧版本从可用打成 `mismatch`。
- 在 `ASEZHPatch` 上增加 `AltFind` / `AltReplace`，与既有 `LegacyReplace`（备用锚点）同源，
  由 `ASEZHPatchAnchors` 统一产出「Find, Replace」变体列表。扫描、应用、撤回都按变体遍历。
- 变体自带配套的 Replace，因此命中哪套签名就写回哪套签名行；`AdaptStyle` 继续只负责缩进与换行风格。
- 以「未声明签名仍然 mismatch」作为安全边界：变体只覆盖已声明集合，未知漂移不猜测。

## Risks / Trade-offs

- 变体表会随 ASE 版本增长 → 保留 fail-closed 与手工接入文档作为兜底，优先修锚点而不是放宽匹配。
- 撤回路径同时遍历变体 → 与既有回执恢复互补；无回执时按当前文本命中的变体还原。

## Migration Plan

0.0.13 只新增备用锚点，不改写既有锚点语义；已接入 1.9.81 的工程重新扫描仍报 `applied`，不会重复写入。
