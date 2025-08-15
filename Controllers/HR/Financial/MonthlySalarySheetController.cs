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
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class MonthlySalarySheetController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<MonthlySalarySheetController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AdditionalAllowanceController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public MonthlySalarySheetController(AppDBContext appDBContext, IConfiguration configuration, ILogger<AdditionalAllowanceController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<MonthlySalarySheetController> localizer)
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(int? Branch, int? MonthsTypeID, int? YearsTypeID)
    {
      if (!Branch.HasValue || !MonthsTypeID.HasValue || !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        MonthsTypeID ??= today.Month;
        YearsTypeID ??= today.Year;
        
      }
      int branchValue = Branch ?? 0;
      var employeeList = await _appDBContext.HR_Employees
            .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == branchValue)
            .ToListAsync();
      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(branchValue, employee.EmployeeID, MonthsTypeID.Value, YearsTypeID.Value);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);
      await PopulateDropdowns(MonthsTypeID, YearsTypeID, Branch);
      return View("~/Views/HR/Financial/MonthlySalarySheet/MonthlySalarySheet.cshtml", salarySheets);
    }
    private async Task PopulateDropdowns(int? month, int? year, int? Branch)
    {
      ViewBag.MonthsTypeID = month;
      ViewBag.YearsTypeID = year;
      ViewBag.Branch = Branch;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypesWithoutZeroLine();
      ViewBag.BranchList = await _utils.GetBranchsWithoutZeroLine();
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
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Already posting found for the specified branch, month, and year.");
          return Json(new { success = false});
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

            var additionalAllowances = await _appDBContext.HR_AdditionalAllowances
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
    public async Task<IActionResult> Print(int? branch, int? monthsTypeID, int? yearsTypeID)
    {

      if (!branch.HasValue || !monthsTypeID.HasValue || !yearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        monthsTypeID ??= today.Month;
        yearsTypeID ??= today.Year;

      }
      int branchValue = branch ?? 0;
      var employeeList = await _appDBContext.HR_Employees
            .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == branchValue)
            .ToListAsync();
      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(branchValue, employee.EmployeeID, monthsTypeID.Value, yearsTypeID.Value);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);
      return View("~/Views/HR/Financial/MonthlySalarySheet/PrintMonthlySalarySheet.cshtml", salarySheets);
    }
    public async Task<IActionResult> ExportToExcel(int? branch, int? monthsTypeID, int? yearsTypeID)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      if (!branch.HasValue || !monthsTypeID.HasValue || !yearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        //Branch ??= 1;
        monthsTypeID ??= today.Month;
        yearsTypeID ??= today.Year;

      }
      int branchValue = branch ?? 0;
      var employeeList = await _appDBContext.HR_Employees
            .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == branchValue)
            .ToListAsync();
      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(branchValue, employee.EmployeeID, monthsTypeID.Value, yearsTypeID.Value);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);

      // Convert to Excel
      byte[] excelData = ExportSalarySheetToExcel(salarySheets.ToList());

      string excelName = _localizer["lbl_MonlySalarySheet"] + $"-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";
      return File(excelData, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    }
    public byte[] ExportSalarySheetToExcel(List<MonthlySalarySheetViewModel> salarySheets)
    {
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("SalarySheet");

        // Step 1: Collect all distinct keys for dynamic columns
        var salaryDetailsColumns = new HashSet<string>();
        var allowancesColumns = new HashSet<string>();
        var overtimeColumns = new HashSet<string>();
        var deductionsColumns = new HashSet<string>();
        var fixedDeductionsColumns = new HashSet<string>();

        foreach (var sheet in salarySheets)
        {
          foreach (var key in sheet.SalaryDetails.Keys) salaryDetailsColumns.Add(key);
          foreach (var key in sheet.AdditionalAllowances.Keys) allowancesColumns.Add(key);
          foreach (var key in sheet.OvertimeDetails.Keys) overtimeColumns.Add(key);
          foreach (var key in sheet.Deductions.Keys) deductionsColumns.Add(key);
          foreach (var key in sheet.FixedDeductions.Keys) fixedDeductionsColumns.Add(key);
        }

        // Step 2: Define static headers
        int col = 1;
        worksheet.Cells[1, col++].Value = "Employee ID";
        worksheet.Cells[1, col++].Value = "Employee Name";
        worksheet.Cells[1, col++].Value = "Month";
        worksheet.Cells[1, col++].Value = "Year";

        // Step 3: Add dynamic columns
        var columnMappings = new Dictionary<string, int>();

        void AddDynamicColumns(HashSet<string> keys)
        {
          foreach (var key in keys)
          {
            if (!columnMappings.ContainsKey(key))
            {
              worksheet.Cells[1, col].Value = key;
              columnMappings[key] = col;
              col++;
            }
          }
        }

        AddDynamicColumns(salaryDetailsColumns);
        AddDynamicColumns(allowancesColumns);
        AddDynamicColumns(overtimeColumns);
        AddDynamicColumns(deductionsColumns);
        AddDynamicColumns(fixedDeductionsColumns);

        // Final column
        int deservedAmountCol = col;
        worksheet.Cells[1, col++].Value = "Deserved Salary";

        // Step 4: Fill rows
        for (int i = 0; i < salarySheets.Count; i++)
        {
          var sheet = salarySheets[i];
          int row = i + 2;

          worksheet.Cells[row, 1].Value = sheet.EmployeeID;
          worksheet.Cells[row, 2].Value = sheet.EmployeeName;
          worksheet.Cells[row, 3].Value = sheet.MonthID;
          worksheet.Cells[row, 4].Value = sheet.Year;

          void FillDynamicValues(Dictionary<string, decimal?> data)
          {
            foreach (var kvp in data)
            {
              if (columnMappings.ContainsKey(kvp.Key))
              {
                double value = Convert.ToDouble(kvp.Value ?? 0);
                worksheet.Cells[row, columnMappings[kvp.Key]].Value = Math.Round(value, 2);
              }
            }
          }

          FillDynamicValues(sheet.SalaryDetails);
          FillDynamicValues(sheet.AdditionalAllowances);
          FillDynamicValues(sheet.OvertimeDetails);
          FillDynamicValues(sheet.Deductions);
          FillDynamicValues(sheet.FixedDeductions);

          worksheet.Cells[row, deservedAmountCol].Value = sheet.DeservedAmount;
        }

        // Format
        worksheet.Row(1).Style.Font.Bold = true;
        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        return package.GetAsByteArray();
      }
    }

    public async Task<IActionResult> PrintDetail(int payrollID)
    {
      var monthlyPayroll = await _appDBContext.HR_MonthlyPayrolls
            .Where(e => e.PayrollID == payrollID )
            .FirstOrDefaultAsync();

      var Branch = monthlyPayroll.BranchTypeID;
      var monthsTypeID = monthlyPayroll.MonthTypeID;
      var yearsTypeID = monthlyPayroll.Year;

      var employeeList = await _appDBContext.HR_Employees
            .Where(e => e.ActiveYNID == 1 && e.FinalApprovalID == 1 && e.BranchTypeID == Branch)
            .ToListAsync();

      var salarySheets = new List<MonthlySalarySheetViewModel>();
      var tasks = employeeList.Select(async employee =>
      {
        var salaryData = await GetMonthlySalarySheetAsync(Branch.Value, employee.EmployeeID, monthsTypeID.Value, yearsTypeID.Value);
        return salaryData;
      });

      var salaryDataResults = await Task.WhenAll(tasks);
      salarySheets.AddRange(salaryDataResults);
      return View("~/Views/HR/Financial/MonthlySalarySheet/PrintMonthlySalarySheet.cshtml", salarySheets);
    }

  }
}
