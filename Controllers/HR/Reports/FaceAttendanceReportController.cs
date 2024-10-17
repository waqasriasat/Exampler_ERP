using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class FaceAttendanceReportController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;

    public FaceAttendanceReportController(AppDBContext appDBContext, IConfiguration configuration, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
    }
    public async Task<IActionResult> Index(int? MonthsTypeID, int? YearsTypeID, string? EmployeeName, int? EmployeeID)
    {
      var query = _appDBContext.CR_FaceAttendances
          .Include(d => d.Employee)
          .AsQueryable();

      if (MonthsTypeID.HasValue && MonthsTypeID != 0 && YearsTypeID.HasValue && YearsTypeID != 0)
      {
        var startDate = new DateTime(YearsTypeID.Value, MonthsTypeID.Value, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);  // Last day of the selected month

        query = query.Where(d => d.MarkDate >= startDate && d.MarkDate <= endDate);
      }

      if (EmployeeID.HasValue)
      {
        query = query.Where(d => d.EmployeeID == EmployeeID.Value);
      }

      if (!string.IsNullOrEmpty(EmployeeName))
      {
        query = query.Where(d =>
            (d.Employee.FirstName + " " + d.Employee.FatherName + " " + d.Employee.FamilyName)
            .Contains(EmployeeName));
      }


      var faceAttendance = await query.ToListAsync();

      ViewBag.MonthsTypeID = MonthsTypeID;
      ViewBag.YearsTypeID = YearsTypeID;
      ViewBag.EmployeeID = EmployeeID;
      ViewBag.EmployeeName = EmployeeName;

      ViewBag.MonthsTypeList = await _utils.GetMonthsTypes();
  
      return View("~/Views/HR/Reports/FaceAttendanceReport/FaceAttendanceReport.cshtml", faceAttendance);
    }
  }
}
