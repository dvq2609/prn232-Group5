using backend.DTOs;
using backend.Models;
using backend.Repositories;

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

        public Task DeleteDispute(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DisputeDto>> GetAllDisputes()
        {
            return await _disputeRepository.GetAllDisputes();
        }

        public async Task<IEnumerable<DisputeDto>> GetDisputesByBuyerId(int buyerId)
        {
            return await _disputeRepository.GetDisputesByBuyerId(buyerId);
        }

        public async Task<IEnumerable<DisputeDto>> GetDisputesBySellerId(int sellerId)
        {
            return await _disputeRepository.GetDisputesBySellerId(sellerId);
        }

        public Task UpdateDispute(Dispute dispute)
        {
            throw new NotImplementedException();
        }
    }
}
