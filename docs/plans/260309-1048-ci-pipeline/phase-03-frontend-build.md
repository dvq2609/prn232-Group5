# Phase 03: Thiết lập Build Frontend
Status: ⬜ Pending
Dependencies: phase-02

## Objective
Thêm các bước để tự động thiết lập môi trường Node.js, cài đặt thư viện và build dự án Frontend (nếu dự án Frontend lưu cùng một kho lưu trữ với Backend).

## Requirements
### Functional
- [ ] Lệnh build phải chạy thành công trên thư mục `frontend/`.
- [ ] Phải sử dụng đúng phiên bản Node.js khớp với môi trường phát triển (vd: Node 18, Node 20).

### Implementation Steps
1. [ ] Thêm step `actions/setup-node@v4` để cài đặt Node.js.
2. [ ] Thêm step chạy lệnh `npm ci` (khuyên dùng hơn `npm install` khi build CI) ở thư mục `frontend`.
3. [ ] Thêm step chạy lệnh `npm run build` ở thư mục `frontend`.

## Files to Create/Modify
- `.github/workflows/ci.yml`

## Test Criteria
- [ ] Push code lỗi Typescript/Cú pháp Javascript -> Actions báo đỏ.
- [ ] Push code đúng -> Build Frontend xanh.

---
Next Phase: (Kết thúc)
