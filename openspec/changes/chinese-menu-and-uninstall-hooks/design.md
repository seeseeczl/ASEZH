## Context

See proposal.md. Unity `MenuItem` paths are compile-time string constants, so ASEZH's own Window menu cannot use `ASELocale.T()` or the canvas language Toggle. Catalog patches already store Find/Replace pairs that can be inverted. ZWrite labels, the language Toggle insert, PaletteParent `MatchesSearch continue`, and the asmdef reference are special-cased today.

## Goals / Non-Goals

**Goals:**

- Chinese `Window/ASEZH` item labels; folder stays `ASEZH`.
- `RemoveAll` inverts applied hooks in reverse dependency order (file hooks first, asmdef last).
- Confirmation dialog before writing.

**Non-Goals:**

- Uninstalling the UPM package from Package Manager.
- Reverting hand-written hooks that do not match installer Find/Replace.
- Making Window menu follow the canvas language Toggle.

## Decisions

- **Hardcoded Chinese `MenuItem` paths.** Alternative `T()` is impossible at attribute time. Dual English+Chinese items would duplicate the menu.
- **Invert catalog Replace→Find with the same flexible whitespace matcher.** Special-case ZWrite (delete Labels field, Popup back to Values), language Toggle (delete inserted DrawLanguageToggle block), palette-build-list (strip bad continue; restore IndexOf if we replaced it), asmdef last.
- **Keep Search-label leak fix in the same 0.0.7 ship** (unpublished). Remove does not reintroduce that continue.

## Risks / Trade-offs

- [Hand-hooked Flyme files match Replace and get reverted] → Expected: remove means remove display hooks.
- [Asmdef has extra references] → Only strip `ASEZH.Editor`; if the file is not one of the two known empty-array shapes, report mismatch and leave it.
- [Partial reverse leaves ASELocale calls without asmdef] → Reverse file hooks before asmdef.

## Migration Plan

1. Ship `0.0.7`. Update, reopen Unity so the Window menu refreshes.
2. Rollback: keep English `MenuItem` aliases is not offered; users who need English use the still-English product folder name.
