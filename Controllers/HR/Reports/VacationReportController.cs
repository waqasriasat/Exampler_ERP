using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class VacationReportController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    public VacationReportController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index()
    {
      var result = await _appDBContext.HR_Vacations
        .Where(emp => emp.FinalApprovalID == 1)
        .Select(emp => new VacationReportViewModel // Use the view model
        {
          EmployeeID = emp.EmployeeID,
          EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
          StartDate = emp.StartDate,
          EndDate = emp.EndDate,
          TotalDays = emp.TotalDays,
          TypeOfVacation = _appDBContext.HR_VacationSettles
                .Where(vac => vac.VacationID == emp.VacationID && vac.FinalApprovalID == 1)
                .Any() ? "Paid" : "UnPaid"


        })
        .OrderBy(emp => emp.EmployeeID)
        .ToListAsync();

      return View("~/Views/HR/Reports/VacationReport/VacationReport.cshtml", result);
    }
  }
}
