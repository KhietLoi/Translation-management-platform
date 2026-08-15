import React from "react";
import { useNotificationDetailQuery } from "../../hooks/useNotifications";
import {
  XMarkIcon,
  ClockIcon,
  UserIcon,
  ArrowDownTrayIcon,
  DocumentIcon,
  CheckCircleIcon,
  ExclamationCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  TagIcon,
  GlobeAltIcon,
  ArchiveBoxIcon,
} from "@heroicons/react/24/outline";
import { formatTimeAgo, formatDateTime } from "../../utils/timeUtils";
import "./Notification.css";

/**
 * Returns icon component based on type or title keywords
 */
function getNotificationIcon(type, title = "") {
  const lowerTitle = (title || "").toLowerCase();

  if (lowerTitle.includes("import")) {
    return <ArchiveBoxIcon className="w-6 h-6 text-indigo-500" />;
  }
  if (lowerTitle.includes("export")) {
    return <ArrowDownTrayIcon className="w-6 h-6 text-blue-500" />;
  }
  if (lowerTitle.includes("publish") || type === 2 || type === "Success") {
    return <CheckCircleIcon className="w-6 h-6 text-green-500" />;
  }
  if (type === 3 || type === "Warning") {
    return <ExclamationTriangleIcon className="w-6 h-6 text-amber-500" />;
  }
  if (type === 4 || type === "Error") {
    return <ExclamationCircleIcon className="w-6 h-6 text-rose-500" />;
  }
  return <InformationCircleIcon className="w-6 h-6 text-blue-500" />;
}

