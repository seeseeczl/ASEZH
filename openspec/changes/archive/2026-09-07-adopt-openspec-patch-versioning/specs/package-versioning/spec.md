## Purpose

Defines how ASEZH publishes package versions so every shipped delivery is a visible, sequential patch bump of 0.0.1.

## ADDED Requirements

### Requirement: Package version is the published version source
The published version SHALL be the `version` field in `package.json`. README, changelog, and Package Manager metadata SHALL show that same value.

#### Scenario: Package Manager shows package.json version
- **WHEN** a consumer inspects the installed ASEZH package
- **THEN** the displayed version equals `package.json` `version`

### Requirement: Each shipped delivery increments by 0.0.1
Every delivery that is pushed for consumers to Update SHALL increment the patch segment by one (`x.y.z` → `x.y.(z+1)`), which is a `0.0.1` step. The project SHALL NOT skip patch numbers. Minor or major bumps SHALL occur only when the maintainer explicitly requests them.

#### Scenario: This delivery becomes 1.0.1
- **WHEN** the OpenSpec governance delivery is published from `1.0.0`
- **THEN** `package.json` version is `1.0.1`

#### Scenario: Next delivery becomes 1.0.2
- **WHEN** a later delivery is published from `1.0.1` without an explicit minor or major request
- **THEN** `package.json` version is `1.0.2`

### Requirement: Changelog records every published version
Each published version SHALL have a `CHANGELOG.md` entry describing user-visible changes in that version.

#### Scenario: 1.0.1 has a changelog entry
- **WHEN** version `1.0.1` is published
- **THEN** `CHANGELOG.md` contains a `1.0.1` section
