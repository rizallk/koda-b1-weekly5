namespace Weekly5.DTOs;

public sealed record TotalTransaction
(
  decimal TopUp,
  decimal Income,
  decimal Expense
);