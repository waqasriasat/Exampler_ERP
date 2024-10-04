using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
   public class DesignationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public DesignationController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {

      var Designationes = await _appDBContext.Settings_DesignationTypes
        .Where(b => b.DeleteYNID != 1)
        .ToListAsync();
      return View("~/Views/HR/MasterInfo/Designation/Designation.cshtml", Designationes);
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
        _appDBContext.Update(Designation);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Designation/EditDesignation.cshtml", Designation);
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
        Designation.DeleteYNID = 0;
        _appDBContext.Settings_DesignationTypes.Add(Designation);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/Designation/AddDesignation.cshtml", Designation);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Designation = await _appDBContext.Settings_DesignationTypes.FindAsync(id);
      if (Designation == null)
      {
        return NotFound();
      }

      Designation.ActiveYNID = 0;
      Designation.DeleteYNID = 1;

      _appDBContext.Settings_DesignationTypes.Update(Designation);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Designationes = await _appDBContext.Settings_DesignationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Designationes");
        worksheet.Cells["A1"].Value = "Designation ID";
        worksheet.Cells["B1"].Value = "Designation Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < Designationes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Designationes[i].DesignationTypeID;
          worksheet.Cells[i + 2, 2].Value = Designationes[i].DesignationTypeName;
          worksheet.Cells[i + 2, 3].Value = Designationes[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Designationes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Designationes = await _appDBContext.Settings_DesignationTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/Designation/PrintDesignations.cshtml", Designationes);
    }

  }
}
