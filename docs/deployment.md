# Deployment and Private Configuration

[Project overview](../README.md) · [Backend](../services/backend/README.md) · [Docker guide](docker.md) · [Secret cleanup status](environment-security-review.md)

## Deployment units

| Unit | Current implementation |
| --- | --- |
| Management API | ASP.NET Core HTTP API and SignalR host; also runs import/export/publish consumers |
| Email | Separate .NET host consuming email messages |
| Web | React build served by Nginx |
| AI | FastAPI service using Ollama; started separately from root Compose |
| Migration | DbUp executable; run deliberately before application startup |
| Domain/Application/Infrastructure | Libraries, not separate deployable containers |

Root `compose.yaml` includes Redis, RabbitMQ, Management API, Email, and Web. PostgreSQL, AI/Ollama, Azure Blob, and SendGrid remain external dependencies. The current Compose setup is a local development deployment, not a complete production configuration.

## Private local configuration

Tracked `appsettings.json` files contain empty values for sensitive settings. The .NET hosts already support environment-variable overrides. Root Compose instead mounts ignored `infra/docker/api.local.json` and `infra/docker/email.local.json` over each host's settings file.

The private handoff is stored outside the repository. For local .NET development, extract `private-config.zip` to a private folder and load the appropriate script into the terminal where the process will run:

```powershell
# Replace this with your actual private directory.
$privateConfigDirectory = 'C:\private\translation-platform\private-config'

# Management API terminal:
. (Join-Path $privateConfigDirectory 'api.env.ps1')
dotnet run --project services/backend/Management/MySolution.Api/MySolution.Api.csproj --launch-profile http
```

In a separate terminal for email:

```powershell
$privateConfigDirectory = 'C:\private\translation-platform\private-config'
. (Join-Path $privateConfigDirectory 'email.env.ps1')
dotnet run --project services/backend/Email/MySolution.Email.Api/MySolution.Email.Api.csproj --launch-profile http
```

For migration, set the environment before changing to the migration directory:

```powershell
$privateConfigDirectory = 'C:\private\translation-platform\private-config'
. (Join-Path $privateConfigDirectory 'migration.env.ps1')
Push-Location services/backend/MySolution.Migration
dotnet run --project MySolution.Migration.csproj -- mysolution
Pop-Location
```

These scripts set variables for the current process only. They contain credentials and must remain private. Do not dot-source scripts received from an untrusted source.

For a new environment without the private export, set the configuration keys listed in the Backend README, including:

- `ConnectionStrings__DefaultConnection` for Management; `ConnectionStrings__mysolution` for migration.
- `Jwt__Issuer`, `Jwt__Audience`, `Jwt__SecretKey` and token lifetimes.
- `Redis__ConnectionString`.
- `RabbitMq__Host`, `RabbitMq__Username`, `RabbitMq__Password`.
- `AzureBlob__ConnectionString`, `AzureBlob__ContainerName`.
- `SendGrid__ApiKey` and sender settings for Email.
- `Frontend__BaseUrl`, `CorsAllowedOrigins`, and `AI__BaseUrl` where used.

Do not assume a plain `.env` file is loaded automatically by `dotnet run`. Use environment variables or the exported loader scripts. Compose and Vite each have their own `.env` handling.

## Container configuration

Follow the [Docker guide](docker.md) to generate ignored local files. After the cleanup, new files generated from tracked settings have empty secret values; populate them privately before starting the hosts. Existing local override files are preserved.

Connection addresses depend on where a service runs:

| Caller | Dependency location | Address pattern |
| --- | --- | --- |
| Host process | Same machine | `localhost:<port>` |
| Compose container | Another Compose service | Service name, such as `redis:6379` or `rabbitmq` |
| Docker Desktop container | Windows host | `host.docker.internal:<port>` |

Nginx forwards `/api`, `/hubs`, and `/swagger` to `api:8080`. Keep WebSocket forwarding enabled for SignalR. The frontend Dockerfile builds with `VITE_API_URL=/api`; client-side environment variables are compiled into the bundle.

## Build and checks

From the repository root:

```powershell
dotnet restore services/backend/MySolution.slnx
dotnet build services/backend/MySolution.slnx --no-restore
dotnet test services/backend/Tests/AuthService.UnitTests/AuthService.UnitTests.csproj
python scripts/check-secrets.py --history

docker build -f services/backend/Management/MySolution.Api/Dockerfile -t translation-api:local services/backend
docker build -f services/backend/Email/MySolution.Email.Api/Dockerfile -t translation-email:local services/backend
docker build -t translation-web:local apps/web
```

Do not include private archives, environment loaders, real `.env` files, or local JSON overrides in images or publication artifacts. Use a secret store/environment injection for production configuration.

## Production work still required

- Rotate credentials previously committed or shared; history cleanup alone does not invalidate them.
- Provide HTTPS, compatible refresh-cookie/CORS settings, and a stable frontend URL.
- Manage database migrations and backup/restore explicitly.
- Keep Data Protection keys persistent where required.
- Plan retries, idempotency, queue failure handling, monitoring, and resource limits.
- Share realtime presence/editing-lock state and configure SignalR before scaling API instances.
- Add integration tests and dependency updates before claiming production readiness.

No deployment or remote force-push is performed by these instructions automatically.
