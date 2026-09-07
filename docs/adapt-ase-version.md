# 适配其他版本的 ASE

ASE 没有稳定的公共 UI API。版本差异几乎都在 `UndoParentNode` 和工具栏绘制上。适配流程固定为：**先扫，再补，再测**。

## 1. 接入包

把 ASEZH 放进目标工程（Package Manager Git URL 或 UPM `file:`）。确认 Console 出现 `ASEZH: entries=...`。

若 ASE 使用 `AmplifyShaderEditor.asmdef`，安装器会写入对 `ASEZH.Editor` 的引用。未写入时编译报 `ASELocale does not exist`。

## 2. 扫描

`Window → ASEZH → Install into Amplify Shader Editor`

| 状态 | 含义 |
| --- | --- |
| `applied` / `patched` | 已接上 |
| `ready` | 锚点匹配，可以自动打 |
| `mismatch` | 这份 ASE 源码和内置锚点不同，必须手工改 |
| `missing` | 找不到文件（改名或裁剪过的 ASE） |

不要对 `mismatch` 强行套补丁。安装器会忽略 tab/空格/换行差异；仍 mismatch 才需要手工改。

`zwrite-labels` 会从 `ZWriteModeValues` 复制出 `ZWriteModeLabels` 再改 Popup。若只改了 Popup 却没有数组，扫描不会标成 applied，再次应用会把数组补上。

## 3. 手工接入顺序

1. `UndoParentNode` 的 Popup / EnumPopup / Toggle（覆盖 90% 设置项）
2. `ToolsWindow` 语言按钮
3. 标题 / 列表 / 搜索（`hook-sites.md` P1）
4. ZWrite Labels 与 Values 拆开

对照当前工程里已接好的参考实现：`FlymeAuto3Test/Assets/AmplifyShaderEditor/...` 中带 `ASELocale.` 的调用。

## 4. 验收

1. `Window → ASEZH → Run Locale Tests` 全过。
2. 打开一张含 ZWrite / Blend / 平台 / Keyword 的 Shader。切「中文」再切「EN」，生成文本 diff 为空。
3. 平台复选框仍为 Direct3D / Vulkan / PlayStation。
4. 只点语言开关，Shader 不应进入 dirty / Live 编译。

## 5. 升级 ASE 之后

1. 用官方 ASE 覆盖源码前，备份已打补丁的文件或重新从 git 提交。
2. 覆盖后重新打开 Installer 扫描。
3. `applied` 变 `mismatch` 的条目，按新源码把同一语义接回去。
4. 词典一般不用改；只有新节点英文文案才需要加 `ASEZHDictionary.json` 条目。

## 6. 词典与引擎分离

换 ASE 版本时**不要**把 `ASEZHDictionary.json` 复制进 ASE 插件目录。词表留在 ASEZH，这样多版本共用一份译文。
