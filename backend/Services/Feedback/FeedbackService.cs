using backend.DTOs.Feedback;
using backend.Models;
using backend.Repositories.Feedback;
using backend.Repositories.Review;
using AutoMapper;
using ReviewModel = backend.Models.Review;

namespace backend.Services.Feedback
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public FeedbackService(IFeedbackRepository feedbackRepository, IReviewRepository reviewRepository, IMapper mapper)
        {
            _feedbackRepository = feedbackRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<FeedbackDto> LeaveFeedbackAsync(CreateFeedbackDto dto, int buyerId)
        {
            // Validate rating range
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");
            if (dto.DeliveryOnTime < 1 || dto.DeliveryOnTime > 5)
                throw new ArgumentException("DeliveryOnTime must be between 1 and 5.");
            if (dto.ExactSame < 1 || dto.ExactSame > 5)
                throw new ArgumentException("ExactSame must be between 1 and 5.");
            if (dto.Communication < 1 || dto.Communication > 5)
                throw new ArgumentException("Communication must be between 1 and 5.");

            // Check order belongs to buyer
            var isBuyerOrder = await _feedbackRepository.OrderBelongsToBuyer(dto.OrderId, buyerId);
            if (!isBuyerOrder)
                throw new InvalidOperationException("This order does not belong to you.");

            // Check order is delivered/completed
            var isDelivered = await _feedbackRepository.IsOrderDelivered(dto.OrderId);
            if (!isDelivered)
                throw new InvalidOperationException("You can only leave feedback for delivered orders.");

            // Check not already reviewed
            var existing = await _feedbackRepository.GetByOrderIdAsync(dto.OrderId);
            if (existing != null)
                throw new InvalidOperationException("You have already left feedback for this order.");

            // Get seller from order items
            var sellerId = await _feedbackRepository.GetSellerIdByOrderId(dto.OrderId);
            if (sellerId == 0)
                throw new InvalidOperationException("Could not determine the seller for this order.");

            var feedback = new Models.Feedback
            {
                SellerId = sellerId,
                OrdersId = dto.OrderId,
                Comment = dto.Comment,
                AverageRating = dto.Rating,
                TotalReviews = 1,
                PositiveRate = dto.Rating >= 4 ? 100m : 0m
            };

            var detail = new DetailFeedback
            {
                DeliveryOnTime = dto.DeliveryOnTime,
                ExactSame = dto.ExactSame,
                Communication = dto.Communication
            };

            var created = await _feedbackRepository.CreateFeedbackAsync(feedback, detail);

            // Also create a product Review so it shows on Product Details page
            var productId = await _feedbackRepository.GetProductIdByOrderId(dto.OrderId);
            if (productId > 0)
            {
                var existingReview = await _reviewRepository.GetByReviewerAndProductAsync(buyerId, productId);
                if (existingReview == null)
                {
                    var review = new ReviewModel
                    {
                        ProductId = productId,
                        ReviewerId = buyerId,
                        Rating = dto.Rating,
                        Comment = dto.Comment,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _reviewRepository.CreateAsync(review);
                }
            }

            // Mark order as commented
            await _feedbackRepository.SetOrderCommented(dto.OrderId);

            return _mapper.Map<FeedbackDto>(created);
        }

        public async Task<FeedbackDto> UpdateFeedbackAsync(CreateFeedbackDto dto, int buyerId)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var isBuyerOrder = await _feedbackRepository.OrderBelongsToBuyer(dto.OrderId, buyerId);
            if (!isBuyerOrder)
                throw new InvalidOperationException("This order does not belong to you.");

            var existing = await _feedbackRepository.GetByOrderIdAsync(dto.OrderId);
            if (existing == null)
                throw new InvalidOperationException("No feedback found for this order.");

            existing.Comment = dto.Comment;
            existing.AverageRating = dto.Rating;
            existing.PositiveRate = dto.Rating >= 4 ? 100m : 0m;

            var detail = existing.DetailFeedbacks.FirstOrDefault();
            if (detail == null)
                throw new InvalidOperationException("Detail feedback not found.");

            detail.DeliveryOnTime = dto.DeliveryOnTime;
            detail.ExactSame = dto.ExactSame;
            detail.Communication = dto.Communication;

            var updated = await _feedbackRepository.UpdateFeedbackAsync(existing, detail);

            // Also update the product Review
            var productId = await _feedbackRepository.GetProductIdByOrderId(dto.OrderId);
            if (productId > 0)
            {
                var existingReview = await _reviewRepository.GetByReviewerAndProductAsync(buyerId, productId);
                if (existingReview != null)
                {
                    existingReview.Rating = dto.Rating;
                    existingReview.Comment = dto.Comment;
                    await _reviewRepository.UpdateAsync(existingReview);
                }
            }

            return _mapper.Map<FeedbackDto>(updated);
        }

        public async Task DeleteFeedbackAsync(int orderId, int buyerId)
        {
            var isBuyerOrder = await _feedbackRepository.OrderBelongsToBuyer(orderId, buyerId);
            if (!isBuyerOrder)
                throw new InvalidOperationException("This order does not belong to you.");

            var existing = await _feedbackRepository.GetByOrderIdAsync(orderId);
            if (existing == null)
                throw new InvalidOperationException("No feedback found for this order.");

            // Delete associated product review
            var productId = await _feedbackRepository.GetProductIdByOrderId(orderId);
            if (productId > 0)
            {
                var review = await _reviewRepository.GetByReviewerAndProductAsync(buyerId, productId);
                if (review != null)
                    await _reviewRepository.DeleteAsync(review.Id);
            }

            await _feedbackRepository.DeleteFeedbackAsync(orderId);
            await _feedbackRepository.SetOrderUncommented(orderId);
        }

        public async Task<List<FeedbackDto>> GetFeedbacksBySellerIdAsync(int sellerId)
        {
            var feedbacks = await _feedbackRepository.GetBySellerId(sellerId);
            return _mapper.Map<List<FeedbackDto>>(feedbacks);
        }

        public async Task<List<FeedbackDto>> GetFeedbacksByBuyerIdAsync(int buyerId)
        {
            var feedbacks = await _feedbackRepository.GetByBuyerOrdersAsync(buyerId);
            return _mapper.Map<List<FeedbackDto>>(feedbacks);
        }

        public async Task<SellerFeedbackProfileDto> GetSellerProfileAsync(int sellerId)
        {
            var feedbacks = await _feedbackRepository.GetBySellerId(sellerId);

            var profile = new SellerFeedbackProfileDto
            {
                SellerId = sellerId,
                TotalFeedbacks = feedbacks.Count
            };

            if (feedbacks.Count > 0)
            {
                var ratings = feedbacks
                    .Where(f => f.AverageRating.HasValue)
                    .Select(f => (double)f.AverageRating!.Value)
                    .ToList();

                profile.AverageRating = ratings.Any()
                    ? Math.Round(ratings.Average(), 2)
                    : 0;

                // Calculate feedback counts by rating
                profile.PositiveCount = ratings.Count(r => r >= 4.0);
                profile.NeutralCount = ratings.Count(r => r == 3.0);
                profile.NegativeCount = ratings.Count(r => r < 3.0);

                profile.PositiveFeedbackPercent = ratings.Any()
                    ? Math.Round(ratings.Count(r => r >= 4.0) * 100.0 / ratings.Count, 1)
                    : 0;

                profile.SellerName = feedbacks.First().Seller?.Username;

                // Calculate detail averages
                var details = feedbacks
                    .SelectMany(f => f.DetailFeedbacks)
                    .ToList();

                if (details.Any())
                {
                    profile.AvgDeliveryOnTime = Math.Round(
                        details.Where(d => d.DeliveryOnTime.HasValue).Average(d => (double)d.DeliveryOnTime!.Value), 2);
                    profile.AvgExactSame = Math.Round(
                        details.Where(d => d.ExactSame.HasValue).Average(d => (double)d.ExactSame!.Value), 2);
                    profile.AvgCommunication = Math.Round(
                        details.Where(d => d.Communication.HasValue).Average(d => (double)d.Communication!.Value), 2);
                }

                profile.RecentFeedbacks = _mapper.Map<List<FeedbackDto>>(feedbacks.Take(10).ToList());
            }

            return profile;
        }
    }
}
