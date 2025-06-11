using Microsoft.AspNetCore.Mvc;
using RescueSystem.Application.Contracts;
using RescueSystem.Application.Contracts.Requests;
using RescueSystem.Application.Contracts.Responses;
using RescueSystem.Application.Services.UserService;
using RescueSystem.Domain.Entities;

namespace RescueSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Создать нового пользователя.</summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserDetailsDto>> Create([FromBody] CreateUserRequest request)
    {
        var createdUser = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    /// <summary>Получить информацию о пользователе по ID.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDetailsDto>> GetById(Guid id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
            return NotFound();
        return Ok(user);
    }

    /// <summary>Получить список пользователей с пагинацией.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserSummaryDto>>> GetAll([FromQuery] PaginationQueryParameters queryParams)
    {
        var pagedResult = await _userService.GetAllUsersAsync(queryParams);
        return Ok(pagedResult);
    }

    /// <summary>Обновить данные пользователя.</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
    {
        await _userService.UpdateUserAsync(id, request);
        return NoContent();
    }

    /// <summary>Удалить пользователя.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _userService.DeleteUserAsync(id);
        return NoContent();
    }
}
