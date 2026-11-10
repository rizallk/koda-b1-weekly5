using Weekly5.DTOs;

namespace Weekly5.Services;

public interface ITransactionService
{
  Task<TotalTransaction?> GetTransactionSummaryByWalletIdAsync(int walletId, CancellationToken cancellationToken);
}