import React, { useEffect, useState } from "react";
import signalRService from "../../services/signalrService";
import { useAuth } from "../../contexts/AuthContext";
import { UserIcon } from "@heroicons/react/24/outline";
import "./ProjectPresence.css";

// Helper to get initials from username or full name
function getInitials(name) {
  if (!name) return "U";
  const parts = name.trim().split(" ");
  if (parts.length >= 2) {
    return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
  }
  return name.slice(0, 2).toUpperCase();
}

// Consistent colors for avatars
const AVATAR_COLORS = [
  "#4f46e5", // Indigo
  "#0891b2", // Cyan
  "#059669", // Emerald
  "#d97706", // Amber
  "#dc2626", // Red
  "#7c3aed", // Violet
  "#db2777", // Pink
  "#2563eb", // Blue
];

function getAvatarColor(identifier) {
  if (!identifier) return AVATAR_COLORS[0];
  let hash = 0;
  for (let i = 0; i < identifier.length; i++) {
    hash = identifier.charCodeAt(i) + ((hash << 5) - hash);
  }
  const index = Math.abs(hash) % AVATAR_COLORS.length;
  return AVATAR_COLORS[index];
}

export default function ProjectPresence({ isDarkMode = false }) {
  const { user } = useAuth();
  const [onlineUsers, setOnlineUsers] = useState([]);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    // Subscribe to real-time OnlineUsersUpdated event
    const unsubscribe = signalRService.subscribePresence((users) => {
      console.log("[ProjectPresence] 👥 Received online users list payload:", users);
      
      let list = [];
      if (Array.isArray(users)) {
        list = users;
      } else if (typeof users === "object" && users !== null) {
        list = Object.values(users);
      }
      setOnlineUsers(list);
    });

    return () => {
      unsubscribe();
    };
  }, []);

  const currentUserName = user?.userName || user?.username || user?.name || "User";
  const currentUserId = user?.id || user?.userId || user?.sub || "me";

  // If onlineUsers list from SignalR hasn't arrived yet, show current logged in user
  const effectiveUsers =
    onlineUsers.length > 0
      ? onlineUsers
      : [
          {
            userId: currentUserId,
            username: currentUserName,
            connectionId: "current-user",
          },
        ];

  const maxVisible = 5;
  const visibleUsers = effectiveUsers.slice(0, maxVisible);
  const extraCount = effectiveUsers.length - maxVisible;

  return (
    <>
      <div
        className={`project-presence-container ms-1 ${isDarkMode ? "dark-theme" : ""}`}
        title="Người dùng đang online trong dự án này"
      >
        <div className="avatar-stack">
          {visibleUsers.map((u, idx) => {
            const name =
              u.username ||
              u.Username ||
              u.userName ||
              u.UserName ||
              u.name ||
              u.Name ||
              "User";
            const initials = getInitials(name);
            const bgColor = getAvatarColor(name);
            const userKey = u.connectionId || u.ConnectionId || u.userId || u.UserId || idx;

            return (
              <div
                key={userKey}
                className="avatar-stack-item presence-tooltip"
                style={{ backgroundColor: bgColor }}
                onClick={() => setShowModal(true)}
              >
                <span>{initials}</span>
                <span className="online-dot-indicator" />
                <span className="tooltip-text">{name} (Online)</span>
              </div>
            );
          })}

          {/* +N Counter Badge when > 5 users online */}
          {extraCount > 0 && (
            <button
              type="button"
              onClick={() => setShowModal(true)}
              className="avatar-counter-badge border-0"
              title={`Xem thêm ${extraCount} người dùng online`}
            >
              +{extraCount}
            </button>
          )}
        </div>
      </div>

      {/* Online Users List Modal */}
      {showModal && (
        <div
          className="modal fade show d-block"
          style={{ backgroundColor: "rgba(0, 0, 0, 0.5)", zIndex: 1060 }}
          onClick={() => setShowModal(false)}
        >
          <div
            className="modal-dialog modal-dialog-centered modal-sm"
            onClick={(e) => e.stopPropagation()}
          >
            <div
              className={`modal-content shadow-lg border-0 rounded-4 ${
                isDarkMode ? "bg-dark text-white border-secondary" : ""
              }`}
            >
              <div className="modal-header border-bottom py-3 px-4">
                <h6 className="modal-title fw-bold mb-0 d-flex align-items-center gap-2 fs-6">
                  <UserIcon width={18} height={18} className="text-primary" />
                  Đang truy cập ({effectiveUsers.length})
                </h6>
                <button
                  type="button"
                  className="btn-close"
                  onClick={() => setShowModal(false)}
                />
              </div>

              <div
                className="modal-body p-3 overflow-auto"
                style={{ maxHeight: "320px" }}
              >
                <div className="d-flex flex-column gap-2">
                  {effectiveUsers.map((u, i) => {
                    const name =
                      u.username ||
                      u.Username ||
                      u.userName ||
                      u.UserName ||
                      u.name ||
                      u.Name ||
                      "User";
                    const initials = getInitials(name);
                    const bgColor = getAvatarColor(name);
                    const userKey = u.connectionId || u.ConnectionId || u.userId || u.UserId || i;

                    return (
                      <div
                        key={userKey}
                        className="d-flex align-items-center gap-2.5 p-2 rounded-3 hover-bg-light"
                      >
                        <div
                          className="rounded-circle text-white fw-bold d-flex align-items-center justify-content-center position-relative flex-shrink-0"
                          style={{
                            width: "36px",
                            height: "36px",
                            backgroundColor: bgColor,
                            fontSize: "0.825rem",
                          }}
                        >
                          {initials}
                          <span className="online-dot-indicator" />
                        </div>

                        <div className="min-w-0 flex-grow-1">
                          <div className="fw-semibold text-truncate small mb-0">
                            {name}
                          </div>
                          <span
                            className="badge bg-success-subtle text-success border border-success-subtle rounded-pill"
                            style={{ fontSize: "0.65rem" }}
                          >
                            ● Online
                          </span>
                        </div>
                      </div>
                    );
                  })}
                </div>
              </div>
            </div>
          </div>
        </div>
      )}
    </>
  );
}
