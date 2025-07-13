using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Hubs;
using Microsoft.AspNetCore.SignalR;
using Exampler_ERP.Utilities;
using Microsoft.Extensions.Localization;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class CardPrintController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IStringLocalizer<CardPrintController> _localizer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CardPrintController> _logger;
    private readonly Utils _utils;
    private readonly IHubContext<NotificationHub> _hubContext;

    public CardPrintController(AppDBContext appDBContext, IConfiguration configuration, ILogger<CardPrintController> logger, Utils utils, IHubContext<NotificationHub> hubContext, IStringLocalizer<CardPrintController> localizer)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
      _hubContext = hubContext;
      _localizer = localizer;

    }

    public async Task<IActionResult> Index(int? id)
    {
      var employeeCards = from emp in _appDBContext.HR_Employees
                          join con in _appDBContext.HR_Contracts
                             on emp.EmployeeID equals con.EmployeeID
                          where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                          select new
                          {
                            emp.EmployeeID,
                            emp.FirstName,
                            emp.FatherName,
                            emp.FamilyName
                          };

      if (id.HasValue)
      {
        employeeCards = employeeCards.Where(e => e.EmployeeID == id.Value);
      }
      var employeeCardPrints = await employeeCards.ToListAsync();

      var employeeCounts = employeeCardPrints.Select(ej => new EmployeeCardPrintViewModel
      {
        EmployeeID = ej.EmployeeID,
        EmployeeName = $"{ej.FirstName} {ej.FatherName} {ej.FamilyName}"
      }).ToList();

      var viewModel = new EmployeeCardPrintListViewModel
      {
        EmployeeCardPrint = employeeCounts
      };

      return View("~/Views/HR/Employeement/CardPrint/CardPrint.cshtml", viewModel);
    }
    public async Task<IActionResult> PrintCard(int employeeId)
    {
      // Fetch the employee details asynchronously, including related entities like Designation and Department
      var employee = await _appDBContext.HR_Employees
          .Include(e => e.DesignationType)
          .Include(e => e.DepartmentType)
          .FirstOrDefaultAsync(e => e.EmployeeID == employeeId);

      // If employee is not found, return a 404 Not Found result
      if (employee == null)
      {
        return NotFound();
      }

      // Return the custom view for printing the employee card, passing the employee model
      return View("~/Views/HR/Employeement/CardPrint/PrintCardPrint.cshtml", employee);
    }


  }
}
