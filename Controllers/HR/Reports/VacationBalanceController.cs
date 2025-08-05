using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class VacationBalanceController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<VacationBalanceController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public VacationBalanceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<VacationBalanceController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }

    public async Task<IActionResult> Index(int? id)
    {
      var today = DateTime.Today;

      var employeesQuery = _appDBContext.HR_Employees
          .AsNoTracking()
          .Select(emp => new
          {
            emp.EmployeeID,
            EmployeeName = emp.FirstName + " " + emp.FatherName + " " + emp.FamilyName,

            HaveVacation = _appDBContext.HR_Contracts
                  .Where(c => c.EmployeeID == emp.EmployeeID)
                  .Select(c => EF.Functions.DateDiffYear(c.StartDate, c.EndDate ?? today) * c.VacationDays)
                  .Sum() ?? 0,

            PaidVacation = _appDBContext.HR_VacationSettles
                  .Include(v => v.Vacation)
                  .Where(v => v.Vacation.EmployeeID == emp.EmployeeID && v.FinalApprovalID == 1 &&
                  v.VacationID == v.Vacation.VacationID && v.Vacation.FinalApprovalID ==1)
                  .Select(v => (double?)v.SettleDays)
                  .Sum() ?? 0
          });

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(emp => emp.EmployeeID == id.Value);
      }

      var employees = await employeesQuery.ToListAsync();

      var result = employees
          .Select(e => new VacationBalanceViewModel
          {
            EmployeeID = e.EmployeeID,
            EmployeeName = e.EmployeeName,
            HaveVacation = e.HaveVacation,
            PaidVacation = e.PaidVacation,
            TotalBalance = e.HaveVacation - e.PaidVacation
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToList();

      return View("~/Views/HR/Reports/VacationBalance/VacationBalance.cshtml", result);
    }

    public async Task<IActionResult> Print()
    {
      var today = DateTime.Today;

      var employeesQuery = _appDBContext.HR_Employees
          .AsNoTracking()
          .Select(emp => new
          {
            emp.EmployeeID,
            EmployeeName = emp.FirstName + " " + emp.FatherName + " " + emp.FamilyName,

            HaveVacation = _appDBContext.HR_Contracts
                  .Where(c => c.EmployeeID == emp.EmployeeID)
                  .Select(c => EF.Functions.DateDiffYear(c.StartDate, c.EndDate ?? today) * c.VacationDays)
                  .Sum() ?? 0,

            PaidVacation = _appDBContext.HR_VacationSettles
                  .Include(v => v.Vacation)
                  .Where(v => v.Vacation.EmployeeID == emp.EmployeeID && v.FinalApprovalID == 1 &&
                  v.VacationID == v.Vacation.VacationID && v.Vacation.FinalApprovalID == 1)
                  .Select(v => (double?)v.SettleDays)
                  .Sum() ?? 0
          });

      var employees = await employeesQuery.ToListAsync();

      var result = employees
          .Select(e => new VacationBalanceViewModel
          {
            EmployeeID = e.EmployeeID,
            EmployeeName = e.EmployeeName,
            HaveVacation = e.HaveVacation,
            PaidVacation = e.PaidVacation,
            TotalBalance = e.HaveVacation - e.PaidVacation
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToList();

      return View("~/Views/HR/Reports/VacationBalance/PrintVacationBalance.cshtml", result);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;


      var today = DateTime.Today;

      var employeesQuery = _appDBContext.HR_Employees
          .AsNoTracking()
          .Select(emp => new
          {
            emp.EmployeeID,
            EmployeeName = emp.FirstName + " " + emp.FatherName + " " + emp.FamilyName,

            HaveVacation = _appDBContext.HR_Contracts
                  .Where(c => c.EmployeeID == emp.EmployeeID)
                  .Select(c => EF.Functions.DateDiffYear(c.StartDate, c.EndDate ?? today) * c.VacationDays)
                  .Sum() ?? 0,

            PaidVacation = _appDBContext.HR_VacationSettles
                  .Include(v => v.Vacation)
                  .Where(v => v.Vacation.EmployeeID == emp.EmployeeID && v.FinalApprovalID == 1 &&
                  v.VacationID == v.Vacation.VacationID && v.Vacation.FinalApprovalID == 1)
                  .Select(v => (double?)v.SettleDays)
                  .Sum() ?? 0
          });

      var employees = await employeesQuery.ToListAsync();

      var result = employees
          .Select(e => new VacationBalanceViewModel
          {
            EmployeeID = e.EmployeeID,
            EmployeeName = e.EmployeeName,
            HaveVacation = e.HaveVacation,
            PaidVacation = e.PaidVacation,
            TotalBalance = e.HaveVacation - e.PaidVacation
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToList();



      var vacationTypesList = await _utils.GetVacationTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_VacationBalance"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_VacationType"];
    
        for (int i = 0; i < result.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = result[i].EmployeeName;
          worksheet.Cells[i + 2, 2].Value = result[i].TotalBalance;
        }

        worksheet.Cells["A1:B1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_VacationBalance"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
