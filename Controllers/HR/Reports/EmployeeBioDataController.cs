using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Exampler_ERP.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Reports
{
  public class EmployeeBioDataController : PositionController
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<EmployeeBioDataController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public EmployeeBioDataController(AppDBContext appDBContext, IConfiguration configuration, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<EmployeeBioDataController> localizer) 
    : base(appDBContext)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }
    public async Task<IActionResult> Index(int? id)
    {
      var employeesQuery = _appDBContext.HR_Employees
       .Where(c => c.DeleteYNID != 1 && c.FinalApprovalID == 1);

      if (id.HasValue)
      {
        employeesQuery = employeesQuery.Where(c => c.EmployeeID == id.Value);
      }

      var employees = await employeesQuery
          .Include(c => c.BranchType)
          .Include(c => c.DepartmentType)
          .ToListAsync();

      return View("~/Views/HR/Reports/EmployeeBioData/EmployeeBioData.cshtml", employees);
    }

  }
}
