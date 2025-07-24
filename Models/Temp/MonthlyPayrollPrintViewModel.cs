using System.ComponentModel.DataAnnotations.Schema;

namespace Exampler_ERP.Models.Temp
{

  public class MonthlyPayrollPrintViewModel
  {
    public List<EmployeePayrollData> EmployeePayrolls { get; set; }
  }

  public class EmployeePayrollData
  {
    public HR_Employee Employee { get; set; }
    public HR_BankAccount BankDetails { get; set; }
    public HR_MonthlyPayroll? PayslipDetails { get; set; }
    public List<HR_MonthlyPayroll_SalaryDetail> SalaryDetails { get; set; }
    public List<HR_AdditionalAllowanceDetail> AdditionalAllowances { get; set; }
    public List<HR_OverTimeTemp> OvertimeData { get; set; }
    public List<HR_DeductionTemp> Deductions { get; set; }
    public List<HR_MonthlyPayroll_FixedDeductionDetail> FixedDeductions { get; set; }
  }
  public class HR_OverTimeTemp
  {
    public int OverTimeTypeID { get; set; }
    public string? OverTimeTypeName { get; set; }
    public float? Amount { get; set; }
  }
  public class HR_DeductionTemp
  {
    public int DeductionTypeID { get; set; }
    public string? DeductionTypeName { get; set; }
    public float? Amount { get; set; }
  }

}
