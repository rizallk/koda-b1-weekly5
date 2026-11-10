namespace Weekly5.DTOs;

public record UserTransaction(
  int TransactionId,
  DateTime CreatedAt,
  string Type,
  decimal Amount,
  string? Description
);