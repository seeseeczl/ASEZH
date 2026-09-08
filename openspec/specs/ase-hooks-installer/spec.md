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

### Requirement: Installer can remove applied display hooks
The installer SHALL provide a remove action that restores ASE source fragments the installer previously rewrote, after a confirmation dialog. Remove SHALL NOT uninstall the ASEZH package. Remove SHALL leave `ZWriteModeValues` as the Shader write source. Remove SHALL last drop the ASE assembly reference to ASEZH if the installer added it.

#### Scenario: Remove restores a catalog hook
- **WHEN** a stock-ASE file has an applied catalog hook and the user confirms remove
- **THEN** that file no longer contains the display-hook replacement text and again contains the original Find fragment

#### Scenario: Remove does not delete the package
- **WHEN** the user confirms remove
- **THEN** the ASEZH package remains imported and the Window/ASEZH menu still exists

#### Scenario: Remove keeps Shader ZWrite values
- **WHEN** `ZWriteModeLabels` was inserted by the installer and the user confirms remove
- **THEN** `ZWriteModeLabels` is gone, the depth Popup uses `ZWriteModeValues`, and `ZWriteModeValues` English entries are unchanged

### Requirement: Equivalent palette Search-label width hooks are recognized safely
The installer SHALL report the Search-label width hook as applied when one local string variable is assigned the result of translating `m_searchFilterStr` and that same variable is passed to the label-size calculation. It SHALL NOT accept an untranslated value or a different variable as equivalent. Apply SHALL leave an accepted equivalent implementation unchanged. Remove without a valid installer receipt SHALL preserve the equivalent implementation and fail closed if its localization references would otherwise remain; Remove with a valid receipt SHALL restore the recorded preimage.

#### Scenario: Equivalent local variable is already applied
- **WHEN** one local variable receives the translated Search label and the same variable is measured for the Search-label width
- **THEN** Scan and Apply report the width hook as applied without rewriting the file

#### Scenario: Different variable is not equivalent
- **WHEN** the translated Search label is assigned to one variable but the width calculation measures another variable
- **THEN** the width hook is not reported as applied

#### Scenario: Untranslated variable is not equivalent
- **WHEN** the width calculation measures a local variable that was assigned the raw Search label
- **THEN** the width hook is not reported as applied

#### Scenario: Receipt-free removal preserves pre-existing equivalent code
- **WHEN** Remove encounters an equivalent local-variable implementation and no valid installer receipt exists
- **THEN** it does not rewrite that implementation and the session does not falsely report a complete removal

#### Scenario: Receipt-owned removal restores preimage
- **WHEN** a valid installer receipt covers the file containing the equivalent implementation
- **THEN** Remove restores the recorded preimage transactionally

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
