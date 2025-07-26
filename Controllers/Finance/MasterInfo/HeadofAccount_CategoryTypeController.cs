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
  public class HeadofAccount_CategoryTypeController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HeadofAccount_CategoryTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public HeadofAccount_CategoryTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HeadofAccount_CategoryTypeController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchCategoryTypeName)
    {
      var HeadofAccount_CategoryTypesQuery = _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchCategoryTypeName))
      {
        HeadofAccount_CategoryTypesQuery = HeadofAccount_CategoryTypesQuery.Where(b => b.CategoryTypeName.Contains(searchCategoryTypeName));
      }

      var HeadofAccount_CategoryTypes = await HeadofAccount_CategoryTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchCategoryTypeName) && HeadofAccount_CategoryTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Category Type found with the name '" + searchCategoryTypeName + "'. Please check the name and try again.");
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/HeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryTypes);
    }


    public async Task<IActionResult> HeadofAccount_CategoryType()
    {
      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes.ToListAsync();
      return Ok(HeadofAccount_CategoryTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_CategoryType = await _appDBContext.Settings_HeadofAccount_CategoryTypes.FindAsync(id);
      if (HeadofAccount_CategoryType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/EditHeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_CategoryType HeadofAccount_CategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_CategoryType.CategoryTypeName))
        {
          return Json(new { success = false, message = "Category Type Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_CategoryType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Category Type updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Category Type. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/AddHeadofAccount_CategoryType.cshtml", new Settings_HeadofAccount_CategoryType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_CategoryType HeadofAccount_CategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_CategoryType.CategoryTypeName))
        {
          return Json(new { success = false, message = "Category Type Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_CategoryType.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_CategoryTypes.Add(HeadofAccount_CategoryType);
        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Category Type created successfully.");
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating Category Type. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_CategoryType = await _appDBContext.Settings_HeadofAccount_CategoryTypes.FindAsync(id);
      if (HeadofAccount_CategoryType == null)
      {
        return NotFound();
      }

      HeadofAccount_CategoryType.ActiveYNID = 2;
      HeadofAccount_CategoryType.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_CategoryTypes.Update(HeadofAccount_CategoryType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Category Type deleted successfully.");

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_CategoryType"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_CategoryTypeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_CategoryTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < HeadofAccount_CategoryTypes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_CategoryTypes[i].CategoryTypeID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_CategoryTypes[i].CategoryTypeName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_CategoryTypes[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_CategoryType"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/PrintHeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryTypes);
    }


  }
}
