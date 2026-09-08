## MODIFIED Requirements

### Requirement: Each shipped delivery increments by 0.0.1
Every delivery that is pushed for consumers to Update SHALL increment the patch segment by one (`x.y.z` → `x.y.(z+1)`), which is a `0.0.1` step. The project SHALL NOT skip patch numbers after the reset baseline `0.0.2`. Minor or major bumps SHALL occur only when the maintainer explicitly requests them. Validation SHALL derive the current version from `package.json` instead of embedding a past delivery as the current version.

#### Scenario: Next delivery increments the package fact
- **WHEN** a consumer-facing delivery is prepared from the current `package.json` version without an explicit minor or major request
- **THEN** its version is exactly the next patch version and no fixed historical version is treated as current

#### Scenario: Skipped patch version blocks delivery
- **WHEN** a prepared consumer delivery increments the patch segment by more than one
- **THEN** version validation fails before the delivery reference is updated

### Requirement: Changelog records every published version
Each published version SHALL have a `CHANGELOG.md` entry describing user-visible changes in that version. The newest changelog version and the version presented by current user documentation SHALL equal `package.json` for a consumer-facing delivery. Historical `1.0.0` MAY remain as a retired Git publish note.

#### Scenario: Current delivery metadata agrees
- **WHEN** a consumer-facing delivery is validated
- **THEN** `package.json`, the newest `CHANGELOG.md` section, and the current version in README identify the same version
