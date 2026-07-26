import { useEffect, useState } from "react";
import { Link, useNavigate, useSearchParams } from "react-router-dom";
import {
    Container,
    Row,
    Col,
    Card,
    Button,
    Spinner,
} from "react-bootstrap";
import {
    verifyEmail,
    resendVerificationEmail,
} from "../../../services/authService";

export default function VerifyEmailPage() {
    const navigate = useNavigate();
    const [searchParams] = useSearchParams();

    const token = searchParams.get("token");

    const [loading, setLoading] = useState(true);
    const [success, setSuccess] = useState(false);
    const [message, setMessage] = useState("");
    const [email, setEmail] = useState("");
    const [errorType, setErrorType] = useState("");
    const [resending, setResending] = useState(false);

    useEffect(() => {
        console.log("========== VERIFY EMAIL ==========");
        console.log("Token:", token);

        if (!token) {
            setLoading(false);
            setSuccess(false);
            setErrorType("invalid");
            setMessage("Verification token is missing.");
            return;
        }

        verifyEmail(token)
            .then((response) => {
                console.log("Verify Success:", response);

                setSuccess(true);

                setMessage(
                    response.message ??
                    "Your email has been verified successfully."
                );
            })
            .catch((error) => {
                console.log("Verify Error:", error);

                setSuccess(false);

                if (!error.response) {
                    console.log("No response from server");

                    setErrorType("network");
                    setMessage(
                        "Cannot connect to server. Please try again later."
                    );
                    return;
                }

                const responseData = error.response.data;

                console.log("Response Data:", responseData);

                const errorMessage =
                    responseData.errorMessage ??
                    "Verification failed.";

                console.log("Error Message:", errorMessage);

                setMessage(errorMessage);

                if (responseData.data?.email) {
                    console.log("Email:", responseData.data.email);
                    setEmail(responseData.data.email);
                } else {
                    console.log("Email not returned from API");
                }

                switch (errorMessage) {
                    case "Verification token is expired.":
                        setErrorType("expired");
                        break;

                    case "Verification token expired.":
                        setErrorType("expired");
                        break;

                    case "Email is already verified.":
                        setErrorType("verified");
                        break;

                    case "User is already verified.":
                        setErrorType("verified");
                        break;

                    case "Invalid verification token.":
                        setErrorType("invalid");
                        break;

                    default:
                        setErrorType("unknown");
                        break;
                }
            })
            .finally(() => {
                console.log("Loading finished");
                setLoading(false);
            });
    }, [token]);

    useEffect(() => {
        console.log("Current errorType:", errorType);
        console.log("Current email:", email);
    }, [errorType, email]);

    const handleResendEmail = async () => {
        console.log("========== RESEND ==========");
        console.log("Email:", email);

        if (!email) {
            console.log("Email is empty");
            setMessage("Email not found.");
            return;
        }

        try {
            setResending(true);

            const response = await resendVerificationEmail(email);

            console.log("Resend Success:", response);

            navigate("/checkemail", {
                state: {
                    email,
                },
            });
        } catch (error) {
            console.log("Resend Error:", error);

            setMessage(
                error.response?.data?.errorMessage ??
                "Unable to resend verification email."
            );
        } finally {
            setResending(false);
        }
    };

    return (
        <Container
            fluid
            className="min-vh-100 d-flex align-items-center justify-content-center bg-light"
        >
            <Row className="w-100 justify-content-center">
                <Col md={6} lg={5} xl={4}>
                    <Card className="shadow border-0">
                        <Card.Body className="p-5 text-center">

                            {loading && (
                                <>
                                    <Spinner animation="border" />
                                    <h4 className="mt-4">
                                        Verifying your email...
                                    </h4>
                                </>
                            )}

                            {!loading && success && (
                                <>
                                    <i
                                        className="bi bi-check-circle-fill text-success"
                                        style={{ fontSize: "4rem" }}
                                    />

                                    <h2 className="fw-bold mt-3">
                                        Email Verified
                                    </h2>

                                    <p>{message}</p>

                                    <Button
                                        variant="success"
                                        onClick={() => navigate("/")}
                                    >
                                        Go to Login
                                    </Button>
                                </>
                            )}

                            {!loading && !success && (
                                <>
                                    <i
                                        className="bi bi-x-circle-fill text-danger"
                                        style={{ fontSize: "4rem" }}
                                    />

                                    <h2 className="fw-bold mt-3">
                                        Verification Failed
                                    </h2>

                                    <p>{message}</p>

                                    <div className="d-grid gap-2">

                                        {errorType === "expired" && (
                                            <Button
                                                variant="warning"
                                                disabled={resending}
                                                onClick={handleResendEmail}
                                            >
                                                {resending
                                                    ? "Sending..."
                                                    : "Resend Verification Email"}
                                            </Button>
                                        )}

                                        {errorType === "verified" && (
                                            <Button
                                                variant="success"
                                                onClick={() => navigate("/")}
                                            >
                                                Go to Login
                                            </Button>
                                        )}

                                        {errorType === "invalid" && (
                                            <Button
                                                as={Link}
                                                to="/register"
                                                variant="outline-danger"
                                            >
                                                Register Again
                                            </Button>
                                        )}

                                        {(errorType === "network" ||
                                            errorType === "unknown") && (
                                                <Button
                                                    variant="primary"
                                                    onClick={() =>
                                                        window.location.reload()
                                                    }
                                                >
                                                    Try Again
                                                </Button>
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