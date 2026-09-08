# 2026-09-08 ASEZH 回归矩阵

目标包版本：`0.0.8`
真实 ASE fixture：维护者外部副本 `1.9.81`，仅复制到系统临时目录
主机：macOS 26.5.1 arm64

## 正式发布门禁

| 目标环境 | 静态检查 | 合成事务生命周期 | 真实 ASE 生命周期 | preimage 恢复 | Locale 自测 | 总结 |
| --- | --- | --- | --- | --- | --- | --- |
| 团结 2022.3.61t9 / macOS + ASE 1.9.81 | pass | pass | pass | pass | pass | pass |

## 扩展观察项（非阻塞）

| 观察项 | 状态 | 说明 |
| --- | --- | --- |
| Shader 中文/原文生成等价 | unverified | 未执行独立生成文本哈希对比，不冒充通过 |
| 真实 Search / Toggle / dirty state | unverified | Locale 自测覆盖搜索逻辑，但不等于真实画布交互证据 |
| 完整 UI / 键盘 / 缩放 / 无障碍 | unverified | 菜单已精简，尚未执行完整人工 UI 验收 |
| Unity 2021.3.7f1c1 / macOS | observed-incompatible | ASE 1.9.81 在接入前即因缺失 API 编译失败，不归因于 ASEZH |
| Unity 2019.4 / macOS | not-run | 非当前发布目标 |
| Windows | not-run | 非当前发布目标 |

## 已验证

- 持久 EditMode 回归 6/6：LF/CRLF、有/无大括号、双根隔离、跨根拒绝、事务提交和中途失败恢复。
- 公开合成隔离工程 6 个阶段全部通过；Apply→Remove 后 7 个受管文件 SHA-256 与批准 preimage 一致。
- 真实 ASE 1.9.81 在团结 2022.3.61t9 完成 6 个阶段；Apply 后新进程重编译成功，Remove 后新进程重编译成功，7 个受管文件哈希恢复。
- Unity 2021.3.7f1c1 在接入前因 ASE 1.9.81 的 `Texture.isDataSRGB`、`MaterialEditor.BeginProperty/EndProperty`、`LocalKeyword.isDynamic` 等 API 不兼容而失败；未改写该 fixture，也未归因于 ASEZH。

## 发布结论

团结 2022.3.61t9 + ASE 1.9.81 的必选门禁为 `pass`，因此 `v0.0.8` 可发布。扩展观察项继续以 `unverified` / `not-run` 保留，不阻止本次团结目标发布。公开 CI 仅执行可再分发静态门禁，不声称团结正式门禁通过。

`v0.0.9` 仅修复 Git 包内 OpenSpec 文档的非法 Unity GUID；全仓库 GUID 格式/重复检查、团结包导入、合成与真实 ASE 生命周期均已重新执行并通过，Editor 日志中无 GUID/YAML Parser 警告。

## 最小追溯链

| 交付 ID / 版本 | OpenSpec change | 需求 / 回归 ID | 实现 revision | 验证证据 |
| --- | --- | --- | --- | --- |
| `REL-ASEZH-0008` / `0.0.8` | `harden-patcher-and-regression-gates` | `AUD-FLOW-001`、`AUD-TARGET-001`、`REG-PATCH-TRANSACTION-001`、`REG-ASE-LIFECYCLE-001` | `v0.0.8` / `Tuanjie 2022.3.61t9` / `pass` | 正式发布复验写入 `/private/tmp/asezh-release-gate-0.0.8/delivery-manifest.json`；扩展观察状态见上表 |
| `REL-ASEZH-0009` / `0.0.9` | 主规范 `release-regression-gates` | `BUG-META-GUID-001`、`REG-META-GUID-001` | `v0.0.9` / `Tuanjie 2022.3.61t9` / `pass` | `/private/tmp/asezh-release-gate-0.0.9/delivery-manifest.json`；合成/真实 ASE 六阶段通过，GUID/YAML 警告扫描为零 |
