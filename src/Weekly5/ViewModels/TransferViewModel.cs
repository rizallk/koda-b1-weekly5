using System.ComponentModel.DataAnnotations;

namespace Weekly5.ViewModels;

public class TransferViewModel
{
  public int SenderUserId { get; set; }

  [Required(ErrorMessage = "Nomor Wallet Tujuan wajib diisi.")]
  [Display(Name = "Nomor Wallet Tujuan")]
  public string ReceiverWalletNumber { get; set; } = string.Empty;

  [Required(ErrorMessage = "Nominal wajib diisi.")]
  [Range(10000, 100000000, ErrorMessage = "Minimal transfer Rp 10.000")]
  public decimal Amount { get; set; }

  [Display(Name = "Catatan (Opsional)")]
  public string? Description { get; set; }
}