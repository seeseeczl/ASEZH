---
id: AUD-2026-09-08-133427
type: audit
status: verified
version: 1
created_at: 2026-09-08T13:34:27+08:00
owner: ASEZH maintainer
related: [AUD-FLOW-001, AUD-TARGET-001, AUD-FLOW-002, AUD-FE-001, AUD-SIZE-001, AUD-GOV-001, AUD-FE-002, AUD-DOC-001]
supersedes: []
evidence: [/private/tmp/asezh-project-architect-deep-evidence-final.json, /private/tmp/asezh-unity-audit.pqwXGF]
---

# 项目审计报告

## 元信息

- 项目路径：`/Users/long/GitHub/ASEZH`
- 项目类型：Unity/团结 Editor-only UPM 包；Amplify Shader Editor 显示层中文化与源码钩子安装器
- 审计时间：2026-09-08T13:34:27+08:00（Asia/Shanghai）
- 当前分支 / commit：`main` / `687440f1c63e6fea5eede19c7ffc7c6d47c16510`，与 `origin/main` 一致（0 ahead / 0 behind）
- 工作区状态：采证开始时 dirty，仅有用户既存未跟踪目录 `.agents/`；本次未读取其业务内容、未修改或清理
- 审计范围：全仓库静态扫描；OpenSpec、模块/入口、源码写入安全、回归、UPM 导入、远端交付、七类专项；隔离团结项目中的编译、自测和合成补丁往返
- 非目标：不修改业务代码、配置、依赖、CI、真实 ASE 工程或远程状态；不初始化 CodeGraph；不发布、不打 tag、不修复发现
- 请求模式 / 实际模式：深度 / 深度（真实 ASE 画布、最低 Unity 2019.4、Unity 2021.3 和发布后安装证据降级为未验证）
- 未覆盖范围：真实 ASE 1.9.x 源码接入与撤回、Shader 生成文本无差异、语言切换 dirty/Live 状态、画布与安装器截图、键盘/焦点/缩放、Windows/其他目标平台、Unity 2019.4；Unity 2021.3 隔离项目创建进程以 143 结束，未形成可用结论

## 审计模式与证据等级

| 项目 | 结果 | 证据/限制 |
| --- | --- | --- |
| 证据采集器 | 已运行 | schema 1.1；152/152 文件索引，72 个文本文件读取，未截断；临时证据 `/private/tmp/asezh-project-architect-deep-evidence-final.json` |
| 静态证据 | 已验证 | 3 个 C# 文件、1079 条词典、4 个 OpenSpec 基线 spec、1 个 active change、包/asmdef/meta、Git 历史与远端状态 |
| 自动化测试 | 部分已验证 | 团结 2022.3.61t9 UPM 导入与 `RunSelfTests()` 通过；合成无大括号 `PaletteParent` 撤回回归按预期失败；仓库没有测试程序集或 CI |
| 运行与 UI | 未验证 | Product Design/UI 捕获因同 bundle ID 被解析到用户现有 `FlymeAuto3Test` 窗口而停止；未操作该工程、无可接受截图 |
| 发布产物 | 部分已验证 | GitHub 公开仓库 `origin/main` 与本地同 commit；Git archive 174 项且 UPM 可导入；无 tag、Release、Actions、分支保护、签名、SBOM 或发布哈希 |

## 结论摘要

- 总体判断：项目方向清晰、词典数据干净、OpenSpec 和版本递增已有基础，当前包自身可在团结 2022.3.61t9 编译并通过内置自测；但会改写消费者 ASE 源码的主链路尚未达到可安全发布的闭环，存在已复现的撤回假成功和残留依赖风险。
- 最大阻塞：缺少可合法纳入自动化的真实 ASE 版本夹具/目标工程，以及本次可安全控制的独立 Editor UI 实例；因此无法证明真实应用、撤回、画布显示、Shader 等价和最低版本兼容。
- 最大回归风险：`ASEZHPatcher` 直接对全局同名文件逐项写入，缺少单根目录锁定、全量预检、精确备份和失败回滚；最近 5 个连续版本均在修补该文件的安装器回归。
- 第一优先优化方向：把安装/撤回改造成“单一 ASE 根目录 + 全量预检 + 精确 preimage 备份 + 事务式写入/回滚 + 残留引用检查”，并先保留当前失败样本为永久回归。
- 问题统计：S0=0，S1=4，S2=3，S3=1；P0=4，P1=3，P2=1

## 项目简介与功能作用

- 项目简介：ASEZH 面向使用 Amplify Shader Editor 的中文 Unity/团结编辑器用户，以独立 UPM 包提供显示层翻译；它不分叉 ASE，也不应改变 Shader/Keyword/平台名等内部语义。
- 主要输入/输出与边界：输入为 1079 条 `table + key + zh` 词典、语言偏好与目标 ASE 源码；输出为中文显示副本和对 ASE 显示路径的源码钩子。包本身不包含 ASE，不拥有 Shader 生成逻辑；对目标工程的源码写入是最高风险信任边界。

| 功能/场景 | 目标用户 | 入口与输入 | 主要输出 | 实现状态 | 证据 |
| --- | --- | --- | --- | --- | --- |
| 中英文显示切换 | ASE 编辑用户 | 画布 `中文/EN` Toggle、英文 KEY | 中文显示或英文原文 | 部分实现、真实画布未验证 | `Editor/ASELocale.cs:56-369`；团结批处理自测通过 |
| 词典加载与用户覆盖 | 维护者/高级用户 | 主词典与 `.user.json` | 只覆盖已有键的内存表 | 已实现、持久性边界未验证 | JSON 1079 条，无空项/重复键/非法表；`ASELocale.cs:79-162` |
| 接入 ASE 显示钩子 | 项目维护者 | Installer 扫描、确认、应用 | 目标 ASE 源码片段与 asmdef 引用 | 部分实现 | 17 个 catalog 项；合成 apply/scan 通过 |
| 撤回 ASE 显示钩子 | 项目维护者 | 菜单/按钮确认 | 恢复接入前片段 | 已确认断链 | 无大括号过滤条件下残留 `ASELocale.MatchesSearch`，严格回归退出 4 |
| Git UPM 分发 | Package Manager 用户 | Git URL / `main` | `com.asezh.locale@0.0.7` | 可安装、发布闭环不完整 | 团结本地 UPM 导入通过；GitHub 无 tag/Release/Actions |

