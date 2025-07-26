using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class ProcurementQueueController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ProcurementQueueController> _localizer;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;


    public ProcurementQueueController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ProcurementQueueController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var ProcurementQueuesQuery = _appDBContext.PR_ProcurementQueues
                                      .Include(d => d.Item)
                                      .Where(d => d.ForwardYNID == 0)// Ensure Item is included in query
                                      .AsQueryable();

      if (!string.IsNullOrEmpty(searchItemName))
      {
        ProcurementQueuesQuery = ProcurementQueuesQuery
            .Where(d => d.Item.ItemName.Contains(searchItemName)); // Use Where() instead of Any()
      }

      var ProcurementQueues = await ProcurementQueuesQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && ProcurementQueues.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Procurement Queue found with the name '{searchItemName}'. Please check the name and try again.");
      }

      return View("~/Views/StoreManagement/StoreManagement/ProcurementQueue/ProcurementQueue.cshtml", ProcurementQueues);
    }

    //[HttpGet]
    //public async Task<IActionResult> Create()
    //{
    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  PR_ProcurementQueue Requisitions = new PR_ProcurementQueue();
    //  Requisitions.ProcurementQueueDetails.Add(new PR_ProcurementQueueDetail() { RequisitionID = 0 });
    //  var model = new ProcurementQueuesIndexViewModel
    //  {
    //    ProcurementQueues = Requisitions
    //  };

    //  return PartialView("~/Views/StoreManagement/StoreManagement/ProcurementQueue/AddProcurementQueue.cshtml", model);
    //}
    //[HttpPost]
    //public async Task<IActionResult> Create(ProcurementQueuesIndexViewModel model)
    //{
    //  if (ModelState.IsValid)
    //  {
    //    try
    //    {
    //      int userID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //      int departmentID = await _utils.PostUserIDGetDepartmentID(userID);
    //      model.ProcurementQueues.DepartmentTypeID = departmentID;
    //      model.ProcurementQueues.FinalApprovalID = 0;
    //      model.ProcurementQueues.RequisitionStatusTypeID = 1;
    //      model.ProcurementQueues.RequisitionDate = DateTime.Now;
    //      _appDBContext.PR_ProcurementQueues.Add(model.ProcurementQueues);
    //      //await _appDBContext.SaveChangesAsync();

    //      model.ProcurementQueues.ProcurementQueueDetails.RemoveAll(e => e.ItemID == 0);
    //      model.ProcurementQueues.ProcurementQueueDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

    //      foreach (var detail in model.ProcurementQueues.ProcurementQueueDetails)
    //      {
    //        detail.RequisitionID = model.ProcurementQueues.RequisitionID;
    //        _appDBContext.PR_ProcurementQueueDetails.Add(detail);
    //      }

    //      await _appDBContext.SaveChangesAsync();

    //      var RequisitionID = model.ProcurementQueues.RequisitionID;

    //      var ProcurementQueueStatus = new PR_ProcurementQueueStatus
    //      {
    //        RequisitionID = RequisitionID,
    //        ActionDate = DateTime.Now,
    //        ActionID = HttpContext.Session.GetInt32("UserID") ?? default(int),
    //        ActionStatusTypeID = 1
    //      };

    //      _appDBContext.PR_ProcurementQueueStatuss.Add(ProcurementQueueStatus);

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
    //            Notes = "Pending New Procurement Queue",
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
    //          model.ProcurementQueues.FinalApprovalID = 1;
    //          model.ProcurementQueues.RequisitionStatusTypeID = 2;
    //          _appDBContext.PR_ProcurementQueues.Update(model.ProcurementQueues);

    //          ProcurementQueueStatus.ActionStatusTypeID = 2;
    //          _appDBContext.PR_ProcurementQueueStatuss.Update(ProcurementQueueStatus);

    //          await _appDBContext.SaveChangesAsync();
    //          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", " Procurement Queue successfully. No process setup found,  Procurement Queue Approved.");
    //          return Json(new { success = true, message = "No process setup found,  Procurement Queue Approved." });
    //        }
    //      }
    //      await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", " Procurement Queue Created successfully. Continue to the Approval Process Setup for  Procurement Queue Approved.");

    //      return Json(new { success = true });
    //    }
    //    catch (Exception ex)
    //    {
    //      return Json(new { success = false, message = "Error: " + ex.Message });
    //    }

    //  }

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  if (model.ProcurementQueues.ProcurementQueueDetails == null || !model.ProcurementQueues.ProcurementQueueDetails.Any())
    //  {
    //    model.ProcurementQueues.ProcurementQueueDetails = new List<PR_ProcurementQueueDetail> { new PR_ProcurementQueueDetail() };
    //  }

    //  if (model.ProcurementQueues == null)
    //  {
    //    model.ProcurementQueues = new PR_ProcurementQueue();
    //  }

    //  return PartialView("~/Views/StoreManagement/StoreManagement/ProcurementQueue/AddProcurementQueue.cshtml", model);
    //}
    //[HttpGet]
    //public async Task<IActionResult> Edit(int id)
    //{
    //  var ProcurementQueues = await _appDBContext.PR_ProcurementQueues
    //      .Include(v => v.ProcurementQueueDetails)
    //      .FirstOrDefaultAsync(v => v.RequisitionID == id);

    //  if (ProcurementQueues == null)
    //  {
    //    return NotFound();
    //  }

    //  // Check RequisitionStatusTypeID
    //  if (ProcurementQueues.RequisitionStatusTypeID != 1)
    //  {
    //    await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "After approval, editing is not allowed.....");

    //  }

    //  ProcurementQueues.ProcurementQueueDetails.Add(new PR_ProcurementQueueDetail()
    //  {
    //    RequisitionID = ProcurementQueues.RequisitionID
    //  });

    //  var model = new ProcurementQueuesIndexViewModel
    //  {
    //    ProcurementQueues = ProcurementQueues
    //  };

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return PartialView("~/Views/StoreManagement/StoreManagement/ProcurementQueue/EditProcurementQueue.cshtml", model);
    //}
    //[HttpPost]
    //public async Task<IActionResult> Edit(ProcurementQueuesIndexViewModel ProcurementQueue)
    //{
    //  if (ModelState.IsValid)
    //  {

    //    var existingProcurementQueue = await _appDBContext.PR_ProcurementQueues
    //        .Include(v => v.ProcurementQueueDetails)
    //        .FirstOrDefaultAsync(v => v.RequisitionID == ProcurementQueue.ProcurementQueues.RequisitionID);
    //    if (existingProcurementQueue?.RequisitionStatusTypeID != 1)
    //    {
    //      await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "After approval, editing is not allowed.....");

    //    }
    //    else
    //    {
    //      if (existingProcurementQueue != null)
    //      {
    //        existingProcurementQueue.Remarks = ProcurementQueue.ProcurementQueues.Remarks;

    //        _appDBContext.Update(existingProcurementQueue);
    //        await _appDBContext.SaveChangesAsync();

    //        var ProcurementQueueDetailsToRemove = _appDBContext.PR_ProcurementQueueDetails
    //        .Where(v => v.RequisitionID == ProcurementQueue.ProcurementQueues.RequisitionID)
    //        .ToList();

    //        _appDBContext.PR_ProcurementQueueDetails.RemoveRange(ProcurementQueueDetailsToRemove);

    //        await _appDBContext.SaveChangesAsync();
    //        ProcurementQueue.ProcurementQueues.ProcurementQueueDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

    //        foreach (var detail in ProcurementQueue.ProcurementQueues.ProcurementQueueDetails)
    //        {
    //          detail.RequisitionID = ProcurementQueue.ProcurementQueues.RequisitionID;
    //          _appDBContext.PR_ProcurementQueueDetails.Add(detail);
    //        }

    //        await _appDBContext.SaveChangesAsync();

    //        return Json(new { success = true, message = "Received ProcurementQueue Edited successfully!" });
    //      }
    //      else
    //      {
    //        return NotFound();
    //      }
    //    }
    //  }

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return PartialView("~/Views/StoreManagement/StoreManagement/ProcurementQueue/EditProcurementQueue.cshtml", ProcurementQueue);
    //}
    //public async Task<IActionResult> Print(int id)
    //{
    //  var ProcurementQueues = await _appDBContext.PR_ProcurementQueues
    //       .Include(v => v.ProcurementQueueDetails)
    //       .Include(v => v.RequisitionStatusTypes)
    //       .FirstOrDefaultAsync(v => v.RequisitionID == id);

    //  if (ProcurementQueues == null)
    //  {
    //    return NotFound();
    //  }


    //  ProcurementQueues.ProcurementQueueDetails.Add(new PR_ProcurementQueueDetail()
    //  {
    //    RequisitionID = ProcurementQueues.RequisitionID
    //  });

    //  var model = new ProcurementQueuesIndexViewModel
    //  {
    //    ProcurementQueues = ProcurementQueues
    //  };

    //  var departmentTypeName = await _appDBContext.Settings_DepartmentTypes
    //    .Where(d => d.DepartmentTypeID == ProcurementQueues.DepartmentTypeID)
    //    .Select(d => d.DepartmentTypeName)
    //    .FirstOrDefaultAsync();

    //  // Store DepartmentTypeName in ViewBag
    //  ViewBag.DepartmentTypeName = departmentTypeName;

    //  ViewBag.ItemList = await _utils.GetItemList();
    //  ViewBag.ItemNameList = await _utils.GetItemList();

    //  return View("~/Views/StoreManagement/StoreManagement/ProcurementQueue/PrintProcurementQueue.cshtml", model);
    //}
    //public async Task<IActionResult> PrintList()
    //{
    //  int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //  int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
    //  var ProcurementQueuesQuery = _appDBContext.PR_ProcurementQueues
    //    .Include(v => v.ProcurementQueueDetails)
    //    .Include(v => v.RequisitionStatusTypes)
    //    .Where(v => v.DepartmentTypeID == departmentID);

    //  var ProcurementQueues = await ProcurementQueuesQuery.ToListAsync();

    //  return View("~/Views/StoreManagement/StoreManagement/ProcurementQueue/PrintListProcurementQueue.cshtml", ProcurementQueues);
    //}
    //public async Task<IActionResult> ExportToExcel()
    //{
    //  ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

    //  int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
    //  int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
    //  var ProcurementQueuesQuery = _appDBContext.PR_ProcurementQueues
    //    .Include(v => v.ProcurementQueueDetails)
    //    .Include(v => v.RequisitionStatusTypes)
    //    .Where(v => v.DepartmentTypeID == departmentID);

    //  var ProcurementQueues = await ProcurementQueuesQuery.ToListAsync();
    //  using (var package = new ExcelPackage())
    //  {
    //    var worksheet = package.Workbook.Worksheets.Add("ProcurementQueues");
    //    worksheet.Cells["A1"].Value = "Requisition #";
    //    worksheet.Cells["B1"].Value = "Requisition Date";
    //    worksheet.Cells["C1"].Value = "Requisition Status";

    //    for (int i = 0; i < ProcurementQueues.Count; i++)
    //    {
    //      worksheet.Cells[i + 2, 1].Value = ProcurementQueues[i].RequisitionID;
    //      worksheet.Cells[i + 2, 2].Value = ProcurementQueues[i].RequisitionDate.ToString("dd-MMM-yyyy");
    //      worksheet.Cells[i + 2, 3].Value = ProcurementQueues[i].RequisitionStatusTypes?.RequisitionStatusTypeName;


    //    }

    //    worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
    //    worksheet.Cells.AutoFitColumns();

    //    var stream = new MemoryStream();
    //    package.SaveAs(stream);
    //    stream.Position = 0;
    //    string excelName = $"ProcurementQueues-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

    //    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
    //  }
    //}
  }
}
