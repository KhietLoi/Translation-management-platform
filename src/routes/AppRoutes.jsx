import { Routes, Route } from "react-router-dom";
import Login from "../pages/auth/Login/Login";
import Register from "../pages/auth/Register/Register";
import Dashboard from "../pages/Dashboard/Dashboard";

import MainLayout from "../layouts/MainLayout";

import Users from "../pages/Users/Users";
import Roles from "../pages/Roles/Roles";
import Permissions from "../pages/Permissions/Permissions";
import CheckEmail from "../pages/auth/CheckEmail/CheckEmail";
import VerifyEmail from "../pages/auth/VerifyEmail/VerifyEmail";
import ForgotPassword from "../pages/auth/ForgotPassword/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword/ResetPassword";
import Forbidden from "../pages/Error/Forbidden";
import Profile from "../pages/Profile/Profile";
import ProjectListPage from "../pages/projects/ProjectListPage";
// import Roles from "../pages/Roles/Roles";
// import Permissions from "../pages/Permissions/Permissions";

export default function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<Login />} />
      <Route path="/register" element={<Register />} />
      <Route path="/logout" element={<Login />} />
      <Route path="/checkemail" element={<CheckEmail />} />
      <Route path="/verify-email" element={<VerifyEmail />} />
      <Route path="/forgot-password" element={<ForgotPassword />} />
      <Route path="/reset-password" element={<ResetPassword />} />
      <Route path="/403" element={<Forbidden />} />
      <Route element={<MainLayout />}>
        <Route path="/dashboard" element={<Dashboard />} />
        <Route path="/users" element={<Users />} />
        <Route path="/roles" element={<Roles />} />
        <Route path="/permissions" element={<Permissions />} />
        <Route
          path="/profile"
          element={<Profile />}
        />
        <Route
          path="/projects"
          element={<ProjectListPage />}
        />
      </Route>

    </Routes>
  );
}