## 审计覆盖率

| 对象 | 分类 | 覆盖状态 | 证据方式 | 未覆盖原因 |
| --- | --- | --- | --- | --- |
| 仓库文件与清单 | 核心 | 已验证 | 152/152 未截断扫描、Git archive 174 项 | 扫描器把未跟踪 `.agents/` 计入工作区证据，但远端产物不含它 |
| 本地化引擎 | 核心 | 已验证到批处理 | 源码审阅、字典校验、团结自测 | 未进入真实 ASE OnGUI/Shader 生成链路 |
| 钩子安装/撤回 | 核心 | 已验证到合成夹具 | 源码审阅、合成 17 项 apply/remove | 不包含受许可约束的真实 ASE 源码版本矩阵 |
| Editor UI | 核心 | 未覆盖 | 已尝试独立实例捕获并主动停止 | CUA 绑定到用户现有工程，继续操作不安全 |
| UPM/远端交付 | 核心 | 抽样 | 本地 UPM、GitHub API、remote SHA | 无不可变 tag/Release、CI artifact、发布后消费者工程 |
| Windows/目标平台 | 支撑 | 未覆盖 | 安装声明与源码推断 | 无对应设备/构建/运行证据 |

## 严重程度总览

| 问题 ID | 证据状态 | 优先级 | 严重程度 | 领域 | 问题 | 核心证据 |
| --- | --- | --- | --- | --- | --- | --- |
| AUD-FLOW-001 | 已验证 | P0 | S1 | 功能闭环 | 无大括号 Search 条件无法完整撤回，仍残留 `ASELocale` 调用 | `ASEZHPatcher.cs:523-567`；严格合成回归退出 4 |
| AUD-TARGET-001 | 已验证 | P0 | S1 | 写入安全 | 目标按全局同名文件逐个解析且直接覆盖，无单根锁定、备份或事务回滚 | `ASEZHPatcher.cs:192-245,335-385` |
| AUD-FLOW-002 | 已验证 | P0 | S1 | 回归/发布 | 核心承诺没有真实 ASE 自动回归或 CI 门禁 | 无 Tests/CI；`RunSelfTests()` 只覆盖本地化查找；连续 0.0.3-0.0.7 热修 |
| AUD-FE-001 | 已验证 | P0 | S1 | 交互反馈 | 撤回入口会把未实际撤回的步骤计为 `removed` 并提示成功 | `ASEZHInstallerWindow.cs:23-30` 与 `ASEZHPatcher.cs:559-563` |
| AUD-SIZE-001 | 已验证 | P1 | S2 | 架构 | 本地化核心与补丁器超过生产代码硬上限，补丁器已形成高变更半径 | 497 行与 947 行；`Catalog()` 约 159 行；严格架构检查失败 |
| AUD-GOV-001 | 已验证 | P1 | S2 | 治理 | 已完成/已交付 change 未归档，版本基线 spec 仍把当前版本写死为 0.0.2 | `openspec list` Complete；`package-versioning/spec.md:24-29` 对比 0.0.7 |
| AUD-FE-002 | 未验证 | P1 | S2 | UI/无障碍 | 关键 UI 没有可复核截图、键盘/焦点/缩放与状态验收 | 本次 Product Design 捕获因应用实例歧义停止 |
| AUD-DOC-001 | 已验证 | P2 | S3 | 文档 | 部分操作路径仍使用已删除的英文菜单名 | `README.md:172,247`；`docs/adapt-ase-version.md:13,41` |

## 项目架构

### 当前结构

- 单一 Editor-only asmdef `ASEZH.Editor`；`ASELocale` 持有语言偏好、字典加载、翻译 API、搜索和自测。
- `ASEZHPatcher` 同时承担补丁目录、AssetDatabase 目标发现、文本变换、状态判定、写入、撤回和特殊迁移；`ASEZHInstallerWindow` 直接调用其静态 API。
- 数据流为 `JSON/EditorPrefs → ASELocale 内存表 → ASE OnGUI 显示副本`；安装流为 `EditorWindow → AssetDatabase 全局检索 → File.ReadAllText/WriteAllText → AssetDatabase.Refresh`。
- 外部依赖只有 UnityEditor/UnityEngine 与目标 ASE 源码；没有网络、数据库、服务端或运行时程序集。

### 关键链路

| 链路 | 入口 | 核心模块 | 数据/状态流 | 外部依赖 | 结论 |
| --- | --- | --- | --- | --- | --- |
| 词典显示 | `T/GUI/TranslateArray` | `ASELocale` | JSON → table map → clone display | AssetDatabase、EditorPrefs、IMGUI | 批处理已验证，真实 ASE 部分闭环 |
| 中英文搜索 | patched `PaletteParent` | `MatchesSearch` + patcher | 英文/中文查询 → bool | ASE Palette 数据 | 合成锚点已验证，真实 UI 未验证 |
| 应用钩子 | Installer apply | `Catalog/Evaluate` + special handlers | 全局路径 → 文本替换 → 覆盖写 | 目标 ASE 源码/asmdef | 高风险、缺事务与真实回归 |
| 撤回钩子 | Installer remove | `Reverse` + special removers | Replace→Find / 特殊正则 → 覆盖写 | 目标 ASE 源码/asmdef | 已复现断链 |
| UPM 交付 | Git URL | package manifest/meta | `main` commit → Package Manager | GitHub/Unity UPM | 可导入但不可变发布证据缺失 |

### 模块边界与隔离

| 模块 | 独立文件/目录 | 公共接口 | 独立测试 | 跨模块依赖 | 修改爆炸半径 | 结论 |
| --- | --- | --- | --- | --- | --- | --- |
| 显示引擎 | 部分 | `ASELocale` 多个 public 静态 API | 仅内嵌自测 | UnityEditor、词典、被补丁 ASE | 中 | 职责偏多但边界可识别 |
| 词典数据 | 是 | 隐式 `table/key/zh` schema | 静态校验仅本次临时执行 | `ASELocale.LoadJson` | 中 | 数据干净，缺持久校验 |
| 钩子安装器 | 目录存在、逻辑集中单文件 | `Scan/ApplyAll/RemoveAll/Catalog` | 无仓库测试 | AssetDatabase、文件系统、ASE 多文件 | 高 | 上帝文件候选且写入边界不安全 |
| UPM 交付 | 顶层文件分散 | `package.json`/Git URL | 无 CI | GitHub `main`、Unity Package Manager | 高 | 版本记录连续，发布门禁薄弱 |

