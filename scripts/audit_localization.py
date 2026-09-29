#!/usr/bin/env python3
"""只读扫描 ASE 源码，报告显示层汉化的覆盖缺口。

需要 ASE 许可源码，只能在维护者机器上运行；源码不会被复制进仓库。
三类缺口：
  hooked_missing   已经接了钩子，但词典查不到这条文案（静默漏译）
  wrapper_missing  经 UndoParentNode 包装方法显示，但词典没有对应词条
  unhooked         界面会显示，但源码里没有钩子（候选清单，含间接绘制路径，需要人工判断）
  node_*           节点标题：类型不在原生白名单、白名单节点缺词、分类缺词
"""

from __future__ import annotations

import argparse
import json
import re
import sys
from collections import defaultdict
from pathlib import Path
from typing import Any

ROOT = Path(__file__).resolve().parents[1]
STRING = r'"((?:\\.|[^"\\])*)"'
HOOKS = ("ASELocale.", "ASENativeDisplay.", "ASESettingsDisplay.")

HOOK_CALLS = (
    re.compile(r"(?:ASESettingsDisplay\.Label|ASELocale\.T|ASELocale\.TranslateArray|"
               r"ASELocale\.TranslateContents|ASENativeDisplay\.Title|ASENativeDisplay\.ListLabel)"
               r"\s*\(\s*" + STRING),
    re.compile(r"(?:ASESettingsDisplay\.Label|ASELocale\.T|ASELocale\.TranslateArray|"
               r"ASENativeDisplay\.Title|ASENativeDisplay\.ListLabel)\s*\(\s*([A-Za-z_][\w.]*)\s*[,)]"),
)

WRAPPERS = (
    "EditorGUILayoutStringField", "EditorGUILayoutTextField", "EditorGUILayoutEnumPopup",
    "EditorGUILayoutIntPopup", "EditorGUILayoutPopup", "EditorGUILayoutToggle",
    "EditorGUILayoutToggleLeft", "EditorGUILayoutIntField", "EditorGUILayoutFloatField",
    "EditorGUILayoutRangedFloatField", "EditorGUILayoutColorField", "EditorGUILayoutSlider",
    "EditorGUILayoutIntSlider", "EditorGUILayoutObjectField", "EditorGUILayoutTextArea",
    "EditorGUILayoutFoldout",
)
WRAPPER_CALL = re.compile(r"\b(" + "|".join(WRAPPERS) + r")\s*\(\s*" + STRING)

CAPTION_CALL = re.compile(
    r"(EditorGUILayout|EditorGUI|GUILayout|GUI)\s*\.\s*"
    r"(Label|LabelField|Button|Toggle|ToggleLeft|Foldout|HelpBox|MiniLabel|Header|"
    r"SelectionGrid|Popup|IntPopup|EnumPopup|TextArea|Box|PropertyField)\s*\(")
GUICONTENT = re.compile(r"new\s+GUIContent\s*\(|GUIContent\s*\(\s*\"")
WINDOW = re.compile(r"(GetWindow|ShowNotification|DisplayDialog|EditorUtility\.DisplayDialog)")
MENUITEM = re.compile(r"MenuItem\s*\(")
STYLE_ARG = re.compile(r"new\s+GUIStyle\s*\(|GUIStyle\s*\(")
NODE_ATTR = re.compile(r"\[NodeAttributes\s*\(")
CLASS_DECL = re.compile(r"\bclass\s+(\w+)")

SKIP_EXACT = {"Amplify Shader Editor", "Amplify Shader Function"}
SKIP_PATTERNS = (
    re.compile(r"^[A-Za-z_][A-Za-z0-9_]*(\.[A-Za-z_][A-Za-z0-9_]*)+$"),  # 点分类型名
    re.compile(r"^[0-9a-f]{16,}$"),                                        # GUID
    re.compile(r"\.(png|jpg|tga|psd|shader|cs|json|asset|mat)$", re.I),    # 资源路径
    re.compile(r"^[\w/\\ .-]*/"),                                          # 含路径分隔
    re.compile(r"^Window/"),                                               # ASE 自己的菜单路径
)


def decode(raw: str) -> str:
    try:
        return json.loads('"' + raw + '"')
    except ValueError:
        return raw


