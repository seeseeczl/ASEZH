## 1. P0 安全与交付门禁

- [x] 1.1 交付 P0.1：以失败回归固定无大括号撤回缺陷，实现单一 ASE 根预检、内存 PatchPlan、带 preimage/SHA-256/临时写入/故障恢复的事务、残留钩子保护和结构化 Installer 结果，并验证格式、双根、失败恢复、幂等场景
- [x] 1.2 交付 P0.2：建立公开静态/合成检查和许可外部真实 ASE fixture 协议，以团结 2022.3.61t9 + ASE 1.9.81 的 install/apply/remove/recompile/preimage/本地化自测作为正式发布门禁，其他环境与人工 UI 作为非阻塞观察项

## 2. P1 架构、治理与真实 UI

- [x] 2.1 交付 P1.1：在 P0 回归保护下提取 patch catalog、target resolver、纯 transforms、transaction IO、locale store 与 GUI adapters，保持 public surface、团结目标环境和产品行为不变，并通过 LOC/依赖/API 回归
- [x] 2.2 交付 P1.2：校准并归档已完成的 `chinese-menu-and-uninstall-hooks`，使动态版本、active change、README、CHANGELOG、package、需求/REG/revision/证据追溯一致且受统一门禁检查
- [x] 2.3 交付 P1.3：记录真实画布、键盘/缩放、其他 Unity 与 Windows 的独立观察状态；未验证项不得标为通过，但不阻断以团结 2022.3.61t9 为目标的发布

## 3. P2 文档与消费者交付

- [x] 3.1 交付 P2.1：最小统一 README 与适配指南的 4 个中文菜单路径，将 `package.json` 从 `0.0.7` 连续提升到 `0.0.8` 并更新 CHANGELOG，运行公开/可用真实矩阵、OpenSpec strict、Project Architect、diff 和归档前验证
