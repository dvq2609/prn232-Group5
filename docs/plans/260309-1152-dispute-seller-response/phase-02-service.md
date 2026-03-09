# Phase 02: Xây dựng Logic Service
Status: ⬜ Pending
Dependencies: phase-01

## Objective
Tạo các service xử lý nghiệp vụ khi Seller đưa ra lựa chọn, đồng thời lưu trữ log và cập nhật trạng thái Dispute.

## Implementation Steps
1. [ ] Cập nhật giao diện `IDisputeRepository` và `DisputeRepository` để thêm hàm cập nhật bản ghi Dispute.
2. [ ] Thêm logic vào `IDisputeService` và `DisputeService` cho hàm `ProcessSellerResponseAsync`.
   - **Validation:** Kiểm tra Dispute có tồn tại? Có đang ở trạng thái `Pending` không? Người gọi API có đúng là Seller của mặt hàng trong Dispute không?
   - **Action 1 (AcceptReturn):** Đổi Status -> `Resolved_Refunded`. (Option: Gọi OrderService để trả tiền).
   - **Action 2 (OfferPartial):** Đổi Status -> `Negotiating`. Cập nhật số tiền đề nghị vào hóa đơn khiếu nại.
   - **Action 3 (Decline):** Đổi Status -> `Escalated` (hoặc `Declined` chờ Buyer kiện tiếp Admin).
3. [ ] (Optional) Tạo thêm table/entity `DisputeMessage` để lưu lại lời biện minh của Seller. (Tùy yêu cầu)

## Files to Create/Modify
- `backend/Repositories/IDisputeRepository.cs`
- `backend/Repositories/DisputeRepository.cs`
- `backend/Services/IDisputeService.cs`
- `backend/Services/DisputeService.cs`
