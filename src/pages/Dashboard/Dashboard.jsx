import { useAuth } from "../../contexts/AuthContext";
import { useNavigate } from "react-router-dom";
import { PlusIcon } from "@heroicons/react/24/outline";

export default function Dashboard() {
  const { user } = useAuth();
  const navigate = useNavigate();

  // Mock data matching exact user screenshot
  const stats = [
    {
      label: "Tổng số dự án",
      value: "12",
      badge: "+2",
      badgeType: "success",
    },
    {
      label: "Tổng số Translation Key",
      value: "6,482",
      badge: "+184",
      badgeType: "success",
    },
    {
      label: "Tiến độ dịch",
      value: "87%",
      badge: "+3%",
      badgeType: "success",
    },
    {
      label: "Chờ review",
      value: "96",
      badge: "cần chú ý",
      badgeType: "warning",
    },
  ];

  const languageProgress = [
    { code: "en-US", percent: 98, color: "#0d9488" },
    { code: "vi-VN", percent: 100, color: "#0d9488" },
    { code: "ja-JP", percent: 74, color: "#b45309" },
    { code: "ko-KR", percent: 61, color: "#b45309" },
    { code: "th-TH", percent: 32, color: "#dc2626" },
  ];

  const recentActivities = [
    {
      user: "Minh Khoa",
      action: "đã publish version v1.8.2 cho",
      target: "WePay Mobile App",
      time: "8 phút trước",
      dotColor: "#10b981",
    },
    {
      user: "Thu Hà",
      action: "đã import 214 key từ",
      target: "ko.xlsx",
      time: "42 phút trước",
      dotColor: "#10b981",
    },
    {
      user: "Bảo Long",
      action: "sửa giá trị",
      target: "checkout.confirm_btn (ja-JP)",
      time: "1 giờ trước",
      dotColor: "#d97706",
    },
    {
      user: "API key prod-mobile-01",
      action: "sắp hết hạn trong",
      target: "3 ngày",
      time: "2 giờ trước",
      dotColor: "#dc2626",
    },
  ];

  return (
    <div className="container-fluid py-3 px-2 px-md-4">
      {/* WELCOME HEADER */}
      <div className="d-flex justify-content-between align-items-center mb-4 flex-wrap gap-3">
        <div>
          <h2 className="fw-bold mb-1 fs-3 text-dark">
            Chào {user?.userName || "Tran An"} 👋
          </h2>
          <p className="text-muted mb-0 fs-6">
            Đây là tình hình dịch thuật của workspace hôm nay, 22 Thg 7.
          </p>
        </div>

        <button
          className="btn btn-dark px-3 py-2 fw-semibold d-flex align-items-center gap-1 rounded-3"
          style={{ backgroundColor: "#111827", borderColor: "#111827" }}
          onClick={() => navigate("/projects")}
        >
          <PlusIcon width={18} />
          + Dự án mới
        </button>
      </div>

      {/* TOP 4 STAT CARDS */}
      <div className="row g-3 mb-4">
        {stats.map((stat, idx) => (
          <div key={idx} className="col-12 col-sm-6 col-xl-3">
            <div className="card border border-light-subtle shadow-sm rounded-4 h-100 bg-white">
              <div className="card-body p-3 p-md-4 d-flex flex-column justify-content-between">
                <div className="d-flex justify-content-between align-items-start mb-2">
                  <span className="text-secondary small fw-medium">
                    {stat.label}
                  </span>
                  <span
                    className={`badge rounded-pill px-2 py-1 fw-semibold ${
                      stat.badgeType === "success"
                        ? "bg-success-subtle text-success"
                        : "bg-warning-subtle text-warning-emphasis"
                    }`}
                    style={{ fontSize: "0.75rem" }}
                  >
                    {stat.badge}
                  </span>
                </div>

                <div className="fs-2 fw-bold text-dark">{stat.value}</div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* BOTTOM ROW: LANGUAGE PROGRESS & RECENT ACTIVITY */}
      <div className="row g-4">
        {/* LANGUAGE PROGRESS CARD */}
        <div className="col-12 col-lg-7">
          <div className="card border border-light-subtle shadow-sm rounded-4 h-100 bg-white">
            <div className="card-header bg-white border-0 pt-4 px-4 pb-2">
              <h5 className="fw-bold mb-0 text-dark fs-6">
                Tiến độ theo ngôn ngữ
              </h5>
            </div>

            <div className="card-body px-4 pb-4 pt-2">
              <div className="d-flex flex-column gap-3">
                {languageProgress.map((lang, idx) => (
                  <div key={idx} className="d-flex align-items-center gap-3">
                    <span
                      className="fw-semibold text-dark font-monospace"
                      style={{ width: "55px", fontSize: "0.85rem" }}
                    >
                      {lang.code}
                    </span>

                    <div className="flex-grow-1">
                      <div
                        className="progress"
                        style={{
                          height: "8px",
                          backgroundColor: "#f3f4f6",
                          borderRadius: "9999px",
                        }}
                      >
                        <div
                          className="progress-bar rounded-pill"
                          role="progressbar"
                          style={{
                            width: `${lang.percent}%`,
                            backgroundColor: lang.color,
                          }}
                          aria-valuenow={lang.percent}
                          aria-valuemin="0"
                          aria-valuemax="100"
                        />
                      </div>
                    </div>

                    <span
                      className="fw-bold text-dark text-end"
                      style={{ width: "40px", fontSize: "0.85rem" }}
                    >
                      {lang.percent}%
                    </span>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>

        {/* RECENT ACTIVITY CARD */}
        <div className="col-12 col-lg-5">
          <div className="card border border-light-subtle shadow-sm rounded-4 h-100 bg-white">
            <div className="card-header bg-white border-0 pt-4 px-4 pb-2">
              <h5 className="fw-bold mb-0 text-dark fs-6">Hoạt động gần đây</h5>
            </div>

            <div className="card-body px-4 pb-4 pt-2">
              <div className="d-flex flex-column gap-3">
                {recentActivities.map((act, idx) => (
                  <div
                    key={idx}
                    className={`pb-3 ${
                      idx !== recentActivities.length - 1 ? "border-bottom" : ""
                    }`}
                  >
                    <div className="d-flex align-items-start gap-2 mb-1">
                      <span
                        className="rounded-circle mt-1 flex-shrink-0"
                        style={{
                          width: "8px",
                          height: "8px",
                          backgroundColor: act.dotColor,
                        }}
                      />
                      <div className="small text-dark lh-sm">
                        <strong className="fw-semibold">{act.user}</strong>{" "}
                        <span>{act.action}</span>{" "}
                        <span className="fw-medium text-dark">{act.target}</span>
                      </div>
                    </div>
                    <div
                      className="text-muted ms-3"
                      style={{ fontSize: "0.75rem" }}
                    >
                      {act.time}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}