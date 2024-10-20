using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Exampler_ERP.Models
{
  public class HR_MonthlyPayroll_SalaryDetail
  {
    [Key]
    public int PayrollSalaryDetailID { get; set; }
    public int? PayrollSalaryID { get; set; }
    [ForeignKey("PayrollSalaryID")]
    public virtual HR_MonthlyPayroll_Salary? PayrollSalary { get; set; }
    public int? SalaryTypeID { get; set; }
    [ForeignKey("SalaryTypeID")]
    public virtual Settings_SalaryType? SalaryType { get; set; }
    public double? SalaryAmount { get; set; }
  }
}
