import importlib.util
import json
import shutil
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

SCANNER = Path(__file__).resolve().parents[1] / 'check-secrets.py'
spec = importlib.util.spec_from_file_location('check_secrets', SCANNER)
scanner = importlib.util.module_from_spec(spec)
spec.loader.exec_module(scanner)


class PatternTests(unittest.TestCase):
    def test_blank_and_placeholder_config_are_allowed(self):
        for value in ('', '<private-key>', '${PRIVATE_KEY}', 'CHANGE_ME'):
            self.assertEqual([], scanner.findings(json.dumps({'SecretKey': value}).encode(), 'settings.json'))

    def test_environment_expression_is_not_a_yaml_credential(self):
        self.assertEqual([], scanner.findings(b'apiKey: import.meta.env.VITE_API_KEY || ""', 'App.jsx'))

    def test_realistic_patterns_and_utf16_are_detected(self):
        data = json.dumps({'SecretKey': 'synthetic-test-' + 'value'})
        self.assertIn('nonempty sensitive JSON field', scanner.findings(data.encode('utf-16'), 'settings.json'))
        key = ('ms_live_' + 'A' * 32).encode()
        self.assertIn('application API key', scanner.findings(key, 'example.js'))


class RepositoryTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.root = Path(self.temp.name)
        self.git('init', '-q')
        self.git('config', 'user.name', 'Scanner Test')
        self.git('config', 'user.email', 'scanner@example.invalid')
        (self.root / 'scripts').mkdir()
        shutil.copyfile(SCANNER, self.root / 'scripts/check-secrets.py')

    def tearDown(self):
        # Git object files may be read-only on Windows.
        for p in (self.root / '.git').rglob('*'):
            if p.is_file():
                p.chmod(0o666)
        self.temp.cleanup()

    def git(self, *args):
        result = subprocess.run(['git', *args], cwd=self.root, capture_output=True)
        self.assertEqual(0, result.returncode)

    def scan(self, *args):
        result = subprocess.run([sys.executable, str(self.root / 'scripts/check-secrets.py'), *args],
                                cwd=self.root, capture_output=True, text=True)
        return result.returncode, json.loads(result.stdout)

    def test_staged_secret_is_detected_when_working_copy_is_clean(self):
        path = self.root / 'settings.json'
        path.write_text(json.dumps({'SecretKey': 'synthetic-test-' + 'value'}))
        self.git('add', 'settings.json')
        path.write_text(json.dumps({'SecretKey': ''}))
        code, report = self.scan()
        self.assertEqual(1, code)
        self.assertTrue(any(x['location'] == 'index:settings.json' for x in report['findings']))

    def test_deleted_historical_secret_is_detected(self):
        path = self.root / 'settings.json'
        path.write_text(json.dumps({'SecretKey': 'synthetic-test-' + 'value'}))
        self.git('add', 'settings.json')
        self.git('commit', '-qm', 'Add fixture')
        path.write_text(json.dumps({'SecretKey': ''}))
        self.git('add', 'settings.json')
        self.git('commit', '-qm', 'Clear fixture')
        self.assertEqual(0, self.scan()[0])
        code, report = self.scan('--history')
        self.assertEqual(1, code)
        self.assertTrue(any(x['location'].startswith('history:') for x in report['findings']))

    def test_tracked_env_is_rejected_even_without_secret_pattern(self):
        (self.root / '.env').write_text('PUBLIC_URL=http://localhost\n')
        self.git('add', '.env')
        code, report = self.scan()
        self.assertEqual(1, code)
        self.assertTrue(any('private file tracked by Git' in x['categories'] for x in report['findings']))


if __name__ == '__main__':
    unittest.main()
