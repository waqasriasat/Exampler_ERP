using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class FI_BankAccount
  {
    [Key]
    public int BankAccountID { get; set; }
    public string? BankAccountName { get; set; }
    public string? AccountNo { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
  }
}
