import { useEffect, useState } from "react";
import {
  MagnifyingGlassIcon,
  PlusIcon,
} from "@heroicons/react/24/outline";
import { toast } from "react-toastify";

import PermissionTable from "../../components/permissions/PermissionTable";
import PermissionModal from "../../components/permissions/PermissionModal";
import DeletePermissionModal from "../../components/permissions/DeletePermissionModal";

import { permissionService } from "../../services/permissionService";

export default function Permissions() {
  const [permissions, setPermissions] =
    useState([]);

  const [keyword, setKeyword] =
    useState("");

  const [loading, setLoading] =
    useState(false);

  const [page, setPage] =
    useState(1);

  const [paging, setPaging] =
    useState({
      page: 1,
      limit: 10,
      totalItem: 0,
      totalPage: 0,
    });

  const [showModal, setShowModal] =
    useState(false);

  const [showDelete, setShowDelete] =
    useState(false);

  const [mode, setMode] =
    useState("create");

  const [
    selectedPermission,
    setSelectedPermission,
  ] = useState(null);

  const loadPermissions =
    async (
      currentPage = page,
      search = keyword
    ) => {
      try {
        setLoading(true);

        const response =
          await permissionService.getPermissions(
            currentPage,
            10,
            search
          );

        const data =
          response.data.data;

        setPermissions(
          data.permissions || []
        );

        setPaging(
          data.paging
        );
      } catch (error) {
        toast.error(
          "Failed to load permissions"
        );
      } finally {
        setLoading(false);
      }
    };

  useEffect(() => {
    loadPermissions();
  }, [page]);

  const handleSearch = () => {
    setPage(1);
    loadPermissions(
      1,
      keyword
    );
  };

  const openCreate = () => {
    setMode("create");
    setSelectedPermission(
      null
    );
    setShowModal(true);
  };

  const handleCreate =
    async (form) => {
      try {
        await permissionService.createPermission(
          form
        );

        toast.success(
          "Permission created successfully"
        );

        setShowModal(false);

        await loadPermissions();
      } catch (error) {
        toast.error(
          error?.response?.data
            ?.errorMessage ||
          "Create failed"
        );
      }
    };

  const openEdit = (
    permission
  ) => {
    setMode("update");
    setSelectedPermission(
      permission
    );
    setShowModal(true);
  };

  const handleUpdate =
    async (form) => {
      try {
        await permissionService.updatePermission(
          selectedPermission.id,
          form
        );

        toast.success(
          "Permission updated successfully"
        );

        setShowModal(false);

        await loadPermissions();
      } catch (error) {
        toast.error(
          error?.response?.data
            ?.errorMessage ||
          "Update failed"
        );
      }
    };

  const openDelete = (
    permission
  ) => {
    setSelectedPermission(
      permission
    );
    setShowDelete(true);
  };

  const handleDelete =
    async () => {
      try {
        await permissionService.deletePermission(
          selectedPermission.id
        );

        toast.success(
          "Permission deleted successfully"
        );

        setShowDelete(false);

        await loadPermissions();
      } catch (error) {
        toast.error(
          error?.response?.data
            ?.errorMessage ||
          "Delete failed"
        );
      }
    };

  const getPageNumbers = (current, total) => {
    if (total <= 7) {
      return Array.from({ length: total }, (_, i) => i + 1);
    }
    if (current <= 4) {
      return [1, 2, 3, 4, 5, "...", total];
    }
    if (current >= total - 3) {
      return [1, "...", total - 4, total - 3, total - 2, total - 1, total];
    }
    return [1, "...", current - 1, current, current + 1, "...", total];
  };

  return (
    <div className="bg-white p-4 min-vh-100">
      {/* HEADER */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="fw-bold">Permissions</h3>
          <p className="text-muted">Manage system permissions</p>
        </div>

        <button
          className="btn btn-warning"
          onClick={openCreate}
        >
          <PlusIcon width={18} /> Add Permission
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
              placeholder="Search permission..."
              value={keyword}
              onChange={(e) =>
                setKeyword(
                  e.target.value
                )
              }
            />
            <button
              className="btn btn-warning"
              onClick={handleSearch}
            >
              Search
            </button>
          </div>

          <PermissionTable
            permissions={permissions}
            loading={loading}
            onEdit={openEdit}
            onDelete={
              openDelete
            }
          />

          <div className="d-flex justify-content-between align-items-center mt-3">
            <small className="text-muted">
              Total Permissions:{" "}
              {
                paging.totalItem
              }
            </small>

            <ul className="pagination pagination-sm mb-0">
              <li
                className={`page-item ${page <= 1 ? "disabled" : ""}`}
              >
                <button
                  className="page-link"
                  onClick={() => setPage(page - 1)}
                >
                  Previous
                </button>
              </li>

              {getPageNumbers(page, paging.totalPage || 1).map((p, idx) =>
                p === "..." ? (
                  <li key={`ellipsis-${idx}`} className="page-item disabled">
                    <span className="page-link">...</span>
                  </li>
                ) : (
                  <li key={p} className={`page-item ${p === page ? "active" : ""}`}>
                    <button className="page-link" onClick={() => setPage(p)}>
                      {p}
                    </button>
                  </li>
                )
              )}

              <li
                className={`page-item ${page >= (paging.totalPage || 1) ? "disabled" : ""}`}
              >
                <button
                  className="page-link"
                  onClick={() => setPage(page + 1)}
                >
                  Next
                </button>
              </li>
            </ul>
          </div>
        </div>
      </div>

      <PermissionModal
        show={showModal}
        mode={mode}
        permissionId={
          selectedPermission?.id
        }
        onClose={() =>
          setShowModal(false)
        }
        onSubmit={
          mode === "create"
            ? handleCreate
            : handleUpdate
        }
      />

      <DeletePermissionModal
        show={showDelete}
        permission={
          selectedPermission
        }
        onClose={() =>
          setShowDelete(false)
        }
        onConfirm={
          handleDelete
        }
      />
    </div>
  );
}