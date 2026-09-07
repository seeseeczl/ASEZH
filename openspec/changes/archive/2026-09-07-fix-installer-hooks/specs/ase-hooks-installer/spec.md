## Purpose

Defines how the ASEZH installer attaches display hooks to Amplify Shader Editor source without breaking compilation across ASE formatting variants.

## ADDED Requirements

### Requirement: Hook matching ignores whitespace
The installer SHALL treat tab/space and newline differences as the same when matching Find and Marker text. A stock ASE method whose tokens match a catalog Find SHALL be `ready` or `patched`, not `mismatch`, solely because indent differs.

#### Scenario: Space-indented EnumPopup is ready
- **WHEN** `EditorGUILayoutEnumPopup( string label, Enum selected, ... )` exists with space indent instead of tabs
- **THEN** `undo-enum-string` is `ready` (or `applied` if already hooked), not `mismatch`

### Requirement: ZWrite patch defines labels before using them
The ZWrite installer step SHALL insert a `ZWriteModeLabels` array whose English entries equal `ZWriteModeValues`, SHALL point the depth Popup at Labels, and SHALL leave `ZWriteModeValues` as the Shader write source. It SHALL NOT report `applied` until both the array definition and the Popup Labels usage exist.

#### Scenario: Clean ASE compiles after ZWrite patch
- **WHEN** a stock `ZBufferOpHelper.cs` has only `ZWriteModeValues` and the installer applies `zwrite-labels`
- **THEN** the file defines `ZWriteModeLabels` and the editor compiles without CS0103

#### Scenario: Partial ZWrite replace is not applied
- **WHEN** Popup already mentions `ZWriteModeLabels` but the array is missing
- **THEN** scan is not `applied` and apply inserts the array

### Requirement: Language toggle hooks after the source-code icon
The installer SHALL insert `ASELocale.DrawLanguageToggle` on the ASE canvas toolbar to the right of the source-code icon, after restoring `GUI.color`, at height 21.

#### Scenario: ToolsWindow without blank line still patches
- **WHEN** `openSourceCodeButton.Draw` is immediately followed by `GUI.color = bufferedColor` with one newline
- **THEN** `tools-language-toggle` applies instead of mismatch
