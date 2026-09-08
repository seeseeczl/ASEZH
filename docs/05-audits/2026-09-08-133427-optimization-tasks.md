---
id: OPT-2026-09-08-133427
type: optimization-plan
status: in-progress
version: 1
created_at: 2026-09-08T13:34:27+08:00
owner: ASEZH maintainer
related: [AUD-2026-09-08-133427]
supersedes: []
evidence: [docs/05-audits/2026-09-08-133427-project-audit-report.md]
---

# 项目优化任务书

## 执行原则

- 只执行本任务书事项，先 P0，再 P1，再 P2；每次只推进一个任务，不擅自扩大范围。
- 每完成一个步骤立即更新 TODO 和当前进展，不做顺手优化。
- 默认复用现有 C#、Unity/团结 Editor API、OpenSpec 和 GitHub；新增依赖必须说明必要性、替代方案、成本和影响。
- 测试失败执行 Test-Fix Loop；非阻塞的计划外问题只记录。
- 所有产品语义变化先建立/继续 OpenSpec change；面向消费者交付时按现有规则 `package.json +0.0.1` 并更新 CHANGELOG，严格验证后归档。
- 真实 ASE 只能使用获得许可的外部测试副本，不把专有源码、日志敏感值或用户工程内容提交到公开仓库。

## 总目标

- 把 ASEZH 从“可导入、靠人工兜底的源码补丁脚本”提升为“目标唯一、失败可恢复、主链路有真实 ASE 回归、发布证据可复核”的 Editor UPM 包。

## 范围

- `Editor/Installer` 的目标解析、补丁计划、写入/撤回、结果反馈和回归；`ASELocale`/词典相关回归；OpenSpec 生命周期、版本/发布门禁；真实 Editor UI 验收；当前文档菜单路径。

## 非目标

- 不重写 ASE、不复制或公开专有 ASE 源码、不新增语言、不改变 Shader 生成语义、不迁移技术栈、不在未授权时修改远端分支保护/tag/Release，不把 P2 性能猜测提前做成代码优化。

## 问题到任务映射

| 问题 ID | 严重程度 | 优先级 | TODO ID | 处理方式 |
| --- | --- | --- | --- | --- |
| AUD-FLOW-001 | S1 | P0 | P0.1 | 单独保留失败 REG，并纳入可逆事务 |
| AUD-TARGET-001 | S1 | P0 | P0.1 | 与同一安全补丁会话合并 |
| AUD-FE-001 | S1 | P0 | P0.1 | 结果模型和最终残留检查同步修复 |
| AUD-FLOW-002 | S1 | P0 | P0.2 | 建立真实 ASE 回归与发布门禁 |
| AUD-SIZE-001 | S2 | P1 | P1.1 | 在 P0 回归保护下拆分职责 |
| AUD-GOV-001 | S2 | P1 | P1.2 | 修复 OpenSpec 生命周期与事实漂移 |
| AUD-FE-002 | S2 | P1 | P1.3 | 形成真实 UI/无障碍验收记录 |
| AUD-DOC-001 | S3 | P2 | P2.1 | 最小文档一致性修订 |

## TODO

P0 必须完成

- [x] P0.1 交付单根、预检、可回滚且不假报成功的 ASE 补丁事务
- [x] P0.2 建立团结 2022.3.61t9 + ASE 1.9.81 安装/撤回正式发布门禁

P1 应该完成

- [x] P1.1 在回归保护下拆分 `ASEZHPatcher` 和 `ASELocale` 超限职责
- [x] P1.2 关闭 OpenSpec 生命周期与版本/发布追溯漂移
- [-] P1.3 记录真实 UI、无障碍、其他 Unity 与 Windows 观察状态（用户确认不作为团结目标发布阻塞）

P2 可选优化

- [x] P2.1 统一用户文档中的当前中文菜单路径

## 任务详情

### P0.1 交付安全可逆的补丁事务

