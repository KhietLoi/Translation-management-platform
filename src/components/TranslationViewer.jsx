import React, { useState, forwardRef, useImperativeHandle } from "react";
import { getTranslations } from "../api/translationApi";

const TranslationViewer = forwardRef(({ config, onNetworkLog, onTranslationsLoaded, onLoadingChange }, ref) => {
  const [language, setLanguage] = useState("vi-VN");
  const [customLanguage, setCustomLanguage] = useState("");
  const [useCustomLang, setUseCustomLang] = useState(false);

  const [translations, setTranslations] = useState(null);
  const [version, setVersion] = useState(null);
  const [responseMeta, setResponseMeta] = useState(null);

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [copied, setCopied] = useState(false);
  const [viewMode, setViewMode] = useState("table"); // 'table' | 'json'

  const activeLanguage = useCustomLang ? customLanguage.trim() : language;

  useImperativeHandle(ref, () => ({
    loadLanguage: (lang) => {
      handleLoadTranslations(lang);
    }
  }));

  const handleLoadTranslations = async (overrideLang) => {
    const isStringLang = typeof overrideLang === "string" && overrideLang.trim() !== "";
    const targetLang = isStringLang ? overrideLang.trim() : activeLanguage;

    if (isStringLang) {
      setLanguage(targetLang);
      setUseCustomLang(false);
    }

    setLoading(true);
    if (onLoadingChange) onLoadingChange(true);
    setError(null);
    setTranslations(null);
    setVersion(null);
    setResponseMeta(null);

    const startTime = performance.now();
    const reqUrl = `${config.baseUrl}/api/sdk/projects/${config.projectId}/translations?language=${targetLang}`;
    const reqHeaders = config.apiKey ? { "X-API-KEY": config.apiKey } : { "X-API-KEY": "[MISSING]" };

    try {
      const response = await getTranslations(config.projectId, targetLang, {
        baseUrl: config.baseUrl,
        apiKey: config.apiKey
      });

      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);

      // Handle envelope structure from TMS API (e.g. { success: true, data: { projectId, version, language, translations } })
      if (response && response.success === false) {
        const errObj = {
          status: 400,
          message: response.errorMessage || "Failed to load translations.",
          data: response
        };
        setError(errObj);
        onNetworkLog({
          method: "GET",
          url: reqUrl,
          headers: reqHeaders,
          status: 400,
          statusText: "Bad Request",
          latency: `${latency}ms`,
          response: response
        });
        return;
      }

      const resData = response.data || response;
      const dict = resData.translations || resData.dictionary || (typeof resData === "object" ? resData : {});
      const ver = resData.version !== undefined ? resData.version : (resData.activeVersion || "Active");

      setTranslations(dict);
      setVersion(ver);
      setResponseMeta({
        projectId: resData.projectId || config.projectId,
        language: resData.language || targetLang,
        version: ver
      });

      if (onTranslationsLoaded) {
        onTranslationsLoaded({
          translations: dict,
          language: resData.language || targetLang,
          version: ver
        });
      }

      onNetworkLog({
        method: "GET",
        url: reqUrl,
        headers: reqHeaders,
        status: 200,
        statusText: "OK",
        latency: `${latency}ms`,
        response: response
      });

    } catch (err) {
      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);

      const status = err.response?.status || 500;
      const statusText = err.response?.statusText || (err.message === "Network Error" ? "Network Error / CORS" : "Error");
      const errBody = err.response?.data || { errorMessage: err.message };

      let userMsg = "Cannot connect to Translation API.";
      if (status === 401) {
        userMsg = "401 Unauthorized: Invalid or missing X-API-KEY header.";
      } else if (status === 403) {
        userMsg = "403 Forbidden: API Key lacks 'TranslationRead' permission or project access is denied.";
      } else if (status === 404) {
        userMsg = "404 Not Found: Project or active release translation file not found.";
      } else if (err.message === "Network Error" || statusText.includes("Network Error")) {
        userMsg = `Network Error / CORS: Failed to reach ${config.baseUrl}. Check protocol (HTTPS vs HTTP), SSL dev certificate, or CORS configuration on backend.`;
      } else if (errBody?.errorMessage) {
        userMsg = errBody.errorMessage;
      }

      setError({
        status,
        statusText,
        message: userMsg,
        detail: errBody
      });

      onNetworkLog({
        method: "GET",
        url: reqUrl,
        headers: reqHeaders,
        status,
        statusText,
        latency: `${latency}ms`,
        response: errBody
      });
    } finally {
      setLoading(false);
      if (onLoadingChange) onLoadingChange(false);
    }
  };

  const copyJson = () => {
    if (!translations) return;
    navigator.clipboard.writeText(JSON.stringify(translations, null, 2));
    setCopied(true);
    setTimeout(() => setCopied(false), 2000);
  };

  const filteredEntries = translations
    ? Object.entries(translations).filter(
      ([k, v]) =>
        k.toLowerCase().includes(searchQuery.toLowerCase()) ||
        String(v).toLowerCase().includes(searchQuery.toLowerCase())
    )
    : [];

  return (
    <div className="card translation-card">
      <div className="card-header">
        <div>
          <h2>Translation API (TranslationRead)</h2>
          <p className="card-subtitle">
            Endpoint: <code className="endpoint-tag">GET /api/sdk/projects/&#123;projectId&#125;/translations</code>
          </p>
        </div>
        {version !== null && (
          <div className="version-badge">
            <span className="version-label">Active Release:</span>
            <span className="version-value">v{version}</span>
          </div>
        )}
      </div>

      <div className="translation-controls">
        <div className="form-group lang-select-group">
          <label>Target Language</label>
          {!useCustomLang ? (
            <div className="select-wrapper">
              <select
                className="input-field"
                value={language}
                onChange={(e) => setLanguage(e.target.value)}
              >
                <option value="vi-VN">Vietnamese (vi-VN)</option>
                <option value="en-US">English (en-US)</option>
                <option value="ja-JP">Japanese (ja-JP)</option>
                <option value="ko-KR">Korean (ko-KR)</option>
                <option value="zh-TW">Traditional Chinese (zh-TW)</option>
                <option value="fr-FR">French (fr-FR)</option>
                <option value="de-DE">German (de-DE)</option>
              </select>
            </div>
          ) : (
            <input
              type="text"
              className="input-field"
              value={customLanguage}
              onChange={(e) => setCustomLanguage(e.target.value)}
              placeholder="e.g. zh-CN, es-ES"
            />
          )}
          <button
            type="button"
            className="btn-text toggle-custom-lang"
            onClick={() => setUseCustomLang(!useCustomLang)}
          >
            {useCustomLang ? "Use Presets" : "Custom Code..."}
          </button>
        </div>

        <button
          type="button"
          className="btn btn-primary btn-load"
          onClick={() => handleLoadTranslations()}
          disabled={loading || (useCustomLang && !customLanguage.trim())}
        >
          {loading ? (
            <>
              <span className="spinner"></span> Loading Translations...
            </>
          ) : (
            <>Load Translations</>
          )}
        </button>
      </div>

      {/* ERROR DISPLAY */}
      {error && (
        <div className="alert alert-error">
          <div className="alert-header">
            <strong>
              HTTP {error.status} — {error.statusText}
            </strong>
          </div>
          <p className="alert-message">{error.message}</p>
          {(error.statusText?.includes("Network Error") || error.status === 500) && (
            <div className="network-troubleshoot-box">
              <p style={{ fontWeight: 600, marginBottom: "6px" }}>Troubleshooting checklist for Network Error / CORS:</p>
              <ul style={{ margin: "0", paddingLeft: "20px", lineHeight: "1.5" }}>
                <li><strong>Port & Protocol check:</strong> Backend is running on <code>http://localhost:5182</code> (matching your Swagger endpoint). Ensure Base API URL is set to <code>http://localhost:5182</code>.</li>
                <li><strong>Trust SSL Dev Cert:</strong> Open <a href={`${config.baseUrl}/api/sdk/projects/${config.projectId}/version`} target="_blank" rel="noreferrer" style={{ color: "#2563eb", textDecoration: "underline" }}>{config.baseUrl}</a> in browser tab and click <em>Proceed to localhost (unsafe)</em>, or run <code>dotnet dev-certs https --trust</code> in terminal.</li>
                <li><strong>Backend CORS Policy:</strong> Ensure ASP.NET Core <code>Program.cs</code> has <code>.WithHeaders("X-API-KEY")</code> and <code>.WithOrigins("http://localhost:5173")</code> configured.</li>
              </ul>
            </div>
          )}
          {error.detail && typeof error.detail === "object" && (
            <pre className="error-json-preview">
              {JSON.stringify(error.detail, null, 2)}
            </pre>
          )}
        </div>
      )}

      {/* SUCCESS TRANSLATION CONTENT */}
      {translations && (
        <div className="results-container">
          <div className="results-toolbar">
            <div className="search-box">
              <input
                type="text"
                placeholder="Search key or text value..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="input-field"
              />
            </div>

            <div className="toolbar-actions">
              <div className="toggle-group">
                <button
                  type="button"
                  className={`btn-toggle ${viewMode === "table" ? "active" : ""}`}
                  onClick={() => setViewMode("table")}
                >
                  Table ({filteredEntries.length})
                </button>
                <button
                  type="button"
                  className={`btn-toggle ${viewMode === "json" ? "active" : ""}`}
                  onClick={() => setViewMode("json")}
                >
                  JSON
                </button>
              </div>

              <button
                type="button"
                className="btn btn-secondary btn-copy"
                onClick={copyJson}
              >
                {copied ? "Copied!" : "Copy JSON"}
              </button>
            </div>
          </div>

          {viewMode === "table" ? (
            <div className="table-responsive">
              <table className="translation-table">
                <thead>
                  <tr>
                    <th style={{ width: "35%" }}>Key</th>
                    <th style={{ width: "65%" }}>
                      Translation ({responseMeta?.language})
                    </th>
                  </tr>
                </thead>
                <tbody>
                  {filteredEntries.length > 0 ? (
                    filteredEntries.map(([key, value]) => (
                      <tr key={key}>
                        <td className="key-cell">
                          <code>{key}</code>
                        </td>
                        <td className="value-cell">{String(value)}</td>
                      </tr>
                    ))
                  ) : (
                    <tr>
                      <td colSpan="2" className="empty-cell">
                        No translation keys match "{searchQuery}"
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          ) : (
            <pre className="json-code-block">
              {JSON.stringify(translations, null, 2)}
            </pre>
          )}
        </div>
      )}
    </div>
  );
});

export default TranslationViewer;
