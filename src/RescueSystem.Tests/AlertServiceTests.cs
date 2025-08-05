using Moq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using FluentValidation;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Domain.Entities.Bracelets;
using RescueSystem.Domain.Entities.Alerts;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Entities.Health;
using RescueSystem.Domain.Interfaces;
using RescueSystem.Application.Interfaces;
using RescueSystem.Application.Services.AlertService;
using FluentValidation.Results;
using RescueSystem.Domain.Constants;

namespace RescueSystem.Tests;

public class AlertServiceTests
{
    private readonly Mock<IRepository<Alert>> _alertRepositoryMock;
    private readonly Mock<IRepository<Bracelet>> _braceletRepositoryMock;
    private readonly Mock<IRepository<HealthProfileThresholds>> _healthProfileThresholdsRepositoryMock;
    private readonly Mock<ILogger<AlertService>> _loggerMock;
    private readonly Mock<IValidator<CreateAlertRequestDto>> _validatorMock;
    private readonly Mock<IAlertNotifier> _alertNotifierMock;
    private readonly AlertService _alertService;

    public AlertServiceTests()
    {
        _alertRepositoryMock = new Mock<IRepository<Alert>>();
        _braceletRepositoryMock = new Mock<IRepository<Bracelet>>();
        _healthProfileThresholdsRepositoryMock = new Mock<IRepository<HealthProfileThresholds>>();
        _loggerMock = new Mock<ILogger<AlertService>>();
        _validatorMock = new Mock<IValidator<CreateAlertRequestDto>>();
        _alertNotifierMock = new Mock<IAlertNotifier>();

        _alertService = new AlertService(
            _alertRepositoryMock.Object,
            _braceletRepositoryMock.Object,
            _healthProfileThresholdsRepositoryMock.Object,
            _validatorMock.Object,
            _loggerMock.Object,
            _alertNotifierMock.Object);

        _healthProfileThresholdsRepositoryMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<HealthProfileThresholds, bool>>>()))
            .ReturnsAsync(new List<HealthProfileThresholds> { new HealthProfileThresholds { ProfileName = HealthProfileThresholdsConstants.DefaultProfileName } });
    }

    [Fact]
    public async Task CreateAlertFromSignalAsync_WithUnknownBracelet_ShouldCreateGrayAlert()
    {
        var request = new CreateAlertRequestDto { SerialNumber = "unknown_serial" };
        _braceletRepositoryMock.Setup(r => r.FindAsync(b => b.SerialNumber == request.SerialNumber)).ReturnsAsync(new List<Bracelet>());

        var result = await _alertService.CreateAlertFromSignalAsync(request);

        result.Should().NotBeNull();
        result.QualityLevel.Should().Be(Contracts.Contracts.Enums.AlertQualityLevel.Gray.ToString());
        _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a => a.QualityLevel == Contracts.Contracts.Enums.AlertQualityLevel.Gray)), Times.Once);
        _alertRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _alertNotifierMock.Verify(n => n.NotifyNewAlertAsync(It.IsAny<AlertSummaryDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateAlertFromSignalAsync_WithValidRequest_ShouldCreateRedAlert()
    {
        var request = new CreateAlertRequestDto { SerialNumber = "known_serial" };
        var bracelet = new Bracelet { Id = Guid.NewGuid(), SerialNumber = "known_serial" };
        _braceletRepositoryMock.Setup(r => r.FindAsync(b => b.SerialNumber == request.SerialNumber)).ReturnsAsync(new List<Bracelet> { bracelet });
        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult());

        var result = await _alertService.CreateAlertFromSignalAsync(request);

        result.Should().NotBeNull();
        result.QualityLevel.Should().Be(Contracts.Contracts.Enums.AlertQualityLevel.Red.ToString());
        _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a => a.QualityLevel == Contracts.Contracts.Enums.AlertQualityLevel.Red)), Times.Once);
        _alertRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _alertNotifierMock.Verify(n => n.NotifyNewAlertAsync(It.IsAny<AlertSummaryDto>()), Times.Once);
    }

    [Fact]
    public async Task CreateAlertFromSignalAsync_WithInvalidRequest_ShouldCreateYellowAlert()
    {
        var request = new CreateAlertRequestDto { SerialNumber = "known_serial" };
        var bracelet = new Bracelet { Id = Guid.NewGuid(), SerialNumber = "known_serial" };
        _braceletRepositoryMock.Setup(r => r.FindAsync(b => b.SerialNumber == request.SerialNumber)).ReturnsAsync(new List<Bracelet> { bracelet });
        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>())).ReturnsAsync(new ValidationResult(new[] { new ValidationFailure("prop", "error") }));

        var result = await _alertService.CreateAlertFromSignalAsync(request);

        result.Should().NotBeNull();
        result.QualityLevel.Should().Be(Contracts.Contracts.Enums.AlertQualityLevel.Yellow.ToString());
        _alertRepositoryMock.Verify(r => r.AddAsync(It.Is<Alert>(a => a.QualityLevel == Contracts.Contracts.Enums.AlertQualityLevel.Yellow)), Times.Once);
        _alertRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _alertNotifierMock.Verify(n => n.NotifyNewAlertAsync(It.IsAny<AlertSummaryDto>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAlertAsync_WithExistingAlert_ShouldDeleteAlert()
    {
        var alertId = Guid.NewGuid();
        var alert = new Alert { Id = alertId };
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alertId)).ReturnsAsync(alert);

        await _alertService.DeleteAlertAsync(alertId);

        _alertRepositoryMock.Verify(r => r.Remove(alert), Times.Once);
        _alertRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAllAlertsSummaryAsync_ShouldReturnPagedResultOfAlerts()
    {
        var alerts = new List<Alert> { new Alert(), new Alert() }.AsQueryable();
        _alertRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(alerts);

        var result = await _alertService.GetAllAlertsSummaryAsync(new PaginationQueryParameters());

        result.Should().NotBeNull();
        result.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAlertDetailsByIdAsync_WithExistingAlert_ShouldReturnAlertDetails()
    {
        var alertId = Guid.NewGuid();
        var alert = new Alert { Id = alertId };
        _alertRepositoryMock.Setup(r => r.GetByIdAsync(alertId)).ReturnsAsync(alert);

        var result = await _alertService.GetAlertDetailsByIdAsync(alertId);

        result.Should().NotBeNull();
        result.Id.Should().Be(alertId);
    }
}
