import React, { useState, useEffect } from "react";
import {
  KeyIcon,
  ClipboardDocumentIcon,
  ClipboardDocumentCheckIcon,
  ExclamationTriangleIcon,
  CheckCircleIcon,
  ArrowPathIcon,
  ShieldCheckIcon,    
  TrashIcon,
} from "@heroicons/react/24/outline";
import { toast } from "react-toastify";

// Backend enum permissions constants
const PERMISSION_VALUE_MAP = {
  TranslationRead: 1,
  VersionRead: 2,
  PackageDownload: 3,
  1: 1,
  2: 2,
  3: 3,
};

const PERMISSION_NAME_MAP = {
  1: "TranslationRead",
  2: "VersionRead",
  3: "PackageDownload",
};

const PERMISSION_OPTIONS = [
  {
    id: "TranslationRead",
    label: "TranslationRead",
    description: "Allow reading translation values (1)",
  },
  {
    id: "VersionRead",
    label: "VersionRead",
    description: "Allow viewing translations release versions (2)",
  },
  {
    id: "PackageDownload",
    label: "PackageDownload",
    description: "Allow downloading translation packages (3)",
  },
];

const toBackendPermissionIds = (permissions = []) => {
  const uniqueIds = new Set();

  (Array.isArray(permissions) ? permissions : []).forEach((permission) => {
    const rawValue = permission;
    const normalized =
      typeof rawValue === "number"
        ? rawValue
        : typeof rawValue === "string"
          ? Number(rawValue)
          : null;

    if (Number.isInteger(normalized) && normalized >= 1 && normalized <= 3) {
      uniqueIds.add(normalized);
      return;
    }

    const enumValue = PERMISSION_VALUE_MAP[rawValue];
    if (Number.isInteger(enumValue) && enumValue >= 1 && enumValue <= 3) {
      uniqueIds.add(enumValue);
    }
  });

  return [...uniqueIds].sort((a, b) => a - b);
};

