using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class EmployeeRequestApprovalDetailController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public EmployeeRequestApprovalDetailController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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

      if (FromDate.HasValue)
      {
        var fromDateTime = FromDate.Value.Date.AddSeconds(1);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date >= fromDateTime);
      }

      if (ToDate.HasValue)
      {
        var toDateTime = ToDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59);
        query = query.Where(pta => pta.HR_EmployeeRequestTypeApproval.Date <= toDateTime);
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

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.EmployeeRequestTypeID = EmployeeRequestTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;
      ViewBag.Status = Status;  // Store the selected Status value in ViewBag

      var result = await query
          .OrderByDescending(pta => pta.EmployeeRequestTypeApprovalDetailID)
          .ToListAsync();

      ViewBag.EmployeeRequestTypeList = await _utils.GetEmployeeRequestTypes();
      ViewBag.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Text = "Select Status", Value = "" },
                new SelectListItem { Text = "Complete", Value = "1" },
                new SelectListItem { Text = "In Process", Value = "2" },
                new SelectListItem { Text = "Pending", Value = "3" }
            };


      return View("~/Views/HR/Reports/EmployeeRequestApprovalDetail/EmployeeRequestApprovalDetail.cshtml", result);
    }

  }
}
