using AutoMapper;
using EasyClean.API.Dtos;
using EasyClean.API.Models;

namespace EasyClean.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>();
            CreateMap<User, UserForDetailedDto>();
        }
    }
}