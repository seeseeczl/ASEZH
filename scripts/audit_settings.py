#!/usr/bin/env python3
"""Read-only native settings vocabulary audit; licensed sources stay outside the repo."""
import argparse
import json
import re
import tarfile
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
STRING = r'"((?:\\.|[^"\\])*)"'


def decode(value):
    try:
        return json.loads('"' + value + '"')
    except ValueError:
        return value


def audit(source):
    found = {}

    def add(key, origin):
        key = key.strip()
        if key and re.search('[A-Za-z]', key):
            found.setdefault(key, set()).add(origin)

    editor = source / 'Plugins/Editor'
    files = list((editor / 'Nodes/Master').glob('*Helper.cs'))
    files += [editor / 'Nodes/Master' / n for n in ['StandardSurface.cs', 'MasterNode.cs']]
    files += list((editor / 'Templates').glob('*Module.cs'))
    files += [editor / 'Templates' / n for n in ['TemplateMasterNode.cs', 'TemplateMultiPassMasterNode.cs', 'TemplateModuleHelper.cs', 'TemplateOptionsUIHelper.cs', 'TemplatePassSelectorHelper.cs', 'TemplateAdditionalDirectivesHelper.cs']]
    for path in files:
        text = path.read_text(encoding='utf-8-sig')
        # Collect constant captions used at UI calls, and all GUIContent captions/tooltips.
        constants = {m[0]: decode(m[1]) for m in re.findall(r'\bstring\s+(\w+)\s*=\s*' + STRING + r'\s*;', text)}
        for m in re.finditer(r'new GUIContent\s*\(([^;\n]+)', text):
            for s in re.findall(STRING, m[1]):
                add(decode(s), path.name)
        for line in text.splitlines():
            if line.lstrip().startswith('//'):
                continue
            if re.search(r'(?:EditorGUI\w*|GUILayout\w*|Draw\w*PropertyGroup|(?:Int|Float|Slider)Field)\s*\(', line) or re.search(r'(?:EditorGUI\w*|GUI|GUILayout)\.(?:Label|HelpBox|Toggle|Button|Foldout)', line):
                for s in re.findall(STRING, line):
                    add(decode(s), path.name)
                for word in re.findall(r'\b\w+\b', line):
                    if word in constants:
                        add(constants[word], path.name)

    def template(text, origin):
        for line in text.splitlines():
            match = re.match(r'\s*(Option(?:,[^:]*)?|Field):([^:]+):([^:]+)', line)
            if match:
                add(match[2].split(',')[0], origin)
                if match[1].startswith('Option'):
                    for label in match[3].split(','):
                        add(label, origin)

    templates = source / 'Plugins/EditorResources/Templates'
    for path in templates.rglob('*.shader'):
        template(path.read_text(encoding='utf-8-sig'), str(path.relative_to(templates)))
    # Audit every shipped URP/HDRP package variant without importing or extracting it.
    for path in templates.glob('*.unitypackage'):
        with tarfile.open(path) as package:
            members = {m.name: m for m in package.getmembers()}
            for name, member in members.items():
                if not name.endswith('/pathname'):
                    continue
                asset_path = package.extractfile(member).read().decode('utf-8-sig')
                if not asset_path.endswith('.shader'):
                    continue
                asset = members.get(name.rsplit('/', 1)[0] + '/asset')
                if asset:
                    template(package.extractfile(asset).read().decode('utf-8-sig'), path.name + '/' + Path(asset_path).name)
    entries = json.loads((ROOT / 'Editor/ASEZHDictionary.json').read_text())['entries']
    translated = {x['key'].strip() for x in entries if x['table'] != 'port_label' and x['key'] != x['zh']}
    retained = {'A', 'B', 'G', 'R', 'LOD', 'PBR', 'AssetLabel Partial'}
    return {'source_files': len(files), 'captions': len(found),
            'retained': {k: sorted(v) for k, v in sorted(found.items()) if k in retained},
            'missing': {k: sorted(v) for k, v in sorted(found.items()) if k not in translated and k not in retained}}


if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('--ase-source', type=Path, required=True)
    args = parser.parse_args()
    print(json.dumps(audit(args.ase_source), ensure_ascii=False, indent=2))