## 技术路径与交付形态

- 技术路径：C# IMGUI Editor 扩展；JSON 词典；EditorPrefs 保存语言；AssetDatabase 发现资源；正则/文本锚点适配 ASE；Git URL UPM 分发。
- 实现/壳形态：Unity/团结编辑器插件和源码适配器，不进入 Player Runtime。
- 构建与交付：没有仓库构建脚本或 CI；当前以 `origin/main` 作为可变 Git 包来源。版本 0.0.2→0.0.7 每次递增 0.0.1，README/package/changelog 当前一致。

| 交付物 | 格式/载体 | 生成方式 | 安装/部署 | 签名/发布证据 | 结论 |
| --- | --- | --- | --- | --- | --- |
| ASEZH UPM 包 | Git 仓库 | 直接推送 `main` | Package Manager Git URL | remote SHA 已核对；无 tag/Release/签名 | 部分已验证 |
| Editor 程序集 | `ASEZH.Editor.dll`（导入生成） | 团结脚本编译 | local file UPM | 团结 2022.3.61t9 批处理成功 | 已验证到隔离项目 |
| 汉化后 ASE | 目标工程源码变体 | 安装器原地改写 | 用户工程内 | 无可归档产物或哈希清单 | 未验证 |

## 平台支持

| 平台/版本 | CPU/运行环境 | 已声明 | 可构建 | 已测试 | 已发布 | 证据/限制 |
| --- | --- | --- | --- | --- | --- | --- |
| Unity 2019.4 | Editor | 是 | 未验证 | 未验证 | Git URL 可取但未验 | 本机无 2019.4；这是 package 最低版本声明 |
| Unity 2021.3.7f1c1 | macOS | 隐含支持 | 未验证 | 否 | 同上 | 隔离创建进程退出 143，不能算失败或通过 |
| 团结 2022.3.61t9 | macOS Apple Silicon | README 声明团结支持 | 是 | 包编译与自测通过 | Git URL 可取 | 未含真实 ASE、UI 或 Shader 链路 |
| Windows / 其他 Unity 平台 | Editor | Unity 跨平台隐含 | 未验证 | 未验证 | 未验证 | 无设备/runner；Editor-only 包不等于跨平台已验 |

## 上帝文件与模块隔离

### 文件行数门禁

- 门禁：仓库没有 `.project-architect.json`，本次使用技能默认分类阈值：普通生产代码预警 250 行、硬上限 400 行；方法预警 40 行、硬上限 60 行；测试 400/600，配置 160/200。
- 扫描范围 / 排除范围：152 个项目文件，未截断；排除 `.git`、Library、Temp、build、vendor 等采集器标准目录。5399 行词典 JSON 属数据资产，未按生产 C# 上帝文件处理，但已单独校验结构。

| 文件 | 行数 | 门禁结果 | 核心链路 | 建议 | 问题 ID |
| --- | ---: | --- | --- | --- | --- |
| `Editor/ASELocale.cs` | 497 | 不通过 | 是 | 按加载/查找、IMGUI 适配、搜索、自测拆分，保持 public facade | AUD-SIZE-001 |
| `Editor/Installer/ASEZHPatcher.cs` | 947 | 不通过 | 是 | 先建立回归，再分离 catalog、target resolver、纯文本 transform、事务写入 | AUD-SIZE-001 |
| `Editor/Installer/ASEZHInstallerWindow.cs` | 88 | 正常 | 是 | 保持薄 UI，仅消费结构化结果 | 不适用 |

### 上帝文件候选

| 文件或逻辑类型族 | 规模/职责 | 扇入/扇出或共享状态 | 修改/回归证据 | 结论 | 拆分边界 |
| --- | --- | --- | --- | --- | --- |
| `ASEZHPatcher` | 947 行；目录、发现、变换、写入、撤回、修复 | 调用 AssetDatabase、IO、Regex，改写至少 7 类 ASE 文件 | 0.0.3-0.0.7 连续 5 个版本均涉及安装器修复 | 已确认上帝文件 | `PatchCatalog`、`AseTargetResolver`、`PatchTransforms`、`PatchTransaction` |
| `ASELocale` | 497 行；状态、加载、API、IMGUI、搜索、自测 | 静态共享表和 EditorPrefs；被所有注入点调用 | 初始化后仅少量变更，回归半径中等 | 候选 | 保留 `ASELocale` facade，内部拆数据加载与显示适配 |

### 模块隔离风险

| 问题 ID | 功能 | 当前分布 | 耦合点 | 连带回归 | 建议边界 |
| --- | --- | --- | --- | --- | --- |
| AUD-TARGET-001 | ASE 源码适配 | 一个静态 patcher 跨多文件 | 每步重新全局 `FindAssets`，没有会话 target | 不同 ASE 副本/PackageCache 可能被混合写入 | 先解析并确认唯一 `AseInstallation`，全部变换只接受其根目录 |
| AUD-SIZE-001 | 补丁生命周期 | catalog 与特殊迁移混在 947 行 | 状态字符串、正则、文件 IO 互相穿透 | 任一新锚点可能影响 apply/remove/报告 | 纯变换与 IO 事务解耦，状态使用受控类型 |

## 项目成熟度

等级：`L0 缺失`、`L1 临时`、`L2 可重复`、`L3 标准化`、`L4 可度量`。

