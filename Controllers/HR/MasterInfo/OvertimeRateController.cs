using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class OverTimeRateController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<OverTimeRateController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public OverTimeRateController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<OverTimeRateController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchOverTimeRateName)
    {

      var OverTimeRatesQuery = _appDBContext.Settings_OverTimeRates
          .Where(b => b.DeleteYNID != 1);

      if (float.TryParse(searchOverTimeRateName, out float searchValue))
      {
        OverTimeRatesQuery = OverTimeRatesQuery.Where(b => b.OverTimeRateValue == searchValue);
      }


      var OverTimeRates = await OverTimeRatesQuery.ToListAsync();

      if (OverTimeRates.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Over Time Rate found.");
      }

      return View("~/Views/HR/MasterInfo/OverTimeRate/OverTimeRate.cshtml", OverTimeRates);
    }

    public async Task<IActionResult> OverTimeRate()
    {
      var OverTimeRates = await _appDBContext.Settings_OverTimeRates.ToListAsync();
      return Ok(OverTimeRates);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var OverTimeRate = await _appDBContext.Settings_OverTimeRates.FindAsync(id);
      if (OverTimeRate == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/OverTimeRate/EditOverTimeRate.cshtml", OverTimeRate);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_OverTimeRate OverTimeRate)
    {
      if (ModelState.IsValid)
      {
        if (OverTimeRate.OverTimeRateValue == null && OverTimeRate.OverTimeRateValue == 0)
        {
          return Json(new { success = false, message = "OverTimeRate Value field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(OverTimeRate);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Overtime Rate Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating OverTimeRate Value. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/OverTimeRate/AddOverTimeRate.cshtml", new Settings_OverTimeRate());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_OverTimeRate OverTimeRate)
    {
      if (ModelState.IsValid)
      {
        if (OverTimeRate.OverTimeRateValue == null && OverTimeRate.OverTimeRateValue == 0)
        {
          return Json(new { success = false, message = "OverTimeRate Value field is required. Please enter a valid text value." });
        }
        OverTimeRate.DeleteYNID = 0;
        _appDBContext.Settings_OverTimeRates.Add(OverTimeRate);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Overtime Rate Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating OverTimeRate Value. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var OverTimeRate = await _appDBContext.Settings_OverTimeRates.FindAsync(id);
      if (OverTimeRate == null)
      {
        return NotFound();
      }

      OverTimeRate.ActiveYNID = 2;
      OverTimeRate.DeleteYNID = 1;

      _appDBContext.Settings_OverTimeRates.Update(OverTimeRate);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Overtime Rate Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var OverTimeRatees = await _appDBContext.Settings_OverTimeRates
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_OverTimeRate"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_OverTimeRateID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_OverTimeRateValue"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < OverTimeRatees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = OverTimeRatees[i].OverTimeRateTypeID;
          worksheet.Cells[i + 2, 2].Value = OverTimeRatees[i].OverTimeRateValue;
          worksheet.Cells[i + 2, 3].Value = OverTimeRatees[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_OverTimeRate"] +$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var OverTimeRatees = await _appDBContext.Settings_OverTimeRates
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/OverTimeRate/PrintOverTimeRate.cshtml", OverTimeRatees);
    }

  }
}
