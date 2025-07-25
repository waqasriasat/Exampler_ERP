using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_AdditionalAllowance
  {
    [Key]
    public int AdditionalAllowanceID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
    public int? PostedID { get; set; }
    public int? PayRollID { get; set; }

    public List<HR_AdditionalAllowanceDetail> AdditionalAllowanceDetails { get; set; } = new List<HR_AdditionalAllowanceDetail>();
  }
}
