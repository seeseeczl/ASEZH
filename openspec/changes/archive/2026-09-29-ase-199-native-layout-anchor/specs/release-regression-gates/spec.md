## MODIFIED Requirements

### Requirement: Tuanjie release evidence comes from an external licensed fixture
Release validation SHALL require Tuanjie 2022.3.61t9 and an explicitly configured external ASE 1.9.9.5 fixture. It SHALL NOT copy proprietary ASE source into repository artifacts and SHALL record engine, ASE and operating-system versions plus a redacted result manifest.

#### Scenario: Required Tuanjie fixture is unavailable
- **WHEN** release mode lacks the target Tuanjie executable or licensed ASE fixture
- **THEN** release validation fails and public fixture success does not replace it

#### Scenario: Fixture version differs from the required target
- **WHEN** release mode is pointed at an ASE fixture whose version is not 1.9.9.5
- **THEN** the real-ASE gate fails and reports both the observed and the expected version

### Requirement: Required release validation covers the Tuanjie consumer lifecycle
The required Tuanjie entry SHALL cover package installation, locale self-tests, clean compilation, scan, Apply, restart compilation, Remove, residual scan, final compilation, and byte-identical restoration of every managed preimage.

#### Scenario: Tuanjie lifecycle passes
- **WHEN** every required stage and preimage check passes in Tuanjie 2022.3.61t9 with ASE 1.9.9.5
- **THEN** the target-engine release gate passes

#### Scenario: Remove leaves the real editor compilable
- **WHEN** Apply has succeeded and Remove is run in a licensed fixture
- **THEN** no installer-owned hook remains and the Editor recompiles without ASEZH reference errors
