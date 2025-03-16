using Exampler_ERP.Controllers.HR.MasterInfo;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class StockController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StockController> _logger;
    private readonly Utils _utils;
    public StockController(AppDBContext appDBContext, IConfiguration configuration, ILogger<StockController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var ItemsQuery = _appDBContext.ST_Items.AsQueryable();

      if (!string.IsNullOrEmpty(searchItemName))
      {
        ItemsQuery = ItemsQuery
            .Where(pt => pt.ItemName.Contains(searchItemName));
      }

      var Items = await ItemsQuery.ToListAsync();
      var Stock = await _appDBContext.ST_Stocks.ToListAsync();
      var viewModel = new StockListViewModel
      {
        ItemsWithStockQuantity = Items.Select(pt => new ItemWithStockQuantityViewModel
        {
          ItemID = pt.ItemID,
          ItemName = pt.ItemName,
          StockQuantity = Stock.Where(ptf => ptf.ItemID == pt.ItemID).Sum(ptf => ptf.Quantity),
          StockCount = Stock.Count(ptf => ptf.ItemID == pt.ItemID)
        }).ToList()
      };

      if (!string.IsNullOrEmpty(searchItemName) && viewModel.ItemsWithStockQuantity.Count == 0)
      {
        TempData["ErrorMessage"] = "No Item found with the name '" + searchItemName + "'. Please check the name and try again.";
      }

      return View("~/Views/StoreManagement/StoreManagement/Stock/Stock.cshtml", viewModel);
    }
    [HttpGet]
    public async Task<IActionResult> GetItemComponents(int itemId)
    {
      var itemDetails = await _appDBContext.ST_Items
          .Where(item => item.ItemID == itemId)
          .Select(item => new
          {
            ItemCategoryTypeID = item.ItemCategoryTypeID,
            HasLotNumberAndExpiryDate = item.HasLotNumberAndExpiryDate
          })
          .FirstOrDefaultAsync();

      if (itemDetails == null)
      {
        return Json(new { hasLotNumberAndExpiryDate = false, components = new List<object>() });
      }

      var components = await _appDBContext.Settings_ItemComponentTypes
          .Where(c => c.ItemCategoryTypeID == itemDetails.ItemCategoryTypeID)
          .Select(c => new
          {
            ItemTypeID = c.ItemComponentTypeID,
            ItemTypeName = c.ItemComponentTypeName,
            ItemDataType = c.ItemComponentDataType
          })
          .ToListAsync();

      return Json(new
      {
        hasLotNumberAndExpiryDate = itemDetails.HasLotNumberAndExpiryDate,
        components
      });
    }
    public async Task<IActionResult> Edit(int id)
    {
      var Stocks = await _appDBContext.ST_Stocks
           .Include(v => v.Vendor)
           .FirstOrDefaultAsync(v => v.StockID == id);

      if (Stocks == null)
      {
        return NotFound();
      }


      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.VendorList = await _utils.GetVendorList();
      ViewBag.UnitList = await _utils.GetItemUnits();
      return PartialView("~/Views/StoreManagement/StoreManagement/Stock/EditStock.cshtml", Stocks);
    }
    public async Task<IActionResult> Print()
    {
      var items = await _appDBContext.ST_Items.ToListAsync();
      var stock = await _appDBContext.ST_Stocks.ToListAsync();

      var viewModel = new StockListViewModel
      {
        ItemsWithStockQuantity = items.Select(pt => new ItemWithStockQuantityViewModel
        {
          ItemID = pt.ItemID,
          ItemName = pt.ItemName,
          StockQuantity = stock.Where(ptf => ptf.ItemID == pt.ItemID).Sum(ptf => ptf.Quantity),
          StockCount = stock.Count(ptf => ptf.ItemID == pt.ItemID)
        }).ToList()
      };

      return View("~/Views/StoreManagement/StoreManagement/Stock/PrintStock.cshtml", viewModel.ItemsWithStockQuantity);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var Items = await _appDBContext.ST_Items.ToListAsync();
      var Stock = await _appDBContext.ST_Stocks.ToListAsync();

      var viewModel = new StockListViewModel
      {
        ItemsWithStockQuantity = Items.Select(pt => new ItemWithStockQuantityViewModel
        {
          ItemID = pt.ItemID,
          ItemName = pt.ItemName,
          StockQuantity = Stock.Where(ptf => ptf.ItemID == pt.ItemID).Sum(ptf => ptf.Quantity),
          StockCount = Stock.Count(ptf => ptf.ItemID == pt.ItemID)
        }).ToList()
      };

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("Stocks");

        // **Header Row**
        worksheet.Cells["A1"].Value = "ItemID #";
        worksheet.Cells["B1"].Value = "Item Name";
        worksheet.Cells["C1"].Value = "Stock Quantity";
        worksheet.Cells["D1"].Value = "Stock Count";

        // **Apply Bold to Headers**
        using (var range = worksheet.Cells["A1:D1"])
        {
          range.Style.Font.Bold = true;
          range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
          range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
        }

        // **Data Insertion**
        for (int i = 0; i < viewModel.ItemsWithStockQuantity.Count; i++)
        {
          var item = viewModel.ItemsWithStockQuantity[i];
          worksheet.Cells[i + 2, 1].Value = item.ItemID;
          worksheet.Cells[i + 2, 2].Value = item.ItemName;
          worksheet.Cells[i + 2, 3].Value = item.StockQuantity;
          worksheet.Cells[i + 2, 4].Value = item.StockCount;
        }

        // **Auto-fit columns**
        worksheet.Cells.AutoFitColumns();

        // **Generate File**
        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"Stocks-{DateTime.Now:yyyyMMddHHmmssfff}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }

  }
}
