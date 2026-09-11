import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { EyeIcon, EyeSlashIcon } from "@heroicons/react/24/outline";
import { register } from "../../../services/authService";
import "./Register.css";

export default function Register() {
  const navigate = useNavigate();

  const [username, setUsername] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");

  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [fieldErrors, setFieldErrors] = useState({});
  const [globalError, setGlobalError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    setFieldErrors({});
    setGlobalError("");

    if (password !== confirmPassword) {
      setFieldErrors({
        confirmPassword: "Passwords do not match!",
      });
      return;
    }

    try {
      const response = await register({
        username: username,
        email: email,
        password: password,
      });

      console.log(response.data);

      navigate("/checkemail", {
        state: {
          email: email,
        },
      });
    } catch (error) {
      const responseData = error.response?.data;

      if (!responseData) {
        setGlobalError("Cannot connect to server. Please try again later.");
        return;
      }

      if (responseData.errors && Array.isArray(responseData.errors)) {
        const newFieldErrors = {};

        responseData.errors.forEach((err) => {
          const fieldName = err.Field.split(".").pop();
          newFieldErrors[fieldName] = err.ErrorMessage;
        });

        setFieldErrors(newFieldErrors);
      } else {
        setGlobalError(
          responseData.errorMessage ||
          responseData.message ||
          "Registration failed."
        );
      }
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-left">
          <div className="text-center mb-4">
            <div className="logo">MS</div>
            <h3 className="fw-bold mt-3">My Solution</h3>
            <p className="text-muted">Register new account</p>
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
                placeholder="Enter your username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
              />
              {fieldErrors.Username && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Username}
                </small>
              )}
            </div>

            <div className="mb-3">
              <label className="form-label">Email</label>
              <input
                type="email"
                className={`form-control ${fieldErrors.Email ? "is-invalid error-animate-shake" : ""
                  }`}
                placeholder="abc@example.com"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
              />
              {fieldErrors.Email && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.Email}
                </small>
              )}
            </div>

            <div className="mb-3">
              <label className="form-label">Password</label>
              <div className="position-relative">
                <input
                  type={showPassword ? "text" : "password"}
                  className={`form-control pe-5 ${fieldErrors.Password ? "is-invalid error-animate-shake" : ""
                    }`}
                  placeholder="Enter your password"
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
            </div>

            <div className="mb-4">
              <label className="form-label">Confirm Password</label>
              <div className="position-relative">
                <input
                  type={showConfirmPassword ? "text" : "password"}
                  className={`form-control pe-5 ${fieldErrors.confirmPassword ? "is-invalid error-animate-shake" : ""
                    }`}
                  placeholder="Confirm your password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
                <button
                  type="button"
                  className="btn btn-link position-absolute top-50 end-0 translate-middle-y text-muted text-decoration-none"
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  style={{ zIndex: 10, padding: "0 12px" }}
                  tabIndex="-1"
                >
                  {showConfirmPassword ? (
                    <EyeSlashIcon width={20} height={20} />
                  ) : (
                    <EyeIcon width={20} height={20} />
                  )}
                </button>
              </div>
              {fieldErrors.confirmPassword && (
                <small className="text-danger error-animate-fade">
                  {fieldErrors.confirmPassword}
                </small>
              )}
            </div>

            <button type="submit" className="btn btn-warning login-btn w-100">
              Register
            </button>

            <p className="text-center mt-3">
              Already have an account?{" "}
              <Link to="/" className="text-warning fw-bold text-decoration-none">
                Login here
              </Link>
            </p>
          </form>
        </div>

        <div className="login-right">
          <h1>Welcome!</h1>
          <h4>Join MySolution</h4>
          <p>Create your account and manage your system easily.</p>
          <div className="line"></div>
          <small>User Management Platform</small>
        </div>
      </div>
    </div>
  );
}