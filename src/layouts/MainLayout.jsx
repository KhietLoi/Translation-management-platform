import { useState } from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import {
  ChartBarIcon,
  ShieldCheckIcon,
  Cog6ToothIcon,
  ArrowRightOnRectangleIcon,
  UserGroupIcon,
  KeyIcon,
  Squares2X2Icon,
  FolderIcon,
  Bars3BottomLeftIcon,
  ArrowDownTrayIcon,
  ArrowsUpDownIcon,
  BellIcon
} from "@heroicons/react/24/outline";
import { ToastContainer } from "react-toastify";
import { useAuth } from "../contexts/AuthContext";
import "react-toastify/dist/ReactToastify.css";
import "./MainLayout.css";

import { logout } from "../services/authService";

export default function MainLayout() {
  const { user, setUser } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  // Access control logic
  const permissions = user?.permissions ?? [];
  const canViewUsers = permissions.includes("USER_VIEW");
  const canViewRoles = permissions.includes("ROLE_VIEW");
  const canViewPermissions = permissions.includes("PERMISSION_VIEW");
  const hasAccessControlPermission = canViewUsers || canViewRoles || canViewPermissions;

  // Restructured menus
  const menuCategories = [
    {
      title: "WORKSPACE",
      items: [
        { label: "Dashboard", path: "/dashboard", icon: Squares2X2Icon },
        { label: "Projects", path: "/projects", icon: FolderIcon },
        { label: "Translations", path: "/translations", icon: Bars3BottomLeftIcon },
      ],
    },
    hasAccessControlPermission && {
      title: "ACCESS CONTROL",
      items: [
        canViewUsers && { label: "Users", path: "/users", icon: UserGroupIcon },
        canViewRoles && { label: "Roles", path: "/roles", icon: ShieldCheckIcon },
        canViewPermissions && { label: "Permissions", path: "/permissions", icon: KeyIcon },
      ].filter(Boolean),
    },
    {
      title: "DELIVERY",
      items: [
        { label: "Publish", path: "/publish", icon: ArrowDownTrayIcon },
        { label: "Import / Export", path: "/import-export", icon: ArrowsUpDownIcon },
      ],
    },
    {
      title: "PLATFORM",
      items: [
        { label: "API Keys", path: "/api-keys", icon: KeyIcon },
        { label: "Notifications", path: "/notifications", icon: BellIcon, badge: 5 },
        { label: "Settings", path: "/settings", icon: Cog6ToothIcon },
      ],
    },
  ].filter(Boolean);

  // Logout handler using authService API
  const handleLogout = async () => {
    try {
      await logout();
    } catch (err) {
      console.error("Logout error:", err);
    } finally {
      if (setUser) setUser(null);
      navigate("/", { replace: true });
    }
  };
  const pageTitle = () => {
    for (const category of menuCategories) {
      const activeItem = category.items.find((item) => item.path === location.pathname);
      if (activeItem) return activeItem.label;
    }
    return "Dashboard";
  };

  const isActive = (path) => location.pathname === path;

  return (
    <>
      <div className="d-flex vh-100 overflow-hidden" style={{ backgroundColor: "-#11121c" }}>
        {/* Sidebar */}
        <aside
          className="sidebar text-white d-flex flex-column border-end"
          style={{ width: "260px", backgroundColor: "#161822", borderColor: "#282a36" }}
        >
          {/* Logo / Brand */}
          <div className="p-4 d-flex align-items-center justify-content-center gap-3">
            <div className="logo">
              MS
            </div>

            <div>
              <h5 className="mb-0 fw-bold text-white">
                MySolution
              </h5>
              <small className="text-secondary">
                Translation Platform
              </small>
            </div>
          </div>

          {/* Menu Navigation */}
          <nav className="flex-grow-1 px-3 pb-3 overflow-auto">
            {menuCategories.map((category, idx) => (
              <div key={idx} className="mb-4">
                <h6
                  className="text-uppercase fw-bold mb-2 ms-2"
                  style={{ fontSize: "0.7rem", color: "#6b7280", letterSpacing: "0.05em" }}
                >
                  {category.title}
                </h6>
                {category.items.map((item) => {
                  const Icon = item.icon;
                  const active = isActive(item.path);

                  return (
                    <button
                      key={item.path}
                      onClick={() => navigate(item.path)}
                      className={`btn w-100 text-start d-flex align-items-center justify-content-between mb-1 py-2 px-3 border-0 rounded sidebar-link ${active ? "text-white fw-semibold" : "text-white-50"
                        }`}
                      style={{
                        backgroundColor: active ? "#282a3a" : "transparent",
                      }}
                    >
                      <div className="d-flex align-items-center gap-3">
                        <Icon width={20} height={20} style={{ color: active ? "#fff" : "#9ca3af" }} />
                        <span style={{ fontSize: "0.95rem" }}>{item.label}</span>
                      </div>
                      {item.badge && (
                        <span className="badge rounded-pill bg-danger" style={{ fontSize: "0.7rem" }}>
                          {item.badge}
                        </span>
                      )}
                    </button>
                  );
                })}
              </div>
            ))}
          </nav>

          {/* Logout - Danger Block */}
          <div className="mt-auto px-3 pb-3 pt-3" style={{ borderTop: "1px solid #282a36" }}>
            <button
              onClick={handleLogout}
              className="btn btn-outline-danger w-100 d-flex align-items-center justify-content-center gap-2 py-2 border-0 rounded"
              style={{ transition: "background-color 0.2s" }}
              onMouseOver={(e) => (e.currentTarget.className = "btn btn-danger w-100 d-flex align-items-center justify-content-center gap-2 py-2 border-0 rounded")}
              onMouseOut={(e) => (e.currentTarget.className = "btn btn-outline-danger w-100 d-flex align-items-center justify-content-center gap-2 py-2 border-0 rounded text-danger")}
              title="Logout"
            >
              <ArrowRightOnRectangleIcon width={20} height={20} />
              <span className="fw-semibold" style={{ fontSize: "0.95rem" }}>Logout</span>
            </button>
          </div>
        </aside>

        {/* Main Content Area */}
        <div className="flex-grow-1 d-flex flex-column bg-light">
          {/* Top Header */}
          <header className="bg-white border-bottom px-4 py-3 d-flex justify-content-between align-items-center shadow-sm">
            <h5 className="mb-0 fw-bold text-dark">{pageTitle()}</h5>

            <div className="d-flex align-items-center gap-3">
              <span className="text-dark fw-medium">
                {user?.userName}
              </span>

              <button
                onClick={() => navigate("/profile")}
                className="border-0 bg-transparent p-0"
                style={{ transition: "transform 0.2s ease" }}
                onMouseOver={(e) => (e.currentTarget.style.transform = "scale(1.05)")}
                onMouseOut={(e) => (e.currentTarget.style.transform = "scale(1)")}
              >
                {user?.avatarBlobName ? (
                  <img
                    src={getAvatarUrl(user.avatarBlobName)} // Ensure getAvatarUrl is defined/imported if you keep this logic
                    alt="avatar"
                    className="rounded-circle border shadow-sm"
                    width="40"
                    height="40"
                    style={{ objectFit: "cover" }}
                  />
                ) : (
                  <div
                    className="rounded-circle border bg-secondary text-white d-flex align-items-center justify-content-center fw-bold shadow-sm"
                    style={{
                      width: "40px",
                      height: "40px"
                    }}
                  >
                    {user?.userName?.charAt(0)?.toUpperCase() || "U"}
                  </div>
                )}
              </button>
            </div>
          </header>

          <main className="flex-grow-1 p-4 overflow-auto">
            {/* Added key based on pathname to trigger animation on route change */}
            <div key={location.pathname} className="fade-in h-100">
              <Outlet />
            </div>
          </main>
        </div>

        <ToastContainer
          position="top-right"
          autoClose={3000}
          hideProgressBar={false}
          newestOnTop
          closeOnClick
          pauseOnHover
          draggable
          theme="light"
        />
      </div>
    </>
  );
}