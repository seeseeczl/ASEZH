## 1. 缺口审计

- [x] 1.1 Scan the maintainer's licensed ASE 1.9.9.5 source and classify localization gaps.
- [x] 1.2 Add the read-only `scripts/audit_localization.py` command with runtime-equivalent key normalization.

## 2. 覆盖补齐

- [x] 2.1 Hook the high-traffic node inspector, canvas and Inspector captions and add their dictionary entries.
- [x] 2.2 Add dictionary entries for hooked captions that resolved to English at runtime.

## 3. Verification and delivery

- [x] 3.1 Verify every new anchor matches the licensed ASE source and that no silent key gaps remain.
- [x] 3.2 Run the Tuanjie release gate plus EditMode regression on the new package version.
- [x] 3.3 Bump the package to 0.0.14, update docs, sync specs and archive the change.
