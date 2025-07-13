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

namespace Exampler_ERP.Controllers.StoreManagement.StoreManagement
{
  public class MaterialRequisitionController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<MaterialRequisitionController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public MaterialRequisitionController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<MaterialRequisitionController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;


    }
    public async Task<IActionResult> Index(string searchItemName)
    {
      if (HttpContext.Session.GetInt32("UserID") != null)
      {
        int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
        int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
        var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .Include(v => v.RequisitionStatusTypes)
          .Where(v => v.DepartmentTypeID == departmentID);

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
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "No Material Requisition found with the name '" + searchItemName + "'. Please check the name and try again.");
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

      ST_MaterialRequisition Requisitions = new ST_MaterialRequisition();
      Requisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail() { RequisitionID = 0 });
      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = Requisitions
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
          int userID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
          int departmentID = await _utils.PostUserIDGetDepartmentID(userID);
          model.MaterialRequisitions.DepartmentTypeID = departmentID;
          model.MaterialRequisitions.FinalApprovalID = 0;
          model.MaterialRequisitions.RequisitionStatusTypeID = 1;
          model.MaterialRequisitions.RequisitionDate = DateTime.Now;
          _appDBContext.ST_MaterialRequisitions.Add(model.MaterialRequisitions);
          //await _appDBContext.SaveChangesAsync();

          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == 0);
          model.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

          foreach (var detail in model.MaterialRequisitions.MaterialRequisitionDetails)
          {
            detail.RequisitionID = model.MaterialRequisitions.RequisitionID;
            _appDBContext.ST_MaterialRequisitionDetails.Add(detail);
          }

          await _appDBContext.SaveChangesAsync();

          var RequisitionID = model.MaterialRequisitions.RequisitionID;

          var MaterialRequisitionStatus = new ST_MaterialRequisitionStatus
          {
            RequisitionID = RequisitionID,
            ActionDate = DateTime.Now,
            ActionID = HttpContext.Session.GetInt32("UserID") ?? default(int),
            ActionStatusTypeID = 1
          };

          _appDBContext.ST_MaterialRequisitionStatuss.Add(MaterialRequisitionStatus);

          await _appDBContext.SaveChangesAsync();


          if (RequisitionID > 0)
          {
            var processCount = await _appDBContext.CR_ProcessTypeApprovalSetups
                                .Where(pta => pta.ProcessTypeID > 0 && pta.ProcessTypeID == 20)
                                .CountAsync();

            if (processCount > 0)
            {

              var newProcessTypeApproval = new CR_ProcessTypeApproval
              {
                ProcessTypeID = 20,
                Notes = "Pending New Material Requisition",
                Date = DateTime.Now,
                EmployeeID = await _utils.PostUserIDGetEmployeeID(userID),
                UserID = HttpContext.Session.GetInt32("UserID") ?? default(int),
                TransactionID = RequisitionID
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
                await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
              }
              else
              {
                return Json(new { success = false, message = "Next approval setup not found." });
              }
            }
            else
            {
              model.MaterialRequisitions.FinalApprovalID = 1;
              model.MaterialRequisitions.RequisitionStatusTypeID = 2;
              _appDBContext.ST_MaterialRequisitions.Update(model.MaterialRequisitions);

              MaterialRequisitionStatus.ActionStatusTypeID = 2;
              _appDBContext.ST_MaterialRequisitionStatuss.Update(MaterialRequisitionStatus);

              await _appDBContext.SaveChangesAsync();
              await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", " Material Requisition successfully. No process setup found,  Material Requisition Approved.");
              return Json(new { success = true, message = "No process setup found,  Material Requisition Approved." });
            }
          }
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessTrue", " Material Requisition Created successfully. Continue to the Approval Process Setup for  Material Requisition Approved.");

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
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
      var MaterialRequisitions = await _appDBContext.ST_MaterialRequisitions
          .Include(v => v.MaterialRequisitionDetails)
          .FirstOrDefaultAsync(v => v.RequisitionID == id);

      if (MaterialRequisitions == null)
      {
        return NotFound();
      }

      // Check RequisitionStatusTypeID
      if (MaterialRequisitions.RequisitionStatusTypeID != 1)
      {
        await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "After approval, editing is not allowed.....");

      }

      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail()
      {
        RequisitionID = MaterialRequisitions.RequisitionID
      });

      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/EditMaterialRequisition.cshtml", model);
    }
    [HttpPost]
    public async Task<IActionResult> Edit(MaterialRequisitionsIndexViewModel MaterialRequisition)
    {
      if (ModelState.IsValid)
      {

        var existingMaterialRequisition = await _appDBContext.ST_MaterialRequisitions
            .Include(v => v.MaterialRequisitionDetails)
            .FirstOrDefaultAsync(v => v.RequisitionID == MaterialRequisition.MaterialRequisitions.RequisitionID);
        if (existingMaterialRequisition?.RequisitionStatusTypeID != 1)
        {
          await _hubContext.Clients.All.SendAsync("ReceiveSuccessFalse", "After approval, editing is not allowed.....");

        }
        else
        {
          if (existingMaterialRequisition != null)
          {
            existingMaterialRequisition.Remarks = MaterialRequisition.MaterialRequisitions.Remarks;

            _appDBContext.Update(existingMaterialRequisition);
            await _appDBContext.SaveChangesAsync();

            var MaterialRequisitionDetailsToRemove = _appDBContext.ST_MaterialRequisitionDetails
            .Where(v => v.RequisitionID == MaterialRequisition.MaterialRequisitions.RequisitionID)
            .ToList();

            _appDBContext.ST_MaterialRequisitionDetails.RemoveRange(MaterialRequisitionDetailsToRemove);

            await _appDBContext.SaveChangesAsync();
            MaterialRequisition.MaterialRequisitions.MaterialRequisitionDetails.RemoveAll(e => e.ItemID == null || e.ItemID == 0);

            foreach (var detail in MaterialRequisition.MaterialRequisitions.MaterialRequisitionDetails)
            {
              detail.RequisitionID = MaterialRequisition.MaterialRequisitions.RequisitionID;
              _appDBContext.ST_MaterialRequisitionDetails.Add(detail);
            }

            await _appDBContext.SaveChangesAsync();

            return Json(new { success = true, message = "Received MaterialRequisition Edited successfully!" });
          }
          else
          {
            return NotFound();
          }
        }
      }

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return PartialView("~/Views/StoreManagement/StoreManagement/MaterialRequisition/EditMaterialRequisition.cshtml", MaterialRequisition);
    }
    public async Task<IActionResult> Print(int id)
    {
      var MaterialRequisitions = await _appDBContext.ST_MaterialRequisitions
           .Include(v => v.MaterialRequisitionDetails)
           .Include(v => v.RequisitionStatusTypes)
           .FirstOrDefaultAsync(v => v.RequisitionID == id);

      if (MaterialRequisitions == null)
      {
        return NotFound();
      }


      MaterialRequisitions.MaterialRequisitionDetails.Add(new ST_MaterialRequisitionDetail()
      {
        RequisitionID = MaterialRequisitions.RequisitionID
      });

      var model = new MaterialRequisitionsIndexViewModel
      {
        MaterialRequisitions = MaterialRequisitions
      };

      var departmentTypeName = await _appDBContext.Settings_DepartmentTypes
        .Where(d => d.DepartmentTypeID == MaterialRequisitions.DepartmentTypeID)
        .Select(d => d.DepartmentTypeName)
        .FirstOrDefaultAsync();

      // Store DepartmentTypeName in ViewBag
      ViewBag.DepartmentTypeName = departmentTypeName;

      ViewBag.ItemList = await _utils.GetItemList();
      ViewBag.ItemNameList = await _utils.GetItemList();

      return View("~/Views/StoreManagement/StoreManagement/MaterialRequisition/PrintMaterialRequisition.cshtml", model);
    }
    public async Task<IActionResult> PrintList()
    {
      int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
      int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
      var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
        .Include(v => v.MaterialRequisitionDetails)
        .Include(v => v.RequisitionStatusTypes)
        .Where(v => v.DepartmentTypeID == departmentID);

      var MaterialRequisitions = await MaterialRequisitionsQuery.ToListAsync();

      return View("~/Views/StoreManagement/StoreManagement/MaterialRequisition/PrintListMaterialRequisition.cshtml", MaterialRequisitions);
    }
    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      int UserID = int.Parse(HttpContext.Session.GetInt32("UserID").ToString());
      int departmentID = await _utils.PostUserIDGetDepartmentID(UserID);
      var MaterialRequisitionsQuery = _appDBContext.ST_MaterialRequisitions
        .Include(v => v.MaterialRequisitionDetails)
        .Include(v => v.RequisitionStatusTypes)
        .Where(v => v.DepartmentTypeID == departmentID);

      var MaterialRequisitions = await MaterialRequisitionsQuery.ToListAsync();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add("MaterialRequisitions");
        worksheet.Cells["A1"].Value = "Requisition #";
        worksheet.Cells["B1"].Value = "Requisition Date";
        worksheet.Cells["C1"].Value = "Requisition Status";

        for (int i = 0; i < MaterialRequisitions.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = MaterialRequisitions[i].RequisitionID;
          worksheet.Cells[i + 2, 2].Value = MaterialRequisitions[i].RequisitionDate.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 3].Value = MaterialRequisitions[i].RequisitionStatusTypes?.RequisitionStatusTypeName;


        }

        worksheet.Cells["B1"].Style.Numberformat.Format = "dd-mmm-yyyy";
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = $"MaterialRequisitions-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