def strip_comments(text: str) -> str:
    """把注释替换成空白，保留字符串字面量。"""
    out = list(text)
    i, n, in_string = 0, len(text), False
    while i < n:
        c = text[i]
        if in_string:
            if c == "\\":
                i += 2
                continue
            if c == '"':
                in_string = False
            i += 1
            continue
        if c == '"':
            in_string = True
            i += 1
            continue
        if c == "/" and i + 1 < n and text[i + 1] == "/":
            j = text.find("\n", i)
            j = n if j < 0 else j
            for k in range(i, j):
                out[k] = " "
            i = j
            continue
        if c == "/" and i + 1 < n and text[i + 1] == "*":
            j = text.find("*/", i + 2)
            j = n if j < 0 else j + 2
            for k in range(i, j):
                if text[k] != "\n":
                    out[k] = " "
            i = j
            continue
        i += 1
    return "".join(out)


def literals(line: str) -> list[str]:
    found = []
    for match in re.finditer(STRING, line):
        found.append(decode(match.group(1)))
    return found


def read_literal(src: str, index: int) -> tuple[str, int]:
    """从 index 处的引号开始读一个 C# 字符串字面量（含 \\u 转义）。"""
    index += 1
    out = []
    escapes = {"n": "\n", "t": "\t", "r": "\r", '"': '"', "\\": "\\"}
    while index < len(src):
        char = src[index]
        if char == "\\":
            nxt = src[index + 1]
            if nxt == "u":
                out.append(chr(int(src[index + 2:index + 6], 16)))
                index += 6
                continue
            out.append(escapes.get(nxt, nxt))
            index += 2
            continue
        if char == '"':
            return "".join(out), index + 1
        out.append(char)
        index += 1
    raise ValueError("unterminated literal")


def node_declarations(source: Path):
    """产出 (类型全名, 节点名, 分类, 文件) —— 逐个 [NodeAttributes]。"""
    for path in sorted((source / "Plugins" / "Editor").rglob("*.cs")):
        src = path.read_text(encoding="utf-8", errors="replace")
        for match in NODE_ATTR.finditer(src):
            index, args = match.end(), []
            while len(args) < 3:
                while index < len(src) and src[index] in " \t\r\n":
                    index += 1
                if index >= len(src) or src[index] != '"':
                    break
                value, index = read_literal(src, index)
                args.append(value)
                while index < len(src) and src[index] in " \t\r\n":
                    index += 1
                if index < len(src) and src[index] == ",":
                    index += 1
                    continue
                break
            if len(args) < 2:
                continue
            owner = CLASS_DECL.search(src[index:index + 600])
            yield (f"AmplifyShaderEditor.{owner.group(1) if owner else '?'}",
                   args[0], args[1], str(path.relative_to(source)))


def looks_display(value: str) -> bool:
    if not value or len(value) > 70 or not re.search(r"[A-Za-z]", value):
        return False
    if value.startswith(("m_", "http", "#", "Assets/", "Packages/")):
        return False
    if value.endswith((".cs", ".shader", ".json", ".asset", ".meta", ".unitypackage", ".txt")):
        return False
    if re.fullmatch(r"[A-Za-z_][A-Za-z0-9_]*", value) or re.fullmatch(r"[A-Z0-9_]{2,}", value):
        return False
    return not value.startswith(("( ", " (", "[", "{"))


def display_kind(line: str) -> str | None:
    if CAPTION_CALL.search(line):
        return "caption"
    if GUICONTENT.search(line):
        return "guicontent"
    if WINDOW.search(line):
        return "window"
    if MENUITEM.search(line):
        return "menu"
    return None


