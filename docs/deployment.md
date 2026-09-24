# Deployment and Private Configuration

[Project overview](../README.md) · [Backend](../services/backend/README.md) · [Docker guide](docker.md)

## Deployment units

| Unit | Current implementation |
| --- | --- |
| Management API | ASP.NET Core HTTP API and SignalR host; also runs import/export/publish consumers |
| Email | Separate .NET host consuming email messages |
| Web | React build served by Nginx |
| AI | FastAPI service using Ollama; both start as `ai-translation`/`ollama` in root Compose |
| Translation Read | Browser integration demo running through the Vite development server |
| Migration | DbUp executable; run deliberately before application startup |
| Domain/Application/Infrastructure | Libraries, not separate deployable containers |

Root `compose.yaml` starts eight services: Redis, RabbitMQ, Management API, Email, Web, Ollama, AI, and Translation Read. PostgreSQL, Azure Blob Storage, and SendGrid remain external dependencies. This Compose setup is for local development.

For a new clone, follow the [complete setup guide](../README.md#6-run-from-a-fresh-clone), including migration and the initial seed accounts.

## Private local configuration

Tracked `appsettings.json` files contain empty credential values. Each developer supplies their own database and provider configuration. A private configuration archive from the repository author is not required.

For Docker, run `scripts/init-docker.ps1` and fill in the ignored `infra/docker/api.local.json` and `infra/docker/email.local.json`. Compose mounts each file over its host's `appsettings.json` and injects internal service addresses through environment variables. Existing local JSON files are preserved by the initialization script.

For local .NET processes, use environment-variable overrides with a double underscore for nested keys:

| Variable | Used by | Purpose |
| --- | --- | --- |
| `ConnectionStrings__DefaultConnection` | Management API | PostgreSQL application connection |
| `ConnectionStrings__mysolution` | Migration | PostgreSQL migration connection |
| `Jwt__SecretKey` | Management API | Random signing secret of at least 32 ASCII characters |
| `Jwt__Issuer`, `Jwt__Audience` | Management API | Token validation settings; retain template defaults for a local trial |
| `Redis__ConnectionString` | Management API | Redis endpoint |
| `RabbitMq__Host`, `RabbitMq__Username`, `RabbitMq__Password` | API and Email | Message broker connection |
| `AzureBlob__ConnectionString`, `AzureBlob__ContainerName` | Management API | File storage and release packages |
| `SendGrid__ApiKey`, `SendGrid__FromEmail`, `SendGrid__FromName` | Email | Provider key and verified sender identity |
| `Frontend__BaseUrl` | API and Email | Frontend links used in workflows and messages |
| `CorsAllowedOrigins` | Management API | Allowed browser origins |
| `AI__BaseUrl` | Management API | FastAPI endpoint |
| `OLLAMA_HOST` | AI service | Ollama endpoint |

JWT settings are required by API startup. The Email host uses startup validation for all three SendGrid fields; an empty API key prevents that host from starting. Azure Blob settings are needed when Blob-dependent services are resolved. The [root configuration table](../README.md#step-3-fill-in-the-required-settings) describes what is required for each feature.

Set overrides in the terminal that will launch the corresponding process. A plain `.env` file is **not** automatically loaded by `dotnet run`. Compose and Vite handle their own environment files separately. Do not copy real values into tracked JSON or scripts.

## Database migration and seed

Run DbUp before starting the application, using one of the [documented migration commands](../README.md#step-4-create-the-database-and-seed-data). The Docker SDK option does not require a host .NET installation.

The command applies `Sequences → Scripts → Functions → Alter → Seed`, with execution recorded in `public.schema_version`. No separate seed step is required. Inspect the success/failure log, since the current runner does not explicitly return a failing process exit code on migration failure.

The checked-in seed includes known development accounts and sample content. Replace or remove these accounts before a deployment outside local development.

## Container networking and persistence

| Caller | Dependency location | Address pattern |
| --- | --- | --- |
| Host process | Same machine | `localhost:<port>` |
| Compose container | Another Compose service | Service name, such as `redis:6379`, `rabbitmq`, or `http://ai-translation:8000` |
| Docker Desktop container | Windows host | `host.docker.internal:<port>` |

Nginx forwards `/api`, `/hubs`, and `/swagger` to `api:8080`. Keep WebSocket forwarding enabled for SignalR. The frontend Dockerfile builds with `VITE_API_URL=/api`; Vite client variables are included in the browser bundle. The Translation Read container uses the Vite development server and calls `http://localhost:5182` from the browser.

Named volumes retain Redis data, RabbitMQ state, Ollama models, and API Data Protection keys. `docker compose down` preserves them; `docker compose down -v` removes them. PostgreSQL and Azure Blob storage are external and have separate lifecycles.

## Build and checks

From the repository root, with the relevant SDK/runtime installed for host commands:

```powershell
dotnet restore services/backend/MySolution.slnx
dotnet build services/backend/MySolution.slnx --no-restore
dotnet test services/backend/Tests/AuthService.UnitTests/AuthService.UnitTests.csproj
```

For Docker configuration and image builds:

```powershell
docker compose config --quiet
docker compose build
```

Maintainers with Python installed can additionally run `python scripts/check-secrets.py --history` before sharing changes. Do not include real environment files, private archives, or local JSON overrides in images or publication artifacts. Browser-delivered API Keys are visible to the client; use appropriately scoped demo keys.

## Production considerations

- Provide HTTPS, suitable cookie/CORS settings, and stable application URLs.
- Inject credentials through a secret store or environment configuration; replace any previously exposed credentials.
- Manage migrations and database/storage backup and restore explicitly.
- Persist Data Protection keys and protect access to them.
- Plan retries, idempotency, queue failure handling, monitoring, and resource limits.
- Share realtime presence/editing-lock state and configure SignalR before scaling API instances.
- Add integration tests and review dependency updates before production deployment.
