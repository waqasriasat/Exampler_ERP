using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_Finance_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HR_HR_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public HR_Finance_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HR_HR_DashboardController> localizer)
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
      ViewBag.WorkDayCount = await _appDBContext.HR_WorkDays.CountAsync();
      ViewBag.FixedDeductionCount = await _appDBContext.HR_FixedDeductions.CountAsync();
      ViewBag.HourlySalarySheetCount = 0;//await _appDBContext.HR_AdditionalAllowances.CountAsync();
      ViewBag.MonthlySalarySheetCount = await _appDBContext.HR_MonthlyPayroll_Salarys.CountAsync();
      return View("~/Views/HR/Main/Dashboard/HR_Finance_Dashboard.cshtml");
    }
  }
}
