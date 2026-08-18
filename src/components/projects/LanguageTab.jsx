import { useEffect, useState } from "react";

import { getLanguages } from "../../services/languageService";
import { updateProjectLanguages } from "../../services/projectService";
import { toast } from "react-toastify";
import { usePermission } from "../../hooks/usePermission";
import { PERMISSIONS } from "../../constants/permissions";

import "./LanguageTab.css";

function LanguagesTab({
    projectId,
    languages,
    onUpdated
}) {
    const { hasPermission } = usePermission();
    const canUpdateProject = hasPermission(PERMISSIONS.PROJECT.UPDATE);

    const [allLanguages, setAllLanguages] = useState([]);
    const [selectedIds, setSelectedIds] = useState([]);
    const [saving, setSaving] = useState(false);

    useEffect(() => {

        loadLanguages();

    }, []);

    useEffect(() => {

        setSelectedIds(
            languages.map(x => x.languageId)
        );

    }, [languages]);

    const loadLanguages = async () => {

        try {

            const result =
                await getLanguages();

            const items =
                (result.data.languages || [])
                    .map(x => ({
                        id: x.languageId,
                        code: x.code,
                        name: x.name
                    }));

            setAllLanguages(items);

        }
        catch (error) {

            console.error(error);

        }
    };

    const handleToggle = (id) => {
        if (!canUpdateProject) return;

        setSelectedIds(prev => {

            if (prev.includes(id)) {

                return prev.filter(
                    x => x !== id
                );
            }

            return [...prev, id];
        });
    };

    const handleSave = async () => {
        if (!canUpdateProject) return;

        try {

            setSaving(true);

            await updateProjectLanguages(
                projectId,
                selectedIds
            );

            if (onUpdated) {
                onUpdated();
            }

            toast.success(
                "Languages updated successfully"
            );

        }
        catch (error) {

            console.error(error);

            toast.error(
                error?.response?.data?.message || "Update failed"
            );
        }
        finally {

            setSaving(false);
        }
    };

    return (

        <div className="card border-0 shadow-sm">

            <div className="card-header bg-white">

                <div className="d-flex justify-content-between align-items-center">

                    <div>

                        <h5 className="mb-0">
                            Project Languages
                        </h5>

                        <small className="text-muted">

                            Select languages available in this project

                        </small>

                    </div>

                    {canUpdateProject && (
                        <button
                            className="btn btn-primary"
                            disabled={saving}
                            onClick={handleSave}
                        >
                            {
                                saving
                                    ? "Saving..."
                                    : "Save Changes"
                            }
                        </button>
                    )}

                </div>

            </div>

            <div className="card-body">

                <div className="row g-3">

                    {allLanguages.map(language => (

                        <div
                            key={language.id}
                            className="col-md-4"
                        >

                            <div
                                className={`language-card ${selectedIds.includes(language.id)
                                    ? "selected"
                                    : ""
                                    }`}
                                onClick={() =>
                                    handleToggle(
                                        language.id
                                    )
                                }
                            >

                                <div className="d-flex align-items-center">

                                    <input
                                        type="checkbox"
                                        checked={
                                            selectedIds.includes(
                                                language.id
                                            )
                                        }
                                        readOnly
                                        className="form-check-input me-3"
                                    />

                                    <div>

                                        <div className="fw-semibold">

                                            {language.name}

                                        </div>

                                        <small className="text-muted">

                                            {language.code}

                                        </small>

                                    </div>

                                </div>

                            </div>

                        </div>

                    ))}

                </div>

            </div>

        </div>

    );
}

export default LanguagesTab;