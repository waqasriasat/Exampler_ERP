using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{
  public class MonthlySalarySheetViewModel
  {
    public int EmployeeID { get; set; }
    public int MonthID { get; set; }
    public int BranchID { get; set; }
    public string? MonthName { get; set; }
    public int? Year { get; set; }
    public string? EmployeeName { get; set; }
    public Dictionary<string, decimal?> SalaryDetails { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> AdditionalAllowances { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> OvertimeDetails { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> Deductions { get; set; } = new Dictionary<string, decimal?>();
    public Dictionary<string, decimal?> FixedDeductions { get; set; } = new Dictionary<string, decimal?>();

    // Sums for each category
    public decimal SumSalary { get; set; } // Sum of Salary values
    public decimal SumAdditionalAllowance { get; set; } // Sum of Additional Allowance values
    public decimal SumOverTime { get; set; } // Sum of Overtime values
    public decimal SumDeduction { get; set; } // Sum of Deduction values
    public decimal SumFixedDeduction { get; set; } // Sum of Fixed Deduction values

    // Final calculated amount (Deserved Amount)
    public decimal DeservedAmount { get; set; } // Calculated DeservedAmount based on the formula
  }
}