| 维度 | 等级 | 当前机制 | 证据 | 主要缺口 |
| --- | --- | --- | --- | --- |
| 需求 | L2 | OpenSpec 当前 spec + change | strict 5/5 | 已完成 change 未归档，基线文本漂移 |
| 架构 | L1 | README 与简短 architecture/hook docs | 静态文档 | 无模块契约/target/transaction 边界，严格架构门禁失败 |
| 构建 | L1 | 依赖人工 Unity/团结导入 | 本次团结批处理通过 | 无仓库脚本、最低版本矩阵和干净 CI |
| 测试 | L1 | 生产代码内 `RunSelfTests()` | 本次 1079 entries 自测通过 | 不测 patcher、真实 ASE、UI、Shader 等价 |
| CI | L0 | 无 | GitHub workflow=0 | 无提交/发布门禁 |
| 发布 | L1 | 直接推送 `main`，手工版本/changelog | 0.0.2→0.0.7 连续 | 无不可变版本引用、artifact/hash/后验收 |
| 回滚 | L0 | `RemoveAll()` 与文档备份建议 | 合成撤回失败 | 无精确 preimage、事务、恢复演练 |
| 环境 | L1 | README 声明 Unity 2019.4+ | 团结 2022.3 已验 | 实际支持的 Unity/ASE 矩阵未冻结 |
| 数据 | L2 | 5 表词典与 overlay-only 限制 | 1079 条静态一致性通过 | 无仓库持久 schema/lint/覆盖率门禁 |
| 文档 | L2 | README、架构、钩子、升级说明 | 主流程较完整 | 4 处菜单文案漂移；证据/版本矩阵不足 |
| 依赖 | L2 | 无第三方运行依赖、Editor-only asmdef | manifest/asmdef 已验 | 目标 ASE 兼容性属于未版本化外部契约 |
| 可观测性 | L1 | Console 加载日志、每项 patch 结果 | 静态已验 | 无结构化 session ID、备份路径、前后哈希与机器可读报告 |
| 安全 | L1 | 确认框、fail-open 翻译 | 无密钥/网络/个人数据 | 文件写入信任边界缺 target/backup/rollback 防护 |
| UI/无障碍 | L0 | IMGUI 文本状态和颜色 | 仅源码 | 无本次有效截图、键盘/焦点/对比度/缩放证据 |

- 总体成熟度与依据：L1。规格与数据已接近 L2，但核心用户旅程“发现 ASE→应用→验证→撤回→恢复”在回滚、测试、CI 和发布证据上仍由个人手工兜底，主链路最低门禁决定总体等级。

## 回归验证机制

| 检查项 | 当前机制 | 自动化/CI | 覆盖 | 证据 | 缺口 |
| --- | --- | --- | --- | --- | --- |
| 本地化核心 | `RunSelfTests()` | 菜单手动；本次批处理调用 | 词典查找、clone、平台 fail-open、搜索样例 | 团结 marker `ASEZH_AUDIT_OK entries=1079 patches=17` | 不遍历全部词条与真实控件 |
| 钩子安装/撤回 | Installer scan/status | 无 | 17 个 catalog 静态路径 | 本次合成 apply 成功；严格 remove 失败 | 无永久 Tests、真实 ASE fixtures、故障注入 |
| UI/Shader | README 人工清单 | 无 | 声明 6 项 | 未执行 | 无截图、dirty/live、Shader hash、跨平台证据 |
| 发布前回归 | OpenSpec strict、手工版本 | 无 CI | 规格与版本文本 | strict 5/5、版本连续 | 不阻断直接推送 `main` |
| 失败留痕、Test-Fix Loop、回滚与复盘 | Changelog 记录结果 | 无 | 版本级描述 | 5 次安装器热修可追溯 | 缺失败测试先行、REG 目录和恢复演练 |

## OpenSpec 规范管理

| 状态 | 结论 | 证据 |
| --- | --- | --- |
| 工具、项目配置、实际使用、规范一致性、strict 校验 | 部分 | OpenSpec 1.7.0 已安装，项目配置存在，5/5 strict 通过；`chinese-menu-and-uninstall-hooks` 显示 Complete 但未 archive；`package-versioning` 当前版本场景仍固定 0.0.2，与 0.0.7 漂移 |

## CodeGraph 使用情况

| 状态 | 结论 | 证据 |
| --- | --- | --- |
| 安装、初始化、索引新鲜度、实际使用、更新机制 | 降级 | CLI 存在，但 `codegraph status` 为 `Not initialized`；遵守只读审计边界未执行 init，以 `rg`、源码调用点、Git history 和架构检查器替代，不宣称依赖图/循环证据 |

## 文档增量与变更留档

| 变更类型 | 载体/机制 | 增量历史 | 关联键 | 字段完整性 | 抽样证据与缺口 |
| --- | --- | --- | --- | --- | --- |
| 需求变更 | `openspec/changes` | 有 | capability/name | 部分 | proposal/design/tasks/spec delta 齐；未使用 FR/CR/REG/REL 稳定 ID，最新 change 未归档 |
| 技术变更 | change design + Git commit | 有 | change 名/commit | 部分 | 设计记录取舍；缺 ADR、回归 ID、发布/回滚证据链接 |
| 文件修改记录 | Git + CHANGELOG | 有 | commit/version | 部分 | 0.0.2-0.0.7 连续；缺 CI artifact、目标 ASE 版本、运行日志与 tag |

## 功能模块闭环度

- 模块统计：发现数：4；已审计数：4；未覆盖数：0
- 清单校准：自动模块发现为 0；人工补充 `FM-DISPLAYLOCALE`、`FM-DICTIONARYDATA`、`FM-ASEHOOKINSTALLER`、`FM-UPMDELIVERY`，因为仓库采用 Unity `Editor/` 与顶层 UPM 结构而非采集器默认 `src/modules`。

| 模块 ID | 模块/能力 | 需求/入口 | 主路径/关键链路 | 数据/状态闭环 | 异常/恢复 | 日志/可观测性 | 回归/验收 | 发布证据 | 结论 | 问题 ID |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| FM-DISPLAYLOCALE | 显示层本地化引擎 | `display-locale`；`FE-CANVASLANGTOGGLE`、两个菜单 | JSON/Prefs→lookup/clone→ASE IMGUI | 内存表与语言偏好可回读 | 缺词典/异常 fail-open；未验真实 ASE | 加载计数日志 | 团结自测通过；Shader/dirty 未验 | UPM main 可导入 | 部分闭环 | AUD-FLOW-002 |
| FM-DICTIONARYDATA | 词典与用户覆盖数据 | `display-locale`；Reload 菜单 | 主 JSON→5 tables；overlay 仅覆盖已有键 | 1079 条、无空/重/非法表 | 解析异常告警并保留原文 | entries/collisions/tables | 本次 jq 校验；无持久 CI/全 UI 覆盖 | 包内 JSON 已远端发布 | 部分闭环 | AUD-FLOW-002 |
| FM-ASEHOOKINSTALLER | ASE 显示钩子安装与撤回 | `ase-hooks-installer`；Installer 菜单/按钮 | FindAssets→文本变换→文件覆盖→Refresh | 每项状态返回；无 session/目标一致性 | mismatch 可停；撤回已复现残留 | UI 列出状态/Detail | 合成 apply 通过、strict remove 失败 | 0.0.7 已在 main | 断链 | AUD-FLOW-001 |
| FM-UPMDELIVERY | Git UPM 包交付 | `package-versioning`；Git URL | package/meta→Git main→UPM resolve→Editor compile | 版本/changelog 0.0.7 一致 | 无不可变回退版本与发布演练 | Git commit/changelog | 团结本地 UPM 通过；无 CI/matrix | remote main SHA 已核对，无 tag/Release | 部分闭环 | AUD-FLOW-002 |

