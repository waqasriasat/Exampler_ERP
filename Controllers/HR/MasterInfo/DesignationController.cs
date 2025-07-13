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
  public class DesignationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<DesignationController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public DesignationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<DesignationController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchDesignationName)
    {

      var DesignationsQuery = _appDBContext.Settings_DesignationTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchDesignationName))
      {
        DesignationsQuery = DesignationsQuery.Where(b => b.DesignationTypeName.Contains(searchDesignationName));
      }

      var Designations = await DesignationsQuery.ToListAsync();


      if (!string.IsNullOrEmpty(searchDesignationName) && Designations.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Designation found with the name '" + searchDesignationName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/Designation/Designation.cshtml", Designations);
    }

    public async Task<IActionResult> Designation()
    {
      var Designations = await _appDBContext.Settings_DesignationTypes.ToListAsync();
      return Ok(Designations);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Designation = await _appDBContext.Settings_DesignationTypes.FindAsync(id);
      if (Designation == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Designation/EditDesignation.cshtml", Designation);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_DesignationType Designation)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Designation.DesignationTypeName))
        {
          return Json(new { success = false, message = "Designation Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(Designation);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Designation Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Designation. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Designation/AddDesignation.cshtml", new Settings_DesignationType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_DesignationType Designation)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Designation.DesignationTypeName))
        {
          return Json(new { success = false, message = "Designation Name field is required. Please enter a valid text value." });
        }
        Designation.DeleteYNID = 0;
        _appDBContext.Settings_DesignationTypes.Add(Designation);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Designation Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Designation. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Designation = await _appDBContext.Settings_DesignationTypes.FindAsync(id);
      if (Designation == null)
      {
        return NotFound();
      }

      Designation.ActiveYNID = 2;
      Designation.DeleteYNID = 1;

      _appDBContext.Settings_DesignationTypes.Update(Designation);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Designation Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Designations = await _appDBContext.Settings_DesignationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Designations");
        worksheet.Cells["A1"].Value = "Designation ID";
        worksheet.Cells["B1"].Value = "Designation Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < Designations.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Designations[i].DesignationTypeID;
          worksheet.Cells[i + 2, 2].Value = Designations[i].DesignationTypeName;
          worksheet.Cells[i + 2, 3].Value = Designations[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Designations-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Designations = await _appDBContext.Settings_DesignationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/Designation/PrintDesignations.cshtml", Designations);
    }

  }
}
