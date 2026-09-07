# package-versioning Specification

## Purpose
Defines how ASEZH publishes package versions so every shipped delivery is a visible, sequential patch bump of 0.0.1.
## Requirements
### Requirement: Package version is the published version source
The published version SHALL be the `version` field in `package.json`. README, changelog, and Package Manager metadata SHALL show that same value.

#### Scenario: Package Manager shows package.json version
- **WHEN** a consumer inspects the installed ASEZH package
- **THEN** the displayed version equals `package.json` `version`

### Requirement: Each shipped delivery increments by 0.0.1
Every delivery that is pushed for consumers to Update SHALL increment the patch segment by one (`x.y.z` → `x.y.(z+1)`), which is a `0.0.1` step. The project SHALL NOT skip patch numbers after the reset baseline. The published baseline SHALL be `0.0.2`. Minor or major bumps SHALL occur only when the maintainer explicitly requests them.

#### Scenario: This delivery is published as 0.0.2
- **WHEN** the maintainer resets the version and publishes the current OpenSpec and package-meta work
- **THEN** `package.json` version is `0.0.2`

#### Scenario: Next delivery becomes 0.0.3
- **WHEN** a later delivery is published from `0.0.2` without an explicit minor or major request
- **THEN** `package.json` version is `0.0.3`

### Requirement: Changelog records every published version
Each published version SHALL have a `CHANGELOG.md` entry describing user-visible changes in that version. The current published version section SHALL be `0.0.2`. Historical `1.0.0` MAY remain as a retired Git publish note.

#### Scenario: 0.0.2 has a changelog entry
- **WHEN** version `0.0.2` is published
- **THEN** `CHANGELOG.md` contains a `0.0.2` section

