import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import ProjectCard from "../../components/projects/ProjectCard";
import CreateProjectModal from "../../components/projects/CreateProject";
import { getProjects } from "../../services/projectService";

import { usePermission } from "../../hooks/usePermission";
import { PERMISSIONS } from "../../constants/permissions";

function ProjectListPage() {
    const navigate = useNavigate();
    const { hasPermission } = usePermission();
    const canCreateProject = hasPermission(PERMISSIONS.PROJECT.CREATE);

    const [projects, setProjects] = useState([]);
    const [showCreateModal, setShowCreateModal] = useState(false);

    useEffect(() => {
        loadProjects();
    }, []);

    const loadProjects = async () => {
        try {
            const result = await getProjects();

            setProjects(
                result.data.projects || []
            );
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <div className="container py-4">

            <div className="d-flex justify-content-between align-items-center mb-4">

                <div>
                    <h2 className="mb-1">
                        Projects
                    </h2>

                    <p className="text-muted mb-0">
                        Manage translation projects
                    </p>
                </div>

                {canCreateProject && (
                    <button
                        className="btn btn-primary"
                        onClick={() => setShowCreateModal(true)}
                    >
                        Create Project
                    </button>
                )}

            </div>

            <div className="row g-4">

                {projects.map((project) => (
                    <div
                        key={project.id}
                        className="col-lg-4 col-md-6"
                    >
                        <ProjectCard
                            project={project}
                            onClick={(id) =>
                                navigate(`/projects/${id}`)
                            }
                        />
                    </div>
                ))}

            </div>

            <CreateProjectModal
                show={showCreateModal}
                onClose={() => setShowCreateModal(false)}
                onSuccess={() => loadProjects()}
            />

        </div>
    );
}

export default ProjectListPage;