using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class HeadofAccount_SecondController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HeadofAccount_SecondController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HeadofAccount_SecondController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HeadofAccount_SecondController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchSecondName)
    {
      var HeadofAccount_SecondsQuery = _appDBContext.Settings_HeadofAccount_Seconds
          .Include(b => b.HeadofAccount_First)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchSecondName))
      {
        HeadofAccount_SecondsQuery = HeadofAccount_SecondsQuery.Where(b => b.HeadofAccount_SecondName.Contains(searchSecondName));
      }

      var HeadofAccount_Seconds = await HeadofAccount_SecondsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchSecondName) && HeadofAccount_Seconds.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No HeadofAccount_Second Name found with the name '" + searchSecondName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Second/HeadofAccount_Second.cshtml", HeadofAccount_Seconds);
    }


    public async Task<IActionResult> HeadofAccount_Second()
    {
      var HeadofAccount_Seconds = await _appDBContext.Settings_HeadofAccount_Seconds.ToListAsync();
      return Ok(HeadofAccount_Seconds);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.FirstList = await _utils.GetHeadofAccount_First();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_Second = await _appDBContext.Settings_HeadofAccount_Seconds.FindAsync(id);
      if (HeadofAccount_Second == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Second/EditHeadofAccount_Second.cshtml", HeadofAccount_Second);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_Second HeadofAccount_Second)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Second.HeadofAccount_SecondName))
        {
          return Json(new { success = false, message = "HeadofAccount_Second Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_Second);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Second Name updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HeadofAccount_Second Name. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.FirstList = await _utils.GetHeadofAccount_First();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Second/AddHeadofAccount_Second.cshtml", new Settings_HeadofAccount_Second());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_Second HeadofAccount_Second)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Second.HeadofAccount_SecondName))
        {
          return Json(new { success = false, message = "HeadofAccount_Second Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_Second.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_Seconds.Add(HeadofAccount_Second);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Second Name created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating HeadofAccount_Second Name. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_Second = await _appDBContext.Settings_HeadofAccount_Seconds.FindAsync(id);
      if (HeadofAccount_Second == null)
      {
        return NotFound();
      }

      HeadofAccount_Second.ActiveYNID = 2;
      HeadofAccount_Second.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_Seconds.Update(HeadofAccount_Second);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Second Name deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_Seconds = await _appDBContext.Settings_HeadofAccount_Seconds
          .Include(b => b.HeadofAccount_First)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HeadofAccount_Seconds");
        worksheet.Cells["A1"].Value = "HeadofAccount_Second ID";
        worksheet.Cells["B1"].Value = "HeadofAccount_Second Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < HeadofAccount_Seconds.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_Seconds[i].HeadofAccount_SecondID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_Seconds[i].HeadofAccount_SecondName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_Seconds[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HeadofAccount_Seconds-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_Seconds = await _appDBContext.Settings_HeadofAccount_Seconds
          .Include(b => b.HeadofAccount_First)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Second/PrintHeadofAccount_Second.cshtml", HeadofAccount_Seconds);
    }


  }
}
