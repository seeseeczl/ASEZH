## ADDED Requirements

### Requirement: Editor captions outside settings panels are localized
Chinese mode SHALL translate the fixed captions ASE draws outside the settings panels, including node inspector help boxes and field labels, canvas helper windows, and the ASE shader and material Inspector buttons. Text field values, Shader identifiers, keywords, platform names and serialized data SHALL stay English.

#### Scenario: Node inspector list is empty
- **WHEN** a property node whose list is empty is inspected in Chinese mode
- **THEN** the help box shows the Chinese caption instead of the English one

#### Scenario: ASE Inspector buttons
- **WHEN** an ASE shader or material Inspector is drawn in Chinese mode
- **THEN** the open, compile and preview buttons show Chinese captions

#### Scenario: Values are untouched
- **WHEN** the same inspector is drawn in Chinese mode
- **THEN** editable text field values, Shader identifiers and platform names are unchanged

### Requirement: Localization coverage gaps are auditable
The repository SHALL provide a read-only command that, pointed at a licensed ASE source tree, reports hooked captions missing from the dictionary, wrapper labels missing from the dictionary, and display sites that have no hook. It SHALL NOT copy licensed sources into the repository.

#### Scenario: Hooked caption missing from dictionary
- **WHEN** a hooked caption has no matching dictionary entry
- **THEN** the command reports it under `hooked_missing` and `--fail-on-missing` exits non-zero

#### Scenario: Display site without hook
- **WHEN** a user-visible caption is drawn with no ASEZH hook
- **THEN** the command lists it under `unhooked` with file and line number
