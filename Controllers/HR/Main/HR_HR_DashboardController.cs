using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_HR_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HR_HR_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public HR_HR_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HR_HR_DashboardController> localizer)
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
      ViewBag.DeductionCount = await _appDBContext.HR_Deductions.CountAsync();
      ViewBag.OverTimeCount = await _appDBContext.HR_OverTimes.CountAsync();
      ViewBag.AdditionalAllowanceCount = await _appDBContext.HR_AdditionalAllowances.CountAsync();
      ViewBag.HolidayCount = await _appDBContext.HR_Holidays.CountAsync();
      ViewBag.VacationSettleCount = await _appDBContext.HR_VacationSettles.CountAsync();
      ViewBag.EndOfServiceCount = await _appDBContext.HR_EndOfServices.CountAsync();
      ViewBag.EvaluationAssignCount = 0;//await _appDBContext.HR_ContractRenewals.CountAsync();
      ViewBag.EvaluationApprovalCount = 0;//await _appDBContext.HR_ContractRenewals.CountAsync();
      ViewBag.EvaluationListCount = 0;//await _appDBContext.HR_ContractRenewals.CountAsync();
      ViewBag.FaceAttendanceTimeAdjustCount = await _appDBContext.CR_FaceAttendances.CountAsync();
      ViewBag.FaceAttendanceForwardingCount = await _appDBContext.CR_FaceAttendances.CountAsync();


      return View("~/Views/HR/Main/Dashboard/HR_HR_Dashboard.cshtml");
    }
  }
}