### 前端功能入口闭环

- 入口统计：发现数：8；已审计数：8；未覆盖数：0
- 清单校准：自动发现 4 个 Unity `MenuItem`；人工补充画布语言 Toggle 与 Installer 的重新扫描/应用/移除 3 个按钮。未把纯信息标签、滚动容器和确认框重复计为顶层入口。

| 入口 ID | 页面/入口与类型 | 条件/权限 | 目标/handler | 状态覆盖 | 返回/恢复 | 测试/运行证据 | 结论 | 问题 ID |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| FE-CANVASLANGTOGGLE | ASE 画布 `中文/EN` Toggle | 语言钩子已接入 | `DrawLanguageToggle` | 源码含中/英文状态；真实 hover/focus 未验 | 再点可切回；dirty 恢复未验 | 自测英语 fail-open；无画布截图 | 未验证 | AUD-FLOW-002 |
| FE-E82CAA2DE7 | `Window/ASEZH/重新加载词典` 菜单 | Editor 可加载包 | `ReloadMenu` | 成功/错误 dialog 有分支 | 可重复执行 | 方法路径静态已验，未真实点击 | 部分闭环 | AUD-FE-002 |
| FE-69722510DC | `Window/ASEZH/运行本地化测试` 菜单 | Editor 可加载包 | `RunLocaleTestsMenu` | 成功/错误 dialog 有分支 | 可重复执行 | 批处理直接调用自测通过，菜单未点击 | 部分闭环 | AUD-FE-002 |
| FE-12ED49E169 | `Window/ASEZH/接入 Amplify Shader Editor` 菜单 | Editor 用户 | 打开 Installer | 空/ready/mismatch/missing 由结果区显示 | 窗口可关闭 | 无可接受 UI 截图 | 未验证 | AUD-FE-002 |
| FE-F2B193CF59 | `Window/ASEZH/移除汉化补丁` 菜单 | 用户确认 | `RemoveAll` | 仅汇总 removed 数 | 取消可返回；执行后恢复不可靠 | 合成撤回残留已复现 | 断链 | AUD-FE-001 |
| FE-INSTALLERRESCAN | Installer `重新扫描` 按钮 | 窗口打开 | `Scan` | 列出 applied/ready/mismatch/missing | 可再次扫描 | 静态 handler；无截图 | 部分闭环 | AUD-FE-002 |
| FE-INSTALLERAPPLY | Installer `应用可自动补丁` 按钮 | 用户确认 | `ApplyAll` | 每项状态；无事务汇总 | 取消可返回；失败恢复不足 | 合成 apply 通过，真实 ASE 未验 | 部分闭环 | AUD-FE-002 |
| FE-INSTALLERREMOVE | Installer `移除汉化补丁` 按钮 | 用户确认 | `RemoveAll` | 每项状态；假成功可显示 | 取消可返回；执行后恢复不可靠 | 合成撤回残留已复现 | 断链 | AUD-FE-001 |

## 交互流程与 UI

### Product Design 参与情况

| 检查项 | 结论 | 证据/限制 |
| --- | --- | --- |
| 历史参与证据、本次调用、当前截图、视觉结论等级 | 本次已调用但 UI 审计阻塞 | Product Design 要求以本次实际截图为证据；CUA 将同 bundle ID 的临时 2022.3.61t9 实例解析到用户现有 `FlymeAuto3Test`，为保护用户工程未点击、未截图、已关闭自己启动的 PID 10931 |

### 关键流程证据

| 步骤 | 用户目标/操作 | 截图或运行证据 | 健康度 | UX/可访问性问题 | 证据限制 |
| --- | --- | --- | --- | --- | --- |
| 01 | 从 Window 菜单打开接入窗口 | 源码入口已对账，无可接受截图 | 有风险 | 可发现性依赖菜单；键盘与焦点未验 | 实例歧义，未操作现有用户工程 |
| 02 | 扫描并理解 applied/ready/mismatch/missing | 源码显示文本状态并辅以颜色 | 有风险 | 文本不只靠颜色是优点；列表长时层级/对比度未验 | 无真实 ASE 数据与截图 |
| 03 | 确认并应用补丁 | 合成批处理 apply 成功 | 有风险 | 确认框有风险提示，但未给备份路径/统一成功标准 | 非真实 UI/ASE |
| 04 | 撤回并恢复 | 合成严格回归失败 | 阻塞 | UI 会统计假 `removed` 并提示成功 | 已确认逻辑缺陷，无截图 |

### UI 检查

| 页面/区域 | 层级与清晰度 | 一致性/状态 | 响应式 | 键盘/焦点/语义 | 问题 ID |
| --- | --- | --- | --- | --- | --- |
| InstallerWindow | 源码显示根目录、主操作、逐项结果与 HelpBox，信息结构合理 | applied/ready/mismatch/missing 有文本；成功汇总语义不可靠 | IMGUI 滚动存在，缩放未验 | 未验证 | AUD-FE-001、AUD-FE-002 |
| 画布语言 Toggle | 单一入口符合 spec，21px 高 | 中/英状态有文字与颜色 | 固定 46×21，DPI/拥挤未验 | 焦点、目标尺寸、读屏未验 | AUD-FE-002 |

## 专项工程审计

