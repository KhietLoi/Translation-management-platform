"""Targeted pre-publish checks. Reports locations, never credential values."""
import argparse
import json
import re
import subprocess
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
PATTERNS = {
    'application API key': re.compile(r'ms_live_[A-Za-z0-9]{24,}'),
    'SendGrid API key': re.compile(r'SG\.[A-Za-z0-9_-]{15,}\.[A-Za-z0-9_-]{20,}'),
    'Azure account key': re.compile(r'AccountKey=[A-Za-z0-9+/=]{20,}'),
    'Google API key': re.compile(r'AIza[0-9A-Za-z_-]{30,}'),
    'GitHub token': re.compile(r'(?:gh[pousr]_[A-Za-z0-9]{20,}|github_pat_[A-Za-z0-9_]{30,})'),
    'private key': re.compile(r'-----BEGIN (?:RSA |EC |OPENSSH )?PRIVATE KEY-----'),
    'JWT token': re.compile(r'eyJ[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{10,}\.[A-Za-z0-9_-]{20,}'),
}
FIELDS = re.compile(r'"(?:ApiKey|SecretKey|Password|WebHookUri|ClientSecret|AccountKey)"\s*:\s*"([^"\r\n]*)"', re.I)
DBPASS = re.compile(r'(?:Host|Server)=[^"\r\n]*?Password=([^;"\r\n]+)', re.I)
YAML = re.compile(r'^\s*(?:RABBITMQ_DEFAULT_PASS|RABBITMQ_PASSWORD|POSTGRES_PASSWORD|Password|SecretKey|ApiKey)\s*:\s*([^\r\n#]+)', re.I | re.M)


def placeholder(value):
    return not value.strip() or any(marker in value for marker in ('<', '${', 'REDACTED', 'CHANGE_ME', 'your-'))


def private_path(name):
    base = Path(name).name.lower()
    return ((base == '.env' or base.startswith('.env.')) and base != '.env.example'
            or base.endswith(('.local.json', '.env.ps1', '.pem', '.pfx', '.p12', '.key'))
            or base in ('secret-values.json', 'repository-before-cleanup.zip', 'private-config.zip'))


def git(*args, data=None):
    result = subprocess.run(['git', '-c', 'safe.directory=' + str(ROOT), *args], cwd=ROOT,
                            input=data, stdout=subprocess.PIPE, stderr=subprocess.PIPE)
    if result.returncode:
        raise RuntimeError('Git inspection failed: ' + args[0])
    return result.stdout


def blobs(ids):
    ids = list(dict.fromkeys(ids))
    if not ids:
        return
    content = git('cat-file', '--batch', data=('\n'.join(ids) + '\n').encode())
    offset = 0
    for _ in ids:
        end = content.index(b'\n', offset)
        oid, kind, size = content[offset:end].split()
        start = end + 1
        size = int(size)
        yield oid.decode(), kind.decode(), content[start:start + size]
        offset = start + size + 1


def findings(data, name=''):
    texts = [data.decode('utf-8-sig', errors='replace')]
    if data.startswith((b'\xff\xfe', b'\xfe\xff')):
        texts.append(data.decode('utf-16', errors='replace'))
    categories = set()
    for text in texts:
        categories.update(label for label, pattern in PATTERNS.items() if pattern.search(text))
        if any(not placeholder(m[1]) for m in FIELDS.finditer(text)):
            categories.add('nonempty sensitive JSON field')
        if any(not placeholder(m[1]) for m in DBPASS.finditer(text)):
            categories.add('database password')
        if name.lower().endswith(('.yaml', '.yml')) and any(not placeholder(m[1].strip().strip('"\'')) for m in YAML.finditer(text)):
            categories.add('literal YAML credential')
    return sorted(categories)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--history', action='store_true', help='also scan all locally reachable Git history')
    args = parser.parse_args()
    problems = []
    stage = {}
    for entry in git('ls-files', '--stage', '-z').split(b'\0'):
        if not entry:
            continue
        metadata, path = entry.split(b'\t', 1)
        name = path.decode('utf-8')
        stage.setdefault(metadata.split()[1].decode(), []).append(name)
        if private_path(name):
            problems.append({'location': 'index:' + name, 'categories': ['private file tracked by Git']})
    for oid, kind, data in blobs(stage):
        result = sorted({item for name in stage[oid] for item in findings(data, name)})
        if result:
            problems.append({'location': 'index:' + ', '.join(stage[oid]), 'categories': result})
    working = git('ls-files', '--cached', '--others', '--exclude-standard', '-z').split(b'\0')
    for raw in sorted(set(working)):
        if not raw:
            continue
        name = raw.decode('utf-8')
        path = ROOT / name
        if path.is_file():
            result = findings(path.read_bytes(), name)
            if result:
                problems.append({'location': 'working:' + name, 'categories': result})
    history_count = 0
    if args.history:
        names = {}
        for line in git('rev-list', '--objects', '--all').splitlines():
            fields = line.split(b' ', 1)
            names[fields[0].decode()] = fields[1].decode('utf-8', errors='replace') if len(fields) == 2 else ''
        for oid, kind, data in blobs(names):
            if kind != 'blob':
                continue
            history_count += 1
            result = findings(data, names[oid])
            if private_path(names[oid]):
                result.append('private file in reachable history')
            if result:
                problems.append({'location': 'history:' + oid + ':' + names[oid], 'categories': result})
        result = findings(git('log', '--all', '--format=%B'))
        if result:
            problems.append({'location': 'commit messages', 'categories': result})
    print(json.dumps({'history_blobs_checked': history_count, 'findings': problems}, indent=2))
    return 1 if problems else 0


if __name__ == '__main__':
    raise SystemExit(main())
