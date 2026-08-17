import React from "react";

function NetworkInspector({ logs, onClear }) {
  if (!logs || logs.length === 0) {
    return (
      <div className="card network-card">
        <div className="card-header">
          <h3>DevTools Network Inspector</h3>
          <span className="badge badge-neutral">Idle</span>
        </div>
        <div className="empty-inspector">
          <p>No API requests captured yet. Click <strong>Load Translations</strong> above to monitor traffic in real-time.</p>
        </div>
      </div>
    );
  }

  const getStatusBadge = (status) => {
    if (status >= 200 && status < 300) return "badge-success";
    if (status === 401) return "badge-warning";
    if (status === 403) return "badge-danger";
    return "badge-error";
  };

  return (
    <div className="card network-card">
      <div className="card-header">
        <div>
          <h3>DevTools Network Inspector</h3>
          <p className="card-subtitle">Real-time HTTP Request & Response Monitor</p>
        </div>
        <button type="button" className="btn-text" onClick={onClear}>
          Clear Console ({logs.length})
        </button>
      </div>

      <div className="network-log-list">
        {logs.map((log, index) => (
          <div key={index} className={`log-item ${index === 0 ? "active-log" : ""}`}>
            <div className="log-summary">
              <span className="method-tag">{log.method}</span>
              <span className={`badge ${getStatusBadge(log.status)}`}>
                {log.status} {log.statusText}
              </span>
              <span className="log-url" title={log.url}>{log.url}</span>
              <span className="log-latency">{log.latency}</span>
            </div>

            <div className="log-details">
              <div className="log-col">
                <span className="col-title">Request Headers:</span>
                <pre className="code-box-mini">
                  {JSON.stringify(log.headers, null, 2)}
                </pre>
              </div>

              <div className="log-col">
                <span className="col-title">Response Body:</span>
                <pre className="code-box-mini">
                  {typeof log.response === "string"
                    ? log.response
                    : JSON.stringify(log.response, null, 2)}
                </pre>
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}

export default NetworkInspector;
