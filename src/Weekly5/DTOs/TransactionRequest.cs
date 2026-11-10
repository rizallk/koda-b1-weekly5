using System.ComponentModel.DataAnnotations;

namespace Weekly5.DTOs;

public sealed record TransactionRequest(
  [Required]
  int SenderWalletId,

  [Required]
  int ReceiverWalletId,

  [Required]
  [StringLength(30)]
  string Type,

  [Required]
  [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
  string Amount,

  [StringLength(100)]
  string Description,

  [DataType(DataType.Date)]
  DateTime CreatedAt
);