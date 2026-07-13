import { useEffect, useState } from "react";
import { permissionService } from "../../services/permissionService";

export default function PermissionModal({ show, mode, permissionId, onClose, onSubmit }) {
  const [loading, setLoading] = useState(false);
  const [code, setCode] = useState("");
  const [description, setDescription] = useState("");
  const [fieldErrors, setFieldErrors] = useState({});

  useEffect(() => {
    if (!show) return;
    setFieldErrors({});
    loadData();
  }, [show, permissionId, mode]);

  const loadData = async () => {
    try {
      if (mode !== "update" || !permissionId) {
        setCode("");
        setDescription("");
        return;
      }

      setLoading(true);
      const response = await permissionService.getPermissionById(permissionId);
      const permission = response?.data?.data;

      setCode(permission?.code || "");
      setDescription(permission?.description || "");
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const validateForm = () => {
    const errors = {};

    if (!code.trim()) {
      errors.code = "Code is required.";
    } else if (code.length > 200) {
      errors.code = "Code must not exceed 200 characters.";
    }

    if (description && description.length > 500) {
      errors.description = "Description must not exceed 500 characters.";
    }

    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = () => {
    if (!validateForm()) return;

    const payload = {
      code: code.trim(),
      description: description.trim(),
    };

    onSubmit(payload);
  };

  if (!show) return null;

  return (
    <div className="modal d-block" style={{ background: "rgba(0,0,0,.5)" }}>
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              {mode === "create" ? "Create Permission" : "Update Permission"}
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
                  <label className="form-label">Permission Code</label>
                  <input
                    type="text"
                    className={`form-control ${fieldErrors.code ? "is-invalid" : ""}`}
                    value={code}
                    onChange={(e) => {
                      setCode(e.target.value);
                      if (fieldErrors.code) setFieldErrors({ ...fieldErrors, code: null });
                    }}
                  />
                  {fieldErrors.code && (
                    <div className="invalid-feedback">{fieldErrors.code}</div>
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