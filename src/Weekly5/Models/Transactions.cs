namespace Weekly5.Models;

public sealed class Transaction
{
  public int Id { get; }
  public int? SenderWalletId { get; }
  public int ReceiverWalletId { get; }
  public string Type { get; }
  public decimal Amount { get; }
  public string Description { get; }
  public DateTime CreatedAt { get; }

  public Transaction(int id, int? senderWalletId, int receiverWalletId, string type, decimal amount, string description, DateTime createdAt)
  {
    if (id <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(id), "Id tidak boleh 0 atau negatif");
    }
    if (senderWalletId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(senderWalletId), "Sender Wallet Id tidak boleh 0 atau negatif");
    }
    if (receiverWalletId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(receiverWalletId), "Receiver Wallet Id tidak boleh 0 atau negatif");
    }
    if (string.IsNullOrWhiteSpace(type))
    {
      throw new ArgumentException("Type wajib diisi", nameof(type));
    }
    if (amount < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(amount), "Balance tidak boleh negatif");
    }
    Id = id;
    SenderWalletId = senderWalletId;
    ReceiverWalletId = receiverWalletId;
    Type = type;
    Amount = amount;
    Description = description;
    CreatedAt = createdAt;
  }

  public Transaction(int? senderWalletId, int receiverWalletId, string type, decimal amount, string description)
  {
    if (receiverWalletId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(receiverWalletId), "Receiver Wallet Id tidak boleh 0 atau negatif");
    }
    if (senderWalletId <= 0)
    {
      throw new ArgumentOutOfRangeException(nameof(senderWalletId), "Sender Wallet Id tidak boleh 0 atau negatif");
    }
    if (string.IsNullOrWhiteSpace(type))
    {
      throw new ArgumentException("Type wajib diisi", nameof(type));
    }
    if (amount < 0)
    {
      throw new ArgumentOutOfRangeException(nameof(amount), "Balance tidak boleh negatif");
    }

    SenderWalletId = senderWalletId;
    ReceiverWalletId = receiverWalletId;
    Type = type;
    Amount = amount;
    Description = description;
  }
}