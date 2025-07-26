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
  public class AdditionalAllowanceTypeController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<AdditionalAllowanceTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public AdditionalAllowanceTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<AdditionalAllowanceTypeController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchAdditionalAllowanceTypeName)
    {

      var AdditionalAllowanceTypesQuery = _appDBContext.Settings_AdditionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchAdditionalAllowanceTypeName))
      {
        AdditionalAllowanceTypesQuery = AdditionalAllowanceTypesQuery.Where(b => b.AdditionalAllowanceTypeName.Contains(searchAdditionalAllowanceTypeName));
      }

      var AdditionalAllowanceTypes = await AdditionalAllowanceTypesQuery.ToListAsync();


      if (!string.IsNullOrEmpty(searchAdditionalAllowanceTypeName) && AdditionalAllowanceTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No AllowanceType found with the name '" + searchAdditionalAllowanceTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/AdditionalAllowanceType/AdditionalAllowanceType.cshtml", AdditionalAllowanceTypes);
    }

    public async Task<IActionResult> AdditionalAllowanceType()
    {
      var AdditionalAllowanceTypes = await _appDBContext.Settings_AdditionalAllowanceTypes.ToListAsync();
      return Ok(AdditionalAllowanceTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var AdditionalAllowanceType = await _appDBContext.Settings_AdditionalAllowanceTypes.FindAsync(id);
      if (AdditionalAllowanceType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/AdditionalAllowanceType/EditAdditionalAllowanceType.cshtml", AdditionalAllowanceType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_AdditionalAllowanceType AdditionalAllowanceType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(AdditionalAllowanceType.AdditionalAllowanceTypeName))
        {
          return Json(new { success = false, message = "AdditionalAllowanceType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(AdditionalAllowanceType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating AdditionalAllowanceType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/AdditionalAllowanceType/AddAdditionalAllowanceType.cshtml", new Settings_AdditionalAllowanceType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_AdditionalAllowanceType AdditionalAllowanceType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(AdditionalAllowanceType.AdditionalAllowanceTypeName))
        {
          return Json(new { success = false, message = "AdditionalAllowanceType Name field is required. Please enter a valid text value." });
        }
        AdditionalAllowanceType.DeleteYNID = 0;
        _appDBContext.Settings_AdditionalAllowanceTypes.Add(AdditionalAllowanceType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating AdditionalAllowanceType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var AdditionalAllowanceType = await _appDBContext.Settings_AdditionalAllowanceTypes.FindAsync(id);
      if (AdditionalAllowanceType == null)
      {
        return NotFound();
      }

      AdditionalAllowanceType.ActiveYNID = 2;
      AdditionalAllowanceType.DeleteYNID = 1;

      _appDBContext.Settings_AdditionalAllowanceTypes.Update(AdditionalAllowanceType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Additional Allowance Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var AdditionalAllowanceTypees = await _appDBContext.Settings_AdditionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_AdditionalAllowanceType"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_AdditionalAllowanceTypeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_AdditionalAllowanceTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < AdditionalAllowanceTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = AdditionalAllowanceTypees[i].AdditionalAllowanceTypeID;
          worksheet.Cells[i + 2, 2].Value = AdditionalAllowanceTypees[i].AdditionalAllowanceTypeName;
          worksheet.Cells[i + 2, 3].Value = AdditionalAllowanceTypees[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_AdditionalAllowanceType"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var AdditionalAllowanceTypees = await _appDBContext.Settings_AdditionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/AdditionalAllowanceType/PrintAdditionalAllowanceType.cshtml", AdditionalAllowanceTypees);
    }

  }
}
