# ASEZH

Amplify Shader Editor（ASE）的**显示层中文包**。

它只翻译编辑器里看到的文字：节点列表、画布标题、设置面板、下拉选项。内部 KEY、生成的 Shader、Keyword、Define、属性名、Pass 名、渲染平台名保持英文。

ASEZH 是独立 UPM 包（`com.asezh.locale`），不是一份「已汉化的 ASE」。官方 ASE 仍放在工程原来的位置；本包叠在上面做显示，升级 ASE 时只重新接入钩子，不必把译文写进 ASE 本体。

仓库：<https://github.com/seeseeczl/ASEZH>  
许可：MIT  
当前版本：`0.0.3`（每次面向 Package Manager 的交付只加 `0.0.1`，见 [`CHANGELOG.md`](CHANGELOG.md)）

## 适合做什么 / 不做什么

**会做**

- 中文 / 原文（EN）一键切换
- 节点分类、节点名、设置项、下拉显示文本汉化
- 中英文都能搜索节点
- 官方自带 Shader Function 的列表名、函数节点设置面板（开关名、内部数据标签）
- 同一套词典接到不同版本的 ASE 源码上

**不会做**

- 不改生成结果：切中文再切回 EN，Shader 文本应无差异
- 不翻译 Direct3D / Vulkan / PlayStation 等平台名
- 不把函数资源的 GUID、`FunctionName`、输入名改成中文
- 不替代 Amplify Shader Editor 本体，也不提供整包 ASE 替换包

## 硬规则

1. 显示层查词典；内部 KEY 永远是英文原文。
2. 生成的 Shader / Keyword / Define / 属性名 / Pass 名不得变成中文。
3. 渲染平台名不进词典，查不到就 fail-open 为原文。
4. 汉化失败不得破坏编辑器功能。
5. 下拉只翻译**显示副本**（clone），按下标读写；禁止改源数组。
6. 语言开关只留 ASE 画布左上角一颗 Toggle，不要在 Preferences 或节点面板再放。
7. 不要对 TextField 的**值**（属性名、Pass 名、Inspector 名）调用 `T()`。

## 要求

- Unity / 团结 2019.4 或更高（本包 `unity` 字段为 `2019.4`）
- 工程里已安装 **Amplify Shader Editor**（通常在 `Assets/AmplifyShaderEditor/`）
- 能访问 GitHub（包管理器按 Git URL 拉取）

## 安装

