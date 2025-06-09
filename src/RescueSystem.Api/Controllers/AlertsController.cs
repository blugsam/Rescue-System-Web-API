using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Application.Services;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService) => _alertService = alertService;

    /// <summary>Получить список всех тревог.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<AlertSummaryDto>>> GetAll()
    {
        var list = await _alertService.GetAllAlertsSummaryAsync();
        return Ok(list);
    }

    /// <summary>Получить детали тревоги по идентификатору.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDetailsDto>> GetById(Guid id)
    {
        var dto = await _alertService.GetAlertDetailsByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>Создать новую тревогу (получить сигнал от браслета).</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertDetailsDto>> Create([FromBody] CreateAlertRequest request)
    {
        var created = await _alertService.CreateAlertFromSignalAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
}
