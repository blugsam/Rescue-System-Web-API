using AutoMapper;
using AutoMapper.QueryableExtensions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Contracts.Contracts.Enums;
using RescueSystem.Application.Exceptions;
using RescueSystem.Application.Interfaces;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Infrastructure;

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
                    new() { Id = Guid.NewGuid(), ErrorMessage = $"Получен сигнал от неизвестного браслета: {request.SerialNumber}" }
                }
            };
            _dbContext.Alerts.Add(grayAlert);
            await _dbContext.SaveChangesAsync();

            return _mapper.Map<AlertDetailsDto>(grayAlert);
        }

        var validationResult = await _validator.ValidateAsync(request);

        var alert = _mapper.Map<Alert>(request);

        alert.Id = Guid.NewGuid();
        alert.Timestamp = DateTimeOffset.UtcNow;
        alert.Status = AlertProcessingStatus.New;
        alert.BraceletId = bracelet.Id;

        if (request.HealthMetrics != null)
        {
            alert.HealthMetric = _mapper.Map<HealthMetric>(request.HealthMetrics);
            alert.HealthMetric.Id = Guid.NewGuid();
        }

        var defaultProfile = await _dbContext.HealthProfileThresholds
            .AsNoTracking()
            .SingleAsync(p => p.ProfileName == "Default");
        var userThresholds = bracelet.User.HealthProfile ?? defaultProfile;
        alert.Triggers = DetermineAlertTriggers(request, userThresholds, defaultProfile);

        if (validationResult.IsValid)
        {
            alert.QualityLevel = AlertQualityLevel.Red;
        }
        else
        {
            alert.QualityLevel = AlertQualityLevel.Yellow;
            alert.ValidationErrors = validationResult.Errors
                .Select(error => new AlertValidationError
                {
                    Id = Guid.NewGuid(),
                    ErrorMessage = error.ErrorMessage
                })
                .ToList();
        }

        _dbContext.Alerts.Add(alert);
        await _dbContext.SaveChangesAsync();

        var alertSummary = _mapper.Map<AlertSummaryDto>(alert);
        await _alertNotifier.NotifyNewAlertAsync(alertSummary);
        _logger.LogInformation("Уведомление о новой тревоге {AlertId} отправлено всем клиентам.", alert.Id);

        alert.Bracelet = bracelet;
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

    public async Task<IEnumerable<AlertSummaryDto>> GetAllAlertsSummaryAsync()
        => await _dbContext.Alerts.AsNoTracking().OrderByDescending(a => a.Timestamp)
            .ProjectTo<AlertSummaryDto>(_mapper.ConfigurationProvider).ToListAsync();

    public async Task<AlertDetailsDto?> GetAlertDetailsByIdAsync(Guid id)
        => await _dbContext.Alerts.AsNoTracking().Where(a => a.Id == id)
            .ProjectTo<AlertDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefaultAsync();
}