## Why

Native ASE nodes still display English port names and some titles. The requested Chinese toggle must cover those labels without changing Shader identifiers or algorithms.

## What Changes

- Translate native node titles and input/output display labels, including sizing and language refresh.
- Explicitly exclude Flyme categories and non-native node types; preserve user-authored names and technical symbols.
- Verify source/serialization equivalence and installer apply/remove behavior.
- Non-goals: changing node algorithms, identifiers, connections, user Shader Functions, or Flyme nodes.

## Capabilities

### New Capabilities

- `native-node-display`: Native-only title and port localization with exclusion and source-equivalence guarantees.

### Modified Capabilities

None.

## Impact

ASELocale display APIs, dictionary, installer display hooks, regression fixtures and package version 0.0.11. ASE sources remain external licensed assets.
