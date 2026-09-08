## Purpose

Defines reproducible checks for the supported Tuanjie release target while keeping unverified compatibility observations explicit and proprietary ASE source outside the repository.

## ADDED Requirements

### Requirement: Public checks use redistributable fixtures
The repository SHALL provide a static/public command that checks dictionary tables and duplicate keys, metadata GUIDs, version consistency, OpenSpec strict validity, source-size policy, sensitive file names, menu structure, and the distributable package without requiring proprietary ASE assets. Public checks SHALL NOT claim that the Tuanjie release gate passed.

#### Scenario: Public fixture regression fails the delivery gate
- **WHEN** a redistributable fixture retains a managed hook after Remove or any required public check fails
- **THEN** the command exits non-zero and the delivery is not marked ready

### Requirement: Tuanjie release evidence comes from an external licensed fixture
Release validation SHALL require Tuanjie 2022.3.61t9 and an explicitly configured external ASE 1.9.81 fixture. It SHALL NOT copy proprietary ASE source into repository artifacts and SHALL record engine, ASE and operating-system versions plus a redacted result manifest.

#### Scenario: Required Tuanjie fixture is unavailable
- **WHEN** release mode lacks the target Tuanjie executable or licensed ASE fixture
- **THEN** release validation fails and public fixture success does not replace it

### Requirement: Required release validation covers the Tuanjie consumer lifecycle
The required Tuanjie entry SHALL cover package installation, locale self-tests, clean compilation, scan, Apply, restart compilation, Remove, residual scan, final compilation, and byte-identical restoration of every managed preimage.

#### Scenario: Tuanjie lifecycle passes
- **WHEN** every required stage and preimage check passes in Tuanjie 2022.3.61t9 with ASE 1.9.81
- **THEN** the target-engine release gate passes

#### Scenario: Remove leaves the real editor compilable
- **WHEN** Apply has succeeded and Remove is run in a licensed fixture
- **THEN** no installer-owned hook remains and the Editor recompiles without ASEZH reference errors

### Requirement: Advisory compatibility evidence does not block the target release
Generated Shader comparison, live canvas interaction, accessibility review, other Unity versions, and Windows SHALL be recorded separately as advisory observations. Missing advisory evidence SHALL NOT block a release whose required Tuanjie gate passes, and SHALL NOT be represented as passed.

#### Scenario: Advisory environment is unavailable
- **WHEN** the required Tuanjie gate passes but an advisory environment is unavailable
- **THEN** the overall release result remains `pass` and the advisory item is recorded as `not-run` or `unverified`
