## MODIFIED Requirements

### Requirement: ASEZH Window submenu is Chinese
The ASEZH entries under Unity's Window menu SHALL use Chinese item labels. The product folder name `ASEZH` SHALL stay English. `Window/ASEZH` SHALL expose only `接入 Amplify Shader Editor` as the normal entry. Remove, dictionary reload, and locale self-test capabilities SHALL remain available inside the installer window and SHALL NOT follow the canvas language Toggle.

#### Scenario: Window submenu has one normal entry
- **WHEN** the user opens `Window/ASEZH`
- **THEN** the only item is `接入 Amplify Shader Editor`

#### Scenario: Maintenance actions stay available in the installer
- **WHEN** the user opens the installer window and expands `高级/诊断`
- **THEN** `重新加载词典` and `运行本地化测试` are available, while `移除汉化补丁` remains available in the main installer actions

#### Scenario: Canvas language does not retitle Window menu
- **WHEN** the canvas language Toggle is switched to original display
- **THEN** the `Window/ASEZH` item label stays Chinese
