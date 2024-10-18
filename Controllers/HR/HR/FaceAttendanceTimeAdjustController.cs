using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class FaceAttendanceTimeAdjustController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public FaceAttendanceTimeAdjustController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.CR_FaceAttendances
           .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);  // Last day of the selected month

        query = query.Where(d => d.MarkDate >= startDate && d.MarkDate <= endDate);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
            .Contains(EmployeeName));
      }


      var faceAttendance = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return View("~/Views/HR/HR/FaceAttendanceTimeAdjust/FaceAttendanceTimeAdjust.cshtml", faceAttendance);
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

      return PartialView("~/Views/HR/HR/FaceAttendanceTimeAdjust/EditFaceAttendanceTimeAdjust.cshtml", FaceAttendance);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var duration = FaceAttendance.OutTime - FaceAttendance.InTime;
          FaceAttendance.DHours = (int)duration.TotalHours;
          FaceAttendance.DMinutes = duration.Minutes;

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

      return PartialView("~/Views/HR/HR/FaceAttendanceTimeAdjust/EditFaceAttendanceTimeAdjust.cshtml", FaceAttendance);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      return PartialView("~/Views/HR/HR/FaceAttendanceTimeAdjust/AddFaceAttendanceTimeAdjust.cshtml", new CR_FaceAttendance());
    }
    [HttpPost]
    public async Task<IActionResult> Create(CR_FaceAttendance FaceAttendance)
    {
      if (ModelState.IsValid)
      {
        FaceAttendance.DeleteYNID = 0;


        //if(FaceAttendance.InTime != null && FaceAttendance.OutTime != null)
        { 
        var duration = FaceAttendance.OutTime - FaceAttendance.InTime;
        FaceAttendance.DHours = (int)duration.TotalHours;
        FaceAttendance.DMinutes = duration.Minutes;
        }

        _appDBContext.CR_FaceAttendances.Add(FaceAttendance);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/HR/FaceAttendanceTimeAdjust/AddFaceAttendanceTimeAdjust.cshtml", FaceAttendance);
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
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var FaceAttendance = await _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("FaceAttendance");
        worksheet.Cells["A1"].Value = "FaceAttendance ID";
        worksheet.Cells["B1"].Value = "Employee Name";
        worksheet.Cells["C1"].Value = "Date";
        worksheet.Cells["D1"].Value = "In Time";
        worksheet.Cells["E1"].Value = "Out Time";
        worksheet.Cells["F1"].Value = "D-Hours";
        worksheet.Cells["G1"].Value = "M-Hours";


        for (int i = 0; i < FaceAttendance.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = FaceAttendance[i].FaceAttendanceID;
          worksheet.Cells[i + 2, 2].Value = FaceAttendance[i].Employee?.FirstName + ' ' + FaceAttendance[i].Employee?.FatherName + ' ' + FaceAttendance[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = FaceAttendance[i].MarkDate.ToString("dd/MMM/yyyy");
          worksheet.Cells[i + 2, 4].Value = FaceAttendance[i].InTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 5].Value = FaceAttendance[i].OutTime.ToString("hh:mm:ss");
          worksheet.Cells[i + 2, 6].Value = FaceAttendance[i].DHours;
          worksheet.Cells[i + 2, 7].Value = FaceAttendance[i].DMinutes;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"FaceAttendance-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var FaceAttendances = await _appDBContext.CR_FaceAttendances
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();
      return View("~/Views/HR/HR/FaceAttendanceTimeAdjust/PrintFaceAttendanceTimeAdjust.cshtml", FaceAttendances);
    }
  }
}
