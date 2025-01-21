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

        var itemDetails = await _appDBContext.ST_Items
     .Where(item => item.ItemID == model.Stocks.ItemID)
     .Select(item => new
     {
       HasLotNumberAndExpiryDate = item.HasLotNumberAndExpiryDate
     })
     .FirstOrDefaultAsync();

        if (itemDetails == null)
        {
          return Json(new { success = false, message = "Invalid ItemID!" });
        }

        ST_Stock existingStock = null;

        if (itemDetails.HasLotNumberAndExpiryDate == false)
        {
          // Find existing stock by ItemID only
          existingStock = await _appDBContext.ST_Stocks
              .Include(s => s.StockComponents)
              .FirstOrDefaultAsync(s => s.ItemID == model.Stocks.ItemID);
        }
        else
        {
          // Find existing stock by ItemID, LotNumber, and ExpiryDate
          existingStock = await _appDBContext.ST_Stocks
              .Include(s => s.StockComponents)
              .FirstOrDefaultAsync(s => s.ItemID == model.Stocks.ItemID
                  && s.LotNumber == model.Stocks.LotNumber
                  && s.ExpiryDate == model.Stocks.ExpiryDate);
        }

        if (existingStock != null)
        {
          // Update existing stock
          existingStock.Quantity += model.Stocks.Quantity; // Increment quantity
          existingStock.VendorID = model.Stocks.VendorID; // Update vendor

          // Conditionally update LotNumber and ExpiryDate if applicable
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            existingStock.LotNumber = model.Stocks.LotNumber;
            existingStock.ExpiryDate = model.Stocks.ExpiryDate;
          }

          existingStock.PONo = model.Stocks.PONo;
          existingStock.GRNNo = model.Stocks.GRNNo;
          existingStock.DCNo = model.Stocks.DCNo;
          existingStock.InvoiceNo = model.Stocks.InvoiceNo;
          existingStock.StockDate = DateTime.Now;

          // Update or add stock components
          if (model.Stocks.StockComponents != null && model.Stocks.StockComponents.Any())
          {
            foreach (var component in model.Stocks.StockComponents)
            {
              var existingComponent = existingStock.StockComponents
                  .FirstOrDefault(sc => sc.ItemComponentTypeID == component.ItemComponentTypeID);

              if (existingComponent != null)
              {
                // Update existing component
                existingComponent.ItemComponentValue = component.ItemComponentValue;
              }
              else if (!string.IsNullOrWhiteSpace(component.ItemComponentValue))
              {
                // Add new component
                existingStock.StockComponents.Add(new ST_StockComponent
                {
                  StockID = existingStock.StockID,
                  ItemComponentTypeID = component.ItemComponentTypeID,
                  ItemComponentValue = component.ItemComponentValue
                });
              }
            }
          }
          var newStockHistory = new ST_StockHistory
          {
            ItemID = model.Stocks.ItemID,
            VendorID = model.Stocks.VendorID,
            Quantity = model.Stocks.Quantity,
            UnitTypeID = model.Stocks.UnitTypeID,
            PONo = model.Stocks.PONo,
            GRNNo = model.Stocks.GRNNo,
            DCNo = model.Stocks.DCNo,
            InvoiceNo = model.Stocks.InvoiceNo,
            StockDate = DateTime.Now,
            StockID = existingStock.StockID,
            ActionType = "Update",
            ActionDate = DateTime.Now,
            ActionUserID = HttpContext.Session.GetInt32("UserID")
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStockHistory.LotNumber = model.Stocks.LotNumber;
            newStockHistory.ExpiryDate = model.Stocks.ExpiryDate;
          }

          _appDBContext.ST_StockHistorys.Add(newStockHistory);
        }
        else
        {
          // Create new stock
          var newStock = new ST_Stock
          {
            ItemID = model.Stocks.ItemID,
            VendorID = model.Stocks.VendorID,
            Quantity = model.Stocks.Quantity,
            UnitTypeID = model.Stocks.UnitTypeID,
            PONo = model.Stocks.PONo,
            GRNNo = model.Stocks.GRNNo,
            DCNo = model.Stocks.DCNo,
            InvoiceNo = model.Stocks.InvoiceNo,
            StockDate = DateTime.Now
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStock.LotNumber = model.Stocks.LotNumber;
            newStock.ExpiryDate = model.Stocks.ExpiryDate;
          }

          // Add stock components
          if (model.Stocks.StockComponents != null && model.Stocks.StockComponents.Any())
          {
            foreach (var component in model.Stocks.StockComponents)
            {
              if (!string.IsNullOrWhiteSpace(component.ItemComponentValue))
              {
                newStock.StockComponents.Add(new ST_StockComponent
                {
                  ItemComponentTypeID = component.ItemComponentTypeID,
                  ItemComponentValue = component.ItemComponentValue
                });
              }
            }
          }

          _appDBContext.ST_Stocks.Add(newStock);
          await _appDBContext.SaveChangesAsync();
          var newStockHistory = new ST_StockHistory
          {
            ItemID = model.Stocks.ItemID,
            VendorID = model.Stocks.VendorID,
            Quantity = model.Stocks.Quantity,
            UnitTypeID = model.Stocks.UnitTypeID,
            PONo = model.Stocks.PONo,
            GRNNo = model.Stocks.GRNNo,
            DCNo = model.Stocks.DCNo,
            InvoiceNo = model.Stocks.InvoiceNo,
            StockDate = DateTime.Now,
            StockID = newStock.StockID,
            ActionType = "Insert",
            ActionDate = DateTime.Now,
            ActionUserID = HttpContext.Session.GetInt32("UserID")
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStockHistory.LotNumber = model.Stocks.LotNumber;
            newStockHistory.ExpiryDate = model.Stocks.ExpiryDate;
          }

          _appDBContext.ST_StockHistorys.Add(newStockHistory);
        }

        // Save changes to the database
        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true, message = "Stock saved successfully!" });


      }
      catch (Exception ex)
      {
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
