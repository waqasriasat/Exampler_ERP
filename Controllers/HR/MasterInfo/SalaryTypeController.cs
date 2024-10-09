using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
   public class SalaryTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public SalaryTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
        _appDBContext.Update(SalaryType);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/SalaryType/EditSalaryType.cshtml", SalaryType);
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
        SalaryType.DeleteYNID = 0;
        _appDBContext.Settings_SalaryTypes.Add(SalaryType);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return PartialView("~/Views/HR/MasterInfo/SalaryType/AddSalaryType.cshtml", SalaryType);
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
        var worksheet = package.Workbook.Worksheets.Add("SalaryTypees");
        worksheet.Cells["A1"].Value = "SalaryType ID";
        worksheet.Cells["B1"].Value = "SalaryType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < SalaryTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = SalaryTypees[i].SalaryTypeID;
          worksheet.Cells[i + 2, 2].Value = SalaryTypees[i].SalaryTypeName;
          worksheet.Cells[i + 2, 3].Value = SalaryTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"SalaryTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
