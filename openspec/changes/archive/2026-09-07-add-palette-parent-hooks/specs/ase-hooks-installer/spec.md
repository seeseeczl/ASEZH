## ADDED Requirements

### Requirement: Installer patches the node Search palette
The installer SHALL patch `PaletteParent.cs` so category foldout labels look up `TableCategory`, node rows use display-only `TNodeListLabel`, and search matches English or Chinese. Dictionary keys and node `Name` values SHALL remain English.

#### Scenario: Built-in categories display Chinese
- **WHEN** Chinese display is on and the Search list contains `Camera And Screen`
- **THEN** that foldout shows `摄像机与屏幕`

#### Scenario: Shader function folders stay as stored
- **WHEN** a shader-function category key is already Chinese (such as `光照`)
- **THEN** the foldout shows that Chinese key without changing the stored category
