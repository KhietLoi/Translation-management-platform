import { useEffect, useState } from "react";
import Select from "react-select"; // Removed 'components' as it's no longer needed

export default function RolePermissionModal({
  show,
  role,
  permissions = [],
  rolePermissions = [],
  onClose,
  onSave
}) {
  const [selectedPermissions, setSelectedPermissions] = useState([]);
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;

    const selected = permissions.filter((p) => rolePermissions.includes(p.id));
    setSelectedPermissions(selected);
    setFieldErrors({});
  }, [show, rolePermissions, permissions]);

  const validateForm = () => {
    const errors = {};
    if (!role?.id) errors.roleId = "Role is required.";
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

  const handleBackdropClick = (e) => {
    if (e.target.id === "modal-backdrop") onClose();
  };

  if (!show) return null;

  const options = permissions.map((p) => ({
    value: p.id,
    label: p.code,
    permission: p,
  }));

  const customSelectStyles = {
    control: (provided, state) => ({
      ...provided,
      borderColor: fieldErrors.permissionIds
        ? "#dc3545"
        : state.isFocused ? "#86b7fe" : "#dee2e6",
      boxShadow: state.isFocused
        ? (fieldErrors.permissionIds ? "0 0 0 0.25rem rgba(220, 53, 69, 0.25)" : "0 0 0 0.25rem rgba(13, 110, 253, 0.25)")
        : "none",
      "&:hover": {
        borderColor: fieldErrors.permissionIds ? "#dc3545" : "#b3d4fc"
      },
      padding: "2px"
    }),
    menu: (provided) => ({
      ...provided,
      borderRadius: "8px",
      boxShadow: "0 4px 12px rgba(0,0,0,0.1)",
    }),
    option: (provided, state) => ({
      ...provided,
      cursor: "pointer",
      // MAKE TEXT BOLD IF SELECTED
      fontWeight: state.isSelected ? "bold" : "normal",
      // Optional: Give it a slight blue tint when selected to make it stand out more
      color: state.isSelected ? "#0d6efd" : "#212529",
      backgroundColor: state.isFocused ? "#f8f9fa" : "transparent",
      "&:active": {
        backgroundColor: "#e9ecef"
      }
    })
  };

  return (
    <div
      id="modal-backdrop"
      className="modal fade show d-block"
      tabIndex="-1"
      role="dialog"
      aria-modal="true"
      style={{ background: "rgba(0, 0, 0, 0.6)", backdropFilter: "blur(2px)" }}
      onClick={handleBackdropClick}
    >
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content border-0 shadow-lg" style={{ borderRadius: "12px" }}>

          <div className="modal-header border-bottom-0 pb-0 pt-4 px-4">
            <h5 className="modal-title d-flex align-items-center fw-bold text-dark">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" className="text-warning me-2" viewBox="0 0 16 16">
                <path d="M5.338 1.59a61.44 61.44 0 0 0-2.837.856.481.481 0 0 0-.328.39c-.554 4.157.726 7.19 2.253 9.188a10.725 10.725 0 0 0 2.287 2.233c.346.244.652.42.893.533.12.057.218.095.293.118a.55.55 0 0 0 .101.025.615.615 0 0 0 .1-.025c.076-.023.174-.061.294-.118.24-.113.547-.29.893-.533a10.726 10.726 0 0 0 2.287-2.233c1.527-1.997 2.807-5.031 2.253-9.188a.48.48 0 0 0-.328-.39c-.651-.213-1.75-.56-2.837-.855C9.552 1.29 8.531 1.067 8 1.067c-.53 0-1.552.223-2.662.524zM5.072.56C6.157.265 7.31 0 8 0s1.843.265 2.928.56c1.11.3 2.229.655 2.887.87a1.54 1.54 0 0 1 1.044 1.262c.596 4.477-.787 7.795-2.465 9.99a11.775 11.775 0 0 1-2.517 2.453 7.159 7.159 0 0 1-1.048.625c-.28.132-.581.24-.829.24s-.548-.108-.829-.24a7.158 7.158 0 0 1-1.048-.625 11.777 11.777 0 0 1-2.517-2.453C1.928 10.487.545 7.169 1.141 2.692A1.54 1.54 0 0 1 2.185 1.43 62.456 62.456 0 0 1 5.072.56z" />
              </svg>
              Manage Permissions
            </h5>
            <button type="button" className="btn-close shadow-none" onClick={onClose} aria-label="Close" />
          </div>

          <div className="modal-body px-4 py-4">

            <div className="mb-4">
              <label className="form-label fw-semibold text-secondary mb-1">Role</label>
              <div className="input-group">
                <span className="input-group-text bg-light text-muted border-end-0">
                  <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" viewBox="0 0 16 16">
                    <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1H3zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6z" />
                  </svg>
                </span>
                <input type="text" className="form-control bg-light text-muted border-start-0" value={role?.name || "Unknown role"} readOnly />
              </div>
            </div>

            <div className="mb-2">
              <label className="form-label fw-semibold text-secondary mb-1">
                Permissions
              </label>
              <Select
                isMulti
                closeMenuOnSelect={false}
                hideSelectedOptions={false}
                options={options}
                styles={customSelectStyles}
                value={options.filter((option) =>
                  selectedPermissions.some((x) => x.id === option.value)
                )}
                onChange={(selected) => {
                  const values = selected?.map((x) => x.permission) || [];
                  setSelectedPermissions(values);

                  if (fieldErrors.permissionIds) {
                    setFieldErrors((prev) => ({ ...prev, permissionIds: null }));
                  }
                }}
                placeholder="Search and select permissions..."
                noOptionsMessage={() => "No permissions found"}
              />
              {fieldErrors.permissionIds && (
                <div className="text-danger small mt-1 d-flex align-items-center">
                  <span className="me-1">⚠️</span> {fieldErrors.permissionIds}
                </div>
              )}
            </div>

          </div>

          <div className="modal-footer border-top-0 px-4 pb-4 pt-0">
            <button type="button" className="btn btn-light px-4 text-secondary" onClick={onClose}>Cancel</button>
            <button type="button" className="btn btn-warning px-4 fw-medium text-dark shadow-sm" onClick={handleSave}>Save Changes</button>
          </div>

        </div>
      </div>
    </div>
  );
}