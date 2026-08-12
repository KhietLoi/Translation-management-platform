import { useState, useEffect } from "react";
import { useOutletContext } from "react-router-dom";
import { toast } from "react-toastify";
import { Modal, Form, Spinner } from "react-bootstrap";
import {
  CheckCircleIcon,
  ArrowPathIcon,
  PaperAirplaneIcon,
  ClockIcon,
} from "@heroicons/react/24/outline";
import { getProjects } from "../../services/projectService";
import {
  publishTranslations,
  getReleaseHistory,
  rollbackRelease,
} from "../../services/translationPipelineService";
import ReleaseDiffModal from "../../components/delivery/ReleaseDiffModal";
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

      // Stepper animation sequence
      setTimeout(() => setPublishStep(2), 1200);
      setTimeout(() => setPublishStep(3), 2400);

      const res = await publishTranslations({
        projectId: selectedProjectId,
        notes: publishNotes,
      });

      setCurrentJobInfo(res.data);
      setPublishNotes("");

      setTimeout(() => setPublishStep(4), 3600);
      setTimeout(() => setPublishStep(5), 4800);
      setTimeout(() => {
        setPublishStep(6);
        toast.success("New version published successfully!");
        loadReleaseHistory(selectedProjectId);
        setIsPublishing(false);
      }, 6000);
    } catch (error) {
      console.error(error);
      setIsPublishing(false);
      setPublishStep(0);
      toast.error(
        error?.response?.data?.errorMessage ||
        "Publish failed. Please ensure translations are Reviewed."
      );
    }
  };

  // Handle Rollback
  const handleRollback = async (release) => {
    if (
      !window.confirm(
        `Are you sure you want to rollback to release v${release.version}?`
      )
    ) {
      return;
    }

    try {
      await rollbackRelease(release.id);
      toast.success(`Successfully rolled back to version v${release.version}!`);
      loadReleaseHistory(selectedProjectId);
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage ||
        `Failed to rollback to version v${release.version}.`
      );
    }
  };

  // Handle View Diff
  const handleOpenDiff = (releaseIndex) => {
    const currentRel = releases[releaseIndex];
    const prevRel = releases[releaseIndex + 1] || releases[releaseIndex];
    setDiffSourceRelease(prevRel);
    setDiffTargetRelease(currentRel);
    setShowDiffModal(true);
  };

  const selectedProject = projects.find((p) => p.id === selectedProjectId);
  const currentProjectName = selectedProject?.name || "Project";

  // Stepper definition in English
  const steps = [
    { id: 1, name: "Validate", sub: "128 valid keys" },
    { id: 2, name: "Queue", sub: "Job #4821" },
    {
      id: 3,
      name: "Generate JSON",
      sub: publishStep === 3 ? "Running..." : publishStep > 3 ? "Completed" : "Pending",
    },
    {
      id: 4,
      name: "Compress",
      sub: publishStep === 4 ? "Compressing..." : publishStep > 4 ? "Completed" : "Pending",
    },
    {
      id: 5,
      name: "Upload Blob",
      sub: publishStep === 5 ? "Uploading..." : publishStep > 5 ? "Completed" : "Pending",
    },
    {
      id: 6,
      name: "Notify",
      sub: publishStep >= 6 ? "Notification sent" : "Pending",
    },
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
                ? `Processing — version v1.8.3`
                : releases.length > 0
                  ? `Publish Status — Current Version v${releases[0].version}`
                  : "Processing — version v1.8.3"}
            </h6>
            {isPublishing && (
              <span className="badge bg-purple-pill">
                <Spinner animation="grow" size="sm" className="me-1" /> Executing...
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
                    : "40%",
                }}
              />
            </div>

            {steps.map((step) => {
              const isCompleted = isPublishing ? publishStep > step.id : step.id <= 2;
              const isActive = isPublishing ? publishStep === step.id : step.id === 3;

              return (
                <div
                  key={step.id}
                  className={`step-item ${isCompleted ? "completed" : ""} ${isActive ? "active" : ""
                    }`}
                >
                  <div className="step-node">
                    {isCompleted ? (
                      <CheckCircleIcon width={24} />
                    ) : (
                      <span>{step.id}</span>
                    )}
                  </div>
                  <div className="step-title">{step.name}</div>
                  <div className="step-subtext">{step.sub}</div>
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
                  key={rel.id || index}
                  className="list-group-item p-3 d-flex align-items-center justify-content-between hover-bg-light border-bottom"
                >
                  <div className="d-flex align-items-center gap-3">
                    <span className="badge-version">v{rel.version}</span>
                    <div>
                      <div className="fw-semibold text-dark fs-6">
                        {rel.environment || env}
                        {rel.totalKey !== undefined && (
                          <span className="text-muted fw-normal">
                            {" "}· {rel.totalKey.toLocaleString()} keys
                          </span>
                        )}
                        {(rel.publishedByName || rel.publishedBy) && (
                          <span className="text-muted fw-normal">
                            {" "}· published by {rel.publishedByName || rel.publishedBy}
                          </span>
                        )}
                      </div>
                      <div className="text-muted small d-flex align-items-center gap-1">
                        <ClockIcon width={13} />
                        {getTimeAgo(rel.publishedAt)}
                        {rel.notes && <span className="ms-2">({rel.notes})</span>}
                      </div>
                    </div>
                  </div>

                  <div className="d-flex align-items-center gap-2">
                    {index === 0 ? (
                      <button
                        className="btn btn-sm btn-outline-custom px-3 py-1 text-dark fw-medium"
                        onClick={() => handleOpenDiff(index)}
                      >
                        View Diff
                      </button>
                    ) : (
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
      />
    </div>
  );
}
