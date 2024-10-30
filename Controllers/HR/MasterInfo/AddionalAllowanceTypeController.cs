using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.MasterInfo
{
  public class AddionalAllowanceTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public AddionalAllowanceTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchAddionalAllowanceTypeName)
    {

      var AddionalAllowanceTypesQuery = _appDBContext.Settings_AddionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchAddionalAllowanceTypeName))
      {
        AddionalAllowanceTypesQuery = AddionalAllowanceTypesQuery.Where(b => b.AddionalAllowanceTypeName.Contains(searchAddionalAllowanceTypeName));
      }

      var AddionalAllowanceTypes = await AddionalAllowanceTypesQuery.ToListAsync();

      return View("~/Views/HR/MasterInfo/AddionalAllowanceType/AddionalAllowanceType.cshtml", AddionalAllowanceTypes);
    }
  
    public async Task<IActionResult> AddionalAllowanceType()
    {
      var AddionalAllowanceTypes = await _appDBContext.Settings_AddionalAllowanceTypes.ToListAsync();
      return Ok(AddionalAllowanceTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var AddionalAllowanceType = await _appDBContext.Settings_AddionalAllowanceTypes.FindAsync(id);
      if (AddionalAllowanceType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/HR/MasterInfo/AddionalAllowanceType/EditAddionalAllowanceType.cshtml", AddionalAllowanceType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_AddionalAllowanceType AddionalAllowanceType)
    {
      if (ModelState.IsValid)
      {
        _appDBContext.Update(AddionalAllowanceType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Addional Allowance Type Updated successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error Updating Addional Allowance Type. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/AddionalAllowanceType/EditAddionalAllowanceType.cshtml", AddionalAllowanceType);
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/HR/MasterInfo/AddionalAllowanceType/AddAddionalAllowanceType.cshtml", new Settings_AddionalAllowanceType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_AddionalAllowanceType AddionalAllowanceType)
    {
      if (ModelState.IsValid)
      {
        AddionalAllowanceType.DeleteYNID = 0;
        _appDBContext.Settings_AddionalAllowanceTypes.Add(AddionalAllowanceType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Addional Allowance Type Created successfully.";
        return Json(new { success = true });
      }
      TempData["ErrorMessage"] = "Error creating Addional Allowance Type. Please check the inputs.";
      return PartialView("~/Views/HR/MasterInfo/AddionalAllowanceType/AddAddionalAllowanceType.cshtml", AddionalAllowanceType);
    }
    public async Task<IActionResult> Delete(int id)
    {
      var AddionalAllowanceType = await _appDBContext.Settings_AddionalAllowanceTypes.FindAsync(id);
      if (AddionalAllowanceType == null)
      {
        return NotFound();
      }

      AddionalAllowanceType.ActiveYNID = 2;
      AddionalAllowanceType.DeleteYNID = 1;

      _appDBContext.Settings_AddionalAllowanceTypes.Update(AddionalAllowanceType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Addional Allowance Type Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var AddionalAllowanceTypees = await _appDBContext.Settings_AddionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("AddionalAllowanceTypees");
        worksheet.Cells["A1"].Value = "AddionalAllowanceType ID";
        worksheet.Cells["B1"].Value = "AddionalAllowanceType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < AddionalAllowanceTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = AddionalAllowanceTypees[i].AddionalAllowanceTypeID;
          worksheet.Cells[i + 2, 2].Value = AddionalAllowanceTypees[i].AddionalAllowanceTypeName;
          worksheet.Cells[i + 2, 3].Value = AddionalAllowanceTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"AddionalAllowanceTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var AddionalAllowanceTypees = await _appDBContext.Settings_AddionalAllowanceTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/HR/MasterInfo/AddionalAllowanceType/PrintAddionalAllowanceTypes.cshtml", AddionalAllowanceTypees);
    }

  }
}
