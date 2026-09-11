import React, { useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { Container, Row, Col, Card, Form, Button } from "react-bootstrap";
import { toast } from "react-toastify";
import { resetPassword } from "../../../services/authService";
import "./ResetPassword.css"; // Remember to import the CSS file
import { EyeIcon, EyeSlashIcon } from "@heroicons/react/24/outline";


export default function ResetPasswordPage() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");

    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);
    const [showPassword, setShowPassword] = useState(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState(false);


    const handleSubmit = async (e) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            toast.error("Passwords do not match!");
            return;
        }

        try {
            setLoading(true);

            const payload = {
                token,
                newPassword: password
            };

            await resetPassword(payload);

            setSuccess(true);
            toast.success("Password updated successfully!");
        } catch (error) {
            console.error("Error:", error.response?.data);
            toast.error(
                error.response?.data?.errorMessage ||
                "Reset password failed. Please try again."
            );
        } finally {
            setLoading(false);
        }
    };

    return (
        <Container fluid className="reset-password-container min-vh-100 d-flex align-items-center justify-content-center">
            <Row className="w-100 justify-content-center">
                <Col md={6} lg={4}>
                    <Card className="reset-password-card border-0">
                        <Card.Body className="p-5">
                            {!success ? (
                                <>
                                    <div className="text-center mb-4">
                                        <div class="logo">MS</div>
                                        <h2 className="fw-bold title-text">
                                            Reset Password
                                        </h2>
                                        <p className="text-muted subtitle-text mt-2">
                                            Please enter and confirm your new password below.
                                        </p>
                                    </div>

                                    <Form onSubmit={handleSubmit}>
                                        <Form.Group className="mb-3">
                                            <Form.Label className="fw-semibold text-secondary">
                                                New Password
                                            </Form.Label>

                                            <div className="position-relative">
                                                <Form.Control
                                                    type={showPassword ? "text" : "password"}
                                                    className="custom-input pe-5"
                                                    placeholder="Enter new password"
                                                    value={password}
                                                    onChange={(e) => setPassword(e.target.value)}
                                                    required
                                                />

                                                <button
                                                    type="button"
                                                    className="password-toggle-btn"
                                                    onClick={() => setShowPassword(!showPassword)}
                                                >
                                                    {showPassword ? (
                                                        <EyeSlashIcon width={20} height={20} />
                                                    ) : (
                                                        <EyeIcon width={20} height={20} />
                                                    )}
                                                </button>
                                            </div>
                                        </Form.Group>

                                        <Form.Group className="mb-4">
                                            <Form.Label className="fw-semibold text-secondary">
                                                Confirm Password
                                            </Form.Label>

                                            <div className="position-relative">
                                                <Form.Control
                                                    type={showConfirmPassword ? "text" : "password"}
                                                    className="custom-input pe-5"
                                                    placeholder="Confirm new password"
                                                    value={confirmPassword}
                                                    onChange={(e) => setConfirmPassword(e.target.value)}
                                                    required
                                                />

                                                <button
                                                    type="button"
                                                    className="password-toggle-btn"
                                                    onClick={() =>
                                                        setShowConfirmPassword(!showConfirmPassword)
                                                    }
                                                >
                                                    {showConfirmPassword ? (
                                                        <EyeSlashIcon width={20} height={20} />
                                                    ) : (
                                                        <EyeIcon width={20} height={20} />
                                                    )}
                                                </button>
                                            </div>
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
                                                    Updating...
                                                </>
                                            ) : (
                                                "Update Password"
                                            )}
                                        </Button>
                                    </Form>
                                </>
                            ) : (
                                <div className="text-center">
                                    <div class="logo">MS</div>

                                    <h2 className="fw-bold title-text mt-3">
                                        Success!
                                    </h2>

                                    <p className="text-muted subtitle-text mb-4">
                                        Your password has been successfully updated. You can now log in with your new credentials.
                                    </p>

                                    <Button
                                        as={Link}
                                        to="/"
                                        variant="warning"
                                        className="w-100 custom-button fw-bold py-2"
                                    >
                                        Login
                                    </Button>
                                </div>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}