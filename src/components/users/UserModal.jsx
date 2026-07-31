import { useEffect, useState } from "react";
import { userService } from "../../services/userService";
import { roleService } from "../../services/roleService";
import Select from "react-select";

export default function UserModal({ show, mode, userId, onClose, onSubmit }) {
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState([]);

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  //const [isActive, setIsActive] = useState(true);
  const [roleIds, setRoleIds] = useState([]);

  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;
    loadData();
    setFieldErrors({});
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [show, userId, mode]);

  const loadData = async () => {
    try {
      setLoading(true);

      const roleResponse = await roleService.getRoles();
      setRoles(roleResponse?.data?.data?.roles || []);

      if (mode === "update" && userId) {
        const userResponse = await userService.getUserById(userId);
        const user = userResponse?.data?.data;

        setUsername(user?.username || "");
        setEmail(user?.email || "");
        // Handle role mapping:
        // Cast all IDs to strings to support both data structures:
        // user.roles can be an array of objects [{ roleId: 1 }] or an array of IDs [1, 2]
        const existingRoleIds = user?.roles?.map((role) =>
          typeof role === "object" ? String(role.roleId) : String(role)
        ) || [];

        setRoleIds(existingRoleIds);
      } else {
        setUsername("");
        setEmail("");
        setRoleIds([]);
      }
    } catch (error) {
      console.error("Failed to load user data:", error);
    } finally {
      setLoading(false);
    }
  };

  const validateForm = () => {
    const newErrors = {};

    if (!username.trim()) {
      newErrors.username = "Username is required.";
    } else if (username.length > 100) {
      newErrors.username = "Username cannot exceed 100 characters.";
    }

    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else {
      const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
      if (!emailRegex.test(email)) {
        newErrors.email = "Email is invalid.";
      }
    }



    if (!roleIds || roleIds.length === 0) {
      newErrors.roleIds = "At least one role is required.";
    }

    setFieldErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) return;

    const payload = {
      username: username.trim(),
      email: email.trim(),
      roleIds: roleIds, // Payload uses the array of selected IDs
    };

    onSubmit(payload);
  };

  // Prepare options for react-select
  // Cast value to String to ensure mapping and filtering are always 100% accurate
  const roleOptions = roles.map((role) => ({
    value: String(role.id),
    label: role.description ? `${role.name} - ${role.description}` : role.name,
    role: role,
  }));

  const selectedRoles = roleOptions.filter((option) =>
    roleIds.map(String).includes(option.value)
  );

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: "rgba(0,0,0,.5)" }}>
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header bg-warning">
            <h5 className="modal-title text-text-muted">
              {mode === "create" ? "Create User" : "Update User"}
            </h5>
            <button type="button" className="btn-close" onClick={onClose} />
          </div>

          <div className="modal-body">
            {loading ? (
              <div className="text-center py-4">
                <div className="spinner-border text-warning" role="status">
                  <span className="visually-hidden">Loading...</span>
                </div>
              </div>
            ) : (
              <>
                <div className="mb-3">
                  <label className="form-label">Username</label>
                  <input
                    type="text"
                    placeholder={mode === "create" ? "Enter Username" : ""}
                    className={`form-control ${fieldErrors.username ? "is-invalid" : ""}`}
                    value={username}
                    onChange={(e) => {
                      setUsername(e.target.value);
                      if (fieldErrors.username) setFieldErrors({ ...fieldErrors, username: null });
                    }}
                  />
                  {fieldErrors.username && (
                    <div className="invalid-feedback">{fieldErrors.username}</div>
                  )}
                </div>

                <div className="mb-3">
                  <label className="form-label">Email</label>
                  <input
                    type="email"
                    placeholder={mode === "create" ? "example@gmail.com" : ""}
                    className={`form-control ${fieldErrors.email ? "is-invalid" : ""}`}
                    value={email}
                    onChange={(e) => {
                      setEmail(e.target.value);
                      if (fieldErrors.email) setFieldErrors({ ...fieldErrors, email: null });
                    }}
                  />
                  {fieldErrors.email && (
                    <div className="invalid-feedback">{fieldErrors.email}</div>
                  )}
                </div>

                <div className="mb-3">
                  <label className="form-label">Roles</label>
                  <Select
                    isMulti
                    options={roleOptions}
                    value={selectedRoles}
                    onChange={(selected) => {
                      const selectedIds = selected?.map((x) => x.value) || [];
                      setRoleIds(selectedIds);

                      if (fieldErrors.roleIds) {
                        setFieldErrors({ ...fieldErrors, roleIds: null });
                      }
                    }}
                    placeholder="Select roles..."
                    className={fieldErrors.roleIds ? "border border-danger rounded" : ""}
                  />
                  {fieldErrors.roleIds && (
                    <div className="text-danger small mt-1">{fieldErrors.roleIds}</div>
                  )}
                </div>


              </>
            )}
          </div>

          <div className="modal-footer">
            <button
              type="button"
              className="btn btn-secondary"
              disabled={loading}
              onClick={onClose}
            >
              Cancel
            </button>
            <button
              type="button"
              className="btn btn-warning"
              disabled={loading}
              onClick={handleSubmit}
            >
              {mode === "create" ? "Create" : "Update"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}