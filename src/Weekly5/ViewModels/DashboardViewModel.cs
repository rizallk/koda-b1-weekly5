using Weekly5.DTOs;

namespace Weekly5.ViewModels;

public class DashboardViewModel
{
  public UserWallet? UserWallet { get; set; }
  public TotalTransaction? Transactions { get; set; }
}