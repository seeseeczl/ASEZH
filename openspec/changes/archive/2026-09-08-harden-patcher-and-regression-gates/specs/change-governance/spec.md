## ADDED Requirements

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
