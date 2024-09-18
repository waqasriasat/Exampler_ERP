using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_OverTime
  {
    [Key]
    public int OverTimeID { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public float? Amount { get; set; }
    public int? DeleteYNID { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }
  }
}
