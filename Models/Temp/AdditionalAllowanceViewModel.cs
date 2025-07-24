using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class AdditionalAllowanceViewModel
  {
    public int AdditionalAllowanceID { get; set; }
    public string? AdditionalAllowanceName { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employees { get; set; }
    public int MonthTypeID { get; set; }

    [ForeignKey("MonthTypeID")]
    public virtual Settings_AdditionalAllowanceType? AdditionalAllowanceType { get; set; }
    public int Year { get; set; }
  }
  public class AdditionalAllowanceIndexViewModel
  {

    public List<Settings_AdditionalAllowanceType> AdditionalAllowanceTypes { get; set; }
    public List<HR_AdditionalAllowance> AdditionalAllowances { get; set; }
    public List<Employee> Employees { get; set; }
  }
}