| 专项领域 | 状态 | 核心证据 | 主要风险/缺口 | 问题 ID |
| --- | --- | --- | --- | --- |
| 安全与隐私 | 已验证 | 无网络、认证、密钥或用户数据；仅敏感字段名扫描命中规范文档 | 主要信任边界是对消费者源码的原地写入，缺单根锁定/备份/事务 | AUD-TARGET-001 |
| 性能与资源 | 推断 | 1079 条词典一次加载；`TranslateArray` 每次中文调用分配；`UseChinese` 每次读 EditorPrefs | 无 OnGUI GC/CPU、词典加载时延或大图节点压力测试 | AUD-RISK-001 |
| 可观测性与运维 | 已验证 | 加载计数、逐补丁状态和 Detail | 无 session ID、目标根、前后哈希、备份路径、机器可读结果；假成功状态损害可信度 | AUD-FE-001 |
| 依赖、供应链与许可证 | 已验证 | 无第三方包依赖；MIT LICENSE 存在；GitHub license API 为 NOASSERTION | 无 SBOM、tag/Release/哈希/签名；ASE 外部契约未版本化 | AUD-FLOW-002 |
| API、数据兼容与迁移 | 已验证 | clone/index 语义与 fail-open 有 spec/selftest；补丁修复迁移逻辑存在 | 撤回非对称、全局目标选择、无精确 preimage | AUD-FLOW-001、AUD-TARGET-001 |
| 无障碍与国际化 | 未验证 | 文本状态与中英 Toggle 可从源码确认 | 无本次截图、键盘、焦点、读屏、对比度、缩放、长文案验证 | AUD-FE-002 |
| 构建可复现与产物完整性 | 部分 | remote SHA、Git archive、meta、团结 UPM 导入与自测 | 无 CI/min-version/Windows/真实 ASE/tag/Release/hash；Unity 2021 尝试无结论 | AUD-FLOW-002 |

## 增量审计对比

- 基线报告：不适用；证据采集未发现历史 `*-project-audit-report.md`，本次建立首个可比较基线。

| 分类 | 问题 ID | 基线 -> 当前 | 证据/说明 |
| --- | --- | --- | --- |
| 不适用 | 全部 | 无历史 -> 当前基线 | 后续审计应使用本报告比较新增、解决、复发、改善和未变化 |

## 阻塞项

| 问题 ID | 阻塞内容 | 影响范围 | 证据 | 解除条件 |
| --- | --- | --- | --- | --- |
| AUD-BLOCK-001 | 无可用于本次自动化的真实 ASE 版本夹具/授权 target project | 安装、撤回、Shader、搜索、画布主链路 | 仓库不含 ASE；本次合成夹具不能替代产品 | 提供可测试的 ASE 1.9.x 工程或自托管 runner，并允许只改测试副本 |
| AUD-BLOCK-002 | UI 自动化无法唯一绑定新临时团结实例 | UI/无障碍截图 | CUA 返回用户现有 `FlymeAuto3Test` 窗口 | 关闭冲突实例或提供可唯一识别的独立应用/测试机窗口 |
| AUD-BLOCK-003 | 本机没有 Unity 2019.4，Unity 2021.3 创建尝试退出 143 | 最低版本/跨引擎声明 | 团结 2022.3 成功，其他版本无有效结果 | 在干净 runner 安装声明矩阵并执行相同 UPM/编译/E2E 门禁 |

## 潜在风险

| 问题 ID | 风险 | 触发条件 | 影响 | 概率 | 应对建议 |
| --- | --- | --- | --- | --- | --- |
| AUD-RISK-001 | OnGUI 高频翻译产生 EditorPrefs 调用和数组分配 | 大型图、频繁 repaint、多个 patched dropdown | GC/编辑器卡顿 | 中 | 先 profiler 量化，再决定缓存语言状态/显示数组；无数据不先优化 |
| AUD-RISK-002 | 用户覆盖文件位于 Git PackageCache 时随更新丢失 | 按 README 在包 `Editor/` 旁复制 override | 自定义译文丢失 | 中 | 明确可持久位置和迁移策略，并在真实 Git UPM 更新中验证 |
| AUD-RISK-003 | 多份或裁剪 ASE 导致跨根混写 | 工程同时存在 Assets/PackageCache/备份副本 | 编译失败或修改错误插件 | 中高 | 由 `AUD-TARGET-001` 的单根解析和确认消除 |

## 问题详情

### AUD-FLOW-001 撤回 Search 过滤钩子存在可复现残留

- 状态：开放
- 证据状态：已验证
- 严重程度：S1
- 优先级：P0
- 影响范围：所有由 `palette-build-list` 使用无大括号 `if` 形式接入的 ASE 版本；执行 Remove 后可能无法编译
- 证据：应用逻辑替换条件表达式不要求 `{`（`ASEZHPatcher.cs:574-627`），撤回逻辑只在匹配条件后看到 `{` 才恢复，却在未变化时返回 `removed`（`:523-567`）。团结合成回归留下 `PaletteParent.cs:8` 的 `ASELocale.MatchesSearch`，随后 asmdef 已恢复空 references；严格 probe 报 `post-remove: palette-build-list=applied` 并退出 4。
- 根因判断：Apply 与 Remove 使用不对称语法假设；状态模型把“未找到可撤回片段”误当作已撤回，且撤回后没有残留 `ASELocale` 校验。
- 优化做法：保留失败夹具；让 transform 返回明确 changed/not-found/mismatch；按真实匹配保存 preimage；Remove 后扫描目标根所有受管文件的残留引用，未清零不得移除 asmdef 或报告成功。
- 技术路径：复用 C#/Unity Test Framework；优先纯文本变换单测，再做隔离 AssetDatabase 集成测试；不要增加通用正则依赖。
- 验收标准：有/无大括号、LF/CRLF、tab/space 夹具均完成 apply→remove 字节级或经批准规范化后的等价恢复；残留引用时状态为失败且 asmdef 引用保留或整体回滚。
- 验证方式：团结/Unity EditMode 测试；对每个受管文件比较 pre/post SHA-256；故障注入后验证恢复。
- 回滚/降级：修复交付前禁用或隐藏 Remove 入口并明确使用备份恢复；若新事务失败，恢复完整 preimage 和旧版本包。

### AUD-TARGET-001 补丁目标和写入过程缺少安全事务边界

