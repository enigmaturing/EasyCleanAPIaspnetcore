using System.Linq;
using AutoMapper;
using EasyClean.API.Dtos;
using EasyClean.API.Models;

namespace EasyClean.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.Age, opt => 
                opt.MapFrom(src => src.DateOfBirth.CalculateAge()));
            CreateMap<User, UserForDetailedDto>();
            CreateMap<Purchase, PurchasesForDetailedDto>();
            CreateMap<Topup, TopupsForDetailedDto>();
        }
    }
}