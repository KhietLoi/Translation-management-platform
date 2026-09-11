import React, { useState } from "react";
import { useOutletContext } from "react-router-dom";
import {
  useNotificationsQuery,
  useMarkAllAsReadMutation,
} from "../../hooks/useNotifications";
import NotificationItem from "./NotificationItem";
import {
  CheckIcon,
  BellIcon,
  ArrowPathIcon,
  SparklesIcon,
  ChevronLeftIcon,
  ChevronRightIcon,
} from "@heroicons/react/24/outline";
import "./Notification.css";

export default function NotificationCenterPage({ isDarkMode = false }) {
  const [filterType, setFilterType] = useState("ALL");
  const [pageNumber, setPageNumber] = useState(1);
  const [pageSize, setPageSize] = useState(15);

  // Retrieve active project from MainLayout outlet context or localStorage
  const outletContext = useOutletContext() || {};
  const selectedProjectId =
    outletContext.selectedProjectId || localStorage.getItem("selectedProjectId") || "";

  // Fetch paginated notification data for the selected project
  const {
    data,
    isLoading,
    refetch,
    isRefetching,
    isError,
  } = useNotificationsQuery({
    projectId: selectedProjectId,
    pageNumber,
    pageSize,
  });

  const markAllAsReadMutation = useMarkAllAsReadMutation();

  const apiNotifications = data?.items || [];
  const totalCount = data?.totalCount || 0;
  const totalPages = data?.totalPages || 1;

  // Filter items by selected tab
  const filteredItems = apiNotifications.filter((item) => {
    const isSuccess =
      item.type === "Success" ||
      item.type === 2 ||
      (item.title || "").toLowerCase().includes("completed");
    const isWarning = item.type === "Warning" || item.type === 3;
    const isError =
      item.type === "Error" ||
      item.type === 4 ||
      (item.title || "").toLowerCase().includes("failed");

    if (filterType === "UNREAD") return !item.isRead;
    if (filterType === "SUCCESS") return isSuccess;
    if (filterType === "WARNING") return isWarning;
    if (filterType === "ERROR") return isError;
    return true;
  });

  const handleMarkAllRead = async () => {
    try {
      await markAllAsReadMutation.mutateAsync();
    } catch (err) {
      console.error("Failed to mark all notifications as read:", err);
    }
  };

  // Generate page numbers array for pagination bar
  const getPageNumbers = () => {
    const pages = [];
    const maxVisible = 5;
    let start = Math.max(1, pageNumber - Math.floor(maxVisible / 2));
    let end = Math.min(totalPages, start + maxVisible - 1);

    if (end - start + 1 < maxVisible) {
      start = Math.max(1, end - maxVisible + 1);
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  };

  const pageNumbers = getPageNumbers();

  return (
    <div className={`container-fluid p-0 max-w-6xl ${isDarkMode ? "dark-theme" : ""}`}>
      {/* Top Title & Subheader Section matching Lexicon Reference Image */}
      <div className="d-flex flex-column flex-md-row justify-content-between align-items-md-center mb-4 gap-3">
        <div>
          <h2 className="fw-bold mb-1 fs-3 text-dark dark-text-white d-flex align-items-center gap-2">
            Notification Center
            <span className="badge bg-primary-subtle text-primary border border-primary-subtle fs-6 rounded-pill">
              <SparklesIcon width={14} height={14} className="me-1 d-inline" />
              Realtime
            </span>
          </h2>
          <p className="text-secondary mb-0 small">
            Cập nhật realtime qua SignalR.
          </p>
        </div>

        <div className="d-flex align-items-center gap-2">
          <button
            type="button"
            onClick={() => refetch()}
            disabled={isRefetching}
            className="btn btn-outline-secondary btn-sm d-flex align-items-center gap-1 rounded-3 px-3 py-2"
          >
            <ArrowPathIcon
              width={16}
              height={16}
              className={isRefetching ? "spin-anim" : ""}
            />
            <span>Làm mới</span>
          </button>

          <button
            type="button"
            onClick={handleMarkAllRead}
            disabled={markAllAsReadMutation.isPending}
            className="btn btn-dark btn-sm rounded-3 px-3 py-2 fw-medium shadow-sm d-flex align-items-center gap-1.5"
          >
            <CheckIcon width={16} height={16} />
            <span>
              {markAllAsReadMutation.isPending
                ? "Đang xử lý..."
                : "Đánh dấu đã đọc tất cả"}
            </span>
          </button>
        </div>
      </div>

      {/* Filter Tabs */}
      <div className="d-flex flex-wrap align-items-center gap-2 mb-3">
        {[
          { key: "ALL", label: "Tất cả" },
          { key: "UNREAD", label: "Chưa đọc" },
          { key: "SUCCESS", label: "Thành công" },
          { key: "WARNING", label: "Cảnh báo" },
          { key: "ERROR", label: "Lỗi / Cần chú ý" },
        ].map((tab) => (
          <button
            key={tab.key}
            onClick={() => {
              setFilterType(tab.key);
              setPageNumber(1);
            }}
            className={`btn btn-sm rounded-pill px-3 py-1 ${
              filterType === tab.key
                ? "btn-primary font-semibold"
                : "btn-light text-secondary border"
            }`}
            style={{ fontSize: "0.825rem" }}
          >
            {tab.label}
          </button>
        ))}
      </div>

      {/* Main Notification Center Card Container matching screenshot */}
      <div className="notification-center-card shadow-sm">
        {isLoading ? (
          <div className="p-5 text-center text-muted">
            <div className="spinner-border me-2" role="status" />
            <div>Đang tải dữ liệu thông báo từ máy chủ...</div>
          </div>
        ) : isError ? (
          <div className="p-5 text-center text-danger">
            <div>Không thể kết nối đến máy chủ API notification.</div>
            <button
              onClick={() => refetch()}
              className="btn btn-sm btn-outline-primary mt-2"
            >
              Thử lại
            </button>
          </div>
        ) : filteredItems.length === 0 ? (
          <div className="empty-notification-state p-5 text-center">
            <BellIcon width={48} height={48} className="mx-auto mb-3 text-secondary opacity-50" />
            <h5 className="fw-semibold mb-1">Không có thông báo nào</h5>
            <p className="text-muted small">
              Hiện tại không có thông báo nào từ hệ thống. Các thông báo mới từ SignalR sẽ xuất hiện tại đây.
            </p>
          </div>
        ) : (
          <div className="divide-y">
            {filteredItems.map((item) => (
              <NotificationItem
                key={item.id}
                item={item}
                isDarkMode={isDarkMode}
              />
            ))}
          </div>
        )}

        {/* Full Pagination Bar */}
        {!isLoading && !isError && totalCount > 0 && (
          <div className="d-flex flex-column flex-md-row justify-content-between align-items-center p-3 border-top gap-3 bg-light-subtle">
            <div className="d-flex align-items-center gap-2 text-secondary small">
              <span>Hiển thị</span>
              <select
                className="form-select form-select-sm border-secondary-subtle rounded-2 cursor-pointer"
                style={{ width: "75px" }}
                value={pageSize}
                onChange={(e) => {
                  setPageSize(Number(e.target.value));
                  setPageNumber(1);
                }}
              >
                <option value={10}>10</option>
                <option value={15}>15</option>
                <option value={25}>25</option>
                <option value={50}>50</option>
              </select>
              <span>
                mục / trang (Hiển thị {(pageNumber - 1) * pageSize + 1} -{" "}
                {Math.min(pageNumber * pageSize, totalCount)} trong tổng số {totalCount})
              </span>
            </div>

            <nav aria-label="Notification Pagination">
              <ul className="pagination pagination-sm mb-0">
                <li className={`page-item ${pageNumber <= 1 ? "disabled" : ""}`}>
                  <button
                    className="page-link"
                    onClick={() => setPageNumber(1)}
                    title="Trang đầu"
                  >
                    «
                  </button>
                </li>
                <li className={`page-item ${pageNumber <= 1 ? "disabled" : ""}`}>
                  <button
                    className="page-link"
                    onClick={() => setPageNumber((prev) => Math.max(1, prev - 1))}
                    title="Trang trước"
                  >
                    <ChevronLeftIcon width={14} height={14} />
                  </button>
                </li>

                {pageNumbers.map((p) => (
                  <li
                    key={p}
                    className={`page-item ${pageNumber === p ? "active" : ""}`}
                  >
                    <button
                      className="page-link"
                      onClick={() => setPageNumber(p)}
                    >
                      {p}
                    </button>
                  </li>
                ))}

                <li className={`page-item ${pageNumber >= totalPages ? "disabled" : ""}`}>
                  <button
                    className="page-link"
                    onClick={() => setPageNumber((prev) => Math.min(totalPages, prev + 1))}
                    title="Trang sau"
                  >
                    <ChevronRightIcon width={14} height={14} />
                  </button>
                </li>
                <li className={`page-item ${pageNumber >= totalPages ? "disabled" : ""}`}>
                  <button
                    className="page-link"
                    onClick={() => setPageNumber(totalPages)}
                    title="Trang cuối"
                  >
                    »
                  </button>
                </li>
              </ul>
            </nav>
          </div>
        )}
      </div>
    </div>
  );
}
