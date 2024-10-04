using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_AddionalAllowance
  {
    [Key]
    public int AddionalAllowanceID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ApprovalProcessID { get; set; }

    public List<HR_AddionalAllowanceDetail> AddionalAllowanceDetails { get; set; } = new List<HR_AddionalAllowanceDetail>();
  }
}
