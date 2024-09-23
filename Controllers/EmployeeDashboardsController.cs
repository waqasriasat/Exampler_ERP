using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers
{
  public class EmployeeDashboardsController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public EmployeeDashboardsController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
    }
    public async Task<IActionResult> Index()
    {
      if (HttpContext.Session.GetInt32("EmployeeID") != null)
      {
        await EmployeeRequestsCount(); // Ensure it's called before returning the view
        ViewBag.MySession = HttpContext.Session.GetInt32("EmployeeID").ToString();
        return View();
      }
      else
      {
        return RedirectToAction("EmployeeLogin", "Auth");
      }
    }
    private async Task EmployeeRequestsCount()
    {
      int count = await _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
          .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
          .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0)
          .CountAsync();

      ViewBag.EmployeeRequestsCount = count;

    }
  }
}
