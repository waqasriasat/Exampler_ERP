using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{

  public class EmployeeRequestTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public EmployeeRequestTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchEmployeeRequestTypeName)
    {

      var EmployeeRequestTypesQuery = _appDBContext.Settings_EmployeeRequestTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName))
      {
        EmployeeRequestTypesQuery = EmployeeRequestTypesQuery.Where(b => b.EmployeeRequestTypeName.Contains(searchEmployeeRequestTypeName));
      }

      var EmployeeRequestTypes = await EmployeeRequestTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchEmployeeRequestTypeName) && EmployeeRequestTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No Employee Request Type found with the name '" + searchEmployeeRequestTypeName + "'. Please check the name and try again.";
      }

      return View("~/Views/HR/MasterInfo/EmployeeRequestType/EmployeeRequestType.cshtml", EmployeeRequestTypes);
    }

    public async Task<IActionResult> EmployeeRequestType()
    {
      var EmployeeRequestTypes = await _appDBContext.Settings_EmployeeRequestTypes.ToListAsync();
      return Ok(EmployeeRequestTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var EmployeeRequestType = await _appDBContext.Settings_EmployeeRequestTypes.FindAsync(id);
      if (EmployeeRequestType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestType/EditEmployeeRequestType.cshtml", EmployeeRequestType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_EmployeeRequestType EmployeeRequestType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(EmployeeRequestType.EmployeeRequestTypeName))
        {
          return Json(new { success = false, message = "EmployeeRequestType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(EmployeeRequestType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Employee Request Type Updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating EmployeeRequestType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/EmployeeRequestType/AddEmployeeRequestType.cshtml", new Settings_EmployeeRequestType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_EmployeeRequestType EmployeeRequestType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(EmployeeRequestType.EmployeeRequestTypeName))
        {
          return Json(new { success = false, message = "EmployeeRequestType Name field is required. Please enter a valid text value." });
        }
        EmployeeRequestType.DeleteYNID = 0;
        _appDBContext.Settings_EmployeeRequestTypes.Add(EmployeeRequestType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Employee Request Type Created successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating EmployeeRequestType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var EmployeeRequestType = await _appDBContext.Settings_EmployeeRequestTypes.FindAsync(id);
      if (EmployeeRequestType == null)
      {
        return NotFound();
      }

      EmployeeRequestType.ActiveYNID = 2;
      EmployeeRequestType.DeleteYNID = 1;

      _appDBContext.Settings_EmployeeRequestTypes.Update(EmployeeRequestType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Employee Request Type Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var EmployeeRequestTypees = await _appDBContext.Settings_EmployeeRequestTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("EmployeeRequestTypees");
        worksheet.Cells["A1"].Value = "EmployeeRequestType ID";
        worksheet.Cells["B1"].Value = "EmployeeRequestType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < EmployeeRequestTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeRequestTypees[i].EmployeeRequestTypeID;
          worksheet.Cells[i + 2, 2].Value = EmployeeRequestTypees[i].EmployeeRequestTypeName;
          worksheet.Cells[i + 2, 3].Value = EmployeeRequestTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"EmployeeRequestTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var EmployeeRequestTypees = await _appDBContext.Settings_EmployeeRequestTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/EmployeeRequestType/PrintEmployeeRequestTypes.cshtml", EmployeeRequestTypees);
    }

  }

}
