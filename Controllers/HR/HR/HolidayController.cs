using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;
using Microsoft.CodeAnalysis.Operations;

namespace Exampler_ERP.Controllers.HR.HR
{
  public class HolidayController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HolidayController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public HolidayController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HolidayController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }

    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? HolidayTypeName, int? HolidayTypeID)
    {
      if (!MonthsTypeID.HasValue && !YearsTypeID.HasValue)
      {
        var today = DateTime.Today;
        MonthsTypeID = today.Month;
        YearsTypeID = today.Year;
      }


      var HolidayQuery = _appDBContext.HR_Holidays
          .Where(c => c.DeleteYNID != 1);

      if (HolidayTypeID.HasValue && HolidayTypeID > 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayTypeID == HolidayTypeID.Value);
      }

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Month == MonthsTypeID.Value);
      }
      if (YearsTypeID.HasValue && YearsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Year == YearsTypeID.Value);
      }
      var Holidays = await HolidayQuery
          .Include(c => c.HolidayType)
      .ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, HolidayTypeID);

      if (Holidays.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Holidays Found for the selected filters.");
      }

      return View("~/Views/HR/HR/Holiday/Holiday.cshtml", Holidays);
    }

    private async Task PopulateDropdowns(int? monthsTypeID, int? yearsTypeID, int? holidayTypeID)
    {
      ViewBag.MonthsTypeID = monthsTypeID;
      ViewBag.YearsTypeID = yearsTypeID;;
      ViewBag.HolidayTypeID = holidayTypeID;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
      ViewBag.HolidayTypeList = await _utils.GetHolidayTypes();
    }

    public async Task<IActionResult> Holiday()
    {
      var Holidays = await _appDBContext.HR_Holidays.ToListAsync();
      return Ok(Holidays);
    }

    public async Task<IActionResult> Edit(int id)
    {
      var Holiday = await _appDBContext.HR_Holidays.FindAsync(id);
      if (Holiday == null)
      {
        return NotFound();
      }

      ViewBag.HolidayTypesList = await _utils.GetHolidayTypes();

      return PartialView("~/Views/HR/HR/Holiday/EditHoliday.cshtml", Holiday);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_Holiday Holiday)
    {
      if (ModelState.IsValid)
      {

        _appDBContext.Update(Holiday);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Holiday. Please check the inputs." });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.HolidayTypesList = await _utils.GetHolidayTypes();
      return PartialView("~/Views/HR/HR/Holiday/AddHoliday.cshtml", new HR_Holiday());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_Holiday Holiday)
    {
      if (ModelState.IsValid)
      {

        Holiday.DeleteYNID = 0;
        Holiday.ActiveYNID = 2;
        Holiday.FinalApprovalID = 0;
        _appDBContext.HR_Holidays.Add(Holiday);
        await _appDBContext.SaveChangesAsync();

        var HolidayId = Holiday.HolidayID;
        if (HolidayId > 0)
        {
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                              .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 15)
                              .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 15,
              Notes = "Holiday Approval",
              Date = DateTime.Now,
              EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
              TransactionID = Holiday.HolidayID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .Where(pta => pta.ProcessTypeID == 15 && pta.Rank == 1)
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
            Holiday.ActiveYNID = 1;
            Holiday.FinalApprovalID = 1;
            _appDBContext.HR_Holidays.Update(Holiday);
            await _appDBContext.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Created successfully. No process setup found, Holiday activated.");
            return Json(new { success = true, message = "No process setup found, Holiday activated." });
          }
        }
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Created successfully. Continue to the Approval Process Setup for Holiday Activation.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Employee. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var Holiday = await _appDBContext.HR_Holidays.FindAsync(id);
      if (Holiday == null)
      {
        return NotFound();
      }

      Holiday.ActiveYNID = 2;
      Holiday.DeleteYNID = 1;

      _appDBContext.HR_Holidays.Update(Holiday);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Deleted successfully.");
      return Json(new { success = true });
    }

    public async Task<IActionResult> Print(int? MonthsTypeID, int? YearsTypeID, string? HolidayTypeName, int? HolidayTypeID)
    {
      var HolidayQuery = _appDBContext.HR_Holidays
          .Where(c => c.DeleteYNID != 1);

      if (HolidayTypeID.HasValue && HolidayTypeID > 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayTypeID == HolidayTypeID.Value);
      }

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Month == MonthsTypeID.Value);
      }
      if (YearsTypeID.HasValue && YearsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Year == YearsTypeID.Value);
      }
      var Holidays = await HolidayQuery
          .Include(c => c.HolidayType)
      .ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, HolidayTypeID);

      return View("~/Views/HR/HR/Holiday/PrintHoliday.cshtml", Holidays);
    }

    public async Task<IActionResult> ExportToExcel(int? MonthsTypeID, int? YearsTypeID, string? HolidayTypeName, int? HolidayTypeID)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HolidayQuery = _appDBContext.HR_Holidays
          .Where(c => c.DeleteYNID != 1);

      if (HolidayTypeID.HasValue && HolidayTypeID > 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayTypeID == HolidayTypeID.Value);
      }

      if (MonthsTypeID.HasValue && MonthsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Month == MonthsTypeID.Value);
      }
      if (YearsTypeID.HasValue && YearsTypeID != 0)
      {
        HolidayQuery = HolidayQuery.Where(c => c.HolidayDate.Year == YearsTypeID.Value);
      }
      var Holidays = await HolidayQuery
          .Include(c => c.HolidayType)
      .ToListAsync();

      await PopulateDropdowns(MonthsTypeID, YearsTypeID, HolidayTypeID);

      var SalaryTypesList = await _utils.GetSalaryOptions();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_Holiday"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_HolidayID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_HolidayDate"];
        worksheet.Cells["C1"].Value = _localizer["lbl_HolidayType"];
        worksheet.Cells["D1"].Value = _localizer["lbl_FinalApprovalID"];
     
        for (int i = 0; i < Holidays.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Holidays[i].HolidayID;
          worksheet.Cells[i + 2, 2].Value = Holidays[i].HolidayDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = Holidays[i].HolidayType?.HolidayTypeName;
          worksheet.Cells[i + 2, 4].Value = Holidays[i].FinalApprovalID;
        }

        worksheet.Cells["A1:J1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_Holiday"] +$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
