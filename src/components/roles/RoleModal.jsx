import {
  useEffect,
  useState,
} from "react";

import { roleService } from "../../services/roleService";

const defaultForm = {
  name: "",
  description: "",
};

export default function RoleModal({
  show,
  mode,
  roleId,
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
  }, [show, roleId, mode]);

  const loadData = async () => {
    try {
      if (
        mode !== "update" ||
        !roleId
      ) {
        setForm(defaultForm);
        return;
      }

      setLoading(true);

      const response =
        await roleService.getRoleById(
          roleId
        );

      const role =
        response.data.data;

      setForm({
        name: role.name,
        description:
          role.description || "",
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
            <h5>
              {mode === "create"
                ? "Create Role"
                : "Update Role"}
            </h5>

            <button
              className="btn-close"
              onClick={onClose}
            />
          </div>

          <div className="modal-body">
            {loading ? (
              <div className="text-center py-4">
                Loading...
              </div>
            ) : (
              <>
                <div className="mb-3">
                  <label className="form-label">
                    Name
                  </label>

                  <input
                    className="form-control"
                    value={form.name}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        name:
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
              onClick={onClose}
            >
              Cancel
            </button>

            <button
              className="btn btn-warning"
              onClick={
                handleSubmit
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