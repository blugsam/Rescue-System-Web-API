using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Application.Interfaces;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Entities.Health.RescueSystem.Domain.Entities;
using RescueSystem.Infrastructure;

namespace RescueSystem.Application.Services
{
    public class AlertService : IAlertService
    {
        private readonly RescueDbContext _dbContext;
        private readonly IMapper _mapper;

        public AlertService(RescueDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<AlertDetailsDto> CreateAlertFromSignalAsync(CreateAlertRequest request)
        {
            var bracelet = await _dbContext.Bracelets
                .Include(b => b.User)
                    .ThenInclude(u => u.HealthProfile)
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.SerialNumber == request.SerialNumber);

            if (bracelet == null)
            {
                throw new KeyNotFoundException($"Браслет с серийным номером '{request.SerialNumber}' не найден.");
            }

            var defaultProfile = await _dbContext.HealthProfileThresholds
                .AsNoTracking()
                .SingleAsync(p => p.ProfileName == "Default");

            var alert = _mapper.Map<Alert>(request);
            alert.Id = Guid.NewGuid();
            alert.Timestamp = DateTimeOffset.UtcNow;
            alert.Status = AlertProcessingStatus.New;
            alert.BraceletId = bracelet.Id;

            var userThresholds = bracelet.User?.HealthProfile ?? defaultProfile;
            alert.Triggers = DetermineAlertTriggers(request, userThresholds, defaultProfile);

            if (request.HealthMetrics != null)
            {
                alert.HealthMetric = _mapper.Map<HealthMetric>(request.HealthMetrics);
                alert.HealthMetric.Id = Guid.NewGuid();
            }

            _dbContext.Alerts.Add(alert);
            await _dbContext.SaveChangesAsync();

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
    }
}