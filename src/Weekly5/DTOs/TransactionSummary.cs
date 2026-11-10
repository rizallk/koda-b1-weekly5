namespace Weekly5.DTOs;

public sealed record TransactionSummary
(
  string Type,
  decimal TotalAmount
);