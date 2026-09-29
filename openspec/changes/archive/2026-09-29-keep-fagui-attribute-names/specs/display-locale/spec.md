## ADDED Requirements

### Requirement: Framework-owned identifiers stay untranslated
Attribute names and other identifiers owned by an external framework SHALL be displayed exactly as the ASE source provides them. The FAGUI property attribute list SHALL bypass the display overlay while the ordinary property attribute list keeps its translations.

#### Scenario: FAGUI attribute list
- **WHEN** the FAGUI attribute checklist is drawn in Chinese mode
- **THEN** every entry keeps the name from the ASE source and is not translated

#### Scenario: Ordinary property attributes stay translated
- **WHEN** the standard property attribute list is drawn in Chinese mode
- **THEN** its known captions are still translated
