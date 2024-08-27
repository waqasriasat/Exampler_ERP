using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_DeductionSetup
  {
    [Key]
    public int DeductionSetupID { get; set; }
    public int DeductionTypeID { get; set; }
    [ForeignKey("DeductionTypeID")]
    public virtual Settings_DeductionType? DeductionType { get; set; }
    public int? ClassID { get; set; }
    public int? SalaryTypeID { get; set; }

    [ForeignKey("SalaryTypeID")]
    public virtual Settings_SalaryType? SalaryType { get; set; }
    public int? DeductionValueID { get; set; }
  }
}
