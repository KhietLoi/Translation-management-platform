import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

import ProjectCard from "../../components/projects/ProjectCard";
import { getProjects } from "../../services/projectService";

function ProjectListPage() {
    const navigate = useNavigate();

    const [projects, setProjects] = useState([]);

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

                <button className="btn btn-primary">
                    Create Project
                </button>

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

        </div>
    );
}

export default ProjectListPage;