# 0.0.13 原生节点显示兼容性证据

目标包版本：`0.0.13`（`REL-ASEZH-0013`）
变更：`ase-199-native-layout-anchor`
主机：macOS 27.0 arm64

## 问题

ASE 1.9.9 把 `ParentNode.OnNodeLayout` 的签名从 `OnNodeLayout( DrawInfo )` 改成
`OnNodeLayout( DrawInfo, NodeUpdateCache cache = null )`。安装器只忽略空白差异，参数列表不同即判
`mismatch`，预检 fail-closed，用户侧表现为「状态：PreflightRejected / 锚点未命中」，
`native-language-layout` 无法接入。

同一次复验还发现第二个真实缺陷：Unity 会按自己的风格重写 `AmplifyShaderEditor.asmdef`，
把引用数组写成多行，`Remove` 因此认不出自己写入的引用并拒绝撤回。

## 复现

在修复前的仓库快照上，用维护者许可的 ASE 1.9.9.5 副本跑门禁基线阶段：

```
Exception: baseline: native-language-layout=mismatch 锚点未命中，需按 docs/adapt-ase-version.md 手工接入：原生节点显示：native-language-layout
```

修复后同一 fixture 的基线阶段通过。

## 正式发布门禁

| 目标环境 | Locale 自测 | Baseline | Apply | VerifyApplied | Remove | VerifyRemoved | preimage 恢复 | 总结 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Tuanjie 2022.3.61t9 / macOS + ASE 1.9.9.5 | pass | pass | pass | pass | pass | pass | pass | pass |

Tuanjie 2022.3.61t9 + ASE 1.9.9.5 的六阶段全部通过，`apply_remove_preimage_restored=pass`，
Apply → Remove 后每个受管文件按 SHA-256 与接入前完全一致；Apply 后新进程重编译成功，
Remove 后新进程重编译成功，残留扫描为 0。结果：`/private/tmp/asezh-evidence-final/manifest.json`。

同一版本的完整交付门禁（静态检查 + 合成生命周期 + 许可 ASE 生命周期）在 `--release` 模式下为 `pass`：
`/private/tmp/asezh-release-gate-0.0.13/delivery-manifest.json`。

EditMode 回归 23/23：新增用例覆盖两套 `OnNodeLayout` 签名的命中、接入后签名行保持原样、再次扫描为
`applied` 且不误判为已撤回，以及未声明签名仍然拒绝。结果：`/private/tmp/asezh-editmode-results3.xml`。

## Fixture 来源

许可 ASE 源码不进入仓库。本次 `1.9.9.5` fixture 由维护者许可副本经 ASEZH `Remove` 还原出未接入状态后，
复制到系统临时目录使用；扫描确认 `ASELocale.` / `ASENativeDisplay.` / `ASESettingsDisplay.` 残留为 0，
`AmplifyShaderEditor.asmdef` 的 `references` 为空。

## 未验证（非阻塞观察项）

Shader 中文/原文生成等价、真实 Search / Toggle / dirty state 画布交互、完整 UI 与无障碍、
其他 Unity 版本与 Windows 均为 `unverified` / `not-run`，不冒充通过。

## 结论

`v0.0.13` 的必选团结门禁为 `pass`，可发布。扩展观察项维持 `unverified`，不阻止本次目标发布。
