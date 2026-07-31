import { PencilSquareIcon, TrashIcon } from "@heroicons/react/24/outline";
export default function UserTable({ users, loading, onEdit, onDelete }) {
  return (
    <div className="table-responsive">
      <table className="table table-hover align-middle table-bordered">
        <thead className="table-light">
          <tr className="table-warning">
            <th>Username</th>
            <th>Email</th>
            <th>Status</th>
            <th>Created Date</th>
            <th width="120">Action</th>
          </tr>
        </thead>

        <tbody>
          {loading ? (
            <tr>
              <td
                colSpan="5"
                className="text-center py-4"
              >
                Loading...
              </td>
            </tr>
          ) : users.length === 0 ? (
            <tr>
              <td
                colSpan="5"
                className="text-center py-4"
              >
                No users found
              </td>
            </tr>
          ) : (
            users.map((user) => (
              <tr key={user.id}>
                <td className="fw-semibold">
                  {user.username}
                </td>

                <td>{user.email}</td>

                <td>
                  <span
                    className={`badge ${user.status === 1
                      ? "bg-success"
                      : user.status === 2
                        ? "bg-danger"
                        : "bg-secondary"
                      }`}
                  >
                    {user.status === 1
                      ? "Active"
                      : user.status === 2
                        ? "Blocked"
                        : "Non Active"}
                  </span>
                </td>

                <td>
                  {new Date(
                    user.createdAt
                  ).toLocaleDateString("vi-VN")}
                </td>

                <td>
                  <div className="d-flex gap-2">
                    <button className="btn btn-sm btn-outline-warning" onClick={() => onEdit(user)}>
                      <PencilSquareIcon width={16} />
                    </button>

                    <button className="btn btn-sm btn-outline-danger" onClick={() => onDelete(user)}>
                      <TrashIcon width={16} />
                    </button>
                  </div>
                </td>

              </tr>
            ))
          )}
        </tbody>
      </table>
    </div>
  );
}