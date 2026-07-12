import { useEffect, useState } from "react";

export default function RolePermissionModal({
  show,
  role,
  permissions = [],
  rolePermissions = [],
  onClose,
  onSave,
}) {
  const [
    selectedPermissions,
    setSelectedPermissions,
  ] = useState([]);

  useEffect(() => {
    if (show) {
      setSelectedPermissions(
        rolePermissions
      );
    }
  }, [show, rolePermissions]);

  const handleToggle = (
    permissionId
  ) => {
    setSelectedPermissions(
      (prev) =>
        prev.includes(permissionId)
          ? prev.filter(
              (x) =>
                x !== permissionId
            )
          : [
              ...prev,
              permissionId,
            ]
    );
  };

  const handleSave = () => {
    onSave({
      roleId: role.id,
      permissionIds:
        selectedPermissions,
    });
  };

  if (!show) return null;

  return (
    <>
      <div className="modal fade show d-block">
        <div className="modal-dialog modal-lg">
          <div className="modal-content">

            <div className="modal-header bg-warning">
              <h5 className="modal-title">
                Manage Permissions
              </h5>

              <button
                className="btn-close"
                onClick={onClose}
              />
            </div>

            <div className="modal-body">

              <div className="mb-3">
                <strong>Role:</strong>{" "}
                {role?.name}
              </div>

              <div className="row">
                {permissions.map(
                  (permission) => (
                    <div
                      key={permission.id}
                      className="col-md-6 mb-2"
                    >
                      <div className="form-check">

                        <input
                          className="form-check-input"
                          type="checkbox"
                          checked={selectedPermissions.includes(
                            permission.id
                          )}
                          onChange={() =>
                            handleToggle(
                              permission.id
                            )
                          }
                        />

                        <label className="form-check-label">

                          <strong>
                            {
                              permission.code
                            }
                          </strong>

                          <div className="text-muted small">
                            {
                              permission.description
                            }
                          </div>

                        </label>
                      </div>
                    </div>
                  )
                )}
              </div>

            </div>

            <div className="modal-footer">

              <button
                className="btn btn-secondary"
                onClick={onClose}
              >
                Cancel
              </button>

              <button
                className="btn btn-warning"
                onClick={handleSave}
              >
                Save Permissions
              </button>

            </div>

          </div>
        </div>
      </div>

      <div className="modal-backdrop fade show"></div>
    </>
  );
}