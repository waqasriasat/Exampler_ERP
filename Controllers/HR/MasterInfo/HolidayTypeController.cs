using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class HolidayTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HolidayTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HolidayTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HolidayTypeController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchHolidayTypeName)
    {

      var HolidayTypesQuery = _appDBContext.Settings_HolidayTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchHolidayTypeName))
      {
        HolidayTypesQuery = HolidayTypesQuery.Where(b => b.HolidayTypeName.Contains(searchHolidayTypeName));
      }

      var HolidayTypes = await HolidayTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchHolidayTypeName) && HolidayTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No HolidayType found with the name '" + searchHolidayTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/HolidayType/HolidayType.cshtml", HolidayTypes);
    }

    public async Task<IActionResult> HolidayType()
    {
      var HolidayTypes = await _appDBContext.Settings_HolidayTypes.ToListAsync();
      return Ok(HolidayTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HolidayType = await _appDBContext.Settings_HolidayTypes.FindAsync(id);
      if (HolidayType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/HolidayType/EditHolidayType.cshtml", HolidayType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HolidayType HolidayType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HolidayType.HolidayTypeName))
        {
          return Json(new { success = false, message = "HolidayType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(HolidayType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HolidayType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/HolidayType/AddHolidayType.cshtml", new Settings_HolidayType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HolidayType HolidayType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HolidayType.HolidayTypeName))
        {
          return Json(new { success = false, message = "HolidayType Name field is required. Please enter a valid text value." });
        }
        HolidayType.DeleteYNID = 0;
        _appDBContext.Settings_HolidayTypes.Add(HolidayType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HolidayType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var HolidayType = await _appDBContext.Settings_HolidayTypes.FindAsync(id);
      if (HolidayType == null)
      {
        return NotFound();
      }

      HolidayType.ActiveYNID = 2;
      HolidayType.DeleteYNID = 1;

      _appDBContext.Settings_HolidayTypes.Update(HolidayType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Holiday Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HolidayTypees = await _appDBContext.Settings_HolidayTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HolidayTypees");
        worksheet.Cells["A1"].Value = "HolidayType ID";
        worksheet.Cells["B1"].Value = "HolidayType Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < HolidayTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HolidayTypees[i].HolidayTypeID;
          worksheet.Cells[i + 2, 2].Value = HolidayTypees[i].HolidayTypeName;
          worksheet.Cells[i + 2, 3].Value = HolidayTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HolidayTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HolidayTypees = await _appDBContext.Settings_HolidayTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/HolidayType/PrintHolidayTypes.cshtml", HolidayTypees);
    }


  }
}
