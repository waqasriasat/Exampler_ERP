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
  public class HeadofAccount_ThirdController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HeadofAccount_ThirdController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HeadofAccount_ThirdController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HeadofAccount_ThirdController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchThirdName)
    {
      var HeadofAccount_ThirdsQuery = _appDBContext.Settings_HeadofAccount_Thirds
          .Include(b => b.HeadofAccount_Second)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchThirdName))
      {
        HeadofAccount_ThirdsQuery = HeadofAccount_ThirdsQuery.Where(b => b.HeadofAccount_ThirdName.Contains(searchThirdName));
      }

      var HeadofAccount_Thirds = await HeadofAccount_ThirdsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchThirdName) && HeadofAccount_Thirds.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No HeadofAccount_Third Name found with the name '" + searchThirdName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Third/HeadofAccount_Third.cshtml", HeadofAccount_Thirds);
    }


    public async Task<IActionResult> HeadofAccount_Third()
    {
      var HeadofAccount_Thirds = await _appDBContext.Settings_HeadofAccount_Thirds.ToListAsync();
      return Ok(HeadofAccount_Thirds);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.SecondList = await _utils.GetHeadofAccount_Second();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_Third = await _appDBContext.Settings_HeadofAccount_Thirds.FindAsync(id);
      if (HeadofAccount_Third == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Third/EditHeadofAccount_Third.cshtml", HeadofAccount_Third);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_Third HeadofAccount_Third)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Third.HeadofAccount_ThirdName))
        {
          return Json(new { success = false, message = "HeadofAccount_Third Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_Third);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Third Name updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HeadofAccount_Third Name. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.SecondList = await _utils.GetHeadofAccount_Second();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Third/AddHeadofAccount_Third.cshtml", new Settings_HeadofAccount_Third());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_Third HeadofAccount_Third)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Third.HeadofAccount_ThirdName))
        {
          return Json(new { success = false, message = "HeadofAccount_Third Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_Third.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_Thirds.Add(HeadofAccount_Third);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Third Name created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating HeadofAccount_Third Name. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_Third = await _appDBContext.Settings_HeadofAccount_Thirds.FindAsync(id);
      if (HeadofAccount_Third == null)
      {
        return NotFound();
      }

      HeadofAccount_Third.ActiveYNID = 2;
      HeadofAccount_Third.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_Thirds.Update(HeadofAccount_Third);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Third Name deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_Thirds = await _appDBContext.Settings_HeadofAccount_Thirds
          .Include(b => b.HeadofAccount_Second)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HeadofAccount_Thirds");
        worksheet.Cells["A1"].Value = "HeadofAccount_Third ID";
        worksheet.Cells["B1"].Value = "HeadofAccount_Third Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < HeadofAccount_Thirds.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_Thirds[i].HeadofAccount_ThirdID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_Thirds[i].HeadofAccount_ThirdName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_Thirds[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HeadofAccount_Thirds-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_Thirds = await _appDBContext.Settings_HeadofAccount_Thirds
          .Include(b => b.HeadofAccount_Second)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Third/PrintHeadofAccount_Third.cshtml", HeadofAccount_Thirds);
    }


  }
}
