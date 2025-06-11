using AutoMapper;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Mappings;

public class UserMappingProfile : Profile
{
    public UserMappingProfile()
    {
        CreateMap<CreateUserRequest, User>();

        CreateMap<UpdateUserRequest, User>();

        CreateMap<User, UserSummaryDto>();

        CreateMap<User, UserDetailsDto>();
    }
}