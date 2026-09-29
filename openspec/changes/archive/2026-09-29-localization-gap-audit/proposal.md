## Why

把维护者本地的 ASE 1.9.9.5 源码扫一遍后发现两类"汉化没彻底"：一类是界面本来就有的固定英文文案（节点检视面板、画布与 Inspector 上的按钮/提示）根本没有钩子；另一类是钩子已经接上、但词典里没有对应词条，于是运行时静默回退成英文。后者尤其难发现，因为界面看起来"接了汉化"。

## What Changes

- 新增只读审计命令 `scripts/audit_localization.py`：指向许可 ASE 源码即可列出 hooked_missing / wrapper_missing / unhooked 三类缺口。
- 补齐 13 处节点检视面板、画布与 Inspector 固定文案的钩子，并补上对应词条。
- 补齐 3 条已挂钩但缺词的静默漏译：`Assign Keyword`、`Depth Mode`、`Keys`。
- 非目标：不改 Shader 标识符、Keyword、平台名、`TextField` 值和序列化数据；本轮不覆盖 ASE 的开始界面与调试类工具窗口（见优化计划）。

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `display-locale`: 设置面板之外编辑器固定文案的汉化覆盖，以及可重复的缺口审计入口。

## Impact

新增界面文案目录、词典条目与审计脚本；包版本 0.0.14。许可 ASE 源码仍只在本机只读使用，不进入仓库。
