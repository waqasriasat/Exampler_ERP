using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class VoucherTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public VoucherTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchVoucherTypeName)
    {
      var VoucherTypesQuery = _appDBContext.Settings_VoucherTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchVoucherTypeName))
      {
        VoucherTypesQuery = VoucherTypesQuery.Where(b => b.VoucherTypeName.Contains(searchVoucherTypeName));
      }

      var VoucherTypes = await VoucherTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchVoucherTypeName) && VoucherTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No VoucherType found with the name '" + searchVoucherTypeName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/MasterInfo/VoucherType/VoucherType.cshtml", VoucherTypes);
    }


    public async Task<IActionResult> VoucherType()
    {
      var VoucherTypes = await _appDBContext.Settings_VoucherTypes.ToListAsync();
      return Ok(VoucherTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var VoucherType = await _appDBContext.Settings_VoucherTypes.FindAsync(id);
      if (VoucherType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/VoucherType/EditVoucherType.cshtml", VoucherType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_VoucherType VoucherType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VoucherType.VoucherTypeName))
        {
          return Json(new { success = false, message = "VoucherType Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(VoucherType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "VoucherType updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating VoucherType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/VoucherType/AddVoucherType.cshtml", new Settings_VoucherType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_VoucherType VoucherType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(VoucherType.VoucherTypeName))
        {
          return Json(new { success = false, message = "VoucherType Name field is required. Please enter a valid text value." });
        }


        VoucherType.DeleteYNID = 0;

        _appDBContext.Settings_VoucherTypes.Add(VoucherType);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "VoucherType created successfully.";
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating VoucherType. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var VoucherType = await _appDBContext.Settings_VoucherTypes.FindAsync(id);
      if (VoucherType == null)
      {
        return NotFound();
      }

      VoucherType.ActiveYNID = 2;
      VoucherType.DeleteYNID = 1;

      _appDBContext.Settings_VoucherTypes.Update(VoucherType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "VoucherType deleted successfully.";

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var VoucherTypes = await _appDBContext.Settings_VoucherTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("VoucherTypes");
        worksheet.Cells["A1"].Value = "VoucherType ID";
        worksheet.Cells["B1"].Value = "Voucher Type Name";
        worksheet.Cells["C1"].Value = "Active";
        worksheet.Cells["D1"].Value = "Voucher Nature";
        worksheet.Cells["E1"].Value = "Voucher Prefix";


        for (int i = 0; i < VoucherTypes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = VoucherTypes[i].VoucherTypeID;
          worksheet.Cells[i + 2, 2].Value = VoucherTypes[i].VoucherTypeName;
          worksheet.Cells[i + 2, 3].Value = VoucherTypes[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 4].Value = VoucherTypes[i].VoucherNature;
          worksheet.Cells[i + 2, 5].Value = VoucherTypes[i].VoucherPrefix;
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"VoucherTypes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var VoucherTypes = await _appDBContext.Settings_VoucherTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/VoucherType/PrintVoucherType.cshtml", VoucherTypes);
    }


  }
}
