using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.StoreManagement.Main
{
  public class ST_Report_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<ST_Report_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public ST_Report_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<ST_Report_DashboardController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index()
    {
      //ViewBag.ApplicantCount = await _appDBContext.HR_Applicants.CountAsync();
      //ViewBag.EmployeeCount = await _appDBContext.HR_Employees.CountAsync();
      //ViewBag.ContractCount = await _appDBContext.HR_Contracts.CountAsync();
      //ViewBag.SalaryCount = await _appDBContext.HR_Salarys.CountAsync();
      //ViewBag.JoiningCount = await _appDBContext.HR_Joinings.CountAsync();
      //ViewBag.BankAccountCount = await _appDBContext.HR_BankAccounts.CountAsync();
      //ViewBag.ContractRenewwalCount = await _appDBContext.HR_ContractRenewals.CountAsync();
      //ViewBag.LeaveBalanceCount = await _appDBContext.leave.CountAsync();
      //ViewBag.CardPrintCount = await _appDBContext.Card.CountAsync();

      return View("~/Views/StoreManagement/Main/Dashboard/ST_Report_Dashboard.cshtml");
    }
  }
}
