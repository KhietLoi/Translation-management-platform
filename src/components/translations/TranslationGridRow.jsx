import { useState, useEffect } from "react";
import TranslationStatusBadge from "./TranslationStatusBadge";
import { getOverallStatus } from "../../helper/calcStatus";
import { getStatusColor } from "../../utils/translationStatus";
import { useTranslationLocks } from "../../hooks/useTranslationLocks";
import signalRService from "../../services/signalrService";
import { LockClosedIcon } from "@heroicons/react/24/solid";

function TranslationGridRow({
  item,
  languages,
  onEdit,
  onDelete,
  onCellClick,
  showActions,
}) {
  const { locks } = useTranslationLocks();
  const [liveValues, setLiveValues] = useState({});

  // Reset liveValues when fresh item data arrives from server
  useEffect(() => {
    setLiveValues({});
  }, [item]);

  useEffect(() => {
    const unsubscribe = signalRService.subscribeTyping((event) => {
      const valId = event?.translationValueId;
      if (valId && event.value !== undefined) {
        const lowerId = valId.toString().toLowerCase();
        setLiveValues((prev) => ({
          ...prev,
          [valId]: event.value,
          [lowerId]: event.value,
        }));
      }
    });

    return () => unsubscribe();
  }, []);

  const overallStatus = getOverallStatus(item.values);
  const hasActions = showActions ?? Boolean(onEdit || onDelete);

  return (
    <tr className="bg-white">
      {/* KEY */}
      <td
        className="py-3 px-3 align-middle"
        style={{
          minWidth: "200px",
        }}
      >
        <div
          className="fw-semibold text-dark fs-6"
          style={{
            fontFamily: "monospace",
          }}
        >
          {item.key}
        </div>

        <small className="text-muted opacity-75">{item.namespaceName}</small>
      </td>

      {/* LANGUAGES */}
      {languages.map((language) => {
        const langId = language.languageId || language.LanguageId || language.id || language.Id;
        const valList = item.values || item.Values || [];
        const valueObj = valList.find(
          (x) => (x.languageId || x.LanguageId) === langId
        );

        const valStatus = valueObj ? (valueObj.status ?? valueObj.Status) : undefined;
        const cellColor = getStatusColor(valStatus);
        const valId = valueObj ? (valueObj.translationValueId || valueObj.TranslationValueId || valueObj.id || valueObj.Id) : null;
        const cellVal = valueObj ? (valueObj.value ?? valueObj.Value) : null;

        const activeLock = valId ? (locks[valId] || locks[valId?.toString()?.toLowerCase()]) : null;
        const displayValue = valId ? (liveValues[valId] ?? liveValues[valId?.toString()?.toLowerCase()] ?? cellVal) : cellVal;

        return (
          <td
            key={langId}
            className={`py-3 align-middle position-relative ${activeLock ? "cell-locked-overlay" : ""
              }`}
            style={{
              minWidth: "150px",
              cursor: "pointer",
            }}
            onClick={() =>
              onCellClick({
                translationValueId: valId,
                translationKeyId: item.translationKeyId || item.TranslationKeyId || item.id || item.Id,
                languageId: langId,
                languageCode: language.languageCode || language.LanguageCode || language.code || language.Code,
                key: item.key || item.Key,
                namespaceName: item.namespaceName || item.NamespaceName,
              })
            }
          >
            <div
              className={`border-start border-3 border-${cellColor} ps-3 py-1`}
            >
              {activeLock && (
                <div className="translation-lock-badge mb-1">
                  <LockClosedIcon width={12} height={12} />
                  <span>{activeLock.username || activeLock.userId || "Locked"}</span>
                </div>
              )}

              {displayValue !== undefined && displayValue !== null ? (
                <span
                  className="text-dark fs-6"
                  style={{
                    wordBreak: "break-word",
                  }}
                >
                  {displayValue}
                </span>
              ) : (
                <span className="text-muted fst-italic opacity-50">
                  - chưa dịch -
                </span>
              )}
            </div>
          </td>
        );
      })}

      {/* OVERALL STATUS */}
      <td className="py-3 text-center align-middle" style={{ width: '1%', whiteSpace: 'nowrap' }}>
        <TranslationStatusBadge status={overallStatus} />
      </td>

      {/* ACTIONS */}
      {hasActions && (
        <td className="py-3 text-center align-middle" style={{ width: '1%', whiteSpace: 'nowrap' }}>
          <div className="btn-group">
            {onEdit && (
              <button
                className="btn btn-sm btn-outline-secondary"
                onClick={() => onEdit(item)}
              >
                Update
              </button>
            )}

            {onDelete && (
              <button
                className="btn btn-sm btn-outline-danger"
                onClick={() => onDelete(item)}
              >
                Delete
              </button>
            )}
          </div>
        </td>
      )}
    </tr>
  );
}

export default TranslationGridRow;