using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class SectionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public SectionController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchSectionName)
    {
      var SectionsQuery = _appDBContext.Settings_SectionTypes
          .Where(d => d.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchSectionName))
      {
        SectionsQuery = SectionsQuery.Where(d => d.SectionTypeName.Contains(searchSectionName));
      }

      var Sections = await SectionsQuery
          .Include(d => d.DepartmentType).ToListAsync();

      if (!string.IsNullOrEmpty(searchSectionName) && Sections.Count == 0)
      {
        TempData["ErrorMessage"] = "No Section found with the name '" + searchSectionName + "'. Please check the name and try again.";
      }

      return View("~/Views/HR/MasterInfo/Section/Section.cshtml", Sections);
    }
  
    public async Task<IActionResult> Section()
    {
      var Sections = await _appDBContext.Settings_SectionTypes.ToListAsync();
      return Ok(Sections);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.DepartmentList = await _utils.GetDepartments();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Section = await _appDBContext.Settings_SectionTypes.FindAsync(id);
      if (Section == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Section/EditSection.cshtml", Section);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_SectionType Section)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(Section);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Section Updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error Updating Section. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/Section/EditSection.cshtml", Section);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.DepartmentList = await _utils.GetDepartments();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Section/AddSection.cshtml", new Settings_SectionType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_SectionType Section)
    {
      if (ModelState.IsValid)
      {
        Section.DeleteYNID = 0;
        _appDBContext.Settings_SectionTypes.Add(Section);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Section Created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Section. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/Section/AddSection.cshtml", Section);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Section = await _appDBContext.Settings_SectionTypes.FindAsync(id);
      if (Section == null)
      {
        return NotFound();
      }

      Section.ActiveYNID = 2;
      Section.DeleteYNID = 1;

      _appDBContext.Settings_SectionTypes.Update(Section);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Section Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Sections = await _appDBContext.Settings_SectionTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.DepartmentType) // Eagerly load the related Department data
        .ToListAsync();


      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Sections");
        worksheet.Cells["A1"].Value = "Section ID";
        worksheet.Cells["B1"].Value = "Department Name";
        worksheet.Cells["C1"].Value = "Section Name";
        worksheet.Cells["D1"].Value = "Active";


        for (int i = 0; i < Sections.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Sections[i].SectionTypeID;
          worksheet.Cells[i + 2, 2].Value = Sections[i].DepartmentType?.DepartmentTypeName;
          worksheet.Cells[i + 2, 3].Value = Sections[i].SectionTypeName;
          worksheet.Cells[i + 2, 4].Value = Sections[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Sections-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Sections = await _appDBContext.Settings_SectionTypes
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.DepartmentType) // Eagerly load the related Department data
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/Section/PrintSections.cshtml", Sections);
    }

  }
}
