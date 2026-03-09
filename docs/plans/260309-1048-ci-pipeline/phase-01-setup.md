# Phase 01: Thiết lập thư mục và cơ chế GitHub Actions
Status: ⬜ Pending
Dependencies: None

## Objective
Tạo thư mục làm việc cho GitHub Actions và một file `.yml` rỗng (bộ khung) để định nghĩa luồng tích hợp liên tục. Đảm bảo nhánh `main` được lắng nghe mỗi khi có code mới.

## Requirements
### Functional
- [ ] Action phải chạy trên nhánh `main` khi có sư kiện `push` hoặc `pull_request`.
- [ ] Chạy trên một server Linux (ubuntu-latest) do GitHub cung cấp.

### Non-Functional
- [ ] File cấu hình đúng cú pháp YAML.

## Implementation Steps
1. [ ] Tạo thư mục `.github/workflows` ở thư mục gốc của project (nếu chưa có).
2. [ ] Tạo file `.github/workflows/ci.yml`.
3. [ ] Khai báo tên Workflow và các Event trigger (`push`, `pull_request` trên `main`).
4. [ ] Khai báo một `job` cơ bản tên là `build` chạy trên `ubuntu-latest`.

## Files to Create/Modify
- `.github/workflows/ci.yml` - File định nghĩa luồng tự động cho GitHub Actions.

## Test Criteria
- [ ] File YAML pass bộ Parse của GitHub (không báo lỗi đỏ trên web GitHub khi commit lên).

## Notes
- Giai đoạn này chỉ cần bộ khung, chưa cần bước Build cụ thể.

---
Next Phase: phase-02-backend-build.md
