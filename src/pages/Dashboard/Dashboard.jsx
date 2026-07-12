export default function Dashboard() {
  return (
    <div className="container-fluid">

    

      {/* Statistics */}
      <div className="row g-4 mb-4">

        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body">
              <div className="text-muted mb-2">
                Total Users
              </div>

              <h2 className="fw-bold text-dark">
                1,245
              </h2>

              <small className="text-success">
                +24 người dùng mới
              </small>
            </div>
          </div>
        </div>

        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body">
              <div className="text-muted mb-2">
                Roles
              </div>

              <h2 className="fw-bold text-dark">
                8
              </h2>

              <small style={{ color: "#ffc107" }}>
                Nhóm quyền hệ thống
              </small>
            </div>
          </div>
        </div>

        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body">
              <div className="text-muted mb-2">
                Permissions
              </div>

              <h2 className="fw-bold text-dark">
                42
              </h2>

              <small className="text-primary">
                Quyền đang hoạt động
              </small>
            </div>
          </div>
        </div>

        <div className="col-md-6 col-xl-3">
          <div className="card border-0 shadow-sm rounded-4 h-100">
            <div className="card-body">
              <div className="text-muted mb-2">
                Online Users
              </div>

              <h2 className="fw-bold text-dark">
                36
              </h2>

              <small className="text-success">
                Đang trực tuyến
              </small>
            </div>
          </div>
        </div>

      </div>

      {/* Content */}
      <div className="row g-4">

        {/* Recent Activities */}
        <div className="col-lg-8">

          <div className="card border-0 shadow-sm rounded-4">

            <div
              className="card-header bg-white"
              style={{
                borderBottom: "3px solid #ffc107",
              }}
            >
              <h5 className="fw-bold mb-0">
                Recent Activities
              </h5>
            </div>

            <div className="card-body">

              <div className="border-bottom py-3">
                <strong>Nguyễn Văn A</strong>
                <span> đã đăng nhập vào hệ thống</span>

                <small className="d-block text-muted mt-1">
                  2 phút trước
                </small>
              </div>

              <div className="border-bottom py-3">
                <strong>Admin</strong>
                <span> đã tạo Role Manager</span>

                <small className="d-block text-muted mt-1">
                  15 phút trước
                </small>
              </div>

              <div className="border-bottom py-3">
                <strong>Trần Văn B</strong>
                <span> đã thay đổi mật khẩu</span>

                <small className="d-block text-muted mt-1">
                  1 giờ trước
                </small>
              </div>

              <div className="py-3">
                <strong>System</strong>
                <span> đã dọn dẹp Refresh Token hết hạn</span>

                <small className="d-block text-muted mt-1">
                  Hôm qua
                </small>
              </div>

            </div>

          </div>

        </div>

        {/* Security Status */}
        <div className="col-lg-4">

          <div className="card border-0 shadow-sm rounded-4">

            <div
              className="card-header bg-white"
              style={{
                borderBottom: "3px solid #ffc107",
              }}
            >
              <h5 className="fw-bold mb-0">
                Security Status
              </h5>
            </div>

            <div className="card-body">

              <div className="d-flex justify-content-between mb-3">
                <span>JWT Authentication</span>

                <span
                  className="badge"
                  style={{
                    background: "#ffc107",
                    color: "#000",
                  }}
                >
                  Enabled
                </span>
              </div>

              <div className="d-flex justify-content-between mb-3">
                <span>Refresh Token</span>

                <span
                  className="badge"
                  style={{
                    background: "#ffc107",
                    color: "#000",
                  }}
                >
                  Active
                </span>
              </div>

              <div className="d-flex justify-content-between mb-3">
                <span>Expired Tokens</span>

                <span className="badge bg-danger">
                  12
                </span>
              </div>

              <div className="d-flex justify-content-between mb-3">
                <span>Failed Login</span>

                <span className="badge bg-dark">
                  5
                </span>
              </div>

              <hr />

              <div className="d-flex justify-content-between">
                <span>System Health</span>

                <span className="text-success fw-bold">
                  Healthy
                </span>
              </div>

            </div>

          </div>

        </div>

      </div>

    </div>
  );
}