# change-governance Specification

## Purpose
Defines how ASEZH records approved behavior and future changes so agents and humans share one spec-driven source of truth.
## Requirements
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

### Requirement: Delivered completed changes do not remain active
A change whose implementation and required validation are complete SHALL be synchronized and archived before the associated consumer delivery is declared ready. Validation SHALL report any active change whose tasks are all complete.

#### Scenario: Complete active change blocks governance closure
- **WHEN** an active change has all tasks checked but has not been archived
- **THEN** governance validation reports it and the delivery is not described as fully traceable

### Requirement: Consumer deliveries have an evidence chain
Each consumer-facing delivery SHALL link the approved change, affected requirement or regression identifiers, implementation revision, package version, and validation evidence. Missing required target evidence SHALL fail the release gate; missing advisory evidence SHALL be recorded as `not-run` or `unverified` rather than silently omitted or represented as passed.

#### Scenario: Delivery can be traced from version to evidence
- **WHEN** a maintainer reviews a delivered package version
- **THEN** they can identify its governing change, relevant regression results, implementation revision, required target result, and any unverified advisory environments
