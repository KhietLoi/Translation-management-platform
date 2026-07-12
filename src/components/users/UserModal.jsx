import { useEffect, useState } from "react";
import { userService } from "../../services/userService";
import { roleService } from "../../services/roleService";


const defaultForm = {
  username: "",
  email: "",
  password: "", //For create user
  isActive: true,
  roleIds: [],
};

export default function UserModal({
  show,
  mode,
  userId,
  onClose,
  onSubmit,
}) {
  const [loading, setLoading] = useState(false);
  const [roles, setRoles] = useState([]);
  const [form, setForm] = useState(defaultForm);

  useEffect(() => {
    if (!show) return;
    loadData();
  }, [show, userId, mode]);
  console.log("===== MODAL DEBUG =====");
    console.log("show:", show);
    console.log("mode:", mode);
    console.log("userId:", userId);
    console.log("=======================");

  const loadData = async () => {
    try {
      setLoading(true);
    console.log("Loading user:", userId);
      const roleResponse =
        await roleService.getRoles();
    console.log("Roles response:", roleResponse);
    console.log("Roles data:", roleResponse?.data?.data?.roles)

      setRoles(
        roleResponse?.data?.data?.roles || []
      );
      console.log("Mode");
      //For update
      if (mode === "update" && userId) {
        const userResponse =
          await userService.getUserById(
            userId
          );
        console.log("User response:", userResponse);

        const user =
          userResponse?.data?.data;
        console.log("========== USER ==========");
            console.log(userResponse);
            console.log(userResponse.data);
            console.log(userResponse.data.data);
            console.log("==========================");
        setForm({
          username:
            user?.username || "",
          email:
            user?.email || "",
          isActive:
            user?.isActive ?? true,
          roleIds:
            user?.roles?.map(
              (role) => role.roleId
            ) || [],
        });
      } else {
        setForm(defaultForm);
      }
    } catch (error) {
      console.error(
        "Failed to load modal data:",
        error
      );
    } finally {
      setLoading(false);
    }
  };

  const handleRoleChange = (
    roleId,
    checked
  ) => {
    setForm((prev) => ({
      ...prev,
      roleIds: checked
        ? [...prev.roleIds, roleId]
        : prev.roleIds.filter(
            (id) => id !== roleId
          ),
    }));
  };

  const handleSubmit = () => {

  const payload = {
    username: form.username,
    email: form.email,
    isActive: form.isActive,
    roleIds: form.roleIds,
  };

  if (mode === "create") {
    payload.password = form.password;
  }

  onSubmit(payload);
};

  if (!show) return null;
console.log("FORM:", form);
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
                ? "Create User"
                : "Update User"}
            </h5>

            <button
              type="button"
              className="btn-close"
              onClick={onClose}
            />
          </div>

          <div className="modal-body">
            {loading ? (
              <div className="text-center py-4">
                <div
                  className="spinner-border text-warning"
                  role="status"
                >
                  <span className="visually-hidden">
                    Loading...
                  </span>
                </div>
              </div>
            ) : (
              <>
                <div className="mb-3">
                  <label className="form-label">
                    Username
                  </label>

                  <input
                    type="text"
                    className="form-control"
                    value={
                      form.username
                    }
                    onChange={(e) =>
                      setForm({
                        ...form,
                        username:
                          e.target
                            .value,
                      })
                    }
                  />
                </div>

                <div className="mb-3">
                  <label className="form-label">
                    Email
                  </label>

                  <input
                    type="email"
                    className="form-control"
                    value={
                      form.email
                    }
                    onChange={(e) =>
                      setForm({
                        ...form,
                        email:
                          e.target
                            .value,
                      })
                    }
                  />

                </div>
                {mode === "create" &&(
                  <div className="mb-3">
                      <label className="form-label">
                        Password
                      </label>

                      <input
                      type="password"
                      className="form-control"
                      value={form.password}
                      onChange={(e) =>
                          setForm({
                          ...form,
                          password: e.target.value,
                          })
                      }
                      />
                  </div>
                )}


                <div className="mb-3">
                  <label className="form-label">
                    Roles
                  </label>

                  <div className="border rounded p-3">
                    {roles.length >
                    0 ? (
                      roles.map(
                        (
                          role
                        ) => (
                          <div
                            key={
                              role.id
                            }
                            className="form-check mb-2"
                          >
                            <input
                              type="checkbox"
                              className="form-check-input"
                              id={`role-${role.id}`}
                              checked={form.roleIds.includes(
                                role.id
                              )}
                              onChange={(
                                e
                              ) =>
                                handleRoleChange(
                                  role.id,
                                  e
                                    .target
                                    .checked
                                )
                              }
                            />

                            <label
                              htmlFor={`role-${role.id}`}
                              className="form-check-label"
                            >
                              <strong>
                                {
                                  role.name
                                }
                              </strong>

                              {role.description && (
                                <div className="text-muted small">
                                  {
                                    role.description
                                  }
                                </div>
                              )}
                            </label>
                          </div>
                        )
                      )
                    ) : (
                      <div className="text-muted">
                        No roles
                        available
                      </div>
                    )}
                  </div>
                </div>

                <div className="form-check">
                  <input
                    type="checkbox"
                    className="form-check-input"
                    id="isActive"
                    checked={
                      form.isActive
                    }
                    onChange={(e) =>
                      setForm({
                        ...form,
                        isActive:
                          e.target
                            .checked,
                      })
                    }
                  />

                  <label
                    htmlFor="isActive"
                    className="form-check-label"
                  >
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
              disabled={
                loading
              }
              onClick={onClose}
            >
              Cancel
            </button>

            <button
              type="button"
              className="btn btn-warning"
              disabled={
                loading
              }
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