using Azure.Core;
using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;
using System.Configuration;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class PurchaseOrderController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PurchaseOrderController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public PurchaseOrderController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PurchaseOrderController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var query = from pr in _appDBContext.PR_PurchaseRequests

                  join rst in _appDBContext.Settings_RequestStatusTypes
                  on pr.RequestStatusTypeID equals rst.RequestStatusTypeID

                  join itm in _appDBContext.ST_Items
                  on pr.ItemID equals itm.ItemID


                  join rfq in _appDBContext.PR_RequestForQuotations on pr.PurchaseRequestID equals rfq.PurchaseRequestID into rfqs
                  from rfq in rfqs.DefaultIfEmpty()

                  join cc in _appDBContext.PR_CostComparison on pr.PurchaseRequestID equals cc.PurchaseRequestID into ccs
                  from cc in ccs.DefaultIfEmpty()

                  join vnd in _appDBContext.FI_Vendors
                  on cc.DeliverdVendorID equals vnd.VendorID into vnds
                  from vnd in vnds.DefaultIfEmpty()

                  join haf in _appDBContext.Settings_HeadofAccount_Fives
                  on vnd.HeadofAccount_FiveID equals haf.HeadofAccount_FiveID into hafs
                  from haf in hafs.DefaultIfEmpty()

                  where pr.RequestStatusTypeID == 6
                  select new PurchaseRequestwithPurchaseOrderViewModel
                  {
                    PurchaseRequests = pr,
                    RequestStatusType = rst,
                    Item = itm,
                    RequestForQuotations = rfq,
                    CostComparisons = cc,
                    Vendor = vnd,
                    HeadofAccount_Five = haf
                  };

      if (!string.IsNullOrEmpty(searchItemName))
      {
        query = query.Where(x => x.Item.ItemName.Contains(searchItemName));
      }

      var rows = await query.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && rows.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Purchase Order found with the item name '{searchItemName}'. Please check the name and try again.");
      }
      ViewBag.VendorList = await _utils.GetVendorList();
      return View("~/Views/Purchase/Management/PurchaseOrder/PurchaseOrder.cshtml", rows);
    }
    [HttpGet]
    public async Task<IActionResult> MakeOrder(List<int> ids)
    {

      try
      {
        if (ids == null || !ids.Any())
        {
          return BadRequest("No Purchase Requests selected.");
        }
        // Get all selected PurchaseRequests (with includes if needed)

        var idString = string.Join(",", ids);
        var sql = $@"
            SELECT pr.* 
            FROM PR_PurchaseRequests AS pr
            WHERE pr.PurchaseRequestID IN ({idString})
        ";

        var list = _appDBContext.PR_PurchaseRequests
            .FromSqlRaw(sql)
            .Include(pr => pr.Item)
            .Include(pr => pr.RequestStatusType)
            .ToList();


        Console.WriteLine(list);

        if (!list.Any())
        {
          return NotFound("No matching Purchase Requests found.");
        }
        var purchaseRequestId = list.Select(pr => pr.PurchaseRequestID).First();
        ViewBag.ItemList = await _utils.GetItemList();
        ViewBag.ItemNameList = await _utils.GetItemList();
        ViewBag.ItemUnitList = await _utils.GetItemUnits();
        ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
        ViewBag.VendorList = await _utils.GetVendorListbyPurchaseOrder(purchaseRequestId);


        return PartialView("~/Views/Purchase/Management/PurchaseOrder/MakePurchaseOrder.cshtml", list);
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex); // or use logger
        return StatusCode(500, "An error occurred.");
      }
    }
    [HttpPost]
    public async Task<IActionResult> MakeOrder(List<PR_PurchaseRequest> requests)
    {
      if (ModelState.IsValid)
      {
        var maxPONO = await _appDBContext.PR_PurchaseOrders
          .MaxAsync(po => (int?)po.PONO) ?? 0;

        var nextPONO = maxPONO + 1;

        foreach (var request in requests)
        {
          var purchaseorders = new PR_PurchaseOrder
          {
            PurchaseRequestID = request.PurchaseRequestID,
            PurchaseOrderDate = DateTime.Now,
            PONO = nextPONO
          };
          _appDBContext.PR_PurchaseOrders.Add(purchaseorders);

        }

        foreach (var request in requests)
        {
          var existing = await _appDBContext.PR_PurchaseRequests
              .FirstOrDefaultAsync(r => r.PurchaseRequestID == request.PurchaseRequestID);
          if (existing != null)
          {
            existing.RequestStatusTypeID = 7;
            _appDBContext.PR_PurchaseRequests.Update(existing);
          }
          bool statusExists = await _appDBContext.PR_PurchaseRequestStatuss
              .AnyAsync(s => s.PurchaseRequestID == request.PurchaseRequestID && s.ActionStatusTypeID == 7);
          int userIDC = HttpContext.Session.GetInt32("UserID") ?? 0;
          if (!statusExists)
          {
            var purchaseRequestStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = request.PurchaseRequestID,
              ActionDate = DateTime.Now,
              ActionID = userIDC,
              ActionStatusTypeID = 7 // Purchase Order
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(purchaseRequestStatus);
          }
        }

        await _appDBContext.SaveChangesAsync();

        int requestID = nextPONO;
        int userID = HttpContext.Session.GetInt32("UserID") ?? 0;

        // Check if approval process is configured
        var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                  .Where(pta => pta.ProcessTypeID == 23)
                                  .CountAsync();

        if (processCount > 0)
        {
          var newProcessTypeApproval = new CR_ProcessTypeApproval
          {
            ProcessTypeID = 23,
            Notes = "Approval For Purchase Order",
            Date = DateTime.Now,
            UserID = userID,
            EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
            TransactionID = requestID
          };

          _appDBContext.CR_ProcessTypeApprovals.Add(newProcessTypeApproval);
          await _appDBContext.SaveChangesAsync(); // Save to get ID for detail

          var nextApprovalSetup = await _appDBContext.CR_ProcessTypeApprovalSetups
                                      .FirstOrDefaultAsync(pta =>
                                          pta.ProcessTypeID == 23 && pta.Rank == 1);

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
          await _appDBContext.SaveChangesAsync();
          await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
        }
        else
        {
          var poList = await _appDBContext.PR_PurchaseOrders
              .Where(x => x.PONO == nextPONO)
              .ToListAsync();

          foreach (var po in poList)
          {
            po.FinalApprovalID = 1;
          }
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", "Purchase Request saved. No process setup found; Request Approved.");
        }
        await _appDBContext.SaveChangesAsync();

        return RedirectToAction("Index");
      }

      // Re-bind dropdowns if model is invalid
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return PartialView("~/Views/Purchase/Management/PurchaseOrder/MakePurchaseOrder.cshtml", requests);
    }


    public async Task<IActionResult> Edit(int id)
    {
      var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
           .Include(v => v.Item)
           .Include(v => v.RequestStatusType)
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      var RequestForQuotationList = await _appDBContext.PR_RequestForQuotations
          .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      var CostComparisonList = await _appDBContext.PR_CostComparison
          .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);


      if (PurchaseRequests == null)
      {
        return NotFound();
      }

      if (RequestForQuotationList == null)
      {
        return NotFound();
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      if (RequestForQuotationList != null)
      {
        ViewBag.QuotationVendorID1 = RequestForQuotationList.QuotationVendorID1;
        ViewBag.QuotationVendorID2 = RequestForQuotationList.QuotationVendorID2;
        ViewBag.QuotationVendorID3 = RequestForQuotationList.QuotationVendorID3;
        ViewBag.VendorListbyComparison = await _utils.GetVendorListbyComparison(RequestForQuotationList.PurchaseRequestID);
      }

      var model = new PurchaseRequestwithCostComparisonViewModel
      {
        PurchaseRequests = new List<PR_PurchaseRequest> { PurchaseRequests },
        CostComparisons = new List<PR_CostComparison> { CostComparisonList },
      };
      return PartialView("~/Views/Purchase/Management/PurchaseOrder/EditPurchaseOrder.cshtml", model);
    }




    public async Task<IActionResult> Print(int id)
    {
      var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
           .Include(v => v.Item)
           .Include(v => v.RequestStatusType)
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      var RequestForQuotationList = await _appDBContext.PR_RequestForQuotations
          .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      var CostComparisonList = await _appDBContext.PR_CostComparison
          .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);


      if (PurchaseRequests == null)
      {
        return NotFound();
      }

      if (RequestForQuotationList == null)
      {
        return NotFound();
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      if (RequestForQuotationList != null)
      {
        ViewBag.QuotationVendorID1 = RequestForQuotationList.QuotationVendorID1;
        ViewBag.QuotationVendorID2 = RequestForQuotationList.QuotationVendorID2;
        ViewBag.QuotationVendorID3 = RequestForQuotationList.QuotationVendorID3;
        ViewBag.VendorListbyComparison = await _utils.GetVendorListbyComparison(RequestForQuotationList.PurchaseRequestID);
      }

      var model = new PurchaseRequestwithCostComparisonViewModel
      {
        PurchaseRequests = new List<PR_PurchaseRequest> { PurchaseRequests },
        CostComparisons = new List<PR_CostComparison> { CostComparisonList },
      };
      return View("~/Views/Purchase/Management/PurchaseOrder/PrintPurchaseOrder.cshtml", model);
    }
    public async Task<IActionResult> PrintList()
    {

      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 6);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      return View("~/Views/Purchase/Management/PurchaseOrder/PrintListPurchaseOrder.cshtml", PurchaseRequests);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 6);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("PurchaseOrder");
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
        string excelName = $"PurchaseOrder-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
