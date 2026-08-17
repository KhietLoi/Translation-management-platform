import React, { useState } from "react";
import { getVersion, getPackage } from "../api/translationApi";

function VersionPackageViewer({ config, onNetworkLog }) {
  const [versionData, setVersionData] = useState(null);
  const [verLoading, setVerLoading] = useState(false);
  const [verError, setVerError] = useState(null);

  const [pkgLoading, setPkgLoading] = useState(false);
  const [pkgError, setPkgError] = useState(null);
  const [pkgSuccess, setPkgSuccess] = useState(null);

  const handleFetchVersion = async () => {
    setVerLoading(true);
    setVerError(null);
    setVersionData(null);

    const startTime = performance.now();
    const reqUrl = `${config.baseUrl}/api/sdk/projects/${config.projectId}/version`;
    const reqHeaders = config.apiKey ? { "X-API-KEY": config.apiKey } : { "X-API-KEY": "[MISSING]" };

    try {
      const response = await getVersion(config.projectId, {
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

  const handleDownloadPackage = async () => {
    setPkgLoading(true);
    setPkgError(null);
    setPkgSuccess(null);

    const startTime = performance.now();
    const reqUrl = `${config.baseUrl}/api/sdk/projects/${config.projectId}/package`;
    const reqHeaders = config.apiKey ? { "X-API-KEY": config.apiKey } : { "X-API-KEY": "[MISSING]" };

    try {
      const res = await getPackage(config.projectId, {
        baseUrl: config.baseUrl,
        apiKey: config.apiKey
      });

      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);

      // Create a blob download link
      const blob = new Blob([res.data], { type: res.headers['content-type'] || 'application/zip' });
      const downloadUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.setAttribute('download', `release-${config.projectId}.zip`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(downloadUrl);

      setPkgSuccess(`Package downloaded successfully! (Blob size: ${(blob.size / 1024).toFixed(1)} KB)`);
      onNetworkLog({
        method: "GET",
        url: reqUrl,
        headers: reqHeaders,
        status: 200,
        statusText: "OK (ZIP File)",
        latency: `${latency}ms`,
        response: `[Binary ZIP File: ${blob.size} bytes]`
      });
    } catch (err) {
      const endTime = performance.now();
      const latency = Math.round(endTime - startTime);
      const status = err.response?.status || 500;
      const statusText = err.response?.statusText || "Error";

      // Blob error responses are blobs, so read text if possible
      let errBody = { errorMessage: err.message };
      if (err.response?.data instanceof Blob) {
        try {
          const text = await err.response.data.text();
          errBody = JSON.parse(text);
        } catch (_) {}
      } else if (err.response?.data) {
        errBody = err.response.data;
      }

      let msg = `HTTP ${status}: Failed to download package.`;
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
            <p className="card-subtitle">GET /api/sdk/projects/&#123;projectId&#125;/version</p>
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
            <p className="card-subtitle">GET /api/sdk/projects/&#123;projectId&#125;/package</p>
          </div>
        </div>
        <p className="card-desc">
          Downloads full <code className="code-highlight">release_xxx.zip</code> archive containing all JSON translation files for offline caching.
        </p>
        <button
          type="button"
          className="btn btn-outline"
          onClick={handleDownloadPackage}
          disabled={pkgLoading}
        >
          {pkgLoading ? "Downloading..." : "Download Release ZIP"}
        </button>

        {pkgError && <div className="alert alert-error mt-12">{pkgError}</div>}
        {pkgSuccess && <div className="alert alert-success mt-12">{pkgSuccess}</div>}
      </div>
    </div>
  );
}

export default VersionPackageViewer;
