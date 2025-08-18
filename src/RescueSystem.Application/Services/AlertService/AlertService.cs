using System.Text.Json;
using FluentValidation.Results;
using FluentValidation;
using Microsoft.Extensions.Logging;
using RescueSystem.Application.Interfaces;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Constants;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Application.Mapping;
using RescueSystem.Domain.Common;

namespace RescueSystem.Application.Services.AlertService;

public class AlertService : IAlertService
{
    private readonly IAlertRepository _alertRepository;
    private readonly IBraceletRepository _braceletRepository;
    private readonly IHealthProfileThresholdsRepository _healthProfileThresholdsRepository;
    private readonly ILogger<AlertService> _logger;
    private readonly IValidator<CreateAlertRequestDto> _validator;
    private readonly IAlertNotifier _alertNotifier;

    public AlertService(
        IAlertRepository alertRepository,
        IBraceletRepository braceletRepository,
        IHealthProfileThresholdsRepository healthProfileThresholdsRepository,
        IValidator<CreateAlertRequestDto> validator, 
        ILogger<AlertService> logger, 
        IAlertNotifier alertNotifier)
    {
        _alertRepository = alertRepository;
        _braceletRepository = braceletRepository;
        _healthProfileThresholdsRepository = healthProfileThresholdsRepository;
        _validator = validator;
        _logger = logger;
        _alertNotifier = alertNotifier;
    }

    public async Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequestDto request)
    {
        var bracelet = await _braceletRepository.GetBraceletBySerialNumber(request.SerialNumber);

        if (bracelet == null)
        {
            return await CreateGrayAlertAsync(request);
        }

        return await CreateRedOrYellowAlertAsync(request, bracelet);
    }

    private async Task<AlertDetailsDto> CreateRedOrYellowAlertAsync(CreateAlertRequestDto request, Bracelet bracelet)
    {
        var validationResult = await _validator.ValidateAsync(request);

        var alert = request.ToEntity();
        alert.Id = Guid.NewGuid();
        alert.Timestamp = DateTimeOffset.UtcNow;
        alert.Status = AlertProcessingStatus.New;
        alert.BraceletId = bracelet.Id;

        if (validationResult.IsValid)
        {
            alert.QualityLevel = AlertQualityLevel.Red;
        }
        else
        {
            alert.QualityLevel = AlertQualityLevel.Yellow;
            alert.ValidationErrors = validationResult.Errors
                .Select(error => new AlertValidationError { Id = Guid.NewGuid(), ErrorMessage = error.ErrorMessage })
                .ToList();
        }

        if (request.HealthMetrics != null)
        {
            alert.HealthMetric = CreateHealthMetric(request, validationResult);
        }

        var defaultProfile = await _healthProfileThresholdsRepository.FindAsync(p => p.ProfileName == HealthProfileThresholdsConstants.DefaultProfileName);
        if (defaultProfile == null)
        {
            throw new InvalidOperationException("Default health profile not found.");
        }
        var userThresholds = bracelet.User?.HealthProfile ?? defaultProfile;
        alert.Triggers = DetermineAlertTriggers(request, userThresholds, defaultProfile);

        await _alertRepository.AddAsync(alert);
        await _alertRepository.SaveChangesAsync();
        _logger.LogInformation("Created new alert {AlertId} with quality {QualityLevel}.", alert.Id, alert.QualityLevel);

        alert.Bracelet = bracelet;
        var alertSummary = alert.ToSummaryDto();
        await _alertNotifier.NotifyNewAlertAsync(alertSummary);
        _logger.LogInformation("New alert notification {AlertId} sended.", alert.Id);

        return alert.ToDetailsDto();
    }

    private HealthMetric CreateHealthMetric(CreateAlertRequestDto request, ValidationResult validationResult)
    {
        var healthMetric = new HealthMetric
        {
            Id = Guid.NewGuid(),
            RawDataJson = JsonSerializer.Serialize(request.HealthMetrics)
        };

        bool pulseIsInvalid = validationResult.Errors
            .Any(e => e.PropertyName == $"{nameof(CreateAlertRequestDto.HealthMetrics)}.{nameof(HealthMetricsRequestDto.Pulse)}");

        if (!pulseIsInvalid && request.HealthMetrics?.Pulse.HasValue == true)
        {
            healthMetric.Pulse = request.HealthMetrics.Pulse.Value;
        }

        bool tempIsInvalid = validationResult.Errors
            .Any(e => e.PropertyName == $"{nameof(CreateAlertRequestDto.HealthMetrics)}.{nameof(HealthMetricsRequestDto.BodyTemperature)}");

        if (!tempIsInvalid && request.HealthMetrics?.BodyTemperature.HasValue == true)
        {
            healthMetric.BodyTemperature = request.HealthMetrics.BodyTemperature.Value;
        }

        return healthMetric;
    }

    private async Task<AlertDetailsDto> CreateGrayAlertAsync(CreateAlertRequestDto request)
    {
        var grayAlert = new Alert
        {
            Id = Guid.NewGuid(),
            Timestamp = DateTimeOffset.UtcNow,
            Latitude = request.Latitude,
            Longitude = request.Longitude,
            Status = AlertProcessingStatus.New,
            QualityLevel = AlertQualityLevel.Gray,
            ValidationErrors = new List<AlertValidationError>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    ErrorMessage = $"Received signal from unknown bracelet: {request.SerialNumber}"
                }
            }
        };

        await _alertRepository.AddAsync(grayAlert);
        await _alertRepository.SaveChangesAsync();

        var grayAlertDto = grayAlert.ToDetailsDto();
        await _alertNotifier.NotifyNewAlertAsync(grayAlert.ToSummaryDto());
        return grayAlertDto;
    }

    private ICollection<AlertTrigger> DetermineAlertTriggers(CreateAlertRequestDto request, HealthProfileThresholds? userThresholds, HealthProfileThresholds? defaultThresholds)
    {
        var triggers = new List<AlertTrigger>();
        if (request.IsSosSignal)
        {
            triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.SosButton });
        }

        if (request.HealthMetrics?.Pulse is double pulse)
        {
            if (pulse > (userThresholds?.HighPulseThreshold ?? defaultThresholds?.HighPulseThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.HighPulse });
            if (pulse < (userThresholds?.LowPulseThreshold ?? defaultThresholds?.LowPulseThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.LowPulse });
        }
        
        if (request.HealthMetrics?.BodyTemperature is double temp)
        {
            if (temp > (userThresholds?.HighTempThreshold ?? defaultThresholds?.HighTempThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.HighTemperature });
            if (temp < (userThresholds?.LowTempThreshold ?? defaultThresholds?.LowTempThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.LowTemperature });
        }

        return triggers;
    }

    public async Task DeleteAlertAsync(Guid alertId)
    {
        _alertRepository.RemoveById(alertId);
        await _alertRepository.SaveChangesAsync();
        _logger.LogWarning("Alert with ID {AlertId} was deleted.", alertId);
    }

    public async Task<PagedResult<AlertSummaryDto>> GetAllAlertsSummaryAsync(PaginationQueryParameters queryParams)
    {
        var alerts = await _alertRepository.GetAllAsync();
        var totalCount = alerts.Count();

        var items = alerts
            .OrderByDescending(a => a.Timestamp)
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ToList();

        var dtos = items.Select(i => i.ToSummaryDto()).ToList();

        return new PagedResult<AlertSummaryDto>
        {
            Items = dtos,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id)
    {
        var alert = await _alertRepository.GetByIdAsync(id);
        return alert?.ToDetailsDto();
    }
}
