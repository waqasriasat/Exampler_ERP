using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_MonthlyPayroll_FixedDeductionDetail
  {
    [Key]
    public int PayrollFixedDeductionDetailID { get; set; }
    public int? PayrollFixedDeductionID { get; set; }
    [ForeignKey("PayrollFixedDeductionID")]
    public virtual HR_MonthlyPayroll_FixedDeduction? PayrollFixedDeduction { get; set; }
    public int? FixedDeductionTypeID { get; set; }
    [ForeignKey("FixedDeductionTypeID")]
    public virtual Settings_FixedDeductionType? FixedDeductionType { get; set; }
    public double? FixedDeductionAmount { get; set; }
  }
}
