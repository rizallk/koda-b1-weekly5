using System.ComponentModel.DataAnnotations;

namespace Weekly5.DTOs;

public record WalletRequest(
  [Required]
  int UserId,

  [Required]
  [StringLength(20)]
  string WalletNumber,

  [Required]
  [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
  decimal Balance
);
