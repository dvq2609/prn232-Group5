using backend.DTOs.SellerReview;
using backend.Models;
using backend.Repositories.SellerReview;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace backend.Services.SellerReview
{
    public class SellerReviewService : ISellerReviewService
    {
        private readonly ISellerReviewRepository _sellerReviewRepository;
        private readonly CloneEbayDbContext _context;
        private readonly IMapper _mapper;

        public SellerReviewService(ISellerReviewRepository sellerReviewRepository, CloneEbayDbContext context, IMapper mapper)
        {
            _sellerReviewRepository = sellerReviewRepository;
            _context = context;
            _mapper = mapper;
        }

        public async Task<SellerReviewDto> LeaveSellerReviewAsync(CreateSellerReviewDto dto, int sellerId)
        {
            // Check if seller already reviewed this order
            var existing = await _sellerReviewRepository.GetByOrderIdAsync(dto.OrderId);
            if (existing != null)
                throw new InvalidOperationException("You have already left a review for this order's buyer.");

            // Verify the order contains products from this seller
            var orderHasSellerProduct = await _context.OrderItems
                .AnyAsync(oi => oi.OrderId == dto.OrderId
                    && oi.Product != null
                    && oi.Product.SellerId == sellerId);

            if (!orderHasSellerProduct)
                throw new InvalidOperationException("This order does not contain your products.");

            // Verify the buyer
            var order = await _context.OrderTables
                .Include(o => o.Buyer)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                throw new InvalidOperationException("Order not found.");

            if (order.BuyerId != dto.BuyerId)
                throw new InvalidOperationException("Buyer ID does not match this order.");

            var sellerUser = await _context.Users.FindAsync(sellerId);
            var productName = order.OrderItems
                .Where(oi => oi.Product?.SellerId == sellerId)
                .Select(oi => oi.Product!.Title)
                .FirstOrDefault() ?? "";

            var review = new SellerToBuyerReview
            {
                SellerId = sellerId,
                SellerName = sellerUser?.Username ?? "",
                BuyerId = dto.BuyerId,
                BuyerName = order.Buyer?.Username ?? "",
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow,
                OrderId = dto.OrderId,
                ProductName = productName
            };

            var created = await _sellerReviewRepository.CreateAsync(review);

            return _mapper.Map<SellerReviewDto>(created);
        }

        public async Task<List<SellerReviewDto>> GetBySellerIdAsync(int sellerId)
        {
            var reviews = await _sellerReviewRepository.GetBySellerIdAsync(sellerId);
            return await MapToDtosWithImage(reviews);
        }

        public async Task<List<SellerReviewDto>> GetByBuyerIdAsync(int buyerId)
        {
            var reviews = await _sellerReviewRepository.GetByBuyerIdAsync(buyerId);
            return await MapToDtosWithImage(reviews);
        }

        public async Task<SellerReviewDto> UpdateSellerReviewAsync(int reviewId, string comment, int sellerId)
        {
            var review = await _sellerReviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");
            if (review.SellerId != sellerId)
                throw new InvalidOperationException("You can only edit your own reviews.");

            review.Comment = comment;
            await _sellerReviewRepository.UpdateAsync(review);
            return _mapper.Map<SellerReviewDto>(review);
        }

        public async Task DeleteSellerReviewAsync(int reviewId, int sellerId)
        {
            var review = await _sellerReviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new InvalidOperationException("Review not found.");
            if (review.SellerId != sellerId)
                throw new InvalidOperationException("You can only delete your own reviews.");

            await _sellerReviewRepository.DeleteAsync(review);
        }

        private async Task<List<SellerReviewDto>> MapToDtosWithImage(List<SellerToBuyerReview> reviews)
        {
            var orderIds = reviews.Select(r => r.OrderId).Distinct().ToList();
            var orderImages = await _context.OrderItems
                .Where(oi => orderIds.Contains(oi.OrderId ?? 0) && oi.Product != null)
                .Select(oi => new { oi.OrderId, oi.Product!.Images })
                .ToListAsync();
            var imageMap = orderImages
                .GroupBy(x => x.OrderId)
                .ToDictionary(g => g.Key ?? 0, g => g.First().Images);

            return reviews.Select(r =>
            {
                var dto = _mapper.Map<SellerReviewDto>(r);
                dto.ProductImage = imageMap.GetValueOrDefault(r.OrderId);
                return dto;
            }).ToList();
        }
    }
}
