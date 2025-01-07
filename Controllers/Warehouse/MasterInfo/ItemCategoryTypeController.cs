using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Warehouse.MasterInfo
{
     public class ItemCategoryTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public ItemCategoryTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemCategoryTypeName)
    {

      var ItemCategoryTypesQuery = _appDBContext.Settings_ItemCategoryTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchItemCategoryTypeName))
      {
        ItemCategoryTypesQuery = ItemCategoryTypesQuery.Where(b => b.ItemCategoryTypeName.Contains(searchItemCategoryTypeName));
      }

      var ItemCategoryTypes = await ItemCategoryTypesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemCategoryTypeName) && ItemCategoryTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No Item Category Type found with the name '" + searchItemCategoryTypeName + "'. Please check the name and try again.";
      }

      return View("~/Views/Warehouse/MasterInfo/ItemCategoryType/ItemCategoryType.cshtml", ItemCategoryTypes);
    }
    public async Task<IActionResult> ItemCategoryType()
    {
      var ItemCategoryTypes = await _appDBContext.Settings_ItemCategoryTypes.ToListAsync();
      return Ok(ItemCategoryTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var ItemCategoryType = await _appDBContext.Settings_ItemCategoryTypes.FindAsync(id);
      if (ItemCategoryType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/Warehouse/MasterInfo/ItemCategoryType/EditItemCategoryType.cshtml", ItemCategoryType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_ItemCategoryType ItemCategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ItemCategoryType.ItemCategoryTypeName))
        {
          return Json(new { success = false, message = "ItemCategoryType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(ItemCategoryType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Item Category Type Updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ItemCategoryType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/Warehouse/MasterInfo/ItemCategoryType/AddItemCategoryType.cshtml", new Settings_ItemCategoryType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_ItemCategoryType ItemCategoryType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ItemCategoryType.ItemCategoryTypeName))
        {
          return Json(new { success = false, message = "ItemCategoryType Name field is required. Please enter a valid text value." });
        }
        ItemCategoryType.DeleteYNID = 0;
        _appDBContext.Settings_ItemCategoryTypes.Add(ItemCategoryType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Item Category Type Created successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ItemCategoryType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var ItemCategoryType = await _appDBContext.Settings_ItemCategoryTypes.FindAsync(id);
      if (ItemCategoryType == null)
      {
        return NotFound();
      }

      ItemCategoryType.ActiveYNID = 2;
      ItemCategoryType.DeleteYNID = 1;

      _appDBContext.Settings_ItemCategoryTypes.Update(ItemCategoryType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Item Category Type Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ItemCategoryTypees = await _appDBContext.Settings_ItemCategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ItemCategoryTypees");
        worksheet.Cells["A1"].Value = "ItemCategoryType ID";
        worksheet.Cells["B1"].Value = "ItemCategoryType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < ItemCategoryTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ItemCategoryTypees[i].ItemCategoryTypeID;
          worksheet.Cells[i + 2, 2].Value = ItemCategoryTypees[i].ItemCategoryTypeName;
          worksheet.Cells[i + 2, 3].Value = ItemCategoryTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ItemCategoryTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ItemCategoryTypees = await _appDBContext.Settings_ItemCategoryTypes
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/Warehouse/MasterInfo/ItemCategoryType/PrintItemCategoryType.cshtml", ItemCategoryTypees);
    }

  }
}
