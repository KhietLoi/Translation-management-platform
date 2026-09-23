# Environment and Secret Cleanup

## Status

The local repository history has been rewritten to remove the credential values identified during the review. Current configuration and recovery data were exported to a private directory outside the repository before any rewrite.

Completed locally:

- Removed `.env` and non-example `.env.*` files from the rewritten history.
- Removed historical build/dependency directories, local JSON overrides, and private-key/certificate paths covered by the cleanup rules.
- Removed the discovered application API Key, JWT signing keys, SendGrid keys, Azure Storage keys, a hardcoded JWT in a historical load-test script, and database password values from retained source/configuration history.
- Rewrote all three local branches and their local remote-tracking refs. There were no tags at cleanup time.
- Restored pre-existing working-tree changes, including files the owner had already deleted, without committing them.
- Kept current local `.env` and Docker overrides ignored and available for local use.
- Added reusable configuration examples and a targeted pre-publish scanner.

**No remote push or force-push was performed.** Rewritten remote-tracking refs are local metadata; they do not establish that the server repository has been cleaned. Existing remote history, clones, forks, and previously built artifacts may still contain old values.

## Private handoff

The owner received an absolute path to a sibling directory named `Translation-management-platform-private-<timestamp>`. It is outside this repository and contains:

| File/directory | Purpose |
| --- | --- |
| `private-config.zip` | Portable configuration handoff; contains sensitive values |
| `private-config/` | Extracted originals and process-environment loader scripts |
| `repository-before-cleanup.zip` | Recovery copy of the original Git directory and working files |
| `secret-values.json` | Private inventory used for exact-value verification |
| `manifest.json` | Pre-cleanup file/ref inventory |
| `verification.json` | Results of the private verification scan |

The archive is not encrypted. Store it privately; do not upload it, attach it to a public issue, or copy it into the publishable source tree. Older recovery copies may also contain the original secrets.

See [deployment configuration](deployment.md#private-local-configuration) for loading the exported values. The exported values are backups, not newly issued credentials.

## Validation scope

The cleanup verification checks retained history against the collected values and targeted credential patterns, and checks for excluded paths. It includes text blobs, known-value checks in binary blobs, and commit messages. The final private scan inspected 3,661 text blobs against 11 distinct credential values collected from the original source/history, with no findings. The public scanner inspected 3,663 historical blobs, including binary blobs, with no findings. Detailed private results are recorded in `verification.json`.

The public scanner can be rerun from the repository root:

```powershell
python scripts/check-secrets.py
python scripts/check-secrets.py --history
python -m unittest discover -s scripts/tests -v
```

It prints file/object locations and categories only, never matched values. It checks non-ignored working files, tracked files, and optionally reachable history. It is a targeted check, not proof that every possible secret format has been detected.

Git and Docker ignore rules keep local configuration out of ordinary adds/build contexts, but they do not protect deliberately force-added files or erase previously published artifacts.

## Publishing the rewritten history

The rewrite changed commit IDs. Before updating an existing remote:

1. Coordinate with collaborators and retain the private recovery copy.
2. Confirm remote branch tips with `git ls-remote` and compare them with the pre-cleanup records. Do not overwrite new collaborator commits.
3. Commit only the intended, reviewed working changes. Re-run the scanner with `--history`.
4. Update each affected branch using an explicit `--force-with-lease=<ref>:<expected-old-tip>` after confirming the remote state and the intended destination. This cleanup did not perform that action.
5. Have collaborators use a fresh clone of the cleaned history. Review old tags/branches, forks, caches, and build artifacts separately.

Do not merge an old clone back into the cleaned repository or fetch old history into it before the remote cleanup is resolved. Keep any remote inspection that would import old objects in a separate disposable clone.

## Credentials and browser clients

Removing a value from history does not revoke it. Any real application, SendGrid, Azure, JWT, or database credentials that were previously shared still need an owner-approved rotation at their issuing service. This cleanup did not rotate or test credentials.

The Translation Read demo no longer has an embedded fallback API Key. A key entered at runtime or supplied through `VITE_API_KEY` is still visible to browser users and request inspection. Use a restricted test key there; use a server-side integration when the key must remain confidential.

## Validation results

- AuthService unit tests: 19 passed.
- Scanner tests: 6 passed, including staged secrets, historical secrets, UTF-16 input, and false-positive prevention.
- `docker compose config --quiet`: passed.
- Existing Auth build warnings about `System.Security.Cryptography.Xml 8.0.2` remain; dependency remediation was not part of this cleanup.
- No live application startup, external credential validation, or remote publication was performed.
