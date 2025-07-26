using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Financial
{
  public class WorkDayController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<WorkDayController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public WorkDayController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<WorkDayController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.HR_WorkDays
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(d => d.Month == MonthsTypeID.Value);
      }

      if (YearsTypeID.HasValue)
      {
        query = query.Where(d => d.Year == YearsTypeID.Value);
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



      var WorkDays = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();

      return View("~/Views/HR/financial/WorkDay/WorkDay.cshtml", WorkDays);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      // Load necessary data for dropdowns or lists
      ViewBag.EmployeesList = await _utils.GetEmployee();

      // Find the WorkDay by ID
      var WorkDay = await _appDBContext.HR_WorkDays
                                         .Include(d => d.Employee)
                                         .FirstOrDefaultAsync(d => d.WorkDayID == id && d.DeleteYNID != 1);

      if (WorkDay == null)
      {
        return NotFound();
      }

      // Return the partial view with the WorkDay model
      return PartialView("~/Views/HR/financial/WorkDay/EditWorkDay.cshtml", WorkDay);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_WorkDay WorkDay)
    {
      if (ModelState.IsValid)
      {
        try
        {
          _appDBContext.HR_WorkDays.Update(WorkDay);
          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "WorkDay updated successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Unable to save changes: " + ex.Message);
        }
      }

      ViewBag.EmployeeList = await _utils.GetEmployee();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating WorkDay. Please check the inputs.");
      return PartialView("~/Views/HR/financial/WorkDay/EditWorkDay.cshtml", WorkDay);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      return PartialView("~/Views/HR/financial/WorkDay/AddWorkDay.cshtml", new HR_WorkDay());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_WorkDay WorkDay)
    {
      if (ModelState.IsValid)
      {
        WorkDay.DeleteYNID = 0;
        _appDBContext.HR_WorkDays.Add(WorkDay);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "WorkDay created successfully.");
        return Json(new { success = true });
      }
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating WorkDay. Please check the inputs.");
      return PartialView("~/Views/HR/financial/WorkDay/AddWorkDay.cshtml", WorkDay);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var WorkDay = await _appDBContext.HR_WorkDays.FindAsync(id);
      if (WorkDay == null)
      {
        return NotFound();
      }

      WorkDay.DeleteYNID = 1;

      _appDBContext.HR_WorkDays.Update(WorkDay);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "WorkDay deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var WorkDay = await _appDBContext.HR_WorkDays
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_WorkDay"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_WorkDay ID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Month"];
        worksheet.Cells["D1"].Value = _localizer["lbl_Year"];
        worksheet.Cells["E1"].Value = _localizer["lbl_DeducationDay"];


        for (int i = 0; i < WorkDay.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = WorkDay[i].WorkDayID;
          worksheet.Cells[i + 2, 2].Value = WorkDay[i].Employee?.FirstName + ' ' + WorkDay[i].Employee?.FatherName + ' ' + WorkDay[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = WorkDay[i].Month;
          worksheet.Cells[i + 2, 4].Value = WorkDay[i].Year;
          worksheet.Cells[i + 2, 5].Value = WorkDay[i].Days;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_WorkDay"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var WorkDay = await _appDBContext.HR_WorkDays
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .ToListAsync();
      return View("~/Views/HR/financial/WorkDay/PrintWorkDay.cshtml", WorkDay);
    }
  }
}
