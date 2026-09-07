## Context

See proposal.md for why. The repo is a Unity Editor UPM package (`com.asezh.locale`) with no `openspec/specs/` baseline. OpenSpec CLI 1.12 is already installed. Cursor agent files were created by `openspec init --tools cursor`. Package Manager consumers track the Git default branch and read `package.json` `version`.

## Goals / Non-Goals

**Goals:**

- Land a spec-driven baseline that existing hard rules can be tested against.
- Make every consumer-facing push bump patch by `0.0.1`.
- Keep Cursor agents on the OpenSpec loop without copying the full product spec into a second document.

**Non-Goals:**

- Reset the published version to `0.0.1`.
- Bootstrap the heavier project-architect docs tree (`docs/00-governance` through `docs/06-operations`).
- Change dictionary entries, locale lookup, or ASE hook behavior in this delivery.

## Decisions

- **Keep `1.0.x` and step patch only.** `1.0.0` already shipped on GitHub. Downgrading to `0.0.1` would look like a Package Manager regression. `0.0.1` is the increment size, not the starting number.
- **Use the default `spec-driven` schema.** No custom schema fork until a real workflow gap appears.
- **Write current product rules as new capabilities in this change, then archive.** That creates `openspec/specs/` in one cycle instead of hand-writing main specs and a second change.
- **Cursor always-apply rule points at OpenSpec.** The rule stays short: version bump, change-first, fail-open locale rules as a reminder, details stay in specs.
- **Changelog is Markdown in repo root.** Unity Git packages have no built-in release feed; `CHANGELOG.md` is the consumer-visible history.

## Risks / Trade-offs

- [Agents ignore OpenSpec and edit code from chat] → Mitigation: always-apply Cursor rule plus README pointer; archive keeps specs current.
- [Someone bumps minor/major by habit] → Mitigation: spec forbids it unless the maintainer explicitly asks.
- [Git Package Manager Update uses commit, not semver range] → Mitigation: still bump `package.json` so the UI version matches the commit consumers pull.

## Migration Plan

1. Merge this change’s artifacts and implementation to `main`.
2. Archive the change so main specs exist.
3. Consumers click Package Manager Update; they should see `1.0.1`.
4. Rollback: revert the commit; version returns to `1.0.0`.
