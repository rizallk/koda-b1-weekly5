using Weekly5.DTOs;

namespace Weekly5.Services;

public interface ITransferService
{
  Task<TransactionResponse?> TransferAsync(TransferRequest transfer, CancellationToken cancellationToken);
}