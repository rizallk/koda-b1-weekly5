using Weekly5.DTOs;
using Weekly5.Models;

namespace Weekly5.Repositories;

public interface ITransactionRepository
{
  Task<bool> TryAddAsync(Transaction trx, CancellationToken cancellationToken);

  Task<IReadOnlyList<TransactionSummary>> GetTransactionSummaryByWalletId(int walletId, CancellationToken cancellationToken);
}