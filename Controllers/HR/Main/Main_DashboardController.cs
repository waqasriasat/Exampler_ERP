using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class Main_DashboardController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;


    public Main_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
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
      ViewBag.AddionalAllowanceTypeCount = await _appDBContext.Settings_AddionalAllowanceTypes.CountAsync();
      ViewBag.OvertimeRateCount = await _appDBContext.Settings_OverTimeRates.CountAsync();
      ViewBag.EmployeeRequestTypeCount = await _appDBContext.Settings_EmployeeRequestTypes.CountAsync();
      //ViewBag.AccessRightsCount = await _appDBContext.Branchs.CountAsync();
      //ViewBag.CopyAccessRightsCount = await _appDBContext.Branchs.CountAsync();
      //ViewBag.EvaluationQuestionsCount = await _appDBContext.Branchs.CountAsync();
      //ViewBag.EvaluationTemplateCount = await _appDBContext.Branchs.CountAsync();
      return View("~/Views/HR/Main/Dashboard/Main_Dashboard.cshtml");
    }
  }
}
