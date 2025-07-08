using Azure.Core;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class CostComparisonController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;


    public CostComparisonController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var CostComparisonsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      CostComparisonsQuery = CostComparisonsQuery
          .Where(v => v.RequestStatusTypeID == 5);

      var CostComparisons = await CostComparisonsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && CostComparisons.Count == 0)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Purchase Request found with the name '{searchItemName}'. Please check the name and try again.");
      }

      return View("~/Views/Purchase/Management/CostComparison/CostComparison.cshtml", CostComparisons);
    }
    [HttpGet]
    public async Task<IActionResult> Forword(int id)
    {
      var purchaseRequestList = await _appDBContext.PR_PurchaseRequests
          .Where(v => v.PurchaseRequestID == id)
          .ToListAsync(); // Fetch all items related to the ID

      var RequestForQuotationList = await _appDBContext.PR_RequestForQuotations
          .Where(v => v.PurchaseRequestID == id)
          .ToListAsync();

      var CostComparisonList = await _appDBContext.PR_CostComparison
          .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      if (purchaseRequestList == null || !purchaseRequestList.Any())
      {
        return NotFound();
      }
      
      if (RequestForQuotationList == null || !RequestForQuotationList.Any())
      {
        return NotFound();
      }
      
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      var firstRequestForQuotation = RequestForQuotationList.FirstOrDefault();
      if (firstRequestForQuotation != null)
      {
        ViewBag.QuotationVendorID1 = firstRequestForQuotation.QuotationVendorID1;
        ViewBag.QuotationVendorID2 = firstRequestForQuotation.QuotationVendorID2;
        ViewBag.QuotationVendorID3 = firstRequestForQuotation.QuotationVendorID3;
        ViewBag.VendorListbyComparison = await _utils.GetVendorListbyComparison(firstRequestForQuotation.PurchaseRequestID);
      }

      var model = new PurchaseRequestwithCostComparisonViewModel
      {
        PurchaseRequests = purchaseRequestList,
        CostComparisons = new List<PR_CostComparison> { CostComparisonList }
      };
      return PartialView("~/Views/Purchase/Management/CostComparison/ForwordCostComparison.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Forword(PurchaseRequestwithCostComparisonViewModel requests)
    {
      if (ModelState.IsValid)
      {
        foreach (var request in requests.PurchaseRequests)
        {
          var existing = await _appDBContext.PR_PurchaseRequests
              .FirstOrDefaultAsync(r => r.PurchaseRequestID == request.PurchaseRequestID);

          if (existing != null)
          {
            existing.RequestStatusTypeID = 6;
            _appDBContext.PR_PurchaseRequests.Update(existing);
          }
          foreach (var request1 in requests.CostComparisons)
          {
            var CostComparisons = new PR_CostComparison
            {
              PurchaseRequestID = request.PurchaseRequestID,
              DeliverdVendorID = request1.DeliverdVendorID,
            };
            _appDBContext.PR_CostComparison.Add(CostComparisons);
          }
        }
        

        await _appDBContext.SaveChangesAsync();

        foreach (var request in requests.PurchaseRequests)
        {
          int requestID = request.PurchaseRequestID;
          int userID = HttpContext.Session.GetInt32("UserID") ?? 0;

          // Check if status exists (avoid duplicate)
          bool statusExists = await _appDBContext.PR_PurchaseRequestStatuss
              .AnyAsync(s => s.PurchaseRequestID == requestID && s.ActionStatusTypeID == 7);

          if (!statusExists)
          {
            var purchaseRequestStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = requestID,
              ActionDate = DateTime.Now,
              ActionID = userID,
              ActionStatusTypeID = 6 // Cost Comparison
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(purchaseRequestStatus);
          }
        }

        await _appDBContext.SaveChangesAsync();

     
        var CostComparisonsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

        CostComparisonsQuery = CostComparisonsQuery
            .Where(v => v.RequestStatusTypeID == 5);

        var purchaseRequests = await CostComparisonsQuery.ToListAsync();

        return View("~/Views/Purchase/Management/CostComparison/CostComparison.cshtml", purchaseRequests);
      }

      // If invalid model, reload view with dropdowns
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();

      return PartialView("~/Views/Purchase/Management/CostComparison/ForwordCostComparison.cshtml", requests);
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
      return View("~/Views/Purchase/Management/CostComparison/PrintCostComparison.cshtml", model);
    }
    public async Task<IActionResult> PrintList()
    {
   
      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 5);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      return View("~/Views/Purchase/Management/CostComparison/PrintListCostComparison.cshtml", PurchaseRequests);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 5);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("CostComparisons");
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
        string excelName = $"CostComparisons-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
