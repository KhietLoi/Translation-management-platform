import { Routes, Route, Navigate } from "react-router-dom";
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
import CreateProject from "../components/projects/CreateProject";
import ProjectDetailPage from "../pages/projects/ProjectDetailPage";
import TranslationManagementPage from "../pages/translations/TranslationManagementPage";
import ApiKeyManagementPage from "../pages/apikeys/ApiKeyManagementPage";
import ImportExportPage from "../pages/delivery/ImportExportPage";
import PublishPage from "../pages/delivery/PublishPage";
import NotificationCenterPage from "../components/notifications/NotificationCenterPage";
import PermissionGuard from "../components/authorization/PermissionGuard";
import { PERMISSIONS } from "../constants/permissions";

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
        <Route
          path="/users"
          element={
            <PermissionGuard permission={PERMISSIONS.USER.VIEW} fallback={<Navigate to="/403" replace />}>
              <Users />
            </PermissionGuard>
          }
        />
        <Route
          path="/roles"
          element={
            <PermissionGuard permission={PERMISSIONS.ROLE.VIEW} fallback={<Navigate to="/403" replace />}>
              <Roles />
            </PermissionGuard>
          }
        />
        <Route
          path="/permissions"
          element={
            <PermissionGuard permission={PERMISSIONS.PERMISSION.VIEW} fallback={<Navigate to="/403" replace />}>
              <Permissions />
            </PermissionGuard>
          }
        />
        <Route
          path="/profile"
          element={<Profile />}
        />
        <Route path="/projects/:id" element={<ProjectDetailPage />} />
        <Route
          path="/projects"
          element={
            <PermissionGuard permission={PERMISSIONS.PROJECT.VIEW} fallback={<Navigate to="/403" replace />}>
              <ProjectListPage />
            </PermissionGuard>
          }
        />
        <Route
          path="/projects/create"
          element={
            <PermissionGuard permission={PERMISSIONS.PROJECT.CREATE} fallback={<Navigate to="/403" replace />}>
              <CreateProject />
            </PermissionGuard>
          }
        />
        <Route
          path="/translations"
          element={
            <PermissionGuard permission={PERMISSIONS.TRANSLATION.VIEW} fallback={<Navigate to="/403" replace />}>
              <TranslationManagementPage />
            </PermissionGuard>
          }
        />
        <Route
          path="/import-export"
          element={<ImportExportPage />}
        />
        <Route
          path="/publish"
          element={
            <PermissionGuard permission={PERMISSIONS.TRANSLATION.PUBLISH} fallback={<Navigate to="/403" replace />}>
              <PublishPage />
            </PermissionGuard>
          }
        />
        <Route
          path="/api-keys"
          element={
            <PermissionGuard permission={PERMISSIONS.API_KEY.VIEW} fallback={<Navigate to="/403" replace />}>
              <ApiKeyManagementPage />
            </PermissionGuard>
          }
        />
        <Route
          path="/notifications"
          element={<NotificationCenterPage />}
        />
      </Route>

    </Routes>
  );
}