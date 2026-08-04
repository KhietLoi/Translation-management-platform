import { useEffect, useState } from "react";

function UpdateTranslationKeyModal({
    show,
    translationKey,
    onClose,
    onSubmit
}) {
    const [formData, setFormData] = useState({
        key: "",
        description: ""
    });

    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        if (!translationKey) return;

        setFormData({
            key: translationKey.key || "",
            description: translationKey.description || ""
        });
    }, [translationKey]);

    if (!show || !translationKey) return null;

    const handleChange = (field, value) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const handleSubmit = async () => {
        if (!formData.key.trim()) {
            alert("Key is required");
            return;
        }

        try {
            setSubmitting(true);

            await onSubmit({
                key: formData.key.trim(),
                description: formData.description
            });
        } finally {
            setSubmitting(false);
        }
    };

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
                        <h5 className="modal-title">
                            Update Translation Key
                        </h5>

                        <button
                            className="btn-close"
                            onClick={onClose}
                        />
                    </div>

                    <div className="modal-body">

                        <div className="mb-3">
                            <label className="form-label">
                                Key
                            </label>

                            <input
                                type="text"
                                className="form-control"
                                value={formData.key}
                                onChange={(e) =>
                                    handleChange(
                                        "key",
                                        e.target.value
                                    )
                                }
                            />
                        </div>

                        <div>
                            <label className="form-label">
                                Description
                            </label>

                            <textarea
                                rows="3"
                                className="form-control"
                                value={formData.description}
                                onChange={(e) =>
                                    handleChange(
                                        "description",
                                        e.target.value
                                    )
                                }
                            />
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
                            className="btn btn-primary"
                            disabled={submitting}
                            onClick={handleSubmit}
                        >
                            {submitting
                                ? "Updating..."
                                : "Update"}
                        </button>

                    </div>

                </div>
            </div>
        </div>
    );
}

export default UpdateTranslationKeyModal;