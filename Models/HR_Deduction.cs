using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_Deduction
  {
    [Key]
    public int DeductionID { get; set; }
    public int DeductionTypeID { get; set; }
    [ForeignKey("DeductionTypeID")]
    public virtual Settings_DeductionType? DeductionType { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? Month { get; set; }
    public int? Year { get; set; }
    public int? Days { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int? DeleteYNID { get; set; }
    public int? FinalApprovalID { get; set; }
    public int? ProcessTypeApprovalID { get; set; }
  }
}
