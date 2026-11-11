using System.ComponentModel.DataAnnotations;

namespace Weekly5.ViewModels;

public sealed class TopUpViewModel
{
    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(20)]
    public string? WalletNumber { get; set; }

    [Required(ErrorMessage = "Jumlah wajib diisi")]
    [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [Display(Name = "Jumlah")]
    public decimal? Balance { get; set; }
}