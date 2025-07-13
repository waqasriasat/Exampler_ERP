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
  public class HeadofAccount_FiveController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HeadofAccount_FiveController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HeadofAccount_FiveController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HeadofAccount_FiveController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchFiveName)
    {
      var HeadofAccount_FivesQuery = _appDBContext.Settings_HeadofAccount_Fives
          .Include(b => b.HeadofAccount_Four)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchFiveName))
      {
        HeadofAccount_FivesQuery = HeadofAccount_FivesQuery.Where(b => b.HeadofAccount_FiveName.Contains(searchFiveName));
      }

      var HeadofAccount_Fives = await HeadofAccount_FivesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchFiveName) && HeadofAccount_Fives.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No HeadofAccount_Five Name found with the name '" + searchFiveName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Five/HeadofAccount_Five.cshtml", HeadofAccount_Fives);
    }


    public async Task<IActionResult> HeadofAccount_Five()
    {
      var HeadofAccount_Fives = await _appDBContext.Settings_HeadofAccount_Fives.ToListAsync();
      return Ok(HeadofAccount_Fives);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.GroupList = await _utils.GetHeadofAccount_GroupType();
      ViewBag.categoryTypeList = await _utils.GetHeadofAccount_CategoryType();
      ViewBag.FourList = await _utils.GetHeadofAccount_Four();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_Five = await _appDBContext.Settings_HeadofAccount_Fives.FindAsync(id);
      if (HeadofAccount_Five == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Five/EditHeadofAccount_Five.cshtml", HeadofAccount_Five);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_Five HeadofAccount_Five)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Five.HeadofAccount_FiveName))
        {
          return Json(new { success = false, message = "HeadofAccount_Five Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_Five);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Five Name updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HeadofAccount_Five Name. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.GroupList = await _utils.GetHeadofAccount_GroupType();
      ViewBag.categoryTypeList = await _utils.GetHeadofAccount_CategoryType();
      ViewBag.FourList = await _utils.GetHeadofAccount_Four();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_Five/AddHeadofAccount_Five.cshtml", new Settings_HeadofAccount_Five());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_Five HeadofAccount_Five)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_Five.HeadofAccount_FiveName))
        {
          return Json(new { success = false, message = "HeadofAccount_Five Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_Five.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_Fives.Add(HeadofAccount_Five);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Five Name created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating HeadofAccount_Five Name. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_Five = await _appDBContext.Settings_HeadofAccount_Fives.FindAsync(id);
      if (HeadofAccount_Five == null)
      {
        return NotFound();
      }

      HeadofAccount_Five.ActiveYNID = 2;
      HeadofAccount_Five.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_Fives.Update(HeadofAccount_Five);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_Five Name deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_Fives = await _appDBContext.Settings_HeadofAccount_Fives
          .Include(b => b.HeadofAccount_Four)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HeadofAccount_Fives");
        worksheet.Cells["A1"].Value = "HeadofAccount_Five ID";
        worksheet.Cells["B1"].Value = "HeadofAccount_Five Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < HeadofAccount_Fives.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_Fives[i].HeadofAccount_FiveID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_Fives[i].HeadofAccount_FiveName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_Fives[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HeadofAccount_Fives-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_Fives = await _appDBContext.Settings_HeadofAccount_Fives
          .Include(b => b.HeadofAccount_Four)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_Five/PrintHeadofAccount_Five.cshtml", HeadofAccount_Fives);
    }


  }
}
