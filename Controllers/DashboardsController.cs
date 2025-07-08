using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : Controller
{
  private readonly AppDBContext _appDBContext;
  private readonly IConfiguration _configuration;
  private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

  public DashboardsController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
  {
    _appDBContext = appDBContext;
    _configuration = configuration;
    _utils = utils;
_hubContext = hubContext;
 
  }
  public async Task<IActionResult> Index()
  {
    if (HttpContext.Session.GetInt32("UserID") != null)
    {
      await ApprovalProcessCount(); // Ensure it's called before returning the view
      ViewBag.MySession = HttpContext.Session.GetInt32("UserID").ToString();
      return View();
    }
    else
    {
      return RedirectToAction("Login", "Auth");
    }
  }
  private async Task ApprovalProcessCount()
  {
    int count = await _appDBContext.CR_ProcessTypeApprovalDetails
        .Include(pta => pta.CR_ProcessTypeApproval)
        .ThenInclude(pta => pta.Employee)
        .Include(pta => pta.CR_ProcessTypeApproval)
        .ThenInclude(pta => pta.ProcessType)
        .Include(pta => pta.ProcessTypeApprovalDetailDoc)
        .Where(pta => pta.AppID == 0)
        .CountAsync();

    ViewBag.ApprovalProcessCount = count;

  }

 

}
