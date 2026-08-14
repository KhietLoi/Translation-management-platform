import React, { useState, useRef, useEffect } from "react";
import { BellIcon } from "@heroicons/react/24/outline";
import { useUnreadCountQuery } from "../../hooks/useNotifications";
import { useNotificationSignalR } from "../../hooks/useNotificationSignalR";
import NotificationDropdown from "./NotificationDropdown";
import "./Notification.css";

export default function NotificationBell({ projectId, isDarkMode = false }) {
  const [isOpen, setIsOpen] = useState(false);
  const containerRef = useRef(null);

  // Fetch unread count for active projectId via React Query
  const { data: unreadCount = 0 } = useUnreadCountQuery(projectId);

  // SignalR real-time handler hook
  const { hasNewNotificationAnimation } = useNotificationSignalR();

  // Close dropdown when clicking outside
  useEffect(() => {
    function handleClickOutside(event) {
      if (containerRef.current && !containerRef.current.contains(event.target)) {
        setIsOpen(false);
      }
    }

    if (isOpen) {
      document.addEventListener("mousedown", handleClickOutside);
    }
    return () => {
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [isOpen]);

  const toggleDropdown = () => {
    setIsOpen((prev) => !prev);
  };

  const formattedCount = unreadCount > 99 ? "99+" : unreadCount;

  return (
    <div className="position-relative d-inline-block" ref={containerRef}>
      {/* Bell Icon Button */}
      <button
        type="button"
        onClick={toggleDropdown}
        className={`btn btn-light btn-sm rounded-circle p-2 border-0 notification-bell-btn ${
          hasNewNotificationAnimation ? "bell-shake-anim" : ""
        } ${isDarkMode ? "dark-theme" : ""}`}
        title="Thông báo"
        aria-label="Thông báo"
        aria-expanded={isOpen}
      >
        <BellIcon width={20} height={20} />

        {/* Unread Count Badge */}
        {unreadCount > 0 && (
          <span
            className={`notification-badge ${
              hasNewNotificationAnimation ? "ping-anim" : ""
            }`}
          >
            {formattedCount}
          </span>
        )}
      </button>

      {/* Notification Dropdown Menu */}
      {isOpen && (
        <NotificationDropdown
          projectId={projectId}
          onClose={() => setIsOpen(false)}
          isDarkMode={isDarkMode}
        />
      )}
    </div>
  );
}
