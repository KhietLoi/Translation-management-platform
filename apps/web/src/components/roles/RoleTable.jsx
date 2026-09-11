import {
  PencilSquareIcon,
  TrashIcon,
  ShieldCheckIcon,
} from "@heroicons/react/24/outline";

export default function RoleTable({
  roles,
  loading,
  onEdit,
  onDelete,
  onPermission,
}) {
  return (
    <div className="table-responsive">
      <table className="table table-hover align-middle table-bordered">
        <thead className="table-light">
          <tr className="table-warning">
            <th>Name</th>
            <th>Description</th>
            <th width="180">Action</th>
          </tr>
        </thead>

        <tbody>
          {loading ? (
            <tr>
              <td colSpan="3" className="text-center py-4">
                Loading...
              </td>
            </tr>
          ) : roles.length === 0 ? (
            <tr>
              <td colSpan="3" className="text-center py-4">
                No roles found
              </td>
            </tr>
          ) : (
            roles.map((role) => (
              <tr key={role.id}>
                <td className="fw-semibold">
                  {role.name}
                </td>

                <td>{role.description}</td>

                <td>
                  <div className="d-flex gap-2">
                    <button
                      className="btn btn-sm btn-outline-warning"
                      onClick={() => onEdit(role)}
                    >
                      <PencilSquareIcon width={16} />
                    </button>

                    <button
                      className="btn btn-sm btn-outline-primary"
                      onClick={() => onPermission(role)}
                      title="Permissions"
                    >
                      <ShieldCheckIcon width={16} />
                    </button>

                    <button
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => onDelete(role)}
                    >
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