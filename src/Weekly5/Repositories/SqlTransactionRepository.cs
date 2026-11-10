using System.Data;
using Microsoft.Data.SqlClient;
using Weekly5.DTOs;
using Weekly5.Models;

namespace Weekly5.Repositories;

public sealed class SqlTransactionRepository : ITransactionRepository
{
  private readonly SqlConnection _connection;

  public SqlTransactionRepository(SqlConnection connection)
  {
    _connection = connection;
  }

  public async Task<bool> TryAddAsync(Transaction trx, CancellationToken cancellationToken)
  {
    const string sql = """
      INSERT INTO dbo.Transactions(SenderWalletId, ReceiverWalletId, Type, Amount, Description) VALUES (@senderWalletId, @receiverWalletId, @type, @amount, @description);
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddTransactionParameter(command, trx);

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

  public async Task<IReadOnlyList<TransactionSummary>> GetTransactionSummaryByWalletId(int walletId, CancellationToken cancellationToken)
  {
    const string sql = """
      SELECT 
        Type,
        SUM(Amount) AS TotalAmount
      FROM Transactions
      WHERE 
        SenderWalletId = @WalletId OR ReceiverWalletId = @WalletId
      GROUP BY Type;
    """;

    await EnsureConnectionOpenAsync(cancellationToken);

    await using var command = new SqlCommand(sql, _connection);

    AddWalletId(command, walletId);

    await using var reader = await command.ExecuteReaderAsync(cancellationToken);

    var summaries = new List<TransactionSummary>();

    while (await reader.ReadAsync(cancellationToken))
    {
      var type = reader.GetString(0);
      var totalAmount = reader.IsDBNull(1) ? 0m : reader.GetDecimal(1);

      summaries.Add(new TransactionSummary(type, totalAmount));
    }

    return summaries;
  }

  // ============= METHOD

  private async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
  {
    if (_connection.State != ConnectionState.Open)
    {
      await _connection.OpenAsync(cancellationToken);
    }
  }

  private static void AddWalletId(SqlCommand command, int walletId)
  {
    command.Parameters.Add("@walletId", SqlDbType.Int).Value = walletId;
  }

  private static void AddTransactionParameter(SqlCommand command, Transaction trx)
  {
    command.Parameters.Add("@senderWalletId", SqlDbType.Int).Value = (object?)trx.SenderWalletId ?? DBNull.Value;

    command.Parameters.Add("@receiverWalletId", SqlDbType.Int).Value = trx.ReceiverWalletId;

    command.Parameters.Add("@type", SqlDbType.NVarChar, 30).Value = trx.Type;

    command.Parameters.Add("@amount", SqlDbType.Decimal).Value = trx.Amount;

    command.Parameters.Add("@description", SqlDbType.NVarChar, 100).Value = trx.Description;
  }
}