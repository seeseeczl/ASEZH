## ADDED Requirements

### Requirement: Native title coverage is complete and auditable
Chinese mode SHALL translate the canvas title and palette label of every shipped built-in ASE node. The native allowlist SHALL include every built-in node type, the `node_title` table SHALL cover every allowlisted node name, and the `category` table SHALL cover every category those nodes use. Custom subclasses, Flyme categories and renamed nodes SHALL keep their original labels.

#### Scenario: Built-in node outside the allowlist
- **WHEN** a shipped built-in node type is missing from the native allowlist
- **THEN** the audit command reports it under `node_types_not_allowlisted` instead of leaving its title silently in English

#### Scenario: Allowlisted node without a dictionary entry
- **WHEN** an allowlisted node name has no `node_title` entry
- **THEN** the audit command reports it under `node_titles_missing`

#### Scenario: Node that draws its own title
- **WHEN** a node draws its title directly instead of through `ParentNode`
- **THEN** that drawing site is hooked so the Chinese title still appears

#### Scenario: Renamed or custom node stays untouched
- **WHEN** a node has been renamed by the user or belongs to a Flyme category
- **THEN** its title stays exactly as stored
