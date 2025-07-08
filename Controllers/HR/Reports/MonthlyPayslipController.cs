using Exampler_ERP.Controllers.HR.Reports;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Linq;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class MonthlyPayslip : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonthlyPayslip> _logger;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;


    public MonthlyPayslip(AppDBContext appDBContext, IConfiguration configuration, ILogger<MonthlyPayslip> logger, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
_hubContext = hubContext;
 
    }
  

    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      var existingPayroll = await _appDBContext.HR_MonthlyPayrolls
          .Where(e => e.BranchTypeID == Branch && e.MonthTypeID == MonthsTypeID && e.Year == YearsTypeID)
          .FirstOrDefaultAsync();

      if (existingPayroll != null)
      {
        return await PrintPayroll(existingPayroll.PayrollID);
      }
      if (existingPayroll == null)
      {
        ViewBag.MonthsTypeID = MonthsTypeID;
        ViewBag.YearsTypeID = YearsTypeID;
        ViewBag.Branch = Branch;

        ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
        ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
        return await PrintPayroll(0);
      }

      return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", existingPayroll);
    }
    public async Task<IActionResult> PrintPayroll(int payrollID)
    {
      if (payrollID == 0)
      {
        // Initialize an empty list for when payrollID is 0, representing no data scenario
        var emptyViewModel = new MonthlyPayrollPrintViewModel
        {
          EmployeePayrolls = new List<EmployeePayrollData>()
        };
        ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
        ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
        return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", emptyViewModel);
      }

      var employeeData = await _appDBContext.HR_MonthlyPayroll_Salarys
          .Where(s => s.PayRollID == payrollID)
          .Include(s => s.Employee)
          .Include(s => s.Employee.BranchType)
          .Include(s => s.Employee.DepartmentType)
          .Include(s => s.Employee.DesignationType)
          .OrderBy(s => s.EmployeeID)
          .ToListAsync();

      if (employeeData == null || !employeeData.Any())
      {
        // Handle the case when no employee data is found for a valid payrollID
        var emptyViewModel = new MonthlyPayrollPrintViewModel
        {
          EmployeePayrolls = new List<EmployeePayrollData>()
        };
        ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
        ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
        return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", emptyViewModel);
      }

      var employeePayrolls = new List<EmployeePayrollData>();

      foreach (var employeeSalary in employeeData)
      {
        // Retrieve salary details, allowances, overtime, deductions, and other payroll data
        var salaryDetails = await _appDBContext.HR_MonthlyPayroll_SalaryDetails
            .Where(sd => sd.PayrollSalary.EmployeeID == employeeSalary.EmployeeID && sd.PayrollSalary.PayRollID == payrollID)
            .Include(sd => sd.PayrollSalary)
            .Include(sd => sd.SalaryType)
            .ToListAsync();

        var additionalAllowances = await _appDBContext.HR_AddionalAllowanceDetails
            .Where(a => a.AddionalAllowance.EmployeeID == employeeSalary.EmployeeID && a.AddionalAllowance.PayRollID == payrollID)
            .Include(a => a.AddionalAllowance)
            .Include(a => a.AddionalAllowanceType)
            .ToListAsync();

        var overtimeData = await _appDBContext.HR_OverTimes
            .Where(o => o.EmployeeID == employeeSalary.EmployeeID && o.PayRollID == payrollID)
            .Include(o => o.OverTimeType)
            .GroupBy(o => new
            {
              o.OverTimeTypeID,
              o.OverTimeType.OverTimeTypeName
            })
            .Select(g => new HR_OverTimeTemp // Assuming HR_OverTime has a suitable constructor or properties
            {
              OverTimeTypeID = g.Key.OverTimeTypeID,
              OverTimeTypeName = g.Key.OverTimeTypeName,
              Amount = g.Sum(o => o.Amount) // Make sure 'Amount' is the right property name
            })
            .ToListAsync();


        var deductions = await _appDBContext.HR_Deductions
            .Where(d => d.EmployeeID == employeeSalary.EmployeeID && d.PayRollID == payrollID)
            .Include(d => d.DeductionType)
            .GroupBy(o => new
            {
              o.DeductionTypeID,
              o.DeductionType.DeductionTypeName
            })
             .Select(g => new HR_DeductionTemp // Assuming HR_OverTime has a suitable constructor or properties
             {
               DeductionTypeID = g.Key.DeductionTypeID,
               DeductionTypeName = g.Key.DeductionTypeName,
               Amount = g.Sum(o => o.Amount) // Make sure 'Amount' is the right property name
             })
            .ToListAsync();

        var fixedDeductionDetails = await _appDBContext.HR_MonthlyPayroll_FixedDeductionDetails
            .Where(fd => fd.PayrollFixedDeduction.EmployeeID == employeeSalary.EmployeeID && fd.PayrollFixedDeduction.PayRollID == payrollID)
            .Include(fd => fd.PayrollFixedDeduction)
            .Include(fd => fd.FixedDeductionType)
            .ToListAsync();

        var bankAccountData = await _appDBContext.HR_BankAccounts
            .Where(s => s.EmployeeID == employeeSalary.EmployeeID)
            .FirstOrDefaultAsync();

        var employeePayslip = await _appDBContext.HR_MonthlyPayrolls
            .Where(s => s.PayrollID == payrollID)
            .Include(s => s.MonthType)
            .FirstOrDefaultAsync();

        var employeePayrollData = new EmployeePayrollData
        {
          Employee = employeeSalary.Employee,
          BankDetails = bankAccountData,
          PayslipDetails = employeePayslip,
          SalaryDetails = salaryDetails,
          AdditionalAllowances = additionalAllowances,
          OvertimeData = overtimeData,
          Deductions = deductions,
          FixedDeductions = fixedDeductionDetails
        };

        employeePayrolls.Add(employeePayrollData);
      }

      var payrollViewModel = new MonthlyPayrollPrintViewModel
      {
        EmployeePayrolls = employeePayrolls
      };

      

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();

      return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", payrollViewModel);
    }

    public async Task<IActionResult> Print(int? branch, int? monthsTypeID, int? yearsTypeID)
    {
      var existingPayroll = await _appDBContext.HR_MonthlyPayrolls
          .Where(e => e.BranchTypeID == branch && e.MonthTypeID == monthsTypeID && e.Year == yearsTypeID)
          .FirstOrDefaultAsync();

      if (existingPayroll != null)
      {
        return await ForPrintPayroll(existingPayroll.PayrollID);
      }
      if (existingPayroll == null)
      {
        return await ForPrintPayroll(0);
      }

      return View("~/Views/HR/Reports/MonthlyPayslip/MonthlyPayslip.cshtml", existingPayroll);
    }
    public async Task<IActionResult> ForPrintPayroll(int payrollID)
    {
      if (payrollID == 0)
      {
        // Initialize an empty list for when payrollID is 0, representing no data scenario
        var emptyViewModel = new MonthlyPayrollPrintViewModel
        {
          EmployeePayrolls = new List<EmployeePayrollData>()
        };
        ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
        ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
        return View("~/Views/HR/Reports/MonthlyPayslip/PrintMonthlyPayslip.cshtml", emptyViewModel);
      }

      var employeeData = await _appDBContext.HR_MonthlyPayroll_Salarys
          .Where(s => s.PayRollID == payrollID)
          .Include(s => s.Employee)
          .Include(s => s.Employee.BranchType)
          .Include(s => s.Employee.DepartmentType)
          .Include(s => s.Employee.DesignationType)
          .OrderBy(s => s.EmployeeID)
          .ToListAsync();

      if (employeeData == null || !employeeData.Any())
      {
        // Handle the case when no employee data is found for a valid payrollID
        var emptyViewModel = new MonthlyPayrollPrintViewModel
        {
          EmployeePayrolls = new List<EmployeePayrollData>()
        };
        ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
        ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
        return View("~/Views/HR/Reports/MonthlyPayslip/PrintMonthlyPayslip.cshtml", emptyViewModel);
      }

      var employeePayrolls = new List<EmployeePayrollData>();

      foreach (var employeeSalary in employeeData)
      {
        // Retrieve salary details, allowances, overtime, deductions, and other payroll data
        var salaryDetails = await _appDBContext.HR_MonthlyPayroll_SalaryDetails
            .Where(sd => sd.PayrollSalary.EmployeeID == employeeSalary.EmployeeID && sd.PayrollSalary.PayRollID == payrollID)
            .Include(sd => sd.PayrollSalary)
            .Include(sd => sd.SalaryType)
            .ToListAsync();

        var additionalAllowances = await _appDBContext.HR_AddionalAllowanceDetails
            .Where(a => a.AddionalAllowance.EmployeeID == employeeSalary.EmployeeID && a.AddionalAllowance.PayRollID == payrollID)
            .Include(a => a.AddionalAllowance)
             .Include(a => a.AddionalAllowanceType)
            .ToListAsync();

        var overtimeData = await _appDBContext.HR_OverTimes
             .Where(o => o.EmployeeID == employeeSalary.EmployeeID && o.PayRollID == payrollID)
             .Include(o => o.OverTimeType)
             .GroupBy(o => new
             {
               o.OverTimeTypeID,
               o.OverTimeType.OverTimeTypeName
             })
             .Select(g => new HR_OverTimeTemp // Assuming HR_OverTime has a suitable constructor or properties
             {
               OverTimeTypeID = g.Key.OverTimeTypeID,
               OverTimeTypeName = g.Key.OverTimeTypeName,
               Amount = g.Sum(o => o.Amount) // Make sure 'Amount' is the right property name
             })
             .ToListAsync();

        var deductions = await _appDBContext.HR_Deductions
             .Where(d => d.EmployeeID == employeeSalary.EmployeeID && d.PayRollID == payrollID)
             .Include(d => d.DeductionType)
             .GroupBy(o => new
             {
               o.DeductionTypeID,
               o.DeductionType.DeductionTypeName
             })
              .Select(g => new HR_DeductionTemp // Assuming HR_OverTime has a suitable constructor or properties
              {
                DeductionTypeID = g.Key.DeductionTypeID,
                DeductionTypeName = g.Key.DeductionTypeName,
                Amount = g.Sum(o => o.Amount) // Make sure 'Amount' is the right property name
              })
             .ToListAsync();

        var fixedDeductionDetails = await _appDBContext.HR_MonthlyPayroll_FixedDeductionDetails
            .Where(fd => fd.PayrollFixedDeduction.EmployeeID == employeeSalary.EmployeeID && fd.PayrollFixedDeduction.PayRollID == payrollID)
            .Include(fd => fd.PayrollFixedDeduction)
            .Include(fd => fd.FixedDeductionType)
            .ToListAsync();

        var bankAccountData = await _appDBContext.HR_BankAccounts
            .Where(s => s.EmployeeID == employeeSalary.EmployeeID)
            .FirstOrDefaultAsync();

        var employeePayslip = await _appDBContext.HR_MonthlyPayrolls
            .Where(s => s.PayrollID == payrollID)
            .Include(s => s.MonthType)
            .FirstOrDefaultAsync();

        var employeePayrollData = new EmployeePayrollData
        {
          Employee = employeeSalary.Employee,
          BankDetails = bankAccountData,
          PayslipDetails = employeePayslip,
          SalaryDetails = salaryDetails,
          AdditionalAllowances = additionalAllowances,
          OvertimeData = overtimeData,
          Deductions = deductions,
          FixedDeductions = fixedDeductionDetails
        };

        employeePayrolls.Add(employeePayrollData);
      }

      var payrollViewModel = new MonthlyPayrollPrintViewModel
      {
        EmployeePayrolls = employeePayrolls
      };

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();

      return View("~/Views/HR/Reports/MonthlyPayslip/PrintMonthlyPayslip.cshtml", payrollViewModel);
    }

  }
}
