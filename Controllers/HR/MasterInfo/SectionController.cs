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
    public async Task<IActionResult> Index()
    {
      var Sections = await _appDBContext.Settings_Sections
    .Where(b => b.DeleteYNID != 1)
    .Include(d => d.Department) // Eagerly load the related Department data
    .ToListAsync();

      return View("~/Views/HR/MasterInfo/Section/Section.cshtml", Sections);

    }
    public async Task<IActionResult> Section()
    {
      var Sections = await _appDBContext.Settings_Sections.ToListAsync();
      return Ok(Sections);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.DepartmentList = await _utils.GetDepartments();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var Section = await _appDBContext.Settings_Sections.FindAsync(id);
      if (Section == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/Section/EditSection.cshtml", Section);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_Section Section)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(Section);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Section/EditSection.cshtml", Section);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.DepartmentList = await _utils.GetDepartments();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/Section/AddSection.cshtml", new Settings_Section());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_Section Section)
    {
      if (ModelState.IsValid)
      {
        Section.DeleteYNID = 0;
        _appDBContext.Settings_Sections.Add(Section);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Section/AddSection.cshtml", Section);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Section = await _appDBContext.Settings_Sections.FindAsync(id);
      if (Section == null)
      {
        return NotFound();
      }

      Section.ActiveYNID = 0;
      Section.DeleteYNID = 1;

      _appDBContext.Settings_Sections.Update(Section);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Sections = await _appDBContext.Settings_Sections
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Department) // Eagerly load the related Department data
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
          worksheet.Cells[i + 2, 1].Value = Sections[i].SectionID;
          worksheet.Cells[i + 2, 2].Value = Sections[i].Department?.DepartmentName;
          worksheet.Cells[i + 2, 3].Value = Sections[i].SectionName;
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
      var Sections = await _appDBContext.Settings_Sections
        .Where(b => b.DeleteYNID != 1)
        .Include(d => d.Department) // Eagerly load the related Department data
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/Section/PrintSections.cshtml", Sections);
    }

  }
}
