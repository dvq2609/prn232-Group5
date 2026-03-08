using AutoMapper;
using backend.DTOs;
using backend.DTOs.Review;
using backend.Repositories.Review;
using ReviewModel = backend.Models.Review;

namespace backend.Services.Review
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public ReviewService(IReviewRepository reviewRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            var existingReview = await _reviewRepository.GetByReviewerAndProductAsync(reviewerId, dto.ProductId);
            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this product.");

            var hasCompletedOrder = await _reviewRepository.HasCompletedOrderAsync(reviewerId, dto.ProductId);
            if (!hasCompletedOrder)
                throw new InvalidOperationException("You must purchase and receive this product before reviewing.");

            var review = new ReviewModel
            {
                ProductId = dto.ProductId,      
                ReviewerId = reviewerId,         
                Rating = dto.Rating,            
                Comment = dto.Comment,        
                CreatedAt = DateTime.UtcNow     

            };

            var createdReview = await _reviewRepository.CreateAsync(review);

            return _mapper.Map<ReviewDto>(createdReview);
        }

 
        public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

      
        public async Task<List<ReviewDto>> GetReviewsByReviewerIdAsync(int reviewerId)
        {
            var reviews = await _reviewRepository.GetByReviewerIdAsync(reviewerId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

     
        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) return null;
            return _mapper.Map<ReviewDto>(review);
        }

       
        public async Task<ReviewDto?> UpdateReviewAsync(int reviewId, CreateReviewDto dto, int reviewerId)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            if (review.ReviewerId != reviewerId)
                throw new UnauthorizedAccessException("You can only edit your own review.");

            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            var updatedReview = await _reviewRepository.UpdateAsync(review);
            return _mapper.Map<ReviewDto>(updatedReview);
        }

  
        public async Task<bool> DeleteReviewAsync(int reviewId, int reviewerId, string role)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

   
            if (review.ReviewerId != reviewerId && role != "admin")
                throw new UnauthorizedAccessException("You can only delete your own review.");

            return await _reviewRepository.DeleteAsync(reviewId);
        }

   
        public async Task<ReviewSummaryDto> GetReviewSummaryAsync(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);

            var summary = new ReviewSummaryDto
            {
                ProductId = productId,
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any()
                    ? Math.Round(reviews.Where(r => r.Rating.HasValue).Average(r => (double)r.Rating!.Value), 2)
                    : 0.0,
                RatingBreakdown = new Dictionary<int, int>
                {
                    { 5, reviews.Count(r => r.Rating == 5) },
                    { 4, reviews.Count(r => r.Rating == 4) },
                    { 3, reviews.Count(r => r.Rating == 3) },
                    { 2, reviews.Count(r => r.Rating == 2) },
                    { 1, reviews.Count(r => r.Rating == 1) }
                }
            };

            return summary;
        }
    }
}