import React, { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import { Container, Row, Col, Card, Form, Button } from "react-bootstrap";
import { toast } from "react-toastify";
import { forgotPassword } from "../../../services/authService";
import "./ForgotPassword.css"; // Remember to import the CSS file

export default function ForgotPasswordPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);
            await forgotPassword(email);

            toast.success("Please check your email!");
            setTimeout(() => {
                navigate("/");
            }, 2000);
        } catch (error) {
            console.error(error);
            toast.error("An error occurred, please try again!");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container fluid className="forgot-password-container min-vh-100 d-flex align-items-center justify-content-center">
            <Row className="w-100 justify-content-center">
                <Col md={6} lg={4}>
                    <Card className="forgot-password-card border-0">
                        <Card.Body className="p-5">
                            <div className="text-center mb-4">
                                <div className="logo">MS</div>
                                <h2 className="fw-bold title-text">Forgot Password?</h2>

                            </div>

                            <Form onSubmit={handleSubmit}>
                                <Form.Group className="mb-4">
                                    <Form.Label className="fw-semibold text-secondary">Email Address</Form.Label>
                                    <Form.Control
                                        type="email"
                                        className="custom-input"
                                        placeholder="e.g., yourname@gmail.com"
                                        value={email}
                                        onChange={(e) => setEmail(e.target.value)}
                                        required
                                    />
                                </Form.Group>

                                <Button
                                    type="submit"
                                    variant="warning"
                                    className="w-100 custom-button fw-bold py-2"
                                    disabled={loading}
                                >
                                    {loading ? (
                                        <>
                                            <span className="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></span>
                                            Sending...
                                        </>
                                    ) : (
                                        "Send Reset Link"
                                    )}
                                </Button>
                            </Form>

                            <div className="text-center mt-4">
                                <Link to="/" className="back-to-login text-decoration-none">
                                    <i className="bi bi-arrow-left me-1"></i> Back to Login
                                </Link>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}