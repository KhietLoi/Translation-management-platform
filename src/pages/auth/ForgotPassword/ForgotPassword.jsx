import { useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import {
    Container,
    Row,
    Col,
    Card,
    Form,
    Button
} from "react-bootstrap";

export default function ForgotPasswordPage() {
    const navigate = useNavigate();

    const [email, setEmail] = useState("");
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);

            // await authService.forgotPassword(email);

            navigate("/check-email", {
                state: { email }
            });
        } catch (error) {
            console.error(error);
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
                            <div className="text-center mb-4">
                                <i
                                    className="bi bi-key-fill text-primary"
                                    style={{ fontSize: "3rem" }}
                                />
                                <h2 className="fw-bold mt-3">
                                    Quên mật khẩu
                                </h2>

                                <p className="text-muted">
                                    Nhập email để nhận liên kết đặt lại mật khẩu
                                </p>
                            </div>

                            <Form onSubmit={handleSubmit}>
                                <Form.Group className="mb-3">
                                    <Form.Label>Email</Form.Label>

                                    <Form.Control
                                        type="email"
                                        placeholder="Nhập email"
                                        value={email}
                                        onChange={(e) =>
                                            setEmail(e.target.value)
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
                                        ? "Đang gửi..."
                                        : "Gửi liên kết"}
                                </Button>
                            </Form>

                            <div className="text-center mt-3">
                                <Link to="/login">
                                    Quay lại đăng nhập
                                </Link>
                            </div>
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}
