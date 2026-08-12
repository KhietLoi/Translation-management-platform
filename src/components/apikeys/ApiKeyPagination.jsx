import React from "react";
import { ChevronLeftIcon, ChevronRightIcon } from "@heroicons/react/24/outline";

function ApiKeyPagination({
  page = 1,
  totalPages = 1,
  totalItems = 0,
  limit = 20,
  onPageChange,
  onLimitChange,
}) {
  const startItem = totalItems === 0 ? 0 : (page - 1) * limit + 1;
  const endItem = Math.min(page * limit, totalItems);

  return (
    <div className="d-flex flex-column flex-md-row justify-content-between align-items-center gap-3 pt-2 pb-4">
      <div className="text-muted" style={{ fontSize: "0.85rem" }}>
        Hiển thị <span className="fw-semibold text-dark">{startItem}</span> -{" "}
        <span className="fw-semibold text-dark">{endItem}</span> trên tổng số{" "}
        <span className="fw-semibold text-dark">{totalItems}</span> API Key
      </div>

      <div className="d-flex align-items-center gap-3">
        {onLimitChange && (
          <div className="d-flex align-items-center gap-2">
            <span className="text-muted" style={{ fontSize: "0.85rem" }}>
              Số dòng:
            </span>
            <select
              className="form-select form-select-sm"
              value={limit}
              onChange={(e) => onLimitChange(Number(e.target.value))}
              style={{ width: "70px", fontSize: "0.85rem" }}
            >
              <option value={10}>10</option>
              <option value={20}>20</option>
              <option value={50}>50</option>
            </select>
          </div>
        )}

        <nav aria-label="API Key Pagination">
          <ul className="pagination pagination-sm mb-0">
            <li className={`page-item ${page <= 1 ? "disabled" : ""}`}>
              <button
                className="page-item-btn btn btn-sm btn-light border me-1 rounded-2"
                onClick={() => onPageChange(page - 1)}
                disabled={page <= 1}
              >
                <ChevronLeftIcon style={{ width: 14, height: 14 }} />
              </button>
            </li>
            <li className="page-item disabled">
              <span className="btn btn-sm btn-white border-0 text-dark fw-medium" style={{ fontSize: "0.85rem" }}>
                Trang {page} / {totalPages || 1}
              </span>
            </li>
            <li className={`page-item ${page >= totalPages ? "disabled" : ""}`}>
              <button
                className="page-item-btn btn btn-sm btn-light border ms-1 rounded-2"
                onClick={() => onPageChange(page + 1)}
                disabled={page >= totalPages}
              >
                <ChevronRightIcon style={{ width: 14, height: 14 }} />
              </button>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  );
}

export default ApiKeyPagination;
