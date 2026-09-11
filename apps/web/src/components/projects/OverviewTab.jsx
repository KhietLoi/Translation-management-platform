function OverviewTab({ project }) {
    const pObj = project?.project || project?.data || project || {};

    return (
        <div className="card border-0 shadow-sm">
            <div className="card-body p-4">
                <div className="row g-4">
                    <div className="col-md-6">
                        <div className="mb-3">
                            <strong className="text-secondary small text-uppercase">Project Name</strong>
                            <div className="fs-5 fw-semibold text-dark mt-1">{pObj.name}</div>
                        </div>

                        <div className="mb-3">
                            <strong className="text-secondary small text-uppercase">Status</strong>
                            <div className="mt-1">
                                <span className={`badge ${pObj.isActive ? "bg-success" : "bg-secondary"} px-2.5 py-1.5 rounded-pill`}>
                                    {pObj.isActive ? "Active" : "Inactive"}
                                </span>
                            </div>
                        </div>
                    </div>

                    <div className="col-md-6">
                        <div className="mb-3">
                            <strong className="text-secondary small text-uppercase">Description</strong>
                            <div className="text-secondary mt-1">
                                {pObj.description || <span className="text-muted fst-italic">No description provided.</span>}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default OverviewTab;