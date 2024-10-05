using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class FixedDeductionViewModel
  {
    public int FixedDeductionTypeID { get; set; }
    public string? FixedDeductionTypeName { get; set; }
    public int TotalRank { get; set; }

    [ForeignKey("FixedDeductionTypeID")]
    public virtual Settings_FixedDeductionType? FixedDeductionType { get; set; }
  }
  public class FixedDeductionIndexViewModel
  {
    public List<Settings_FixedDeductionType> FixedDeductionTypes { get; set; }
    public List<HR_FixedDeduction> FixedDeductions { get; set; }
    public List<Employee> Employees { get; set; }
  }
}
