import { BrowserRouter, Routes, Route } from "react-router-dom";

import Login from "../pages/Login/Login";
import Register from "../pages/Register/Register";
import Dashboard from "../pages/Dashboard/Dashboard";

import MainLayout from "../layouts/MainLayout";

// import Users from "../pages/Users/Users";
// import Roles from "../pages/Roles/Roles";
// import Permissions from "../pages/Permissions/Permissions";

export default function AppRoutes() {
  return (
    <BrowserRouter>
      <Routes>

        <Route path="/" element={<Login />} />
        <Route path="/register" element={<Register />} />
        <Route path="/logout" element={<Login />} />

        <Route element={<MainLayout />}>
          <Route path="/dashboard" element={<Dashboard />} />

          {/* Thêm sau khi tạo page */}
          {/* <Route path="/users" element={<Users />} /> */}
          {/* <Route path="/roles" element={<Roles />} /> */}
          {/* <Route path="/permissions" element={<Permissions />} /> */}

        </Route>

      </Routes>
    </BrowserRouter>
  );
}