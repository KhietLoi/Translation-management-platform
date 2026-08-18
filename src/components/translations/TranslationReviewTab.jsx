import { useEffect, useState } from "react";
import { toast } from "react-toastify";
import {
    getReviewTranslations,
    batchReviewTranslations
} from "../../services/translationManagementService";
import { getProjectLanguages } from "../../services/projectService";
import {
    CheckCircleIcon,
    XCircleIcon,
    ExclamationTriangleIcon,
    CheckIcon,
    XMarkIcon,
    ArrowPathIcon
} from "@heroicons/react/24/outline";
import "./TranslationReviewTab.css";

function TranslationReviewTab({ projectId, namespaces = [], canReview, canPublish }) {
    // Selection state
    const [selectedNamespaceId, setSelectedNamespaceId] = useState("");
    const [selectedLanguageId, setSelectedLanguageId] = useState("");
    const [projectLanguages, setProjectLanguages] = useState([]);

    // Core data & state
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const [loadingLanguages, setLoadingLanguages] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    // Decisions object: { [translationValueId]: { status: 3|4, rejectReason: string } }
    const [decisions, setDecisions] = useState({});

    // Checkbox selections for bulk action: [translationValueId, ...]
    const [selectedItemIds, setSelectedItemIds] = useState([]);

    // ==========================================
    // EFFECT: Load languages when project changes
    // ==========================================
    useEffect(() => {
        if (!projectId) {
            setProjectLanguages([]);
            setSelectedLanguageId("");
            return;
        }

        const loadLanguages = async () => {
            try {
                setLoadingLanguages(true);
                const res = await getProjectLanguages(projectId);
                const list = res.data?.languages || res.data || [];
                setProjectLanguages(list);

                // Keep selected language if it's still in the list, otherwise pick the first
                const stillExists = list.some(l => (l.languageId || l.id) === selectedLanguageId);
                if (!stillExists) {
                    if (list.length > 0) {
                        setSelectedLanguageId(list[0].languageId || list[0].id);
                    } else {
                        setSelectedLanguageId("");
                    }
                }
            } catch (error) {
                console.error("Failed to load project languages:", error);
                toast.error("Failed to load languages for the project");
            } finally {
                setLoadingLanguages(false);
            }
        };

        loadLanguages();
    }, [projectId]);

    // Keep namespace in sync when namespace list changes (e.g. active project changes)
    useEffect(() => {
        if (namespaces.length > 0) {
            // Check if current selected namespace is in the new list
            const exists = namespaces.some(n => n.id === selectedNamespaceId);
            if (!exists) {
                setSelectedNamespaceId(namespaces[0].id);
            }
        } else {
            setSelectedNamespaceId("");
        }
    }, [namespaces, projectId]);

    // ==========================================
    // EFFECT: Load review items
    // ==========================================
    const loadReviewItems = async () => {
        if (!projectId || !selectedNamespaceId || !selectedLanguageId) {
            setItems([]);
            setDecisions({});
            setSelectedItemIds([]);
            return;
        }

        try {
            setLoading(true);
            setDecisions({});
            setSelectedItemIds([]);
            const res = await getReviewTranslations({
                projectId,
                namespaceId: selectedNamespaceId,
                languageId: selectedLanguageId
            });
            const list = res.data?.items || res.data || [];
            setItems(list);
        } catch (error) {
            console.error("Failed to load review items:", error);
            toast.error("Failed to load translations pending review");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadReviewItems();
    }, [projectId, selectedNamespaceId, selectedLanguageId]);

    // ==========================================
    // INDIVIDUAL ACTIONS
    // ==========================================
    const handleSetDecision = (id, status) => {
        setDecisions(prev => {
            const next = { ...prev };
            if (status === null) {
                delete next[id];
            } else {
                const currentReason = next[id]?.rejectReason || "";
                next[id] = {
                    status,
                    rejectReason: status === 3 ? currentReason : ""
                };
            }
            return next;
        });
    };

    const handleRejectReasonChange = (id, reason) => {
        setDecisions(prev => {
            if (!prev[id] || prev[id].status !== 3) return prev;
            return {
                ...prev,
                [id]: {
                    ...prev[id],
                    rejectReason: reason
                }
            };
        });
    };

    // ==========================================
    // BULK ACTIONS
    // ==========================================
    const handleSelectRow = (id) => {
        setSelectedItemIds(prev => {
            if (prev.includes(id)) {
                return prev.filter(x => x !== id);
            }
            return [...prev, id];
        });
    };

    const handleSelectAll = (e) => {
        if (e.target.checked) {
            setSelectedItemIds(items.map(item => item.translationValueId));
        } else {
            setSelectedItemIds([]);
        }
    };

    const handleBulkApprove = () => {
        if (selectedItemIds.length === 0) return;
        setDecisions(prev => {
            const next = { ...prev };
            selectedItemIds.forEach(id => {
                next[id] = { status: 4, rejectReason: "" };
            });
            return next;
        });
        toast.info(`Marked ${selectedItemIds.length} items as Approved locally`);
    };

    const handleBulkReject = () => {
        if (selectedItemIds.length === 0) return;

        const commonReason = window.prompt("Enter a common rejection reason for all selected items (optional):") || "";

        setDecisions(prev => {
            const next = { ...prev };
            selectedItemIds.forEach(id => {
                next[id] = { status: 3, rejectReason: commonReason };
            });
            return next;
        });
        toast.info(`Marked ${selectedItemIds.length} items as Rejected locally`);
    };

    const handleBulkReset = () => {
        if (selectedItemIds.length === 0) return;
        setDecisions(prev => {
            const next = { ...prev };
            selectedItemIds.forEach(id => {
                delete next[id];
            });
            return next;
        });
    };

    const handleResetAllDecisions = () => {
        setDecisions({});
        toast.info("All local review decisions have been cleared");
    };

    // ==========================================
    // SUBMISSION
    // ==========================================
    const handleSubmitReview = async () => {
        const decidedIds = Object.keys(decisions);
        if (decidedIds.length === 0) return;

        // Validation: reject items must have reasons? We can warn but allow or enforce it. Let's check.
        // It's a best practice to require a rejection reason, let's enforce non-empty reason for rejected items.
        const invalidRejects = decidedIds.filter(id => {
            const dec = decisions[id];
            return dec.status === 3 && !dec.rejectReason.trim();
        });

        if (invalidRejects.length > 0) {
            toast.error("Please provide a rejection reason for all rejected translations.");
            return;
        }

        try {
            setSubmitting(true);

            const payload = {
                projectId,
                languageId: selectedLanguageId,
                namespaceId: selectedNamespaceId,
                items: decidedIds.map(id => ({
                    translationValueId: id,
                    status: decisions[id].status,
                    rejectReason: decisions[id].rejectReason || null
                }))
            };

            const response = await batchReviewTranslations(payload);
            const data = response.data || response;

            toast.success(
                `Successfully submitted review! Approved: ${data.approved || 0}, Rejected: ${data.rejected || 0}`
            );

            // Reload the list since items are now approved/rejected and will disappear from review list
            await loadReviewItems();
        } catch (error) {
            console.error("Batch review submit failed:", error);
            toast.error(error?.response?.data?.errorMessage || "Failed to submit review decisions.");
        } finally {
            setSubmitting(false);
        }
    };

    // Helper counts
    const decisionsArray = Object.values(decisions);
    const approvedCount = decisionsArray.filter(d => d.status === 4).length;
    const rejectedCount = decisionsArray.filter(d => d.status === 3).length;
    const totalDecidedCount = decisionsArray.length;

    // Filter selectors render
    return (
        <div className="review-tab-container fade-in">
            {/* Filter selectors card */}
            <div className="card border-0 shadow-sm mb-4">
                <div className="card-body p-3">
                    <div className="row g-3 align-items-center">
                        {/* Namespace Selector */}
                        <div className="col-12 col-md-5">
                            <label className="form-label fw-semibold text-secondary small mb-1">Namespace</label>
                            <select
                                className="form-select border-1 rounded-3"
                                value={selectedNamespaceId}
                                onChange={(e) => setSelectedNamespaceId(e.target.value)}
                                disabled={namespaces.length === 0}
                            >
                                {namespaces.length === 0 && (
                                    <option value="">No Namespaces Available</option>
                                )}
                                {namespaces.map(ns => (
                                    <option key={ns.id} value={ns.id}>
                                        {ns.name}
                                    </option>
                                ))}
                            </select>
                        </div>

                        {/* Language Selector */}
                        <div className="col-12 col-md-5">
                            <label className="form-label fw-semibold text-secondary small mb-1">Target Language</label>
                            <select
                                className="form-select border-1 rounded-3"
                                value={selectedLanguageId}
                                onChange={(e) => setSelectedLanguageId(e.target.value)}
                                disabled={projectLanguages.length === 0 || loadingLanguages}
                            >
                                {loadingLanguages ? (
                                    <option value="">Loading languages...</option>
                                ) : projectLanguages.length === 0 ? (
                                    <option value="">No Languages Configured</option>
                                ) : (
                                    projectLanguages.map(lang => {
                                        const id = lang.languageId || lang.id;
                                        const code = lang.languageCode || lang.code;
                                        const name = lang.languageName || lang.name;
                                        return (
                                            <option key={id} value={id}>
                                                {name} ({code})
                                            </option>
                                        );
                                    })
                                )}
                            </select>
                        </div>

                        {/* Refresh Button */}
                        <div className="col-12 col-md-2 d-flex align-items-end h-100 mt-md-4 pt-md-2">
                            <button
                                className="btn btn-outline-secondary rounded-3 w-100 d-flex align-items-center justify-content-center gap-2 py-2"
                                onClick={loadReviewItems}
                                disabled={loading || !selectedNamespaceId || !selectedLanguageId}
                                title="Reload List"
                            >
                                <ArrowPathIcon width={16} className={loading ? "spin-animation" : ""} />
                                Refresh
                            </button>
                        </div>
                    </div>
                </div>
            </div>

            {/* MAIN CONTENT AREA */}
            {!selectedNamespaceId || !selectedLanguageId ? (
                <div className="review-empty-state">
                    <div className="review-empty-state-icon">
                        <ExclamationTriangleIcon width={32} />
                    </div>
                    <h5 className="fw-semibold text-dark mb-1">Select Namespace and Language</h5>
                    <p className="text-muted mb-0">Please choose a namespace and target language from the filters above to retrieve translations pending review.</p>
                </div>
            ) : loading ? (
                <div className="card border-0 shadow-sm">
                    <div className="card-body py-5 text-center text-muted">
                        <div className="spinner-border text-dark spinner-border-sm me-2" role="status" />
                        <span>Loading pending translations...</span>
                    </div>
                </div>
            ) : items.length === 0 ? (
                <div className="review-empty-state border-success-subtle bg-success-subtle bg-opacity-10">
                    <div className="review-empty-state-icon text-success bg-success bg-opacity-10 border border-success-subtle">
                        <CheckIcon width={32} />
                    </div>
                    <h5 className="fw-semibold text-success mb-1">All Caught Up!</h5>
                    <p className="text-muted mb-0">No translations are pending review for this namespace and language.</p>
                </div>
            ) : (
                <div className="card border-0 shadow-sm mb-4">
                    {/* Bulk Operations Toolbar */}
                    <div className="card-header bg-white border-bottom py-3 px-3 d-flex flex-wrap align-items-center justify-content-between gap-3">
                        <div className="d-flex align-items-center gap-2">
                            <div className="form-check fs-6 mb-0">
                                <input
                                    type="checkbox"
                                    className="form-check-input"
                                    id="selectAllHeader"
                                    checked={selectedItemIds.length === items.length}
                                    onChange={handleSelectAll}
                                />
                                <label className="form-check-label text-dark fw-medium" htmlFor="selectAllHeader">
                                    Select All ({items.length} items)
                                </label>
                            </div>
                            {selectedItemIds.length > 0 && (
                                <span className="badge bg-secondary-subtle text-secondary rounded-pill px-2.5 py-1.5 ms-2">
                                    {selectedItemIds.length} Selected
                                </span>
                            )}
                        </div>

                        {selectedItemIds.length > 0 && (
                            <div className="d-flex align-items-center gap-2 fade-in">
                                <span className="small text-muted fw-medium me-1">Bulk action:</span>
                                <button
                                    className="btn btn-sm btn-success rounded-3 d-flex align-items-center gap-1.5"
                                    onClick={handleBulkApprove}
                                >
                                    <CheckCircleIcon width={16} /> Approve
                                </button>
                                <button
                                    className="btn btn-sm btn-danger rounded-3 d-flex align-items-center gap-1.5"
                                    onClick={handleBulkReject}
                                >
                                    <XCircleIcon width={16} /> Reject
                                </button>
                                <button
                                    className="btn btn-sm btn-outline-secondary rounded-3"
                                    onClick={handleBulkReset}
                                >
                                    Reset Decisions
                                </button>
                            </div>
                        )}
                    </div>

                    {/* Table View */}
                    <div className="card-body p-0">
                        <div className="table-responsive">
                            <table className="table review-table align-middle mb-0">
                                <thead className="table-light text-uppercase">
                                    <tr>
                                        <th className="py-3 px-3 text-center" style={{ width: "50px" }}></th>
                                        <th className="py-3 px-3" style={{ width: "25%" }}>Key</th>
                                        <th className="py-3 px-3">Translated Value</th>
                                        <th className="py-3 px-3 text-center" style={{ width: "260px" }}>Review Decision</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {items.map(item => {
                                        const id = item.translationValueId;
                                        const isSelected = selectedItemIds.includes(id);
                                        const decision = decisions[id];

                                        let rowClass = "review-row";
                                        if (decision?.status === 4) rowClass += " review-row-approved";
                                        if (decision?.status === 3) rowClass += " review-row-rejected";

                                        return (
                                            <tr key={id} className={rowClass}>
                                                {/* Checkbox */}
                                                <td className="text-center py-3 px-2">
                                                    <input
                                                        type="checkbox"
                                                        className="form-check-input"
                                                        checked={isSelected}
                                                        onChange={() => handleSelectRow(id)}
                                                    />
                                                </td>

                                                {/* Key */}
                                                <td className="py-3 px-3">
                                                    <div className="fw-semibold text-dark text-break" style={{ fontFamily: "monospace", fontSize: "0.85rem" }}>
                                                        {item.key}
                                                    </div>
                                                </td>

                                                {/* Translated Value */}
                                                <td className="py-3 px-3">
                                                    <div className="p-2.5 rounded-3 bg-light border border-light-subtle text-dark text-break fs-6">
                                                        {item.value}
                                                    </div>

                                                    {/* If rejected, show reason input inline */}
                                                    {decision?.status === 3 && (
                                                        <div className="reject-reason-container mt-2.5">
                                                            <div className="d-flex align-items-center gap-1.5 text-danger small mb-1 fw-medium">
                                                                <ExclamationTriangleIcon width={14} />
                                                                <span>Rejection Reason (Required)</span>
                                                            </div>
                                                            <textarea
                                                                className={`form-control form-control-sm rounded-3 border-danger-subtle ${!decision.rejectReason.trim() ? "is-invalid" : ""}`}
                                                                rows="2"
                                                                placeholder="Please specify why this translation was rejected..."
                                                                value={decision.rejectReason}
                                                                onChange={(e) => handleRejectReasonChange(id, e.target.value)}
                                                            />
                                                            {!decision.rejectReason.trim() && (
                                                                <div className="invalid-feedback">Rejection reason cannot be blank.</div>
                                                            )}
                                                        </div>
                                                    )}
                                                </td>

                                                {/* Decision Buttons */}
                                                <td className="py-3 px-3 text-center">
                                                    <div className="btn-group decision-btn-group rounded-3 shadow-sm" role="group">
                                                        <button
                                                            type="button"
                                                            className={`btn btn-approve d-inline-flex align-items-center gap-1 ${decision?.status === 4 ? "active" : ""}`}
                                                            onClick={() => handleSetDecision(id, 4)}
                                                        >
                                                            <CheckIcon width={14} className="fw-bold" />
                                                            Approve
                                                        </button>
                                                        <button
                                                            type="button"
                                                            className={`btn btn-reject d-inline-flex align-items-center gap-1 ${decision?.status === 3 ? "active" : ""}`}
                                                            onClick={() => handleSetDecision(id, 3)}
                                                        >
                                                            <XMarkIcon width={14} className="fw-bold" />
                                                            Reject
                                                        </button>
                                                        {decision && (
                                                            <button
                                                                type="button"
                                                                className="btn btn-neutral"
                                                                onClick={() => handleSetDecision(id, null)}
                                                                title="Clear decision"
                                                            >
                                                                Reset
                                                            </button>
                                                        )}
                                                    </div>
                                                </td>
                                            </tr>
                                        );
                                    })}
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            )}

            {/* STICKY FOOTER DECISION COMMITTAL BAR */}
            {items.length > 0 && !loading && (
                <div className="sticky-review-bar d-flex flex-wrap align-items-center justify-content-between gap-3 rounded-4">
                    <div className="d-flex align-items-center gap-3">
                        <div className="text-dark fw-medium fs-6">
                            Decisions:{" "}
                            <span className="badge bg-success rounded-pill px-2.5 py-1.5 ms-1">
                                {approvedCount} Approved
                            </span>
                            <span className="badge bg-danger rounded-pill px-2.5 py-1.5 ms-2">
                                {rejectedCount} Rejected
                            </span>
                            <span className="text-muted ms-2 small">
                                ({items.length - totalDecidedCount} remaining)
                            </span>
                        </div>
                    </div>

                    <div className="d-flex align-items-center gap-2">
                        {totalDecidedCount > 0 && (
                            <button
                                className="btn btn-light border rounded-3 fw-medium text-secondary"
                                onClick={handleResetAllDecisions}
                                disabled={submitting}
                            >
                                Clear All
                            </button>
                        )}
                        <button
                            className="btn btn-dark px-4 py-2 rounded-3 fw-medium d-flex align-items-center gap-2 shadow-sm"
                            disabled={totalDecidedCount === 0 || submitting}
                            onClick={handleSubmitReview}
                        >
                            {submitting ? (
                                <>
                                    <div className="spinner-border spinner-border-sm" role="status" />
                                    Submitting Review...
                                </>
                            ) : (
                                <>
                                    Submit Review ({totalDecidedCount})
                                </>
                            )}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default TranslationReviewTab;
