import React from "react";

function Header() {
  return (
    <header className="app-header">
      <div className="header-container">
        <div className="brand-section">
          <div className="logo-badge">TMS</div>
          <div>
            <h1>MySolution External Application</h1>
            <p className="subtitle">Translation Management System — React SDK Integration Demo</p>
          </div>
        </div>

        <div className="flow-card">
          <span className="flow-title">Authorization & Fetch Flow</span>
          <div className="flow-steps">
            <span className="step">External React App</span>
            <span className="arrow">─── X-API-KEY ───►</span>
            <span className="step highlight">GET /api/sdk/projects/...</span>
            <span className="arrow">───►</span>
            <span className="step">TMS Active Release</span>
            <span className="arrow">───►</span>
            <span className="step success">Dictionary (JSON)</span>
          </div>
        </div>
      </div>
    </header>
  );
}

export default Header;
