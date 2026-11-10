using System.ComponentModel.DataAnnotations;

namespace Weekly5.DTOs;

public sealed record TransferRequest(
  [Required]
  int SenderWalletId,

  [Required]
  int ReceiverWalletId,

  [Required]
  [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
  decimal Amount,

  [StringLength(100)]
  string Description
);