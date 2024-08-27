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
                                         where con.ActiveID == 1 && emp.ActiveID == 1
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
  }
}
