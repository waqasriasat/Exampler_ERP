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
  public class QualificationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<QualificationController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public QualificationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<QualificationController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchQualificationName)
    {

      var QualificationesQuery = _appDBContext.Settings_QualificationTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchQualificationName))
      {
        QualificationesQuery = QualificationesQuery.Where(b => b.QualificationTypeName.Contains(searchQualificationName));
      }

      var Qualificationes = await QualificationesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchQualificationName) && Qualificationes.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Qualification found with the name '" + searchQualificationName + "'. Please check the name and try again.");
      }

      return View("~/Views/HR/MasterInfo/Qualification/Qualification.cshtml", Qualificationes);
    }

    public async Task<IActionResult> Qualification()
    {
      var Qualifications = await _appDBContext.Settings_QualificationTypes.ToListAsync();
      return Ok(Qualifications);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Qualification = await _appDBContext.Settings_QualificationTypes.FindAsync(id);
      if (Qualification == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Qualification/EditQualification.cshtml", Qualification);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_QualificationType Qualification)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Qualification.QualificationTypeName))
        {
          return Json(new { success = false, message = "Qualification Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(Qualification);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Qualification Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Qualification. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Qualification/AddQualification.cshtml", new Settings_QualificationType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_QualificationType Qualification)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Qualification.QualificationTypeName))
        {
          return Json(new { success = false, message = "Qualification Name field is required. Please enter a valid text value." });
        }
        Qualification.DeleteYNID = 0;
        _appDBContext.Settings_QualificationTypes.Add(Qualification);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Qualification Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Qualification. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Qualification = await _appDBContext.Settings_QualificationTypes.FindAsync(id);
      if (Qualification == null)
      {
        return NotFound();
      }

      Qualification.ActiveYNID = 2;
      Qualification.DeleteYNID = 1;

      _appDBContext.Settings_QualificationTypes.Update(Qualification);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Qualification Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Qualificationes = await _appDBContext.Settings_QualificationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_Qualification"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_QualificationID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_QualificationName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Active"];


        for (int i = 0; i < Qualificationes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Qualificationes[i].QualificationTypeID;
          worksheet.Cells[i + 2, 2].Value = Qualificationes[i].QualificationTypeName;
          worksheet.Cells[i + 2, 3].Value = Qualificationes[i].ActiveYNID == 1 ? _localizer["lbl_Yes"] : _localizer["lbl_No"];
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_Qualification"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Qualificationes = await _appDBContext.Settings_QualificationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/Qualification/PrintQualifications.cshtml", Qualificationes);
    }

  }
}
