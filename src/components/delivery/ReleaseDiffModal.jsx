import { useState, useEffect } from "react";
import { Modal, Spinner, Nav } from "react-bootstrap";
import { getReleaseDiff } from "../../services/translationPipelineService";
import { toast } from "react-toastify";
import {
  PlusCircleIcon,
  PencilSquareIcon,
  TrashIcon,
  ArrowsRightLeftIcon,
} from "@heroicons/react/24/outline";

export default function ReleaseDiffModal({
  show,
  onClose,
  sourceRelease,
  targetRelease,
}) {
  const [loading, setLoading] = useState(false);
  const [diffData, setDiffData] = useState(null);
  const [activeTab, setActiveTab] = useState("all");

  useEffect(() => {
    if (show && sourceRelease && targetRelease) {
      loadDiff();
    }
  }, [show, sourceRelease, targetRelease]);

  const loadDiff = async () => {
    try {
      setLoading(true);
      const response = await getReleaseDiff(
        sourceRelease.id,
        targetRelease.id
      );
      setDiffData(response.data || response);
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage ||
          "Failed to load release diff comparison."
      );
    } finally {
      setLoading(false);
    }
  };

  const added = diffData?.added || [];
  const updated = diffData?.updated || [];
  const removed = diffData?.removed || [];

  return (
    <Modal show={show} onHide={onClose} size="lg" centered scrollable>
      <Modal.Header closeButton className="border-bottom-0 pb-0">
        <Modal.Title className="d-flex align-items-center gap-2 fs-5 fw-bold text-dark">
          <ArrowsRightLeftIcon width={22} height={22} className="text-primary" />
          Release Diff Comparison
        </Modal.Title>
      </Modal.Header>
      <Modal.Body className="pt-2">
        <div className="bg-light p-3 rounded-3 mb-3 d-flex justify-content-between align-items-center">
          <div>
            <span className="badge bg-secondary me-2">
              Source Release: {sourceRelease?.version ? `v${sourceRelease.version}` : "v1.0"}
            </span>
            <span className="text-muted fs-7">→</span>
            <span className="badge bg-purple-pill ms-2">
              Target Release: {targetRelease?.version ? `v${targetRelease.version}` : "Latest"}
            </span>
          </div>
          <div className="text-muted small">
            {sourceRelease?.notes || "Comparing translation changes"}
          </div>
        </div>

        {loading ? (
          <div className="text-center py-5">
            <Spinner animation="border" variant="primary" role="status" />
            <p className="mt-2 text-muted small">Comparing release diff...</p>
          </div>
        ) : diffData ? (
          <>
            {/* Summary Counters */}
            <div className="row g-2 mb-3">
              <div className="col-4">
                <div className="border rounded-3 p-2 text-center bg-success-subtle border-success-subtle">
                  <div className="fs-4 fw-bold text-success">
                    +{diffData.addedCount || added.length}
                  </div>
                  <div className="small text-success fw-medium">Added</div>
                </div>
              </div>
              <div className="col-4">
                <div className="border rounded-3 p-2 text-center bg-primary-subtle border-primary-subtle">
                  <div className="fs-4 fw-bold text-primary">
                    ~{diffData.updatedCount || updated.length}
                  </div>
                  <div className="small text-primary fw-medium">Updated</div>
                </div>
              </div>
              <div className="col-4">
                <div className="border rounded-3 p-2 text-center bg-danger-subtle border-danger-subtle">
                  <div className="fs-4 fw-bold text-danger">
                    -{diffData.removedCount || removed.length}
                  </div>
                  <div className="small text-danger fw-medium">Removed</div>
                </div>
              </div>
            </div>

            {/* Nav Tabs */}
            <Nav
              variant="tabs"
              activeKey={activeTab}
              onSelect={(k) => setActiveTab(k)}
              className="mb-3"
            >
              <Nav.Item>
                <Nav.Link eventKey="all" className="fw-semibold">
                  All ({added.length + updated.length + removed.length})
                </Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="added" className="text-success fw-semibold">
                  Added ({added.length})
                </Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="updated" className="text-primary fw-semibold">
                  Updated ({updated.length})
                </Nav.Link>
              </Nav.Item>
              <Nav.Item>
                <Nav.Link eventKey="removed" className="text-danger fw-semibold">
                  Removed ({removed.length})
                </Nav.Link>
              </Nav.Item>
            </Nav>

            {/* List Content */}
            <div className="diff-list border rounded-3 p-2" style={{ maxHeight: "350px", overflowY: "auto" }}>
              {/* Added */}
              {(activeTab === "all" || activeTab === "added") &&
                added.map((item, idx) => (
                  <div
                    key={`add-${idx}`}
                    className="p-2 mb-2 rounded bg-success-subtle border-start border-success border-4 text-dark"
                  >
                    <div className="d-flex align-items-center justify-content-between">
                      <span className="fw-bold text-success font-monospace">
                        <PlusCircleIcon width={16} className="me-1 inline" />
                        {item.key}
                      </span>
                      <span className="badge bg-success small">{item.languageCode || "Global"}</span>
                    </div>
                    <div className="small mt-1 text-muted">
                      New Value: <code className="text-dark bg-white px-1 rounded">{item.newValue}</code>
                    </div>
                  </div>
                ))}

              {/* Updated */}
              {(activeTab === "all" || activeTab === "updated") &&
                updated.map((item, idx) => (
                  <div
                    key={`upd-${idx}`}
                    className="p-2 mb-2 rounded bg-primary-subtle border-start border-primary border-4 text-dark"
                  >
                    <div className="d-flex align-items-center justify-content-between">
                      <span className="fw-bold text-primary font-monospace">
                        <PencilSquareIcon width={16} className="me-1 inline" />
                        {item.key}
                      </span>
                      <span className="badge bg-primary small">{item.languageCode || "Global"}</span>
                    </div>
                    <div className="small mt-1 d-flex flex-column gap-1">
                      <div className="text-danger">
                        Old: <del className="bg-white px-1 rounded">{item.oldValue}</del>
                      </div>
                      <div className="text-success">
                        New: <code className="bg-white px-1 rounded text-dark">{item.newValue}</code>
                      </div>
                    </div>
                  </div>
                ))}

              {/* Removed */}
              {(activeTab === "all" || activeTab === "removed") &&
                removed.map((item, idx) => (
                  <div
                    key={`rem-${idx}`}
                    className="p-2 mb-2 rounded bg-danger-subtle border-start border-danger border-4 text-dark"
                  >
                    <div className="d-flex align-items-center justify-content-between">
                      <span className="fw-bold text-danger font-monospace">
                        <TrashIcon width={16} className="me-1 inline" />
                        {item.key}
                      </span>
                      <span className="badge bg-danger small">{item.languageCode || "Global"}</span>
                    </div>
                    <div className="small mt-1 text-muted">
                      Old Value: <del className="bg-white px-1 rounded">{item.oldValue}</del>
                    </div>
                  </div>
                ))}

              {added.length === 0 && updated.length === 0 && removed.length === 0 && (
                <div className="text-center py-4 text-muted">
                  No differences found between these two releases.
                </div>
              )}
            </div>
          </>
        ) : (
          <div className="text-center py-4 text-muted">
            No diff data available.
          </div>
        )}
      </Modal.Body>
      <Modal.Footer className="border-top-0 pt-0">
        <button className="btn btn-secondary px-4 rounded-3" onClick={onClose}>
          Close
        </button>
      </Modal.Footer>
    </Modal>
  );
}
