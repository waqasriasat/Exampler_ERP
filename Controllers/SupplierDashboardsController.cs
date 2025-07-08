using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers
{
  public class SupplierDashboardsController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    
    public SupplierDashboardsController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
      
    }
    public async Task<IActionResult> Index()
    {
      if (HttpContext.Session.GetInt32("SupplierID") != null)
      {
        await SupplierRequestsCount(); // Ensure it's called before returning the view
        ViewBag.MySession = HttpContext.Session.GetInt32("SupplierID").ToString();
        return View();
      }
      else
      {
        return RedirectToAction("SupplierLogin", "Auth");
      }
    }
    private async Task SupplierRequestsCount()
    {
      int count = await _appDBContext.CR_ProcessTypeApprovalDetails
          .Include(pta => pta.CR_ProcessTypeApproval)
          .ThenInclude(pta => pta.Employee)
          .Include(pta => pta.CR_ProcessTypeApproval)
          .ThenInclude(pta => pta.ProcessType)
          .Include(pta => pta.ProcessTypeApprovalDetailDoc)
          .Where(pta => pta.AppID == 0)
          .CountAsync();

      ViewBag.SupplierRequestsCount = count;

    }
  }
}
