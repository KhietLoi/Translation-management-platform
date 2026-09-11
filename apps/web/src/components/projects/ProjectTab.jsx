function ProjectTabs({
    activeTab,
    setActiveTab
}) {
    return (
        <ul className="nav nav-tabs mb-4">

            <li className="nav-item">
                <button
                    className={`nav-link ${activeTab === "overview"
                            ? "active"
                            : ""
                        }`}
                    onClick={() =>
                        setActiveTab("overview")
                    }
                >
                    Overview
                </button>
            </li>

            <li className="nav-item">
                <button
                    className={`nav-link ${activeTab === "languages"
                            ? "active"
                            : ""
                        }`}
                    onClick={() =>
                        setActiveTab("languages")
                    }
                >
                    Languages
                </button>
            </li>

            <li className="nav-item">
                <button
                    className={`nav-link ${activeTab === "namespaces"
                            ? "active"
                            : ""
                        }`}
                    onClick={() =>
                        setActiveTab("namespaces")
                    }
                >
                    Namespaces
                </button>
            </li>

            <li className="nav-item">
                <button
                    className={`nav-link ${activeTab === "members"
                            ? "active"
                            : ""
                        }`}
                    onClick={() =>
                        setActiveTab("members")
                    }
                >
                    Members
                </button>
            </li>

        </ul>
    );
}

export default ProjectTabs;