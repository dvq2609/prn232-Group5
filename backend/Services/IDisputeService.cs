using backend.DTOs;
using backend.Models;

namespace backend.Services
{
    public interface IDisputeService
    {
        Task<IEnumerable<DisputeDto>> GetAllDisputes();
        Task<IEnumerable<DisputeDto>> GetDisputesByBuyerId(int buyerId);
        Task<IEnumerable<DisputeDto>> GetDisputesBySellerId(int sellerId);
        Task<Dispute> AddDispute(DisputeCreateDto dispute, int currentUserId);
        Task UpdateDispute(Dispute dispute);
        Task DeleteDispute(int id);

    }
}