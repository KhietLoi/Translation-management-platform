import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { login, resendVerificationEmail } from "../../../services/authService";
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

      const responseData = error.response?.data;

      console.log(responseData);


      if (!responseData) {
        setGlobalError(
          "Cannot connect to server. Please try again later."
        );
        return;
      }


      // User chưa verify
      if (
        responseData.errorMessage === "User is not verified."
        &&
        responseData.data?.email
      ) {

        const email = responseData.data.email;
        try {

          // tự động gửi mail verify
          await resendVerificationEmail(email);


          // chuyển sang màn check email
          navigate("/checkemail", {
            state: {
              email: email
            }
          });


          return;

        } catch (err) {

          console.error(err);

          setGlobalError(
            "Cannot resend verification email."
          );

          return;
        }
      }



      if (
        responseData.errors &&
        Array.isArray(responseData.errors)
      ) {

        const newFieldErrors = {};


        responseData.errors.forEach((err) => {

          const fieldName = err.Field
            .split(".")
            .pop();


          newFieldErrors[fieldName] =
            err.ErrorMessage;

        });


        setFieldErrors(newFieldErrors);

      }
      else {

        setGlobalError(
          responseData.errorMessage ||
          "Login failed."
        );

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
              <div className="text-end mt-1">
                <Link to="/forgot-password" className="text-muted text-decoration-none hover-warning" >Forgot password?</Link>
              </div>

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