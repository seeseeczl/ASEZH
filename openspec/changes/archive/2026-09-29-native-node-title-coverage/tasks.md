## 1. 覆盖补齐

- [x] 1.1 Compare every ASE node declaration against the native allowlist and the node_title table.
- [x] 1.2 Add the missing built-in types, node entries and category entry, and hook the self-drawn Toggle Switch title.

## 2. 可审计性

- [x] 2.1 Extend the localization audit with node allowlist, node title and category coverage checks.

## 3. Verification and delivery

- [x] 3.1 Re-run the audit against the licensed ASE source until all three node checks are zero.
- [x] 3.2 Run the Tuanjie release gate plus EditMode regression on the new package version.
- [x] 3.3 Bump the package to 0.0.15, update docs, sync specs and archive the change.
