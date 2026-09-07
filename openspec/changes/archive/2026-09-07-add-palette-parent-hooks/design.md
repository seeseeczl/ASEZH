## Context

See proposal.md. Flyme already hooks `PaletteParent.cs`; the installer catalog never included it. Colleague stock ASE therefore left built-in categories in English.

## Goals / Non-Goals

**Goals:**

- Add catalog patches for PaletteParent display and search.
- Keep create-node callbacks on English `Name`.

**Non-Goals:**

- Translating canvas function port names.
- Rewriting shader-function folder names on disk.

## Decisions

- **Five catalog rows, unique markers.** Category foldout is the screenshot bug; search/items ship in the same drop.
- **Do not mutate `current.Key`.** Only wrap it in `T()` at Toggle.

## Risks / Trade-offs

- [ASE Toggle uses GUIContent instead of string] → Mitigation: second node-item Find for `ItemUIContent` without `.text`.

## Migration Plan

1. Ship `0.0.5`. Colleague Updates and reapplies installer.
2. Rollback: revert package; PaletteParent hooks remain in ASE until they revert that file.
