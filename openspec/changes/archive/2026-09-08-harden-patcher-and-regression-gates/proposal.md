## Why

ASEZH 当前能够对合成 ASE 源码应用补丁，但撤回无大括号搜索条件时会残留 `ASELocale` 钩子，且目标发现、写入和成功提示缺少单根锁定与事务保证。消费者交付需要以目标团结引擎和许可真实 ASE 主链路作为发布门禁，避免把未承诺的 Unity/Windows 兼容矩阵误设为发布阻塞。

## What Changes

- 将一次 Apply/Remove 限定到预检确认的唯一 ASE 根目录；双根、跨根、缺文件、只读或不可安全持久化时关闭写入。
- 在落盘前完成全部纯变换与残留验证，以完整 preimage、SHA-256、临时文件和会话级回滚保证全有或全无。
- 修复 `palette-build-list` 有/无大括号形式的撤回对称性；残留钩子或哈希不符时保留 asmdef 引用并返回失败状态。
- 建立公开静态/合成检查与团结 2022.3.61t9 + 许可外部 ASE 1.9.81 正式发布门禁，验证 install/apply/remove/recompile、preimage 恢复和本地化自测；Shader 生成对比、真实画布交互、其他 Unity 与 Windows 作为非阻塞观察项单独记录。
- 在回归保护下拆分补丁目录、目标解析、纯变换、事务 IO 及 locale 存储/GUI 适配，同时保持现有 public facade。
- 将版本规则改为相对 `package.json` 的连续 patch 一致性，并阻止已完成未归档 change、CHANGELOG/README/包版本漂移。
- 统一当前用户文档中的中文菜单路径，并区分目标团结发布证据与扩展兼容性证据。
- 非目标：不复制或提交 ASE 专有源码，不新增补丁钩子、语言或支持版本，不改变生成 Shader 语义，不重做 Installer 视觉，不修改远端保护。

## Capabilities

### New Capabilities

- `release-regression-gates`: 定义公开合成门禁、外部真实 ASE fixture 协议、支持矩阵、证据清单及发布阻断语义。

### Modified Capabilities

- `ase-hooks-installer`: 增加唯一目标、只读预检、事务回滚、对称撤回、残留检查和可信结果要求。
- `package-versioning`: 以 `package.json` 为当前事实源验证连续 patch、README、CHANGELOG 与交付证据，不再写死旧版本。
- `change-governance`: 增加已完成 change 的归档与需求到回归/交付证据追溯门禁。

## Impact

影响 `Editor/Installer`、`Editor/ASELocale.cs` 的内部结构、Editor 测试/统一验证脚本、OpenSpec 主规范、版本与用户文档。保持 `ASEZHPatcher.Scan/ApplyAll/RemoveAll` 和 `ASELocale` 对外入口兼容；不新增运行时依赖。消费者交付版本由 `0.0.7` 连续提升至 `0.0.8`，真实 ASE 资产仅通过外部路径注入。
