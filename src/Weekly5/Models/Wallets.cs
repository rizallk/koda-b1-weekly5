namespace Weekly5.Models;

public sealed class Wallet
{
  public int Id { get; }
  public int UserId { get; }
  public string WalletNumber { get; }
  public decimal Balance { get; private set; }

  public Wallet(int id, int userId, string walletNumber, decimal balance)
  {
    if (id <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(id), "Id tidak boleh 0 atau negatif");
    }
    if (userId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(userId), "UserId tidak boleh 0 atau negatif");
    }
    if (string.IsNullOrWhiteSpace(walletNumber))
    {
      throw new ArgumentException("Wallet Number wajib diisi", nameof(walletNumber));
    }
    if (balance < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(balance), "Balance tidak boleh negatif");
    }

    Id = id;
    UserId = userId;
    WalletNumber = walletNumber;
    Balance = balance;
  }

  public Wallet(int userId, string walletNumber, decimal balance)
  {
    if (userId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(userId), "UserId tidak boleh 0 atau negatif");
    }
    if (string.IsNullOrWhiteSpace(walletNumber))
    {
      throw new ArgumentException("Wallet Number wajib diisi", nameof(walletNumber));
    }
    if (balance < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(balance), "Balance tidak boleh negatif");
    }

    UserId = userId;
    WalletNumber = walletNumber;
    Balance = balance;
  }

  public void IncreaseBalance(decimal amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(amount), "Jumlah top up harus lebih dari 0");
    }
    Balance += amount;
  }

  public void DecreaseBalance(decimal amount)
  {
    if (amount <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(amount), "Jumlah transfer harus lebih dari 0");
    }
    if (Balance < amount)
    {
      throw new InvalidOperationException("Saldo tidak mencukupi untuk melakukan transfer");
    }
    Balance -= amount;
  }
}