- 来源问题 ID：AUD-FLOW-001、AUD-TARGET-001、AUD-FE-001
- 依赖：无
- 严重程度：S1
- 目标：一次 Apply/Remove 只作用于用户确认的唯一 ASE 根目录，任意失败恢复执行前内容，且 UI 不再把残留钩子报告为成功。
- 范围：`Editor/Installer/ASEZHPatcher.cs`、`Editor/Installer/ASEZHInstallerWindow.cs`、新增/现有 Editor 测试程序集、对应 OpenSpec change；生产文件不超过 5 个。
- 非目标：不增加新钩子、不扩大支持的 ASE 版本、不改译文、不重做 Installer 视觉设计。
- 涉及文件/模块/符号：`FindAseRoot`、`FindFile`、`ApplyAll`、`RemoveAll`、`RemovePaletteBuildList`、`ReverseAseAssemblyReference`、结果状态模型、Installer 成功汇总。
- 现有技术栈复用：C#、AssetDatabase、System.IO、Unity Test Framework、OpenSpec；保持 `Scan/ApplyAll/RemoveAll` 对外入口，必要时以内部分层兼容。
- 新增依赖：无；测试夹具使用自有最小文本，不包含 ASE 专有实现。
- 技术路径：先把本次无大括号失败样本写成 RED REG；引入 `AseInstallation` 和 `PatchPlan`，在写入前锁定唯一根、检查可写性并计算 preimage/SHA-256；将纯变换与 IO 分开；保存完整备份后临时文件写入/原子替换；失败恢复全部 preimage；Remove 最终扫描残留 `ASELocale` 后才处理 asmdef；用受控状态替代宽松字符串统计。
- 执行步骤：
  1. 创建 OpenSpec change，明确唯一目标、PackageCache 策略、备份目录、事务、状态和回滚语义；把无大括号 remove 样本加入会失败的永久 REG。
  2. 实现一次性 target resolver 和只读 preflight；双根、跨根、缺文件、只读或不可持久 PackageCache 默认 fail closed，并在 UI 展示目标根。
  3. 实现带 session ID、preimage、SHA-256、临时写入和全局回滚的 `PatchTransaction`；所有 transform 先在内存验证，再落盘。
  4. 修复 apply/remove 对称性；残留 `ASELocale`、哈希不符或任何 mismatch 时不得撤掉 asmdef 引用或显示成功。
  5. 覆盖 LF/CRLF、tab/space、有/无大括号、双 ASE 根、部分写入失败、重复执行和重启后扫描；运行同命令直到全绿。
- 验收标准：所有受管文件在 apply→remove 后恢复批准的 preimage 哈希；任意注入失败后全体文件回到执行前哈希；双根零写入；失败 dialog 给出目标、文件、备份路径和恢复结论；本次 `post-remove: palette-build-list=applied` 回归转绿。
- 验证方式：Unity/团结 EditMode 测试 XML、隔离工程 batchmode、故障注入、SHA-256 清单、`openspec validate --all --strict`、`git diff --check`。
- 风险：原子替换和文件权限在非目标 Unity/Windows 环境可能不同；AssetDatabase Refresh 可能打断会话；旧手工钩子无法可靠认领。
- 回滚/降级方案：以完整 preimage 恢复；无法判定归属的手工钩子只报告不修改；新事务未通过矩阵前将 Apply/Remove 降级为 Scan-only。
- 变更留档：OpenSpec CR/requirements/scenarios/tasks；新增 REG ID；CHANGELOG 与 package patch bump（仅面向消费者交付时）；记录 target engine/ASE、commit、测试 XML 和哈希。
- 计划外问题处理规则：记录，不展开；只有阻塞 P0 时暂停并请求确认。

### P0.2 建立团结目标的真实 ASE 主链路回归与发布门禁

> 范围修订（2026-09-08）：用户确认正式发布目标仅为团结引擎 2022.3.61t9 + ASE 1.9.81。旧任务书中 Unity 2019.4、Windows、完整 UI 和 Shader 人工对比改为非阻塞扩展观察项。

