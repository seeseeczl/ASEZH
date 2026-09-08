## Why

ASE 1.9.81 may cache the translated Search label in a local variable before measuring it. The installer currently accepts only the inline expression, so a valid localized implementation is reported as `mismatch` and the transactional preflight rejects the whole Apply operation.

## What Changes

- Recognize a local string variable assigned from `ASELocale.T(m_searchFilterStr)` and later passed to `GUI.skin.label.CalcSize(new GUIContent(...))` as an equivalent installed `palette-search-width` hook.
- Require the same identifier at assignment and use, so untranslated or unrelated variables remain rejected.
- Treat the equivalent form as an Apply no-op and do not rewrite it.
- Preserve a pre-existing equivalent form during Remove unless an installer receipt proves that the installer owns the original content.
- Add regression coverage and publish the consumer-facing repair as `0.0.10`.
- Non-goals: no changes to translation keys, Search behavior, Shader output, ASE source layout, or supported Editor/ASE versions.

## Capabilities

### New Capabilities

None.

### Modified Capabilities

- `ase-hooks-installer`: accept a semantically equivalent Search-label-width implementation while retaining fail-closed matching and ownership-safe removal.

## Impact

Impacts the installer scanner/reverse transform, EditMode regression tests, release documentation, and package patch version. No public API or dependency changes.
