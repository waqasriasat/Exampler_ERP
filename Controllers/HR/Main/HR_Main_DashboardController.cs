using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_Main_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HR_Main_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;



    public HR_Main_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HR_Main_DashboardController> localizer) 
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
      ViewBag.BranchCount = await _appDBContext.Settings_BranchTypes.CountAsync();
      ViewBag.DepartmentCount = await _appDBContext.Settings_DepartmentTypes.CountAsync();
      ViewBag.SectionCount = await _appDBContext.Settings_SectionTypes.CountAsync();
      ViewBag.QualificationCount = await _appDBContext.Settings_QualificationTypes.CountAsync();
      ViewBag.SubQualificationCount = await _appDBContext.Settings_SubQualificationTypes.CountAsync();
      ViewBag.DesignationCount = await _appDBContext.Settings_DesignationTypes.CountAsync();
      ViewBag.SalaryTypeCount = await _appDBContext.Settings_SalaryTypes.CountAsync();
      ViewBag.DeductionTypeCount = await _appDBContext.Settings_DeductionTypes.CountAsync();
      ViewBag.DeductionSetupCount = await _appDBContext.HR_DeductionSetups.CountAsync();
      ViewBag.EmployeeStatusTypeCount = await _appDBContext.Settings_EmployeeStatusTypes.CountAsync();
      ViewBag.VacationTypeCount = await _appDBContext.Settings_VacationTypes.CountAsync();
      ViewBag.ProcessTypeCount = await _appDBContext.Settings_ProcessTypes.CountAsync();
      ViewBag.ProcessTypeApprovalSetupCount = await _appDBContext.CR_ProcessTypeApprovalSetups.CountAsync();
      ViewBag.ProcessTypeForwardCount = await _appDBContext.CR_ProcessTypeForwards.CountAsync();
      ViewBag.RolesCount = await _appDBContext.Settings_RoleTypes.CountAsync();
      ViewBag.UsersCount = await _appDBContext.CR_Users.CountAsync();
      ViewBag.AdditionalAllowanceTypeCount = await _appDBContext.Settings_AdditionalAllowanceTypes.CountAsync();
      ViewBag.OvertimeRateCount = await _appDBContext.Settings_OverTimeRates.CountAsync();
      ViewBag.EmployeeRequestTypeCount = await _appDBContext.Settings_EmployeeRequestTypes.CountAsync();
      ViewBag.EmployeeRequestSetupCount = await _appDBContext.HR_EmployeeRequestTypeApprovalSetups.CountAsync();
      ViewBag.EmployeeRequestForwordCount = await _appDBContext.HR_EmployeeRequestTypeForwards.CountAsync();
      ViewBag.HolidayTypeCount = await _appDBContext.Settings_HolidayTypes.CountAsync();
      //ViewBag.EvaluationTemplateCount = await _appDBContext.Branchs.CountAsync();
      return View("~/Views/HR/Main/Dashboard/HR_Main_Dashboard.cshtml");
    }
  }
}
