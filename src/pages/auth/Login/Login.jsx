import { useEffect, useState } from "react";
import { useNavigate, Link, useLocation } from "react-router-dom";
import { EyeIcon, EyeSlashIcon } from "@heroicons/react/24/outline";
import { login, resendVerificationEmail, getCurrentUser } from "../../../services/authService";
import { saveToken } from "../../../utils/auth";
import { useAuth } from "../../../contexts/AuthContext";
import { toast } from "react-toastify";
import "./Login.css";

export default function Login() {
  const location = useLocation();
  const navigate = useNavigate();
  const { setUser } = useAuth();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);

  const [fieldErrors, setFieldErrors] = useState({});
  const [globalError, setGlobalError] = useState("");

  useEffect(() => {
    if (location.state?.verified) {
      toast.success("Email verified successfully!");

      navigate("/", {
        replace: true,
        state: null,
      });
    }
  }, [location.state, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();

    setFieldErrors({});
    setGlobalError("");

    try {
      const loginResponse = await login({
        username,
        password,
      });

      saveToken(loginResponse.data.accessToken);

      const me = await getCurrentUser();

      localStorage.setItem(
        "currentUser",
        JSON.stringify(me.data)
      );

      setUser(me.data);

      navigate("/dashboard");
    } catch (error) {
      const responseData = error.response?.data;

      console.log(error);
      console.log(error.response);
      console.log(error.message);

      if (!responseData) {
        setGlobalError("Cannot connect to server. Please try again later.");
        return;
      }

      if (
        responseData.errorMessage === "User is not verified." &&
        responseData.data?.email
      ) {
        const email = responseData.data.email;

        try {
          await resendVerificationEmail(email);

          navigate("/checkemail", {
            state: {
              email: email,
            },
          });

          return;
        } catch (err) {
          console.error(err);
          setGlobalError("Cannot resend verification email.");
          return;
        }
      }

      if (responseData.errors && Array.isArray(responseData.errors)) {
        const newFieldErrors = {};

        responseData.errors.forEach((err) => {
          const fieldName = err.Field.split(".").pop();
          newFieldErrors[fieldName] = err.ErrorMessage;
        });

        setFieldErrors(newFieldErrors);
      } else {
        setGlobalError(responseData.errorMessage || "Login failed.");
      }
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-left">
          <div className="text-center mb-4">
            <div className="logo">MS</div>
            <h3 className="fw-bold">MySolution</h3>
            <p className="text-muted">Welcome back</p>
          </div>

          <form onSubmit={handleSubmit}>
            {globalError && (
              <div className="alert alert-danger error-animate-shake">
                {globalError}
              </div>
            )}

            <div className="mb-3">
              <label className="form-label">Username</label>
              <input
                type="text"
                className={`form-control ${fieldErrors.Username ? "is-invalid error-animate-shake" : ""
                  }`}
                placeholder="Enter username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
              {fieldErrors.Username && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Username}
                </small>
              )}
            </div>

            <div className="mb-4">
              <label className="form-label">Password</label>
              <div className="position-relative">
                <input
                  type={showPassword ? "text" : "password"}
                  className={`form-control pe-5 ${fieldErrors.Password ? "is-invalid error-animate-shake" : ""
                    }`}
                  placeholder="Enter password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
                <button
                  type="button"
                  className="btn btn-link position-absolute top-50 end-0 translate-middle-y text-muted text-decoration-none"
                  onClick={() => setShowPassword(!showPassword)}
                  style={{ zIndex: 10, padding: "0 12px" }}
                  tabIndex="-1"
                >
                  {showPassword ? (
                    <EyeSlashIcon width={20} height={20} />
                  ) : (
                    <EyeIcon width={20} height={20} />
                  )}
                </button>
              </div>

              {fieldErrors.Password && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Password}
                </small>
              )}

              <div className="text-end mt-1">
                <Link
                  to="/forgot-password"
                  className="text-muted text-decoration-none hover-warning"
                >
                  Forgot password?
                </Link>
              </div>
            </div>

            <button type="submit" className="btn btn-warning login-btn w-100">
              Login
            </button>

            <p className="text-center mt-3">
              Don't have an account?{" "}
              <Link
                to="/register"
                className="text-warning fw-bold text-decoration-none"
              >
                Register
              </Link>
            </p>
          </form>
        </div>

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