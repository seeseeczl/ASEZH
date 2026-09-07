## Context

See proposal.md. Catalog Finds currently embed `\n\t\t\t` from one ASE snapshot. `Evaluate` uses `string.Contains`. `zwrite-labels` Marker is `ZWriteModeLabels`, which becomes true as soon as the Popup line is rewritten.

## Goals / Non-Goals

**Goals:**

- Token-level matching for catalog patches.
- Multi-step ZWrite: copy Values initializer to Labels, then retarget Popup only.

**Non-Goals:**

- Translating canvas function port names.
- Auto-patching every P1 file in hook-sites.md.

## Decisions

- **Regex `\s+` over a second catalog per ASE version.** One Find, many whitespace layouts.
- **Dedicated EnsureZWriteLabels / EnsureLanguageToggle.** Same pattern as asmdef reference; ZWrite is not a single substring swap.
- **Popup replace only `ZWriteModeStr, ZWriteModeValues` in EnumTypePopup / EditorGUILayoutPopup.** Never rewrite Shader-side `ZWriteModeValues[...]`.

## Risks / Trade-offs

- [Flexible regex over-matches] → Mitigation: Finds still include method signatures and distinctive call text.
- [Duplicate Labels array] → Mitigation: skip insert when `string[] ZWriteModeLabels` already exists.

## Migration Plan

1. Ship `0.0.3`. Consumers Update, re-run Installer Apply.
2. If ZWrite already CS0103, Apply inserts the missing array.
3. Rollback: revert the package commit; do not unpatch ASE automatically.
