import React, { useState } from "react";

function ApiConfigPanel({ config, setConfig, onPresetSelect }) {
  const [showKey, setShowKey] = useState(false);

  const handleChange = (field, value) => {
    setConfig((prev) => ({
      ...prev,
      [field]: value
    }));
  };

  const defaultApiKey = import.meta.env.VITE_API_KEY || "";
  const defaultProjectId = import.meta.env.VITE_PROJECT_ID || "";

  return (
    <div className="card config-card">
      <div className="card-header">
        <h3>API & Project Credentials</h3>
        <span className="badge badge-info">Configurable</span>
      </div>

      <div className="config-grid">
        <div className="form-group">
          <label htmlFor="apiUrl">Base API URL</label>
          <input
            id="apiUrl"
            type="text"
            className="input-field"
            value={config.baseUrl}
            onChange={(e) => handleChange("baseUrl", e.target.value)}
            placeholder="http://127.0.0.1:5174"
          />
        </div>

        <div className="form-group full-width">
          <div className="label-with-action">
            <label htmlFor="apiKey">X-API-KEY Header Value</label>
            <button
              type="button"
              className="btn-text"
              onClick={() => setShowKey(!showKey)}
            >
              {showKey ? "Hide Key" : "Reveal Key"}
            </button>
          </div>
          <input
            id="apiKey"
            type={showKey ? "text" : "password"}
            className="input-field code-font"
            value={config.apiKey}
            onChange={(e) => handleChange("apiKey", e.target.value)}
            placeholder="ms_live_..."
          />
        </div>
      </div>

      <div className="preset-section">
        <span className="preset-label">Test Scenarios:</span>
        <div className="preset-buttons">
          <button
            type="button"
            className="chip chip-success"
            onClick={() => {
              handleChange("apiKey", defaultApiKey);
              handleChange("projectId", defaultProjectId);
            }}
          >
            Valid Credentials (200 OK)
          </button>
          <button
            type="button"
            className="chip chip-warning"
            onClick={() => handleChange("apiKey", "")}
          >
            Omit API Key (401 Unauthorized)
          </button>
          <button
            type="button"
            className="chip chip-danger"
            onClick={() => handleChange("apiKey", "ms_live_invalid_key_99999")}
          >
            Invalid API Key (401/403)
          </button>
          <button
            type="button"
            className="chip chip-purple"
            onClick={() => handleChange("projectId", "00000000-0000-0000-0000-000000000000")}
          >
            Other Project (403 Forbidden)
          </button>
        </div>
      </div>
    </div>
  );
}

export default ApiConfigPanel;
