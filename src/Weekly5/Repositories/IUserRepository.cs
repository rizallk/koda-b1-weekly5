using Weekly5.DTOs;
using Weekly5.Models;

namespace Weekly5.Repositories;

public interface IUserRepository
{
  Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken);
  Task<UserWallet?> GetByIdAsync(int id, CancellationToken cancellationToken);
  Task<IReadOnlyList<UserTransaction>?> GetUserTransactionByUserIdAsync(int id, CancellationToken cancellationToken);
  Task<int> AddAsync(User user, CancellationToken cancellationToken);
  Task<bool> TryUpdateAsync(User user, CancellationToken cancellationToken);
}