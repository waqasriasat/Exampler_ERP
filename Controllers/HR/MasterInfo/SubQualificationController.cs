using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
    public class SubQualificationController : Controller
    {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public SubQualificationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var SubQualification = await _appDBContext.Settings_SubQualifications
    .Where(b => b.DeleteYNID != 1)
    .Include(d => d.Qualification) // Eagerly load the related Qualification data
    .ToListAsync();

      return View("~/Views/HR/MasterInfo/SubQualification/SubQualification.cshtml", SubQualification);

    }
    public async Task<IActionResult> SubQualification()
    {
      var SubQualification = await _appDBContext.Settings_SubQualifications.ToListAsync();
      return Ok(SubQualification);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.QualificationList = await _utils.GetQualifications();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var SubQualification = await _appDBContext.Settings_SubQualifications.FindAsync(id);
      if (SubQualification == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/SubQualification/EditSubQualification.cshtml", SubQualification);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_SubQualification SubQualification)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(SubQualification);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/SubQualification/EditSubQualification.cshtml", SubQualification);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.QualificationList = await _utils.GetQualifications();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/SubQualification/AddSubQualification.cshtml", new Settings_SubQualification());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_SubQualification SubQualification)
    {
      if (ModelState.IsValid)
      {
        SubQualification.DeleteYNID = 0;
        _appDBContext.Settings_SubQualifications.Add(SubQualification);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/SubQualification/AddSubQualification.cshtml", SubQualification);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var SubQualification = await _appDBContext.Settings_SubQualifications.FindAsync(id);
      if (SubQualification == null)
      {
        return NotFound();
      }

      SubQualification.ActiveYNID = 0;
      SubQualification.DeleteYNID = 1;

      _appDBContext.Settings_SubQualifications.Update(SubQualification);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var SubQualification = await _appDBContext.Settings_SubQualifications
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Qualification) // Eagerly load the related Qualification data
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("SubQualification");
        worksheet.Cells["A1"].Value = "SubQualification ID";
        worksheet.Cells["B1"].Value = "Qualification Name";
        worksheet.Cells["C1"].Value = "SubQualification Name";
        worksheet.Cells["D1"].Value = "Active";


        for (int i = 0; i < SubQualification.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = SubQualification[i].SubQualificationID;
          worksheet.Cells[i + 2, 2].Value = SubQualification[i].Qualification?.QualificationName;
          worksheet.Cells[i + 2, 3].Value = SubQualification[i].SubQualificationName;
          worksheet.Cells[i + 2, 4].Value = SubQualification[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"SubQualification-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var SubQualification = await _appDBContext.Settings_SubQualifications
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Qualification) // Eagerly load the related Qualification data
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/SubQualification/PrintSubQualifications.cshtml", SubQualification);
    }

  }
}
