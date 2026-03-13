using backend.DTOs;
using backend.Models;
using Sieve.Models;

namespace backend.Services
{
    public interface IDisputeService
    {
        Task<PagedResult<DisputeDto>> GetAllDisputes(SieveModel sieveModel);
        Task<PagedResult<DisputeDto>> GetDisputesByBuyerId(int buyerId, SieveModel sieveModel);
        Task<PagedResult<DisputeDto>> GetDisputesBySellerId(int sellerId, SieveModel sieveModel);
        Task<Dispute> AddDispute(DisputeCreateDto dispute, int currentUserId);
        Task UpdateDispute(Dispute dispute);
        Task DeleteDispute(int id);
        Task AddDisputeImages(List<DisputeImage> images);
        Task<DisputeDto> GetDisputeById(int id);
        Task<bool> ProcessSellerResponseAsync(int disputeId, DisputeSellerResponseDto responseDto, int sellerId);
        Task<bool> ProcessBuyerResponseAsync(int disputeId, DisputeBuyerResponseDto responseDto, int buyerId);
        Task<bool> ProcessAdminResponseAsync(int disputeId, DisputeAdminResponseDto responseDto, int adminId);
    }
}