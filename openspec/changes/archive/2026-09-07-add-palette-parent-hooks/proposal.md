## Why

安装器 11 条都 applied 后，同事的 Search 窗口上半截分类仍是 `Camera And Screen` 等英文。这些是 ASE 内置节点分类，显示在 `PaletteParent.cs`。该文件从未进入安装器目录。下半截中文是 Shader Function 目录本身就是中文 key，不经词典也会显示中文。

## What Changes

- 安装器增加 `PaletteParent.cs` 钩子：分类折叠 `T(key, TableCategory)`、列表项 `TNodeListLabel`、搜索框 `T("Search")`、中英搜索 `MatchesSearch`。
- 不改 `current.Key` / `Name` 存储值；创建节点仍传英文 `Name`。

## Capabilities

### New Capabilities

- （无）

### Modified Capabilities

- `ase-hooks-installer`: 安装器必须接入 `PaletteParent`，否则 Search 分类列表在干净 ASE 上会半英半中。

## Impact

- `Editor/Installer/ASEZHPatcher.cs`
- 同事更新后需再点一次「应用可自动补丁」
