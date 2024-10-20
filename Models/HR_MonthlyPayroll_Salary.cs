using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models
{
  public class HR_MonthlyPayroll_Salary
  {
    [Key]
    public int PayrollSalaryID { get; set; }
    [Required]
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employee { get; set; }
    public int? MonthTypeID { get; set; }
    [ForeignKey("MonthTypeID")]
    public virtual Settings_MonthType? MonthType { get; set; }
    public int? Year { get; set; }
    public int? PostedID { get; set; }
    public int? PayRollID { get; set; }
  }
}
