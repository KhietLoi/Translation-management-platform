import {
  PencilSquareIcon,
  TrashIcon,
} from "@heroicons/react/24/outline";

export default function PermissionTable({
  permissions,
  loading,
  onEdit,
  onDelete,
}) {
  return (
    <div className="table-responsive">
      <table className="table table-hover align-middle table-bordered">
        <thead className="table-light">
          <tr className="table-warning">
            <th>Code</th>
            <th>Description</th>
            <th width="120">
              Action
            </th>
          </tr>
        </thead>

        <tbody>
          {loading ? (
            <tr>
              <td
                colSpan="3"
                className="text-center py-4"
              >
                <div
                  className="spinner-border text-warning"
                  role="status"
                />
              </td>
            </tr>
          ) : permissions.length === 0 ? (
            <tr>
              <td
                colSpan="3"
                className="text-center py-4 text-muted"
              >
                No permissions found
              </td>
            </tr>
          ) : (
            permissions.map(
              (permission) => (
                <tr
                  key={
                    permission.id
                  }
                >
                  <td>
                    <span className="badge bg-success text-white">
                      {
                        permission.code
                      }
                    </span>
                  </td>

                  <td>
                    {
                      permission.description
                    }
                  </td>

                  <td>
                    <div className="d-flex gap-2">
                      <button
                        className="btn btn-sm btn-outline-warning"
                        onClick={() =>
                          onEdit(
                            permission
                          )
                        }
                      >
                        <PencilSquareIcon width={16} />
                      </button>

                      <button
                        className="btn btn-sm btn-outline-danger"
                        onClick={() =>
                          onDelete(
                            permission
                          )
                        }
                      >
                        <TrashIcon width={16} />
                      </button>
                    </div>
                  </td>
                </tr>
              )
            )
          )}
        </tbody>
      </table>
    </div>
  );
}