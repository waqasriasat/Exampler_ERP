using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AspnetCoreMvcFull.Models;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Exampler_ERP.Controllers;
using Exampler_ERP.Models.Temp;
using Microsoft.AspNetCore.Http;

namespace AspnetCoreMvcFull.Controllers;

public class DashboardsController : PositionController
{
  private readonly AppDBContext _appDBContext;
  private readonly IStringLocalizer<DashboardsController> _localizer;
  private readonly IConfiguration _configuration;
  private readonly Utils _utils;
  private readonly IHubContext<NotificationHub> _hubContext;

  public DashboardsController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<DashboardsController> localizer) 
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
    int? userId = HttpContext.Session.GetInt32("UserID");
    if (userId != null)
    {
      
      await ApprovalProcessCount(); // Ensure it's called before returning the view
      ViewBag.MySession = HttpContext.Session.GetInt32("UserID").ToString();
      var fullName = await GetEmployeeName(userId.Value);
      double? YourUnderEmployee = await GetYourUnderEmployee(userId.Value);
      string percentageStr = YourUnderEmployee?.ToString() ?? "0";
      HttpContext.Session.SetString("EmployeeName", fullName);
      HttpContext.Session.SetString("YourUnderEmployee", percentageStr+'%');
      return View();
    }
    else
    {
      return RedirectToAction("Login", "Auth");
    }
  }

  private async Task<double?> GetYourUnderEmployee(int managerId)
  {
    var totalEmployee = await _appDBContext.HR_Employees
        .Where(e => e.ActiveYNID == 1)
        .CountAsync();
    HttpContext.Session.SetInt32("totalEmployee", totalEmployee);

    var totalActive = await _appDBContext.HR_Employees
        .Where(e => e.ActiveYNID == 1)
        .CountAsync();
    HttpContext.Session.SetInt32("totalActiveEmployee", totalActive);

    var underYou = await _appDBContext.HR_Employees
        .Where(e => e.ActiveYNID == 1 && e.ReportToID == managerId)
        .CountAsync();

    if (totalActive > 0)
    {
      double percentage = ((double)underYou * 100) / totalActive;
      return percentage;
    }

    return null;
  }


  private async Task<string> GetEmployeeName(int userid)
  {
    var employee = await _appDBContext.CR_Users
        .Where(u => u.UserID == userid)
        .Join(_appDBContext.HR_Employees,
            cu => cu.EmployeeID,
            he => he.EmployeeID,
            (cu, he) => new { he.FirstName, he.FatherName, he.FamilyName })
        .FirstOrDefaultAsync();

    if (employee != null)
    {
      return employee.FirstName + " " + employee.FatherName + " " + employee.FamilyName;
    }
    if (employee == null)
    {
      var UserName = HttpContext.Session.GetString("UserName");
      return UserName;
    }
    return null;
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
