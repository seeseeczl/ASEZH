# REL-ASEZH-0012 — Shader 设置面板汉化审计

版本：0.0.12；发布标签：v0.0.12。用户已授权提交、推送和发布，并明确批准本版本的单次发布门禁豁免。

## 实现范围

补齐 47 种共用控件重载，以及普通／嵌套分组、输出面板标题、模板编辑按钮、颜色掩码、依赖、标签和指令提示的直接显示入口。只包装绘制参数，文本输入的值、枚举与选项索引、Shader 标识符、Pass 名、平台名不变。

词典新增／完善 145 项映射，并修正前向渲染、烘焙通道等混合语言术语。已有原生节点与 FLYME 排除规则不变。

## 审计矩阵

| 类型 | 设置代码及模板词条 | 中英文生成与图数据 | 可视化／运行限制 |
| --- | --- | --- | --- |
| Built-in Surface | pass，共用及专用设置审计 | pass | 未逐个光照模型做渲染验收 |
| Legacy Lit／Unlit／Lightmap／Multi Pass | pass | pass，4 种模板 | 共用面板覆盖，未逐模板截图 |
| Legacy 后处理／精灵／粒子 | pass | pass，4 种模板 | 未做各用途实际渲染 |
| UI Default／Custom RT 初始化与更新 | pass | pass，3 种模板 | 未做 UI／纹理更新实际渲染 |
| URP Lit／Unlit／Decal／Custom Lit／Custom Unlit | pass | pass，5 种模板 | 实际 URP Lit 面板截图抽查通过 |
| URP 2D Lit／Unlit／Custom Lit／Custom 2D Unlit | pass | pass，4 种模板 | 未做 2D Renderer 实际渲染 |
| HDRP Lit／Unlit／Decal／Hair／Fabric | pass，随 ASE 附带的 10x、12x、14x–17x 包 | not-run | 未在项目中安装 HDRP，不宣称面板截图或 HDRP 渲染通过 |

`scripts/audit_settings.py` 对 34 个设置源码文件及随包模板选项做只读词条审计：295 项候选，保留 RGBA 通道符号、LOD、PBR 和内部样式名共 7 项；其余未翻译项为 0。此扫描是词条抽取和显示入口人工检查的辅助证据，不代表穷尽所有运行时分支。模板包只在内存中读取，不导入用户项目或复制到仓库。

## 验证记录

- 实测环境：macOS、Tuanjie 2022.3.62t13、ASE 1.9.6.2。
- 21 项 Editor 单元测试 pass，包含控件标签、提示副本、原数组及标识符保护。
- 21 种 Shader／模板默认设置生成完整文件逐字节一致；图数据序列化与修改状态一致。此项是语言切换生成检查，不是所有组合的渲染验收。
- 合成安装、编译、扫描、撤回、再次编译、受管文件原始字节恢复六阶段 pass。
- 现有实际安装成功撤回后重新接入；最终共管理 31 个文件。原始备份保留在用户项目 Library/ASEZH。
- 外部证据位于 `/Users/long/Documents/ChatGPT/ASEZH/verification-settings-0.0.12/`。仅测试临时图；没有保存或覆盖用户原本未保存的 Shader。

Tuanjie 2022.3.61t9 + ASE 1.9.81 正式发布门禁仍未运行；用户于 2026-09-14 明确批准 v0.0.12 的单次豁免，不改变后续版本的发布门禁。Windows 与未安装管线的实际运行验证未覆盖。
