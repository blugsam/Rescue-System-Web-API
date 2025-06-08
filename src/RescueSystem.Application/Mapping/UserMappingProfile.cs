using AutoMapper;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Application.Mappings
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<User, UserDetailsDto>();
        }
    }
}