export default function DeletePermissionModal({
  show,
  permission,
  onClose,
  onConfirm,
}) {
  if (!show) return null;

  return (
    <div
      className="modal d-block"
      style={{
        background:
          "rgba(0,0,0,.5)",
      }}
    >
      <div className="modal-dialog modal-dialog-centered">
        <div className="modal-content">
          <div className="modal-header">
            <h5 className="modal-title">
              Delete Permission
            </h5>

            <button
              className="btn-close"
              onClick={
                onClose
              }
            />
          </div>

          <div className="modal-body">
            Are you sure you want
            to delete permission
            <strong>
              {" "}
              {
                permission?.code
              }
            </strong>
            ?
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
              className="btn btn-danger"
              onClick={
                onConfirm
              }
            >
              Delete
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}