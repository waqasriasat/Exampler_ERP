using Exampler_ERP.Controllers.HR.Employeement;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.AspNetCore.SignalR;
using Exampler_ERP.Controllers.HR.HR;
using Exampler_ERP.Hubs;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class MonthlySalarySheetController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<MonthlySalarySheetController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AddionalAllowanceController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public MonthlySalarySheetController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AddionalAllowanceController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<MonthlySalarySheetController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(int Branch, int MonthsTypeID = 10, int YearsTypeID = 1998)
    {
      var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == 0)
          .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(Branch, employee.EmployeeID, MonthsTypeID, YearsTypeID);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);


      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();

      return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", salarySheets);
    }
    public async Task<IActionResult> GeneratePayroll(int Branch, int MonthsTypeID, int YearsTypeID)
    {
      //var FatchExistingPosting = await _appDBContext.HR_MonthlyPayrollPosteds
      //  .Where(e => e.BranchTypeID == Branch && e.MonthTypeID == MonthsTypeID && e.Year == YearsTypeID)
      //  .FirstOrDefaultAsync();

      //if (FatchExistingPosting != null)
      //{
      //  ViewBag.MonthsTypeID = MonthsTypeID;
      //  ViewBag.YearsTypeID = YearsTypeID;
      //  ViewBag.Branch = Branch;

      //  ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      //  ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
      //  await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Already posting found for the specified branch, month, and year.");
      //  return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", new List<MonthlySalarySheetViewModel>());
      //}

      var employeeList = await _appDBContext.HR_Employees
          .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == Branch)
          .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(Branch, employee.EmployeeID, MonthsTypeID, YearsTypeID);
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
    private async Task<MonthlySalarySheetViewModel> GetMonthlySalarySheetAsync(int Branch, int employeeId, int month, int year)
    {


      var salarySheet = new MonthlySalarySheetViewModel
      {
        EmployeeID = employeeId
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

        var commandText = "EXEC GetMonthlySalarySheet @BranchID, @EmployeeID, @Month, @Year;";
        using (var command = new SqlCommand(commandText, connection))
        {
          command.Parameters.AddWithValue("@BranchID", Branch);
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
        var FatchExistingPosting = await _appDBContext.HR_MonthlyPayrollPosteds
        .Where(e => e.BranchTypeID == BranchID && e.MonthTypeID == MonthID && e.Year == Year)
        .FirstOrDefaultAsync();

        if (FatchExistingPosting != null)
        {
          return Json(new { success = false, message = "Already posting found for the specified branch, month, and year." });
        }
        var monthlyPayrollPosted = new HR_MonthlyPayrollPosted
        {
          BranchTypeID = BranchID,
          MonthTypeID = MonthID,
          Year = Year
        };
        _appDBContext.HR_MonthlyPayrollPosteds.Add(monthlyPayrollPosted);
        await _appDBContext.SaveChangesAsync();

        int PayrollPostedID = monthlyPayrollPosted.PayrollPostedID;



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
              EmployeeID = 1,
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
              await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
            }
            else
            {
              return Json(new { success = false, Message = "Next approval setup not found." });
            }
          }
          else
          {
            var monthlyPayroll = new HR_MonthlyPayroll
            {
              BranchTypeID = BranchID,
              MonthTypeID = MonthID,
              Year = Year
            };

            _appDBContext.HR_MonthlyPayrolls.Add(monthlyPayroll);
            await _appDBContext.SaveChangesAsync();

            var additionalAllowances = await _appDBContext.HR_AddionalAllowances
              .Where(a => a.MonthTypeID == MonthID && a.Year == Year)
              .ToListAsync();

            foreach (var allowance in additionalAllowances)
            {
              allowance.PayRollID = monthlyPayroll.PayrollID;
              allowance.PostedID = PayrollPostedID;
            }

            var overTimes = await _appDBContext.HR_OverTimes
                .Where(o => o.MonthTypeID == MonthID && o.Year == Year)
                .ToListAsync();

            foreach (var overtime in overTimes)
            {
              overtime.PayRollID = monthlyPayroll.PayrollID;
              overtime.PostedID = PayrollPostedID;
            }

            var hrDeductions = await _appDBContext.HR_Deductions
                .Where(d => d.Month == MonthID && d.Year == Year)
                .ToListAsync();

            foreach (var deduction in hrDeductions)
            {
              deduction.PayRollID = monthlyPayroll.PayrollID;
              deduction.PostedID = PayrollPostedID;
            }
            await _appDBContext.SaveChangesAsync();







            var salaryDetails = await _appDBContext.HR_SalaryDetails
            .Where(sd => sd.Salary.Employee.BranchTypeID == BranchID)
            .Include(sd => sd.Salary)
            .Include(sd => sd.Salary.Employee)
            .ToListAsync();

            foreach (var salaryGroup in salaryDetails.GroupBy(sd => sd.Salary.EmployeeID))
            {
              var firstSalaryDetail = salaryGroup.FirstOrDefault();
              if (firstSalaryDetail?.Salary != null)
              {
                var monthlyPayroll_Salary = new HR_MonthlyPayroll_Salary
                {
                  EmployeeID = firstSalaryDetail.Salary.EmployeeID,
                  MonthTypeID = MonthID,
                  Year = Year,
                  PostedID = PayrollPostedID,
                  PayRollID = monthlyPayroll.PayrollID // Assuming this is available
                };

                _appDBContext.HR_MonthlyPayroll_Salarys.Add(monthlyPayroll_Salary);
                await _appDBContext.SaveChangesAsync();

                foreach (var salaryDetail in salaryGroup)
                {
                  var monthlyPayroll_SalaryDetail = new HR_MonthlyPayroll_SalaryDetail
                  {
                    PayrollSalaryID = monthlyPayroll_Salary.PayrollSalaryID, // Use the same PayrollSalaryID
                    SalaryTypeID = salaryDetail.SalaryTypeID,
                    SalaryAmount = salaryDetail.SalaryAmount
                  };

                  _appDBContext.HR_MonthlyPayroll_SalaryDetails.Add(monthlyPayroll_SalaryDetail);
                }

                await _appDBContext.SaveChangesAsync();
              }
            }




            var FixedDeductionDetails = await _appDBContext.HR_FixedDeductionDetails
           .Where(sd => sd.FixedDeduction.Employee.BranchTypeID == BranchID)
           .Include(sd => sd.FixedDeduction)
           .Include(sd => sd.FixedDeduction.Employee)
           .ToListAsync();

            foreach (var FixedDeductionGroup in FixedDeductionDetails.GroupBy(sd => sd.FixedDeduction.EmployeeID))
            {
              var firstFixedDeductionDetail = FixedDeductionGroup.FirstOrDefault();
              if (firstFixedDeductionDetail?.FixedDeduction != null)
              {
                var monthlyPayroll_FixedDeduction = new HR_MonthlyPayroll_FixedDeduction
                {
                  EmployeeID = firstFixedDeductionDetail.FixedDeduction.EmployeeID,
                  MonthTypeID = MonthID,
                  Year = Year,
                  PostedID = PayrollPostedID,
                  PayRollID = monthlyPayroll.PayrollID // Assuming this is available
                };

                _appDBContext.HR_MonthlyPayroll_FixedDeductions.Add(monthlyPayroll_FixedDeduction);
                await _appDBContext.SaveChangesAsync();

                foreach (var FixedDeductionDetail in FixedDeductionGroup)
                {
                  var monthlyPayroll_FixedDeductionDetail = new HR_MonthlyPayroll_FixedDeductionDetail
                  {
                    PayrollFixedDeductionID = monthlyPayroll_FixedDeduction.PayrollFixedDeductionID, // Use the same PayrollFixedDeductionID
                    FixedDeductionTypeID = FixedDeductionDetail.FixedDeductionTypeID,
                    FixedDeductionAmount = FixedDeductionDetail.FixedDeductionAmount
                  };

                  _appDBContext.HR_MonthlyPayroll_FixedDeductionDetails.Add(monthlyPayroll_FixedDeductionDetail);
                }

                await _appDBContext.SaveChangesAsync();
              }
            }




            monthlyPayrollPosted.FinalApprovalID = 1;
            _appDBContext.HR_MonthlyPayrollPosteds.Update(monthlyPayrollPosted);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true, message = "Salary created successfully. No process setup found, Salary activated." });
          }
        }
        return Json(new { success = true, message = "Salary Created successfully. Continue to the Approval Process Setup for Salary Final Approved." });
      }
      catch (Exception ex)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating Salary. " + ex);
        // Handle the error and return failure response
        return Json(new { success = false, message = ex.Message });
      }
    }
    public async Task<IActionResult> Print(int Branch, int MonthsTypeID, int YearsTypeID)
    {

      var employeeList = await _appDBContext.HR_Employees
        .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == Branch)
        .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(Branch, employee.EmployeeID, MonthsTypeID, YearsTypeID);
        return salaryData;
      });
      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);
      return View("~/Views/HR/Financial/MonthlySalarySheet/PrintMonthlySalarySheet.cshtml", salarySheets);
    }
  }
}
