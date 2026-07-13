import { useEffect, useState } from "react";
import { roleService } from "../../services/roleService";

export default function RoleModal({ show, mode, roleId, onClose, onSubmit }) {
  const [loading, setLoading] = useState(false);
  const [name, setName] = useState("");
  const [description, setDescription] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;
    setFieldErrors({});
    loadData();
  }, [show, roleId, mode]);

  const loadData = async () => {
    try {
      if (mode !== "update" || !roleId) {
        setName("");
        setDescription("");
        return;
      }

      setLoading(true);
      const response = await roleService.getRoleById(roleId);
      const role = response?.data?.data;

      setName(role?.name || "");
      setDescription(role?.description || "");
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const validateForm = () => {
    const errors = {};

    if (!name.trim()) {
      errors.name = "Please provide a name.";
    } else if (name.length > 200) {
      errors.name = "Name must not exceed 200 characters.";
    }

    if (description && description.length > 500) {
      errors.description = "Description must not exceed 500 characters.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) return;

    onSubmit({
      name: name.trim(),
      description: description.trim(),
    });
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: "rgba(0,0,0,.5)" }}>
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              {mode === "create" ? "Create Role" : "Update Role"}
            </h5>
            <button type="button" className="btn-close" onClick={onClose} />
          </div>

          <div className="modal-body">
            {loading ? (
              <div className="text-center py-4">
                <div className="spinner-border text-warning" role="status" />
              </div>
            ) : (
              <>
                <div className="mb-3">
                  <label className="form-label">Name</label>
                  <input
                    type="text"
                    className={`form-control ${fieldErrors.name ? "is-invalid" : ""}`}
                    value={name}
                    onChange={(e) => {
                      setName(e.target.value);
                      if (fieldErrors.name) setFieldErrors({ ...fieldErrors, name: null });
                    }}
                  />
                  {fieldErrors.name && (
                    <div className="invalid-feedback">{fieldErrors.name}</div>
                  )}
                </div>

                <div className="mb-3">
                  <label className="form-label">Description</label>
                  <textarea
                    rows="3"
                    className={`form-control ${fieldErrors.description ? "is-invalid" : ""}`}
                    value={description}
                    onChange={(e) => {
                      setDescription(e.target.value);
                      if (fieldErrors.description) setFieldErrors({ ...fieldErrors, description: null });
                    }}
                  />
                  {fieldErrors.description && (
                    <div className="invalid-feedback">{fieldErrors.description}</div>
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