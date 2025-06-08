using AutoMapper;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Mappings
{
    public class BraceletMappingProfile : Profile
    {
        public BraceletMappingProfile()
        {
            CreateMap<Bracelet, BraceletDetailsDto>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()));
        }
    }
}