- 来源问题 ID：AUD-FLOW-002
- 依赖：P0.1
- 严重程度：S1
- 目标：每次消费者交付前，以团结 2022.3.61t9 和获得许可的 ASE 1.9.81 自动证明安装、Locale 自测、接入、重启编译、撤回、最终编译和 preimage 恢复，失败阻止更新发布引用。
- 范围：测试 harness、可配置外部 ASE fixture、自托管或维护者 runner、最小 GitHub workflow/本地统一命令、发布证据清单；不提交 ASE 本体。
- 非目标：不承诺未执行的平台/ASE 版本，不把合成夹具替代真实 ASE，不自动创建 GitHub Release 或改分支保护，除非维护者另行授权。
- 涉及文件/模块/符号：测试 asmdef/runner、`ASELocale.RunSelfTests`、`ASEZHPatcher` public flow、package/version/meta/OpenSpec gates、发布记录。
- 现有技术栈复用：Unity/团结 batchmode、Unity Test Framework、GitHub Actions（静态门禁）、自托管 runner（真实 ASE）、shell/Python 轻量校验、SHA-256。
- 新增依赖：默认无；若选择第三方测试报告工具，必须先证明 Unity Test Framework 不能满足并单独批准。
- 技术路径：公开 CI 运行无专有资产的字典/schema/OpenSpec/meta/version 静态检查；真实 ASE 在受控本机由路径注入；统一产生引擎/ASE/OS 版本、阶段状态和文件前后哈希，扩展 UI/Shader/平台观察单独记录。
- 执行步骤：
  1. 定义团结 2022.3.61t9 + ASE 1.9.81 正式目标和外部 fixture 协议；其他 Unity 与 Windows 缺失时记为 `not-run` / `unverified`，不得标为通过。
  2. 自动执行 install→Locale 自测→compile→scan→apply→restart compile→remove→restart compile→residual scan→preimage hash。
  3. 增加字典 5 表、重复键、平台名、meta GUID、版本连续、OpenSpec strict、LOC、敏感文件名和 Git archive 清单门禁。
  4. 保存脱敏 artifact 与统一摘要；只在团结目标全部必选项通过时允许更新不可变版本引用，远端保护/tag/Release 另行授权。
  5. 在全新消费者工程按不可变 commit/tag 安装一次，读取 package version，重跑核心 smoke 并记录后验收。
- 验收标准：团结目标全部必选项为 `pass`；真实 ASE apply/remove 后零编译错误并恢复受管 preimage；扩展观察项状态明确；失败 workflow 阻断交付。
- 验证方式：统一脚本退出码、Editor.log 脱敏摘要、阶段/文件哈希清单、GitHub 静态 checks。
- 风险：ASE 许可和团结 runner 可用性；Editor GUI 自动化波动；GitHub 公共 runner 不能持有专有插件。
- 回滚/降级方案：专有 E2E 仅在受控 runner；团结目标 runner 不可用时发布失败，不用合成测试替代；扩展观察缺失不改变目标门禁结论。
- 变更留档：OpenSpec NFR/REG/REL 记录、支持矩阵、workflow/run URL、artifact hash、目标 commit/tag 与回滚版本。
- 计划外问题处理规则：记录，不展开；只有阻塞 P0 时暂停并请求确认。

### P1.1 拆分核心超限职责（已完成）

- 来源问题 ID：AUD-SIZE-001
- 依赖：P0.1、P0.2
- 严重程度：S2
- 目标：在不改变 public API 和产品行为的前提下，把补丁目录、目标解析、纯变换、事务 IO 与本地化加载/IMGUI 适配分离为可独立测试的内部模块。
- 范围：`Editor/Installer` 与 `Editor/ASELocale.cs` 的内部结构、`.project-architect.json` 分类阈值；每张实施任务至多 5 个生产文件、净新增不超过 300 行。
- 非目标：不新增功能、不改翻译、不借拆分重命名公开 API、不全局格式化。
- 涉及文件/模块/符号：`ASEZHPatcher.Catalog`、target resolver、special transforms、IO transaction、`ASELocale` data loader/search/IMGUI helpers。
- 现有技术栈复用：同一 `ASEZH.Editor` asmdef、internal 类、现有静态 facade、现有回归套件。
- 新增依赖：无。
- 技术路径：建立 `PatchCatalog`、`AseTargetResolver`、`PatchTransforms`、`PatchTransaction`；`ASELocale` 保持 facade，内部委托 `LocaleStore` 与 `LocaleGuiAdapters`；依赖只朝纯模型/变换方向。
- 执行步骤：
  1. 记录拆分前 public surface、回归结果、依赖调用点和文件哈希，设定 `.project-architect.json` Unity 分类/排除项。
  2. 逐个提取纯 catalog/transform 与 target/IO，保持每次提交可编译、同测试结果，禁止同时改语义。
  3. 最后拆 `ASELocale` 内部职责并运行真实 ASE E2E、严格 LOC/fitness 和 API diff。
