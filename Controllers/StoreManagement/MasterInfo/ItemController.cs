using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.MasterInfo.StoreManagement
{
  public class ItemController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ItemController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public ItemController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ItemController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchItemName)
    {

      var ItemsQuery = _appDBContext.ST_Items
        .Include(b => b.ItemCategoryType)
        .Include(b => b.UnitType)
          .Where(b => b.DeleteYNID != 1);

      if (!string.IsNullOrEmpty(searchItemName))
      {
        ItemsQuery = ItemsQuery.Where(b => b.ItemName.Contains(searchItemName));
      }

      var Items = await ItemsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && Items.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Item found with the name '" + searchItemName + "'. Please check the name and try again.");
      }

      return View("~/Views/StoreManagement/MasterInfo/Item/Item.cshtml", Items);
    }
    public async Task<IActionResult> Item()
    {
      var Items = await _appDBContext.ST_Items.ToListAsync();
      return Ok(Items);
    }// Add the Edit action
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.UnitTypeIDList = await _utils.GetItemUnits();
      ViewBag.ItemCategoryTypeIDList = await _utils.GetItemCategorys();
      ViewBag.ManufacturerTypeIDList = await _utils.GetItemManufacturers();
      var Item = await _appDBContext.ST_Items.FindAsync(id);
      if (Item == null)
      {
        return NotFound();
      }
      return PartialView("~/Views/StoreManagement/MasterInfo/Item/EditItem.cshtml", Item);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(ST_Item Item)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Item.ItemName))
        {
          return Json(new { success = false, message = "Item Name field is required. Please enter a valid text value." });
        }
        _appDBContext.Update(Item);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Item Updated successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Item. Please check the inputs." });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ActiveYNIDList = await _utils.GetActiveYNIDList();
      ViewBag.UnitTypeIDList = await _utils.GetItemUnits();
      ViewBag.ItemCategoryTypeIDList = await _utils.GetItemCategorys();
      ViewBag.ManufacturerTypeIDList = await _utils.GetItemManufacturers();

      return PartialView("~/Views/StoreManagement/MasterInfo/Item/AddItem.cshtml", new ST_Item());
    }

    [HttpPost]
    public async Task<IActionResult> Create(ST_Item Item)
    {
      if (ModelState.IsValid)
      {
        if (string.IsNullOrEmpty(Item.ItemName))
        {
          return Json(new { success = false, message = "Item Name field is required. Please enter a valid text value." });
        }
        Item.DeleteYNID = 0;
        _appDBContext.ST_Items.Add(Item);
        await _appDBContext.SaveChangesAsync();
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Item Created successfully.");
        return Json(new { success = true });
      }
      return Json(new { success = false, message = "Error creating Item. Please check the inputs." });
    }
    public async Task<IActionResult> Delete(int id)
    {
      var Item = await _appDBContext.ST_Items.FindAsync(id);
      if (Item == null)
      {
        return NotFound();
      }

      Item.ActiveYNID = 2;
      Item.DeleteYNID = 1;

      _appDBContext.ST_Items.Update(Item);
      await _appDBContext.SaveChangesAsync();
      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Item Deleted successfully.");
      return Json(new { success = true });
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Itemes = await _appDBContext.ST_Items
        .Include(b => b.ItemCategoryType)
        .Include(b => b.UnitType)
        .Include(b => b.ManufacturerType)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Itemes");
        worksheet.Cells["A1"].Value = "Item ID";
        worksheet.Cells["B1"].Value = "Item Code";
        worksheet.Cells["C1"].Value = "Item Name";
        worksheet.Cells["D1"].Value = "Status";
        worksheet.Cells["E1"].Value = "Unit";
        worksheet.Cells["F1"].Value = "Category";
        worksheet.Cells["G1"].Value = "Manufacturer";
        worksheet.Cells["H1"].Value = "Reorder Level Min";
        worksheet.Cells["I1"].Value = "Reorder Level Max";
        worksheet.Cells["J1"].Value = "BinLocation";
        worksheet.Cells["K1"].Value = "BarCode";
        worksheet.Cells["L1"].Value = "OpeningBalance";


        for (int i = 0; i < Itemes.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = Itemes[i].ItemID;
          worksheet.Cells[i + 2, 2].Value = Itemes[i].ItemCode;
          worksheet.Cells[i + 2, 3].Value = Itemes[i].ItemName;
          worksheet.Cells[i + 2, 4].Value = Itemes[i].ActiveYNID == 1 ? "Yes" : "No";
          worksheet.Cells[i + 2, 5].Value = Itemes[i].UnitType.UnitTypeName;
          worksheet.Cells[i + 2, 6].Value = Itemes[i].ItemCategoryType.ItemCategoryTypeName;
          worksheet.Cells[i + 2, 7].Value = Itemes[i].ManufacturerType.ManufacturerTypeName;
          worksheet.Cells[i + 2, 8].Value = Itemes[i].ReorderLevelMin;
          worksheet.Cells[i + 2, 9].Value = Itemes[i].ReorderLevelMax;
          worksheet.Cells[i + 2, 10].Value = Itemes[i].BinLocation;
          worksheet.Cells[i + 2, 11].Value = Itemes[i].BarCode;
          worksheet.Cells[i + 2, 12].Value = Itemes[i].OpeningBalance;

        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Itemes-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print()
    {
      var Itemes = await _appDBContext.ST_Items
        .Include(b => b.ItemCategoryType)
        .Include(b => b.UnitType)
        .Include(b => b.ManufacturerType)
          .Where(b => b.DeleteYNID != 1)
          .ToListAsync();
      return View("~/Views/StoreManagement/MasterInfo/Item/PrintItem.cshtml", Itemes);
    }
  }
}
