using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class SalaryViewModel
  {
    public int SalaryTypeID { get; set; }
    public string? SalaryTypeName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("SalaryTypeID")]
    public virtual Settings_SalaryType? SalaryType { get; set; }
  }
  public class SalaryIndexViewModel
  {
    public List<Settings_SalaryType> SalaryTypes { get; set; }
    public List<HR_Salary> Salarys { get; set; }
    public List<Employee> Employees { get; set; }
  }
}
