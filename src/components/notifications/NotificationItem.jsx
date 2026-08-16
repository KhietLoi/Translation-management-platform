import React from "react";
import { useNavigate } from "react-router-dom";
import {
  CheckCircleIcon,
  ExclamationTriangleIcon,
  ExclamationCircleIcon,
  InformationCircleIcon,
  ArrowDownTrayIcon,
  KeyIcon,
  ChatBubbleLeftIcon,
  ArrowUpTrayIcon,
  ClipboardDocumentCheckIcon,
} from "@heroicons/react/24/outline";
import { formatTimeAgo } from "../../utils/timeUtils";
import { useMarkAsReadMutation } from "../../hooks/useNotifications";
import "./Notification.css";

/**
 * Returns icon component based on type or title keywords
 */
function getNotificationIcon(type, title = "") {
  const lowerTitle = (title || "").toLowerCase();

  if (lowerTitle.includes("batch review") || lowerTitle.includes("duyệt hàng loạt")) {
    return <ClipboardDocumentCheckIcon width={18} height={18} />;
  }

  if (lowerTitle.includes("import")) {
    return <ArrowDownTrayIcon width={18} height={18} />;
  }

  if (lowerTitle.includes("export")) {
    return <ArrowUpTrayIcon width={18} height={18} />;
  }

  if (lowerTitle.includes("api key") || lowerTitle.includes("key")) {
    return <KeyIcon width={18} height={18} />;
  }

  if (lowerTitle.includes("bình luận") || lowerTitle.includes("comment")) {
    return <ChatBubbleLeftIcon width={18} height={18} />;
  }

  if (lowerTitle.includes("publish") || type === 2 || type === "Success") {
    return <CheckCircleIcon width={18} height={18} />;
  }

  if (type === 3 || type === "Warning") {
    return <ExclamationTriangleIcon width={18} height={18} />;
  }

  if (type === 4 || type === "Error") {
    return <ExclamationCircleIcon width={18} height={18} />;
  }

  return <InformationCircleIcon width={18} height={18} />;
}

/**
 * Returns CSS class for type icon badge
 */
function getTypeIconClass(type, title = "") {
  const lowerTitle = (title || "").toLowerCase();

  if (lowerTitle.includes("batch review") || lowerTitle.includes("duyệt hàng loạt")) {
    return "info";
  }

  if (type === 2 || type === "Success" || lowerTitle.includes("completed")) {
    return "success";
  }
  if (type === 3 || type === "Warning") {
    return "warning";
  }
  if (type === 4 || type === "Error" || lowerTitle.includes("failed")) {
    return "error";
  }

  const strType = (type || "").toString().toLowerCase();
  if (strType === "success") return "success";
  if (strType === "warning") return "warning";
  if (strType === "error") return "error";

  return "info";
}

export default function NotificationItem({ item, onItemClick, isDarkMode = false }) {
  const navigate = useNavigate();
  const markAsReadMutation = useMarkAsReadMutation();

  const handleClick = (e) => {
    e.preventDefault();

    // 1. Mark as read if unread
    if (!item.isRead && item.id) {
      markAsReadMutation.mutate(item.id);
    }

    // 2. Callback for parent (e.g. close dropdown)
    if (onItemClick) {
      onItemClick(item);
    }

    // 3. Dispatch event to open detail modal
    if (item.id) {
      window.dispatchEvent(
        new CustomEvent("showNotificationDetail", {
          detail: { id: item.id }
        })
      );
    } else if (item.navigationUrl) {
      navigate(item.navigationUrl);
    }
  };

  const iconClass = getTypeIconClass(item.type, item.title);
  const timeAgoText = formatTimeAgo(item.createdAt);
  const senderName = item.triggeredByUserName || item.createdBy;

  return (
    <div
      onClick={handleClick}
      className={`notification-item ${!item.isRead ? "unread" : ""} ${
        item.isNew ? "new-arrival" : ""
      } ${isDarkMode ? "dark-theme" : ""}`}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => e.key === "Enter" && handleClick(e)}
    >
      <div className={`type-icon-wrapper ${iconClass}`}>
        {getNotificationIcon(item.type, item.title)}
      </div>

      <div className="flex-grow-1 min-w-0">
        <div className="d-flex justify-content-between align-items-baseline gap-2">
          <div className="notification-title text-truncate">{item.title}</div>
          <span className="notification-time">{timeAgoText}</span>
        </div>

        <div className="notification-message">
          {item.message}
          {senderName && (
            <span className="ms-1 text-muted" style={{ fontSize: "0.75rem" }}>
              (bởi {senderName})
            </span>
          )}
        </div>
      </div>
    </div>
  );
}
