## Purpose

Localize native Shader settings across Built-in, URP and HDRP editor families while retaining original identifiers and identical generated Shader source.

## ADDED Requirements

### Requirement: Native settings display coverage
Chinese mode SHALL translate known native settings titles, foldouts, field labels, buttons, tooltips and option captions in surface and template Shader panels, including common properties, SubShader, Pass and optional modules.

#### Scenario: Template common and advanced settings
- **WHEN** a user opens a supported template output panel in Chinese mode
- **THEN** its standard and expanded module controls show Chinese captions, with unknown custom text preserved

### Requirement: Settings values remain original
Translations SHALL use display copies only and SHALL preserve editable values, template identifiers, platform names, selected indices, enums, graph state and generated Shader code.

#### Scenario: Switch language and generate
- **WHEN** the same settings are displayed and generated in both languages
- **THEN** stored settings and generated code are identical and switching alone does not mark the graph modified

### Requirement: Cross-family audit is explicit
Delivery SHALL record coverage for Built-in surface, legacy templates, URP and HDRP templates, distinguishing source audit, editor UI verification and actual pipeline render verification.

#### Scenario: Pipeline unavailable
- **WHEN** a pipeline cannot run in the available editor project
- **THEN** its source and panel checks are reported separately and runtime rendering is not claimed as passed
