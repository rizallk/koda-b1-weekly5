using System.ComponentModel.DataAnnotations;

namespace Weekly5.ViewModels;

public sealed class UserViewModel
{
    [Required(ErrorMessage = "Nama wajib diisi.")]
    [StringLength(100, ErrorMessage = "Maksimal 100 karakter.")]
    [Display(Name = "Nama")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email wajib diisi.")]
    [EmailAddress(ErrorMessage = "Email harus valid.")]
    [StringLength(100, ErrorMessage = "Maksimal 100 karakter.")]
    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Maksimal 100 karakter.")]
    [Phone(ErrorMessage = "Nomor HP harus valid.")]
    [Display(Name = "Phone")]
    public string? Phone { get; set; }
}