- 验收标准：普通生产文件不超过批准硬上限或有到期 ADR；方法不超过 60 行或有例外；依赖无循环/跨私有边界；P0 全回归与 public surface 不变。
- 验证方式：Project Architect strict、CodeGraph（经授权初始化后）或 compiler/rg 降级、Unity/团结编译、EditMode/E2E、API 清单 diff。
- 风险：静态类初始化时序、团结 2022.3 C# 语言级别、拆分导致 internal 可见性变化。
- 回滚/降级方案：按提取提交逐步回退；保留测试和 target transaction，不做一次性大重写。
- 变更留档：OpenSpec 可标 `skip_specs: true` 的纯重构 change、ADR/例外、模块图、测试和 commit 关联。
- 计划外问题处理规则：记录，不展开；只有阻塞 P0/P1 时暂停并请求确认。

### P1.2 修复 OpenSpec 与发布事实漂移（已完成）

- 来源问题 ID：AUD-GOV-001
- 依赖：P0.2
- 严重程度：S2
- 目标：批准语义、active/archived 状态、当前版本和发布证据互相一致，自动检查能阻止再次漂移。
- 范围：OpenSpec package-versioning/change-governance、当前 Complete change、轻量版本/追溯校验、发布记录；不机械创建完整 Project Architect 目录树。
- 非目标：不改写历史 archive、不删除 1.0.0 退役记录、不在未授权时创建远端发布。
- 涉及文件/模块/符号：`openspec/specs/package-versioning/spec.md`、`openspec/changes/chinese-menu-and-uninstall-hooks`、`package.json`、`README.md`、`CHANGELOG.md`、最小 trace/release evidence。
- 现有技术栈复用：OpenSpec 1.7.0、Git、现有 Markdown/YAML、P0.2 校验脚本。
- 新增依赖：无。
- 技术路径：用新 change 把固定“当前版本 0.0.2”改为动态一致性与连续递增验收；核对 0.0.7 delta 后归档 Complete change；为需求→实现→REG→发布添加最小稳定 ID/证据链接。
- 执行步骤：
  1. 创建治理 change，明确哪些固定历史场景保留、哪些当前事实改为自动读取，并增加失败场景。
  2. 校准并归档 `chinese-menu-and-uninstall-hooks`，验证 main specs 与当前代码一致。
  3. 将版本、active Complete、CHANGELOG、README、package、测试与发布证据校验接入 P0.2 门禁。
- 验收标准：OpenSpec strict 通过；active 列表无已交付 Complete change；当前版本只由 package 事实源驱动且 README/changelog 一致；每次交付能回链 change/REG/commit/证据。
- 验证方式：`openspec list`、`openspec validate --all --strict`、版本脚本、Git history/remote SHA、审计严格校验。
- 风险：归档顺序可能重复合并已手工同步的 delta；固定历史示例与当前规则容易混淆。
- 回滚/降级方案：归档前先 diff；冲突则停止并修 change，不改写 archive；自动检查先 warning 一次，修正基线后再升为阻断。
- 变更留档：治理 CR、归档记录、版本/追溯脚本、timeline 或现有等价记录、CI/run artifact。
- 计划外问题处理规则：记录，不展开；只有阻塞 P1 时暂停并请求确认。

### P1.3 记录真实 UI 与扩展平台观察（非阻塞）

- 来源问题 ID：AUD-FE-002
- 依赖：P0.1、P0.2
- 严重程度：S2
- 目标：真实 Editor/ASE 的画布交互、状态、键盘、缩放和恢复证据在具备环境时可复核，未执行项保持明确状态。
- 范围：Installer window、单一 Window 菜单、画布 Toggle；ready/mixed/mismatch/failure/success/remove-failure；其他 Unity 与 Windows 作为扩展观察环境。
- 非目标：不重新设计 UI、不创建 Figma board（除非维护者另行要求）、不把截图当作完整 WCAG 合规证明。
- 涉及文件/模块/符号：单一 MenuItem、`DrawLanguageToggle`、Installer 主操作与高级/诊断区、结果列表、确认/总结 dialog。
- 现有技术栈复用：Unity/团结 IMGUI、Product Design audit、现有测试工程和 P0 session/result ID。
- 新增依赖：无；若需要截图工具，优先系统/现有 CUA，不安装新软件。
- 技术路径：使用可唯一识别的独立测试机/用户会话；每步先操作再保存并检查截图；视觉证据绑定同一 patch session 和测试日志；另做 Tab/Enter/Space/Escape、focus、缩放和状态文本检查。
- 执行步骤：
  1. 准备不含用户资产的独立真实 ASE 工程，确认 UI 工具绑定到正确窗口并记录版本。
  2. 按打开→扫描→应用→验证→撤回/失败恢复顺序采集截图和交互记录，覆盖 8 个入口。
  3. 检查 150%-200% 缩放、键盘焦点、取消/返回、颜色之外状态、错误恢复，并修复阻断性问题后重录全流程。
