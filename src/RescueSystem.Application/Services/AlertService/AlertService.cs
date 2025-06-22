using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RescueSystem.Application.Exceptions;
using RescueSystem.Application.Interfaces;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Infrastructure;
using System.Text.Json;

namespace RescueSystem.Application.Services.AlertService;

public class AlertService : IAlertService
{
    private readonly RescueDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly ILogger<AlertService> _logger;
    private readonly IValidator<CreateAlertRequest> _validator;
    private readonly IAlertNotifier _alertNotifier;

    public AlertService(RescueDbContext dbContext, IMapper mapper, IValidator<CreateAlertRequest> validator, ILogger<AlertService> logger, IAlertNotifier alertNotifier)
    {
        _dbContext = dbContext;
        _mapper = mapper;
        _validator = validator;
        _logger = logger;
        _alertNotifier = alertNotifier;
    }

    public async Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequest request)
    {
        var bracelet = await _dbContext.Bracelets
            .Include(b => b.User).ThenInclude(u => u.HealthProfile)
            .FirstOrDefaultAsync(b => b.SerialNumber == request.SerialNumber);

        if (bracelet == null)
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

            _dbContext.Alerts.Add(grayAlert);
            await _dbContext.SaveChangesAsync();

            var grayAlertDto = _mapper.Map<AlertDetailsDto>(grayAlert);
            await _alertNotifier.NotifyNewAlertAsync(_mapper.Map<AlertSummaryDto>(grayAlert));
            return grayAlertDto;
        }

        var validationResult = await _validator.ValidateAsync(request);

        var alert = _mapper.Map<Alert>(request);
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
            var healthMetric = new HealthMetric
            {
                Id = Guid.NewGuid(),
                RawDataJson = JsonSerializer.Serialize(request.HealthMetrics)
            };

            bool pulseIsInvalid = validationResult.Errors
                .Any(e => e.PropertyName == $"{nameof(CreateAlertRequest.HealthMetrics)}.{nameof(HealthMetricsRequestDto.Pulse)}");

            if (!pulseIsInvalid && request.HealthMetrics.Pulse.HasValue)
            {
                healthMetric.Pulse = request.HealthMetrics.Pulse.Value;
            }

            bool tempIsInvalid = validationResult.Errors
                .Any(e => e.PropertyName == $"{nameof(CreateAlertRequest.HealthMetrics)}.{nameof(HealthMetricsRequestDto.BodyTemperature)}");

            if (!tempIsInvalid && request.HealthMetrics.BodyTemperature.HasValue)
            {
                healthMetric.BodyTemperature = request.HealthMetrics.BodyTemperature.Value;
            }

            alert.HealthMetric = healthMetric;
        }

        var defaultProfile = await _dbContext.HealthProfileThresholds.AsNoTracking().SingleAsync(p => p.ProfileName == "Default");
        var userThresholds = bracelet.User.HealthProfile ?? defaultProfile;
        alert.Triggers = DetermineAlertTriggers(request, userThresholds, defaultProfile);

        _dbContext.Alerts.Add(alert);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("Created new alert {AlertId} with quality {QualityLevel}.", alert.Id, alert.QualityLevel);

        alert.Bracelet = bracelet;
        var alertSummary = _mapper.Map<AlertSummaryDto>(alert);
        await _alertNotifier.NotifyNewAlertAsync(alertSummary);
        _logger.LogInformation("New alert notification {AlertId} sended.", alert.Id);

        return _mapper.Map<AlertDetailsDto>(alert);
    }

    private ICollection<AlertTrigger> DetermineAlertTriggers(CreateAlertRequest request, HealthProfileThresholds userThresholds, HealthProfileThresholds defaultThresholds)
    {
        var triggers = new List<AlertTrigger>();
        if (request.IsSosSignal)
        {
            triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.SosButton });
        }

        if (request.HealthMetrics?.Pulse is double pulse)
        {
            if (pulse > (userThresholds.HighPulseThreshold ?? defaultThresholds.HighPulseThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.HighPulse });
            if (pulse < (userThresholds.LowPulseThreshold ?? defaultThresholds.LowPulseThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.LowPulse });
        }
        
        if (request.HealthMetrics?.BodyTemperature is double temp)
        {
            if (temp > (userThresholds.HighTempThreshold ?? defaultThresholds.HighTempThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.HighTemperature });
            if (temp < (userThresholds.LowTempThreshold ?? defaultThresholds.LowTempThreshold))
                triggers.Add(new AlertTrigger { Id = Guid.NewGuid(), Type = AlertType.LowTemperature });
        }

        return triggers;
    }

    public async Task DeleteAlertAsync(Guid alertId)
    {
        var alert = await _dbContext.Alerts.FirstOrDefaultAsync(a => a.Id == alertId);

        if (alert == null)
        {
            throw new NotFoundException($"Тревога с ID '{alertId}' не найдена.");
        }

        _dbContext.Alerts.Remove(alert);

        await _dbContext.SaveChangesAsync();

        _logger.LogWarning("Тревога {AlertId} была удалена.", alertId);
    }

    public async Task<PagedResult<AlertSummaryDto>> GetAllAlertsSummaryAsync(PaginationQueryParameters queryParams)
    {
        var query = _dbContext.Alerts.AsNoTracking();

        //фильтрацию по статусу или качеству
        //if (!string.IsNullOrWhiteSpace(queryParams.SearchTerm)) ...

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
            .Take(queryParams.PageSize)
            .ProjectTo<AlertSummaryDto>(_mapper.ConfigurationProvider)
            .ToListAsync();

        return new PagedResult<AlertSummaryDto>
        {
            Items = items,
            PageNumber = queryParams.PageNumber,
            PageSize = queryParams.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id)
        => await _dbContext.Alerts.AsNoTracking().Where(a => a.Id == id)
            .ProjectTo<AlertDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
}