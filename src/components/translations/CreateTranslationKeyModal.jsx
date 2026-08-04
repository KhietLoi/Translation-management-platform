import { useState } from "react";

function CreateTranslationKeyModal({
    show,
    projectId,
    namespaces,
    onClose,
    onSubmit
}) {
    const [formData, setFormData] = useState({
        namespaceId: "",
        key: "",
        description: ""
    });

    const [submitting, setSubmitting] = useState(false);

    if (!show) return null;

    const handleChange = (field, value) => {
        setFormData(prev => ({
            ...prev,
            [field]: value
        }));
    };

    const handleSubmit = async () => {
        if (!formData.namespaceId) {
            alert("Namespace is required");
            return;
        }

        if (!formData.key.trim()) {
            alert("Key is required");
            return;
        }

        try {
            setSubmitting(true);

            await onSubmit({
                projectId,
                namespaceId: formData.namespaceId,
                key: formData.key.trim(),
                description: formData.description
            });

            setFormData({
                namespaceId: "",
                key: "",
                description: ""
            });
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <>
            <div
                className="modal fade show d-block"
                style={{ backgroundColor: "rgba(0,0,0,0.4)" }}
            >
                <div className="modal-dialog modal-dialog-centered">
                    <div className="modal-content">

                        <div className="modal-header">
                            <h5 className="modal-title">
                                Create Translation Key
                            </h5>

                            <button
                                className="btn-close"
                                onClick={onClose}
                            />
                        </div>

                        <div className="modal-body">

                            <div className="mb-3">
                                <label className="form-label">
                                    Namespace
                                </label>

                                <select
                                    className="form-select"
                                    value={formData.namespaceId}
                                    onChange={(e) =>
                                        handleChange(
                                            "namespaceId",
                                            e.target.value
                                        )
                                    }
                                >
                                    <option value="">
                                        Select Namespace
                                    </option>

                                    {namespaces.map(x => (
                                        <option
                                            key={x.id}
                                            value={x.id}
                                        >
                                            {x.name}
                                        </option>
                                    ))}
                                </select>
                            </div>

                            <div className="mb-3">
                                <label className="form-label">
                                    Key
                                </label>

                                <input
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
                                    ? "Creating..."
                                    : "Create"}
                            </button>

                        </div>

                    </div>
                </div>
            </div>
        </>
    );
}

export default CreateTranslationKeyModal;