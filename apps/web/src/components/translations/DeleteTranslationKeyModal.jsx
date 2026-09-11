function DeleteTranslationKeyModal({
    show,
    translationKey,
    onClose,
    onDelete
}) {
    if (!show || !translationKey) {
        return null;
    }

    return (
        <div
            className="modal fade show d-block"
            style={{
                backgroundColor: "rgba(0,0,0,0.4)"
            }}
        >
            <div className="modal-dialog modal-dialog-centered">
                <div className="modal-content">

                    <div className="modal-header">
                        <h5 className="modal-title text-danger">
                            Delete Translation Key
                        </h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        />
                    </div>

                    <div className="modal-body">

                        <p>
                            Are you sure you want to delete this translation key?
                        </p>

                        <div className="border rounded p-3 bg-light">
                            <div className="fw-semibold">
                                {translationKey.key}
                            </div>

                            <small className="text-muted">
                                {translationKey.namespaceName}
                            </small>
                        </div>

                        <div className="text-danger mt-3">
                            This action cannot be undone.
                        </div>

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
                            onClick={() =>
                                onDelete(
                                    translationKey.translationKeyId
                                )
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

export default DeleteTranslationKeyModal;