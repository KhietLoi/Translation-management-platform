import { useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { Container, Row, Col, Card, Form, Button } from "react-bootstrap";
import { resetPassword } from "../../../services/authService";
export default function ResetPasswordPage() {
    const [searchParams] = useSearchParams();
    const token = searchParams.get("token");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [loading, setLoading] = useState(false);
    const [success, setSuccess] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        if (password !== confirmPassword) {
            alert("Mật khẩu không khớp");
            return;
        }

        try {
            setLoading(true);

            const payload = {
                token,
                newPassword: password
            };

            console.log("Payload:", payload);

            const response = await resetPassword(payload);

            console.log("Response:", response);

            setSuccess(true);
        } catch (error) {
            console.log("Error:", error.response?.data);
            alert(
                error.response?.data?.errorMessage ||
                "Reset password failed."
            );
        } finally {
            setLoading(false);
        }
    };
    return (
        <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
            <Row className="w-100 justify-content-center">
                <Col md={5} lg={4}>
                    <Card className="shadow border-0">
                        <Card.Body className="p-4">
                            {!success ? (
                                <>
                                    <div className="text-center mb-4">
                                        <i
                                            className="bi bi-shield-lock-fill text-primary"
                                            style={{ fontSize: "3rem" }}
                                        />

                                        <h2 className="fw-bold mt-3">
                                            Đặt lại mật khẩu
                                        </h2>
                                    </div>

                                    <Form onSubmit={handleSubmit}>
                                        <Form.Group className="mb-3">
                                            <Form.Label>
                                                Mật khẩu mới
                                            </Form.Label>

                                            <Form.Control
                                                type="password"
                                                value={password}
                                                onChange={(e) =>
                                                    setPassword(e.target.value)
                                                }
                                                required
                                            />
                                        </Form.Group>

                                        <Form.Group className="mb-4">
                                            <Form.Label>
                                                Xác nhận mật khẩu
                                            </Form.Label>

                                            <Form.Control
                                                type="password"
                                                value={confirmPassword}
                                                onChange={(e) =>
                                                    setConfirmPassword(
                                                        e.target.value
                                                    )
                                                }
                                                required
                                            />
                                        </Form.Group>

                                        <Button
                                            type="submit"
                                            variant="primary"
                                            className="w-100"
                                            disabled={loading}
                                        >
                                            {loading
                                                ? "Đang cập nhật..."
                                                : "Cập nhật mật khẩu"}
                                        </Button>
                                    </Form>
                                </>
                            ) : (
                                <div className="text-center">
                                    <i
                                        className="bi bi-check-circle-fill text-success"
                                        style={{ fontSize: "4rem" }}
                                    />

                                    <h2 className="fw-bold mt-3">
                                        Thành công
                                    </h2>

                                    <p className="text-muted">
                                        Mật khẩu đã được cập nhật.
                                    </p>

                                    <Button
                                        as={Link}
                                        to="/"
                                        variant="primary"
                                    >
                                        Đăng nhập
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
