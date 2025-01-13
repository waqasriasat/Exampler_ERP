using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialRequisitionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public MaterialRequisitionController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
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
        var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .Include(v => v.RequisitionStatusTypes)
          .Include(v => v.HR_Employees)
          .Where(v => v.HR_Employees.EmployeeID == EmployeeID);

        if (!string.IsNullOrEmpty(searchItemName))
        {
          MaterialRequisitionsQuery = MaterialRequisitionsQuery
              .Where(v => v.MaterialRequisitionDetails
                  .Any(d => d.Items.ItemName
                      .Contains(searchItemName)));
        }

        var MaterialRequisitions = await MaterialRequisitionsQuery.ToListAsync();

        if (!string.IsNullOrEmpty(searchItemName) && MaterialRequisitions.Count == 0)
        {
          TempData["ErrorMessage"] = "No Material Requisition found with the name '" + searchItemName + "'. Please check the name and try again.";
        }

        return View("~/Views/StoreManagement/StoreManagement/MaterialRequisition/MaterialRequisition.cshtml", MaterialRequisitions);
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

      ST_MaterialRequisition MaterialRequisitions = new ST_MaterialRequisition();
      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail() { RequisitionID = 0 });
      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/AddMaterialRequisition.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(MaterialRequisitionsIndexViewModel model)
    {
      if (ModelState.IsValid)
      {
        try
        {
          

          model.MaterialRequisitions.FinalApprovalID = 0;
          model.MaterialRequisitions.RequisitionDate = DateTime.Now;
          _appDBContext.ST_MaterialRequisitions.Add(model.MaterialRequisitions);


          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == 0);
          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in model.MaterialRequisitions.MaterialRequisitionDetails)
          {
            detail.RequisitionID = model.MaterialRequisitions.RequisitionID;
            _appDBContext.ST_MaterialRequisitionDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          var MaterialRequisitionId = model.MaterialRequisitions.RequisitionID;
          if (MaterialRequisitionId > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 19)
                                .CountAsync();

            if (processCount > 0)
            {
              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 19,
                Notes = "Pending New Material Requisition",
                Date = DateTime.Now,
                EmployeeID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = MaterialRequisitionId
              };

              _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
              await _appDBContext.SaveChangesAsync();

              var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                          .Where(pta => pta.ProcessTypeID == 19 && pta.Rank == 1)
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
              model.MaterialRequisitions.FinalApprovalID = 1;
              _appDBContext.ST_MaterialRequisitions.Update(model.MaterialRequisitions);
              await _appDBContext.SaveChangesAsync();
              TempData["SuccessMessage"] = " Material Requisition successfully. No process setup found,  Material Requisition Approved.";
              return Json(new { success = true, message = "No process setup found,  Material Requisition Approved." });
            }
          }
          TempData["SuccessMessage"] = " Material Requisition Created successfully. Continue to the Approval Process Setup for  Material Requisition Approved.";

          return Json(new { success = true });
        }
        catch (Exception ex)
        {
          return Json(new { success = false, message = "Error: " + ex.Message });
        }

      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      if (model.MaterialRequisitions.MaterialRequisitionDetails == null || !model.MaterialRequisitions.MaterialRequisitionDetails.Any())
      {
        model.MaterialRequisitions.MaterialRequisitionDetails = new List<ST_MaterialRequisitionDetail> { new ST_MaterialRequisitionDetail() };
      }

      if (model.MaterialRequisitions == null)
      {
        model.MaterialRequisitions = new ST_MaterialRequisition();
      }

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/AddMaterialRequisition.cshtml", model);
    }
  }
}
