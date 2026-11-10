using Weekly5.DTOs;

namespace Weekly5.Services;

public interface IUserService
{
  Task<IReadOnlyList<UserResponse>> GetAllAsync(CancellationToken cancellationToken);
  Task<UserWallet?> GetByIdAsync(int id, CancellationToken cancellationToken);
  Task<IReadOnlyList<UserTransaction>?> GetUserTransactionByUserIdAsync(int id, CancellationToken cancellationToken);
  Task<UserResponse?> CreateAsync(UserRequest request, CancellationToken cancellationToken);
  Task<UserResponse?> UpdateAsync(int id, UserRequest request, CancellationToken cancellationToken);
}