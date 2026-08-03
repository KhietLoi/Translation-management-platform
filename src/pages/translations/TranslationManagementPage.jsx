import { useEffect, useState } from "react";
import { getProjects, getProjectNamespaces } from "../../services/projectService";
import { getTranslationGrid } from "../../services/translationManagementService";

import TranslationFilter from "../../components/translations/TranslationFilter";
import TranslationGrid from "../../components/translations/TranslationGrid";
import TranslationPagination from "../../components/translations/TranslationPagination";

function TranslationManagementPage() {
    const [loading, setLoading] = useState(false);
    const [projects, setProjects] = useState([]);
    const [namespaces, setNamespaces] = useState([]);

    const [selectedProjectId, setSelectedProjectId] = useState("");
    const [selectedNamespaceId, setSelectedNamespaceId] = useState("");
    const [keyword, setKeyword] = useState("");
    const [status, setStatus] = useState("");
    const [numberOfLanguages, setNumberOfLanguages] = useState();
    const [pageNumber, setPageNumber] = useState(1);
    const [pageSize] = useState(20);
    const [gridData, setGridData] = useState(null);

    // ==========================
    // LOAD PROJECTS
    // ==========================
    const loadProjects = async () => {
        try {
            const response = await getProjects();
            const projectList = response.data?.projects || [];

            setProjects(projectList);

            if (projectList.length > 0) {
                setSelectedProjectId(projectList[0].id);
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

    // ==========================
    // EFFECTS
    // ==========================
    useEffect(() => {
        loadProjects();
    }, []);

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
                        Namespace: {currentNamespaceName} · {totalCount} keys
                    </p>
                </div>
                <button className="btn btn-dark fw-medium px-4 py-2 rounded-3">
                    + Thêm key
                </button>
            </div>

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
            />

            {/* PAGINATION COMPONENT */}
            <TranslationPagination
                pageNumber={pageNumber}
                totalPages={totalPages}
                onPrevious={() => setPageNumber(prev => prev - 1)}
                onNext={() => setPageNumber(prev => prev + 1)}
            />

        </div>
    );
}

export default TranslationManagementPage;