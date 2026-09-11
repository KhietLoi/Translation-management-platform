import { useState } from "react";
import { useLocation } from "react-router-dom";
import { resendVerificationEmail } from "../../../services/authService";
import "./CheckEmail.css";

export default function CheckEmailPage() {
    const location = useLocation();

    // Email được truyền từ Register hoặc Login
    const email = location.state?.email || "";

    const [loading, setLoading] = useState(false);
    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const handleResendEmail = async () => {
        try {
            if (!email) {
                setError("Email information is missing.");
                return;
            }

            setLoading(true);
            setMessage("");
            setError("");

            const response = await resendVerificationEmail(email);

            setMessage(
                response.message ??
                "Verification email has been sent again."
            );
        } catch (error) {
            console.error(error);

            setError(
                error.response?.data?.errorMessage ??
                "Unable to resend email. Please try again later."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="container min-vh-100 d-flex align-items-center justify-content-center">
            <div
                className="card shadow-sm p-5 text-center"
                style={{
                    maxWidth: "500px",
                    width: "100%",
                    border: "1px solid var(--bs-warning)",
                }}
            >
                <div className="mb-4">
                    <i
                        className="bi bi-envelope-check-fill text-warning"
                        style={{ fontSize: "4rem" }}
                    />
                </div>

                <h2 className="fw-bold mb-3">
                    Please check your email!
                </h2>

                <p className="text-muted">
                    We've sent a confirmation link to:
                </p>

                {email ? (
                    <div className="alert alert-light border fw-semibold">
                        {email}
                    </div>
                ) : (
                    <div className="alert alert-danger">
                        Email information is missing.
                    </div>
                )}

                <p className="text-muted">
                    Check your inbox and click the verification link
                    to complete your registration.
                </p>

                <div className="text-center my-4 floating-img">
                    <img
                        src="/src/assets/hero.png"
                        alt="Confirm Email"
                        className="img-fluid"
                        style={{
                            maxWidth: "220px",
                            width: "100%",
                        }}
                    />
                </div>

                <a
                    href="https://mail.google.com"
                    target="_blank"
                    rel="noreferrer"
                    className="btn btn-warning mb-3"
                >
                    Open Gmail
                </a>

                {message && (
                    <div className="alert alert-success">
                        {message}
                    </div>
                )}

                {error && (
                    <div className="alert alert-danger">
                        {error}
                    </div>
                )}

                <small className="text-muted d-block">
                    Didn't receive the email?
                </small>

                <button
                    type="button"
                    className="btn btn-link mt-2"
                    disabled={!email || loading}
                    onClick={handleResendEmail}
                >
                    {loading ? "Sending..." : "Resend Email"}
                </button>
            </div>
        </div>
    );
}