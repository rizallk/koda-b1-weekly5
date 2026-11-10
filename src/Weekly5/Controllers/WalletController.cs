using Microsoft.AspNetCore.Mvc;
using Weekly5.DTOs;
using Weekly5.Services;

namespace Weekly5.Controllers;

[ApiController]
[Route("api/wallets")]

public sealed class WalletController : ControllerBase
{
  private readonly IWalletService _service;

  public WalletController(IWalletService service)
  {
    _service = service;
  }

  [HttpGet("{userId}")]
  [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<UserResponse>> GetByUserId(
      [FromRoute] int userId, CancellationToken cancellationToken)
  {
    var wallet = await _service.GetByUserIdAsync(userId, cancellationToken);

    return wallet is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Wallet not found") : Ok(wallet);
  }

  // Top Up
  [HttpPost("{userId}/topups")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult<WalletResponse>> TopUp(
      [FromRoute] int userId, [FromBody] WalletRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var updated = await _service.UpdateBalanceAsync(userId, request, cancellationToken);

      return updated is null ? Problem(statusCode: StatusCodes.Status404NotFound, title: "Wallet not found", detail: $"No wallet was found for user id {userId}.") : Ok(updated);
    }
    catch (ArgumentException e)
    {
      return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          title: "Invalid wallet data!",
          detail: e.Message);
    }
  }
}