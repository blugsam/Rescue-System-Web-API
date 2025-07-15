using Microsoft.AspNetCore.Mvc;
using RescueSystem.Contracts.Contracts.Requests;
using RescueSystem.Contracts.Contracts.Responses;
using RescueSystem.Application.Services.BraceletService;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BraceletsController : ControllerBase
{
    private readonly IBraceletService _braceletService;

    public BraceletsController(IBraceletService braceletService)
    {
        _braceletService = braceletService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BraceletDetailsDto>> Create([FromBody] CreateBraceletRequestDto request)
    {
        var createdBracelet = await _braceletService.CreateBraceletAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdBracelet.Id }, createdBracelet);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<BraceletDto>>> GetAll([FromQuery] PaginationQueryParameters queryParams)
    {
        var paged = await _braceletService.GetAllBraceletsAsync(queryParams);
        return Ok(paged);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BraceletDetailsDto>> GetById(Guid id)
    {
        var dto = await _braceletService.GetBraceletByIdAsync(id);
        if (dto == null)
            return NotFound();
        return Ok(dto);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _braceletService.DeleteBraceletAsync(id);
        return NoContent();
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateBraceletRequestDto request)
    {
        await _braceletService.UpdateBraceletStatusAsync(id, request);
        return NoContent();
    }

    [HttpPost("{id:guid}/assignment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AssignUser(Guid id, [FromBody] AssignUserToBraceletRequest request)
    {
        await _braceletService.AssignUserToBraceletAsync(id, request.UserId);
        return NoContent();
    }

    [HttpDelete("{id:guid}/assignment")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UnassignUser(Guid id)
    {
        await _braceletService.UnassignUserFromBraceletAsync(id);
        return NoContent();
    }
}
