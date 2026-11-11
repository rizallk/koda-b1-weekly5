using Weekly5.DTOs;
using Weekly5.Models;

namespace Weekly5.Services;

public interface IWalletService
{
  Task<WalletResponse?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);
  Task<WalletResponse?> CreateAsync(WalletRequest request, CancellationToken cancellationToken);
  Task<WalletResponse?> UpdateBalanceAsync(int userId, WalletRequest wallet, CancellationToken cancellationToken);
  Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken);
  Task<Wallet?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken);
}