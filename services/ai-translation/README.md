```markdown

\# AI Translation Service



\[!\[Python](https://img.shields.io/badge/Python-3.10+-blue.svg)](https://www.python.org/)

\[!\[FastAPI](https://img.shields.io/badge/FastAPI-0.110.0+-00a393.svg)](https://fastapi.tiangolo.com/)

\[!\[Ollama](https://img.shields.io/badge/Ollama-Local\_LLM-black.svg)](https://ollama.com/)



Một microservice xử lý bất đồng bộ (asynchronous) được xây dựng bằng Python và FastAPI. Service này đóng vai trò làm cầu nối giữa backend .NET Core và hệ thống AI nội bộ (Ollama). Hệ thống cung cấp khả năng tự động gợi ý bản dịch UI/UX dựa trên ngữ cảnh với độ trễ thấp và hoàn toàn bảo mật.



\## Tính năng nổi bật



\* \*\*Dịch thuật theo ngữ cảnh:\*\* Xử lý và dịch chính xác văn bản dựa trên UI Keys (ví dụ: auth.login, common.cancel) thay vì dịch từng từ đơn lẻ.

\* \*\*Bảo toàn định dạng:\*\* Tự động giữ nguyên các tham số định dạng hệ thống (như {0}, {{name}}) và các thẻ HTML trong chuỗi.

\* \*\*Hoàn toàn ngoại tuyến (Offline):\*\* Mọi tác vụ xử lý ngôn ngữ tự nhiên được thực hiện trên máy chủ nội bộ thông qua Ollama, loại bỏ hoàn toàn giới hạn truy vấn (Rate Limits) của bên thứ ba.

\* \*\*Hiệu suất cao:\*\* Giao tiếp bất đồng bộ (async/await) mượt mà, không gây hiện tượng thắt nút cổ chai (bottleneck) khi xử lý đa luồng.



\## Ngăn xếp công nghệ (Tech Stack)



\* \*\*Framework:\*\* FastAPI

\* \*\*Server:\*\* Uvicorn

\* \*\*AI Engine:\*\* Ollama

\* \*\*Language Model:\*\* Qwen 2.5 (7B) - Tối ưu hóa cho các tác vụ đa ngôn ngữ, bao gồm Tiếng Việt và Tiếng Anh.



\## Hướng dẫn cài đặt và khởi chạy



\### 1. Yêu cầu hệ thống

\* Python 3.10 trở lên.

\* Ollama đã được cài đặt và khởi chạy trên máy chủ.



\### 2. Tải và cấu hình AI Model

Mở terminal và tải model ngôn ngữ (kích thước khoảng 4.7GB):



```bash

ollama run qwen2.5:7b



```



(Gõ `/bye` để thoát khỏi giao diện tương tác sau khi quá trình tải xuống hoàn tất).



\### 3. Thiết lập môi trường



Khởi tạo môi trường ảo (Virtual Environment) và cài đặt các thư viện phụ thuộc:



```bash

\# Tạo môi trường ảo

python -m venv .venv



\# Kích hoạt môi trường ảo

\# Trên Windows:

.venv\\Scripts\\activate

\# Trên macOS/Linux:

source .venv/bin/activate



\# Cài đặt thư viện

pip install -r requirements.txt



```



\### 4. Khởi động Microservice



Chạy lệnh sau để bật server FastAPI:



```bash

uvicorn app.main:app --reload --port 8000



```



Server sẽ lắng nghe tại địa chỉ: `http://localhost:8000`



\## Tài liệu API (API Reference)



\### Cấp phát bản dịch gợi ý



`POST /api/review/suggest`



Tạo bản dịch gợi ý dựa trên chuỗi gốc và ngữ cảnh cung cấp.



\*\*Request Body (application/json):\*\*



```json

{

&#x20; "source\_text": "Hủy bỏ",

&#x20; "source\_language": "vi-VN",

&#x20; "target\_language": "en-US",

&#x20; "context": "common.actions.cancel"

}



```



\*\*Response (text/plain):\*\*



```text

Cancel



```



\## Cấu trúc thư mục



```text

ai-translation-service/

├── app/

│   ├── main.py

│   └── services/

│       └── suggestion\_service.py

├── .gitignore

├── requirements.txt

└── README.md



```



```



```

