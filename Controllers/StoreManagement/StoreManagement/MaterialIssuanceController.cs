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
      if (HttpContext.Session.GetInt32("UserID") != null)
      {
        int EmployeeID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
        var MaterialIssuancesQuery = _appDBContext.ST_MaterialIssuances
          .Include(v => v.MaterialIssuanceDetails)
          .Include(v => v.IssuanceStatusTypes)
          .Include(v => v.HR_Employees)
          .Where(v => v.EmployeeID == EmployeeID);

        if (!string.IsNullOrEmpty(searchItemName))
        {
          MaterialIssuancesQuery = MaterialIssuancesQuery
              .Where(v => v.MaterialIssuanceDetails
                  .Any(d => d.Items.ItemName
                      .Contains(searchItemName)));
        }

        var MaterialIssuances = await MaterialIssuancesQuery.ToListAsync();

        if (!string.IsNullOrEmpty(searchItemName) && MaterialIssuances.Count == 0)
        {
          TempData["ErrorMessage"] = "No Material Issuance found with the name '" + searchItemName + "'. Please check the name and try again.";
        }

        return View("~/Views/StoreManagement/StoreManagement/MaterialIssuance/MaterialIssuance.cshtml", MaterialIssuances);
      }
      else
      {
        return RedirectToAction("Login", "Auth");
      }
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
    public async Task<IActionResult> GetRequisitionDetails(int requisitionId1)
    {
      try
      {
        // Database se data fetch karna
        var requisitionDetails = await _appDBContext.ST_MaterialRequisitionDetails
          .Include(r => r.Items)
            .Where(r => r.RequisitionID == requisitionId1)
            .Select(r => new
            {
              ItemID = r.ItemID,
              itemName = r.Items.ItemName,
              Quantity = r.Quantity
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
    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public async Task<IActionResult> Create(MaterialIssuanceCreateViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    var issuance = new ST_MaterialIssuance
    //    {
    //      IssuanceDate = model.IssuanceDate,
    //      RequisitionID = model.RequisitionID,
    //      EmployeeID = model.EmployeeID,
    //      Remarks = model.Remarks,
    //      IssuanceStatusTypeID = model.IssuanceStatusTypeID,
    //      MaterialIssuanceDetails = model.Details.Select(d => new ST_MaterialIssuanceDetail
    //      {
    //        ItemID = d.ItemID,
    //        RequisitionQuantity = d.RequisitionQuantity,
    //        IssuanceQuantity = d.IssuanceQuantity,
    //        BalanceQuantity = d.RequisitionQuantity - d.IssuanceQuantity,
    //        Remarks = d.Remarks
    //      }).ToList()
    //    };

    //    _appDBContext.ST_MaterialIssuances.Add(issuance);
    //    await _appDBContext.SaveChangesAsync();
    //    return RedirectToAction("Index"); // Redirect to index or any other page
    //  }

    //  return View(model);
    //}
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