- 状态：开放
- 证据状态：已验证
- 严重程度：S1
- 优先级：P0
- 影响范围：存在多份 ASE、裁剪文件、PackageCache ASE、只读文件或中途写入失败的消费者工程
- 证据：`FindFile` 对每个文件独立执行全局 `AssetDatabase.FindAssets` 并取首个同名尾缀（`ASEZHPatcher.cs:192-214`）；`ApplyAll/RemoveAll` 无预检会话（`:216-245`）；多处直接 `File.WriteAllText`，无备份、临时文件、前后哈希或 catch/rollback。
- 根因判断：实现从单机单副本的脚本演进而来，缺少 `AseInstallation`/`PatchPlan`/`PatchTransaction` 一等模型。
- 优化做法：一次解析唯一根并展示确认；拒绝跨根、不可写 PackageCache 与歧义；先生成完整计划、读取 preimage/哈希和验证全部变换，再写临时文件并原子替换；任一步失败恢复全部 preimage。
- 技术路径：现有 C#、AssetDatabase 和 `System.IO`；不引入新依赖。备份放目标工程可持久目录并在 UI 返回精确路径，记录 session ID、目标根、版本和 SHA-256。
- 验收标准：双 ASE 根、缺文件、只读、写到一半失败和 PackageCache 场景均 fail closed；没有跨根写入；所有失败都能恢复到执行前哈希。
- 验证方式：纯 resolver 测试 + 隔离 Unity 工程故障注入 + 文件哈希对账 + 重启后重新扫描。
- 回滚/降级：默认只允许 Scan；事务能力未通过前要求用户先提交/复制 ASE，并在任何歧义时拒绝 Apply/Remove。

### AUD-FLOW-002 核心产品承诺没有真实 ASE 回归与发布门禁

- 状态：开放
- 证据状态：已验证
- 严重程度：S1
- 优先级：P0
- 影响范围：中英文显示、节点搜索、下拉语义、Shader 等价、dirty/Live 行为、ASE 版本兼容和所有消费者更新
- 证据：仓库无 Tests 目录、测试 asmdef、`.github/workflows` 或构建脚本；`RunSelfTests()` 只覆盖少数翻译/搜索断言（`ASELocale.cs:435-495`）。0.0.3-0.0.7 五个连续版本均为安装器回归热修。远端无 tag、Release、Actions 或分支保护。
- 根因判断：验证资产停留在可交互菜单自测和 README 清单，未把真实 ASE 主链路转成可重复、可阻断发布的 REG。
- 优化做法：建立不提交专有 ASE 源码的自托管/可配置夹具；覆盖 install/compile/search/toggle/shader-hash/remove/recompile；把字典 schema、OpenSpec、版本、meta、目标矩阵和 artifact hash 合并为最小发布门禁。
- 技术路径：Unity Test Framework、batchmode、自托管 runner 或维护者本机脚本；使用受许可的外部 ASE 测试副本，通过路径配置接入，不复制到公开仓库。
- 验收标准：至少一个已声明 ASE 版本在最低 Unity、主力 Unity 和团结矩阵通过；中文/EN Shader SHA-256 一致；Toggle 不置 dirty/Live；apply/remove 后均零编译错误；失败阻止发布引用更新。
- 验证方式：记录每个引擎/ASE/OS 版本、命令、退出码、测试 XML、日志摘要、前后哈希和截图；在干净消费者工程按不可变版本引用复验。
- 回滚/降级：门禁建成前把兼容性表述限定为“已验证团结 2022.3 包自身导入”，不扩大发布声明；失败版本不发布。

### AUD-FE-001 撤回入口会向用户报告虚假成功

- 状态：开放
- 证据状态：已验证
- 严重程度：S1
- 优先级：P0
- 影响范围：菜单和 Installer 内两个“移除汉化补丁”入口及其成功提示
- 证据：窗口把所有 `Status == "removed"` 计入成功数并提示“已撤回”（`ASEZHInstallerWindow.cs:23-30`）；`RemovePaletteBuildList` 未变化也返回 `removed`（`ASEZHPatcher.cs:559-563`），本次严格 probe 证明钩子仍为 applied。
- 根因判断：UI 汇总依赖宽松字符串状态，没有区分 restored、already-clean、skipped、mismatch、failed，也没有最终健康检查。
- 优化做法：使用受控结果类型与 session summary；只把实际恢复且最终 residual scan 通过计为成功；对 partial/failed 显示醒目结论、受影响文件和备份路径。
- 技术路径：保持现有 IMGUI；由 patch transaction 返回结构化 summary，Window 不自行推断结果。
- 验收标准：复现样本不再显示成功；错误消息说明残留文件、asmdef 处理、恢复动作和备份路径；取消操作零写入。
- 验证方式：EditMode 结果模型测试 + 真实窗口截图 + 键盘执行/取消验证。
- 回滚/降级：在结构化 summary 完成前，不显示总成功 dialog；保留逐项结果并要求人工复查。

### AUD-SIZE-001 两个核心文件超限且补丁器形成上帝文件

- 状态：开放
- 证据状态：已验证
- 严重程度：S2
- 优先级：P1
- 影响范围：本地化 API、17 个补丁、目标发现、恢复、后续 ASE 适配和回归定位
- 证据：严格架构检查报告 `ASELocale.cs` 497 行、`ASEZHPatcher.cs` 947 行，均超过默认 400 硬上限；`Catalog()` 约 159 行；Git 历史显示后者连续高频修改。
- 根因判断：初版把所有逻辑集中以便快速交付，后续特殊迁移不断追加，没有在回归资产建立后拆职责。
- 优化做法：先完成 P0 回归，再按 catalog/target/transform/transaction 拆 patcher；`ASELocale` 保留兼容 facade，内部拆加载和 IMGUI helper；配置 `.project-architect.json` 的 Unity 分类/排除项。
- 技术路径：同 asmdef 内部类与文件拆分，不改变公共入口；每次拆分保持行为与二进制调用面。
- 验收标准：普通生产文件不超过批准阈值，方法不超过 60 行或有带到期日例外；依赖方向单向；所有 P0 回归不变绿。
- 验证方式：严格架构检查、编译、EditMode/E2E、public API diff 和 `git diff --check`。
- 回滚/降级：逐模块小提交；出现兼容差异时回退最近拆分，不回退已建立的回归测试。

### AUD-GOV-001 OpenSpec 生命周期和版本基线已发生漂移

- 状态：开放
- 证据状态：已验证
- 严重程度：S2
- 优先级：P1
- 影响范围：后续代理/维护者判断当前批准语义、版本发布和变更完成状态
- 证据：`openspec list` 显示 `chinese-menu-and-uninstall-hooks` Complete，代码和 0.0.7 已在 remote main，但 change 未归档；`openspec/specs/package-versioning/spec.md:24-29` 仍要求当前发布 section 为 0.0.2。
- 根因判断：strict 校验只验证结构/语法，没有校验“完成 change 必须 archive”或动态版本事实；固定版本场景被误当长期基线。
- 优化做法：通过新的治理 change 把固定“当前版本”改为通用一致性/连续性规则；确认 delta 已同步后归档完成 change；增加版本/active-complete/CHANGELOG 检查与最小追溯记录。
- 技术路径：复用 OpenSpec 1.7.0、现有 package/changelog 和轻量脚本；不机械初始化 11 份与小型仓库不相称的文档。
- 验收标准：`openspec list` 无已交付 Complete active change；strict 通过；spec 不再声称 0.0.2 是当前版本；package/README/changelog/远端引用一致。
- 验证方式：OpenSpec strict、版本一致性脚本、Git history 抽样、审计 strict 检查。
- 回滚/降级：若归档 delta 不符合当前实现，先停止归档并修正 change；保留历史 archive，不改写旧记录。

