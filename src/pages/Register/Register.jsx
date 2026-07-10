import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { register } from "../../services/authService";
import "./Register.css";

export default function Register() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    username: "",
    email: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (form.password !== form.confirmPassword) {
      alert("Mật khẩu không khớp");
      return;
    }

    try {

      const response = await register({
        username: form.username,
        email: form.email,
        password: form.password,
      });


      console.log(response.data);

      alert("Đăng ký thành công");
""
      navigate("/");


    } catch(error) {

      console.error(error.response?.data);

      alert(
        error.response?.data?.message 
        || "Đăng ký thất bại"
      );

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

            <div className="mb-3">
              <label className="form-label">Username</label>
              <input
                className="form-control"
                type="text"
                name="username"
                value={form.username}
                placeholder="Enter your username"
                onChange={handleChange}
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Email</label>
              <input
                className="form-control"
                type="email"
                name="email"
                value={form.email}
                placeholder="abc@example.com"
                onChange={handleChange}
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Password</label>
              <input
                className="form-control"
                type="password"
                name="password"
                value={form.password}
                placeholder="Enter your password"
                onChange={handleChange}
              />
            </div>

            <div className="mb-3">
              <label className="form-label">Confirm Password</label>
              <input
                className="form-control"
                type="password"
                name="confirmPassword"
                value={form.confirmPassword}
                placeholder="Confirm your password"
                onChange={handleChange}
              />
            </div>

            <button className="btn btn-warning login-btn w-100">
              Register
            </button>

            <p className="text-center mt-3">
              Already have an account? <a href="/">Login here</a>
            </p>

          </form>

        </div>


        <div className="login-right">
          <h1>Welcome!</h1>
          <h4>Join MySolution</h4>
          <p>
            Create your account and manage
            your system easily.
          </p>
          <div className="line"></div>
          <small>User Management Platform</small>
        </div>

      </div>
    </div>
  );
}