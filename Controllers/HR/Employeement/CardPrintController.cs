using Exampler_ERP.Models.Temp;
using Exampler_ERP.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Exampler_ERP.Utilities;

namespace Exampler_ERP.Controllers.HR.Employeement
{
  public class CardPrintController : Controller
  {
    private readonly AppDBContext _appDBContext;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CardPrintController> _logger;
    private readonly Utils _utils;
    public CardPrintController(AppDBContext appDBContext, IConfiguration configuration, ILogger<CardPrintController> logger, Utils utils)
    {
      _appDBContext = appDBContext;
      _configuration = configuration;
      _logger = logger;
      _utils = utils;
    }

    public async Task<IActionResult> Index()
    {
      var employeeCardPrints = await (from emp in _appDBContext.HR_Employees
                                      join con in _appDBContext.HR_Contracts
                                         on emp.EmployeeID equals con.EmployeeID
                                         where con.ActiveYNID == 1 && emp.ActiveYNID == 1
                                         select new
                                         {
                                           emp.EmployeeID,
                                           emp.FirstName,
                                           emp.FatherName,
                                           emp.FamilyName
                                         }).ToListAsync();

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
