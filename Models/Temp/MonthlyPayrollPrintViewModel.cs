namespace Exampler_ERP.Models.Temp
{

  public class MonthlyPayrollPrintViewModel
  {
    public HR_Employee Employee { get; set; }
    public List<HR_MonthlyPayroll_SalaryDetail> SalaryDetails { get; set; }
    public List<HR_AddionalAllowanceDetail> AdditionalAllowances { get; set; }
    public List<HR_OverTime> OvertimeData { get; set; }
    public List<HR_Deduction> Deductions { get; set; }
    public List<HR_MonthlyPayroll_FixedDeductionDetail> FixedDeductions { get; set; }
  }

 
}
