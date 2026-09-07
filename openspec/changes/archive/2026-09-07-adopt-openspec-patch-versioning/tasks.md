## 1. OpenSpec context

- [x] 1.1 Fill `openspec/config.yaml` project context with package identity, version step, and locale hard rules
- [x] 1.2 Add a Cursor always-apply rule that requires OpenSpec changes and a `0.0.1` version bump on consumer deliveries

## 2. Version and docs

- [x] 2.1 Set `package.json` `version` from `1.0.0` to `1.0.1`
- [x] 2.2 Add `CHANGELOG.md` with `1.0.0` and `1.0.1` entries
- [x] 2.3 Point README at OpenSpec workflow and the `0.0.1` version rule

## 3. Validate and archive

- [x] 3.1 Run `openspec validate --all --strict` and fix format issues
- [x] 3.2 Archive `adopt-openspec-patch-versioning` after implementation so main specs exist
