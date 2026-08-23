import React, { useState } from "react";
import { getVersion, getPackage } from "../api/translationApi";

function VersionPackageViewer({ config, onNetworkLog }) {
  const [versionData, setVersionData] = useState(null);
  const [verLoading, setVerLoading] = useState(false);
  const [verError, setVerError] = useState(null);

  const [pkgLoading, setPkgLoading] = useState(false);
  const [pkgError, setPkgError] = useState(null);
  const [pkgData, setPkgData] = useState(null);

  const handleFetchVersion = async () => {
    setVerLoading(true);
    setVerError(null);
    setVersionData(null);

    const startTime = performance.now();
    const reqUrl = `${config.baseUrl}/api/sdk/projects/version`;
    const reqHeaders = config.apiKey ? { "X-API-KEY": config.apiKey } : { "X-API-KEY": "[MISSING]" };

    try {
      const response = await getVersion({
        baseUrl: config.baseUrl,
        apiKey: config.apiKey
      });

      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);

      setVersionData(response);
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
      const statusText = err.response?.statusText || "Error";
      const errBody = err.response?.data || { errorMessage: err.message };

      let msg = `HTTP ${status}: Failed to get version.`;
      if (status === 403) msg = "403 Forbidden: API Key requires 'VersionRead' permission.";
      if (status === 401) msg = "401 Unauthorized: Invalid or missing API key.";

      setVerError(msg);
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
      setVerLoading(false);
    }
  };

  const handleGetPackage = async () => {
    setPkgLoading(true);
    setPkgError(null);
    setPkgData(null);

    const startTime = performance.now();
    const reqUrl = `${config.baseUrl}/api/sdk/projects/package`;
    const reqHeaders = config.apiKey ? { "X-API-KEY": config.apiKey } : { "X-API-KEY": "[MISSING]" };

    try {
      const response = await getPackage({
        baseUrl: config.baseUrl,
        apiKey: config.apiKey
      });

      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);

      setPkgData(response);
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
      const statusText = err.response?.statusText || "Error";
      const errBody = err.response?.data || { errorMessage: err.message };

      let msg = `HTTP ${status}: Failed to get package URL.`;
      if (status === 403) msg = "403 Forbidden: API Key requires 'PackageDownload' permission.";
      if (status === 401) msg = "401 Unauthorized: Invalid or missing API key.";

      setPkgError(msg);
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
      setPkgLoading(false);
    }
  };

  return (
    <div className="card-grid-two">
      {/* VERSION READ CARD */}
      <div className="card">
        <div className="card-header">
          <div>
            <h3>Version Query (VersionRead)</h3>
            <p className="card-subtitle">GET /api/sdk/projects/version</p>
          </div>
        </div>
        <p className="card-desc">
          Checks current active release version number without loading dictionary payload.
        </p>
        <button
          type="button"
          className="btn btn-secondary"
          onClick={handleFetchVersion}
          disabled={verLoading}
        >
          {verLoading ? "Checking..." : "Check Active Version"}
        </button>

        {verError && <div className="alert alert-error mt-12">{verError}</div>}

        {versionData && (
          <div className="code-box mt-12">
            <pre>{JSON.stringify(versionData, null, 2)}</pre>
          </div>
        )}
      </div>

      {/* PACKAGE DOWNLOAD CARD */}
      <div className="card">
        <div className="card-header">
          <div>
            <h3>Package Download (PackageDownload)</h3>
            <p className="card-subtitle">GET /api/sdk/projects/package</p>
          </div>
        </div>
        <p className="card-desc">
          Retrieves active release package info and a secure SAS download URL for direct downloading.
        </p>
        <button
          type="button"
          className="btn btn-outline"
          onClick={handleGetPackage}
          disabled={pkgLoading}
        >
          {pkgLoading ? "Fetching URL..." : "Get Package Download URL"}
        </button>

        {pkgError && <div className="alert alert-error mt-12">{pkgError}</div>}

        {pkgData && (
          <div className="mt-12">
            {pkgData?.data?.downloadUrl && (
              <div className="alert alert-success" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', gap: '12px' }}>
                <div>
                  <strong>Package ready:</strong> {pkgData.data.fileName || "release.zip"} (v{pkgData.data.version})
                </div>
                <a
                  href={pkgData.data.downloadUrl}
                  target="_blank"
                  rel="noopener noreferrer"
                  className="btn btn-primary"
                  style={{ textDecoration: 'none', whiteSpace: 'nowrap' }}
                >
                  Download ZIP
                </a>
              </div>
            )}
            <div className="code-box mt-12">
              <pre>{JSON.stringify(pkgData, null, 2)}</pre>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

export default VersionPackageViewer;
