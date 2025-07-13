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
  public class DeductionTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<DeductionTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public DeductionTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<DeductionTypeController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchDeductionTypeName)
    {

      var DeductionTypesQuery = _appDBContext.Settings_DeductionTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchDeductionTypeName))
      {
        DeductionTypesQuery = DeductionTypesQuery.Where(b => b.DeductionTypeName.Contains(searchDeductionTypeName));
      }

      var DeductionTypes = await DeductionTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchDeductionTypeName) && DeductionTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No DeductionType found with the name '" + searchDeductionTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/DeductionType/DeductionType.cshtml", DeductionTypes);
    }

    public async Task<IActionResult> DeductionType()
    {
      var DeductionTypes = await _appDBContext.Settings_DeductionTypes.ToListAsync();
      return Ok(DeductionTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var DeductionType = await _appDBContext.Settings_DeductionTypes.FindAsync(id);
      if (DeductionType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/DeductionType/EditDeductionType.cshtml", DeductionType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_DeductionType DeductionType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(DeductionType.DeductionTypeName))
        {
          return Json(new { success = false, message = "DeductionType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(DeductionType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Deduction Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating DeductionType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/DeductionType/AddDeductionType.cshtml", new Settings_DeductionType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_DeductionType DeductionType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(DeductionType.DeductionTypeName))
        {
          return Json(new { success = false, message = "DeductionType Name field is required. Please enter a valid text value." });
        }
        DeductionType.DeleteYNID = 0;
        _appDBContext.Settings_DeductionTypes.Add(DeductionType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Deduction Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating DeductionType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var DeductionType = await _appDBContext.Settings_DeductionTypes.FindAsync(id);
      if (DeductionType == null)
      {
        return NotFound();
      }

      DeductionType.ActiveYNID = 2;
      DeductionType.DeleteYNID = 1;

      _appDBContext.Settings_DeductionTypes.Update(DeductionType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Deduction Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var DeductionTypees = await _appDBContext.Settings_DeductionTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("DeductionTypees");
        worksheet.Cells["A1"].Value = "DeductionType ID";
        worksheet.Cells["B1"].Value = "DeductionType Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < DeductionTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = DeductionTypees[i].DeductionTypeID;
          worksheet.Cells[i + 2, 2].Value = DeductionTypees[i].DeductionTypeName;
          worksheet.Cells[i + 2, 3].Value = DeductionTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"DeductionTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var DeductionTypees = await _appDBContext.Settings_DeductionTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/DeductionType/PrintDeductionTypes.cshtml", DeductionTypees);
    }


  }
}
