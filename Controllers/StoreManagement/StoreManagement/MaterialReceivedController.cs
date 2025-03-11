using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialReceivedController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public MaterialReceivedController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var MaterialReceivedsQuery = _appDBContext.ST_MaterialReceiveds.Include(m => m.Item).AsQueryable();

      if (!string.IsNullOrEmpty(searchItemName))
      {
        MaterialReceivedsQuery = MaterialReceivedsQuery
            .Where(m => m.Item != null && m.Item.ItemName.Contains(searchItemName));
      }

      var MaterialReceiveds = await MaterialReceivedsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && MaterialReceiveds.Count == 0)
      {
        TempData["ErrorMessage"] = "No Material Received found with the name '" + searchItemName + "'. Please check the name and try again.";
      }

      return View("~/Views/StoreManagement/StoreManagement/MaterialReceived/MaterialReceived.cshtml", MaterialReceiveds);
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
    public async Task<IActionResult> GetReceivedItemComponents(int materialReceivedID)
    {
      var ReceiveditemDetails = await _appDBContext.ST_MaterialReceivedComponents
          .Where(item => item.MaterialReceivedID == materialReceivedID)
          .Select(item => new
          {
            ItemCategoryTypeID = item.ItemComponentTypeID
          })
          .FirstOrDefaultAsync();

      

      //var components = await _appDBContext.ST_Items
      //    .Where(c => c.ItemCategoryTypeID == ReceiveditemDetails.)
      //    .Select(c => new
      //    {
      //      ItemTypeID = c.ItemComponentTypeID,
      //      ItemTypeName = c.ItemComponentTypeName,
      //      ItemDataType = c.ItemComponentDataType,
      //      ItemTypeValue = c.
      //    })
      //    .ToListAsync();

      return Json(new
      {
        //components
      });
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.VendorList = await _utils.GetVendorList();
      ViewBag.UnitList = await _utils.GetItemUnits();
      ST_MaterialReceived MaterialReceived = new ST_MaterialReceived();
      MaterialReceived.MaterialReceivedComponents.Add(new ST_MaterialReceivedComponent() { MaterialReceivedID = 0 });
      var model = new MaterialReceivedsIndexViewModel
      {
        MaterialReceiveds = MaterialReceived
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialReceived/AddMaterialReceived.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(MaterialReceivedsIndexViewModel model)
    {
      if (!ModelState.IsValid)
      {
        // If the model is invalid, return the partial view with validation errors
        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.VendorList = await _utils.GetVendorList();
        ViewBag.UnitList = await _utils.GetItemUnits();
        return PartialView("~/Views/StoreManagement/StoreManagement/MaterialReceived/AddMaterialReceived.cshtml", model);
      }

      try
      {

        var itemDetails = await _appDBContext.ST_Items
     .Where(item => item.ItemID == model.MaterialReceiveds.ItemID)
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
              .FirstOrDefaultAsync(s => s.ItemID == model.MaterialReceiveds.ItemID);
        }
        else
        {
          // Find existing stock by ItemID, LotNumber, and ExpiryDate
          existingStock = await _appDBContext.ST_Stocks
              .FirstOrDefaultAsync(s => s.ItemID == model.MaterialReceiveds.ItemID
                  && s.LotNumber == model.MaterialReceiveds.LotNumber
                  && s.ExpiryDate == model.MaterialReceiveds.ExpiryDate);
        }

        if (existingStock != null)
        {
          // Update existing stock
          existingStock.Quantity += model.MaterialReceiveds.Quantity; // Increment quantity
          existingStock.VendorID = model.MaterialReceiveds.VendorID; // Update vendor

          // Conditionally update LotNumber and ExpiryDate if applicable
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            existingStock.LotNumber = model.MaterialReceiveds.LotNumber;
            existingStock.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          existingStock.PONo = model.MaterialReceiveds.PONo;
          existingStock.GRNNo = model.MaterialReceiveds.GRNNo;
          existingStock.DCNo = model.MaterialReceiveds.DCNo;
          existingStock.InvoiceNo = model.MaterialReceiveds.InvoiceNo;
          existingStock.StockDate = DateTime.Now;

          // Update or add stock components
          
          var newStockHistory = new ST_StockHistory
          {
            ItemID = model.MaterialReceiveds.ItemID,
            VendorID = model.MaterialReceiveds.VendorID,
            Quantity = model.MaterialReceiveds.Quantity,
            UnitTypeID = model.MaterialReceiveds.UnitTypeID,
            PONo = model.MaterialReceiveds.PONo,
            GRNNo = model.MaterialReceiveds.GRNNo,
            DCNo = model.MaterialReceiveds.DCNo,
            InvoiceNo = model.MaterialReceiveds.InvoiceNo,
            StockDate = DateTime.Now,
            StockID = existingStock.StockID,
            ActionType = "Update",
            ActionDate = DateTime.Now,
            ActionUserID = HttpContext.Session.GetInt32("UserID")
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStockHistory.LotNumber = model.MaterialReceiveds.LotNumber;
            newStockHistory.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          _appDBContext.ST_StockHistorys.Add(newStockHistory);

          var newItemLedger = new ST_ItemLedger
          {
            ItemLedgerDate = DateTime.Now,
            ItemID = model.MaterialReceiveds.ItemID,
            StockID = existingStock.StockID,
            StockIn = model.MaterialReceiveds.Quantity
          };

          _appDBContext.ST_ItemLedgers.Add(newItemLedger);

          var newMaterialReceived = new ST_MaterialReceived
          {
            ItemID = model.MaterialReceiveds.ItemID,
            VendorID = model.MaterialReceiveds.VendorID,
            Quantity = model.MaterialReceiveds.Quantity,
            UnitTypeID = model.MaterialReceiveds.UnitTypeID,
            PONo = model.MaterialReceiveds.PONo,
            GRNNo = model.MaterialReceiveds.GRNNo,
            DCNo = model.MaterialReceiveds.DCNo,
            InvoiceNo = model.MaterialReceiveds.InvoiceNo,
            MaterialReceivedDate = DateTime.Now
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newMaterialReceived.LotNumber = model.MaterialReceiveds.LotNumber;
            newMaterialReceived.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          // Add MaterialReceived components
          if (model.MaterialReceiveds.MaterialReceivedComponents != null && model.MaterialReceiveds.MaterialReceivedComponents.Any())
          {
            foreach (var component in model.MaterialReceiveds.MaterialReceivedComponents)
            {
              if (!string.IsNullOrWhiteSpace(component.ItemComponentValue))
              {
                newMaterialReceived.MaterialReceivedComponents.Add(new ST_MaterialReceivedComponent
                {
                  ItemComponentTypeID = component.ItemComponentTypeID,
                  ItemComponentValue = component.ItemComponentValue
                });
              }
            }
          }

          _appDBContext.ST_MaterialReceiveds.Add(newMaterialReceived);
        }
        else
        {
          // Create new stock
          var newStock = new ST_Stock
          {
            ItemID = model.MaterialReceiveds.ItemID,
            VendorID = model.MaterialReceiveds.VendorID,
            Quantity = model.MaterialReceiveds.Quantity,
            UnitTypeID = model.MaterialReceiveds.UnitTypeID,
            PONo = model.MaterialReceiveds.PONo,
            GRNNo = model.MaterialReceiveds.GRNNo,
            DCNo = model.MaterialReceiveds.DCNo,
            InvoiceNo = model.MaterialReceiveds.InvoiceNo,
            StockDate = DateTime.Now
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStock.LotNumber = model.MaterialReceiveds.LotNumber;
            newStock.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          

          _appDBContext.ST_Stocks.Add(newStock);
          await _appDBContext.SaveChangesAsync();
          var newStockHistory = new ST_StockHistory
          {
            ItemID = model.MaterialReceiveds.ItemID,
            VendorID = model.MaterialReceiveds.VendorID,
            Quantity = model.MaterialReceiveds.Quantity,
            UnitTypeID = model.MaterialReceiveds.UnitTypeID,
            PONo = model.MaterialReceiveds.PONo,
            GRNNo = model.MaterialReceiveds.GRNNo,
            DCNo = model.MaterialReceiveds.DCNo,
            InvoiceNo = model.MaterialReceiveds.InvoiceNo,
            StockDate = DateTime.Now,
            StockID = newStock.StockID,
            ActionType = "Insert",
            ActionDate = DateTime.Now,
            ActionUserID = HttpContext.Session.GetInt32("UserID")
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newStockHistory.LotNumber = model.MaterialReceiveds.LotNumber;
            newStockHistory.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          _appDBContext.ST_StockHistorys.Add(newStockHistory);

          var newItemLedger = new ST_ItemLedger
          {
            ItemLedgerDate = DateTime.Now,
            ItemID = model.MaterialReceiveds.ItemID,
            StockID = newStock.StockID,
            StockIn = model.MaterialReceiveds.Quantity
          };

          _appDBContext.ST_ItemLedgers.Add(newItemLedger);

          var newMaterialReceived = new ST_MaterialReceived
          {
            ItemID = model.MaterialReceiveds.ItemID,
            VendorID = model.MaterialReceiveds.VendorID,
            Quantity = model.MaterialReceiveds.Quantity,
            UnitTypeID = model.MaterialReceiveds.UnitTypeID,
            PONo = model.MaterialReceiveds.PONo,
            GRNNo = model.MaterialReceiveds.GRNNo,
            DCNo = model.MaterialReceiveds.DCNo,
            InvoiceNo = model.MaterialReceiveds.InvoiceNo,
            MaterialReceivedDate = DateTime.Now
          };

          // Conditionally set LotNumber and ExpiryDate
          if (itemDetails.HasLotNumberAndExpiryDate)
          {
            newMaterialReceived.LotNumber = model.MaterialReceiveds.LotNumber;
            newMaterialReceived.ExpiryDate = model.MaterialReceiveds.ExpiryDate;
          }

          // Add MaterialReceived components
          if (model.MaterialReceiveds.MaterialReceivedComponents != null && model.MaterialReceiveds.MaterialReceivedComponents.Any())
          {
            foreach (var component in model.MaterialReceiveds.MaterialReceivedComponents)
            {
              if (!string.IsNullOrWhiteSpace(component.ItemComponentValue))
              {
                newMaterialReceived.MaterialReceivedComponents.Add(new ST_MaterialReceivedComponent
                {
                  ItemComponentTypeID = component.ItemComponentTypeID,
                  ItemComponentValue = component.ItemComponentValue
                });
              }
            }
          }

           _appDBContext.ST_MaterialReceiveds.Add(newMaterialReceived);

        }
        

        // Save changes to the database
        await _appDBContext.SaveChangesAsync();

        return Json(new { success = true, message = "MaterialReceived saved successfully!" });


      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = "An error occurred while adding the MaterialReceived. Please try again later.", error = ex.Message });
      }
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var MaterialReceiveds = await _appDBContext.ST_MaterialReceiveds
          .Include(m => m.MaterialReceivedComponents) // Include related components
          .FirstOrDefaultAsync(v => v.MaterialReceivedID == id);

      if (MaterialReceiveds == null)
      {
        return NotFound();
      }

      // Load dropdown lists
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.VendorList = await _utils.GetVendorList();
      ViewBag.UnitList = await _utils.GetItemUnits();

      // Prepare the ViewModel
      var model = new MaterialReceivedsIndexViewModel
      {
        MaterialReceiveds = MaterialReceiveds
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialReceived/EditMaterialReceived.cshtml", model);
    }

    public async Task<IActionResult> Print()
    {
      var MaterialReceivedsQuery = _appDBContext.ST_MaterialReceiveds.Include(m => m.Item).AsQueryable();
      var MaterialReceiveds = await MaterialReceivedsQuery.ToListAsync();
      return View("~/Views/StoreManagement/StoreManagement/MaterialReceived/PrintMaterialReceived.cshtml", MaterialReceiveds);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var MaterialReceivedsQuery = _appDBContext.ST_MaterialReceiveds.Include(m => m.Item).AsQueryable();
      var MaterialReceiveds = await MaterialReceivedsQuery.ToListAsync();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("MaterialReceiveds");
        worksheet.Cells["A1"].Value = "Received #";
        worksheet.Cells["B1"].Value = "Received Date";
        worksheet.Cells["C1"].Value = "Item Name";
        worksheet.Cells["D1"].Value = "Quantity";

        for (int i = 0; i < MaterialReceiveds.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = MaterialReceiveds[i].MaterialReceivedID;
          worksheet.Cells[i + 2, 2].Value = MaterialReceiveds[i].MaterialReceivedDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = MaterialReceiveds[i].Item?.ItemName;
          worksheet.Cells[i + 2, 4].Value = MaterialReceiveds[i].Quantity;


        }

        worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"MaterialReceiveds-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
