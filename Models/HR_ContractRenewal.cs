using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_ContractRenewal
  {
    [Key]
    public int ContractRenewalID { get; set; }
    public int ContractID { get; set; }
    [ForeignKey("ContractID")]
    public virtual HR_Contract? Contract { get; set; }
    public DateTime PStartDate { get; set; }
    public DateTime? PEndDate { get; set; }
    public DateTime NStartDate { get; set; }
    public DateTime? NEndDate { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
  }
}
