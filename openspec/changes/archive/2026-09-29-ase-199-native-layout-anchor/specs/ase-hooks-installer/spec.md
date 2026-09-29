## ADDED Requirements

### Requirement: Hook anchors survive ASE signature drift
When a supported ASE release changes only the parameter list of a hooked method, the installer SHALL try every anchor declared for that hook and SHALL preserve the source's own signature line in the replacement. It SHALL report such a hook as `ready` before Apply and as `applied` after Apply, never `mismatch`. Signature drift that no declared anchor covers SHALL remain `mismatch` and SHALL write nothing.

#### Scenario: OnNodeLayout with an added cache parameter
- **WHEN** `ParentNode.cs` declares `public virtual void OnNodeLayout( DrawInfo drawInfo, NodeUpdateCache cache = null )`
- **THEN** `native-language-layout` is `ready`, Apply inserts the layout guard after the opening brace, and the signature line is unchanged

#### Scenario: Previous OnNodeLayout signature stays ready
- **WHEN** `ParentNode.cs` declares `public virtual void OnNodeLayout( DrawInfo drawInfo )`
- **THEN** `native-language-layout` is still `ready` or `applied`

#### Scenario: Rescan after apply on the drifted signature
- **WHEN** the layout guard was applied on the drifted signature
- **THEN** Scan reports `applied` and does not report `mismatch`

#### Scenario: Undeclared signature drift stays fail-closed
- **WHEN** `ParentNode.cs` declares an `OnNodeLayout` parameter list that no declared anchor covers
- **THEN** the installer reports `mismatch` and writes nothing
