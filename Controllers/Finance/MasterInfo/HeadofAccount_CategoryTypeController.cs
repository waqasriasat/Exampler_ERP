using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Finance.MasterInfo
{
  public class HeadofAccount_CategoryTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public HeadofAccount_CategoryTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchCategoryTypeName)
    {
      var HeadofAccount_CategoryTypesQuery = _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchCategoryTypeName))
      {
        HeadofAccount_CategoryTypesQuery = HeadofAccount_CategoryTypesQuery.Where(b => b.CategoryTypeName.Contains(searchCategoryTypeName));
      }

      var HeadofAccount_CategoryTypes = await HeadofAccount_CategoryTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchCategoryTypeName) && HeadofAccount_CategoryTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No Category Type found with the name '" + searchCategoryTypeName + "'. Please check the name and try again.";
      }
      return View("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/HeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryTypes);
    }


    public async Task<IActionResult> HeadofAccount_CategoryType()
    {
      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes.ToListAsync();
      return Ok(HeadofAccount_CategoryTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var HeadofAccount_CategoryType = await _appDBContext.Settings_HeadofAccount_CategoryTypes.FindAsync(id);
      if (HeadofAccount_CategoryType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/EditHeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_HeadofAccount_CategoryType HeadofAccount_CategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_CategoryType.CategoryTypeName))
        {
          return Json(new { success = false, message = "Category Type Name field is required. Please enter a valid text value." });
        }


        _appDBContext.Update(HeadofAccount_CategoryType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Category Type updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Category Type. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/AddHeadofAccount_CategoryType.cshtml", new Settings_HeadofAccount_CategoryType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_HeadofAccount_CategoryType HeadofAccount_CategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(HeadofAccount_CategoryType.CategoryTypeName))
        {
          return Json(new { success = false, message = "Category Type Name field is required. Please enter a valid text value." });
        }


        HeadofAccount_CategoryType.DeleteYNID = 0;

        _appDBContext.Settings_HeadofAccount_CategoryTypes.Add(HeadofAccount_CategoryType);
        await _appDBContext.SaveChangesAsync();

        TempData["SuccessMessage"] = "Category Type created successfully.";
        return Json(new { success = true });
      }

      return Json(new { success = false, message = "Error creating Category Type. Please check the inputs." });
    }

    public async Task<IActionResult> Delete(int id)
    {
      var HeadofAccount_CategoryType = await _appDBContext.Settings_HeadofAccount_CategoryTypes.FindAsync(id);
      if (HeadofAccount_CategoryType == null)
      {
        return NotFound();
      }

      HeadofAccount_CategoryType.ActiveYNID = 2;
      HeadofAccount_CategoryType.DeleteYNID = 1;

      _appDBContext.Settings_HeadofAccount_CategoryTypes.Update(HeadofAccount_CategoryType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Category Type deleted successfully.";

      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("HeadofAccount_CategoryTypes");
        worksheet.Cells["A1"].Value = "Category Type ID";
        worksheet.Cells["B1"].Value = "Category Type Name";
        worksheet.Cells["C1"].Value = "Active";
    

        for (int i = 0; i < HeadofAccount_CategoryTypes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = HeadofAccount_CategoryTypes[i].CategoryTypeID;
          worksheet.Cells[i + 2, 2].Value = HeadofAccount_CategoryTypes[i].CategoryTypeName;
          worksheet.Cells[i + 2, 3].Value = HeadofAccount_CategoryTypes[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:l1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"HeadofAccount_CategoryTypes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var HeadofAccount_CategoryTypes = await _appDBContext.Settings_HeadofAccount_CategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Finance/MasterInfo/HeadofAccount_CategoryType/PrintHeadofAccount_CategoryType.cshtml", HeadofAccount_CategoryTypes);
    }


  }
}
