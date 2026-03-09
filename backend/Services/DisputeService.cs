using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Sieve.Models;

namespace backend.Services
{
    public class DisputeService : IDisputeService
    {
        private readonly IDisputeRepository _disputeRepository;
        public DisputeService(IDisputeRepository disputeRepository)
        {
            _disputeRepository = disputeRepository;
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
            return true;
        }
    }
}
