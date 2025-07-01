using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.Purchase.Management
{
  public class RequestForQuotationController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _conSTguration;
    private readonly Utils _utils;

    public RequestForQuotationController(AppDBContext appDBContext, IConfiguration conSTguration, Utils utils)
    {
      _appDBContext = appDBContext;
      _conSTguration = conSTguration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      var RequestForQuotationsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      RequestForQuotationsQuery = RequestForQuotationsQuery
          .Where(v => v.RequestStatusTypeID == 2);

      var RequestForQuotations = await RequestForQuotationsQuery.ToListAsync();

      if (!string.IsNullOrEmpty(searchItemName) && RequestForQuotations.Count == 0)
      {
        TempData["ErrorMessage"] = $"No Purchase Request found with the name '{searchItemName}'. Please check the name and try again.";
      }

      return View("~/Views/Purchase/Management/RequestForQuotation/RequestForQuotation.cshtml", RequestForQuotations);
    }
    [HttpGet]
    public async Task<IActionResult> Forword(int id)
    {
      var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
          .Where(v => v.PurchaseRequestID == id)
          .ToListAsync(); // Fetch all items related to the ID

      var RequestForQuotations = await _appDBContext.PR_RequestForQuotations
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      if (PurchaseRequests == null || !PurchaseRequests.Any())
      {
        return NotFound();
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      var model = new PurchaseRequestwithRequestForQuotationViewModel
      {
        PurchaseRequests = PurchaseRequests,
        RequestForQuotations = new List<PR_RequestForQuotation> { RequestForQuotations }
      };

      return PartialView("~/Views/Purchase/Management/RequestForQuotation/ForwordRequestForQuotation.cshtml", model);
    }

    [HttpPost]
    public async Task<IActionResult> Forword(PurchaseRequestwithRequestForQuotationViewModel requests)
    {
      if (ModelState.IsValid)
      {
        // Update Purchase Requests
        foreach (var request in requests.PurchaseRequests)
        {
          if (request == null) continue; // safety

          var existing = await _appDBContext.PR_PurchaseRequests
              .FirstOrDefaultAsync(r => r.PurchaseRequestID == request.PurchaseRequestID);

          if (existing != null)
          {
            existing.RequestStatusTypeID = 5;
            // No need to call .Update for tracked entity
          }
          // Insert Request For Quotations
          foreach (var request1 in requests.RequestForQuotations)
          {
            if (request1 == null) continue; // safety

            var newRequestForQuotation = new PR_RequestForQuotation
            {
              QuotationVendorID1 = request1.QuotationVendorID1,
              QuotationVendorID2 = request1.QuotationVendorID2,
              QuotationVendorID3 = request1.QuotationVendorID3,
              PurchaseRequestID = request.PurchaseRequestID
            };

            _appDBContext.PR_RequestForQuotations.Add(newRequestForQuotation);
          }
        }
        await _appDBContext.SaveChangesAsync();


        foreach (var request in requests.PurchaseRequests)
        {
          int requestID = request.PurchaseRequestID;
          int userID = HttpContext.Session.GetInt32("UserID") ?? 0;

          // Check if status exists (avoid duplicate)
          bool statusExists = await _appDBContext.PR_PurchaseRequestStatuss
              .AnyAsync(s => s.PurchaseRequestID == requestID && s.ActionStatusTypeID == 5);

          if (!statusExists)
          {
            var purchaseRequestStatus = new PR_PurchaseRequestStatus
            {
              PurchaseRequestID = requestID,
              ActionDate = DateTime.Now,
              ActionID = userID,
              ActionStatusTypeID = 5 // Request for Quotation
            };
            _appDBContext.PR_PurchaseRequestStatuss.Add(purchaseRequestStatus);
          }
        }

        await _appDBContext.SaveChangesAsync();

       var RequestForQuotationsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

        RequestForQuotationsQuery = RequestForQuotationsQuery
            .Where(v => v.RequestStatusTypeID == 2);

        var purchaseRequests = await RequestForQuotationsQuery.ToListAsync();

        return View("~/Views/Purchase/Management/RequestForQuotation/RequestForQuotation.cshtml", purchaseRequests);
      }

      // If invalid model, reload view with dropdowns
      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      return PartialView("~/Views/Purchase/Management/RequestForQuotation/ForwordRequestForQuotation.cshtml", requests);
    }

    public async Task<IActionResult> Print(int id)
    {
      var PurchaseRequests = await _appDBContext.PR_PurchaseRequests
           .Include(v => v.Item)
           .Include(v => v.RequestStatusType)
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      var RequestForQuotations = await _appDBContext.PR_RequestForQuotations
           .FirstOrDefaultAsync(v => v.PurchaseRequestID == id);

      if (PurchaseRequests == null)
      {
        return NotFound();
      }
      

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();
      ViewBag.ItemUnitList = await _utils.GetItemUnits();
      ViewBag.PriorityLevelList = await _utils.GetPriorityLevel();
      ViewBag.VendorList = await _utils.GetVendorList();

      var model = new PurchaseRequestwithRequestForQuotationViewModel
      {
        PurchaseRequests = new List<PR_PurchaseRequest> { PurchaseRequests },
        RequestForQuotations = new List<PR_RequestForQuotation> { RequestForQuotations }
      };

      return View("~/Views/Purchase/Management/RequestForQuotation/PrintRequestForQuotation.cshtml", model);
    }
    public async Task<IActionResult> PrintList()
    {
      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 2);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();


      return View("~/Views/Purchase/Management/RequestForQuotation/PrintListRequestForQuotation.cshtml", PurchaseRequests);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var PurchaseRequestsQuery = _appDBContext.PR_PurchaseRequests
          .Include(v => v.RequestStatusType)
          .Include(v => v.Item)
          .AsQueryable();

      PurchaseRequestsQuery = PurchaseRequestsQuery
          .Where(v => v.RequestStatusTypeID == 2);

      var PurchaseRequests = await PurchaseRequestsQuery.ToListAsync();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("RequestForQuotations");
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
        string excelName = $"RequestForQuotations-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

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
