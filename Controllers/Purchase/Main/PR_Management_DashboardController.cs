using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.Purchase.Main
{
  public class PR_Management_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<PR_Management_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public PR_Management_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<PR_Management_DashboardController> localizer) 
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
      ViewBag.PurchaseRequestCount = await _appDBContext.PR_PurchaseRequests.CountAsync();
      ViewBag.RequestForQuotationCount = await _appDBContext.PR_PurchaseRequests.Where(v => v.RequestStatusTypeID == 2).CountAsync();
      ViewBag.CostComparisonCount = await _appDBContext.PR_PurchaseRequests.Where(v => v.RequestStatusTypeID == 5).CountAsync();
      ViewBag.PurchaseOrderCount = await _appDBContext.PR_PurchaseRequests.Where(v => v.RequestStatusTypeID == 6).CountAsync();
      //ViewBag.ItemCount = await _appDBContext.ST_Items.CountAsync();
      //ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/Purchase/Main/Dashboard/PR_Management_Dashboard.cshtml");
    }
  }
}
