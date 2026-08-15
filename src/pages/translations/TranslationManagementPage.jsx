import { useEffect, useState } from "react";
import { useOutletContext } from "react-router-dom";
import { getProjects, getProjectNamespaces } from "../../services/projectService";
import { getTranslationGrid, createTranslationKey, updateTranslationKey, deleteTranslationKey, getTranslationValueById } from "../../services/translationManagementService";

import TranslationFilter from "../../components/translations/TranslationFilter";
import TranslationGrid from "../../components/translations/TranslationGrid";
import TranslationPagination from "../../components/translations/TranslationPagination";
import CreateTranslationKeyModal from "../../components/translations/CreateTranslationKeyModal";
import UpdateTranslationKeyModal from "../../components/translations/UpdateTranslationKeyModal";
import DeleteTranslationKeyModal from "../../components/translations/DeleteTranslationKeyModal";
import TranslationDetailDrawer from "../../components/translations/TranslationDetailDrawer";
import TranslationReviewTab from "../../components/translations/TranslationReviewTab";
import { toast } from "react-toastify";

function TranslationManagementPage() {
    const outletContext = useOutletContext() || {};
    const {
        projects: contextProjects = [],
        selectedProjectId: contextProjectId = "",
        setSelectedProjectId: setContextProjectId,
    } = outletContext;

    const [loading, setLoading] = useState(false);
    const [localProjects, setLocalProjects] = useState([]);
    const [namespaces, setNamespaces] = useState([]);
    const [selectedProjectId, setSelectedProjectId] = useState(contextProjectId || "");

    const projects = contextProjects.length > 0 ? contextProjects : localProjects;
    const [selectedNamespaceId, setSelectedNamespaceId] = useState("");
    const [keyword, setKeyword] = useState("");
    const [status, setStatus] = useState("");
    const [numberOfLanguages, setNumberOfLanguages] = useState();
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(20);
    const [gridData, setGridData] = useState(null);
    const [activeTab, setActiveTab] = useState("grid"); // "grid" | "review"

    const [showCreateKeyModal, setShowCreateKeyModal] = useState(false);
    const [selectedTranslationKey, setSelectedTranslationKey] = useState(null);
    const [showUpdateModal, setShowUpdateModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);

    //Detail drawer.
    const [showDrawer, setShowDrawer] = useState(false);
    const [selectedTranslationValue, setSelectedTranslationValue] =
        useState(null);

    // Sync with contextProjectId
    useEffect(() => {
        if (contextProjectId) {
            setSelectedProjectId(contextProjectId);
        }
    }, [contextProjectId]);

    // ==========================
    // LOAD PROJECTS
    // ==========================
    const loadProjects = async () => {
        try {
            const response = await getProjects();
            const projectList = response.data?.projects || response.data || [];
            const list = Array.isArray(projectList) ? projectList : [];

            setLocalProjects(list);

            if (list.length > 0 && !selectedProjectId && !contextProjectId) {
                setSelectedProjectId(list[0].id);
            }
        } catch (error) {
            console.error(error);
        }
    };

    // ==========================
    // LOAD NAMESPACES
    // ==========================
    const loadNamespaces = async (projectId) => {
        try {
            const response = await getProjectNamespaces(projectId);
            const namespaceList = response.data?.namespaces || response.data || [];

            setNamespaces(namespaceList);
        } catch (error) {
            console.error(error);
        }
    };

    // ==========================
    // LOAD GRID
    // ==========================
    const loadGrid = async () => {
        if (!selectedProjectId) return;

        try {
            setLoading(true);
            const response = await getTranslationGrid({
                projectId: selectedProjectId,
                namespaceId: selectedNamespaceId || null,
                keyword,
                status: status === "" ? null : Number(status),
                numberOfLanguages,
                pageNumber,
                pageSize
            });

            setGridData(response.data);
        } catch (error) {
            console.error(error);
        } finally {
            setLoading(false);
        }
    };

    const handleCreateTranslationKey = async (payload) => {
        try {

            await createTranslationKey(payload);
            toast.success("Translation key created successfully");
            setShowCreateKeyModal(false);
            await loadGrid();

        } catch (error) {
            console.error(error);
            toast.error("Failed to create translation key");
        }
    };

    const handleUpdateTranslationKey = async (
        payload
    ) => {
        try {

            await updateTranslationKey(
                selectedTranslationKey.translationKeyId,
                payload
            );
            toast.success("Translation key updated successfully");

            setShowUpdateModal(false);
            setSelectedTranslationKey(null);

            await loadGrid();

        } catch (error) {
            console.error(error);
            toast.error("Failed to update translation key");
        }
    };

    const handleDeleteTranslationKey = async (
        id
    ) => {
        try {

            await deleteTranslationKey(id);

            toast.success(
                "Translation key deleted successfully."
            );

            setShowDeleteModal(false);
            setSelectedTranslationKey(null);

            await loadGrid();

        } catch (error) {

            toast.error(
                error?.response?.data?.errorMessage ||
                "Failed to delete translation key."
            );

            console.error(error);
        }
    };

    //Clilk cell:
    const handleCellClick = (data) => {

        setSelectedTranslationValue(data);

        setShowDrawer(true);
    };
    // ==========================
    // EFFECTS
    // ==========================
    useEffect(() => {
        if (contextProjects.length === 0) {
            loadProjects();
        }
    }, [contextProjects]);

    useEffect(() => {
        if (selectedProjectId) {
            loadNamespaces(selectedProjectId);
        }
    }, [selectedProjectId]);

    useEffect(() => {
        loadGrid();
    }, [
        selectedProjectId,
        selectedNamespaceId,
        numberOfLanguages,
        pageNumber
    ]);

    // Auto refresh grid on SignalR notification
    useEffect(() => {
        const handleNotification = () => {
            if (selectedProjectId) {
                loadGrid();
            }
        };
        window.addEventListener("translationNotification", handleNotification);
        return () => window.removeEventListener("translationNotification", handleNotification);
    }, [selectedProjectId]);
    // ==========================
    // DYNAMIC LANGUAGES & STATS
    // ==========================
    const languages = gridData?.items?.[0]?.values?.map(x => ({
        languageId: x.languageId,
        languageCode: x.languageCode
    })) || [];

    const totalCount = gridData?.totalCount || 0;
    const totalPages = Math.ceil(totalCount / pageSize);

    // Tìm tên Project đang chọn để hiển thị trên Header
    const currentProjectName = projects.find(p => p.id === selectedProjectId)?.name || "Project";
    // Tìm tên Namespace đang chọn
    const currentNamespaceName = namespaces.find(n => n.id === selectedNamespaceId)?.name || "All";

    return (
        <div className="container-fluid py-4 px-4" style={{ backgroundColor: "#f8f9fa", minHeight: "100vh" }}>

            {/* HEADER AREA */}
            <div className="d-flex justify-content-between align-items-center mb-4">
                <div>
                    <h2 className="fw-bold mb-1 fs-4">Translations · {currentProjectName}</h2>
                    <p className="text-muted mb-0 fs-6">
                        {activeTab === "grid"
                            ? `Namespace: ${currentNamespaceName} · ${totalCount} keys`
                            : "Batch review and approve/reject namespace translations"}
                    </p>
                </div>
                {activeTab === "grid" && (
                    <button
                        className="btn btn-dark fw-medium px-4 py-2 rounded-3"
                        onClick={() => setShowCreateKeyModal(true)}
                    >
                        + Create Key
                    </button>
                )}
            </div>

            {/* TABS FOR SWITCHING MODES */}
            <ul className="nav nav-pills mb-4 gap-2 bg-white p-2 rounded-3 border-0 shadow-sm d-inline-flex">
                <li className="nav-item">
                    <button
                        className={`nav-link px-4 py-2 fw-medium rounded-3 border-0 transition ${
                            activeTab === "grid"
                                ? "active bg-dark text-white shadow-sm"
                                : "bg-transparent text-secondary hover-text-dark"
                        }`}
                        onClick={() => setActiveTab("grid")}
                    >
                        Translations Grid
                    </button>
                </li>
                <li className="nav-item">
                    <button
                        className={`nav-link px-4 py-2 fw-medium rounded-3 border-0 transition ${
                            activeTab === "review"
                                ? "active bg-dark text-white shadow-sm"
                                : "bg-transparent text-secondary hover-text-dark"
                        }`}
                        onClick={() => setActiveTab("review")}
                    >
                        Batch Review
                    </button>
                </li>
            </ul>

            {activeTab === "grid" ? (
                <>
                    {/* FILTER COMPONENT */}
                    <TranslationFilter
                        projects={projects}
                        namespaces={namespaces}
                        selectedProjectId={selectedProjectId}
                        selectedNamespaceId={selectedNamespaceId}
                        keyword={keyword}
                        status={status}
                        numberOfLanguages={numberOfLanguages}
                        onProjectChange={(value) => {
                            setSelectedProjectId(value);
                            if (setContextProjectId) setContextProjectId(value);
                            setPageNumber(1);
                        }}
                        onNamespaceChange={(value) => {
                            setSelectedNamespaceId(value);
                            setPageNumber(1);
                        }}
                        onKeywordChange={setKeyword}
                        onStatusChange={setStatus}
                        onNumberOfLanguagesChange={setNumberOfLanguages}
                        onSearch={() => {
                            setPageNumber(1);
                            loadGrid();
                        }}
                    />

                    {/* GRID COMPONENT */}
                    <TranslationGrid
                        loading={loading}
                        gridData={gridData}
                        languages={languages}
                        onEdit={item => {
                            setSelectedTranslationKey(item);
                            setShowUpdateModal(true);
                        }}
                        onDelete={item => {
                            setSelectedTranslationKey(item);
                            setShowDeleteModal(true);
                        }}
                        onCellClick={handleCellClick}
                    />

                    {/* PAGINATION COMPONENT */}
                    <TranslationPagination
                        pageNumber={pageNumber}
                        totalPages={totalPages}
                        onPrevious={() => setPageNumber(prev => prev - 1)}
                        onNext={() => setPageNumber(prev => prev + 1)}
                    />
                </>
            ) : (
                <TranslationReviewTab
                    projectId={selectedProjectId}
                    namespaces={namespaces}
                />
            )}

            <CreateTranslationKeyModal
                show={showCreateKeyModal}
                projectId={selectedProjectId}
                namespaces={namespaces}
                onClose={() => setShowCreateKeyModal(false)}
                onSubmit={handleCreateTranslationKey}
            />
            <UpdateTranslationKeyModal
                show={showUpdateModal}
                translationKey={selectedTranslationKey}
                onClose={() => {
                    setShowUpdateModal(false);
                    setSelectedTranslationKey(null);
                }}
                onSubmit={handleUpdateTranslationKey}
            />
            <DeleteTranslationKeyModal
                show={showDeleteModal}
                translationKey={selectedTranslationKey}
                onClose={() => {
                    setShowDeleteModal(false);
                    setSelectedTranslationKey(null);
                }}
                onDelete={handleDeleteTranslationKey}
            />

            <TranslationDetailDrawer
                show={showDrawer}
                translationValue={selectedTranslationValue}
                onClose={() => {
                    setShowDrawer(false);
                    setSelectedTranslationValue(null);
                }}
                onSuccess={loadGrid}
            />
        </div>
    );
}

export default TranslationManagementPage;