export default function NotificationDetailModal({ notificationId, onClose, isDarkMode = false }) {
  const { data: notification, isLoading, isError, refetch } = useNotificationDetailQuery(notificationId);

  if (!notificationId) return null;

  // Detect which layout to render inside detail
  const renderDetailContent = (detail) => {
    if (!detail) return null;

    // 1. Translation Job
    if (detail.totalRecords !== undefined || detail.fileName !== undefined) {
      const isExport = detail.type === "Export" || (notification.title || "").toLowerCase().includes("export");
      const isCompleted = detail.status === "Completed" || (detail.status || "").toLowerCase().includes("complete");
      const total = detail.totalRecords || 0;
      const success = detail.successRecords || 0;
      const failed = detail.failedRecords || 0;
      const skipped = detail.skippedRecords || 0;

      // Calculate progress percentage
      const progressPercent = total > 0 ? Math.round((success / total) * 100) : 0;

      return (
        <div className="detail-card-section border rounded-3 p-3 mb-3 bg-light-subtle">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <span className="fw-semibold text-secondary small text-uppercase tracking-wider">Chi tiết tiến trình Job</span>
            <span className={`badge ${isCompleted ? "bg-success text-white border border-success" : "bg-danger text-white border border-danger"} px-2.5 py-1 rounded-pill fw-semibold`}>
              {detail.status || "N/A"}
            </span>
          </div>

          <div className="row g-3 mb-3">
            <div className="col-12">
              <div className="d-flex align-items-start gap-2">
                <DocumentIcon className="w-5 h-5 text-secondary flex-shrink-0 mt-0.5" style={{ width: "20px", height: "20px" }} />
                <div className="min-w-0">
                  <div className="text-secondary small">Tên tệp tin</div>
                  <div className="fw-medium text-dark dark-text-white text-break">{detail.fileName || "N/A"}</div>
                </div>
              </div>
            </div>

            <div className="col-md-6">
              <div className="text-secondary small">Loại Job</div>
              <div className="fw-semibold text-primary">{isExport ? "Export (Xuất bản bản dịch)" : "Import (Nhập bản dịch)"}</div>
            </div>

            {detail.errorMessage && (
              <div className="col-12">
                <div className="alert alert-danger border-0 rounded-3 mb-0 py-2.5 px-3 small">
                  <span className="fw-semibold">Thông báo lỗi:</span> {detail.errorMessage}
                </div>
              </div>
            )}
          </div>

          {/* Record Count Badges / Stats */}
          <div className="card border-0 bg-white dark-bg-gray-800 p-3 rounded-3 shadow-sm mb-3">
            <div className="d-flex align-items-center justify-content-between mb-2">
              <span className="small text-muted fw-semibold">Tỷ lệ thành công</span>
              <span className="small text-muted fw-bold">{progressPercent}% ({success}/{total} dòng)</span>
            </div>
            <div className="progress mb-3" style={{ height: "6px" }}>
              <div 
                className={`progress-bar ${failed > 0 ? "bg-warning" : "bg-success"}`}
                role="progressbar" 
                style={{ width: `${progressPercent}%` }} 
                aria-valuenow={progressPercent} 
                aria-valuemin="0" 
                aria-valuemax="100"
              />
            </div>

            <div className="row g-2 text-center">
              <div className="col-3">
                <div className="bg-light dark-bg-gray-700 py-2 rounded-2">
                  <div className="h5 mb-0 fw-bold text-dark dark-text-white">{total}</div>
                  <div className="text-muted" style={{ fontSize: "0.675rem" }}>Tổng số</div>
                </div>
              </div>
              <div className="col-3">
                <div className="bg-success-subtle py-2 rounded-2 text-success">
                  <div className="h5 mb-0 fw-bold">{success}</div>
                  <div className="text-success-emphasis" style={{ fontSize: "0.675rem" }}>Thành công</div>
                </div>
              </div>
              <div className="col-3">
                <div className="bg-danger-subtle py-2 rounded-2 text-danger">
                  <div className="h5 mb-0 fw-bold">{failed}</div>
                  <div className="text-danger-emphasis" style={{ fontSize: "0.675rem" }}>Thất bại</div>
                </div>
              </div>
              <div className="col-3">
                <div className="bg-warning-subtle py-2 rounded-2 text-warning">
                  <div className="h5 mb-0 fw-bold">{skipped}</div>
                  <div className="text-warning-emphasis" style={{ fontSize: "0.675rem" }}>Bỏ qua</div>
                </div>
              </div>
            </div>
          </div>

          {/* Download Button */}
          {detail.downloadUrl && (
            <a 
              href={detail.downloadUrl} 
              target="_blank" 
              rel="noreferrer" 
              className="btn btn-primary btn-sm w-100 py-2 rounded-3 d-flex align-items-center justify-content-center gap-2 fw-semibold shadow-sm"
            >
              <ArrowDownTrayIcon className="w-4 h-4" style={{ width: "16px", height: "16px" }} />
              <span>Tải xuống tệp tin kết quả</span>
            </a>
          )}
        </div>
      );
    }

    // 2. Translation Release
    if (detail.version !== undefined || detail.totalKey !== undefined) {
      return (
        <div className="detail-card-section border rounded-3 p-3 mb-3 bg-light-subtle">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <span className="fw-semibold text-secondary small text-uppercase tracking-wider">Thông tin phát hành (Release)</span>
            <span className="badge bg-primary-subtle text-primary border border-primary-subtle px-2.5 py-1 rounded-pill fw-bold">
              v{detail.version || "N/A"}
            </span>
          </div>

          <div className="row g-3 mb-3">
            <div className="col-6">
              <div className="text-secondary small">Tổng số Key phát hành</div>
              <div className="h4 mb-0 fw-bold text-dark dark-text-white">{detail.totalKey || 0} keys</div>
            </div>
            {detail.publishedAt && (
              <div className="col-6">
                <div className="text-secondary small">Thời gian phát hành</div>
                <div className="fw-medium text-dark dark-text-white">{formatDateTime(detail.publishedAt)}</div>
              </div>
            )}
            
            {detail.notes && (
              <div className="col-12">
                <div className="text-secondary small mb-1">Ghi chú phát hành</div>
                <div className="release-notes-box p-3 border rounded-3 bg-white dark-bg-gray-800 text-secondary text-break small">
                  {detail.notes}
                </div>
              </div>
            )}
          </div>

          {detail.downloadUrl && (
            <a 
              href={detail.downloadUrl} 
              target="_blank" 
              rel="noreferrer" 
              className="btn btn-outline-primary btn-sm w-100 py-2 rounded-3 d-flex align-items-center justify-content-center gap-2 fw-semibold"
            >
              <ArrowDownTrayIcon className="w-4 h-4" style={{ width: "16px", height: "16px" }} />
              <span>Tải xuống bản phát hành (JSON)</span>
            </a>
          )}
        </div>
      );
    }

    // 3. Translation Value Changes
    if (detail.translationKey !== undefined || detail.language !== undefined) {
      return (
        <div className="detail-card-section border rounded-3 p-3 mb-3 bg-light-subtle">
          <div className="d-flex justify-content-between align-items-center mb-3">
            <span className="fw-semibold text-secondary small text-uppercase tracking-wider">Thông tin bản dịch</span>
            <span className={`badge ${
              detail.status === "Approved" || detail.status === "Published" 
                ? "bg-success text-white" 
                : "bg-warning text-dark"
            } px-2.5 py-1 rounded-pill fw-semibold`}>
              {detail.status || "Chờ duyệt"}
            </span>
          </div>

          <div className="row g-3">
            <div className="col-12">
              <div className="d-flex align-items-start gap-2">
                <TagIcon className="w-5 h-5 text-secondary flex-shrink-0 mt-0.5" style={{ width: "20px", height: "20px" }} />
                <div>
                  <div className="text-secondary small">Translation Key</div>
                  <code className="text-primary fw-semibold fs-6 text-break">{detail.translationKey || "N/A"}</code>
                </div>
              </div>
            </div>

            <div className="col-md-6">
              <div className="d-flex align-items-start gap-2">
                <GlobeAltIcon className="w-5 h-5 text-secondary flex-shrink-0 mt-0.5" style={{ width: "20px", height: "20px" }} />
                <div>
                  <div className="text-secondary small">Ngôn ngữ</div>
                  <div className="fw-medium text-dark dark-text-white">{detail.language || "N/A"}</div>
                </div>
              </div>
            </div>

            <div className="col-12">
              <div className="text-secondary small mb-1">Giá trị bản dịch mới</div>
              <div className="p-3 border rounded-3 bg-white dark-bg-gray-800 text-dark dark-text-white font-monospace text-break small">
                {detail.value || <span className="text-muted italic">Chuỗi rỗng / Không có dữ liệu</span>}
              </div>
            </div>

            {/* Workflow logs */}
            <div className="col-12 border-top pt-3 mt-2">
              <span className="fw-semibold text-secondary small text-uppercase tracking-wider d-block mb-2">Nhật ký xử lý</span>
              <div className="workflow-timeline d-flex flex-column gap-2 small">
                {detail.translatedBy && (
                  <div className="d-flex justify-content-between">
                    <span className="text-muted">Dịch bởi: <strong className="text-dark dark-text-white">{detail.translatedBy}</strong></span>
                    {detail.translatedAt && <span className="text-muted">{formatDateTime(detail.translatedAt)}</span>}
                  </div>
                )}
                {detail.reviewedBy && (
                  <div className="d-flex justify-content-between">
                    <span className="text-muted">Duyệt bởi: <strong className="text-dark dark-text-white">{detail.reviewedBy}</strong></span>
                    {detail.reviewedAt && <span className="text-muted">{formatDateTime(detail.reviewedAt)}</span>}
                  </div>
                )}
                {detail.publishedBy && (
                  <div className="d-flex justify-content-between">
                    <span className="text-muted">Phát hành bởi: <strong className="text-dark dark-text-white">{detail.publishedBy}</strong></span>
                    {detail.publishedAt && <span className="text-muted">{formatDateTime(detail.publishedAt)}</span>}
                  </div>
                )}
              </div>
            </div>
          </div>
        </div>
      );
    }

    return null;
  };

  const getModalHeaderClass = (type, title = "") => {
    const lowerTitle = (title || "").toLowerCase();
    if (type === 4 || type === "Error" || lowerTitle.includes("failed")) {
      return "border-danger bg-danger-subtle";
    }
    if (type === 3 || type === "Warning") {
      return "border-warning bg-warning-subtle";
    }
    if (type === 2 || type === "Success" || lowerTitle.includes("completed")) {
      return "border-success bg-success-subtle";
    }
    return "border-primary bg-primary-subtle";
  };

  return (
    <div className={`modal d-block notification-detail-modal-overlay ${isDarkMode ? "dark-theme" : ""}`} style={{ background: "rgba(0,0,0,.6)", zIndex: 1100 }}>
      <div className="modal-dialog modal-dialog-centered modal-lg">
        <div className="modal-content border-0 rounded-4 overflow-hidden shadow bg-white">
          
          {/* Modal Header */}
          {isLoading ? (
            <div className="modal-header border-bottom px-4 py-3 justify-content-between">
              <h5 className="modal-title fw-bold">Đang tải thông báo...</h5>
              <button type="button" className="btn-close" onClick={onClose} aria-label="Close" />
            </div>
          ) : isError ? (
            <div className="modal-header border-bottom bg-danger-subtle text-danger px-4 py-3 justify-content-between">
              <h5 className="modal-title fw-bold">Lỗi tải thông tin</h5>
              <button type="button" className="btn-close" onClick={onClose} aria-label="Close" />
            </div>
          ) : (
            <div className={`modal-header border-start border-4 px-4 py-3.5 d-flex align-items-center justify-content-between ${getModalHeaderClass(notification.type, notification.title)}`}>
              <div className="d-flex align-items-center gap-3">
                <div className="p-2 bg-white rounded-circle shadow-sm d-flex align-items-center justify-content-center" style={{ width: "40px", height: "40px" }}>
                  {getNotificationIcon(notification.type, notification.title)}
                </div>
                <div>
                  <h5 className="modal-title fw-bold mb-0 text-dark">{notification.title}</h5>
                  <span className="small text-muted">{formatTimeAgo(notification.createdAt)}</span>
                </div>
              </div>
              <button 
                type="button" 
                className="btn-close border-0 bg-transparent p-1.5 text-secondary hover-text-dark rounded-circle"
                onClick={onClose}
                aria-label="Close"
                style={{ outline: "none", boxShadow: "none" }}
              >
                <XMarkIcon className="w-5 h-5" style={{ width: "20px", height: "20px" }} />
              </button>
            </div>
          )}

          {/* Modal Body */}
          <div className="modal-body p-4" style={{ maxHeight: "70vh", overflowY: "auto" }}>
            {isLoading ? (
              <div className="text-center py-5 text-muted">
                <div className="spinner-border text-primary mb-3" role="status" />
                <div className="small fw-semibold">Vui lòng đợi giây lát...</div>
              </div>
            ) : isError ? (
              <div className="text-center py-5 text-danger">
                <p className="mb-3 fw-medium">Không tìm thấy thông báo hoặc xảy ra lỗi kết nối.</p>
                <button onClick={() => refetch()} className="btn btn-sm btn-outline-danger px-3.5 py-1.5 rounded-3 fw-semibold">
                  Thử lại
                </button>
              </div>
            ) : (
              <div>
                {/* Main Message Block */}
                <div className="mb-4">
                  <p className="fs-6 text-dark mb-3 text-break" style={{ lineHeight: "1.5" }}>
                    {notification.message}
                  </p>

                  <div className="d-flex flex-wrap gap-3 text-secondary small border-top border-bottom py-2.5">
                    <div className="d-flex align-items-center gap-1.5">
                      <ClockIcon className="w-4 h-4 text-muted" style={{ width: "16px", height: "16px" }} />
                      <span>Tạo lúc: {formatDateTime(notification.createdAt)}</span>
                    </div>
                    {notification.triggeredByUserName && (
                      <div className="d-flex align-items-center gap-1.5">
                        <UserIcon className="w-4 h-4 text-muted" style={{ width: "16px", height: "16px" }} />
                        <span>Người thực hiện: <strong>{notification.triggeredByUserName}</strong></span>
                      </div>
                    )}
                  </div>
                </div>

                {/* Sub-detail Content cards */}
                {renderDetailContent(notification.detail)}
              </div>
            )}
          </div>

          {/* Modal Footer */}
          <div className="modal-footer px-4 py-3 bg-light-subtle d-flex justify-content-end gap-2">
            <button 
              type="button" 
              className="btn btn-secondary px-4 py-2 rounded-3 fw-semibold shadow-sm"
              onClick={onClose}
            >
              Đóng
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
