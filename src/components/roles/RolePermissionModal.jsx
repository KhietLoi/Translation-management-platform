import { useEffect, useState } from "react";
import Select from "react-select";

export default function RolePermissionModal({ show, role, permissions = [], rolePermissions = [], onClose, onSave }) {
  const [selectedPermissions, setSelectedPermissions] = useState([]);
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;

    // Lọc ra các quyền đã được gán cho role hiện tại
    const selected = permissions.filter((p) => rolePermissions.includes(p.id));
    setSelectedPermissions(selected);
    setFieldErrors({});
  }, [show, rolePermissions, permissions]);

  const validateForm = () => {
    const errors = {};

    if (!role?.id) {
      errors.roleId = "Role is required.";
    }

    if (!selectedPermissions || selectedPermissions.length === 0) {
      errors.permissionIds = "Please select at least one permission.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSave = () => {
    if (!validateForm()) return;

    onSave({
      roleId: role.id,
      permissionIds: selectedPermissions.map((x) => x.id),
    });
  };

  if (!show) return null;

  // Format lại data cho thư viện react-select
  const options = permissions.map((p) => ({
    value: p.id,
    label: p.code,
    permission: p,
  }));

  return (
    <div className="modal d-block" style={{ background: "rgba(0,0,0,.5)" }}>
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          
          <div className="modal-header">
            <h5 className="modal-title">Manage Permissions</h5>
            <button type="button" className="btn-close" onClick={onClose} />
          </div>

          <div className="modal-body">
            
            {/* Hiển thị Role dưới dạng input disabled cho đồng bộ form */}
            <div className="mb-3">
              <label className="form-label">Role</label>
              <input 
                type="text" 
                className="form-control text-muted" 
                value={role?.name || ""} 
                disabled 
              />
              {fieldErrors.roleId && (
                <div className="text-danger small mt-1">{fieldErrors.roleId}</div>
              )}
            </div>

            {/* Component chọn quyền đa mục (Multi Select) */}
            <div className="mb-3">
              <label className="form-label">Permissions</label>
              <Select
                isMulti
                options={options}
                value={options.filter((option) =>
                  selectedPermissions.some((x) => x.id === option.value)
                )}
                onChange={(selected) => {
                  const values = selected?.map((x) => x.permission) || [];
                  setSelectedPermissions(values);
                  
                  // Xoá lỗi khi người dùng bắt đầu chọn lại
                  if (fieldErrors.permissionIds) {
                    setFieldErrors({ ...fieldErrors, permissionIds: null });
                  }
                }}
                placeholder="Select permissions..."
                className={fieldErrors.permissionIds ? "border border-danger rounded" : ""}
              />
              {fieldErrors.permissionIds && (
                <div className="text-danger small mt-1">{fieldErrors.permissionIds}</div>
              )}
            </div>

          </div>

          <div className="modal-footer">
            <button type="button" className="btn btn-secondary" onClick={onClose}>
              Cancel
            </button>
            <button type="button" className="btn btn-warning" onClick={handleSave}>
              Save Permissions
            </button>
          </div>

        </div>
      </div>
    </div>
  );
}