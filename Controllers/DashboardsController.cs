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
      await GetTodayAbsentwithInform();
      await GetTodayLateComing();
      return View();
    }
    else
    {
      return RedirectToAction("Login", "Auth");
    }
  }

  private async Task<int> GetTodayLateComing()
  {
    var today = DateTime.Today;

    int lateEmployees = await (
        from face in _appDBContext.CR_FaceAttendances
        join hre in _appDBContext.HR_Employees
            on face.EmployeeID equals hre.EmployeeID
        from gs in _appDBContext.HR_GlobalSettings // cross join
        where EF.Functions.DateDiffDay(face.MarkDate, today) == 0
        let dutyTime = today.AddMinutes(hre.FromDutyTime ?? 0) // convert int? to DateTime
        let graceTime = dutyTime.AddMinutes(gs.LateGraceMinute ?? 0)
        where face.InTime > graceTime
        select face.EmployeeID
    ).CountAsync();

    HttpContext.Session.SetInt32("lateEmployees", lateEmployees);

    return lateEmployees;
  }



  private async Task<int> GetTodayAbsentwithInform()
  {
    DateTime today = DateTime.Today;
    int totalTodayAbsentwithInform = await _appDBContext.HR_Vacations
        .Where(a => a.StartDate.Date >= today && a.StartDate.Date <= today)
        .Select(a => a.EmployeeID)
        .Distinct()
        .CountAsync();

    HttpContext.Session.SetInt32("AbsentWithInform", totalTodayAbsentwithInform);

    return totalTodayAbsentwithInform; // ✅ Return added
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
  public class AttendanceChartDto
  {
    public string MonthName { get; set; }
    public string YearName { get; set; }
    public double PresentCount { get; set; }
    public double AbsentCount { get; set; }
    public double VacationCount { get; set; }
  }

  public async Task<List<AttendanceChartDto>> GetLastSixMonthsAttendance()
  {
    int? wd = await _appDBContext.HR_GlobalSettings
                 .Select(w => w.WorkingDayInWeek)
                 .FirstOrDefaultAsync();

    int workingDaysInWeek = wd ?? 0;

    var sixMonthsAgo = DateTime.Today.AddMonths(-5);

    // Holidays by month
    var holidayData = await _appDBContext.HR_Holidays
        .Where(a => a.HolidayDate >= sixMonthsAgo)
        .GroupBy(a => new { a.HolidayDate.Year, a.HolidayDate.Month })
        .Select(g => new {
          g.Key.Year,
          g.Key.Month,
          HolidayCount = g.Count()
        })
        .ToListAsync();

    // Present Counts by month
    var presentData = await _appDBContext.CR_FaceAttendances
        .Where(a => a.MarkDate >= sixMonthsAgo)
        .GroupBy(a => new { a.MarkDate.Year, a.MarkDate.Month })
        .Select(g => new {
          g.Key.Year,
          g.Key.Month,
          PresentCount = g.Count()
        })
        .ToListAsync();

    int totalActive = await _appDBContext.HR_Employees
         .Where(e => e.ActiveYNID == 1)
         .CountAsync();

    // Create DTO
    List<AttendanceChartDto> list = new List<AttendanceChartDto>();

    // loop each record from presentData
    foreach (var item in presentData)
    {
      int totalDaysInMonth = DateTime.DaysInMonth(item.Year, item.Month);

      // Weekly off days = total weeks * (7 - WorkingDaysInWeek)
      // approx assumption:
      int weeklyOffDays = (7 - workingDaysInWeek) * 4;  // 4 weeks per month assumption

      int holidayCount = holidayData
          .FirstOrDefault(h => h.Year == item.Year && h.Month == item.Month)?.HolidayCount ?? 0;

      int availableDays = totalDaysInMonth - weeklyOffDays - holidayCount;

      // For multiple employees, availableDays * employeeCount
      int totalAvailableEmployeeDays = availableDays * totalActive;

      double presentRatio = 0, absentRatio = 0;
      if (totalAvailableEmployeeDays > 0)
      {
        presentRatio = (item.PresentCount * 100.0) / totalAvailableEmployeeDays;
        absentRatio = 100 - presentRatio;
      }

      list.Add(new AttendanceChartDto
      {
        MonthName = new DateTime(item.Year, item.Month, 1).ToString("MMM"),
        PresentCount = Math.Round(presentRatio, 2),
        AbsentCount = Math.Round(absentRatio, 2)
      });
    }

    // sort
    return list.OrderBy(x => x.MonthName).ToList();
  }



  public async Task<IActionResult> GetAttendanceChartData()
  {
    var data = await GetLastSixMonthsAttendance();
    return Json(data);
  }

  public async Task<List<AttendanceChartDto>> GetLastthreeYearsAttendance()
  {
    int? wd = await _appDBContext.HR_GlobalSettings
                   .Select(w => w.WorkingDayInWeek)
                   .FirstOrDefaultAsync();

    int workingDaysInWeek = wd ?? 0;  // e.g. 5

    int totalActive = await _appDBContext.HR_Employees
       .Where(e => e.ActiveYNID == 1)
       .CountAsync();

    var threeYearsAgo = DateTime.Today.AddYears(-2);

    // Get yearly present counts
    var presentPerYear = await _appDBContext.CR_FaceAttendances
        .Where(a => a.MarkDate >= threeYearsAgo)
        .GroupBy(a => a.MarkDate.Year)
        .Select(g => new {
          Year = g.Key,
          PresentCount = g.Count()
        }).ToListAsync();

    // Yearly vacation days sum
    var vacationPerYear = await _appDBContext.HR_VacationSettles
       .Include(v => v.Vacation)
       .Where(v => v.Vacation.StartDate >= threeYearsAgo && v.Vacation.StartDate <= DateTime.Today)
       .GroupBy(v => v.Vacation.StartDate.Year)
       .Select(g => new {
         Year = g.Key,
         VacationDays = g.Sum(x => x.SettleDays)
       }).ToListAsync();

    // Yearly holidays total
    var holidayPerYear = await _appDBContext.HR_Holidays
       .Where(h => h.HolidayDate >= threeYearsAgo)
       .GroupBy(h => h.HolidayDate.Year)
       .Select(g => new {
         Year = g.Key,
         HolidayCount = g.Count()
       }).ToListAsync();

    // Combine into DTO list
    List<AttendanceChartDto> list = new List<AttendanceChartDto>();

    // Get last 3 years values
    int thisYear = DateTime.Today.Year;
    var yearsRange = new[] { thisYear, thisYear - 1, thisYear - 2 };

    foreach (var year in yearsRange)
    {
      // Total days in year
      int totalDaysInYear = DateTime.IsLeapYear(year) ? 366 : 365;
      // rough weekends: (7 - workingDaysInWeek) * 52 weeks
      int weekends = (7 - workingDaysInWeek) * 52;

      int holidays = holidayPerYear.FirstOrDefault(x => x.Year == year)?.HolidayCount ?? 0;
      int availableDays = totalDaysInYear - weekends - holidays;
      int totalAvailableEmployeeDays = availableDays * totalActive;

      int presentCount = presentPerYear.FirstOrDefault(x => x.Year == year)?.PresentCount ?? 0;
      int vacationDays = vacationPerYear.FirstOrDefault(x => x.Year == year)?.VacationDays ?? 0;

      double presentRatio = 0, absentRatio = 0, vacationRatio = 0;

      if (totalAvailableEmployeeDays > 0)
      {
        presentRatio = (presentCount * 100.0) / totalAvailableEmployeeDays;
        vacationRatio = (vacationDays * 100.0) / totalAvailableEmployeeDays;
        absentRatio = 100 - (presentRatio + vacationRatio); // total = 100
      }

      list.Add(new AttendanceChartDto
      {
        YearName = year.ToString(),
        PresentCount = Math.Round(presentRatio, 2),
        AbsentCount = Math.Round(absentRatio, 2),
        VacationCount = Math.Round(vacationRatio, 2)
      });
    }

    return list.OrderByDescending(x => x.YearName).ToList();
  }

  public async Task<IActionResult> GetPresentRatioByYear(int year)
  {
    var list = await GetLastthreeYearsAttendance();
    var item = list.FirstOrDefault(x => x.YearName == year.ToString());

    var present = item?.PresentCount ?? 0;
    var absent = item?.AbsentCount ?? 0;
    var vacation = item?.VacationCount ?? 0;

    return Json(new { present, absent, vacation });
  }
}
