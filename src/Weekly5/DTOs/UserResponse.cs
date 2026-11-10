using Weekly5.Models;

namespace Weekly5.DTOs;

public sealed record UserResponse(
  int Id,
  string Name,
  string Email,
  string? Phone
)
{
  public static UserResponse FromModel(User user) => new(user.Id, user.Name, user.Email, user.Phone);
}