import React, { useState } from "react";
import {
  ClipboardDocumentIcon,
  ClipboardDocumentCheckIcon,
  EllipsisVerticalIcon,
  ShieldCheckIcon,
  TrashIcon,
  ArrowPathIcon,
} from "@heroicons/react/24/outline";
import { toast } from "react-toastify";

const PERMISSION_LABEL_MAP = {
  1: "TranslationRead",
  2: "VersionRead",
  3: "PackageDownload",
  "1": "TranslationRead",
  "2": "VersionRead",
  "3": "PackageDownload",
  TranslationRead: "TranslationRead",
  VersionRead: "VersionRead",
  PackageDownload: "PackageDownload",
};

// Row item component defined locally
function ApiKeyGridRow({ item, onRotate, onRevoke, onAssignPermissions }) {
  const [copied, setCopied] = useState(false);

  const handleCopyPrefix = (text) => {
    if (!text) return;
    navigator.clipboard.writeText(text);
    setCopied(true);
    toast.info(`Copied key prefix: ${text}`);
    setTimeout(() => setCopied(false), 2000);
  };

  const renderExpirationBadge = () => {
    if (item.isRevoked) {
      return (
        <span className="apikey-expire-pill revoked">
          <span className="apikey-status-dot revoked"></span>
          Đã thu hồi
        </span>
      );
    }

    if (item.isExpired) {
      return (
        <span className="apikey-expire-pill expired">
          <span className="apikey-status-dot expired"></span>
          Đã hết hạn
        </span>
      );
    }

    if (!item.expiresAt) {
      return (
        <span className="apikey-expire-pill active">
          <span className="apikey-status-dot active"></span>
          Vĩnh viễn
        </span>
      );
    }

    const expireDate = new Date(item.expiresAt);
    const now = new Date();
    const diffTime = expireDate - now;
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    if (diffDays <= 0) {
      return (
        <span className="apikey-expire-pill expired">
          <span className="apikey-status-dot expired"></span>
          Đã hết hạn
        </span>
      );
    }

    if (diffDays <= 7) {
      return (
        <span className="apikey-expire-pill warning">
          <span className="apikey-status-dot warning"></span>
          {diffDays} ngày nữa
        </span>
      );
    }

    return (
      <span className="apikey-expire-pill active">
        <span className="apikey-status-dot active"></span>
        {diffDays} ngày nữa
      </span>
    );
  };

  const sparklineHeights = item.id
    ? [
        (item.id.charCodeAt(0) % 60) + 20,
        (item.id.charCodeAt(1) % 70) + 30,
        (item.id.charCodeAt(2) % 40) + 15,
        (item.id.charCodeAt(3) % 80) + 20,
        (item.id.charCodeAt(4) % 60) + 40,
        (item.id.charCodeAt(5) % 90) + 10,
        (item.id.charCodeAt(6) % 75) + 25,
      ]
    : [30, 45, 20, 65, 80, 50, 70];

  const permissions = item.permissions || [];

  return (
    <tr>
      {/* APPLICATION */}
      <td>
        <div className="d-flex flex-column">
          <span
            className="fw-semibold text-dark font-monospace"
            style={{ fontSize: "0.9rem" }}
          >
            {item.applicationName || item.name || "application-default"}
          </span>
          {item.projectName && (
            <small className="text-muted" style={{ fontSize: "0.75rem" }}>
              Project: {item.projectName}
            </small>
          )}
        </div>
      </td>

      {/* API KEY PREFIX */}
      <td>
        <div className="d-flex align-items-center gap-2">
          <span className="apikey-prefix-code">
            {item.keyPrefix
              ? `${item.keyPrefix.substring(0, 7)}...${item.keyPrefix.slice(-4)}`
              : "1x_live_••••8f2a"}
          </span>
          <button
            type="button"
            className="btn btn-sm btn-link text-muted p-0 border-0"
            title="Copy Prefix"
            onClick={() => handleCopyPrefix(item.keyPrefix || item.name)}
          >
            {copied ? (
              <ClipboardDocumentCheckIcon
                style={{ width: 16, height: 16 }}
                className="text-success"
              />
            ) : (
              <ClipboardDocumentIcon style={{ width: 16, height: 16 }} />
            )}
          </button>
        </div>
      </td>

      {/* PERMISSIONS (QUYỀN) */}
      <td>
        <div className="d-flex flex-wrap gap-1">
          {permissions.length > 0 ? (
            permissions.map((perm, idx) => (
              <span key={idx} className="apikey-permission-chip">
                {PERMISSION_LABEL_MAP[perm] || perm}
              </span>
            ))
          ) : (
            <span
              className="text-muted fst-italic"
              style={{ fontSize: "0.8rem" }}
            >
              Chưa gán quyền
            </span>
          )}
        </div>
      </td>

      {/* SỬ DỤNG (7 NGÀY) */}
      <td>
        <div className="apikey-sparkline" title="Sparkline 7 ngày sử dụng">
          {sparklineHeights.map((h, i) => (
            <div
              key={i}
              className={`apikey-sparkline-bar ${h < 30 ? "low" : ""}`}
              style={{ height: `${h}%` }}
            />
          ))}
        </div>
      </td>

      {/* HẾT HẠN */}
      <td>{renderExpirationBadge()}</td>

      {/* ACTIONS */}
      <td className="text-end">
        <div className="d-flex align-items-center justify-content-end gap-2">
          <button
            type="button"
            className="apikey-rotate-btn d-inline-flex align-items-center gap-1"
            disabled={item.isRevoked}
            onClick={() => onRotate(item)}
          >
            <ArrowPathIcon style={{ width: 13, height: 13 }} />
            <span>Rotate</span>
          </button>

          <div className="dropdown">
            <button
              className="btn btn-sm btn-light border-0 p-1 text-secondary rounded-circle"
              type="button"
              data-bs-toggle="dropdown"
              aria-expanded="false"
            >
              <EllipsisVerticalIcon style={{ width: 18, height: 18 }} />
            </button>
            <ul
              className="dropdown-menu dropdown-menu-end shadow-sm border-0"
              style={{ fontSize: "0.85rem" }}
            >
              <li>
                <button
                  className="dropdown-item d-flex align-items-center gap-2 py-2"
                  onClick={() => onAssignPermissions(item)}
                >
                  <ShieldCheckIcon style={{ width: 16, height: 16 }} />
                  <span>Phân quyền</span>
                </button>
              </li>
              {!item.isRevoked && (
                <li>
                  <button
                    className="dropdown-item d-flex align-items-center gap-2 py-2 text-danger"
                    onClick={() => onRevoke(item)}
                  >
                    <TrashIcon style={{ width: 16, height: 16 }} />
                    <span>Thu hồi Key</span>
                  </button>
                </li>
              )}
            </ul>
          </div>
        </div>
      </td>
    </tr>
  );
}

