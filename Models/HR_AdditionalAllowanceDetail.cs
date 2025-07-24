using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_AdditionalAllowanceDetail
  {
    [Key]
    public int AdditionalAllowanceDetailID { get; set; }
    public int? AdditionalAllowanceID { get; set; }
    [ForeignKey("AdditionalAllowanceID")]
    public virtual HR_AdditionalAllowance? AdditionalAllowance { get; set; }
    public int? AdditionalAllowanceTypeID { get; set; }
    [ForeignKey("AdditionalAllowanceTypeID")]
    public virtual Settings_AdditionalAllowanceType? AdditionalAllowanceType { get; set; }
    public double? AdditionalAllowanceAmount { get; set; }
  }
}
