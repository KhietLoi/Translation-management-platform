import { useState } from "react";
import { useNavigate, Link} from "react-router-dom";
import { login } from "../../services/authService";
import "./Login.css";

export default function Login() {

  const navigate = useNavigate();
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {

      const response = await login({
        username,
        password,
      });

      localStorage.setItem(
        "accessToken",
        response.data.accessToken
      );

      navigate("/dashboard");

    } catch (error) {

      console.error(error);
      alert("Đăng nhập thất bại");

    }
  };

  return (

    <div className="login-page">

      <div className="login-card">

        {/* Form */}

        <div className="login-left">
          <div className="text-center mb-4">
            <div className="logo">
              MS
            </div>
            <h3 className="fw-bold">
              MySolution
            </h3>
            <p className="text-muted">
              Welcome back
            </p>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">
                Username
              </label>
              <input
                className="form-control"
                placeholder="Enter username"
                value={username}
                onChange={(e)=>
                  setUsername(e.target.value)
                }
              />

            </div>
            <div className="mb-4">

              <label className="form-label">
                Password
              </label>
              <input
                type="password"
                className="form-control"
                placeholder="Enter password"
                value={password}
                onChange={(e)=>
                  setPassword(e.target.value)
                }
              />
            </div>
            <button
              className="btn btn-warning login-btn w-100"
            >
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
        {/* Banner */}

        <div className="login-right">
          <h1>
            MySolution
          </h1>
          <h4>
            Manage your company's 
          </h4>
          <p>
            User, Role and Permission management platform.
          </p>

          <div className="login-banner"></div>
        </div>
      </div>

    </div>

  );
}