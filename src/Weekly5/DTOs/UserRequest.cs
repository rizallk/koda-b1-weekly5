using System.ComponentModel.DataAnnotations;

namespace Weekly5.DTOs;

public sealed record UserRequest(
  [Required]
  [StringLength(100)]
  string Name,

  [Required]
  [StringLength(80)]
  [EmailAddress]
  string Email,

  [Phone]
  [StringLength(20)]
  string? Phone
);