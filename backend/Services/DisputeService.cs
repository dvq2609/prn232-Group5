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

        public Task UpdateDispute(Dispute dispute)
        {
            throw new NotImplementedException();
        }
    }
}
