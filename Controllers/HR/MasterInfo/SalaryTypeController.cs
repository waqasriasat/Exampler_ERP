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
  public class SalaryTypeController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<SalaryTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public SalaryTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<SalaryTypeController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchSalaryTypeName)
    {

      var SalaryTypesQuery = _appDBContext.Settings_SalaryTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchSalaryTypeName))
      {
        SalaryTypesQuery = SalaryTypesQuery.Where(b => b.SalaryTypeName.Contains(searchSalaryTypeName));
      }

      var SalaryTypes = await SalaryTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchSalaryTypeName) && SalaryTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Salary Type found with the name '" + searchSalaryTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/SalaryType/SalaryType.cshtml", SalaryTypes);
    }
    public async Task<IActionResult> SalaryType()
    {
      var SalaryTypes = await _appDBContext.Settings_SalaryTypes.ToListAsync();
      return Ok(SalaryTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var SalaryType = await _appDBContext.Settings_SalaryTypes.FindAsync(id);
      if (SalaryType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/SalaryType/EditSalaryType.cshtml", SalaryType);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(Settings_SalaryType SalaryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(SalaryType.SalaryTypeName))
        {
          return Json(new { success = false, message = "SalaryType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(SalaryType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "SalaryType Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating SalaryType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/SalaryType/AddSalaryType.cshtml", new Settings_SalaryType());
    }
    [HttpPost]
    public async Task<IActionResult> Create(Settings_SalaryType SalaryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(SalaryType.SalaryTypeName))
        {
          return Json(new { success = false, message = "SalaryType Name field is required. Please enter a valid text value." });
        }
        SalaryType.DeleteYNID = 0;
        _appDBContext.Settings_SalaryTypes.Add(SalaryType);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating SalaryType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var SalaryType = await _appDBContext.Settings_SalaryTypes.FindAsync(id);
      if (SalaryType == null)
      {
        return NotFound();
      }

      SalaryType.ActiveYNID = 2;
      SalaryType.DeleteYNID = 1;

      _appDBContext.Settings_SalaryTypes.Update(SalaryType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Salary Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var SalaryTypees = await _appDBContext.Settings_SalaryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_SalaryType"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_SalaryTypeID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_SalaryTypeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < SalaryTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = SalaryTypees[i].SalaryTypeID;
          worksheet.Cells[i + 2, 2].Value = SalaryTypees[i].SalaryTypeName;
          worksheet.Cells[i + 2, 3].Value = SalaryTypees[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_SalaryType"] +$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var SalaryTypees = await _appDBContext.Settings_SalaryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/SalaryType/PrintSalaryTypes.cshtml", SalaryTypees);
    }
  }
}
