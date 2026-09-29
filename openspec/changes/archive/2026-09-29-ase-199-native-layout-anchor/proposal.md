## Why

ASE 1.9.9 把 `ParentNode.OnNodeLayout` 的签名从 `OnNodeLayout( DrawInfo )` 改成
`OnNodeLayout( DrawInfo, NodeUpdateCache cache = null )`。安装器的模糊匹配只忽略 tab/空格/换行，
参数列表不同即判为 `mismatch`；预检 fail-closed，用户看到「预检未通过，目标未写入」，
`native-language-layout` 无法接入，原生节点标题与端口汉化整体失效。

## What Changes

- 让一个钩子可以声明多套同语义锚点：命中任一即接入，并把源码里真实的签名行原样写回。
- 为 `native-language-layout` 增加 ASE 1.9.9 的 `NodeUpdateCache` 签名锚点。
- 把正式发布门禁的外部 ASE fixture 目标从 1.9.81 更新为当前在用的 1.9.9.5。
- 非目标：不改守卫块本身；未声明的签名漂移仍 fail-closed 拒绝；不把 ASE 源码纳入仓库。

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `ase-hooks-installer`: 钩子锚点需容忍 ASE 只改方法签名的版本漂移。
- `release-regression-gates`: 外部 ASE fixture 目标版本更新为 1.9.9.5。

## Impact

安装器锚点模型（`ASEZHPatch` 增加备用锚点）、`native-language-layout` 目录项、单元测试、
`docs/adapt-ase-version.md` 说明、发布门禁脚本与证据文档，包版本 0.0.13。
