using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_FixedDeduction
  {
    [Key]
    public int FixedDeductionID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
  }
}
