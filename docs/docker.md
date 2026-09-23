# Running Translation Management Platform with Docker on Windows

Run the following commands in PowerShell from the repository root.

[Deployment configuration](deployment.md) · [Secret cleanup status](environment-security-review.md)

## 1. Prerequisites

- Start Docker Desktop with Linux containers and wait for Docker Engine.
- Start PostgreSQL and apply the project schema using DbUp.
- Start AI/FastAPI and Ollama on the host if suggestions are needed.
- Keep Azure Blob and SendGrid configuration available privately.

```powershell
docker version
docker compose version
Test-Path .\compose.yaml
```

The final check should return `True`.

## 2. Create local configuration

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\init-docker.ps1
```

The script preserves existing local JSON files and generates a RabbitMQ password when missing. Check the expected files:

```powershell
Test-Path .\.env
Test-Path .\infra\docker\api.local.json
Test-Path .\infra\docker\email.local.json
```

After the secret cleanup, newly generated JSON files contain empty credential values inherited from the public templates. A successful initialization message means the files exist, not that every application setting is ready.

Populate these files privately, or use matching originals from the private handoff:

| File | Required review |
| --- | --- |
| `infra/docker/api.local.json` | PostgreSQL, JWT, Blob, Redis and AI settings |
| `infra/docker/email.local.json` | SendGrid, sender identity, and other required email settings |
| `.env` | Nonempty `RABBITMQ_PASSWORD` |

For PostgreSQL/AI running on Windows, use `host.docker.internal` rather than `localhost` inside the container configuration. Root Compose supplies the RabbitMQ host/credentials and the API's Redis address.

Do not copy credentials into tracked `appsettings.json`. The private archive is a backup of existing values, not a rotation of exposed credentials.

## 3. Verify exclusions and Compose

```powershell
git check-ignore .env infra/docker/api.local.json infra/docker/email.local.json
python scripts/check-secrets.py
docker compose config --quiet
```

The ignore check should list all three files. Use `--quiet` for Compose validation so expanded configuration values are not printed into logs. Compose validation checks structure, not whether database/cloud credentials are correct.

## 4. Build and start

```powershell
docker compose build
docker compose up -d
docker compose ps -a
```

Expected services: `web`, `api`, `email`, `redis`, and `rabbitmq`. Root Compose does not start PostgreSQL or AI.

Inspect local logs when troubleshooting, and remove any sensitive values before sharing output:

```powershell
docker compose logs --tail 100 api email web
docker compose logs -f --tail 100 api email web
```

Press Ctrl+C to stop following logs.

## 5. Access and verify

| Component | Address |
| --- | --- |
| Web | `http://localhost:5173` |
| Swagger through Nginx | `http://localhost:5173/swagger` |
| RabbitMQ Management | `http://localhost:15674` |

RabbitMQ uses username `mysolution` and the password from the local `RABBITMQ_PASSWORD` setting.

```powershell
(Invoke-WebRequest -UseBasicParsing http://localhost:5173/healthz).Content
```

Expected response: `ok`. This checks Nginx, not all backend dependencies. Also verify login, database-backed pages, email, AI suggestions, and SignalR as needed.

## 6. Stop and restart

```powershell
docker compose down
```

Do not add `-v` if you intend to preserve named-volume data.

For later runs, start Docker Desktop and external dependencies, then:

```powershell
docker compose up -d
```

After source changes:

```powershell
docker compose up -d --build
```

After changing local JSON configuration:

```powershell
docker compose up -d --force-recreate api email
```

Keep `.env` and both local JSON files private. Changing `RABBITMQ_PASSWORD` in `.env` does not automatically change a RabbitMQ user's password already stored in its persistent volume.
