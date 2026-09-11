# TMS External React Demo — i18n & TranslationRead SDK Integration

Ứng dụng demo tích hợp dịch đa ngôn ngữ (i18n) cho ứng dụng React Web, cho phép kết nối trực tiếp với **MySolution Translation Management Platform (TMS)** qua API `TranslationRead`.

---

## Tổng Quan Dự Án

Dự án mô phỏng việc tích hợp và tải bản dịch tự động từ API SDK của hệ thống TMS. Ứng dụng cung cấp bảng điều khiển xem bản dịch, bộ kiểm tra log mạng (Network Inspector) và một **User Profile i18n Live Demo (MiniAppPreview)** giúp xem trực tiếp giao diện ứng dụng khi thay đổi ngôn ngữ.

---

## Các Tính Năng Chính

### 1. 👤 User Profile i18n Live Demo (`MiniAppPreview.jsx`)
Màn hình mô phỏng giao diện cá nhân (User Profile) render nội dung động dựa trên dữ liệu bản dịch fetched từ API.

* **Chuyển Đổi Ngôn Ngữ Trực Tiếp (Quick Language Switcher)**:
  * Cho phép người dùng chuyển đổi ngôn ngữ nhanh chóng qua thanh nút chip hoặc `select` trong mục **Preferences**.
  * Hỗ trợ các mã ngôn ngữ: `vi-VN` (Tiếng Việt), `en-US` (Tiếng Anh), `ja-JP` (Tiếng Nhật), `ko-KR` (Tiếng Hàn), `zh-TW` (Tiếng Trung Phồn thể), `fr-FR` (Tiếng Pháp), `de-DE` (Tiếng Đức).
* **Dynamic Translation Helper `t(key, fallback)`**:
  * Hàm tiện ích giúp truy xuất giá trị dịch động theo key (ví dụ: `profile.title`, `profile.preferences`, `profile.language`).
  * Tự động hiển thị chuỗi `fallback` nếu key chưa được tải hoặc chưa được định nghĩa.
* **Key Inspector (Soi Key Bản Dịch)**:
  * Kiểm tra chi tiết từng khóa dịch `profile.*` đang active.
  * Hiển thị tên key, giá trị dịch thực tế theo ngôn ngữ đã chọn, kiểu dữ liệu và độ dài chuỗi.

---

### 2. Translation Viewer (`TranslationViewer.jsx`)
* Tải bản dịch theo `projectId` và mã ngôn ngữ (`language`).
* Chế độ xem linh hoạt: dạng **Bảng (Table)** hoặc dạng **JSON thô**.
* Lọc từ khóa tìm kiếm nhanh theo key hoặc value.
* Hỗ trợ copy toàn bộ JSON bản dịch chỉ với một cú nhấp chuột.

---

### 3. API Configuration Panel (`ApiConfigPanel.jsx`)
* Cấu hình tham số kết nối API:
  * **Base API URL**: Địa chỉ server backend (mặc định: `http://localhost:5182` hoặc `https://localhost:7185`).
  * **X-API-KEY**: API Key xác thực quyền `TranslationRead`.
  * **Project ID**: ID của project cần lấy bản dịch.

---

### 4. Version & Package Viewer (`VersionPackageViewer.jsx`)
* Kiểm tra phiên bản phát hành hiện tại (**Active Release Version**).
* Tải xuống toàn bộ gói bản dịch dạng tập tin nén ZIP (`.zip`).

---

### 5. Network Inspector (`NetworkInspector.jsx`)
* Ghi log thời gian thực các request HTTP gửi đến TMS API.
* Hiển thị thông số chi tiết: HTTP Method, Status Code, Latency (độ trễ ms), Headers và Response Payload.

---

## Cấu Trúc Mã Nguồn

```text
translationread/
├── src/
│   ├── api/
│   │   └── translationApi.js     # Helper gửi request axios tới TMS Backend API
│   ├── components/
│   │   ├── ApiConfigPanel.jsx       # Form cấu hình URL, API Key, Project ID
│   │   ├── Header.jsx               # Header ứng dụng
│   │   ├── MiniAppPreview.jsx       # Component Live Demo User Profile i18n
│   │   ├── NetworkInspector.jsx     # Bảng theo dõi log request/response
│   │   ├── TranslationViewer.jsx    # Bảng xem và tra cứu bản dịch
│   │   └── VersionPackageViewer.jsx # Xem phiên bản & tải ZIP package
│   ├── App.css                      # Styling chính với Glassmorphism UI
│   ├── App.jsx                      # Main Layout & State Management
│   └── main.jsx                     # Entry point React
├── DataTest/                        # Thư mục lưu file JSON dữ liệu mẫu
│   ├── vi-VN.json
│   ├── en-US.json
│   ├── zh-TW.json
│   ├── ko-KR.json
│   └── fr-FR.json
├── package.json
└── README.md
```

---

## Hướng Dẫn Cài Đặt & Chạy Dự Án

### 1. Cài đặt thư viện
```bash
npm install
```

### 2. Khởi tạo file môi trường (Tùy chọn)
Tạo file `.env` tại thư mục gốc dự án:
```env
VITE_API_URL=http://localhost:5182
VITE_API_KEY=
VITE_PROJECT_ID=01a00d6f-864a-78ed-8bd4-70bcbda5a552
```

### 3. Chạy môi trường phát triển (Development)
```bash
npm run dev
```
Ứng dụng sẽ chạy tại địa chỉ: `http://localhost:5173`.

---

## Ví Dụ Sử Dụng Key trong Component (`MiniAppPreview.jsx`)

```jsx
// Hàm tiện ích lookup bản dịch
const t = (key, fallback = "") => {
  if (!translations) return fallback;
  return translations[key] !== undefined ? translations[key] : fallback;
};

// Render giao diện với key profile.preferences
<h4 className="section-title">
  {t("profile.preferences", "Preferences")}
</h4>

// Render thẻ select đổi ngôn ngữ trong phần Preferences
<select
  className="input-field"
  value={language}
  onChange={(e) => onSwitchLanguage && onSwitchLanguage(e.target.value)}
>
  <option value="vi-VN">Vietnamese (vi-VN)</option>
  <option value="en-US">English (en-US)</option>
  <option value="ja-JP">Japanese (ja-JP)</option>
  <option value="ko-KR">Korean (ko-KR)</option>
  <option value="zh-TW">Traditional Chinese (zh-TW)</option>
  <option value="fr-FR">French (fr-FR)</option>
  <option value="de-DE">German (de-DE)</option>
</select>
```
