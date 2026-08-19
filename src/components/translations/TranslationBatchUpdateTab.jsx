import { useEffect, useState, useRef } from "react";
import { toast } from "react-toastify";
import {
    getBatchTranslationValues,
    batchUpdateTranslations
} from "../../services/translationManagementService";
import { getProjectLanguages } from "../../services/projectService";
import { useAuth } from "../../contexts/AuthContext";
import signalRService, { getAuthUser } from "../../services/signalrService";
import { useTranslationLocks } from "../../hooks/useTranslationLocks";
import {
    ExclamationTriangleIcon,
    CheckIcon,
    ArrowPathIcon
} from "@heroicons/react/24/outline";
import { LockClosedIcon } from "@heroicons/react/24/solid";
import {
    TranslationStatus,
    getStatusText,
    getStatusBadgeClass
} from "../../utils/translationStatus";
import "./TranslationBatchUpdateTab.css";

function TranslationBatchUpdateTab({ projectId, namespaces = [], canUpdate }) {
    const { user } = useAuth();
    const { locks } = useTranslationLocks();

    const auth = getAuthUser(user);
    const currentUserId = auth?.userId || "";
    const currentUserName = auth?.username || "User";

    // Filter Selection State
    const [selectedNamespaceId, setSelectedNamespaceId] = useState("");
    const [selectedLanguageId, setSelectedLanguageId] = useState("");
    const [projectLanguages, setProjectLanguages] = useState([]);

    // Core Data & State
    const [items, setItems] = useState([]);
    const [loading, setLoading] = useState(false);
    const [loadingLanguages, setLoadingLanguages] = useState(false);
    const [submitting, setSubmitting] = useState(false);

    // Local changes: { [translationValueId]: string }
    const [localValues, setLocalValues] = useState({});

    // Checkbox selections for batch update: [translationValueId, ...]
    const [selectedItemIds, setSelectedItemIds] = useState([]);

    // Typing status for other users: { [translationValueId]: username }
    const [typingUsers, setTypingUsers] = useState({});
    const typingTimeoutsRef = useRef({}); // { [translationValueId]: timeoutId }

    const lastTypedRef = useRef({}); // { [translationValueId]: timestamp }
    const typingDebounceTimeoutsRef = useRef({}); // { [translationValueId]: timeoutId }

    // Track previous items to unlock them on switch
    const prevItemsRef = useRef([]);

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

    // Keep namespace in sync
    useEffect(() => {
        if (namespaces.length > 0) {
            const exists = namespaces.some(n => n.id === selectedNamespaceId);
            if (!exists) {
                setSelectedNamespaceId(namespaces[0].id);
            }
        } else {
            setSelectedNamespaceId("");
        }
    }, [namespaces, projectId]);

    // ==========================================
    // EFFECT: Load update items
    // ==========================================
    const loadUpdateItems = async () => {
        if (!projectId || !selectedNamespaceId || !selectedLanguageId) {
            setItems([]);
            setLocalValues({});
            setSelectedItemIds([]);
            return;
        }

        try {
            setLoading(true);
            setSelectedItemIds([]);
            const res = await getBatchTranslationValues({
                projectId,
                namespaceId: selectedNamespaceId,
                languageId: selectedLanguageId
            });
            const list = res.data?.items || res.data || [];
            setItems(list);

            // Populate local inputs state
            const valMap = {};
            list.forEach(item => {
                valMap[item.translationValueId] = item.value ?? "";
            });
            setLocalValues(valMap);
        } catch (error) {
            console.error("Failed to load batch update items:", error);
            toast.error("Failed to load translation values for update");
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        loadUpdateItems();
    }, [projectId, selectedNamespaceId, selectedLanguageId]);

    // ==========================================
    // EFFECT: SignalR Locking Management
    // ==========================================
    useEffect(() => {
        if (!currentUserId) return;

        const currentItems = items || [];
        const prevItems = prevItemsRef.current || [];

        const currentIds = currentItems.map(x => x.translationValueId);
        const prevIds = prevItems.map(x => x.translationValueId);

        // Unlock old translation values
        const toUnlock = prevIds.filter(id => !currentIds.includes(id));
        toUnlock.forEach(id => {
            signalRService.releaseLock(id, currentUserId);
            signalRService.leaveTranslationValueGroup(id);
        });

        // Lock new editable translation values
        const toLock = currentItems.filter(item => {
            const id = item.translationValueId;
            const isNew = !prevIds.includes(id);
            const isImmutableStatus = item.status === TranslationStatus.Reviewed || item.status === TranslationStatus.Published;
            return isNew && !isImmutableStatus;
        });

        toLock.forEach(item => {
            const id = item.translationValueId;
            signalRService.acquireLock(id, currentUserId, currentUserName);
            signalRService.joinTranslationValueGroup(id);
        });

        prevItemsRef.current = currentItems;
    }, [items, currentUserId, currentUserName]);

    // Clean up locks on unmount and browser window reload/close
    useEffect(() => {
        const releaseAllActiveLocks = () => {
            if (!currentUserId) return;
            const finalItems = prevItemsRef.current || [];
            finalItems.forEach(item => {
                signalRService.releaseLock(item.translationValueId, currentUserId);
                signalRService.leaveTranslationValueGroup(item.translationValueId);
            });
        };

        const handleBeforeUnload = () => {
            releaseAllActiveLocks();
        };

        window.addEventListener("beforeunload", handleBeforeUnload);

        return () => {
            window.removeEventListener("beforeunload", handleBeforeUnload);
            releaseAllActiveLocks();
        };
    }, [currentUserId]);

    // ==========================================
    // EFFECT: SignalR Realtime Typing Listeners
    // ==========================================
    useEffect(() => {
        const unsubscribe = signalRService.subscribeTyping((event) => {
            const eventUserId = event?.userId?.toString().toLowerCase();
            const myUserId = currentUserId?.toString().toLowerCase();
            const targetValueId = event?.translationValueId;

            // Sync typing updates from others
            if (targetValueId && eventUserId && myUserId && eventUserId !== myUserId) {
                // Update local inputs state
                if (event.value !== undefined) {
                    setLocalValues(prev => ({
                        ...prev,
                        [targetValueId]: event.value
                    }));
                }

                // Show typing indicator
                setTypingUsers(prev => ({
                    ...prev,
                    [targetValueId]: event.username || "Ai đó"
                }));

                // Auto-clear typing indicator after 3 seconds of silence
                if (typingTimeoutsRef.current[targetValueId]) {
                    clearTimeout(typingTimeoutsRef.current[targetValueId]);
                }
                typingTimeoutsRef.current[targetValueId] = setTimeout(() => {
                    setTypingUsers(prev => {
                        const next = { ...prev };
                        delete next[targetValueId];
                        return next;
                    });
                }, 3000);
            }
        });

        return () => {
            unsubscribe();
            Object.values(typingTimeoutsRef.current).forEach(clearTimeout);
            Object.values(typingDebounceTimeoutsRef.current).forEach(clearTimeout);
        };
    }, [currentUserId]);

    // ==========================================
    // EFFECT: Realtime Auto Refresh Grid on SignalR Notification
    // ==========================================
    useEffect(() => {
        const handleNotification = () => {
            if (projectId && selectedNamespaceId && selectedLanguageId) {
                loadUpdateItems();
            }
        };
        window.addEventListener("translationNotification", handleNotification);
        return () => window.removeEventListener("translationNotification", handleNotification);
    }, [projectId, selectedNamespaceId, selectedLanguageId]);

    // ==========================================
    // TYPING / INPUT CHANGE
    // ==========================================
    const handleInputChange = (id, newVal) => {
        setLocalValues(prev => ({
            ...prev,
            [id]: newVal
        }));

        // Automatically select the checkbox for the modified row if not selected
        if (!selectedItemIds.includes(id)) {
            setSelectedItemIds(prev => [...prev, id]);
        }

        // Throttle SignalR Typing to 200ms
        const now = Date.now();
        const lastTyped = lastTypedRef.current[id] || 0;

        if (now - lastTyped > 200) {
            signalRService.sendTyping(id, newVal);
            lastTypedRef.current[id] = now;
        } else {
            if (typingDebounceTimeoutsRef.current[id]) {
                clearTimeout(typingDebounceTimeoutsRef.current[id]);
            }
            typingDebounceTimeoutsRef.current[id] = setTimeout(() => {
                signalRService.sendTyping(id, newVal);
                lastTypedRef.current[id] = Date.now();
            }, 250);
        }
    };

    // ==========================================
    // CHECKBOX SELECTION
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
            // Select all editable items
            const editableIds = items
                .filter(item => {
                    const id = item.translationValueId;
                    const activeLock = locks[id] || locks[id?.toString()?.toLowerCase()];
                    const isLockedByOther = activeLock && activeLock.userId &&
                        activeLock.userId.toString().toLowerCase() !== currentUserId.toString().toLowerCase();
                    const isImmutableStatus = item.status === TranslationStatus.Reviewed || item.status === TranslationStatus.Published;
                    return !isLockedByOther && !isImmutableStatus;
                })
                .map(item => item.translationValueId);
            setSelectedItemIds(editableIds);
        } else {
            setSelectedItemIds([]);
        }
    };

    const handleDiscardChanges = () => {
        if (selectedItemIds.length === 0) return;
        if (window.confirm(`Discard modifications on ${selectedItemIds.length} selected translation value(s)?`)) {
            setLocalValues(prev => {
                const next = { ...prev };
                selectedItemIds.forEach(id => {
                    const originalItem = items.find(i => i.translationValueId === id);
                    if (originalItem) {
                        next[id] = originalItem.value ?? "";
                    }
                });
                return next;
            });
            toast.info("Changes discarded for selected items");
        }
    };

    // ==========================================
    // BATCH UPDATE SUBMISSION
    // ==========================================
    const handleSubmitBatchUpdate = async (isSubmit = false) => {
        if (selectedItemIds.length === 0) return;

        try {
            setSubmitting(true);

            // Construct payload items with trimmed values
            const payloadItems = selectedItemIds.map(id => ({
                translationValueId: id,
                value: (localValues[id] ?? "").trim()
            }));

            const payload = {
                projectId,
                languageId: selectedLanguageId,
                namespaceId: selectedNamespaceId,
                isSubmit,
                IsSubmit: isSubmit,
                items: payloadItems
            };

            await batchUpdateTranslations(payload);

            toast.success(
                isSubmit
                    ? `Successfully submitted ${selectedItemIds.length} translation(s) for review`
                    : `Successfully saved ${selectedItemIds.length} draft translation(s)`
            );

            // Release locks and leave SignalR groups for updated/submitted items
            selectedItemIds.forEach(id => {
                signalRService.releaseLock(id, currentUserId);
                signalRService.leaveTranslationValueGroup(id);
            });

            // Remove released items from prevItemsRef so loadUpdateItems will re-evaluate lock state if needed
            if (prevItemsRef.current) {
                prevItemsRef.current = prevItemsRef.current.filter(
                    item => !selectedItemIds.includes(item.translationValueId)
                );
            }

            // Dispatch translationNotification to refresh grid data across all listening tabs/components
            window.dispatchEvent(
                new CustomEvent("translationNotification", { detail: { type: "BatchUpdateSuccess" } })
            );

            // Reload grid list
            await loadUpdateItems();
        } catch (error) {
            console.error("Batch update submission failed:", error);
            toast.error(error?.response?.data?.errorMessage || "Failed to submit batch updates.");
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <div className="batch-update-tab-container fade-in">
            {/* FILTER SELECTION CARD */}
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
                                onClick={loadUpdateItems}
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

            {/* MAIN DATA AREA */}
            {!selectedNamespaceId || !selectedLanguageId ? (
                <div className="update-empty-state">
                    <div className="update-empty-state-icon">
                        <ExclamationTriangleIcon width={32} />
                    </div>
                    <h5 className="fw-semibold text-dark mb-1">Select Namespace and Language</h5>
                    <p className="text-muted mb-0">Choose a namespace and target language from the filters above to retrieve translation values for editing.</p>
                </div>
            ) : loading ? (
                <div className="card border-0 shadow-sm">
                    <div className="card-body py-5 text-center text-muted">
                        <div className="spinner-border text-dark spinner-border-sm me-2" role="status" />
                        <span>Loading translation values...</span>
                    </div>
                </div>
            ) : items.length === 0 ? (
                <div className="update-empty-state">
                    <h5 className="fw-semibold text-dark mb-1">No items found</h5>
                    <p className="text-muted mb-0">There are no translation keys configured for this namespace.</p>
                </div>
            ) : (
                <div className="card border-0 shadow-sm mb-4">
                    {/* Toolbar */}
                    <div className="card-header bg-white border-bottom py-3 px-3 d-flex flex-wrap align-items-center justify-content-between gap-3">
                        <div className="d-flex align-items-center gap-2">
                            <div className="form-check fs-6 mb-0">
                                <input
                                    type="checkbox"
                                    className="form-check-input"
                                    id="selectAllHeader"
                                    checked={selectedItemIds.length > 0 && selectedItemIds.length === items.filter(i => {
                                        const id = i.translationValueId;
                                        const activeLock = locks[id] || locks[id?.toString()?.toLowerCase()];
                                        const isLockedByOther = activeLock && activeLock.userId && activeLock.userId.toString().toLowerCase() !== currentUserId.toString().toLowerCase();
                                        const isImmutableStatus = i.status === TranslationStatus.Reviewed || i.status === TranslationStatus.Published;
                                        return !isLockedByOther && !isImmutableStatus;
                                    }).length}
                                    onChange={handleSelectAll}
                                />
                                <label className="form-check-label text-dark fw-medium" htmlFor="selectAllHeader">
                                    Select All Editable ({items.length} total)
                                </label>
                            </div>
                            {selectedItemIds.length > 0 && (
                                <span className="badge bg-dark-subtle text-dark rounded-pill px-2.5 py-1.5 ms-2">
                                    {selectedItemIds.length} Selected
                                </span>
                            )}
                        </div>

                        {selectedItemIds.length > 0 && (
                            <div className="d-flex align-items-center gap-2 fade-in">
                                <button
                                    className="btn btn-sm btn-outline-danger rounded-3"
                                    onClick={handleDiscardChanges}
                                >
                                    Discard Changes
                                </button>
                            </div>
                        )}
                    </div>

                    {/* Table Grid */}
                    <div className="card-body p-0">
                        <div className="table-responsive">
                            <table className="table update-table align-middle mb-0">
                                <thead className="table-light text-uppercase">
                                    <tr>
                                        <th className="py-3 px-3 text-center" style={{ width: "50px" }}></th>
                                        <th className="py-3 px-3" style={{ width: "25%" }}>Key</th>
                                        <th className="py-3 px-3" style={{ width: "120px" }}>Status</th>
                                        <th className="py-3 px-3">Translation Value</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {items.map(item => {
                                        const id = item.translationValueId;
                                        const isSelected = selectedItemIds.includes(id);

                                        // Lock State
                                        const activeLock = locks[id] || locks[id?.toString()?.toLowerCase()];
                                        const isLockedByOther = activeLock && activeLock.userId &&
                                            activeLock.userId.toString().toLowerCase() !== currentUserId.toString().toLowerCase();

                                        const isImmutableStatus = item.status === TranslationStatus.Reviewed || item.status === TranslationStatus.Published;

                                        // Typing State
                                        const typingUser = typingUsers[id];

                                        // CSS Highlight
                                        let rowClass = "update-row";
                                        if (isLockedByOther) rowClass += " update-row-locked";
                                        if (isImmutableStatus) rowClass += " update-row-immutable";
                                        if (isSelected) rowClass += " update-row-selected";

                                        return (
                                            <tr key={id} className={rowClass}>
                                                {/* Checkbox Column */}
                                                <td className="text-center py-3 px-2">
                                                    <input
                                                        type="checkbox"
                                                        className="form-check-input"
                                                        checked={isSelected}
                                                        disabled={isLockedByOther || isImmutableStatus}
                                                        onChange={() => handleSelectRow(id)}
                                                    />
                                                </td>

                                                {/* Key Name */}
                                                <td className="py-3 px-3">
                                                    <div className="fw-semibold text-dark text-break" style={{ fontFamily: "monospace", fontSize: "0.85rem" }}>
                                                        {item.key}
                                                    </div>
                                                </td>

                                                {/* Status Badge */}
                                                <td className="py-3 px-3">
                                                    <span className={`badge ${getStatusBadgeClass(item.status)}`}>
                                                        {getStatusText(item.status)}
                                                    </span>
                                                </td>

                                                {/* Interactive Input Column */}
                                                <td className="py-3 px-3">
                                                    <div className="position-relative">
                                                        <textarea
                                                            rows="2"
                                                            className="form-control form-control-sm rounded-3 py-2 px-3"
                                                            value={localValues[id] ?? ""}
                                                            disabled={!canUpdate || isLockedByOther || isImmutableStatus}
                                                            onChange={(e) => handleInputChange(id, e.target.value)}
                                                            placeholder={isImmutableStatus ? "Already finalized/published (Read-Only)" : "Type translation..."}
                                                        />

                                                        {/* Realtime Typing Notification */}
                                                        {typingUser && (
                                                            <div className="typing-bubble small text-primary d-flex align-items-center gap-1 mt-1">
                                                                <span className="spinner-grow spinner-grow-sm text-primary" role="status" style={{ width: "0.5rem", height: "0.5rem" }} />
                                                                <span className="fst-italic"><strong>{typingUser}</strong> is typing...</span>
                                                            </div>
                                                        )}

                                                        {/* Lock Banner / Indicator */}
                                                        {isLockedByOther && (
                                                            <div className="lock-bubble small text-danger d-flex align-items-center gap-1 mt-1">
                                                                <LockClosedIcon width={12} height={12} className="text-danger flex-shrink-0" />
                                                                <span>Locked by <strong>{activeLock.username || "another user"}</strong> (Read-Only)</span>
                                                            </div>
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

            {/* STICKY BOTTOM COMMITTAL ACTIONS FOOTER */}
            {items.length > 0 && !loading && (
                <div className="sticky-update-bar d-flex flex-wrap align-items-center justify-content-between gap-3 rounded-4">
                    <div className="text-dark fw-medium fs-6">
                        Queue:{" "}
                        <span className="badge bg-dark rounded-pill px-2.5 py-1.5 ms-1">
                            {selectedItemIds.length} to save
                        </span>
                        <span className="text-muted ms-2 small">
                            (Select rows or edit value to add to queue)
                        </span>
                    </div>

                    <div className="d-flex align-items-center gap-2">
                        {selectedItemIds.length > 0 && (
                            <button
                                className="btn btn-light border rounded-3 fw-medium text-secondary"
                                onClick={() => setSelectedItemIds([])}
                                disabled={submitting}
                            >
                                Clear Selection
                            </button>
                        )}
                        <button
                            className="btn btn-outline-dark px-4 py-2 rounded-3 fw-medium d-flex align-items-center gap-2 shadow-sm"
                            disabled={selectedItemIds.length === 0 || submitting}
                            onClick={() => handleSubmitBatchUpdate(false)}
                        >
                            {submitting ? (
                                <>
                                    <div className="spinner-border spinner-border-sm" role="status" />
                                    Saving...
                                </>
                            ) : (
                                <>
                                    Save Draft ({selectedItemIds.length})
                                </>
                            )}
                        </button>
                        <button
                            className="btn btn-dark px-4 py-2 rounded-3 fw-medium d-flex align-items-center gap-2 shadow-sm"
                            disabled={selectedItemIds.length === 0 || submitting}
                            onClick={() => handleSubmitBatchUpdate(true)}
                        >
                            {submitting ? (
                                <>
                                    <div className="spinner-border spinner-border-sm" role="status" />
                                    Submitting...
                                </>
                            ) : (
                                <>
                                    Submit Batch ({selectedItemIds.length})
                                </>
                            )}
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default TranslationBatchUpdateTab;
