
import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import {
    Container,
    Row,
    Col,
    Card,
    Button,
    Spinner
} from "react-bootstrap";

export default function VerifyEmailPage() {
    const [searchParams] = useSearchParams();

    const token = searchParams.get("token");

    const [loading, setLoading] = useState(true);
    const [success, setSuccess] = useState(false);

    useEffect(() => {
        const verifyEmail = async () => {
            try {
                // await authService.verifyEmail(token);

                setSuccess(true);
            } catch {
                setSuccess(false);
            } finally {
                setLoading(false);
            }
        };

        verifyEmail();
    }, [token]);

    return (
        <Container fluid className="min-vh-100 d-flex align-items-center justify-content-center bg-light">
            <Row className="w-100 justify-content-center">
                <Col md={5} lg={4}>
                    <Card className="shadow border-0">
                        <Card.Body className="p-5 text-center">
                            {loading ? (
                                <>
                                    <Spinner
                                        animation="border"
                                        variant="primary"
                                    />

                                    <h4 className="mt-3">
                                        Đang xác thực email...
                                    </h4>
                                </>
                            ) : success ? (
                                <>
                                    <i
                                        className="bi bi-check-circle-fill text-success"
                                        style={{ fontSize: "4rem" }}
                                    />

                                    <h2 className="fw-bold mt-3">
                                        Xác thực thành công
                                    </h2>

                                    <p className="text-muted">
                                        Email của bạn đã được xác thực.
                                    </p>

                                    <Button
                                        as={Link}
                                        to="/"
                                        variant="primary"
                                    >
                                        Đăng nhập
                                    </Button>
                                </>
                            ) : (
                                <>
                                    <i
                                        className="bi bi-x-circle-fill text-danger"
                                        style={{ fontSize: "4rem" }}
                                    />

                                    <h2 className="fw-bold mt-3">
                                        Xác thực thất bại
                                    </h2>

                                    <p className="text-muted">
                                        Link xác thực không hợp lệ hoặc đã hết hạn.
                                    </p>
                                </>
                            )}
                        </Card.Body>
                    </Card>
                </Col>
            </Row>
        </Container>
    );
}
