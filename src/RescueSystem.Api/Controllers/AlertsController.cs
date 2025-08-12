using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Services.AlertService;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Domain.Common;

namespace RescueSystem.Api.Controllers;

[ApiController]
[Route("api/alerts")]
public class AlertsController : ControllerBase
{
    private readonly IAlertService _alertService;

    public AlertsController(IAlertService alertService) => _alertService = alertService;

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<AlertSummaryDto>>> GetAll([FromQuery] PaginationQueryParameters queryParams)
    {
        var pagedResult = await _alertService.GetAllAlertsSummaryAsync(queryParams);
        return Ok(pagedResult);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertDetailsDto>> GetById(Guid id)
    {
        var dto = await _alertService.GetAlertDetailsByIdAsync(id);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AlertDetailsDto>> Create([FromBody] CreateAlertRequestDto request)
    {
        var created = await _alertService.CreateAlertFromSignalAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _alertService.DeleteAlertAsync(id);

        return NoContent();
    }
}