using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_MonthlyPayrollPosted
  {
    [Key]
    public int PayrollPostedID { get; set; }
    [Required]
    public int? BranchTypeID { get; set; }
    [ForeignKey("BranchTypeID")]
    public virtual Settings_BranchType? BranchType { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? FinalApprovalID { get; set; } 
    public int? ProcessTypeApprovalID { get; set; } 
    public int? DeleteYNID { get; set; } 
  }
}
