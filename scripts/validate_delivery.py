#!/usr/bin/env python3
"""Validate redistributable ASEZH delivery facts with no third-party packages."""

from __future__ import annotations

import argparse
from collections import Counter
import json
from pathlib import Path
import plistlib
import re
import subprocess
import sys
import tempfile
from typing import Any


ROOT = Path(__file__).resolve().parents[1]
EXPECTED_TABLES = {"category", "node_title", "option_label", "option_value", "panel"}
SENSITIVE_NAMES = re.compile(r"(^|/)(\.env($|\.)|id_rsa|.*\.(p12|pfx|pem|key)|credentials?\.json)$", re.I)
CURRENT_MENU_PATH = "Window/ASEZH/接入 Amplify Shader Editor"
REMOVED_MENU_PATHS = (
    "Window/ASEZH/移除汉化补丁",
    "Window/ASEZH/重新加载词典",
    "Window/ASEZH/运行本地化测试",
)
OLD_MENU_LABELS = ("Install into Amplify Shader Editor", "Reload Dictionary", "Run Locale Tests")


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--editor", type=Path, help="Run the synthetic Unity/Tuanjie lifecycle gate")
    parser.add_argument("--real-ase", type=Path, help="Also copy and test this licensed ASE source")
    parser.add_argument("--evidence", type=Path)
    parser.add_argument("--release", action="store_true",
                        help="Require the Tuanjie 2022.3.61t9 + ASE 1.9.81 release target")
    parser.add_argument("--allow-blocked", action="store_true", help=argparse.SUPPRESS)
    return parser.parse_args()


def record(checks: dict[str, Any], name: str, passed: bool, detail: str) -> None:
    checks[name] = {"status": "pass" if passed else "fail", "detail": detail}


def static_checks() -> dict[str, Any]:
    checks: dict[str, Any] = {}
    package = json.loads((ROOT / "package.json").read_text(encoding="utf-8"))
    version = package["version"]
    readme = (ROOT / "README.md").read_text(encoding="utf-8")
    changelog = (ROOT / "CHANGELOG.md").read_text(encoding="utf-8")
    readme_match = re.search(r"当前版本：`([^`]+)`", readme)
    changelog_match = re.search(r"^##\s+([0-9]+\.[0-9]+\.[0-9]+)\b", changelog, re.M)
    metadata_versions = [version, readme_match.group(1) if readme_match else None,
                         changelog_match.group(1) if changelog_match else None]
    record(checks, "version_consistency", len(set(metadata_versions)) == 1,
           " / ".join(str(item) for item in metadata_versions))
    changelog_versions = re.findall(r"^##\s+([0-9]+)\.([0-9]+)\.([0-9]+)\b", changelog, re.M)
    consecutive = len(changelog_versions) >= 2 and changelog_versions[0][:2] == changelog_versions[1][:2] and \
        int(changelog_versions[0][2]) == int(changelog_versions[1][2]) + 1
    record(checks, "version_patch_continuity", consecutive,
           " -> ".join(".".join(parts) for parts in changelog_versions[:2]))

    guide = (ROOT / "docs" / "adapt-ase-version.md").read_text(encoding="utf-8")
    current_menu_docs = readme + "\n" + guide
    installer_source = (ROOT / "Editor" / "Installer" / "ASEZHInstallerWindow.cs").read_text(encoding="utf-8")
    locale_source = (ROOT / "Editor" / "ASELocale.cs").read_text(encoding="utf-8")
    menu_paths = re.findall(r'\[MenuItem\(\s*"(Window/ASEZH/[^"]+)"', installer_source + locale_source)
    missing_actions = [label for label in ("移除汉化补丁", "高级/诊断", "重新加载词典", "运行本地化测试")
                       if label not in installer_source]
    removed_doc_paths = [path for path in REMOVED_MENU_PATHS if path in current_menu_docs.replace(" → ", "/")]
    stale_menus = [label for label in OLD_MENU_LABELS if label in current_menu_docs]
    valid_menu = menu_paths == [CURRENT_MENU_PATH]
    record(checks, "current_menu_paths", valid_menu and not missing_actions and not removed_doc_paths and not stale_menus,
           f"menus={menu_paths} missing_actions={missing_actions} removed_docs={removed_doc_paths} stale={stale_menus}")

    matrix = (ROOT / "docs" / "04-delivery" / "2026-09-08-regression-matrix.md").read_text(encoding="utf-8")
    trace_tokens = ["REL-ASEZH-0008", version, "harden-patcher-and-regression-gates",
                    "REG-PATCH-TRANSACTION-001", "v0.0.8", "Tuanjie 2022.3.61t9", "pass"]
    missing_trace = [token for token in trace_tokens if token not in matrix]
    record(checks, "delivery_traceability", not missing_trace, f"missing={missing_trace}")

    dictionary = json.loads((ROOT / "Editor" / "ASEZHDictionary.json").read_text(encoding="utf-8"))
    entries = dictionary.get("entries", [])
    keys = [(item.get("table"), item.get("key")) for item in entries]
    duplicates = [key for key, count in Counter(keys).items() if count > 1]
    tables = {item.get("table") for item in entries}
    valid_rows = all(isinstance(item.get("key"), str) and item["key"] and
                     isinstance(item.get("zh"), str) and item["zh"] for item in entries)
    record(checks, "dictionary_schema", valid_rows and tables == EXPECTED_TABLES,
           f"entries={len(entries)} tables={sorted(tables)}")
    record(checks, "dictionary_duplicate_keys", not duplicates, f"duplicates={len(duplicates)}")

    imported_roots = [ROOT / "Editor", ROOT / "Tests", ROOT / "scripts"]
    missing_meta: list[str] = []
    guids: list[str] = []
    for imported_root in imported_roots:
        if not imported_root.exists():
            continue
        for path in imported_root.rglob("*"):
            if path.name.endswith(".meta") or path.name.startswith("."):
                continue
            if not Path(str(path) + ".meta").exists():
                missing_meta.append(path.relative_to(ROOT).as_posix())
        for meta in imported_root.rglob("*.meta"):
            match = re.search(r"^guid:\s*([0-9a-f]{32})\s*$", meta.read_text(encoding="utf-8"), re.M)
            if match:
                guids.append(match.group(1))
    duplicate_guids = [guid for guid, count in Counter(guids).items() if count > 1]
    record(checks, "unity_meta_pairs", not missing_meta, f"missing={missing_meta}")
    record(checks, "unity_meta_guids", not duplicate_guids, f"duplicates={duplicate_guids}")

    inventory = subprocess.run(
        ["git", "ls-files", "--cached", "--others", "--exclude-standard"],
        cwd=ROOT, check=False, text=True, capture_output=True,
    ).stdout.splitlines()
    sensitive = [name for name in inventory if SENSITIVE_NAMES.search(name)]
    record(checks, "sensitive_file_names", not sensitive, f"matches={sensitive}")

    oversized = []
    for source in (ROOT / "Editor").rglob("*.cs"):
        lines = len(source.read_text(encoding="utf-8").splitlines())
        if lines > 400:
            oversized.append(f"{source.relative_to(ROOT)}:{lines}")
    record(checks, "production_loc", not oversized, f"over_400={oversized}")

    openspec = subprocess.run(
        ["openspec", "validate", "--all", "--strict"], cwd=ROOT, check=False,
        text=True, capture_output=True,
    )
    record(checks, "openspec_strict", openspec.returncode == 0,
           (openspec.stdout + openspec.stderr).strip()[-1000:])
    completed_active = []
    changes_dir = ROOT / "openspec" / "changes"
    for tasks in changes_dir.glob("*/tasks.md"):
        text = tasks.read_text(encoding="utf-8")
        if "- [x]" in text and "- [ ]" not in text:
            completed_active.append(tasks.parent.name)
    record(checks, "no_completed_active_change", not completed_active,
           f"completed_active={completed_active}")
    return checks