和 [MCP for Unity](https://github.com/CoplayDev/unity-mcp) 一样：用 Unity 包管理器填 Git URL。源码进 `Library/PackageCache`，**不进 `Assets/`**，也不会把汉化目录提交进你的游戏工程仓库。

### 1. 用包管理器添加

1. `Window → Package Manager`
2. 左上角 `+` → `Add package from git URL…`
3. 粘贴：

```
https://github.com/seeseeczl/ASEZH.git
```

4. 点 Add，等到 Project 窗口出现 `Packages / ASEZH`

ASE 若使用独立程序集 `AmplifyShaderEditor.asmdef`，必须引用 `ASEZH.Editor`（安装器会自动写入）。未引用时会出现 `The name 'ASELocale' does not exist`。

也可以直接改 `Packages/manifest.json`：

```json
{
  "dependencies": {
    "com.asezh.locale": "https://github.com/seeseeczl/ASEZH.git"
  }
}
```

指定分支：

```
https://github.com/seeseeczl/ASEZH.git#main
```

### 2. 去掉工程里旧的内嵌汉化

如果曾经把 `ASELocale.cs` 放进 ASE 插件目录，必须先删掉或停用，否则两个 `AmplifyShaderEditor.ASELocale` 会重复定义，工程编不过：

```
Assets/AmplifyShaderEditor/Plugins/Editor/Localization/ASELocale.cs
Assets/AmplifyShaderEditor/Plugins/Editor/Localization/ASELocaleDictionary.json
```

### 3. 把显示层接到当前 ASE

ASE 没有稳定的公共 UI API。装包之后还要在 ASE **显示路径**上调用本包 API。

1. `Window → ASEZH → Install into Amplify Shader Editor`
2. 看扫描结果，点「应用可自动补丁」（会改 ASE 源码里的锚点片段，建议先备份或提交 ASE）
3. `Window → ASEZH → Run Locale Tests`

扫描状态：

| 状态 | 含义 |
| --- | --- |
| `applied` / `patched` | 已接上 |
| `ready` | 锚点匹配，可以自动打 |
| `mismatch` | 这份 ASE 和内置锚点不同，**不要强行套补丁**，按 `docs/hook-sites.md` 手工接 |
| `missing` | 找不到对应文件（ASE 被裁剪或改名） |

Console 正常加载时会有类似日志：`ASEZH: entries=... collisions=... tables=...`

### 4. 装在哪、改在哪

| 东西 | 位置 |
| --- | --- |
| ASEZH 包（引擎 + 词表 + 安装器） | `Packages/ASEZH`（磁盘在 PackageCache，或 Git URL 解析结果） |
| 中文界面 | 仍在 **ASE 编辑器窗口**里（列表、画布、设置面板） |
| 语言开关 | ASE 画布左上角，源码图标右侧 |
| 安装器改动的文件 | ASE 插件源码，例如 `UndoParentNode.cs`、`ToolsWindow.cs` |

本包**不会**把整个汉化文件夹塞进 `Assets/AmplifyShaderEditor/`。

## 使用

- 打开任意 ASE Shader 图。工具栏点 **中文** / **EN** 切换。
- 只点语言开关时，Shader 不应变成 dirty，也不应触发 Live 编译。
- 节点搜索同时匹配英文 KEY 和中文译文，例如搜 `Add` 或「加法」都能命中。
- 创建节点、连线、生成 Shader 仍使用英文内部名。
- 官方 Shader Function 的 GUID、`FunctionName` 不会因切语言而改变；换一台没装 ASEZH 的机器，图还能打开，只是界面变回英文。

菜单：

| 菜单 | 作用 |
| --- | --- |
| `Window/ASEZH/Install into Amplify Shader Editor` | 扫描并打显示钩子 |
| `Window/ASEZH/Reload Dictionary` | 重载 JSON 词表，无需域重载 |
| `Window/ASEZH/Run Locale Tests` | 自测查找、clone、平台名、搜索 |

## 词表

长期维护面只有 `Editor/ASEZHDictionary.json`。不要把词表复制进 ASE 插件目录。

每条：

```json
{ "table": "option_value", "key": "Opaque", "zh": "不透明" }
```

`table` 必须是下面之一：

| table | 用途 |
| --- | --- |
| `category` | 节点分类（如 Functions → 函数） |
| `node_title` | 节点 / Shader Function 标题 |
| `option_label` | 设置项名称、函数开关标签 |
| `option_value` | 下拉选项显示文本 |
| `panel` | 面板、分组等壳层文案 |

同一英文可以出现在不同 table，译文可以不同：

- `Add` 作节点名 → 「加法」
- `Add` 作 Blend Op → 「相加」
- `True` 作标签 → 「真」；作选项 → 「是」

查找顺序：

- 字段标题 `T(key)`：`option_label` → `category` → `node_title` → `panel` → `option_value`
- 下拉 `TranslateArray`：`option_value` 优先

同 table+key 出现不同 `zh` 时会 `LogError`。查不到则显示原文。

改完 JSON 后执行 `Window → ASEZH → Reload Dictionary`。

可选本地覆盖：把 `Editor/ASEZHDictionary.user.json.example` 复制为 `ASEZHDictionary.user.json`（已被 `.gitignore`）。覆盖文件**只能改已有 table+key**，不能新增条目。

## 治理

需求与变更用 [OpenSpec](https://github.com/Fission-AI/OpenSpec) 管理。

- 当前批准行为：[`openspec/specs/`](openspec/specs/)
- 进行中的变更：`openspec/changes/<name>/`（proposal → specs → design → tasks）
- 完成后归档，规格合入 `openspec/specs/`
- Cursor：`/opsx-propose`、`/opsx-apply`、`/opsx-archive`

未写入规格的语义不要当成已批准需求。聊天记录不能替代 `openspec/`。

版本号只改 `package.json` 的 `version`，每次可更新交付 `+0.0.1`。需要升 minor/major 时必须明确说。

## 架构

```
ASEZH（本仓库）                         目标 Unity 工程
┌──────────────────────┐               ┌─────────────────────────────┐
│ ASEZHDictionary.json │               │ AmplifyShaderEditor/        │
│ ASELocale 引擎       │  显示时调用    │  UndoParentNode → T / clone │
│ Installer 锚点补丁   │               │  ToolsWindow 语言开关       │
└──────────────────────┘               │  生成 Shader（英文 KEY）    │
                                       └─────────────────────────────┘
```

- **词典**：只加 `table + key + zh`
- **引擎**：不知道 ASE 节点类型，只做查找、clone、开关、搜索
- **钩子**：随 ASE 源码漂移；安装器能打的自动打，对不上就手工接

核心 API：`ASELocale.T(key)` / `T(key, table)` / `GUI()` / `TranslateArray()` / `LayoutEnumPopup()`。

更细的钩子表见 [`docs/hook-sites.md`](docs/hook-sites.md)，换 ASE 版本见 [`docs/adapt-ase-version.md`](docs/adapt-ase-version.md)。

## 升级 ASE

1. 用官方 ASE 覆盖源码前，备份已打补丁的文件，或先提交 Git。
2. 覆盖后打开 Installer 重新扫描。
3. 变成 `mismatch` 的条目，按新源码把同一语义接回去，不要硬套旧锚点。
4. 词典一般不用动；只有新英文文案才加 JSON 条目。

## 仓库结构

```
ASEZH/
  package.json                         UPM：com.asezh.locale，version 每次 +0.0.1
  CHANGELOG.md
  LICENSE                              MIT
  openspec/                            需求基线与变更
  Editor/
    ASELocale.cs                       引擎
    ASEZHDictionary.json               词表
    ASEZHDictionary.user.json.example  本地覆盖模板
    Installer/                         扫描 / 写入 ASE 锚点
  docs/
    architecture.md
    hook-sites.md
    adapt-ase-version.md
```

## 限制与已知缺口

- 自动补丁只覆盖锚点仍接近官方 ASE 的文件。P1/P2 部分钩子（调色板搜索、部分 Popup 重载等）可能要手工接。
- ZWrite：必须先有 `ZWriteModeLabels` 数组，Popup 用 Labels、生成用 `ZWriteModeValues`。安装器目前主要改 Popup 调用；干净 ASE 若没有 Labels 数组，需要按 `hook-sites.md` 补定义，否则会编不过。
- 用户自己做的 Shader Function 没有词条时保持英文（fail-open），这是预期行为。
- 函数节点**画布引脚名**（如 Tex）默认仍走 `port.Name`，未接显示钩子；设置面板里的开关名 / 内部数据已走 `T()`。
- 不要把 ASEZH 和工程内嵌的 `ASELocale.cs` 同时启用。

## 验收清单

装好并打完钩子后建议确认：

- [ ] `Run Locale Tests` 通过
- [ ] 画布左上角只有一颗中文 / EN 开关
- [ ] 切语言后，含 ZWrite / Blend / 平台 / Keyword 的 Shader 生成文本无 diff
- [ ] 平台名仍为 Direct3D / Vulkan / PlayStation
- [ ] 只切语言，Update 不进 dirty、Live 不编译
- [ ] 搜「加法」和 `Add` 都能找到节点

## 常见问题

**切中文后函数还能给别人用吗？**  
能。图里存的是函数 GUID 和英文名，不是中文。另一台机器没装 ASEZH，只是界面变回英文。

**为什么不要用 unitypackage 整包替换 ASE？**  
那等于维护一份中文 ASE 分叉，官方一升级就要整目录覆盖。本包的目标是官方 ASE + 独立显示层。

**Git URL 装失败？**  
确认仓库已公开、地址是 `https://github.com/seeseeczl/ASEZH.git`，且 `package.json` 在仓库根目录（本仓库不需要 `?path=`）。

## 文档

- [架构](docs/architecture.md)
- [钩子清单](docs/hook-sites.md)
- [适配其他 ASE 版本](docs/adapt-ase-version.md)
- [变更记录](CHANGELOG.md)
- [OpenSpec 规格](openspec/specs/)
