# Phase 03: Xây dựng Controller
Status: ⬜ Pending
Dependencies: phase-02

## Objective
Tạo bộ điểm cuối (Endpoint) cho Frontend hoặc Mobile App gọi lên để Người Bán phản hồi khiếu nại.

## Implementation Steps
1. [ ] Cập nhật controller `DisputeController.cs`.
2. [ ] Thêm POST/PUT endpoint `api/disputes/{id}/seller-response`.
3. [ ] Endpoint nhận vào `DisputeSellerResponseDto`.
4. [ ] Check `Authorize` và lấy UserId hiện tại để đảm bảo đúng là Seller đang tác động.
5. [ ] Trả về HTTP 200 (OK) nếu thành công, HTTP 400 (BadRequest) nếu data lỗi.

## Expected API Call
`POST /api/disputes/{id}/seller-response`
Body:
```json
{
  "action": "OfferPartial",
  "amount": 50000,
  "message": "Hàng gửi đi nguyên vẹn, bạn giữ hàng mình đền 50k nhé."
}
```

## Files to Create/Modify
- `backend/Controllers/DisputeController.cs`
