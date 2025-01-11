using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.StoreManagement.MasterInfo
{
  public class ItemComponentTypeController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public ItemComponentTypeController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemComponentTypeName)
    {

      var ItemComponentTypesQuery = _appDBContext.Settings_ItemComponentTypes
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchItemComponentTypeName))
      {
        ItemComponentTypesQuery = ItemComponentTypesQuery.Where(b => b.ItemComponentTypeName.Contains(searchItemComponentTypeName));
      }

      var ItemComponentTypes = await ItemComponentTypesQuery
        .Include(d => d.ItemCategoryType).ToListAsync();

      if (!string.IsNullOrEmpty(searchItemComponentTypeName) && ItemComponentTypes.Count == 0)
      {
        TempData["ErrorMessage"] = "No Item Component Type found with the name '" + searchItemComponentTypeName + "'. Please check the name and try again.";
      }

      return View("~/Views/StoreManagement/MasterInfo/ItemComponentType/ItemComponentType.cshtml", ItemComponentTypes);
    }
    public async Task<IActionResult> ItemComponentType()
    {
      var ItemComponentTypes = await _appDBContext.Settings_ItemComponentTypes.ToListAsync();
      return Ok(ItemComponentTypes);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ItemCategoryList = await _utils.GetItemCategorys();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      var ItemComponentType = await _appDBContext.Settings_ItemComponentTypes.FindAsync(id);
      if (ItemComponentType == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/StoreManagement/MasterInfo/ItemComponentType/EditItemComponentType.cshtml", ItemComponentType);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Settings_ItemComponentType ItemComponentType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ItemComponentType.ItemComponentTypeName))
        {
          return Json(new { success = false, message = "ItemComponentType Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(ItemComponentType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Item Component Type Updated successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ItemComponentType. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemCategoryList = await _utils.GetItemCategorys();
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      return PartialView("~/Views/StoreManagement/MasterInfo/ItemComponentType/AddItemComponentType.cshtml", new Settings_ItemComponentType());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Settings_ItemComponentType ItemComponentType)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(ItemComponentType.ItemComponentTypeName))
        {
          return Json(new { success = false, message = "ItemComponentType Name field is required. Please enter a valid text value." });
        }
        ItemComponentType.DeleteYNID = 0;
        _appDBContext.Settings_ItemComponentTypes.Add(ItemComponentType);
        await _appDBContext.SaveChangesAsync();
        TempData["SuccessMessage"] = "Item Component Type Created successfully.";
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating ItemComponentType. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var ItemComponentType = await _appDBContext.Settings_ItemComponentTypes.FindAsync(id);
      if (ItemComponentType == null)
      {
        return NotFound();
      }

      ItemComponentType.ActiveYNID = 2;
      ItemComponentType.DeleteYNID = 1;

      _appDBContext.Settings_ItemComponentTypes.Update(ItemComponentType);
      await _appDBContext.SaveChangesAsync();
      TempData["SuccessMessage"] = "Item Component Type Deleted successfully.";
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var ItemComponentTypees = await _appDBContext.Settings_ItemComponentTypes
        .Include(d => d.ItemCategoryType)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("ItemComponentTypees");
        worksheet.Cells["A1"].Value = "ItemComponentType ID";
        worksheet.Cells["B1"].Value = "ItemComponentType Name";
        worksheet.Cells["C1"].Value = "Active";


        for (int i = 0; i < ItemComponentTypees.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = ItemComponentTypees[i].ItemComponentTypeID;
          worksheet.Cells[i + 2, 2].Value = ItemComponentTypees[i].ItemComponentTypeName;
          worksheet.Cells[i + 2, 3].Value = ItemComponentTypees[i].ActiveYNID == 1 ? "Yes" : "No";
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"ItemComponentTypees-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var ItemComponentTypees = await _appDBContext.Settings_ItemComponentTypes
        .Include(d => d.ItemCategoryType)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/StoreManagement/MasterInfo/ItemComponentType/PrintItemComponentType.cshtml", ItemComponentTypees);
    }

  }
}
