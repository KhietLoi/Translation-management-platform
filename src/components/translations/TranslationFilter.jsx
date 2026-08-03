function TranslationFilter({
    projects,
    namespaces,
    selectedProjectId,
    selectedNamespaceId,
    keyword,
    status,
    numberOfLanguages,
    onProjectChange,
    onNamespaceChange,
    onKeywordChange,
    onStatusChange,
    onNumberOfLanguagesChange,
    onSearch
}) {
    return (
        <div className="card mb-3">
            <div className="card-body">
                <div className="row g-3">

                    <div className="col-md-3">
                        <label className="form-label">
                            Search
                        </label>

                        <input
                            type="text"
                            className="form-control"
                            placeholder="Search translation key..."
                            value={keyword}
                            onChange={(e) =>
                                onKeywordChange(e.target.value)
                            }
                        />
                    </div>

                    <div className="col-md-2">
                        <label className="form-label">
                            Project
                        </label>

                        <select
                            className="form-select"
                            value={selectedProjectId}
                            onChange={(e) =>
                                onProjectChange(e.target.value)
                            }
                        >
                            {projects.map(project => (
                                <option
                                    key={project.id}
                                    value={project.id}
                                >
                                    {project.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="col-md-2">
                        <label className="form-label">
                            Namespace
                        </label>

                        <select
                            className="form-select"
                            value={selectedNamespaceId}
                            onChange={(e) =>
                                onNamespaceChange(e.target.value)
                            }
                        >
                            <option value="">
                                All
                            </option>

                            {namespaces.map(ns => (
                                <option
                                    key={ns.id}
                                    value={ns.id}
                                >
                                    {ns.name}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="col-md-2">
                        <label className="form-label">
                            Languages
                        </label>

                        <select
                            className="form-select"
                            value={numberOfLanguages}
                            onChange={(e) =>
                                onNumberOfLanguagesChange(
                                    Number(e.target.value)
                                )
                            }
                        >
                            {[1, 2, 3, 4, 5, 6].map(number => (
                                <option
                                    key={number}
                                    value={number}
                                >
                                    {number}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="col-md-2">
                        <label className="form-label">
                            Status
                        </label>

                        <select
                            className="form-select"
                            value={status}
                            onChange={(e) =>
                                onStatusChange(e.target.value)
                            }
                        >
                            <option value="">All</option>
                            <option value="0">Missing</option>
                            <option value="1">Draft</option>
                            <option value="2">Translated</option>
                            <option value="3">Rejected</option>
                            <option value="4">Reviewed</option>
                            <option value="5">Published</option>
                        </select>
                    </div>

                    <div className="col-md-1 d-grid">
                        <label className="form-label invisible">
                            Search
                        </label>

                        <button
                            className="btn btn-primary"
                            onClick={onSearch}
                        >
                            Search
                        </button>
                    </div>

                </div>
            </div>
        </div>
    );
}

export default TranslationFilter;