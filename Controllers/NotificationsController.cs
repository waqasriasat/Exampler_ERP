using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers
{
  public class NotificationsController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;

    public NotificationsController(AppDBContext appDBContext, IConfiguration configuration)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
    }

    public async Task<IActionResult> GetNotifications()
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

  }
}
