using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
   public class VacationTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public VacationTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
        _appDBContext.Update(VacationType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Vacation Type Updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error Updating Vacation Type. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/VacationType/EditVacationType.cshtml", VacationType);
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
        VacationType.DeleteYNID = 0;
        _appDBContext.Settings_VacationTypes.Add(VacationType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Vacation Type Created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Vacation Type. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/VacationType/AddVacationType.cshtml", VacationType);
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
      TempData["SuccessMessage"] = "Vacation Type Deleted successfully.";
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
        worksheet.Cells["C1"].Value = "Active";


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
