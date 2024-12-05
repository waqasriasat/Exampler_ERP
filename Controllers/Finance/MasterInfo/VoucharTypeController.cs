using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class VoucharTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public VoucharTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchVoucharTypeName)
    {
      var VoucharTypesQuery = _appDBContext.Settings_VoucharTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchVoucharTypeName))
      {
        VoucharTypesQuery = VoucharTypesQuery.Where(b => b.VoucharTypeName.Contains(searchVoucharTypeName));
      }

      var VoucharTypes = await VoucharTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchVoucharTypeName) && VoucharTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No VoucharType found with the name '" + searchVoucharTypeName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/MasterInfo/VoucharType/VoucharType.cshtml", VoucharTypes);
    }


    public async Task<IActionResult> VoucharType()
    {
      var VoucharTypes = await _appDBContext.Settings_VoucharTypes.ToListAsync();
      return Ok(VoucharTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var VoucharType = await _appDBContext.Settings_VoucharTypes.FindAsync(id);
      if (VoucharType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/VoucharType/EditVoucharType.cshtml", VoucharType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_VoucharType VoucharType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VoucharType.VoucharTypeName))
        {
          return Json(new { success = false, message = "VoucharType Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(VoucharType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "VoucharType updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating VoucharType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/VoucharType/AddVoucharType.cshtml", new Settings_VoucharType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_VoucharType VoucharType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VoucharType.VoucharTypeName))
        {
          return Json(new { success = false, message = "VoucharType Name field is required. Please enter a valid text value." });
        }


        VoucharType.DeleteYNID = 0;

        _appDBContext.Settings_VoucharTypes.Add(VoucharType);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "VoucharType created successfully.";
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating VoucharType. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var VoucharType = await _appDBContext.Settings_VoucharTypes.FindAsync(id);
      if (VoucharType == null)
      {
        return NotFound();
      }

      VoucharType.ActiveYNID = 2;
      VoucharType.DeleteYNID = 1;

      _appDBContext.Settings_VoucharTypes.Update(VoucharType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "VoucharType deleted successfully.";

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var VoucharTypes = await _appDBContext.Settings_VoucharTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("VoucharTypes");
        worksheet.Cells["A1"].Value = "VoucharType ID";
        worksheet.Cells["B1"].Value = "Vouchar Type Name";
        worksheet.Cells["C1"].Value = "Active";
        worksheet.Cells["D1"].Value = "Vouchar Nature";
        worksheet.Cells["E1"].Value = "Vouchar Prefix";


        for (int i = 0; i < VoucharTypes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = VoucharTypes[i].VoucharTypeID;
          worksheet.Cells[i + 2, 2].Value = VoucharTypes[i].VoucharTypeName;
          worksheet.Cells[i + 2, 3].Value = VoucharTypes[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 4].Value = VoucharTypes[i].VoucharNature;
          worksheet.Cells[i + 2, 5].Value = VoucharTypes[i].VoucharPrefix;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"VoucharTypes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var VoucharTypes = await _appDBContext.Settings_VoucharTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/VoucharType/PrintVoucharType.cshtml", VoucharTypes);
    }


  }
}
