using System.Transactions;
using Weekly5.DTOs;
using Weekly5.Models;
using Weekly5.Repositories;

namespace Weekly5.Services;

public sealed class UserService : IUserService
{
  private readonly IUserRepository _userRepository;
  private readonly IWalletRepository _walletRepository;

  public UserService(IUserRepository userRepository, IWalletRepository walletRepository)
  {
    _userRepository = userRepository;
    _walletRepository = walletRepository;
  }

  public async Task<UserResponse?> CreateAsync(UserRequest request, CancellationToken cancellationToken)
  {
    // Begin Transaction
    using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

    var user = new User(request.Name, request.Email, request.Phone);

    int newUserId = await _userRepository.AddAsync(user, cancellationToken);
    if (newUserId <= 0) return null;

    string generatedWalletNumber = "W001";

    string? lastWalletNumber = await _walletRepository.GetLastWalletNumberAsync(cancellationToken);

    if (!string.IsNullOrWhiteSpace(lastWalletNumber) && lastWalletNumber.StartsWith('W'))
    {
      string numberPart = lastWalletNumber.Substring(1);

      if (int.TryParse(numberPart, out int lastNumber))
      {
        int newNumber = lastNumber + 1;
        generatedWalletNumber = $"W{newNumber:D3}";
      }
    }

    var newWallet = new Wallet(newUserId, generatedWalletNumber, 0);

    await _walletRepository.TryAddAsync(newWallet, cancellationToken);

    // Commit Transaction
    scope.Complete();

    var finalUser = new User(newUserId, request.Name, request.Email, request.Phone);

    return UserResponse.FromModel(finalUser);
  }

  public async Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken)
  {
    var users = await _userRepository.GetAllAsync(cancellationToken);

    return users.Select(UserResponse.FromModel).ToArray();
  }

  public async Task<UserWallet?> GetByIdAsync(int id, CancellationToken cancellationToken)
  {
    if (id <= 0)
    {
      return null;
    }
    var user = await _userRepository.GetByIdAsync(id, cancellationToken);

    return user is null ? null : user;
  }

  public async Task<IReadOnlyList<UserTransaction>?> GetUserTransactionByUserIdAsync(int id, CancellationToken cancellationToken)
  {
    if (id <= 0)
    {
      return null;
    }

    var userTransactions = await _userRepository.GetUserTransactionByUserIdAsync(id, cancellationToken);

    return userTransactions is null ? null : userTransactions;
  }

  public async Task<UserResponse?> UpdateAsync(int id, UserRequest request, CancellationToken cancellationToken)
  {
    var user = new User(id, request.Name, request.Email, request.Phone);

    var updated = await _userRepository.TryUpdateAsync(user, cancellationToken);

    return updated ? UserResponse.FromModel(user) : null;
  }
}