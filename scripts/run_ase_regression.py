#!/usr/bin/env python3
"""Run ASEZH lifecycle checks in an isolated Unity/Tuanjie project.

With --ase-source this copies a licensed ASE installation into the temporary
project. Without it, only the repository's redistributable synthetic fixture is
used. Proprietary source and full Editor logs stay outside the repository.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import subprocess
import tempfile
from typing import Any


ROOT = Path(__file__).resolve().parents[1]
FIXTURE = ROOT / "Tests" / "Fixtures" / "FakeASE"
PROBE = ROOT / "Tests" / "Integration" / "ASEZHGateProbe.cs.txt"
STAGES = ("Locale", "Baseline", "Apply", "VerifyApplied", "Remove", "VerifyRemoved")
STAGE_MARKERS = {
    "Locale": "locale",
    "Baseline": "baseline",
    "Apply": "apply",
    "VerifyApplied": "verify-applied",
    "Remove": "remove",
    "VerifyRemoved": "verify-removed",
}
MANAGED_NAMES = {
    "AmplifyShaderEditor.asmdef", "UndoParentNode.cs", "ToolsWindow.cs",
    "NodeUtils.cs", "ParentNode.cs", "ZBufferOpHelper.cs", "PaletteParent.cs",
}


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--editor", required=True, type=Path)
    parser.add_argument("--ase-source", type=Path)
    parser.add_argument("--ase-version")
    parser.add_argument("--evidence", type=Path, required=True)
    parser.add_argument("--keep-project", action="store_true")
    parser.add_argument("--timeout", type=int, default=600)
    return parser.parse_args()


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def tree_hashes(root: Path) -> dict[str, str]:
    return {
        path.relative_to(root).as_posix(): sha256(path)
        for path in sorted(root.rglob("*"))
        if path.is_file() and path.name in MANAGED_NAMES
    }


def fixture_version(source: Path | None, explicit: str | None) -> str:
    if explicit:
        return explicit
    if source and (source / "package.json").is_file():
        return str(json.loads((source / "package.json").read_text(encoding="utf-8")).get("version", "unknown"))
    return "synthetic"


def prepare_project(project: Path, ase_source: Path | None) -> Path:
    assets = project / "Assets"
    editor = assets / "Editor"
    packages = project / "Packages"
    settings = project / "ProjectSettings"
    editor.mkdir(parents=True)
    packages.mkdir(parents=True)
    settings.mkdir(parents=True)
    (settings / "ProjectVersion.txt").write_text("m_EditorVersion: 2022.3.0f1\n", encoding="utf-8")

    dependencies: dict[str, str] = {"com.asezh.locale": f"file:{ROOT}"}
    if ase_source and any("Newtonsoft" in p.read_text(encoding="utf-8", errors="ignore")
                          for p in ase_source.rglob("PhraseWindow.cs")):
        dependencies["com.unity.nuget.newtonsoft-json"] = "3.2.1"
    (packages / "manifest.json").write_text(
        json.dumps({"dependencies": dependencies}, ensure_ascii=False, indent=2) + "\n",
        encoding="utf-8",
    )
    shutil.copy2(PROBE, editor / "ASEZHGateProbe.cs")

    target = assets / "AmplifyShaderEditor"
    if ase_source:
        shutil.copytree(ase_source, target)
    else:
        target_editor = target / "Plugins" / "Editor"
        target_editor.mkdir(parents=True)
        for template in FIXTURE.glob("*.txt"):
            if template.name.endswith(".meta"):
                continue
            shutil.copy2(template, target_editor / template.name.removesuffix(".txt"))
    return target


def run_stage(editor: Path, project: Path, evidence_dir: Path, stage: str, timeout: int) -> dict[str, Any]:
    log = evidence_dir / f"{stage.lower()}.log"
    command = [
        str(editor), "-batchmode", "-nographics", "-projectPath", str(project),
        "-executeMethod", f"ASEZHGateProbe.{stage}", "-logFile", str(log),
    ]
    completed = subprocess.run(command, timeout=timeout, check=False)
    text = log.read_text(encoding="utf-8", errors="replace") if log.exists() else ""
    marker = f"ASEZH_GATE_OK stage={STAGE_MARKERS[stage]}"
    return {
        "stage": stage,
        "status": "pass" if completed.returncode == 0 and marker in text else "fail",
        "exit_code": completed.returncode,
        "marker": marker in text,
        "log": str(log),
    }


def main() -> int:
    args = parse_args()
    if not args.editor.is_file():
        raise SystemExit(f"Editor executable not found: {args.editor}")
    if args.ase_source and not args.ase_source.is_dir():
        raise SystemExit(f"ASE source not found: {args.ase_source}")

    evidence = args.evidence.resolve()
    evidence.mkdir(parents=True, exist_ok=True)
    work = Path(tempfile.mkdtemp(prefix="asezh-regression-"))
    project = work / "project"
    mode = "licensed-real-ase" if args.ase_source else "redistributable-synthetic"
    report: dict[str, Any] = {
        "schema": 1,
        "mode": mode,
        "status": "fail",
        "os": platform.platform(),
        "editor": str(args.editor),
        "ase_version": fixture_version(args.ase_source, args.ase_version),
        "package_version": json.loads((ROOT / "package.json").read_text(encoding="utf-8"))["version"],
        "project": str(project) if args.keep_project else "temporary-redacted",
        "stages": [],
        "checks": {},
    }
    try:
        ase_target = prepare_project(project, args.ase_source)
        before: dict[str, str] | None = None
        for stage in STAGES:
            outcome = run_stage(args.editor, project, evidence, stage, args.timeout)
            report["stages"].append(outcome)
            if outcome["status"] != "pass":
                break
            if stage == "Baseline":
                before = tree_hashes(ase_target)
        after = tree_hashes(ase_target)
        restored = before is not None and before == after
        report["checks"]["apply_remove_preimage_restored"] = "pass" if restored else "fail"
        lifecycle_pass = len(report["stages"]) == len(STAGES) and all(
            stage["status"] == "pass" for stage in report["stages"]
        ) and restored
        if args.ase_source:
            report["advisory_checks"] = {
                "shader_equivalence": "unverified",
                "live_chinese_english_search": "unverified",
                "live_toggle_dirty_state": "unverified",
                "real_ui_accessibility": "unverified",
                "other_unity_versions": "unverified",
                "windows": "unverified",
            }
            report["checks"].update({
                "locale_self_tests": "pass" if report["stages"] and report["stages"][0]["status"] == "pass" else "fail",
                "tuanjie_ase_lifecycle": "pass" if lifecycle_pass else "fail",
            })
            report["status"] = "pass" if lifecycle_pass else "fail"
        else:
            report["status"] = "pass" if lifecycle_pass else "fail"
    except Exception as error:  # keep a machine-readable failure even on setup errors
        report["error"] = f"{type(error).__name__}: {error}"
        report["status"] = "fail"
    finally:
        (evidence / "manifest.json").write_text(
            json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8"
        )
        if not args.keep_project:
            shutil.rmtree(work, ignore_errors=True)

    print(json.dumps({"status": report["status"], "manifest": str(evidence / "manifest.json")}, ensure_ascii=False))
    return 0 if report["status"] == "pass" else 1


if __name__ == "__main__":
    raise SystemExit(main())
