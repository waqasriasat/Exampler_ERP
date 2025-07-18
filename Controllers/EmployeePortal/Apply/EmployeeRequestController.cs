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
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.EmployeePortal.Apply
{
  public class EmployeeRequestController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EmployeeRequestController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeRequestController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EmployeeRequestController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/EmployeeRequest/EmployeeRequest.cshtml", EmployeeRequests);
    }
    public async Task<IActionResult> Edit(int id)
    {
      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals
      .Where(v => v.EmployeeRequestTypeApprovalID == id)
                                   .Include(c => c.Employee)
                                   .Include(c => c.EmployeeRequestType)
                                   .FirstOrDefaultAsync();

      return PartialView("~/Views/EmployeePortal/Apply/EmployeeRequest/EditEmployeeRequest.cshtml", EmployeeRequests);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(HR_EmployeeRequestTypeApproval EmployeeRequest)
    {
      if (ModelState.IsValid)
      {

        _appDBContext.Update(EmployeeRequest);
        await _appDBContext.SaveChangesAsync();
        return Json(new { success = true });
      }
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();

      return PartialView("~/Views/EmployeePortal/Apply/EmployeeRequest/AddEmployeeRequest.cshtml", new HR_EmployeeRequestTypeApproval());
    }

    [HttpPost]
    public async Task<IActionResult> Create(HR_EmployeeRequestTypeApproval EmployeeRequest)
    {
      if (ModelState.IsValid)
      {
        EmployeeRequest.Date = DateTime.Now;


        // Add the EmployeeRequest entry to the database
        _appDBContext.Add(EmployeeRequest);
        await _appDBContext.SaveChangesAsync();

        var EmployeeRequestTypeID = EmployeeRequest.EmployeeRequestTypeID;
        var EmployeeRequestTypeApprovalID = EmployeeRequest.EmployeeRequestTypeApprovalID;
        if (EmployeeRequestTypeID > 0)
        {
          var EmployeeRequestCount = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                             .Where(pta => pta.EmployeeRequestTypeID > 0 && pta.EmployeeRequestTypeID == EmployeeRequestTypeID)
                             .CountAsync();

          if (EmployeeRequestCount > 0)
          {
            var nextApprovalSetup = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                                     .Where(pta => pta.EmployeeRequestTypeID == EmployeeRequestTypeID && pta.Rank == 1)
                                     .FirstOrDefaultAsync();

            if (nextApprovalSetup != null)
            {
              var newEmployeeRequestTypeApprovalDetail = new HR_EmployeeRequestTypeApprovalDetail
              {
                EmployeeRequestTypeApprovalID = EmployeeRequestTypeApprovalID,
                Date = DateTime.Now,
                RoleID = nextApprovalSetup.RoleTypeID,
                AppID = 0,
                AppUserID = 0,
                Notes = null,
                Rank = nextApprovalSetup.Rank
              };

              _appDBContext.HR_EmployeeRequestTypeApprovalDetails.Add(newEmployeeRequestTypeApprovalDetail);
              await _appDBContext.SaveChangesAsync();
              await _hubContext.Clients.All.SendAsync("ReceiveProcessNotification");
              return Json(new { success = true });
            }
            else
            {
              return Json(new { success = true });
            }
          }
          else
          {
            EmployeeRequest.FinalApprovalID = 1;
            _appDBContext.HR_EmployeeRequestTypeApprovals.Update(EmployeeRequest);
            await _appDBContext.SaveChangesAsync();
            return Json(new { success = true });
          }
        }

        return Json(new { success = true });
      }

      // If the model state is invalid, return a failure response with validation errors
      return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
    }


    public async Task<IActionResult> Delete(int id)
    {
      var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals.FindAsync(id);
      if (EmployeeRequests == null)
      {
        return NotFound();
      }

      EmployeeRequests.DeleteYNID = 1;

      _appDBContext.HR_EmployeeRequestTypeApprovals.Update(EmployeeRequests);
      await _appDBContext.SaveChangesAsync();

      return Json(new { success = true });
    }

    public async Task<IActionResult> Print()
    {
      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();

      return View("~/Views/EmployeePortal/Apply/EmployeeRequest/PrintEmployeeRequest.cshtml", EmployeeRequests);
    }

    public async Task<IActionResult> ExportToExcel()
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var employeeID = HttpContext.Session.GetInt32("EmployeeID");

      var EmployeeRequests = await _appDBContext.HR_EmployeeRequestTypeApprovals
                                   .Where(v => employeeID != null && v.EmployeeID == employeeID && v.DeleteYNID != 1)
                                   .OrderByDescending(v => v.EmployeeRequestTypeID)
                                   .Include(c => c.Employee)
                                    .Include(c => c.EmployeeRequestType)
                                   .ToListAsync();


      var EmployeeRequestsList = await _utils.GetEmployeeRequestTypes();
      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_EmployeeRequest"]);

        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeRequest"]  + " " + _localizer["lbl_ID"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["C1"].Value = _localizer["lbl_EmployeeRequest"]  + " " + _localizer["lbl_Type"];
        worksheet.Cells["D1"].Value = _localizer["lbl_ApplyDate"];
        worksheet.Cells["E1"].Value = _localizer["lbl_Note"];


        for (int i = 0; i < EmployeeRequests.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = EmployeeRequests[i].EmployeeRequestTypeID;
          worksheet.Cells[i + 2, 2].Value = EmployeeRequests[i].Employee?.FirstName + ' ' + EmployeeRequests[i].Employee?.FatherName + ' ' + EmployeeRequests[i].Employee?.FamilyName;
          worksheet.Cells[i + 2, 3].Value = EmployeeRequests[i].EmployeeRequestTypeID == 0 || EmployeeRequests[i]?.EmployeeRequestTypeID == null
          ? ""
          : EmployeeRequestsList.FirstOrDefault(g => g.Value == EmployeeRequests[i].EmployeeRequestTypeID.ToString())?.Text;
          worksheet.Cells[i + 2, 4].Value = EmployeeRequests[i].Date.ToString("dd-MMM-yyyy");
          worksheet.Cells[i + 2, 5].Value = EmployeeRequests[i].Notes;


        }

        worksheet.Cells["B1:G1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_EmployeeRequest"]+$"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
  }
}
