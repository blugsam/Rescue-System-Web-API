using AutoMapper;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Application.Mappings
{
    public class AlertMappingProfile : Profile
    {
        public AlertMappingProfile()
        {
            CreateMap<HealthMetricsRequestDto, HealthMetric>();

            CreateMap<CreateAlertRequest, Alert>()
                .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
                .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
                .ForAllMembers(opt => opt.Ignore());

            CreateMap<AlertTrigger, AlertTriggerDto>()
                .ForMember(dest => dest.Type,
                           opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<HealthMetric, HealthMetricsDto>();

            CreateMap<Alert, AlertDetailsDto>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Triggers, opt => opt.MapFrom(src => src.Triggers))
                .ForMember(dest => dest.Bracelet, opt => opt.MapFrom(src => src.Bracelet))
                .ForMember(dest => dest.HealthMetrics, opt => opt.MapFrom(src => src.HealthMetric));

            CreateMap<Alert, AlertSummaryDto>()
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Triggers,
                           opt => opt.MapFrom(src => src.Triggers))
                .ForMember(dest => dest.UserFullName,
                           opt => opt.MapFrom(src => src.Bracelet.User.FullName))
                .ForMember(dest => dest.BraceletSerialNumber,
                           opt => opt.MapFrom(src => src.Bracelet.SerialNumber));
        }
    }
}