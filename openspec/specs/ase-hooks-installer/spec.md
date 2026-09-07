# ase-hooks-installer Specification

## Purpose
Defines how the ASEZH installer attaches display hooks to Amplify Shader Editor source without breaking compilation across ASE formatting variants.
## Requirements
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

#### Scenario: ZWrite patch does not break ZTestModeDict
- **WHEN** `ZBufferOpHelper.cs` already has `ZWriteModeValues` closed with `};` and `ZTestModeDict` follows
- **THEN** apply does not insert a semicolon after `{ZTestMode.Less,1 }`

#### Scenario: Broken ZTestModeDict semicolon is repaired
- **WHEN** the first `ZTestModeDict` entry is `{ZTestMode.Less,1 };` from an older installer
- **THEN** apply rewrites it to a comma so the file compiles

### Requirement: Language toggle hooks after the source-code icon
The installer SHALL insert `ASELocale.DrawLanguageToggle` on the ASE canvas toolbar to the right of the source-code icon, after restoring `GUI.color`, at height 21.

#### Scenario: ToolsWindow without blank line still patches
- **WHEN** `openSourceCodeButton.Draw` is immediately followed by `GUI.color = bufferedColor` with one newline
- **THEN** `tools-language-toggle` applies instead of mismatch

### Requirement: Installer patches the node Search palette
The installer SHALL patch `PaletteParent.cs` so category foldout labels look up `TableCategory`, node rows use display-only `TNodeListLabel`, and search matches English or Chinese. Dictionary keys and node `Name` values SHALL remain English.

#### Scenario: Built-in categories display Chinese
- **WHEN** Chinese display is on and the Search list contains `Camera And Screen`
- **THEN** that foldout shows `摄像机与屏幕`

#### Scenario: Shader function folders stay as stored
- **WHEN** a shader-function category key is already Chinese (such as `光照`)
- **THEN** the foldout shows that Chinese key without changing the stored category

#### Scenario: Empty Search still lists nodes
- **WHEN** Chinese display is on and the Search field is empty or still holds the Search label `搜索`
- **THEN** the node list still shows all categories and items

#### Scenario: Palette build list does not skip the unfiltered pass
- **WHEN** the installer applies `palette-build-list`
- **THEN** it does not insert `if( !ASELocale.MatchesSearch(...) ) continue` before adding items in the empty-search loop

