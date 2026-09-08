## Context

See `proposal.md` for motivation. Catalog hooks normally recognize an exact token sequence with flexible whitespace. ASE 1.9.81 can instead translate the Search label once into a local variable and reuse it for both width calculation and the TextField. Remove first plans reverse transforms, then restores receipt-owned preimages and checks for residual localization references.

## Goals / Non-Goals

**Goals:**

- Recognize only the local-variable data flow that is semantically equivalent to the inline width hook.
- Keep Apply idempotent and preserve equivalent source formatting.
- Let receipt-backed Remove reach preimage restoration while keeping receipt-free Remove fail-closed.

**Non-Goals:**

- General C# data-flow analysis or arbitrary expression equivalence.
- Reformatting or normalizing ASE source.
- Claiming ownership of localization code not written by the current installation receipt.

## Decisions

1. Use a bounded, identifier-aware detector for `string <id> = ASELocale.T(m_searchFilterStr);` followed by width calculation using the same `<id>`. The identifier back-reference rejects similar-looking but semantically different code. A full C# parser was considered but would add a dependency and complexity disproportionate to this narrow compatibility form.
2. Run equivalent-form detection before the normal catalog Find fallback during Evaluate. Return `applied`, producing no planned write.
3. During Reverse, first retain the existing exact-replacement reversal. If only the equivalent local-variable form remains, return a safe no-op result for both Search-label catalog entries. The transaction's receipt restoration remains the only authority to overwrite it; absent a receipt, the existing residual-hook gate rejects completion and preserves the assembly reference.
4. Test the pure detector for accepted and rejected flows, and exercise the release lifecycle against both synthetic and licensed real ASE fixtures.

## Risks / Trade-offs

- [Equivalent source is separated by unusually large code] → Keep the detector bounded and fail closed as `mismatch`; adapt deliberately if a real supported ASE requires it.
- [Comments or strings resemble code] → Anchor complete assignment and calculation statements and require the same legal identifier; retain release testing against the actual supported fixture.
- [Safe Reverse status could be mistaken for full removal] → The transaction-level residual scan remains authoritative and prevents success without receipt restoration.

## Migration Plan

Publish as `0.0.10`. Existing users update the Git package and rerun the installer; the existing equivalent ASE source is recognized without modification. Rollback is the previous `v0.0.9` tag.
