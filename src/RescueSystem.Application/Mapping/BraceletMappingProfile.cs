using AutoMapper;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities.Bracelets;

namespace RescueSystem.Application.Mappings;

public class BraceletMappingProfile : Profile
{
    public BraceletMappingProfile()
    {
        CreateMap<CreateBraceletRequest, Bracelet>();

        CreateMap<Bracelet, BraceletDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<Bracelet, BraceletDetailsDto>()
            .ForMember(dest => dest.AssignedUser, opt => opt.MapFrom(src => src.User));

    }
}
