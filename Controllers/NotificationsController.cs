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
        return Json(new { count = 0, notifications = new List<CR_ProcessTypeApprovalDetail>() });

      // Fetch unread notifications for the logged-in user
      var notifications = await _appDBContext.CR_ProcessTypeApprovalDetails
        .Include(pta => pta.CR_ProcessTypeApproval)
        .ThenInclude(pta => pta.Employee)
        .Include(pta => pta.CR_ProcessTypeApproval)
        .ThenInclude(pta => pta.ProcessType)
        .Include(pta => pta.ProcessTypeApprovalDetailDoc)
        .Where(pta => pta.AppID == 0)
        .CountAsync();

      var count = notifications;

      return Json(new { count = count, notifications = notifications });
    }
  }
}