const toDisplayPermissionIds = (permissions = []) => {
  const ids = Array.isArray(permissions) ? permissions : [];
  return ids
    .map((permission) => {
      if (typeof permission === "string") {
        return PERMISSION_VALUE_MAP[permission] ? String(PERMISSION_VALUE_MAP[permission]) : permission;
      }
      if (typeof permission === "number") {
        return String(permission);
      }
      return null;
    })
    .filter(Boolean)
    .map((permission) => PERMISSION_NAME_MAP[permission] || permission)
    .filter((permission) => PERMISSION_OPTIONS.some((item) => item.id === permission));
};
// 1. Create Application Modal
export function CreateApplicationModal({ show, projects = [], onClose, onSubmit }) {
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [projectId, setProjectId] = useState("");
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (show) {
      setFieldErrors({});
    }
  }, [show]);

  if (!show) return null;

  const validateForm = () => {
    const errors = {};

    if (!name.trim()) {
      errors.name = "Application name cannot be empty.";
    } else {
      const trimmedName = name.trim();
      if (trimmedName.length < 3) {
        errors.name = "Application name must be at least 3 characters.";
      } else if (trimmedName.length > 100) {
        errors.name = "Application name must be under 100 characters.";
      }
    }

    if (description.trim() && description.trim().length > 500) {
      errors.description = "Description must be under 500 characters.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      setLoading(true);
      await onSubmit({
        name: name.trim(),
        description: description.trim() || null,
        projectId: projectId || null,
      });
      setName("");
      setDescription("");
      setProjectId("");
      setFieldErrors({});
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.5)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-bottom-0 pb-0">
            <h5 className="modal-title fw-bold text-dark">
              Register New Application
            </h5>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body py-4">
              <div className="mb-3">
                <label className="form-label fw-medium text-secondary fs-6">
                  Application Name <span className="text-danger">*</span>
                </label>
                <input
                  type="text"
                  className={`form-control rounded-3 ${fieldErrors.name ? "is-invalid" : ""}`}
                  placeholder="e.g., prod-mobile-01, merchant-portal-web"
                  value={name}
                  onChange={(e) => {
                    setName(e.target.value);
                    if (fieldErrors.name) {
                      setFieldErrors((prev) => ({ ...prev, name: "" }));
                    }
                  }}
                  required
                  maxLength={100}
                />
                {fieldErrors.name && (
                  <div className="invalid-feedback d-block">{fieldErrors.name}</div>
                )}
              </div>

              {projects.length > 0 && (
                <div className="mb-3">
                  <label className="form-label fw-medium text-secondary fs-6">
                    Project
                  </label>
                  <select
                    className="form-select rounded-3"
                    value={projectId}
                    onChange={(e) => setProjectId(e.target.value)}
                  >
                    <option value="">-- Select Project (Optional) --</option>
                    {projects.map((p, idx) => (
                      <option
                        key={p.id || p.projectId || `proj-opt-${idx}`}
                        value={p.id || p.projectId || ""}
                      >
                        {p.name || "Unnamed Project"}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              <div className="mb-3">
                <label className="form-label fw-medium text-secondary fs-6">
                  Description
                </label>
                <textarea
                  className={`form-control rounded-3 ${fieldErrors.description ? "is-invalid" : ""}`}
                  rows="3"
                  placeholder="Describe the purpose of this application..."
                  value={description}
                  onChange={(e) => {
                    setDescription(e.target.value);
                    if (fieldErrors.description) {
                      setFieldErrors((prev) => ({ ...prev, description: "" }));
                    }
                  }}
                  maxLength={500}
                />
                {fieldErrors.description && (
                  <div className="invalid-feedback d-block">{fieldErrors.description}</div>
                )}
              </div>
            </div>

            <div className="modal-footer border-top-0 pt-0">
              <button
                type="button"
                className="btn btn-light rounded-3 px-4"
                onClick={onClose}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-dark rounded-3 px-4"
                disabled={loading || !name.trim()}
              >
                {loading ? "Registering..." : "Register Application"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

// 2. Generate API Key Modal
export function GenerateApiKeyModal({
  show,
  applications = [],
  defaultApplicationId = "",
  onClose,
  onSubmit,
}) {
  const [applicationId, setApplicationId] = useState(defaultApplicationId);
  const [name, setName] = useState("");
  const [numofDaysExpires, setNumofDaysExpires] = useState(90);
  const [selectedPermissions, setSelectedPermissions] = useState([
    "TranslationRead",
    "VersionRead",
  ]);
  const [loading, setLoading] = useState(false);
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (show) {
      setFieldErrors({});
    }

    if (defaultApplicationId) {
      setApplicationId(defaultApplicationId);
    } else if (applications.length > 0) {
      setApplicationId(applications[0].id || applications[0].applicationId || "");
    }
  }, [defaultApplicationId, applications, show]);

  if (!show) return null;

  const validateForm = () => {
    const errors = {};

    if (!applicationId) {
      errors.applicationId = "Please select an application.";
    }

    if (!name.trim()) {
      errors.name = "API Key name cannot be empty.";
    } else {
      const trimmedName = name.trim();
      if (trimmedName.length < 3) {
        errors.name = "API Key name must be at least 3 characters.";
      } else if (trimmedName.length > 100) {
        errors.name = "API Key name must be under 100 characters.";
      } else if (!/^[A-Za-z0-9._-]+$/.test(trimmedName)) {
        errors.name = "API Key name can only contain letters, numbers, dots, underscores, and dashes.";
      }
    }

    const days = Number(numofDaysExpires);
    if (!Number.isInteger(days) || days < 0 || days > 3650) {
      errors.numofDaysExpires = "Invalid expiration period. Please choose a value from 0 to 3650 days.";
    }

    if (!selectedPermissions || selectedPermissions.length === 0) {
      errors.permissions = "Please select at least one permission for the API Key.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleTogglePermission = (permId) => {
    setSelectedPermissions((prev) => {
      const next = prev.includes(permId)
        ? prev.filter((p) => p !== permId)
        : [...prev, permId];

      if (fieldErrors.permissions && next.length > 0) {
        setFieldErrors((prevErrors) => ({ ...prevErrors, permissions: "" }));
      }

      return next;
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      setLoading(true);
      await onSubmit(applicationId, {
        name: name.trim(),
        numofDaysExpires: Number(numofDaysExpires),
        permissions: toBackendPermissionIds(selectedPermissions),
      });
      setName("");
      setNumofDaysExpires(90);
      setSelectedPermissions(["TranslationRead", "VersionRead"]);
      setFieldErrors({});
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.5)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-bottom-0 pb-0">
            <div className="d-flex align-items-center gap-2">
              <div className="bg-dark text-white rounded-3 p-2 d-flex align-items-center justify-content-center">
                <KeyIcon style={{ width: 20, height: 20 }} />
              </div>
              <div>
                <h5 className="modal-title fw-bold text-dark mb-0">
                  Generate New API Key
                </h5>
                <small className="text-secondary">
                  Generate API access key for the application
                </small>
              </div>
            </div>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body py-4">
              <div className="row g-3">
                <div className="col-12 col-md-6">
                  <label className="form-label fw-medium text-secondary fs-6">
                    Application <span className="text-danger">*</span>
                  </label>
                  <select
                    className={`form-select rounded-3 ${fieldErrors.applicationId ? "is-invalid" : ""}`}
                    value={applicationId}
                    onChange={(e) => {
                      setApplicationId(e.target.value);
                      if (fieldErrors.applicationId) {
                        setFieldErrors((prev) => ({ ...prev, applicationId: "" }));
                      }
                    }}
                    required
                  >
                    <option value="">-- Select Application --</option>
                    {applications.map((app, idx) => (
                      <option
                        key={app.id || app.applicationId || `gen-app-opt-${idx}`}
                        value={app.id || app.applicationId || ""}
                      >
                        {app.name || app.applicationName || "Unnamed App"}
                      </option>
                    ))}
                  </select>
                  {fieldErrors.applicationId && (
                    <div className="invalid-feedback d-block">{fieldErrors.applicationId}</div>
                  )}
                </div>

                <div className="col-12 col-md-6">
                  <label className="form-label fw-medium text-secondary fs-6">
                    API Key Name <span className="text-danger">*</span>
                  </label>
                  <input
                    type="text"
                    className={`form-control rounded-3 ${fieldErrors.name ? "is-invalid" : ""}`}
                    placeholder="e.g., prod-mobile-key-2026, server-key"
                    value={name}
                    onChange={(e) => {
                      setName(e.target.value);
                      if (fieldErrors.name) {
                        setFieldErrors((prev) => ({ ...prev, name: "" }));
                      }
                    }}
                    required
                    maxLength={100}
                  />
                  {fieldErrors.name && (
                    <div className="invalid-feedback d-block">{fieldErrors.name}</div>
                  )}
                </div>

                <div className="col-12">
                  <label className="form-label fw-medium text-secondary fs-6">
                    Expiration Period
                  </label>
                  <select
                    className={`form-select rounded-3 ${fieldErrors.numofDaysExpires ? "is-invalid" : ""}`}
                    value={numofDaysExpires}
                    onChange={(e) => {
                      setNumofDaysExpires(e.target.value);
                      if (fieldErrors.numofDaysExpires) {
                        setFieldErrors((prev) => ({ ...prev, numofDaysExpires: "" }));
                      }
                    }}
                  >
                    <option value={30}>30 days (1 month)</option>
                    <option value={60}>60 days (2 months)</option>
                    <option value={90}>90 days (3 months)</option>
                    <option value={180}>180 days (6 months)</option>
                    <option value={365}>365 days (1 year)</option>
                    <option value={0}>Never expire (Permanent)</option>
                  </select>
                  {fieldErrors.numofDaysExpires && (
                    <div className="invalid-feedback d-block">{fieldErrors.numofDaysExpires}</div>
                  )}
                </div>

                <div className="col-12">
                  <label className="form-label fw-medium text-secondary fs-6">
                    Permissions
                  </label>
                  <div
                    className={`border rounded-3 p-3 ${fieldErrors.permissions ? "border-danger-subtle" : ""}`}
                  >
                    <div className="d-flex flex-wrap gap-2 pt-1">
                      {PERMISSION_OPTIONS.map((p) => {
                        const isChecked = selectedPermissions.includes(p.id);
                        return (
                          <button
                            key={p.id}
                            type="button"
                            className={`btn btn-sm rounded-pill px-3 py-2 border transition-all ${
                              isChecked
                                ? "btn-dark text-white border-dark"
                                : "btn-light text-secondary border-light-subtle"
                            }`}
                            onClick={() => handleTogglePermission(p.id)}
                          >
                            {isChecked ? "✓ " : "+ "}
                            {p.label}
                          </button>
                        );
                      })}
                    </div>
                    {fieldErrors.permissions && (
                      <div className="text-danger small mt-2">{fieldErrors.permissions}</div>
                    )}
                  </div>
                </div>
              </div>
            </div>

            <div className="modal-footer border-top-0 pt-0">
              <button
                type="button"
                className="btn btn-light rounded-3 px-4"
                onClick={onClose}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-dark rounded-3 px-4"
                disabled={loading || !applicationId || !name.trim()}
              >
                {loading ? "Generating..." : "Generate API Key"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

// 3. One-Time API Key Display Modal
export function ApiKeyCreatedModal({ show, generatedData, isRotation = false, onClose }) {
  const [copied, setCopied] = useState(false);

  if (!show || !generatedData) return null;
  const rawKey = generatedData.apiKey || generatedData.rawKey || "";

  const handleCopy = () => {
    if (!rawKey) return;
    navigator.clipboard.writeText(rawKey);
    setCopied(true);
    toast.success("API Key copied to clipboard!");
    setTimeout(() => setCopied(false), 3000);
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.65)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content border-0 shadow-lg rounded-4 overflow-hidden">
          <div className="modal-header bg-dark text-white border-bottom-0 py-3">
            <div className="d-flex align-items-center gap-2">
              <CheckCircleIcon
                className="text-success"
                style={{ width: 24, height: 24 }}
              />
              <h5 className="modal-title fw-bold mb-0">
                {isRotation
                  ? "API Key Rotated Successfully!"
                  : "API Key Generated Successfully!"}
              </h5>
            </div>
            <button
              type="button"
              className="btn-close btn-close-white"
              onClick={onClose}
            ></button>
          </div>

          <div className="modal-body p-4">
            <div className="alert alert-warning border-0 rounded-3 d-flex align-items-start gap-3 mb-4">
              <ExclamationTriangleIcon
                className="text-warning flex-shrink-0 mt-1"
                style={{ width: 24, height: 24 }}
              />
              <div>
                <h6 className="fw-bold mb-1">
                  Important Security Notice
                </h6>
                <p className="mb-0 text-dark" style={{ fontSize: "0.875rem" }}>
                  This API Key will <strong>only be displayed once</strong>.
                  Please copy and store it in a secure location.
                </p>
              </div>
            </div>

            <div className="mb-3">
              <label className="form-label fw-bold text-dark fs-6 mb-2">
                API Key Name: {generatedData.name}
              </label>
              <div className="apikey-secret-box d-flex justify-content-between align-items-center gap-3">
                <span className="user-select-all font-monospace">{rawKey}</span>
                <button
                  type="button"
                  className={`btn btn-sm ${
                    copied ? "btn-success" : "btn-light"
                  } d-flex align-items-center gap-1 rounded-2 text-nowrap`}
                  onClick={handleCopy}
                >
                  {copied ? (
                    <>
                      <ClipboardDocumentCheckIcon style={{ width: 16, height: 16 }} />
                      <span>Copied!</span>
                    </>
                  ) : (
                    <>
                      <ClipboardDocumentIcon style={{ width: 16, height: 16 }} />
                      <span>Copy Key</span>
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>

          <div className="modal-footer bg-light border-top-0 py-3 px-4">
            <button
              type="button"
              className="btn btn-dark rounded-3 px-4 w-100 w-md-auto"
              onClick={onClose}
            >
              I have copied the key & finished
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

// 4. Rotate Key Modal
export function RotateApiKeyModal({ show, item, onClose, onConfirm }) {
  const [loading, setLoading] = useState(false);

  if (!show || !item) return null;

  const handleRotate = async () => {
    try {
      setLoading(true);
      await onConfirm(item.id);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.5)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-bottom-0 pb-0">
            <div className="d-flex align-items-center gap-2">
              <div className="bg-warning-subtle text-warning-emphasis p-2 rounded-3">
                <ArrowPathIcon style={{ width: 20, height: 20 }} />
              </div>
              <h5 className="modal-title fw-bold text-dark">
                Confirm API Key Rotation
              </h5>
            </div>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body py-4">
            <p className="text-secondary mb-3">
              Are you sure you want to rotate the API Key{" "}
              <strong className="text-dark">{item.name}</strong>?
            </p>
            <div
              className="alert alert-warning border-0 rounded-3 d-flex align-items-start gap-2 mb-0"
              style={{ fontSize: "0.85rem" }}
            >
              <ExclamationTriangleIcon
                style={{ width: 18, height: 18 }}
                className="flex-shrink-0 mt-1 text-warning"
              />
              <span>
                This action will <strong>immediately revoke the old API Key</strong>{" "}
                and generate a new key.
              </span>
            </div>
          </div>

          <div className="modal-footer border-top-0 pt-0">
            <button
              type="button"
              className="btn btn-light rounded-3 px-4"
              onClick={onClose}
            >
              Cancel
            </button>
            <button
              type="button"
              className="btn btn-warning text-dark fw-semibold rounded-3 px-4"
              disabled={loading}
              onClick={handleRotate}
            >
              {loading ? "Rotating..." : "Rotate API Key Now"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

// 5. Assign Permissions Modal
export function AssignApiKeyPermissionModal({ show, item, onClose, onSubmit }) {
  const [selectedPermissions, setSelectedPermissions] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (item && item.permissions) {
      const displayIds = toDisplayPermissionIds(item.permissions);
      setSelectedPermissions(displayIds.length > 0 ? displayIds : ["TranslationRead"]);
    } else {
      setSelectedPermissions(["TranslationRead"]);
    }
  }, [item]);

  if (!show || !item) return null;

  const handleToggle = (permId) => {
    setSelectedPermissions((prev) =>
      prev.includes(permId)
        ? prev.filter((p) => p !== permId)
        : [...prev, permId]
    );
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      setLoading(true);
      await onSubmit(item.id, toBackendPermissionIds(selectedPermissions));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.5)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-bottom-0 pb-0">
            <div className="d-flex align-items-center gap-2">
              <div className="bg-primary-subtle text-primary p-2 rounded-3">
                <ShieldCheckIcon style={{ width: 20, height: 20 }} />
              </div>
              <div>
                <h5 className="modal-title fw-bold text-dark mb-0">
                  Assign Permissions
                </h5>
                <small className="text-secondary">{item.name}</small>
              </div>
            </div>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="modal-body py-4">
              <label className="form-label fw-medium text-secondary fs-6 mb-3">
                Select permissions to assign:
              </label>

              <div className="d-flex flex-column gap-2">
                {PERMISSION_OPTIONS.map((perm) => {
                  const isChecked = selectedPermissions.includes(perm.id);
                  return (
                    <div
                      key={perm.id}
                      className={`p-3 rounded-3 border transition-all cursor-pointer ${
                        isChecked
                          ? "bg-light border-dark"
                          : "bg-white border-light-subtle"
                      }`}
                      onClick={() => handleToggle(perm.id)}
                      style={{ cursor: "pointer" }}
                    >
                      <div className="form-check mb-0">
                        <input
                          className="form-check-input"
                          type="checkbox"
                          id={`perm-${perm.id}`}
                          checked={isChecked}
                          onChange={() => handleToggle(perm.id)}
                        />
                        <label
                          className="form-check-label ms-2 fw-semibold text-dark cursor-pointer"
                          htmlFor={`perm-${perm.id}`}
                        >
                          {perm.label}
                        </label>
                        <p className="text-muted mb-0 small ms-2">
                          {perm.description}
                        </p>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>

            <div className="modal-footer border-top-0 pt-0">
              <button
                type="button"
                className="btn btn-light rounded-3 px-4"
                onClick={onClose}
              >
                Cancel
              </button>
              <button
                type="submit"
                className="btn btn-dark rounded-3 px-4"
                disabled={loading}
              >
                {loading ? "Saving..." : "Save Permissions"}
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}

// 6. Revoke Key Modal
export function RevokeApiKeyModal({ show, item, onClose, onConfirm }) {
  const [loading, setLoading] = useState(false);

  if (!show || !item) return null;

  const handleRevoke = async () => {
    try {
      setLoading(true);
      await onConfirm(item.id);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div
      className="modal fade show d-block"
      style={{ backgroundColor: "rgba(15, 23, 42, 0.5)" }}
      tabIndex="-1"
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg rounded-4">
          <div className="modal-header border-bottom-0 pb-0">
            <div className="d-flex align-items-center gap-2">
              <div className="bg-danger-subtle text-danger p-2 rounded-3">
                <TrashIcon style={{ width: 20, height: 20 }} />
              </div>
              <h5 className="modal-title fw-bold text-dark">
                Revoke API Key
              </h5>
            </div>
            <button type="button" className="btn-close" onClick={onClose}></button>
          </div>

          <div className="modal-body py-4">
            <p className="text-secondary mb-3">
              Are you sure you want to revoke the API Key{" "}
              <strong className="text-dark">{item.name}</strong>?
            </p>
            <div
              className="alert alert-danger border-0 rounded-3 d-flex align-items-start gap-2 mb-0"
              style={{ fontSize: "0.85rem" }}
            >
              <ExclamationTriangleIcon
                style={{ width: 18, height: 18 }}
                className="flex-shrink-0 mt-1 text-danger"
              />
              <span>
                This action <strong>cannot be undone</strong>. Revoked keys will immediately lose all API access.
              </span>
            </div>
          </div>

          <div className="modal-footer border-top-0 pt-0">
            <button
              type="button"
              className="btn btn-light rounded-3 px-4"
              onClick={onClose}
            >
              Cancel
            </button>
            <button
              type="button"
              className="btn btn-danger rounded-3 px-4 fw-semibold"
              disabled={loading}
              onClick={handleRevoke}
            >
              {loading ? "Revoking..." : "Revoke API Key"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
