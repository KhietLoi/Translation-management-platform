import { Outlet, useLocation, useNavigate } from "react-router-dom";
import {
  ChartBarIcon,
  ShieldCheckIcon,
  Cog6ToothIcon,
  ArrowRightOnRectangleIcon,
  UserGroupIcon,
  KeyIcon,
  ChevronDownIcon,
} from "@heroicons/react/24/outline";
import { useState } from "react";
import { ToastContainer} from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const menus = [
  {
    label: "Dashboard",
    path: "/dashboard",
    icon: ChartBarIcon,
  },
  {
    label: "Access Control",
    icon: ShieldCheckIcon,
    children: [
      { label: "Users", path: "/users", icon: UserGroupIcon },
      { label: "Roles", path: "/roles", icon: ShieldCheckIcon },
      { label: "Permissions", path: "/permissions", icon: KeyIcon },
    ],
  },
  {
    label: "Settings",
    path: "/settings",
    icon: Cog6ToothIcon,
  },
];

export default function MainLayout() {
  const navigate = useNavigate();
  const location = useLocation();
  const [openAccess, setOpenAccess] = useState(true);

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/");
  };

  const pageTitle = () => {
    for (const item of menus) {
      if (item.path === location.pathname) return item.label;
      if (item.children) {
        const child = item.children.find((x) => x.path === location.pathname);
        if (child) return child.label;
      }
    }
    return "Dashboard";
  };

  const isActive = (path) => location.pathname === path;

  return (
    <div className="d-flex vh-100 overflow-hidden bg-dark">
      {/* Sidebar */}
      <aside className="sidebar bg-black text-white d-flex flex-column border-end border-warning" style={{ width: "280px" }}>
        {/* Logo */}
        <div className="p-4 text-center border-bottom border-warning">
          <div
            className="mx-auto mb-3 d-flex align-items-center justify-content-center fw-bold border border-warning"
            style={{
              width: "72px",
              height: "72px",
              fontSize: "1.9rem",
              background: "#1a1a1a",
              color: "#d4af37",
            }}
          >
            MS
          </div>
          <h5 className="text-warning fw-bold mb-1">MySolution</h5>
          <small className="text-secondary">Authentication System</small>
        </div>

        {/* Menu */}
        <nav className="flex-grow-1 p-3 overflow-auto">
          {menus.map((item) => {
            const Icon = item.icon;

            if (item.children) {
              const isChildActive = item.children.some((child) => isActive(child.path));

              return (
                <div key={item.label} className="mb-3">
                  <button
                    onClick={() => setOpenAccess(!openAccess)}
                    className={`btn w-100 text-start d-flex align-items-center justify-content-between px-3 py-3 mb-2 border-0 fw-semibold ${
                      isChildActive || openAccess ? "bg-warning text-dark" : "text-white hover-bg-secondary"
                    }`}
                  >
                    <div className="d-flex align-items-center gap-3">
                      <Icon width={24} height={24} />
                      <span>{item.label}</span>
                    </div>
                    <ChevronDownIcon
                      width={20}
                      height={20}
                      className={`transition-transform ${openAccess ? "rotate-180" : ""}`}
                    />
                  </button>

                  {openAccess && (
                    <div className="ms-4">
                      {item.children.map((child) => {
                        const ChildIcon = child.icon;
                        const active = isActive(child.path);

                        return (
                          <button
                            key={child.path}
                            onClick={() => navigate(child.path)}
                            className={`btn w-100 text-start d-flex align-items-center gap-3 mb-1 py-2.5 px-3 border-0 ${
                              active ? "bg-warning text-dark fw-semibold" : "text-white-50 hover-bg-secondary"
                            }`}
                          >
                            <ChildIcon width={18} height={18} />
                            <span>{child.label}</span>
                          </button>
                        );
                      })}
                    </div>
                  )}
                </div>
              );
            }

            const active = isActive(item.path);
            return (
              <button
                key={item.path}
                onClick={() => navigate(item.path)}
                className={`btn w-100 text-start d-flex align-items-center gap-3 mb-2 py-3 px-3 border-0 fw-medium ${
                  active ? "bg-warning text-dark" : "text-white-50 hover-bg-secondary"
                }`}
              >
                <Icon width={24} height={24} />
                <span>{item.label}</span>
              </button>
            );
          })}
        </nav>

        {/* Logout */}
        <div className="p-3 border-top border-warning mt-auto">
          <button
            onClick={logout}
            className="btn btn-outline-danger w-100 d-flex align-items-center gap-3 py-3"
          >
            <ArrowRightOnRectangleIcon width={24} height={24} />
            <span>Logout</span>
          </button>
        </div>
      </aside>

      {/* Main Content */}
      <div className="flex-grow-1 d-flex flex-column">
        <header className="bg-black border-bottom border-warning px-4 py-3 d-flex justify-content-between align-items-center">
          <h5 className="mb-0 fw-bold text-warning">{pageTitle()}</h5>

          <div className="d-flex align-items-center gap-3">
            <span className="text-light">Administrator</span>
            <div
              className="rounded-0 border border-warning bg-black text-warning d-flex align-items-center justify-content-center fw-bold"
              style={{ width: "46px", height: "46px" }}
            >
              A
            </div>
          </div>
        </header>

        <main className="flex-grow-1 p-3 overflow-auto bg-white text-white border-top border-warning">
          <Outlet />
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
        theme="dark"
      />
    </div>
  );
}