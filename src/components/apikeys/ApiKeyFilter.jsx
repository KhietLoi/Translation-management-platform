import React from "react";
import {
  MagnifyingGlassIcon,
  FunnelIcon,
  PlusIcon,
  KeyIcon,
} from "@heroicons/react/24/outline";

function ApiKeyFilter({
  applications = [],
  selectedApplicationId,
  onApplicationChange,
  envFilter,
  onEnvFilterChange,
  keyword,
  onKeywordChange,
  isRevoked,
  onIsRevokedChange,
  onSearch,
  onOpenCreateAppModal,
  onOpenGenerateKeyModal,
}) {
  const handleKeyDown = (e) => {
    if (e.key === "Enter") {
      onSearch();
    }
  };

  return (
    <div className="mb-4">
      {/* HEADER ROW */}
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-start align-items-md-center gap-3 mb-4">
        <div>
          <h2 className="fw-bold text-dark mb-1 fs-3">API Key Management</h2>
          <p className="text-secondary mb-0 fs-6">
            Manage registered applications, permissions, and life cycle of API keys.
          </p>
        </div>

        <div className="d-flex align-items-center gap-2">
          <button
            type="button"
            className="btn btn-outline-dark d-flex align-items-center gap-2 rounded-3 px-3 py-2 fw-semibold shadow-sm"
            onClick={onOpenCreateAppModal}
          >
            <PlusIcon style={{ width: 18, height: 18 }} />
            <span>Register Application</span>
          </button>

          <button
            type="button"
            className="btn btn-dark d-flex align-items-center gap-2 rounded-3 px-3.5 py-2 fw-semibold shadow-sm"
            onClick={onOpenGenerateKeyModal}
          >
            <KeyIcon style={{ width: 18, height: 18 }} />
            <span>Create API Key</span>
          </button>
        </div>
      </div>

      {/* FILTER CARD */}
      <div className="apikey-filter-card p-3">
        <div className="row g-3 align-items-center">
          {/* Left: Application Selector Dropdown */}
          <div className="col-12 col-md-3">
            <div className="input-group input-group-sm">
              <label
                className="input-group-text bg-light text-secondary border-end-0"
                htmlFor="appSelect"
              >
                <FunnelIcon style={{ width: 15, height: 15 }} />
              </label>
              <select
                id="appSelect"
                className="form-select form-select-sm border-start-0 text-dark fw-medium"
                value={selectedApplicationId || ""}
                onChange={(e) => onApplicationChange(e.target.value)}
              >
                <option value="">All Applications</option>
                {applications.map((app, idx) => (
                  <option
                    key={app.id || app.applicationId || `app-opt-${idx}`}
                    value={app.id || app.applicationId || ""}
                  >
                    {app.name || app.applicationName || "Unnamed App"}
                  </option>
                ))}
              </select>
            </div>
          </div>

          {/* Center: Environment Tabs (Production / Staging / All) */}
          <div className="col-12 col-md-4 col-lg-3">
            <div className="apikey-env-tab-container w-100 justify-content-center justify-content-md-start">
              <button
                type="button"
                className={`apikey-env-tab ${
                  envFilter === "all" ? "active" : ""
                }`}
                onClick={() => onEnvFilterChange("all")}
              >
                All
              </button>
              <button
                type="button"
                className={`apikey-env-tab ${
                  envFilter === "production" ? "active" : ""
                }`}
                onClick={() => onEnvFilterChange("production")}
              >
                Production
              </button>
              <button
                type="button"
                className={`apikey-env-tab ${
                  envFilter === "staging" ? "active" : ""
                }`}
                onClick={() => onEnvFilterChange("staging")}
              >
                Staging
              </button>
            </div>
          </div>

          {/* Right: Search Bar & Switch */}
          <div className="col-12 col-md-5 col-lg-6">
            <div className="d-flex align-items-center gap-2">
              <div className="position-relative flex-grow-1">
                <MagnifyingGlassIcon
                  className="position-absolute text-muted"
                  style={{
                    width: 16,
                    height: 16,
                    left: 12,
                    top: "50%",
                    transform: "translateY(-50%)",
                  }}
                />
                <input
                  type="text"
                  className="form-select-sm form-control ps-5 pe-3 rounded-3"
                  placeholder="Search keys, projects, members..."
                  value={keyword}
                  onChange={(e) => onKeywordChange(e.target.value)}
                  onKeyDown={handleKeyDown}
                  style={{ fontSize: "0.875rem", minHeight: "36px" }}
                />
              </div>

              <div className="form-check form-switch mb-0 text-nowrap ms-1">
                <input
                  className="form-check-input"
                  type="checkbox"
                  role="switch"
                  id="isRevokedSwitch"
                  checked={isRevoked || false}
                  onChange={(e) => onIsRevokedChange(e.target.checked)}
                />
                <label
                  className="form-check-small text-muted ms-1"
                  htmlFor="isRevokedSwitch"
                  style={{ fontSize: "0.8rem" }}
                >
                  Revoked
                </label>
              </div>

              <button
                type="button"
                className="btn btn-dark btn-sm rounded-3 px-3 py-2 text-nowrap"
                onClick={onSearch}
                style={{ minHeight: "36px" }}
              >
                Search
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ApiKeyFilter;
