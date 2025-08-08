using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class VacationReportController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<VacationReportController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public VacationReportController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<VacationReportController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
   
    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? VacationTypeID)
    {
      var vacationQuery = _appDBContext.HR_Vacations
          .Where(emp => emp.FinalApprovalID == 1);

      if (FromDate.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.StartDate >= FromDate.Value);
      }

      if (ToDate.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.EndDate <= ToDate.Value);
      }

      if (EmployeeID.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.EmployeeID == EmployeeID.Value);
      }

      if (VacationTypeID.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.VacationTypeID == VacationTypeID.Value);
      }

      var result = await vacationQuery
          .Select(emp => new VacationReportViewModel
          {
            EmployeeID = emp.EmployeeID,
            EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
            StartDate = emp.StartDate,
            EndDate = emp.EndDate,
            TotalDays = emp.TotalDays,

            TypeOfVacation = _appDBContext.HR_VacationSettles
                  .Where(vac => vac.VacationID == emp.VacationID && vac.FinalApprovalID == 1)
                  .Any() ? "Paid" : "UnPaid"
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToListAsync();

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.VacationTypeID = VacationTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.VacationTypeList = await _utils.GetVacationTypes();

      return View("~/Views/HR/Reports/VacationReport/VacationReport.cshtml", result);
    }
    public async Task<IActionResult> Print()
    {
      var vacationQuery = _appDBContext.HR_Vacations
          .Where(emp => emp.FinalApprovalID == 1);

      var result = await vacationQuery
          .Select(emp => new VacationReportViewModel
          {
            EmployeeID = emp.EmployeeID,
            EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
            StartDate = emp.StartDate,
            EndDate = emp.EndDate,
            TotalDays = emp.TotalDays,

            TypeOfVacation = _appDBContext.HR_VacationSettles
                  .Where(vac => vac.VacationID == emp.VacationID && vac.FinalApprovalID == 1)
                  .Any() ? "Paid" : "UnPaid"
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToListAsync();

   
      return View("~/Views/HR/Reports/VacationReport/PrintVacationReport.cshtml", result);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var vacationQuery = _appDBContext.HR_Vacations
          .Where(emp => emp.FinalApprovalID == 1);

      var result = await vacationQuery
          .Select(emp => new VacationReportViewModel
          {
            EmployeeID = emp.EmployeeID,
            EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
            StartDate = emp.StartDate,
            EndDate = emp.EndDate,
            TotalDays = emp.TotalDays,

            TypeOfVacation = _appDBContext.HR_VacationSettles
                  .Where(vac => vac.VacationID == emp.VacationID && vac.FinalApprovalID == 1)
                  .Any() ? "Paid" : "UnPaid"
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToListAsync();


      var vacationTypesList = await _utils.GetVacationTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_Vacation"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_StartDate"];
        worksheet.Cells["C1"].Value = _localizer["lbl_EndDate"];
        worksheet.Cells["D1"].Value = _localizer["lbl_TotalDays"];
        worksheet.Cells["E1"].Value = _localizer["lbl_VacationType"];

        for (int i = 0; i < result.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = result[i].EmployeeName;
          worksheet.Cells[i + 2, 2].Value = result[i].StartDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = result[i].EndDate?.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 5].Value = result[i].TotalDays;
          worksheet.Cells[i + 2, 6].Value = result[i].TypeOfVacation;

        }

        worksheet.Cells["B1:G1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_Vacation"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
