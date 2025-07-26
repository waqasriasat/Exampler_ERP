using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Finance.Main
{
  public class FI_Management_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<FI_Management_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public FI_Management_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<FI_Management_DashboardController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    //public async Task<IActionResult> Index()
    public async Task<IActionResult> Index()
    {
      ViewBag.Bank = await _appDBContext.FI_Banks.CountAsync();
      ViewBag.ChequeBookCount = await _appDBContext.FI_ChequeBooks.CountAsync();
      ViewBag.VendorCount = await _appDBContext.FI_Vendors.CountAsync();
      return View("~/Views/Finance/Main/Dashboard/FI_Management_Dashboard.cshtml");
    }
  }
}
