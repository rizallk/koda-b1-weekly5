using System.Transactions;
using Weekly5.DTOs;
using Weekly5.Repositories;

namespace Weekly5.Services;

public sealed class TransferService : ITransferService
{
  private readonly IWalletRepository _walletRepository;
  private readonly ITransactionRepository _transactionRepository;

  public TransferService(IWalletRepository walletRepository, ITransactionRepository transactionRepository)
  {
    _walletRepository = walletRepository;
    _transactionRepository = transactionRepository;
  }

  public async Task<TransactionResponse?> TransferAsync(TransferRequest request, CancellationToken cancellationToken)
  {
    if (request.SenderWalletId == request.ReceiverWalletId)
    {
      throw new InvalidOperationException("Tidak bisa melakukan transfer ke akun sendiri.");
    }

    // Begin Transaction
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var existingSenderWallet = await _walletRepository.GetByUserIdAsync(request.SenderWalletId, cancellationToken);
    var existingReceiverWallet = await _walletRepository.GetByUserIdAsync(request.ReceiverWalletId, cancellationToken);

    if (existingSenderWallet == null || existingReceiverWallet == null) return null;

    existingSenderWallet.DecreaseBalance(request.Amount);
    existingReceiverWallet.IncreaseBalance(request.Amount);

    var isSenderUpdated = await _walletRepository.TryUpdateBalanceAsync(existingSenderWallet, cancellationToken);
    var isReceiverUpdated = await _walletRepository.TryUpdateBalanceAsync(existingReceiverWallet, cancellationToken);

    if (isSenderUpdated && isReceiverUpdated)
    {
      var expenseTrx = new Models.Transaction(
        request.SenderWalletId,
        request.ReceiverWalletId,
        "expense",
        request.Amount,
        request.Description ?? ""
      );

      var incomeTrx = new Models.Transaction(
        request.SenderWalletId,
        request.ReceiverWalletId,
        "income",
        request.Amount,
        request.Description ?? ""
      );

      await _transactionRepository.TryAddAsync(expenseTrx, cancellationToken);
      await _transactionRepository.TryAddAsync(incomeTrx, cancellationToken);

      // Commit
      scope.Complete();

      return new TransactionResponse(
        Id: 0,
        SenderWalletId: existingSenderWallet.Id,
        ReceiverWalletId: existingReceiverWallet.Id,
        Type: "transfer",
        Amount: request.Amount,
        Description: "",
        CreatedAt: DateTime.Today
      );
    }

    return null;
  }
}