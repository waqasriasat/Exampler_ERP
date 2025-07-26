using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Exampler_ERP.Controllers.MasterInfo;

namespace Exampler_ERP.Controllers
{
  public class NotificationsController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<NotificationsController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationsController(AppDBContext appDBContext, IConfiguration configuration, IHubContext<NotificationHub> hubContext, IStringLocalizer<NotificationsController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _hubContext = hubContext;
      _localizer = localizer;
    }

    public async Task<IActionResult> GetProcessNotifications()
    {
      var userId = HttpContext.Session.GetInt32("UserID");

      if (userId == null)
        return Json(new { count = 0, notifications = new List<object>() });

      // Fetch unread notifications for the logged-in user
      var processCounts = await _appDBContext.CR_ProcessTypeApprovalDetails
          .Where(pta => pta.AppID == 0)
          .Join(_appDBContext.CR_ProcessTypeApprovals,
              pta => pta.ProcessTypeApprovalID,
              cta => cta.ProcessTypeApprovalID,
              (pta, cta) => new { pta, cta })
          .Join(_appDBContext.HR_Employees,
              combined => combined.cta.EmployeeID,
              e => e.EmployeeID,
              (combined, e) => new { combined.pta, combined.cta, e })
          .Join(_appDBContext.Settings_ProcessTypes,
              combined => combined.cta.ProcessTypeID,
              pt => pt.ProcessTypeID,
              (combined, pt) => new { combined.pta, combined.cta, combined.e, pt })
          .GroupBy(x => x.cta.ProcessTypeID)
          .Select(g => new
          {
            ProcessTypeID = g.Key,
            ProcessCount = g.Count(x => x.pta.ProcessTypeApprovalID != null) // Count only non-null ProcessTypeApprovalIDs
          })
          .ToListAsync();

      // Calculate the total count of notifications
      var totalCount = processCounts.Sum(pc => pc.ProcessCount);

      // Return the result with only ProcessTypeID and ProcessCount
      return Json(new { count = totalCount, notifications = processCounts });
    }
    public async Task<IActionResult> GetEmployeeRequestNotifications()
    {
      var userId = HttpContext.Session.GetInt32("UserID");

      if (userId == null)
        return Json(new { count = 0, notifications = new List<object>() });

      // Fetch unread notifications for the logged-in user
      var EmployeeRequestCounts = await _appDBContext.HR_EmployeeRequestTypeApprovalDetails
          .Where(pta => pta.AppID == 0)
          .Join(_appDBContext.HR_EmployeeRequestTypeApprovals,
              pta => pta.EmployeeRequestTypeApprovalID,
              cta => cta.EmployeeRequestTypeApprovalID,
              (pta, cta) => new { pta, cta })
          .Join(_appDBContext.HR_Employees,
              combined => combined.cta.EmployeeID,
              e => e.EmployeeID,
              (combined, e) => new { combined.pta, combined.cta, e })
          .Join(_appDBContext.Settings_EmployeeRequestTypes,
              combined => combined.cta.EmployeeRequestTypeID,
              pt => pt.EmployeeRequestTypeID,
              (combined, pt) => new { combined.pta, combined.cta, combined.e, pt })
          .GroupBy(x => x.cta.EmployeeRequestTypeID)
          .Select(g => new
          {
            EmployeeRequestTypeID = g.Key,
            EmployeeRequestCount = g.Count(x => x.pta.EmployeeRequestTypeApprovalID != null) // Count only non-null EmployeeRequestTypeApprovalIDs
          })
          .ToListAsync();

      // Calculate the total count of notifications
      var totalCount = EmployeeRequestCounts.Sum(pc => pc.EmployeeRequestCount);

      // Return the result with only EmployeeRequestTypeID and EmployeeRequestCount
      return Json(new { count = totalCount, notifications = EmployeeRequestCounts });
    }

  }
}
