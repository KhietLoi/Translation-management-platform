import { Link, useLocation } from "react-router-dom";
import "./CheckEmail.css";
export default function CheckEmailPage() {
    const location = useLocation();

    const email = location.state?.email || "";

    return (
        <div className="container min-vh-100 d-flex align-items-center justify-content-center ">
            <div className="card body p-5 text-center" style={{ maxWidth: "500px", width: "100%", border: "1px solid var(--bs-warning)" }}>
                <div className="mb-4"> <i className="bi bi-envelope-check-fill text-primary" style={{ fontSize: "4rem" }} ></i> </div>

                <h2 className="fw-bold mb-3">Please check your email!</h2>
                <p className="text-muted">
                    We've sent a confirmation link to:
                </p>

                {
                    email && (
                        <div className="alert alert-light border fw-semibold">
                            {email}
                        </div>
                    )
                }

                <p className="text-muted">Check your inbox and click on the confirmation link to complete the registration.</p>
                <div className="text-center mt-2 floating-img">
                    <img src="src\assets\hero.png" alt="Confirm Email" className="img-fluid" style={{ maxWidth: "200px", width: "100%" }} />
                </div>


                <small className="text-muted mt-3">If you didn't receive an email, please check your spam folder or resend the email.</small>
                <div className="text-center mt-3">
                    <Link to="/" className="btn btn-link">Resend email</Link>
                </div>
            </div>
        </div>
    )
}