using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using OfficeOpenXml;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class ProcessApprovalDetailController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ProcessApprovalDetailController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public ProcessApprovalDetailController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ProcessApprovalDetailController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }

    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? ProcessTypeID, int? Status)
    {
      var query = _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.CR_ProcessTypeApproval.Date >= fromDateTime && pta.CR_ProcessTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by ProcessTypeID
      if (ProcessTypeID.HasValue && ProcessTypeID != 0)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == ProcessTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcess
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;
      ViewBag.Status = Status;  // Store the selected Status value in ViewBag

      var result = await query
          .OrderByDescending(pta => pta.ProcessTypeApprovalDetailID)
          .ToListAsync();

      await PopulateDropdowns(FromDate, ToDate, ProcessTypeID, EmployeeID, EmployeeName, Status);

      return View("~/Views/HR/Reports/ProcessApprovalDetail/ProcessApprovalDetail.cshtml", result);
    }
    private async Task PopulateDropdowns(DateTime? fromDate, DateTime? toDate, int? processTypeID, int? employeeID, string? employeeName, int? status)
    {
      ViewBag.FromDate = fromDate;
      ViewBag.ToDate = toDate;
      ViewBag.ProcessTypeID = processTypeID;
      ViewBag.EmployeeID = employeeID;
      ViewBag.EmployeeName = employeeName;
      ViewBag.Status = status;

      ViewBag.ProcessTypeList = await _utils.GetProcessTypes();
      ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "" },
                new SelectListItem { Text = "Complete", Value = "1" },
                new SelectListItem { Text = "In Process", Value = "2" },
                new SelectListItem { Text = "Pending", Value = "3" }
            };

    }

    public async Task<IActionResult> ExportToExcel(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? ProcessTypeID, int? Status)
    {
      ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

      var query = _appDBContext.CR_ProcessTypeApprovalDetails
           .Include(pta => pta.CR_ProcessTypeApproval)
               .ThenInclude(pta => pta.Employee)
           .Include(pta => pta.CR_ProcessTypeApproval)
               .ThenInclude(pta => pta.ProcessType)
           .Include(pta => pta.ProcessTypeApprovalDetailDoc)
           .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.CR_ProcessTypeApproval.Date >= fromDateTime && pta.CR_ProcessTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by ProcessTypeID
      if (ProcessTypeID.HasValue && ProcessTypeID != 0)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == ProcessTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcess
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;
      ViewBag.Status = Status;  // Store the selected Status value in ViewBag

      var result = await query
          .OrderByDescending(pta => pta.ProcessTypeApprovalDetailID)
          .ToListAsync();

      await PopulateDropdowns(FromDate, ToDate, ProcessTypeID, EmployeeID, EmployeeName, Status);

      using (var package = new ExcelPackage())
      {
        var worksheet = package.Workbook.Worksheets.Add(_localizer["lbl_ProcessApprovals"]);
        worksheet.Cells["A1"].Value = _localizer["lbl_EmployeeName"];
        worksheet.Cells["B1"].Value = _localizer["lbl_ProcessType"];
        worksheet.Cells["C1"].Value = _localizer["lbl_Rank"];
        worksheet.Cells["D1"].Value = _localizer["lbl_PendingDate"];


        for (int i = 0; i < result.Count; i++)
        {
          worksheet.Cells[i + 2, 1].Value = result[i].CR_ProcessTypeApproval?.Employee?.FirstName + ' ' + result[i].CR_ProcessTypeApproval?.Employee?.FatherName + ' ' + result[i].CR_ProcessTypeApproval?.Employee?.FamilyName;
          worksheet.Cells[i + 2, 2].Value = result[i].CR_ProcessTypeApproval?.ProcessType?.ProcessTypeName;
          worksheet.Cells[i + 2, 3].Value = result[i].Rank;
          worksheet.Cells[i + 2, 4].Value = result[i].Date.ToString("dd/MMM/yyyy");
        }

        worksheet.Cells["A1:C1"].Style.Font.Bold = true;
        worksheet.Cells.AutoFitColumns();

        var stream = new MemoryStream();
        package.SaveAs(stream);
        stream.Position = 0;
        string excelName = _localizer["lbl_ProcessApprovals"] + $"-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

        return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
      }
    }
    public async Task<IActionResult> Print(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? ProcessTypeID, int? Status)
    {
      var query = _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
              .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0);

      if (FromDate.HasValue && ToDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.CR_ProcessTypeApproval.Date >= fromDateTime && pta.CR_ProcessTypeApproval.Date <= toDateTime);
      }

      // Filter by EmployeeID
      if (EmployeeID.HasValue)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.EmployeeID == EmployeeID.Value);
      }

      // Filter by ProcessTypeID
      if (ProcessTypeID.HasValue && ProcessTypeID != 0)
      {
        query = query.Where(pta => pta.CR_ProcessTypeApproval.ProcessTypeID == ProcessTypeID.Value);
      }

      // Add status filtering
      if (Status.HasValue)
      {
        query = query.Where(pta =>
            Status.Value == 1 // Complete
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank == pta.Rank)
                .Any()
            : Status.Value == 2 // InProcess
            ? _appDBContext.CR_ProcessTypeApprovalSetups
                .Where(setup => setup.ProcessTypeID == pta.CR_ProcessTypeApproval.ProcessTypeID && setup.Rank > pta.Rank)
                .Any()
            : Status.Value == 3 // Pending
            ? pta.Rank == 1
            : false);
      }

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.ProcessTypeID = ProcessTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;
      ViewBag.Status = Status;  // Store the selected Status value in ViewBag

      var result = await query
          .OrderByDescending(pta => pta.ProcessTypeApprovalDetailID)
          .ToListAsync();

      await PopulateDropdowns(FromDate, ToDate, ProcessTypeID, EmployeeID, EmployeeName, Status);

      return View("~/Views/HR/Reports/ProcessApprovalDetail/PrintProcessApprovalDetail.cshtml", result);
    }
  }
}
