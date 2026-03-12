using AutoMapper;
using backend.Models;
using backend.DTOs;
using backend.DTOs.Review;
using backend.DTOs.Feedback;
using backend.DTOs.SellerReview;

namespace backend.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ReviewerUsername,
                    opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.Username : "Anonymous"))
                .ForMember(dest => dest.Productname,
                    opt => opt.MapFrom(src => src.Product != null ? src.Product.Title : null));

            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.Username : null))
                .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Image,
                    opt => opt.MapFrom(src => src.Images))
                .ForMember(dest => dest.Reviews,
                    opt => opt.MapFrom(src => src.Reviews))
                .ForMember(dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Reviews.Any()
                        ? Math.Round(src.Reviews.Where(r => r.Rating.HasValue).Average(r => (double)r.Rating!.Value), 1)
                        : 0.0))
                .ForMember(dest => dest.ReviewCount,
                    opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.RatingCounts,
                    opt => opt.MapFrom(src => src.Reviews
                        .Where(r => r.Rating.HasValue)
                        .GroupBy(r => r.Rating!.Value)
                        .ToDictionary(g => g.Key, g => g.Count())))
                .ReverseMap();

            CreateMap<CreateReviewDto, Review>();

            // Feedback -> FeedbackDto
            CreateMap<Feedback, FeedbackDto>()
                .ForMember(dest => dest.OrderId,
                    opt => opt.MapFrom(src => src.OrdersId ?? 0))
                .ForMember(dest => dest.SellerId,
                    opt => opt.MapFrom(src => src.SellerId ?? 0))
                .ForMember(dest => dest.SellerName,
                    opt => opt.MapFrom(src => src.Seller != null ? src.Seller.Username : null))
                .ForMember(dest => dest.BuyerName,
                    opt => opt.MapFrom(src => src.Orders != null && src.Orders.Buyer != null ? src.Orders.Buyer.Username : null))
                .ForMember(dest => dest.ProductId,
                    opt => opt.MapFrom(src => src.Orders != null && src.Orders.OrderItems != null
                        ? src.Orders.OrderItems.Select(oi => oi.ProductId).FirstOrDefault()
                        : (int?)null))
                .ForMember(dest => dest.ProductTitle,
                    opt => opt.MapFrom(src => src.Orders != null && src.Orders.OrderItems != null
                        ? src.Orders.OrderItems.Select(oi => oi.Product != null ? oi.Product.Title : null).FirstOrDefault()
                        : null))
                .ForMember(dest => dest.ProductImage,
                    opt => opt.MapFrom(src => src.Orders != null && src.Orders.OrderItems != null
                        ? src.Orders.OrderItems.Select(oi => oi.Product != null ? oi.Product.Images : null).FirstOrDefault()
                        : null))
                .ForMember(dest => dest.OrderDate,
                    opt => opt.MapFrom(src => src.Orders != null ? src.Orders.OrderDate : (DateTime?)null))
                .ForMember(dest => dest.DeliveryOnTime,
                    opt => opt.MapFrom(src => src.DetailFeedbacks.Any()
                        ? src.DetailFeedbacks.First().DeliveryOnTime : (int?)null))
                .ForMember(dest => dest.ExactSame,
                    opt => opt.MapFrom(src => src.DetailFeedbacks.Any()
                        ? src.DetailFeedbacks.First().ExactSame : (int?)null))
                .ForMember(dest => dest.Communication,
                    opt => opt.MapFrom(src => src.DetailFeedbacks.Any()
                        ? src.DetailFeedbacks.First().Communication : (int?)null));

            // SellerToBuyerReview -> SellerReviewDto
            CreateMap<SellerToBuyerReview, SellerReviewDto>();
        }
    }
}
