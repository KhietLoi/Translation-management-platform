import { useState, useEffect, useRef } from "react";
import { Outlet, useLocation, useNavigate } from "react-router-dom";
import {
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
  BellIcon,
  MagnifyingGlassIcon,
  ChevronDownIcon,
  Bars3Icon,
} from "@heroicons/react/24/outline";
import { ToastContainer } from "react-toastify";
import { useAuth } from "../contexts/AuthContext";
import { getProjects } from "../services/projectService";
import { logout } from "../services/authService";
import signalRService, { getAuthUser } from "../services/signalrService";
import NotificationBell from "../components/notifications/NotificationBell";
import NotificationDetailModal from "../components/notifications/NotificationDetailModal";
import ProjectPresence from "../components/presence/ProjectPresence";
import { useUnreadCountQuery } from "../hooks/useNotifications";
import "react-toastify/dist/ReactToastify.css";
import "./MainLayout.css";

export default function MainLayout() {
  const { user, setUser } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  // Mobile sidebar state
  const [sidebarOpen, setSidebarOpen] = useState(false);

  // Unread notifications count from React Query
  const { data: unreadCount = 0 } = useUnreadCountQuery();

  // Projects & Top Bar Global State
  const [projects, setProjects] = useState([]);
  const [selectedProjectId, setSelectedProjectId] = useState(
    () => localStorage.getItem("selectedProjectId") || ""
  );
  const [env, setEnv] = useState("Production");
  const [searchQuery, setSearchQuery] = useState("");

  // Notification Detail Modal State
  const [detailNotificationId, setDetailNotificationId] = useState(null);

  // Close sidebar on route change on mobile
  useEffect(() => {
    setSidebarOpen(false);
  }, [location.pathname]);

  // Load Projects for Header Selector
  useEffect(() => {
    loadProjects();

    const handleNotification = (event) => {
      console.log("[MainLayout] Received translationNotification, reloading projects:", event.detail);
      loadProjects();
    };

    const handleProjectsChanged = (event) => {
      const selectId = event.detail?.selectProjectId || null;
      console.log("[MainLayout] Received projectsChanged, reloading projects. Select ID:", selectId);
      loadProjects(selectId);
    };

    window.addEventListener("translationNotification", handleNotification);
    window.addEventListener("projectsChanged", handleProjectsChanged);

    return () => {
      window.removeEventListener("translationNotification", handleNotification);
      window.removeEventListener("projectsChanged", handleProjectsChanged);
    };
  }, []);

  // Initialize SignalR Connection
  useEffect(() => {
    signalRService.startConnection();
  }, []);

  // Listen to showNotificationDetail events to open the detail modal
  useEffect(() => {
    const handleShowDetail = (event) => {
      if (event.detail?.id) {
        setDetailNotificationId(event.detail.id);
      }
    };
    window.addEventListener("showNotificationDetail", handleShowDetail);
    return () => {
      window.removeEventListener("showNotificationDetail", handleShowDetail);
    };
  }, []);

  const prevProjectIdRef = useRef(null);

  useEffect(() => {
    if (!selectedProjectId) return;

    const auth = getAuthUser(user);
    if (!auth || !auth.userId) return;

    // Leave previous project group when switching
    if (prevProjectIdRef.current && prevProjectIdRef.current !== selectedProjectId) {
      signalRService.leaveProject(prevProjectIdRef.current);
    }
    prevProjectIdRef.current = selectedProjectId;

    // Join active selected project group on SignalR
    signalRService.joinProject(selectedProjectId, auth.userId, auth.username);
  }, [selectedProjectId, user]);

  const loadProjects = async (selectId = null) => {
    try {
      const res = await getProjects();
      const list = res.data?.projects || res.data || [];
      setProjects(list);
      if (list.length > 0) {
        const savedId = selectId || localStorage.getItem("selectedProjectId");
        const exists = list.some((p) => p.id === savedId);
        const activeId = exists ? savedId : list[0].id;
        setSelectedProjectId(activeId);
        localStorage.setItem("selectedProjectId", activeId);
      } else {
        setSelectedProjectId("");
        localStorage.removeItem("selectedProjectId");
      }
    } catch (error) {
      console.error("Failed to load projects for header:", error);
    }
  };

  const handleProjectSelect = (id) => {
    setSelectedProjectId(id);
    localStorage.setItem("selectedProjectId", id);
  };

  // Access control logic
  const permissions = user?.permissions ?? [];
  const canViewUsers = permissions.includes("USER_VIEW");
  const canViewRoles = permissions.includes("ROLE_VIEW");
  const canViewProjects = permissions.includes("PROJECT_VIEW");
  const canViewPermissions = permissions.includes("PERMISSION_VIEW");
  const canViewPublish = permissions.includes("TRANSLATION_PUBLISH");
  const canViewApiKey = permissions.includes("APIKEY_VIEW") || permissions.includes("API_KEY_VIEW");

  const hasAccessControlPermission =
    canViewUsers || canViewRoles || canViewPermissions;

  // Restructured menus in English
  const menuCategories = [
    {
      title: "WORKSPACE",
      items: [
        { label: "Dashboard", path: "/dashboard", icon: Squares2X2Icon },
        canViewProjects && { label: "Projects", path: "/projects", icon: FolderIcon },
        { label: "Translations", path: "/translations", icon: Bars3BottomLeftIcon },
      ].filter(Boolean),
    },
    hasAccessControlPermission && {
      title: "ACCESS CONTROL",
      items: [
        canViewUsers && { label: "Users", path: "/users", icon: UserGroupIcon },
        canViewRoles && { label: "Roles", path: "/roles", icon: ShieldCheckIcon },
      ].filter(Boolean),
    },
    {
      title: "DELIVERY",
      items: [
        canViewPublish && { label: "Publish", path: "/publish", icon: ArrowDownTrayIcon },
        { label: "Import / Export", path: "/import-export", icon: ArrowsUpDownIcon },
      ].filter(Boolean),
    },
    {
      title: "PLATFORM",
      items: [
        canViewApiKey && { label: "API Keys", path: "/api-keys", icon: KeyIcon },
        {
          label: "Notifications",
          path: "/notifications",
          icon: BellIcon,
          badge: unreadCount > 0 ? unreadCount : undefined,
        }
      ].filter(Boolean),
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

  const isActive = (path) => location.pathname === path;

  // Helper for user initials
  const getUserInitials = (name) => {
    if (!name) return "TA";
    const parts = name.trim().split(" ");
    if (parts.length >= 2) {
      return (parts[0][0] + parts[parts.length - 1][0]).toUpperCase();
    }
    return name.slice(0, 2).toUpperCase();
  };

  return (
    <>
      <div className="d-flex vh-100 overflow-hidden" style={{ backgroundColor: "#11121c" }}>
        {/* Mobile Backdrop */}
        {sidebarOpen && (
          <div
            className="sidebar-overlay d-lg-none"
            onClick={() => setSidebarOpen(false)}
          />
        )}

        {/* Sidebar */}
        <aside
          className={`sidebar text-white d-flex flex-column border-end ${sidebarOpen ? "show" : ""
            }`}
          style={{
            width: "260px",
            backgroundColor: "#161822",
            borderColor: "#282a36",
          }}
        >
          {/* Original MySolution Logo / Brand Header */}
          <div className="p-4 d-flex align-items-center gap-3">
            <div className="logo">MS</div>

            <div>
              <h5 className="mb-0 fw-bold text-white fs-6">MySolution</h5>
              <small className="text-secondary" style={{ fontSize: "0.75rem" }}>
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
                  style={{
                    fontSize: "0.7rem",
                    color: "#6b7280",
                    letterSpacing: "0.05em",
                  }}
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
                        <Icon
                          width={18}
                          height={18}
                          style={{ color: active ? "#fff" : "#9ca3af" }}
                        />
                        <span style={{ fontSize: "0.9rem" }}>{item.label}</span>
                      </div>
                      {item.badge && (
                        <span
                          className="badge rounded-pill bg-danger"
                          style={{ fontSize: "0.7rem" }}
                        >
                          {item.badge}
                        </span>
                      )}
                    </button>
                  );
                })}
              </div>
            ))}
          </nav>

          {/* User Profile Card at Sidebar Bottom */}
          <div
            className="mt-auto p-3 d-flex align-items-center justify-content-between border-top"
            style={{ borderColor: "#282a36", backgroundColor: "#11121c" }}
          >
            <div
              className="d-flex align-items-center gap-2 cursor-pointer"
              onClick={() => navigate("/profile")}
            >
              <div
                className="rounded-circle d-flex align-items-center justify-content-center text-white fw-bold shadow-sm"
                style={{
                  width: "36px",
                  height: "36px",
                  backgroundColor: "#ea580c",
                  fontSize: "0.85rem",
                }}
              >
                {getUserInitials(user?.userName)}
              </div>
              <div className="overflow-hidden" style={{ maxWidth: "130px" }}>
                <div className="fw-semibold text-white text-truncate small">
                  {user?.userName || "Tran An"}
                </div>
                <div
                  className="text-secondary text-truncate"
                  style={{ fontSize: "0.7rem" }}
                >
                  {user?.roles?.[0] || "Backend Intern"}
                </div>
              </div>
            </div>

            <button
              onClick={handleLogout}
              className="btn btn-link text-white-50 p-1 border-0 hover-text-danger"
              title="Logout"
            >
              <ArrowRightOnRectangleIcon width={18} height={18} />
            </button>
          </div>
        </aside>

        {/* Main Content Area */}
        <div className="flex-grow-1 d-flex flex-column bg-light overflow-hidden">
          {/* HEADER INTEGRATING CONTROL BAR WITH RESPONSIVE HAMBURGER */}
          <header className="bg-white border-bottom px-3 px-md-4 py-2.5 d-flex justify-content-between align-items-center shadow-sm flex-wrap gap-2">
            {/* Left Controls: Hamburger on mobile + Project Selector + Env Switcher */}
            <div className="d-flex align-items-center gap-2 gap-md-3">
              <button
                className="btn btn-light btn-sm d-lg-none p-1.5 border-0 text-dark me-1"
                onClick={() => setSidebarOpen(!sidebarOpen)}
                title="Toggle Sidebar"
              >
                <Bars3Icon width={22} height={22} />
              </button>

              <select
                className="form-select border-0 bg-light fw-bold text-dark rounded-3 px-2 px-md-3 py-1.5 cursor-pointer"
                style={{ width: "170px", maxWidth: "200px", fontSize: "0.85rem" }}
                value={selectedProjectId}
                onChange={(e) => handleProjectSelect(e.target.value)}
              >
                {projects.map((p) => (
                  <option key={p.id} value={p.id}>
                    ■ {p.name}
                  </option>
                ))}
                {projects.length === 0 && (
                  <option value="">-- Select Project --</option>
                )}
              </select>

              {/* Online Users Presence Component */}
              <ProjectPresence />

              <div className="env-selector">
                <button
                  type="button"
                  className={`env-pill ${env === "Production" ? "active" : ""}`}
                  onClick={() => setEnv("Production")}
                >
                  Production
                </button>
                <button
                  type="button"
                  className={`env-pill ${env === "Staging" ? "active" : ""}`}
                  onClick={() => setEnv("Staging")}
                >
                  Staging
                </button>
              </div>
            </div>

            {/* Right Controls: Responsive Search Input, Bell Icon, Account Avatar */}
            <div className="d-flex align-items-center gap-2 gap-md-3">
              <div
                className="position-relative d-none d-sm-block header-search-box"
                style={{ width: "260px", maxWidth: "100%" }}
              >
                <MagnifyingGlassIcon
                  width={16}
                  className="position-absolute top-50 translate-middle-y start-0 ms-3 text-muted"
                />
                <input
                  type="text"
                  className="form-control border-0 bg-light rounded-3 ps-5 pe-4 py-1.5 text-dark"
                  style={{ fontSize: "0.85rem" }}
                  placeholder="Search keys, projects..."
                  value={searchQuery}
                  onChange={(e) => setSearchQuery(e.target.value)}
                />
                <ChevronDownIcon
                  width={12}
                  className="position-absolute top-50 translate-middle-y end-0 me-3 text-muted pointer-events-none"
                />
              </div>

              {/* Notification Bell Component */}
              <NotificationBell projectId={selectedProjectId} />

              <button
                onClick={() => navigate("/profile")}
                className="border-0 bg-transparent p-0"
                style={{ transition: "transform 0.2s ease" }}
                onMouseOver={(e) =>
                  (e.currentTarget.style.transform = "scale(1.05)")
                }
                onMouseOut={(e) => (e.currentTarget.style.transform = "scale(1)")}
                title="Account Settings"
              >
                <div
                  className="rounded-circle text-white d-flex align-items-center justify-content-center fw-bold shadow-sm"
                  style={{
                    width: "36px",
                    height: "36px",
                    backgroundColor: "#ea580c",
                    fontSize: "0.85rem",
                  }}
                >
                  {getUserInitials(user?.userName)}
                </div>
              </button>
            </div>
          </header>

          <main className="flex-grow-1 p-3 p-md-4 overflow-auto">
            <div key={location.pathname} className="fade-in h-100">
              <Outlet
                context={{
                  projects,
                  selectedProjectId,
                  setSelectedProjectId,
                  env,
                  setEnv,
                  searchQuery,
                  setSearchQuery,
                }}
              />
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

        {detailNotificationId && (
          <NotificationDetailModal
            notificationId={detailNotificationId}
            onClose={() => setDetailNotificationId(null)}
          />
        )}
      </div>
    </>
  );
}