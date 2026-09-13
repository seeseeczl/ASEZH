## Why

Output settings remain partly English because common controls, nested groups and template-specific options bypass the existing display hooks. Audit all shipped Shader settings families and complete their display localization without changing generated code.

## What Changes

- Audit Built-in surface, legacy templates, URP and HDRP template settings, including common, SubShader, Pass, material, and optional sections.
- Translate labels, foldouts, buttons, tooltips and option display copies at shared drawing boundaries; supplement direct template UI hooks and dictionary coverage.
- Preserve all editable data, platform names, custom identifiers, selection indices and Shader algorithms.
- Verify install/remove, actual panels, language switching and generated Shader equivalence; document unavailable runtime pipelines explicitly.

## Capabilities

### New Capabilities
- `shader-settings-display`: Complete display localization of native Shader settings across available Shader families.

### Modified Capabilities
None.

## Impact

Editor display helpers, installer hooks, dictionary, synthetic fixtures and tests. Version 0.0.12. Non-goals: render algorithms, user graph edits, FLYME node titles/ports, translating user values, or automatic release/publication.
