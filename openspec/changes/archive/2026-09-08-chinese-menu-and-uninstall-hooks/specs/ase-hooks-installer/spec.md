## ADDED Requirements

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
