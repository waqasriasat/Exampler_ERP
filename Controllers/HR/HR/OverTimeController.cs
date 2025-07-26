using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class OverTimeController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<OverTimeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public OverTimeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<OverTimeController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID, int? OvertimeTypeID)
    {
      var query = _appDBContext.HR_OverTimes
          .Where(b => b.DeleteYNID != 1)
          .Include(d => d.Employee)
          .Include(d => d.OverTimeType)
          .Include(d => d.MonthType)
          .AsQueryable();  // Move this up to allow further filtering

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        query = query.Where(d => d.MonthTypeID == MonthsTypeID.Value);
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

      if (OvertimeTypeID.HasValue && OvertimeTypeID != 0)
      {
        query = query.Where(d => d.OverTimeTypeID == OvertimeTypeID.Value);
      }

      var overTimes = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.OvertimeTypeID = OvertimeTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.OvertimeTypeList = await _utils.GetOverTimeTypes();  // Assuming a similar utility exists for overtime types

      return View("~/Views/HR/HR/OverTime/OverTime.cshtml", overTimes);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      // Load necessary data for dropdowns or lists
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.OverTimeTypesList = await _utils.GetOverTimeTypes();
      ViewBag.OvertimeRatesList = await _utils.GetOverTimeRates();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      // Find the OverTime by ID
      var OverTime = await _appDBContext.HR_OverTimes
                                         .Include(d => d.Employee)
                                         .Include(d => d.OverTimeType)
                                         .FirstOrDefaultAsync(d => d.OverTimeID == id && d.DeleteYNID != 1);

      if (OverTime == null)
      {
        return NotFound();
      }
      if (OverTime.PostedID != null && OverTime.PostedID != 0)
      {
        //await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "This deduction has already been posted to the Payroll Department and cannot be edited..");
        return NotFound();
      }

      // Return the partial view with the OverTime model
      return PartialView("~/Views/HR/HR/OverTime/EditOverTime.cshtml", OverTime);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(HR_OverTime OverTime)
    {
      if (ModelState.IsValid)
      {
        try
        {
          _appDBContext.HR_OverTimes.Update(OverTime);
          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "OverTime updated successfully.");
          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Unable to save changes: " + ex.Message);
        }
      }

      // If model state is invalid, reload dropdowns or lists
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.OverTimeTypesList = await _utils.GetOverTimeTypes();
      ViewBag.OvertimeRatesList = await _utils.GetOverTimeRates();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();

      // Return the partial view with validation errors
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error updating OverTime. Please check the inputs.");
      return PartialView("~/Views/HR/HR/OverTime/EditOverTime.cshtml", OverTime);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeesList = await _utils.GetEmployee();
      ViewBag.OverTimeTypesList = await _utils.GetOverTimeTypes();
      ViewBag.OvertimeRatesList = await _utils.GetOverTimeRates();
      ViewBag.MonthsList = await _utils.GetMonthsTypes();
      return PartialView("~/Views/HR/HR/OverTime/AddOverTime.cshtml", new HR_OverTime());
    }
    [HttpPost]
    public async Task<IActionResult> Create(HR_OverTime OverTime)
    {
      if (ModelState.IsValid)
      {
        OverTime.DeleteYNID = 0;
        _appDBContext.HR_OverTimes.Add(OverTime);
        await _appDBContext.SaveChangesAsync();

        int generatedOverTimeID = OverTime.OverTimeID;
        if (generatedOverTimeID > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 7)
                              .CountAsync();
          var getEmployeeID = await _appDBContext.HR_OverTimes
                            .Where(pta => pta.OverTimeID == generatedOverTimeID)
                            .FirstOrDefaultAsync();
          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 7,
              Notes = "OverTime",
              Date = DateTime.Now,
              EmployeeID = getEmployeeID.EmployeeID,
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = generatedOverTimeID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 7 && pta.Rank == 1)
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
              return Json(new { success = false, message = "Next approval setup not found." });
            }
          }
          else
          {
            OverTime.FinalApprovalID = 1;
            _appDBContext.HR_OverTimes.Update(OverTime);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "OverTime Created successfully. No process setup found, OverTime activated.");
          }
        }
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "OverTime Created successfully. Continue to the Approval Process Setup for OverTime Activation.");

        return Json(new { success = true });
      }
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "Error creating OverTime. Please check the inputs.");
      return PartialView("~/Views/HR/HR/OverTime/AddOverTime.cshtml", OverTime);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var OverTime = await _appDBContext.HR_OverTimes.FindAsync(id);
      if (OverTime == null)
      {
        return NotFound();
      }

      OverTime.DeleteYNID = 1;

      _appDBContext.HR_OverTimes.Update(OverTime);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "OverTime deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var OverTime = await _appDBContext.HR_OverTimes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(d => d.OverTimeType)
        .Include(d => d.MonthType)
        .Include(d => d.OverTimeRate)
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_OverTime"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_OverTimeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_OverTimeType"];
        worksheet.Cells["D1"].Value = _localizer["lbl_Month"];
        worksheet.Cells["E1"].Value = _localizer["lbl_Year"];
        worksheet.Cells["F1"].Value = _localizer["lbl_TotalDays"];
        worksheet.Cells["G1"].Value = _localizer["lbl_TotalHours"];
        worksheet.Cells["H1"].Value = _localizer["lbl_OverTimeRate"];
        worksheet.Cells["I1"].Value = _localizer["lbl_Amount"];


        for (int i = 0; i < OverTime.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = OverTime[i].OverTimeID;
          worksheet.Cells[i + 2, 2].Value = OverTime[i].Employee?.FirstName + ' ' + OverTime[i].Employee?.FatherName + ' ' + OverTime[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = OverTime[i].OverTimeType?.OverTimeTypeName;
          worksheet.Cells[i + 2, 4].Value = OverTime[i].MonthType?.MonthTypeName;
          worksheet.Cells[i + 2, 5].Value = OverTime[i].Year;
          worksheet.Cells[i + 2, 6].Value = OverTime[i].Days;
          worksheet.Cells[i + 2, 7].Value = OverTime[i].Hours;
          worksheet.Cells[i + 2, 8].Value = OverTime[i].OverTimeRate?.OverTimeRateValue;
          worksheet.Cells[i + 2, 9].Value = OverTime[i].Amount;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_OverTime"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var OverTimes = await _appDBContext.HR_OverTimes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Employee)
        .Include(d => d.OverTimeType)
        .Include(d => d.MonthType)
        .Include(d => d.OverTimeRate)
        .ToListAsync();
      return View("~/Views/HR/HR/OverTime/PrintOverTime.cshtml", OverTimes);
    }
  }

}
