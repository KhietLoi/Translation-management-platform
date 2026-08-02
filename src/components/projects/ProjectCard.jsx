import "./ProjectCard.css";

function ProjectCard({ project, onClick }) {
    return (
        <div
            className="project-card"
            onClick={() => onClick(project.id)}
        >
            <div className="project-card-header">
                <div>
                    <h5 className="project-title">
                        {project.name}
                    </h5>

                    <p className="project-description">
                        {project.description}
                    </p>
                </div>

                <div>
                    <span
                        className={`badge ${project.isActive
                            ? "bg-success"
                            : "bg-secondary"
                            }`}
                    >
                        {project.isActive
                            ? "Active"
                            : "Inactive"}
                    </span>
                </div>
            </div>

            <div className="project-stats">

                <div>
                    <strong>{project.languageCount}</strong>
                    <span>Languages</span>
                </div>

                <div>
                    <strong>{project.memberCount}</strong>
                    <span>Members</span>
                </div>

                <div>
                    <strong>{project.namespaceCount}</strong>
                    <span>Namespaces</span>
                </div>

            </div>

            <div className="project-key-count">
                <strong>
                    {project.totalKeys}
                </strong>
                <span> Keys</span>
            </div>

            <div className="project-languages">
                {project.languages
                    ?.slice(0, 3)
                    .map((language) => (
                        <span
                            key={language.id}
                            className="language-chip"
                        >
                            {language.code}
                        </span>
                    ))}

                {project.languages?.length > 3 && (
                    <span className="language-chip-more">
                        +{project.languages.length - 3}
                    </span>
                )}
            </div>

            <div className="project-progress">

                <div className="d-flex justify-content-between mb-2">

                    <span>
                        Translation Progress
                    </span>

                    <strong>
                        {project.translationProgress}%
                    </strong>

                </div>

                <div className="progress">

                    <div
                        className="progress-bar"
                        role="progressbar"
                        style={{
                            width: `${project.translationProgress}%`,
                        }}
                    />

                </div>

            </div>
        </div>
    );
}

export default ProjectCard;