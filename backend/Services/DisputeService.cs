using backend.DTOs;
using backend.Models;
using backend.Repositories;
using backend.Services.Notification;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;

namespace backend.Services
{
    public class DisputeService : IDisputeService
    {
        private readonly IDisputeRepository _disputeRepository;
        private readonly INotificationService _notificationService;
        private readonly CloneEbayDbContext _context;
        public DisputeService(IDisputeRepository disputeRepository, INotificationService notificationService, CloneEbayDbContext context)
        {
            _disputeRepository = disputeRepository;
            _notificationService = notificationService;
            _context = context;
        }
        public async Task<Dispute> AddDispute(DisputeCreateDto dispute, int currentUserId)
        {
            return await _disputeRepository.AddDispute(dispute, currentUserId);
        }
        public async Task<DisputeDto> GetDisputeById(int id)
        {
            return await _disputeRepository.GetDisputeById(id);
        }
        public async Task AddDisputeImages(List<DisputeImage> images)
        {
            await _disputeRepository.AddDisputeImages(images);
        }

        public Task DeleteDispute(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedResult<DisputeDto>> GetAllDisputes(SieveModel sieveModel)
        {
            return await _disputeRepository.GetAllDisputes(sieveModel);
        }

        public async Task<PagedResult<DisputeDto>> GetDisputesByBuyerId(int buyerId, SieveModel sieveModel)
        {
            return await _disputeRepository.GetDisputesByBuyerId(buyerId, sieveModel);
        }

        public async Task<PagedResult<DisputeDto>> GetDisputesBySellerId(int sellerId, SieveModel sieveModel)
        {
            return await _disputeRepository.GetDisputesBySellerId(sellerId, sieveModel);
        }

        public async Task UpdateDispute(Dispute dispute)
        {
            await _disputeRepository.UpdateDispute(dispute);
        }

        public async Task<bool> ProcessSellerResponseAsync(int disputeId, DisputeSellerResponseDto responseDto, int sellerId)
        {
            var dispute = await _disputeRepository.GetDisputeEntityById(disputeId);
            if (dispute == null) throw new Exception("Dispute not found");

            var isSeller = dispute.Order?.OrderItems.Any(oi => oi.Product?.SellerId == sellerId) ?? false;
            if (!isSeller) throw new Exception("You are not the seller of this order");

            if (dispute.Status != "Pending") throw new Exception("Dispute is not in a state that can be responded to by seller");

            switch (responseDto.Action)
            {
                case DisputeActionType.AcceptReturn:
                    dispute.Status = "Resolved_Refunded";
                    dispute.Resolution = "Người bán đồng ý hoàn tiền";
                    if (dispute.Order != null) dispute.Order.Status = "Refunded";
                    break;
                case DisputeActionType.OfferPartial:
                    dispute.Status = "Negotiating";
                    dispute.Resolution = $"Người bán đề nghị đền bù: {responseDto.AmountOffer}";
                    break;
                case DisputeActionType.Decline:
                    dispute.Status = "Escalated";
                    dispute.Resolution = "Người bán từ chối khiếu nại";
                    break;
                default:
                    throw new Exception("Hành động không hợp lệ");
            }

            dispute.SellerResponse = responseDto.Message;


            await _disputeRepository.UpdateDispute(dispute);
            await _notificationService.SendNotificationAsync(dispute.RaisedBy ?? 0, $"Người bán đã phản hồi khiếu nại #{disputeId} của bạn", $"/Dispute/BuyerDisputes/{disputeId}");
            return true;
        }
        public async Task<bool> ProcessBuyerResponseAsync(int disputeId, DisputeBuyerResponseDto responseDto, int buyerId)
        {
            var dispute = await _disputeRepository.GetDisputeEntityById(disputeId);
            if (dispute == null) throw new Exception("Dispute not found");

            var isBuyer = dispute.Order?.BuyerId == buyerId;
            if (!isBuyer) throw new Exception("You are not the buyer of this order");

            if (dispute.Status != "Negotiating") throw new Exception("Dispute is not in a state that can be responded to by buyer");

            switch (responseDto.Action)
            {
                case DisputeBuyerActionType.Accept:
                    dispute.Status = "Resolved_Refunded";
                    dispute.Resolution = "Người mua đồng ý với đề nghị của người bán";
                    if (dispute.Order != null) dispute.Order.Status = "Refunded";
                    dispute.SolvedDate = DateTime.Now;
                    break;
                case DisputeBuyerActionType.Decline:
                    dispute.Status = "Escalated";
                    dispute.Resolution = "Người mua từ chối đề nghị của người bán";

                    break;
                default:
                    throw new Exception("Hành động không hợp lệ");
            }

            dispute.BuyerResponse = responseDto.Message;

            await _disputeRepository.UpdateDispute(dispute);
            var sellerId = await GetSellerIdByDisputeId(disputeId);
            await _notificationService.SendNotificationAsync(sellerId, $"Người mua đã phản hồi khiếu nại #{disputeId} của bạn", $"/Dispute/SellerDisputes/{disputeId}");
            return true;
        }
        public async Task<bool> ProcessAdminResponseAsync(int disputeId, DisputeAdminResponseDto responseDto, int adminId)
        {
            var dispute = await _disputeRepository.GetDisputeEntityById(disputeId);
            if (dispute == null) throw new Exception("Dispute not found");

            if (dispute.Status != "Escalated") throw new Exception("Dispute is not in a state that can be responded to by admin");

            switch (responseDto.Action)
            {
                case DisputeAdminActionType.BuyerWin:
                    dispute.Status = "Resolved_Refunded";
                    dispute.Resolution = "Admin quyết định hoàn tiền cho người mua";
                    if (dispute.Order != null) dispute.Order.Status = "Refunded";
                    break;
                case DisputeAdminActionType.SellerWin:
                    dispute.Status = "Resolved_NotRefunded";
                    dispute.Resolution = "Admin quyết định không hoàn tiền cho người mua";
                    break;
                default:
                    throw new Exception("Hành động không hợp lệ");
            }

            dispute.AdminResponse = responseDto.Message;
            dispute.SolvedDate = DateTime.Now;

            await _disputeRepository.UpdateDispute(dispute);
            var sellerId = await GetSellerIdByDisputeId(disputeId);
            await _notificationService.SendNotificationAsync(sellerId, $"Khiếu nại #{disputeId} đã được Admin xử lý", $"/Dispute/SellerDisputes/{disputeId}");
            await _notificationService.SendNotificationAsync(dispute.RaisedBy ?? 0, $"Khiếu nại #{disputeId} đã được Admin xử lý", $"/Dispute/BuyerDisputes/{disputeId}");
            return true;
        }
        public async Task<int> GetSellerIdByDisputeId(int disputeId)
        {
            var dispute = await _context.Disputes.Include(d => d.Order).ThenInclude(o => o.OrderItems).ThenInclude(oi => oi.Product).Where(d => d.Id == disputeId).FirstOrDefaultAsync();
            if (dispute == null) throw new Exception("Dispute not found");
            var sellerId = dispute.Order.OrderItems.Select(oi => oi.Product.SellerId).FirstOrDefault();
            return sellerId ?? throw new Exception("Seller not found");
        }
        public async Task<int> GetBuyerIdByDisputeId(int disputeId)
        {
            var dispute = await _context.Disputes.Include(d => d.Order).Where(d => d.Id == disputeId).FirstOrDefaultAsync();
            if (dispute == null) throw new Exception("Dispute not found");
            var buyerId = dispute.Order.BuyerId;
            return buyerId ?? throw new Exception("Buyer not found");
        }
    }
}
