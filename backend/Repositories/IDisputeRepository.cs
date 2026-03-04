using backend.DTOs;
using backend.Models;

namespace backend.Repositories
{
    public interface IDisputeRepository
    {
        Task<IEnumerable<DisputeDto>> GetAllDisputes();
        Task<IEnumerable<DisputeDto>> GetDisputesByBuyerId(int buyerId);
        Task<IEnumerable<DisputeDto>> GetDisputesBySellerId(int sellerId);
        Task<Dispute> AddDispute(DisputeCreateDto dispute, int currentUserId);
        Task UpdateDispute(Dispute dispute);
        Task DeleteDispute(int id);
        Task<bool> HasPendingDisputeAsync(int OrderId);
    }
}