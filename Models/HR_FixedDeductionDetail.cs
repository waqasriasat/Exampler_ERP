using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_FixedDeductionDetail
  {
    [Key]
    public int FixedDeductionDetailID { get; set; }
    public int? FixedDeductionID { get; set; }
    [ForeignKey("FixedDeductionID")]
    public virtual HR_FixedDeduction? FixedDeduction { get; set; }
    public int? FixedDeductionTypeID { get; set; }
    [ForeignKey("FixedDeductionTypeID")]
    public virtual Settings_FixedDeductionType? FixedDeductionType { get; set; }
    public double? FixedDeductionAmount { get; set; }
  }
}
