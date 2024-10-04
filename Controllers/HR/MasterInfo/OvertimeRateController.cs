using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class OvertimeRateController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public OvertimeRateController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {

      var OvertimeRatees = await _appDBContext.Settings_OverTimeRates
        .Where(b => b.DeleteYNID != 1)
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/OvertimeRate/OvertimeRate.cshtml", OvertimeRatees);
    }
    public async Task<IActionResult> OvertimeRate()
    {
      var OvertimeRates = await _appDBContext.Settings_OverTimeRates.ToListAsync();
      return Ok(OvertimeRates);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var OvertimeRate = await _appDBContext.Settings_OverTimeRates.FindAsync(id);
      if (OvertimeRate == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/OvertimeRate/EditOvertimeRate.cshtml", OvertimeRate);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_OverTimeRate OvertimeRate)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(OvertimeRate);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/OvertimeRate/EditOvertimeRate.cshtml", OvertimeRate);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/OvertimeRate/AddOvertimeRate.cshtml", new Settings_OverTimeRate());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_OverTimeRate OvertimeRate)
    {
      if (ModelState.IsValid)
      {
        OvertimeRate.DeleteYNID = 0;
        _appDBContext.Settings_OverTimeRates.Add(OvertimeRate);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/OvertimeRate/AddOvertimeRate.cshtml", OvertimeRate);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var OvertimeRate = await _appDBContext.Settings_OverTimeRates.FindAsync(id);
      if (OvertimeRate == null)
      {
        return NotFound();
      }

      OvertimeRate.ActiveYNID = 0;
      OvertimeRate.DeleteYNID = 1;

      _appDBContext.Settings_OverTimeRates.Update(OvertimeRate);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var OvertimeRatees = await _appDBContext.Settings_OverTimeRates
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("OvertimeRatees");
        worksheet.Cells["A1"].Value = "OvertimeRate ID";
        worksheet.Cells["B1"].Value = "OvertimeRate Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < OvertimeRatees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = OvertimeRatees[i].OverTimeRateTypeID;
          worksheet.Cells[i + 2, 2].Value = OvertimeRatees[i].OverTimeRateValue;
          worksheet.Cells[i + 2, 3].Value = OvertimeRatees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"OvertimeRatees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var OvertimeRatees = await _appDBContext.Settings_OverTimeRates
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/OvertimeRate/PrintOvertimeRates.cshtml", OvertimeRatees);
    }

  }
}
