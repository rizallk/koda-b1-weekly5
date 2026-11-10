using Weekly5.Models;

namespace Weekly5.DTOs;

public sealed record TransactionResponse(
  int? Id,
  int? SenderWalletId,
  int ReceiverWalletId,
  string Type,
  decimal Amount,
  string Description,
  DateTime CreatedAt
)
{
  public static TransactionResponse FromModel(Transaction trx) => new(trx.Id, trx.SenderWalletId, trx.ReceiverWalletId, trx.Type, trx.Amount, trx.Description, trx.CreatedAt);
}