using Microsoft.AspNetCore.Mvc;
using Weekly5.DTOs;
using Weekly5.Services;

namespace Weekly5.Controllers;

[ApiController]
[Route("api/transfers")]

public sealed class TransferController : ControllerBase
{
  private readonly ITransferService _service;

  public TransferController(ITransferService service)
  {
    _service = service;
  }

  [HttpPost]
  [ProducesResponseType(StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
  [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
  public async Task<ActionResult> Transfer(
      [FromBody] TransferRequest request, CancellationToken cancellationToken)
  {
    try
    {
      var updated = await _service.TransferAsync(request, cancellationToken);

      if (updated is null)
      {
        return Problem(
            statusCode: StatusCodes.Status404NotFound,
            title: "Wallet not found",
            detail: $"Sender wallet ({request.SenderWalletId}) or Receiver wallet ({request.ReceiverWalletId}) was not found.");
      }

      return Ok(updated);
    }
    catch (ArgumentException e)
    {
      return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          title: "Invalid transfer data",
          detail: e.Message);
    }
    catch (InvalidOperationException e)
    {
      return Problem(
          statusCode: StatusCodes.Status400BadRequest,
          title: "Transfer failed",
          detail: e.Message);
    }
  }
}