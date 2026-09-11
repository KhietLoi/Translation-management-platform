import { MagnifyingGlassIcon, ChevronDownIcon } from "@heroicons/react/24/outline";

export default function TopControlBar({
  projects = [],
  selectedProjectId = "",
  onProjectChange = () => {},
  env = "Production",
  onEnvChange = () => {},
  searchQuery = "",
  onSearchChange = () => {},
}) {
  return (
    <div className="bg-white border rounded-4 p-2.5 px-3 shadow-sm mb-4 d-flex align-items-center justify-content-between flex-wrap gap-3">
      {/* Left side: Project dropdown & Environment switcher */}
      <div className="d-flex align-items-center gap-3">
        {/* Project Selector */}
        <select
          className="form-select border-0 bg-light fw-bold text-dark rounded-3 px-3 py-2 cursor-pointer"
          style={{ width: "220px", fontSize: "0.9rem" }}
          value={selectedProjectId}
          onChange={(e) => onProjectChange(e.target.value)}
        >
          {projects.map((p) => (
            <option key={p.id} value={p.id}>
              ■ {p.name}
            </option>
          ))}
          {projects.length === 0 && <option value="">-- Select Project --</option>}
        </select>

        {/* Environment Switcher Pills */}
        <div className="env-selector">
          <button
            type="button"
            className={`env-pill ${env === "Production" ? "active" : ""}`}
            onClick={() => onEnvChange("Production")}
          >
            Production
          </button>
          <button
            type="button"
            className={`env-pill ${env === "Staging" ? "active" : ""}`}
            onClick={() => onEnvChange("Staging")}
          >
            Staging
          </button>
        </div>
      </div>

      {/* Right side: Search Input */}
      <div className="position-relative" style={{ width: "320px", maxWidth: "100%" }}>
        <MagnifyingGlassIcon
          width={18}
          className="position-absolute top-50 translate-middle-y start-0 ms-3 text-muted"
        />
        <input
          type="text"
          className="form-control border-0 bg-light rounded-3 ps-5 pe-4 py-2 text-dark"
          style={{ fontSize: "0.875rem" }}
          placeholder="Search keys, projects, members..."
          value={searchQuery}
          onChange={(e) => onSearchChange(e.target.value)}
        />
        <ChevronDownIcon
          width={14}
          className="position-absolute top-50 translate-middle-y end-0 me-3 text-muted pointer-events-none"
        />
      </div>
    </div>
  );
}
