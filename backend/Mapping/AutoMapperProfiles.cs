using AutoMapper;
using backend.Models;
using backend.DTOs;
namespace backend.Mapping
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();

            // ✅ Phải có dòng này trước khi Product map dùng Reviews
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ReviewerUsername,
                    opt => opt.MapFrom(src => src.Reviewer != null ? src.Reviewer.Username : "Anonymous"));

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
                    // ✅ Dùng Any() trước Average() để tránh crash khi Reviews rỗng
                    opt => opt.MapFrom(src => src.Reviews.Any()
                        ? src.Reviews.Where(r => r.Rating.HasValue).Average(r => (double)r.Rating!.Value)
                        : 0.0))
                .ForMember(dest => dest.ReviewCount,
                    opt => opt.MapFrom(src => src.Reviews.Count))
                .ForMember(dest => dest.RatingCounts,
                    // ✅ Lọc null Rating trước khi GroupBy
                    opt => opt.MapFrom(src => src.Reviews
                        .Where(r => r.Rating.HasValue)
                        .GroupBy(r => r.Rating!.Value)
                        .ToDictionary(g => g.Key, g => g.Count())))
                .ReverseMap();
        }
    }
}
