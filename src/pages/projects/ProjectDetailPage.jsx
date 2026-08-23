import React, { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";

import ProjectTabs from "../../components/projects/ProjectTab";
import OverviewTab from "../../components/projects/OverviewTab";
import LanguagesTab from "../../components/projects/LanguageTab";
import NamespacesTab from "../../components/projects/NamespaceTab";
import MembersTab from "../../components/projects/MemberTab";

import { toast } from "react-toastify";

import {
    getProjectById,
    getProjectLanguages,
    getProjectNamespaces,
    getProjectMembers,
    updateProject,
    deleteProject
} from "../../services/projectService";

import { usePermission } from "../../hooks/usePermission";
import { PERMISSIONS } from "../../constants/permissions";

import "./ProjectDetailPage.css";

function ProjectDetailPage() {
    const { id } = useParams();
    const navigate = useNavigate();

    const { hasPermission } = usePermission();
    const canUpdateProject = hasPermission(PERMISSIONS.PROJECT.UPDATE);
    const canDeleteProject = hasPermission(PERMISSIONS.PROJECT.DELETE);

    const [project, setProject] = useState(null);
    const [languages, setLanguages] = useState([]);
    const [namespaces, setNamespaces] = useState([]);
    const [members, setMembers] = useState([]);
    const [activeTab, setActiveTab] = useState("overview");
    const [loading, setLoading] = useState(true);

    // Edit modal & Delete state
    const [showEditModal, setShowEditModal] = useState(false);
    const [editForm, setEditForm] = useState({ name: "", description: "", isActive: true });
    const [saving, setSaving] = useState(false);
    const [deleting, setDeleting] = useState(false);

    const loadData = async () => {
        setLoading(true);
        try {
            const [
                projectRes,
                languageRes,
                namespaceRes,
                memberRes
            ] = await Promise.all([
                getProjectById(id),
                getProjectLanguages(id),
                getProjectNamespaces(id),
                getProjectMembers(id)
            ]);

            setProject(projectRes.data);
            setLanguages(languageRes.data.languages || []);
            setNamespaces(namespaceRes.data.namespaces || []);
            setMembers(memberRes.data.members || []);
        } catch (error) {
            console.error("Failed to load project data:", error);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadData();
    }, [id]);

    const handleOpenEdit = () => {
        if (!canUpdateProject) return;
        if (project) {
            setEditForm({
                name: project.name || "",
                description: project.description || "",
                isActive: project.isActive || true
            });
            setShowEditModal(true);
        }
    };

    const handleSaveEdit = async () => {
        if (!canUpdateProject || !editForm.name.trim()) return;
        try {
            setSaving(true);
            await updateProject(id, {
                name: editForm.name,
                description: editForm.description,
                isActive: editForm.isActive
            });
            toast.success("Project updated successfully");
            // Dispatch projectsChanged event so MainLayout can reload projects list (to show updated project name)
            window.dispatchEvent(new CustomEvent("projectsChanged", { detail: { selectProjectId: id } }));
            setShowEditModal(false);
            await loadData();
        } catch (error) {
            console.error("Failed to update project:", error);
            toast.error(error?.response?.data?.message || "Failed to update project");
        } finally {
            setSaving(false);
        }
    };

    const handleDeleteProject = async () => {
        if (!canDeleteProject) return;
        if (window.confirm(`Are you sure you want to delete project "${project.name}"?`)) {
            try {
                setDeleting(true);
                await deleteProject(id);
                toast.success("Project deleted successfully");
                // Dispatch projectsChanged event to refresh dropdown list in MainLayout
                window.dispatchEvent(new CustomEvent("projectsChanged"));
                navigate("/projects");
            } catch (error) {
                console.error("Failed to delete project:", error);
                toast.error(error?.response?.data?.message || "Failed to delete project");
                setDeleting(false);
            }
        }
    };

    // Xử lý giao diện Loading với Spinner của Bootstrap 5
    if (loading) {
        return (
            <div className="container loading-wrapper">
                <div className="spinner-border text-primary mb-3" role="status" style={{ width: '3rem', height: '3rem' }}></div>
                <h5 className="text-muted">Loading project details...</h5>
            </div>
        );
    }

    // Xử lý giao diện Lỗi / Không tìm thấy
    if (!project) {
        return (
            <div className="container py-5 text-center">
                <div className="alert alert-warning shadow-sm" role="alert">
                    <i className="bi bi-exclamation-triangle-fill me-2"></i>
                    Project not found or you don't have permission to view it.
                </div>
            </div>
        );
    }

    // Khai báo mảng stat để render HTML gọn hơn
    const stats = [
        { label: "Languages", value: project.languageCount || 0, color: "text-primary" },
        { label: "Members", value: project.memberCount || 0, color: "text-success" },
        { label: "Namespaces", value: project.namespaceCount || 0, color: "text-info" },
        {
            label: "Progress",
            value: `${project.progressPercentage ?? 0}%`,
            color: (project.progressPercentage ?? 0) >= 100 ? "text-success" : "text-warning",
            subValue: `${project.completedTranslationCount ?? 0} / ${project.totalTranslationCount ?? 0} translations`
        }
    ];

    return (
        <div className="container py-5">

            {/* Header Section */}
            <div className="project-header shadow-sm mb-4 d-flex justify-content-between align-items-center">
                <div>
                    <h2 className="fw-bold mb-2">{project.name}</h2>
                    <p className="text-muted mb-0">{project.description}</p>
                </div>
                <div className="d-flex align-items-center gap-2">
                    <span className={`badge badge-status me-2 ${project.isActive ? "bg-success" : "bg-secondary"}`}>
                        {project.isActive ? "Active" : "Inactive"}
                    </span>
                    {canUpdateProject && (
                        <button
                            className="btn btn-outline-primary btn-sm me-2"
                            onClick={handleOpenEdit}
                        >
                            Edit
                        </button>
                    )}
                    {canDeleteProject && (
                        <button
                            className="btn btn-outline-danger btn-sm"
                            onClick={handleDeleteProject}
                            disabled={deleting}
                        >
                            {deleting ? "Deleting..." : "Delete"}
                        </button>
                    )}
                </div>
            </div>

            {/* Stats Section */}
            <div className="row g-4 mb-5">
                {stats.map((stat, index) => (
                    <div className="col-md-3 col-sm-6" key={index}>
                        <div className="card stat-card shadow-sm h-100">
                            <div className="card-body text-center d-flex flex-column justify-content-center py-4">
                                <span className="text-uppercase text-muted fw-semibold" style={{ fontSize: '0.85rem' }}>
                                    {stat.label}
                                </span>
                                <h3 className={`stat-value ${stat.color} mb-0`}>
                                    {stat.value}
                                </h3>
                                {stat.subValue && (
                                    <span className="text-muted mt-2" style={{ fontSize: '0.8rem' }}>
                                        {stat.subValue}
                                    </span>
                                )}
                            </div>
                        </div>
                    </div>
                ))}
            </div>

            {/* Tabs Menu */}
            <div className="bg-white rounded shadow-sm p-3 mb-4">
                <ProjectTabs activeTab={activeTab} setActiveTab={setActiveTab} />
            </div>

            {/* Tab Content Rendering */}
            <div className="tab-content-wrapper">
                {activeTab === "overview" && <OverviewTab project={project} />}
                {activeTab === "languages" && (
                    <LanguagesTab
                        projectId={id}
                        languages={languages}
                        onUpdated={loadData}
                    />
                )}
                {activeTab === "namespaces" && <NamespacesTab projectId={project.id} namespaces={namespaces} onUpdate={loadData} />}
                {activeTab === "members" && (
                    <MembersTab
                        projectId={id}
                        members={members}
                        onUpdated={loadData}
                    />
                )}
            </div>

            {/* Edit Project Modal */}
            {showEditModal && (
                <div className="modal d-block" tabIndex="-1" style={{ backgroundColor: "rgba(0,0,0,0.5)" }}>
                    <div className="modal-dialog modal-dialog-centered">
                        <div className="modal-content">
                            <div className="modal-header">
                                <h5 className="modal-title">Edit Project</h5>
                                <button
                                    type="button"
                                    className="btn-close"
                                    onClick={() => setShowEditModal(false)}
                                    disabled={saving}
                                ></button>
                            </div>
                            <div className="modal-body">
                                <div className="mb-3">
                                    <label className="form-label fw-semibold">Project Name</label>
                                    <input
                                        type="text"
                                        className="form-control"
                                        value={editForm.name}
                                        onChange={(e) => setEditForm({ ...editForm, name: e.target.value })}
                                    />
                                </div>
                                <div className="mb-3">
                                    <label className="form-label fw-semibold">Description</label>
                                    <textarea
                                        className="form-control"
                                        rows="3"
                                        value={editForm.description}
                                        onChange={(e) => setEditForm({ ...editForm, description: e.target.value })}
                                    ></textarea>
                                </div>
                                {/* Active Toggle */}
                                <div className="mb-3 form-check form-switch">
                                    <input
                                        className="form-check-input"
                                        type="checkbox"
                                        id="activeToggle"
                                        checked={editForm.isActive}
                                        onChange={(e) =>
                                            setEditForm({ ...editForm, isActive: e.target.checked })}
                                    />
                                    <label className="form-check-label" htmlFor="activeToggle">
                                        {editForm.isActive ? "Active" : "Inactive"}
                                    </label>
                                </div>
                            </div>
                            <div className="modal-footer">
                                <button
                                    type="button"
                                    className="btn-secondary btn"
                                    onClick={() => setShowEditModal(false)}
                                    disabled={saving}
                                >
                                    Cancel
                                </button>
                                <button
                                    type="button"
                                    className="btn btn-primary"
                                    onClick={handleSaveEdit}
                                    disabled={saving || !editForm.name.trim()}
                                >
                                    {saving ? "Saving..." : "Save Changes"}
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            )}

        </div>
    );
}

export default ProjectDetailPage;