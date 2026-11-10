using Weekly5.Models;

namespace Weekly5.DTOs;

public sealed record WalletResponse(
  int Id,
  int UserId,
  string WalletNumber,
  decimal Balance
)
{
  public static WalletResponse FromModel(Wallet wallet) => new(wallet.Id, wallet.UserId, wallet.WalletNumber, wallet.Balance);
}