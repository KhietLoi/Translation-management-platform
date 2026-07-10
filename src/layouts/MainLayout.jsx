import { Outlet, useLocation, useNavigate } from "react-router-dom";
import {
  ChartBarIcon,
  UserGroupIcon,
  ShieldCheckIcon,
  KeyIcon,
  ArrowRightOnRectangleIcon,
} from "@heroicons/react/24/outline";

const menus = [
  { label: "Dashboard", path: "/dashboard", icon: ChartBarIcon },
  { label: "Users", path: "/users", icon: UserGroupIcon },
  { label: "Roles", path: "/roles", icon: ShieldCheckIcon },
  { label: "Permissions", path: "/permissions", icon: KeyIcon },
];

export default function MainLayout() {
  const navigate = useNavigate();
  const location = useLocation();

  const logout = () => {
    localStorage.removeItem("token");
    navigate("/");
  };

  return (
    <div className="d-flex vh-100 overflow-hidden">

      {/* Sidebar */}
      <aside
        className="bg-dark text-white d-flex flex-column"
        style={{ width: "260px" }}
      >
        {/* Logo */}
        <div className="text-center mb-4 p-4">
            <div className="logo">
              MS
            </div>
            <h3 className="fw-bold">
              MySolution
            </h3>
            <p className="text-muted">
              Welcome back
            </p>
        </div>

        {/* Menu */}
        <nav className="flex-grow-1 p-3 overflow-auto">

          {menus.map((item) => {
            const Icon = item.icon;
            const active = location.pathname === item.path;

            return (
              <button
                key={item.path}
                onClick={() => navigate(item.path)}
                className={`btn w-100 text-start d-flex align-items-center gap-3 mb-2 py-3 px-3 ${
                  active
                    ? "btn-primary"
                    : "btn-dark text-white"
                }`}
              >
                <Icon style={{ width: 22, height: 22 }} />
                <span>{item.label}</span>
              </button>
            );
          })}

        </nav>


        {/* Logout */}
        <div className="p-3 border-top border-secondary">

          <button
            onClick={logout}
            className="btn btn-danger text-white w-100 d-flex align-items-center gap-3 py-3 px-3"
          >
            <ArrowRightOnRectangleIcon
              style={{ width:22, height:22 }}
            />
            Logout
          </button>

        </div>

      </aside>



      {/* Right Content */}
      <div className="flex-grow-1 d-flex flex-column">

        {/* Content */}
        <main className="flex-grow-1 bg-light p-4 overflow-auto">

          <Outlet />

        </main>

      </div>

    </div>
  );
}