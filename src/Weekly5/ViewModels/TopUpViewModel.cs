using System.ComponentModel.DataAnnotations;

namespace Weekly5.ViewModels;

public sealed class TopUpViewModel
{
    [Required(ErrorMessage = "Jumlah wajib diisi")]
    [Range(typeof(decimal), "0", "99999999999.99", ParseLimitsInInvariantCulture = true, ConvertValueInInvariantCulture = true)]
    [Display(Name = "Jumlah")]
    public decimal? Balance { get; set; }
}