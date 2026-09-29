# 0.0.15 节点标题覆盖补齐证据

目标包版本：`0.0.15`（`REL-ASEZH-0015`）
变更：`native-node-title-coverage`
主机：macOS 27.0 arm64

## 问题

对照本地 ASE 1.9.9.5 源码逐条核对 296 个 `[NodeAttributes]` 声明后，确认节点名仍显示英文有三个原因：

| 原因 | 数量 | 说明 |
| --- | --- | --- |
| 类型不在原生白名单 | 8 | 白名单按类型全名精确放行，不在名单里的节点标题与 Search 列表永远英文 |
| 白名单内但 `node_title` 缺词 | 5 | 类型通过，词条缺失，同样静默回退英文 |
| 分类缺词 | 1 | `Debug` 分类在 Search 列表显示英文 |

另有 `Toggle Switch` 节点自己绘制标题、绕过 `ParentNode` 的标题钩子。

## 修复

- 白名单新增 8 个内置类型：`CameraDirection`、`EyeIndex`、`ViewVectorNode`、`Matrix2X2Node`、`NaNNode`、`MatrixSplit`、`IMMatrixNode`、`PositionNode`。
- `node_title` 新增 13 条：上述 8 个节点名，以及 `Camera Position`、`View Direction`、`Main Light Attenuation`、`Main Light Color`、`Matrix Create`。
- `category` 新增 `Debug`。
- `ToggleSwitchNode` 标题按 `ScreenColorNode` / `StaticSwitch` 的同一模式接入 `ASENativeDisplay.Title`。
- 审计命令新增节点标题三类检查（类型白名单、节点词条、分类词条）。

修复后审计结果：`node_types_not_allowlisted = 0`、`node_titles_missing = 0`、`categories_missing = 0`，
同时 `hooked_missing = 0`、`wrapper_missing = 0`。词典 1433 → 1447 条，白名单 290 → 298 条。

被排除的行为保持不变：节点重命名文本、自定义 Shader Function、Flyme 分类与用户数据不参与翻译。

## 验证

| 目标环境 | 静态检查 | 合成生命周期 | 许可 ASE 生命周期 | preimage 恢复 | 总结 |
| --- | --- | --- | --- | --- | --- |
| Tuanjie 2022.3.61t9 / macOS + ASE 1.9.9.5 | pass | pass | pass | pass | pass |

`v0.0.15` 的必选团结门禁为 `pass`。

## 未验证（非阻塞观察项）

真实画布逐节点截图比对、其他 Unity 版本与 Windows 未执行；`unhooked` 的 170 处工具窗口候选
（开始界面、偏好设置、调试控制台等）不在本次范围内。
