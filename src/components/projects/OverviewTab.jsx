function OverviewTab({ project }) {
    return (
        <div className="card">

            <div className="card-body">

                <div className="row">
                    <div className="col-md-6 mb-3 mb-md-0">
                        <div className="mb-3">
                            <strong>Name</strong>
                            <div>{project.name}</div>
                        </div>

                        <div className="mb-3">
                            <strong>Description</strong>
                            <div>{project.description || <span className="text-muted fst-italic">No description</span>}</div>
                        </div>

                        <div>
                            <strong>Status</strong>
                            <div>
                                <span className={`badge ${project.isActive ? "bg-success" : "bg-secondary"}`}>
                                    {project.isActive ? "Active" : "Inactive"}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-6 border-start-md">
                        <div>
                            <strong>Translation Status</strong>
                            <div className="mt-2">
                                <div className="d-flex justify-content-between mb-1 small">
                                    <span className="text-muted">Progress</span>
                                    <span className="fw-bold">{project.progressPercentage ?? 0}%</span>
                                </div>
                                <div className="progress mb-3" style={{ height: "8px" }}>
                                    <div
                                        className="progress-bar"
                                        role="progressbar"
                                        style={{
                                            width: `${project.progressPercentage ?? 0}%`,
                                            backgroundColor: (project.progressPercentage ?? 0) >= 100 ? "#198754" : "#0d6efd"
                                        }}
                                        aria-valuenow={project.progressPercentage ?? 0}
                                        aria-valuemin="0"
                                        aria-valuemax="100"
                                    />
                                </div>
                                <div className="d-flex justify-content-between small text-muted mb-1">
                                    <span>Completed Translations:</span>
                                    <span className="fw-semibold text-dark">{project.completedTranslationCount ?? 0}</span>
                                </div>
                                <div className="d-flex justify-content-between small text-muted">
                                    <span>Total Translations:</span>
                                    <span className="fw-semibold text-dark">{project.totalTranslationCount ?? 0}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

        </div>
    );
}

export default OverviewTab;