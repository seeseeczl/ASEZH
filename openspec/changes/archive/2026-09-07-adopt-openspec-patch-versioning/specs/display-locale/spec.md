## Purpose

Defines the display-only Chinese overlay: editors see translated UI while shader identifiers, keywords, and platform names stay English.

## ADDED Requirements

### Requirement: Display lookup keeps English keys
The overlay SHALL treat the original English string as the lookup key and SHALL NOT replace stored node names, property names, keyword names, or serialized identifiers with Chinese.

#### Scenario: Chinese mode shows translated title
- **WHEN** Chinese display is enabled and a known English UI key exists in the dictionary
- **THEN** the editor shows the Chinese string while the internal key remains the English original

#### Scenario: Unknown key fails open
- **WHEN** Chinese display is enabled and the key is absent from the dictionary
- **THEN** the editor shows the original English string and continues operating

### Requirement: Generated shader text stays English
Switching display language SHALL NOT change generated Shader source, Keyword names, Define names, property names, or Pass names.

#### Scenario: Language toggle does not alter generated shader
- **WHEN** a shader is generated in Chinese display and again in original display
- **THEN** the two generated texts are identical

### Requirement: Render platform names stay English
Direct3D, Vulkan, PlayStation, and other render-platform names SHALL NOT be translated. Missing platform keys SHALL fail open to the original string.

#### Scenario: Platform checkbox stays English
- **WHEN** Chinese display is enabled and the UI lists render platforms
- **THEN** each platform name is shown in the original English form

### Requirement: Localization failure must not break the editor
A missing dictionary file, a lookup miss, or a display-layer exception SHALL NOT prevent opening or editing shaders.

#### Scenario: Overlay failure leaves editor usable
- **WHEN** the display overlay cannot complete a translation
- **THEN** the original English UI remains and editor commands still run

### Requirement: Dropdowns translate a cloned display copy
Dropdown display text SHALL be translated on a cloned copy. Selection SHALL be read and written by index. Source value arrays SHALL NOT be mutated.

#### Scenario: Blend-op values remain English internally
- **WHEN** a dropdown option is shown in Chinese and the user selects it
- **THEN** the stored value stays the original English option and the source array is unchanged

### Requirement: Single canvas language toggle
The overlay SHALL expose exactly one language Toggle on the ASE canvas toolbar, to the right of the source-code icon. Preferences and node-panel titles SHALL NOT add extra language controls.

#### Scenario: Toggle switches Chinese and original
- **WHEN** the user clicks the canvas language Toggle
- **THEN** display language switches between Chinese and original without marking the shader dirty solely because of the click

### Requirement: Text field values are not translated
The overlay SHALL NOT translate the editable value of a TextField that holds a property name, Pass name, or Inspector name.

#### Scenario: Property name field stays English
- **WHEN** Chinese display is enabled and a property-name TextField is shown
- **THEN** the field value remains the original English identifier
