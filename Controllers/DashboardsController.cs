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
      await GetEmployeeName(userId.Value);
      await GetYourUnderEmployee(userId.Value);
      await GetEmployeesMarkedToday();
      return View();
    }
    else
    {
      return RedirectToAction("Login", "Auth");
    }
  }

  private async Task<int> GetEmployeesMarkedToday()
  {
    DateTime today = DateTime.Today;
    int totalEmployeesMarkedToday = await _appDBContext.CR_FaceAttendances
        .Where(a => a.MarkDate.Date == today)
        .Select(a => a.EmployeeID)
        .Distinct()
        .CountAsync();

    HttpContext.Session.SetInt32("PresentEmployee", totalEmployeesMarkedToday);

    return totalEmployeesMarkedToday; // ✅ Return added
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
      string percentageStr = percentage.ToString() ?? "0";
      HttpContext.Session.SetString("YourUnderEmployee", percentageStr + '%');
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
      var fullName = employee.FirstName + " " + employee.FatherName + " " + employee.FamilyName;
      HttpContext.Session.SetString("EmployeeName", fullName);
      return fullName;

    }
    if (employee == null)
    {
      var UserName = HttpContext.Session.GetString("UserName");
      HttpContext.Session.SetString("EmployeeName", UserName);
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
