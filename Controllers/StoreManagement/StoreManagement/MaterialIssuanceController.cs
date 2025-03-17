using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Exampler_ERP.Utilities;
using OfficeOpenXml;

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
        .Include(v => v.IssuanceStatusTypes)
        .Include(v => v.MaterialIssuanceDetails)
            .ThenInclude(d => d.Items)
        .AsQueryable(); 

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

        var requisitionDetails = await (from req in _appDBContext.ST_MaterialRequisitionDetails
                                        join stock in _appDBContext.ST_Stocks
                                            on req.ItemID equals stock.ItemID into stockGroup
                                        where req.RequisitionID == requisitionId
                                              && req.Quantity > 0 // Exclude requisitions with Quantity = 0
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
                                          AvailableStock = stockGroup.Sum(s => s.Quantity) 
                                        })
                         .Where(x => x.Quantity > 0)
                         .ToListAsync();
        if (requisitionDetails == null || !requisitionDetails.Any())
        {
          return Json(new { success = false, message = "No data found for the given Requisition ID." });
        }

        return Json(new { success = true, data = requisitionDetails });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = ex.Message });
      }
    }
    [HttpGet]
    public async Task<IActionResult> GetIssuanceDetails(int issuanceID)
    {
      try
      {

        var issueDetails = await _appDBContext.ST_MaterialIssuanceDetails
        .Include(s => s.MaterialIssuances)
        .Include(s => s.Items)
        .Where(s => s.IssuanceID == issuanceID)
        .Select(s => new
        {
          ItemID = s.ItemID,
          ItemName = s.Items.ItemName,
          RequisitionQuantity = s.RequisitionQuantity,
          IssuanceQuantity = s.IssuanceQuantity,
          BalanceQuantity = s.BalanceQuantity
        })
        .ToListAsync();


        if (issueDetails == null || !issueDetails.Any())
        {
          return Json(new { success = false, message = "No data found for the given issue ID." });
        }

        return Json(new { success = true, data = issueDetails });
      }
      catch (Exception ex)
      {
        return Json(new { success = false, message = ex.Message });
      }
    }

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
        var requisitionQuantities = _appDBContext.ST_MaterialRequisitionDetails
            .Where(r => r.RequisitionID == issuance.RequisitionID)
            .GroupBy(r => r.ItemID)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(r => r.Quantity)
            );

        var issuanceQuantities = _appDBContext.ST_MaterialIssuanceDetails
            .Where(i => i.MaterialIssuances.RequisitionID == issuance.RequisitionID)
            .GroupBy(i => i.ItemID)
            .ToDictionary(
                g => g.Key,
                g => g.Sum(i => i.IssuanceQuantity)
            );

        bool allNoIssuance = false;
        bool allFullyIssued = true;

        foreach (var itemId in requisitionQuantities.Keys)
        {
          var requisitionQty = requisitionQuantities[itemId];
          issuanceQuantities.TryGetValue(itemId, out var issuedQty);

          if (issuedQty == 0)
          {
            allNoIssuance = true; 
            break; 
          }
          else if (issuedQty < requisitionQty)
          {
            allFullyIssued = false; 
          }
          else if (issuedQty == requisitionQty)
          {
          }
        }

        int actionStatusTypeID = allNoIssuance ? 3 : (allFullyIssued ? 4 : 3);

        var requisitionStatus = new ST_MaterialRequisitionStatus
        {
          RequisitionID = issuance.RequisitionID,
          ActionID = 2, 
          ActionDate = DateTime.Now,
          ActionStatusTypeID = actionStatusTypeID
        };
        _appDBContext.ST_MaterialRequisitionStatuss.Add(requisitionStatus);
        await _appDBContext.SaveChangesAsync();

        var requisition = await _appDBContext.ST_MaterialRequisitions
            .FirstOrDefaultAsync(r => r.RequisitionID == issuance.RequisitionID);

        if (requisition != null)
        {
          requisition.RequisitionStatusTypeID = actionStatusTypeID;
          _appDBContext.ST_MaterialRequisitions.Update(requisition);
          await _appDBContext.SaveChangesAsync();
        }

        var issuancestatus = await _appDBContext.ST_MaterialIssuances
           .FirstOrDefaultAsync(r => r.IssuanceID == issuance.IssuanceID);

        if (issuancestatus != null)
        {
          issuancestatus.IssuanceStatusTypeID = actionStatusTypeID;
          _appDBContext.ST_MaterialIssuances.Update(issuancestatus);
          await _appDBContext.SaveChangesAsync();
        }

        foreach (var detail in issuance.MaterialIssuanceDetails)
        {
          var stock = await _appDBContext.ST_Stocks
              .FirstOrDefaultAsync(s => s.ItemID == detail.ItemID);

          if (stock != null)
          {
            if (stock.Quantity >= detail.IssuanceQuantity)
            {
              stock.Quantity -= detail.IssuanceQuantity; 
            }
            else
            {
              return Json(new { success = false, message = $"Insufficient stock for Item ID {detail.ItemID}." });
            }
          }
          else
          {
            return Json(new { success = false, message = $"Stock not found for Item ID {detail.ItemID}." });
          }

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
      ViewBag.PendingRequisitionsList = await _utils.GetPendingRequisitions();

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
    public async Task<IActionResult> Print(int id)
    {
      var MaterialIssuances = await _appDBContext.ST_MaterialIssuances
           .Include(v => v.MaterialIssuanceDetails)
           .Include(v => v.IssuanceStatusTypes)
           .FirstOrDefaultAsync(v => v.IssuanceID == id);

      if (MaterialIssuances == null)
      {
        return NotFound();
      }


      MaterialIssuances.MaterialIssuanceDetails.Add(new ST_MaterialIssuanceDetail()
      {
        IssuanceID = MaterialIssuances.IssuanceID
      });

      var model = new MaterialIssuancesIndexViewModel
      {
        MaterialIssuances = MaterialIssuances
      };

      var departmentTypeName = await _appDBContext.Settings_DepartmentTypes
        .Where(d => d.DepartmentTypeID == MaterialIssuances.DepartmentTypeID)
        .Select(d => d.DepartmentTypeName)
        .FirstOrDefaultAsync();

      ViewBag.DepartmentTypeName = departmentTypeName;

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return View("~/Views/StoreManagement/StoreManagement/MaterialIssuance/PrintMaterialIssuance.cshtml", model);
    }
    public async Task<IActionResult> PrintList()
    {
      int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
      int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
      var MaterialIssuancesQuery = _appDBContext.ST_MaterialIssuances
        .Include(v => v.MaterialIssuanceDetails)
        .Include(v => v.IssuanceStatusTypes)
        .Where(v => v.DepartmentTypeID == departmentID);

      var MaterialIssuances = await MaterialIssuancesQuery.ToListAsync();

      return View("~/Views/StoreManagement/StoreManagement/MaterialIssuance/PrintListMaterialIssuance.cshtml", MaterialIssuances);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
      int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
      var MaterialIssuancesQuery = _appDBContext.ST_MaterialIssuances
        .Include(v => v.MaterialIssuanceDetails)
        .Include(v => v.IssuanceStatusTypes)
        .Where(v => v.DepartmentTypeID == departmentID);

      var MaterialIssuances = await MaterialIssuancesQuery.ToListAsync();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("MaterialIssuances");
        worksheet.Cells["A1"].Value = "Issuance #";
        worksheet.Cells["B1"].Value = "Issuance Date";
        worksheet.Cells["C1"].Value = "Issuance Status";

        for (int i = 0; i < MaterialIssuances.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = MaterialIssuances[i].IssuanceID;
          worksheet.Cells[i + 2, 2].Value = MaterialIssuances[i].IssuanceDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = MaterialIssuances[i].IssuanceStatusTypes?.IssuanceStatusTypeName;


        }

        worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"MaterialIssuances-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
