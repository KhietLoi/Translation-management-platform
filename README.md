# MySolution Platform

Monorepo chứa source ứng dụng và lịch sử Git gốc. Thư mục `.git` duy nhất nằm ở root.

| Thư mục | Thành phần |
| --- | --- |
| `apps/web` | Frontend React/Vite |
| `services/backend` | Solution .NET 10, API, email, workers và migration |
| `services/ai-review` | Dịch vụ đánh giá bản dịch, gồm source MetricX |
| `services/ai-translation` | Dịch vụ AI dịch thuật |
| `examples/translation-read` | Ứng dụng thử SDK đọc bản dịch |
| `examples/package-download` | Ứng dụng thử tải package |
| `tools/load-tests` | Script kiểm thử tải Python |
| `infra` | Cấu hình hạ tầng dùng chung |

## Git và lịch sử

Lịch sử được nhập bằng merge không squash, giữ nguyên hash commit gốc.
Các nhánh cũ nằm ở `archive/<source>/<branch>`, tag cũ ở `archive/<source>/<tag>`.
MetricX được nhập thành source thông thường để clone monorepo không cần lấy submodule.
Git ngoài của frontend chưa có commit nên không có lịch sử để nhập.

```sh
git log --all --graph --oneline
git branch --list 'archive/*'
git status --short
```

Thay đổi chưa commit tại source gốc được sao chép vào working tree và giữ chưa commit.
Trạng thái staged/unstaged gốc không được tái tạo; nội dung file hiện tại được giữ.
File `.env` được sao chép phục vụ chạy local, không thêm mới vào Git.
Các dependency, môi trường ảo và build output bị bỏ qua khi sao chép.
Lịch sử nhập vẫn chứa mọi file đã từng commit, bao gồm cấu hình nhạy cảm nếu source gốc đã commit chúng.

Muốn lưu cả các nhánh lịch sử khi đưa lên remote mới:

```sh
git remote add origin <repository-url>
git push -u origin main
git push origin 'refs/heads/archive/*:refs/heads/archive/*'
git push origin 'refs/tags/archive/*:refs/tags/archive/*'
```

Review và commit thay đổi local trước khi push nếu muốn remote có source hiện tại.
Chưa cấu hình remote hoặc push tự động.

## Phát triển local

Chạy Redis/RabbitMQ bằng `docker compose -f infra/compose.yaml up -d` từ root.
Đây là hạ tầng local, chưa phải cấu hình triển khai toàn bộ ứng dụng.

- Frontend: vào `apps/web`, chạy `npm ci`, sau đó `npm run dev`.
- Backend: chạy `dotnet restore services/backend/MySolution.slnx`, sau đó chạy project mong muốn bằng `dotnet run --project <path.csproj>`.
- AI: tạo môi trường Python riêng tại mỗi service, cài `requirements.txt`, cấu hình `.env` theo code/source tương ứng.
- Kiểm thử .NET: `dotnet test services/backend/MySolution.slnx`.

Xem [lộ trình triển khai](docs/deployment.md) trước khi triển khai độc lập.
