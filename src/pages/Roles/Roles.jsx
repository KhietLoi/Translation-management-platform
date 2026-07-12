import { useEffect, useState } from "react";
import { MagnifyingGlassIcon, PlusIcon } from "@heroicons/react/24/outline";
import { toast } from "react-toastify";

import RoleTable from "../../components/roles/RoleTable";
import RoleModal from "../../components/roles/RoleModal";
import DeleteRoleModal from "../../components/roles/DeleteRoleModal";
import RolePermissionModal from "../../components/roles/RolePermissionModal";

import { roleService } from "../../services/roleService";
import { permissionService } from "../../services/permissionService";

export default function Roles() {
  const [roles, setRoles] = useState([]);
  const [keyword, setKeyword] = useState("");
  const [loading, setLoading] = useState(false);
  const [page, setPage] = useState(1);
  const [paging, setPaging] = useState({
    page: 1,
    limit: 5,
    totalItem: 0,
    totalPage: 0,
  });

  const [showModal, setShowModal] = useState(false);
  const [showDelete, setShowDelete] = useState(false);
  const [showPermissionModal, setShowPermissionModal] = useState(false);
  const [mode, setMode] = useState("create");
  const [selectedRole, setSelectedRole] = useState(null);

  // Permission
  const [permissions, setPermissions] = useState([]);
  const [rolePermissions, setRolePermissions] = useState([]);

  // =========================
  // LOAD ROLES
  // =========================
  const loadRoles = async (currentPage = page, search = keyword) => {
    try {
      setLoading(true);
      const response = await roleService.getRolesPaging(currentPage, 5, search);
      const data = response.data.data;

      setRoles(data.roles);
      setPaging(data.paging);
    } catch (error) {
      console.error(error);
      toast.error("Failed to load roles");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadRoles();
  }, [page]);

  const handleSearch = () => {
    setPage(1);
    loadRoles(1, keyword);
  };

  // =========================
  // CREATE
  // =========================
  const openCreate = () => {
    setMode("create");
    setSelectedRole(null);
    setShowModal(true);
  };

  const handleCreate = async (form) => {
    try {
      await roleService.createRole(form);
      toast.success("Role created successfully");
      setShowModal(false);
      loadRoles();
    } catch (error) {
      toast.error("Create role failed");
    }
  };

  // =========================
  // UPDATE
  // =========================
  const openEdit = (role) => {
    setMode("update");
    setSelectedRole(role);
    setShowModal(true);
  };

  const handleUpdate = async (form) => {
    try {
      await roleService.updateRole(selectedRole.id, form);
      toast.success("Role updated successfully");
      setShowModal(false);
      loadRoles();
    } catch (error) {
      toast.error("Update role failed");
    }
  };

  // =========================
  // DELETE
  // =========================
  const openDelete = (role) => {
    setSelectedRole(role);
    setShowDelete(true);
  };

  const handleDelete = async () => {
    try {
      await roleService.deleteRole(selectedRole.id);
      toast.success("Role deleted successfully");
      setShowDelete(false);
      loadRoles();
    } catch (error) {
      toast.error("Delete role failed");
    }
  };

  // =========================
  // ROLE PERMISSION
  // =========================
  const handleOpenPermissionModal = async (role) => {
    try {
      setLoading(true);

      // Lấy tất cả permission
      const permissionResponse = await permissionService.getAllPermissions();

      // Lấy role + permission hiện tại
      const roleResponse = await roleService.getRoleById(role.id);

      const allPermissions = permissionResponse.data.data.roles;
      const currentPermissions = roleResponse.data.data.permissions;

      setPermissions(allPermissions);
      setRolePermissions(currentPermissions.map((x) => x.id));
      setSelectedRole(role);
      setShowPermissionModal(true);
    } catch (error) {
      console.error(error);
      toast.error("Load permissions failed");
    } finally {
      setLoading(false);
    }
  };

  const handleSavePermissions = async (payload) => {
    try {
      await roleService.updatePermissions(payload);
      toast.success("Permissions updated successfully");
      setShowPermissionModal(false);
    } catch (error) {
      console.error(error);
      toast.error("Update permissions failed");
    }
  };

  return (
    <div className="bg-white p-4 min-vh-100">
      {/* HEADER */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="fw-bold">Roles</h3>
          <p className="text-muted">Manage system roles</p>
        </div>
        <button className="btn btn-warning" onClick={openCreate}>
          <PlusIcon width={18} /> Add Role
        </button>
      </div>

      <div className="card shadow-sm border-0">
        <div className="card-body">
          <div className="input-group mb-4 w-50">
            <span className="input-group-text">
              <MagnifyingGlassIcon width={18} />
            </span>
            <input
              className="form-control"
              placeholder="Search role..."
              value={keyword}
              onChange={(e) => setKeyword(e.target.value)}
            />
            <button className="btn btn-warning" onClick={handleSearch}>
              Search
            </button>
          </div>

          <RoleTable
            roles={roles}
            loading={loading}
            onEdit={openEdit}
            onDelete={openDelete}
            onPermission={handleOpenPermissionModal}
          />
        </div>
      </div>

      <RoleModal
        show={showModal}
        mode={mode}
        roleId={selectedRole?.id}
        onClose={() => setShowModal(false)}
        onSubmit={mode === "create" ? handleCreate : handleUpdate}
      />

      <DeleteRoleModal
        show={showDelete}
        role={selectedRole}
        onClose={() => setShowDelete(false)}
        onConfirm={handleDelete}
      />

      <RolePermissionModal
        show={showPermissionModal}
        role={selectedRole}
        permissions={permissions}
        rolePermissions={rolePermissions}
        onClose={() => setShowPermissionModal(false)}
        onSave={handleSavePermissions}
      />
    </div>
  );
}