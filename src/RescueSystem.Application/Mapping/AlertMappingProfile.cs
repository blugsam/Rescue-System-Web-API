using AutoMapper;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;

namespace RescueSystem.Application.Mappings;

public class AlertMappingProfile : Profile
{
    public AlertMappingProfile()
    {
        CreateMap<CreateAlertRequest, Alert>()
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Timestamp, opt => opt.Ignore())
            .ForMember(dest => dest.Triggers, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.BraceletId, opt => opt.Ignore())
            .ForMember(dest => dest.Bracelet, opt => opt.Ignore())
            .ForMember(dest => dest.HealthMetric, opt => opt.Ignore())
            .ForMember(dest => dest.QualityLevel, opt => opt.Ignore())
            .ForMember(dest => dest.ValidationErrors, opt => opt.Ignore());

        CreateMap<AlertTrigger, AlertTriggerDto>()
            .ForMember(dest => dest.Type,
                       opt => opt.MapFrom(src => src.Type.ToString()));

        CreateMap<HealthMetricsRequestDto, HealthMetric>();

        CreateMap<HealthMetric, HealthMetricsDto>();

        CreateMap<Alert, AlertDetailsDto>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.QualityLevel,
                       opt => opt.MapFrom(src => src.QualityLevel.ToString()))
            .ForMember(dest => dest.ValidationErrors,
                       opt => opt.MapFrom(src => src.ValidationErrors.Select(ve => ve.ErrorMessage)))
            .ForMember(dest => dest.Triggers,
                       opt => opt.MapFrom(src => src.Triggers))
            .ForMember(dest => dest.Bracelet,
                       opt => opt.MapFrom(src => src.Bracelet))
            .ForMember(dest => dest.HealthMetrics,
                       opt => opt.MapFrom(src => src.HealthMetric));

        CreateMap<Alert, AlertSummaryDto>()
            .ForMember(dest => dest.Status,
                       opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(dest => dest.QualityLevel,
                       opt => opt.MapFrom(src => src.QualityLevel.ToString()))
            .ForMember(dest => dest.Triggers,
                       opt => opt.MapFrom(src => src.Triggers))
            .ForMember(dest => dest.UserFullName,
                       opt => opt.MapFrom(src => src.Bracelet != null && src.Bracelet.User != null ? src.Bracelet.User.FullName : string.Empty))
            .ForMember(dest => dest.BraceletSerialNumber,
                       opt => opt.MapFrom(src => src.Bracelet != null ? src.Bracelet.SerialNumber : string.Empty));
    }
}
