using backend.DTOs;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

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
