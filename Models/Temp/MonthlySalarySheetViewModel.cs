namespace Exampler_ERP.Models.Temp
{
  public class MonthlySalarySheetViewModel
  {
    public int EmployeeID { get; set; }
    public Dictionary<string, decimal?> SalaryDetails { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> AdditionalAllowances { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> OvertimeDetails { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> Deductions { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> FixedDeductions { get; set; } = new Dictionary<string, decimal?>();
  }
}
