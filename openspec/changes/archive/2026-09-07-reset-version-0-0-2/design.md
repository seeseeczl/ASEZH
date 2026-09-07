## Context

See proposal.md. `1.0.1` never left this working tree. GitHub currently shows `1.0.0` from the first two commits.

## Goals / Non-Goals

**Goals:**

- Publish one consumer-facing version: `0.0.2`.
- Keep patch-only increments afterwards.

**Non-Goals:**

- Rewriting git history of the `1.0.0` commits.
- Changing locale behavior.

## Decisions

- **Reset to `0.0.2`, not `0.0.1`.** Maintainer specified `0.0.2`. `0.0.1` is reserved as the increment size, not this tag.
- **Single publish.** Do not push `1.0.1` then immediately reset; Package Manager would flicker.

## Risks / Trade-offs

- [Installed `1.0.0` looks like a downgrade in the version field] → Mitigation: changelog states the reset; Git Update still pulls newer commits.

## Migration Plan

1. Set version files to `0.0.2`, archive this change, push `main`.
2. Consumers Update the Git package.
3. Rollback: revert the commit; version returns to `1.0.0` on GitHub.
