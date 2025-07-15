using AutoMapper;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserRequestDto, User>();

        CreateMap<UpdateUserRequestDto, User>();

        CreateMap<User, UserSummaryDto>();

        CreateMap<User, UserDetailsDto>();
    }
}