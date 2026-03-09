# Phase 02: Thiết lập Build Backend (.NET)
Status: ⬜ Pending
Dependencies: phase-01

## Objective
Thêm các bước (steps) vào file `ci.yml` để tự động kéo code, thiết lập môi trường .NET và chạy lệnh biên dịch cho mã nguồn backend.

## Requirements
### Functional
- [ ] Lệnh build phải chạy thành công trên thư mục `backend/`.
- [ ] Phải sử dụng đúng phiên bản .NET SDK (dự đoán là .NET 8, tuỳ cấu hình project).

### Implementation Steps
1. [ ] Thêm step `actions/checkout@v4` để Github Actions kéo mã nguồn về.
2. [ ] Thêm step `actions/setup-dotnet@v4` để cài đặt .NET SDK (vd bản 8.0.x).
3. [ ] Thêm step chạy lệnh `dotnet restore` ở thư mục `backend`.
4. [ ] Thêm step chạy lệnh `dotnet build --no-restore` ở thư mục `backend`.

## Files to Create/Modify
- `.github/workflows/ci.yml`

## Test Criteria
- [ ] Push code lỗi cú pháp C# -> Actions báo đỏ (Failed).
- [ ] Push code đúng cú pháp -> Build backend xanh (Passed).

---
Next Phase: phase-03-frontend-build.md
