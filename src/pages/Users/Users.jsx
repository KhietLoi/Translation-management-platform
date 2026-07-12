import { useEffect, useState } from "react";
import {
  MagnifyingGlassIcon,
  PlusIcon,
} from "@heroicons/react/24/outline";

import UserTable from "../../components/users/UserTable";
import UserModal from "../../components/users/UserModal";
import DeleteUserModal from "../../components/users/DeleteUserModal";

import { userService } from "../../services/userService";
import { toast } from "react-toastify";

export default function Users() {
  const [users, setUsers] = useState([]);
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

  const [mode, setMode] = useState("create");
  const [selectedUser, setSelectedUser] = useState(null);

  const loadUsers = async (
    currentPage = page,
    search = keyword
  ) => {
    try {
      setLoading(true);

      const response = await userService.getUsers(
        currentPage,
        5,
        search
      );

      const data = response.data.data;

      setUsers(data.users);
      setPaging(data.paging);
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadUsers();
  }, [page]);

  const handleSearch = () => {
    setPage(1);
    loadUsers(1, keyword);
  };

  // CREATE

  const openCreate = () => {
    setMode("create");
    setSelectedUser(null);
    setShowModal(true);
  };

  const handleCreate = async (form) => {
    try {
      await userService.createUser(form);

      toast.success("User created successfully");
      setShowModal(false);
      await loadUsers();

    } catch (error) {
      toast.error(error?.response?.data?.message ||"Create user failed");
      console.error(error);
    }
  };

  // UPDATE

  const openEdit = (user) => {
    setMode("update");
    setSelectedUser(user);
    setShowModal(true);
  };

  const handleUpdate = async (form) => {
    try {
      await userService.updateUser(
        selectedUser.id,
        form
      );
      toast.success("User updated successfully");
      setShowModal(false);

      await loadUsers();

    } catch (error) {

      toast.error(error?.response?.data?.message || "Update user failed");
      console.error(error);
    }
  };

  // DELETE

  const openDelete = (user) => {
    setSelectedUser(user);
    setShowDelete(true);
  };

  const handleDelete = async () => {
    try {
      await userService.deleteUser(
        selectedUser.id
      );
      toast.success("User deleted successfully");

      setShowDelete(false);

      await loadUsers();
    } catch (error) {

      toast.error(error?.response?.data?.message ||"Delete user failed");
      console.error(error);
      
    }
  };

  return (
    <div className="bg-white p-4 min-vh-100">
      {/* Header */}
      <div className="d-flex justify-content-between align-items-center mb-4">
        <div>
          <h3 className="fw-bold mb-1">Users</h3>

          <p className="text-muted mb-0">
            Manage system users and assigned roles
          </p>
        </div>

        <button
          className="btn btn-warning d-flex align-items-center gap-2"
          onClick={openCreate}
        >
          <PlusIcon width={18} />
          Add User
        </button>
      </div>

      {/* Main Card */}
      <div className="card border-0 shadow-sm">
        <div className="card-body">
          {/* Search */}
          <div className="row mb-4">
            <div className="col-md-5">
              <div className="input-group">
                <span className="input-group-text">
                  <MagnifyingGlassIcon width={18} />
                </span>

                <input
                  type="text"
                  className="form-control"
                  placeholder="Search username or email..."
                  value={keyword}
                  onChange={(e) =>
                    setKeyword(e.target.value)
                  }
                  onKeyDown={(e) => {
                    if (e.key === "Enter") {
                      handleSearch();
                    }
                  }}
                />

                <button
                  className="btn btn-warning"
                  onClick={handleSearch}
                >
                  Search
                </button>
              </div>
            </div>
          </div>

          {/* Table */}
          <UserTable
            users={users}
            loading={loading}
            onEdit={openEdit}
            onDelete={openDelete}
          />

          {/* Pagination */}
          <div className="d-flex justify-content-between align-items-center mt-3">
            <small className="text-muted">
              Total Users: {paging.totalItem}
            </small>

            <ul className="pagination pagination-sm mb-0">
              <li
                className={`page-item ${
                  page === 1 ? "disabled" : ""
                }`}
              >
                <button
                  className="page-link"
                  onClick={() =>
                    setPage(page - 1)
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
                  page >= paging.totalPage
                    ? "disabled"
                    : ""
                }`}
              >
                <button
                  className="page-link"
                  onClick={() =>
                    setPage(page + 1)
                  }
                >
                  Next
                </button>
              </li>
            </ul>
          </div>
        </div>
      </div>

      {/*  Update Modal */}
     <UserModal
  show={showModal}
  mode={mode}
  userId={selectedUser?.id}
  onClose={() =>
    setShowModal(false)
  }
  onSubmit={
    mode === "create"
      ? handleCreate
      : handleUpdate
  }
/>

      {/* Delete Modal */}
      <DeleteUserModal
        show={showDelete}
        user={selectedUser}
        onClose={() => setShowDelete(false)}
        onConfirm={handleDelete}
      />
    </div>
  );
}