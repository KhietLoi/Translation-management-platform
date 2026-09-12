# Chạy Translation Management Platform bằng Docker trên Windows

Thực hiện các lệnh bằng PowerShell tại root project.

## 1. Chuẩn bị

- Mở Docker Desktop.
- Sử dụng chế độ Linux containers trong Docker Desktop.
- Chờ Docker Engine sẵn sàng.
- Khởi động PostgreSQL trên Windows.
- Bảo đảm database đã có schema của project.
- Khởi động AI translation trên Windows nếu sử dụng.

## 2. Kiểm tra Docker

```powershell
docker version
docker compose version
```

## 3. Kiểm tra vị trí terminal

```powershell
Get-Location
Test-Path .\compose.yaml
```

Kết quả kiểm tra file phải là True.

## 4. Tạo cấu hình local

```powershell
powershell -ExecutionPolicy Bypass -File .\scripts\init-docker.ps1
```

Kết quả mong đợi:

```text
Docker local configuration is ready.
```

Kiểm tra các file:

```powershell
Test-Path .\.env
Test-Path .\infra\docker\api.local.json
Test-Path .\infra\docker\email.local.json
```

Cả ba phải trả về True.

## 5. Kiểm tra cấu hình ứng dụng

Mở các file:

- infra/docker/api.local.json
- infra/docker/email.local.json

Trong api.local.json:

- Kiểm tra ConnectionStrings.DefaultConnection.
- Điền đúng host, port, database, username và password.
- Dùng host.docker.internal nếu PostgreSQL chạy trên Windows.
- Kiểm tra AI.BaseUrl nếu sử dụng AI translation.
- Dùng host.docker.internal nếu AI chạy trên Windows.

Trong email.local.json:

- Kiểm tra cấu hình SendGrid.
- Kiểm tra địa chỉ người gửi.
- Bổ sung các cấu hình Email API cần sử dụng.

Bổ sung cấu hình cần thiết từ appsettings.Development.json
hoặc user secrets nếu chưa có trong các file local.

Mở .env và bảo đảm RABBITMQ_PASSWORD có giá trị.

## 6. Kiểm tra Git ignore

```powershell
git check-ignore .env infra/docker/api.local.json infra/docker/email.local.json
```

Kết quả cần liệt kê đủ:

```text
.env
infra/docker/api.local.json
infra/docker/email.local.json
```

Không commit `.env` và các file `*.local.json`.

## 7. Kiểm tra Compose

```powershell
docker compose config --quiet
```

Chỉ tiếp tục khi lệnh không báo lỗi.

## 8. Build image

```powershell
docker compose build
```

Chờ build thành công web, api và email.

## 9. Khởi động container

```powershell
docker compose up -d
```

## 10. Kiểm tra trạng thái

```powershell
docker compose ps -a
```

Cần có đủ các service:

- web
- api
- email
- redis
- rabbitmq

## 11. Xem log

```powershell
docker compose logs --tail 100 api email web
```

Nếu cần kiểm tra từng service:

```powershell
docker compose logs --tail 100 api
docker compose logs --tail 100 email
docker compose logs --tail 100 web
docker compose logs --tail 100 redis rabbitmq
```

Theo dõi log liên tục:

```powershell
docker compose logs -f --tail 100 api email web
```

Nhấn Ctrl+C để thoát chế độ theo dõi log.

## 12. Truy cập ứng dụng

- Web: http://localhost:5173
- Swagger: http://localhost:5173/swagger
- RabbitMQ Management: http://localhost:15674

Đăng nhập RabbitMQ:

- Username: mysolution
- Password: giá trị RABBITMQ_PASSWORD trong .env

## 13. Kiểm tra Nginx

```powershell
(Invoke-WebRequest -UseBasicParsing http://localhost:5173/healthz).Content
```

Kết quả mong đợi:

```text
ok
```

## 14. Kiểm tra chức năng

- Đăng nhập bằng tài khoản hiện có.
- Mở trang có dữ liệu từ database.
- Kiểm tra chức năng gửi email.
- Kiểm tra AI translation nếu sử dụng.
- Kiểm tra SignalR nếu sử dụng.

## 15. Dừng sau buổi thực hành

```powershell
docker compose down
```

Không thêm `-v` nếu muốn giữ dữ liệu trong named volumes.

## 16. Chạy lại lần sau

Mở Docker Desktop, khởi động PostgreSQL và AI nếu cần.

Tại root project, chạy:

```powershell
docker compose up -d
```

## 17. Sau khi sửa code

```powershell
docker compose up -d --build
```

## 18. Sau khi sửa cấu hình JSON local

```powershell
docker compose up -d --force-recreate api email
```

## 19. Giữ cấu hình giữa các lần chạy

Giữ lại các file:

- .env
- infra/docker/api.local.json
- infra/docker/email.local.json

Script không ghi đè các file JSON local đã tồn tại.

Đổi RABBITMQ_PASSWORD trong .env không tự đổi mật khẩu
user đã được lưu trong RabbitMQ.