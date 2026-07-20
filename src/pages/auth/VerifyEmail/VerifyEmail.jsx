import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { Container, Row, Col, Card, Button, Spinner } from "react-bootstrap";
import { verifyEmail, resendVerificationEmail } from "../../../services/authService";
import { useNavigate } from "react-router-dom";

export default function VerifyEmailPage() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");
    const [loading, setLoading] = useState(true);
    const [success, setSuccess] = useState(false);
    const [message, setMessage] = useState("");
    const [errorType, setErrorType] = useState("");
    const [resending, setResending] = useState(false);
    const [email, setEmail] = useState("");

    const handleResendEmail = async () => {
        try {
            if (!email) {
                setMessage("Email not found!");
                return;
            }
            setResending(true);
            const response = await resendVerificationEmail(email);
            navigate("/checkemail", {
                state: {
                    email: email,
                },
            });

        } catch (error) {
            setMessage(error.response?.data?.errorMessage || "Cannot resend verification email.");
        } finally {
            setResending(false);
        }
    };

    useEffect(() => {
        if (!token) {
            setSuccess(false);
            setErrorType("invalid");
            setMessage("Verification token is missing.");
            setLoading(false);
            return;
        }
        verifyEmail(token)
            .then((response) => {
                setSuccess(true);
                setMessage(response.message || "Your email has been verified successfully.");
            })
            .catch((error) => {
                setSuccess(false);
                if (!error.response) {
                    setErrorType("network");
                    setMessage("Cannot connect to server. Please try again later.");
                    return;
                }
                const responseData = error.response.data;
                const msg = responseData.errorMessage || "Verification failed.";
                setMessage(msg);

                if (responseData?.data?.email) {
                    setEmail(responseData.data.email);
                }

                const errorMap = {
                    "Verification token expired.": "expired",
                    "Verification token already used.": "used",
                    "User is already verified.": "verified",
                    "Invalid verification token.": "invalid",
                };
                setErrorType(errorMap[msg] || "unknown");
            })
            .finally(() => setLoading(false));
    }, [token]);

    return (
        <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
            <Row className="w-100 justify-content-center">
                <Col md={6} lg={5} xl={4}>
                    <Card className="shadow border-0">
                        <Card.Body className="p-5 text-center">
                            {loading ? (
                                <>
                                    <Spinner animation="border" variant="primary" />
                                    <h4 className="mt-4">Verifying your email...</h4>
                                    <p className="text-muted">Please wait a moment.</p>
                                </>
                            ) : success ? (
                                <>
                                    <i className="bi bi-check-circle-fill text-success" style={{ fontSize: "4rem" }} />
                                    <h2 className="fw-bold mt-3">Email Verified</h2>
                                    <p className="text-muted">{message}</p>
                                    <Button as={Link} to="/" variant="success">Go to Login</Button>
                                </>
                            ) : (
                                <>
                                    <i className="bi bi-x-circle-fill text-danger" style={{ fontSize: "4rem" }} />
                                    <h2 className="fw-bold mt-3">Verification Failed</h2>
                                    <p className="text-muted">{message}</p>
                                    <div className="d-grid gap-2 mt-3">
                                        {errorType === "expired" && (
                                            <Button variant="warning" onClick={handleResendEmail} disabled={resending}>
                                                {resending ? "Sending..." : "Resend Verification Email"}

                                            </Button>
                                        )}
                                        {(errorType === "used" || errorType === "verified") && (
                                            <Button as={Link} to="/" variant="success">Go to Login</Button>
                                        )}
                                        {errorType === "invalid" && (
                                            <Button as={Link} to="/register" variant="outline-danger">Register Again</Button>
                                        )}
                                        {(errorType === "network" || errorType === "unknown") && (
                                            <Button onClick={() => window.location.reload()} variant="primary">Try Again</Button>
                                        )}
                                    </div>
                                </>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}