namespace Exampler_ERP.Models.Temp
{
  public class MonthlySalarySheetViewModel
  {
    public int EmployeeID { get; set; }
    public Dictionary<int, decimal?> SalaryDetails { get; set; } = new Dictionary<int, decimal?>();
    public Dictionary<int, decimal?> AdditionalAllowances { get; set; } = new Dictionary<int, decimal?>();
    public Dictionary<int, decimal?> OvertimeDetails { get; set; } = new Dictionary<int, decimal?>();
    public Dictionary<int, decimal?> Deductions { get; set; } = new Dictionary<int, decimal?>();
    public Dictionary<int, decimal?> FixedDeductions { get; set; } = new Dictionary<int, decimal?>();
  }
}
