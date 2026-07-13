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
      limit: 5,
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
            5,
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

  return (
    <div className="bg-white p-4 min-vh-100">
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="fw-bold mb-1">
            Permissions
          </h3>

          <p className="text-muted mb-0">
            Manage system
            permissions
          </p>
        </div>

        <button
          className="btn btn-warning d-flex align-items-center gap-2"
          onClick={openCreate}
        >
          <PlusIcon width={18} />
          Add Permission
        </button>
      </div>

      <div className="card border-0 shadow-sm">
        <div className="card-body">
          <div className="row mb-4">
            <div className="col-md-5">
              <div className="input-group">
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
                  onKeyDown={(e) => {
                    if (
                      e.key ===
                      "Enter"
                    ) {
                      handleSearch();
                    }
                  }}
                />

                <button
                  className="btn btn-warning"
                  onClick={
                    handleSearch
                  }
                >
                  Search
                </button>
              </div>
            </div>
          </div>

          <PermissionTable
            permissions={
              permissions
            }
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
                className={`page-item ${
                  page === 1
                    ? "disabled"
                    : ""
                }`}
              >
                <button
                  className="page-link"
                  onClick={() =>
                    setPage(
                      page - 1
                    )
                  }
                >
                  Previous
                </button>
              </li>

              <li className="page-item active">
                <button className="page-link">
                  {page}
                </button>
              </li>

              <li
                className={`page-item ${
                  page >=
                  paging.totalPage
                    ? "disabled"
                    : ""
                }`}
              >
                <button
                  className="page-link"
                  onClick={() =>
                    setPage(
                      page + 1
                    )
                  }
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