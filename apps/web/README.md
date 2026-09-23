# Translation Management Platform — Frontend

The React administration application for managing projects, editing and reviewing translations, publishing releases, and monitoring background work. It connects to the Management API through Axios and receives realtime updates through SignalR.

[Project overview](../../README.md) · [Backend](../../services/backend/README.md) · [AI](../../services/ai-translation/README.md) · [Translation Read demo](../../examples/translation-read/README.md)

## 1. Features

| Screen group | Capabilities |
| --- | --- |
| Authentication | Login, registration, email verification, forgot/reset password |
| Administration | Users, roles, permissions, personal profiles |
| Dashboard | System overview |
| Projects | Project lists/details, namespaces, languages, members |
| Translation workspace | Grid, filters, pagination, editing, batch updates |
| Review | Submit, approve/reject, batch review |
| AI suggestions | Translation suggestions requested through the backend |
| Delivery | Import/export, publishing, release history, version comparison |
| Integration | API Key management |
| Notifications | Lists, details, realtime notifications |

Presence and editing locks indicate who is working on translations. UI permissions control navigation and available actions; the backend remains responsible for enforcing access and validating data.

## 2. Technology

- React 19, React DOM, Vite 8.
- React Router 7 for navigation.
- TanStack React Query 5 for server data.
- Axios for HTTP; `@microsoft/signalr` for realtime communication.
- Bootstrap, React Bootstrap, React Select, React Toastify.
- Heroicons, Lucide React, JWT Decode.
- ESLint; Docker and Nginx for deployment builds.

Exact versions are defined in `package.json` and `package-lock.json`.

## 3. Structure

```text
apps/web/
├── src/
│   ├── pages/          # Feature pages
│   ├── components/     # Business and shared UI components
│   ├── routes/         # AppRoutes
│   ├── layouts/        # Layout and selected-project scope
│   ├── contexts/       # Shared state and authentication
│   ├── hooks/          # Custom hooks
│   ├── services/       # Axios API, authentication, SignalR
│   ├── constants/      # Constants
│   ├── utils/          # Authentication, time, statuses, blobs
│   └── App.jsx
├── package.json
├── vite.config.js
├── Dockerfile
└── nginx.conf
```

## 4. Installation and configuration

Use Node.js matching `^20.19.0 || >=22.12.0`, as required by Vite in the lockfile. Node 22.12+ is an option. From `apps/web`:

```powershell
npm ci
```

Create `.env.local`:

```dotenv
VITE_API_URL=http://localhost:5182/api
```

**Include `/api` in this URL.** Axios appends endpoints to it; SignalR removes the `/api` suffix to construct `http://localhost:5182/hubs/translation`. Set this variable before startup because SignalR reads it when the module loads.

The backend must allow the frontend origin through `CorsAllowedOrigins` and use an appropriate `Frontend:BaseUrl`. For `Secure` refresh-cookie testing, use HTTPS with compatible origin/cookie settings. The backend HTTPS profile uses `https://localhost:7179`.

`VITE_*` variables are embedded in the browser bundle. Use them for client configuration, not backend secrets.

## 5. Development and builds

```powershell
npm run dev
```

Vite normally starts at `http://localhost:5173`; use the actual address printed in the terminal. To require that port:

```powershell
npm run dev -- --port 5173 --strictPort
```

| Command | Purpose |
| --- | --- |
| `npm run lint` | Run ESLint |
| `npm run build` | Generate a production build in `dist/` |
| `npm run preview` | Preview the build locally |

`preview` is for local build verification, not a production hosting configuration. The current `package.json` has no `test` script.

## 6. Authentication and API calls

```text
Login → Backend verifies credentials → Client stores access token and user data
→ Axios attaches Bearer token → Business API call
→ On an eligible 401, attempt cookie-based refresh → Retry the request
```

`src/services/api.js` reads `accessToken` from localStorage and enables `withCredentials`. The backend manages the refresh token through an HttpOnly cookie. User information uses the `currentUser` key; selected-project state uses `selectedProjectId`.

`PermissionGuard` and protected routes control the UI according to permissions. Editing client state/localStorage does not grant server-side permissions.

## 7. Main routes

| Route | Screen |
| --- | --- |
| `/` | Login |
| `/register`, `/checkemail`, `/verify-email` | Registration and email verification |
| `/forgot-password`, `/reset-password` | Password recovery |
| `/dashboard` | Overview |
| `/users`, `/roles`, `/permissions` | User and permission administration |
| `/profile` | Profile |
| `/projects`, `/projects/create`, `/projects/:id` | Projects |
| `/translations` | Translation workspace |
| `/import-export`, `/publish` | Import/export and releases |
| `/api-keys` | API Keys |
| `/notifications` | Notifications |
| `/403` | Access denied |

See `src/routes/AppRoutes.jsx` for current guards and route definitions.

## 8. Realtime behavior

`src/services/signalrService.js` connects to `/hubs/translation`, supplies the access token, and distributes events to UI components. Realtime updates support notifications, project state, presence, editing locks, and progress.

For manual checks, open two user sessions in the same project and observe editing/publishing updates. The backend currently stores presence/editing locks in process memory; multiple-instance deployments require additional configuration.

## 9. Docker and Nginx

The Dockerfile builds with Node 22 Alpine, sets `VITE_API_URL=/api`, and serves `dist` through Nginx. From the repository root:

```powershell
docker build -t translation-web:local apps/web
```

The image uses upstream `http://api:8080`, requiring a network with an `api` service, as provided by root Compose. Nginx routes:

- `/api/`: HTTP API proxy.
- `/hubs/`: SignalR/WebSocket proxy.
- `/swagger`: Swagger proxy.
- `/healthz`: Nginx health response.
- Other routes: `index.html` fallback for React Router.

Run a configured stack with `docker compose up -d --build` from the root. Vite configuration is embedded at build time; changing container variables afterward does not rewrite the bundle.

## 10. Validation and troubleshooting

| Symptom | Check |
| --- | --- |
| SignalR/API URL fails during loading | `VITE_API_URL` is defined and includes `/api` |
| Incorrect API address | No duplicated `/api/api`; correct backend port |
| CORS failure | Actual Vite origin is allowed by the backend |
| Refresh fails | Backend availability, cookie delivery, HTTPS/SameSite/CORS settings |
| Hub does not connect | JWT, `/hubs/translation`, WebSocket proxy |
| Nginx returns 502 | API availability and `api` hostname on the Docker network |
| Missing AI suggestions | Backend → FastAPI → Ollama connectivity |

Manual checklist: log in; select a project; load the grid; edit/review; import/export; publish and compare/roll back releases; check notifications; log out. Lint/build checks do not replace these integration scenarios.
