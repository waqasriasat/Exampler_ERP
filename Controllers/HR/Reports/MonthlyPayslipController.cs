using Exampler_ERP.Controllers.HR.Reports;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class MonthlyPayslip : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonthlyPayslip> _logger;
    private readonly Utils _utils;

    public MonthlyPayslip(AppDBContext appDBContext, IConfiguration configuration, ILogger<MonthlyPayslip> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
  

    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      var existingPayroll = await _appDBContext.HR_MonthlyPayrolls
          .Where(e => e.BranchTypeID == 1 && e.MonthTypeID == 12 && e.Year == 2024)
          .FirstOrDefaultAsync();

      if (existingPayroll != null)
      {
        return await PrintPayroll(existingPayroll.PayrollID);
      }

      return NotFound("No payroll entry found for the specified criteria.");
    }

    public async Task<IActionResult> PrintPayroll(int payrollID)
    {
      var employeeData = await _appDBContext.HR_MonthlyPayroll_Salarys
          .Include(s => s.Employee)
          .Include(s => s.Employee.BranchType)
          .Include(s => s.Employee.DepartmentType)
          .Include(s => s.Employee.DesignationType)
          .FirstOrDefaultAsync(s => s.PayrollSalaryID == payrollID);

      if (employeeData == null)
      {
        return NotFound();
      }

      var salaryDetails = await _appDBContext.HR_MonthlyPayroll_SalaryDetails
          .Where(sd => sd.PayrollSalary.PayRollID == payrollID)
          .Include(sd => sd.PayrollSalary)
          .Include(sd => sd.SalaryType)
          .ToListAsync();

      var additionalAllowanceDetails = await _appDBContext.HR_AddionalAllowanceDetails
          .Where(a => a.AddionalAllowance.PayRollID == payrollID)
          .Include(a => a.AddionalAllowance)
          .ToListAsync();

      var overtimeData = await _appDBContext.HR_OverTimes
          .Where(o => o.PayRollID == payrollID)
          .ToListAsync();

      var deductions = await _appDBContext.HR_Deductions
          .Where(d => d.PayRollID == payrollID)
          .ToListAsync();

      var fixedDeductionDetails = await _appDBContext.HR_MonthlyPayroll_FixedDeductionDetails
          .Where(fd => fd.PayrollFixedDeduction.PayRollID == payrollID)
          .Include(fd => fd.PayrollFixedDeduction)
          .Include(fd => fd.FixedDeductionType)
          .ToListAsync();

      var payrollViewModel = new MonthlyPayrollPrintViewModel
      {
        Employee = employeeData.Employee,
        SalaryDetails = salaryDetails,
        AdditionalAllowances = additionalAllowanceDetails,
        OvertimeData = overtimeData,
        Deductions = deductions,
        FixedDeductions = fixedDeductionDetails
      };

      return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", payrollViewModel);
    }

    //public async Task<IActionResult> PrintPayroll(int Branch, int MonthsTypeID, int YearsTypeID)
    //{
    //  var FatchExisting = await _appDBContext.HR_MonthlyPayrolls
    //      .Where(e => e.BranchTypeID == Branch && e.MonthTypeID == MonthsTypeID && e.Year == YearsTypeID)
    //      .FirstOrDefaultAsync();


    //    var Salary = await _appDBContext.HR_MonthlyPayroll_SalaryDetails
    //        .Where(e => e.PayrollSalary.PayRollID == FatchExisting.PayrollID)
    //        .Include(e => e.SalaryType)
    //        .Include(e => e.PayrollSalary.Employee) 
    //        .ToListAsync();


    //    return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", Salary);

    //}

    //private async Task<MonthlyPayslipViewModel> GetMonthlyPayslipAsync(int Branch, int employeeId, int month, int year)
    //{
    //  var Payslip = new MonthlyPayslipViewModel
    //  {
    //    EmployeeID = employeeId
    //  };
    //  decimal sumSalary = 0;
    //  decimal sumAdditionalAllowance = 0;
    //  decimal sumOverTime = 0;
    //  decimal sumDeduction = 0;
    //  decimal sumFixedDeduction = 0;

    //  var connectionString = _configuration.GetConnectionString("AppDb");
    //  using (var connection = new SqlConnection(connectionString))
    //  {
    //    await connection.OpenAsync();

    //    var commandText = "EXEC GetMonthlyPayslip @BranchID, @EmployeeID, @Month, @Year;";
    //    using (var command = new SqlCommand(commandText, connection))
    //    {
    //      command.Parameters.AddWithValue("@BranchID", Branch);
    //      command.Parameters.AddWithValue("@EmployeeID", employeeId);
    //      command.Parameters.AddWithValue("@Month", month);
    //      command.Parameters.AddWithValue("@Year", year);

    //      using (var reader = await command.ExecuteReaderAsync())
    //      {
    //        while (await reader.ReadAsync())
    //        {
    //          for (int i = 0; i < reader.FieldCount; i++)
    //          {
    //            string columnName = reader.GetName(i);

    //            // Handle nullable or non-nullable values
    //            var value = reader.IsDBNull(i) ? "0" : reader.GetValue(i).ToString();

    //            if (columnName == "EmployeeName")
    //            {
    //              Payslip.EmployeeName = value;
    //            }
    //            if (columnName == "BranchID")
    //            {
    //              Payslip.BranchID = int.Parse(value);
    //            }
    //            if (columnName == "MonthID")
    //            {
    //              Payslip.MonthID = int.Parse(value);
    //            }
    //            if (columnName == "MonthName")
    //            {
    //              Payslip.MonthName = value;
    //            }
    //            if (columnName == "Year")
    //            {
    //              Payslip.Year = int.Parse(value);
    //            }

    //            // Process each column by category
    //            if (columnName.StartsWith("Salary_"))
    //            {
    //              Payslip.SalaryDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
    //              sumSalary += decimal.Parse(value);
    //            }
    //            else if (columnName.StartsWith("AdditionalAllowance_"))
    //            {
    //              Payslip.AdditionalAllowances.Add(columnName.Split('_')[1], decimal.Parse(value));
    //              sumAdditionalAllowance += decimal.Parse(value);
    //            }
    //            else if (columnName.StartsWith("OverTime_"))
    //            {
    //              Payslip.OvertimeDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
    //              sumOverTime += decimal.Parse(value);
    //            }
    //            else if (columnName.StartsWith("Deduction_"))
    //            {
    //              Payslip.Deductions.Add(columnName.Split('_')[1], decimal.Parse(value));
    //              sumDeduction += decimal.Parse(value);
    //            }
    //            else if (columnName.StartsWith("FixedDeduction_"))
    //            {
    //              Payslip.FixedDeductions.Add(columnName.Split('_')[1], decimal.Parse(value));
    //              sumFixedDeduction += decimal.Parse(value);
    //            }
    //          }
    //        }
    //      }
    //    }
    //  }
    //  Payslip.SumSalary = sumSalary;
    //  Payslip.SumAdditionalAllowance = sumAdditionalAllowance;
    //  Payslip.SumOverTime = sumOverTime;
    //  Payslip.SumDeduction = sumDeduction;
    //  Payslip.SumFixedDeduction = sumFixedDeduction;

    //  Payslip.DeservedAmount = (sumSalary + sumAdditionalAllowance + sumOverTime) - (sumDeduction + sumFixedDeduction);

    //  return Payslip;
    //}
    //public async Task<IActionResult> Print(int Branch, int MonthsTypeID, int YearsTypeID)
    //{

    //  var employeeList = await _appDBContext.HR_Employees
    //    .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == Branch)
    //    .ToListAsync();

    //  var Payslips = new List<MonthlyPayslipViewModel>();
    //  var tasks = employeeList.Select(async employee =>
    //  {
    //    var salaryData = await GetMonthlyPayslipAsync(Branch, employee.EmployeeID, MonthsTypeID, YearsTypeID);
    //    return salaryData;
    //  });
    //  var salaryDataResults = await Task.WhenAll(tasks);
    //  Payslips.AddRange(salaryDataResults);
    //  return View("~/Views/HR/Reports/MonthlyPayslip/PrintMonthlyPayslip.cshtml", Payslips);
    //}
  }
}
