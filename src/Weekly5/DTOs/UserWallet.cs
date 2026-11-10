namespace Weekly5.DTOs;

public record UserWallet(
  int UserId,
  string Name,
  string Email,
  string Phone,
  int WalletId,
  string WalletNumber,
  decimal Balance
);