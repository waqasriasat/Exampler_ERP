using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Purchase.Main
{
  public class PR_Report_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PR_Report_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public PR_Report_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PR_Report_DashboardController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index()
    {
      //ViewBag.ItemCategoryTypeCount = await _appDBContext.Settings_ItemCategoryTypes.CountAsync();
      //ViewBag.UnitTypeCount = await _appDBContext.Settings_UnitTypes.CountAsync();
      //ViewBag.ItemComponentTypeCount = await _appDBContext.Settings_ItemComponentTypes.CountAsync();
      //ViewBag.ManufacturerTypeCount = await _appDBContext.Settings_ManufacturerTypes.CountAsync();
      //ViewBag.ItemCount = await _appDBContext.ST_Items.CountAsync();
      //ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/Purchase/Main/Dashboard/PR_Report_Dashboard.cshtml");
    }
  }

}
