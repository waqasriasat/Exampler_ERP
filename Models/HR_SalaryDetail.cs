using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_SalaryDetail
  {
    [Key]
    public int SalaryDetailID { get; set; }
    public int? SalaryID { get; set; }
    [ForeignKey("SalaryID")]
    public virtual HR_Salary? Salary { get; set; }
    public int? SalaryTypeID { get; set; }
    [ForeignKey("SalaryTypeID")]
    public virtual Settings_SalaryType? SalaryType { get; set; }
    public double? SalaryAmount { get; set; }
  }
}
