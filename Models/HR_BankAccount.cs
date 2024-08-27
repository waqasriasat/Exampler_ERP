using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_BankAccount
  {
    [Key]
    public int BankAccountID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public string? AccountHolderName { get; set; }
    public string? AccountNumber { get; set; }
    public string? BankName { get; set; }
    public string? BranchName { get; set; }
    public string? AccountType { get; set; }
    public string? BankContact { get; set; }
    public string? BankAddress { get; set; }
    
  }
}
