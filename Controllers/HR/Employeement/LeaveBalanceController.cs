using Exampler_ERP.Controllers.HR.MasterInfo;
using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class LeaveBalanceController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LeaveBalanceController> _logger;
    private readonly Utils _utils;
    public LeaveBalanceController(AppDBContext appDBContext, IConfiguration configuration, ILogger<LeaveBalanceController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }

    public async Task<IActionResult> Index()
    {
      var employeeLeaveBalances = await (from emp in _appDBContext.HR_Employees
                                         join con in _appDBContext.HR_Contracts
                                         on emp.EmployeeID equals con.EmployeeID
                                         where con.ActiveID == 1 && emp.ActiveID == 1
                                         select new
                                         {
                                           emp.EmployeeID,
                                           emp.FirstName,
                                           emp.FatherName,
                                           emp.FamilyName
                                         }).ToListAsync();

      var employeeCounts = employeeLeaveBalances.Select(ej => new EmployeeLeaveBlanceViewModel
      {
        EmployeeID = ej.EmployeeID,
        EmployeeName = $"{ej.FirstName} {ej.FatherName} {ej.FamilyName}",
        TotalBlanceLeave = 0
      }).ToList();

      var viewModel = new EmployeeLeaveBlanceListViewModel
      {
        EmployeeLeaveBlance = employeeCounts
      };

      return View("~/Views/HR/Employeement/LeaveBalance/LeaveBalance.cshtml", viewModel);
    }

  }
}
