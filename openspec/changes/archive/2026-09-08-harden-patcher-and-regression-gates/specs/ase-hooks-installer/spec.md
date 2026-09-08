## ADDED Requirements

### Requirement: A patch session targets one ASE installation
Before Apply or Remove writes any file, the installer SHALL resolve every managed source file and assembly definition to one canonical ASE root and SHALL show that root to the user. If no root, multiple roots, a cross-root file set, a missing required file, or an unsafe read-only installation is found, the session SHALL fail closed without writing.

#### Scenario: Two ASE roots cause zero writes
- **WHEN** two candidate ASE installations are discoverable and neither has been uniquely selected
- **THEN** Apply and Remove report the ambiguity, identify the candidates, and leave every candidate file unchanged

#### Scenario: Managed files cannot cross roots
- **WHEN** the resolved managed files do not all belong to the same canonical ASE root
- **THEN** preflight fails before any source or assembly definition is changed

### Requirement: Patch writes are transactional
Apply and Remove SHALL precompute and validate every managed transformation before writing. A session SHALL retain complete pre-operation content and hashes, and if any write or post-write validation fails it SHALL restore every touched file to its pre-operation content and report whether restoration succeeded.

#### Scenario: Mid-session write failure restores all files
- **WHEN** one managed write fails after an earlier managed file was written
- **THEN** every touched file has its pre-operation SHA-256 after recovery and the session is reported as failed

#### Scenario: Repeated operation is idempotent
- **WHEN** Apply is run twice or Remove is run twice against the same installation
- **THEN** the second session performs no unsafe duplicate transformation and reports an accurate no-op state

### Requirement: Remove is symmetric and truthful
Remove SHALL recognize every installer-owned formatting variant that Apply supports, including LF/CRLF, tab/space, and palette search conditions with or without braces. It SHALL verify that no installer-owned `ASELocale` reference remains before removing the ASE assembly reference or reporting success.

#### Scenario: Palette condition without braces is removed
- **WHEN** the installed palette build-list hook uses an `if` condition without braces
- **THEN** Remove restores the original condition and the post-remove scan does not report `palette-build-list` as applied

#### Scenario: Residual hook prevents successful removal
- **WHEN** any installer-owned `ASELocale` hook remains after the planned reverse transforms
- **THEN** Remove preserves the assembly reference needed for compilation and reports the residual file and failed removal state

### Requirement: Installer results identify recovery state
The Installer SHALL distinguish success, safe no-op, preflight refusal, failed-and-restored, and failed-with-incomplete-restoration. A failure result SHALL identify the target root, affected file, retained backup location when applicable, and recovery conclusion without exposing unrelated user content.

#### Scenario: Restored failure is not shown as success
- **WHEN** a patch session fails and restores all pre-operation content
- **THEN** the summary states that no change was committed and does not display an Apply or Remove success message
