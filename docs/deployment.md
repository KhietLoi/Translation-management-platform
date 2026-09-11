# Chuẩn bị triển khai microservice

Việc gom Git không tự động tách kiến trúc thành microservice. Cấu trúc này giữ nguyên project references và hành vi ứng dụng, tạo nơi quản lý chung cho các đơn vị triển khai.

## Đơn vị triển khai dự kiến

Frontend, API chính, Email API, Worker API, TranslationPipeline Worker, AI review và AI translation có thể có image/pipeline riêng. Migration cần chạy như job có kiểm soát. Các thư mục Domain/Application/Infrastructure là thư viện, không cần container riêng.

Backend hiện có Dockerfile cho API chính và Email API. Build context phải là `services/backend`, ví dụ từ root:

```sh
docker build -f services/backend/MySolution.Api/Dockerfile -t mysolution-api:local services/backend
docker build -f services/backend/MySolution.Email.Api/Dockerfile -t mysolution-email:local services/backend
```

## Các bước tiếp theo

1. Chuẩn hóa cấu hình môi trường: database PostgreSQL, JWT, SMTP, RabbitMQ, Redis và endpoint AI. Dùng secret store trên môi trường triển khai; không đưa thông tin bí mật mới vào Git.
2. Hoàn thiện Dockerfile cho từng worker, frontend và AI. Xác nhận dependency/model của AI trước khi đóng image.
3. Viết Compose toàn hệ thống với healthcheck, network, persistent volumes và migration job. `localhost` trong container phải đổi sang hostname phù hợp của dịch vụ phụ thuộc.
4. Thiết lập CI theo thư mục thay đổi. Thay đổi thư viện .NET dùng chung phải chạy build/test cho mọi ứng dụng tham chiếu.
5. Xác định quyền sở hữu dữ liệu của từng service, hợp đồng HTTP/message, retry, idempotency và quan sát log/trace trước khi scale độc lập.
6. Khi cần, thêm manifest Kubernetes/Helm, resource limits, secret injection và chiến lược rollout.

Chưa xác nhận build hoặc khởi động toàn hệ thống trong lần gom repository này. Cấu hình hạ tầng local được giữ từ source backend; cần đánh giá riêng cho production.
