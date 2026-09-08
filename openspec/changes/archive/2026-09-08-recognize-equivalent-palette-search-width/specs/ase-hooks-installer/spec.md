## ADDED Requirements

### Requirement: Equivalent palette Search-label width hooks are recognized safely
The installer SHALL report the Search-label width hook as applied when one local string variable is assigned the result of translating `m_searchFilterStr` and that same variable is passed to the label-size calculation. It SHALL NOT accept an untranslated value or a different variable as equivalent. Apply SHALL leave an accepted equivalent implementation unchanged. Remove without a valid installer receipt SHALL preserve the equivalent implementation and fail closed if its localization references would otherwise remain; Remove with a valid receipt SHALL restore the recorded preimage.

#### Scenario: Equivalent local variable is already applied
- **WHEN** one local variable receives the translated Search label and the same variable is measured for the Search-label width
- **THEN** Scan and Apply report the width hook as applied without rewriting the file

#### Scenario: Different variable is not equivalent
- **WHEN** the translated Search label is assigned to one variable but the width calculation measures another variable
- **THEN** the width hook is not reported as applied

#### Scenario: Untranslated variable is not equivalent
- **WHEN** the width calculation measures a local variable that was assigned the raw Search label
- **THEN** the width hook is not reported as applied

#### Scenario: Receipt-free removal preserves pre-existing equivalent code
- **WHEN** Remove encounters an equivalent local-variable implementation and no valid installer receipt exists
- **THEN** it does not rewrite that implementation and the session does not falsely report a complete removal

#### Scenario: Receipt-owned removal restores preimage
- **WHEN** a valid installer receipt covers the file containing the equivalent implementation
- **THEN** Remove restores the recorded preimage transactionally
