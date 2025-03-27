using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class PurchaseRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public PurchaseRequestController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.PurchaseRequestDetails)
              .ThenInclude(d => d.Item) // Ensure Item is included to avoid null reference issues
          .Include(v => v.RequestStatusTypes)
          .AsQueryable(); // Ensure efficient query building

      if (!string.IsNullOrEmpty(searchItemName))
      {
        PurchaseRequestsQuery = PurchaseRequestsQuery
            .Where(v => v.PurchaseRequestDetails
                .Any(d => d.Item != null && d.Item.ItemName.Contains(searchItemName))); // Null check added
      }

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && PurchaseRequests.Count == 0)
      {
        TempData["ErrorMessage"] = $"No Purchase Request found with the name '{searchItemName}'. Please check the name and try again.";
      }

      return View("~/Views/Purchase/Purchase/PurchaseRequest/PurchaseRequest.cshtml", PurchaseRequests);
    }

    //[HttpGet]
    //public async Task<IActionResult> Create()
    //{
    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  PR_PurchaseRequest Requisitions = new PR_PurchaseRequest();
    //  Requisitions.PurchaseRequestDetails.Add(new PR_PurchaseRequestDetail() { RequisitionID = 0 });
    //  var model = new PurchaseRequestsIndexViewModel
    //  {
    //    PurchaseRequests = Requisitions
    //  };

    //  return PartialView("~/Views/Purchase/Purchase/PurchaseRequest/AddPurchaseRequest.cshtml", model);
    //}
    //[HttpPost]
    //public async Task<IActionResult> Create(PurchaseRequestsIndexViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    try
    //    {
    //      int userID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //      int departmentID = await _utils.PostUserIDGetDepartmentID(userID);
    //      model.PurchaseRequests.DepartmentTypeID = departmentID;
    //      model.PurchaseRequests.FinalApprovalID = 0;
    //      model.PurchaseRequests.RequisitionStatusTypeID = 1;
    //      model.PurchaseRequests.RequisitionDate = DateTime.Now;
    //      _appDBContext.PR_PurchaseRequests.Add(model.PurchaseRequests);
    //      //await _appDBContext.SaveChangesAsync();

    //      model.PurchaseRequests.PurchaseRequestDetails.RemoveAll(e => e.ItemID == 0);
    //      model.PurchaseRequests.PurchaseRequestDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

    //      foreach (var detail in model.PurchaseRequests.PurchaseRequestDetails)
    //      {
    //        detail.RequisitionID = model.PurchaseRequests.RequisitionID;
    //        _appDBContext.PR_PurchaseRequestDetails.Add(detail);
    //      }

    //      await _appDBContext.SaveChangesAsync();

    //      var RequisitionID = model.PurchaseRequests.RequisitionID;

    //      var PurchaseRequestStatus = new PR_PurchaseRequestStatus
    //      {
    //        RequisitionID = RequisitionID,
    //        ActionDate = DateTime.Now,
    //        ActionID = HttpContext.Session.GetInt32("UserID") ?? default(int),
    //        ActionStatusTypeID = 1
    //      };

    //      _appDBContext.PR_PurchaseRequestStatuss.Add(PurchaseRequestStatus);

    //      await _appDBContext.SaveChangesAsync();


    //      if (RequisitionID > 0)
    //      {
    //        var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
    //                            .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 20)
    //                            .CountAsync();

    //        if (processCount > 0)
    //        {

    //          var newProcessTypeApproval = new CR_ProcessTypeApproval
    //          {
    //            ProcessTypeID = 20,
    //            Notes = "Pending New Purchase Request",
    //            Date = DateTime.Now,
    //            EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
    //            UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
    //            TransactionID = RequisitionID
    //          };

    //          _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
    //          await _appDBContext.SaveChangesAsync();

    //          var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
    //                                      .Where(pta => pta.ProcessTypeID == 20 && pta.Rank == 1)
    //                                      .FirstOrDefaultAsync();

    //          if (nextApprovalSetup != null)
    //          {
    //            var newProcessTypeApprovalDetail = new CR_ProcessTypeApprovalDetail
    //            {
    //              ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
    //              Date = DateTime.Now,
    //              RoleID = nextApprovalSetup.RoleTypeID,
    //              AppID = 0,
    //              AppUserID = 0,
    //              Notes = null,
    //              Rank = nextApprovalSetup.Rank
    //            };

    //            _appDBContext.CR_ProcessTypeApprovalDetails.Add(newProcessTypeApprovalDetail);
    //            await _appDBContext.SaveChangesAsync();
    //          }
    //          else
    //          {
    //            return Json(new { success = false, message = "Next approval setup not found." });
    //          }
    //        }
    //        else
    //        {
    //          model.PurchaseRequests.FinalApprovalID = 1;
    //          model.PurchaseRequests.RequisitionStatusTypeID = 2;
    //          _appDBContext.PR_PurchaseRequests.Update(model.PurchaseRequests);

    //          PurchaseRequestStatus.ActionStatusTypeID = 2;
    //          _appDBContext.PR_PurchaseRequestStatuss.Update(PurchaseRequestStatus);

    //          await _appDBContext.SaveChangesAsync();
    //          TempData["SuccessMessage"] = " Purchase Request successfully. No process setup found,  Purchase Request Approved.";
    //          return Json(new { success = true, message = "No process setup found,  Purchase Request Approved." });
    //        }
    //      }
    //      TempData["SuccessMessage"] = " Purchase Request Created successfully. Continue to the Approval Process Setup for  Purchase Request Approved.";

    //      return Json(new { success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //      return Json(new { success = false, message = "Error: " + ex.Message });
    //    }

    //  }

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  if (model.PurchaseRequests.PurchaseRequestDetails == null || !model.PurchaseRequests.PurchaseRequestDetails.Any())
    //  {
    //    model.PurchaseRequests.PurchaseRequestDetails = new List<PR_PurchaseRequestDetail> { new PR_PurchaseRequestDetail() };
    //  }

    //  if (model.PurchaseRequests == null)
    //  {
    //    model.PurchaseRequests = new PR_PurchaseRequest();
    //  }

    //  return PartialView("~/Views/Purchase/Purchase/PurchaseRequest/AddPurchaseRequest.cshtml", model);
    //}
    //[HttpGet]
    //public async Task<IActionResult> Edit(int id)
    //{
    //  var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
    //      .Include(v => v.PurchaseRequestDetails)
    //      .FirstOrDefaultAsync(v => v.RequisitionID == id);

    //  if (PurchaseRequests == null)
    //  {
    //    return NotFound();
    //  }

    //  // Check RequisitionStatusTypeID
    //  if (PurchaseRequests.RequisitionStatusTypeID != 1)
    //  {
    //    TempData["ErrorMessage"] = "After approval, editing is not allowed.....";

    //  }

    //  PurchaseRequests.PurchaseRequestDetails.Add(new PR_PurchaseRequestDetail()
    //  {
    //    RequisitionID = PurchaseRequests.RequisitionID
    //  });

    //  var model = new PurchaseRequestsIndexViewModel
    //  {
    //    PurchaseRequests = PurchaseRequests
    //  };

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return PartialView("~/Views/Purchase/Purchase/PurchaseRequest/EditPurchaseRequest.cshtml", model);
    //}
    //[HttpPost]
    //public async Task<IActionResult> Edit(PurchaseRequestsIndexViewModel PurchaseRequest)
    //{
    //  if (ModelState.IsValid)
    //  {

    //    var existingPurchaseRequest = await _appDBContext.PR_PurchaseRequests
    //        .Include(v => v.PurchaseRequestDetails)
    //        .FirstOrDefaultAsync(v => v.RequisitionID == PurchaseRequest.PurchaseRequests.RequisitionID);
    //    if (existingPurchaseRequest?.RequisitionStatusTypeID != 1)
    //    {
    //      TempData["ErrorMessage"] = "After approval, editing is not allowed.....";

    //    }
    //    else
    //    {
    //      if (existingPurchaseRequest != null)
    //      {
    //        existingPurchaseRequest.Remarks = PurchaseRequest.PurchaseRequests.Remarks;

    //        _appDBContext.Update(existingPurchaseRequest);
    //        await _appDBContext.SaveChangesAsync();

    //        var PurchaseRequestDetailsToRemove = _appDBContext.PR_PurchaseRequestDetails
    //        .Where(v => v.RequisitionID == PurchaseRequest.PurchaseRequests.RequisitionID)
    //        .ToList();

    //        _appDBContext.PR_PurchaseRequestDetails.RemoveRange(PurchaseRequestDetailsToRemove);

    //        await _appDBContext.SaveChangesAsync();
    //        PurchaseRequest.PurchaseRequests.PurchaseRequestDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

    //        foreach (var detail in PurchaseRequest.PurchaseRequests.PurchaseRequestDetails)
    //        {
    //          detail.RequisitionID = PurchaseRequest.PurchaseRequests.RequisitionID;
    //          _appDBContext.PR_PurchaseRequestDetails.Add(detail);
    //        }

    //        await _appDBContext.SaveChangesAsync();

    //        return Json(new { success = true, message = "Received PurchaseRequest Edited successfully!" });
    //      }
    //      else
    //      {
    //        return NotFound();
    //      }
    //    }
    //  }

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return PartialView("~/Views/Purchase/Purchase/PurchaseRequest/EditPurchaseRequest.cshtml", PurchaseRequest);
    //}
    //public async Task<IActionResult> Print(int id)
    //{
    //  var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
    //       .Include(v => v.PurchaseRequestDetails)
    //       .Include(v => v.RequisitionStatusTypes)
    //       .FirstOrDefaultAsync(v => v.RequisitionID == id);

    //  if (PurchaseRequests == null)
    //  {
    //    return NotFound();
    //  }


    //  PurchaseRequests.PurchaseRequestDetails.Add(new PR_PurchaseRequestDetail()
    //  {
    //    RequisitionID = PurchaseRequests.RequisitionID
    //  });

    //  var model = new PurchaseRequestsIndexViewModel
    //  {
    //    PurchaseRequests = PurchaseRequests
    //  };

    //  var departmentTypeName = await _appDBContext.Settings_DepartmentTypes
    //    .Where(d => d.DepartmentTypeID == PurchaseRequests.DepartmentTypeID)
    //    .Select(d => d.DepartmentTypeName)
    //    .FirstOrDefaultAsync();

    //  // Store DepartmentTypeName in ViewBag
    //  ViewBag.DepartmentTypeName = departmentTypeName;

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return View("~/Views/Purchase/Purchase/PurchaseRequest/PrintPurchaseRequest.cshtml", model);
    //}
    //public async Task<IActionResult> PrintList()
    //{
    //  int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //  int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
    //  var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
    //    .Include(v => v.PurchaseRequestDetails)
    //    .Include(v => v.RequisitionStatusTypes)
    //    .Where(v => v.DepartmentTypeID == departmentID);

    //  var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

    //  return View("~/Views/Purchase/Purchase/PurchaseRequest/PrintListPurchaseRequest.cshtml", PurchaseRequests);
    //}
    //public async Task<IActionResult> ExportToExcel()
    //{
    //  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    //  int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //  int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
    //  var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
    //    .Include(v => v.PurchaseRequestDetails)
    //    .Include(v => v.RequisitionStatusTypes)
    //    .Where(v => v.DepartmentTypeID == departmentID);

    //  var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();
    //  using (var package = new ExcelPackage())
    //  {
    //    var worksheet = package.Workbook.Worksheets.Add("PurchaseRequests");
    //    worksheet.Cells["A1"].Value = "Requisition #";
    //    worksheet.Cells["B1"].Value = "Requisition Date";
    //    worksheet.Cells["C1"].Value = "Requisition Status";

    //    for (int i = 0; i < PurchaseRequests.Count; i++)
    //    {
    //      worksheet.Cells[i + 2, 1].Value = PurchaseRequests[i].RequisitionID;
    //      worksheet.Cells[i + 2, 2].Value = PurchaseRequests[i].RequisitionDate.ToString("dd-MMM-yyyy");
    //      worksheet.Cells[i + 2, 3].Value = PurchaseRequests[i].RequisitionStatusTypes?.RequisitionStatusTypeName;


    //    }

    //    worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
    //    worksheet.Cells.AutoFitColumns();

    //    var stream = new MemoryStream();
    //    package.SaveAs(stream);
    //    stream.Position = 0;
    //    string excelName = $"PurchaseRequests-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

    //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    //  }
    //}
  }
}

