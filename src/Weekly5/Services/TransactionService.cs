using System.Transactions;
using Weekly5.DTOs;
using Weekly5.Repositories;

namespace Weekly5.Services;

public sealed class TransactionService : ITransactionService
{
  private readonly ITransactionRepository _transactionRepository;

  public TransactionService(ITransactionRepository transactionRepository)
  {
    _transactionRepository = transactionRepository;
  }

  public async Task<TotalTransaction?> GetTransactionSummaryByWalletIdAsync(int walletId, CancellationToken cancellationToken)
  {
    var summaries = await _transactionRepository.GetTransactionSummaryByWalletId(walletId, cancellationToken);

    if (summaries.Count == 0)
    {
      return null;
    }

    return new TotalTransaction(
            Income: summaries.FirstOrDefault(x => string.Equals(x.Type, "Income", StringComparison.OrdinalIgnoreCase))?.TotalAmount ?? 0m,
            Expense: summaries.FirstOrDefault(x => string.Equals(x.Type, "Expense", StringComparison.OrdinalIgnoreCase))?.TotalAmount ?? 0m,
            TopUp: summaries.FirstOrDefault(x => string.Equals(x.Type, "TopUp", StringComparison.OrdinalIgnoreCase))?.TotalAmount ?? 0m
        );
  }
}