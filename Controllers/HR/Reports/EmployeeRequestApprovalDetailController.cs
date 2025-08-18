using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Exampler_ERP.Models.Temp;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class EmployeeRequestApprovalDetailController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EmployeeRequestApprovalDetailController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeRequestApprovalDetailController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EmployeeRequestApprovalDetailController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }

    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? EmployeeRequestTypeID, int? Status)
    {
      var query = _appDBContext.HR_EmployeeRequestTypeApprovalDetails
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.HR_EmployeeRequestTypeApproval)
              .ThenInclude(pta => pta.EmployeeRequestType)
          .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date >= fromDateTime && pta.HR_EmployeeRequestTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by EmployeeRequestTypeID
      if (EmployeeRequestTypeID.HasValue && EmployeeRequestTypeID != 0)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID == EmployeeRequestTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcessRequest
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }

      

      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();

      
      await PopulateDropdowns(FromDate, ToDate, EmployeeRequestTypeID, EmployeeID, EmployeeName, Status);

      return View("~/Views/HR/Reports/EmployeeRequestApprovalDetail/EmployeeRequestApprovalDetail.cshtml", result);
    }

    private async Task PopulateDropdowns(DateTime? fromDate, DateTime? toDate, int? employeeRequestTypeID, int? employeeID, string? employeeName, int? status)
    {
      ViewBag.FromDate = fromDate;
      ViewBag.ToDate = toDate;
      ViewBag.EmployeeRequestTypeID = employeeRequestTypeID;
      ViewBag.EmployeeID = employeeID;
      ViewBag.EmployeeName = employeeName;
      ViewBag.Status = status;  // Store the selected Status value in ViewBag

      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "" },
                new SelectListItem { Text = "Complete", Value = "1" },
                new SelectListItem { Text = "In Process", Value = "2" },
                new SelectListItem { Text = "Pending", Value = "3" }
            };
    }

    public async Task<IActionResult> ExportToExcel(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? EmployeeRequestTypeID, int? Status)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var query = _appDBContext.HR_EmployeeRequestTypeApprovalDetails
         .Include(pta => pta.HR_EmployeeRequestTypeApproval)
             .ThenInclude(pta => pta.Employee)
         .Include(pta => pta.HR_EmployeeRequestTypeApproval)
             .ThenInclude(pta => pta.EmployeeRequestType)
         .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
         .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date >= fromDateTime && pta.HR_EmployeeRequestTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by EmployeeRequestTypeID
      if (EmployeeRequestTypeID.HasValue && EmployeeRequestTypeID != 0)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID == EmployeeRequestTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcessRequest
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }



      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();


      await PopulateDropdowns(FromDate, ToDate, EmployeeRequestTypeID, EmployeeID, EmployeeName, Status);

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_EmployeeRequestApprovals"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_EmployeeRequestType"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Rank"];
        worksheet.Cells["D1"].Value = _localizer["lbl_PendingDate"];


        for (int i = 0; i < result.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = result[i].HR_EmployeeRequestTypeApproval?.Employee?.FirstName + ' ' + result[i].HR_EmployeeRequestTypeApproval?.Employee?.FatherName + ' ' + result[i].HR_EmployeeRequestTypeApproval?.Employee?.FamilyName;
          worksheet.Cells[i + 2, 2].Value = result[i].HR_EmployeeRequestTypeApproval?.EmployeeRequestType?.EmployeeRequestTypeName;
          worksheet.Cells[i + 2, 3].Value = result[i].Rank;
          worksheet.Cells[i + 2, 4].Value = result[i].Date.ToString("dd/MMM/yyyy");
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_EmployeeRequestApprovals"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? EmployeeRequestTypeID, int? Status)
    {
      var query = _appDBContext.HR_EmployeeRequestTypeApprovalDetails
         .Include(pta => pta.HR_EmployeeRequestTypeApproval)
             .ThenInclude(pta => pta.Employee)
         .Include(pta => pta.HR_EmployeeRequestTypeApproval)
             .ThenInclude(pta => pta.EmployeeRequestType)
         .Include(pta => pta.EmployeeRequestTypeApprovalDetailDoc)
         .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date >= fromDateTime && pta.HR_EmployeeRequestTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by EmployeeRequestTypeID
      if (EmployeeRequestTypeID.HasValue && EmployeeRequestTypeID != 0)
      {
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID == EmployeeRequestTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcessRequest
            ? _appDBContext.HR_EmployeeRequestTypeApprovalSetups
                .Where(setup => setup.EmployeeRequestTypeID == pta.HR_EmployeeRequestTypeApproval.EmployeeRequestTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }



      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();


      await PopulateDropdowns(FromDate, ToDate, EmployeeRequestTypeID, EmployeeID, EmployeeName, Status);

      return View("~/Views/HR/Reports/EmployeeRequestApprovalDetail/PrintEmployeeRequestApprovalDetail.cshtml", result);
    }
  }
}
