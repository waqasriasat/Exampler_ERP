using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

      ST_MaterialIssuance Issuances = new ST_MaterialIssuance();
      Issuances.MaterialIssuanceDetails.Add(new ST_MaterialIssuanceDetail() { IssuanceID = 0 });
      var model = new MaterialIssuancesIndexViewModel
      {
        MaterialIssuances = Issuances
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialIssuance/AddMaterialIssuance.cshtml", model);
    }


    [HttpPost]
    public async Task<IActionResult> Create(MaterialIssuancesIndexViewModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {

          model.MaterialIssuances.EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int);
          model.MaterialIssuances.FinalApprovalID = 0;
          model.MaterialIssuances.IssuanceStatusTypeID = 1;
          model.MaterialIssuances.IssuanceDate = DateTime.Now;
          _appDBContext.ST_MaterialIssuances.Add(model.MaterialIssuances);
          //await _appDBContext.SaveChangesAsync();

          model.MaterialIssuances.MaterialIssuanceDetails.RemoveAll(e => e.ItemID == 0);
          model.MaterialIssuances.MaterialIssuanceDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in model.MaterialIssuances.MaterialIssuanceDetails)
          {
            detail.IssuanceID = model.MaterialIssuances.IssuanceID;
            _appDBContext.ST_MaterialIssuanceDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          var IssuanceID = model.MaterialIssuances.IssuanceID;

          var MaterialRequisitionStatus = new ST_MaterialRequisitionStatus
          {
            RequisitionID = IssuanceID,
            ActionDate = DateTime.Now,
            ActionID = HttpContext.Session.GetInt32("UserID") ?? default(int),
            ActionStatusTypeID = 1
          };

          _appDBContext.ST_MaterialRequisitionStatuss.Add(MaterialRequisitionStatus);

          await _appDBContext.SaveChangesAsync();


          if (IssuanceID > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 20)
                                .CountAsync();

            if (processCount > 0)
            {
              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 20,
                Notes = "Pending New Material Issuance",
                Date = DateTime.Now,
                EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = IssuanceID
              };

              _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
              await _appDBContext.SaveChangesAsync();

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                          .Where(pta => pta.ProcessTypeID == 20 && pta.Rank == 1)
                                          .FirstOrDefaultAsync();

              if (nextApprovalSetup != null)
              {
                var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
                {
                  ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                  Date = DateTime.Now,
                  RoleID = nextApprovalSetup.RoleTypeID,
                  AppID = 0,
                  AppUserID = 0,
                  Notes = null,
                  Rank = nextApprovalSetup.Rank
                };

                _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
                await _appDBContext.SaveChangesAsync();
              }
              else
              {
                return Json(new { success = false, message = "Next approval setup not found." });
              }
            }
            else
            {
              model.MaterialIssuances.FinalApprovalID = 1;
              model.MaterialIssuances.IssuanceStatusTypeID = 2;
              _appDBContext.ST_MaterialIssuances.Update(model.MaterialIssuances);

              MaterialRequisitionStatus.ActionStatusTypeID = 2;
              _appDBContext.ST_MaterialRequisitionStatuss.Update(MaterialRequisitionStatus);

              await _appDBContext.SaveChangesAsync();
              TempData["SuccessMessage"] = " Material Issuance successfully. No process setup found,  Material Issuance Approved.";
              return Json(new { success = true, message = "No process setup found,  Material Issuance Approved." });
            }
          }
          TempData["SuccessMessage"] = " Material Issuance Created successfully. Continue to the Approval Process Setup for  Material Issuance Approved.";

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, message = "Error: " + ex.Message });
        }

      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      if (model.MaterialIssuances.MaterialIssuanceDetails == null || !model.MaterialIssuances.MaterialIssuanceDetails.Any())
      {
        model.MaterialIssuances.MaterialIssuanceDetails = new List<ST_MaterialIssuanceDetail> { new ST_MaterialIssuanceDetail() };
      }

      if (model.MaterialIssuances == null)
      {
        model.MaterialIssuances = new ST_MaterialIssuance();
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
