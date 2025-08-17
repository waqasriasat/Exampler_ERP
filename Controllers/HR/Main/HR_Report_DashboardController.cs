using Exampler_ERP.Hubs;
using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Main
{
  public class HR_Report_DashboardController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<HR_Report_DashboardController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;
    public HR_Report_DashboardController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<HR_Report_DashboardController> localizer)
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
      var today = DateTime.Today;

      // Fetch employees with vacation balances
      var employeeVacationData = await _appDBContext.HR_Employees
          .AsNoTracking()
          .Select(emp => new
          {
            emp.EmployeeID,
            EmployeeName = emp.FirstName + " " + emp.FatherName + " " + emp.FamilyName,

            HaveVacation = _appDBContext.HR_Contracts
                  .Where(c => c.EmployeeID == emp.EmployeeID)
                  .Select(c => EF.Functions.DateDiffYear(c.StartDate, c.EndDate ?? today) * c.VacationDays)
                  .Sum() ?? 0,

            PaidVacation = _appDBContext.HR_VacationSettles
                  .Where(v =>
                      v.FinalApprovalID == 1 &&
                      v.Vacation != null &&
                      v.Vacation.EmployeeID == emp.EmployeeID &&
                      v.Vacation.FinalApprovalID == 1)
                  .Select(v => (double?)v.SettleDays)
                  .Sum() ?? 0
          })
          .ToListAsync();

      // Fetch active contracts
      var activeContracts = await _appDBContext.HR_Contracts
          .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1)
          .Include(c => c.Employee)
              .ThenInclude(e => e.BranchType)
          .Include(c => c.Employee)
              .ThenInclude(e => e.DepartmentType)
          .Include(c => c.Settings_ContractType)
          .Select(c => new ContractEndDaysViewModel
          {
            EmployeeName = c.Employee.FirstName + " " + c.Employee.FatherName + " " + c.Employee.FamilyName,
            BranchName = c.Employee.BranchType.BranchTypeName,
            DepartmentName = c.Employee.DepartmentType.DepartmentTypeName,
            ContractTypeName = c.Settings_ContractType.ContractTypeName,
            EndDays = c.EndDate.HasValue ? (c.EndDate.Value - DateTime.Now).Days : 0
          })
          .ToListAsync();

      // ViewBag assignments
      ViewBag.EmployeeBioDataCount = await _appDBContext.HR_Employees.CountAsync();
      ViewBag.VacationReportCount = await _appDBContext.HR_Vacations.CountAsync();

      // Vacation Balance calculation
      double totalHaveVacation = employeeVacationData.Sum(e => e.HaveVacation);
      double totalPaidVacation = employeeVacationData.Sum(e => e.PaidVacation);
      ViewBag.VacationBalanceCount = totalHaveVacation - totalPaidVacation;

      // Contract Expiry Count: Number of contracts ending in next 30 days (example logic)
      ViewBag.ContractExpiryCount = activeContracts.Count(c => c.EndDays <= 30 && c.EndDays >= 0);

      ViewBag.UserMonitorCount = 0; //await _appDBContext.HR_Deductions.CountAsync();
      ViewBag.ExperienceCertificateCount = await _appDBContext.HR_EmployeeExperiences.CountAsync();
      ViewBag.EducationDocumentCount = await _appDBContext.HR_EmployeeEducations.CountAsync();
      ViewBag.EvaluationCount = 0; //await _appDBContext.HR_OverTimes.CountAsync();
      ViewBag.ProcessApprovalDetailCount = await _appDBContext.CR_ProcessTypeApprovals.CountAsync();
      ViewBag.EmployeeRequestApprovalDetailCount = await _appDBContext.HR_EmployeeRequestTypeApprovals.CountAsync();
      ViewBag.ApprovedMonthlySalarySheetCount = await _appDBContext.HR_MonthlyPayrolls.CountAsync();
      ViewBag.MonthlyPayslipCount = await _appDBContext.HR_MonthlyPayroll_Salarys.CountAsync();
      ViewBag.FaceAttendanceReportCount = await _appDBContext.CR_FaceAttendances.CountAsync();

      return View("~/Views/HR/Main/Dashboard/HR_Report_Dashboard.cshtml");
    }

  }
}
