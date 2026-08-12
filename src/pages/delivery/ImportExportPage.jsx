import { useState, useEffect, useRef } from "react";
import { toast } from "react-toastify";
import { Spinner, Form } from "react-bootstrap";
import {
  ArrowUpTrayIcon,
  ArrowPathIcon,
  DocumentCheckIcon,
  DocumentIcon,
  XMarkIcon,
  ClockIcon,
} from "@heroicons/react/24/outline";
import {
  getProjects,
  getProjectLanguages,
  getProjectNamespaces,
} from "../../services/projectService";
import { getLanguages } from "../../services/languageService";
import {
  importTranslations,
  exportTranslations,
  getTranslationHistory,
} from "../../services/translationPipelineService";
import TopControlBar from "../../components/delivery/TopControlBar";
import "./delivery.css";

export default function ImportExportPage() {
  const [loadingProjects, setLoadingProjects] = useState(false);
  const [projects, setProjects] = useState([]);
  const [languages, setLanguages] = useState([]);
  const [namespaces, setNamespaces] = useState([]);
  const [env, setEnv] = useState("Production");
  const [searchQuery, setSearchQuery] = useState("");

  // Import State
  const [importProjectId, setImportProjectId] = useState("");
  const [importLanguageId, setImportLanguageId] = useState("");
  const [importNamespaceId, setImportNamespaceId] = useState("");
  const [selectedFile, setSelectedFile] = useState(null);
  const [fileFormat, setFileFormat] = useState(0); // 0: JSON, 2: Excel, 1: CSV
  const [isImporting, setIsImporting] = useState(false);
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef(null);

  // Export State
  const [exportProjectId, setExportProjectId] = useState("");
  const [exportFormat, setExportFormat] = useState(0); // 0: JSON (.zip), 2: Excel, 1: CSV
  const [isExporting, setIsExporting] = useState(false);

  // History State
  const [historyLoading, setHistoryLoading] = useState(false);
  const [history, setHistory] = useState([]);

  // Load Initial Projects
  useEffect(() => {
    loadProjects();
  }, []);

  const loadProjects = async () => {
    try {
      setLoadingProjects(true);
      const res = await getProjects();
      const list = res.data?.projects || res.data || [];
      setProjects(list);
      if (list.length > 0) {
        setImportProjectId(list[0].id);
        setExportProjectId(list[0].id);
      }
    } catch (error) {
      console.error(error);
      toast.error("Failed to load projects list.");
    } finally {
      setLoadingProjects(false);
    }
  };

  // Load Languages & Namespaces whenever Import Project changes
  useEffect(() => {
    if (importProjectId) {
      loadLanguagesAndNamespaces(importProjectId);
      loadHistory(importProjectId);
    }
  }, [importProjectId]);

  const loadLanguagesAndNamespaces = async (projectId) => {
    try {
      // Fetch Languages
      let langList = [];
      try {
        const langRes = await getProjectLanguages(projectId);
        langList = langRes.data?.languages || langRes.data || [];
      } catch {
        // Fallback to global languages
        const globalLangRes = await getLanguages();
        langList = globalLangRes.data?.languages || globalLangRes.data || [];
      }
      setLanguages(langList);
      if (langList.length > 0) {
        setImportLanguageId(langList[0].id || langList[0].languageId);
      } else {
        setImportLanguageId("");
      }

      // Fetch Namespaces
      const nsRes = await getProjectNamespaces(projectId);
      const nsList = nsRes.data?.namespaces || nsRes.data || [];
      setNamespaces(nsList);
      if (nsList.length > 0) {
        setImportNamespaceId(nsList[0].id);
      } else {
        setImportNamespaceId("");
      }
    } catch (error) {
      console.error(error);
    }
  };

  // Load History
  const loadHistory = async (projectId) => {
    try {
      setHistoryLoading(true);
      const res = await getTranslationHistory({ projectId });
      const items = res.data?.items || res.data || [];
      setHistory(Array.isArray(items) ? items : []);
    } catch (error) {
      console.error(error);
      setHistory([]);
      toast.error("Failed to load import/export history.");
    } finally {
      setHistoryLoading(false);
    }
  };

  // Drag & Drop File Handlers
  const handleDragOver = (e) => {
    e.preventDefault();
    setIsDragging(true);
  };

  const handleDragLeave = (e) => {
    e.preventDefault();
    setIsDragging(false);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    setIsDragging(false);
    if (e.dataTransfer.files && e.dataTransfer.files.length > 0) {
      const file = e.dataTransfer.files[0];
      handleFileSelected(file);
    }
  };

  const handleFileSelected = (file) => {
    setSelectedFile(file);
    // Auto-detect format by extension
    const ext = file.name.split(".").pop().toLowerCase();
    if (ext === "json") setFileFormat(0);
    else if (ext === "xlsx" || ext === "xls") setFileFormat(2);
    else if (ext === "csv") setFileFormat(1);
  };

  // Submit Import
  const handleExecuteImport = async () => {
    if (!importProjectId) {
      toast.warning("Please select a Project.");
      return;
    }
    if (!importLanguageId) {
      toast.warning("Please select a Language.");
      return;
    }
    if (!importNamespaceId) {
      toast.warning("Please select a Namespace.");
      return;
    }
    if (!selectedFile) {
      toast.warning("Please select or drop a file to import.");
      return;
    }

    try {
      setIsImporting(true);
      const formData = new FormData();
      formData.append("ProjectId", importProjectId);
      formData.append("LanguageId", importLanguageId);
      formData.append("NamespaceId", importNamespaceId);
      formData.append("Format", fileFormat);
      formData.append("File", selectedFile);

      await importTranslations(formData);
      toast.success("File imported successfully! Background job initiated.");
      setSelectedFile(null);
      if (fileInputRef.current) fileInputRef.current.value = "";
      loadHistory(importProjectId);
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage ||
          "Import failed. Please check your file format and try again."
      );
    } finally {
      setIsImporting(false);
    }
  };

  // Submit Export
  const handleExecuteExport = async () => {
    if (!exportProjectId) {
      toast.warning("Please select a Project to export.");
      return;
    }

    try {
      setIsExporting(true);
      const res = await exportTranslations({
        projectId: exportProjectId,
        format: exportFormat,
      });

      toast.success("Export package created successfully!");

      if (res.data?.downloadUrl) {
        window.open(res.data.downloadUrl, "_blank");
      }

      loadHistory(exportProjectId);
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Failed to initiate export."
      );
    } finally {
      setIsExporting(false);
    }
  };

  // Helper for time ago in English
  const getTimeAgo = (dateStr) => {
    if (!dateStr) return "Recently";
    const diffMs = Date.now() - new Date(dateStr).getTime();
    const diffMins = Math.floor(diffMs / 60000);
    if (diffMins < 1) return "Just now";
    if (diffMins < 60) return `${diffMins} mins ago`;
    const diffHours = Math.floor(diffMins / 60);
    if (diffHours < 24) return `${diffHours} hours ago`;
    const diffDays = Math.floor(diffHours / 24);
    return `${diffDays} days ago`;
  };

  return (
    <div className="container-fluid py-4 px-4 delivery-container">
      {/* TOP CONTROL BAR MATCHING SCREENSHOT 1 STRUCTURE */}
      <TopControlBar
        projects={projects}
        selectedProjectId={importProjectId}
        onProjectChange={(val) => {
          setImportProjectId(val);
          setExportProjectId(val);
        }}
        env={env}
        onEnvChange={setEnv}
        searchQuery={searchQuery}
        onSearchChange={setSearchQuery}
      />

      {/* HEADER SECTION */}
      <div className="mb-4">
        <h2 className="fw-bold mb-1 fs-3 text-dark">Import / Export</h2>
        <p className="text-muted mb-0 fs-6">
          Synchronize translation keys with JSON, CSV, or Excel files.
        </p>
      </div>

      {/* TOP ROW: 2 CARDS */}
      <div className="row g-4 mb-4">
        {/* LEFT CARD: IMPORT FILE */}
        <div className="col-lg-7">
          <div className="delivery-card h-100 d-flex flex-column">
            <div className="delivery-card-header">
              <h6 className="fw-bold mb-0 text-dark fs-6">Import File</h6>
            </div>

            <div className="delivery-card-body d-flex flex-column flex-grow-1">
              {/* SELECTORS FOR PROJECT, LANGUAGE, NAMESPACE */}
              <div className="row g-3 mb-3">
                <div className="col-md-4">
                  <Form.Group>
                    <Form.Label className="fw-medium text-dark small mb-1">
                      Project <span className="text-danger">*</span>
                    </Form.Label>
                    <Form.Select
                      size="sm"
                      className="rounded-3 border-secondary-subtle"
                      value={importProjectId}
                      onChange={(e) => setImportProjectId(e.target.value)}
                    >
                      {projects.map((p) => (
                        <option key={p.id} value={p.id}>
                          {p.name}
                        </option>
                      ))}
                      {projects.length === 0 && (
                        <option value="">-- Select Project --</option>
                      )}
                    </Form.Select>
                  </Form.Group>
                </div>

                <div className="col-md-4">
                  <Form.Group>
                    <Form.Label className="fw-medium text-dark small mb-1">
                      Language <span className="text-danger">*</span>
                    </Form.Label>
                    <Form.Select
                      size="sm"
                      className="rounded-3 border-secondary-subtle"
                      value={importLanguageId}
                      onChange={(e) => setImportLanguageId(e.target.value)}
                    >
                      {languages.map((l) => (
                        <option key={l.id || l.languageId} value={l.id || l.languageId}>
                          {l.code || l.name} ({l.name || l.code})
                        </option>
                      ))}
                      {languages.length === 0 && (
                        <option value="">-- Select Language --</option>
                      )}
                    </Form.Select>
                  </Form.Group>
                </div>

                <div className="col-md-4">
                  <Form.Group>
                    <Form.Label className="fw-medium text-dark small mb-1">
                      Namespace <span className="text-danger">*</span>
                    </Form.Label>
                    <Form.Select
                      size="sm"
                      className="rounded-3 border-secondary-subtle"
                      value={importNamespaceId}
                      onChange={(e) => setImportNamespaceId(e.target.value)}
                    >
                      {namespaces.map((n) => (
                        <option key={n.id} value={n.id}>
                          {n.name}
                        </option>
                      ))}
                      {namespaces.length === 0 && (
                        <option value="">-- Select Namespace --</option>
                      )}
                    </Form.Select>
                  </Form.Group>
                </div>
              </div>

              {/* DROPZONE AREA */}
              <div
                className={`dropzone-container mb-3 ${
                  isDragging ? "drag-over" : ""
                }`}
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
                onDrop={handleDrop}
                onClick={() => fileInputRef.current?.click()}
              >
                <input
                  type="file"
                  ref={fileInputRef}
                  className="d-none"
                  accept=".json,.xlsx,.xls,.csv"
                  onChange={(e) => {
                    if (e.target.files && e.target.files.length > 0) {
                      handleFileSelected(e.target.files[0]);
                    }
                  }}
                />

                <div className="dropzone-icon-box">
                  <ArrowUpTrayIcon width={24} height={24} />
                </div>

                {selectedFile ? (
                  <div className="d-flex align-items-center justify-content-center gap-2">
                    <DocumentCheckIcon width={20} className="text-success" />
                    <span className="fw-bold text-dark">{selectedFile.name}</span>
                    <span className="text-muted small">
                      ({(selectedFile.size / 1024).toFixed(1)} KB)
                    </span>
                    <button
                      type="button"
                      className="btn btn-sm text-danger p-0 border-0 ms-2"
                      onClick={(e) => {
                        e.stopPropagation();
                        setSelectedFile(null);
                        if (fileInputRef.current) fileInputRef.current.value = "";
                      }}
                    >
                      <XMarkIcon width={18} />
                    </button>
                  </div>
                ) : (
                  <>
                    <div className="fw-bold text-dark mb-1">
                      Drag and drop file here
                    </div>
                    <div className="text-muted small">
                      or <span className="text-purple fw-semibold">choose file</span> — supports json, xlsx, csv
                    </div>
                  </>
                )}
              </div>

              {/* FORMAT TOGGLE PILLS */}
              <div className="format-pills mb-4">
                <button
                  className={`format-pill-btn ${fileFormat === 0 ? "active" : ""}`}
                  onClick={() => setFileFormat(0)}
                >
                  JSON
                </button>
                <button
                  className={`format-pill-btn ${fileFormat === 2 ? "active" : ""}`}
                  onClick={() => setFileFormat(2)}
                >
                  Excel
                </button>
                <button
                  className={`format-pill-btn ${fileFormat === 1 ? "active" : ""}`}
                  onClick={() => setFileFormat(1)}
                >
                  CSV
                </button>
              </div>

              {/* SUBMIT IMPORT BUTTON */}
              <button
                className="btn btn-purple w-100 py-2 mt-auto fw-bold"
                onClick={handleExecuteImport}
                disabled={isImporting || !selectedFile}
              >
                {isImporting ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Executing Import...
                  </>
                ) : (
                  "Execute Import"
                )}
              </button>
            </div>
          </div>
        </div>

        {/* RIGHT CARD: EXPORT PACKAGE */}
        <div className="col-lg-5">
          <div className="delivery-card h-100 d-flex flex-column">
            <div className="delivery-card-header">
              <h6 className="fw-bold mb-0 text-dark fs-6">Export Package</h6>
            </div>

            <div className="delivery-card-body d-flex flex-column flex-grow-1">
              <div className="mb-4">
                <Form.Group className="mb-3">
                  <Form.Label className="fw-medium text-dark small mb-1">
                    Project
                  </Form.Label>
                  <Form.Select
                    size="sm"
                    className="rounded-3 border-secondary-subtle"
                    value={exportProjectId}
                    onChange={(e) => setExportProjectId(e.target.value)}
                  >
                    {projects.map((p) => (
                      <option key={p.id} value={p.id}>
                        {p.name}
                      </option>
                    ))}
                    {projects.length === 0 && (
                      <option value="">-- Select Project --</option>
                    )}
                  </Form.Select>
                </Form.Group>

                {/* DETAILS OVERVIEW TABLE */}
                <div className="bg-light p-3 rounded-3 mb-3 border">
                  <div className="d-flex justify-content-between py-2 border-bottom">
                    <span className="text-muted small">Language</span>
                    <span className="fw-bold text-dark small">
                      All ({languages.length || 6})
                    </span>
                  </div>
                  <div className="d-flex justify-content-between py-2 border-bottom">
                    <span className="text-muted small">Format</span>
                    <span className="fw-bold text-dark small">
                      {exportFormat === 0
                        ? ".zip (JSON)"
                        : exportFormat === 2
                        ? ".xlsx (Excel)"
                        : ".csv (CSV)"}
                    </span>
                  </div>
                  <div className="d-flex justify-content-between py-2 border-bottom">
                    <span className="text-muted small">Namespace</span>
                    <span className="fw-bold text-dark small">All</span>
                  </div>
                  <div className="d-flex justify-content-between py-2">
                    <span className="text-muted small">Estimated Size</span>
                    <span className="fw-bold text-dark small">~1.2 MB</span>
                  </div>
                </div>

                {/* FORMAT SELECTION */}
                <Form.Group className="mb-2">
                  <Form.Label className="fw-medium text-dark small mb-1">
                    Export File Format
                  </Form.Label>
                  <Form.Select
                    size="sm"
                    className="rounded-3 border-secondary-subtle"
                    value={exportFormat}
                    onChange={(e) => setExportFormat(Number(e.target.value))}
                  >
                    <option value={0}>.zip (JSON files per language)</option>
                    <option value={2}>.xlsx (Full Excel sheet)</option>
                    <option value={1}>.csv (Standard CSV file)</option>
                  </Form.Select>
                </Form.Group>
              </div>

              {/* ACTION BUTTON */}
              <button
                className="btn btn-dark-custom w-100 py-2.5 mt-auto fw-bold"
                onClick={handleExecuteExport}
                disabled={isExporting}
              >
                {isExporting ? (
                  <>
                    <Spinner animation="border" size="sm" className="me-2" />
                    Generating package...
                  </>
                ) : (
                  "Generate & Download from Blob"
                )}
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* BOTTOM CARD: HISTORY TABLE */}
      <div className="delivery-card">
        <div className="delivery-card-header">
          <h6 className="fw-bold mb-0 text-dark fs-6">
            Recent Import / Export History
          </h6>
          <button
            className="btn btn-sm btn-outline-custom d-flex align-items-center gap-1"
            onClick={() => loadHistory(importProjectId)}
            disabled={historyLoading}
          >
            <ArrowPathIcon width={14} className={historyLoading ? "spin" : ""} />
            Refresh
          </button>
        </div>

        <div className="delivery-card-body p-0">
          {historyLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="primary" />
              <p className="mt-2 text-muted small">Loading history...</p>
            </div>
          ) : (
            <div className="table-responsive">
              <table className="table table-hover align-middle mb-0">
                <thead className="table-light">
                  <tr className="text-uppercase text-muted" style={{ fontSize: "0.75rem" }}>
                    <th className="ps-4 py-3">FILE</th>
                    <th className="py-3">TYPE</th>
                    <th className="py-3">RESULT</th>
                    <th className="pe-4 py-3 text-end">TIME</th>
                  </tr>
                </thead>
                <tbody>
                  {history.map((row, idx) => (
                    <tr key={row.id || idx}>
                      <td className="ps-4 fw-semibold text-dark font-monospace">
                        <DocumentIcon width={16} className="me-2 text-muted inline" />
                        {row.fileName || row.name || `file_${idx}.json`}
                      </td>
                      <td>
                        <span
                          className={`badge ${
                            row.type === "Import" || row.type === 0
                              ? "bg-primary-subtle text-primary"
                              : "bg-purple-pill"
                          }`}
                        >
                          {row.type === 0 ? "Import" : row.type === 1 ? "Export" : row.type}
                        </span>
                      </td>
                      <td>
                        <span
                          className={
                            row.status === "warning" || row.result?.includes("warning")
                              ? "badge-warning-pill"
                              : "badge-success-pill"
                          }
                        >
                          • {row.result || row.status || "Completed"}
                        </span>
                      </td>
                      <td className="pe-4 text-end text-muted small">
                        <ClockIcon width={13} className="me-1 inline" />
                        {row.timeAgo || (row.createdAt ? getTimeAgo(row.createdAt) : "Recently")}
                      </td>
                    </tr>
                  ))}

                  {history.length === 0 && (
                    <tr>
                      <td colSpan={4} className="text-center py-4 text-muted">
                        No import/export history available.
                      </td>
                    </tr>
                  )}
                </tbody>
              </table>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
