function OverviewTab({ project }) {
    return (
        <div className="card">

            <div className="card-body">

                <div className="mb-3">
                    <strong>Name</strong>
                    <div>{project.name}</div>
                </div>

                <div className="mb-3">
                    <strong>Description</strong>
                    <div>{project.description}</div>
                </div>

                <div>
                    <strong>Status</strong>
                    <div>
                        {project.isActive
                            ? "Active"
                            : "Inactive"}
                    </div>
                </div>

            </div>

        </div>
    );
}

export default OverviewTab;