import TranslationStatusBadge from "./TranslationStatusBadge";
import { getOverallStatus } from "../../helper/calcStatus";
import { getStatusColor } from "../../utils/translationStatus";
import { useTranslationLocks } from "../../hooks/useTranslationLocks";
import { LockClosedIcon } from "@heroicons/react/24/solid";

function TranslationGridRow({
  item,
  languages,
  onEdit,
  onDelete,
  onCellClick,
}) {
  const { locks } = useTranslationLocks();
  const overallStatus = getOverallStatus(item.values);

  return (
    <tr className="bg-white">
      {/* KEY */}
      <td
        className="py-3 px-3 align-middle"
        style={{
          width: "25%",
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
        const valueObj = item.values.find(
          (x) => x.languageId === language.languageId
        );

        const cellColor = getStatusColor(valueObj?.status);
        const valId = valueObj?.translationValueId;
        const activeLock = valId ? locks[valId] : null;

        return (
          <td
            key={language.languageId}
            className={`py-3 align-middle position-relative ${
              activeLock ? "cell-locked-overlay" : ""
            }`}
            style={{
              width: "20%",
              cursor: "pointer",
            }}
            onClick={() =>
              onCellClick({
                translationValueId: valueObj?.translationValueId ?? null,
                translationKeyId: item.translationKeyId,
                languageId: language.languageId,
                languageCode: language.languageCode,
                key: item.key,
                namespaceName: item.namespaceName,
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

              {valueObj ? (
                <span
                  className="text-dark fs-6"
                  style={{
                    wordBreak: "break-word",
                  }}
                >
                  {valueObj.value}
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
      <td className="py-3 text-center align-middle">
        <TranslationStatusBadge status={overallStatus} />
      </td>

      {/* ACTIONS */}
      <td className="py-3 text-center align-middle">
        <div className="btn-group">
          <button
            className="btn btn-sm btn-outline-secondary"
            onClick={() => onEdit(item)}
          >
            Sửa Key
          </button>
          <button
            className="btn btn-sm btn-outline-danger"
            onClick={() => onDelete(item)}
          >
            Xóa
          </button>
        </div>
      </td>
    </tr>
  );
}

export default TranslationGridRow;