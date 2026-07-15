import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login } from "../../../services/authService";
import { saveToken } from "../../../utils/auth";
import "./Login.css";

export default function Login() {
  const navigate = useNavigate();

  // 1. Tách riêng State của các Input để dễ quản lý và dễ đọc
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  // 2. Tách riêng State quản lý lỗi thành 2 loại rõ ràng:
  // - fieldErrors: Lỗi của từng ô input (VD: để trống, chưa đủ độ dài...)
  // - globalError: Lỗi chung (VD: Sai tài khoản, server sập...)
  const [fieldErrors, setFieldErrors] = useState({});
  const [globalError, setGlobalError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    // 3. Reset toàn bộ lỗi cũ trước khi gửi request mới
    setFieldErrors({});
    setGlobalError("");

    try {
      // 4. Gọi API
      const response = await login({
        username: username,
        password: password,
      });

      // 5. Nếu thành công -> Lưu token và chuyển hướng
      saveToken(
          response.data.accessToken
      );
      //localStorage.setItem("accessToken", response.data.accessToken);
      navigate("/dashboard");

    } catch (error) {
      // 6. Xử lý khi thất bại
      const responseData = error.response?.data;
      console.log(responseData);
      
      // Trường hợp A: Mất mạng hoặc Server không phản hồi
      if (!responseData) {
        setGlobalError("Cannot connect to server. Please try again later.");
        return; // Dừng hàm tại đây
      }

      // Trường hợp B: Lỗi Validation từ FluentValidation (trả về một mảng các lỗi)
      if (responseData.errors && Array.isArray(responseData.errors)) {
        const newFieldErrors = {};

        // Dùng forEach duyệt qua từng lỗi một cách dễ hiểu
        responseData.errors.forEach((err) => {
          // err.Field thường có dạng "Payload.Username" -> Cắt lấy chữ "Username"
          const fieldName = err.Field.split(".").pop();
          
          // Gán thông báo lỗi vào object
          newFieldErrors[fieldName] = err.ErrorMessage;
        });

        // Cập nhật state để hiển thị lỗi đỏ dưới input
        setFieldErrors(newFieldErrors);
      } 
      // Trường hợp C: Lỗi Business logic (Sai mật khẩu, tài khoản bị khoá...)
      else {
        setGlobalError(responseData.errorMessage || "Login failed. Please check your credentials.");
      }
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-left">
          
          {/* Tiêu đề */}
          <div className="text-center mb-4">
            <div className="logo">MS</div>
            <h3 className="fw-bold">MySolution</h3>
            <p className="text-muted">Welcome back</p>
          </div>

          <form onSubmit={handleSubmit}>
            
            {/* Vùng hiển thị Lỗi Chung (Global Error) */}
            {globalError && (
              <div className="alert alert-danger error-animate-shake">
                {globalError}
              </div>
            )}

            {/* Vùng nhập Username */}
            <div className="mb-3">
              <label className="form-label">Username</label>
              <input
                type="text"
                className={`form-control ${fieldErrors.Username ? "is-invalid error-animate-shake" : ""}`}
                placeholder="Enter username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
              {/* Nơi hiển thị lỗi riêng của Username */}
              {fieldErrors.Username && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Username}
                </small>
              )}
            </div>

            {/* Vùng nhập Password */}
            <div className="mb-4">
              <label className="form-label">Password</label>
              <input
                type="password"
                className={`form-control ${fieldErrors.Password ? "is-invalid error-animate-shake" : ""}`}
                placeholder="Enter password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
              />
              {/* Nơi hiển thị lỗi riêng của Password */}
              {fieldErrors.Password && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Password}
                </small>
              )}
            </div>

            {/* Nút Submit */}
            <button type="submit" className="btn btn-warning login-btn w-100">
              Login
            </button>

            {/* Link Đăng ký */}
            <p className="text-center mt-3">
              Don't have an account?{" "}
              <Link to="/register" className="text-warning fw-bold text-decoration-none">
                Register
              </Link>
            </p>
            
          </form>
        </div>

        {/* Banner trang trí bên phải */}
        <div className="login-right">
          <h1>MySolution</h1>
          <h4>Manage your company's</h4>
          <p>User, Role and Permission management platform.</p>
          <div className="login-banner"></div>
        </div>
        
      </div>
    </div>
  );
}