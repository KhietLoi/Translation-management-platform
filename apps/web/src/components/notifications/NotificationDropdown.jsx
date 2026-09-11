import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  useNotificationsInfiniteQuery,
  useMarkAllAsReadMutation,
} from "../../hooks/useNotifications";
import NotificationItem from "./NotificationItem";
import { CheckIcon, BellSlashIcon } from "@heroicons/react/24/outline";
import "./Notification.css";

export default function NotificationDropdown({ projectId, onClose, isDarkMode = false }) {
  const navigate = useNavigate();
  const [filterUnreadOnly, setFilterUnreadOnly] = useState(false);

  // Fetch notifications using React Query Infinite Query with projectId
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    isLoading,
    isError,
  } = useNotificationsInfiniteQuery({
    projectId,
    pageSize: 8,
    isRead: filterUnreadOnly ? false : undefined,
  });

  const markAllAsReadMutation = useMarkAllAsReadMutation();

  // Combine items across infinite query pages and deduplicate by item.id
  const rawNotifications = data?.pages.flatMap((page) => page.items) || [];
  const allNotifications = Array.from(
    new Map(
      rawNotifications
        .filter((item) => item && item.id)
        .map((item) => [item.id, item])
    ).values()
  );

  const handleMarkAllRead = async (e) => {
    e.stopPropagation();
    try {
      await markAllAsReadMutation.mutateAsync();
    } catch (err) {
      console.error("Failed to mark all as read:", err);
    }
  };

  const handleViewAll = () => {
    if (onClose) onClose();
    navigate("/notifications");
  };

  return (
    <div
      className={`notification-dropdown-menu ${isDarkMode ? "dark-theme" : ""}`}
      onClick={(e) => e.stopPropagation()}
    >
      {/* Dropdown Header */}
      <div className="notification-dropdown-header">
        <div className="d-flex align-items-center gap-2">
          <h6 className="mb-0 fw-bold fs-6">Thông báo</h6>
          <div className="btn-group btn-group-sm" role="group">
            <button
              type="button"
              className={`btn ${
                !filterUnreadOnly ? "btn-primary" : "btn-outline-secondary"
              } py-0 px-2`}
              style={{ fontSize: "0.75rem" }}
              onClick={() => setFilterUnreadOnly(false)}
            >
              Tất cả
            </button>
            <button
              type="button"
              className={`btn ${
                filterUnreadOnly ? "btn-primary" : "btn-outline-secondary"
              } py-0 px-2`}
              style={{ fontSize: "0.75rem" }}
              onClick={() => setFilterUnreadOnly(true)}
            >
              Chưa đọc
            </button>
          </div>
        </div>

        <button
          onClick={handleMarkAllRead}
          disabled={markAllAsReadMutation.isPending}
          className="btn-mark-all-read d-flex align-items-center gap-1"
          title="Đánh dấu tất cả là đã đọc"
        >
          <CheckIcon width={14} height={14} />
          <span>
            {markAllAsReadMutation.isPending ? "Đang xử lý..." : "Đọc tất cả"}
          </span>
        </button>
      </div>

      {/* Notifications List Container */}
      <div className="notification-list-container">
        {isLoading ? (
          <div className="p-4 text-center text-muted">
            <div className="spinner-border spinner-border-sm me-2" role="status" />
            <span>Đang tải thông báo...</span>
          </div>
        ) : isError ? (
          <div className="p-4 text-center text-danger small">
            Không thể tải thông báo. Vui lòng kiểm tra lại dự án.
          </div>
        ) : allNotifications.length === 0 ? (
          <div className="empty-notification-state">
            <BellSlashIcon width={36} height={36} className="mx-auto mb-2 opacity-50" />
            <div className="fw-medium small">Không có thông báo nào</div>
            <div className="text-muted" style={{ fontSize: "0.75rem" }}>
              {filterUnreadOnly
                ? "Bạn đã đọc hết tất cả thông báo!"
                : "Hộp thư thông báo của bạn đang trống."}
            </div>
          </div>
        ) : (
          <>
            {allNotifications.map((item) => (
              <NotificationItem
                key={item.id}
                item={item}
                onItemClick={() => {
                  if (onClose) onClose();
                }}
                isDarkMode={isDarkMode}
              />
            ))}

            {/* Load More Button */}
            {hasNextPage && (
              <div className="p-2 text-center border-top border-light">
                <button
                  type="button"
                  onClick={() => fetchNextPage()}
                  disabled={isFetchingNextPage}
                  className="btn btn-sm btn-link text-decoration-none text-primary"
                  style={{ fontSize: "0.8rem" }}
                >
                  {isFetchingNextPage ? (
                    <>
                      <span className="spinner-border spinner-border-sm me-1" role="status" />
                      Đang tải...
                    </>
                  ) : (
                    "Tải thêm thông báo"
                  )}
                </button>
              </div>
            )}
          </>
        )}
      </div>

      {/* Dropdown Footer */}
      <div className="p-2 border-top text-center bg-light-subtle">
        <button
          onClick={handleViewAll}
          className="btn btn-sm btn-link text-decoration-none fw-semibold text-primary p-1"
          style={{ fontSize: "0.825rem" }}
        >
          Xem tất cả trong Notification Center →
        </button>
      </div>
    </div>
  );
}
