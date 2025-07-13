using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Configuration;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class PurchaseRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PurchaseRequestController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public PurchaseRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PurchaseRequestController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var purchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.Item)
          .Include(v => v.RequestStatusType)
          .AsQueryable();

      if (!string.IsNullOrWhiteSpace(searchItemName))
      {
        searchItemName = searchItemName.Trim();
        purchaseRequestsQuery = purchaseRequestsQuery
            .Where(d => d.Item != null && d.Item.ItemName.ToLower().Contains(searchItemName.ToLower()));
      }

      var purchaseRequests = await purchaseRequestsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && purchaseRequests.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Purchase Request found with the name '{searchItemName}'. Please check the name and try again.");
      }

      ViewBag.SearchItemName = searchItemName; // Optional, for displaying in view
      return View("~/Views/Purchase/Management/PurchaseRequest/PurchaseRequest.cshtml", purchaseRequests);
    }


    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      var model = new List<PR_PurchaseRequest> { new PR_PurchaseRequest() };

      return PartialView("~/Views/Purchase/Management/PurchaseRequest/AddPurchaseRequest.cshtml", model);
    }
    [HttpGet]
    public async Task<IActionResult> CreateFromStore()
    {
      ViewBag.ItemFromProcurementQueueList = await _utils.GetItemFromProcurementQueueList();
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      var model = new List<PR_PurchaseRequest> { new PR_PurchaseRequest() };

      return PartialView("~/Views/Purchase/Management/PurchaseRequest/AddFromStorePurchaseRequest.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Create(List<PR_PurchaseRequest> requests)
    {
      if (ModelState.IsValid)
      {
        // 1. Add all requests first
        foreach (var request in requests)
        {
          request.PurchaseRequestDate = DateTime.Now;
          _appDBContext.PR_PurchaseRequests.Add(request);
        }

        await _appDBContext.SaveChangesAsync(); // Save to get generated IDs

        // 2. Process each request after saving
        foreach (var request in requests)
        {
          int requestID = request.PurchaseRequestID;
          int userID = HttpContext.Session.GetInt32("UserID") ?? 0;

          var purchaseRequestStatus = new PR_PurchaseRequestStatus
          {
            PurchaseRequestID = requestID,
            ActionDate = DateTime.Now,
            ActionID = userID,
            ActionStatusTypeID = 1 // Default as pending
          };
          _appDBContext.PR_PurchaseRequestStatuss.Add(purchaseRequestStatus);

          // Check if approval process is configured
          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                  .Where(pta => pta.ProcessTypeID == 21)
                                  .CountAsync();

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 21,
              Notes = "Pending New Purchase Request",
              Date = DateTime.Now,
              UserID = userID,
              EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
              TransactionID = requestID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync(); // Save to get ID for detail

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                        .FirstOrDefaultAsync(pta =>
                                            pta.ProcessTypeID == 21 && pta.Rank == 1);

            if (nextApprovalSetup != null)
            {
              var newDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newDetail);
            }
          }
          else
          {
            // No approval process: auto-approve
            request.FinalApprovalID = 1;
            request.RequestStatusTypeID = 2;
            // No need to call Update since the entity is already tracked after Add and SaveChanges
            var approvedStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = requestID,
              ActionDate = DateTime.Now,
              ActionID = userID,
              ActionStatusTypeID = 2 // Approved
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(approvedStatus);

            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Purchase Request saved. No process setup found; Request Approved.");
          }
        }

        await _appDBContext.SaveChangesAsync(); // Final save

        await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
        var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
        .Include(v => v.Item)
        .Include(v => v.RequestStatusType)
        .ToListAsync();

        return View("~/Views/Purchase/Management/PurchaseRequest/PurchaseRequest.cshtml", PurchaseRequests);
      }

      // Re-bind dropdowns if model is invalid
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return PartialView("~/Views/Purchase/Management/PurchaseRequest/AddPurchaseRequest.cshtml", requests);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var purchaseRequestList = await _appDBContext.PR_PurchaseRequests
          .Where(v => v.PurchaseRequestID == id)
          .ToListAsync(); // Fetch all items related to the ID

      if (purchaseRequestList == null || !purchaseRequestList.Any())
      {
        return NotFound();
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return PartialView("~/Views/Purchase/Management/PurchaseRequest/EditPurchaseRequest.cshtml", purchaseRequestList);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(List<PR_PurchaseRequest> requests)
    {
      if (ModelState.IsValid)
      {
        foreach (var request in requests)
        {
          var existing = await _appDBContext.PR_PurchaseRequests
              .FirstOrDefaultAsync(r => r.PurchaseRequestID == request.PurchaseRequestID);

          if (existing != null)
          {
            // Update existing fields
            existing.ItemID = request.ItemID;
            existing.UnitTypeID = request.UnitTypeID;
            existing.Quantity = request.Quantity;
            existing.PriorityLevel = request.PriorityLevel;
            existing.PurchaseRequestDate = DateTime.Now;
            // Any other fields you want to update
            _appDBContext.PR_PurchaseRequests.Update(existing);
          }
          else
          {
            // New request (not found), add it
            request.PurchaseRequestDate = DateTime.Now;
            _appDBContext.PR_PurchaseRequests.Add(request);
          }
        }

        await _appDBContext.SaveChangesAsync();

        foreach (var request in requests)
        {
          int requestID = request.PurchaseRequestID;
          int userID = HttpContext.Session.GetInt32("UserID") ?? 0;

          // Check if status exists (avoid duplicate)
          bool statusExists = await _appDBContext.PR_PurchaseRequestStatuss
              .AnyAsync(s => s.PurchaseRequestID == requestID && s.ActionStatusTypeID == 1);

          if (!statusExists)
          {
            var purchaseRequestStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = requestID,
              ActionDate = DateTime.Now,
              ActionID = userID,
              ActionStatusTypeID = 1 // Pending
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(purchaseRequestStatus);
          }

          var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
              .CountAsync(pta => pta.ProcessTypeID == 22);

          if (processCount > 0)
          {
            var newProcessTypeApproval = new CR_ProcessTypeApproval
            {
              ProcessTypeID = 22,
              Notes = "Edited Purchase Request",
              Date = DateTime.Now,
              UserID = userID,
              EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
              TransactionID = requestID
            };

            _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
            await _appDBContext.SaveChangesAsync();

            var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                .FirstOrDefaultAsync(pta => pta.ProcessTypeID == 22 && pta.Rank == 1);

            if (nextApprovalSetup != null)
            {
              var newDetail = new CR_ProcessTypeApprovalDetail
              {
                ProcessTypeApprovalID = newProcessTypeApproval.ProcessTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.CR_ProcessTypeApprovalDetails.Add(newDetail);
            }
          }
          else
          {
            // No process setup: auto-approve
            var req = await _appDBContext.PR_PurchaseRequests
                .FirstOrDefaultAsync(r => r.PurchaseRequestID == requestID);

            if (req != null)
            {
              req.FinalApprovalID = 1;
              req.RequestStatusTypeID = 2;
            }

            var approvedStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = requestID,
              ActionDate = DateTime.Now,
              ActionID = userID,
              ActionStatusTypeID = 2 // Approved
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(approvedStatus);

            await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Purchase Request updated. No process setup found; auto-approved.");
          }
        }

        await _appDBContext.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
        var purchaseRequests = await _appDBContext.PR_PurchaseRequests
            .Include(v => v.Item)
            .Include(v => v.RequestStatusType)
            .ToListAsync();

        return View("~/Views/Purchase/Management/PurchaseRequest/PurchaseRequest.cshtml", purchaseRequests);
      }

      // If invalid model, reload view with dropdowns
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return PartialView("~/Views/Purchase/Management/PurchaseRequest/EditPurchaseRequest.cshtml", requests);
    }

    public async Task<IActionResult> Print(int id)
    {
      var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
           .Include(v => v.Item)
           .Include(v => v.RequestStatusType)
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      if (PurchaseRequests == null)
      {
        return NotFound();
      }


      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return View("~/Views/Purchase/Management/PurchaseRequest/PrintPurchaseRequest.cshtml", PurchaseRequests);
    }
    public async Task<IActionResult> PrintList()
    {
      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
        .Include(v => v.Item)
        .Include(v => v.RequestStatusType);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      return View("~/Views/Purchase/Management/PurchaseRequest/PrintListPurchaseRequest.cshtml", PurchaseRequests);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
        .Include(v => v.Item)
        .Include(v => v.RequestStatusType);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("PurchaseRequests");
        worksheet.Cells["A1"].Value = "Request #";
        worksheet.Cells["B1"].Value = "Request Date";
        worksheet.Cells["C1"].Value = "Item Name";
        worksheet.Cells["D1"].Value = "Quantity";
        worksheet.Cells["E1"].Value = "Request Status";

        for (int i = 0; i < PurchaseRequests.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = PurchaseRequests[i].PurchaseRequestID;
          worksheet.Cells[i + 2, 2].Value = PurchaseRequests[i].PurchaseRequestDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = PurchaseRequests[i].Item?.ItemName;
          worksheet.Cells[i + 2, 4].Value = PurchaseRequests[i].Quantity;
          worksheet.Cells[i + 2, 5].Value = PurchaseRequests[i].RequestStatusType?.RequestStatusTypeName;

        }

        worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"PurchaseRequests-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    [HttpGet]
    public async Task<IActionResult> GetProcurementQueueDetails(int id)
    {
      var item = await _appDBContext.PR_ProcurementQueues
          .Include(q => q.Item)
          .Where(q => q.ProcurementQueueID == id)
          .Select(q => new
          {
            ItemID = q.ItemID,
            UnitTypeID = q.UnitTypeID,
            Quantity = q.Quantity,
            ProcurementQueueID = q.ProcurementQueueID
          })
          .FirstOrDefaultAsync();

      if (item == null)
      {
        return NotFound();
      }

      return Json(item);
    }

  }
}

