# ASEZH 外部 ASE 回归协议

## 边界

- 真实 ASE 必须来自维护者有权使用的外部副本，通过 `--ase-source` 注入隔离临时工程。
- 脚本只修改临时副本；ASE 源码、完整 Editor.log 和用户工程内容不得进入仓库或公共 CI artifact。
- 正式发布目标固定为团结引擎 2022.3.61t9 + ASE 1.9.81；其必选项使用 `pass` / `fail`。
- Shader 对比、真实画布交互、完整无障碍、其他 Unity 和 Windows 是扩展观察项，未执行时必须记录为 `not-run` 或 `unverified`，不得伪装成通过。

## 统一命令

公开静态检查（公共 CI 使用，不代表团结正式门禁通过）：

```bash
python3 scripts/validate_delivery.py \
  --evidence /safe/output/public
```

合成事务生命周期（本地诊断）：

```bash
python3 scripts/run_ase_regression.py \
  --editor /Applications/Tuanjie/Hub/Editor/2022.3.61t9/Tuanjie.app/Contents/MacOS/Tuanjie \
  --evidence /safe/output/synthetic
```

许可真实 ASE 生命周期：

```bash
python3 scripts/run_ase_regression.py \
  --editor /Applications/Tuanjie/Hub/Editor/2022.3.61t9/Tuanjie.app/Contents/MacOS/Tuanjie \
  --ase-source /licensed/path/AmplifyShaderEditor \
  --ase-version 1.9.81 \
  --evidence /safe/output/real-ase
```

消费者交付总门禁：

```bash
python3 scripts/validate_delivery.py \
  --release \
  --editor /Applications/Tuanjie/Hub/Editor/2022.3.61t9/Tuanjie.app/Contents/MacOS/Tuanjie \
  --real-ase /licensed/path/AmplifyShaderEditor \
  --evidence /safe/output/delivery
```

退出码：`0=pass`、`1=fail`。不带 `--release` 时只验证公开静态项，并将团结正式门禁记为 `not-run`。

## 自动阶段

同一个全新隔离工程依次运行，并在 Apply/Remove 后用新的 Editor 进程触发真实重编译：

1. `Locale`：包导入、Editor 编译、词典自测。
2. `Baseline`：唯一目标与全部受管补丁状态预检。
3. `Apply`：事务接入，记录 session、目标和 backup。
4. `VerifyApplied`：重启后编译并确认全部适用钩子为 applied。
5. `Remove`：按安装回执恢复批准的 preimage。
6. `VerifyRemoved`：重启后编译、零残留扫描和受管文件 SHA-256 对账。

## 非阻塞扩展观察项

当前脚本不会伪造以下证据；没有对应自动化或人工记录时必须是 `not-run` 或 `unverified`：

- 同一 Shader 在中文/原文显示下生成文本的 SHA-256 完全相同。
- Search 中英文命中、下拉显示值与内部值分离。
- 画布 Toggle 不置 dirty、不触发 Live，且只有一个入口。
- 菜单、Installer、键盘焦点、取消/恢复、150%-200% 缩放和 Windows UI 截图验收。

每份证据清单至少记录：包版本、commit、引擎版本、ASE 版本、OS、阶段状态、测试 XML/日志索引、受管文件前后哈希、Shader 哈希、截图索引和脱敏说明。

## 发布判定

- 团结 2022.3.61t9 + ASE 1.9.81 的任一必选项 `fail` 或不可用：发布失败。
- 团结正式目标的全部必选项为 `pass`：允许更新不可变版本引用。
- 扩展观察项即使是 `not-run` / `unverified` 也不阻止该团结目标发布，但必须原样披露。
- tag、GitHub Release、分支保护和远端发布仍需维护者单独授权。
