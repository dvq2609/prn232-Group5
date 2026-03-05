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

        /// <summary>
        /// Tạo review mới
        /// Flow: Validate rating → Check đã review chưa → Check đơn hàng đã delivered chưa → Tạo review → Trả về DTO
        /// </summary>
        public async Task<ReviewDto> CreateReviewAsync(CreateReviewDto dto, int reviewerId)
        {
            // 1. Validate rating 1-5
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // 2. Kiểm tra user đã review product này chưa (1 review / product / user)
            var existingReview = await _reviewRepository.GetByReviewerAndProductAsync(reviewerId, dto.ProductId);
            if (existingReview != null)
                throw new InvalidOperationException("You have already reviewed this product.");

            // 3. Kiểm tra user đã mua và nhận hàng chưa (order status = "Delivered")
            var hasCompletedOrder = await _reviewRepository.HasCompletedOrderAsync(reviewerId, dto.ProductId);
            if (!hasCompletedOrder)
                throw new InvalidOperationException("You must purchase and receive this product before reviewing.");

            // 4. Tạo Review entity
            var review = new ReviewModel
            {
                ProductId = dto.ProductId,      // từ CreateReviewDto
                ReviewerId = reviewerId,         // từ JWT token (Controller truyền vào)
                Rating = dto.Rating,             // từ CreateReviewDto
                Comment = dto.Comment,           // từ CreateReviewDto
                CreatedAt = DateTime.UtcNow      // server tự set
                // Id → DB tự sinh (IDENTITY)
            };

            // 5. Lưu vào DB (Repository sẽ auto-load Reviewer)
            var createdReview = await _reviewRepository.CreateAsync(review);

            // 6. Map sang ReviewDto để trả về client
            return _mapper.Map<ReviewDto>(createdReview);
        }

        /// <summary>
        /// Lấy tất cả review của 1 product
        /// </summary>
        public async Task<List<ReviewDto>> GetReviewsByProductIdAsync(int productId)
        {
            var reviews = await _reviewRepository.GetByProductIdAsync(productId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        /// <summary>
        /// Lấy tất cả review mà 1 user đã viết
        /// </summary>
        public async Task<List<ReviewDto>> GetReviewsByReviewerIdAsync(int reviewerId)
        {
            var reviews = await _reviewRepository.GetByReviewerIdAsync(reviewerId);
            return _mapper.Map<List<ReviewDto>>(reviews);
        }

        /// <summary>
        /// Lấy 1 review theo Id
        /// </summary>
        public async Task<ReviewDto?> GetReviewByIdAsync(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null) return null;
            return _mapper.Map<ReviewDto>(review);
        }

        /// <summary>
        /// Cập nhật review — chỉ chủ review mới được sửa
        /// </summary>
        public async Task<ReviewDto?> UpdateReviewAsync(int reviewId, CreateReviewDto dto, int reviewerId)
        {
            // 1. Tìm review
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            // 2. Kiểm tra quyền: chỉ người viết review mới được sửa
            if (review.ReviewerId != reviewerId)
                throw new UnauthorizedAccessException("You can only edit your own review.");

            // 3. Validate rating
            if (dto.Rating < 1 || dto.Rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5.");

            // 4. Cập nhật fields
            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            // 5. Lưu
            var updatedReview = await _reviewRepository.UpdateAsync(review);
            return _mapper.Map<ReviewDto>(updatedReview);
        }

        /// <summary>
        /// Xóa review — chủ review hoặc admin
        /// </summary>
        public async Task<bool> DeleteReviewAsync(int reviewId, int reviewerId, string role)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new KeyNotFoundException("Review not found.");

            // Chỉ chủ review hoặc admin mới được xóa
            if (review.ReviewerId != reviewerId && role != "admin")
                throw new UnauthorizedAccessException("You can only delete your own review.");

            return await _reviewRepository.DeleteAsync(reviewId);
        }

        /// <summary>
        /// Thống kê rating của 1 product: average, total, breakdown (1-5 sao)
        /// </summary>
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