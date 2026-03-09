# Phase 01: Thiết lập Dữ liệu (DTOs & Enums)
Status: ✅ Complete
Dependencies: None

## Objective
Tạo các đối tượng truyền tải dữ liệu (DTO) cần thiết để Client (Frontend) gửi quyết định của Seller xuống cho hệ thống.

## Implementation Steps
1. [ ] Tạo `DisputeSellerResponseDto` để nhận: Quyết định (ActionType), Số tiền đền bù (nếu có), và Lời giải thích/Bằng chứng.
2. [ ] Định nghĩa Enum hoặc file Constants `DisputeActionType` lưu các hành động hợp lệ (Chấp nhận hoàn tiền, Đền bù 1 phần, Từ chối).

## Files to Create/Modify
- `backend/DTOs/DisputeSellerResponseDto.cs`
- `backend/DTOs/...` (nếu cần thêm)
