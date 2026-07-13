import { useEffect, useState } from "react";
import { userService } from "../../services/userService";
import { roleService } from "../../services/roleService";

export default function UserModal({ show, mode, userId, onClose, onSubmit }) {
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState([]);

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [isActive, setIsActive] = useState(true);
  const [roleIds, setRoleIds] = useState([]);

  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;
    loadData();
    setFieldErrors({});
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
        setPassword("");
        setIsActive(user?.isActive ?? true);
        setRoleIds(user?.roles?.map((role) => role.id) || []);
      } else {
        setUsername("");
        setEmail("");
        setPassword("");
        setIsActive(true);
        setRoleIds([]);
      }
    } catch (error) {
      console.error(error);
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

    if (mode === "create") {
      if (!password.trim()) {
        newErrors.password = "Password is required.";
      } else if (password.length < 6) {
        newErrors.password = "Password must be at least 6 characters.";
      }
    }

    if (!roleIds || roleIds.length === 0) {
      newErrors.roleIds = "At least one role is required.";
    }

    setFieldErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleRoleChange = (roleId, isChecked) => {
    if (isChecked) {
      setRoleIds((prev) => [...prev, roleId]);
    } else {
      setRoleIds((prev) => prev.filter((id) => id !== roleId));
    }
    setFieldErrors((prev) => ({ ...prev, roleIds: null }));
  };

  const handleSubmit = () => {
    if (!validateForm()) return;

    const payload = {
      username: username.trim(),
      email: email.trim(),
      isActive: isActive,
      roleIds: roleIds,
    };

    if (mode === "create") {
      payload.password = password;
    }

    onSubmit(payload);
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: "rgba(0,0,0,.5)" }}>
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
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

                {mode === "create" && (
                  <div className="mb-3">
                    <label className="form-label">Password</label>
                    <input
                      type="password"
                      className={`form-control ${fieldErrors.password ? "is-invalid" : ""}`}
                      value={password}
                      onChange={(e) => {
                        setPassword(e.target.value);
                        if (fieldErrors.password) setFieldErrors({ ...fieldErrors, password: null });
                      }}
                    />
                    {fieldErrors.password && (
                      <div className="invalid-feedback">{fieldErrors.password}</div>
                    )}
                  </div>
                )}

                <div className="mb-3">
                  <label className="form-label">Roles</label>
                  <div className={`border rounded p-3 ${fieldErrors.roleIds ? "border-danger" : ""}`}>
                    {roles.length > 0 ? (
                      roles.map((role) => (
                        <div key={role.id} className="form-check mb-2">
                          <input
                            type="checkbox"
                            className="form-check-input"
                            id={`role-${role.id}`}
                            checked={roleIds.includes(role.id)}
                            onChange={(e) => handleRoleChange(role.id, e.target.checked)}
                          />
                          <label htmlFor={`role-${role.id}`} className="form-check-label">
                            <strong>{role.name}</strong>
                            {role.description && (
                              <div className="text-muted small">{role.description}</div>
                            )}
                          </label>
                        </div>
                      ))
                    ) : (
                      <div className="text-muted">No roles available</div>
                    )}
                  </div>
                  {fieldErrors.roleIds && (
                    <div className="text-danger small mt-2">{fieldErrors.roleIds}</div>
                  )}
                </div>

                <div className="form-check">
                  <input
                    type="checkbox"
                    className="form-check-input"
                    id="isActive"
                    checked={isActive}
                    onChange={(e) => setIsActive(e.target.checked)}
                  />
                  <label htmlFor="isActive" className="form-check-label">
                    Active
                  </label>
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