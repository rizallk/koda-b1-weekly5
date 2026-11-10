using System.Data;
using Microsoft.Data.SqlClient;
using Weekly5.DTOs;
using Weekly5.Models;

namespace Weekly5.Repositories;

public sealed class SqlUserRepository : IUserRepository
{
  private readonly SqlConnection _connection;

  public SqlUserRepository(SqlConnection connection)
  {
    _connection = connection;
  }

  public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Id, Name, Email, Phone FROM dbo.Users ORDER BY Id DESC;
    """;

    var users = new List<User>();

    await EnsureConnectionOpenAsync(cancellationToken);

    using var command = new SqlCommand(sql, _connection);
    using var reader = await command.ExecuteReaderAsync(cancellationToken);

    while (await reader.ReadAsync(cancellationToken))
    {
      users.Add(ReadUser(reader));
    }

    return users;
  }

  public async Task<UserWallet?> GetByIdAsync(int id, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT u.Id AS UserId, u.Name, u.Email, u.Phone, w.Id AS WalletId, w.WalletNumber, w.Balance
      FROM dbo.Users u
      JOIN dbo.Wallets w
        ON u.Id = w.UserId
      WHERE u.Id = @id;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddId(command, id);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return ReadUserWallet(reader);
  }

  public async Task<IReadOnlyList<UserTransaction>?> GetUserTransactionByUserIdAsync(int id, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT DISTINCT 
        t.Id AS TransactionId,
        t.CreatedAt,
        t.Type,
        t.Amount,
        t.Description
      FROM dbo.Transactions t
      INNER JOIN dbo.Wallets w 
        ON w.Id IN (t.SenderWalletId, t.ReceiverWalletId)
      WHERE w.UserId = @id
      ORDER BY t.CreatedAt DESC;
    """;

    var transactions = new List<UserTransaction>();

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddId(command, id);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    while (await reader.ReadAsync(cancellationToken))
    {
      transactions.Add(ReadUserTransaction(reader));
    }

    return transactions;
  }

  public async Task<int> AddAsync(User user, CancellationToken cancellationToken)
  {
    const string sql = """
      INSERT INTO dbo.Users(Name, Email, Phone)
      OUTPUT INSERTED.Id
      VALUES (@name, @email, @phone);
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddUserParameter(command, user);

    var result = await command.ExecuteScalarAsync(cancellationToken);

    return result != null ? Convert.ToInt32(result) : 0;
  }

  public async Task<bool> TryDeleteAsync(int id, CancellationToken cancellationToken)
  {
    const string sql = """
      DELETE FROM dbo.Users WHERE Id = @id;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddId(command, id);

    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  public async Task<bool> TryUpdateAsync(User user, CancellationToken cancellationToken)
  {
    const string sql = """
      UPDATE dbo.Users SET
        Name = @name,
        Email = @email,
        Phone = @phone
      WHERE Id = @id;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddId(command, user.Id);
    AddUserParameter(command, user);

    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  // ============= METHOD

  private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
  {
    if (_connection.State != ConnectionState.Open)
    {
      await _connection.OpenAsync(cancellationToken);
    }
  }

  private static void AddUserParameter(SqlCommand command, User user)
  {
    command.Parameters.Add("@name", SqlDbType.NVarChar, 100).Value = user.Name;

    command.Parameters.Add("@email", SqlDbType.NVarChar, 80).Value = user.Email;

    command.Parameters.Add("@phone", SqlDbType.NVarChar, 20).Value = user.Phone;
  }

  private static void AddId(SqlCommand command, int id)
  {
    command.Parameters.Add("@id", SqlDbType.Int).Value = id;
  }

  private static UserWallet ReadUserWallet(SqlDataReader reader)
  {
    return new UserWallet(
      reader.GetInt32(0),
      reader.GetString(1),
      reader.GetString(2),
      reader.GetString(3),
      reader.GetInt32(4),
      reader.GetString(5),
      reader.GetDecimal(6)
    );
  }

  private static User ReadUser(SqlDataReader reader)
  {
    return new User(
      reader.GetInt32(0),
      reader.GetString(1),
      reader.GetString(2),
      reader.GetString(3)
    );
  }

  private static UserTransaction ReadUserTransaction(SqlDataReader reader)
  {
    return new UserTransaction(
      reader.GetInt32(0),
      reader.GetDateTime(1),
      reader.GetString(2),
      reader.GetDecimal(3),
      reader.IsDBNull(4) ? null : reader.GetString(4)
    );
  }
}