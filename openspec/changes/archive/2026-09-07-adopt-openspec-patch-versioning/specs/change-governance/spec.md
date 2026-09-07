## Purpose

Defines how ASEZH records approved behavior and future changes so agents and humans share one spec-driven source of truth.

## ADDED Requirements

### Requirement: Approved behavior lives in OpenSpec specs
The current approved product behavior SHALL be stored under `openspec/specs/`. README and architecture docs MAY summarize, but they SHALL NOT be the only record of a requirement.

#### Scenario: Locale hard rules are in specs
- **WHEN** a contributor looks up whether platform names may be translated
- **THEN** the answer is in `openspec/specs/display-locale/spec.md`

### Requirement: Semantic changes use an OpenSpec change
A change that adds, modifies, or removes approved behavior SHALL have a folder under `openspec/changes/<name>/` with proposal, spec deltas, design when needed, and tasks before implementation. Pure refactors with no behavior change MAY set `skip_specs: true`.

#### Scenario: New UI behavior requires a change folder
- **WHEN** a contributor wants to add a second language control
- **THEN** they create an OpenSpec change and a spec delta before editing editor UI code

### Requirement: Agents follow OpenSpec then implement
Coding agents working in this repository SHALL read the active change and existing specs before editing production files, SHALL implement only the approved tasks, and SHALL bump the package version by `0.0.1` when the work is intended for consumers to Update.

#### Scenario: Agent starts a behavior change
- **WHEN** the user asks for a product behavior change
- **THEN** the agent creates or continues an OpenSpec change and does not treat chat text as the approved spec

### Requirement: Completed changes are archived
After a change is implemented and validated, it SHALL be archived so delta specs merge into `openspec/specs/` and the change folder moves to `openspec/changes/archive/`.

#### Scenario: Governance change is archived after ship
- **WHEN** this governance delivery is complete and `openspec validate` passes
- **THEN** the change is archived and main specs contain the new capabilities
