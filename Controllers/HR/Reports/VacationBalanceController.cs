using Exampler_ERP.Models;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class VacationBalanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public VacationBalanceController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
 
    public async Task<IActionResult> Index(int? id)
    {
      var contractsQuery = _appDBContext.HR_Contracts
          .Where(emp => emp.ActiveYNID == 1);

      if (id.HasValue)
      {
        contractsQuery = contractsQuery.Where(emp => emp.EmployeeID == id.Value);
      }

      var result = await contractsQuery
          .Select(emp => new VacationBalanceViewModel
          {
            EmployeeID = emp.EmployeeID,
            EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
            StartDate = emp.StartDate,
            EndDate = emp.EndDate,
            YearlyVacation = emp.VacationDays,

            TotalVacation = (EF.Functions.DateDiffDay(emp.StartDate, emp.EndDate) / 365.25) * emp.VacationDays,

            VacationBalance = ((EF.Functions.DateDiffDay(emp.StartDate, emp.EndDate) / 365.25) * emp.VacationDays)
                  - (_appDBContext.HR_Vacations
                      .Where(vac => vac.EmployeeID == emp.Employee.EmployeeID && vac.FinalApprovalID == 1)
                      .Sum(vac => (int?)vac.TotalDays) ?? 0)
          })
          .OrderBy(emp => emp.EmployeeID)
          .ToListAsync();

      return View("~/Views/HR/Reports/VacationBalance/VacationBalance.cshtml", result);
    }
  }
}
