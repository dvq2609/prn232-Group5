# 💡 BRIEF: Thiết lập CI cho dự án Clone eBay

**Ngày tạo:** 09/03/2026

---

## 1. VẤN ĐỀ CẦN GIẢI QUYẾT
Dự án cần một luồng tích hợp liên tục (CI) để tự động kiểm tra xem mã nguồn (code) có biên dịch (build) thành công hay không mỗi khi có người đẩy (push) code mới lên kho lưu trữ chính. Điều này giúp phát hiện sớm các lỗi cú pháp hoặc thiếu thư viện, đảm bảo dự án luôn trong trạng thái "có thể chạy được".

## 2. GIẢI PHÁP ĐỀ XUẤT
Sử dụng **GitHub Actions** để thiết lập một workflow CI cơ bản.

- **CI (Continuous Integration):** Mỗi khi có code mới đẩy (push) lên nhánh `main` (hoặc tạo Pull Request vào `main`), GitHub Actions sẽ tự động:
  1. Tải mã nguồn về môi trường chạy của GitHub (runner).
  2. Build phần Backend (.NET).
  3. Build phần Frontend.
  4. Báo cáo kết quả trực tiếp trên giao diện GitHub.

*Lưu ý: Do đây là dự án học tập, chúng ta sẽ bỏ qua phần CD (Continuous Deployment - tự động triển khai lên server) và các bước chạy Automation Test (do dự án hiện chưa viết Unit Test).*

## 3. CÁC THÀNH PHẦN CẦN THIẾT
- **Nơi chạy CI/CD:** GitHub Actions (miễn phí và tích hợp sẵn với kho lưu trữ GitHub hiện tại).
- **Thành phần cần build:**
  - **Backend:** Thư mục `backend` (chạy lệnh `dotnet build`).
  - **Frontend:** Thư mục `frontend` (tuỳ thuộc vào công nghệ đang dùng, ví dụ `npm install` và `npm run build`).

## 4. TÍNH NĂNG CI CƠ BẢN (MVP)

### 🚀 CI (Kiểm tra Build):
- [ ] Thiết lập file YAML cấu hình (ví dụ: `.github/workflows/ci.yml`).
- [ ] Step 1: Checkout mã nguồn.
- [ ] Step 2: Thiết lập môi trường .NET SDK phù hợp.
- [ ] Step 3: Chạy `dotnet restore` và `dotnet build` cho thư mục `backend` để kiểm tra có lỗi biên dịch hay không.
- [ ] Step 4: Thiết lập môi trường Node.js (nếu frontend dùng React/Vue/Angular...).
- [ ] Step 5: Cài đặt thư viện (`npm install/ci`) và chạy lệnh build cho thư mục `frontend` để kiểm tra.

## 5. ƯỚC TÍNH SƠ BỘ
- **Độ phức tạp:** � Đơn giản. Tập trung vào việc tạo file YAML đúng cú pháp và trỏ đúng đường dẫn thư mục `backend`, `frontend`.
- **Rủi ro:** Không có rủi ro đáng kể. Cần đảm bảo phiên bản .NET và Node.js khai báo trong YAML khớp với môi trường đang dùng.

## 6. BƯỚC TIẾP THEO
→ Chạy `/plan` để lên cấu hình chi tiết cho file Workflow YAML này.
