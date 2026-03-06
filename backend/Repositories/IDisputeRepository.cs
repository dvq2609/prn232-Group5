using backend.DTOs;
using backend.Models;
using Sieve.Models;

namespace backend.Repositories
{
    public interface IDisputeRepository
    {
        Task<PagedResult<DisputeDto>> GetAllDisputes(SieveModel sieveModel);
        Task<PagedResult<DisputeDto>> GetDisputesByBuyerId(int buyerId, SieveModel sieveModel);
        Task<PagedResult<DisputeDto>> GetDisputesBySellerId(int sellerId, SieveModel sieveModel);
        Task<Dispute> AddDispute(DisputeCreateDto dispute, int currentUserId);
        Task UpdateDispute(Dispute dispute);
        Task DeleteDispute(int id);
        Task<bool> HasPendingDisputeAsync(int OrderId);
        Task AddDisputeImages(List<DisputeImage> images);
        Task<DisputeDto> GetDisputeById(int id);
    }
}