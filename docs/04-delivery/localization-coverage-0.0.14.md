# 0.0.14 汉化覆盖补齐证据

目标包版本：`0.0.14`（`REL-ASEZH-0014`）
变更：`localization-gap-audit`
主机：macOS 27.0 arm64

## 做法

新增只读审计命令 `scripts/audit_localization.py`，指向维护者许可的 ASE 1.9.9.5 源码，按运行时规则
（`TryLookup` 会先精确匹配，再按去掉首尾空格后的键匹配并还原缩进）列出三类缺口：

| 类别 | 含义 |
| --- | --- |
| `hooked_missing` | 钩子已接，但词典没有对应词条 —— 界面看起来已汉化，实际静默显示英文 |
| `wrapper_missing` | 走 `UndoParentNode` 包装方法显示的字面量标签，词典缺词 |
| `unhooked` | 界面会显示该文案，但源码里根本没有钩子 |

## 本轮修复

- 静默漏译 3 条：`Assign Keyword`、`Depth Mode`、`Keys`。这三条所在代码已经调用 `ASELocale.T`，
  仅缺词条，因此只补词典即可生效。
- 新增 13 条界面钩子（`Editor/Installer/ASEUiCaptionPatchCatalog.cs`），覆盖节点检视面板、画布提示
  与 ASE 的 Shader/Material Inspector：数组列表空状态、`Description`、`Custom URL`、Triplanar 的
  `None (Texture2D)` 与 `Select`、`Compile and show code | ▾`、`Open in Shader Editor`、
  `Open in Text Editor`、`Set as Preview`、`Get Local Var` 提示、端口图例窗口的 `Helper` 与 `Wiki Page`。
- 词典新增 16 条（含上面两类），总数 1417 → 1433。
- 13 条新锚点在许可 ASE 1.9.9.5 上逐条校验命中，合成 fixture 同步补齐对应文件。

修复后审计结果：`hooked_missing = 0`、`wrapper_missing = 0`、`unhooked = 170` 处候选。
`unhooked` 仍包含 ASE 开始界面、偏好设置、调试与批处理工具窗口，以及需要人工复核的间接绘制路径，
作为下一轮的清单保留在审计报告里，不在本版声称已覆盖。

## 验证

| 目标环境 | 静态检查 | 合成生命周期 | 许可 ASE 生命周期 | preimage 恢复 | 总结 |
| --- | --- | --- | --- | --- | --- |
| Tuanjie 2022.3.61t9 / macOS + ASE 1.9.9.5 | pass | pass | pass | pass | pass |

`v0.0.14` 的必选团结门禁为 `pass`。许可源码不进入仓库，审计只在系统临时目录只读运行。

## 未验证（非阻塞观察项）

Shader 生成文本逐字节对比、真实画布交互、其他 Unity 版本与 Windows 均为 `unverified` / `not-run`；
`unhooked` 剩余候选的汉化不在本次范围内。
