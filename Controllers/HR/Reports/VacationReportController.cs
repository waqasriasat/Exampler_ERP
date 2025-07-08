using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class VacationReportController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
private readonly IHubContext<NotificationHub> _hubContext;

    public VacationReportController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
_hubContext = hubContext;
 
    }
    //public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, int? EmployeeID, int? VacationTypeID)
    //{
    //  var result = await _appDBContext.HR_Vacations
    //    .Where(emp => emp.FinalApprovalID == 1)
    //    .Select(emp => new VacationReportViewModel // Use the view model
    //    {
    //      EmployeeID = emp.EmployeeID,
    //      EmployeeName = emp.Employee.FirstName + " " + emp.Employee.FatherName + " " + emp.Employee.FamilyName,
    //      StartDate = emp.StartDate,
    //      EndDate = emp.EndDate,
    //      TotalDays = emp.TotalDays,
    //      TypeOfVacation = _appDBContext.HR_VacationSettles
    //            .Where(vac => vac.VacationID == emp.VacationID && vac.FinalApprovalID == 1)
    //            .Any() ? "Paid" : "UnPaid"


    //    })
    //    .OrderBy(emp => emp.EmployeeID)
    //    .ToListAsync();

    //  return View("~/Views/HR/Reports/VacationReport/VacationReport.cshtml", result);
    //}
    public async Task<IActionResult> Index(DateTime? FromDate, DateTime? ToDate, string? EmployeeName, int? EmployeeID, int? VacationTypeID)
    {
      var vacationQuery = _appDBContext.HR_Vacations
          .Where(emp => emp.FinalApprovalID == 1);

      if (FromDate.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.StartDate >= FromDate.Value);
      }

      if (ToDate.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.EndDate <= ToDate.Value);
      }

      if (EmployeeID.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.EmployeeID == EmployeeID.Value);
      }

      if (VacationTypeID.HasValue)
      {
        vacationQuery = vacationQuery.Where(emp => emp.VacationTypeID == VacationTypeID.Value);
      }

      var result = await vacationQuery
          .Select(emp => new VacationReportViewModel
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

      ViewBag.FromDate = FromDate;
      ViewBag.ToDate = ToDate;
      ViewBag.VacationTypeID = VacationTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.VacationTypeList = await _utils.GetVacationTypes();

      return View("~/Views/HR/Reports/VacationReport/VacationReport.cshtml", result);
    }

  }
}
