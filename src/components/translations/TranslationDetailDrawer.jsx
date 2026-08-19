import { useEffect, useState, useRef } from "react";
import { toast } from "react-toastify";

import {
  getTranslationValueById,
  updateTranslationValue,
  submitTranslation,
  rejectTranslation,
  reviewTranslation,
} from "../../services/translationManagementService";

import { useAuth } from "../../contexts/AuthContext";
import signalRService, { getAuthUser } from "../../services/signalrService";
import { useTranslationLocks } from "../../hooks/useTranslationLocks";
import { LockClosedIcon } from "@heroicons/react/24/solid";

import {
  TranslationStatus,
  getStatusText,
  getStatusBadgeClass,
  normalizeStatus,
} from "../../utils/translationStatus";

function Info({ label, children }) {
  return (
    <div className="mb-3">
      <div className="fw-semibold text-secondary small">{label}</div>
      <div className="fs-6 text-dark">{children}</div>
    </div>
  );
}

function TranslationDetailDrawer({ show, translationValue, canUpdate: propCanUpdate, onClose, onSuccess }) {
  const { user } = useAuth();
  const { locks } = useTranslationLocks();

  const auth = getAuthUser(user);
  const currentUserId = auth?.userId || "";
  const currentUserName = auth?.username || "User";

  const permissions = user?.permissions ?? [];

  const canUpdate = propCanUpdate !== undefined ? propCanUpdate : permissions.includes("TRANSLATION_UPDATE");
  const canReview = permissions.includes("TRANSLATION_REVIEW");

  const [loading, setLoading] = useState(false);
  const [processing, setProcessing] = useState(false);

  const [detail, setDetail] = useState(null);

  const [value, setValue] = useState("");
  const [reason, setReason] = useState("");

  const [typingUser, setTypingUser] = useState(null);
  const typingTimeoutRef = useRef(null);
  const lastTypedRef = useRef(0);
  const typingDebounceTimeoutRef = useRef(null);

  const valId = translationValue?.translationValueId || translationValue?.id;
  const activeLock = valId ? (locks[valId] || locks[valId?.toString()?.toLowerCase()]) : null;

  // Check if locked by another user
  const isLockedByOther =
    activeLock &&
    activeLock.userId &&
    activeLock.userId.toString().toLowerCase() !== currentUserId.toString().toLowerCase();

  useEffect(() => {
    if (!show || !valId) {
      return;
    }

    loadDetail();

    // 1. Acquire Lock on SignalR Hub
    signalRService.acquireLock(valId, currentUserId, currentUserName);

    // 2. Join the translation value group to receive typing events
    signalRService.joinTranslationValueGroup(valId);

    // 3. Release Lock and leave group when closing or unmounting
    return () => {
      signalRService.releaseLock(valId, currentUserId);
      signalRService.leaveTranslationValueGroup(valId);
    };
  }, [show, valId, currentUserId, currentUserName]);

  useEffect(() => {
    if (!show || !valId) {
      return;
    }

    const unsubscribe = signalRService.subscribeTyping((event) => {
      const eventUserId = event?.userId?.toString().toLowerCase();
      const myUserId = currentUserId?.toString().toLowerCase();

      // Sync only if event is from another user and both IDs are present
      if (eventUserId && myUserId && eventUserId !== myUserId) {
        setTypingUser(event.username || "Ai đó");

        // Sync the value in real-time
        if (event.value !== undefined) {
          setValue(event.value);
        }

        // Reset the timeout to clear typing state
        if (typingTimeoutRef.current) {
          clearTimeout(typingTimeoutRef.current);
        }

        typingTimeoutRef.current = setTimeout(() => {
          setTypingUser(null);
        }, 3000); // 3 seconds timeout
      }
    });

    return () => {
      unsubscribe();
      if (typingTimeoutRef.current) {
        clearTimeout(typingTimeoutRef.current);
      }
      if (typingDebounceTimeoutRef.current) {
        clearTimeout(typingDebounceTimeoutRef.current);
      }
      setTypingUser(null);
    };
  }, [show, valId, currentUserId]);

  // Auto refresh drawer detail on SignalR notification
  useEffect(() => {
    const handleNotification = () => {
      if (show && valId) {
        loadDetail();
      }
    };
    window.addEventListener("translationNotification", handleNotification);
    return () => window.removeEventListener("translationNotification", handleNotification);
  }, [show, valId]);

  const handleTextareaChange = (e) => {
    const newVal = e.target.value;
    setValue(newVal);

    // Clear any previous debounce timer
    if (typingDebounceTimeoutRef.current) {
      clearTimeout(typingDebounceTimeoutRef.current);
    }

    // Send typing notification with value, throttled to once every 200ms
    const now = Date.now();
    if (now - lastTypedRef.current > 200) {
      signalRService.sendTyping(valId, newVal);
      lastTypedRef.current = now;
    } else {
      // Schedule a fallback update to ensure final typed characters are sent
      typingDebounceTimeoutRef.current = setTimeout(() => {
        signalRService.sendTyping(valId, newVal);
        lastTypedRef.current = Date.now();
      }, 250);
    }
  };

  const loadDetail = async () => {
    if (!valId) return;
    try {
      setLoading(true);
      const { data } = await getTranslationValueById(valId);

      setDetail(data);
      setValue(data.value ?? "");
      setReason(data.rejectionReason ?? "");
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  const execute = async (action, successMessage = "Operation successful") => {
    try {
      setProcessing(true);
      await action();
      toast.success(successMessage);
      window.dispatchEvent(
        new CustomEvent("translationNotification", { detail: { type: "TranslationDetailUpdated" } })
      );
      await loadDetail();
      if (onSuccess) {
        await onSuccess();
      }
    } catch (error) {
      console.error(error);
      toast.error(
        error?.response?.data?.errorMessage || "Action failed. Please try again."
      );
    } finally {
      setProcessing(false);
    }
  };

  const targetId = detail?.id || detail?.translationValueId || valId;

  const handleSave = () =>
    execute(
      () =>
        updateTranslationValue(targetId, {
          value,
        }),
      "Translation saved successfully"
    );

  const handleSubmit = () =>
    execute(
      async () => {
        await updateTranslationValue(targetId, { value });
        await submitTranslation(targetId);
      },
      "Translation submitted for review"
    );

  const handleReview = () =>
    execute(
      () => reviewTranslation(targetId),
      "Translation reviewed and approved"
    );

  const handleReject = () =>
    execute(
      () =>
        rejectTranslation(targetId, {
          reason,
        }),
      "Translation rejected"
    );

  if (!show) return null;

  const numStatus = normalizeStatus(detail?.status);

  const canSubmit = [
    TranslationStatus.Missing,
    TranslationStatus.Draft,
    TranslationStatus.Rejected,
  ].includes(numStatus);

  const isImmutable =
    numStatus === TranslationStatus.Reviewed ||
    numStatus === TranslationStatus.Published;

  return (
    <>
      <div
        className="position-fixed top-0 start-0 w-100 h-100"
        style={{
          backgroundColor: "rgba(0,0,0,0.3)",
          zIndex: 1040,
        }}
        onClick={onClose}
      />

      <div
        className="position-fixed top-0 end-0 bg-white shadow"
        style={{
          width: "500px",
          height: "100vh",
          zIndex: 1050,
          overflowY: "auto",
        }}
      >
        <div className="p-4">
          <div className="d-flex justify-content-between align-items-center mb-4">
            <h4 className="mb-0 fw-bold">Translation Detail</h4>
            <button className="btn btn-sm btn-outline-secondary" onClick={onClose}>
              ✕
            </button>
          </div>

          {/* Realtime Lock Alert Banner */}
          {isLockedByOther && (
            <div className="alert alert-danger d-flex align-items-center gap-2 mb-4 py-2.5 px-3 rounded-3 shadow-sm border-danger-subtle">
              <LockClosedIcon width={20} height={20} className="text-danger flex-shrink-0" />
              <div className="small">
                <strong className="d-block">Đang bị khóa chỉnh sửa!</strong>
                Bản dịch này đang được thao tác bởi{" "}
                <span className="fw-bold">{activeLock?.username || "người dùng khác"}</span>.
                Bạn chỉ có thể xem chế độ Read-Only.
              </div>
            </div>
          )}

          {loading ? (
            <div className="p-4 text-center text-muted">
              <div className="spinner-border spinner-border-sm me-2" role="status" />
              <span>Loading details...</span>
            </div>
          ) : (
            detail && (
              <>
                <Info label="Key">{detail.translationKey}</Info>

                <Info label="Namespace">{detail.namespaceName}</Info>

                <Info label="Language">
                  {detail.languageName} ({detail.languageCode})
                </Info>

                <Info label="Status">
                  <span className={`badge ${getStatusBadgeClass(detail.status)}`}>
                    {getStatusText(detail.status)}
                  </span>
                </Info>

                <div className="mb-3">
                  <label className="fw-semibold mb-1">Value</label>
                  <textarea
                    rows="6"
                    className="form-control"
                    value={value}
                    disabled={!canUpdate || isLockedByOther || isImmutable}
                    onChange={handleTextareaChange}
                  />
                  {typingUser && (
                    <div className="text-muted small mt-1 d-flex align-items-center gap-1">
                      <span className="spinner-grow spinner-grow-sm text-primary" role="status" style={{ width: "0.6rem", height: "0.6rem" }} />
                      <span className="fst-italic"><strong>{typingUser}</strong> đang nhập...</span>
                    </div>
                  )}
                </div>

                <div className="mb-3">
                  <label className="fw-semibold mb-1">Reject Reason</label>
                  <textarea
                    rows="3"
                    className="form-control"
                    value={reason}
                    disabled={
                      !canReview ||
                      isLockedByOther ||
                      numStatus !== TranslationStatus.Translated
                    }
                    onChange={(e) => setReason(e.target.value)}
                  />
                </div>

                <Info label="Translated By">{detail.translatedBy ?? "-"}</Info>

                <Info label="Reviewed By">{detail.reviewedBy ?? "-"}</Info>

                <Info label="Translated At">{detail.translatedAt ?? "-"}</Info>

                <Info label="Reviewed At">{detail.reviewedAt ?? "-"}</Info>

                <div className="d-flex gap-2 mt-4 pt-2 border-top">
                  {canUpdate && !isLockedByOther && !isImmutable && (
                    <button
                      className="btn btn-primary"
                      disabled={processing}
                      onClick={handleSave}
                    >
                      Save
                    </button>
                  )}

                  {canUpdate && !isLockedByOther && canSubmit && (
                    <button
                      className="btn btn-warning"
                      disabled={processing}
                      onClick={handleSubmit}
                    >
                      Submit
                    </button>
                  )}

                  {canReview &&
                    !isLockedByOther &&
                    numStatus === TranslationStatus.Translated && (
                      <>
                        <button
                          className="btn btn-success"
                          disabled={processing}
                          onClick={handleReview}
                        >
                          Review
                        </button>
                        <button
                          className="btn btn-danger"
                          disabled={processing}
                          onClick={handleReject}
                        >
                          Reject
                        </button>
                      </>
                    )}
                </div>
              </>
            )
          )}
        </div>
      </div>
    </>
  );
}

export default TranslationDetailDrawer;