def audit(source: Path) -> dict[str, Any]:
    entries = json.loads((ROOT / "Editor/ASEZHDictionary.json").read_text(encoding="utf-8"))["entries"]
    known = {item["key"] for item in entries}
    normalized = {key.strip() for key in known}          # 运行时也会去掉首尾空格再查一次

    hooked_missing: dict[str, list[str]] = defaultdict(list)
    wrapper_missing: dict[str, list[str]] = defaultdict(list)
    unhooked: dict[str, list[str]] = defaultdict(list)
    hooked_hits = wrapper_hits = 0

    for path in sorted((source / "Plugins" / "Editor").rglob("*.cs")):
        text = path.read_text(encoding="utf-8", errors="replace")
        rel = f"{path.relative_to(source)}"
        clean = strip_comments(text)
        constants = {m[0]: decode(m[1])
                     for m in re.findall(r"\bstring\s+(\w+)\s*=\s*" + STRING + r"\s*;", text)}
        for line_number, line in enumerate(clean.splitlines(), 1):
            for match in HOOK_CALLS[0].finditer(line):
                key = decode(match.group(1))
                if not re.search(r"[A-Za-z]", key):
                    continue
                if key.strip() in normalized:
                    hooked_hits += 1
                else:
                    hooked_missing[key].append(f"{rel}:{line_number}")
            for match in HOOK_CALLS[1].finditer(line):
                key = constants.get(match.group(1).split(".")[-1])
                if key is None or not re.search(r"[A-Za-z]", key):
                    continue
                if key.strip() in normalized:
                    hooked_hits += 1
                else:
                    hooked_missing[key].append(f"{rel}:{line_number}")
            for match in WRAPPER_CALL.finditer(line):
                key = decode(match.group(2))
                if not re.search(r"[A-Za-z]", key):
                    continue
                if key.strip() in normalized:
                    wrapper_hits += 1
                else:
                    wrapper_missing[key].append(f"{rel}:{line_number}")
            if any(prefix in line for prefix in HOOKS) or STYLE_ARG.search(line):
                continue
            kind = display_kind(line)
            if kind is None:
                continue
            for value in literals(line):
                candidate = value.strip()
                if not looks_display(candidate) or candidate in SKIP_EXACT:
                    continue
                if any(rx.search(candidate) for rx in SKIP_PATTERNS):
                    continue
                # 没有钩子的显示点即使词典里有词也不会被翻译，所以这里不按词典过滤。
                unhooked[candidate].append(f"{kind}\t{rel}:{line_number}")

    def pack(data: dict[str, list[str]]) -> dict[str, Any]:
        return {key: sorted(value) for key, value in sorted(data.items())}

    allow = set(re.findall(r'"(AmplifyShaderEditor\.[A-Za-z0-9_]+)"',
                           (ROOT / "Editor/ASENativeTypes.cs").read_text(encoding="utf-8")))
    node_titles = {item["key"] for item in entries if item["table"] == "node_title"}
    category_keys = {item["key"] for item in entries if item["table"] == "category"}
    not_allowlisted: dict[str, list[str]] = defaultdict(list)
    node_titles_missing: dict[str, list[str]] = defaultdict(list)
    categories_missing: dict[str, list[str]] = defaultdict(list)
    for full_name, name, category, path in node_declarations(source):
        if "flyme" in category.lower():
            continue
        if full_name not in allow:
            not_allowlisted[name].append(f"{full_name} ({path})")
            continue
        if name not in node_titles:
            node_titles_missing[name].append(f"{full_name} ({path})")
        if category not in category_keys:
            categories_missing[category].append(f"{full_name} ({path})")

    return {
        "source": str(source),
        "dictionary_entries": len(entries),
        "hooked_resolved": hooked_hits,
        "wrapper_resolved": wrapper_hits,
        "hooked_missing": pack(hooked_missing),
        "wrapper_missing": pack(wrapper_missing),
        "unhooked": pack(unhooked),
        "node_types_not_allowlisted": pack(not_allowlisted),
        "node_titles_missing": pack(node_titles_missing),
        "categories_missing": pack(categories_missing),
        "counts": {
            "hooked_missing": len(hooked_missing),
            "wrapper_missing": len(wrapper_missing),
            "unhooked": len(unhooked),
            "node_types_not_allowlisted": len(not_allowlisted),
            "node_titles_missing": len(node_titles_missing),
            "categories_missing": len(categories_missing),
        },
    }


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument("--ase-source", type=Path, required=True)
    parser.add_argument("--fail-on-missing", action="store_true",
                        help="有任何缺口（含节点白名单/词条/分类）时以非零退出")
    parser.add_argument("--json", type=Path, help="把完整报告写入该路径")
    args = parser.parse_args()

    report = audit(args.ase_source)
    if args.json:
        args.json.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    print(json.dumps(report["counts"], ensure_ascii=False))
    for key, locations in report["hooked_missing"].items():
        print(f"  hooked_missing  {key!r}  {locations[0]}")
    for key, locations in report["wrapper_missing"].items():
        print(f"  wrapper_missing {key!r}  {locations[0]}")
    for key, locations in report["node_types_not_allowlisted"].items():
        print(f"  node_not_allowlisted {key!r}  {locations[0]}")
    for key, locations in report["node_titles_missing"].items():
        print(f"  node_title_missing  {key!r}  {locations[0]}")
    for key, locations in report["categories_missing"].items():
        print(f"  category_missing    {key!r}  {locations[0]}")
    silent = sum(report["counts"][name] for name in
                 ("hooked_missing", "wrapper_missing",
                  "node_types_not_allowlisted", "node_titles_missing", "categories_missing"))
    return 1 if (args.fail_on_missing and silent) else 0


if __name__ == "__main__":
    sys.exit(main())
