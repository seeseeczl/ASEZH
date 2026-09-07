# ASEZH agent notes

This repo is governed by OpenSpec (`openspec/`).

1. Read `openspec/specs/` before changing product behavior.
2. For a semantic change, create or continue `openspec/changes/<name>/` (`/opsx-propose`) before editing `Editor/`.
3. Every consumer-facing delivery increments `package.json` `version` by `0.0.1` and adds a `CHANGELOG.md` entry.
4. After implementation, validate and `/opsx-archive`.
5. New files that Unity would import need a `.meta` with a stable GUID. Git packages are immutable, so Unity cannot generate them.

Details: `.cursor/rules/openspec-governance.mdc`.
