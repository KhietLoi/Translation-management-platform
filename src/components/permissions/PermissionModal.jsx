import {
  useEffect,
  useState,
} from "react";

import { permissionService } from "../../services/permissionService";

const defaultForm = {
  code: "",
  description: "",
};

export default function PermissionModal({
  show,
  mode,
  permissionId,
  onClose,
  onSubmit,
}) {
  const [loading, setLoading] =
    useState(false);

  const [form, setForm] =
    useState(defaultForm);

  useEffect(() => {
    if (!show) return;

    loadData();
  }, [
    show,
    permissionId,
    mode,
  ]);

  const loadData = async () => {
    try {
      if (
        mode !== "update" ||
        !permissionId
      ) {
        setForm(defaultForm);
        return;
      }

      setLoading(true);

      const response =
        await permissionService.getPermissionById(
          permissionId
        );

      const permission =
        response.data.data;

      setForm({
        code:
          permission.code ||
          "",
        description:
          permission.description ||
          "",
      });
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const handleSubmit = () => {
    onSubmit(form);
  };

  if (!show) return null;

  return (
    <div
      className="modal d-block"
      style={{
        background:
          "rgba(0,0,0,.5)",
      }}
    >
      <div className="modal-dialog modal-lg modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              {mode === "create"
                ? "Create Permission"
                : "Update Permission"}
            </h5>

            <button
              type="button"
              className="btn-close"
              onClick={
                onClose
              }
            />
          </div>

          <div className="modal-body">
            {loading ? (
              <div className="text-center py-4">
                <div
                  className="spinner-border text-warning"
                  role="status"
                />
              </div>
            ) : (
              <>
                <div className="mb-3">
                  <label className="form-label">
                    Permission Code
                  </label>

                  <input
                    type="text"
                    className="form-control"
                    value={
                      form.code
                    }
                    onChange={(e) =>
                      setForm({
                        ...form,
                        code:
                          e.target
                            .value,
                      })
                    }
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">
                    Description
                  </label>

                  <textarea
                    rows="3"
                    className="form-control"
                    value={
                      form.description
                    }
                    onChange={(e) =>
                      setForm({
                        ...form,
                        description:
                          e.target
                            .value,
                      })
                    }
                  />
                </div>
              </>
            )}
          </div>

          <div className="modal-footer">
            <button
              className="btn btn-secondary"
              onClick={
                onClose
              }
            >
              Cancel
            </button>

            <button
              className="btn btn-warning"
              onClick={
                handleSubmit
              }
              disabled={
                loading
              }
            >
              {mode === "create"
                ? "Create"
                : "Update"}
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}