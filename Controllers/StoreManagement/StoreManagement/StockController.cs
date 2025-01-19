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
          StockQuantity = Stock.Count(ptf => ptf.ItemID == pt.ItemID)
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
      var categoryID = await _appDBContext.ST_Items
          .Where(item => item.ItemID == itemId)
          .Select(item => item.ItemCategoryTypeID)
          .FirstOrDefaultAsync();

      if (categoryID == null)
      {
        return Json(new List<object>()); 
      }

      var components = await _appDBContext.Settings_ItemComponentTypes
         .Where(c => c.ItemCategoryTypeID == categoryID)
         .Select(c => new
         {
           ItemTypeID = c.ItemComponentTypeID,
           ItemTypeName = c.ItemComponentTypeName,
           ItemDataType = c.ItemComponentDataType
         })
         .ToListAsync();

      return Json(components);
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.VendorList = await _utils.GetVendorList();
      ViewBag.UnitList = await _utils.GetItemUnits();
      ST_Stock stock = new ST_Stock();
      stock.StockComponents.Add(new ST_StockComponent() { StockID = 0 });
      var model = new StocksIndexViewModel
      {
        Stocks = stock
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/Stock/AddStock.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(StocksIndexViewModel model)
    {
      if (!ModelState.IsValid)
      {
        // If the model is invalid, return the partial view with validation errors
        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.VendorList = await _utils.GetVendorList();
        ViewBag.UnitList = await _utils.GetItemUnits();
        return PartialView("~/Views/StoreManagement/StoreManagement/Stock/AddStock.cshtml", model);
      }

      try
      {
        // Create a new stock object and populate its properties from the model
        var newStock = new ST_Stock
        {
          ItemID = model.Stocks.ItemID,
          VendorID = model.Stocks.VendorID,
          Quantity = model.Stocks.Quantity,
          UnitTypeID = model.Stocks.UnitTypeID,
          LotNumber = model.Stocks.LotNumber,
          PONo = model.Stocks.PONo,
          GRNNo = model.Stocks.GRNNo,
          DCNo = model.Stocks.DCNo,
          InvoiceNo = model.Stocks.InvoiceNo,
          StockDate = DateTime.Now
        };

        // Add stock components if provided
        if (model.Stocks.StockComponents != null && model.Stocks.StockComponents.Any())
        {
          foreach (var component in model.Stocks.StockComponents)
          {
            if (!string.IsNullOrWhiteSpace(component.ItemComponentValue)) // Check if ItemComponentValue is not null or empty
            {
              newStock.StockComponents.Add(new ST_StockComponent
              {
                StockID = model.Stocks.StockID,
                ItemComponentTypeID = component.ItemComponentTypeID,
                ItemComponentValue = component.ItemComponentValue
              });
            }
          }
        }


        // Save the new stock record to the database
        _appDBContext.ST_Stocks.Add(newStock);
        await _appDBContext.SaveChangesAsync();

        // Return success response
        return Json(new { success = true, message = "Stock added successfully!" });
      }
      catch (Exception ex)
      {
        // Log the error and return a failure response
        // Use your logging mechanism here
        return Json(new { success = false, message = "An error occurred while adding the stock. Please try again later.", error = ex.Message });
      }
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

      //Stocks.StockComponents.Add(new ST_StockComponent() { StockID = Stocks.StockID });

      //var model = new StocksIndexViewModel
      //{
      //  Stocks = Stocks
      //};

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.VendorList = await _utils.GetVendorList();
      ViewBag.UnitList = await _utils.GetItemUnits();
      return PartialView("~/Views/StoreManagement/StoreManagement/Stock/EditStock.cshtml", Stocks);
    }
  }
}
