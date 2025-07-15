using AutoMapper;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Mappings;

public class BraceletMappingProfile : Profile
{
    public BraceletMappingProfile()
    {
        CreateMap<CreateBraceletRequestDto, Bracelet>();

        CreateMap<Bracelet, BraceletDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Bracelet, BraceletDetailsDto>()
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.User));
    }
}
