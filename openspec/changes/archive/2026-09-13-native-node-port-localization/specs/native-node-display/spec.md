## Purpose

Provide Chinese titles and port labels for built-in ASE nodes while preserving graph data, generated Shader source, and custom Flyme node presentation.

## ADDED Requirements

### Requirement: Native-only display localization
Chinese mode SHALL translate known built-in node titles and input/output labels. Flyme categories, custom node types and user Shader Functions SHALL retain their original labels. Technical symbols and unknown labels SHALL remain unchanged.

#### Scenario: Native node with named ports
- **WHEN** Chinese mode draws a native node with a known title and port label
- **THEN** both show the Chinese translations

#### Scenario: Flyme and custom exclusion
- **WHEN** a Flyme-category node or non-native type uses the same title or port text as a native node
- **THEN** its title and port labels remain unchanged

### Requirement: Original data survives display and editing
Translations SHALL affect display copies only. Original titles, port names, IDs, types, connection topology, values and algorithms MUST remain unchanged. Editable identifier fields SHALL use original values.

#### Scenario: Equivalent generation
- **WHEN** the same graph is drawn and generated in English and Chinese
- **THEN** serialized graph data and generated Shader code are identical

### Requirement: Language changes refresh display geometry
Switching language SHALL refresh title and port measurement and repaint without marking the Shader modified or requesting compilation. Node positions SHALL be preserved.

#### Scenario: Toggle on an existing graph
- **WHEN** the user switches languages on an open graph
- **THEN** translated labels fit their measured areas and the graph remains clean
