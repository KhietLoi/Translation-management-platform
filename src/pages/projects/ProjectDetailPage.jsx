import React, { useEffect, useState } from "react";
import { useParams } from "react-router-dom";

import ProjectTabs from "../../components/projects/ProjectTab";
import OverviewTab from "../../components/projects/OverviewTab";
import LanguagesTab from "../../components/projects/LanguageTab";
import NamespacesTab from "../../components/projects/NamespaceTab";
import MembersTab from "../../components/projects/MemberTab";

import {
    getProjectById,
    getProjectLanguages,
    getProjectNamespaces,
    getProjectMembers
} from "../../services/projectService";

import "./ProjectDetailPage.css";

function ProjectDetailPage() {
    const { id } = useParams();

    const [project, setProject] = useState(null);
    const [languages, setLanguages] = useState([]);
    const [namespaces, setNamespaces] = useState([]);
    const [members, setMembers] = useState([]);
    const [activeTab, setActiveTab] = useState("overview");
    const [loading, setLoading] = useState(true);
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
        { label: "Languages", value: project.languageCount, color: "text-primary" },
        { label: "Members", value: project.memberCount, color: "text-success" },
        { label: "Namespaces", value: project.namespaceCount, color: "text-info" }
    ];

    return (
        <div className="container py-5">

            {/* Header Section */}
            <div className="project-header shadow-sm mb-4 d-flex justify-content-between align-items-center">
                <div>
                    <h2 className="fw-bold mb-2">{project.name}</h2>
                    <p className="text-muted mb-0">{project.description}</p>
                </div>
                <div>
                    <span className={`badge badge-status ${project.isActive ? "bg-success" : "bg-secondary"}`}>
                        {project.isActive ? "Active" : "Inactive"}
                    </span>
                </div>
            </div>

            {/* Stats Section (Dùng map để tránh lặp code) */}
            <div className="row g-4 mb-5">
                {stats.map((stat, index) => (
                    <div className="col-md-4" key={index}>
                        <div className="card stat-card shadow-sm h-100">
                            <div className="card-body text-center d-flex flex-column justify-content-center">
                                <span className="text-uppercase text-muted fw-semibold" style={{ fontSize: '0.85rem' }}>
                                    {stat.label}
                                </span>
                                <h3 className={`stat-value ${stat.color}`}>
                                    {stat.value || 0}
                                </h3>
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

        </div>
    );
}

export default ProjectDetailPage;