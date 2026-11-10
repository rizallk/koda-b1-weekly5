using Microsoft.AspNetCore.Mvc;
using Weekly5.DTOs;
using Weekly5.Models;
using Weekly5.Services;

namespace Weekly5.Controllers;

[ApiController]
[Route("api/users")]

public sealed class UserController : ControllerBase
{
  private readonly IUserService _service;

  public UserController(IUserService service)
  {
    _service = service;
  }

  [HttpGet]
  [ProducesResponseType(
      typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IReadOnlyList<User>>> GetAll(CancellationToken cancellationToken)
  {
    var users = await _service.GetAllAsync(cancellationToken);
    return Ok(users);
  }

  [HttpGet("{id}/transactions")]
  [ProducesResponseType(
      typeof(IReadOnlyList<UserResponse>), StatusCodes.Status200OK)]
  public async Task<ActionResult<IReadOnlyList<UserTransaction>>> GetUserTransactionById([FromRoute] int id, CancellationToken cancellationToken)
  {
    var userTransactions = await _service.GetUserTransactionByUserIdAsync(id, cancellationToken);
    return Ok(userTransactions);
  }

  [HttpGet("{id}")]
  [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<UserResponse>> GetByCode(
      [FromRoute] int id, CancellationToken cancellationToken)
  {
    var user = await _service.GetByIdAsync(id, cancellationToken);

    return user is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "User not found") : Ok(user);
  }

  [HttpPost]
  [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<UserResponse>> Create([FromBody] UserRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var created = await _service.CreateAsync(request, cancellationToken);

      if (created is null)
      {
        return Problem(statusCode: StatusCodes.Status409Conflict, title: "User already exists");
      }

      return CreatedAtAction(
          nameof(GetByCode),
          new { id = created.Id }, created);
    }
    catch (ArgumentException e)
    {
      return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          title: "Invalid user data!",
          detail: e.Message);
    }
  }

  [HttpPut("{id}")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<UserResponse>> Update(
      [FromRoute] int id, [FromBody] UserRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var updated = await _service.UpdateAsync(id, request, cancellationToken);

      return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "User not found", detail: $"No user was found for id {id}.") : Ok(updated);
    }
    catch (ArgumentException e)
    {
      return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          title: "Invalid user data!",
          detail: e.Message);
    }
  }
}