- 验收标准：每个入口有结论或明确不适用；四个关键步骤有已检查截图；成功/失败状态不裁切、不只靠颜色；键盘操作和焦点可见；画布 Toggle 不与原控件重叠。
- 验证方式：截图清单、窗口/项目标识、人工步骤记录、session ID、P0 E2E 日志和目标环境矩阵。
- 风险：Editor 版本/DPI 导致布局差异；自动化绑定到错误用户工程；截图可能包含工程名或路径。
- 回滚/降级方案：绑定不唯一立即停止；截图先脱敏工程标识；UI 修复逐项回退且不改核心 patch transaction。
- 变更留档：UI acceptance/REG、脱敏截图、引擎/ASE/OS/DPI、问题→修复→复验记录。
- 计划外问题处理规则：记录，不展开；只有阻塞 P1 时暂停并请求确认。

### P2.1 统一中文菜单操作路径（已完成）

- 来源问题 ID：AUD-DOC-001
- 依赖：P1.2
- 严重程度：S3
- 目标：README 与适配指南只使用当前可点击的 4 个中文 MenuItem 路径。
- 范围：`README.md`、`docs/adapt-ase-version.md` 和必要的文档一致性检查。
- 非目标：不改 MenuItem 代码、不重写整份 README、不改变版本策略。
- 涉及文件/模块/符号：`Reload Dictionary`、`Run Locale Tests`、`Install into Amplify Shader Editor` 旧字符串及对应中文路径。
- 现有技术栈复用：Markdown、`rg`、现有 OpenSpec 菜单要求。
- 新增依赖：无。
- 技术路径：最小替换为当前中文菜单，若需帮助英文读者则用括号说明而不作为点击路径；增加旧路径负向搜索。
- 执行步骤：
  1. 对照 4 个 `MenuItem` 常量和 OpenSpec scenario，更新 README/适配指南的旧英文路径。
  2. 运行全仓库旧字符串搜索和真实菜单抽样，确认没有不存在的点击路径。
- 验收标准：用户文档中的点击路径与代码 4 个 MenuItem 完全一致；旧英文路径搜索只允许出现在历史 archive/变更语境。
- 验证方式：`rg` 检索、Markdown 链接检查、真实 Editor 菜单截图抽样。
- 风险：历史 OpenSpec archive 应保留当时原文，机械替换会破坏历史。
- 回滚/降级方案：只回退当前用户文档，不改历史 archive；发现语义歧义时保留中英对照。
- 变更留档：关联 AUD-DOC-001 的文档提交；若随消费者交付则按版本规则记录，否则明确 docs-only。
- 计划外问题处理规则：记录，不展开；只有阻塞 P2 时暂停并请求确认。

## 进度更新模板

```markdown
## TODO

P0 必须完成
- [*] P0.1 当前任务
- [ ] P0.2 下一个任务

P1 应该完成
- [ ] P1.1 后续任务
- [ ] P1.2 后续任务
- [ ] P1.3 后续任务

P2 可选优化
- [ ] P2.1 后续任务

当前进展：
- 已完成：
- 正在做：
- 下一步：
- 阻塞/风险：
```

## Definition of Done

- [ ] 所有 S0/S1 和 P0/P1 问题均映射到 TODO，且每个 TODO 有完整详情。
- [ ] 已按 `.project-architect.json` 的分类阈值执行 LOC 门禁，排除项、截断和超限项均已记录。
- [ ] P0 完成并通过相关测试、构建或替代验证。
- [ ] 回归覆盖受影响模块和关键链路。
- [ ] OpenSpec 与 CodeGraph 的更新或不适用原因已说明。
- [ ] 需求变更、技术决策和实际修改文件已按现有机制增量留档并关联。
- [ ] 审计范围内每个功能模块均完成“需求→入口→主路径→数据/状态→异常恢复→可观测性→回归→发布证据”闭环对账。
- [ ] 审计范围内每个前端入口均有闭环结论或未验证原因。
- [ ] 七类专项工程审计均有状态，P0/P1 专项问题均映射 TODO。
- [ ] 请求/实际审计模式及降级原因已说明；增量对比已写入报告或说明不适用。
- [ ] UI 任务的 Product Design、视觉证据和可访问性验证情况已说明。
- [ ] 计划外发现已记录但未扩展处理。
