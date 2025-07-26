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
  public class HeadofAccount_FirstController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HeadofAccount_FirstController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HeadofAccount_FirstController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HeadofAccount_FirstController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchFirstName)
    {
      var HeadofAccount_FirstsQuery = _appDBContext.Settings_HeadofAccount_Firsts
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchFirstName))
      {
        HeadofAccount_FirstsQuery = HeadofAccount_FirstsQuery.Where(b => b.HeadofAccount_FirstName.Contains(searchFirstName));
      }

      var HeadofAccount_Firsts = await HeadofAccount_FirstsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchFirstName) && HeadofAccount_Firsts.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No HeadofAccount_First Name found with the name '" + searchFirstName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_First/HeadofAccount_First.cshtml", HeadofAccount_Firsts);
    }


    public async Task<IActionResult> HeadofAccount_First()
    {
      var HeadofAccount_Firsts = await _appDBContext.Settings_HeadofAccount_Firsts.ToListAsync();
      return Ok(HeadofAccount_Firsts);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_First = await _appDBContext.Settings_HeadofAccount_Firsts.FindAsync(id);
      if (HeadofAccount_First == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_First/EditHeadofAccount_First.cshtml", HeadofAccount_First);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_First HeadofAccount_First)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_First.HeadofAccount_FirstName))
        {
          return Json(new { success = false, message = "HeadofAccount_First Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_First);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_First Name updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating HeadofAccount_First Name. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_First/AddHeadofAccount_First.cshtml", new Settings_HeadofAccount_First());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_First HeadofAccount_First)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_First.HeadofAccount_FirstName))
        {
          return Json(new { success = false, message = "HeadofAccount_First Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_First.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_Firsts.Add(HeadofAccount_First);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_First Name created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating HeadofAccount_First Name. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_First = await _appDBContext.Settings_HeadofAccount_Firsts.FindAsync(id);
      if (HeadofAccount_First == null)
      {
        return NotFound();
      }

      HeadofAccount_First.ActiveYNID = 2;
      HeadofAccount_First.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_Firsts.Update(HeadofAccount_First);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "HeadofAccount_First Name deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_Firsts = await _appDBContext.Settings_HeadofAccount_Firsts
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_HeadofAccountFirst"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_HeadofAccountFirstID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_HeadofAccountFirstName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < HeadofAccount_Firsts.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_Firsts[i].HeadofAccount_FirstID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_Firsts[i].HeadofAccount_FirstName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_Firsts[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_HeadofAccountFirst"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_Firsts = await _appDBContext.Settings_HeadofAccount_Firsts
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_First/PrintHeadofAccount_First.cshtml", HeadofAccount_Firsts);
    }


  }
}
