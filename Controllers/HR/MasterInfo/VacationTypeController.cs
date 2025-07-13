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
  public class VacationTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<VacationTypeController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public VacationTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<VacationTypeController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchVacationTypeName)
    {

      var VacationTypesQuery = _appDBContext.Settings_VacationTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchVacationTypeName))
      {
        VacationTypesQuery = VacationTypesQuery.Where(b => b.VacationTypeName.Contains(searchVacationTypeName));
      }

      var VacationTypes = await VacationTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchVacationTypeName) && VacationTypes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Vacation Type found with the name '" + searchVacationTypeName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/VacationType/VacationType.cshtml", VacationTypes);
    }
    public async Task<IActionResult> VacationType()
    {
      var VacationTypes = await _appDBContext.Settings_VacationTypes.ToListAsync();
      return Ok(VacationTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var VacationType = await _appDBContext.Settings_VacationTypes.FindAsync(id);
      if (VacationType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/VacationType/EditVacationType.cshtml", VacationType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_VacationType VacationType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VacationType.VacationTypeName))
        {
          return Json(new { success = false, message = "VacationType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(VacationType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Type Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating VacationType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/VacationType/AddVacationType.cshtml", new Settings_VacationType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_VacationType VacationType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VacationType.VacationTypeName))
        {
          return Json(new { success = false, message = "VacationType Name field is required. Please enter a valid text value." });
        }
        VacationType.DeleteYNID = 0;
        _appDBContext.Settings_VacationTypes.Add(VacationType);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Type Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating VacationType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var VacationType = await _appDBContext.Settings_VacationTypes.FindAsync(id);
      if (VacationType == null)
      {
        return NotFound();
      }

      VacationType.ActiveYNID = 2;
      VacationType.DeleteYNID = 1;

      _appDBContext.Settings_VacationTypes.Update(VacationType);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Vacation Type Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var VacationTypees = await _appDBContext.Settings_VacationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("VacationTypees");
        worksheet.Cells["A1"].Value = "VacationType ID";
        worksheet.Cells["B1"].Value = "VacationType Name";
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < VacationTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = VacationTypees[i].VacationTypeID;
          worksheet.Cells[i + 2, 2].Value = VacationTypees[i].VacationTypeName;
          worksheet.Cells[i + 2, 3].Value = VacationTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"VacationTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var VacationTypees = await _appDBContext.Settings_VacationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/VacationType/PrintVacationTypes.cshtml", VacationTypees);
    }

  }
}
