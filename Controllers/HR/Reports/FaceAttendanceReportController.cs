using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class FaceAttendanceReportController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<FaceAttendanceReportController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public FaceAttendanceReportController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<FaceAttendanceReportController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? DayID, int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      if (!DayID.HasValue && !EmployeeID.HasValue && !MonthsTypeID.HasValue && !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        DayID = today.Day;
        MonthsTypeID = today.Month;
        YearsTypeID = today.Year;
      }
      var query = _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
                YearsTypeID.HasValue && YearsTypeID != 0 &&
                DayID.HasValue && DayID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.MarkDate.Day == DayID.Value
        );
      }
      else if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
               YearsTypeID.HasValue && YearsTypeID != 0 &&
               EmployeeID.HasValue && EmployeeID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.EmployeeID == EmployeeID.Value
        );
      }
      else
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Please select either a Day or an Employee.");
        ViewBag.FaceAttendance = new List<CR_FaceAttendance>();
        await PopulateDropdowns(MonthsTypeID, YearsTypeID, EmployeeID, EmployeeName, DayID);
        return View("~/Views/HR/HR/FaceAttendanceReport/FaceAttendanceReport.cshtml", new List<CR_FaceAttendance>());
      }

      var faceAttendance = await query.ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, EmployeeID, EmployeeName, DayID);
     
      return View("~/Views/HR/Reports/FaceAttendanceReport/FaceAttendanceReport.cshtml", faceAttendance);
    }
    private async Task PopulateDropdowns(int? month, int? year, int? employeeId, string? employeeName, int? day)
    {
      ViewBag.MonthsTypeID = month;
      ViewBag.YearsTypeID = year;
      ViewBag.EmployeeID = employeeId;
      ViewBag.EmployeeName = employeeName;
      ViewBag.SelectedDay = day;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      // Find the FaceAttendance by ID
      var FaceAttendance = await _appDBContext.CR_FaceAttendances
                                         .Include(d => d.Employee)
                                         .FirstOrDefaultAsync(d => d.FaceAttendanceID == id && d.DeleteYNID != 1);

      if (FaceAttendance == null)
      {
        return NotFound();
      }

      return PartialView("~/Views/HR/Reports/FaceAttendanceReport/EditFaceAttendanceReport.cshtml", FaceAttendance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        try
        {
          _appDBContext.CR_FaceAttendances.Update(FaceAttendance);
          await _appDBContext.SaveChangesAsync();
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          ModelState.AddModelError("", "Unable to save changes: " + ex.Message);
        }
      }

      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return PartialView("~/Views/HR/Reports/FaceAttendanceReport/EditFaceAttendanceReport.cshtml", FaceAttendance);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      return PartialView("~/Views/HR/Reports/FaceAttendanceReport/AddOFaceAttendanceReport.cshtml", new CR_FaceAttendance());
    }
    [HttpPost]
    public async Task<IActionResult> Create(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        FaceAttendance.DeleteYNID = 0;
        _appDBContext.CR_FaceAttendances.Add(FaceAttendance);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/Reports/FaceAttendanceReport/AddFaceAttendanceReport.cshtml", FaceAttendance);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var FaceAttendance = await _appDBContext.CR_FaceAttendances.FindAsync(id);
      if (FaceAttendance == null)
      {
        return NotFound();
      }

      FaceAttendance.DeleteYNID = 1;

      _appDBContext.CR_FaceAttendances.Update(FaceAttendance);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel(int? DayID, int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var query = _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
                YearsTypeID.HasValue && YearsTypeID != 0 &&
                DayID.HasValue && DayID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.MarkDate.Day == DayID.Value
        );
      }
      else if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
               YearsTypeID.HasValue && YearsTypeID != 0 &&
               EmployeeID.HasValue && EmployeeID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.EmployeeID == EmployeeID.Value
        );
      }

      var faceAttendance = await query.ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, EmployeeID, EmployeeName, DayID);

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_FaceAttendance"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_FaceAttendanceID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Date"];
        worksheet.Cells["D1"].Value = _localizer["lbl_InTime"];
        worksheet.Cells["E1"].Value = _localizer["lbl_OutTime"];
        worksheet.Cells["F1"].Value = _localizer["lbl_DutyHours"];
        worksheet.Cells["G1"].Value = _localizer["lbl_DutyMinutes"];


        for (int i = 0; i < faceAttendance.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = faceAttendance[i].FaceAttendanceID;
          worksheet.Cells[i + 2, 2].Value = faceAttendance[i].Employee?.FirstName + ' ' + faceAttendance[i].Employee?.FatherName + ' ' + faceAttendance[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = faceAttendance[i].MarkDate.ToString("dd/MMM/yyyy");
          worksheet.Cells[i + 2, 4].Value = faceAttendance[i].InTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 5].Value = faceAttendance[i].OutTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 6].Value = faceAttendance[i].DHours;
          worksheet.Cells[i + 2, 7].Value = faceAttendance[i].DMinutes;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_FaceAttendance"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print(int? DayID, int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
                YearsTypeID.HasValue && YearsTypeID != 0 &&
                DayID.HasValue && DayID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.MarkDate.Day == DayID.Value
        );
      }
      else if (MonthsTypeID.HasValue && MonthsTypeID != 0 &&
               YearsTypeID.HasValue && YearsTypeID != 0 &&
               EmployeeID.HasValue && EmployeeID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        query = query.Where(d =>
            d.MarkDate >= startDate &&
            d.MarkDate <= endDate &&
            d.EmployeeID == EmployeeID.Value
        );
      }
     
      var faceAttendance = await query.ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, EmployeeID, EmployeeName, DayID);
      return View("~/Views/HR/Reports/FaceAttendanceReport/PrintFaceAttendanceReport.cshtml", faceAttendance);
    }
  }
}
