---
id: AA-OPT-2026-09-29-190550
type: optimization-plan
status: open
created_at: 2026-09-29T19:05:50+08:00
owner: ASEZH maintainer
related: [AA-CORR-001, AA-CORR-002, AA-GOV-001, AA-OPS-001, AA-TEST-001, AA-TEST-002, AA-DOC-001]
---

# 优化计划 — ASEZH 0.0.13 锚点兼容变更

来源：`docs/adversarial-audits/2026-09-29-190550-audit-report.md`。AA-CORR-001 与 AA-CORR-002 已在本次提交前修复，本计划保留其补测与收口任务。

## 任务表

| 任务 | 关联发现 | 优先级 | 目标 | 验收条件 | 回滚 |
| --- | --- | --- | --- | --- | --- |
| `AA-OPT-001` | `AA-CORR-001` | P2 | 为 asmdef 引用增删抽出可测纯函数并补单元测试 | 覆盖单行/多行/混合引用三种数组形态，以及"引用在别处出现"必须 `mismatch`；EditMode 全绿 | 纯测试与内部函数抽取，回退即恢复内联实现 |
| `AA-OPT-002` | `AA-GOV-001` | P2 | 恢复 1.9.8.x 覆盖，或在规范中显式降级并记录未验证 | 规范、fixture 协议与 `validate_delivery.py` 三者一致；门禁能同时报出两个 ASE 版本的结论 | 只改规范与脚本常量，不动产品代码 |
| `AA-OPT-003` | `AA-OPS-001` | P2 | Remove 无回执时显式提示"将还原为官方片段"，并考虑 Apply NoOp 时保留回执 | 安装器窗口与文档明确提示；无回执撤回不再静默 | 提示文案可单独回退 |
| `AA-OPT-004` | `AA-TEST-001` | P3 | 让合成 fixture 覆盖 1.9.9 的 `OnNodeLayout` 新签名 | 公开/可再分发门禁在没有许可 ASE 时也能验证新签名锚点 | harness 增加 fixture 变体开关，可单独关闭 |
| `AA-OPT-005` | `AA-TEST-002` | P3 | 把"锚点 × 真实 ASE 源码"稽核入库为公开静态检查 | 一条命令列出每条锚点的命中/失配（含变量拼接条目），退出码可进 CI | 新增脚本与 meta，删除即回退 |
| `AA-OPT-006` | `AA-DOC-001` | P3 | 修正回归矩阵文档头部与新增记录的矛盾 | 头部标明历史快照或改为索引式，读者不会误读当前发布目标 | 文档改动，可单独回退 |

## 优先级说明

- `AA-OPT-001` 到 `AA-OPT-003` 对应 4 条 P2 发现中的 OPEN 项与补测项，建议在下一次交付前完成，其中 `AA-OPT-001` 成本最低、收益最直接。
- `AA-OPT-004` 到 `AA-OPT-006` 为覆盖与文档债务，可随下一次相关改动一并处理。

## 本次发布（0.0.13）的收口状态

- 已修复：`AA-CORR-001`、`AA-CORR-002`。
- 已复验：团结 2022.3.61t9 + ASE 1.9.9.5 六阶段与 preimage 恢复 `pass`；合成门禁 `pass`；静态检查 12/12；OpenSpec 严格校验 8/8；EditMode 23/23。
- 未修复但不阻断：`AA-GOV-001`、`AA-OPS-001`，以及 3 条 P3。
