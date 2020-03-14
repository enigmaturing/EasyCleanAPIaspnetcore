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
             CreateMap<MachineUsage, MachineUsageForDetailedDto>()
                .ForMember(dest => dest.IconUrl, opt => 
                opt.MapFrom(src => src.Machine.MachineGroup.IconUrl))
                .ForMember(dest => dest.TotalDurationInMinutes, opt =>
                opt.MapFrom(src => src.QuantityOfServicesBooked * src.Tariff.DurationInMinutes))
                .ForMember(dest => dest.TotalAmountPaid, opt =>
                opt.MapFrom(src => src.QuantityOfServicesBooked * src.Tariff.Price))
                .ForMember(dest => dest.MachineLabeledAs, opt =>
                opt.MapFrom(src => src.Machine.LabeledAs));
            CreateMap<Topup, TopupsForDetailedDto>();
        }
    }
}