// Main ApiKeyGrid component
function ApiKeyGrid({
  loading,
  items = [],
  onRotate,
  onRevoke,
  onAssignPermissions,
}) {
  return (
    <div className="apikey-table-card mb-4">
      <div className="table-responsive">
        <table className="table align-middle apikey-table">
          <thead>
            <tr>
              <th style={{ width: "22%" }}>APPLICATION</th>
              <th style={{ width: "22%" }}>API KEY</th>
              <th style={{ width: "20%" }}>QUYỀN</th>
              <th style={{ width: "14%" }}>SỬ DỤNG (7 NGÀY)</th>
              <th style={{ width: "14%" }}>HẾT HẠN</th>
              <th style={{ width: "8%" }} className="text-end">
                ACTION
              </th>
            </tr>
          </thead>
          <tbody>
            {loading ? (
              <tr>
                <td colSpan={6} className="text-center py-5 text-muted">
                  <div
                    className="spinner-border spinner-border-sm text-dark me-2"
                    role="status"
                  ></div>
                  <span>Đang tải danh sách API Key...</span>
                </td>
              </tr>
            ) : !items || items.length === 0 ? (
              <tr>
                <td colSpan={6} className="text-center py-5 text-muted">
                  <p className="mb-1 fw-semibold text-secondary">
                    Không tìm thấy API Key nào
                  </p>
                  <small style={{ fontSize: "0.8rem" }}>
                    Nhấn nút "Tạo API Key" hoặc "Đăng ký ứng dụng" để khởi tạo
                    key mới.
                  </small>
                </td>
              </tr>
            ) : (
              items.map((item, idx) => (
                <ApiKeyGridRow
                  key={item.id || item.apiKeyId || `key-row-${idx}`}
                  item={item}
                  onRotate={onRotate}
                  onRevoke={onRevoke}
                  onAssignPermissions={onAssignPermissions}
                />
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default ApiKeyGrid;
