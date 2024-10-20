using Exampler_ERP.Controllers.HR.Employeement;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class MonthlySalarySheetController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<MonthlySalarySheetController> _logger;
    private readonly Utils _utils;

    public MonthlySalarySheetController(AppDBContext appDBContext, IConfiguration configuration, ILogger<MonthlySalarySheetController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }

    public async Task<IActionResult> Index(int? Branch, int MonthsTypeID = 10, int YearsTypeID = 1998)
    {
      var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1)
          .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(employee.EmployeeID.ToString(), MonthsTypeID, YearsTypeID);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.Branch = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();

      return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", salarySheets);
    }
    public async Task<IActionResult> GeneratePayroll(int Branch, int MonthsTypeID, int YearsTypeID)
    {
      var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID==Branch)
          .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(employee.EmployeeID.ToString(), MonthsTypeID, YearsTypeID);
        return salaryData;
      });
      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.Branch = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();

      return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", salarySheets);
    }
    private async Task<MonthlySalarySheetViewModel> GetMonthlySalarySheetAsync(string employeeId, int month, int year)
    {
      var salarySheet = new MonthlySalarySheetViewModel
      {
        EmployeeID = int.Parse(employeeId)
      };
      decimal sumSalary = 0;
      decimal sumAdditionalAllowance = 0;
      decimal sumOverTime = 0;
      decimal sumDeduction = 0;
      decimal sumFixedDeduction = 0;

      var connectionString = _configuration.GetConnectionString("AppDb");
      using (var connection = new SqlConnection(connectionString))
      {
        await connection.OpenAsync();

        var commandText = "EXEC GetMonthlySalarySheet @EmployeeID, @Month, @Year;";
        using (var command = new SqlCommand(commandText, connection))
        {
          command.Parameters.AddWithValue("@EmployeeID", employeeId);
          command.Parameters.AddWithValue("@Month", month);
          command.Parameters.AddWithValue("@Year", year);

          using (var reader = await command.ExecuteReaderAsync())
          {
            while (await reader.ReadAsync())
            {
              for (int i = 0; i < reader.FieldCount; i++)
              {
                string columnName = reader.GetName(i);

                // Handle nullable or non-nullable values
                var value = reader.IsDBNull(i) ? "0" : reader.GetValue(i).ToString();

                if (columnName == "EmployeeName")
                {
                  salarySheet.EmployeeName = value; 
                }
                if (columnName == "BranchID")
                {
                  salarySheet.BranchID = int.Parse(value); 
                }
                if (columnName == "MonthID")
                {
                  salarySheet.MonthID = int.Parse(value);
                }
                if (columnName == "MonthName")
                {
                  salarySheet.MonthName = value; 
                }
                if (columnName == "Year")
                {
                  salarySheet.Year = int.Parse(value); 
                }

                // Process each column by category
                if (columnName.StartsWith("Salary_"))
                {
                  salarySheet.SalaryDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumSalary += decimal.Parse(value);
                }
                else if (columnName.StartsWith("AdditionalAllowance_"))
                {
                  salarySheet.AdditionalAllowances.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumAdditionalAllowance += decimal.Parse(value);
                }
                else if (columnName.StartsWith("OverTime_"))
                {
                  salarySheet.OvertimeDetails.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumOverTime += decimal.Parse(value);
                }
                else if (columnName.StartsWith("Deduction_"))
                {
                  salarySheet.Deductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumDeduction += decimal.Parse(value);
                }
                else if (columnName.StartsWith("FixedDeduction_"))
                {
                  salarySheet.FixedDeductions.Add(columnName.Split('_')[1], decimal.Parse(value));
                  sumFixedDeduction += decimal.Parse(value);
                }
              }
            }
          }
        }
      }
      salarySheet.SumSalary = sumSalary;
      salarySheet.SumAdditionalAllowance = sumAdditionalAllowance;
      salarySheet.SumOverTime = sumOverTime;
      salarySheet.SumDeduction = sumDeduction;
      salarySheet.SumFixedDeduction = sumFixedDeduction;

      salarySheet.DeservedAmount = (sumSalary + sumAdditionalAllowance + sumOverTime) - (sumDeduction + sumFixedDeduction);

      return salarySheet;
    }
    [HttpPost]
    public async Task<JsonResult> UpdatePostedAsync(int BranchID, int MonthID, int Year)
    {
      try
      {
        var monthlyPayrollPosted = new HR_MonthlyPayrollPosted
        {
          BranchTypeID = BranchID,
          MonthTypeID = MonthID,
          Year = Year
        };
        _appDBContext.HR_MonthlyPayrollPosteds.Add(monthlyPayrollPosted);
        await _appDBContext.SaveChangesAsync();

        int PayrollPostedID = monthlyPayrollPosted.PayrollPostedID;

        var additionalAllowances = await _appDBContext.HR_AddionalAllowances
              .Where(a => a.MonthTypeID == MonthID && a.Year == Year)
              .ToListAsync();

          foreach (var allowance in additionalAllowances)
          {
            allowance.PostedID = PayrollPostedID;
          }

          var overTimes = await _appDBContext.HR_OverTimes
              .Where(o => o.MonthTypeID == MonthID && o.Year == Year)
              .ToListAsync();

          foreach (var overtime in overTimes)
          {
            overtime.PostedID = PayrollPostedID;
          }

          var hrDeductions = await _appDBContext.HR_Deductions
              .Where(d => d.Month == MonthID && d.Year == Year)
              .ToListAsync();

          foreach (var deduction in hrDeductions)
          {
            deduction.PostedID = PayrollPostedID;
          }
          await _appDBContext.SaveChangesAsync();

        if (PayrollPostedID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 13)
                              .CountAsync();
          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 13,
              Notes = "PayRoll Posted",
              Date = DateTime.Now,
              EmployeeID = 0,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = PayrollPostedID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 13 && pta.Rank == 1)
                                        .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();
            }
            else
            {
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            monthlyPayrollPosted.FinalApprovalID = 1;
            _appDBContext.HR_MonthlyPayrollPosteds.Update(monthlyPayrollPosted);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true, message = "No process setup found." });
          }
        }

        return Json(new { success = true });
      }
      catch (Exception ex)
      {
        // Handle the error and return failure response
        return Json(new { success = false, message = ex.Message });
      }
    }



    public async Task<IActionResult> PrintCard(int employeeId)
    {
      var employee = await _appDBContext.HR_Employees
          .Include(e => e.DesignationType)
          .Include(e => e.DepartmentType)
          .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

      // If employee is not found, return a 404 Not Found result
      if (employee == null)
      {
        return NotFound();
      }

      // Return the custom view for printing the employee card, passing the employee model
      return View("~/Views/HR/Financial/MonthlySalarySheet/PrintMonthlySalarySheet.cshtml", employee);
    }
  }
}
