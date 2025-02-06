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
    .Include(v => v.MaterialIssuanceDetails)
        .ThenInclude(d => d.Items) // Ensure Items is included
    .Include(v => v.IssuanceStatusTypes)
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
        // Database se data fetch karna
        //var requisitionDetails = await _appDBContext.ST_MaterialRequisitionDetails
        //  .Include(r => r.Items)
        //    .Where(r => r.RequisitionID == requisitionId)
        //    .Select(r => new
        //    {
        //      ItemID = r.ItemID,
        //      itemName = r.Items.ItemName,
        //      Quantity = r.Quantity,
        //      AvaiableStock = r.Items.
        //    })
        //    .ToListAsync();
        var requisitionDetails = await (from req in _appDBContext.ST_MaterialRequisitionDetails
                                        join stock in _appDBContext.ST_Stocks
                                        on req.ItemID equals stock.ItemID into stockGroup
                                        where req.RequisitionID == requisitionId
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

        // Create Item Ledger Entries for Each Issued Item
        foreach (var detail in issuance.MaterialIssuanceDetails)
        {
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
