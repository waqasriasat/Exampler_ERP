using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class QualificationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public QualificationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
        _appDBContext.Update(Qualification);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Qualification Updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error Updating Qualification. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/Qualification/EditQualification.cshtml", Qualification);
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
        Qualification.DeleteYNID = 0;
        _appDBContext.Settings_QualificationTypes.Add(Qualification);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Qualification Created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Qualification. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/Qualification/AddQualification.cshtml", Qualification);
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
      TempData["SuccessMessage"] = "Qualification Deleted successfully.";
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
        var worksheet = package.Workbook.Worksheets.Add("Qualificationes");
        worksheet.Cells["A1"].Value = "Qualification ID";
        worksheet.Cells["B1"].Value = "Qualification Name";
        worksheet.Cells["C1"].Value = "Active";
      

        for (int i = 0; i < Qualificationes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Qualificationes[i].QualificationTypeID;
          worksheet.Cells[i + 2, 2].Value = Qualificationes[i].QualificationTypeName;
          worksheet.Cells[i + 2, 3].Value = Qualificationes[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Qualificationes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
