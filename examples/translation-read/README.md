# Translation Read — External Integration Example

A React example demonstrating how an external client consumes published Translation Management Platform content using an API Key. It includes a data table, JSON viewer, multilingual UI preview, version/package checks, and a Network Inspector.

[Project overview](../../README.md) · [Backend/API](../../services/backend/README.md) · [Administration frontend](../../apps/web/README.md)

## 1. Purpose

- Configure the backend address and API Key.
- Read translations for a language code.
- Inspect table/JSON results and copy JSON.
- Switch languages in `MiniAppPreview` using loaded translations.
- Retrieve the active release version and package information.
- Inspect requests, headers, responses, latency, and errors.

This is a demo application calling SDK HTTP endpoints, not a published npm SDK library. Key creation, translation editing, and publishing belong to the administration application.

## 2. Technology and structure

React 19, Vite 8, Axios, and ESLint.

```text
examples/translation-read/
├── src/
│   ├── api/translationApi.js          # getTranslations/getVersion/getPackage
│   ├── components/
│   │   ├── ApiConfigPanel.jsx         # Connection configuration
│   │   ├── TranslationViewer.jsx      # Languages, table/JSON
│   │   ├── MiniAppPreview.jsx         # Multilingual example UI
│   │   ├── VersionPackageViewer.jsx   # Version and package information
│   │   └── NetworkInspector.jsx       # Request/response inspection
│   ├── App.jsx                       # State and component wiring
│   └── main.jsx
├── DataTest/                          # Sample data
├── package.json
└── vite.config.js
```

## 3. Prepare platform data

1. Start the backend and configure dependencies using the [Backend README](../../services/backend/README.md).
2. Create a project, languages, namespaces, keys, and values.
3. Review content, publish a release, and ensure an active release exists.
4. Create an application with the appropriate scope and issue an API Key.
5. Grant `TranslationRead`, `VersionRead`, and/or `PackageDownload` as required.
6. Allow the demo origin in backend CORS.

The server determines project access through the application and API Key. Entering another Project ID in the UI does not grant access to that project.

## 4. Installation and startup

Use Node.js `^20.19.0 || >=22.12.0`, as required by the lockfile. From `examples/translation-read`:

```powershell
npm ci
```

Create `.env.local`:

```dotenv
VITE_API_URL=http://localhost:5182
VITE_API_KEY=<api-key-for-demo>
VITE_PROJECT_ID=<project-id-for-display>
```

**Do not append `/api` to this URL:** `translationApi.js` appends `/api/sdk/...`. You can also change settings through `ApiConfigPanel`.

`VITE_PROJECT_ID` provides client state/display fallback. The three API helper functions do not send Project ID in the URL or query. The translation endpoint receives `language` as a query parameter.

Use a different port from the administration frontend:

```powershell
npm run dev -- --port 5174 --strictPort
```

Open `http://localhost:5174` and allow that origin in backend CORS. App and API helper fallback settings differ; configure the URL and demo key explicitly instead of relying on source defaults.

Browser API Keys and `VITE_*` values are visible to users. Network Inspector displays request headers for debugging. Use a scoped test key and do not share logs containing it.

## 5. API contract

| Client function | Request | Permission |
| --- | --- | --- |
| `getTranslations(language, options)` | `GET /api/sdk/projects/translations?language=en-US` | `TranslationRead` |
| `getVersion(options)` | `GET /api/sdk/projects/version` | `VersionRead` |
| `getPackage(options)` | `GET /api/sdk/projects/package` | `PackageDownload` |

Header:

```http
X-API-KEY: <api-key-for-demo>
```

`options` accepts `baseUrl` and `apiKey`. An empty or `null` key omits the header, allowing missing-key scenarios. Helpers return Axios `response.data`; viewers support both a `data` envelope and a direct payload.

Example:

```javascript
import { getTranslations } from './src/api/translationApi.js';

const result = await getTranslations('en-US', {
  baseUrl: 'http://localhost:5182',
  apiKey: '<api-key-for-demo>',
});

const payload = result.data ?? result;
console.log(payload);
```

The package endpoint returns package metadata/download information as defined by the backend, not necessarily ZIP bytes directly. Download URLs may expire and require a new request.

## 6. Demo scenario

1. Enter the Base URL and API Key.
2. Select a published language and load translations.
3. Inspect table/JSON output and compare it with published content.
4. Switch preview languages and observe the updated UI.
5. Retrieve the version/package and inspect network responses.
6. Publish another release in the administration application and reload.
7. Roll back a release and reload version/content.
8. Try a missing, invalid, or insufficiently privileged key.

Preview completeness depends on the keys provided by the project's translation data.

## 7. Scripts and validation

```powershell
npm run lint
npm run build
npm run preview
```

Build output is written to `dist/`. There is no automated `test` script in the current `package.json`; exercise integration scenarios against a running backend. `npm run dev` uses Vite's default port unless overridden.

## 8. Troubleshooting

| Problem | Check |
| --- | --- |
| Connection/CORS failure | API availability, Base URL, allowed demo origin |
| 401 | Valid header/key; key has not expired or been revoked |
| 403 | Required endpoint permission and application scope |
| Empty data/no release | Active release, published content, correct language code |
| URL-related 404 | No `/api` suffix in Base URL; no Project ID inserted into the documented routes |
| HTTPS certificate failure | Trust the valid development certificate; use the correct HTTPS port |
| Package download failure | URL validity, Blob configuration, package permission |

Network Inspector supports session-level debugging; it does not replace server logging or load-testing tools.
