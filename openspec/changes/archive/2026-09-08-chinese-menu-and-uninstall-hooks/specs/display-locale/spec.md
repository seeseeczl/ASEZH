## ADDED Requirements

### Requirement: ASEZH Window submenu is Chinese
The ASEZH entries under Unity's Window menu SHALL use Chinese item labels. The product folder name `ASEZH` SHALL stay English. These menu paths SHALL be compile-time constants and SHALL NOT follow the canvas language Toggle.

#### Scenario: Installer menu is Chinese
- **WHEN** the user opens `Window/ASEZH`
- **THEN** the items are `接入 Amplify Shader Editor`, `移除汉化补丁`, `重新加载词典`, and `运行本地化测试`

#### Scenario: Canvas language does not retitle Window menu
- **WHEN** the canvas language Toggle is switched to original display
- **THEN** the `Window/ASEZH` item labels stay Chinese
