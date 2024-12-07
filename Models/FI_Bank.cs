using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class FI_Bank
  {
    [Key]
    public int BankID { get; set; }
    public string? BankName { get; set; }
    public string? AccountNo { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public int? ActiveYNID { get; set; }
    public int? DeleteYNID { get; set; }
  }
}