def editor_identity(editor: Path) -> tuple[str, str]:
    family = "tuanjie" if "tuanjie" in str(editor).lower() else "unknown"
    version = "unknown"
    info = editor.parents[1] / "Info.plist" if len(editor.parents) > 1 else None
    if info and info.is_file():
        try:
            version = str(plistlib.loads(info.read_bytes()).get("CFBundleVersion", "unknown"))
        except (OSError, ValueError, plistlib.InvalidFileException):
            pass
    return family, version


def run_gate(editor: Path, ase_source: Path | None, evidence: Path) -> tuple[str, str, dict[str, Any]]:
    command = [sys.executable, str(ROOT / "scripts" / "run_ase_regression.py"),
               "--editor", str(editor), "--evidence", str(evidence)]
    if ase_source:
        command += ["--ase-source", str(ase_source)]
    completed = subprocess.run(command, cwd=ROOT, check=False, text=True, capture_output=True)
    manifest = evidence / "manifest.json"
    status = "fail"
    if manifest.exists():
        data = json.loads(manifest.read_text(encoding="utf-8"))
        status = data.get("status", "fail")
    else:
        data = {}
    return status, (completed.stdout + completed.stderr).strip(), data


def main() -> int:
    args = parse_args()
    evidence = (args.evidence or Path(tempfile.mkdtemp(prefix="asezh-delivery-evidence-"))).resolve()
    evidence.mkdir(parents=True, exist_ok=True)
    report: dict[str, Any] = {"schema": 1, "checks": static_checks(), "gates": {}}
    if args.release:
        if not args.editor or not args.real_ase:
            report["gates"]["tuanjie_release_target"] = {
                "status": "fail", "detail": "release mode requires --editor and --real-ase"
            }
        else:
            family, version = editor_identity(args.editor)
            identity_ok = family == "tuanjie" and version == "2022.3.61t9"
            report["gates"]["target_editor"] = {
                "status": "pass" if identity_ok else "fail",
                "detail": f"family={family} version={version}",
            }
            if identity_ok:
                status, detail, _ = run_gate(args.editor, None, evidence / "synthetic")
                report["gates"]["synthetic_tuanjie"] = {"status": status, "detail": detail}
                status, detail, real = run_gate(args.editor, args.real_ase, evidence / "real-ase")
                if real.get("ase_version") != "1.9.81":
                    status = "fail"
                    detail += f" ase_version={real.get('ase_version')} expected=1.9.81"
                report["gates"]["tuanjie_real_ase"] = {"status": status, "detail": detail}
    else:
        report["gates"]["tuanjie_release_target"] = {
            "status": "not-run", "detail": "static/public validation only; use --release for publication"
        }

    required_statuses = [item["status"] for item in report["checks"].values()]
    if args.release:
        required_statuses += [item["status"] for item in report["gates"].values()]
    report["status"] = "fail" if "fail" in required_statuses else "pass"
    manifest = evidence / "delivery-manifest.json"
    manifest.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps({"status": report["status"], "manifest": str(manifest)}, ensure_ascii=False))
    return 0 if report["status"] == "pass" else 1


if __name__ == "__main__":
    raise SystemExit(main())
