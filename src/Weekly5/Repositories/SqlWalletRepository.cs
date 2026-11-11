using System.Data;
using Microsoft.Data.SqlClient;
using Weekly5.Models;

namespace Weekly5.Repositories;

public sealed class SqlWalletRepository : IWalletRepository
{
  private readonly SqlConnection _connection;

  public SqlWalletRepository(SqlConnection connection)
  {
    _connection = connection;
  }

  public async Task<Wallet?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Id, UserId, WalletNumber, Balance FROM dbo.Wallets WHERE UserId = @userId;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddUserId(command, userId);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return ReadWallet(reader);
  }

  public async Task<Wallet?> GetByIdAsync(int id, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Id, UserId, WalletNumber, Balance FROM dbo.Wallets WHERE Id = @id;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);
    command.Parameters.Add("@id", SqlDbType.Int).Value = id;

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return ReadWallet(reader);
  }

  public async Task<Wallet?> GetByWalletNumberAsync(string walletNumber, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT Id, UserId, WalletNumber, Balance FROM dbo.Wallets WHERE WalletNumber = @walletNumber;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);
    command.Parameters.Add("@walletNumber", SqlDbType.NVarChar, 20).Value = walletNumber;

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    if (!await reader.ReadAsync(cancellationToken))
    {
      return null;
    }

    return ReadWallet(reader);
  }

  public async Task<bool> TryAddAsync(Wallet wallet, CancellationToken cancellationToken)
  {
    const string sql = """
      INSERT INTO dbo.Wallets(UserId, WalletNumber, Balance) VALUES (@userId, @walletNumber, @balance);
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddWalletParameter(command, wallet);

    try
    {
      var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

      return affectedRows == 1;
    }
    catch (SqlException e) when (e.Number is 2601 or 2627)
    {
      return false;
    }
  }

  // Top Up
  public async Task<bool> TryUpdateBalanceAsync(Wallet wallet, CancellationToken cancellationToken)
  {
    const string sql = """
      UPDATE dbo.Wallets SET Balance = @balance WHERE UserId = @userId;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    UpdateBalanceParameter(command, wallet);

    var affectedRows = await command.ExecuteNonQueryAsync(cancellationToken);

    return affectedRows == 1;
  }

  public async Task<string?> GetLastWalletNumberAsync(CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT TOP 1 WalletNumber FROM dbo.Wallets ORDER BY Id DESC;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    var result = await command.ExecuteScalarAsync(cancellationToken);

    return result as string;
  }

  // ============= METHOD

  private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
  {
    if (_connection.State != ConnectionState.Open)
    {
      await _connection.OpenAsync(cancellationToken);
    }
  }

  private static void UpdateBalanceParameter(SqlCommand command, Wallet wallet)
  {
    AddUserId(command, wallet.UserId);

    command.Parameters.Add("@balance", SqlDbType.Decimal).Value = wallet.Balance;
  }

  private static void AddWalletParameter(SqlCommand command, Wallet wallet)
  {
    AddUserId(command, wallet.UserId);

    command.Parameters.Add("@WalletNumber", SqlDbType.NVarChar, 20).Value = wallet.WalletNumber;

    command.Parameters.Add("@Balance", SqlDbType.Decimal).Value = wallet.Balance;
  }

  private static void AddUserId(SqlCommand command, int userId)
  {
    command.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
  }

  private static Wallet ReadWallet(SqlDataReader reader)
  {
    return new Wallet(
      reader.GetInt32(0),
      reader.GetInt32(1),
      reader.GetString(2),
      reader.GetDecimal(3)
    );
  }
}