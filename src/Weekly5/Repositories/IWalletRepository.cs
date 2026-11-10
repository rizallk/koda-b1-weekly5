using Weekly5.Models;

namespace Weekly5.Repositories;

public interface IWalletRepository
{
  Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
  Task<bool> TryAddAsync(Wallet wallet, CancellationToken cancellationToken);
  Task<bool> TryUpdateBalanceAsync(Wallet wallet, CancellationToken cancellationToken);
  Task<string?> GetLastWalletNumberAsync(CancellationToken cancellationToken);
}