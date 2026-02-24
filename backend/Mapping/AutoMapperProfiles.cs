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
        }
    }
}