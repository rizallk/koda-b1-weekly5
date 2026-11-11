using System.Transactions;
using Weekly5.DTOs;
using Weekly5.Models;
using Weekly5.Repositories;

namespace Weekly5.Services;

public sealed class WalletService : IWalletService
{
  private readonly IWalletRepository _walletRepository;
  private readonly ITransactionRepository _transactionRepository;

  public WalletService(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
  {
    _walletRepository = walletRepository;
    _transactionRepository = transactionRepository;
  }

  public async Task<WalletResponse?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
  {
    if (userId <= 0)
    {
      return null;
    }
    var wallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);
    return wallet is null ? null : WalletResponse.FromModel(wallet);
  }

  public async Task<WalletResponse?> CreateAsync(WalletRequest request, CancellationToken cancellationToken)
  {
    var wallet = new Wallet(request.UserId, request.WalletNumber, request.Balance);

    var created = await _walletRepository.TryAddAsync(wallet, cancellationToken);

    return created ? WalletResponse.FromModel(wallet) : null;
  }

  // Top Up
  public async Task<WalletResponse?> UpdateBalanceAsync(int userId, WalletRequest request, CancellationToken cancellationToken)
  {
    // Begin Transaction
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var existingWallet = await _walletRepository.GetByUserIdAsync(userId, cancellationToken);

    if (existingWallet == null) return null;

    // var newBalance = existingWallet.Balance + request.Balance;
    existingWallet.IncreaseBalance(request.Balance);

    var updated = await _walletRepository.TryUpdateBalanceAsync(existingWallet, cancellationToken);

    if (updated)
    {
      var newTrx = new Models.Transaction(null, userId, "income", request.Balance, "Top up via Virtual Account Bank BCA");

      await _transactionRepository.TryAddAsync(newTrx, cancellationToken);

      // Commit
      scope.Complete();
      return WalletResponse.FromModel(existingWallet);
    }

    return null;
  }
}