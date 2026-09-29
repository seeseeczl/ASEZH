## Context

见 proposal.md。ASEZH 的显示层通过改写 ASE 的绘制调用生效：`UndoParentNode` 系列包装方法被改成先调用 `ASELocale.T`。任何走这些包装方法的字符串都会被翻译，包括框架自有的标识——这次就是 FAGUI 属性名。

## Goals / Non-Goals

目标：给"框架自有标识"一个明确的退出通道，而不是靠从词典里删词（删词会连带影响其它使用同一英文词的位置）。
非目标：不改 FAGUI 属性名文本本身，不动普通属性列表与其它界面文案。

## Decisions

- 在该调用点直接换成 Unity 原生 `EditorGUILayout.ToggleLeft`：签名与语义一致（布尔勾选、返回新值），只是不再经过 `ASELocale.T`。相比"从词典里删掉 Foldout / Ramp / Vector 等词"，这样不会误伤其它同样使用这些英文词的界面。
- 用与其它文案钩子相同的 Find/Replace + Marker 模型，可被扫描、可被回执整体回滚。
- 词典条目保留：它们仍被别处使用，且删除会改变已发布行为。

## Risks / Trade-offs

- 该锚点同样版本敏感 → 与其它钩子一致，先在许可 ASE 上校验命中，再由发布门禁跑完整生命周期。
- "保持原文"类补丁目前逐点声明，没有通用规则 → 后续若出现更多框架标识，按同样方式逐点登记。

## Migration Plan

0.0.16 只新增一条补丁；已接入的工程重新扫描时该项报 `ready`，应用后 FAGUI 列表恢复原文显示。
