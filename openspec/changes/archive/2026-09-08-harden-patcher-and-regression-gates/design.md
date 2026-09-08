## Context

See `proposal.md` for motivation and the four delta specifications for behavior. The current static facade discovers individual files globally and writes each result directly; this couples target selection, transformation, IO, result reporting and assembly-reference cleanup. Tuanjie 2022.3.61t9 is the release target; other Unity versions and Windows are compatibility observations rather than release targets. Licensed ASE boundaries, current public entry points and user worktree contents must be preserved.

## Goals / Non-Goals

**Goals:**

- Make a patch session deterministic: one canonical target, complete preflight, all transformations planned in memory, atomic-enough commit, postcondition checks and full rollback.
- Keep pure matching/transformation testable with redistributable minimal fixtures and allow IO failure injection.
- Separate required Tuanjie release evidence from advisory UI and compatibility observations in one manifest.
- Reduce oversized facade responsibilities after behavior is protected, without changing its public surface.

**Non-Goals:**

- No new patch catalog entries, translation changes, dependencies, Shader behavior, automatic remote publication or proprietary fixture capture.
- No claim of atomic filesystem semantics beyond verified platforms; recovery remains explicit when replacement cannot be atomic.

## Decisions

### Resolve an immutable installation snapshot once

`AseTargetResolver` returns an `AseInstallation` containing canonical root, required file map, asmdef and persistence classification. Every later operation consumes this snapshot and rejects paths outside it. Explicit user selection is required when discovery yields multiple candidates. This is preferred over repeated `AssetDatabase.FindAssets` calls because repeated discovery can mix roots and change between steps.

### Separate plan, transaction and facade

`PatchCatalog` describes patches; `PatchTransforms` are pure functions over input text; `PatchPlan` contains expected preimage hash and proposed output; `PatchTransaction` owns backup, temporary replacement, verification and recovery. `ASEZHPatcher` remains the compatible facade and `ASEZHInstallerWindow` renders a structured session result. This staged extraction is preferred over a one-shot rewrite so each boundary can retain existing regressions.

### Preflight all outputs before commit

The operation reads every preimage once, computes SHA-256, runs every transformation in memory and checks planned postconditions before any write. Commit uses same-directory temporary files and replacement/move appropriate to the host; complete preimages are stored under a session directory outside the package target. On any error, touched paths are restored and rehashed. Assembly-reference removal is the final Remove plan item and is excluded whenever residual hooks exist. Direct sequential writes were rejected because they cannot guarantee cross-file recovery.

### Use typed states, not message parsing

Results use an enum for `Succeeded`, `NoOp`, `PreflightRejected`, `FailedRestored`, and `FailedRecoveryIncomplete`, with target, file outcomes, backup location and recovery details. Existing per-patch status remains available for compatibility. UI success is derived only from the session state and verified postconditions.

### Target the release gate at Tuanjie

A repository script runs redistributable static checks and the synthetic transaction lifecycle. Release mode additionally requires the configured Tuanjie 2022.3.61t9 executable and licensed ASE 1.9.81 fixture to pass Locale, baseline, Apply, restart compilation, Remove, final compilation, and preimage hash recovery. Real GUI, generated Shader comparison, other Unity versions, and Windows stay visible as advisory observations; they cannot be reported as passed without evidence, but their absence does not block this Tuanjie-targeted release.

### Extract locale responsibilities only after P0 gates

`ASELocale` keeps public methods and delegates data loading/search to `LocaleStore` and IMGUI helpers to `LocaleGuiAdapters`. Extraction uses language features supported by the target Tuanjie editor and the existing editor assembly. No new public types or dependency direction from pure logic back to Unity UI is introduced.

## Risks / Trade-offs

- [File replacement can differ outside the verified Tuanjie/macOS target] → classify persistence during preflight, test fault injection, retain preimages, and list other platforms as unverified compatibility observations.
- [Legacy/manual hooks are indistinguishable from installer-owned edits] → only reverse recognized complete patterns; report unknown residuals without destructive guessing.
- [Facade extraction can change static initialization order] → preserve public signatures and establish API/hash/regression baselines before moving logic.
- [Licensed Tuanjie runner is unavailable] → release mode fails; never substitute public fixtures.
- [UI automation can bind to the wrong Editor] → omit the advisory UI run instead of operating a user project or blocking the verified headless release path.

## Migration Plan

1. Add the failing brace-less Remove fixture, structured model, resolver and transaction behind the current facade; keep Apply/Remove scan-only if transaction validation is unavailable.
2. Run public fixtures and the available isolated Tuanjie gate, then enable writes and verify rollback/fault cases.
3. Add the external licensed fixture protocol and delivery manifest; make Tuanjie 2022.3.61t9 + ASE 1.9.81 the required release target and record other environments as advisory.
4. Extract patcher and locale internals under the protected facade.
5. Synchronize governance/version specs, bump `0.0.7` to `0.0.8`, update changelog/docs, validate, then archive. A rollback restores code and metadata together; external ASE changes restore from the session preimages.