### AUD-FE-002 Editor UI 与无障碍没有本次可复核证据

- 状态：开放
- 证据状态：未验证
- 严重程度：S2
- 优先级：P1
- 影响范围：Installer 窗口、4 个菜单、3 个按钮、画布 Toggle 的可发现性、状态理解与输入可达性
- 证据：本次尝试启动独立临时实例；CUA 返回用户现有 `FlymeAuto3Test` 窗口，按 Product Design 截图真实性要求停止，未操作、未保留错误窗口截图。源码只能证明控件存在，不能证明布局、焦点、对比度或缩放。
- 根因判断：没有隔离的可唯一识别 UI 测试工程/截图基线，也没有无障碍验收记录。
- 优化做法：在专用测试机/独立用户会话运行真实 ASE 流程，按步骤保存截图和焦点/键盘/缩放记录；覆盖 ready、mixed、mismatch、failure、success 和 remove-failure 状态。
- 技术路径：Product Design audit + 团结/Unity 原生 UI；截图只作为视觉证据，键盘/语义另做实操记录。
- 验收标准：8 个入口均有状态与恢复结论；核心四步有已检查截图；Tab/Space/Enter/Escape、焦点可见、150%-200% 缩放、颜色之外的状态表达通过。
- 验证方式：真实 GUI 录制/截图、人工键盘步骤、目标尺寸/对比度检查，并与测试日志绑定同一 session ID。
- 回滚/降级：UI 证据不通过时不改变逻辑结论；先修阻断性反馈和焦点问题，再重录整条流程。

### AUD-DOC-001 操作文档仍引用旧英文菜单

- 状态：开放
- 证据状态：已验证
- 严重程度：S3
- 优先级：P2
- 影响范围：按 README 和适配指南执行 Reload/Run Tests/Install 的用户
- 证据：代码菜单为中文；`README.md:172,247` 仍写 `Reload Dictionary`、`Run Locale Tests`，`docs/adapt-ase-version.md:13,41` 仍写 `Install into Amplify Shader Editor`、`Run Locale Tests`。
- 根因判断：0.0.7 菜单中文化只更新了主要安装段落，未对全仓库路径做一致性搜索。
- 优化做法：统一为当前中文路径，必要时括号保留英文解释；增加菜单常量/文档词条一致性检查。
- 技术路径：最小文档修订与 `rg` 校验，不改产品行为。
- 验收标准：用户文档不再把不存在的英文菜单作为点击路径；4 个实际 MenuItem 与文档一一对应。
- 验证方式：`rg` 全仓库检索旧路径，真实 Editor 菜单截图抽样。
- 回滚/降级：文档改动可独立回退；不影响代码或版本语义。

## 验证记录

| 命令/检查 | 目的 | 结果摘要 | 是否通过 |
| --- | --- | --- | --- |
| `audit_project.py collect ... --mode deep --supplement ...` | 全仓库、FM/FE 证据 | schema 1.1；152 indexed；4 FM；8 FE；未截断 | 是 |
| `check_project_architecture.py --check-docs --check-traceability --check-fitness --strict` | LOC/架构/治理门禁 | 退出 1；2 个 LOC errors，11 个完整 Project Architect 基线文件缺失；无 fitness error | 否（发现已入报告） |
| `openspec validate --all --strict` | 规格结构 | 5 passed，0 failed | 是 |
| JSON/词典 jq 校验 | 包、asmdef、词典 schema | 1079 条；5 表；0 空项、0 重复键、0 非法表 | 是 |
| 团结 2022.3.61t9 local UPM + `ASEZHAuditProbe.Run` | 包导入、编译、自测 | 退出 0；`ASEZH_AUDIT_OK entries=1079 patches=17` | 是 |
| 合成 `Scan→Apply→Scan→Remove→Scan` 宽松 probe | 当前 catalog 可执行性 | 退出 0，但复查发现撤回后仍有 `ASELocale.MatchesSearch` | 否（probe 断言不足） |
| 收紧后的合成撤回 probe | 固化已发现回归 | 退出 4；`post-remove: palette-build-list=applied` | 否（按预期复现缺陷） |
| Unity 2021.3.7f1c1 隔离项目创建 | 跨引擎兼容 | 进程退出 143，日志停在默认包导入，无 ASEZH 结论 | 未运行完成 |
| `git diff --check` | 工作区文本完整性 | 退出 0 | 是 |
| Git/GitHub 只读核对 | 远端交付状态 | main SHA 一致；0 tag、0 Release、0 workflow、branch unprotected | 部分 |
| `codegraph status` | 依赖图能力 | CLI 存在但项目 Not initialized；未 init | 未运行索引 |
| Product Design + CUA | 真实 UI/无障碍 | 绑定到用户现有工程，按边界停止，无可接受截图 | 未运行完成 |

## 计划外发现

- `gh api` 识别仓库 LICENSE 为 `NOASSERTION`，尽管仓库内是标准 MIT 文本；这不影响本次核心结论，建议发布治理任务中确认 GitHub license detection/manifest metadata。
- Git archive 包含 OpenSpec、Cursor 命令和完整历史 change 文档，当前 174 项；本次未将包体积视为缺陷，但可在发布门禁建立后评估消费者包噪声。
- `Editor/ASEZHDictionary.user.json.example` 建议复制到包目录旁；对 Git PackageCache 的持久性尚未实测，已列为潜在风险。

## 遗留问题

- 所有 8 个开放问题均未修复；本次只生成报告与任务书。
- 临时证据与隔离项目位于 `/private/tmp`，不是长期发布证明；后续整改需把测试 XML、哈希、截图和目标矩阵保存到仓库约定或 CI artifact。
- 下一次审计建议日期：2026-09-15；若 P0 修复提前完成，应在合并/发布前立即做增量审计。
