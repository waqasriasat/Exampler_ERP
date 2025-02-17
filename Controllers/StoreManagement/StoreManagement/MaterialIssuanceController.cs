using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Exampler_ERP.Utilities;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialIssuanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public MaterialIssuanceController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var MaterialIssuancesQuery = _appDBContext.ST_MaterialIssuances
        .Include(v => v.IssuanceStatusTypes)
        .Include(v => v.MaterialIssuanceDetails)
            .ThenInclude(d => d.Items) // Ensure Items is included
        .AsQueryable(); // ✅ Ensure it's a queryable object

      if (!string.IsNullOrEmpty(searchItemName))
      {
        MaterialIssuancesQuery = MaterialIssuancesQuery
            .Where(v => v.MaterialIssuanceDetails
                .Any(d => d.Items != null && d.Items.ItemName.Contains(searchItemName))); // ✅ Null check added
      }

      var MaterialIssuances = await MaterialIssuancesQuery.ToListAsync();



      if (!string.IsNullOrEmpty(searchItemName) && MaterialIssuances.Count == 0)
        {
          TempData["ErrorMessage"] = "No Material Issuance found with the name '" + searchItemName + "'. Please check the name and try again.";
        }

        return View("~/Views/StoreManagement/StoreManagement/MaterialIssuance/MaterialIssuance.cshtml", MaterialIssuances);
      
    }
    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.PendingRequisitionsList = await _utils.GetPendingRequisitions();
      ST_MaterialIssuance Issuances = new ST_MaterialIssuance();
      Issuances.MaterialIssuanceDetails.Add(new ST_MaterialIssuanceDetail() { IssuanceID = 0 });
      var model = new MaterialIssuancesIndexViewModel
      {
        MaterialIssuances = Issuances
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialIssuance/AddMaterialIssuance.cshtml", model);
    }

    [HttpGet]
    public async Task<IActionResult> GetRequisitionDetails(int requisitionId)
    {
      try
      {

        //var requisitionDetails = await (from req in _appDBContext.ST_MaterialRequisitionDetails
        //                                join stock in _appDBContext.ST_Stocks
        //                                on req.ItemID equals stock.ItemID into stockGroup
        //                                where req.RequisitionID == requisitionId
        //                                select new
        //                                {
        //                                  ItemID = req.ItemID,
        //                                  ItemName = req.Items.ItemName,
        //                                  Quantity = req.Quantity - (
        //                                _appDBContext.ST_MaterialIssuances
        //                                    .Where(iss => iss.RequisitionID == req.RequisitionID)
        //                                    .SelectMany(iss => iss.MaterialIssuanceDetails)
        //                                    .Where(detail => detail.ItemID == req.ItemID)
        //                                    .Sum(detail => (int?)detail.IssuanceQuantity) ?? 0
        //                            ),
        //                                  AvailableStock = stockGroup.Sum(s => s.Quantity) // Total stock quantity
        //                                })
        //                        .ToListAsync();
        var requisitionDetails = await (from req in _appDBContext.ST_MaterialRequisitionDetails
                                        join stock in _appDBContext.ST_Stocks
                                            on req.ItemID equals stock.ItemID into stockGroup
                                        where req.RequisitionID == requisitionId
                                              && req.Quantity > 0 // Exclude requisitions with Quantity = 0
                                        select new
                                        {
                                          ItemID = req.ItemID,
                                          ItemName = req.Items.ItemName,
                                          Quantity = req.Quantity - (
                                                _appDBContext.ST_MaterialIssuances
                                                    .Where(iss => iss.RequisitionID == req.RequisitionID)
                                                    .SelectMany(iss => iss.MaterialIssuanceDetails)
                                                    .Where(detail => detail.ItemID == req.ItemID)
                                                    .Sum(detail => (int?)detail.IssuanceQuantity) ?? 0
                                            ),
                                          AvailableStock = stockGroup.Sum(s => s.Quantity) // Total stock quantity
                                        })
                         .Where(x => x.Quantity > 0) // Optional: Exclude if remaining quantity is 0
                         .ToListAsync();



        // Agar data na mile
        if (requisitionDetails == null || !requisitionDetails.Any())
        {
          return Json(new { success = false, message = "No data found for the given Requisition ID." });
        }

        return Json(new { success = true, data = requisitionDetails });
      }
      catch (Exception ex)
      {
        // Error handling
        return Json(new { success = false, message = ex.Message });
      }
    }


    // POST: Create MaterialIssuance
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaterialIssuancesIndexViewModel model)
    {
      if (ModelState.IsValid)
      {
        model.MaterialIssuances.MaterialIssuanceDetails.RemoveAll(e => e.IssuanceQuantity == null || e.IssuanceQuantity == 0);
        var issuance = new ST_MaterialIssuance
        {
          IssuanceDate = DateTime.Now,
          RequisitionID = model.MaterialIssuances.RequisitionID,
          DepartmentTypeID = model.MaterialIssuances.DepartmentTypeID,
          Remarks = model.MaterialIssuances.Remarks,
          IssuanceStatusTypeID = model.MaterialIssuances.IssuanceStatusTypeID,
          
          MaterialIssuanceDetails = model.MaterialIssuances.MaterialIssuanceDetails.Select(d => new ST_MaterialIssuanceDetail
          {
            ItemID = d.ItemID,
            RequisitionQuantity = d.RequisitionQuantity,
            IssuanceQuantity = d.IssuanceQuantity,
            BalanceQuantity = d.RequisitionQuantity - d.IssuanceQuantity,
            IssuanceStatusTypeID = d.IssuanceQuantity != 0
        ? (d.RequisitionQuantity - d.IssuanceQuantity == 0 ? 4 : 3)
        : 1,
            Remarks = d.Remarks
          }).ToList()
        };
        _appDBContext.ST_MaterialIssuances.Add(issuance);

        await _appDBContext.SaveChangesAsync();

        //bool allCompleted = issuance.MaterialIssuanceDetails.All(d => d.IssuanceStatusTypeID == 4);
        //int actionStatusTypeID = allCompleted ? 4 : 3;
        // 1️⃣ Get total requisition quantities from ST_MaterialRequisitionDetail
        // 1️⃣ Get total requisition quantities from ST_MaterialRequisitionDetails
        var requisitionQuantities = _appDBContext.ST_MaterialRequisitionDetails
            .Where(r => r.RequisitionID == issuance.RequisitionID)
            .GroupBy(r => r.ItemID)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(r => r.Quantity)
            );

        // 2️⃣ Get total issuance quantities from MaterialIssuanceDetails
        var issuanceQuantities = _appDBContext.ST_MaterialIssuanceDetails
            .Where(i => i.MaterialIssuances.RequisitionID == issuance.RequisitionID)
            .GroupBy(i => i.ItemID)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.IssuanceQuantity)
            );

        // 3️⃣ Initialize flags for checking statuses
        bool allNoIssuance = false; // Start as false (will be true if any item has no issuance)
        bool allFullyIssued = true; // Start as true (will be false if any item is partially issued)

        foreach (var itemId in requisitionQuantities.Keys)
        {
          var requisitionQty = requisitionQuantities[itemId];
          issuanceQuantities.TryGetValue(itemId, out var issuedQty);

          if (issuedQty == 0)
          {
            allNoIssuance = true; // Found an item with no issuance
            break; // If one item has no issuance, we can stop and set actionStatusTypeID to 3
          }
          else if (issuedQty < requisitionQty)
          {
            allFullyIssued = false; // Found an item with partial issuance
          }
          else if (issuedQty == requisitionQty)
          {
            // Fully issued, no change needed for `allFullyIssued`
          }
        }

        // 4️⃣ Set Action Status
        int actionStatusTypeID = allNoIssuance ? 3 : (allFullyIssued ? 4 : 3);

        // **Insert into ST_MaterialRequisitionStatus Table**
        var requisitionStatus = new ST_MaterialRequisitionStatus
        {
          RequisitionID = issuance.RequisitionID,
          ActionID = 2, // Example: 'Issued' action
          ActionDate = DateTime.Now,
          ActionStatusTypeID = actionStatusTypeID
        };
        _appDBContext.ST_MaterialRequisitionStatuss.Add(requisitionStatus);
        await _appDBContext.SaveChangesAsync();

        var requisition = await _appDBContext.ST_MaterialRequisitions
            .FirstOrDefaultAsync(r => r.RequisitionID == issuance.RequisitionID);

        if (requisition != null)
        {
          requisition.RequisitionStatusTypeID = actionStatusTypeID;
          _appDBContext.ST_MaterialRequisitions.Update(requisition);
          await _appDBContext.SaveChangesAsync();
        }

        var issuancestatus = await _appDBContext.ST_MaterialIssuances
           .FirstOrDefaultAsync(r => r.IssuanceID == issuance.IssuanceID);

        if (issuancestatus != null)
        {
          issuancestatus.IssuanceStatusTypeID = actionStatusTypeID;
          _appDBContext.ST_MaterialIssuances.Update(issuancestatus);
          await _appDBContext.SaveChangesAsync();
        }

        // Create Item Ledger Entries for Each Issued Item
        foreach (var detail in issuance.MaterialIssuanceDetails)
        {
          // 1. Update Stock Quantity
          var stock = await _appDBContext.ST_Stocks
              .FirstOrDefaultAsync(s => s.ItemID == detail.ItemID);

          if (stock != null)
          {
            if (stock.Quantity >= detail.IssuanceQuantity)
            {
              stock.Quantity -= detail.IssuanceQuantity; // Deduct the issuance quantity
            }
            else
            {
              return Json(new { success = false, message = $"Insufficient stock for Item ID {detail.ItemID}." });
            }
          }
          else
          {
            return Json(new { success = false, message = $"Stock not found for Item ID {detail.ItemID}." });
          }

          // 2. Create Item Ledger Entry
          var itemLedger = new ST_ItemLedger
          {
            ItemLedgerDate = DateTime.Now,
            ItemID = detail.ItemID,
            IssuanceID = issuance.IssuanceID,
            StockOut = detail.IssuanceQuantity
          };
          _appDBContext.ST_ItemLedgers.Add(itemLedger);
        }

        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialIssuance/AddMaterialIssuance.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var MaterialIssuances = await _appDBContext.ST_MaterialIssuances
          .Include(v => v.MaterialIssuanceDetails)
          .FirstOrDefaultAsync(v => v.IssuanceID == id);

      if (MaterialIssuances == null)
      {
        return NotFound();
      }

      MaterialIssuances.MaterialIssuanceDetails.Add(new ST_MaterialIssuanceDetail() { IssuanceID = MaterialIssuances.IssuanceID });

      var model = new MaterialIssuancesIndexViewModel
      {
        MaterialIssuances = MaterialIssuances
      };

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialIssuance/EditMaterialIssuance.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Edit(MaterialIssuancesIndexViewModel MaterialIssuance)
    {
      if (ModelState.IsValid)
      {

        var existingMaterialIssuance = await _appDBContext.ST_MaterialIssuances
            .Include(v => v.MaterialIssuanceDetails)
            .FirstOrDefaultAsync(v => v.IssuanceID == MaterialIssuance.MaterialIssuances.IssuanceID);

        if (existingMaterialIssuance != null)
        {
          existingMaterialIssuance.Remarks = MaterialIssuance.MaterialIssuances.Remarks;

          _appDBContext.Update(existingMaterialIssuance);
          await _appDBContext.SaveChangesAsync();

          var MaterialIssuanceDetailsToRemove = _appDBContext.ST_MaterialIssuanceDetails
          .Where(v => v.IssuanceID == MaterialIssuance.MaterialIssuances.IssuanceID)
          .ToList();

          _appDBContext.ST_MaterialIssuanceDetails.RemoveRange(MaterialIssuanceDetailsToRemove);

          await _appDBContext.SaveChangesAsync();
          MaterialIssuance.MaterialIssuances.MaterialIssuanceDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in MaterialIssuance.MaterialIssuances.MaterialIssuanceDetails)
          {
            detail.IssuanceID = MaterialIssuance.MaterialIssuances.IssuanceID;
            _appDBContext.ST_MaterialIssuanceDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          return Json(new { success = true, message = "Received MaterialIssuance Edited successfully!" });
        }
        else
        {
          return NotFound();
        }
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialIssuance/EditMaterialIssuance.cshtml", MaterialIssuance);
    }
  }
}
