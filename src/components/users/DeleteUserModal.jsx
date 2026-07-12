export default function DeleteUserModal({
  show,
  user,
  onClose,
  onConfirm,
}) {
  if (!show) return null;

  return (
    <div
      className="modal d-block"
      style={{
        background: "rgba(0,0,0,.5)",
      }}
    >
      <div className="modal-dialog modal-sm">
        <div className="modal-content">
          <div className="modal-header">
            <h5>Delete User</h5>
          </div>

          <div className="modal-body">
            Delete user:

            <strong>
              {" "}
              {user?.username}
            </strong>
            ?
          </div>

          <div className="modal-footer">
            <button
              className="btn btn-secondary"
              onClick={onClose}
            >
              Cancel
            </button>

            <button
              className="btn btn-danger"
              onClick={onConfirm}
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}