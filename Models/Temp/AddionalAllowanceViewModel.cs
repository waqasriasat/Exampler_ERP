using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class AddionalAllowanceViewModel
  {
    public int AddionalAllowanceID { get; set; }
    public string? AddionalAllowanceName { get; set; }
    public int EmployeeID { get; set; }
    [ForeignKey("EmployeeID")]
    public virtual HR_Employee? Employees { get; set; }
    public int MonthTypeID { get; set; }

    [ForeignKey("MonthTypeID")]
    public virtual Settings_AddionalAllowanceType? AddionalAllowanceType { get; set; }
    public int Year { get; set; }
  }
  public class AddionalAllowanceIndexViewModel
  {

    public List<Settings_AddionalAllowanceType> AddionalAllowanceTypes { get; set; }
    public List<HR_AddionalAllowance> AddionalAllowances { get; set; }
    public List<Employee> Employees { get; set; }
  }
}
