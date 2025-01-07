using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Warehouse.MasterInfo
{
  public class UnitTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public UnitTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchUnitTypeName)
    {

      var UnitTypesQuery = _appDBContext.Settings_UnitTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchUnitTypeName))
      {
        UnitTypesQuery = UnitTypesQuery.Where(b => b.UnitTypeName.Contains(searchUnitTypeName));
      }

      var UnitTypes = await UnitTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchUnitTypeName) && UnitTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No Unit Type found with the name '" + searchUnitTypeName + "'. Please check the name and try again.";
      }

      return View("~/Views/Warehouse/MasterInfo/UnitType/UnitType.cshtml", UnitTypes);
    }
    public async Task<IActionResult> UnitType()
    {
      var UnitTypes = await _appDBContext.Settings_UnitTypes.ToListAsync();
      return Ok(UnitTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var UnitType = await _appDBContext.Settings_UnitTypes.FindAsync(id);
      if (UnitType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Warehouse/MasterInfo/UnitType/EditUnitType.cshtml", UnitType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_UnitType UnitType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(UnitType.UnitTypeName))
        {
          return Json(new { success = false, message = "UnitType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(UnitType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Unit Type Updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating UnitType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Warehouse/MasterInfo/UnitType/AddUnitType.cshtml", new Settings_UnitType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_UnitType UnitType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(UnitType.UnitTypeName))
        {
          return Json(new { success = false, message = "UnitType Name field is required. Please enter a valid text value." });
        }
        UnitType.DeleteYNID = 0;
        _appDBContext.Settings_UnitTypes.Add(UnitType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Unit Type Created successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating UnitType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var UnitType = await _appDBContext.Settings_UnitTypes.FindAsync(id);
      if (UnitType == null)
      {
        return NotFound();
      }

      UnitType.ActiveYNID = 2;
      UnitType.DeleteYNID = 1;

      _appDBContext.Settings_UnitTypes.Update(UnitType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Unit Type Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var UnitTypees = await _appDBContext.Settings_UnitTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("UnitTypees");
        worksheet.Cells["A1"].Value = "UnitType ID";
        worksheet.Cells["B1"].Value = "UnitType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < UnitTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = UnitTypees[i].UnitTypeID;
          worksheet.Cells[i + 2, 2].Value = UnitTypees[i].UnitTypeName;
          worksheet.Cells[i + 2, 3].Value = UnitTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"UnitTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var UnitTypees = await _appDBContext.Settings_UnitTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Warehouse/MasterInfo/UnitType/PrintUnitType.cshtml", UnitTypees);
    }

  }
}
