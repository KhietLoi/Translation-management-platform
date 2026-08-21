import { useState, useEffect } from "react";
import { useOutletContext } from "react-router-dom";
import { toast } from "react-toastify";
import { Modal, Form, Spinner } from "react-bootstrap";
import {
  CheckCircleIcon,
  ArrowPathIcon,
  PaperAirplaneIcon,
  ClockIcon,
  XCircleIcon,
} from "@heroicons/react/24/outline";
import { getProjects } from "../../services/projectService";
import {
  publishTranslations,
  getReleaseHistory,
  rollbackRelease,
} from "../../services/translationPipelineService";
import ReleaseDiffModal from "../../components/delivery/ReleaseDiffModal";
import { signalRService } from "../../services/signalrService";
import "./delivery.css";

export default function PublishPage() {
  const outletContext = useOutletContext() || {};
  const {
    projects: contextProjects = [],
    selectedProjectId: contextProjectId = "",
    setSelectedProjectId: setContextProjectId,
    env: contextEnv = "Production",
  } = outletContext;

  const [loading, setLoading] = useState(false);
  const [localProjects, setLocalProjects] = useState([]);
  const [localProjectId, setLocalProjectId] = useState("");

  const projects = contextProjects.length > 0 ? contextProjects : localProjects;
  const selectedProjectId = contextProjectId || localProjectId;
  const setSelectedProjectId = setContextProjectId || setLocalProjectId;
  const env = contextEnv;

  // History & Release State
  const [releases, setReleases] = useState([]);
  const [isPublishing, setIsPublishing] = useState(false);
  const [publishStep, setPublishStep] = useState(0); // 0: Idle/Ready, 1..6: In progress, 6: Completed
  const [currentJobInfo, setCurrentJobInfo] = useState(null);
  const [publishError, setPublishError] = useState(null);
  const [currentProgress, setCurrentProgress] = useState(null);

  // Modals
  const [showPublishModal, setShowPublishModal] = useState(false);
  const [publishNotes, setPublishNotes] = useState("");
  const [showDiffModal, setShowDiffModal] = useState(false);
  const [diffSourceRelease, setDiffSourceRelease] = useState(null);
  const [diffTargetRelease, setDiffTargetRelease] = useState(null);

  // Load Projects if context is not present
  useEffect(() => {
    if (contextProjects.length === 0) {
      loadProjects();
    }
  }, [contextProjects]);

  const loadProjects = async () => {
    try {
      const res = await getProjects();
      const list = res.data?.projects || res.data || [];
      setLocalProjects(list);
      if (list.length > 0 && !selectedProjectId) {
        setLocalProjectId(list[0].id);
      }
    } catch (error) {
      console.error(error);
      toast.error("Failed to load projects list.");
    }
  };

  // Load Release History
  const loadReleaseHistory = async (projectId) => {
    if (!projectId) return;
    try {
      setLoading(true);
      const res = await getReleaseHistory({ projectId });
      const historyList = res.data?.items || res.data || res.items || [];
      setReleases(Array.isArray(historyList) ? historyList : []);
    } catch (error) {
      console.error(error);
      setReleases([]);
      toast.error("Failed to load release history.");
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (selectedProjectId) {
      loadReleaseHistory(selectedProjectId);
    }
  }, [selectedProjectId]);

  // Auto refresh release history on SignalR notification
  useEffect(() => {
    const handleNotification = () => {
      if (selectedProjectId) {
        loadReleaseHistory(selectedProjectId);
      }
    };
    window.addEventListener("translationNotification", handleNotification);
    return () => window.removeEventListener("translationNotification", handleNotification);
  }, [selectedProjectId]);

  // Subscribe to SignalR Publish Progress updates
  useEffect(() => {
    const unsubscribe = signalRService.subscribePublishProgress((progress) => {
      console.log("[PublishPage] Publish progress received via SignalR:", progress);
      setCurrentProgress(progress);

      // Map progress name to step index (1..5)
      let stepNum = 1;
      const lowerName = (progress.name || progress.Name || "").toLowerCase();

      if (lowerName.includes("validate")) {
        stepNum = 1;
      } else if (lowerName.includes("generate package") || lowerName.includes("json")) {
        stepNum = 2;
      } else if (lowerName.includes("upload") || lowerName.includes("blob")) {
        stepNum = 3;
      } else if (lowerName.includes("create release") || lowerName.includes("database") || lowerName.includes("save")) {
        stepNum = 4;
      } else if (
        lowerName.includes("complete") ||
        lowerName.includes("finished") ||
        lowerName.includes("success") ||
        lowerName.includes("skipped")
      ) {
        stepNum = 5;
      }

      const status = progress.status || progress.Status || "";
      if (status === "Failed" || status.toLowerCase().includes("failed")) {
        setPublishError(progress.message || progress.Message || "Publish failed.");
        setTimeout(() => {
          setIsPublishing(false);
        }, 2000);
      } else {
        setPublishStep(stepNum);

        // If final step completed, wait 2 seconds then finalize
        if (stepNum === 5 && (status === "Completed" || status.toLowerCase().includes("complete"))) {
          setTimeout(() => {
            setIsPublishing(false);
            if (selectedProjectId) {
              loadReleaseHistory(selectedProjectId);
            }
          }, 2000);
        }
      }
    });

    return () => unsubscribe();
  }, [selectedProjectId]);

  // Handle Submit Publish
  const handleStartPublish = async () => {
    if (!selectedProjectId) {
      toast.warning("Please select a project to publish.");
      return;
    }

    try {
      setShowPublishModal(false);
      setIsPublishing(true);
      setPublishStep(1);
      setPublishError(null);
      setCurrentProgress(null);

      const res = await publishTranslations({
        projectId: selectedProjectId,
        notes: publishNotes,
      });

      setCurrentJobInfo(res.data || res);
      setPublishNotes("");

      // Finalize progress on success - let the SignalR complete callback do it, or fallback after 2.5 seconds
      setPublishStep(5);

      const stats = res.data || res;
      if (stats && stats.skippedRecords > 0 && stats.successRecords === 0) {
        toast.info("Publish skipped: No changes detected.");
      } else {
        toast.success("New version published successfully!");
      }

      setTimeout(() => {
        setIsPublishing((prev) => {
          if (prev) {
            loadReleaseHistory(selectedProjectId);
            return false;
          }
          return prev;
        });
      }, 2500);
    } catch (error) {
      console.error(error);
      setIsPublishing(false);
      setPublishError(
        error?.response?.data?.errorMessage ||
        error?.message ||
        "Publish failed."
      );
      toast.error(
        error?.response?.data?.errorMessage ||
        "Publish failed. Please ensure translations are Reviewed."
      );
    }
  };

  // Handle Rollback
  const handleRollback = async (release) => {
    const versionNum = release.versionNumber ?? release.version ?? "";
    console.log("handleRollback target release:", release.releaseId || release.id);
    if (
      !window.confirm(
        `Are you sure you want to rollback to release v${versionNum}?`
      )
    ) {
      return;
    }

    try {
      await rollbackRelease(release.releaseId || release.id);
      toast.success(`Successfully rolled back to version v${versionNum}!`);
      loadReleaseHistory(selectedProjectId);
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage ||
        `Failed to rollback to version v${versionNum}.`
      );
    }
  };

  // Handle View Diff
  const handleOpenDiff = (releaseIndex) => {
    const currentRel = releases[releaseIndex];
    const prevRel = releases[releaseIndex + 1] || null;
    setDiffSourceRelease(prevRel);
    setDiffTargetRelease(currentRel);
    setShowDiffModal(true);
  };

  const selectedProject = projects.find((p) => p.id === selectedProjectId);
  const currentProjectName = selectedProject?.name || "Project";

  // Active release & Max version computation based on isActive
  const activeRelease = releases.find((r) => r.isActive);
  const maxVersionNum = releases.reduce(
    (max, r) => Math.max(max, r.versionNumber ?? r.version ?? 0),
    0
  );
  const activeVersionStr = activeRelease
    ? (activeRelease.versionNumber ?? activeRelease.version)
    : releases[0]
      ? (releases[0].versionNumber ?? releases[0].version)
      : "1";

  // Stepper definition using dynamic C# progress tracking
  const steps = [
    { id: 1, name: "Validate", sub: "Xác thực dữ liệu" },
    { id: 2, name: "Generate Package", sub: "Tạo JSON bản dịch" },
    { id: 3, name: "Upload Blob", sub: "Tải lên Cloud Storage" },
    { id: 4, name: "Create Release", sub: "Lưu phiên bản DB" },
    { id: 5, name: "Complete", sub: "Hoàn tất phát hành" },
  ];

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
    <div className="container-fluid py-2 px-3 delivery-container">
      {/* PAGE TITLE BAR */}
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-2">
        <div>
          <h2 className="fw-bold mb-1 fs-3 text-dark">Publish</h2>
          <p className="text-muted mb-0 fs-6">
            {currentProjectName} → <span className="fw-medium text-dark">{env}</span>
          </p>
        </div>
        <button
          className="btn btn-purple px-4 py-2 d-flex align-items-center gap-2"
          onClick={() => setShowPublishModal(true)}
          disabled={isPublishing}
        >
          {isPublishing ? (
            <Spinner animation="border" size="sm" />
          ) : (
            <PaperAirplaneIcon width={18} />
          )}
          Publish New Version
        </button>
      </div>

      {/* STEPPER CONTAINER CARD */}
      <div className="delivery-card mb-4">
        <div className="delivery-card-body">
          <div className="d-flex align-items-center justify-content-between mb-3">
            <h6 className="fw-bold mb-0 text-dark fs-6">
              {isPublishing
                ? `Processing — version v${maxVersionNum > 0 ? maxVersionNum + 1 : '1'}`
                : releases.length > 0
                  ? `Publish Status — Current Version v${activeVersionStr}`
                  : "Publish Status — Ready to Publish"}
            </h6>
            {isPublishing && (
              <span className="badge bg-purple-pill" style={{ color: "#7c3aed", backgroundColor: "#f3e8ff", padding: "6px 12px", borderRadius: "9999px", fontWeight: "600" }}>
                <Spinner animation="grow" size="sm" className="me-1 text-purple" style={{ width: "12px", height: "12px" }} /> Executing...
              </span>
            )}
          </div>

          {/* Stepper Workflow Visual */}
          <div className="stepper-container px-2 px-md-4">
            <div className="stepper-line">
              <div
                className="stepper-line-progress"
                style={{
                  width: isPublishing
                    ? `${((publishStep - 1) / (steps.length - 1)) * 100}%`
                    : publishError
                      ? `${((publishStep - 1) / (steps.length - 1)) * 100}%`
                      : releases.length > 0
                        ? "100%"
                        : "0%",
                }}
              />
            </div>

            {steps.map((step) => {
              const isCompleted = isPublishing
                ? (publishStep > step.id || (publishStep === 5 && step.id === 5 && !publishError))
                : (releases.length > 0);
              const isActive = isPublishing && publishStep === step.id && !publishError;
              const isFailed = isPublishing && publishStep === step.id && publishError;

              let nodeContent = <span>{step.id}</span>;
              let stepClass = "";
              let subText = step.sub;

              if (isCompleted) {
                nodeContent = <CheckCircleIcon width={24} className="text-white" />;
                stepClass = "completed";
                subText = "Completed";
              } else if (isFailed) {
                nodeContent = <XCircleIcon width={24} className="text-white" />;
                stepClass = "failed";
                subText = publishError;
              } else if (isActive) {
                nodeContent = <Spinner animation="border" size="sm" className="text-white" style={{ width: "16px", height: "16px" }} />;
                stepClass = "active";
                subText = currentProgress?.message || currentProgress?.Message || "Processing...";
              }

              return (
                <div
                  key={step.id}
                  className={`step-item ${stepClass}`}
                >
                  <div className="step-node d-flex align-items-center justify-content-center">
                    {nodeContent}
                  </div>
                  <div className="step-title fw-semibold mt-2">{step.name}</div>
                  <div className="step-subtext text-muted small mt-1" style={{ fontSize: "0.725rem", wordBreak: "break-word", maxWidth: "120px" }}>
                    {subText}
                  </div>
                </div>
              );
            })}
          </div>
        </div>
      </div>

      {/* VERSION HISTORY CARD */}
      <div className="delivery-card">
        <div className="delivery-card-header">
          <h6 className="fw-bold mb-0 text-dark fs-6">Release Version History</h6>
          <button
            className="btn btn-sm btn-outline-custom d-flex align-items-center gap-1"
            onClick={() => loadReleaseHistory(selectedProjectId)}
            disabled={loading}
          >
            <ArrowPathIcon width={14} className={loading ? "spin" : ""} />
            Refresh
          </button>
        </div>

        <div className="delivery-card-body p-0">
          {loading ? (
            <div className="text-center py-5">
              <Spinner animation="border" variant="primary" />
              <p className="mt-2 text-muted small">Loading version history...</p>
            </div>
          ) : releases.length === 0 ? (
            <div className="text-center py-5 text-muted">
              No versions published yet for this project.
            </div>
          ) : (
            <div className="list-group list-group-flush">
              {releases.map((rel, index) => (
                <div
                  key={rel.releaseId || rel.id || index}
                  className="list-group-item p-3 d-flex align-items-center justify-content-between hover-bg-light border-bottom"
                >
                  <div className="d-flex align-items-center gap-3">
                    <span className="badge-version">
                      v{rel.versionNumber ?? rel.version ?? "1.0"}
                    </span>
                    {rel.isActive && (
                      <span className="badge bg-success-subtle text-success small fw-semibold px-2 py-1 rounded-pill">
                        Active
                      </span>
                    )}
                    <div>
                      <div className="fw-semibold text-dark fs-6">
                        {rel.environment || env}
                        {rel.totalKey !== undefined && rel.totalKey !== null && (
                          <span className="text-muted fw-normal">
                            {" "}· {rel.totalKey.toLocaleString()} keys
                          </span>
                        )}
                        {(rel.publishingUserName || rel.publishedByName || rel.publishedBy) && (
                          <span className="text-muted fw-normal">
                            {" "}· published by {rel.publishingUserName || rel.publishedByName || rel.publishedBy}
                          </span>
                        )}
                      </div>
                      <div className="text-muted small d-flex align-items-center gap-1">
                        <ClockIcon width={13} />
                        {getTimeAgo(rel.releaseDate || rel.publishedAt)}
                        {rel.notes && <span className="ms-2">({rel.notes})</span>}
                      </div>
                    </div>
                  </div>

                  <div className="d-flex align-items-center gap-2">
                    <button
                      className="btn btn-sm btn-outline-custom px-3 py-1 text-dark fw-medium"
                      onClick={() => handleOpenDiff(index)}
                    >
                      View Diff
                    </button>
                    {!rel.isActive && (
                      <button
                        className="btn btn-sm btn-outline-custom px-3 py-1 text-dark fw-medium"
                        onClick={() => handleRollback(rel)}
                      >
                        Rollback
                      </button>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* CONFIRM PUBLISH MODAL */}
      <Modal show={showPublishModal} onHide={() => setShowPublishModal(false)} centered>
        <Modal.Header closeButton className="border-bottom-0">
          <Modal.Title className="fw-bold fs-5 text-dark">
            Confirm New Release Version
          </Modal.Title>
        </Modal.Header>
        <Modal.Body className="py-0">
          <p className="text-muted small mb-3">
            The system will aggregate all <strong className="text-success">Reviewed</strong> translation keys and build a release JSON package for environment <strong>{env}</strong>.
          </p>

          <Form.Group className="mb-3">
            <Form.Label className="fw-medium text-dark small">Release Notes (Optional)</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              placeholder="Enter release notes (e.g., Fix typos, add locale...)"
              value={publishNotes}
              onChange={(e) => setPublishNotes(e.target.value)}
            />
          </Form.Group>
        </Modal.Body>
        <Modal.Footer className="border-top-0 pt-0">
          <button
            className="btn btn-outline-custom px-4"
            onClick={() => setShowPublishModal(false)}
          >
            Cancel
          </button>
          <button
            className="btn btn-purple px-4 d-flex align-items-center gap-2"
            onClick={handleStartPublish}
          >
            <PaperAirplaneIcon width={16} /> Start Publish
          </button>
        </Modal.Footer>
      </Modal>

      {/* RELEASE DIFF MODAL */}
      <ReleaseDiffModal
        show={showDiffModal}
        onClose={() => setShowDiffModal(false)}
        sourceRelease={diffSourceRelease}
        targetRelease={diffTargetRelease}
        releases={releases}
      />
    </div>
  );
}
