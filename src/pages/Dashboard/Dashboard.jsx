import { useEffect, useState, useCallback } from "react";
import { useAuth } from "../../contexts/AuthContext";
import { useNavigate } from "react-router-dom";
import { getDashboardData } from "../../services/dashboardService";
import { toast } from "react-toastify";
import "./Dashboard.css";

import {
  FolderIcon,
  KeyIcon,
  ChartBarIcon,
  ClockIcon,
  ArrowPathIcon,
  PlusIcon,
  GlobeAltIcon,
  SparklesIcon,
  LanguageIcon,
  CheckCircleIcon,
} from "@heroicons/react/24/outline";

export default function Dashboard() {
  const { user } = useAuth();
  const navigate = useNavigate();

  const [loading, setLoading] = useState(true);
  const [refreshing, setRefreshing] = useState(false);
  const [dashboardData, setDashboardData] = useState(null);
  const [error, setError] = useState(null);

  const fetchDashboard = useCallback(async (isSilent = false) => {
    try {
      if (!isSilent) setLoading(true);
      else setRefreshing(true);

      const response = await getDashboardData();
      const data = response?.data || response;

      setDashboardData(data);
      setError(null);
    } catch (err) {
      return;

    } finally {
      setLoading(false);
      setRefreshing(false);
    }
  }, []);

  useEffect(() => {
    fetchDashboard();

    // Listen for realtime translation events across the app
    const handleNotification = () => {
      fetchDashboard(true);
    };

    window.addEventListener("translationNotification", handleNotification);
    return () =>
      window.removeEventListener("translationNotification", handleNotification);
  }, [fetchDashboard]);

  const summary = dashboardData?.dashboardSummary || {
    totalProjects: 0,
    totalTranslationKeys: 0,
    translationProgress: 0,
    pendingReview: 0,
  };

  const languageProgress = dashboardData?.languageProgress || [];
  const recentActivities = dashboardData?.recentActivities || [];

  // Helper for language name mapping
  const getLanguageName = (code, backendName) => {
    if (backendName && backendName.trim()) return backendName;
    if (!code) return "Ngôn ngữ";
    const lower = code.toLowerCase();
    if (lower.includes("vi")) return "Tiếng Việt";
    if (lower.includes("en")) return "Tiếng Anh";
    if (lower.includes("ja") || lower.includes("jp")) return "Tiếng Nhật";
    if (lower.includes("ko") || lower.includes("kr")) return "Tiếng Hàn";
    if (lower.includes("zh") || lower.includes("cn")) return "Tiếng Trung";
    if (lower.includes("fr")) return "Tiếng Pháp";
    if (lower.includes("de")) return "Tiếng Đức";
    if (lower.includes("th")) return "Tiếng Thái";
    return code;
  };

  // Helper for relative time
  const formatRelativeTime = (dateString) => {
    if (!dateString) return "Vừa xong";
    try {
      const date = new Date(dateString);
      const now = new Date();
      const diffSec = Math.floor((now - date) / 1000);
      if (diffSec < 60) return "Vừa xong";
      const diffMin = Math.floor(diffSec / 60);
      if (diffMin < 60) return `${diffMin} phút trước`;
      const diffHour = Math.floor(diffMin / 60);
      if (diffHour < 24) return `${diffHour} giờ trước`;
      const diffDay = Math.floor(diffHour / 24);
      if (diffDay < 30) return `${diffDay} ngày trước`;
      return date.toLocaleDateString("vi-VN");
    } catch {
      return dateString;
    }
  };

  // Helper for action metadata
  const getActionMeta = (action, entityName) => {
    const entity =
      entityName === "TranslationValue"
        ? "Bản dịch"
        : entityName === "TranslationKey"
          ? "Key dịch"
          : entityName === "Project"
            ? "Dự án"
            : entityName || "mục";

    const actNum = Number(action);
    switch (actNum) {
      case 1:
        return { text: `đã tạo ${entity} mới`, color: "#10b981" };
      case 2:
        return { text: `đã cập nhật ${entity}`, color: "#3b82f6" };
      case 3:
        return { text: `đã xóa ${entity}`, color: "#ef4444" };
      case 4:
        return { text: `đã duyệt ${entity}`, color: "#059669" };
      case 5:
        return { text: `đã nộp ${entity} để review`, color: "#f59e0b" };
      case 6:
        return { text: `đã xuất bản ${entity}`, color: "#8b5cf6" };
      case 7:
        return { text: `đã từ chối ${entity}`, color: "#dc2626" };
      default:
        return {
          text:
            typeof action === "string"
              ? action
              : `đã thao tác trên ${entity}`,
          color: "#6b7280",
        };
    }
  };

  // Progress bar color generator
  const getProgressColor = (percent) => {
    if (percent >= 100) return "#10b981"; // Emerald
    if (percent >= 80) return "#0d9488"; // Teal
    if (percent >= 50) return "#f59e0b"; // Amber
    return "#ef4444"; // Red
  };

  const currentDateText = new Date().toLocaleDateString("vi-VN", {
    weekday: "long",
    year: "numeric",
    month: "long",
    day: "numeric",
  });

  return (
    <div className="dashboard-container py-4 px-3 px-md-4">
      {/* WELCOME HEADER */}
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div>
          <h2 className="fw-bold mb-1 fs-3 text-dark d-flex align-items-center gap-2">
            <span>Chào {user?.userName || user?.name || "Bạn"}</span>
            <SparklesIcon width={24} className="text-warning" />
          </h2>
          <p className="text-muted mb-0 fs-6">
            Tổng quan tình hình dịch thuật hôm nay &bull;{" "}
            <span className="text-capitalize">{currentDateText}</span>
          </p>
        </div>

        <div className="d-flex align-items-center gap-2">
          <button
            className="btn btn-outline-secondary px-3 py-2 fw-semibold d-flex align-items-center gap-2 rounded-3 bg-white shadow-sm"
            onClick={() => fetchDashboard(true)}
            disabled={refreshing || loading}
          >
            <ArrowPathIcon
              width={18}
              className={refreshing ? "spin-animation" : ""}
            />
            <span>Làm mới</span>
          </button>

          <button
            className="btn btn-primary px-3.5 py-2 fw-semibold d-flex align-items-center gap-2 rounded-3 shadow-sm"
            onClick={() => navigate("/projects")}
          >
            <PlusIcon width={18} />
            <span>Dự án mới</span>
          </button>
        </div>
      </div>

      {/* ERROR ALERT */}
      {error && (
        <div className="alert alert-danger d-flex align-items-center justify-content-between mb-4 rounded-3 shadow-sm">
          <span>{error}</span>
          <button
            className="btn btn-sm btn-outline-danger"
            onClick={() => fetchDashboard()}
          >
            Thử lại
          </button>
        </div>
      )}

      {/* TOP 4 STAT CARDS */}
      <div className="row g-3.5 mb-4">
        {/* CARD 1: TOTAL PROJECTS */}
        <div className="col-12 col-sm-6 col-xl-3">
          <div className="dashboard-stat-card p-4">
            {loading ? (
              <div className="d-flex flex-column gap-2">
                <div className="skeleton-pulse" style={{ height: "16px", width: "60%" }} />
                <div className="skeleton-pulse" style={{ height: "36px", width: "40%" }} />
              </div>
            ) : (
              <div className="d-flex align-items-center justify-content-between">
                <div>
                  <span className="text-secondary small fw-medium d-block mb-1">
                    Tổng số dự án
                  </span>
                  <div className="fs-2 fw-bold text-dark lh-1">
                    {summary.totalProjects?.toLocaleString() ?? 0}
                  </div>
                </div>
                <div className="stat-icon-wrapper stat-icon-blue">
                  <FolderIcon width={24} height={24} />
                </div>
              </div>
            )}
          </div>
        </div>

        {/* CARD 2: TOTAL TRANSLATION KEYS */}
        <div className="col-12 col-sm-6 col-xl-3">
          <div className="dashboard-stat-card p-4">
            {loading ? (
              <div className="d-flex flex-column gap-2">
                <div className="skeleton-pulse" style={{ height: "16px", width: "60%" }} />
                <div className="skeleton-pulse" style={{ height: "36px", width: "40%" }} />
              </div>
            ) : (
              <div className="d-flex align-items-center justify-content-between">
                <div>
                  <span className="text-secondary small fw-medium d-block mb-1">
                    Tổng Translation Key
                  </span>
                  <div className="fs-2 fw-bold text-dark lh-1">
                    {summary.totalTranslationKeys?.toLocaleString() ?? 0}
                  </div>
                </div>
                <div className="stat-icon-wrapper stat-icon-indigo">
                  <KeyIcon width={24} height={24} />
                </div>
              </div>
            )}
          </div>
        </div>

        {/* CARD 3: TRANSLATION PROGRESS */}
        <div className="col-12 col-sm-6 col-xl-3">
          <div className="dashboard-stat-card p-4">
            {loading ? (
              <div className="d-flex flex-column gap-2">
                <div className="skeleton-pulse" style={{ height: "16px", width: "60%" }} />
                <div className="skeleton-pulse" style={{ height: "36px", width: "40%" }} />
              </div>
            ) : (
              <div className="d-flex align-items-center justify-content-between">
                <div>
                  <span className="text-secondary small fw-medium d-block mb-1">
                    Tiến độ hoàn thành
                  </span>
                  <div className="fs-2 fw-bold text-dark lh-1 d-flex align-items-baseline gap-1">
                    <span>{Math.round(summary.translationProgress ?? 0)}%</span>
                  </div>
                </div>
                <div className="stat-icon-wrapper stat-icon-emerald">
                  <ChartBarIcon width={24} height={24} />
                </div>
              </div>
            )}
          </div>
        </div>

        {/* CARD 4: PENDING REVIEW */}
        <div className="col-12 col-sm-6 col-xl-3">
          <div className="dashboard-stat-card p-4">
            {loading ? (
              <div className="d-flex flex-column gap-2">
                <div className="skeleton-pulse" style={{ height: "16px", width: "60%" }} />
                <div className="skeleton-pulse" style={{ height: "36px", width: "40%" }} />
              </div>
            ) : (
              <div className="d-flex align-items-center justify-content-between">
                <div>
                  <span className="text-secondary small fw-medium d-block mb-1">
                    Chờ review
                  </span>
                  <div className="fs-2 fw-bold text-dark lh-1 d-flex align-items-center gap-2">
                    <span>{summary.pendingReview?.toLocaleString() ?? 0}</span>
                    {summary.pendingReview > 0 && (
                      <span className="badge bg-warning-subtle text-warning-emphasis fs-6 fw-semibold px-2 py-0.5 rounded-pill">
                        Cần duyệt
                      </span>
                    )}
                  </div>
                </div>
                <div className="stat-icon-wrapper stat-icon-amber">
                  <ClockIcon width={24} height={24} />
                </div>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* BOTTOM SECTION: LANGUAGE PROGRESS & RECENT ACTIVITY */}
      <div className="row g-4">
        {/* LEFT COLUMN: LANGUAGE PROGRESS */}
        <div className="col-12 col-lg-7">
          <div className="dashboard-card h-100 p-4">
            <div className="d-flex align-items-center justify-content-between mb-4 border-bottom pb-3">
              <div className="d-flex align-items-center gap-2">
                <GlobeAltIcon width={22} className="text-primary" />
                <h5 className="fw-bold mb-0 text-dark fs-6">
                  Tiến độ theo ngôn ngữ
                </h5>
              </div>
              <span className="badge bg-light text-secondary border fw-medium px-2.5 py-1">
                {languageProgress.length} Ngôn ngữ
              </span>
            </div>

            {loading ? (
              <div className="d-flex flex-column gap-3 py-2">
                {[1, 2, 3].map((n) => (
                  <div key={n} className="skeleton-pulse" style={{ height: "32px" }} />
                ))}
              </div>
            ) : languageProgress.length === 0 ? (
              <div className="text-center py-5 text-muted">
                <LanguageIcon width={36} className="opacity-50 mb-2" />
                <p className="mb-0">Chưa có dữ liệu tiến độ ngôn ngữ.</p>
              </div>
            ) : (
              <div className="d-flex flex-column gap-4">
                {languageProgress.map((lang, idx) => {
                  const percent = Math.min(100, Math.max(0, Math.round(lang.progress ?? 0)));
                  const barColor = getProgressColor(percent);
                  const name = getLanguageName(lang.languageCode, lang.languageName);

                  return (
                    <div key={lang.languageId || idx} className="d-flex flex-column gap-1.5">
                      <div className="d-flex justify-content-between align-items-center small">
                        <div className="d-flex align-items-center gap-2">
                          <span className="badge bg-dark-subtle text-dark font-monospace px-2 py-0.5">
                            {lang.languageCode}
                          </span>
                          <span className="fw-semibold text-dark">{name}</span>
                        </div>
                        <span className="fw-bold text-dark">{percent}%</span>
                      </div>

                      <div
                        className="progress"
                        style={{
                          height: "10px",
                          backgroundColor: "#f1f5f9",
                          borderRadius: "9999px",
                        }}
                      >
                        <div
                          className="progress-bar rounded-pill"
                          role="progressbar"
                          style={{
                            width: `${percent}%`,
                            backgroundColor: barColor,
                            transition: "width 0.6s cubic-bezier(0.4, 0, 0.2, 1)",
                          }}
                          aria-valuenow={percent}
                          aria-valuemin="0"
                          aria-valuemax="100"
                        />
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </div>

        {/* RIGHT COLUMN: RECENT ACTIVITIES */}
        <div className="col-12 col-lg-5">
          <div className="dashboard-card h-100 p-4">
            <div className="d-flex align-items-center justify-content-between mb-4 border-bottom pb-3">
              <div className="d-flex align-items-center gap-2">
                <ClockIcon width={22} className="text-primary" />
                <h5 className="fw-bold mb-0 text-dark fs-6">
                  Hoạt động gần đây
                </h5>
              </div>
              <button
                className="btn btn-sm btn-link text-decoration-none text-muted p-0"
                onClick={() => navigate("/translations")}
              >
                Xem tất cả bản dịch &rarr;
              </button>
            </div>

            {loading ? (
              <div className="d-flex flex-column gap-3 py-2">
                {[1, 2, 3, 4].map((n) => (
                  <div key={n} className="skeleton-pulse" style={{ height: "48px" }} />
                ))}
              </div>
            ) : recentActivities.length === 0 ? (
              <div className="text-center py-5 text-muted">
                <CheckCircleIcon width={36} className="opacity-50 mb-2" />
                <p className="mb-0">Chưa có hoạt động nào gần đây.</p>
              </div>
            ) : (
              <div className="d-flex flex-column">
                {recentActivities.map((act, idx) => {
                  const meta = getActionMeta(act.action, act.entityName);
                  const initial = (act.actorName || "A").charAt(0).toUpperCase();

                  return (
                    <div key={act.id || idx} className="timeline-item">
                      <div
                        className="timeline-dot"
                        style={{ backgroundColor: meta.color }}
                      />
                      <div className="d-flex align-items-start gap-2.5">
                        <div className="actor-avatar flex-shrink-0">
                          {initial}
                        </div>
                        <div className="flex-grow-1 min-w-0">
                          <div className="small text-dark lh-sm">
                            <strong className="fw-semibold text-dark">
                              {act.actorName || "Người dùng"}
                            </strong>{" "}
                            <span className="text-secondary">{meta.text}</span>
                            {act.projectName && (
                              <span className="badge bg-light text-dark border ms-1 fw-normal">
                                {act.projectName}
                              </span>
                            )}
                          </div>
                          <div className="text-muted small mt-1" style={{ fontSize: "0.75rem" }}>
                            {formatRelativeTime(act.createdAt)}
                          </div>
                        </div>
                      </div>
                    </div>
                  );
                })}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}